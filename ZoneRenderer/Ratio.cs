using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace ZoneRenderer
{
    [Serializable]
    public class Ratio
    {
        private static Regex ValidationRegex = new Regex(@"^\d+/\d+$");

        [XmlText]
        public string Value
        {
            get
            {
                return denominator == 1 ?
                    $"{numerator}" :
                    $"{numerator}/{denominator}";
            }
            set
            {
                if (ValidationRegex.IsMatch(value))
                {
                    var args = value.Split('/');
                    numerator = Int32.Parse(args[0]);
                    denominator = Int32.Parse(args[1]);
                }
                else if (int.TryParse(value, out var result))
                {
                    numerator = result;
                    denominator = 1;
                }
                else
                {
                    throw new InvalidDataException($"Unable to parse ratio {value}, must be of form \"[integer]/[integer]\"");
                }
                if (denominator == 0)
                {
                    throw new InvalidDataException($"Unable to parse ratio {value}, cannot divide by zero");
                }
            }
        }
        private int numerator;
        private int denominator;

        public static implicit operator Ratio(string str)
        {
            return new Ratio() { Value = str };
        }

        public Decimal Decimal => ((decimal)numerator / (decimal)denominator);
    }
}
