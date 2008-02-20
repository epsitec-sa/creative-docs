//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemPanelSelectionChanged</c> class contains the list of selected
	/// and deselected items; it also provides a <c>Cancel</c> property which can
	/// be used to cancel the operation.
	/// </summary>
	public class ItemPanelSelectionChangedEventArgs : Support.CancelEventArgs
	{
		public ItemPanelSelectionChangedEventArgs(IList<ItemView> selected, IList<ItemView> deselected)
		{
			this.selected = new ReadOnlyList<ItemView> (new List<ItemView> (selected));
			this.deselected = new ReadOnlyList<ItemView> (new List<ItemView> (deselected));
		}

		public ReadOnlyList<ItemView> SelectedItems
		{
			get
			{
				return this.selected;
			}
		}

		public ReadOnlyList<ItemView> DeselectedItems
		{
			get
			{
				return this.deselected;
			}
		}

		private ReadOnlyList<ItemView> selected;
		private ReadOnlyList<ItemView> deselected;
	}
}
