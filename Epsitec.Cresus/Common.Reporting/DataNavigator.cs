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
			this.itemStack.Push (new ItemState (this.view.Item, this));
		}


		public DataViewContext					Context
		{
			get
			{
				return this.view.Context;
			}
		}

		private ItemState						CurrentItemState
		{
			get
			{
				return this.itemStack.Peek ();
			}
		}

		public IDataItem						CurrentDataItem
		{
			get
			{
				//	TODO: ...

				return this.CurrentItemState.Item;
			}
		}

		public IEnumerable<IDataItem>			CurrentViewStack
		{
			get
			{
				foreach (var item in this.itemStack)
				{
					yield return item.Item;
				}
			}
		}

		public string							CurrentDataPath
		{
			get
			{
				return this.CurrentItemState.Path;
			}
		}

		public bool								EnableSyntheticNodes
		{
			get;
			set;
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
			return new ItemState (dataItem, this);
		}


		/// <summary>
		/// Navigates to the specified absolute path.
		/// </summary>
		/// <param name="path">The absolute path.</param>
		/// <returns><c>true</c> if the navigation succeeded.</returns>
		public bool NavigateTo(string path)
		{
			this.Reset ();

			IDataItem found = DataView.GetDataItem (this.view, path, item => this.itemStack.Push (this.CreateItemState (item)));
			
			return found != null;
		}

		/// <summary>
		/// Navigates to the specified relative path. This is only possible if
		/// the current data item is not a leaf of the graph.
		/// </summary>
		/// <param name="path">The relative path.</param>
		/// <returns><c>true</c> if the navigation succeeded.</returns>
		public bool NavigateToRelative(string path)
		{
			DataView  view  = this.CurrentItemState.Item.DataView;
			IDataItem found = DataView.GetDataItem (view, path, item => this.itemStack.Push (this.CreateItemState (item)));
			
			return found != null;
		}

		public bool NavigateToSibling(string path)
		{
			ItemState state = this.itemStack.Pop ();
			DataView  view  = state.ParentItem.DataView;
			IDataItem found = DataView.GetDataItem (view, path, item => this.itemStack.Push (this.CreateItemState (item)));
			
			return found != null;
		}

		public bool NavigateToNext()
		{
			ItemState state = this.CurrentItemState;

			switch (state.ParentItem.ItemType)
			{
				case DataItemType.Table:
				case DataItemType.Vector:
					return state.NavigateToNext (this);

				default:
					return false;
			}
		}

		public bool NavigateToPrevious()
		{
			ItemState state = this.CurrentItemState;

			switch (state.ParentItem.ItemType)
			{
				case DataItemType.Table:
				case DataItemType.Vector:
					return state.NavigateToPrevious (this);

				default:
					return false;
			}
		}

		public bool NavigateToFirstChild()
		{
			ItemState state = this.CurrentItemState;
			string    child = null;

			switch (state.Item.ItemType)
			{
				case DataItemType.Table:
				case DataItemType.Vector:
					child = state.Item.GetFirstChildId ();
					
					if (child != null)
					{
						return this.NavigateToRelative (child);
					}
					break;
			}

			return false;
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

			System.Diagnostics.Debug.Assert (this.itemStack.Count > 0);

			this.itemStack.Pop ();

			return true;
		}

		
		
		public void RequestBreak()
		{
			throw new System.NotImplementedException ();
		}

		public void RequestBreak(IEnumerable<CellSplitInfo> collection, int index)
		{
			this.currentSplitInfos = collection;
			this.currentSplitIndex = index;
		}


		#region ItemState class

		/// <summary>
		/// The <c>ItemState</c> class wraps some logic around the data item,
		/// in order to handle a statefull navigation (i.e. to add synthetic
		/// items as required by the item type).
		/// </summary>
		private class ItemState
		{
			public ItemState(DataView.DataItem item, DataNavigator navigator)
			{
				this.item = item;
				this.Reset (navigator);
			}

			public DataView.DataItem Item
			{
				get
				{
					switch (this.VirtualNodeType)
					{
						case VirtualNodeType.Data:
						case VirtualNodeType.BodyData:
							return this.item;

						default:
							return this.ParentItem.GetVirtualItem (this.VirtualNodeType);
					}
					
				}
			}

			public DataView.DataItem ParentItem
			{
				get
				{
					DataView view = this.item.ParentDataView;
					
					if (view == null)
					{
						return DataItems.EmptyDataItem.Value;
					}
					else
					{
						return view.Item;
					}
				}
			}

			public string Path
			{
				get
				{
					DataView.DataItem parent = this.ParentItem;

					if (DataItems.EmptyDataItem.IsEmpty (parent))
					{
						return this.Name ?? "";
					}
					else
					{
						return DataView.GetPath (parent, this.Name);
					}
				}
			}

			public string Name
			{
				get
				{
					return DataView.GetVirtualNodeId (this.VirtualNodeType) ?? this.item.Id;
				}
			}

			public VirtualNodeType VirtualNodeType
			{
				get;
				private set;
			}

			public void Reset(DataNavigator navigator)
			{
				if (navigator.EnableSyntheticNodes)
				{
					switch (this.ParentItem.ItemType)
					{
						case DataItemType.Table:
							this.VirtualNodeType = VirtualNodeType.Header1;
							break;

						default:
							this.VirtualNodeType = VirtualNodeType.Data;
							break;
					}
				}
				else
				{
					this.VirtualNodeType = VirtualNodeType.Data;
				}
			}
			

			public bool NavigateToNext(DataNavigator navigator)
			{
				switch (this.VirtualNodeType)
				{
					case VirtualNodeType.Header1:
						this.VirtualNodeType = VirtualNodeType.Header2;
						return true;

					case VirtualNodeType.Header2:
						this.VirtualNodeType = VirtualNodeType.BodyData;
						return true;

					case VirtualNodeType.Footer2:
						this.VirtualNodeType = VirtualNodeType.Footer1;
						return true;

					case VirtualNodeType.Footer1:
						return false;
				}

				DataView.DataItem parent = this.ParentItem;
				string id = parent.GetNextChildId (this.Name);

				if (id == null)
				{
					if (this.VirtualNodeType == VirtualNodeType.BodyData)
					{
						this.VirtualNodeType = VirtualNodeType.Footer2;
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					this.item = DataView.GetDataItem (parent.DataView, id) as DataView.DataItem;

					System.Diagnostics.Debug.Assert (this.item != null);

					return true;
				}
			}

			public bool NavigateToPrevious(DataNavigator dataNavigator)
			{
				switch (this.VirtualNodeType)
				{
					case VirtualNodeType.Header1:
						return false;

					case VirtualNodeType.Header2:
						this.VirtualNodeType = VirtualNodeType.Header1;
						return true;

					case VirtualNodeType.Footer2:
						this.VirtualNodeType = VirtualNodeType.BodyData;
						return true;

					case VirtualNodeType.Footer1:
						this.VirtualNodeType = VirtualNodeType.Footer2;
						return true;
				}

				DataView.DataItem parent = this.ParentItem;
				string id = parent.GetPreviousChildId (this.Name);

				if (id == null)
				{
					if (this.VirtualNodeType == VirtualNodeType.BodyData)
					{
						this.VirtualNodeType = VirtualNodeType.Header2;
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					this.item = DataView.GetDataItem (parent.DataView, id) as DataView.DataItem;

					System.Diagnostics.Debug.Assert (this.item != null);

					return true;
				}
			}
			
			private DataView.DataItem item;
		}

		#endregion


		private readonly DataView				view;
		private readonly Stack<ItemState>		itemStack;
	
		private IEnumerable<CellSplitInfo>		currentSplitInfos;
		private int								currentSplitIndex;
	}
}
