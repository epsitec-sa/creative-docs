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

		public IDataItem CurrentDataItem
		{
			get
			{
				//	TODO: ...

				return this.itemStack.Peek ().Item;
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
				return DataView.GetPath (this.itemStack.Peek ().Item);
			}
		}

		
		public void Reset()
		{
			this.itemStack.Clear ();
			this.itemStack.Push (new ItemState (this.view.Item));
			
			this.currentSplitIndex = 0;
			this.currentSplitInfos = null;
		}

		
		public bool NavigateTo(string path)
		{
			this.Reset ();

			DataView.GetDataItem (this.itemStack.Peek ().Item.DataView, path, item => this.itemStack.Push (new ItemState (item)));

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

			private readonly DataView.DataItem item;
		}


		private readonly DataView view;
		private readonly Stack<ItemState> itemStack;
	
		private IEnumerable<CellSplitInfo> currentSplitInfos;
		private int currentSplitIndex;
	}
}
