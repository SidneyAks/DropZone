using System;
using System.Xml.Serialization;

namespace ZoneRenderer
{
    [Serializable]
    public abstract class ZoneBase<T>
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public LayoutKind Layout { get; set; }
     
        public Bounds<T> Target { get; set; }
        public Bounds<T> Trigger { get; set; }
    }
}
