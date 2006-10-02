//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>CollectionChangedEventArgs</c> provides data for the <see cref="INotifyCollectionChanged.CollectionChanged"/>
	/// event.
	/// </summary>
	public class CollectionChangedEventArgs : System.EventArgs
	{
		public CollectionChangedEventArgs(CollectionChangedAction action)
		{
			this.action = action;
			this.newItems = null;
			this.oldItems = null;
			this.newStartingIndex = 0;
			this.oldStartingIndex = 0;
		}

		public CollectionChangedEventArgs(CollectionChangedAction action, int newIndex, object[] newItems)
		{
			this.action = action;
			this.newItems = newItems;
			this.oldItems = null;
			this.newStartingIndex = newIndex+1;
			this.oldStartingIndex = 0;
		}

		public CollectionChangedEventArgs(CollectionChangedAction action, int oldIndex, object[] oldItems, int newIndex, object[] newItems)
		{
			this.action = action;
			this.newItems = newItems;
			this.oldItems = oldItems;
			this.newStartingIndex = newIndex+1;
			this.oldStartingIndex = oldIndex+1;
		}

		
		public CollectionChangedAction			Action
		{
			get
			{
				return this.action;
			}
		}

		public System.Collections.IList			NewItems
		{
			get
			{
				return this.newItems;
			}
		}

		public System.Collections.IList			OldItems
		{
			get
			{
				return this.oldItems;
			}
		}

		public int								NewStartingIndex
		{
			get
			{
				return this.newStartingIndex-1;
			}
		}

		public int								OldStartingIndex
		{
			get
			{
				return this.oldStartingIndex-1;
			}
		}
		
		
		private CollectionChangedAction			action;
		private object[]						newItems;
		private object[]						oldItems;
		private int								newStartingIndex;
		private int								oldStartingIndex;
	}
}
