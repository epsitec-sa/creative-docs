//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemView</c> class defines a view of an (untyped) item in an
	/// <see cref="ItemPanel"/>.
	/// </summary>
	public class ItemView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ItemView"/> class.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="defaultSize">Default size for this item view.</param>
		public ItemView(object item, Drawing.Size defaultSize)
		{
			this.item  = item;
			this.size  = defaultSize;
			this.index = -1;
		}

		/// <summary>
		/// Gets the item represented by this item view.
		/// </summary>
		/// <value>The item.</value>
		public object Item
		{
			get
			{
				return this.item;
			}
		}

		/// <summary>
		/// Gets the index of this item view in its <see cref="ItemPanel"/>.
		/// </summary>
		/// <value>The index or <c>-1</c>.</value>
		public int Index
		{
			get
			{
				return this.index;
			}
		}

		/// <summary>
		/// Gets the factory which is used to create the user interface for
		/// this item view.
		/// </summary>
		/// <value>The factory.</value>
		public IItemViewFactory Factory
		{
			get
			{
				if (this.factory == null)
				{
					this.factory = ItemViewFactories.Factory.GetItemViewFactory (this);
				}

				return this.factory;
			}
		}

		/// <summary>
		/// Gets the widget which represents this item view.
		/// </summary>
		/// <value>The widget or <c>null</c>.</value>
		public Widgets.Widget Widget
		{
			get
			{
				return this.widget;
			}
		}

		/// <summary>
		/// Gets the size of this item view.
		/// </summary>
		/// <value>The size of the item view.</value>
		public Drawing.Size Size
		{
			get
			{
				return this.size;
			}
		}

		/// <summary>
		/// Gets or sets the bounds of this item view. The coordinate system is
		/// relative to the containing <see cref="ItemPanel"/>.
		/// </summary>
		/// <value>The bounds.</value>
		public Drawing.Rectangle Bounds
		{
			get
			{
				return this.bounds;
			}
			set
			{
				if (this.bounds != value)
				{
					this.bounds = value;
					
					if (this.widget != null)
					{
						this.widget.SetManualBounds (this.bounds);
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this item view is selected.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this item view is selected; otherwise, <c>false</c>.
		/// </value>
		public virtual bool IsSelected
		{
			get
			{
				return this.isSelected;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this item view is visible in the
		/// containing <see cref="ItemPanel"/>.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this item view is visible; otherwise, <c>false</c>.
		/// </value>
		public bool IsVisible
		{
			get
			{
				return this.widget != null;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this item view is expanded.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this item view is expanded; otherwise, <c>false</c>.
		/// </value>
		public bool IsExpanded
		{
			get
			{
				return this.isExpanded;
			}
			internal set
			{
				if (this.isExpanded != value)
				{
					this.isExpanded = value;

					ItemPanelGroup group = this.widget as ItemPanelGroup;
					
					if (group != null)
					{
						group.NotifyItemViewChanged (this);
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this item view has a dirty user
		/// interface. If this is the case, then <c>CreateUserInterface</c>
		/// must be called to recreate the user interface.
		/// </summary>
		/// <value>
		///	<c>true</c> if this item view has a dirty user interface;
		/// otherwise, <c>false</c>.
		/// </value>
		internal bool IsUserInterfaceDirty
		{
			get
			{
				return this.isCleared;
			}
		}

		/// <summary>
		/// Updates the size of this item view.
		/// </summary>
		/// <param name="panel">The panel.</param>
		internal void UpdateSize(ItemPanel panel)
		{
			IItemViewFactory factory = this.Factory;
			
			if (factory != null)
			{
				this.DefineSize (factory.GetPreferredSize (panel, this), panel);
			}
		}

		/// <summary>
		/// Defines the size of this item view.
		/// </summary>
		/// <param name="size">The size.</param>
		/// <param name="panel">The containing <see cref="ItemPanel"/>.</param>
		internal void DefineSize(Drawing.Size size, ItemPanel panel)
		{
			Drawing.Size oldSize = this.size;
			Drawing.Size newSize = size;

			if (oldSize != newSize)
			{
				this.size = newSize;

				if (panel != null)
				{
					panel.NotifyItemViewSizeChanged (this, oldSize, newSize);
				}
			}
		}

		/// <summary>
		/// Defines the index of this item view.
		/// </summary>
		/// <param name="index">The index.</param>
		internal void DefineIndex(int index)
		{
			this.index = index;
		}

		/// <summary>
		/// Selects or deselects this item view.
		/// </summary>
		/// <param name="value">Select this item view if <c>true</c>.</param>
		internal virtual void Select(bool value)
		{
			if (this.isSelected != value)
			{
				this.isSelected = value;

				if (this.widget != null)
				{
					this.widget.Invalidate ();
				}
			}
		}


		/// <summary>
		/// Creates the user interface for this item view.
		/// </summary>
		/// <param name="panel">The containing <see cref="ItemPanel"/>.</param>
		internal void CreateUserInterface(ItemPanel panel)
		{
			if (this.widget == null)
			{
				IItemViewFactory factory = this.Factory;
				
				if (factory != null)
				{
					this.widget = factory.CreateUserInterface (panel, this);
				}
			}
			if (this.widget != null)
			{
				if (this.isCleared)
				{
					this.isCleared = false;
					
					ItemPanelGroup group = this.widget as ItemPanelGroup;

					if (group != null)
					{
						group.RefreshUserInterface ();
					}
				}
				
				this.widget.SetEmbedder (panel);
				this.widget.SetManualBounds (this.bounds);
			}
		}

		/// <summary>
		/// Clears the user interface. This will either remove the associated
		/// widget or, if it is an <see cref="ItemPanelGroup"/>, then it will
		/// simply mark the user interface as being dirty.
		/// </summary>
		internal void ClearUserInterface()
		{
			if (this.widget != null)
			{
				ItemPanelGroup group = this.widget as ItemPanelGroup;

				if (group == null)
				{
					this.factory.DisposeUserInterface (this, this.widget);
					
					this.widget    = null;
					this.isCleared = false;
				}
				else
				{
					group.ClearUserInterface ();
					this.isCleared = true;
				}
			}
		}

		/// <summary>
		/// Disposes the user interface.
		/// </summary>
		internal void DisposeUserInterface()
		{
			if (this.widget != null)
			{
				this.factory.DisposeUserInterface (this, this.widget);

				this.widget    = null;
				this.isCleared = false;
			}
		}

		private object item;
		private int index;
		private IItemViewFactory factory;
		private Widgets.Widget widget;
		private Drawing.Size size;
		private Drawing.Rectangle bounds;
		private bool isSelected;
		private bool isExpanded;
		private bool isCleared;
	}
}
