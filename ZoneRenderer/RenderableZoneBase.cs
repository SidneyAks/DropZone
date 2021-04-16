using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ZoneRenderer
{
    public interface IRenderableZoneBase<out Tleft, out Ttop, out Tright, out Tbottom>
        where Tleft : IRenderableBound
        where Ttop : IRenderableBound
        where Tright : IRenderableBound
        where Tbottom : IRenderableBound
    {
        string Name { get; set; }

        LayoutKind Layout { get; set; }

        int[] ScreenIndexes { get; set; }

        IBounds<Tleft, Ttop, Tright, Tbottom> Target { get; }

        IBounds<Tleft, Ttop, Tright, Tbottom> Trigger { get; }

        RenderedZone Render(ScreenInfo.DisplayInfoCollection DI, int x, int y, int LayoutWidth, int LayoutHeight);
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

        IBounds<Tleft, Ttop, Tright, Tbottom> IRenderableZoneBase<Tleft, Ttop, Tright, Tbottom>.Target => RenderableTarget;

        IBounds<Tleft, Ttop, Tright, Tbottom> IRenderableZoneBase<Tleft, Ttop, Tright, Tbottom>.Trigger => RenderableTrigger;
    }
}
