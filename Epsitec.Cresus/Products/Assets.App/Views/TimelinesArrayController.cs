//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
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
					break;

				case BaseType.Categories:
					this.title = "Catégories d'immobilisation";
					break;

				case BaseType.Groups:
					this.title = "Groupes d'immobilisation";
					break;
			}

			var primaryNodesGetter = this.accessor.GetNodesGetter (this.baseType);
			var levelNodesGetter = new LevelNodesGetter (primaryNodesGetter, this.accessor, this.baseType);
			this.nodesGetter = new TreeObjectsNodesGetter (levelNodesGetter);

			this.dataArray = new DataArray ();

			this.nodesGetter.UpdateData ();
			this.UpdateData ();
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
				this.SetSelection (this.nodesGetter.Nodes.ToList ().FindIndex (x => x.Guid == value), this.selectedColumn);
			}
		}

		public Timestamp?						Timestamp
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
				RowHeight         = TimelinesArrayController.lineHeight,
				Margins           = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),
			};

			//	Partie droite.
			this.timelinesToolbar = new TimelinesToolbar ();
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
			this.controller.RelativeWidth = 1.0;

			this.scroller = new VScroller
			{
				Parent     = bottomRightBox,
				Dock       = DockStyle.Right,
				Margins    = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),
				IsInverted = true,  // le zéro est en haut
			};

			this.UpdateController ();
			this.UpdateScroller ();
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
				this.UpdateController ();
				this.UpdateScroller ();
			};

			this.scroller.ValueChanged += delegate
			{
				this.UpdateController ();
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
#if false
			var target = this.toolbar.GetCommandWidget (ToolbarCommand.New);
			var timestamp = this.SelectedTimestamp;

			if (target != null && timestamp.HasValue)
			{
				System.DateTime? createDate = timestamp.Value.Date;

				var popup = new NewEventPopup
				{
					BaseType   = this.baseType,
					DataObject = this.obj,
					Timestamp  = timestamp.Value,
				};

				popup.Create (target);

				popup.DateChanged += delegate (object sender, System.DateTime? dateTime)
				{
					var index = this.GetEventIndex (dateTime);

					if (index.HasValue)
					{
						this.SelectedCell = index.Value;
					}
					else
					{
						this.SelectedCell = -1;
					}

					if (dateTime.HasValue)
					{
						createDate = dateTime.Value;
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
#endif
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
			var timestamp = this.Timestamp;

			this.nodesGetter.CompactOrExpand (row);
			this.UpdateData ();
			this.UpdateController ();
			this.UpdateScroller ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
			this.Timestamp = timestamp;
		}

		protected void OnCompactAll()
		{
			//	Compacte toutes les lignes.
			var guid = this.SelectedGuid;
			var timestamp = this.Timestamp;

			this.nodesGetter.CompactAll ();
			this.UpdateData ();
			this.UpdateController ();
			this.UpdateScroller ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
			this.Timestamp = timestamp;
		}

		protected void OnExpandAll()
		{
			//	Etend toutes les lignes.
			var guid = this.SelectedGuid;
			var timestamp = this.Timestamp;

			this.nodesGetter.ExpandAll ();
			this.UpdateData ();
			this.UpdateController ();
			this.UpdateScroller ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
			this.Timestamp = timestamp;
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
#if false
			var target = this.toolbar.GetCommandWidget (ToolbarCommand.New);
			var timestamp = this.SelectedTimestamp;

			if (target != null && timestamp.HasValue)
			{
				System.DateTime? createDate = timestamp.Value.Date;

				var popup = new NewEventPopup
				{
					BaseType   = this.baseType,
					DataObject = this.obj,
					Timestamp  = timestamp.Value,
				};

				popup.Create (target);

				popup.DateChanged += delegate (object sender, System.DateTime? dateTime)
				{
					var index = this.GetEventIndex (dateTime);

					if (index.HasValue)
					{
						this.SelectedCell = index.Value;
					}
					else
					{
						this.SelectedCell = -1;
					}

					if (dateTime.HasValue)
					{
						createDate = dateTime.Value;
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
#endif
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
					}
				};
			}
		}

		private void OnTimelineDeselect()
		{
			this.SetSelection (this.selectedRow, -1);
		}
		#endregion


		private void UpdateController(bool crop = true)
		{
			this.UpdateTree (crop);
			this.UpdateTimelines (crop);
		}


		private void UpdateTree(bool crop = true)
		{
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
	
			var list = new List<TreeTableCellTree> ();
			var timestamp = this.Timestamp;

			foreach (var row in this.EnumVisibleRows)
			{
				var node = this.nodesGetter[row];
				var obj  = this.accessor.GetObject (this.baseType, node.Guid);
				var nom  = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Nom);
				bool isSelected = node.Guid == this.SelectedGuid;

				if (timestamp.HasValue &&
					!ObjectCalculator.IsExistingObject (obj, timestamp.Value))
				{
					nom = StaticDescriptions.OutOfDateName;
				}

				var cell = new TreeTableCellTree (true, node.Level, node.Type, nom, isSelected);
				list.Add (cell);
			}

			this.treeColumn.SetCells (list.ToArray ());
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

			this.controller.SetRowDayCells   (line++, dates.ToArray ());
			this.controller.SetRowMonthCells (line++, dates.ToArray ());

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

				list.Add (new TimelineRowDescription (TimelineRowType.Days, "Jour"));
				list.Add (new TimelineRowDescription (TimelineRowType.Months, "Mois"));

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
				return this.TopVisibleRow + count + dummy - 2 + 1 - line;
			}
			else
			{
				return -1;
			}
		}

		private int DummyCount
		{
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
				return (int) (this.scroller.ActualHeight / TimelinesArrayController.lineHeight) - 2;
			}
		}


		private void UpdateData()
		{
			//	Met à jour this.dataArray en fonction de l'ensemble des événements de
			//	tous les objets. Cela nécessite d'accéder à l'ensemble des données, ce
			//	qui peut être long. Néanmoins, cela est nécessaire, même si la timeline
			//	n'affiche qu'un nombre limité de lignes. En effet, il faut allouer toutes
			//	les colonnes pour lesquelles il existe un événement.
			this.dataArray.Clear (this.nodesGetter.Count);

			for (int row=0; row<this.nodesGetter.Count; row++)
			{
				var node = this.nodesGetter[row];
				var obj = this.accessor.GetObject (this.baseType, node.Guid);

				var label = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Nom);
				this.dataArray.RowsLabel.Add (label);

				foreach (var e in obj.Events)
				{
					var column = this.dataArray.GetColumn (e.Timestamp);
					column[row] = this.EventToCell (obj, e);
				}
			}
		}

		private DataCell EventToCell(DataObject obj, DataEvent e)
		{
			var glyph      = TimelineData.TypeToGlyph (e.Type);
			string tooltip = BusinessLogic.GetTooltip (obj, e.Timestamp, e.Type, 8);

			return new DataCell (glyph, false, tooltip);
		}

		#region IInputData Members
		public int NodesCount
		{
			get
			{
				return this.accessor.GetObjectsCount (this.baseType);
			}
		}

		public void GetData(int row, out Guid guid, out int level)
		{
			guid = Guid.Empty;
			level = 0;

			if (row >= 0 && row < this.accessor.GetObjectsCount (this.baseType))
			{
				guid = this.accessor.GetObjectGuids (this.baseType, row, 1).FirstOrDefault ();

				var obj = this.accessor.GetObject (this.baseType, guid);
				var timestamp = new Timestamp (System.DateTime.MaxValue, 0);
				var p = ObjectCalculator.GetObjectSyntheticProperty (obj, timestamp, ObjectField.Level) as DataIntProperty;
				if (p != null)
				{
					level = p.Value;
				}
			}
		}
		#endregion


		#region Data manager
		/// <summary>
		/// Tableau des cellules. Il y a une colonne par Timestamp existant et
		/// une ligne par objet.
		/// </summary>
		private class DataArray
		{
			public DataArray()
			{
				this.columns = new List<DataColumn> ();
				this.rowsLabel = new List<string> ();
			}

			public List<string> RowsLabel
			{
				get
				{
					return this.rowsLabel;
				}
			}

			public int RowsCount
			{
				get
				{
					return this.rowsCount;
				}
			}

			public int ColumnsCount
			{
				get
				{
					return this.columns.Count;
				}
			}

			public DataCell GetCell(int row, int column)
			{
				return this.columns[column][row];
			}

			public void Clear(int rowsCount)
			{
				this.rowsCount = rowsCount;
				this.columns.Clear ();
			}

			public int FindColumnIndex(Timestamp timestamp)
			{
				return this.columns.FindIndex (x => x.Timestamp == timestamp);
			}

			public IEnumerable<DataColumn> Columns
			{
				get
				{
					return this.columns;
				}
			}

			public DataColumn GetColumn(int column)
			{
				if (column >= 0 && column < this.columns.Count)
				{
					return this.columns[column];
				}
				else
				{
					return null;
				}
			}

			public DataColumn GetColumn(Timestamp timestamp)
			{
				//	Retourne la colonne à utiliser pour un Timestamp donné.
				//	Si elle n'existe pas, elle est créée.
				var column = this.columns.Where (x => x.Timestamp == timestamp).FirstOrDefault ();

				if (column == null)
				{
					column = new DataColumn (this.rowsCount, timestamp);

					//	Les colonnes sont triées chronologiquement. Il faut donc insérer
					//	la nouvelle colonne à la bonne place.
					int i = this.columns.Where (x => x.Timestamp < timestamp).Count ();
					this.columns.Insert (i, column);
				}

				return column;
			}

			private readonly List<DataColumn> columns;
			private readonly List<string> rowsLabel;
			private int rowsCount;
		}

		/// <summary>
		/// Une colonne pour un Timestamp donné, avec une ligne par objet.
		/// </summary>
		private class DataColumn
		{
			public DataColumn(int rowsCount, Timestamp timestamp)
			{
				this.Timestamp = timestamp;

				this.cells = new DataCell[rowsCount];

				for (int i=0; i<rowsCount; i++)
				{
					this.cells[i] = DataCell.Empty;
				}
			}

			public readonly Timestamp Timestamp;

			public DataCell this[int index]
			{
				get
				{
					return this.cells[index];
				}
				set
				{
					this.cells[index] = value;
				}
			}

			private readonly DataCell[] cells;
		}

		/// <summary>
		/// Une cellule, correspondant à un événement d'un objet.
		/// </summary>
		private class DataCell
		{
			public DataCell(TimelineGlyph glyph, bool locked = false, string tooltip = null)
			{
				this.Glyph   = glyph;
				this.Locked  = locked;
				this.Tooltip = tooltip;
			}

			public bool IsEmpty
			{
				get
				{
					return this.Glyph == TimelineGlyph.Empty;
				}
			}

			public static DataCell Empty = new DataCell (TimelineGlyph.Empty);

			public readonly TimelineGlyph	Glyph;
			public readonly bool			Locked;
			public readonly string			Tooltip;
		}
		#endregion


		protected void UpdateToolbar()
		{
			this.UpdateObjectCommand (ToolbarCommand.First, this.selectedRow, this.FirstRowIndex);
			this.UpdateObjectCommand (ToolbarCommand.Prev,  this.selectedRow, this.PrevRowIndex);
			this.UpdateObjectCommand (ToolbarCommand.Next,  this.selectedRow, this.NextRowIndex);
			this.UpdateObjectCommand (ToolbarCommand.Last,  this.selectedRow, this.LastRowIndex);

			this.objectsToolbar.UpdateCommand (ToolbarCommand.CompactAll, !this.nodesGetter.IsAllCompacted);
			this.objectsToolbar.UpdateCommand (ToolbarCommand.ExpandAll,  !this.nodesGetter.IsAllExpanded);

			this.objectsToolbar.UpdateCommand (ToolbarCommand.New,      this.selectedRow != -1);
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
			if (this.SelectedCellChanged != null)
			{
				this.SelectedCellChanged (this);
			}
		}

		public delegate void SelectedCellChangedEventHandler(object sender);
		public event SelectedCellChangedEventHandler SelectedCellChanged;


		private void OnCellDoubleClicked()
		{
			if (this.CellDoubleClicked != null)
			{
				this.CellDoubleClicked (this);
			}
		}

		public delegate void CellDoubleClickedEventHandler(object sender);
		public event CellDoubleClickedEventHandler CellDoubleClicked;
		#endregion


		private static readonly int lineHeight      = 20;
		private static readonly int leftColumnWidth = 220;

		private readonly DataAccessor			accessor;
		private readonly BaseType				baseType;
		private readonly TreeObjectsNodesGetter	nodesGetter;
		private readonly DataArray				dataArray;

		private string							title;
		private TopTitle						topTitle;
		private TreeTableToolbar				objectsToolbar;
		private VSplitter						splitter;
		private TimelinesToolbar				timelinesToolbar;
		private TreeTableColumnTree				treeColumn;
		private NavigationTimelineController	controller;
		private VScroller						scroller;
		private int								selectedRow;
		private int								selectedColumn;
	}
}
