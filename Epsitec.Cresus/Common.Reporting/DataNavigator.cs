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

		private ItemState						CurrentItemState
		{
			get
			{
				return this.itemStack.Peek ();
			}
		}


		/// <summary>
		/// Resets this navigator and starts again at the root.
		/// </summary>
		public void Reset()
		{
			this.itemStack.Clear ();
			this.itemStack.Push (this.CreateItemState (this.view.Item));
			
			this.currentSplitIndex = 0;
			this.currentSplitInfos = null;
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

		/// <summary>
		/// Navigates to the specified sibling item.
		/// </summary>
		/// <param name="path">The path to the sibling.</param>
		/// <returns><c>true</c> if the navigation succeeded.</returns>
		public bool NavigateToSibling(string path)
		{
			System.Diagnostics.Debug.Assert (path != null);
			System.Diagnostics.Debug.Assert (path.Length > 0);
			System.Diagnostics.Debug.Assert (path.Contains (".") == false);

			ItemState state = this.itemStack.Pop ();
			DataView  view  = state.ParentItem.DataView;
			IDataItem found = DataView.GetDataItem (view, path, item => this.itemStack.Push (this.CreateItemState (item)));
			
			return found != null;
		}

		/// <summary>
		/// Navigates to the next item at the same level than the current one.
		/// When the last item is reached, nothing happens and the method
		/// returns <c>false</c>.
		/// </summary>
		/// <returns><c>true</c> if the navigation succeeded.</returns>
		public bool NavigateToNext()
		{
			ItemState state = this.CurrentItemState;

			switch (state.ParentItem.ItemType)
			{
				case DataItemType.Table:
				case DataItemType.Vector:
					return state.NavigateToNext ();

				default:
					return false;
			}
		}

		/// <summary>
		/// Navigates to the previous item at the same level than the current one.
		/// When the first item is reached, nothing happens and the method
		/// returns <c>false</c>.
		/// </summary>
		/// <returns><c>true</c> if the navigation succeeded.</returns>
		public bool NavigateToPrevious()
		{
			ItemState state = this.CurrentItemState;

			switch (state.ParentItem.ItemType)
			{
				case DataItemType.Table:
				case DataItemType.Vector:
					return state.NavigateToPrevious ();

				default:
					return false;
			}
		}

		/// <summary>
		/// Navigates to the first child item of the current item.
		/// </summary>
		/// <returns><c>true</c> if the navigation succeeded.</returns>
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


		/// <summary>
		/// Creates the item state for the specified item.
		/// </summary>
		/// <param name="dataItem">The data item.</param>
		/// <returns>The item state.</returns>
		private ItemState CreateItemState(DataView.DataItem dataItem)
		{
			return new ItemState (dataItem, this);
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

			
			public DataView.DataItem			Item
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

			public DataView.DataItem			ParentItem
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

			public string						Path
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

			public string						Name
			{
				get
				{
					return DataView.GetVirtualNodeId (this.VirtualNodeType) ?? this.item.Id;
				}
			}

			public VirtualNodeType				VirtualNodeType
			{
				get;
				private set;
			}


			/// <summary>
			/// Navigates to the next item, while taking in account the virtual
			/// nodes.
			/// </summary>
			/// <returns><c>true</c> if the navigation succeeded.</returns>
			public bool NavigateToNext()
			{
				//	Natural sequence is Header1/Header2/BodyData/Footer2/Footer1

				switch (this.VirtualNodeType)
				{
					case VirtualNodeType.Header1: this.VirtualNodeType = VirtualNodeType.Header2;	return true;
					case VirtualNodeType.Header2: this.VirtualNodeType = VirtualNodeType.BodyData;	return true;
					case VirtualNodeType.Footer2: this.VirtualNodeType = VirtualNodeType.Footer1;	return true;
					case VirtualNodeType.Footer1:													return false;
				}

				DataView.DataItem parent = this.ParentItem;
				string id = parent.GetNextChildId (this.Name);

				if (id == null)
				{
					//	No more data children. This means we have either reached
					//	the last child, or we have to start producing the virtual
					//	nodes :

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
					this.item = DataView.GetDataItem (parent.DataView, id);

					System.Diagnostics.Debug.Assert (this.item != null);

					return true;
				}
			}

			/// <summary>
			/// Navigates to previous item, while taking in account the virtual
			/// nodes.
			/// </summary>
			/// <returns><c>true</c> if the navigation succeeded.</returns>
			public bool NavigateToPrevious()
			{
				//	Natural sequence is Header1/Header2/BodyData/Footer2/Footer1
				
				switch (this.VirtualNodeType)
				{
					case VirtualNodeType.Header1:					return false;
					case VirtualNodeType.Header2: this.VirtualNodeType = VirtualNodeType.Header1;	return true;
					case VirtualNodeType.Footer2: this.VirtualNodeType = VirtualNodeType.BodyData;	return true;
					case VirtualNodeType.Footer1: this.VirtualNodeType = VirtualNodeType.Footer2;	return true;
				}

				DataView.DataItem parent = this.ParentItem;
				string id = parent.GetPreviousChildId (this.Name);

				if (id == null)
				{
					//	No more data children. This means we have either reached
					//	the last child, or we have to start producing the virtual
					//	nodes :

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
					this.item = DataView.GetDataItem (parent.DataView, id);

					System.Diagnostics.Debug.Assert (this.item != null);

					return true;
				}
			}

			private void Reset(DataNavigator navigator)
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
			private DataView.DataItem item;
		}

		#endregion


		private readonly DataView				view;
		private readonly Stack<ItemState>		itemStack;
	
		private IEnumerable<CellSplitInfo>		currentSplitInfos;
		private int								currentSplitIndex;
	}
}
