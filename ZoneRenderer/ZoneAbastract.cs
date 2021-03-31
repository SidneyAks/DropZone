using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace ZoneRenderer
{
    [Serializable]
    public abstract class ZoneBase<T>
    {
        public Bounds<T> Target { get; set; }
        public Bounds<T> Trigger { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public LayoutKind Layout { get; set; }

        [XmlAttribute]
        public string Screens { get; set; }

        [Browsable(false)]
        public int[] ScreenIndexes {
            get => Screens?.Split(',').Select(x => Int32.Parse(x)).ToArray() ?? new List<int>().ToArray();
            set => Screens = string.Join(",", value);
        }

        public override bool Equals(object obj)
        {
            if (obj is TemplatableZoneBase<T> templatable)
            {
                return templatable.Equals(this);
            }
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
            return (lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs,null));
        }

        public static bool operator !=(ZoneBase<T> lhs, ZoneBase<T> rhs)
        {
            return !(lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs, null)); 
        }
    }

    public interface TemplatableZoneBase<Ttemplate>
    {

    }

    public abstract class TemplatedZoneBase<T, Ttemplate> : ZoneBase<T>, TemplatableZoneBase<Ttemplate>
    {
        public ZoneBase<Ttemplate> Template { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ZoneBase<Ttemplate> TemplateOther)
            {
                return this.Name == TemplateOther.Name &&
                    this.Screens == TemplateOther.Screens &&
                    this.Template.Target == TemplateOther.Target &&
                    this.Template.Trigger == TemplateOther.Trigger;
            } else if (obj is ZoneBase<T> SameTypeOther)
            {
                return this.Name == SameTypeOther.Name &&
                    this.Screens == SameTypeOther.Screens &&
                    this.Target == SameTypeOther.Target &&
                    this.Trigger == SameTypeOther.Trigger;
            }
            return false;
        }
    }
}
