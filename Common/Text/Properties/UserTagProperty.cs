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
    /// La classe UserTagProperty permet de stocker des informations supplémentaires,
    /// ainsi que des commentaires, à des fragments de texte.
    /// </summary>
    public class UserTagProperty : Property, Common.Support.IXMLSerializable<UserTagProperty>
    {
        public UserTagProperty() { }

        public UserTagProperty(string tagType, string tagData, long id)
        {
            this.tagType = tagType;
            this.tagData = tagData;
            this.id = id;
        }

        public override WellKnownType WellKnownType
        {
            get { return WellKnownType.UserTag; }
        }

        public override PropertyType PropertyType
        {
            get { return PropertyType.ExtraSetting; }
        }

        public override CombinationMode CombinationMode
        {
            get { return CombinationMode.Accumulate; }
        }

        public string TagType
        {
            get { return this.tagType; }
        }

        public string TagData
        {
            get { return this.tagData; }
        }

        public long Id
        {
            get { return this.id; }
        }

        public static System.Collections.IComparer Comparer
        {
            get { return new UserTagComparer(); }
        }

        public override Property EmptyClone()
        {
            return new UserTagProperty();
        }

        public override void SerializeToText(System.Text.StringBuilder buffer)
        {
            SerializerSupport.Join(
                buffer,
                /**/SerializerSupport.SerializeString(this.tagType),
                /**/SerializerSupport.SerializeString(this.tagData),
                /**/SerializerSupport.SerializeLong(this.id)
            );
        }

        public override bool HasEquivalentData(Common.Support.IXMLWritable otherWritable)
        {
            UserTagProperty other = (UserTagProperty)otherWritable;
            return this.tagType == other.tagType
                && this.tagData == other.tagData
                && this.id == other.id;
        }

        public override XElement ToXML()
        {
            return new XElement(
                "UserTagProperty",
                new XAttribute("TagType", this.tagType),
                new XAttribute("TagData", this.tagData),
                new XAttribute("Id", this.id)
            );
        }

        public static UserTagProperty FromXML(XElement xml)
        {
            return new UserTagProperty(xml);
        }

        private UserTagProperty(XElement xml)
        {
            this.tagType = xml.Attribute("TagType").Value;
            this.tagData = xml.Attribute("TagData").Value;
            this.id = (long)xml.Attribute("Id");
        }

        public override void DeserializeFromText(
            TextContext context,
            string text,
            int pos,
            int length
        )
        {
            string[] args = SerializerSupport.Split(text, pos, length);

            System.Diagnostics.Debug.Assert(args.Length == 3);

            string tagType = SerializerSupport.DeserializeString(args[0]);
            string tagData = SerializerSupport.DeserializeString(args[1]);
            long id = SerializerSupport.DeserializeLong(args[2]);

            this.tagType = tagType;
            this.tagData = tagData;
            this.id = id;
        }

        public override Property GetCombination(Property property)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateContentsSignature(IO.IChecksum checksum)
        {
            checksum.UpdateValue(this.tagType);
            checksum.UpdateValue(this.tagData);
            checksum.UpdateValue(this.id);
        }

        public override bool CompareEqualContents(object value)
        {
            return UserTagProperty.CompareEqualContents(this, value as UserTagProperty);
        }

        private static bool CompareEqualContents(UserTagProperty a, UserTagProperty b)
        {
            return a.tagType == b.tagType && a.tagData == b.tagData && a.id == b.id;
        }

        #region UserTagComparer Class
        private class UserTagComparer : System.Collections.IComparer
        {
            #region IComparer Members
            public int Compare(object x, object y)
            {
                Properties.UserTagProperty px = x as Properties.UserTagProperty;
                Properties.UserTagProperty py = y as Properties.UserTagProperty;

                int result = string.Compare(px.tagType, py.tagType);

                if (result == 0)
                {
                    result = string.Compare(px.tagData, py.tagData);

                    if (result == 0)
                    {
                        if (px.id != py.id)
                        {
                            result = (px.id < py.id) ? -1 : 1;
                        }
                    }
                }

                return result;
            }
            #endregion
        }
        #endregion


        private string tagType;
        private string tagData;
        private long id;
    }
}
