//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (FormTextField))]

namespace Epsitec.Common.Widgets
{
	public class FormTextField : TextField
	{
		public FormTextField()
		{
			this.TextFieldStyle = TextFieldStyle.Flat;
			this.BackColor = Drawing.Color.Transparent;
		}
		
		private Drawing.TextStyle DefaultTextStyle
		{
			get
			{
				if (FormTextField.defaultStyle == null)
				{
					FormTextField.defaultStyle = new Drawing.TextStyle ();

					FormTextField.defaultStyle.Font = Drawing.Font.GetFont ("Calibri", "Regular") ?? Drawing.Font.DefaultFont;
					FormTextField.defaultStyle.FontSize = 16.0;
				}

				return FormTextField.defaultStyle;
			}
		}


		protected override void CreateTextLayout()
		{
			base.CreateTextLayout ();

			this.TextLayout.Style = this.DefaultTextStyle;
		}
		

		protected override void PaintTextFieldBackground(Drawing.Graphics graphics, IAdorner adorner, WidgetPaintState state, Drawing.Rectangle fill, Drawing.Point pos)
		{
			base.PaintTextFieldBackground (graphics, adorner, state, fill, pos);

			double y = this.GetBaseLine ().Y - 1.5;
			
			using (Drawing.Path path = Drawing.Path.FromLine (fill.Left, y, fill.Right, y))
			{
				graphics.Rasterizer.AddOutline (path, 0.5, Epsitec.Common.Drawing.CapStyle.Butt, Epsitec.Common.Drawing.JoinStyle.Round);
				graphics.RenderSolid (Drawing.Color.FromAlphaRgb (0.5, 0.2, 0.2, 0.5));
			}
		}
		
		private static Drawing.TextStyle defaultStyle;
	}
}
