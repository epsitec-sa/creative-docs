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
            return new XElement("String", base.IterXMLParts(), new XAttribute("Value", this.Value));
        }

        public static String FromXML(XElement xml)
        {
            return new String(xml);
        }

        private String(XElement xml)
            : base(xml)
        {
            this.Value = xml.Attribute("Value").Value;
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
