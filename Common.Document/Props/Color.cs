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
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Properties
{
    /// <summary>
    /// La classe Color représente une propriété d'un objet graphique.
    /// </summary>
    [System.Serializable()]
    public class Color : Abstract, Support.IXMLSerializable<Color>
    {
        public Color(Document document, Type type)
            : base(document, type) { }

        protected override void Initialize()
        {
            base.Initialize();
            this.color = Drawing.RichColor.FromBrightness(0.0);
        }

        public Drawing.RichColor ColorValue
        {
            //	Couleur de la propriété.
            get { return this.color; }
            set
            {
                if (this.color != value)
                {
                    this.NotifyBefore();
                    this.color = value;
                    this.NotifyAfter();
                }
            }
        }

        public override bool IsComplexPrinting
        {
            //	Indique si une impression complexe est nécessaire.
            get
            {
                if (this.color.A > 0.0 && this.color.A < 1.0)
                    return true;
                return false;
            }
        }

        public override void CopyTo(Abstract property)
        {
            //	Effectue une copie de la propriété.
            base.CopyTo(property);
            Color p = property as Color;
            p.color = this.color;
        }

        public override bool Compare(Abstract property)
        {
            //	Compare deux propriétés.
            if (!base.Compare(property))
                return false;

            Color p = property as Color;
            if (p.color != this.color)
                return false;

            return true;
        }

        public override Panels.Abstract CreatePanel(Document document)
        {
            //	Crée le panneau permettant d'éditer la propriété.
            Panels.Abstract.StaticDocument = document;
            return new Panels.Color(document);
        }

        public override bool ChangeColorSpace(ColorSpace cs)
        {
            //	Modifie l'espace des couleurs.
            this.NotifyBefore();
            this.color.ColorSpace = cs;
            this.NotifyAfter();
            this.document.Notifier.NotifyPropertyChanged(this);
            return true;
        }

        public override bool ChangeColor(double adjust, bool stroke)
        {
            //	Modifie les couleurs.
            this.NotifyBefore();
            this.color.ChangeBrightness(adjust);
            this.NotifyAfter();
            this.document.Notifier.NotifyPropertyChanged(this);
            return true;
        }

        #region Serialization
        public new bool HasEquivalentData(Support.IXMLWritable other)
        {
            Color otherColor = (Color)other;
            return base.HasEquivalentData(other) && this.color == otherColor.color;
        }

        public override XElement ToXML()
        {
            return new XElement(
                "Color",
                base.IterXMLParts(),
                new XElement("Color", this.color.ToXML())
            );
        }

        public static Color FromXML(XElement xml)
        {
            return new Color(xml);
        }

        private Color(XElement xml)
            : base(xml)
        {
            this.color = RichColor.FromXML(xml.Element("Color").Element("RichColor"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //	Sérialise la propriété.
            base.GetObjectData(info, context);

            info.AddValue("Color", this.color);
        }

        protected Color(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //	Constructeur qui désérialise la propriété.
            if (this.document.IsRevisionGreaterOrEqual(1, 0, 22))
            {
                this.color = (Drawing.RichColor)info.GetValue("Color", typeof(Drawing.RichColor));
            }
            else
            {
                Drawing.Color c = (Drawing.Color)info.GetValue("Color", typeof(Drawing.Color));
                this.color = new RichColor(c);
            }
        }
        #endregion


        protected Drawing.RichColor color;
    }
}
