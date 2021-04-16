using System;
using System.Diagnostics;

namespace ZoneRenderer
{
    public interface IBounds<out Tleft, out Ttop, out Tright, out Tbottom> { }

    public interface IBounds<out T> : IBounds<T,T,T,T> { }


    [DebuggerDisplay("{Left},{Top},{Right},{Bottom}")]
    public class Bounds<Tleft, Ttop, Tright, Tbottom> : IBounds<Tleft, Ttop, Tright, Tbottom>
    {
        public override bool Equals(object obj)
        {
            if (obj is Bounds<Tleft, Ttop, Tright, Tbottom> other)
            {
                return
                    this.Left.Equals(other.Left) &&
                    this.Top.Equals(other.Top) &&
                    this.Right.Equals(other.Right) &&
                    this.Bottom.Equals(other.Bottom);
            }
            return false;
        }

        public static bool operator ==(Bounds<Tleft, Ttop, Tright, Tbottom> lhs, Bounds<Tleft, Ttop, Tright, Tbottom> rhs)
        {
            return (lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs, null));
        }

        public static bool operator !=(Bounds<Tleft, Ttop, Tright, Tbottom> lhs, Bounds<Tleft, Ttop, Tright, Tbottom> rhs)
        {
            return !(lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs, null));
        }

        public Tleft Left { get; set; }
        public Ttop Top { get; set; }
        public Tright Right { get; set; }
        public Tbottom Bottom { get; set; }

        public override string ToString() => $"Left : {Left} , Top : {Top}, Right : {Right}, Bottom : {Bottom}";
    }

    public class RenderableBounds<Tleft, Ttop, Tright, Tbottom> : Bounds<Tleft, Ttop, Tright, Tbottom>
        where Tleft : IRenderableBound
        where Ttop : IRenderableBound
        where Tright : IRenderableBound
        where Tbottom : IRenderableBound
    {
        public Bounds<int, int, int, int> RenderBounds(ScreenInfo.DisplayInfoCollection DI, int x, int y, int LayoutWidth, int LayoutHeight)
        {
            return new Bounds<int, int, int, int>
            {
                Top = Top.RenderBound(DI, RectangleSide.Top, y, LayoutHeight),//(int)(LayoutHeight * Top.Decimal) + y,
                Bottom = Bottom.RenderBound(DI, RectangleSide.Bottom, y, LayoutHeight),//(int)(LayoutHeight * Bottom.Decimal) + y,
                Left = Left.RenderBound(DI, RectangleSide.Left, x, LayoutWidth),//(int)(LayoutWidth * Left.Decimal) + x,
                Right = Right.RenderBound(DI, RectangleSide.Right, x, LayoutWidth),//(int)(LayoutWidth * Right.Decimal) + x,

            };
        }
    }
}
