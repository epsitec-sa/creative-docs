//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public sealed class ItemListColumn
	{
		public ItemListColumn()
		{
			this.layout = new ColumnLayoutInfo ();
		}
		
		
		public ColumnLayoutInfo Layout
		{
			get
			{
				return this.layout;
			}
		}

		public FormattedText Title
		{
			get;
			set;
		}

		public Caption Caption
		{
			get;
			set;
		}

		public ItemSortOrder SortOrder
		{
			get;
			set;
		}

		public int Index
		{
			get;
			set;
		}


		private readonly ColumnLayoutInfo		layout;
	}
}