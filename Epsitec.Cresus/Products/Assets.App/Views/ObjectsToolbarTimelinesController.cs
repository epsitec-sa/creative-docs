//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsToolbarTimelinesController
	{
		public ObjectsToolbarTimelinesController(DataAccessor accessor, BaseType baseType)
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

			this.nodeFiller = new FinalObjectsNodeFiller (this.accessor, this.baseType);
			this.dataArray = new DataArray ();
			this.UpdateData ();
		}


		public Guid SelectedGuid
		{
			get
			{
				if (this.selectedRow != -1)
				{
					var node = this.nodeFiller.GetNode (this.selectedRow);
					if (!node.IsEmpty)
					{
						return node.Guid;
					}
				}

				return Guid.Empty;
			}
			set
			{
				int sel = this.nodeFiller.Nodes.ToList ().FindIndex (x => x.Guid == value);

				if (this.selectedRow != sel)
				{
					this.selectedRow = sel;
					this.UpdateController ();
				}
			}
		}

		public Timestamp? Timestamp
		{
			get
			{
				if (this.selectedColumn == -1)
				{
					return null;
				}
				else
				{
					return this.dataArray.GetColumn (this.selectedColumn).Timestamp;
				}
			}
			set
			{
				if (value.HasValue)
				{
					int sel = this.dataArray.FindColumnIndex (value.Value);

					if (this.selectedColumn != sel)
					{
						this.selectedColumn = sel;
						this.UpdateController ();
					}
				}
			}
		}
	
		
		public void CreateUI(Widget parent)
		{
			this.topTitle = new TopTitle
			{
				Parent = parent,
			};

			this.topTitle.SetTitle (this.title);

			this.toolbar = new TimelinesToolbar ();
			this.toolbar.CreateUI (parent);

			var box = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.scroller = new VScroller
			{
				Parent     = box,
				Dock       = DockStyle.Right,
				Margins    = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),
				IsInverted = true,  // le zéro est en haut
			};

			this.controller = new NavigationTimelineController ();
			this.controller.CreateUI (box);
			this.controller.RelativeWidth = 1.0;
			this.controller.ShowLabels = true;
			this.controller.CellsCount = this.dataArray.ColumnsCount;
			
			this.UpdateController ();
			this.UpdateScroller ();
			this.UpdateToolbar ();
			
			//	Connexion des événements.
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
					this.selectedRow = sel;
					this.selectedColumn = this.controller.LeftVisibleCell + rank;
					this.UpdateController ();
					this.UpdateToolbar ();
					this.OnSelectedCellChanged ();
				}
			};
			
			this.controller.CellDoubleClicked += delegate (object sender, int row, int rank)
			{
				int sel = this.LineToRow (row);
				if (sel != -1)
				{
					this.selectedRow = sel;
					this.selectedColumn = this.controller.LeftVisibleCell + rank;
					this.UpdateController ();
					this.UpdateToolbar ();
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

			this.toolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.First:
						this.OnFirst ();
						break;

					case ToolbarCommand.Last:
						this.OnLast ();
						break;

					case ToolbarCommand.Prev:
						this.OnPrev ();
						break;

					case ToolbarCommand.Next:
						this.OnNext ();
						break;

					case ToolbarCommand.New:
						this.OnNew ();
						break;

					case ToolbarCommand.Delete:
						this.OnDelete ();
						break;

					case ToolbarCommand.Deselect:
						this.OnDeselect ();
						break;
				}
			};
		}


		private void OnFirst()
		{
			var index = this.FirstColumnIndex;

			if (index.HasValue)
			{
				this.selectedColumn = index.Value;
				this.UpdateController ();
				this.UpdateToolbar ();
			}
		}

		private void OnPrev()
		{
			var index = this.PrevColumnIndex;

			if (index.HasValue)
			{
				this.selectedColumn = index.Value;
				this.UpdateController ();
				this.UpdateToolbar ();
			}
		}

		private void OnNext()
		{
			var index = this.NextColumnIndex;

			if (index.HasValue)
			{
				this.selectedColumn = index.Value;
				this.UpdateController ();
				this.UpdateToolbar ();
			}
		}

		private void OnLast()
		{
			var index = this.LastColumnIndex;

			if (index.HasValue)
			{
				this.selectedColumn = index.Value;
				this.UpdateController ();
				this.UpdateToolbar ();
			}
		}

		private void OnNew()
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

		private void OnDelete()
		{
			var target = this.toolbar.GetCommandWidget (ToolbarCommand.Delete);

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

		private void OnDeselect()
		{
			this.selectedColumn = -1;
			this.UpdateController ();
			this.UpdateToolbar ();
		}

	
		private void UpdateController(bool crop = true)
		{
			this.controller.SetRows (this.TimelineRows);

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

			var totalRows   = (decimal) this.dataArray.RowsCount;
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
				int firstRow = this.FirstVisibleRow;
				int visibleRows = this.VisibleRows;
				int count = System.Math.Min (this.dataArray.RowsCount, firstRow+visibleRows);

				for (int row=firstRow; row<count; row++)
				{
					yield return row;
				}
			}
		}

		private int LineToRow(int line)
		{
			var dummy = this.DummyCount;
			int count = System.Math.Min (this.dataArray.RowsCount, this.VisibleRows);

			if (line >= dummy && line < dummy+count)
			{
				return this.FirstVisibleRow + count + dummy - 2 + 1 - line;
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
				return System.Math.Max (this.VisibleRows - this.dataArray.RowsCount, 0);
			}
		}

		private int FirstVisibleRow
		{
			get
			{
				return (int) this.scroller.Value;
			}
		}

		private int VisibleRows
		{
			get
			{
				return (int) (this.scroller.ActualHeight / ObjectsToolbarTimelinesController.lineHeight);
			}
		}


		private void UpdateData()
		{
			//	Met à jour this.dataArray en fonction de l'ensemble des événements de
			//	tous les objets. Cela nécessite d'accéder à l'ensemble des données, ce
			//	qui peut être long. Néanmoins, cela est nécessaire, même si la timeline
			//	n'affiche qu'un nombre limité de lignes. En effet, il faut allouer toutes
			//	les colonnes pour lesquelles il existe un événement.

			this.dataArray.Clear (this.nodeFiller.NodesCount);

			for (int row=0; row<this.nodeFiller.NodesCount; row++)
			{
				var node = this.nodeFiller.GetNode (row);
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
			this.UpdateCommand (ToolbarCommand.First, this.selectedColumn, this.FirstColumnIndex);
			this.UpdateCommand (ToolbarCommand.Prev,  this.selectedColumn, this.PrevColumnIndex);
			this.UpdateCommand (ToolbarCommand.Next,  this.selectedColumn, this.NextColumnIndex);
			this.UpdateCommand (ToolbarCommand.Last,  this.selectedColumn, this.LastColumnIndex);

			this.toolbar.UpdateCommand (ToolbarCommand.New,      true);
			this.toolbar.UpdateCommand (ToolbarCommand.Delete,   this.selectedColumn != -1);
			this.toolbar.UpdateCommand (ToolbarCommand.Deselect, this.selectedColumn != -1);
		}

		private void UpdateCommand(ToolbarCommand command, int selectedCell, int? newSelection)
		{
			bool enable = (newSelection.HasValue && selectedCell != newSelection.Value);
			this.toolbar.UpdateCommand (command, enable);
		}


		private int? FirstColumnIndex
		{
			get
			{
				var obj = this.SelectedObject;
				if (obj != null && obj.Events.Any ())
				{
					var timestamp = obj.Events.First().Timestamp;
					return this.dataArray.FindColumnIndex (timestamp);
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
						int i = obj.Events.Where (x => x.Timestamp < column.Timestamp).Count () + 1;
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
				if (obj != null && obj.Events.Any ())
				{
					var timestamp = obj.Events.Last ().Timestamp;
					return this.dataArray.FindColumnIndex (timestamp);
				}

				return null;
			}
		}

		private DataObject SelectedObject
		{
			get
			{
				if (this.selectedRow != -1)
				{
					var node = this.nodeFiller.GetNode (this.selectedRow);
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


		private static readonly int lineHeight = 20;

		private readonly DataAccessor			accessor;
		private readonly BaseType				baseType;
		private readonly FinalObjectsNodeFiller	nodeFiller;
		private readonly DataArray				dataArray;

		private string							title;
		private TopTitle						topTitle;
		private TimelinesToolbar				toolbar;
		private NavigationTimelineController	controller;
		private VScroller						scroller;
		private int								selectedRow;
		private int								selectedColumn;
	}
}
