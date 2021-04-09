using System;
using System.Collections.Generic;

namespace ZoneRenderer
{
    public interface IRenderableZoneBase<out T> where T : IRenderableBound
    {
        string Name { get; set; }

        LayoutKind Layout { get; set; }

        int[] ScreenIndexes { get; set; }

        IBounds<T> Target { get; }

        IBounds<T> Trigger { get; }

        RenderedZone Render(int x, int y, int LayoutWidth, int LayoutHeight);
    }

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

        IBounds<T> IRenderableZoneBase<T>.Target => RenderableTarget;

        IBounds<T> IRenderableZoneBase<T>.Trigger => RenderableTrigger;
    }
}
