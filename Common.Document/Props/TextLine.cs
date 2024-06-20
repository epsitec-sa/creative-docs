using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Epsitec.Common.Document.Properties
{
    /// <summary>
    /// La classe TextLine représente une propriété d'un objet graphique.
    /// </summary>
    [System.Serializable()]
    public class TextLine : Abstract, Support.IXMLSerializable<TextLine>
    {
        public TextLine(Document document, Type type)
            : base(document, type) { }

        protected override void Initialize()
        {
            base.Initialize();
            this.horizontal = JustifHorizontal.Left;
            this.offset = 0.0;
            this.add = 0.0;
        }

        public JustifHorizontal Horizontal
        {
            get { return this.horizontal; }
            set
            {
                if (this.horizontal != value)
                {
                    this.NotifyBefore();
                    this.horizontal = value;
                    this.NotifyAfter();
                }
            }
        }

        public double Offset
        {
            get { return this.offset; }
            set
            {
                if (this.offset != value)
                {
                    this.NotifyBefore();
                    this.offset = value;
                    this.NotifyAfter();
                }
            }
        }

        public double Add
        {
            get { return this.add; }
            set
            {
                if (this.add != value)
                {
                    this.NotifyBefore();
                    this.add = value;
                    this.NotifyAfter();
                }
            }
        }

        public override string SampleText
        {
            //	Donne le petit texte pour les échantillons.
            get
            {
                if (this.horizontal == JustifHorizontal.Left)
                    return "|ab |";
                if (this.horizontal == JustifHorizontal.Center)
                    return "| ab |";
                if (this.horizontal == JustifHorizontal.Right)
                    return "| ab|";
                if (this.horizontal == JustifHorizontal.Justif)
                    return "|ab|";
                if (this.horizontal == JustifHorizontal.All)
                    return "|ab.|";
                if (this.horizontal == JustifHorizontal.Stretch)
                    return "<ab>";
                return "|ab|";
            }
        }

        public override bool AlterBoundingBox
        {
            //	Indique si un changement de cette propriété modifie la bbox de l'objet.
            get { return true; }
        }

        public override void CopyTo(Abstract property)
        {
            //	Effectue une copie de la propriété.
            base.CopyTo(property);
            TextLine p = property as TextLine;
            p.horizontal = this.horizontal;
            p.offset = this.offset;
            p.add = this.add;
        }

        public override bool Compare(Abstract property)
        {
            //	Compare deux propriétés.
            if (!base.Compare(property))
                return false;

            TextLine p = property as TextLine;
            if (p.horizontal != this.horizontal)
                return false;
            if (p.offset != this.offset)
                return false;
            if (p.add != this.add)
                return false;

            return true;
        }

        public override Panels.Abstract CreatePanel(Document document)
        {
            //	Crée le panneau permettant d'éditer la propriété.
            Panels.Abstract.StaticDocument = document;
            return new Panels.TextLine(document);
        }

        #region Serialization
        public new bool HasEquivalentData(Support.IXMLWritable other)
        {
            TextLine otherTextLine = (TextLine)other;
            return base.HasEquivalentData(other)
                && this.horizontal == otherTextLine.horizontal
                && this.offset == otherTextLine.offset
                && this.add == otherTextLine.add;
        }

        public override XElement ToXML()
        {
            return new XElement(
                "TextLine",
                base.IterXMLParts(),
                new XAttribute("Horizontal", this.horizontal),
                new XAttribute("Offset", this.offset),
                new XAttribute("Add", this.add)
            );
        }

        public static TextLine FromXML(XElement xml)
        {
            return new TextLine(xml);
        }

        private TextLine(XElement xml)
            : base(xml)
        {
            JustifHorizontal.TryParse(xml.Attribute("Horizontal").Value, out this.horizontal);
            this.offset = double.Parse(xml.Attribute("Offset").Value);
            this.add = double.Parse(xml.Attribute("Add").Value);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //	Sérialise la propriété.
            base.GetObjectData(info, context);

            info.AddValue("Horizontal", this.horizontal);
            info.AddValue("Offset", this.offset);
            info.AddValue("Add", this.add);
        }

        protected TextLine(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //	Constructeur qui désérialise la propriété.
            this.horizontal = (JustifHorizontal)
                info.GetValue("Horizontal", typeof(JustifHorizontal));
            this.offset = info.GetDouble("Offset");
            this.add = info.GetDouble("Add");
        }
        #endregion


        protected JustifHorizontal horizontal;
        protected double offset;
        protected double add;
    }
}
