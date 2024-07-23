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

using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Common.Text.Properties
{
    /// <summary>
    /// La classe ManagedParagraphProperty décrit un paragraphe géré par une
    /// class implémentant IParagraphManager (en principe une liste à puces,
    /// par exemple) qui génère du texte automatique (AutoText).
    /// </summary>
    public class ManagedParagraphProperty
        : Property,
            Common.Support.IXMLSerializable<ManagedParagraphProperty>
    {
        public ManagedParagraphProperty() { }

        public ManagedParagraphProperty(string managerName, string[] managerParameters)
        {
            this.managerName = managerName;
            this.managerParameters =
                managerParameters == null
                    ? Epsitec.Common.Types.Collections.EmptyArray<string>.Instance
                    : (managerParameters.Clone() as string[]);
        }

        public override WellKnownType WellKnownType
        {
            get { return WellKnownType.ManagedParagraph; }
        }

        public override PropertyType PropertyType
        {
            get { return PropertyType.CoreSetting; }
        }

        public override CombinationMode CombinationMode
        {
            get { return CombinationMode.Combine; }
        }

        public override bool RequiresUniformParagraph
        {
            get { return true; }
        }

        public string ManagerName
        {
            get { return this.managerName; }
        }

        public string[] ManagerParameters
        {
            get { return this.managerParameters.Clone() as string[]; }
        }

        public static System.Collections.IComparer Comparer
        {
            get { return new ManagedParagraphComparer(); }
        }

        public override Property EmptyClone()
        {
            return new ManagedParagraphProperty();
        }

        public override bool HasEquivalentData(Common.Support.IXMLWritable otherWritable)
        {
            ManagedParagraphProperty other = (ManagedParagraphProperty)otherWritable;
            bool allOk =
                this.managerName == other.managerName
                && this.managerParameters.SequenceEqual(
                    other.managerParameters.Where(item => item != null)
                );
            return allOk;
        }

        public override XElement ToXML()
        {
            return new XElement(
                "ManagedParagraphProperty",
                new XAttribute("ManagerName", this.managerName),
                new XElement(
                    "ManagerParameters",
                    this.managerParameters.Where(item => item != null)
                        .Select(item => new XElement("Item", new XAttribute("Value", item)))
                )
            );
        }

        public static ManagedParagraphProperty FromXML(XElement xml)
        {
            return new ManagedParagraphProperty(xml);
        }

        private ManagedParagraphProperty(XElement xml)
        {
            this.managerName = xml.Attribute("ManagerName").Value;
            this.managerParameters = xml.Element("ManagerParameters")
                .Elements()
                .Select(item => item.Attribute("Value").Value)
                .ToArray();
        }

        public override void SerializeToText(System.Text.StringBuilder buffer)
        {
            SerializerSupport.Join(
                buffer,
                /**/SerializerSupport.SerializeString(this.managerName),
                /**/SerializerSupport.SerializeStringArray(this.managerParameters)
            );
        }

        public override void DeserializeFromText(
            TextContext context,
            string text,
            int pos,
            int length
        )
        {
            string[] args = SerializerSupport.Split(text, pos, length);

            Debug.Assert.IsTrue(args.Length == 2);

            string managerName = SerializerSupport.DeserializeString(args[0]);
            string[] managerParameters = SerializerSupport.DeserializeStringArray(args[1]);

            this.managerName = managerName;
            this.managerParameters = managerParameters;
        }

        public override Property GetCombination(Property property)
        {
            Debug.Assert.IsTrue(property is Properties.ManagedParagraphProperty);

            ManagedParagraphProperty a = this;
            ManagedParagraphProperty b = property as ManagedParagraphProperty;
            ManagedParagraphProperty c = new ManagedParagraphProperty();

            c.managerName = b.managerName;
            c.managerParameters = b.managerParameters;

            return c;
        }

        public override void UpdateContentsSignature(IO.IChecksum checksum)
        {
            checksum.UpdateValue(this.managerName);
            checksum.UpdateValue(this.managerParameters);
        }

        public override bool CompareEqualContents(object value)
        {
            return ManagedParagraphProperty.CompareEqualContents(
                this,
                value as ManagedParagraphProperty
            );
        }

        public static ManagedParagraphProperty Find(Property[] properties, string name)
        {
            foreach (Property property in properties)
            {
                if (property.WellKnownType == WellKnownType.ManagedParagraph)
                {
                    ManagedParagraphProperty managed = property as ManagedParagraphProperty;

                    if (managed.ManagerName == name)
                    {
                        return managed;
                    }
                }
            }

            return null;
        }

        public static ManagedParagraphProperty[] Filter(System.Collections.ICollection properties)
        {
            int count = 0;

            foreach (Property property in properties)
            {
                if (property is ManagedParagraphProperty)
                {
                    count++;
                }
            }

            ManagedParagraphProperty[] filtered = new ManagedParagraphProperty[count];

            int index = 0;

            foreach (Property property in properties)
            {
                if (property is ManagedParagraphProperty)
                {
                    filtered[index++] = property as ManagedParagraphProperty;
                }
            }

            System.Diagnostics.Debug.Assert(index == count);

            return filtered;
        }

        private static bool CompareEqualContents(
            ManagedParagraphProperty a,
            ManagedParagraphProperty b
        )
        {
            return a.managerName == b.managerName
                && Types.Comparer.Equal(a.managerParameters, b.managerParameters);
        }

        #region ManagedParagraphComparer Class
        private class ManagedParagraphComparer : System.Collections.IComparer
        {
            #region IComparer Members
            public int Compare(object x, object y)
            {
                Properties.ManagedParagraphProperty px = x as Properties.ManagedParagraphProperty;
                Properties.ManagedParagraphProperty py = y as Properties.ManagedParagraphProperty;

                int result = string.Compare(px.managerName, py.managerName);

                if (result == 0)
                {
                    //	TODO: comparer les paramètres...
                }

                return result;
            }
            #endregion
        }
        #endregion

        private string managerName;
        private string[] managerParameters;
    }
}
