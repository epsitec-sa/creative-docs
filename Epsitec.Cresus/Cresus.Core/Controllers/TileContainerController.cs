//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	using LayoutContext=Epsitec.Common.Widgets.Layouts.LayoutContext;
	
	public class TileContainerController : System.IDisposable
	{
		public TileContainerController(CoreViewController controller, Widget container, SummaryDataItems dataItems)
		{
			this.controller  = controller;
			this.container   = container;
			this.dataItems   = dataItems;
			this.activeItems = new List<SummaryData> ();

			this.container.SizeChanged += this.HandleContainerSizeChanged;
		}

		
		public void MapDataToTiles()
		{
			this.RefreshCollectionItems ();
		}

		public void LayoutTiles(double maxHeight)
		{
			bool expandWhenSpaceIsAvailable = true;

			while (true)
			{
				this.RefreshTilesContent ();

				var tiles  = this.GetTitleTiles ();
				var height = TileContainerController.GetTotalHeight (tiles);

				if (height < maxHeight)
				{
					if (expandWhenSpaceIsAvailable)
					{
						var lastCompactItem = this.activeItems.FirstOrDefault (item => item.AutoGroup == false && item.IsCompact);

						if (lastCompactItem != null)
						{
							lastCompactItem.IsCompact = false;
							continue;
						}
					}
				}
				else if (height > maxHeight)
				{
					expandWhenSpaceIsAvailable = false;
					
					var lastExpandedItem = this.activeItems.LastOrDefault (item => item.AutoGroup == false && !item.IsCompact);

					if (lastExpandedItem != null)
					{
						lastExpandedItem.IsCompact = true;
						continue;
					}
				}

				break;
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.container.SizeChanged -= this.HandleContainerSizeChanged;
			
			this.GetTitleTiles ().ForEach (x => x.Parent = null);

			TileContainerController.DisposeDataItems (this.dataItems);
		}

		#endregion
		
		private void QueueTasklet<T>(string name, T source, System.Action<T> action, params SimpleCallback[] andThen)
		{
			andThen.ForEach (callback => Application.QueueAsyncCallback (callback));

			//	TODO: execute following code asynchronously :
			
			action (source);
			andThen.ForEach (callback => Application.QueueAsyncCallback (callback));
		}

		
		private void RefreshCollectionItems()
		{
			this.QueueTasklet ("RefreshCollectionItems", this.dataItems, items => items.RefreshCollectionItems (),
				this.RefreshActiveItems);
		}
		
		private void RefreshActiveItems()
		{
			var currentItems  = new List<SummaryData> (this.dataItems);
			var obsoleteItems = new List<SummaryData> (this.activeItems.Except (currentItems));

			this.activeItems.Clear ();
			this.activeItems.AddRange (currentItems);

			TileContainerController.DisposeDataItems (obsoleteItems);

			this.QueueTasklet ("ExecuteAccessors", this.activeItems, items => items.ForEach (x => x.ExecuteAccessors ()),
				this.RefreshDataTiles);
		}

		private void RefreshDataTiles()
		{
			this.SortActiveItems ();
			this.CreateMissingSummaryTiles ();
			this.RefreshTitleTiles ();
			this.RefreshLayout ();
			this.SetDataTilesParent (this.container);
		}

		private void RefreshLayout()
		{
			if (this.container.IsActualGeometryDirty)
			{
				LayoutContext.SyncArrange (this.container);
			}

			this.LayoutTiles (this.container.ActualHeight);
		}

		
		private static void DisposeDataItems(IEnumerable<SummaryData> collection)
		{
			foreach (var item in collection)
			{
				TileContainerController.DisposeDataItem (item);
			}
		}

		private static void DisposeDataItem(SummaryData item)
		{
			var summary = item.SummaryTile;
			var title   = item.TitleTile;

			item.TitleTile   = null;
			item.SummaryTile = null;

			if (summary != null)
			{
				summary.Dispose ();
			}

			if ((title != null) &&
				(title.Items.Count == 0))
			{
				title.Dispose ();
			}
		}

		
		private void SortActiveItems()
		{
			this.activeItems.Sort ();
		}

		private void CreateMissingSummaryTiles()
		{
			foreach (var item in this.activeItems)
			{
				if (item.SummaryTile == null)
				{
					this.CreateSummaryTile (item);

					this.CreateSummaryTileClickHandler (item.SummaryTile);
				}

				if (item.TitleTile == null)
				{
					this.CreateTitleTile (item);
				}
			}
		}

		private void CreateSummaryTileClickHandler(SummaryTile tile)
		{
			tile.Clicked +=
				delegate
				{
					tile.ToggleSubView (this.controller.Orchestrator, this.controller);
				};
		}

		private void CreateTitleTileClickHandler(TitleTile tile)
		{
			tile.Clicked +=
				delegate
				{
					tile.Items[0].ToggleSubView (this.controller.Orchestrator, this.controller);
				};
		}

		private void CreateSummaryTile(SummaryData item)
		{
			if (item.DataType == SummaryDataType.CollectionItem)
			{
				var tile = new CollectionItemTile ();

				tile.AddClicked    += sender =>
				{
					item.AddNewItem ();
					this.MapDataToTiles ();
				};
				tile.RemoveClicked += sender =>
				{
					item.DeleteItem ();
					this.MapDataToTiles ();
				};

				item.SummaryTile = tile;

				if (item.AutoGroup)
				{
					tile.Margins = new Margins (0, 2, 0, 0);
				}
			}
			else
			{
				var tile = new SummaryTile ();
				item.SummaryTile = tile;
			}
			
			item.SummaryTile.Controller = item;
		}

		private TitleTile CreateTitleTile(SummaryData item)
		{
			System.Diagnostics.Debug.Assert (item.TitleTile == null);
			System.Diagnostics.Debug.Assert (item.SummaryTile != null);
			
			item.TitleTile = new TitleTile ();

			this.CreateTitleTileClickHandler (item.TitleTile);

			System.Diagnostics.Debug.Assert (item.TitleTile.Items.Contains (item.SummaryTile));
			
			return item.TitleTile;
		}

		private void RemoveTitleTile(SummaryData item)
		{
			item.TitleTile = null;
		}
		
		private void RefreshTitleTiles()
		{
			var visualIds = new HashSet<long> ();
			var tileCache = new Dictionary<string, TitleTile> ();

			foreach (var item in this.activeItems)
			{
				System.Diagnostics.Debug.Assert (item.SummaryTile != null);

				if (item.AutoGroup)
				{
					this.ResetAutoGroupTitleTile (item, tileCache);
				}
				else
				{
					this.ResetStandardTitleTile (item, visualIds);
				}

				item.SummaryTile.IsCompact  = item.DataType == SummaryDataType.CollectionItem && item.AutoGroup;
				item.SummaryTile.AutoHilite = item.DataType == SummaryDataType.CollectionItem && item.AutoGroup;
			}

			tileCache.Values.ForEach (tile => tile.Parent = null);
		}

		private void ResetAutoGroupTitleTile(SummaryData item, Dictionary<string, TitleTile> tileCache)
		{
			TitleTile other;
			string prefix = SummaryData.GetNamePrefix (item.Name);

			if (tileCache.TryGetValue (prefix, out other))
			{
				item.TitleTile = other;
			}
			else
			{
				if (item.TitleTile == null)
				{
					this.CreateTitleTile (item);
				}

				tileCache.Add (prefix, item.TitleTile);
			}
		}

		private void ResetStandardTitleTile(SummaryData item, HashSet<long> visualIdCache)
		{
			if (item.TitleTile == null)
			{
				this.CreateTitleTile (item);
				
				visualIdCache.Add (item.TitleTile.GetVisualSerialId ());
			}
			else
			{
				if (! visualIdCache.Add (item.TitleTile.GetVisualSerialId ()))
				{
					this.RemoveTitleTile (item);
					this.CreateTitleTile (item);
				}
			}
		}
		
		private void RefreshTilesContent()
		{
			foreach (var item in this.activeItems)
			{
				TileContainerController.SetTileContent (item);
			}
		}

		private static void SetTileContent(SummaryData item)
		{
			if (item.IsCompact)
			{
				item.SummaryTile.Summary = item.CompactText.ToString ();
				item.TitleTile.Title     = item.CompactTitle.ToString ();
			}
			else
			{
				item.SummaryTile.Summary = item.Text.ToString ();
				item.TitleTile.Title     = item.DefaultTitle.ToString ();
			}

			item.TitleTile.IconUri = item.IconUri;
		}

		private IEnumerable<TitleTile> GetTitleTiles()
		{
			var visualIds  = new HashSet<long> ();

			foreach (var item in this.activeItems)
			{
				if (visualIds.Add (item.TitleTile.GetVisualSerialId ()))
				{
					yield return item.TitleTile;
				}
			}
		}

		private static double GetTotalHeight(IEnumerable<TitleTile> collection)
		{
			double height = 0;

			foreach (var tile in collection)
			{
				if (height > 0)
				{
					height += 5;
				}

				height += tile.GetFullHeight ();
			}
			
			return height;
		}
		
		private void SetDataTilesParent(Widget parent)
		{
			foreach (var titleTile in this.GetTitleTiles ())
			{
				titleTile.Parent         = parent;
				titleTile.Dock           = DockStyle.Top;
				titleTile.Margins        = new Margins (0, 0, 0, 5);
				titleTile.ArrowDirection = Direction.Right;
				titleTile.IsReadOnly     = true;
			}

			Window.RefreshEnteredWidgets ();
		}


		private void HandleContainerSizeChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Size oldSize = (Size) e.OldValue;
			Size newSize = (Size) e.NewValue;

			if (oldSize.Height != newSize.Height)
			{
				this.LayoutTiles (newSize.Height);
			}
		}

		private readonly Widget					container;
		private readonly CoreViewController		controller;
		private readonly SummaryDataItems		dataItems;
		private readonly List<SummaryData>		activeItems;
	}
}
