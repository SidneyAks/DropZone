using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ZoneRenderer
{
    [Serializable]
    [XmlRoot("Layouts")]
    public class LayoutCollection : List<Layout>
    {
        public RenderedLayout ActiveLayout => activeLayout ?? (activeLayout = this.First().Render());
        private RenderedLayout activeLayout;

        public RenderedLayout ActivateNextZone()
        {
            activeLayout = this[(this.IndexOf(ActiveLayout.Base) + 1) % this.Count].Render();
            return activeLayout;
        }
    }
}
