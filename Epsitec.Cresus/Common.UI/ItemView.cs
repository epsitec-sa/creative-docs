//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		/// <param name="owner">The owner panel.</param>
		/// <param name="defaultSize">Default size for this item view.</param>
		public ItemView(object item, ItemPanel owner, Drawing.Size defaultSize)
		{
			this.item  = item;
			this.owner = owner;
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


		public int RowIndex
		{
			get
			{
				return this.rowIndex;
			}
			internal set
			{
				this.rowIndex = value;
			}
		}

		public int ColumnIndex
		{
			get
			{
				return this.columnIndex;
			}
			internal set
			{
				this.columnIndex = value;
			}
		}

		public ItemPanel Owner
		{
			get
			{
				return this.owner;
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
					if ((this.owner != null) &&
						(this.owner.CustomItemViewFactoryGetter != null))
					{
						this.factory = this.owner.CustomItemViewFactoryGetter (this);
					}

					if (this.factory == null)
					{
						this.factory = ItemViewFactories.Factory.GetItemViewFactory (this);
					}
				}

				return this.factory;
			}
		}

		/// <summary>
		/// Gets the widget which represents this item view.
		/// </summary>
		/// <value>The widget or <c>null</c>.</value>
		public ItemViewWidget Widget
		{
			get
			{
				return this.widget;
			}
		}

		/// <summary>
		/// Gets the widget which represents the group associated with this
		/// item view.
		/// </summary>
		/// <value>The widget or <c>null</c>.</value>
		public ItemPanelGroup Group
		{
			get
			{
				return this.widget as ItemPanelGroup;
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

		public Drawing.Rectangle FocusBounds
		{
			get
			{
				ItemPanelGroup group = this.Group;

				Drawing.Rectangle bounds = this.Bounds;
				
				if (group != null)
				{
					Drawing.Point offset;

					offset = bounds.Location;
					bounds = group.FocusBounds;

					bounds.Offset (offset);
				}
				
				return bounds;
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
				if (this.widget == null)
				{
					return false;
				}
				else
				{
					return this.Bounds.IntersectsWith (this.owner.Aperture);
				}
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

					ItemPanelGroup group = this.Group;
					
					if (group != null)
					{
						group.SetPanelIsExpanded (this.isExpanded);
						group.NotifyItemViewChanged (this);
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this item view represents a group.
		/// </summary>
		/// <value><c>true</c> if this item view represents a group; otherwise, <c>false</c>.</value>
		public bool IsGroup
		{
			get
			{
				return this.item is CollectionViewGroup;
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
		internal bool HasValidUserInterface
		{
			get
			{
				if (this.isCleared)
				{
					return false;
				}
				if (this.widget == null)
				{
					return false;
				}

				ItemPanelGroup group = this.Group;

				if (group == null)
				{
					return true;
				}
				else
				{
					return group.HasValidUserInterface;
				}
			}
		}

		/// <summary>
		/// Updates the size of this item view.
		/// </summary>
		internal void UpdateSize()
		{
			IItemViewFactory factory = this.Factory;
			
			if (factory != null)
			{
				Drawing.Size oldSize = this.size;
				Drawing.Size newSize = factory.GetPreferredSize (this);

				if (oldSize != newSize)
				{
					this.DefineSize (newSize);
					this.owner.NotifyItemViewSizeChanged (this, oldSize, newSize);
				}
			}
		}

		/// <summary>
		/// Defines the size of this item view.
		/// </summary>
		/// <param name="size">The size.</param>
		internal void DefineSize(Drawing.Size size)
		{
			this.size = size;
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
					this.widget.NotifyItemViewIsSelectedChanged ();
				}
			}
		}

		/// <summary>
		/// Creates the user interface for this item view.
		/// </summary>
		internal void CreateUserInterface()
		{
			if (this.widget == null)
			{
				IItemViewFactory factory = this.Factory;

				if (factory != null)
				{
					this.widget = factory.CreateUserInterface (this);
				}
			}
			
			if (this.widget != null)
			{
				if (!this.HasValidUserInterface)
				{
					this.isCleared = false;

					ItemPanelGroup group = this.Group;

					if (group != null)
					{
						group.RefreshUserInterface ();
					}
				}
				
				//-System.Diagnostics.Debug.WriteLine ("Created " + this.index + "/" + this.GetCollectionIndex () + " -> " + this.widget.ToString () + " in " + this.owner.GetVisualSerialId ());

				this.widget.SetEmbedder (this.owner);
				this.widget.SetManualBounds (this.bounds);

				if ((this.owner.GetFocusedItemView () == this) &&
					(this.owner.ContainsKeyboardFocus))
				{
					//-System.Diagnostics.Debug.WriteLine ("Refocus " + this.widget);
					this.widget.Focus ();
				}
			}
		}

		/// <summary>
		/// Defines the widget used to represent the associated group. The widget
		/// might only be partially initialized and will still need to go through
		/// the full <c>CreateUserInterface</c> procedure.
		/// </summary>
		/// <param name="group">The widget used to represent the group.</param>
		internal void DefineGroup(ItemPanelGroup group)
		{
			System.Diagnostics.Debug.Assert (group != null);

			if (this.widget != group)
			{
				System.Diagnostics.Debug.Assert (this.widget == null);
				
				this.widget = group;
			}
			
			if (this.widget != null)
			{
				//-System.Diagnostics.Debug.WriteLine ("Defining " + this.index + "/" + this.GetCollectionIndex () + " -> " + this.widget.ToString () + " in " + this.owner.ToString ());
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
				//-System.Diagnostics.Debug.WriteLine ("Clearing " + this.index + "/" + this.GetCollectionIndex () + " -> " + this.widget.ToString () + " in " + this.owner.ToString ());
				
				ItemPanelGroup group = this.Group;

				if (group == null)
				{
					this.factory.DisposeUserInterface (this.widget);

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
			//-System.Diagnostics.Debug.WriteLine ("Disposing " + this.index + "/" + this.GetCollectionIndex () + " -> " + this.widget.ToString () + " in " + this.owner.ToString ());

			if (this.widget != null)
			{
				this.factory.DisposeUserInterface (this.widget);

				this.widget    = null;
				this.isCleared = false;
			}
		}

		/// <summary>
		/// Gets the index in the global collection for the item represented by
		/// this view.
		/// </summary>
		/// <returns>The index.</returns>
		internal int GetCollectionIndex()
		{
			return this.owner.RootPanel.Items.Items.IndexOf (this.item);
		}

		private ItemPanel owner;
		private object item;
		private int index;
		private int rowIndex;
		private int columnIndex;
		private IItemViewFactory factory;
		private ItemViewWidget widget;
		private Drawing.Size size;
		private Drawing.Rectangle bounds;
		private bool isSelected;
		private bool isExpanded;
		private bool isCleared;
	}
}
