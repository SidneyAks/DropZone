using System;

namespace ZoneRenderer
{
    [Serializable]
    public class Zone : ZoneBase<Ratio>
    {
        public RenderedZone Render(int x, int y, int LayoutWidth, int LayoutHeight)
        {
            return new RenderedZone()
            {
                Name = this.Name,
                Target = new Bounds<int>
                {
                    Top = (int)(LayoutHeight * Target.Top.Decimal) + y,
                    Bottom = (int)(LayoutHeight * Target.Bottom.Decimal) + y,
                    Left = (int)(LayoutWidth * Target.Left.Decimal) + x,
                    Right = (int)(LayoutWidth * Target.Right.Decimal) + x,

                },
                Trigger = this.Trigger == null ? null : new Bounds<int>
                {
                    Top = (int)(LayoutHeight * Trigger.Top.Decimal) + y,
                    Bottom = (int)(LayoutHeight * Trigger.Bottom.Decimal) + y,
                    Left = (int)(LayoutWidth * Trigger.Left.Decimal) + x,
                    Right = (int)(LayoutWidth * Trigger.Right.Decimal) + x

                }
            };



        }
    }
}
