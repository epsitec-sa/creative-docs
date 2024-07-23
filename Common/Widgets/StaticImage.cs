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


namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe StaticImage dessine une image. Cette image est spécifiée
    /// par son nom.
    /// </summary>
    public class StaticImage : StaticText
    {
        public StaticImage() { }

        public StaticImage(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public StaticImage(string name)
            : this()
        {
            this.ImageName = name;
        }

        public StaticImage(Widget embedder, string name)
            : this(embedder)
        {
            this.ImageName = name;
        }

        public string ImageName
        {
            get { return this.imageName; }
            set
            {
                if (this.imageName != value)
                {
                    this.imageName = value;
                    this.RebuildTextLayout();
                }
            }
        }

        public Drawing.Size ImageSize
        {
            get { return this.imageSize; }
            set
            {
                if (this.imageSize != value)
                {
                    this.imageSize = value;
                    this.RebuildTextLayout();
                }
            }
        }

        public double VerticalOffset
        {
            get { return this.verticalOffset; }
            set
            {
                if (this.verticalOffset != value)
                {
                    this.verticalOffset = value;
                    this.RebuildTextLayout();
                }
            }
        }

        public Drawing.Image Image
        {
            get { return this.image; }
            set
            {
                this.image = value;
                this.imageName = "callback:";
                this.RebuildTextLayout();
            }
        }

        private void RebuildTextLayout()
        {
            if ((this.ImageName == null) || (this.ImageName.Length == 0))
            {
                this.Text = "";
            }
            else
            {
                System.Text.StringBuilder buffer = new System.Text.StringBuilder();

                buffer.Append("<img src=\"");
                buffer.Append(TextLayout.ConvertToTaggedText(this.ImageName));
                buffer.Append("\"");

                int vOffset = (int)(this.VerticalOffset * 100 + 0.5);

                int imageDx = (int)(this.imageSize.Width + 0.5);
                int imageDy = (int)(this.imageSize.Height + 0.5);

                if (vOffset != 0)
                {
                    buffer.Append(" voff=\"");
                    buffer.AppendFormat(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "{0}",
                        vOffset / 100.0
                    );
                    buffer.Append("\"");
                }

                if (imageDx > 0)
                {
                    buffer.Append(" dx=\"");
                    buffer.AppendFormat(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "{0}",
                        imageDx
                    );
                    buffer.Append("\"");
                }
                if (imageDy > 0)
                {
                    buffer.Append(" dy=\"");
                    buffer.AppendFormat(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "{0}",
                        imageDy
                    );
                    buffer.Append("\"");
                }

                buffer.Append("/>");

                this.Text = buffer.ToString();
            }
        }

        protected override void UpdateTextLayout()
        {
            this.TextLayout.ImageCallback = this.ImageCallback;

            base.UpdateTextLayout();
        }

        private Drawing.Image ImageCallback(string name)
        {
            return this.image;
        }

        protected string imageName;
        protected Drawing.Size imageSize;
        protected double verticalOffset;
        protected Drawing.Image image;
    }
}
