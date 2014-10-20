//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class TimelinesArrayController : IDirty, System.IDisposable
	{
		public TimelinesArrayController(DataAccessor accessor, CommandContext commandContext, MainToolbar toolbar)
		{
			this.accessor       = accessor;
			this.commandContext = commandContext;
			this.mainToolbar    = toolbar;

			this.selectedRow    = -1;
			this.selectedColumn = -1;

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode -> SortableCumulNode
			var groupNodeGetter  = this.accessor.GetNodeGetter (BaseType.Groups);
			var objectNodeGetter = this.accessor.GetNodeGetter (BaseType.Assets);
			this.nodeGetter      = new ObjectsNodeGetter (this.accessor, groupNodeGetter, objectNodeGetter);

			this.dataFiller = new SingleObjectsTreeTableFiller (this.accessor, this.nodeGetter);

			this.arrayLogic = new TimelinesArrayLogic (this.accessor);
			this.dataArray = new TimelineArray ();

			this.timelinesMode = TimelinesMode.Wide;

			this.amortizations = new Amortizations (this.accessor);

			this.commandDispatcher = new CommandDispatcher (this.GetType ().FullName, CommandDispatcherLevel.Primary, CommandDispatcherOptions.AutoForwardCommands);
			this.commandDispatcher.RegisterController (this);  // nécesaire pour [Command (Res.CommandIds...)]
		}

		public void Dispose()
		{
			if (this.assetsToolbar != null)
			{
				this.assetsToolbar.Dispose ();
				this.assetsToolbar = null;
			}

			if (this.timelinesToolbar != null)
			{
				this.timelinesToolbar.Dispose ();
				this.timelinesToolbar = null;
			}

			this.commandDispatcher.Dispose ();
		}


		#region IDirty Members
		public bool DirtyData
		{
			get;
			set;
		}
		#endregion


		public string							Title;
		public System.Func<DataEvent, bool>		Filter;
		public bool								HasAmortizationsOper;

		public Guid								FilterGuid
		{
			get
			{
				return this.rootGuid;
			}
			set
			{
				if (this.rootGuid != value)
				{
					this.rootGuid = value;
					this.UpdateTreeColumn ();
					this.UpdateData ();
				}
			}
		}
	
	
		public void UpdateData()
		{
			Timestamp? timestamp;

			if (this.stateAtController == null)
			{
				timestamp = null;
			}
			else
			{
				timestamp = new Timestamp (this.stateAtController.Date.Value, 0);
			}

			var sortingInstructions = new SortingInstructions (this.accessor.GetMainStringField (BaseType.Assets), SortedType.Ascending, ObjectField.Unknown, SortedType.None);

			this.nodeGetter.SetParams (timestamp, this.rootGuid, Guid.Empty, sortingInstructions);
			this.dataFiller.Timestamp = timestamp;

			this.UpdateDataArray ();
			this.UpdateScroller ();
			this.UpdateController ();
			this.UpdateToolbar ();
			this.UpdateWarningsRedDot ();
		}


		public Guid								SelectedGuid
		{
			get
			{
				if (this.selectedRow != -1)
				{
					var node = this.nodeGetter[this.selectedRow];
					if (!node.IsEmpty)
					{
						return node.Guid;
					}
				}

				return Guid.Empty;
			}
			set
			{
				var selectedRow = this.nodeGetter.GetNodes ().ToList ().FindIndex (x => x.Guid == value);
				this.SetSelection (selectedRow, this.selectedColumn);
			}
		}

		public Timestamp?						SelectedTimestamp
		{
			get
			{
				if (this.selectedColumn != -1)
				{
					var column = this.dataArray.GetColumn (this.selectedColumn);
					if (column != null)
					{
						return column.Timestamp;
					}
				}

				return null;
			}
			set
			{
				if (value.HasValue)
				{
					this.SetSelection (this.selectedRow, this.dataArray.FindColumnIndex (value.Value));
				}
			}
		}

		private void SetSelection(int selectedRow, int selectedColumn)
		{
			if (this.selectedRow != selectedRow || this.selectedColumn != selectedColumn)
			{
				this.selectedRow    = selectedRow;
				this.selectedColumn = selectedColumn;

				this.UpdateController ();
				this.UpdateToolbar ();
				this.OnSelectedCellChanged ();
			}
		}
	
		
		public void CreateUI(Widget parent)
		{
			var common = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			CommandDispatcher.SetDispatcher (common, this.commandDispatcher);  // nécesaire pour [Command (Res.CommandIds...)]

			this.topTitle = new TopTitle
			{
				Parent = common,
			};

			this.topTitle.SetTitle (this.Title);

			var box = new FrameBox
			{
				Parent = common,
				Dock   = DockStyle.Fill,
			};

			var leftBox = new FrameBox
			{
				Parent         = box,
				Dock           = DockStyle.Left,
				PreferredWidth = LocalSettings.SplitterAssetsMultiplePos,
			};

			this.splitter = new VSplitter
			{
				Parent         = box,
				Dock           = DockStyle.Left,
				PreferredWidth = 10,
			};

			var rightBox = new FrameBox
			{
				Parent = box,
				Dock   = DockStyle.Fill,
			};

			//	Partie gauche.
			this.assetsToolbar = new AssetsLeftToolbar (this.accessor, this.commandContext);
			this.assetsToolbar.CreateUI (leftBox);

			this.assetsToolbar.Search += delegate (object sender, SearchDefinition definition, int direction)
			{
				this.Search (definition, direction);
			};

			this.treeColumn = new TreeTableColumnTree
			{
				Parent            = leftBox,
				Dock              = DockStyle.Fill,
				IndependentColumn = true,
				DockToLeft        = true,  // pour avoir la couleur grise
				HoverMode         = TreeTableHoverMode.VerticalGradient,
				HeaderHeight      = TimelinesArrayController.lineHeight*2,
				FooterHeight      = 0,
				HeaderDescription = Res.Strings.TimelinesArrayController.Header.ToString (),
				RowHeight         = TimelinesArrayController.lineHeight,
				Margins           = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),
				VerticalAdjust    = -1,
			};

			this.CreateStateAt (leftBox);

			//	Partie droite.
			if (this.HasAmortizationsOper)
			{
				this.timelinesToolbar = new AmortizationToolbar (this.accessor, this.commandContext);
			}
			else
			{
				this.timelinesToolbar = new TimelinesToolbar (this.accessor, this.commandContext);
			}

			this.timelinesToolbar.CreateUI (rightBox);

			var bottomRightBox = new FrameBox
			{
				Parent = rightBox,
				Dock   = DockStyle.Fill,
			};

			var timelinesBox = new FrameBox
			{
				Parent = bottomRightBox,
				Dock   = DockStyle.Fill,
			};

			this.controller = new NavigationTimelineController ();
			this.controller.CreateUI (timelinesBox);

			this.scroller = new VScroller
			{
				Parent     = bottomRightBox,
				Dock       = DockStyle.Right,
				Margins    = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),
				IsInverted = true,  // le zéro est en haut
			};

			this.UpdateScroller ();
			this.UpdateController ();
			this.UpdateToolbar ();

			//	Connexion des événements.
			this.splitter.SplitterDragged += delegate
			{
				LocalSettings.SplitterAssetsMultiplePos = (int) leftBox.PreferredWidth;
			};

			{
				this.treeColumn.RowClicked += delegate (object sender, int row)
				{
					this.SetSelection (this.TopVisibleRow + row, this.selectedColumn);
				};

				this.treeColumn.TreeButtonClicked += delegate (object sender, int row, NodeType type)
				{
					this.OnAssetsCompactOrExpand (this.TopVisibleRow + row);
				};
			}

			{
				this.controller.ContentChanged += delegate (object sender, bool crop)
				{
					this.UpdateController (crop);
					this.UpdateToolbar ();
				};

				this.controller.CellClicked += delegate (object sender, int row, int rank)
				{
					int sel = this.LineToRow (row);
					if (sel != -1)
					{
						this.SetSelection (sel, this.controller.LeftVisibleCell + rank);
					}
				};

				this.controller.CellDoubleClicked += delegate (object sender, int row, int rank)
				{
					int sel = this.LineToRow (row);
					if (sel != -1)
					{
						this.OnCellDoubleClicked ();
					}
				};

				this.controller.CellRightClicked += delegate (object sender, int row, int rank, Point pos)
				{
					int sel = this.LineToRow (row);
					if (sel != -1)
					{
						this.SetSelection (sel, this.controller.LeftVisibleCell + rank);
						this.ShowContextMenu (pos);
					}
				};

				this.controller.DokeySelect += delegate (object sender, KeyCode key)
				{
					this.OnDokeySelect (key);
				};
			}

			{
				this.scroller.SizeChanged += delegate
				{
					this.UpdateScroller ();
					this.UpdateController ();
				};

				this.scroller.ValueChanged += delegate
				{
					this.UpdateController (crop: false);
					this.UpdateToolbar ();
				};
			}
		}

		private void CreateStateAt(Widget parent)
		{
			this.stateAtController = new StateAtController (this.accessor);
			var frame = this.stateAtController.CreateUI (parent);
			frame.Anchor = AnchorStyles.BottomLeft;

			this.stateAtController.Date = Timestamp.Now.Date;

			this.stateAtController.DateChanged += delegate
			{
				this.UpdateTreeColumn ();
			};
		}

		private void Search(SearchDefinition definition, int direction)
		{
			var row = FillerSearchEngine<SortableCumulNode>.Search (this.nodeGetter, this.dataFiller, definition, this.selectedRow, direction);

			if (row != -1)
			{
				this.SelectedGuid = this.nodeGetter.GetNodes ().ToList ()[row].Guid;
			}
		}

		public TimelinesMode					TimelinesMode
		{
			get
			{
				return this.timelinesMode;
			}
			set
			{
				if (this.timelinesMode != value)
				{
					var sel = this.SelectedTimestamp;

					this.timelinesMode = value;
					this.UpdateDataArray ();

					this.selectedColumn = -1;  // pour forcer la mise à jour
					this.SelectedTimestamp = sel;

					this.UpdateScroller ();
					this.UpdateController ();
					this.UpdateToolbar ();
				}
			}
		}


		private void ShowContextMenu(Point pos)
		{
			//	Affiche le menu contextuel.
			MenuPopup.Show (this.timelinesToolbar, this.scroller, pos,
				Res.Commands.Timelines.Amortizations.Preview,
				Res.Commands.Timelines.Amortizations.Fix,
				Res.Commands.Timelines.Amortizations.ToExtra,
				Res.Commands.Timelines.Amortizations.Unpreview,
				Res.Commands.Timelines.Amortizations.Delete,
				null,
				Res.Commands.Timelines.New,
				Res.Commands.Timelines.Delete,
				null,
				Res.Commands.Timelines.Copy,
				Res.Commands.Timelines.Paste);
		}

		private void OnDokeySelect(KeyCode key)
		{
			switch (key)
			{
				case KeyCode.Home:
					this.OnAssetsFirst ();
					this.OnTimelinesFirst ();
					break;
		
				case KeyCode.End:
					this.OnAssetsLast ();
					this.OnTimelinesLast ();
					break;
		
				case KeyCode.ArrowUp:
					this.OnAssetsPrev ();
					break;
		
				case KeyCode.ArrowDown:
					this.OnAssetsNext ();
					break;
		
				case KeyCode.ArrowLeft:
					this.OnTimelinesPrev ();
					break;
		
				case KeyCode.ArrowRight:
					this.OnTimelinesNext ();
					break;
			}
		}


		#region Assets commands
		[Command (Res.CommandIds.AssetsLeft.Filter)]
		private void OnAssetsFilter(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.assetsToolbar.GetTarget (e);
			var popup = new FilterPopup (this.accessor, this.rootGuid);

			popup.Create (target, leftOrRight: true);

			popup.Navigate += delegate (object sender, Guid guid)
			{
				this.rootGuid = guid;
				this.UpdateTreeColumn ();
			};
		}

		[Command (Res.CommandIds.AssetsLeft.First)]
		private void OnAssetsFirst()
		{
			var index = this.FirstRowIndex;

			if (index.HasValue)
			{
				this.SetSelection (index.Value, this.selectedColumn);
			}
		}

		[Command (Res.CommandIds.AssetsLeft.Prev)]
		private void OnAssetsPrev()
		{
			var index = this.PrevRowIndex;

			if (index.HasValue)
			{
				this.SetSelection (index.Value, this.selectedColumn);
			}
		}

		[Command (Res.CommandIds.AssetsLeft.Next)]
		private void OnAssetsNext()
		{
			var index = this.NextRowIndex;

			if (index.HasValue)
			{
				this.SetSelection (index.Value, this.selectedColumn);
			}
		}

		[Command (Res.CommandIds.AssetsLeft.Last)]
		private void OnAssetsLast()
		{
			var index = this.LastRowIndex;

			if (index.HasValue)
			{
				this.SetSelection (index.Value, this.selectedColumn);
			}
		}

		[Command (Res.CommandIds.AssetsLeft.New)]
		private void OnAssetsNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.assetsToolbar.GetTarget (e);
			this.ShowCreatePopup (target);
		}

		[Command (Res.CommandIds.AssetsLeft.Delete)]
		private void OnAssetsDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.assetsToolbar.GetTarget (e);
			YesNoPopup.Show (target, Res.Strings.TimelinesArrayController.ObjectDelete.ToString (), this.AssetsDeleteSelection);
		}

		private void AssetsDeleteSelection()
		{
			this.accessor.RemoveObject (BaseType.Assets, this.SelectedGuid);
			this.UpdateData ();
		}

		[Command (Res.CommandIds.AssetsLeft.Deselect)]
		private void OnAssetsDeselect()
		{
			this.SetSelection (-1, -1);
		}

		private void OnAssetsCompactOrExpand(int row)
		{
			//	Etend ou compacte une ligne (inverse son mode actuel).
			var guid = this.SelectedGuid;
			var timestamp = this.SelectedTimestamp;

			this.nodeGetter.CompactOrExpand (row);
			this.UpdateDataArray ();
			this.UpdateScroller ();
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
			this.SelectedTimestamp = timestamp;
		}

		[Command (Res.CommandIds.AssetsLeft.CompactAll)]
		protected void OnAssetsCompactAll()
		{
			//	Compacte toutes les lignes.
			var guid = this.SelectedGuid;
			var timestamp = this.SelectedTimestamp;

			this.nodeGetter.CompactAll ();
			this.UpdateDataArray ();
			this.UpdateScroller ();
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
			this.SelectedTimestamp = timestamp;
		}

		[Command (Res.CommandIds.AssetsLeft.CompactOne)]
		protected void OnAssetsCompactOne()
		{
			//	Compacte une ligne.
			var guid = this.SelectedGuid;
			var timestamp = this.SelectedTimestamp;

			this.nodeGetter.CompactOne ();
			this.UpdateDataArray ();
			this.UpdateScroller ();
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
			this.SelectedTimestamp = timestamp;
		}

		[Command (Res.CommandIds.AssetsLeft.ExpandOne)]
		protected void OnAssetsExpandOne()
		{
			//	Compacte une ligne.
			var guid = this.SelectedGuid;
			var timestamp = this.SelectedTimestamp;

			this.nodeGetter.ExpandOne ();
			this.UpdateDataArray ();
			this.UpdateScroller ();
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
			this.SelectedTimestamp = timestamp;
		}

		[Command (Res.CommandIds.AssetsLeft.ExpandAll)]
		protected void OnAssetsExpandAll()
		{
			//	Etend toutes les lignes.
			var guid = this.SelectedGuid;
			var timestamp = this.SelectedTimestamp;

			this.nodeGetter.ExpandAll ();
			this.UpdateDataArray ();
			this.UpdateScroller ();
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
			this.SelectedTimestamp = timestamp;
		}


		private void ShowCreatePopup(Widget target)
		{
			CreateAssetPopup.Show (target, this.accessor, delegate (System.DateTime date, IEnumerable<AbstractDataProperty> requiredProperties, decimal? value, Guid cat)
			{
				this.CreateAsset (date, requiredProperties, value, cat);
			});
		}

		private void CreateAsset(System.DateTime date, IEnumerable<AbstractDataProperty> requiredProperties, decimal? value, Guid cat)
		{
			var asset = AssetsLogic.CreateAsset (this.accessor, date, requiredProperties, value, cat);
			var guid = asset.Guid;

			this.UpdateData ();

			this.SelectedGuid = guid;
			this.SelectedTimestamp = AssetCalculator.GetLastTimestamp (asset);

			this.OnStartEditing (EventType.Input, this.SelectedTimestamp.GetValueOrDefault ());
		}
		#endregion


		#region Timeline commands
		[Command (Res.CommandIds.Timelines.Narrow)]
		private void OnTimelinesNarrow()
		{
			this.TimelinesMode = TimelinesMode.Narrow;
			this.UpdateToolbar ();
		}

		[Command (Res.CommandIds.Timelines.Wide)]
		private void OnTimelinesWide()
		{
			this.TimelinesMode = TimelinesMode.Wide;
			this.UpdateToolbar ();
		}

		[Command (Res.CommandIds.Timelines.GroupedByMonth)]
		private void OnTimelinesGroupedByMonth()
		{
			this.TimelinesMode = TimelinesMode.GroupedByMonth;
			this.UpdateToolbar ();
		}

		[Command (Res.CommandIds.Timelines.GroupedByTrim)]
		private void OnTimelinesGroupedByTrim()
		{
			this.TimelinesMode = TimelinesMode.GroupedByTrim;
			this.UpdateToolbar ();
		}

		[Command (Res.CommandIds.Timelines.GroupedByYear)]
		private void OnTimelinesGroupedByYear()
		{
			this.TimelinesMode = TimelinesMode.GroupedByYear;
			this.UpdateToolbar ();
		}

		[Command (Res.CommandIds.Timelines.First)]
		private void OnTimelinesFirst()
		{
			var index = this.FirstColumnIndex;

			if (index.HasValue)
			{
				this.SetSelection (this.selectedRow, index.Value);
			}
		}

		[Command (Res.CommandIds.Timelines.Prev)]
		private void OnTimelinesPrev()
		{
			var index = this.PrevColumnIndex;

			if (index.HasValue)
			{
				this.SetSelection (this.selectedRow, index.Value);
			}
		}

		[Command (Res.CommandIds.Timelines.Next)]
		private void OnTimelinesNext()
		{
			var index = this.NextColumnIndex;

			if (index.HasValue)
			{
				this.SetSelection (this.selectedRow, index.Value);
			}
		}

		[Command (Res.CommandIds.Timelines.Last)]
		private void OnTimelinesLast()
		{
			var index = this.LastColumnIndex;

			if (index.HasValue)
			{
				this.SetSelection (this.selectedRow, index.Value);
			}
		}

		[Command (Res.CommandIds.Timelines.New)]
		private void OnTimelinesNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var timestamp = this.SelectedTimestamp;

			if (timestamp.HasValue)
			{
				var target = this.timelinesToolbar.GetTarget (e);
				var obj = this.accessor.GetObject (BaseType.Assets, this.SelectedGuid);

				CreateEventPopup.Show (target, this.accessor, BaseType.Assets, obj, timestamp.Value,
				timestampChanged: delegate (Timestamp? t)
				{
					this.SelectedTimestamp = t;
				},
				action: delegate (System.DateTime date, string name)
				{
					this.CreateEvent (date, name);
				});
			}
		}

		[Command (Res.CommandIds.Timelines.Delete)]
		private void OnTimelines(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (this.SelectedEventType == EventType.AmortizationPreview)
			{
				//	Si on supprime un événement AmortizationPreview, on ne demande pas de confirmation.
				this.TimelineDeleteSelection ();
			}
			else
			{
				var target = this.timelinesToolbar.GetTarget (e);

				if (AssetCalculator.IsLocked (this.SelectedObject, this.SelectedTimestamp.GetValueOrDefault ()))
				{
					MessagePopup.ShowAssetsDeleteEventWarning (target);
				}
				else
				{
					YesNoPopup.ShowAssetsDeleteEventQuestion (target, this.TimelineDeleteSelection);
				}
			}
		}

		[Command (Res.CommandIds.Timelines.Amortizations.Preview)]
		private void OnTimelinesAmortizationsPreview(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.timelinesToolbar.GetTarget (e);

			this.ShowAmortizationsPopup (target, true, true,
				Res.Strings.Popup.Amortizations.Preview.Title.ToString (),
				Res.Strings.Popup.Amortizations.Preview.One.ToString (),
				Res.Strings.Popup.Amortizations.Preview.All.ToString (),
				this.DoAmortisationsPreview);
		}

		[Command (Res.CommandIds.Timelines.Amortizations.Fix)]
		private void OnTimelinesAmortizationsFix(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.timelinesToolbar.GetTarget (e);

			this.ShowAmortizationsPopup (target, false, false,
				Res.Strings.Popup.Amortizations.Fix.Title.ToString (),
				Res.Strings.Popup.Amortizations.Fix.One.ToString (),
				Res.Strings.Popup.Amortizations.Fix.All.ToString (),
				this.DoAmortisationsFix);
		}

		[Command (Res.CommandIds.Timelines.Amortizations.ToExtra)]
		private void OnTimelinesAmortizationsToExtra(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			//	Transforme un amortissement ordinaire en extraordinaire.
			if (!this.SelectedGuid.IsEmpty && this.SelectedTimestamp.HasValue)
			{
				var asset = this.accessor.GetObject (BaseType.Assets, this.SelectedGuid);
				if (asset != null)
				{
					var ev = asset.GetEvent (this.SelectedTimestamp.Value);
					if (ev != null)
					{
						//	Supprime l'amortissement ordinaire.
						asset.RemoveEvent (ev);

						//	Crée un amortissement extraordinaire.
						var newEvent = new DataEvent (ev.Guid, ev.Timestamp, EventType.AmortizationExtra);
						newEvent.SetProperties (ev);
						asset.AddEvent (newEvent);

						this.UpdateData ();
						this.OnStopEditing ();
					}
				}
			}
		}

		[Command (Res.CommandIds.Timelines.Amortizations.Unpreview)]
		private void OnTimelinesAmortizationsUnpreview(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.timelinesToolbar.GetTarget (e);

			this.ShowAmortizationsPopup (target, false, false,
				Res.Strings.Popup.Amortizations.Unpreview.Title.ToString (),
				Res.Strings.Popup.Amortizations.Unpreview.One.ToString (),
				Res.Strings.Popup.Amortizations.Unpreview.All.ToString (),
				this.DoAmortisationsUnpreview);
		}

		[Command (Res.CommandIds.Timelines.Amortizations.Delete)]
		private void OnTimelinesAmortizationsDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.timelinesToolbar.GetTarget (e);

			this.ShowAmortizationsPopup (target, true, false,
				Res.Strings.Popup.Amortizations.Delete.Title.ToString (),
				Res.Strings.Popup.Amortizations.Delete.One.ToString (),
				Res.Strings.Popup.Amortizations.Delete.All.ToString (),
				this.DoAmortisationsDelete);
		}

		[Command (Res.CommandIds.Timelines.Deselect)]
		private void OnTimelinesDeselect()
		{
			this.SetSelection (this.selectedRow, -1);
		}

		[Command (Res.CommandIds.Timelines.Copy)]
		private void OnTimelinesCopy(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.timelinesToolbar.GetTarget (e);
			var obj = this.SelectedObject;

			if (obj != null && this.SelectedTimestamp.HasValue)
			{
				var ev = obj.GetEvent (this.SelectedTimestamp.Value);
				this.accessor.Clipboard.CopyEvent (this.accessor, ev);

				this.UpdateToolbar ();
			}
			else
			{
				MessagePopup.ShowError (target, Res.Strings.TimelinesArrayController.CopyError.ToString ());
			}
		}

		[Command (Res.CommandIds.Timelines.Paste)]
		private void OnTimelinesPaste(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.timelinesToolbar.GetTarget (e);
			var obj = this.SelectedObject;

			if (obj != null && this.accessor.Clipboard.HasEvent)
			{
				EventPastePopup.Show (target, this.accessor, obj,
				this.accessor.Clipboard.EventType,
				this.accessor.Clipboard.EventTimestamp.Value.Date,
				dateChanged: delegate (System.DateTime? date)
				{
					if (date.HasValue)
					{
						this.SelectedTimestamp = new Timestamp (date.Value, 0);
					}
					else
					{
						this.SelectedTimestamp = null;
					}
				},
				action: delegate (System.DateTime date)
				{
					var ev = this.accessor.Clipboard.PasteEvent (this.accessor, obj, date);

					if (ev == null)
					{
						MessagePopup.ShowError (target, Res.Strings.TimelinesArrayController.PasteIncompatible.ToString ());
					}
					else
					{
						this.UpdateDataArray ();
						this.UpdateScroller ();
						this.UpdateController ();
						this.UpdateToolbar ();
						this.SetSelection (this.selectedRow, this.dataArray.FindColumnIndex (ev.Timestamp));
						//?this.OnStartEditing (e.Type, e.Timestamp);
					}
				});
			}
			else
			{
				MessagePopup.ShowError (target, Res.Strings.TimelinesArrayController.PasteError.ToString ());
			}
		}

		private EventType SelectedEventType
		{
			//	Retourne le type de l'événement sélectionné.
			get
			{
				var obj = this.accessor.GetObject (BaseType.Assets, this.SelectedGuid);
				if (obj != null && this.SelectedTimestamp.HasValue)
				{
					var e = obj.GetEvent (this.SelectedTimestamp.Value);
					if (e != null)
					{
						return e.Type;
					}
				}

				return EventType.Unknown;
			}
		}

		private void TimelineDeleteSelection()
		{
			this.accessor.RemoveObjectEvent (this.SelectedObject, this.SelectedTimestamp);
			this.UpdateData ();
		}

		private void ShowAmortizationsPopup(Widget target, bool fromAllowed, bool toAllowed, string title, string one, string all, System.Action<DateRange, bool> action)
		{
			var popup = new AmortizationsPopup (this.accessor)
			{
				Title               = title,
				ActionOne           = one,
				ActionAll           = all,
				DateFromAllowed     = fromAllowed,
				DateToAllowed       = toAllowed,
				OneSelectionAllowed = !this.SelectedGuid.IsEmpty,
				IsAll               =  this.SelectedGuid.IsEmpty,
				DateFrom            = LocalSettings.AmortizationDateFrom,
				DateTo              = LocalSettings.AmortizationDateTo,
			};

			popup.Create (target);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					System.Diagnostics.Debug.Assert (popup.DateFrom.HasValue);
					System.Diagnostics.Debug.Assert (popup.DateTo.HasValue);
					var range = new DateRange (popup.DateFrom.Value, popup.DateTo.Value.AddDays (1));

					LocalSettings.AmortizationDateFrom = popup.DateFrom.Value;
					LocalSettings.AmortizationDateTo   = popup.DateTo.Value;

					action (range, popup.IsAll);
				}
			};
		}

		private void DoAmortisationsPreview(DateRange processRange, bool allObjects)
		{
			if (allObjects)
			{
				this.amortizations.Preview (processRange);
			}
			else
			{
				this.amortizations.Create (processRange, this.SelectedGuid);
			}

			this.UpdateData ();
			this.OnStopEditing ();
		}

		private void DoAmortisationsFix(DateRange processRange, bool allObjects)
		{
			if (allObjects)
			{
				this.amortizations.Fix ();
			}
			else
			{
				this.amortizations.Fix (this.SelectedGuid);
			}

			this.UpdateData ();
			this.OnStopEditing ();
		}

		private void DoAmortisationsUnpreview(DateRange processRange, bool allObjects)
		{
			if (allObjects)
			{
				this.amortizations.Unpreview ();
			}
			else
			{
				this.amortizations.Unpreview (this.SelectedGuid);
			}

			this.UpdateData ();
			this.OnStopEditing ();
		}

		private void DoAmortisationsDelete(DateRange processRange, bool allObjects)
		{
			if (allObjects)
			{
				this.amortizations.Delete (processRange.IncludeFrom);
			}
			else
			{
				this.amortizations.Delete (processRange.IncludeFrom, this.SelectedGuid);
			}

			this.UpdateData ();
			this.OnStopEditing ();
		}
		#endregion


		private void CreateEvent(System.DateTime date, string buttonName)
		{
			var obj = this.accessor.GetObject (BaseType.Assets, this.SelectedGuid);
			if (obj != null)
			{
				var type = TimelinesArrayController.ParseEventType (buttonName);
				var e = this.accessor.CreateAssetEvent (obj, date, type);

				if (e != null)
				{
					this.UpdateDataArray ();
					this.UpdateScroller ();
					this.UpdateController ();
					this.UpdateToolbar ();

					this.SetSelection (this.selectedRow, this.dataArray.FindColumnIndex (e.Timestamp));
				}

				this.OnStartEditing (e.Type, e.Timestamp);
			}
		}

		private static EventType ParseEventType(string text)
		{
			var type = EventType.Unknown;
			System.Enum.TryParse<EventType> (text, out type);
			return type;
		}


		private void UpdateController(bool crop = true)
		{
			this.UpdateTree (crop);
			this.UpdateTimelines (crop);
		}


		private void UpdateTree(bool crop = true)
		{
			this.treeColumn.HeaderHeight = TimelinesArrayController.lineHeight * this.HeaderLinesCount;
			
			int visibleCount = this.VisibleRows;
			int rowsCount    = this.dataArray.RowsCount;
			int count        = System.Math.Min (visibleCount, rowsCount);
			int firstRow     = this.TopVisibleRow;
			int selection    = this.selectedRow;

			if (selection != -1)
			{
				//	La sélection ne peut pas dépasser le nombre maximal de lignes.
				selection = System.Math.Min (selection, rowsCount-1);

				//	Si la sélection est hors de la zone visible, on choisit un autre cadrage.
				if (crop && (selection < firstRow || selection >= firstRow+count))
				{
					firstRow = this.GetTopVisibleRow (selection);
				}

				if (this.TopVisibleRow != firstRow)
				{
					this.TopVisibleRow = firstRow;
				}

				selection -= this.TopVisibleRow;
			}
	
			var contentItem = this.dataFiller.GetContent (firstRow, count, selection);
			this.treeColumn.SetCells (contentItem.Columns.First ());
		}

		private int GetTopVisibleRow(int sel)
		{
			//	Il faut forcer le calcul du layout pour pouvoir calculer le nombre
			//	de lignes visibles. Ceci met à jour la hauteur (ActualHeight).
			if (this.topTitle.Window != null)
			{
				this.topTitle.Window.ForceLayout ();
			}

			int visibleRows = this.VisibleRows;
			System.Diagnostics.Debug.Assert (visibleRows > 0);
			int count = System.Math.Min (visibleRows, this.dataArray.RowsCount);

			sel = System.Math.Min (sel + count/2, this.dataArray.RowsCount-1);
			sel = System.Math.Max (sel - count+1, 0);

			return sel;
		}


		private void UpdateTimelines(bool crop = true)
		{
			this.controller.TopRowsWithExactHeight = this.HeaderLinesCount;
			this.controller.RelativeWidth = (this.timelinesMode == TimelinesMode.Narrow) ? 1.0 : 2.0;
			this.controller.SetRows (this.TimelineRows);
			this.controller.CellsCount = this.dataArray.ColumnsCount;

			int visibleCount = this.controller.VisibleCellsCount;
			int cellsCount   = this.dataArray.ColumnsCount;
			int count        = System.Math.Min (visibleCount, cellsCount);
			int firstCell    = this.controller.LeftVisibleCell;
			int selection    = this.selectedColumn;

			if (selection != -1)
			{
				//	La sélection ne peut pas dépasser le nombre maximal de cellules.
				selection = System.Math.Min (selection, cellsCount-1);

				//	Si la sélection est hors de la zone visible, on choisit un autre cadrage.
				if (crop && (selection < firstCell || selection >= firstCell+count))
				{
					firstCell = this.controller.GetLeftVisibleCell (selection);
				}

				if (this.controller.LeftVisibleCell != firstCell)
				{
					this.controller.LeftVisibleCell = firstCell;
				}

				selection -= this.controller.LeftVisibleCell;
			}

			int line = 0;

			//	Ajoute les lignes vides bidon.
			int dummy = this.DummyCount;
			for (int row=0; row<dummy; row++)
			{
				var glyphs = new List<TimelineCellGlyph> ();
				this.controller.SetRowGlyphCells (line++, glyphs.ToArray ());
			}

			//	Ajoute les lignes des objets, de bas en haut.
			foreach (var row in this.EnumVisibleRows.Reverse ())
			{
				var glyphs = new List<TimelineCellGlyph> ();

				for (int i = 0; i < count; i++)
				{
					var cell = this.dataArray.GetCell (row, firstCell+i);
					bool selected = (row == this.selectedRow && firstCell+i == this.selectedColumn);

					var g = new TimelineCellGlyph (cell.Glyphs, cell.Flags, cell.Tooltip, selected);
					glyphs.Add (g);
				}

				this.controller.SetRowGlyphCells (line++, glyphs.ToArray ());
			}

			//	Ajoute les 2 lignes supérieures pour les dates.
			var dates = new List<TimelineCellDate> ();

			for (int i = 0; i < count; i++)
			{
				var column = this.dataArray.GetColumn (firstCell+i);
				if (column != null)
				{
					var d = new TimelineCellDate (column.Timestamp.Date, column.GroupedMode);
					dates.Add (d);
				}
			}

			if (this.timelinesMode == TimelinesMode.Narrow)
			{
				this.controller.SetRowDayCells   (line++, dates.ToArray ());
				this.controller.SetRowMonthCells (line++, dates.ToArray ());
				this.controller.SetRowYearCells  (line++, dates.ToArray ());
			}
			else if (TimelinesArrayLogic.IsGrouped (this.timelinesMode))
			{
				this.controller.SetRowDayMonthCells (line++, dates.ToArray ());
				this.controller.SetRowYearCells     (line++, dates.ToArray ());
			}
			else
			{
				this.controller.SetRowDayMonthCells (line++, dates.ToArray ());
				this.controller.SetRowYearCells     (line++, dates.ToArray ());
			}

			this.controller.PermanentGrid = true;
		}

		private TimelineRowDescription[] TimelineRows
		{
			//	Retourne les descriptions des lignes, de bas en haut.
			get
			{
				var list = new List<TimelineRowDescription> ();

				int dummy = this.DummyCount;
				for (int row=0; row<dummy; row++)
				{
					list.Add (new TimelineRowDescription (TimelineRowType.Glyph, null));
				}

				foreach (var row in this.EnumVisibleRows.Reverse ())
				{
					string desc = this.dataArray.RowsLabel[row];
					list.Add (new TimelineRowDescription (TimelineRowType.Glyph, desc));
				}

				if (this.timelinesMode == TimelinesMode.Narrow)
				{
					list.Add (new TimelineRowDescription (TimelineRowType.Days,   Res.Strings.TimelinesArrayController.Row.Days.ToString ()));
					list.Add (new TimelineRowDescription (TimelineRowType.Months, Res.Strings.TimelinesArrayController.Row.Months.ToString ()));
					list.Add (new TimelineRowDescription (TimelineRowType.Years,  Res.Strings.TimelinesArrayController.Row.Years.ToString ()));
				}
				else if (TimelinesArrayLogic.IsGrouped (this.timelinesMode))
				{
					list.Add (new TimelineRowDescription (TimelineRowType.DaysMonths, Res.Strings.TimelinesArrayController.Row.DaysMonths.ToString ()));
					list.Add (new TimelineRowDescription (TimelineRowType.Years,      Res.Strings.TimelinesArrayController.Row.Years.ToString ()));
				}
				else
				{
					list.Add (new TimelineRowDescription (TimelineRowType.DaysMonths, Res.Strings.TimelinesArrayController.Row.DaysMonths.ToString ()));
					list.Add (new TimelineRowDescription (TimelineRowType.Years,      Res.Strings.TimelinesArrayController.Row.Years.ToString ()));
				}

				return list.ToArray ();
			}
		}


		private void UpdateTreeColumn()
		{
			var selectedGuid = this.SelectedGuid;
			var sel = this.SelectedTimestamp;
			{
				this.UpdateData ();
			}
			this.SelectedGuid = selectedGuid;
			this.selectedColumn = -1;  // pour forcer la mise à jour
			this.SelectedTimestamp = sel;
		}


		private void UpdateScroller()
		{
			if (this.scroller == null)
			{
				return;
			}

			var totalRows   = (decimal) this.nodeGetter.Count;
			var visibleRows = (decimal) this.VisibleRows;

			if (visibleRows < 0 || totalRows == 0)
			{
				this.scroller.Resolution = 1.0m;
				this.scroller.VisibleRangeRatio = 1.0m;

				this.scroller.MinValue = 0.0m;
				this.scroller.MaxValue = 1.0m;

				this.scroller.SmallChange = 1.0m;
				this.scroller.LargeChange = 1.0m;
			}
			else
			{
				this.scroller.Resolution = 1.0m;
				this.scroller.VisibleRangeRatio = System.Math.Min (visibleRows/totalRows, 1.0m);

				this.scroller.MinValue = 0.0m;
				this.scroller.MaxValue = System.Math.Max (totalRows - visibleRows, 0.0m);

				this.scroller.SmallChange = 1.0m;
				this.scroller.LargeChange = visibleRows;
			}
		}

		private IEnumerable<int> EnumVisibleRows
		{
			get
			{
				int firstRow    = this.TopVisibleRow;
				int visibleRows = this.VisibleRows;
				int count = System.Math.Min (this.nodeGetter.Count, firstRow+visibleRows);

				for (int row=firstRow; row<count; row++)
				{
					yield return row;
				}
			}
		}

		private int LineToRow(int line)
		{
			var dummy = this.DummyCount;
			int count = System.Math.Min (this.nodeGetter.Count, this.VisibleRows);

			if (line >= dummy && line < dummy+count)
			{
				return this.TopVisibleRow + count + dummy - line - 1;
			}
			else
			{
				return -1;
			}
		}

		private int DummyCount
		{
			//	Retourne le nombre de lignes vides à ajouter en bas des timelines,
			//	pour occuper l'espace vide.
			get
			{
				return System.Math.Max (this.VisibleRows - this.nodeGetter.Count, 0);
			}
		}

		private int TopVisibleRow
		{
			get
			{
				return (int) this.scroller.Value;
			}
			set
			{
				this.scroller.Value = value;
			}
		}

		private int VisibleRows
		{
			get
			{
				return (int) (this.scroller.ActualHeight / TimelinesArrayController.lineHeight)
					 - this.HeaderLinesCount;
			}
		}

		private int HeaderLinesCount
		{
			get
			{
				if (this.timelinesMode == TimelinesMode.Narrow)
				{
					return 3;  // Years, Months, Days
				}
				else if (TimelinesArrayLogic.IsGrouped (this.timelinesMode))
				{
					return 2;  // Years, DaysMonths
				}
				else
				{
					return 2;  // Years, DaysMonths
				}
			}
		}


		private void UpdateDataArray()
		{
			//	Met à jour this.dataArray en fonction de l'ensemble des événements de
			//	tous les objets. Cela nécessite d'accéder à l'ensemble des données, ce
			//	qui peut être long. Néanmoins, cela est nécessaire, même si la timeline
			//	n'affiche qu'un nombre limité de lignes. En effet, il faut allouer toutes
			//	les colonnes pour lesquelles il existe un événement.
			System.DateTime date;

			if (this.stateAtController.Date.HasValue)
			{
				date = this.stateAtController.Date.Value;
			}
			else
			{
				date = Timestamp.Now.Date;
			}

			var fromDate = new System.DateTime (date.Year,   1, 1);  // date inclue
			var toDate   = new System.DateTime (date.Year+1, 1, 1);  // date exclue
			var groupedExcludeRange = new DateRange (fromDate, toDate);

			this.arrayLogic.Update (this.dataArray, this.nodeGetter, this.timelinesMode, groupedExcludeRange, this.Filter);
		}


		private void UpdateWarningsRedDot()
		{
			//	Met à jour le nombre d'avertissements dans la pastille rouge sur le
			//	bouton de la vue des avertissements.
			//	ATTENTION: Il faut construire la liste complète des avertissements,
			//	ce qui peut prendre du temps !
			//	TODO: Rendre cela asynchrone !?
			if (this.accessor.WarningsDirty)
			{
				var list = new List<Warning> ();
				WarningsLogic.GetWarnings (list, this.accessor);

				this.mainToolbar.WarningsRedDotCount = list.Count;

				this.accessor.WarningsDirty = false;
			}
		}

		private void UpdateToolbar()
		{
			//	Mise à jour de la toolbar des objets.
			this.assetsToolbar.SetActiveState (Res.Commands.AssetsLeft.Filter, !this.rootGuid.IsEmpty);

			this.UpdateAssetsCommand (Res.Commands.AssetsLeft.First, this.selectedRow, this.FirstRowIndex);
			this.UpdateAssetsCommand (Res.Commands.AssetsLeft.Prev,  this.selectedRow, this.PrevRowIndex);
			this.UpdateAssetsCommand (Res.Commands.AssetsLeft.Next,  this.selectedRow, this.NextRowIndex);
			this.UpdateAssetsCommand (Res.Commands.AssetsLeft.Last,  this.selectedRow, this.LastRowIndex);

			bool compactEnable = !this.nodeGetter.IsAllCompacted;
			bool expandEnable  = !this.nodeGetter.IsAllExpanded;

			this.assetsToolbar.SetEnable (Res.Commands.AssetsLeft.CompactAll, compactEnable);
			this.assetsToolbar.SetEnable (Res.Commands.AssetsLeft.CompactOne, compactEnable);
			this.assetsToolbar.SetEnable (Res.Commands.AssetsLeft.ExpandOne,  expandEnable);
			this.assetsToolbar.SetEnable (Res.Commands.AssetsLeft.ExpandAll,  expandEnable);

			this.assetsToolbar.SetEnable (Res.Commands.AssetsLeft.New,      true);
			this.assetsToolbar.SetEnable (Res.Commands.AssetsLeft.Delete,   this.SelectedObject != null);
			this.assetsToolbar.SetEnable (Res.Commands.AssetsLeft.Deselect, this.selectedRow != -1);

			//	Mise à jour de la toolbar des timelines.
			this.timelinesToolbar.SetActiveState (Res.Commands.Timelines.Narrow,         this.timelinesMode == TimelinesMode.Narrow);
			this.timelinesToolbar.SetActiveState (Res.Commands.Timelines.Wide,           this.timelinesMode == TimelinesMode.Wide);
			this.timelinesToolbar.SetActiveState (Res.Commands.Timelines.GroupedByMonth, this.timelinesMode == TimelinesMode.GroupedByMonth);
			this.timelinesToolbar.SetActiveState (Res.Commands.Timelines.GroupedByTrim,  this.timelinesMode == TimelinesMode.GroupedByTrim);
			this.timelinesToolbar.SetActiveState (Res.Commands.Timelines.GroupedByYear,  this.timelinesMode == TimelinesMode.GroupedByYear);

			this.UpdateTimelineCommand (Res.Commands.Timelines.First, this.selectedColumn, this.FirstColumnIndex);
			this.UpdateTimelineCommand (Res.Commands.Timelines.Prev,  this.selectedColumn, this.PrevColumnIndex);
			this.UpdateTimelineCommand (Res.Commands.Timelines.Next,  this.selectedColumn, this.NextColumnIndex);
			this.UpdateTimelineCommand (Res.Commands.Timelines.Last,  this.selectedColumn, this.LastColumnIndex);

			this.timelinesToolbar.SetVisibility (Res.Commands.Timelines.New,                    !this.HasAmortizationsOper);
			this.timelinesToolbar.SetVisibility (Res.Commands.Timelines.Delete,                 !this.HasAmortizationsOper);
			this.timelinesToolbar.SetVisibility (Res.Commands.Timelines.Amortizations.Preview,   this.HasAmortizationsOper);
			this.timelinesToolbar.SetVisibility (Res.Commands.Timelines.Amortizations.Fix,       this.HasAmortizationsOper);
			this.timelinesToolbar.SetVisibility (Res.Commands.Timelines.Amortizations.ToExtra,   this.HasAmortizationsOper);
			this.timelinesToolbar.SetVisibility (Res.Commands.Timelines.Amortizations.Unpreview, this.HasAmortizationsOper);
			this.timelinesToolbar.SetVisibility (Res.Commands.Timelines.Amortizations.Delete,    this.HasAmortizationsOper);

			if (this.HasAmortizationsOper)
			{
				this.timelinesToolbar.SetEnable (Res.Commands.Timelines.Amortizations.Preview,   true);
				this.timelinesToolbar.SetEnable (Res.Commands.Timelines.Amortizations.Fix,       true);
				this.timelinesToolbar.SetEnable (Res.Commands.Timelines.Amortizations.ToExtra,   this.IsToExtraPossible);
				this.timelinesToolbar.SetEnable (Res.Commands.Timelines.Amortizations.Unpreview, true);
				this.timelinesToolbar.SetEnable (Res.Commands.Timelines.Amortizations.Delete,    true);
			}
			else
			{
				this.timelinesToolbar.SetEnable (Res.Commands.Timelines.New,      this.selectedColumn != -1 && this.HasSelectedTimeline);
				this.timelinesToolbar.SetEnable (Res.Commands.Timelines.Delete,   this.HasSelectedEvent);
				this.timelinesToolbar.SetEnable (Res.Commands.Timelines.Deselect, this.selectedColumn != -1);
			}

			this.timelinesToolbar.SetEnable (Res.Commands.Timelines.Copy,  this.HasSelectedEvent);
			this.timelinesToolbar.SetEnable (Res.Commands.Timelines.Paste, this.accessor.Clipboard.HasEvent);
		}

		private void UpdateAssetsCommand(Command command, int currentSelection, int? newSelection)
		{
			bool enable = (newSelection.HasValue && currentSelection != newSelection.Value);
			this.assetsToolbar.SetEnable (command, enable);
		}

		private void UpdateTimelineCommand(Command command, int currentSelection, int? newSelection)
		{
			bool enable = (newSelection.HasValue && currentSelection != newSelection.Value);
			this.timelinesToolbar.SetEnable (command, enable);
		}


		private bool IsToExtraPossible
		{
			get
			{
				if (!this.SelectedGuid.IsEmpty && this.SelectedTimestamp.HasValue)
				{
					var asset = this.accessor.GetObject (BaseType.Assets, this.SelectedGuid);
					if (asset != null)
					{
						var e = asset.GetEvent (this.SelectedTimestamp.Value);
						if (e != null)
						{
							return e.Type == EventType.AmortizationPreview
								|| e.Type == EventType.AmortizationAuto;
						}
					}
				}

				return false;
			}
		}

		private bool HasSelectedTimeline
		{
			get
			{
				if (this.selectedRow != -1)
				{
					var node = this.nodeGetter[this.selectedRow];
					if (!node.IsEmpty)
					{
						return node.BaseType == BaseType.Assets;
					}
				}

				return false;
			}
		}

		public bool HasSelectedEvent
		{
			get
			{
				var column = this.dataArray.GetColumn (this.selectedColumn);
				if (column != null)
				{
					if (this.selectedRow != -1)
					{
						var cell = column[this.selectedRow];
						return !cell.IsEmpty && cell.Glyphs.Count == 1;
					}
				}

				return false;
			}
		}


		#region Objects
		protected int? FirstRowIndex
		{
			get
			{
				return 0;
			}
		}

		protected int? PrevRowIndex
		{
			get
			{
				if (this.selectedRow == -1)
				{
					return null;
				}
				else
				{
					int i = this.selectedRow - 1;
					i = System.Math.Max (i, 0);
					i = System.Math.Min (i, this.nodeGetter.Count - 1);
					return i;
				}
			}
		}

		protected int? NextRowIndex
		{
			get
			{
				if (this.selectedRow == -1)
				{
					return null;
				}
				else
				{
					int i = this.selectedRow + 1;
					i = System.Math.Max (i, 0);
					i = System.Math.Min (i, this.nodeGetter.Count - 1);
					return i;
				}
			}
		}

		protected int? LastRowIndex
		{
			get
			{
				return this.nodeGetter.Count - 1;
			}
		}
		#endregion


		#region Timeline
		private int? FirstColumnIndex
		{
			get
			{
				if (this.HasSelectedTimeline)
				{
					var obj = this.SelectedObject;

					if (this.selectedColumn == -1)
					{
						if (obj != null && this.GetFilteredEvents (obj).Any ())
						{
							var timestamp = this.GetFilteredEvents (obj).First ().Timestamp;
							return this.dataArray.FindColumnIndex (timestamp);
						}
					}
					else
					{
						if (this.PrevColumnIndex.HasValue)
						{
							if (obj != null && this.GetFilteredEvents (obj).Any ())
							{
								var timestamp = this.GetFilteredEvents (obj).First ().Timestamp;
								return this.dataArray.FindColumnIndex (timestamp);
							}
						}
					}
				}

				return null;
			}
		}

		private int? PrevColumnIndex
		{
			get
			{
				if (this.HasSelectedTimeline)
				{
					var obj = this.SelectedObject;
					if (obj != null && this.GetFilteredEvents (obj).Any ())
					{
						var column = this.dataArray.GetColumn (this.selectedColumn);
						if (column != null)
						{
							var e = this.GetFilteredEvents (obj).Where (x => x.Timestamp < column.Timestamp).LastOrDefault ();
							if (e != null)
							{
								return this.dataArray.FindColumnIndex (e.Timestamp);
							}
						}
					}
				}

				return null;
			}
		}

		private int? NextColumnIndex
		{
			get
			{
				if (this.HasSelectedTimeline)
				{
					var obj = this.SelectedObject;
					if (obj != null && this.GetFilteredEvents (obj).Any ())
					{
						var column = this.dataArray.GetColumn (this.selectedColumn);
						if (column != null)
						{
							var e = this.GetFilteredEvents (obj).Where (x => x.Timestamp > column.Timestamp).FirstOrDefault ();
							if (e != null)
							{
								return this.dataArray.FindColumnIndex (e.Timestamp);
							}
						}
					}
				}

				return null;
			}
		}

		private int? LastColumnIndex
		{
			get
			{
				if (this.HasSelectedTimeline)
				{
					var obj = this.SelectedObject;

					if (this.selectedColumn == -1)
					{
						if (obj != null && this.GetFilteredEvents (obj).Any ())
						{
							var timestamp = this.GetFilteredEvents (obj).Last ().Timestamp;
							return this.dataArray.FindColumnIndex (timestamp);
						}
					}
					else
					{
						if (this.NextColumnIndex.HasValue)
						{
							if (obj != null && this.GetFilteredEvents (obj).Any ())
							{
								var timestamp = this.GetFilteredEvents (obj).Last ().Timestamp;
								return this.dataArray.FindColumnIndex (timestamp);
							}
						}
					}
				}

				return null;
			}
		}
		#endregion


		private IEnumerable<DataEvent> GetFilteredEvents(DataObject obj)
		{
			if (this.Filter == null)
			{
				return obj.Events;
			}
			else
			{
				return obj.Events.Where (x => this.Filter (x));
			}
		}


		private DataObject SelectedObject
		{
			get
			{
				if (this.selectedRow != -1)
				{
					var node = this.nodeGetter[this.selectedRow];
					if (!node.IsEmpty)
					{
						return this.accessor.GetObject (node.BaseType, node.Guid);
					}
				}

				return null;
			}
		}

	
		#region Events handler
		private void OnSelectedCellChanged()
		{
			this.SelectedCellChanged.Raise (this);
		}

		public event EventHandler SelectedCellChanged;


		private void OnCellDoubleClicked()
		{
			this.CellDoubleClicked.Raise (this);
		}

		public event EventHandler CellDoubleClicked;


		private void OnStartEditing(EventType eventType, Timestamp timestamp)
		{
			this.StartEditing.Raise (this, eventType, timestamp);
		}

		public event EventHandler<EventType, Timestamp> StartEditing;


		private void OnStopEditing()
		{
			this.StopEditing.Raise (this);
		}

		public event EventHandler StopEditing;
		#endregion


		private const int lineHeight = 18;

		private readonly DataAccessor						accessor;
		private readonly CommandDispatcher					commandDispatcher;
		private readonly CommandContext						commandContext;
		private readonly MainToolbar						mainToolbar;
		private readonly ObjectsNodeGetter					nodeGetter;
		private readonly SingleObjectsTreeTableFiller		dataFiller;
		private readonly TimelinesArrayLogic				arrayLogic;
		private readonly TimelineArray						dataArray;
		private readonly Amortizations						amortizations;

		private TopTitle									topTitle;
		private TimelinesMode								timelinesMode;
		private AssetsLeftToolbar							assetsToolbar;
		private VSplitter									splitter;
		private AbstractCommandToolbar						timelinesToolbar;
		private TreeTableColumnTree							treeColumn;
		private NavigationTimelineController				controller;
		private VScroller									scroller;
		private StateAtController							stateAtController;
		private int											selectedRow;
		private int											selectedColumn;
		private Guid										rootGuid;
	}
}
