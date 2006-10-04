//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>ICollectionView</c> interface enables collections to have functionalities
	/// such as current record management, sorting, filtering and grouping.
	/// </summary>
	public interface ICollectionView
	{
		object CurrentItem
		{
			get;
		}

		int CurrentPosition
		{
			get;
		}

		/// <summary>
		/// Gets the top-level groups.
		/// </summary>
		/// <value>A read-only collection of the top-level groups or <c>null</c> if
		/// there are no groups.</value>
		Collections.ReadOnlyObservableList<string> Groups
		{
			get;
		}

		void Refresh();

		event Support.EventHandler CurrentChanged;
		event Support.EventHandler<CurrentChangingEventArgs> CurrentChanging;
	}
}
