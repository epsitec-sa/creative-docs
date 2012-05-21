using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Data;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.Graph.Styles;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Graph.Widgets
{
    public class SeriesCaptionsView : Widget
    {

        public AbstractRenderer Renderer { get; set; }

        protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
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

                    var boxPos = new Point ();
                    var txtPos = new Point ();

                    // Compute x
                    switch (position.Alignment)
                    {
                        case ContentAlignment.BottomCenter:
                        case ContentAlignment.MiddleCenter:
                        case ContentAlignment.TopCenter:
                            txtPos.X = position.Position.X - txtWidth / 2;
                            boxPos.X = txtPos.X - SeriesCaptionsView.horizontalOffset;
                            break;

                        case ContentAlignment.BottomRight:
                        case ContentAlignment.MiddleRight:
                        case ContentAlignment.TopRight:
                            txtPos.X = position.Position.X - txtWidth - SeriesCaptionsView.horizontalOffset;
                            boxPos.X = txtPos.X - SeriesCaptionsView.horizontalOffset;
                            break;

                        case ContentAlignment.BottomLeft:
                        case ContentAlignment.MiddleLeft:
                        case ContentAlignment.TopLeft:
                        default:
                            boxPos.X = position.Position.X;
                            txtPos.X = boxPos.X + SeriesCaptionsView.horizontalOffset;
                            break;
                    }

                    // Compute y
                    switch (position.Alignment)
                    {
                        case ContentAlignment.MiddleLeft:
                        case ContentAlignment.MiddleCenter:
                        case ContentAlignment.MiddleRight:
                            boxPos.Y = position.Position.Y - txtHeight / 2;
                            txtPos.Y = boxPos.Y + offset.Y;
                            break;

                        case ContentAlignment.TopLeft:
                        case ContentAlignment.TopCenter:
                        case ContentAlignment.TopRight:
                            boxPos.Y = position.Position.Y - txtHeight;
                            txtPos.Y = boxPos.Y + offset.Y;
                            break;

                        case ContentAlignment.BottomLeft:
                        case ContentAlignment.BottomCenter:
                        case ContentAlignment.BottomRight:
                        default:
                            boxPos.Y = position.Position.Y;
                            txtPos.Y = boxPos.Y + offset.Y;
                            break;
                    }

                    // Rotate Graphics
                    graphics.Transform = graphics.Transform.RotateDeg (angle, position.Position);

                    // Create a border
                    using (var border = new Path (new Rectangle (boxPos.X, boxPos.Y, txtWidth + SeriesCaptionsView.horizontalOffset * 2, txtHeight)))
                    {
                        graphics.Color = Color.FromBrightness(1);
                        graphics.PaintSurface(border);
                        graphics.Color = Color.FromBrightness(0);
                        graphics.PaintOutline(border);
                    }

                    graphics.Color = style.FontColor;
                    graphics.PaintText (txtPos.X, txtPos.Y, txt, style.Font, style.FontSize);

                    // Put back rotation
                    graphics.Transform = graphics.Transform.RotateDeg (-angle, position.Position);
                }

                ++i;
            }

        }

        private static readonly int horizontalOffset = 2;
    }
}
