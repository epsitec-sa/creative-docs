//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// <see cref="TileDataItem"/> found in <see cref="TileDataItems"/>.
	/// </summary>
	public sealed partial class TileContainerController : IClickSimulator, IWidgetUpdater, IIsDisposed
	{
		private TileContainerController(TileContainer container, Widget parent = null)
		{
			this.controller   = container.Controller as EntityViewController;
			this.navigator    = this.controller.Navigator;
			this.container    = container;
			this.parent       = parent ?? this.container;
			this.dataItems    = new TileDataItems (this.controller);
			this.liveItems    = new List<TileDataItem> ();
			this.dataContext  = this.controller.DataContext;
			this.refreshTimer = new Timer ()
			{
				AutoRepeat = 0.2,
				Delay = 0.5,
			};

			this.closeButton = UIBuilder.CreateColumnTileCloseButton (this.container);

			this.refreshTimer.TimeElapsed += this.HandleTimerTimeElapsed;
			this.parent.SizeChanged += this.HandleContainerSizeChanged;

			this.controller.ActivateNextSubView = cyclic => Library.UI.Services.ExecuteWithDirectSetFocus (() => this.ActivateNextGenericTile (this.GetCyclicGenericTiles (cyclic)));
			this.controller.ActivatePrevSubView = cyclic => Library.UI.Services.ExecuteWithReverseSetFocus (() => this.ActivateNextGenericTile (this.GetCyclicGenericTiles (cyclic).Reverse ()));

			this.controller.Disposing += this.HandleControllerDisposing;
			this.dataContext.EntityChanged += this.HandleEntityChanged;

			this.navigator.Register (this);
			
			this.refreshTimer.Start ();
			this.container.Add (this);
		}


		public TileDataItems					DataItems
		{
			get
			{
				return this.dataItems;
			}
		}

		public EntityViewController				EntityViewController
		{
			get
			{
				return this.controller;
			}
		}


		public static Initializer Setup(EntityViewController controller)
		{
			return TileContainerController.Setup (controller.TileContainer);
		}

		public static Initializer Setup(TileContainer container)
		{
			return new Initializer (new TileContainerController (container));
		}

		public static Initializer Setup(UIBuilder builder)
		{
			var frame = new FrameBox
			{
				Padding = new Margins (0, 0, 0, 1),
			};

			builder.Add (frame);

			return new Initializer (new TileContainerController (builder.TileContainer, frame));
		}


		public TileDataItem FindTileData(string name)
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
#if true
			this.GenerateTiles ();
			this.OpenSubView (index, itemName);
#else
			this.QueueTasklets ("CreateNewTile",
				new TaskletJob (() => this.GenerateTiles (), TaskletRunMode.After),
				new TaskletJob (() => this.OpenSubView (index, itemName), TaskletRunMode.After));
#endif
		}

		#region IClickSimulator Members

		bool IClickSimulator.SimulateClick(string name)
		{
			var item = this.FindTileData (name);

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
			this.isDisposed = true;
			this.refreshTimer.Stop ();
			
			this.navigator.Unregister (this);

			this.refreshTimer.TimeElapsed -= this.HandleTimerTimeElapsed;
			this.dataContext.EntityChanged -= this.HandleEntityChanged;
			this.parent.SizeChanged -= this.HandleContainerSizeChanged;
			
			this.GetTitleTiles ().ForEach (x => x.Parent = null);

			TileContainerController.DisposeDataItems (this.dataItems);
		}

		#endregion

		#region IIsDisposed Members

		public bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		#endregion

#if false
		private void QueueTasklets(string name, params TaskletJob[] jobs)
		{
			//	TODO: use another thread to run the asynchronous work

			jobs.ForEach (job => job.Owner = this);

			Application.QueueTasklets (name, jobs);
		}
#endif
		
		private void RefreshCollectionItems()
		{
#if true
			this.dataItems.RefreshCollectionItems ();
			this.RefreshLiveItems ();
#else
			this.QueueTasklets ("RefreshCollectionItems",
				new TaskletJob (() => this.dataItems.RefreshCollectionItems (), TaskletRunMode.Async),
				new TaskletJob (() => this.RefreshLiveItems (), TaskletRunMode.BeforeAndAfter));
#endif
		}
		
		private void RefreshLiveItems()
		{
			var currentItems  = new List<TileDataItem> (this.dataItems);
			var obsoleteItems = new List<TileDataItem> (this.liveItems.Except (currentItems));

			this.liveItems.Clear ();
			this.liveItems.AddRange (currentItems);

			TileContainerController.DisposeDataItems (obsoleteItems);

#if true
			this.liveItems.ForEach (x => x.ExecuteAccessors ());
			this.RefreshDataTiles ();
#else
			this.QueueTasklets ("ExecuteAccessors",
				new TaskletJob (() => this.liveItems.ForEach (x => x.ExecuteAccessors ()), TaskletRunMode.Async),
				new TaskletJob (() => this.RefreshDataTiles (), TaskletRunMode.BeforeAndAfter));
#endif
		}

		private void RefreshDataTiles()
		{
			this.refreshNeeded = false;
			this.SortLiveItems ();
			this.CreateMissingTiles ();
			this.RefreshTitleTiles ();
			this.RefreshTitleTilesFreezeMode ();
			this.RefreshLayout ();
			this.SetDataTilesParent ();
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

		private static void DisposeDataItems(IEnumerable<TileDataItem> collection)
		{
			foreach (var item in collection)
			{
				TileContainerController.DisposeDataItem (item);
			}
		}

		private static void DisposeDataItem(TileDataItem item)
		{
			var tile  = item.Tile;
			var title = item.TitleTile;

			item.TitleTile   = null;
			item.Tile = null;

			if (tile != null)
			{
				tile.Dispose ();
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

		private void CreateMissingTiles()
		{
			using (var builder = new UIBuilder (this.controller))
			{
				foreach (var item in this.liveItems)
				{
					if (item.Tile == null)  // tuile pas encore créée ?
					{
						if (item.DataType == TileDataType.EditableItem)
						{
							this.CreateEditionTile (item, builder);
						}
						else if (item.DataType == TileDataType.CustomizedItem)
						{
							this.CreateCustomizedTile (item, builder);
						}
						else
						{
							this.CreateSummaryTile (item);
							this.CreateSummaryTileClickHandler (item);
						}
					}

					if (item.TitleTile == null)
					{
						this.CreateTitleTile (item);
					}
				}
			}
		}

		private void CreateSummaryTileClickHandler(TileDataItem item)
		{
			item.Tile.Clicked += (sender, e) => this.HandleTileClicked (item);
		}

		private void CreateTitleTileClickHandler(TileDataItem item, TitleTile tile)
		{
			if (item.DataType == TileDataType.EditableItem  ||
				item.DataType == TileDataType.CustomizedItem)
			{
				return;
			}

			tile.Clicked += (sender, e) => this.HandleTileClicked (this.liveItems.First (x => x.TitleTile == tile));

			tile.AddClicked += (sender, e) =>
				{
					string itemName = TileDataItem.GetNamePrefix (item.Name);
#if true
					item.AddNewItem ();
					this.GenerateTiles ();
					this.OpenSubViewForCreatedTile (item, itemName);
#else
					this.QueueTasklets ("CreateNewTile",
						new TaskletJob (() => item.AddNewItem (), TaskletRunMode.Async),
						new TaskletJob (() => this.GenerateTiles (), TaskletRunMode.After),
						new TaskletJob (() => this.OpenSubViewForCreatedTile (item, itemName), TaskletRunMode.After));
#endif
				};
			tile.RemoveClicked += (sender, e) =>
				{
					this.controller.Orchestrator.CloseSubViews (this.controller);
					item.DeleteItem ();
					this.GenerateTiles ();
				};

		}

		private void HandleTileClicked(TileDataItem item)
		{
			if (item.DataType == TileDataType.EditableItem  ||
				item.DataType == TileDataType.CustomizedItem)
			{
				return;
			}

			if ((item.DataType == TileDataType.EmptyItem) &&
				(item.AddNewItem != null))
			{
				string itemName = item.Name;
#if true
				item.AddNewItem ();
				this.GenerateTiles ();
				this.OpenSubViewForCreatedTile (item, itemName);
#else
				this.QueueTasklets ("CreateNewTile",
					new TaskletJob (() => item.AddNewItem (), TaskletRunMode.Async),
					new TaskletJob (() => this.GenerateTiles (), TaskletRunMode.After),
					new TaskletJob (() => this.OpenSubViewForCreatedTile (item, itemName), TaskletRunMode.After));
#endif
			}
			else
			{
				item.Tile.ToggleSubView (this.controller.Orchestrator, this.controller);
			}
		}

		private void OpenSubViewForCreatedTile(TileDataItem item, string itemName)
		{
			//	Ouvre la vue correspondant à la dernière entité créée dans une collection.
			this.OpenSubView (item.CreatedIndex, itemName);
		}

		private void OpenSubView(int index, string itemName)
		{
			//	Ouvre une vue correspondant à une entité dans une collection.
			TileDataItem sel = null;

			foreach (var x in this.liveItems)
			{
				if (TileDataItem.GetNamePrefix (x.Name) == itemName)
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
				var last = this.liveItems.LastOrDefault (x => TileDataItem.GetNamePrefix (x.Name) == itemName);

				if (last != null)
				{
					last.Tile.OpenSubView (this.controller.Orchestrator, this.controller);
				}
			}
			else
			{
				sel.Tile.OpenSubView (this.controller.Orchestrator, this.controller);
			}
		}

		private void CreateSummaryTile(TileDataItem item)
		{
			if (item.DataType == TileDataType.CollectionItem ||
				item.DataType == TileDataType.EmptyItem      )
			{
				var tile = new CollectionItemTile ();

				if (item.DeleteItem != null)
				{
					tile.RemoveClicked += sender =>
					{
						this.controller.Orchestrator.CloseSubViews (this.controller);
						item.DeleteItem ();
						this.GenerateTiles ();
					};
				}

				tile.EnableRemoveButton = (item.DataType == TileDataType.CollectionItem && item.AutoGroup && !item.HideRemoveButton);
				item.Tile = tile;

				if (item.AutoGroup)
				{
					tile.Margins = new Margins (0, 2, 0, 0);
				}
			}
			else
			{
				var tile = new SummaryTile ();

				item.Tile = tile;
			}
			
			item.Tile.Controller = item;
		}

		private void CreateEditionTile(TileDataItem item, UIBuilder builder)
		{
			var tile = TileContainerController.CreateEditionTile (item);
			item.CreateEditionUI (tile, builder);
			item.Tile = tile;
		}

		private void CreateCustomizedTile(TileDataItem item, UIBuilder builder)
		{
			var tile = TileContainerController.CreateEditionTile (item);
			item.CreateCustomizedUI (tile, builder);
			item.Tile = tile;
		}

		private static EditionTile CreateEditionTile(TileDataItem item)
		{
			return new EditionTile
			{
				Controller = item,
			};
		}
		
		private void CreateTitleTile(TileDataItem item)
		{
			//	Crée la tuile de titre, parente des SummaryTile et EditionTile.
			System.Diagnostics.Debug.Assert (item.TitleTile == null);
			System.Diagnostics.Debug.Assert (item.Tile != null);

			item.TitleTile = new TitleTile ()  // item.TitleTile aura item.Tile dans sa collection Items !
			{
				IsReadOnly = (item.DataType != TileDataType.EditableItem),  // fond bleuté si tuile d'édition
				Frameless  = item.Frameless,
				Dock = item.FullHeightStretch ? DockStyle.StackFill : DockStyle.Stacked,
			};

			System.Diagnostics.Debug.Assert (item.TitleTile.Items.Contains (item.Tile));

			this.CreateTitleTileClickHandler (item, item.TitleTile);
		}

		private void RemoveTitleTile(TileDataItem item)
		{
			item.TitleTile = null;  // enlève item.Tile de la collection Items de item.TitleTile
		}

		private void RefreshTitleTiles()
		{
			var visualIds = new HashSet<long> ();
			var tileCache = new Dictionary<string, TitleTile> ();

			foreach (var item in this.liveItems)
			{
				System.Diagnostics.Debug.Assert (item.Tile != null);

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
					case TileDataType.CollectionItem:
					case TileDataType.EmptyItem:
						item.TitleTile.EnableAddItems   = !item.HideAddButton;
						item.TitleTile.EnableRemoveItems = !item.HideRemoveButton;
						break;

					case TileDataType.EditableItem:
					case TileDataType.CustomizedItem:
						item.TitleTile.EnableAddItems   = false;
						item.TitleTile.EnableRemoveItems = false;
						break;

					default:
						item.TitleTile.EnableAddItems   = false;
						item.TitleTile.EnableRemoveItems = false;
						isItemPartOfCollection = false;
						break;
				}
				
				item.Tile.IsCompact  = isItemPartOfCollection;
				item.Tile.AutoHilite = isItemPartOfCollection;
			}

			tileCache.Values.ForEach (tile => tile.Parent = null);
		}

		private void RefreshTitleTilesFreezeMode()
		{
			foreach (var item in this.liveItems)
			{
				if (item.AutoGroup || item.DataType == TileDataType.EditableItem)
				{
					item.Tile.SetFrozen (false);
				}
				else
				{
					item.Tile.SetFrozen (true);
				}
			}
		}

		private void ResetAutoGroupTitleTile(TileDataItem item, Dictionary<string, TitleTile> tileCache)
		{
			TitleTile other;
			string prefix = TileDataItem.GetNamePrefix (item.Name);

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

		private void ResetStandardTitleTile(TileDataItem item, HashSet<long> visualIdCache)
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

		private static void SetTileContent(TileDataItem item)
		{
			item.TitleTile.IconUri = item.IconUri;

			if (item.IsCompact || item.AutoGroup)
			{
				item.TitleTile.Title = item.DisplayedCompactTitle.ToString ();
			}
			else
			{
				item.TitleTile.Title = item.DisplayedTitle.ToString ();
			}

			if (item.Tile is SummaryTile)
			{
				var summaryTile = item.Tile as SummaryTile;

				if (item.IsCompact)
				{
					summaryTile.Summary = item.DisplayedCompactText.ToString ();
				}
				else
				{
					summaryTile.Summary = item.DisplayedText.ToString ();
				}
			}
		}

		private IEnumerable<TitleTile> GetTitleTiles()
		{
			//	Les tuiles d'un groupe ont un même TitleTile comme parent. Le HashSet sur le VisualSerialId
			//	évite de prendre plusieurs fois le même.
			var visualIds  = new HashSet<long> ();

			foreach (var titleTile in this.liveItems.Select (x => x.TitleTile))
			{
				if (titleTile != null)
				{
					if (visualIds.Add (titleTile.GetVisualSerialId ()))
					{
						yield return titleTile;
					}
				}
			}
		}

		private IEnumerable<GenericTile> GetGenericTiles()
		{
			return this.liveItems.Select (x => x.Tile);
		}

		private IEnumerable<GenericTile> GetCyclicGenericTiles(bool cyclic)
		{
			if (cyclic)
			{
				return this.GetGenericTiles ().Concat (this.GetGenericTiles ());
			}
			else
			{
				return this.GetGenericTiles ();
			}
		}

		private static GenericTile GetNextLiveGenericTile(IEnumerable<GenericTile> tiles)
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
		
		private void SetDataTilesParent()
		{
			this.ResetTitleTilesOrder (this.GetTitleTiles ());

			var window = this.parent.Window;

			if (window != null)
			{
				window.RefreshFocusedWidget ();
				window.RefreshEnteredWidgets ();
			}
		}

		/// <summary>
		/// Resets the order of the title tiles within the container, by making sure the
		/// parent's children are in the right order.
		/// </summary>
		/// <param name="titleTiles">The title tiles.</param>
		private void ResetTitleTilesOrder(IEnumerable<TitleTile> titleTiles)
		{
			if (titleTiles.Any (x => x.Parent == null))
			{
				titleTiles.ForEach (x => x.Parent = null);
			}

			foreach (var titleTile in titleTiles)
			{
				titleTile.Parent  = this.parent;
				titleTile.Margins = new Margins (0, 0, 0, -1);
			}
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

		private bool ActivateNextGenericTile(IEnumerable<GenericTile> tiles)
		{
			var tile = TileContainerController.GetNextLiveGenericTile (tiles);

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
			//	do its job asynchronously.

			this.refreshNeeded = true;
		}

		private void HandleControllerDisposing(object sender)
		{
			this.Dispose ();
		}

		private void HandleTimerTimeElapsed(object sender)
		{
			if (this.refreshNeeded)
			{
				this.RefreshCollectionItems ();
			}
		}

		private readonly EntityViewController		controller;
		private readonly NavigationOrchestrator		navigator;
		private readonly Widget						parent;
		private readonly TileContainer				container;
		private readonly TileDataItems				dataItems;
		private readonly List<TileDataItem>			liveItems;
		private readonly DataContext				dataContext;
		private readonly Timer						refreshTimer;

		private readonly Button						closeButton;

		private bool								refreshNeeded;
		private bool								isDisposed;
	}
}