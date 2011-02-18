//	Copyright © 2004-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// The <c>IKeyedStringSelection</c> interface provides the basic mechanism used
	/// to select an item in a list, using either its index or its value.
	/// </summary>
	public interface IStringSelection
	{
		/// <summary>
		/// Gets or sets the index of the selected item.
		/// </summary>
		/// <value>The index of the selected item (<c>-1</c> means no item is selected).</value>
		int SelectedItemIndex
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the selected item text value.
		/// </summary>
		/// <value>The selected item text value.</value>
		string SelectedItem
		{
			get;
			set;
		}

		event EventHandler	SelectedItemChanged;
	}
}
