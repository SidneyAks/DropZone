using System;
using System.Collections.Generic;
using System.Linq;
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

        [XmlAttribute]
        public string Screens { get; set; }

        public int[] ScreenIndexes {
            get => Screens.Split(',').Select(x => Int32.Parse(x)).ToArray();
            set => Screens = string.Join(",", value);
        }
     
        public Bounds<T> Target { get; set; }
        public Bounds<T> Trigger { get; set; }
    }
}
