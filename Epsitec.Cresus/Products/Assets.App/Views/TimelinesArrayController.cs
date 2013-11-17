//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class TimelinesArrayController
	{
		public TimelinesArrayController(DataAccessor accessor, BaseType baseType)
		{
			this.accessor = accessor;
			this.baseType = baseType;

			this.selectedRow    = -1;
			this.selectedColumn = -1;

			switch (this.baseType)
			{
				case BaseType.Objects:
					this.title = "Objets d'immobilisation";
					this.subtitle = "Objets";
					break;

				case BaseType.Categories:
					this.title = "Catégories d'immobilisation";
					this.subtitle = "Catégories";
					break;

				case BaseType.Groups:
					this.title = "Groupes d'immobilisation";
					this.subtitle = "Groupes";
					break;
			}

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
			var primaryNodesGetter = this.accessor.GetNodesGetter (this.baseType);
			this.nodesGetter = new TreeNodesGetter (this.accessor, this.baseType, primaryNodesGetter);

			this.dataFiller = new SingleObjectsTreeTableFiller (this.accessor, this.baseType, this.nodesGetter);

			this.arrayLogic = new TimelinesArrayLogic (this.accessor, this.baseType);
			this.dataArray = new TimelinesArrayLogic.DataArray ();

			this.timelineMode = TimelineMode.Expanded;
		}


		public bool								DataFreezed;

		public void UpdateData()
		{
			this.nodesGetter.UpdateData ();

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
					var node = this.nodesGetter[this.selectedRow];
					if (!node.IsEmpty)
					{
						return node.Guid;
					}
				}

				return Guid.Empty;
			}
			set
			{
				var selectedRow = this.nodesGetter.Nodes.ToList ().FindIndex (x => x.Guid == value);
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

			this.topTitle.SetTitle (this.title);

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
				Parent = box,
				Dock   = DockStyle.Left,
			};

			var rightBox = new FrameBox
			{
				Parent = box,
				Dock   = DockStyle.Fill,
			};

			//	Partie gauche.
			this.objectsToolbar = new TreeTableToolbar ();
			this.objectsToolbar.CreateUI (leftBox);
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
				HeaderDescription = this.subtitle,
				RowHeight         = TimelinesArrayController.lineHeight,
				Margins           = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),
				VerticalAdjust    = -1,
			};

			this.CreateStateAt (leftBox);

			//	Partie droite.
			this.timelinesToolbar = new TimelinesToolbar ();
			this.timelinesToolbar.CreateUI (rightBox);
			this.timelinesToolbar.TimelineMode = this.timelineMode;

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
			this.treeColumn.RowClicked += delegate (object sender, int row)
			{
				this.SetSelection (this.TopVisibleRow + row, this.selectedColumn);
			};

			this.treeColumn.TreeButtonClicked += delegate (object sender, int row, NodeType type)
			{
				this.OnCompactOrExpand (this.TopVisibleRow + row);
			};

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

			this.objectsToolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
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

					case ToolbarCommand.Deselect:
						this.OnTimelineDeselect ();
						break;
				}
			};

			this.timelinesToolbar.ModeChanged += delegate
			{
				this.TimelineMode = this.timelinesToolbar.TimelineMode;
			};
		}

		private void CreateStateAt(Widget parent)
		{
			this.stateAtController = new StateAtController ();
			this.stateAtController.CreateUI (parent);

			this.stateAtController.DateChanged += delegate
			{
				if (this.stateAtController.Date.HasValue)
				{
					this.nodesGetter.Timestamp = new Timestamp (this.stateAtController.Date.Value, 0);
				}
				else
				{
					this.nodesGetter.Timestamp = null;
				}

				this.UpdateDataArray ();
				this.UpdateScroller ();
				this.UpdateController ();
				this.UpdateToolbar ();
			};
		}


		public TimelineMode						TimelineMode
		{
			get
			{
				return this.timelineMode;
			}
			set
			{
				if (this.timelineMode != value)
				{
					this.timelineMode = value;

					this.UpdateScroller ();
					this.UpdateController ();
					this.UpdateToolbar ();
				}
			}
		}


		#region Objects commands
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
			var target = this.objectsToolbar.GetCommandWidget (ToolbarCommand.New);
			this.ShowCreatePopup (target);
			//-var modelGuid = this.SelectedGuid;
			//-if (modelGuid.IsEmpty)
			//-{
			//-	return;
			//-}
			//-
			//-int sel = this.selectedRow;
			//-if (sel == -1)
			//-{
			//-	return;
			//-}
			//-
			//-var e = this.accessor.CreateObject (this.baseType, sel+1, modelGuid);
			//-
			//-this.UpdateData ();
			//-
			//-var column = this.dataArray.FindColumnIndex (e.Timestamp);
			//-this.SetSelection (sel+1, column);
			//-
			//-this.OnStartEditing (e.Type, e.Timestamp);
		}

		private void OnObjectDelete()
		{
			var target = this.objectsToolbar.GetCommandWidget (ToolbarCommand.Delete);

			if (target != null)
			{
				var popup = new YesNoPopup
				{
					Question = "Voulez-vous supprimer l'objet sélectionné ?",
				};

				popup.Create (target);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
						this.accessor.RemoveObject (this.baseType, this.SelectedGuid);
						this.UpdateData ();
					}
				};
			}
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

			this.nodesGetter.CompactOrExpand (row);
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

			this.nodesGetter.CompactAll ();
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

			this.nodesGetter.ExpandAll ();
			this.UpdateDataArray ();
			this.UpdateScroller ();
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
			this.SelectedTimestamp = timestamp;
		}


		private void ShowCreatePopup(Widget target)
		{
			var popup = new CreateObjectPopup (this.accessor, this.baseType, this.SelectedGuid);

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "create")
				{
					this.CreateObject (popup.ObjectDate.Value, popup.ObjectName, popup.ObjectParent, popup.ObjectGrouping);
				}
			};
		}

		private void CreateObject(System.DateTime date, string name, Guid parent, bool grouping)
		{
			var guid = this.accessor.CreateObject (this.baseType, date, name, parent, grouping);
			var obj = this.accessor.GetObject (this.baseType, guid);
			System.Diagnostics.Debug.Assert (obj != null);

			this.UpdateData ();

			this.SelectedGuid = guid;
			this.SelectedTimestamp = ObjectCalculator.GetLastTimestamp (obj);

			this.OnStartEditing (EventType.Entrée, this.SelectedTimestamp.GetValueOrDefault ());
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
			var target = this.timelinesToolbar.GetCommandWidget (ToolbarCommand.New);
			var timestamp = this.SelectedTimestamp;

			if (target != null && timestamp.HasValue)
			{
				System.DateTime? createDate = timestamp.Value.Date;

				var popup = new NewEventPopup
				{
					BaseType   = this.baseType,
					DataObject = this.accessor.GetObject (this.baseType, this.SelectedGuid),
					Timestamp  = timestamp.Value,
				};

				popup.Create (target);

				popup.DateChanged += delegate (object sender, System.DateTime? dateTime)
				{
					if (dateTime.HasValue)
					{
						createDate = dateTime.Value;

						int column = this.dataArray.FindColumnIndex (new Timestamp (dateTime.Value, 0));
						this.SetSelection (this.selectedRow, column);
					}
				};

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (createDate.HasValue)
					{
						this.CreateEvent (createDate.Value, name);
					}
				};
			}
		}

		private void OnTimelineDelete()
		{
			var target = this.timelinesToolbar.GetCommandWidget (ToolbarCommand.Delete);

			if (target != null)
			{
				var popup = new YesNoPopup
				{
					Question = "Voulez-vous supprimer l'événement sélectionné ?",
				};

				popup.Create (target);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
						this.accessor.RemoveObjectEvent (this.SelectedObject, this.SelectedTimestamp);
						this.UpdateData ();
					}
				};
			}
		}

		private void OnTimelineDeselect()
		{
			this.SetSelection (this.selectedRow, -1);
		}
		#endregion


		private void CreateEvent(System.DateTime date, string buttonName)
		{
			var obj = this.accessor.GetObject (this.baseType, this.SelectedGuid);
			if (obj != null)
			{
				var type = TimelinesArrayController.ParseEventType (buttonName);
				var e = this.accessor.CreateObjectEvent (obj, date, type);

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
			System.Diagnostics.Debug.Assert (this.VisibleRows > 0);
			int count = System.Math.Min (this.VisibleRows, this.dataArray.RowsCount);

			sel = System.Math.Min (sel + count/2, this.dataArray.RowsCount-1);
			sel = System.Math.Max (sel - count+1, 0);

			return sel;
		}


		private void UpdateTimelines(bool crop = true)
		{
			this.controller.TopRowsWithExactHeight = this.HeaderLinesCount;
			this.controller.RelativeWidth = (this.timelineMode == TimelineMode.Compacted) ? 1.0 : 2.0;
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

					var g = new TimelineCellGlyph (cell.Glyph, cell.Locked, cell.Tooltip, selected);
					glyphs.Add (g);
				}

				this.controller.SetRowGlyphCells (line++, glyphs.ToArray ());
			}

			//	Ajoute les 2 lignes supérieures pour les dates.
			var dates  = new List<TimelineCellDate> ();

			for (int i = 0; i < count; i++)
			{
				var column = this.dataArray.GetColumn (firstCell+i);
				if (column != null)
				{
					var d = new TimelineCellDate (column.Timestamp.Date);
					dates.Add (d);
				}
			}

			if (this.timelineMode == TimelineMode.Compacted)
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

				if (this.timelineMode == TimelineMode.Compacted)
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


		private void UpdateScroller()
		{
			if (this.scroller == null)
			{
				return;
			}

			var totalRows   = (decimal) this.nodesGetter.Count;
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
				int count = System.Math.Min (this.nodesGetter.Count, firstRow+visibleRows);

				for (int row=firstRow; row<count; row++)
				{
					yield return row;
				}
			}
		}

		private int LineToRow(int line)
		{
			var dummy = this.DummyCount;
			int count = System.Math.Min (this.nodesGetter.Count, this.VisibleRows);

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
				return System.Math.Max (this.VisibleRows - this.nodesGetter.Count, 0);
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
				if (this.timelineMode == TimelineMode.Compacted)
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
			this.arrayLogic.Update (this.dataArray, this.nodesGetter);
		}


		protected void UpdateToolbar()
		{
			this.UpdateObjectCommand (ToolbarCommand.First, this.selectedRow, this.FirstRowIndex);
			this.UpdateObjectCommand (ToolbarCommand.Prev,  this.selectedRow, this.PrevRowIndex);
			this.UpdateObjectCommand (ToolbarCommand.Next,  this.selectedRow, this.NextRowIndex);
			this.UpdateObjectCommand (ToolbarCommand.Last,  this.selectedRow, this.LastRowIndex);

			this.objectsToolbar.UpdateCommand (ToolbarCommand.CompactAll, !this.nodesGetter.IsAllCompacted);
			this.objectsToolbar.UpdateCommand (ToolbarCommand.ExpandAll,  !this.nodesGetter.IsAllExpanded);

			this.objectsToolbar.UpdateCommand (ToolbarCommand.New,      true);
			this.objectsToolbar.UpdateCommand (ToolbarCommand.Delete,   this.SelectedObject != null);
			this.objectsToolbar.UpdateCommand (ToolbarCommand.Deselect, this.selectedRow != -1);

			this.UpdateTimelineCommand (ToolbarCommand.First, this.selectedColumn, this.FirstColumnIndex);
			this.UpdateTimelineCommand (ToolbarCommand.Prev,  this.selectedColumn, this.PrevColumnIndex);
			this.UpdateTimelineCommand (ToolbarCommand.Next,  this.selectedColumn, this.NextColumnIndex);
			this.UpdateTimelineCommand (ToolbarCommand.Last,  this.selectedColumn, this.LastColumnIndex);

			this.timelinesToolbar.UpdateCommand (ToolbarCommand.New,      this.selectedColumn != -1);
			this.timelinesToolbar.UpdateCommand (ToolbarCommand.Delete,   this.HasSelectedEvent);
			this.timelinesToolbar.UpdateCommand (ToolbarCommand.Deselect, this.selectedColumn != -1);
		}

		private void UpdateObjectCommand(ToolbarCommand command, int currentSelection, int? newSelection)
		{
			bool enable = (newSelection.HasValue && currentSelection != newSelection.Value);
			this.objectsToolbar.UpdateCommand (command, enable);
		}

		private void UpdateTimelineCommand(ToolbarCommand command, int currentSelection, int? newSelection)
		{
			bool enable = (newSelection.HasValue && currentSelection != newSelection.Value);
			this.timelinesToolbar.UpdateCommand (command, enable);
		}


		private bool HasSelectedEvent
		{
			get
			{
				var obj = this.SelectedObject;
				if (obj != null && obj.Events.Any ())
				{
					var column = this.dataArray.GetColumn (this.selectedColumn);
					if (column != null)
					{
						return obj.Events.Where (x => x.Timestamp == column.Timestamp).Any ();
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
					i = System.Math.Min (i, this.nodesGetter.Count - 1);
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
					i = System.Math.Min (i, this.nodesGetter.Count - 1);
					return i;
				}
			}
		}

		protected int? LastRowIndex
		{
			get
			{
				return this.nodesGetter.Count - 1;
			}
		}
		#endregion


		#region Timeline
		private int? FirstColumnIndex
		{
			get
			{
				var obj = this.SelectedObject;

				if (this.selectedColumn == -1)
				{
					if (obj != null && obj.Events.Any ())
					{
						var timestamp = obj.Events.First ().Timestamp;
						return this.dataArray.FindColumnIndex (timestamp);
					}
				}
				else
				{
					if (this.PrevColumnIndex.HasValue)
					{
						if (obj != null && obj.Events.Any ())
						{
							var timestamp = obj.Events.First ().Timestamp;
							return this.dataArray.FindColumnIndex (timestamp);
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
				var obj = this.SelectedObject;
				if (obj != null && obj.Events.Any ())
				{
					var column = this.dataArray.GetColumn (this.selectedColumn);
					if (column != null)
					{
						int i = obj.Events.Where (x => x.Timestamp < column.Timestamp).Count () - 1;
						if (i >= 0)
						{
							var e = obj.GetEvent (i);
							return this.dataArray.FindColumnIndex (e.Timestamp);
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
				var obj = this.SelectedObject;
				if (obj != null && obj.Events.Any ())
				{
					var column = this.dataArray.GetColumn (this.selectedColumn);
					if (column != null)
					{
						int i = obj.Events.Where (x => x.Timestamp <= column.Timestamp).Count ();
						if (i < obj.EventsCount)
						{
							var e = obj.GetEvent (i);
							return this.dataArray.FindColumnIndex (e.Timestamp);
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
				var obj = this.SelectedObject;

				if (this.selectedColumn == -1)
				{
					if (obj != null && obj.Events.Any ())
					{
						var timestamp = obj.Events.Last ().Timestamp;
						return this.dataArray.FindColumnIndex (timestamp);
					}
				}
				else
				{
					if (this.NextColumnIndex.HasValue)
					{
						if (obj != null && obj.Events.Any ())
						{
							var timestamp = obj.Events.Last ().Timestamp;
							return this.dataArray.FindColumnIndex (timestamp);
						}
					}
				}

				return null;
			}
		}
		#endregion


		private DataObject SelectedObject
		{
			get
			{
				if (this.selectedRow != -1)
				{
					var node = this.nodesGetter[this.selectedRow];
					if (!node.IsEmpty)
					{
						return this.accessor.GetObject (this.baseType, node.Guid);
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


		private static readonly int lineHeight      = 18;
		private static readonly int leftColumnWidth = 180;

		private readonly DataAccessor						accessor;
		private readonly BaseType							baseType;
		private readonly TreeNodesGetter					nodesGetter;
		private readonly SingleObjectsTreeTableFiller		dataFiller;
		private readonly TimelinesArrayLogic				arrayLogic;
		private readonly TimelinesArrayLogic.DataArray		dataArray;
		private readonly string								title;
		private readonly string								subtitle;

		private TopTitle									topTitle;
		private TimelineMode								timelineMode;
		private TreeTableToolbar							objectsToolbar;
		private VSplitter									splitter;
		private TimelinesToolbar							timelinesToolbar;
		private TreeTableColumnTree							treeColumn;
		private NavigationTimelineController				controller;
		private VScroller									scroller;
		private StateAtController							stateAtController;
		private int											selectedRow;
		private int											selectedColumn;
	}
}
