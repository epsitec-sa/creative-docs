using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Data;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.Graph.Styles;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Graph.Widgets
{
    public class SeriesCaptionsView : FrameBox
    {

        public AbstractRenderer Renderer { get; set; }

        protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
        {
            var style = new CaptionStyle();

            var i = 0;
            foreach (var item in Renderer.SeriesItems)
            {
                var position = Renderer.GetSeriesCaptionPosition(item, i);

                if (position.ShowCaption)
                {
                    var txt = DataCube.CleanUpLabel (item.Label);
                    var txtWidth = Renderer.Captions.Style.GetTextWidth (txt);
                    var txtHeight = Renderer.Captions.Style.GetTextLineHeight ();
                    var offset = Renderer.Captions.Style.GetTextLineOffset ();
                    var angle = position.Angle;

                    // Change orientation for quadrants II and III
                    if (angle > 90 && angle < 270)
                        angle -= 180;

                    // Rotate Graphics
                    graphics.Transform = graphics.Transform.RotateDeg (angle, position.Position);

                    // Create a border
                    using (var border = new Path (new Rectangle (position.Position.X - txtWidth / 2 - 2, position.Position.Y - offset.Y, txtWidth + 4, txtHeight)))
                    {
                        graphics.Color = Color.FromBrightness(1);
                        graphics.PaintSurface(border);
                        graphics.Color = Color.FromBrightness(0);
                        graphics.PaintOutline(border);
                    }

                    graphics.Color = style.FontColor;
                    graphics.PaintText (position.Position.X - txtWidth / 2, position.Position.Y, txt, style.Font, style.FontSize);

                    // Put back rotation
                    graphics.Transform = graphics.Transform.RotateDeg (-angle, position.Position);
                }

                ++i;
            }

        }
    }
}
