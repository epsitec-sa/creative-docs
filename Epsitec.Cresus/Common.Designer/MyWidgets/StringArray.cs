using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Tableau de plusieurs colonnes, où chaque colonne est un StringList.
	/// </summary>
	public class StringArray : Widget
	{
		public StringArray() : base()
		{
			this.AutoEngage = false;
			this.AutoFocus  = true;

			this.InternalState |= InternalState.Focusable;
			this.InternalState |= InternalState.Engageable;

			this.scroller = new VScroller(this);
			this.scroller.IsInverted = true;
			this.scroller.ValueChanged += new EventHandler(this.HandleScrollerValueChanged);
		}

		public StringArray(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.scroller.ValueChanged -= new EventHandler(this.HandleScrollerValueChanged);
			}

			base.Dispose(disposing);
		}


		public int Columns
		{
			//	Choix du nombre de colonnes.
			get
			{
				if ( this.columns == null )  return 0;
				return this.columns.Length;
			}

			set
			{
				if (this.columns != null)
				{
					for (int i=0; i<this.columns.Length; i++)
					{
						this.columns[i].DraggingCellSelectionChanged -= new EventHandler(this.HandleDraggingCellSelectionChanged);
						this.columns[i].FinalCellSelectionChanged -= new EventHandler(this.HandleFinalCellSelectionChanged);
					}
					this.columns[this.columns.Length-1].CellsQuantityChanged -= new EventHandler(this.HandleCellsQuantityChanged);
				}

				this.columns = new StringList[value];
				for (int i=0; i<this.columns.Length; i++)
				{
					this.columns[i] = new StringList(this);
					this.columns[i].DraggingCellSelectionChanged += new EventHandler(this.HandleDraggingCellSelectionChanged);
					this.columns[i].FinalCellSelectionChanged += new EventHandler(this.HandleFinalCellSelectionChanged);
					ToolTip.Default.SetToolTip(this.columns[i], "*");
				}
				this.columns[this.columns.Length-1].CellsQuantityChanged += new EventHandler(this.HandleCellsQuantityChanged);
			}
		}

		public void SetColumnsRelativeWidth(int column, double width)
		{
			//	Modifie la largeur relative d'une colonne.
			this.columns[column].RelativeWidth = width;
		}

		public double GetColumnsRelativeWidth(int column)
		{
			//	Retourne la largeur relative d'une colonne.
			return this.columns[column].RelativeWidth;
		}

		public double ColumnsRelativeTotalWidth
		{
			//	Retourne la somme des largeurs relatives des colonnes.
			get
			{
				double total = 0;
				for (int i=0; i<this.columns.Length; i++)
				{
					total += this.columns[i].RelativeWidth;
				}
				return total;
			}
		}

		public double GetColumnsAbsoluteWidth(int column)
		{
			//	Retourne la largeur absolue d'une colonne.
			double w = this.Client.Bounds.Width - this.scroller.Width;
			return System.Math.Floor(w*this.GetColumnsRelativeWidth(column)/this.ColumnsRelativeTotalWidth);
		}

		public double LineHeight
		{
			//	Hauteur d'une ligne.
			get
			{
				if ( this.columns == null )  return 0;
				return this.columns[0].LineHeight;
			}

			set
			{
				if ( this.columns == null )  return;
				if ( this.columns[0].LineHeight != value )
				{
					for (int i=0; i<this.columns.Length; i++)
					{
						this.columns[i].LineHeight = value;
					}
				}
			}
		}

		public int LineCount
		{
			//	Nombre total de ligne en fonction de la hauteur du widget et de la hauteur d'une ligne.
			get
			{
				if ( this.columns == null )  return 0;
				return this.columns[0].LineCount;
			}
		}

		public void SetLineString(int column, int row, string text)
		{
			//	Spécifie le texte contenu dans une ligne.
			if ( this.columns == null )  return;
			if ( column < 0 || column >= this.columns.Length )  return;
			this.columns[column].SetLineString(row-this.firstVisibleRow, text);
		}

		public string GetLineString(int column, int row)
		{
			//	Retourne le texte contenu dans une ligne.
			if ( this.columns == null )  return null;
			if ( column < 0 || column >= this.columns.Length )  return null;
			return this.columns[column].GetLineString(row-this.firstVisibleRow);
		}

		public void SetLineState(int column, int row, StringList.CellState state)
		{
			//	Spécifie l'état d'une ligne.
			if ( this.columns == null )  return;
			this.columns[column].SetLineState(row-this.firstVisibleRow, state);
		}

		public StringList.CellState GetLineState(int column, int row)
		{
			//	Retourne l'état d'une ligne.
			if ( this.columns == null )  return StringList.CellState.Normal;
			return this.columns[column].GetLineState(row-this.firstVisibleRow);
		}

		public void SetDynamicsToolTips(int column, bool state)
		{
			//	Spécifie si une colonne génère des tooltips dynamiques.
			this.columns[column].IsDynamicsToolTips = state;
		}

		public bool GetDynamicsToolTips(int column)
		{
			//	Retourne si une colonne génère des tooltips dynamiques.
			return this.columns[column].IsDynamicsToolTips;
		}

		public int TotalRows
		{
			//	Nombre total de lignes, pour la gestion de l'ascenseur.
			get
			{
				return this.totalRows;
			}

			set
			{
				if (this.totalRows != value)
				{
					this.totalRows = value;
					this.UpdateScroller();
				}
			}
		}

		public int SelectedRow
		{
			//	Ligne sélectionnée.
			get
			{
				return this.selectedRow;
			}

			set
			{
				this.SetSelectedRow(value, -1);
			}
		}

		protected void SetSelectedRow(int row, int column)
		{
			//	Sélectionne une ligne, en indiquant également la colonne cliquée, pour SelectedColumn.
			this.selectedColumn = column;

			row = System.Math.Max(row, -1);
			row = System.Math.Min(row, this.TotalRows-1);

			if (this.selectedRow != row)
			{
				this.selectedRow = row;
				this.UpdateSelectedRow();
			}

			//	Il faut envoyer l'événement même si la ligne n'a pas changé !
			this.OnSelectedRowChanged();
		}

		public int SelectedColumn
		{
			//	Colonne dans laquelle on a cliqué pour sélectionner la ligne.
			get
			{
				return this.selectedColumn;
			}
		}

		public int FirstVisibleRow
		{
			//	Première ligne visible.
			get
			{
				return this.firstVisibleRow;
			}

			set
			{
				value = System.Math.Min(value, this.TotalRows-this.LineCount);
				value = System.Math.Max(value, 0);

				if (this.firstVisibleRow != value)
				{
					this.firstVisibleRow = value;
					this.UpdateSelectedRow();
					this.UpdateScroller();
					this.OnCellsContentChanged();
				}
			}
		}

		public void ShowSelectedRow()
		{
			//	Montre la ligne sélectionnée.
			int first = 0;

			if (this.SelectedRow != -1)
			{
				if (this.SelectedRow >= this.FirstVisibleRow && this.SelectedRow < this.FirstVisibleRow+this.LineCount)
				{
					return;
				}

				first = this.SelectedRow;
				first = System.Math.Min(first+this.LineCount/2, this.TotalRows);
				first = System.Math.Max(first-this.LineCount, 0);
			}

			this.FirstVisibleRow = first;
		}


		protected void UpdateSelectedRow()
		{
			for (int i=0; i<this.columns.Length; i++)
			{
				this.columns[i].CellSelected = this.selectedRow-this.firstVisibleRow;
			}
		}

		protected void UpdateScroller()
		{
			this.ignoreChange = true;

			if (this.totalRows <= this.LineCount)
			{
				this.scroller.Enable = false;
			}
			else
			{
				this.scroller.Enable = true;
				this.scroller.MinValue = (decimal) 0;
				this.scroller.MaxValue = (decimal) (this.totalRows - this.LineCount);
				this.scroller.Value = (decimal) this.firstVisibleRow;
				this.scroller.VisibleRangeRatio = (decimal) this.LineCount / (decimal) this.totalRows;
				this.scroller.LargeChange = (decimal) this.LineCount-1;
				this.scroller.SmallChange = (decimal) 1;
			}

			this.ignoreChange = false;
		}


		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			if (this.IsEnabled == false)
			{
				return;
			}

			if (message.Type == MessageType.MouseWheel)
			{
				if (message.Wheel > 0)  this.FirstVisibleRow -= 3;
				if (message.Wheel < 0)  this.FirstVisibleRow += 3;
			}

			if (message.Type == MessageType.MouseDown)
			{
				this.WidthDraggingBegin(pos);
				if (this.widthDraggingRank != -1)
				{
					message.Captured = true;
					message.Consumer = this;
					return;
				}
			}

			if (message.Type == MessageType.MouseMove)
			{
				if (this.widthDraggingRank == -1)
				{
					// TODO: pourquoi ça ne marche pas (clignottements insupportables) ?
#if false
					MouseCursor cursor = MouseCursor.AsArrow;

					if (this.WidthDraggingDetect(pos) != -1)
					{
						cursor = MouseCursor.AsVSplit;
					}

					if (this.MouseCursor != cursor)
					{
						this.MouseCursor = cursor;
					}
#endif
				}
				else
				{
					this.WidthDraggingMove(pos);
					message.Captured = true;
					message.Consumer = this;
					return;
				}
			}

			if (message.Type == MessageType.MouseUp)
			{
				if (this.widthDraggingRank != -1)
				{
					this.WidthDraggingEnd(pos);
					message.Captured = true;
					message.Consumer = this;
					return;
				}
			}

			if (message.Type == MessageType.KeyDown)
			{
				if (message.KeyCode == KeyCode.ArrowUp)
				{
					if (message.IsControlPressed)
					{
						this.FirstVisibleRow--;
					}
					else
					{
						this.SelectedRow--;
						this.ShowSelectedRow();
					}
				}

				if (message.KeyCode == KeyCode.ArrowDown)
				{
					if (message.IsControlPressed)
					{
						this.FirstVisibleRow++;
					}
					else
					{
						this.SelectedRow++;
						this.ShowSelectedRow();
					}
				}

				if (message.KeyCode == KeyCode.PageUp)
				{
					if (message.IsControlPressed)
					{
						this.FirstVisibleRow = this.FirstVisibleRow-this.LineCount;
					}
					else
					{
						this.SelectedRow = this.SelectedRow-this.LineCount;
						this.ShowSelectedRow();
					}
				}

				if (message.KeyCode == KeyCode.PageDown)
				{
					if (message.IsControlPressed)
					{
						this.FirstVisibleRow = this.FirstVisibleRow+this.LineCount;
					}
					else
					{
						this.SelectedRow = this.SelectedRow+this.LineCount;
						this.ShowSelectedRow();
					}
				}

				if (message.KeyCode == KeyCode.Home)
				{
					if (message.IsControlPressed)
					{
						this.FirstVisibleRow = 0;
					}
					else
					{
						this.SelectedRow = 0;
						this.ShowSelectedRow();
					}
				}

				if (message.KeyCode == KeyCode.End)
				{
					if (message.IsControlPressed)
					{
						this.FirstVisibleRow = 100000;
					}
					else
					{
						this.SelectedRow = 100000;
						this.ShowSelectedRow();
					}
				}
			}

			base.ProcessMessage(message, pos);
		}


		#region WidthDragging
		protected bool WidthDraggingBegin(Point pos)
		{
			this.widthDraggingRank = this.WidthDraggingDetect(pos);
			if ( this.widthDraggingRank == -1 )  return false;

			this.widthDraggingAbsolutes = new double[this.columns.Length+1];
			double x = this.Client.Bounds.Left;
			this.widthDraggingAbsolutes[0] = x;
			for (int i=0; i<this.columns.Length; i++)
			{
				x += this.GetColumnsAbsoluteWidth(i);
				this.widthDraggingAbsolutes[i+1] = x;
			}

			this.widthDraggingMin = this.widthDraggingAbsolutes[this.widthDraggingRank+0]+20;
			this.widthDraggingMax = this.widthDraggingAbsolutes[this.widthDraggingRank+2]-20;
			
			this.widthDraggingPos = pos.X;
			this.Invalidate();
			return true;
		}

		protected void WidthDraggingMove(Point pos)
		{
			pos.X = System.Math.Max(pos.X, this.widthDraggingMin);
			pos.X = System.Math.Min(pos.X, this.widthDraggingMax);
			if (this.widthDraggingPos != pos.X)
			{
				this.widthDraggingPos = pos.X;
				this.Invalidate();
			}
		}

		protected void WidthDraggingEnd(Point pos)
		{
			this.widthDraggingAbsolutes[this.widthDraggingRank+1] = this.widthDraggingPos;

			for (int i=0; i<this.columns.Length; i++)
			{
				double w = this.widthDraggingAbsolutes[i+1]-this.widthDraggingAbsolutes[i];
				this.SetColumnsRelativeWidth(i, w);
			}
			this.UpdateClientGeometry();

			this.widthDraggingRank = -1;
			this.Invalidate();
			this.OnColumnsWidthChanged();
		}

		protected int WidthDraggingDetect(Point pos)
		{
			//	Détecte dans quel séparateur de colonne est la souris.
			double x = this.Client.Bounds.Left;
			for (int i=0; i<this.columns.Length-1; i++)
			{
				x += this.GetColumnsAbsoluteWidth(i);
				if (pos.X >= x-StringList.WidthDraggingDetectMargin && pos.X <= x+StringList.WidthDraggingDetectMargin)
				{
					return i;
				}
			}
			return -1;
		}
		#endregion


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.columns == null )  return;

			Rectangle rect = this.Client.Bounds;

			for (int i=0; i<this.columns.Length; i++)
			{
				if (i < this.columns.Length-1)
				{
					rect.Width = this.GetColumnsAbsoluteWidth(i) + 1;
				}
				else
				{
					rect.Right = this.Client.Bounds.Right-this.scroller.Width;
				}

				this.columns[i].Bounds = rect;

				rect.Left = rect.Right-1;
			}

			rect = this.Client.Bounds;
			rect.Left = rect.Right-this.scroller.Width;
			this.scroller.Bounds = rect;
		}


		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (this.widthDraggingRank != -1)
			{
				IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
				Rectangle rect = this.Client.Bounds;
				rect.Left  = this.widthDraggingPos-StringList.WidthDraggingDetectMargin;
				rect.Right = this.widthDraggingPos+StringList.WidthDraggingDetectMargin;
				graphics.Align(ref rect);

				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(adorner.ColorBorder);
			}
		}


		#region Events caller
		void HandleCellsQuantityChanged(object sender)
		{
			this.OnCellsQuantityChanged();
		}

		void HandleDraggingCellSelectionChanged(object sender)
		{
			MyWidgets.StringList array = sender as MyWidgets.StringList;
			int sel = array.CellSelected;

			for (int i=0; i<this.columns.Length; i++)
			{
				this.columns[i].CellSelected = sel;
			}
		}

		void HandleFinalCellSelectionChanged(object sender)
		{
			MyWidgets.StringList array = sender as MyWidgets.StringList;
			int sel = array.CellSelected;
			int column = -1;

			for (int i=0; i<this.columns.Length; i++)
			{
				this.columns[i].CellSelected = sel;

				if (this.columns[i] == sender)
				{
					column = i;
				}
			}

			this.SetSelectedRow(this.firstVisibleRow+sel, column);
		}

		void HandleScrollerValueChanged(object sender)
		{
			this.FirstVisibleRow = (int) System.Math.Floor(this.scroller.Value+0.5M);
		}
		#endregion


		#region Events handler
		protected virtual void OnColumnsWidthChanged()
		{
			//	Génère un événement pour dire que la largeur de colonnes a changé.
			if (this.ColumnsWidthChanged != null)  // qq'un écoute ?
			{
				this.ColumnsWidthChanged(this);
			}
		}

		public event Support.EventHandler ColumnsWidthChanged;


		protected virtual void OnCellsQuantityChanged()
		{
			//	Génère un événement pour dire que le nombre de cellules a changé.
			if (this.CellsQuantityChanged != null)  // qq'un écoute ?
			{
				this.CellsQuantityChanged(this);
			}
		}

		public event Support.EventHandler CellsQuantityChanged;


		protected virtual void OnCellsContentChanged()
		{
			//	Génère un événement pour dire que le contenu des cellules a changé.
			if (this.CellsContentChanged != null)  // qq'un écoute ?
			{
				this.CellsContentChanged(this);
			}
		}

		public event Support.EventHandler CellsContentChanged;


		protected virtual void OnSelectedRowChanged()
		{
			//	Génère un événement pour dire que la ligne sélectionnée a changé.
			if (this.SelectedRowChanged != null)  // qq'un écoute ?
			{
				this.SelectedRowChanged(this);
			}
		}

		public event Support.EventHandler SelectedRowChanged;
		#endregion


		protected StringList[]				columns;
		protected VScroller					scroller;
		protected int						totalRows;
		protected int						selectedRow = -1;
		protected int						selectedColumn = -1;
		protected int						firstVisibleRow = 0;
		protected bool						ignoreChange = false;

		protected int						widthDraggingRank = -1;
		protected double[]					widthDraggingAbsolutes;
		protected double					widthDraggingMin;
		protected double					widthDraggingMax;
		protected double					widthDraggingPos;
	}
}
