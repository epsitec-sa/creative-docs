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
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class TimelinesArrayController : IDirty
	{
		public TimelinesArrayController(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.selectedRow    = -1;
			this.selectedColumn = -1;

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode -> CumulNode
			var groupNodeGetter  = this.accessor.GetNodeGetter (BaseType.Groups);
			var objectNodeGetter = this.accessor.GetNodeGetter (BaseType.Assets);
			this.nodeGetter      = new ObjectsNodeGetter (this.accessor, groupNodeGetter, objectNodeGetter);

			this.dataFiller = new SingleObjectsTreeTableFiller (this.accessor, this.nodeGetter);

			this.arrayLogic = new TimelinesArrayLogic (this.accessor);
			this.dataArray = new TimelinesArrayLogic.DataArray ();

			this.timelinesMode = TimelinesMode.Wide;

			this.amortizations = new Amortizations (this.accessor);
		}


		#region IDirty Members
		public bool InUse
		{
			get;
			set;
		}

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
			var timestamp = new Timestamp (this.stateAtController.Date.Value, 0);
			var sortingInstructions = new SortingInstructions (this.accessor.GetMainStringField (BaseType.Assets), SortedType.Ascending, ObjectField.Unknown, SortedType.None);

			this.nodeGetter.SetParams (timestamp, this.rootGuid, sortingInstructions);
			this.dataFiller.Timestamp = timestamp;

			this.UpdateDataArray ();
			this.UpdateScroller ();
			this.UpdateController ();
			this.UpdateToolbar ();
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
			this.topTitle = new TopTitle
			{
				Parent = parent,
			};

			this.topTitle.SetTitle (this.Title);

			var box = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			var leftBox = new FrameBox
			{
				Parent         = box,
				Dock           = DockStyle.Left,
				PreferredWidth = TimelinesArrayController.leftColumnWidth,
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
			this.objectsToolbar = new TreeTableToolbar
			{
				NewCustomization      = new CommandCustomization ("TreeTable.New.Asset",   "Nouvel objet d'immobilisation"),
				DeleteCustomization   = new CommandCustomization (null,                    "Supprimer l'objet d'immobilisation"),
				DeselectCustomization = new CommandCustomization (null,                    "Désélectionner l'objet d'immobilisation"),
				CopyCustomization     = new CommandCustomization ("TreeTable.Copy.Asset",  "Copier l'objet d'immobilisation"),
				PasteCustomization    = new CommandCustomization ("TreeTable.Paste.Asset", "Coller l'objet d'immobilisation"),
				ExportCustomization   = new CommandCustomization (null,                    "Exporter les objets d'immobilisations"),
			};

			this.objectsToolbar.CreateUI (leftBox);
			this.objectsToolbar.HasFilter = true;
			this.objectsToolbar.HasTreeOperations = true;

			this.treeColumn = new TreeTableColumnTree
			{
				Parent            = leftBox,
				Dock              = DockStyle.Fill,
				IndependentColumn = true,
				DockToLeft        = true,  // pour avoir la couleur grise
				HoverMode         = TreeTableHoverMode.VerticalGradient,
				HeaderHeight      = TimelinesArrayController.lineHeight*2,
				FooterHeight      = 0,
				HeaderDescription = "Objet",
				RowHeight         = TimelinesArrayController.lineHeight,
				Margins           = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),
				VerticalAdjust    = -1,
			};

			this.CreateStateAt (leftBox);

			//	Partie droite.
			this.timelinesToolbar = new TimelinesToolbar ();
			this.timelinesToolbar.CreateUI (rightBox);
			this.timelinesToolbar.TimelinesMode = this.timelinesMode;

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
			{
				this.treeColumn.RowClicked += delegate (object sender, int row)
				{
					this.SetSelection (this.TopVisibleRow + row, this.selectedColumn);
				};

				this.treeColumn.TreeButtonClicked += delegate (object sender, int row, NodeType type)
				{
					this.OnCompactOrExpand (this.TopVisibleRow + row);
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

			this.objectsToolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.Filter:
						this.OnObjectFilter ();
						break;

					case ToolbarCommand.First:
						this.OnObjectFirst ();
						break;

					case ToolbarCommand.Last:
						this.OnObjectLast ();
						break;

					case ToolbarCommand.Prev:
						this.OnObjectPrev ();
						break;

					case ToolbarCommand.Next:
						this.OnObjectNext ();
						break;

					case ToolbarCommand.CompactAll:
						this.OnCompactAll ();
						break;

					case ToolbarCommand.CompactOne:
						this.OnCompactOne ();
						break;

					case ToolbarCommand.ExpandOne:
						this.OnExpandOne ();
						break;

					case ToolbarCommand.ExpandAll:
						this.OnExpandAll ();
						break;

					case ToolbarCommand.New:
						this.OnObjectNew ();
						break;

					case ToolbarCommand.Delete:
						this.OnObjectDelete ();
						break;

					case ToolbarCommand.Deselect:
						this.OnObjectDeselect ();
						break;
				}
			};

			this.timelinesToolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.First:
						this.OnTimelineFirst ();
						break;

					case ToolbarCommand.Last:
						this.OnTimelineLast ();
						break;

					case ToolbarCommand.Prev:
						this.OnTimelinePrev ();
						break;

					case ToolbarCommand.Next:
						this.OnTimelineNext ();
						break;

					case ToolbarCommand.New:
						this.OnTimelineNew ();
						break;

					case ToolbarCommand.Delete:
						this.OnTimelineDelete ();
						break;

					case ToolbarCommand.AmortizationsPreview:
						this.OnAmortizationPreview ();
						break;
					
					case ToolbarCommand.AmortizationsFix:
						this.OnAmortizationFix ();
						break;
					
					case ToolbarCommand.AmortizationsToExtra:
						this.OnAmortizationToExtra ();
						break;
					
					case ToolbarCommand.AmortizationsUnpreview:
						this.OnAmortizationUnpreview ();
						break;
					
					case ToolbarCommand.AmortizationsDelete:
						this.OnAmortizationDelete ();
						break;

					case ToolbarCommand.Deselect:
						this.OnTimelineDeselect ();
						break;

					case ToolbarCommand.Copy:
						this.OnTimelineCopy ();
						break;

					case ToolbarCommand.Paste:
						this.OnTimelinePaste ();
						break;
				}
			};

			this.timelinesToolbar.ModeChanged += delegate
			{
				this.TimelinesMode = this.timelinesToolbar.TimelinesMode;
			};
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
					this.timelinesMode = value;

					this.UpdateScroller ();
					this.UpdateController ();
					this.UpdateToolbar ();
				}
			}
		}


		private void ShowContextMenu(Point pos)
		{
			//	Affiche le menu contextuel.
			var popup = new SimplePopup ();

			popup.Items.Add ("Rouge");
			popup.Items.Add ("Vert");
			popup.Items.Add ("Bleu");

			popup.Create (this.scroller, pos, leftOrRight: true);

			popup.ItemClicked += delegate (object sender, int rank)
			{
			};
		}

		private void OnDokeySelect(KeyCode key)
		{
			switch (key)
			{
				case KeyCode.Home:
					this.OnObjectFirst ();
					this.OnTimelineFirst ();
					break;

				case KeyCode.End:
					this.OnObjectLast ();
					this.OnTimelineLast ();
					break;

				case KeyCode.ArrowUp:
					this.OnObjectPrev ();
					break;

				case KeyCode.ArrowDown:
					this.OnObjectNext ();
					break;

				case KeyCode.ArrowLeft:
					this.OnTimelinePrev ();
					break;

				case KeyCode.ArrowRight:
					this.OnTimelineNext ();
					break;
			}
		}


		#region Objects commands
		private void OnObjectFilter()
		{
			var target = this.objectsToolbar.GetTarget (ToolbarCommand.Filter);
			var popup = new FilterPopup (this.accessor, this.rootGuid);

			popup.Create (target, leftOrRight: true);

			popup.Navigate += delegate (object sender, Guid guid)
			{
				this.rootGuid = guid;
				this.UpdateTreeColumn ();
			};
		}

		private void OnObjectFirst()
		{
			var index = this.FirstRowIndex;

			if (index.HasValue)
			{
				this.SetSelection (index.Value, this.selectedColumn);
			}
		}

		private void OnObjectPrev()
		{
			var index = this.PrevRowIndex;

			if (index.HasValue)
			{
				this.SetSelection (index.Value, this.selectedColumn);
			}
		}

		private void OnObjectNext()
		{
			var index = this.NextRowIndex;

			if (index.HasValue)
			{
				this.SetSelection (index.Value, this.selectedColumn);
			}
		}

		private void OnObjectLast()
		{
			var index = this.LastRowIndex;

			if (index.HasValue)
			{
				this.SetSelection (index.Value, this.selectedColumn);
			}
		}

		private void OnObjectNew()
		{
			var target = this.objectsToolbar.GetTarget (ToolbarCommand.New);
			this.ShowCreatePopup (target);
		}

		private void OnObjectDelete()
		{
			var target = this.objectsToolbar.GetTarget (ToolbarCommand.Delete);
			YesNoPopup.Show (target, "Voulez-vous supprimer l'objet sélectionné ?", this.ObjectDeleteSelection);
		}

		private void ObjectDeleteSelection()
		{
			this.accessor.RemoveObject (BaseType.Assets, this.SelectedGuid);
			this.UpdateData ();
		}

		private void OnObjectDeselect()
		{
			this.SetSelection (-1, -1);
		}

		private void OnCompactOrExpand(int row)
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

		protected void OnCompactAll()
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

		protected void OnCompactOne()
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

		protected void OnExpandOne()
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

		protected void OnExpandAll()
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
			CreateAssetPopup.Show (target, this.accessor, delegate (System.DateTime date, string name)
			{
				this.CreateAsset (date, name);
			});
		}

		private void CreateAsset(System.DateTime date, string name)
		{
			var guid = this.accessor.CreateObject (BaseType.Assets, date, name, Guid.Empty);
			var obj = this.accessor.GetObject (BaseType.Assets, guid);
			System.Diagnostics.Debug.Assert (obj != null);

			this.UpdateData ();

			this.SelectedGuid = guid;
			this.SelectedTimestamp = AssetCalculator.GetLastTimestamp (obj);

			this.OnStartEditing (EventType.Input, this.SelectedTimestamp.GetValueOrDefault ());
		}
		#endregion


		#region Timeline commands
		private void OnTimelineFirst()
		{
			var index = this.FirstColumnIndex;

			if (index.HasValue)
			{
				this.SetSelection (this.selectedRow, index.Value);
			}
		}

		private void OnTimelinePrev()
		{
			var index = this.PrevColumnIndex;

			if (index.HasValue)
			{
				this.SetSelection (this.selectedRow, index.Value);
			}
		}

		private void OnTimelineNext()
		{
			var index = this.NextColumnIndex;

			if (index.HasValue)
			{
				this.SetSelection (this.selectedRow, index.Value);
			}
		}

		private void OnTimelineLast()
		{
			var index = this.LastColumnIndex;

			if (index.HasValue)
			{
				this.SetSelection (this.selectedRow, index.Value);
			}
		}

		private void OnTimelineNew()
		{
			var timestamp = this.SelectedTimestamp;

			if (timestamp.HasValue)
			{
				var target = this.timelinesToolbar.GetTarget (ToolbarCommand.New);
				var obj = this.accessor.GetObject (BaseType.Assets, this.SelectedGuid);

				NewEventPopup.Show (target, this.accessor, BaseType.Assets, obj, timestamp.Value,
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

		private void OnTimelineDelete()
		{
			if (this.SelectedEventType == EventType.AmortizationPreview)
			{
				//	Si on supprime un événement AmortizationPreview, on ne demande pas de confirmation.
				this.TimelineDeleteSelection ();
			}
			else
			{
				var target = this.timelinesToolbar.GetTarget (ToolbarCommand.Delete);

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

		private void OnTimelineDeselect()
		{
			this.SetSelection (this.selectedRow, -1);
		}

		private void OnTimelineCopy()
		{
			var target = this.timelinesToolbar.GetTarget (ToolbarCommand.Copy);
			var obj = this.SelectedObject;

			if (obj != null && this.SelectedTimestamp.HasValue)
			{
				var e = obj.GetEvent (this.SelectedTimestamp.Value);
				this.accessor.Clipboard.CopyEvent (this.accessor, e);

				this.UpdateToolbar ();
			}
			else
			{
				MessagePopup.ShowError (target, "La copie est impossible, car aucun événement n'est sélectionné.");
			}
		}

		private void OnTimelinePaste()
		{
			var target = this.timelinesToolbar.GetTarget (ToolbarCommand.Paste);
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
					var e = this.accessor.Clipboard.PasteEvent (this.accessor, obj, date);

					if (e == null)
					{
						MessagePopup.ShowError (target, "Les données sont incompatibles.");
					}
					else
					{
						this.UpdateDataArray ();
						this.UpdateScroller ();
						this.UpdateController ();
						this.UpdateToolbar ();
						this.SetSelection (this.selectedRow, this.dataArray.FindColumnIndex (e.Timestamp));
						//?this.OnStartEditing (e.Type, e.Timestamp);
					}
				});
			}
			else
			{
				MessagePopup.ShowError (target, "Aucun événement ne peut être collé, car le bloc-notes est vide.");
			}
		}

		private void OnAmortizationPreview()
		{
			var target = this.timelinesToolbar.GetTarget (ToolbarCommand.AmortizationsPreview);

			this.ShowAmortizationsPopup (target, true, true,
				"Générer les préamortissements",
				"Générer pour un",
				"Générer pour tous",
				this.DoAmortisationsPreview);
		}

		private void OnAmortizationFix()
		{
			var target = this.timelinesToolbar.GetTarget (ToolbarCommand.AmortizationsFix);

			this.ShowAmortizationsPopup (target, false, false,
				"Fixer les préamortissements",
				"Fixer pour un",
				"Fixer pour tous",
				this.DoAmortisationsFix);
		}

		private void OnAmortizationToExtra()
		{
			//	Transforme un amortissement ordinaire en extraordinaire.
			if (!this.SelectedGuid.IsEmpty && this.SelectedTimestamp.HasValue)
			{
				var asset = this.accessor.GetObject (BaseType.Assets, this.SelectedGuid);
				if (asset != null)
				{
					var e = asset.GetEvent (this.SelectedTimestamp.Value);
					if (e != null)
					{
						//	Supprime l'amortissement ordinaire.
						asset.RemoveEvent (e);

						//	Crée un amortissement extraordinaire.
						var newEvent = new DataEvent (e.Guid, e.Timestamp, EventType.AmortizationExtra);
						newEvent.SetProperties (e);
						asset.AddEvent (newEvent);

						this.UpdateData ();
					}
				}
			}
		}

		private void OnAmortizationUnpreview()
		{
			var target = this.timelinesToolbar.GetTarget (ToolbarCommand.AmortizationsUnpreview);

			this.ShowAmortizationsPopup (target, false, false,
				"Supprimer les préamortissements",
				"Supprimer pour un",
				"Supprimer pour tous",
				this.DoAmortisationsUnpreview);
		}

		private void OnAmortizationDelete()
		{
			var target = this.timelinesToolbar.GetTarget (ToolbarCommand.AmortizationsDelete);

			this.ShowAmortizationsPopup (target, true, false,
				"Supprimer des amortissements ordinaires",
				"Supprimer pour un",
				"Supprimer pour tous",
				this.DoAmortisationsDelete);
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
			this.topTitle.Window.ForceLayout ();

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

					var g = new TimelineCellGlyph (cell.Glyph, cell.Flags, cell.Tooltip, selected);
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
					var d = new TimelineCellDate (column.Timestamp.Date);
					dates.Add (d);
				}
			}

			if (this.timelinesMode == TimelinesMode.Narrow)
			{
				this.controller.SetRowDayCells   (line++, dates.ToArray ());
				this.controller.SetRowMonthCells (line++, dates.ToArray ());
				this.controller.SetRowYearCells  (line++, dates.ToArray ());
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
					list.Add (new TimelineRowDescription (TimelineRowType.Days,   "Jour"));
					list.Add (new TimelineRowDescription (TimelineRowType.Months, "Mois"));
					list.Add (new TimelineRowDescription (TimelineRowType.Years,  "Années"));
				}
				else
				{
					list.Add (new TimelineRowDescription (TimelineRowType.DaysMonths, "Jour"));
					list.Add (new TimelineRowDescription (TimelineRowType.Years,      "Années"));
				}

				return list.ToArray ();
			}
		}


		private void UpdateTreeColumn()
		{
			var selectedGuid = this.SelectedGuid;
			{
				this.UpdateData ();
			}
			this.SelectedGuid = selectedGuid;
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
			this.arrayLogic.Update (this.dataArray, this.nodeGetter, this.Filter);
		}


		protected void UpdateToolbar()
		{
			this.objectsToolbar.SetCommandActivate (ToolbarCommand.Filter, !this.rootGuid.IsEmpty);

			this.UpdateObjectCommand (ToolbarCommand.First, this.selectedRow, this.FirstRowIndex);
			this.UpdateObjectCommand (ToolbarCommand.Prev,  this.selectedRow, this.PrevRowIndex);
			this.UpdateObjectCommand (ToolbarCommand.Next,  this.selectedRow, this.NextRowIndex);
			this.UpdateObjectCommand (ToolbarCommand.Last,  this.selectedRow, this.LastRowIndex);

			bool compactEnable = !this.nodeGetter.IsAllCompacted;
			bool expandEnable  = !this.nodeGetter.IsAllExpanded;

			this.objectsToolbar.SetCommandEnable (ToolbarCommand.CompactAll, compactEnable);
			this.objectsToolbar.SetCommandEnable (ToolbarCommand.CompactOne, compactEnable);
			this.objectsToolbar.SetCommandEnable (ToolbarCommand.ExpandOne,  expandEnable);
			this.objectsToolbar.SetCommandEnable (ToolbarCommand.ExpandAll,  expandEnable);

			this.objectsToolbar.SetCommandEnable (ToolbarCommand.New,      true);
			this.objectsToolbar.SetCommandEnable (ToolbarCommand.Delete,   this.SelectedObject != null);
			this.objectsToolbar.SetCommandEnable (ToolbarCommand.Deselect, this.selectedRow != -1);

			this.UpdateTimelineCommand (ToolbarCommand.First, this.selectedColumn, this.FirstColumnIndex);
			this.UpdateTimelineCommand (ToolbarCommand.Prev,  this.selectedColumn, this.PrevColumnIndex);
			this.UpdateTimelineCommand (ToolbarCommand.Next,  this.selectedColumn, this.NextColumnIndex);
			this.UpdateTimelineCommand (ToolbarCommand.Last,  this.selectedColumn, this.LastColumnIndex);

			if (this.HasAmortizationsOper)
			{
				this.timelinesToolbar.SetCommandState  (ToolbarCommand.New,                    ToolbarCommandState.Hide);
				this.timelinesToolbar.SetCommandState  (ToolbarCommand.Delete,                 ToolbarCommandState.Hide);
				this.timelinesToolbar.SetCommandEnable (ToolbarCommand.AmortizationsPreview,   true);
				this.timelinesToolbar.SetCommandEnable (ToolbarCommand.AmortizationsFix,       true);
				this.timelinesToolbar.SetCommandEnable (ToolbarCommand.AmortizationsToExtra,   this.IsToExtraPossible);
				this.timelinesToolbar.SetCommandEnable (ToolbarCommand.AmortizationsUnpreview, true);
				this.timelinesToolbar.SetCommandEnable (ToolbarCommand.AmortizationsDelete,    true);
				this.timelinesToolbar.SetCommandEnable (ToolbarCommand.Deselect,               this.selectedColumn != -1);
			}
			else
			{
				this.timelinesToolbar.SetCommandEnable (ToolbarCommand.New,                    this.selectedColumn != -1 && this.HasSelectedTimeline);
				this.timelinesToolbar.SetCommandEnable (ToolbarCommand.Delete,                 this.HasSelectedEvent);
				this.timelinesToolbar.SetCommandState  (ToolbarCommand.AmortizationsPreview,   ToolbarCommandState.Hide);
				this.timelinesToolbar.SetCommandState  (ToolbarCommand.AmortizationsFix,       ToolbarCommandState.Hide);
				this.timelinesToolbar.SetCommandState  (ToolbarCommand.AmortizationsToExtra,   ToolbarCommandState.Hide);
				this.timelinesToolbar.SetCommandState  (ToolbarCommand.AmortizationsUnpreview, ToolbarCommandState.Hide);
				this.timelinesToolbar.SetCommandState  (ToolbarCommand.AmortizationsDelete,    ToolbarCommandState.Hide);
				this.timelinesToolbar.SetCommandEnable (ToolbarCommand.Deselect,               this.selectedColumn != -1);
			}

			this.timelinesToolbar.SetCommandEnable (ToolbarCommand.Copy,  this.HasSelectedEvent);
			this.timelinesToolbar.SetCommandEnable (ToolbarCommand.Paste, this.accessor.Clipboard.HasEvent);
		}

		private void UpdateObjectCommand(ToolbarCommand command, int currentSelection, int? newSelection)
		{
			bool enable = (newSelection.HasValue && currentSelection != newSelection.Value);
			this.objectsToolbar.SetCommandEnable (command, enable);
		}

		private void UpdateTimelineCommand(ToolbarCommand command, int currentSelection, int? newSelection)
		{
			bool enable = (newSelection.HasValue && currentSelection != newSelection.Value);
			this.timelinesToolbar.SetCommandEnable (command, enable);
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

		private bool HasSelectedEvent
		{
			get
			{
				var obj = this.SelectedObject;
				if (obj != null && this.GetFilteredEvents (obj).Any ())
				{
					var column = this.dataArray.GetColumn (this.selectedColumn);
					if (column != null)
					{
						return this.GetFilteredEvents (obj).Where (x => x.Timestamp == column.Timestamp).Any ();
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
		#endregion


		private const int lineHeight      = 18;
		private const int leftColumnWidth = 180;

		private readonly DataAccessor						accessor;
		private readonly ObjectsNodeGetter					nodeGetter;
		private readonly SingleObjectsTreeTableFiller		dataFiller;
		private readonly TimelinesArrayLogic				arrayLogic;
		private readonly TimelinesArrayLogic.DataArray		dataArray;
		private readonly Amortizations						amortizations;

		private TopTitle									topTitle;
		private TimelinesMode								timelinesMode;
		private TreeTableToolbar							objectsToolbar;
		private VSplitter									splitter;
		private TimelinesToolbar							timelinesToolbar;
		private TreeTableColumnTree							treeColumn;
		private NavigationTimelineController				controller;
		private VScroller									scroller;
		private StateAtController							stateAtController;
		private int											selectedRow;
		private int											selectedColumn;
		private Guid										rootGuid;
	}
}
