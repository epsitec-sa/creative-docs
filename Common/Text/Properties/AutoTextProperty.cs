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
    /// La classe AutoTextProperty permet de décrire un morceau de texte
    /// généré automatiquement (avec styles particuliers); comparer avec
    /// GeneratorProperty qui génère aussi du texte.
    /// Attention: cette propriété requiert un traitement spécial de la part
    /// de TextContext.GetPropertiesQuickAndDirty.
    /// </summary>
    public class AutoTextProperty : Property, Common.Support.IXMLSerializable<AutoTextProperty>
    {
        public AutoTextProperty()
        {
            lock (AutoTextProperty.uniqueIdLock)
            {
                this.uniqueId = AutoTextProperty.nextUniqueId++;
            }
        }

        public AutoTextProperty(string tag)
            : this()
        {
            this.tag = tag;
        }

        public override WellKnownType WellKnownType
        {
            get { return WellKnownType.AutoText; }
        }

        public override PropertyType PropertyType
        {
            get { return PropertyType.ExtraSetting; }
        }

        public override CombinationMode CombinationMode
        {
            get { return CombinationMode.Accumulate; }
        }

        public string Tag
        {
            get { return this.tag; }
        }

        public long UniqueId
        {
            //	Grâce au UniqueId, on garantit que deux propriétés décrivant un
            //	même générateur sont toujours distinctes; ceci évite que deux
            //	textes adjacents faisant référence au même générateur ne soient
            //	considérés que comme un texte unique.

            get { return this.uniqueId; }
        }

        public override Property EmptyClone()
        {
            return new AutoTextProperty();
        }

        public override void SerializeToText(System.Text.StringBuilder buffer)
        {
            SerializerSupport.Join(
                buffer,
                /**/SerializerSupport.SerializeString(this.tag),
                /**/SerializerSupport.SerializeLong(this.uniqueId)
            );
        }

        public override bool HasEquivalentData(Common.Support.IXMLWritable otherWritable)
        {
            AutoTextProperty other = (AutoTextProperty)otherWritable;
            return this.tag == other.tag && this.uniqueId == other.uniqueId;
        }

        public override XElement ToXML()
        {
            return new XElement(
                "AutoTextProperty",
                new XAttribute("Tag", this.tag),
                new XAttribute("UniqueId", this.uniqueId)
            );
        }

        public static AutoTextProperty FromXML(XElement xml)
        {
            return new AutoTextProperty(xml);
        }

        private AutoTextProperty(XElement xml)
        {
            this.tag = xml.Attribute("Tag").Value;
            this.uniqueId = (long)xml.Attribute("UniqueId");
        }

        public override void DeserializeFromText(
            TextContext context,
            string text,
            int pos,
            int length
        )
        {
            string[] args = SerializerSupport.Split(text, pos, length);

            System.Diagnostics.Debug.Assert(args.Length == 2);

            string tag = SerializerSupport.DeserializeString(args[0]);
            long uniqueId = SerializerSupport.DeserializeLong(args[1]);

            this.tag = tag;
            this.uniqueId = uniqueId;

            lock (AutoTextProperty.uniqueIdLock)
            {
                if (AutoTextProperty.nextUniqueId <= this.uniqueId)
                {
                    AutoTextProperty.nextUniqueId = this.uniqueId + 1;
                }
            }
        }

        public override Property GetCombination(Property property)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateContentsSignature(IO.IChecksum checksum)
        {
            checksum.UpdateValue(this.tag);
            checksum.UpdateValue(this.uniqueId);
        }

        public override bool CompareEqualContents(object value)
        {
            return AutoTextProperty.CompareEqualContents(this, value as AutoTextProperty);
        }

        public static AutoTextProperty Find(Property[] properties, string tag)
        {
            foreach (Property property in properties)
            {
                if (property.WellKnownType == WellKnownType.AutoText)
                {
                    AutoTextProperty autoText = property as AutoTextProperty;

                    if (autoText.Tag == tag)
                    {
                        return autoText;
                    }
                }
            }

            return null;
        }

        private static bool CompareEqualContents(AutoTextProperty a, AutoTextProperty b)
        {
            return a.tag == b.tag && a.uniqueId == b.uniqueId;
        }

        private static long nextUniqueId;
        private static object uniqueIdLock = new object();

        private string tag;
        private long uniqueId;
    }
}
