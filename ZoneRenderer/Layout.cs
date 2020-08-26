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

        private IEnumerable<RenderedZone> renderZones()
        {
            var displayinfo = ScreenInfo.GetDisplays();
            /*return LayoutType == LayoutKind.PerScreen ?
                                
                displayinfo.SelectMany(y => List.Select(x => 
                    x.Render(
                        x: y.WorkArea.Left,
                        y: y.WorkArea.Top,
                        LayoutWidth: y.WorkArea.Right - y.WorkArea.Left,
                        LayoutHeight: y.WorkArea.Bottom - y.WorkArea.Top)
                )):
                List.Select(x =>
                    x.Render(
                            x: 0,
                            y: 0,
                            LayoutWidth: displayinfo.MaxWidth,
                            LayoutHeight: displayinfo.MaxHeight
                )).ToList();*/
            return List.SelectMany(zone =>  
                zone.Layout == LayoutKind.PerScreen ? displayinfo.Select(y => zone.Render(
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
                throw new Exception("Unknown layout kind (how did you even do that with an enum?")
            );
        }

        public RenderedZone GetActiveZoneFromPoint(int x, int y)
        {
            return RenderedZones.FirstOrDefault(rect =>
                rect.Trigger.Left < x && x < rect.Trigger.Right &&
                rect.Trigger.Top < y && y < rect.Trigger.Bottom);
        }
    }
}
