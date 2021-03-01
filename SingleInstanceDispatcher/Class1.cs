using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SingleInstance
{
    public static class Events
    {
        public static event EventHandler<SingleInstanceLogEventArgs> LogDebug;
        public static event EventHandler<SingleInstanceLogEventArgs> LogInfo;
        public static event EventHandler<SingleInstanceLogEventArgs> LogWarning;
        public static event EventHandler<SingleInstanceLogEventArgs> LogError;

        internal static void WriteLogDebug(string LogLine) => LogDebug?.Invoke(null, new SingleInstanceLogEventArgs(LogLine));
        internal static void WriteLogInfo(string LogLine) => LogInfo?.Invoke(null, new SingleInstanceLogEventArgs(LogLine));
        internal static void WriteLogWarning(string LogLine) => LogWarning?.Invoke(null, new SingleInstanceLogEventArgs(LogLine));
        internal static void WriteLogError(string LogLine) => LogError?.Invoke(null, new SingleInstanceLogEventArgs(LogLine));
    }

    public class SingleInstanceLogEventArgs : EventArgs
    {
        public readonly string LogLine;
        public SingleInstanceLogEventArgs(string LogText)
        {
            LogLine = LogText;
        }
    }

    public class DipatchRegistrationAttribute : Attribute
    {
        public string DispatchLabel;
        public string DispatchLookup => DispatchLabel.Replace("ForceSingleInstance ", "");

        public string appIDSuff { get; set; }

        public DipatchRegistrationAttribute(string Label, bool ForceOwnInstance = false, string AppIDSuffix = null)
        {
            DispatchLabel = Label;

            if (ForceOwnInstance)
            {
                DispatchLabel = "ForceSingleInstance " + DispatchLabel;
                appIDSuff = AppIDSuffix;
            }
        }
    }

    public static class Dispatcher
    {
        #region DispatchingFunctionality
        private static PipeStream CommandPipe;

        public static void ClosePipe()
        {
            CommandPipe?.Close();
        }

        public static bool PerUserInstanceAllowed { get; set; } = true;

        internal static string AppID
        {
            get
            {
                return _appID;
            }
            set
            {
                _appID = value;
                SetCurrentProcessExplicitAppUserModelID(value);
            }
        }
        internal static string _appID;

        [DllImport("shell32.dll", SetLastError = true)]
        static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

        /// <summary>
        /// Itialization of the dispatch listener. Stands up command pipe of open, otherwise sends messages to open command pipe.
        /// </summary>
        /// <param name="CMDPipeName">The name of the command pipe to stand up. Must be unique per application.</param>
        /// <param name="AppID">The app ID of the application utilized. Of the form COMPANYNAME.PRODUCTNAME.SUBPRODUCT.VERSIONINFO Subproduct is optional</param>
        /// <param name="args">The args aray from main</param>
        public static void Intialize(string CMDPipeName, string ApplicationID, params string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += (s, e) => { ClosePipe(); };

            AppID = ApplicationID;
            if (args.FirstOrDefault() == "ForceSingleInstance")
            {
                Dispatch(args.Skip(1).ToArray());
                return;
            }
            CMDPipeName = PerUserInstanceAllowed ? CMDPipeName + Environment.UserName : CMDPipeName;
            //Create a named pipe for if multiple instances of the application open
            try
            {
                PipeSecurity ps = new PipeSecurity();
                NTAccount f = new NTAccount(Environment.UserName);
                SecurityIdentifier sid = (SecurityIdentifier)f.Translate(typeof(SecurityIdentifier));


                PipeAccessRule par = new PipeAccessRule(sid, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow);
                ps.AddAccessRule(par);

                CommandPipe = new NamedPipeServerStream(CMDPipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 65535, 65535, ps);
                Task dispatcher = new Task(() =>
                {
                    while (true)
                    {
                        if (!CommandPipe.IsConnected)
                        {
                            try
                            {
                                (CommandPipe as NamedPipeServerStream).WaitForConnection();
                            }
                            catch (System.IO.IOException ex) when (ex.Message.Contains("The pipe has been ended"))
                            {
                                Events.WriteLogDebug("Pipe disconnected: failed to get connection");
                            }
                            Events.WriteLogDebug("Pipe Connected");
                        }
                        using (var SR = new StreamReader(CommandPipe, Encoding.Default, true, 65535, true))
                        {
                            using (var sw = new StreamWriter(CommandPipe, Encoding.Default, 65535, true))
                            {
                                try
                                {
                                    sw.WriteLine(Dispatch(SR.ReadLine().Split('/')));
                                }
                                catch (Exception DispatcherException)
                                {
                                    sw.WriteLine("EXCEPTION");
                                    sw.WriteLine(new Error(DispatcherException).XmlSerialize());

                                }
                            }
                            (CommandPipe as NamedPipeServerStream)?.Disconnect();
                            Events.WriteLogDebug("Pipe Disconnected");
                        }
                    }
                });
                dispatcher.Start();
                if (args.Length > 0)
                {
                    Dispatch(args);
                }

            }
            //If a command pipe is already opened, we implicitely know we need to send any args
            //to the new application. If there are any args, send them, if not we can
            //assume the user just wanted to open the exe, so we bring it to the front, additionally
            //we should kill the current process
            catch (Exception ex)
            {
                if (!(ex is UnauthorizedAccessException) && !(ex is IOException))
                {
                    throw;
                }
                SendArgToPipeLine(CMDPipeName, args);
                throw new CantStartSecondInstanceException("Application Instance Already running", ex);

            }

        }

        public static void SendArgToPipeLine(string CMDPipeName, params string[] args)
        {
            try
            {
                CMDPipeName = PerUserInstanceAllowed ? CMDPipeName + Environment.UserName : CMDPipeName;
                CommandPipe = new NamedPipeClientStream(CMDPipeName);
                (CommandPipe as NamedPipeClientStream).Connect(1500);
                using (var sw = new StreamWriter(CommandPipe, Encoding.Default, 65535, true))
                {
                    sw.WriteLine(args.Length > 0
                            ? String.Join("/", args)
                            : "NullArgs"
                    );
                }
                using (var SR = new StreamReader(CommandPipe, Encoding.Default, true, 65535, true))
                {
                    while (CommandPipe.IsConnected)
                    {
                        System.Console.Write(SR.ReadLine());
                    }
                }
            }
            catch (Exception ex2)
            {
                throw new Exception("Primary application found, but cannot connect to it via pipe. Is another application pending communication?");
            }
        }

        public static readonly Lazy<Dictionary<DipatchRegistrationAttribute, MethodInfo>> ActionLookup = new Lazy<Dictionary<DipatchRegistrationAttribute, MethodInfo>>(() =>
        {
            var assm = AppDomain.CurrentDomain.GetAssemblies();
            var types = assm.SelectMany(x => x.GetTypes());
            var methods = types.SelectMany(x => x.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static));
            var decoratedMethods = methods.Where(x => x.GetCustomAttributes(typeof(DipatchRegistrationAttribute), false).Length > 0);

            return decoratedMethods.ToDictionary(k => k.GetCustomAttributes(typeof(DipatchRegistrationAttribute), false).Cast<DipatchRegistrationAttribute>().First(), v => v);
        }
        );

        public static string Dispatch(params string[] args)
        {
            if (args.Length > 0 && ActionLookup.Value.Keys.Select(x => x.DispatchLookup).Contains(args.First()))
            {
                var Action = ActionLookup.Value.First(x => x.Key.DispatchLookup == args.First()).Value;

                var attr = Action.GetCustomAttribute<DipatchRegistrationAttribute>();

                if (attr.appIDSuff != null)
                {
                    AppID = AppID + "." + attr.appIDSuff;
                }

                return Action.Invoke(null,
                    args.Length == 1 ? null :
                    args.Skip(1).ToArray()
                )?.ToString() ?? "OK!";
            }
            return $"No registered action with name {args.First()}";
        }

        public static void LaunchNewInstace(Action mi, params string[] args)
        {

            var attr = mi.GetMethodInfo().GetCustomAttribute<DipatchRegistrationAttribute>();
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = System.Reflection.Assembly.GetEntryAssembly().Location,
                    UseShellExecute = true,
                    Arguments = attr.DispatchLabel
                }
            );
        }
        #endregion
    }

    public class CantStartSecondInstanceException : Exception
    {
        public CantStartSecondInstanceException(string message, Exception ex = null) :
            base(message, ex)
        {

        }
    }

    [Serializable]
    public class Error
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public Error InnerError { get; set; }

        public Error()
        {

        }

        public Error(System.Exception ex)
        {
            this.Message = ex.Message;
            this.StackTrace = ex.StackTrace;
            if (ex.InnerException != null)
            {
                this.InnerError = new Error(ex.InnerException);
            }
        }
    }

    public static class extensions
    {
        public static T XmlDeserialize<T>(string toDeserialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader textReader = new StringReader(toDeserialize))
            {
                return (T)xmlSerializer.Deserialize(textReader);
            }
        }

        public static string XmlSerialize<T>(this T obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, obj);
                return textWriter.ToString().Replace(Environment.NewLine, "");
            }
        }
    }
}
