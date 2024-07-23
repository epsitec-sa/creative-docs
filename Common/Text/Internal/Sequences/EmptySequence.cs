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
    /// La classe Empty produit des séquences vides.
    /// </summary>
    public class Empty : Generator.Sequence, Common.Support.IXMLSerializable<Empty>
    {
        public Empty() { }

        public override Generator.SequenceType WellKnownType
        {
            get { return Generator.SequenceType.Empty; }
        }

        public override bool ParseText(string text, out int value)
        {
            value = 1;
            return true;
        }

        protected override string GetRawText(int rank, System.Globalization.CultureInfo culture)
        {
            return "";
        }

        public override XElement ToXML()
        {
            return new XElement("Empty", base.IterXMLParts());
        }

        public static Empty FromXML(XElement xml)
        {
            return new Empty(xml);
        }

        private Empty(XElement xml)
            : base(xml) { }
    }
}
