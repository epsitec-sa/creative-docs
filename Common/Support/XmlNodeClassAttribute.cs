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

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>XmlNodeClassAttribute</c> attribute is used by the <see cref="XmlNodeClassFactory"/>
    /// to associate an XML node name with a class, which should implement a static <c>Restore</c>
    /// method for deserialization based on XML.
    /// </summary>
    [System.AttributeUsage(
        System.AttributeTargets.Assembly,
        /* */AllowMultiple = true
    )]
    public sealed class XmlNodeClassAttribute : System.Attribute, IPlugInAttribute<string>
    {
        public XmlNodeClassAttribute(string id, System.Type type)
        {
            this.id = id;
            this.type = type;
        }

        public XmlNodeClassAttribute(System.Type type)
            : this("-", type) { }

        #region IPlugInAttribute<string> Members

        public string Id
        {
            get { return this.id; }
        }

        public System.Type Type
        {
            get { return this.type; }
        }

        #endregion


        private readonly string id;
        private readonly System.Type type;
    }
}
