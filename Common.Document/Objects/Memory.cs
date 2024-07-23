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

namespace Epsitec.Common.Document.Objects
{
    /// <summary>
    /// La classe Memory est un objet caché qui collectionne toutes les propriétés.
    /// </summary>
    [System.Serializable()]
    public class Memory : Objects.Abstract, Support.IXMLSerializable<Memory>
    {
        public Memory(Document document, Objects.Abstract model)
            : base(document, model)
        {
            System.Diagnostics.Debug.Assert(model == null);
            if (this.document == null)
                return; // objet factice ?
            this.CreateProperties(model, false);
        }

        protected override bool ExistingProperty(Properties.Type type)
        {
            if (type == Properties.Type.None)
                return false;
            if (type == Properties.Type.Shadow)
                return false;
            if (type == Properties.Type.TextJustif)
                return false;
            if (type == Properties.Type.TextLine)
                return false;
            return true;
        }

        protected override Objects.Abstract CreateNewObject(
            Document document,
            Objects.Abstract model
        )
        {
            return new Memory(document, model);
        }

        #region Serialization
        public override XElement ToXML()
        {
            return new XElement("Memory", this.IterXMLParts());
        }

        public static Memory FromXML(XElement xml)
        {
            return new Memory(xml);
        }

        private Memory(XElement xml)
            : base(xml) { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //	Sérialise l'objet.
            base.GetObjectData(info, context);
        }

        protected Memory(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //	Constructeur qui désérialise l'objet.
        }
        #endregion
    }
}
