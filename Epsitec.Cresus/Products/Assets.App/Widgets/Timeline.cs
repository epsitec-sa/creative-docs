//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Timeline de base, constituée de lignes AbstractTimelineRow créées en fonction du
	/// tableau de TimelineRowDescription. On ne gère ici aucun déplacement dans le temps.
	/// On se contente d'afficher les TimelineCell passées avec SetCells.
	/// Un seul événement CellClicked permet de connaître la ligne et la cellule cliquée.
	/// </summary>
	public class Timeline : Widget
	{
		public IEnumerable<AbstractTimelineRow>	Rows
		{
			get
			{
				return this.Children.Cast<AbstractTimelineRow> ();
			}
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
				return (int) ((this.ActualBounds.Width - this.labelsWidth) / this.CellWidth);
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

		private int								CellWidth
		{
			get
			{
				return (int) (this.ActualBounds.Height / this.RelativeHeights);
			}
		}

		private double							RelativeHeights
		{
			get
			{
				return this.Children.Cast<AbstractTimelineRow> ().Sum (x => x.RelativeHeight);
			}
		}


		public void SetRows(List<AbstractTimelineRow> rows)
		{
			//	Spécifie toutes les lignes-enfant contenues dans la timeline.
			this.Children.Clear ();

			int index = 0;

			foreach (var row in rows)
			{
				row.RowIndex = index++;
				this.Children.Add (row);

				row.CellClicked += delegate (object sender, int rank)
				{
					this.OnCellClicked (row.RowIndex, rank);
				};
			}

			this.UpdateLabelsWidth ();
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateChildrensGeometry ();
		}

		private void UpdateChildrensGeometry()
		{
			//	Met à jour la géométrie de toutes les lignes-enfant.
			this.UpdateLabelsWidth ();

			int w = this.CellWidth;
			int bottom = 0;

			foreach (var children in this.Children)
			{
				var row = children as AbstractTimelineRow;
				System.Diagnostics.Debug.Assert (row != null);

				var h = (int) (w * row.RelativeHeight);
				var top = this.ActualHeight-bottom-h;

				row.Anchor = AnchorStyles.All;
				row.CellWidth = w;
				row.Margins = new Margins (0, 0, top, bottom);
				row.PreferredHeight = h;

				bottom += h;
			}
		}

		private void UpdateChildrensPivot()
		{
			//	Met à jour le pivot de toutes les lignes-enfant.
			foreach (var children in this.Children)
			{
				var row = children as AbstractTimelineRow;
				System.Diagnostics.Debug.Assert (row != null);

				row.Pivot = this.pivot;
			}
		}

		private void UpdateLabelsWidth()
		{
			//	Met à jour la largeur nécessaire pour les labels de gauche.
			this.labelsWidth = 0;

			if (this.showLabels)
			{
				foreach (var children in this.Children)
				{
					var row = children as AbstractTimelineRow;
					System.Diagnostics.Debug.Assert (row != null);

					if (!string.IsNullOrEmpty (row.Description))
					{
						this.labelsWidth = System.Math.Max (this.labelsWidth, this.GetTextWidth (row.Description));
					}
				}

				if (this.labelsWidth > 0)
				{
					this.labelsWidth += this.LabelMargin * 2;
				}

				foreach (var children in this.Children)
				{
					var row = children as AbstractTimelineRow;
					System.Diagnostics.Debug.Assert (row != null);

					row.LabelWidth = this.labelsWidth;
				}
			}
		}

		private int GetTextWidth(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return 0;
			}
			else
			{
				var font = Font.DefaultFont;
				return (int) new TextGeometry (0, 0, 1000, 100, text, font, this.FontSize, ContentAlignment.MiddleLeft).Width + 1;
			}
		}

		private int LabelMargin
		{
			get
			{
				return (int) (this.CellWidth * 0.5);
			}
		}

		private double FontSize
		{
			get
			{
				return this.CellWidth * 0.6;
			}
		}


		#region Events handler
		private void OnCellClicked(int row, int rank)
		{
			if (this.CellClicked != null)
			{
				this.CellClicked (this, row, rank);
			}
		}

		public delegate void CellClickedEventHandler(object sender, int row, int rank);
		public event CellClickedEventHandler CellClicked;
		#endregion


		private double							pivot;
		private bool							showLabels;
		private int								labelsWidth;
	}
}