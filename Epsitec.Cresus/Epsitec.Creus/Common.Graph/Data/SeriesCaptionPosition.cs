using Epsitec.Common.Drawing;

namespace Epsitec.Common.Graph.Data
{
    /// <summary>
    /// User by he renderers to tell where to paint the SeriesCaptions
    /// </summary>
    public class SeriesCaptionPosition
    {
        /// <summary>Show or not the captions.
        /// Default: true</summary>
        public bool ShowCaption
        {
            get
            {
                return showCaption;
            }
            set
            {
                showCaption = value;
            }
        }

        /// <summary>Angle that the renderer prefers to use to paint the caption</summary>
        public double Angle { get; set; }

        /// <summary>Reference point</summary>
        public Point Position { get; set; }

        /// <summary>Aligment of the text, from the <see cref="Position"/></summary>
        public ContentAlignment Alignment
        {
            get
            {
                return alignement;
            }
            set
            {
                alignement = value;
            }
        }

        private bool showCaption = true;
        private ContentAlignment alignement = ContentAlignment.MiddleCenter;
    }
}
