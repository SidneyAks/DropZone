using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.Remoting.Messaging;
using System.Xml.Schema;

namespace ZoneRenderer
{
    public abstract class IZoneRenderer
    {
        public abstract void UpdateZones(Layout Layout);

        public abstract void RenderZone(int ScreenWidth, int ScreenHeight);

        public abstract void HideZone();

        public abstract void ActivateSector(RenderedZone Zone);

        public static Color ActiveZoneColor = Color.FromArgb(195, 255, 255, 255);
        public static Color BackgroundColor = Color.FromArgb(255, 1, 1, 1);
        public static Color LabelColor = Color.FromArgb(255, 255, 125, 125);
        public static byte BackgroundOpacity = 128;
        public static Font LabelFont = new Font(FontFamily.GenericMonospace, 24.0F, FontStyle.Bold, GraphicsUnit.Pixel);
    }

    public enum LayoutKind
    {
        Duplicated,
        Spanning,
        SelectedScreens
    }
}
