using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace ZoneRenderer
{

    public abstract class ZoneBase<Tleft, Ttop, Tright, Tbottom>
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
            if (obj is ZoneBase<Tleft, Ttop, Tright, Tbottom> other)
            {
                return this.Name == other.Name &&
                    this.Screens == other.Screens &&
                    this.Target == other.Target &&
                    this.Trigger == other.Trigger;
            }
            return false;
        }

        public static bool operator ==(ZoneBase<Tleft, Ttop, Tright, Tbottom> lhs, ZoneBase<Tleft, Ttop, Tright, Tbottom> rhs)
        {
            return (lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs, null));
        }

        public static bool operator !=(ZoneBase<Tleft, Ttop, Tright, Tbottom> lhs, ZoneBase<Tleft, Ttop, Tright, Tbottom> rhs)
        {
            return !(lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs, null));
        }

        public Bounds<Tleft, Ttop, Tright, Tbottom> Target { get; set; }
        public Bounds<Tleft, Ttop, Tright, Tbottom> Trigger { get; set; }
    }

}
