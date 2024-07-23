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
    /// <summary>
    /// La classe String contient un réglage numérique.
    /// </summary>
    [System.Serializable()]
    public class String : Abstract, Support.IXMLSerializable<String>
    {
        public String(Document document, string name)
            : base(document, name)
        {
            this.Initialize();
        }

        protected void Initialize()
        {
            this.conditionName = "";
            this.conditionState = false;

            switch (this.name)
            {
                case "PrintName":
                    this.text = "";
                    break;

                case "PrintFilename":
                    this.text = "";
                    break;
            }
        }

        public string Value
        {
            get
            {
                switch (this.name)
                {
                    case "PrintName":
                        return this.document.Settings.PrintInfo.PrintName;

                    case "PrintFilename":
                        return this.document.Settings.PrintInfo.PrintFilename;
                }

                return "";
            }
            set
            {
                switch (this.name)
                {
                    case "PrintName":
                        this.document.Settings.PrintInfo.PrintName = value;
                        break;

                    case "PrintFilename":
                        this.document.Settings.PrintInfo.PrintFilename = value;
                        break;
                }
            }
        }

        #region Serialization
        public new bool HasEquivalentData(Support.IXMLWritable other)
        {
            String otherString = (String)other;
            return base.HasEquivalentData(other) && this.Value == otherString.Value;
        }

        public override XElement ToXML()
        {
            var root = new XElement("String", base.IterXMLParts());
            if (this.Value != null)
            {
                root.Add(new XAttribute("Value", this.Value));
            }
            return root;
        }

        public static String FromXML(XElement xml)
        {
            return new String(xml);
        }

        private String(XElement xml)
            : base(xml)
        {
            this.Value = xml.Attribute("Value")?.Value;
            this.Initialize();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //	Sérialise le réglage.
            base.GetObjectData(info, context);
            info.AddValue("Value", this.Value);
        }

        protected String(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //	Constructeur qui désérialise le réglage.
            this.Value = info.GetString("Value");
            this.Initialize();
        }
        #endregion
    }
}
