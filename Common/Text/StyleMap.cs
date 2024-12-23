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

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Common.Text
{
    /// <summary>
    /// La classe StyleMap permet de faire correspondre des styles à des noms
    /// de haut niveau, tels que vus par l'utilisateur.
    /// </summary>
    public sealed class StyleMap : System.Collections.IEnumerable, Common.Support.IXMLWritable
    {
        internal StyleMap(StyleList list)
        {
            this.styleList = list;
            this.tStyleHash = new();
            this.captionHash = new();
            this.rankHash = new();
        }

        public void SetCaption(Common.Support.OpletQueue queue, TextStyle style, string caption)
        {
            string key = this.GetKeyName(style);

            int oldRank = this.GetRank(style);
            string oldCaption = this.GetCaption(style);
            string newCaption = caption;

            if (oldCaption != newCaption)
            {
                if (oldCaption != null)
                {
                    if (this.captionHash[oldCaption] as string == key)
                    {
                        this.captionHash.Remove(oldCaption);
                    }
                }

                if (newCaption != null)
                {
                    if (this.captionHash.ContainsKey(newCaption) == false)
                    {
                        this.captionHash[newCaption] = key;
                        this.tStyleHash[key] = newCaption;
                    }
                }
                else
                {
                    this.tStyleHash.Remove(key);
                }

                if (queue != null)
                {
                    TextStory.InsertOplet(queue, new ChangeOplet(this, style, oldCaption, oldRank));
                }

                this.styleList.NotifyStyleMapChanged();
            }

            System.Diagnostics.Debug.Assert(this.GetCaption(style) == newCaption);
            System.Diagnostics.Debug.Assert(
                newCaption == null || this.GetTextStyle(newCaption) == style
            );
        }

        public void SetRank(Common.Support.OpletQueue queue, TextStyle style, int rank)
        {
            int oldRank = this.GetRank(style);
            int newRank = rank;
            string oldCaption = this.GetCaption(style);

            if (oldRank != newRank)
            {
                if (newRank < 0)
                {
                    if (newRank != -1)
                    {
                        throw new System.ArgumentOutOfRangeException(
                            "rank",
                            newRank,
                            string.Format("Rank {0} not allowed", newRank)
                        );
                    }

                    if (this.rankHash.ContainsKey(style))
                    {
                        this.rankHash.Remove(style);
                        this.ClearCache();
                    }
                }
                else
                {
                    this.rankHash[style] = newRank;
                    this.ClearCache();
                }

                if (queue != null)
                {
                    TextStory.InsertOplet(queue, new ChangeOplet(this, style, oldCaption, oldRank));
                }

                this.styleList.NotifyStyleMapChanged();
            }
        }

        public string GetCaption(TextStyle style)
        {
            string key = this.GetKeyName(style);

            if (this.tStyleHash.ContainsKey(key))
            {
                return this.tStyleHash[key] as string;
            }

            return null;
        }

        public int GetRank(TextStyle style)
        {
            if (this.rankHash.ContainsKey(style))
            {
                return (int)this.rankHash[style];
            }

            return -1;
        }

        public TextStyle GetTextStyle(string caption)
        {
            string key = this.captionHash[caption] as string;

            if (key != null)
            {
                return this.FindTextStyle(key);
            }

            return null;
        }

        public TextStyle GetTextStyle(int rank)
        {
            foreach (var (key, value) in this.rankHash)
            {
                if (value == rank)
                {
                    return key;
                }
            }

            return null;
        }

        public TextStyle[] GetSortedStyles()
        {
            this.UpdateCache();
            return (TextStyle[])this.sortedList.Clone();
        }

        internal void Serialize(System.Text.StringBuilder buffer)
        {
            throw new System.NotImplementedException();
            System.Diagnostics.Debug.Assert(this.tStyleHash.Count == this.captionHash.Count);

            //SerializerSupport.SerializeStringStringHash(this.tStyleHash, buffer);
            buffer.Append("/");

            System.Collections.Hashtable hash = new System.Collections.Hashtable();

            foreach (var (key, value) in this.rankHash)
            {
                string name = StyleList.GetFullName(key);

                hash[name] = value;
            }

            SerializerSupport.SerializeStringIntHash(hash, buffer);
        }

        public bool HasEquivalentData(Common.Support.IXMLWritable other)
        {
            StyleMap otherStyleMap = (StyleMap)other;
            // bl-converter rankHash serialization is not working
            return this.tStyleHash.SequenceEqual(otherStyleMap.tStyleHash);
            //&& this.rankHash.SequenceEqual(otherStyleMap.rankHash);
        }

        public XElement ToXML()
        {
            return new XElement(
                "StyleMap",
                new XElement(
                    "StyleHash",
                    this.tStyleHash.Select(item => new XElement(
                        "Item",
                        new XAttribute("Key", item.Key),
                        new XAttribute("Value", item.Value)
                    ))
                ),
                new XElement(
                    "RankHash",
                    this.rankHash.Select(item => new XElement(
                        "Item",
                        new XAttribute("Key", StyleList.GetFullName(item.Key)),
                        new XAttribute("Value", item.Value)
                    ))
                )
            );
        }

        internal StyleMap(XElement xml, StyleList list)
        {
            this.styleList = list;
            this.tStyleHash = xml.Element("StyleHash")
                .Elements()
                .Select(item => (item.Attribute("Key").Value, item.Attribute("Value").Value))
                .ToDictionary();
            this.rankHash = xml.Element("RankHash")
                .Elements()
                .Select(this.ExtractRankHashEntry)
                .ToDictionary();

            //	Construit encore le dictionnaire inverse utilisé pour retrouver
            //	rapidement un style d'après son nom (caption) :

            this.captionHash = new();
            foreach (var (key, value) in this.tStyleHash)
            {
                this.captionHash[value] = key;
            }
        }

        private (TextStyle, int) ExtractRankHashEntry(XElement xml)
        {
            string key = xml.Attribute("Key").Value;
            TextStyle textStyle = this.styleList.GetTextStyle(key);
            System.Diagnostics.Debug.Assert(textStyle != null);
            return (textStyle, (int)xml.Attribute("Value"));
        }

        internal void Deserialize(TextContext context, int version, string[] args, ref int offset)
        {
            this.tStyleHash = new();
            this.captionHash = new();
            this.rankHash = new();
            this.sortedList = null;

            System.Collections.Hashtable hash = new System.Collections.Hashtable();

            SerializerSupport.DeserializeStringStringHash(args, ref offset, this.tStyleHash);
            SerializerSupport.DeserializeStringIntHash(args, ref offset, hash);

            foreach (System.Collections.DictionaryEntry entry in hash)
            {
                string name = entry.Key as string;
                TextStyle key = this.styleList.GetTextStyle(name);

                this.rankHash[key] = (int)entry.Value;
            }

            //	Construit encore le dictionnaire inverse utilisé pour retrouver
            //	rapidement un style d'après son nom (caption) :

            foreach (var (key, value) in this.tStyleHash)
            {
                this.captionHash[value] = key;
            }
        }

        private string GetKeyName(TextStyle style)
        {
            return string.Concat(style.Name, ".", style.TextStyleClass.ToString());
        }

        private TextStyle FindTextStyle(string key)
        {
            string[] s = key.Split('.');
            string name = string.Join(".", s, 0, s.Length - 1);

            TextStyleClass textStyleClass = (TextStyleClass)
                System.Enum.Parse(typeof(TextStyleClass), s[s.Length - 1]);

            return this.styleList[name, textStyleClass];
        }

        private void ClearCache()
        {
            this.sortedList = null;
        }

        private void UpdateCache()
        {
            if (this.sortedList == null)
            {
                lock (this)
                {
                    if (this.sortedList == null)
                    {
                        int n = this.rankHash.Count;

                        int[] ranks = new int[n];
                        TextStyle[] styles = new TextStyle[n];

                        this.rankHash.Values.CopyTo(ranks, 0);
                        this.rankHash.Keys.CopyTo(styles, 0);

                        System.Array.Sort(ranks, styles);

                        this.sortedList = styles;
                    }
                }
            }
        }

        #region IEnumerable Members
        public System.Collections.IEnumerator GetEnumerator()
        {
            this.UpdateCache();
            return this.sortedList.GetEnumerator();
        }
        #endregion

        #region ChangeOplet Class
        public class ChangeOplet : Common.Support.AbstractOplet
        {
            public ChangeOplet(StyleMap map, TextStyle style, string oldCaption, int oldRank)
            {
                this.map = map;
                this.style = style;
                this.caption = oldCaption;
                this.rank = oldRank;
            }

            public override Epsitec.Common.Support.IOplet Undo()
            {
                string newCaption = this.caption;
                string oldCaption = this.map.GetCaption(this.style);

                int newRank = this.rank;
                int oldRank = this.map.GetRank(this.style);

                this.map.SetCaption(null, this.style, newCaption);
                this.map.SetRank(null, this.style, newRank);

                this.caption = oldCaption;
                this.rank = oldRank;

                return this;
            }

            public override Epsitec.Common.Support.IOplet Redo()
            {
                return this.Undo();
            }

            public bool MergeWith(ChangeOplet other)
            {
                if ((this.style == other.style) && (this.map == other.map))
                {
                    return true;
                }

                return false;
            }

            private StyleMap map;
            private TextStyle style;
            private int rank;
            private string caption;
        }
        #endregion

        private StyleList styleList;
        private Dictionary<string, string> tStyleHash; //	text style -> caption
        private Dictionary<string, string> captionHash; //	caption -> text style
        private Dictionary<TextStyle, int> rankHash; //	rank -> text style
        private TextStyle[] sortedList;
    }
}
