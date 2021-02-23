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

        [XmlIgnore]
        public List<RenderedZone> RenderedZones => rZones ?? (rZones = renderZones().ToList());
        private List<RenderedZone> rZones;

        public void DestroyCache()
        {
            rZones = null;
        }

        private IEnumerable<RenderedZone> renderZones()
        {
            var displayinfo = ScreenInfo.GetDisplays();

            //I realize I could write this without using a disgusting amalgamation of ternary
            //and linq, but honestly, I kind of think it's beautiful in it's own horrible way.
            var foo =  List.SelectMany(zone =>  
                zone.Layout == LayoutKind.Duplicated ? displayinfo.Select(y => zone.Render(
                        x: y.WorkArea.Left,
                        y: y.WorkArea.Top,
                        LayoutWidth: y.WorkArea.Right - y.WorkArea.Left,
                        LayoutHeight: y.WorkArea.Bottom - y.WorkArea.Top
                    )):
                zone.Layout == LayoutKind.Spanning ? new RenderedZone[] {zone.Render(
                        x: 0,
                        y: 0,
                        LayoutWidth: displayinfo.MaxWidth,
                        LayoutHeight: displayinfo.MaxHeight
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
            );

            Console.WriteLine($"Getting {foo.Count()} zones from {Name}");

            return foo;
        }

        public RenderedZone GetActiveZoneFromPoint(int x, int y)
        {
            return RenderedZones.FirstOrDefault(rect =>
                rect.Trigger.Left < x && x < rect.Trigger.Right &&
                rect.Trigger.Top < y && y < rect.Trigger.Bottom);
        }
    }
}
