//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	using LayoutContext=Epsitec.Common.Widgets.Layouts.LayoutContext;

	/// <summary>
	/// The <c>TileContainerController</c> populates a <see cref="TileContainer"/>
	/// with <see cref="TitleTile"/>s and <see cref="SummaryTile"/>s based on the
	/// <see cref="SummaryData"/> found in <see cref="SummaryDataItems"/>.
	/// </summary>
	public sealed class TileContainerController : System.IDisposable, IClickSimulator, IWidgetUpdater
	{
		private TileContainerController(TileContainer container, Widget parent = null)
		{
			this.controller  = container.Controller as EntityViewController;
			this.navigator   = this.controller.Navigator;
			this.container   = container;
			this.parent      = parent ?? this.container;
			this.dataItems   = new SummaryDataItems (this.controller);
			this.liveItems = new List<SummaryData> ();
			this.dataContext = this.controller.DataContext;
			this.refreshTimer = new Timer ()
			{
				AutoRepeat = 0.2,
				Delay = 0.5,
			};

			this.closeButton = UIBuilder.CreateColumnTileCloseButton (this.container);

			this.refreshTimer.TimeElapsed += this.HandleTimerTimeElapsed;
			this.parent.SizeChanged += this.HandleContainerSizeChanged;

			this.controller.ActivateNextSubView = cyclic => UI.ExecuteWithDirectSetFocus (() => this.ActivateNextSummaryTile (this.GetCyclicSummaryTiles (cyclic)));
			this.controller.ActivatePrevSubView = cyclic => UI.ExecuteWithReverseSetFocus (() => this.ActivateNextSummaryTile (this.GetCyclicSummaryTiles (cyclic).Reverse ()));

			this.controller.Disposing += this.HandleControllerDisposing;
			this.dataContext.EntityChanged += this.HandleEntityChanged;

			this.navigator.Register (this);
			
			this.refreshTimer.Start ();
			this.container.Add (this);
		}


		public static TileContainerControllerInitializer Setup(EntityViewController controller)
		{
			return TileContainerController.Setup (controller.TileContainer);
		}

		public static TileContainerControllerInitializer Setup(TileContainer container)
		{
			return new TileContainerControllerInitializer (new TileContainerController (container));
		}

		public static TileContainerControllerInitializer Setup(UIBuilder builder)
		{
			var frame = new FrameBox
			{
				Padding = new Margins (0, 0, 0, 1),
			};

			builder.Add (frame);

			return new TileContainerControllerInitializer (new TileContainerController (builder.TileContainer, frame));
		}


		public class TileContainerControllerInitializer : System.IDisposable
		{
			public TileContainerControllerInitializer(TileContainerController controller)
			{
				this.controller = controller;
			}

			public void Add(SummaryData item)
			{
				this.controller.DataItems.Add (item);
			}

			public void Add(CollectionAccessor item)
			{
				this.controller.DataItems.Add (item);
			}

			public static implicit operator SummaryDataItems(TileContainerControllerInitializer x)
			{
				return x.controller.DataItems;
			}

			#region IDisposable Members

			public void Dispose()
			{
				this.controller.GenerateTiles ();
			}

			#endregion

			private readonly TileContainerController controller;
		}

		public SummaryDataItems DataItems
		{
			get
			{
				return this.dataItems;
			}
		}

		public EntityViewController EntityViewController
		{
			get
			{
				return this.controller;
			}
		}

		public SummaryData FindSummaryData(string name)
		{
			return this.liveItems.Where (x => x.Name == name).FirstOrDefault ();
		}

		public void GenerateTiles()
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
						var lastCompactItem = this.liveItems.FirstOrDefault (item => item.AutoGroup == false && item.IsCompact);

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
					
					var lastExpandedItem = this.liveItems.LastOrDefault (item => item.AutoGroup == false && !item.IsCompact);

					if (lastExpandedItem != null)
					{
						lastExpandedItem.IsCompact = true;
						continue;
					}
				}

				break;
			}
		}

		public void ForceRefresh()
		{
			this.refreshNeeded = true;
		}


		public void ShowSubView(int index, string itemName)
		{
			this.QueueTasklets ("CreateNewTile",
				new TaskletJob (() => this.GenerateTiles (), TaskletRunMode.After),
				new TaskletJob (() => this.OpenSubView (index, itemName), TaskletRunMode.After));
		}

		#region IClickSimulator Members

		bool IClickSimulator.SimulateClick(string name)
		{
			var item = this.FindSummaryData (name);

			if (item != null)
			{
				this.HandleTileClicked (item);
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region IWidgetUpdater Members

		void IWidgetUpdater.Update()
		{
			this.RefreshCollectionItems ();
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			this.refreshTimer.Stop ();

			this.navigator.Unregister (this);

			this.refreshTimer.TimeElapsed -= this.HandleTimerTimeElapsed;
			this.dataContext.EntityChanged -= this.HandleEntityChanged;
			this.parent.SizeChanged -= this.HandleContainerSizeChanged;
			
			this.GetTitleTiles ().ForEach (x => x.Parent = null);

			TileContainerController.DisposeDataItems (this.dataItems);
		}

		#endregion
		
		private void QueueTasklets(string name, params TaskletJob[] jobs)
		{
			//	TODO: use another thread to run the asynchronous work

			Application.QueueTasklets (name, jobs);
		}

		
		private void RefreshCollectionItems()
		{
			System.Diagnostics.Debug.WriteLine ("About to RefreshCollectionItems on DataContext #" + this.dataContext.UniqueId);

			this.QueueTasklets ("RefreshCollectionItems",
				new TaskletJob (() => this.dataItems.RefreshCollectionItems (), TaskletRunMode.Async),
				new TaskletJob (() => this.RefreshLiveItems (), TaskletRunMode.BeforeAndAfter));
		}
		
		private void RefreshLiveItems()
		{
			var currentItems  = new List<SummaryData> (this.dataItems);
			var obsoleteItems = new List<SummaryData> (this.liveItems.Except (currentItems));

			this.liveItems.Clear ();
			this.liveItems.AddRange (currentItems);

			TileContainerController.DisposeDataItems (obsoleteItems);

			System.Diagnostics.Debug.WriteLine ("About to RefreshLiveItems on DataContext #" + this.dataContext.UniqueId);

			this.QueueTasklets ("ExecuteAccessors",
				new TaskletJob (() => this.liveItems.ForEach (x => x.ExecuteAccessors ()), TaskletRunMode.Async),
				new TaskletJob (() => this.RefreshDataTiles (), TaskletRunMode.BeforeAndAfter));
		}

		private void RefreshDataTiles()
		{
			this.refreshNeeded = false;
			this.SortLiveItems ();
			this.CreateMissingSummaryTiles ();
			this.RefreshTitleTiles ();
			this.RefreshTitleTilesFreezeMode ();
			this.RefreshLayout ();
			this.SetDataTilesParent (this.parent);
			this.SetCloseButtonVisibility ();
		}

		private void RefreshLayout()
		{
			if (this.parent.IsActualGeometryDirty)
			{
				LayoutContext.SyncArrange (this.parent);
			}

			this.LayoutTiles (this.parent.ActualHeight);
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

		
		private void SortLiveItems()
		{
			this.liveItems.Sort ();
		}

		private void CreateMissingSummaryTiles()
		{
			foreach (var item in this.liveItems)
			{
				if (item.SummaryTile == null)
				{
					this.CreateSummaryTile (item);
					this.CreateSummaryTileClickHandler (item);
				}

				if (item.TitleTile == null)
				{
					this.CreateTitleTile (item);
				}
			}
		}

		private void CreateSummaryTileClickHandler(SummaryData item)
		{
			item.SummaryTile.Clicked += (sender, e) => this.HandleTileClicked (item);
		}

		private void CreateTitleTileClickHandler(SummaryData item, TitleTile tile)
		{
			tile.Clicked += (sender, e) => this.HandleTileClicked (this.liveItems.First (x => x.TitleTile == tile));
			tile.AddClicked += (sender, e) =>
				{
					string itemName = SummaryData.GetNamePrefix (item.Name);

					this.QueueTasklets ("CreateNewTile",
						new TaskletJob (() => item.AddNewItem (), TaskletRunMode.Async),
						new TaskletJob (() => this.GenerateTiles (), TaskletRunMode.After),
						new TaskletJob (() => this.OpenSubViewForCreatedSummaryTile (item, itemName), TaskletRunMode.After));
				};
			tile.RemoveClicked += (sender, e) =>
				{
					this.controller.Orchestrator.CloseSubViews (this.controller);
					item.DeleteItem ();
					this.GenerateTiles ();
				};

		}

		private void HandleTileClicked(SummaryData item)
		{
			if (item.DataType == SummaryDataType.EmptyItem)
			{
				string itemName = item.Name;

				this.QueueTasklets ("CreateNewTile",
					new TaskletJob (() => item.AddNewItem (), TaskletRunMode.Async),
					new TaskletJob (() => this.GenerateTiles (), TaskletRunMode.After),
					new TaskletJob (() => this.OpenSubViewForCreatedSummaryTile (item, itemName), TaskletRunMode.After));
			}
			else
			{
				if (!item.SummaryTile.IsClickForDrag)
				{
					item.SummaryTile.ToggleSubView (this.controller.Orchestrator, this.controller);
				}
			}
		}

		private void OpenSubViewForCreatedSummaryTile(SummaryData item, string itemName)
		{
			//	Ouvre la vue correspondant à la dernière entité créée dans une collection.
			this.OpenSubView (item.CreatedIndex, itemName);
		}

		private void OpenSubView(int index, string itemName)
		{
			//	Ouvre une vue correspondant à une entité dans une collection.
			SummaryData sel = null;

			foreach (var x in this.liveItems)
			{
				if (SummaryData.GetNamePrefix (x.Name) == itemName)
				{
					if (index-- == 0)
					{
						sel = x;
						break;
					}
				}
			}

			if (sel == null)
			{
				var last = this.liveItems.LastOrDefault (x => SummaryData.GetNamePrefix (x.Name) == itemName);

				if (last != null)
				{
					last.SummaryTile.OpenSubView (this.controller.Orchestrator, this.controller);
				}
			}
			else
			{
				sel.SummaryTile.OpenSubView (this.controller.Orchestrator, this.controller);
			}
		}

		private void CreateSummaryTile(SummaryData item)
		{
			if ((item.DataType == SummaryDataType.CollectionItem) ||
				(item.DataType == SummaryDataType.EmptyItem))
			{
				var tile = new CollectionItemTile ();

				tile.AddClicked += sender =>
				{
					item.AddNewItem ();
					this.GenerateTiles ();
				};

				tile.RemoveClicked += sender =>
				{
					this.controller.Orchestrator.CloseSubViews (this.controller);
					item.DeleteItem ();
					this.GenerateTiles ();
				};

				tile.EnableAddRemoveButtons = item.DataType == SummaryDataType.CollectionItem && item.AutoGroup;
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

			this.CreateTitleTileClickHandler (item, item.TitleTile);

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

			foreach (var item in this.liveItems)
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

				bool isItemPartOfCollection = item.AutoGroup;

				switch (item.DataType)
				{
					case SummaryDataType.CollectionItem:
					case SummaryDataType.EmptyItem:
						item.TitleTile.ContainsCollectionItemTiles = true;
						break;

					default:
						item.TitleTile.ContainsCollectionItemTiles = false;
						isItemPartOfCollection = false;
						break;
				}
				
				item.SummaryTile.IsCompact  = isItemPartOfCollection;
				item.SummaryTile.AutoHilite = isItemPartOfCollection;
			}

			tileCache.Values.ForEach (tile => tile.Parent = null);
		}

		private void RefreshTitleTilesFreezeMode()
		{
			foreach (var item in this.liveItems)
			{
				if (item.AutoGroup)
				{
					item.SummaryTile.SetFrozen (false);
				}
				else
				{
					item.SummaryTile.SetFrozen (true);
				}
			}
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
			foreach (var item in this.liveItems)
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

			item.TitleTile.TitleIconUri = item.IconUri;
		}

		private IEnumerable<TitleTile> GetTitleTiles()
		{
			var visualIds  = new HashSet<long> ();

			foreach (var item in this.liveItems)
			{
				if (visualIds.Add (item.TitleTile.GetVisualSerialId ()))
				{
					yield return item.TitleTile;
				}
			}
		}

		private IEnumerable<SummaryTile> GetSummaryTiles()
		{
			return this.liveItems.Select (x => x.SummaryTile);
		}

		private IEnumerable<SummaryTile> GetCyclicSummaryTiles(bool cyclic)
		{
			if (cyclic)
			{
				return this.GetSummaryTiles ().Concat (this.GetSummaryTiles ());
			}
			else
			{
				return this.GetSummaryTiles ();
			}
		}
		
		private static SummaryTile GetNextLiveSummaryTile(IEnumerable<SummaryTile> tiles)
		{
			return tiles.SkipWhile (x => x.IsSelected == false).Skip (1).FirstOrDefault ();
		}


		private static double GetTotalHeight(IEnumerable<TitleTile> collection)
		{
			double height = 0;

			foreach (var tile in collection)
			{
				if (height > 0)
				{
					height -= 1;
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
				titleTile.Margins        = new Margins (0, 0, 0, -1);
				titleTile.ArrowDirection = Direction.Right;
				titleTile.IsReadOnly     = true;
			}

			Window.RefreshEnteredWidgets ();
		}

		private void SetCloseButtonVisibility()
		{
			var navigator = this.controller.Navigator;

			if ((navigator != null) &&
				(navigator.GetLevel (this.controller) > 0))
			{
				this.closeButton.ZOrder = 0;
				this.closeButton.Visibility = true;
			}
			else
			{
				this.closeButton.Visibility = false;
			}
		}

		private bool ActivateNextSummaryTile(IEnumerable<SummaryTile> tiles)
		{
			var tile = TileContainerController.GetNextLiveSummaryTile (tiles);

			if (tile == null)
			{
				return false;
			}
			else
			{
				tile.OpenSubView (this.controller.Orchestrator, this.controller);
				return true;
			}
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

		private void HandleEntityChanged(object sender, Epsitec.Cresus.DataLayer.Context.EntityChangedEventArgs e)
		{
			//	We don't refresh synchronously, since this method could be called very, very often
			//	and also produce deep recursive calls. Simply set a flag and let the refresh timer
			//	to its job asynchronously.

			this.refreshNeeded = true;
		}

		private void HandleControllerDisposing(object sender)
		{
			this.navigator.Unregister (this);
		}

		private void HandleTimerTimeElapsed(object sender)
		{
			if (this.refreshNeeded)
			{
				this.RefreshCollectionItems ();
			}
		}

		private readonly EntityViewController	controller;
		private readonly NavigationOrchestrator	navigator;
		private readonly Widget					parent;
		private readonly TileContainer		container;
		private readonly SummaryDataItems		dataItems;
		private readonly List<SummaryData>		liveItems;
		private readonly DataContext			dataContext;
		private readonly Timer					refreshTimer;

		private readonly Button					closeButton;

		private bool							refreshNeeded;
	}
}