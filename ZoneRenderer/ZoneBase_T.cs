using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace ZoneRenderer
{

    public abstract class ZoneBase<T>
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public LayoutKind Layout { get; set; }

        [XmlAttribute]
        public string Screens { get; set; }

        [Browsable(false)]
        public int[] ScreenIndexes
        {
            get => Screens?.Split(',').Select(x => Int32.Parse(x)).ToArray() ?? new List<int>().ToArray();
            set => Screens = string.Join(",", value);
        }

        public override bool Equals(object obj)
        {
            if (obj is ZoneBase<T> other)
            {
                return this.Name == other.Name &&
                    this.Screens == other.Screens &&
                    this.Target == other.Target &&
                    this.Trigger == other.Trigger;
            }
            return false;
        }

        public static bool operator ==(ZoneBase<T> lhs, ZoneBase<T> rhs)
        {
            return (lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs, null));
        }

        public static bool operator !=(ZoneBase<T> lhs, ZoneBase<T> rhs)
        {
            return !(lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs, null));
        }

        public Bounds<T> Target { get; set; }
        public Bounds<T> Trigger { get; set; }
    }

}
