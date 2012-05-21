using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Data;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.Graph.Styles;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Graph.Widgets
{
    public class FloatingCaptionsView : FrameBox
    {

        public AbstractRenderer Renderer { get; set; }

        protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
        {
            var style = new CaptionStyle();

            var i = 0;
            foreach (var item in Renderer.SeriesItems)
            {
                var point = Renderer.GetFloatingLabelPosition(item, i);

                if (!point.IsZero)
                {
                    var txt = DataCube.CleanUpLabel (item.Label);
                    var txtWidth = Renderer.Captions.Style.GetTextWidth (txt);
                    var txtHeight = Renderer.Captions.Style.GetTextLineHeight ();
                    var offset = Renderer.Captions.Style.GetTextLineOffset ();

                    using (var r = new Path(new Rectangle(point.X - txtWidth / 2 - 2, point.Y - offset.Y, txtWidth + 4, txtHeight)))
                    {
                        graphics.Color = Color.FromBrightness(1);
                        graphics.PaintSurface(r);
                        graphics.Color = Color.FromBrightness(0);
                        graphics.PaintOutline(r);
                    }

                    graphics.Color = style.FontColor;
                    graphics.PaintText(point.X - txtWidth / 2, point.Y, txt, style.Font, style.FontSize);
                }

                ++i;
            }

        }
    }
}
