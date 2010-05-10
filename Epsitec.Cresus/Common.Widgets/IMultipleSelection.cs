//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	public interface IMultipleSelection : Collections.IStringCollectionHost, Support.Data.IKeyedStringSelection
	{
		int SelectionCount
		{
			get;
		}

		void AddSelection(IEnumerable<int> selection);
		void RemoveSelection(IEnumerable<int> selection);
		void ClearSelection();

		ICollection<int> GetSortedSelection();

		bool IsItemSelected(int index);
	}
}
