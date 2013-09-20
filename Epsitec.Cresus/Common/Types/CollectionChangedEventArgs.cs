//	Copyright © 2006-2013, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>CollectionChangedEventArgs</c> provides data for the <see cref="INotifyCollectionChanged.CollectionChanged"/>
	/// event.
	/// </summary>
	public class CollectionChangedEventArgs : System.EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CollectionChangedEventArgs"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		public CollectionChangedEventArgs(CollectionChangedAction action)
		{
			this.action = action;
			this.newItems = null;
			this.oldItems = null;
			this.newStartingIndex = 0;
			this.oldStartingIndex = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CollectionChangedEventArgs"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <param name="newIndex">The index of the new items.</param>
		/// <param name="newItems">The new items.</param>
		public CollectionChangedEventArgs(CollectionChangedAction action, int newIndex, object[] newItems = null)
		{
			this.action = action;
			this.newItems = newItems;
			this.oldItems = null;
			this.newStartingIndex = newIndex+1;
			this.oldStartingIndex = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CollectionChangedEventArgs"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <param name="newIndex">The index of the new items.</param>
		/// <param name="newItems">The new items.</param>
		/// <param name="oldIndex">The index of the old items.</param>
		/// <param name="oldItems">The old items.</param>
		public CollectionChangedEventArgs(CollectionChangedAction action, int newIndex, object[] newItems, int oldIndex, object[] oldItems)
		{
			this.action = action;
			this.newItems = newItems;
			this.oldItems = oldItems;
			this.newStartingIndex = newIndex+1;
			this.oldStartingIndex = oldIndex+1;
		}


		/// <summary>
		/// Gets the action that caused the event.
		/// </summary>
		/// <value>The action.</value>
		public CollectionChangedAction			Action
		{
			get
			{
				return this.action;
			}
		}

		/// <summary>
		/// Gets the list of new items involved in the change (<see cref="CollectionChangedAction.Add"/> and
		/// <see cref="CollectionChangedAction.Replace"/> actions).
		/// </summary>
		/// <value>The list of new items or <c>null</c>.</value>
		public System.Collections.IList			NewItems
		{
			get
			{
				return this.newItems;
			}
		}

		/// <summary>
		/// Gets the list of old items involved in the change (<see cref="CollectionChangedAction.Remove"/> and
		/// <see cref="CollectionChangedAction.Replace"/> actions).
		/// </summary>
		/// <value>The list of old items or <c>null</c>.</value>
		public System.Collections.IList			OldItems
		{
			get
			{
				return this.oldItems;
			}
		}

		/// <summary>
		/// Gets the index at which the changed occurred (destination index in the case of <see cref="CollectionChangedAction.Add"/>
		/// and <see cref="CollectionChangedAction.Replace"/> actions).
		/// </summary>
		/// <value>The index at which the changed occurred.</value>
		public int								NewStartingIndex
		{
			get
			{
				return this.newStartingIndex-1;
			}
		}

		/// <summary>
		/// Gets the index at which the changed occurred (source index in the case of <see cref="CollectionChangedAction.Move"/>,
		/// <see cref="CollectionChangedAction.Remove"/> and <see cref="CollectionChangedAction.Replace"/> actions).
		/// </summary>
		/// <value>The index at which the changed occurred.</value>
		public int								OldStartingIndex
		{
			get
			{
				return this.oldStartingIndex-1;
			}
		}


		public override string ToString()
		{
			switch (this.action)
			{
				case CollectionChangedAction.Reset:
					return string.Format ("{0}", this.action);

				case CollectionChangedAction.Add:
				case CollectionChangedAction.Replace:
					return string.Format ("{0} at {1}{2}", this.action, this.NewStartingIndex, this.GetCountSuffix (this.NewItems));
				
				case CollectionChangedAction.Remove:
					return string.Format ("{0} at {1}{2}", this.action, this.OldStartingIndex, this.GetCountSuffix (this.OldItems));

				case CollectionChangedAction.Move:
					return string.Format ("{0} from {1} to {2}{3}", this.action, this.OldStartingIndex, this.NewStartingIndex, this.GetCountSuffix (this.NewItems));

				default:
					throw new System.NotSupportedException ();
			}
		}


		/// <summary>
		/// Merges the specified change event data into a single one.
		/// </summary>
		/// <param name="a">The <see cref="Epsitec.Common.Types.CollectionChangedEventArgs"/> instance containing the event data.</param>
		/// <param name="b">The <see cref="Epsitec.Common.Types.CollectionChangedEventArgs"/> instance containing the event data.</param>
		/// <returns>The new event data.</returns>
		public static CollectionChangedEventArgs Merge(CollectionChangedEventArgs a, CollectionChangedEventArgs b)
		{
			return new CollectionChangedEventArgs (CollectionChangedAction.Reset);
		}


		private string GetCountSuffix(System.Collections.IList list)
		{
			if (list == null)
			{
				return "";
			}
			else
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, ", count={0}", list.Count);
			}
		}
		
		
		private readonly CollectionChangedAction action;
		private readonly object[]				newItems;
		private readonly object[]				oldItems;
		private readonly int					newStartingIndex;
		private readonly int					oldStartingIndex;
	}
}
