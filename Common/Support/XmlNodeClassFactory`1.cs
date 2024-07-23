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


using Epsitec.Common.Support.PlugIns;
using System.Xml.Linq;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>XmlNodeClassFactory</c> class is used to deserialize classes derived from
    /// <typeparamref name="TClass"/>based on an XML element. The classes must be identified
    /// using a <see cref="XmlNodeClassAttribute"/> at the <c>assembly</c> level.
    /// </summary>
    /// <typeparam name="TClass">The type of the base class of the instances created by this factory.</typeparam>
    internal class XmlNodeClassFactory<TClass>
        : PlugInFactory<TClass, XmlNodeClassAttribute, string>
        where TClass : IXmlNodeClass
    {
        /// <summary>
        /// Restores the class based on the XML element. The class must be declared with an
        /// <see cref="XmlNodeClassAttribute"/> attribute at the <c>assembly</c> level, and
        /// must derive from <typeparamref name="TClass"/>.
        /// </summary>
        /// <param name="xml">The XML element.</param>
        /// <returns>The class instance.</returns>
        public static TClass Restore(XElement xml)
        {
            if (xml == null)
            {
                return default(TClass);
            }

            string xmlNodeName = xml.Name.LocalName;

            return XmlNodeClassFactory<TClass>.CreateInstance(xmlNodeName, xml);
        }
    }
}
