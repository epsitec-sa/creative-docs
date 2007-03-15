//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemViewWidget</c> class defines the root widgets which contain
	/// the graphic representation of the <see cref="ItemView"/> instances in
	/// a <see cref="ItemTable"/>.
	/// </summary>
	public class ItemViewWidget : Widgets.Widget
	{
		public ItemViewWidget(ItemView view)
		{
			this.view = view;
		}


		public ItemView ItemView
		{
			get
			{
				return this.view;
			}
		}

		protected ItemPanel GetParentPanel()
		{
			ItemPanel panel = this.Parent as ItemPanel;
			
			return panel;
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			ItemPanel panel = this.GetParentPanel ();
			
			Widgets.WidgetPaintState state   = this.PaintState;
			Widgets.IAdorner         adorner = Widgets.Adorners.Factory.Active;
			
			if (this.view.IsSelected)
			{
				state |= Widgets.WidgetPaintState.Selected;
			}
			if (panel != null)
			{
				object currentItem = panel.RootPanel.Items.CurrentItem;
				
				if (panel.IsFocused)
				{
					if (currentItem == this.view.Item)
					{
						state |= Widgets.WidgetPaintState.Focused;
					}
					
					state |= Widgets.WidgetPaintState.InheritedFocus;
				}
			}

			adorner.PaintCellBackground (graphics, this.Client.Bounds, state);
		}

		private ItemView view;
	}
}
