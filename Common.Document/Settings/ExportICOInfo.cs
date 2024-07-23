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

namespace Epsitec.Common.Document.Settings
{
    public enum ICOFormat
    {
        XP,
        Vista,
        Paginated,
    }

    /// <summary>
    /// La classe ExportICOInfo contient tous les réglages pour l'exportation d'une icône.
    /// </summary>
    [System.Serializable()]
    public class ExportICOInfo : ISerializable, Support.IXMLSerializable<ExportICOInfo>
    {
        public ExportICOInfo(Document document)
        {
            this.document = document;
            this.Initialize();
        }

        protected void Initialize()
        {
            this.format = ICOFormat.Paginated;
        }

        public ICOFormat Format
        {
            get { return this.format; }
            set { this.format = value; }
        }

        #region Serialization
        public bool HasEquivalentData(Support.IXMLWritable other)
        {
            ExportICOInfo otherExportICOInfo = (ExportICOInfo)other;
            return this.format == otherExportICOInfo.format;
        }

        public XElement ToXML()
        {
            return new XElement("ExportICOInfo", new XAttribute("Format", this.format));
        }

        public static ExportICOInfo FromXML(XElement xml)
        {
            return new ExportICOInfo(xml);
        }

        private ExportICOInfo(XElement xml)
        {
            this.document = Document.ReadDocument;
            this.Initialize();
            ICOFormat.TryParse(xml.Attribute("Format").Value, out this.format);
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //	Sérialise les réglages.
            info.AddValue("Rev", 0);
            info.AddValue("ICOFormat", this.format);
        }

        protected ExportICOInfo(SerializationInfo info, StreamingContext context)
        {
            //	Constructeur qui désérialise les réglages.
            this.document = Document.ReadDocument;
            this.Initialize();

            int rev = info.GetInt32("Rev");
            this.format = (ICOFormat)info.GetValue("ICOFormat", typeof(ICOFormat));
        }
        #endregion


        protected Document document;
        protected ICOFormat format;
    }
}
