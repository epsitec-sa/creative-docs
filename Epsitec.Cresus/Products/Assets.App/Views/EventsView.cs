//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EventsView : AbstractView
	{
		public EventsView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
			this.baseType = BaseType.Objects;

			this.dataArray = new DataArray ();
			this.UpdateData ();
		}

		public override void CreateUI(Widget parent)
		{
			this.scroller = new VScroller
			{
				Parent     = parent,
				Dock       = DockStyle.Right,
				Margins    = new Margins (0, 0, EventsView.lineHeight*2, AbstractScroller.DefaultBreadth),
				IsInverted = true,  // le zéro est en haut
			};

			this.controller = new NavigationTimelineController ();
			this.controller.CreateUI (parent);
			this.controller.RelativeWidth = 1.0;
			this.controller.ShowLabels = true;
			this.controller.CellsCount = this.dataArray.ColumnsCount;
			
			this.UpdateController ();
			this.UpdateScroller ();
			
			//	Connexion des événements.
			parent.SizeChanged += delegate
			{
				this.UpdateScroller ();
			};

			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};
			
			this.controller.CellClicked += delegate (object sender, int row, int rank)
			{
			};
			
			this.controller.CellDoubleClicked += delegate (object sender, int row, int rank)
			{
			};

			this.scroller.ValueChanged += delegate
			{
				this.UpdateController ();
			};

			this.Update ();
		}


		private TimelineRowDescription[] TimelineRows
		{
			//	Retourne les descriptions des lignes, de bas en haut.
			get
			{
				var list = new List<TimelineRowDescription> ();

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


		private void UpdateController(bool crop = true)
		{
			this.controller.SetRows (this.TimelineRows);

			int visibleCount = this.controller.VisibleCellsCount;
			int cellsCount   = this.dataArray.ColumnsCount;
			int count        = System.Math.Min (visibleCount, cellsCount);
			int firstCell    = this.controller.LeftVisibleCell;
			int selection    = -1;

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

			//	Ajoute les lignes des objets, de bas en haut.
			foreach (var row in this.EnumVisibleRows.Reverse ())
			{
				var glyphs = new List<TimelineCellGlyph> ();

				for (int i = 0; i < count; i++)
				{
					var cell = this.dataArray.GetCell (row, firstCell+i);

					var g = new TimelineCellGlyph (cell.Glyph, cell.Locked, cell.Tooltip);
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
				int firstRow = (int) this.scroller.Value;
				int visibleRows = this.VisibleRows;

				for (int row=0; row<this.dataArray.RowsCount; row++)
				{
					if (row >= firstRow && row < firstRow+visibleRows)
					{
						yield return row;
					}
				}
			}
		}

		private int VisibleRows
		{
			get
			{
				return (int) (this.scroller.ActualHeight / EventsView.lineHeight);
			}
		}


		private void UpdateData()
		{
			var nodeFiller = new FinalObjectsNodeFiller (this.accessor, this.baseType);
			this.dataArray.Clear (nodeFiller.NodesCount);

			for (int row=0; row<nodeFiller.NodesCount; row++)
			{
				var node = nodeFiller.GetNode (row);
				var obj = this.accessor.GetObject (this.baseType, node.Guid);

				var label = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Nom);
				this.dataArray.RowsLabel.Add (label);

				foreach (var e in obj.Events)
				{
					var column = this.dataArray.GetColumn (e.Timestamp);
					column[row] = this.GetCell (obj, e);
				}
			}
		}

		private DataCell GetCell(DataObject obj, DataEvent e)
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


		private static readonly int lineHeight = 20;

		private readonly DataArray				dataArray;

		private NavigationTimelineController	controller;
		private VScroller						scroller;
	}
}
