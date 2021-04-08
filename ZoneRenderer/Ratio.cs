using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace ZoneRenderer
{
    [Serializable]
    [DebuggerDisplay("{numerator}/{denominator}")]
    public class Ratio : IRenderableBound
    {
        private static Regex ValidationRegex = new Regex(@"^\d+/\d+$");

        [XmlText]
        public string Value
        {
            get
            {
                return denominator == 1 ?
                    $"{numerator}" :
                    $"{numerator}/{denominator}";
            }
            set
            {
                if (ValidationRegex.IsMatch(value))
                {
                    var args = value.Split('/');
                    numerator = Int32.Parse(args[0]);
                    denominator = Int32.Parse(args[1]);
                }
                else if (int.TryParse(value, out var result))
                {
                    numerator = result;
                    denominator = 1;
                }
                else
                {
                    throw new InvalidDataException($"Unable to parse ratio {value}, must be of form \"[integer]/[integer]\"");
                }
                if (denominator == 0)
                {
                    throw new InvalidDataException($"Unable to parse ratio {value}, cannot divide by zero");
                }
            }
        }
        private int numerator;
        private int denominator;

        public static implicit operator Ratio(string str)
        {
            return new Ratio() { Value = str };
        }

        public Decimal Decimal => ((decimal)numerator / (decimal)denominator);

        public override bool Equals(object obj)
        {
            if (obj is Ratio other)
            {
                return
                    this.numerator.Equals(other.numerator) &&
                    this.denominator.Equals(other.denominator);
            }
            return false;
        }

        public int RenderBound(RectangleSide Side, int Offset, int Dimension)
        {
            return (int)(Dimension * Decimal) + Offset;
        }

        public static bool operator ==(Ratio lhs, Ratio rhs)
        {
            return (lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs, null));
        }

        public static bool operator !=(Ratio lhs, Ratio rhs)
        {
            return !(lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs, null));
        }
    }
}
