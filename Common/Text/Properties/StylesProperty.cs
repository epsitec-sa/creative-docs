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
    /// La classe StylesProperty contient une liste de styles (cascadés)
    /// qui doivent s'appliquer au texte.
    /// </summary>
    public class StylesProperty : Property, Common.Support.IXMLSerializable<StylesProperty>
    {
        public StylesProperty()
        {
            this.Setup(null);
        }

        public StylesProperty(TextStyle style)
        {
            this.Setup(new TextStyle[] { style });
        }

        public StylesProperty(System.Collections.ICollection styles)
        {
            this.Setup(styles);
        }

        private void Setup(System.Collections.ICollection styles)
        {
            int n = styles == null ? 0 : styles.Count;
            int i = 0;

            this.styleNames = new string[n];

            if (n > 0)
            {
                foreach (TextStyle style in styles)
                {
                    this.styleNames[i++] = StyleList.GetFullName(style.Name, style.TextStyleClass);
                }
            }
        }

        public override PropertyType PropertyType
        {
            get { return PropertyType.Polymorph; }
        }

        public override WellKnownType WellKnownType
        {
            get { return WellKnownType.Styles; }
        }

        public string[] StyleNames
        {
            get { return this.styleNames.Clone() as string[]; }
        }

        public int StyleCount
        {
            get { return this.styleNames.Length; }
        }

        public TextStyle[] GetTextStyles(TextContext context)
        {
            if (this.styleCache == null)
            {
                this.RefreshStyleCache(context.StyleList);
            }

            return (TextStyle[])this.styleCache.Clone();
        }

        public override Property EmptyClone()
        {
            return new StylesProperty();
        }

        public override void SerializeToText(System.Text.StringBuilder buffer)
        {
            SerializerSupport.Join(
                buffer,
                /**/SerializerSupport.SerializeStringArray(this.styleNames)
            );
        }

        public override bool HasEquivalentData(Common.Support.IXMLWritable otherWritable)
        {
            StylesProperty other = (StylesProperty)otherWritable;
            return this.styleNames.SequenceEqual(other.styleNames);
        }

        public override XElement ToXML()
        {
            return new XElement(
                "StylesProperty",
                new XElement(
                    "StyleNames",
                    this.styleNames.Select(item => new XElement(
                        "Item",
                        new XAttribute("Value", item)
                    ))
                )
            );
        }

        public static StylesProperty FromXML(XElement xml)
        {
            return new StylesProperty(xml);
        }

        private StylesProperty(XElement xml)
        {
            this.styleNames = xml.Element("StyleNames")
                .Elements()
                .Select(item => item.Attribute("Value").Value)
                .ToArray();
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

            string[] names = SerializerSupport.DeserializeStringArray(args[0]);

            this.styleNames = names;
            this.styleCache = null;
        }

        public override Property GetCombination(Property property)
        {
            //	Produit une propriété qui est le résultat de la combinaison de
            //	la propriété actuelle avec celle passée en entrée (qui vient
            //	ajouter ses attributs aux attributs actuels).

            Debug.Assert.IsTrue(property is Properties.StylesProperty);

            StylesProperty a = this;
            StylesProperty b = property as StylesProperty;
            StylesProperty c = new StylesProperty();

            //	TODO: gérer les doublets

            c.styleNames = new string[a.styleNames.Length + b.styleNames.Length];

            a.styleNames.CopyTo(c.styleNames, 0);
            b.styleNames.CopyTo(c.styleNames, a.styleNames.Length);

            return c;
        }

        public override bool CompareEqualContents(object value)
        {
            return StylesProperty.CompareEqualContents(this, value as StylesProperty);
        }

        public override void UpdateContentsSignature(Epsitec.Common.IO.IChecksum checksum)
        {
            foreach (string style in this.styleNames)
            {
                checksum.UpdateValue(style);
            }
        }

        public static int CountMatchingStyles(TextStyle[] styles, TextStyleClass styleClass)
        {
            int count = 0;

            for (int i = 0; i < styles.Length; i++)
            {
                if (styles[i].TextStyleClass == styleClass)
                {
                    count++;
                }
            }

            return count;
        }

        public static bool ContainsStylesProperties(System.Collections.ICollection properties)
        {
            foreach (Property property in properties)
            {
                if (property is StylesProperty)
                {
                    return true;
                }
            }

            return false;
        }

        public static Property[] RemoveStylesProperties(System.Collections.ICollection properties)
        {
            //	Supprime les propriétés StylesProprty de la liste.

            int count = 0;

            foreach (Property property in properties)
            {
                if (!(property is StylesProperty))
                {
                    count++;
                }
            }

            Property[] filtered = new Property[count];

            int index = 0;

            foreach (Property property in properties)
            {
                if (!(property is StylesProperty))
                {
                    filtered[index++] = property;
                }
            }

            System.Diagnostics.Debug.Assert(index == count);

            return filtered;
        }

        private void RefreshStyleCache(StyleList list)
        {
            //	La propriété stocke de manière interne une table des TextStyle
            //	correspondant à la table des noms de styles. Celle-ci ne peut
            //	pas être construite à la désérialisation (car les styles ne sont
            //	pas encore disponibles dans le StyleList).

            //	Profite de l'occasion pour remplacer d'éventuelles références à
            //	des styles supprimés (qui ne se trouvent plus dans le StyleList)
            //	par le style par défaut correspondant.

            int n = (this.styleNames == null) ? 0 : this.styleNames.Length;

            bool refreshNames = false;
            bool replaceDone = false;
            bool filterNull = false;

            this.styleCache = new TextStyle[n];

            for (int i = 0; i < n; i++)
            {
                TextStyle style = list.GetTextStyle(this.styleNames[i]);

                if (style == null)
                {
                    if (!replaceDone)
                    {
                        //	Le style a été supprimé et nous n'avons pas encore généré
                        //	de référence au style de paragraphe par défaut :

                        string name;
                        TextStyleClass textStyleClass;

                        StyleList.SplitFullName(this.styleNames[i], out name, out textStyleClass);

                        if (textStyleClass == TextStyleClass.Paragraph)
                        {
                            //	Ne remplace que le style qui faisait référence à un
                            //	style de paragraphe; les autres styles peuvent simple-
                            //	ment être "oubliés".

                            style = list.TextContext.DefaultParagraphStyle;
                            replaceDone = true;
                        }

                        refreshNames = true;
                    }
                }

                if (style == null)
                {
                    filterNull = true;
                }

                this.styleCache[i] = style;
            }

            if (filterNull)
            {
                this.styleCache = TextStyle.FilterNullStyles(this.styleCache);
            }

            if (refreshNames)
            {
                //	Regénère la table des noms de styles; ceci est nécessaire, car
                //	nous avons fait le ménage plus haut :

                this.styleNames = new string[this.styleCache.Length];

                for (int i = 0; i < this.styleCache.Length; i++)
                {
                    TextStyle style = this.styleCache[i];
                    string name = StyleList.GetFullName(style.Name, style.TextStyleClass);

                    this.styleNames[i] = name;
                }
            }
        }

        private static bool CompareEqualContents(StylesProperty a, StylesProperty b)
        {
            if (a == b)
            {
                return true;
            }

            return Types.Comparer.Equal(a.styleNames, b.styleNames);
        }

        private static bool CompareEqualContents(TextStyle[] a, TextStyle[] b)
        {
            if (a == b)
            {
                return true;
            }

            if ((a == null) || (b == null))
            {
                return false;
            }

            if (a.Length != b.Length)
            {
                return false;
            }

            int n = a.Length;

            for (int i = 0; i < n; i++)
            {
                if (
                    (a[i].GetContentsSignature() != b[i].GetContentsSignature())
                    || (a[i].Name != b[i].Name)
                )
                {
                    return false;
                }
            }

            for (int i = 0; i < n; i++)
            {
                if (TextStyle.CompareEqualContents(a[i], b[i]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        private string[] styleNames;
        private TextStyle[] styleCache;
    }
}
