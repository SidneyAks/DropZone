using System;

namespace ZoneRenderer
{
    public class Bounds
    {

    }

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
}
