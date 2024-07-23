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

//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
    /// <summary>
    /// La classe TabProperty décrit une tabulation.
    /// </summary>
    public class TabProperty : Property, Common.Support.IXMLSerializable<TabProperty>
    {
        public TabProperty() { }

        public TabProperty(string tag)
        {
            this.tabTag = tag;
        }

        public override WellKnownType WellKnownType
        {
            get { return WellKnownType.Tab; }
        }

        public override PropertyType PropertyType
        {
            get { return PropertyType.LocalSetting; }
        }

        public override PropertyAffinity PropertyAffinity
        {
            get { return PropertyAffinity.Symbol; }
        }

        public override CombinationMode CombinationMode
        {
            get { return CombinationMode.Invalid; }
        }

        public string TabTag
        {
            get { return this.tabTag; }
        }

        public override Property EmptyClone()
        {
            return new TabProperty();
        }

        public override void SerializeToText(System.Text.StringBuilder buffer)
        {
            SerializerSupport.Join(
                buffer,
                /**/SerializerSupport.SerializeString(this.tabTag)
            );
        }

        public override bool HasEquivalentData(Common.Support.IXMLWritable otherWritable)
        {
            TabProperty other = (TabProperty)otherWritable;
            return this.tabTag == other.tabTag;
        }

        public override XElement ToXML()
        {
            return new XElement("TabProperty", new XAttribute("TabTag", this.tabTag));
        }

        public static TabProperty FromXML(XElement xml)
        {
            return new TabProperty(xml);
        }

        private TabProperty(XElement xml)
        {
            this.tabTag = xml.Attribute("TabTag").Value;
        }

        public override void DeserializeFromText(
            TextContext context,
            string text,
            int pos,
            int length
        )
        {
            string[] args = SerializerSupport.Split(text, pos, length);

            Debug.Assert.IsTrue(args.Length == 1);

            string tabTag = SerializerSupport.DeserializeString(args[0]);

            this.tabTag = tabTag;
        }

        public override Property GetCombination(Property property)
        {
            throw new System.InvalidOperationException();
        }

        public override void UpdateContentsSignature(IO.IChecksum checksum)
        {
            checksum.UpdateValue(this.tabTag);
        }

        public override bool CompareEqualContents(object value)
        {
            return TabProperty.CompareEqualContents(this, value as TabProperty);
        }

        private static bool CompareEqualContents(TabProperty a, TabProperty b)
        {
            return a.tabTag == b.tabTag;
        }

        private string tabTag;
    }
}
