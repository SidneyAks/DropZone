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
        public Layout ActiveLayout => activeLayout ?? (activeLayout = this.First());
        private Layout activeLayout;

        public Layout ActivateNextZone()
        {
            activeLayout = this[(this.IndexOf(ActiveLayout) + 1) % this.Count];
            return activeLayout;
        }

        public void DestroyCache()
        {
            this.ForEach(x => x.DestroyCache());
        }
    }
}
