//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	public sealed class DataNavigator
	{
		public DataNavigator(DataView view)
		{
			if (view == null)
			{
				throw new System.ArgumentNullException ();
			}

			this.view = view;
			this.itemStack = new Stack<ItemState> ();
			this.itemStack.Push (new ItemState (this.view.Item));
		}

		public bool IsValid
		{
			get
			{
				//	TODO: ...
				return true;
			}
		}

		private ItemState CurrentItemState
		{
			get
			{
				return this.itemStack.Peek ();
			}
		}

		public IDataItem CurrentDataItem
		{
			get
			{
				//	TODO: ...

				return this.CurrentItemState.Item;
			}
		}

		public IEnumerable<IDataItem> CurrentViewStack
		{
			get
			{
				foreach (var item in this.itemStack)
				{
					yield return item.Item;
				}
			}
		}

		public string CurrentDataPath
		{
			get
			{
				return this.CurrentItemState.Path;
			}
		}

		
		public void Reset()
		{
			this.itemStack.Clear ();
			this.itemStack.Push (this.CreateItemState (this.view.Item));
			
			this.currentSplitIndex = 0;
			this.currentSplitInfos = null;
		}

		private ItemState CreateItemState(DataView.DataItem dataItem)
		{
			return new ItemState (dataItem);
		}


		/// <summary>
		/// Navigates to the specified absolute path.
		/// </summary>
		/// <param name="path">The absolute path.</param>
		/// <returns><c>true</c> if the navigation succeeded.</returns>
		public bool NavigateTo(string path)
		{
			this.Reset ();

			DataView.GetDataItem (this.view, path, item => this.itemStack.Push (this.CreateItemState (item)));

			return this.IsValid;
		}

		/// <summary>
		/// Navigates to the specified relative path. This is only possible if
		/// the current data item is not a leaf of the graph.
		/// </summary>
		/// <param name="path">The relative path.</param>
		/// <returns><c>true</c> if the navigation succeeded.</returns>
		public bool NavigateToRelative(string path)
		{
			DataView.GetDataItem (this.itemStack.Peek ().Item.DataView, path, item => this.itemStack.Push (this.CreateItemState (item)));

			return this.IsValid;
		}
		
		public bool NavigateToNext()
		{
			if (this.IsValid)
			{
				//	TODO: ...
			}

			return this.IsValid;
		}

		public bool NavigateToPrevious()
		{
			if (this.IsValid)
			{
				//	TODO: ...
			}

			return this.IsValid;
		}

		public bool NavigateToFirstChild()
		{
			if (this.IsValid)
			{
				//	TODO: ...
			}

			return this.IsValid;
		}

		/// <summary>
		/// Navigates to the parent. This can be seen as a <c>Pop</c> of the last
		/// path element.
		/// </summary>
		/// <returns><c>true</c> if the navigation succeeded.</returns>
		public bool NavigateToParent()
		{
			if (this.itemStack.Count == 1)
			{
				return false;
			}

			System.Diagnostics.Debug.Assert (this.itemStack.Count > 1);

			ItemState state = this.itemStack.Pop ();

			return this.IsValid;
		}

		
		
		public void RequestBreak()
		{
			this.EnsureValid ();

			throw new System.NotImplementedException ();
		}

		public void RequestBreak(IEnumerable<CellSplitInfo> collection, int index)
		{
			this.EnsureValid ();

			this.currentSplitInfos = collection;
			this.currentSplitIndex = index;
		}


		private void EnsureValid()
		{
			if (this.IsValid)
			{
				return;
			}

			throw new System.InvalidOperationException ("Invalid state for navigator");
		}


		#region ItemState class

		/// <summary>
		/// The <c>ItemState</c> class wraps some logic around the data item,
		/// in order to handle a statefull navigation (i.e. to add synthetic
		/// items as required by the item type).
		/// </summary>
		private class ItemState
		{
			public ItemState(DataView.DataItem item)
			{
				this.item = item;
			}

			public DataView.DataItem Item
			{
				get
				{
					return this.item;
				}
			}

			public string Path
			{
				get
				{
					return DataView.GetPath (this.item);
				}
			}

			private readonly DataView.DataItem item;
		}

		#endregion


		private readonly DataView				view;
		private readonly Stack<ItemState>		itemStack;
	
		private IEnumerable<CellSplitInfo>		currentSplitInfos;
		private int								currentSplitIndex;
	}
}
