/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using System.Collections.Concurrent;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Common.Support
{
    public static class XmlNodeClassFactory
    {
        public static TClass Restore<TClass>(XElement xml)
            where TClass : class, IXmlNodeClass
        {
            var entry = XmlNodeClassFactory.FindEntry(typeof(TClass));
            return entry.Restore(xml) as TClass;
        }

        public static XElement Save<TClass>(TClass instance)
            where TClass : class, IXmlNodeClass
        {
            return instance.Save(XmlNodeClassFactory.GetXmlNodeName(instance.GetType()));
        }

        public static string GetXmlNodeName<TClass>()
            where TClass : class, IXmlNodeClass
        {
            return XmlNodeClassFactory.GetXmlNodeName(typeof(TClass));
        }

        public static string GetXmlNodeName(System.Type type)
        {
            var entry = XmlNodeClassFactory.FindEntry(type);
            return entry.NodeName;
        }

        private static Entry FindEntry(System.Type type)
        {
            Entry entry;

            if (XmlNodeClassFactory.entries.TryGetValue(type, out entry))
            {
                return entry;
            }

            entry = XmlNodeClassFactory.CreateEntry(type);
            XmlNodeClassFactory.entries[type] = entry;

            return entry;
        }

        private static Entry CreateEntry(System.Type type)
        {
            var entryType = typeof(Entry<>).MakeGenericType(XmlNodeClassFactory.FindRootType(type));
            var attribute = TypeEnumerator
                .Instance.GetAllAssemblyLevelAttributes<XmlNodeClassAttribute>()
                .FirstOrDefault(x => x.Type == type);

            if (attribute == null)
            {
                throw new System.ArgumentException(
                    string.Format("Type {0} has no XmlNodeClassAttribute", type.FullName)
                );
            }

            var nodeName = attribute.Id;
            var entry = System.Activator.CreateInstance(entryType, nodeName) as Entry;

            return entry;
        }

        private static System.Type FindRootType(System.Type type)
        {
            var baseType = type.BaseType;

            while (baseType.ContainsInterface<IXmlNodeClass>())
            {
                type = baseType;
                baseType = type.BaseType;
            }

            return type;
        }

        private abstract class Entry
        {
            public Entry(string nodeName)
            {
                this.nodeName = nodeName;
            }

            public abstract IXmlNodeClass Restore(XElement xml);

            public string NodeName
            {
                get { return this.nodeName; }
            }

            private readonly string nodeName;
        }

        private sealed class Entry<TClass> : Entry
            where TClass : class, IXmlNodeClass
        {
            public Entry(string nodeName)
                : base(nodeName) { }

            public override IXmlNodeClass Restore(XElement xml)
            {
                return XmlNodeClassFactory<TClass>.Restore(xml);
            }
        }

        private static readonly ConcurrentDictionary<System.Type, Entry> entries =
            new ConcurrentDictionary<System.Type, Entry>();
    }
}
