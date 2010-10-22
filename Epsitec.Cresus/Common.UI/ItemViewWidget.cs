//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			this.InternalState |= Widgets.WidgetInternalState.Focusable;

			this.AddEventHandler (Widgets.Visual.KeyboardFocusProperty, this.HandleKeyboardFocusChanged);
		}


		/// <summary>
		/// Gets the item view which is represented by this widget.
		/// </summary>
		/// <value>The item view.</value>
		public ItemView ItemView
		{
			get
			{
				return this.view;
			}
		}

		/// <summary>
		/// Gets the parent panel (the one which hosts the widget).
		/// </summary>
		/// <value>The parent panel.</value>
		public ItemPanel ParentPanel
		{
			get
			{
				return this.view.Owner;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this widget is passive.
		/// A passive widget won't display any visual feed-back when it is
		/// entered, engaged or selected.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this widget is passive; otherwise, <c>false</c>.
		/// </value>
		public bool IsPassive
		{
			get
			{
				return this.isPassive;
			}
			set
			{
				if (this.isPassive != value)
				{
					this.isPassive = value;
					this.Invalidate ();
				}
			}
		}

		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			ItemPanel panel = this.ParentPanel;
			
			Widgets.WidgetPaintState state   = this.PaintState;
			Widgets.IAdorner         adorner = Widgets.Adorners.Factory.Active;

			if (this.IsPassive)
			{
				state &= ~Epsitec.Common.Widgets.WidgetPaintState.Engaged;
				state &= ~Epsitec.Common.Widgets.WidgetPaintState.Entered;
				state &= ~Epsitec.Common.Widgets.WidgetPaintState.Selected;
				state &= ~Epsitec.Common.Widgets.WidgetPaintState.Focused;
			}
			else
			{
				if (this.view.IsSelected)
				{
					state |= Widgets.WidgetPaintState.Selected;
				}
				if (panel != null)
				{
					if (panel.RootPanel.ContainsKeyboardFocus)
					{
						state |= Widgets.WidgetPaintState.InheritedFocus;
					}
				}
			}

			adorner.PaintCellBackground (graphics, this.Client.Bounds, state);
		}

		protected override void ProcessMessage(Widgets.Message message, Drawing.Point pos)
		{
			base.ProcessMessage (message, pos);

			ItemPanel panel = this.ParentPanel;
			
			if (message.IsMouseType)
			{
				if (message.Button == Widgets.MouseButtons.Left)
				{
					switch (message.MessageType)
					{
						case Widgets.MessageType.MouseDown:
							message.Consumer = this;
							panel.NotifyWidgetPressed (this, message, pos);
							break;

						case Epsitec.Common.Widgets.MessageType.MouseUp:
							message.Consumer = this;
							break;
					}
				}
			}
		}

		protected override void AboutToBecomeOrphan()
		{
			base.AboutToBecomeOrphan ();
			this.AboutToLoseFocus ();
		}

		protected virtual void AboutToLoseFocus()
		{
			if (this.KeyboardFocus)
			{
				ItemPanel panel = this.ParentPanel;

				if (panel != null)
				{
					panel.NotifyFocusChanged (this, false);
				}
			}
		}

		internal void NotifyItemViewIsSelectedChanged()
		{
			bool newValue = this.ItemView.IsSelected;
			bool oldValue = !newValue;

			this.InvalidateProperty (Widgets.Visual.SelectedProperty, oldValue, newValue);
			this.Invalidate ();
		}
		
		private void HandleKeyboardFocusChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			bool focus = (bool) e.NewValue;

			//-System.Diagnostics.Debug.WriteLine ("Focus=" + focus.ToString () + ", " + this.ToString ());

			if (focus)
			{
				ItemPanel panel = this.ParentPanel;

				if (panel != null)
				{
					panel.NotifyFocusChanged (this, focus);
				}
			}
			else
			{
				this.AboutToLoseFocus ();
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
		private bool isPassive;

	}
}
