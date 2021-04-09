using System;
using System.Diagnostics;

namespace ZoneRenderer
{
    public interface IBounds<out Tleft, out Ttop, out Tright, out TBottom> { }

    public interface IBounds<out T> : IBounds<T,T,T,T> { }


    [DebuggerDisplay("{Left},{Top},{Right},{Bottom}")]
    public class Bounds<T> : IBounds<T>
    {
        public override bool Equals(object obj)
        {
            if (obj is Bounds<T> other)
            {
                return
                    this.Left.Equals(other.Left) &&
                    this.Top.Equals(other.Top) &&
                    this.Right.Equals(other.Right) &&
                    this.Bottom.Equals(other.Bottom);
            }
            return false;
        }

        public static bool operator ==(Bounds<T> lhs, Bounds<T> rhs)
        {
            return (lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs, null));
        }

        public static bool operator !=(Bounds<T> lhs, Bounds<T> rhs)
        {
            return !(lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs, null));
        }

        public T Left { get; set; }
        public T Top { get; set; }
        public T Right { get; set; }
        public T Bottom { get; set; }

        public override string ToString() => $"{typeof(T).Name} -- Left : {Left} , Top : {Top}, Right : {Right}, Bottom : {Bottom}";
    }

    public class RenderableBounds<T> : Bounds<T> where T: IRenderableBound
    {
        public Bounds<int> RenderBounds(int x, int y, int LayoutWidth, int LayoutHeight)
        {
            return new Bounds<int>
            {
                Top = Top.RenderBound(RectangleSide.Top, y, LayoutHeight),//(int)(LayoutHeight * Top.Decimal) + y,
                Bottom = Bottom.RenderBound(RectangleSide.Bottom, y, LayoutHeight),//(int)(LayoutHeight * Bottom.Decimal) + y,
                Left = Left.RenderBound(RectangleSide.Left, x, LayoutWidth),//(int)(LayoutWidth * Left.Decimal) + x,
                Right = Right.RenderBound(RectangleSide.Right, x, LayoutWidth),//(int)(LayoutWidth * Right.Decimal) + x,

            };
        }
    }
}
