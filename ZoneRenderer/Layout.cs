using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace ZoneRenderer
{
    [Serializable]
    public class Layout
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlArray("Zones")]
        [XmlArrayItem("DropZone")]
        public List<Zone> List { get; private set; } = new List<Zone>();

        public RenderedLayout Render(ScreenInfo.DisplayInfoCollection di = null) => new RenderedLayout(this, di);
    }

    public class RenderedLayout
    {
        public readonly Layout Base;
        public readonly List<RenderedZone> Zones;


        public RenderedLayout(Layout layoutBase, ScreenInfo.DisplayInfoCollection displayinfo = null)
        {
            Base = layoutBase;

            displayinfo = displayinfo ?? ScreenInfo.GetDisplays();
            Zones = Base.List.SelectMany(zone =>
                zone.Layout == LayoutKind.Duplicated ? displayinfo.Select(y => zone.Render(
                        x: y.WorkArea.Left,
                        y: y.WorkArea.Top,
                        LayoutWidth: y.WorkArea.Right - y.WorkArea.Left,
                        LayoutHeight: y.WorkArea.Bottom - y.WorkArea.Top
                    )) :
                zone.Layout == LayoutKind.Spanning ? new RenderedZone[] {zone.Render(
                        x: displayinfo.Min(y => y.WorkArea.Left),
                        y: displayinfo.Min(y => y.WorkArea.Top),
                        LayoutWidth: displayinfo.MaxWidth - displayinfo.Min(y => y.WorkArea.Left),
                        LayoutHeight: displayinfo.MaxHeight - displayinfo.Min(y => y.WorkArea.Top)
                    )} :
                zone.Layout == LayoutKind.SelectedScreens ? zone.ScreenIndexes
                                                                    .Where(x => x < displayinfo.Count())
                                                                    .Select(x => displayinfo[x])
                                                                    .Select(y => zone.Render(
                        x: y.WorkArea.Left,
                        y: y.WorkArea.Top,
                        LayoutWidth: y.WorkArea.Right - y.WorkArea.Left,
                        LayoutHeight: y.WorkArea.Bottom - y.WorkArea.Top
                    )) :
                throw new Exception("Unknown layout kind (how did you even do that with an enum?")
            ).ToList();
        }

        public RenderedZone GetActiveZoneFromPoint(int x, int y)
        {
            return Zones.FirstOrDefault(rect =>
                rect.Trigger.Left < x && x < rect.Trigger.Right &&
                rect.Trigger.Top < y && y < rect.Trigger.Bottom);
        }
    }
}
