using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	public interface IMultipleSelection
	{
		int SelectionCount { get; }
		void AddSelection(IEnumerable<int> selection);
		void RemoveSelection(IEnumerable<int> selection);
		void ClearSelection();
		ICollection<int> GetSortedSelection();
		bool IsItemSelected(int index);
	}
}
