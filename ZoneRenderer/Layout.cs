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
        public List<IRenderableZoneBase<IRenderableBound>> List { get; private set; } = new List<IRenderableZoneBase<IRenderableBound>>();

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
}
