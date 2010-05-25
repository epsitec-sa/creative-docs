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
	public class TileContainerController
	{
		public TileContainerController(CoreViewController controller, Widget container, SummaryDataItems dataItems)
		{
			this.controller  = controller;
			this.container   = container;
			this.dataItems   = dataItems;
			this.activeItems = new List<SummaryData> ();
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
				this.RefreshDataTilesContent ();

				var tiles  = this.GetTitleTiles ();
				var height = TileContainerController.GetTotalHeight (tiles);

				if (height < maxHeight)
				{
					if (expandWhenSpaceIsAvailable)
					{
						var lastCompactItem = this.activeItems.LastOrDefault (item => item.AutoGroup == false && item.SummaryTile.IsCompact);

						if (lastCompactItem != null)
						{
							lastCompactItem.SummaryTile.IsCompact = false;
							continue;
						}
					}
				}
				else if (height > maxHeight)
				{
					expandWhenSpaceIsAvailable = false;
					
					var lastExpandedItem = this.activeItems.LastOrDefault (item => item.AutoGroup == false && !item.SummaryTile.IsCompact);

					if (lastExpandedItem != null)
					{
						lastExpandedItem.SummaryTile.IsCompact = true;
						continue;
					}
				}

				break;
			}
		}
		
		
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

		private static void DisposeDataItems(IEnumerable<SummaryData> collection)
		{
			foreach (var item in collection)
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
		}


		private void RefreshDataTiles()
		{
			this.SortActiveItems ();
			this.CreateMissingSummaryTiles ();
			this.ResetDataTiles ();

			if (!this.container.IsActualGeometryValid)
			{
				Common.Widgets.Layouts.LayoutContext.SyncArrange (this.container);
			}

			double maxHeight = this.container.ActualHeight;

			this.LayoutTiles (maxHeight);
			this.SetDataTilesParent (this.container);
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

					TileContainerController.CreateSummaryTileHandler (item.SummaryTile, this.controller);
				}

				if (item.TitleTile == null)
				{
					this.CreateTitleTile (item);
				}
			}
		}

		private static void CreateSummaryTileHandler(SummaryTile tile, CoreViewController controller)
		{
			tile.Clicked +=
				delegate
				{
					tile.ToggleSubView (controller.Orchestrator, controller);
				};
		}

		private static void CreateTitleTileHandler(TitleTile tile, CoreViewController controller)
		{
			tile.Clicked +=
				delegate
				{
					tile.Items[0].ToggleSubView (controller.Orchestrator, controller);
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

			TileContainerController.CreateTitleTileHandler (item.TitleTile, this.controller);

			System.Diagnostics.Debug.Assert (item.TitleTile.Items.Contains (item.SummaryTile));
			
			return item.TitleTile;
		}

		private void RemoveTitleTile(SummaryData item)
		{
			item.TitleTile = null;
		}
		
		private void ResetDataTiles()
		{
			var visualIds = new HashSet<long> ();
			var tileCache = new Dictionary<string, TitleTile> ();

			foreach (var item in this.activeItems)
			{
				System.Diagnostics.Debug.Assert (item.SummaryTile != null);

				if (item.AutoGroup)
				{
					this.ResetAutoGroupItemDataTile (item, tileCache);
				}
				else
				{
					this.ResetStandardItemDataTile (item, visualIds);
				}

				item.SummaryTile.IsCompact = false;
			}

			tileCache.Values.ForEach (tile => tile.Parent = null);
		}

		private void ResetAutoGroupItemDataTile(SummaryData item, Dictionary<string, TitleTile> tileCache)
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

		private void ResetStandardItemDataTile(SummaryData item, HashSet<long> visualIds)
		{
			if (item.TitleTile == null)
			{
				this.CreateTitleTile (item);
				
				visualIds.Add (item.TitleTile.GetVisualSerialId ());
			}
			else
			{
				if (! visualIds.Add (item.TitleTile.GetVisualSerialId ()))
				{
					this.RemoveTitleTile (item);
					this.CreateTitleTile (item);
				}
			}
		}
		
		private void RefreshDataTilesContent()
		{
			foreach (var item in this.activeItems)
			{
				TileContainerController.RefreshDataTileContent (item);
			}
		}

		private static void RefreshDataTileContent(SummaryData item)
		{
			if (item.SummaryTile.IsCompact)
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
			var visualIds = new HashSet<long> ();

			foreach (var item in this.activeItems)
			{
				var titleTile = item.TitleTile;
				long visualId = titleTile.GetVisualSerialId ();

				if (!visualIds.Contains (visualId))
				{
					visualIds.Add (visualId);
					titleTile.Parent = parent;
					titleTile.Dock = DockStyle.Top;
					titleTile.Margins = new Margins (0, 0, 0, 5);
					titleTile.ArrowDirection = Direction.Right;
					titleTile.IsReadOnly = true;
				}
			}
		}

		private readonly Widget					container;
		private readonly CoreViewController		controller;
		private readonly SummaryDataItems		dataItems;
		private readonly List<SummaryData>		activeItems;

		private int currentGeneration;
	}
}
