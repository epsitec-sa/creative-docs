//	Copyright © 2008-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La class ConfirmationButton représente un bouton pour le dialogue ConfirmationDialog.
	/// </summary>
	public class ConfirmationButton : Button
	{
		public ConfirmationButton()
		{
			this.ButtonStyle = ButtonStyle.Confirmation;
			this.ContentAlignment = Drawing.ContentAlignment.MiddleLeft;
		}
		
		public ConfirmationButton(string text)
		{
			this.Text = text;
		}
		
		public ConfirmationButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public static FormattedText FormatContent(FormattedText subtitle, FormattedText text)
		{
			//	Formate un texte pour le bouton.
			//	La première partie contient le titre en gros caractère.
			//	La deuxième partie contient le texte explicatif en taille standard.
			return FormattedText.Concat (new FormattedText ("<font size=\"150%\">"), subtitle, new FormattedText ("</font><br/>"), text);
		}

		public static string FormatContent(string subtitle, string text)
		{
			//	Formate un texte pour le bouton.
			//	La première partie contient le titre en gros caractère.
			//	La deuxième partie contient le texte explicatif en taille standard.
			return string.Concat("<font size=\"150%\">", subtitle, "</font><br/>", text);
		}
		
		
		protected override Drawing.Size GetTextLayoutSize()
		{
			Drawing.Size size = this.IsActualGeometryValid ? this.Client.Size : this.PreferredSize;
			size.Width -= ConfirmationButton.marginLeft+ConfirmationButton.marginRight;
			size.Height -= ConfirmationButton.marginY*2;
			return size;
		}

		protected override Drawing.Point GetTextLayoutOffset()
		{
			return new Drawing.Point(ConfirmationButton.marginLeft, ConfirmationButton.marginY);
		}

		protected override void OnSizeChanged(Drawing.Size oldValue, Drawing.Size newValue)
		{
			base.OnSizeChanged(oldValue, newValue);

			if (oldValue.Width != newValue.Width)  // largeur changée ?
			{
				double h = this.TextLayout.FindTextHeight();
				this.PreferredHeight = h+ConfirmationButton.marginY*2+2;  // TODO: pourquoi +2 ?
			}
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine le bouton.
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState  state = this.GetPaintState ();
			Drawing.Point     pos   = this.GetTextLayoutOffset();
			
			if ( (state & WidgetPaintState.Enabled) == 0 )
			{
				state &= ~WidgetPaintState.Focused;
				state &= ~WidgetPaintState.Entered;
				state &= ~WidgetPaintState.Engaged;
			}
			
			if ( this.BackColor.IsTransparent )
			{
				//	Ne peint pas le fond du bouton si celui-ci a un fond explicitement défini
				//	comme "transparent".
			}
			else
			{
				//	Ne reproduit pas l'état sélectionné si on peint nous-même le fond du bouton.
				state &= ~WidgetPaintState.Selected;
				adorner.PaintButtonBackground(graphics, rect, state, Direction.Down, this.ButtonStyle);
			}

			pos.Y += this.GetBaseLineVerticalOffset ();
			adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, this.ButtonStyle);

			double lineY = System.Math.Floor (pos.Y + this.TextLayout.GetLineOrigin (0).Y);

			//	Dessine un petit ">" positionné de façon empyrique.
			rect = new Drawing.Rectangle(rect.Left+2, /*rect.Top-20-13*/ lineY-5, 20, 20);
			adorner.PaintGlyph(graphics, rect, state, adorner.ColorCaption, GlyphShape.TriangleRight, PaintTextStyle.StaticText);
		}


		protected static readonly double marginLeft = 30;
		protected static readonly double marginRight = 10;
		protected static readonly double marginY = 10;
	}
}
