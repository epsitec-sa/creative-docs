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

namespace Epsitec.Common.Text.Properties
{
    /// <summary>
    /// La classe LinkProperty permet de représenter l'information liée à un
    /// lien hypertexte (mais pas l'aspect graphique qui est géré par un
    /// soulignement via UnderlineProperty).
    /// </summary>
    public class LinkProperty : Property, Common.Support.IXMLSerializable<LinkProperty>
    {
        public LinkProperty() { }

        public LinkProperty(string link)
        {
            this.link = link;
        }

        public override WellKnownType WellKnownType
        {
            get { return WellKnownType.Link; }
        }

        public override PropertyType PropertyType
        {
            get { return PropertyType.ExtraSetting; }
        }

        public override CombinationMode CombinationMode
        {
            get { return CombinationMode.Accumulate; }
        }

        public string Link
        {
            get { return this.link; }
        }

        public static System.Collections.IComparer Comparer
        {
            get { return new LinkComparer(); }
        }

        public override Property EmptyClone()
        {
            return new LinkProperty();
        }

        public override void SerializeToText(System.Text.StringBuilder buffer)
        {
            SerializerSupport.Join(
                buffer,
                /**/SerializerSupport.SerializeString(this.link)
            );
        }

        public override bool HasEquivalentData(Common.Support.IXMLWritable otherWritable)
        {
            LinkProperty other = (LinkProperty)otherWritable;
            return this.link == other.link;
        }

        public override XElement ToXML()
        {
            return new XElement("LinkProperty", new XAttribute("Link", this.link));
        }

        public static LinkProperty FromXML(XElement xml)
        {
            return new LinkProperty(xml);
        }

        private LinkProperty(XElement xml)
        {
            this.link = xml.Attribute("Link").Value;
        }

        public override void DeserializeFromText(
            TextContext context,
            string text,
            int pos,
            int length
        )
        {
            string[] args = SerializerSupport.Split(text, pos, length);

            System.Diagnostics.Debug.Assert(args.Length == 1);

            string link = SerializerSupport.DeserializeString(args[0]);

            this.link = link;
        }

        public override Property GetCombination(Property property)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateContentsSignature(IO.IChecksum checksum)
        {
            checksum.UpdateValue(this.link);
        }

        public override bool CompareEqualContents(object value)
        {
            return LinkProperty.CompareEqualContents(this, value as LinkProperty);
        }

        private static bool CompareEqualContents(LinkProperty a, LinkProperty b)
        {
            return a.link == b.link;
        }

        #region LinkComparer Class
        private class LinkComparer : System.Collections.IComparer
        {
            #region IComparer Members
            public int Compare(object x, object y)
            {
                Properties.LinkProperty px = x as Properties.LinkProperty;
                Properties.LinkProperty py = y as Properties.LinkProperty;

                return string.Compare(px.link, py.link);
            }
            #endregion
        }
        #endregion


        private string link;
    }
}
