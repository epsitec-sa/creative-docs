using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.MyWidgets
{
	/// <summary>
	/// Dessine un trait noir.
	/// </summary>
	public class Line : Widget
	{
		public Line() : base()
		{
		}

		public Line(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle rect = this.Client.Bounds;

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(Color.FromBrightness(0));
		}

	}
}
