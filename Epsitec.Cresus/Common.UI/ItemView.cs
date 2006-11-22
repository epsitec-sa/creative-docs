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
				this.bounds = value;
			}
		}


		private object item;
		private int index;
		private Widgets.Widget widget;
		private Drawing.Size size;
		private Drawing.Rectangle bounds;
		private bool isSelected;
		private bool isDisabled;
		private bool isExpanded;
		private bool isVisible;
	}
}
