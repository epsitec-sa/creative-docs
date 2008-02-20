//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// La classe ItemViewText représente du texte non éditable. Si un parent de cet objet
	/// est de type ItemViewWidget, le texte adopte l'aspect sélectionné de ce parent.
	/// Cet objet sert à afficher des textes dans l'objet UI.ItemTable.
	/// </summary>
	public class ItemViewText : StaticText
	{
		public ItemViewText()
		{
		}
		
		public ItemViewText(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		public ItemViewText(string text) : this ()
		{
			this.Text = text;
		}
		
		public ItemViewText(Widget embedder, string text) : this (embedder)
		{
			this.Text = text;
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState  state = this.PaintState;
			Drawing.Point     pos   = Drawing.Point.Zero;
			
			if (this.BackColor.IsVisible)
			{
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.BackColor);
			}
			
			if (this.TextLayout != null)
			{
				IAdorner adorner = Widgets.Adorners.Factory.Active;

				ItemViewWidget item = this.SearchItemViewParent();
				if (item != null && item.ItemView.IsSelected)
				{
					state |= WidgetPaintState.Selected;
				}

				adorner.PaintGeneralTextLayout (graphics, clipRect, pos, this.TextLayout, state, this.paintTextStyle, TextFieldDisplayMode.Default, this.BackColor);
			}
		}

		protected ItemViewWidget SearchItemViewParent()
		{
			//	Retourne le premier parent de type ItemViewWidget.
			Widgets.Widget widget = this;

			while (widget.Parent != null)
			{
				widget = widget.Parent;

				if (widget is ItemViewWidget)
				{
					return widget as ItemViewWidget;
				}
			}

			return null;
		}
	}
}
