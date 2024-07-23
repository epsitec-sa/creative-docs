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

namespace Epsitec.Common.Types.Serialization.IO
{
    public class XmlWriter : AbstractWriter
    {
        public XmlWriter(System.Xml.XmlWriter xml)
        {
            this.xml = xml;
        }

        public override void WriteAttributeStrings()
        {
            if (this.emitAttributes)
            {
                this.xml.WriteAttributeString(
                    "xmlns",
                    Xml.StructurePrefix,
                    null,
                    Xml.StructureNamespace
                );
                this.xml.WriteAttributeString("xmlns", Xml.FieldsPrefix, null, Xml.FieldsNamespace);

                this.emitAttributes = false;
            }
        }

        public override void BeginStorageBundle(
            int id,
            int externalCount,
            int typeCount,
            int objectCount
        )
        {
            this.xml.WriteStartElement(Xml.StructurePrefix, "storage", Xml.StructureNamespace);

            this.xml.WriteAttributeString("root", Xml.StructureNamespace, Context.IdToString(id));
            this.xml.WriteAttributeString(
                "n_ext",
                Xml.StructureNamespace,
                Context.NumToString(externalCount)
            );
            this.xml.WriteAttributeString(
                "n_typ",
                Xml.StructureNamespace,
                Context.NumToString(typeCount)
            );
            this.xml.WriteAttributeString(
                "n_obj",
                Xml.StructureNamespace,
                Context.NumToString(objectCount)
            );

            if (this.emitAttributes)
            {
                //				this.xml.WriteAttributeString ("xmlns", Xml.StructurePrefix, null, this.nsStructure);
                this.xml.WriteAttributeString("xmlns", Xml.FieldsPrefix, null, Xml.FieldsNamespace);

                this.emitAttributes = false;
            }
        }

        public override void EndStorageBundle()
        {
            this.xml.WriteEndElement();
        }

        public override void WriteExternalReference(string name)
        {
            this.xml.WriteStartElement("external", Xml.StructureNamespace);
            this.xml.WriteAttributeString("name", Xml.StructureNamespace, name);
            this.xml.WriteEndElement();
        }

        public override void WriteTypeDefinition(int id, string name)
        {
            this.xml.WriteStartElement("type", Xml.StructureNamespace);
            this.xml.WriteAttributeString("id", Context.IdToString(id));
            this.xml.WriteAttributeString("name", Xml.StructureNamespace, name);
            this.xml.WriteEndElement();
        }

        public override void WriteObjectDefinition(int id, int typeId)
        {
            this.xml.WriteStartElement("object", Xml.StructureNamespace);
            this.xml.WriteAttributeString("id", Context.IdToString(id));
            this.xml.WriteAttributeString(
                "type",
                Xml.StructureNamespace,
                Context.IdToString(typeId)
            );
            this.xml.WriteEndElement();
        }

        public override void BeginObject(int id, DependencyObject obj)
        {
            this.xml.WriteStartElement("data", Xml.StructureNamespace);
            this.xml.WriteAttributeString("id", Context.IdToString(id));
        }

        public override void WriteObjectFieldReference(DependencyObject obj, string name, int id)
        {
            this.xml.WriteAttributeString(
                Xml.FieldsPrefix,
                name,
                Xml.FieldsNamespace,
                Context.IdToString(id)
            );
        }

        public override void WriteObjectFieldReferenceList(
            DependencyObject obj,
            string name,
            IList<int> ids
        )
        {
            this.xml.WriteStartElement(Xml.FieldsPrefix, name, Xml.FieldsNamespace);

            foreach (int id in ids)
            {
                this.xml.WriteStartElement("ref", Xml.StructureNamespace);
                this.xml.WriteAttributeString(
                    "oid",
                    Xml.StructureNamespace,
                    Context.IdToString(id)
                );
                this.xml.WriteEndElement();
            }

            this.xml.WriteEndElement();
        }

        public override void WriteObjectFieldValue(DependencyObject obj, string name, string value)
        {
            this.xml.WriteAttributeString(Xml.FieldsPrefix, name, Xml.FieldsNamespace, value);
        }

        public override void EndObject(int id, DependencyObject obj)
        {
            this.xml.WriteEndElement();
        }

        private System.Xml.XmlWriter xml;
        private bool emitAttributes = true;
    }
}
