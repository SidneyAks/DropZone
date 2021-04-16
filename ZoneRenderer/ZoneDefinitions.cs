using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace ZoneRenderer
{
    public interface IZoneBase<out Tleft, out Ttop, out Tright, out Tbottom>
    {
        string Name { get; set; }
        LayoutKind Layout { get; set; }
        int[] ScreenIndexes { get; set; }
        IBounds<Tleft, Ttop, Tright, Tbottom> Trigger { get; }
        IBounds<Tleft, Ttop, Tright, Tbottom> Target { get; }
    }

    public interface IRenderableZoneBase<out Tleft, out Ttop, out Tright, out Tbottom> : 
        IZoneBase<Tleft, Ttop, Tright, Tbottom>
        where Tleft : IRenderableBound
        where Ttop : IRenderableBound
        where Tright : IRenderableBound
        where Tbottom : IRenderableBound
    {
        #region Duplicitous data, needed in order to properly display in DataGrid. I'm unhappy about it.
        string Name { get; set; }
        LayoutKind Layout { get; set; }
        int[] ScreenIndexes { get; set; }
        IBounds<Tleft, Ttop, Tright, Tbottom> Trigger { get; }
        IBounds<Tleft, Ttop, Tright, Tbottom> Target { get; }
        #endregion
        RenderedZone Render(ScreenInfo.DisplayInfoCollection DI, int x, int y, int LayoutWidth, int LayoutHeight);
    }

    public abstract class ZoneBase<Tleft, Ttop, Tright, Tbottom> : IZoneBase<Tleft, Ttop, Tright, Tbottom>
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

        public IBounds<Tleft, Ttop, Tright, Tbottom> Target { get; set; }
        public IBounds<Tleft, Ttop, Tright, Tbottom> Trigger { get; set; }
    }

    public class RenderedZone : ZoneBase<int, int, int, int>
    {
        public IRenderableZoneBase<IRenderableBound, IRenderableBound, IRenderableBound, IRenderableBound> Zone { get; set; }

        public new IBounds<int, int, int, int> Trigger
        {
            get => base.Trigger ?? base.Target; set => base.Trigger = value;
        }

        public int TargetWidth => targetWidth ?? (targetWidth = Target.Right - Target.Left).Value;
        private int? targetWidth;

        public int TriggerWidth => triggerWidth ?? (triggerWidth = Trigger.Right - Trigger.Left).Value;
        private int? triggerWidth;

        public int TargetHeight => targetHeight ?? (targetHeight = Target.Bottom - Target.Top).Value;
        private int? targetHeight;

        public int TriggerHeight => triggerHeight ?? (triggerHeight = Trigger.Bottom - Trigger.Top).Value;
        private int? triggerHeight;
    }

    [Serializable]
    public class RenderableZoneBase<Tleft, Ttop, Tright, Tbottom> : 
        ZoneBase<Tleft, Ttop, Tright, Tbottom>, 
        IRenderableZoneBase<Tleft, Ttop, Tright, Tbottom>
        where Tleft : IRenderableBound
        where Ttop : IRenderableBound
        where Tright : IRenderableBound
        where Tbottom : IRenderableBound
    {
        public RenderedZone Render(ScreenInfo.DisplayInfoCollection DI, int x, int y, int LayoutWidth, int LayoutHeight)
        {
            return new RenderedZone()
            {
                Name = this.Name,
                Zone = (IRenderableZoneBase<IRenderableBound, IRenderableBound, IRenderableBound, IRenderableBound>)this,
                Target = this.RenderableTarget.RenderBounds(DI, x, y, LayoutWidth, LayoutHeight),
                Trigger = this.RenderableTrigger?.RenderBounds(DI, x, y, LayoutWidth, LayoutHeight)
            };
        }

        public RenderableBounds<Tleft, Ttop, Tright, Tbottom> RenderableTarget {
            get => new RenderableBounds<Tleft, Ttop, Tright, Tbottom>()
            {
                Left = Target.Left,
                Top = Target.Top,
                Right = Target.Right,
                Bottom = Target.Bottom,
            };
        }
        public RenderableBounds<Tleft, Ttop, Tright, Tbottom> RenderableTrigger
        {
            get => new RenderableBounds<Tleft, Ttop, Tright, Tbottom> ()
            {
                Left = Trigger.Left,
                Top = Trigger.Top,
                Right = Trigger.Right,
                Bottom = Trigger.Bottom,
            };
        }

//        IBounds<Tleft, Ttop, Tright, Tbottom> IRenderableZoneBase<Tleft, Ttop, Tright, Tbottom>.Target => RenderableTarget;

//        IBounds<Tleft, Ttop, Tright, Tbottom> IRenderableZoneBase<Tleft, Ttop, Tright, Tbottom>.Trigger => RenderableTrigger;
    }

}
