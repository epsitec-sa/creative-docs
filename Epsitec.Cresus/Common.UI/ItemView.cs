//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	public class ItemView
	{
		public ItemView(object item, Drawing.Size defaultSize)
		{
			this.item  = item;
			this.size  = defaultSize;
			this.index = -1;
		}

		public object Item
		{
			get
			{
				return this.item;
			}
		}

		public int Index
		{
			get
			{
				return this.index;
			}
			internal set
			{
				this.index = value;
			}
		}

		public IItemViewFactory Factory
		{
			get
			{
				return this.factory;
			}
			internal set
			{
				this.factory = value;
			}
		}

		public Widgets.Widget Widget
		{
			get
			{
				return this.widget;
			}
		}

		public Drawing.Size Size
		{
			get
			{
				return this.size;
			}
		}

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

		public bool IsSelected
		{
			get
			{
				return this.isSelected;
			}
			internal set
			{
				this.isSelected = value;
			}
		}

		public bool IsVisible
		{
			get
			{
				return this.widget != null;
			}
		}

		internal void UpdatePreferredSize(ItemPanel panel)
		{
			if (this.factory == null)
			{
				this.factory = ItemViewFactories.Factory.GetItemViewFactory (this);
			}

			if (this.factory != null)
			{
				Drawing.Size oldSize = this.size;
				Drawing.Size newSize = this.factory.GetPreferredSize (panel, this);

				if (oldSize != newSize)
				{
					this.size = newSize;
					panel.NotifyItemViewSizeChanged (this, oldSize, newSize);
				}
			}
		}
		
		internal void CreateUserInterface(ItemPanel panel)
		{
			if (this.widget == null)
			{
				if (this.factory == null)
				{
					this.factory = ItemViewFactories.Factory.GetItemViewFactory (this);
				}
				
				if (this.factory != null)
				{
					this.widget = this.factory.CreateUserInterface (panel, this);
				}
			}
			if (this.widget != null)
			{
				this.widget.SetEmbedder (panel);
				this.widget.SetManualBounds (this.bounds);
			}
		}

		internal void DisposeUserInterface()
		{
			if (this.widget != null)
			{
				this.widget.Dispose ();
				this.widget = null;
			}
		}

		private object item;
		private int index;
		private IItemViewFactory factory;
		private Widgets.Widget widget;
		private Drawing.Size size;
		private Drawing.Rectangle bounds;
		private bool isSelected;
		private bool isDisabled;
		private bool isExpanded;
	}
}
