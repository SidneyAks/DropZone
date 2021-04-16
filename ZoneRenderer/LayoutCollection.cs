using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ZoneRenderer
{
    [Serializable]
    [XmlRoot("Layouts")]
    public class LayoutCollection
    {

        public List<Layout> List { get; set; }

        public Layout ParentLayout { get; set; }

        public RenderedLayout ActiveLayout => activeLayout ?? (activeLayout = List.First().Render(Parent: ParentLayout));
        private RenderedLayout activeLayout;

        public RenderedLayout ActivateNextZone()
        {
            activeLayout = List[(List.IndexOf(ActiveLayout.Base) + 1) % List.Count].Render(Parent: ParentLayout);
            return activeLayout;
        }
    }
}
