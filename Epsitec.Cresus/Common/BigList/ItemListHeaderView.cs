//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

[assembly: DependencyClass (typeof (ItemListHeaderView))]

namespace Epsitec.Common.BigList
{
	public class ItemListHeaderView : Widget
	{
		public ItemListHeaderView()
		{
			this.IsMasterView = true;
		}


		public bool								IsMasterView
		{
			get;
			set;
		}


		private void DefineColumnCollection(ItemListColumnCollection collection)
		{
			if (this.columns != null)
			{
				this.columns.CollectionChanged -= this.HandleColumnsCollectionChanged;
			}

			this.columns = collection;

			if (this.columns != null)
			{
				this.columns.CollectionChanged += this.HandleColumnsCollectionChanged;
			}
		}


		private void HandleColumnsCollectionChanged(object sender, CollectionChangedEventArgs e)
		{
			this.Invalidate ();

			if (this.IsMasterView)
			{
				this.RefreshColumnLayout ();
			}
		}


		private void RefreshColumnLayout()
		{
			double headerWidth = this.Client.Width;

			
		}


		private ItemListColumnCollection		columns;
	}
}