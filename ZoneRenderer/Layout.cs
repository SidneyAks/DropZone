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

        public override bool Equals(object obj)
        {
            if (obj is Layout other)
            {
                return this.Name == other.Name && this.List.Select(x => x.Name).SequenceEqual(other.List.Select(x => x.Name));
            }
            return false;
        }

        public static bool operator ==(Layout lhs, Layout rhs)
        {
            return (lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs, null));
        }

        public static bool operator !=(Layout lhs, Layout rhs)
        {
            return !(lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs, null));
        }

        public RenderedLayout Render(ScreenInfo.DisplayInfoCollection di = null, Layout Parent = null) => new RenderedLayout(this, di, Parent);

        public override int GetHashCode()
        {
            int hashCode = -1387617897;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
//            hashCode = hashCode * -1521134295 + EqualityComparer<List<Zone>>.Default.GetHashCode(List);
            return hashCode;
        }
    }

    public class RenderedLayout
    {
        public readonly Layout Base;
        public readonly List<RenderedZone> Zones;


        public RenderedLayout(Layout layoutBase, ScreenInfo.DisplayInfoCollection displayinfo = null, Layout Parent = null)
        {
            Base = layoutBase;

            var parentZones = Parent?.Render(displayinfo).Zones;

            Zones = parentZones?.Union(RenderZones(layoutBase, displayinfo)).ToList() ?? RenderZones(layoutBase, displayinfo);
        }

        private List<RenderedZone> RenderZones (Layout layoutBase, ScreenInfo.DisplayInfoCollection displayinfo = null)
        {
            displayinfo = displayinfo ?? ScreenInfo.GetDisplays();
            return layoutBase.List.SelectMany(zone =>
                //If layout is duplicated, create layout once per each monitor using said monitors bounds
                zone.Layout == LayoutKind.Duplicated ? displayinfo.Select(y => zone.Render(
                        x: y.WorkArea.Left,
                        y: y.WorkArea.Top,
                        LayoutWidth: y.WorkArea.Right - y.WorkArea.Left,
                        LayoutHeight: y.WorkArea.Bottom - y.WorkArea.Top
                    )) :
                //If layout is spanning, create layout once using full work area bounds
                zone.Layout == LayoutKind.Spanning ? new RenderedZone[] {zone.Render(
                        x: displayinfo.Min(y => y.WorkArea.Left),
                        y: displayinfo.Min(y => y.WorkArea.Top),
                        LayoutWidth: displayinfo.MaxWidth - displayinfo.Min(y => y.WorkArea.Left),
                        LayoutHeight: displayinfo.MaxHeight - displayinfo.Min(y => y.WorkArea.Top)
                    )} :
                //If layout is selected screans, create layout once per selected monitor, using said monitors bounds
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
