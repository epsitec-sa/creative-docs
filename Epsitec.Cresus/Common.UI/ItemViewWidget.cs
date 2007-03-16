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
			this.InternalState |= Widgets.InternalState.Focusable;
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

		protected override void ProcessMessage(Widgets.Message message, Drawing.Point pos)
		{
			base.ProcessMessage (message, pos);
			
			if (message.IsMouseType)
			{
				if (message.MessageType == Widgets.MessageType.MouseDown)
				{
					this.Focus ();
				}
			}
		}

		protected override bool AboutToGetFocus(Widgets.TabNavigationDir dir, Widgets.TabNavigationMode mode, out Widgets.Widget focus)
		{
			if (base.AboutToGetFocus (dir, mode, out focus))
			{
				ItemPanel panel = this.GetParentPanel ();

				focus = panel.NotifyWidgetAboutToGetFocus (focus);

				return true;
			}
			else
			{
				return false;
			}
		}

		internal static ItemView FindItemView(Widgets.Widget widget)
		{
			while (widget != null)
			{
				ItemViewWidget view = widget as ItemViewWidget;

				if (view != null)
				{
					return view.ItemView;
				}

				widget = widget.Parent;
			}

			return null;
		}

		internal static ItemView FindParentItemView(Widgets.Widget widget)
		{
			bool skip = true;

			if (widget != null)
			{
				ItemViewWidget view = widget as ItemViewWidget;

				if (view != null)
				{
					if (skip)
					{
						skip = false;
					}
					else
					{
						return view.ItemView;
					}
				}

				widget = widget.Parent;
			}

			return null;
		}

		internal static ItemView FindGroupItemView(ItemView itemView)
		{
			if (itemView != null)
			{
				return ItemViewWidget.FindGroupItemView (itemView.Widget);
			}
			else
			{
				return null;
			}
		}

		internal static ItemView FindGroupItemView(Widgets.Widget widget)
		{
			while (widget != null)
			{
				ItemViewWidget view = widget as ItemViewWidget;

				if (view != null)
				{
					ItemView itemView = view.ItemView;

					if ((itemView != null) &&
						(itemView.IsGroup))
					{
						return itemView;
					}
				}

				widget = widget.Parent;
			}

			return null;
		}

		private ItemView view;
	}
}
