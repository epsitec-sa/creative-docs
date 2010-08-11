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

        /// <summary>Center of the caption</summary>
        public Point Position { get; set; }

        private bool showCaption = true;
    }
}
