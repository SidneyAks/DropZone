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

    public interface IRenderableZoneBase<out T> { }

    [Serializable]
    public class RenderableZoneBase<T> : ZoneBase<T>, IRenderableZoneBase<T> where T : IRenderableBound
    {
        public RenderedZone Render(int x, int y, int LayoutWidth, int LayoutHeight)
        {
            IRenderableZoneBase<IRenderableBound> foo = new RenderableZoneBase<Ratio>();
            return new RenderedZone()
            {
                Name = this.Name,
                Zone = (IRenderableZoneBase<IRenderableBound>)this,
                Target = this.RenderableTarget.RenderBounds(x, y, LayoutWidth, LayoutHeight),
                Trigger = this.RenderableTrigger?.RenderBounds(x, y, LayoutWidth, LayoutHeight)
            };
        }

        public RenderableBounds<T> RenderableTarget {
            get => new RenderableBounds<T>()
            {
                Left = Target.Left,
                Top = Target.Top,
                Right = Target.Right,
                Bottom = Target.Bottom,
            };
            //            set; 
        }
        public RenderableBounds<T> RenderableTrigger
        {
            get => new RenderableBounds<T>()
            {
                Left = Trigger.Left,
                Top = Trigger.Top,
                Right = Trigger.Right,
                Bottom = Trigger.Bottom,
            };
            //            set; 
        }
    }
}
