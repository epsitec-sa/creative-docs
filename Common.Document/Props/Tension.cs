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
    /// La classe Tension représente une propriété d'un objet graphique.
    /// </summary>
    [System.Serializable()]
    public class Tension : Abstract, Support.IXMLSerializable<Tension>
    {
        public Tension(Document document, Type type)
            : base(document, type) { }

        protected override void Initialize()
        {
            base.Initialize();
            this.tensionValue = 0.6; // tension de 60% par défaut
        }

        public double TensionValue
        {
            get { return this.tensionValue; }
            set
            {
                if (this.tensionValue != value)
                {
                    this.NotifyBefore();
                    this.tensionValue = value;
                    this.NotifyAfter();
                }
            }
        }

        public override string SampleText
        {
            //	Donne le petit texte pour les échantillons.
            get
            {
                return string.Concat(
                    Res.Strings.Property.Tension.Short.Value,
                    this.tensionValue.ToString()
                );
            }
        }

        public override void PutStyleBrief(System.Text.StringBuilder builder)
        {
            //	Construit le texte résumé d'un style pour une propriété.
            this.PutStyleBriefPrefix(builder);
            builder.Append(this.SampleText);
            this.PutStyleBriefPostfix(builder);
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
            Tension p = property as Tension;
            p.tensionValue = this.tensionValue;
        }

        public override bool Compare(Abstract property)
        {
            //	Compare deux propriétés.
            if (!base.Compare(property))
                return false;

            Tension p = property as Tension;
            if (p.tensionValue != this.tensionValue)
                return false;

            return true;
        }

        public override Panels.Abstract CreatePanel(Document document)
        {
            //	Crée le panneau permettant d'éditer la propriété.
            Panels.Abstract.StaticDocument = document;
            return new Panels.Tension(document);
        }

        #region Serialization
        public new bool HasEquivalentData(Support.IXMLWritable other)
        {
            Tension otherTension = (Tension)other;
            return base.HasEquivalentData(other) && this.tensionValue == otherTension.tensionValue;
        }

        public override XElement ToXML()
        {
            return new XElement(
                "Tension",
                base.IterXMLParts(),
                new XAttribute("TensionValue", this.tensionValue)
            );
        }

        public static Tension FromXML(XElement xml)
        {
            return new Tension(xml);
        }

        private Tension(XElement xml)
            : base(xml)
        {
            this.tensionValue = (double)xml.Attribute("TensionValue");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //	Sérialise la propriété.
            base.GetObjectData(info, context);

            info.AddValue("TensionValue", this.tensionValue);
        }

        protected Tension(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //	Constructeur qui désérialise la propriété.
            this.tensionValue = info.GetDouble("TensionValue");
        }
        #endregion


        protected double tensionValue;
    }
}
