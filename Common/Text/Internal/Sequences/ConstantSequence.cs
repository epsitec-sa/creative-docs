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


using System.Xml.Linq;

namespace Epsitec.Common.Text.Internal.Sequences
{
    /// <summary>
    /// La classe Constant produit des séquences de puces prédéfinies. Chaque
    /// niveau possède sa puce.
    /// </summary>
    public class Constant : Generator.Sequence, Common.Support.IXMLSerializable<Constant>
    {
        public Constant()
            : this("\u25CF") { }

        public Constant(string constant)
        {
            this.constant = constant;
        }

        public override Generator.SequenceType WellKnownType
        {
            get { return Generator.SequenceType.Constant; }
        }

        public override bool ParseText(string text, out int value)
        {
            if ((text == null) || (text.Length == 0))
            {
                value = 0;
                return false;
            }

            value = this.constant.IndexOf(text[0]) + 1;

            return (value > 0);
        }

        protected override string GetRawText(int rank, System.Globalization.CultureInfo culture)
        {
            rank = System.Math.Min(rank, this.constant.Length);

            return this.constant.Substring(rank - 1, 1);
        }

        protected override string GetSetupArgument()
        {
            return this.constant;
        }

        protected override void Setup(string argument)
        {
            this.constant = argument;
        }

        public override XElement ToXML()
        {
            return new XElement(
                "Constant",
                base.IterXMLParts(),
                new XAttribute("Constant", this.constant)
            );
        }

        public static Constant FromXML(XElement xml)
        {
            return new Constant(xml);
        }

        private Constant(XElement xml)
            : base(xml)
        {
            this.constant = xml.Attribute("Constant").Value;
        }

        private string constant;
    }
}
