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

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Epsitec.Common.Document.Settings
{
    /// <summary>
    /// La classe Abstract représente un réglage.
    /// </summary>
    [System.Serializable()]
    public abstract class Abstract : ISerializable, Support.IXMLWritable
    {
        public Abstract(Document document, string name)
        {
            this.document = document;
            this.name = name;
            this.conditionName = "";
            this.conditionState = false;
        }

        public string Name
        {
            //	Nom logique.
            get { return this.name; }
        }

        public string Text
        {
            //	Texte explicatif.
            get { return this.text; }
        }

        public string ConditionName
        {
            //	Nom de la condition.
            get { return this.conditionName; }
        }

        public bool ConditionState
        {
            //	Etat de la condition.
            get { return this.conditionState; }
        }

        #region Serialization
        public bool HasEquivalentData(Support.IXMLWritable other)
        {
            Abstract otherAbstract = (Abstract)other;
            return this.name == otherAbstract.name;
        }

        public abstract XElement ToXML();

        public IEnumerable<XObject> IterXMLParts()
        {
            yield return new XAttribute("Name", this.name);
        }

        protected Abstract(XElement xml)
        {
            this.document = Document.ReadDocument;
            this.name = xml.Attribute("Name").Value;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //	Sérialise le réglage.
            info.AddValue("Name", this.name);
        }

        protected Abstract(SerializationInfo info, StreamingContext context)
        {
            //	Constructeur qui désérialise le réglage.
            this.document = Document.ReadDocument;
            this.name = info.GetString("Name");
        }
        #endregion


        protected Document document;
        protected string name;
        protected string text;
        protected string conditionName;
        protected bool conditionState;
    }
}
