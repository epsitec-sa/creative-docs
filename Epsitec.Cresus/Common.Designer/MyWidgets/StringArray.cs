using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Tableau de plusieurs colonnes, o� chaque colonne est un StringList.
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
			//	Sp�cifie le texte contenu dans une ligne.
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
			//	Sp�cifie l'�tat d'une ligne.
			if ( this.columns == null )  return;
			this.columns[column].SetLineState(row-this.firstVisibleRow, state);
		}

		public StringList.CellState GetLineState(int column, int row)
		{
			//	Retourne l'�tat d'une ligne.
			if ( this.columns == null )  return StringList.CellState.Normal;
			return this.columns[column].GetLineState(row-this.firstVisibleRow);
		}

		public void SetDynamicsToolTips(int column, bool state)
		{
			//	Sp�cifie si une colonne g�n�re des tooltips dynamiques.
			this.columns[column].IsDynamicsToolTips = state;
		}

		public bool GetDynamicsToolTips(int column)
		{
			//	Retourne si une colonne g�n�re des tooltips dynamiques.
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
			//	Ligne s�lectionn�e.
			get
			{
				return this.selectedRow;
			}

			set
			{
				value = System.Math.Max(value, -1);
				value = System.Math.Min(value, this.TotalRows-1);

				if (this.selectedRow != value)
				{
					this.selectedRow = value;
					this.UpdateSelectedRow();
					this.OnSelectedRowChanged();
				}
			}
		}

		public int FirstVisibleRow
		{
			//	Premi�re ligne visible.
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
			//	Montre la ligne s�lectionn�e.
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

		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
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

			for (int i=0; i<this.columns.Length; i++)
			{
				this.columns[i].CellSelected = sel;
			}

			this.SelectedRow = this.firstVisibleRow + sel;
		}

		void HandleScrollerValueChanged(object sender)
		{
			this.FirstVisibleRow = (int) System.Math.Floor(this.scroller.Value+0.5M);
		}
		#endregion


		#region Events handler
		protected virtual void OnCellsQuantityChanged()
		{
			//	G�n�re un �v�nement pour dire que le nombre de cellules a chang�.
			if (this.CellsQuantityChanged != null)  // qq'un �coute ?
			{
				this.CellsQuantityChanged(this);
			}
		}

		public event Support.EventHandler CellsQuantityChanged;


		protected virtual void OnCellsContentChanged()
		{
			//	G�n�re un �v�nement pour dire que le contenu des cellules a chang�.
			if (this.CellsContentChanged != null)  // qq'un �coute ?
			{
				this.CellsContentChanged(this);
			}
		}

		public event Support.EventHandler CellsContentChanged;


		protected virtual void OnSelectedRowChanged()
		{
			//	G�n�re un �v�nement pour dire que la ligne s�lectionn�e a chang�.
			if (this.SelectedRowChanged != null)  // qq'un �coute ?
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
		protected int						firstVisibleRow = 0;
		protected bool						ignoreChange = false;
	}
}
