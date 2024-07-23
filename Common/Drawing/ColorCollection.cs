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
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Epsitec.Common.Drawing
{
    /// <summary>
    /// La classe ColorCollection contient une collection de couleurs.
    /// </summary>

    [System.Serializable]
    public sealed class ColorCollection
        : ISerializable,
            System.Collections.ICollection,
            ICollection<RichColor>,
            Types.INotifyChanged,
            Support.IXMLSerializable<ColorCollection>
    {
        public ColorCollection()
            : this(ColorCollectionType.Default) { }

        public ColorCollection(ColorCollectionType type)
            : this(ColorCollection.DefaultCount)
        {
            this.Initialize(type);
        }

        public ColorCollection(int count)
        {
            this.colors = new RichColor[count];

            for (int i = 0; i < count; i++)
            {
                this.colors[i] = RichColor.Empty;
            }
        }

        public ColorCollection(IEnumerable<RichColor> colors)
            : this(ColorCollection.CountItems(colors))
        {
            int index = 0;

            foreach (var item in colors)
            {
                this.colors[index++] = item;
            }
        }

        public RichColor this[int index]
        {
            get
            {
                if (index >= 0 && index < this.colors.Length)
                {
                    return this.colors[index];
                }
                else
                {
                    return RichColor.Empty;
                }
            }
            set
            {
                if (index >= 0 && index < this.colors.Length)
                {
                    if (this.colors[index] != value)
                    {
                        this.colors[index] = value;
                        this.OnChanged();
                    }
                }
            }
        }

        public void CopyTo(ColorCollection destination)
        {
            if (this.colors.Length == destination.colors.Length)
            {
                this.colors.CopyTo(destination.colors, 0);
                destination.OnChanged();
            }
            else
            {
                throw new System.InvalidOperationException("ColorCollection size mismatch");
            }
        }

        public void CopyFrom(ColorCollection source)
        {
            source.CopyTo(this);
        }

        #region ICollection Members

        public int Count
        {
            get { return this.colors.Length; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        void System.Collections.ICollection.CopyTo(System.Array array, int index)
        {
            this.colors.CopyTo(array, index);
        }

        #endregion

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            return this.colors.GetEnumerator();
        }

        #endregion

        #region ICollection<RichColor> Members

        public void Add(RichColor item)
        {
            throw new System.InvalidOperationException("Add not possible in fixed size collection");
        }

        public void Clear()
        {
            throw new System.InvalidOperationException(
                "Clear not possible in fixed size collection"
            );
        }

        public bool Contains(RichColor item)
        {
            for (int i = 0; i < this.colors.Length; i++)
            {
                if (this.colors[i] == item)
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(RichColor[] array, int arrayIndex)
        {
            this.colors.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(RichColor item)
        {
            throw new System.InvalidOperationException(
                "Remove not possible in fixed size collection"
            );
        }

        #endregion

        #region IEnumerable<RichColor> Members

        IEnumerator<RichColor> IEnumerable<RichColor>.GetEnumerator()
        {
            foreach (RichColor item in this.colors)
            {
                yield return item;
            }
        }

        #endregion

        #region INotifyChanged Members

        public event Support.EventHandler Changed;

        #endregion

        #region Serialization
        public bool HasEquivalentData(Support.IXMLWritable other)
        {
            ColorCollection otherCollection = (ColorCollection)other;
            return this.colors.Zip(otherCollection.colors).All(item => item.First == item.Second);
        }

        public XElement ToXML()
        {
            return new XElement("ColorCollection", this.colors.Select(color => color.ToXML()));
        }

        public static ColorCollection FromXML(XElement xml)
        {
            return new ColorCollection(xml);
        }

        private ColorCollection(XElement xml)
        {
            var root = xml.Element("ColorCollection");
            this.colors = root.Elements().Select(color => RichColor.FromXML(color)).ToArray();
        }

        private ColorCollection(SerializationInfo info, StreamingContext context)
        {
            int revision = info.GetInt32("Rev");

            if (revision == 0)
            {
                Color[] temp = (Color[])info.GetValue("Colors", typeof(Color[]));

                this.colors = new RichColor[ColorCollection.DefaultCount];
                for (int i = 0; i < ColorCollection.DefaultCount; i++)
                {
                    this.colors[i] = new RichColor(temp[i]);
                }
            }
            else
            {
                this.colors = (RichColor[])info.GetValue("Colors", typeof(RichColor[]));
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Rev", 1);
            info.AddValue("Colors", this.colors);
        }

        #endregion

        private void OnChanged()
        {
            if (this.Changed != null) // qq'un écoute ?
            {
                this.Changed(this);
            }
        }

        private void Initialize(ColorCollectionType type)
        {
            switch (type)
            {
                case ColorCollectionType.Gray:

                    this.colors[0 + 8 * 0] = RichColor.FromAlphaRgb(0.0, 1.0, 1.0, 1.0);
                    this.colors[0 + 8 * 1] = RichColor.FromAlphaRgb(1.0, 1.0, 0.0, 0.0);
                    this.colors[0 + 8 * 2] = RichColor.FromAlphaRgb(1.0, 0.0, 1.0, 0.0);
                    this.colors[0 + 8 * 3] = RichColor.FromAlphaRgb(1.0, 0.0, 0.0, 1.0);

                    for (int i = 0; i < 28; i++)
                    {
                        int j = 1 + (i / 4) + (i % 4) * 8;
                        this.colors[j] = RichColor.FromGray(1.0 - i / 27.0);
                    }

                    break;

                case ColorCollectionType.Rainbow:

                    this.colors[0 + 8 * 0] = RichColor.FromAGray(0.0, 1.00);
                    this.colors[0 + 8 * 1] = RichColor.FromAGray(1.0, 1.00);
                    this.colors[0 + 8 * 2] = RichColor.FromAGray(1.0, 0.83);
                    this.colors[0 + 8 * 3] = RichColor.FromAGray(1.0, 0.67);
                    this.colors[1 + 8 * 0] = RichColor.FromAGray(1.0, 0.50);
                    this.colors[1 + 8 * 1] = RichColor.FromAGray(1.0, 0.33);
                    this.colors[1 + 8 * 2] = RichColor.FromAGray(1.0, 0.17);
                    this.colors[1 + 8 * 3] = RichColor.FromAGray(1.0, 0.00);

                    for (int i = 0; i < 24; i++)
                    {
                        int j = 2 + (i / 4) + (i % 4) * 8;
                        double h = 1.0 - (i / 24.0);
                        this.colors[j] = RichColor.FromHsv(h * 360, 1.0, 1.0);
                    }

                    break;

                case ColorCollectionType.Light:

                    this.colors[0 + 8 * 0] = RichColor.FromAGray(0.0, 1.00);
                    this.colors[0 + 8 * 1] = RichColor.FromAGray(1.0, 1.00);
                    this.colors[0 + 8 * 2] = RichColor.FromAGray(1.0, 0.83);
                    this.colors[0 + 8 * 3] = RichColor.FromAGray(1.0, 0.67);
                    this.colors[1 + 8 * 0] = RichColor.FromAGray(1.0, 0.50);
                    this.colors[1 + 8 * 1] = RichColor.FromAGray(1.0, 0.33);
                    this.colors[1 + 8 * 2] = RichColor.FromAGray(1.0, 0.17);
                    this.colors[1 + 8 * 3] = RichColor.FromAGray(1.0, 0.00);

                    for (int i = 0; i < 24; i++)
                    {
                        int j = 2 + (i / 4) + (i % 4) * 8;
                        double h = 1.0 - (i / 24.0);
                        this.colors[j] = RichColor.FromHsv(h * 360, 0.3, 1.0);
                    }

                    break;

                case ColorCollectionType.Dark:

                    this.colors[0 + 8 * 0] = RichColor.FromAGray(0.0, 1.00);
                    this.colors[0 + 8 * 1] = RichColor.FromAGray(1.0, 1.00);
                    this.colors[0 + 8 * 2] = RichColor.FromAGray(1.0, 0.83);
                    this.colors[0 + 8 * 3] = RichColor.FromAGray(1.0, 0.67);
                    this.colors[1 + 8 * 0] = RichColor.FromAGray(1.0, 0.50);
                    this.colors[1 + 8 * 1] = RichColor.FromAGray(1.0, 0.33);
                    this.colors[1 + 8 * 2] = RichColor.FromAGray(1.0, 0.17);
                    this.colors[1 + 8 * 3] = RichColor.FromAGray(1.0, 0.00);

                    for (int i = 0; i < 24; i++)
                    {
                        int j = 2 + (i / 4) + (i % 4) * 8;
                        double h = 1.0 - (i / 24.0);
                        this.colors[j] = RichColor.FromHsv(h * 360, 1.0, 0.7);
                    }

                    break;

                case ColorCollectionType.Default:

                    this.colors[0 * 8 + 0] = RichColor.FromAGray(0.0, 1.0);
                    this.colors[0 * 8 + 1] = RichColor.FromAlphaRgb(1.0, 1.0, 0.7, 0.7);
                    this.colors[0 * 8 + 2] = RichColor.FromAlphaRgb(1.0, 1.0, 1.0, 0.7);
                    this.colors[0 * 8 + 3] = RichColor.FromAlphaRgb(1.0, 0.7, 1.0, 0.7);
                    this.colors[0 * 8 + 4] = RichColor.FromAlphaRgb(1.0, 0.7, 1.0, 1.0);
                    this.colors[0 * 8 + 5] = RichColor.FromAlphaRgb(1.0, 0.7, 0.7, 1.0);
                    this.colors[0 * 8 + 6] = RichColor.FromAlphaRgb(1.0, 1.0, 0.7, 1.0);
                    this.colors[0 * 8 + 7] = RichColor.FromAGray(0.2, 0.5);

                    this.colors[1 * 8 + 0] = RichColor.FromAGray(0.3, 1.0);
                    this.colors[1 * 8 + 1] = RichColor.FromAlphaRgb(1.0, 1.0, 0.0, 0.0);
                    this.colors[1 * 8 + 2] = RichColor.FromAlphaRgb(1.0, 1.0, 1.0, 0.0);
                    this.colors[1 * 8 + 3] = RichColor.FromAlphaRgb(1.0, 0.0, 1.0, 0.0);
                    this.colors[1 * 8 + 4] = RichColor.FromAlphaRgb(1.0, 0.0, 1.0, 1.0);
                    this.colors[1 * 8 + 5] = RichColor.FromAlphaRgb(1.0, 0.0, 0.0, 1.0);
                    this.colors[1 * 8 + 6] = RichColor.FromAlphaRgb(1.0, 1.0, 0.0, 1.0);
                    this.colors[1 * 8 + 7] = RichColor.FromAGray(0.5, 0.5);

                    this.colors[2 * 8 + 0] = RichColor.FromAGray(0.6, 1.0);
                    this.colors[2 * 8 + 1] = RichColor.FromAlphaRgb(1.0, 0.7, 0.0, 0.0);
                    this.colors[2 * 8 + 2] = RichColor.FromAlphaRgb(1.0, 0.7, 0.7, 0.0);
                    this.colors[2 * 8 + 3] = RichColor.FromAlphaRgb(1.0, 0.0, 0.7, 0.0);
                    this.colors[2 * 8 + 4] = RichColor.FromAlphaRgb(1.0, 0.0, 0.7, 0.7);
                    this.colors[2 * 8 + 5] = RichColor.FromAlphaRgb(1.0, 0.0, 0.0, 0.7);
                    this.colors[2 * 8 + 6] = RichColor.FromAlphaRgb(1.0, 0.7, 0.0, 0.7);
                    this.colors[2 * 8 + 7] = RichColor.FromAGray(0.8, 0.5);

                    this.colors[3 * 8 + 0] = RichColor.FromGray(1.0);
                    this.colors[3 * 8 + 1] = RichColor.FromGray(0.9);
                    this.colors[3 * 8 + 2] = RichColor.FromGray(0.8);
                    this.colors[3 * 8 + 3] = RichColor.FromGray(0.7);
                    this.colors[3 * 8 + 4] = RichColor.FromGray(0.6);
                    this.colors[3 * 8 + 5] = RichColor.FromGray(0.5);
                    this.colors[3 * 8 + 6] = RichColor.FromGray(0.4);
                    this.colors[3 * 8 + 7] = RichColor.FromGray(0.0);

                    break;

                default:
                    throw new System.ArgumentOutOfRangeException(
                        "type",
                        type,
                        "Type not supported."
                    );
            }

            this.OnChanged();
        }

        private static int CountItems(IEnumerable<RichColor> colors)
        {
            //	We do not want to use LINQ here (this DLL must be .NET 2.0-only compatible).
            int count = 0;

            foreach (var item in colors)
            {
                count++;
            }

            return count;
        }

        private const int DefaultCount = 32;

        private readonly RichColor[] colors;
    }
}
