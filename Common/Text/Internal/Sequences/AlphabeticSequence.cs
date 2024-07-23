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
    /// La classe Alphabetic produit des lettres a, b, c ...
    /// </summary>
    public class Alphabetic : Generator.Sequence, Common.Support.IXMLSerializable<Alphabetic>
    {
        public Alphabetic()
            : this("abcdefghijklmnopqrstuvwxyz") { }

        public Alphabetic(string alphabet)
        {
            this.alphabet = alphabet;
        }

        public override Generator.SequenceType WellKnownType
        {
            get { return Generator.SequenceType.Alphabetic; }
        }

        public override bool ParseText(string text, out int value)
        {
            if ((text == null) || (text.Length == 0))
            {
                value = 0;
                return false;
            }

            value = this.alphabet.ToLower().IndexOf(text.ToLower()[0]) + 1;

            return (value > 0);
        }

        protected override string GetRawText(int rank, System.Globalization.CultureInfo culture)
        {
            if ((rank < 1) || (rank > this.alphabet.Length))
            {
                return "?";
            }

            return string.Format(culture, "{0}", this.alphabet[rank - 1]);
        }

        protected override string GetSetupArgument()
        {
            return this.alphabet;
        }

        protected override void Setup(string argument)
        {
            this.alphabet = argument;
        }

        public override XElement ToXML()
        {
            return new XElement(
                "Alphabetic",
                base.IterXMLParts(),
                new XAttribute("Alphabet", this.alphabet)
            );
        }

        public static Alphabetic FromXML(XElement xml)
        {
            return new Alphabetic(xml);
        }

        private Alphabetic(XElement xml)
            : base(xml)
        {
            this.alphabet = xml.Attribute("Alphabet").Value;
        }

        private string alphabet;
    }
}
