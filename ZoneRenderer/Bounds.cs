using System;
using System.Diagnostics;

namespace ZoneRenderer
{
    public enum RectangleSide
    {
        Left,
        Top,
        Right,
        Bottom
    }

    public interface IRenderableBound
    {
        int RenderBound(RectangleSide Side, int Offset, int Dimension);
    }

    public class Bounds
    {

    }

    [DebuggerDisplay("{Left},{Top},{Right},{Bottom}")]
    public class Bounds<T> : Bounds// : IXmlSerializable
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

        public Bounds renderedFrom { get; set; }

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
