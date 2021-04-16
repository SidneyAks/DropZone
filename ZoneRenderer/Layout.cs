using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ZoneRenderer
{
    [Serializable]
    public class Layout
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement]
        public SerializablePolymorphicList<IRenderableZoneBase<IRenderableBound, IRenderableBound, IRenderableBound, IRenderableBound>> List { get; set; } 
            = new SerializablePolymorphicList<IRenderableZoneBase <IRenderableBound, IRenderableBound, IRenderableBound, IRenderableBound>>();

        public override bool Equals(object obj)
        {
            if (obj is Layout other)
            {
                return this.Name == other.Name && this.List.Select(x => x.Name).SequenceEqual(other.List.Select(x => x.Name));
            }
            return false;
        }

        public static bool operator ==(Layout lhs, Layout rhs)
        {
            return (lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs, null));
        }

        public static bool operator !=(Layout lhs, Layout rhs)
        {
            return !(lhs?.Equals(rhs) ?? Object.ReferenceEquals(rhs, null));
        }

        public RenderedLayout Render(ScreenInfo.DisplayInfoCollection di = null, Layout Parent = null) => new RenderedLayout(this, di, Parent);

        public override int GetHashCode()
        {
            int hashCode = -1387617897;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
//            hashCode = hashCode * -1521134295 + EqualityComparer<List<Zone>>.Default.GetHashCode(List);
            return hashCode;
        }
    }

    public class SerializablePolymorphicList<T> : List<T>, IXmlSerializable
    {
        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            while (reader.NodeType == XmlNodeType.Element)
            {
                var item = (GenericTypeWrapper)(new XmlSerializer(typeof(GenericTypeWrapper)).Deserialize(reader));
                Add((T)item.Obj);
            }
            reader.ReadEndElement();
            int i = 0;
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var item in this)
            {
                var Container = new GenericTypeWrapper(item);

                new XmlSerializer(Container.GetType()).Serialize(writer, Container);
                //                new XmlSerializer(item.GetType()).Serialize(writer, item);
            }
        }

        public class GenericTypeWrapper : IXmlSerializable
        {
            public GenericTypeWrapper() { }

            public GenericTypeWrapper(object obj)
            {
                TypeDescripter = new GenericTypeDescripter(obj);
                Obj = obj;
            }

            public GenericTypeDescripter TypeDescripter { get; set; }
            public object Obj;

            public XmlSchema GetSchema() => null;

            public void ReadXml(XmlReader reader)
            {
                reader.ReadStartElement();
                var GTD = new XmlSerializer(typeof(GenericTypeDescripter)).Deserialize(reader) as GenericTypeDescripter;
                Obj = new XmlSerializer(GTD.TypeDef).Deserialize(reader);
                reader.ReadEndElement();

            }

            public void WriteXml(XmlWriter writer)
            {
                new XmlSerializer(TypeDescripter.GetType()).Serialize(writer, TypeDescripter);
                new XmlSerializer(Obj.GetType()).Serialize(writer, Obj);
            }
        }

        public class GenericTypeDescripter
        {
            public GenericTypeDescripter() { }

            public GenericTypeDescripter(object obj)
            {
                TypeDef = obj.GetType();
            }

            public GenericTypeDescripter(params string[] TypeNames)
            {
                GenericTypeDefString = TypeNames[0];
                GenericArgStrings = TypeNames.Skip(1).ToArray();
            }

            public string GenericTypeDefString
            {
                get => GenericTypeDef.FullName;
                set
                {
                    GenericTypeDef = Type.GetType(value);
                }
            }

            public string[] GenericArgStrings
            {
                get => GenericTypes?.Select(x => x.AssemblyQualifiedName).ToArray();
                set
                {
                    GenericTypes = value.Select(x => Type.GetType(x)).ToList();
                }
            }

            [XmlIgnore]
            Type GenericTypeDef { get; set; }

            [XmlIgnore]
            List<Type> GenericTypes { get; set; }

            [XmlIgnore]
            public Type TypeDef
            {
                get
                {
                    return GenericTypeDef.IsGenericType ? GenericTypeDef.MakeGenericType(GenericTypes.ToArray()) : GenericTypeDef;
                }
                set
                {
                        if (value.IsGenericType)
                        {
                            GenericTypeDef = value.GetGenericTypeDefinition();
                            GenericTypes = value.GetGenericArguments().ToList();
                        }
                        else
                        {
                            GenericTypeDef = value;
                        }
                }
            }
        }
    }

}
