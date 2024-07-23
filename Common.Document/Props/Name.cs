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

using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Epsitec.Common.Document.Properties
{
    /// <summary>
    /// La classe Name représente une propriété d'un objet graphique.
    /// </summary>
    [System.Serializable()]
    public class Name : Abstract, Support.IXMLSerializable<Name>
    {
        public Name(Document document, Type type)
            : base(document, type) { }

        protected override void Initialize()
        {
            base.Initialize();
            this.stringValue = "";
        }

        public string String
        {
            get { return this.stringValue; }
            set
            {
                value = value.Trim(); // enlève les espaces superflus avant et après

                if (this.stringValue != value)
                {
                    this.NotifyBefore();
                    this.stringValue = value;
                    if (value != "")
                    {
                        this.document.Modifier.NamesExist = true;
                    }
                    this.NotifyAfter();
                }
            }
        }

        public override void CopyTo(Abstract property)
        {
            //	Effectue une copie de la propriété.
            base.CopyTo(property);
            Name p = property as Name;
            p.stringValue = this.stringValue;
        }

        public override bool Compare(Abstract property)
        {
            //	Compare deux propriétés.
            if (!base.Compare(property))
                return false;

            Name p = property as Name;
            if (p.stringValue != this.stringValue)
                return false;

            return true;
        }

        public override Panels.Abstract CreatePanel(Document document)
        {
            //	Crée le panneau permettant d'éditer la propriété.
            Panels.Abstract.StaticDocument = document;
            return new Panels.Name(document);
        }

        #region Serialization
        public new bool HasEquivalentData(Support.IXMLWritable other)
        {
            Name otherName = (Name)other;
            return base.HasEquivalentData(other) && this.stringValue == otherName.stringValue;
        }

        public override XElement ToXML()
        {
            return new XElement(
                "Name",
                base.IterXMLParts(),
                new XAttribute("StringValue", this.stringValue)
            );
            ;
        }

        public static Name FromXML(XElement xml)
        {
            return new Name(xml);
        }

        private Name(XElement xml)
            : base(xml)
        {
            this.stringValue = xml.Attribute("StringValue").Value;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //	Sérialise la propriété.
            base.GetObjectData(info, context);

            info.AddValue("StringValue", this.stringValue);
        }

        protected Name(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //	Constructeur qui désérialise la propriété.
            this.stringValue = info.GetString("StringValue");
        }
        #endregion


        protected string stringValue;
    }
}
