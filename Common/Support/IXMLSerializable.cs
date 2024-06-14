using System.Xml.Linq;

namespace Epsitec.Common.Support
{
    public interface IXMLWritable
    {
        public XElement ToXML();

        public bool HasEquivalentData(IXMLWritable other);
    }

    public interface IXMLSerializable<T> : IXMLWritable
    {
        public static abstract T FromXML(XElement xml);
    }
}
