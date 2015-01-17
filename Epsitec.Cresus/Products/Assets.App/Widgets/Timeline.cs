//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Timeline de base, constituée de lignes AbstractTimelineRow créées créées avec SetRows.
	/// On ne gère ici aucun déplacement dans le temps.
	/// On se contente d'afficher les AbstractTimelineRow passées avec SetRows.
	/// Un seul événement CellClicked permet de connaître la ligne et la cellule cliquée.
	/// </summary>
	public class Timeline : Widget
	{
		public Timeline()
		{
			//	Permet de mettre le focus sur le TreeTable. Cela est nécessaire pour que les
			//	touches flèches fonctionnent. Sinon, ProcessMessage n'est pas appelé !
			this.AutoFocus = true;
			this.InternalState |= WidgetInternalState.Focusable;

			this.timelineRows = new List<AbstractTimelineRow> ();
			this.RelativeWidth = 1.0;
		}


		public double							Pivot
		{
			get
			{
				return this.pivot;
			}
			set
			{
				if (this.pivot != value)
				{
					this.pivot = value;
					this.UpdateChildrensPivot ();
					this.Invalidate ();
				}
			}
		}

		public int								VisibleCellsCount
		{
			get
			{
				return (int) ((this.ActualBounds.Width - this.labelsWidth) / (this.CellHeight * this.RelativeWidth));
			}
		}

		public int								BeforePivotCount
		{
			get
			{
				var num   = this.VisibleCellsCount;
				var pivot = this.Pivot;

				return (int) (num * pivot);
			}
		}
		
		public int								AfterPivotCount
		{
			get
			{
				return this.VisibleCellsCount - this.BeforePivotCount;
			}
		}

		public bool								ShowLabels
		{
			get
			{
				return this.showLabels;
			}
			set
			{
				if (this.showLabels != value)
				{
					this.showLabels = value;
					this.UpdateLabelsWidth ();
				}
			}
		}

		public bool								PermanentGrid
		{
			get
			{
				if (this.timelineRows.Any ())
				{
					return this.timelineRows.First ().PermanentGrid;
				}
				else
				{
					return false;
				}
			}
			set
			{
				foreach (var row in this.timelineRows)
				{
					row.PermanentGrid = value;
				}
			}
		}

		private int								CellHeight
		{
			get
			{
				return (int) (this.ActualHeight / this.RelativeHeights);
			}
		}

		private double							RelativeHeights
		{
			get
			{
				return this.timelineRows.Sum (x => x.RelativeHeight);
			}
		}

		public double							RelativeWidth;

		public int								TopRowsWithExactHeight;


		public void SetRows(TimelineRowDescription[] descriptions)
		{
			//	Spécifie toutes les lignes-enfant contenues dans la timeline, de bas en haut.
			this.timelineRows.Clear ();
			this.Children.Clear ();

			int index = 0;

			foreach (var description in descriptions)
			{
				var row = TimelineRowDescription.Create (description);
				row.Index = index++;

				this.timelineRows.Add (row);
				this.Children.Add (row);

				row.CellHovered += delegate (object sender, int rank)
				{
					this.SetHilitedHoverRank (rank);
				};

				row.CellClicked += delegate (object sender, int rank)
				{
					this.OnCellClicked (row.Index, rank);
					this.Focus ();  // pour que les touches flèches fonctionnent
				};

				row.CellDoubleClicked += delegate (object sender, int rank)
				{
					this.OnCellDoubleClicked (row.Index, rank);
					this.Focus ();  // pour que les touches flèches fonctionnent
				};

				row.CellRightClicked += delegate (object sender, int rank, Point pos)
				{
					this.OnCellRightClicked (row.Index, rank, pos);
					this.Focus ();  // pour que les touches flèches fonctionnent
				};
			}

			this.UpdateChildrensGeometry ();
		}

		public void SetRowYearCells(int rank, TimelineCellDate[] cells)
		{
			var row = this.GetRow (rank) as TimelineRowYears;
			System.Diagnostics.Debug.Assert (row != null);

			row.SetCells (cells);
		}

		public void SetRowMonthCells(int rank, TimelineCellDate[] cells)
		{
			var row = this.GetRow (rank) as TimelineRowMonths;
			System.Diagnostics.Debug.Assert (row != null);

			row.SetCells (cells);
		}

		public void SetRowWeekOfYearCells(int rank, TimelineCellDate[] cells)
		{
			var row = this.GetRow (rank) as TimelineRowWeeksOfYear;
			System.Diagnostics.Debug.Assert (row != null);

			row.SetCells (cells);
		}

		public void SetRowDayOfWeekCells(int rank, TimelineCellDate[] cells)
		{
			var row = this.GetRow (rank) as TimelineRowDaysOfWeek;
			System.Diagnostics.Debug.Assert (row != null);

			row.SetCells (cells);
		}

		public void SetRowDayCells(int rank, TimelineCellDate[] cells)
		{
			var row = this.GetRow (rank) as TimelineRowDays;
			System.Diagnostics.Debug.Assert (row != null);

			row.SetCells (cells);
		}

		public void SetRowDayMonthCells(int rank, TimelineCellDate[] cells)
		{
			var row = this.GetRow (rank) as TimelineRowDaysMonths;
			System.Diagnostics.Debug.Assert (row != null);

			row.SetCells (cells);
		}

		public void SetRowValueCells(int rank, TimelineCellValue[] cells, decimal min, decimal max, IEnumerable<string> valuesFieldNames)
		{
			var row = this.GetRow (rank) as TimelineRowValues;
			System.Diagnostics.Debug.Assert (row != null);

			row.SetCells (cells);
			row.SetMinMax (min, max);

			row.FieldNames.Clear ();
			row.FieldNames.AddRange (valuesFieldNames);
		}

		public void SetRowGlyphCells(int rank, TimelineCellGlyph[] cells)
		{
			var row = this.GetRow (rank) as TimelineRowGlyphs;
			System.Diagnostics.Debug.Assert (row != null);

			row.SetCells (cells);
		}


		protected override void OnExited(MessageEventArgs e)
		{
			foreach (var row in this.timelineRows)
			{
				row.ClearDetectedHoverRank ();
			}

			base.OnExited (e);
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateChildrensGeometry ();
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			switch (message.MessageType)
			{
				case MessageType.KeyDown:
					if (this.ProcessKey (message.KeyCode))
					{
						message.Captured = true;
						message.Consumer = this;
					}
					break;
			}

			base.ProcessMessage (message, pos);
		}

		private bool ProcessKey(KeyCode keyCode)
		{
			switch (keyCode)
			{
				case KeyCode.ArrowLeft:
				case KeyCode.ArrowRight:
				case KeyCode.ArrowUp:
				case KeyCode.ArrowDown:
				case KeyCode.PageUp:
				case KeyCode.PageDown:
				case KeyCode.Home:
				case KeyCode.End:
					this.OnDokeySelect (keyCode);
					return true;

				default:
					return false;
			}
		}


		private void SetHilitedHoverRank(int rank)
		{
			foreach (var row in this.timelineRows)
			{
				row.HilitedHoverRank = rank;
			}
		}

		private void UpdateChildrensGeometry()
		{
			//	Met à jour la géométrie de toutes les lignes-enfant.
			this.UpdateLabelsWidth ();

			//	La hauteur totale est répartie entre les différentes lignes, afin
			//	d'occuper toujours toute la hauteur. Il en résulte des hauteurs
			//	légèrement différentes (1 pixel) d'une ligne à l'autre.
			if (this.timelineRows.Count > 0)
			{
				if (this.TopRowsWithExactHeight == 0)
				{
					//	Cas de la timeline normale en bas.
					int ch = this.CellHeight;

					double bottom = 0;

					for (int i=0; i<this.timelineRows.Count; i++)
					{
						var row = this.timelineRows[i];

						row.CellWidth     = (int) (ch * this.RelativeWidth);
						row.RelativeWidth = this.RelativeWidth;

						double h = ch * row.RelativeHeight;
						double top = bottom + h;

						int bi = (int) bottom;
						int ti = (int) top;

						var rect = new Rectangle (0, bi, this.ActualWidth, ti-bi);
						row.SetManualBounds (rect);

						bottom = top;
					}
				}
				else
				{
					//	Cas de la timeline en haut, intégrée au tableau des événements.
					int ch = this.CellHeight;
					int hi = (int) this.ActualHeight / this.timelineRows.Count;
					var h = (this.ActualHeight - hi*this.TopRowsWithExactHeight) / (this.timelineRows.Count - this.TopRowsWithExactHeight);

					double bottom = 0;

					for (int i=0; i<this.timelineRows.Count; i++)
					{
						var row = this.timelineRows[i];

						row.CellWidth     = (int) (ch * this.RelativeWidth);
						row.RelativeWidth = this.RelativeWidth;

						double top = bottom + h;

						if (i == this.timelineRows.Count - this.TopRowsWithExactHeight - 1)
						{
							h = hi;
							top = System.Math.Floor (top+0.5);
						}

						int bi = (int) bottom;
						int ti = (int) top;

						var rect = new Rectangle (0, bi, this.ActualWidth, ti-bi);
						row.SetManualBounds (rect);

						bottom = top;
					}
				}
			}
		}

		private void UpdateChildrensPivot()
		{
			//	Met à jour le pivot de toutes les lignes-enfant.
			foreach (var row in this.timelineRows)
			{
				row.Pivot = this.pivot;
			}
		}

		private void UpdateLabelsWidth()
		{
			//	Met à jour la largeur nécessaire pour les labels de gauche.
			this.labelsWidth = 0;

			if (this.showLabels && this.timelineRows.Any ())
			{
				this.labelsWidth = this.timelineRows.Max
				(
					row => row.Description.GetTextWidth (Font.DefaultFont, this.FontSize)
				);

				if (this.labelsWidth > 0)
				{
					this.labelsWidth += this.LabelMargin * 2;
				}

				foreach (var row in this.timelineRows)
				{
					row.LabelWidth = this.labelsWidth;
				}
			}
		}

		private int LabelMargin
		{
			get
			{
				return (int) (this.CellHeight * 0.5);
			}
		}

		private double FontSize
		{
			get
			{
				return this.CellHeight * 0.6;
			}
		}

		private AbstractTimelineRow GetRow(int rank)
		{
			System.Diagnostics.Debug.Assert (rank >= 0 && rank < this.timelineRows.Count);
			return this.timelineRows[rank];
		}


		#region Events handler
		private void OnCellClicked(int row, int rank)
		{
			this.CellClicked.Raise (this, row, rank);
		}

		public event EventHandler<int, int> CellClicked;


		private void OnCellDoubleClicked(int row, int rank)
		{
			this.CellDoubleClicked.Raise (this, row, rank);
		}

		public event EventHandler<int, int> CellDoubleClicked;


		private void OnCellRightClicked(int row, int rank, Point pos)
		{
			this.CellRightClicked.Raise (this, row, rank, pos);
		}

		public event EventHandler<int, int, Point> CellRightClicked;


		private void OnDokeySelect(KeyCode key)
		{
			this.DokeySelect.Raise (this, key);
		}

		public event EventHandler<KeyCode> DokeySelect;
		#endregion


		private readonly List<AbstractTimelineRow> timelineRows;

		private double							pivot;
		private bool							showLabels;
		private int								labelsWidth;
	}
}