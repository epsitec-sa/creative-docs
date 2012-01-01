//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Core.Widgets
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

			this.InternalState |= WidgetInternalState.Focusable;
			this.InternalState |= WidgetInternalState.Engageable;

			this.scroller = new VScroller(this);
			this.scroller.IsInverted = true;
			this.scroller.ValueChanged += this.HandleScrollerValueChanged;
		}

		public StringArray(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.scroller.ValueChanged -= this.HandleScrollerValueChanged;
			}

			base.Dispose(disposing);
		}


		public int Columns
		{
			//	Choix du nombre de colonnes.
			get
			{
				if (this.columns == null)
				{
					return 0;
				}

				return this.columns.Length;
			}

			set
			{
				if (this.columns != null)
				{
					for (int i=0; i<this.columns.Length; i++)
					{
						this.columns[i].DraggingCellSelectionChanged -= this.HandleDraggingCellSelectionChanged;
						this.columns[i].FinalCellSelectionChanged -= this.HandleFinalCellSelectionChanged;
						this.columns[i].DoubleClicked -= this.HandleDoubleClicked;
					}
					this.columns[this.columns.Length-1].CellCountChanged -= this.HandleCellCountChanged;

					for (int i=0; i<this.columns.Length; i++)
					{
						this.columns[i].Dispose();
						this.columns[i] = null;
					}

					this.isDirtyGeometry = true;
				}

				this.columns = new StringList[value];
				for (int i=0; i<this.columns.Length; i++)
				{
					this.columns[i] = new StringList(this);
					this.columns[i].DraggingCellSelectionChanged += this.HandleDraggingCellSelectionChanged;
					this.columns[i].FinalCellSelectionChanged += this.HandleFinalCellSelectionChanged;
					this.columns[i].DoubleClicked += this.HandleDoubleClicked;
					ToolTip.Default.RegisterDynamicToolTipHost (this.columns[i]);  // pour voir les tooltips dynamiques
				}
				this.columns[this.columns.Length-1].CellCountChanged += this.HandleCellCountChanged;
			}
		}

		public void SetColumnsRelativeWidth(int column, double width)
		{
			//	Modifie la largeur relative d'une colonne.
			if (this.columns[column].RelativeWidth != width)
			{
				this.columns[column].RelativeWidth = width;

				this.UpdateClientGeometry();
				this.Invalidate();
				this.OnColumnsWidthChanged();
			}
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

		public void SetColumnsAbsoluteWidth(int column, double width)
		{
			//	Modifie la largeur absolue d'une colonne.
			//	Il faut au préalable spécifier les largeurs relatives de toutes
			//	les colonnes, avec SetColumnsRelativeWidth !
			this.absoluteColumn = column;  // ce sera fait au prochain UpdateClientGeometry
			this.absoluteWidth = width;
		}

		public double GetColumnsAbsoluteWidth(int column)
		{
			//	Retourne la largeur absolue d'une colonne.
			double w = this.Client.Bounds.Width - this.scroller.PreferredWidth;
			return System.Math.Floor(w*this.GetColumnsRelativeWidth(column)/this.ColumnsRelativeTotalWidth);
		}

		public void SetColumnAlignment(int column, ContentAlignment alignment)
		{
			//	Modifie l'alignement d'une colonne.
			this.columns[column].Alignment = alignment;
		}

		public ContentAlignment GetColumnAlignment(int column)
		{
			//	Retourne l'alignement d'une colonne.
			return this.columns[column].Alignment;
		}

		public void SetColumnBreakMode(int column, TextBreakMode breakMode)
		{
			//	Modifie la césure d'une colonne.
			this.columns[column].BreakMode = breakMode;
		}

		public TextBreakMode GetColumnBreakMode(int column)
		{
			//	Retourne la césure d'une colonne.
			return this.columns[column].BreakMode;
		}

		public double LineHeight
		{
			//	Hauteur d'une ligne.
			get
			{
				if (this.columns == null)
				{
					return 0;
				}

				return this.columns[0].LineHeight;
			}

			set
			{
				if (this.columns == null)
				{
					return;
				}

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
				if (this.columns == null)
				{
					return 0;
				}

				return this.columns[0].LineCount;
			}
		}

		public void SetLineString(int column, int row, string text)
		{
			//	Spécifie le texte contenu dans une ligne.
			if (this.columns == null || column < 0 || column >= this.columns.Length)
			{
				return;
			}

			this.columns[column].SetLineString(row-this.firstVisibleRow, text);
		}

		public string GetLineString(int column, int row)
		{
			//	Retourne le texte contenu dans une ligne.
			if (this.columns == null || column < 0 || column >= this.columns.Length)
			{
				return null;
			}

			return this.columns[column].GetLineString(row-this.firstVisibleRow);
		}

		public void SetLineTooltip(int column, int row, string text)
		{
			//	Spécifie le tooltip d'une ligne.
			if (this.columns == null || column < 0 || column >= this.columns.Length)
			{
				return;
			}

			this.columns[column].SetLineTooltip(row-this.firstVisibleRow, text);
		}

		public string GetLineTooltip(int column, int row)
		{
			//	Retourne le texte d'une ligne.
			if (this.columns == null || column < 0 || column >= this.columns.Length)
			{
				return null;
			}

			return this.columns[column].GetLineTooltip(row-this.firstVisibleRow);
		}

		public void SetLineState(int column, int row, StringList.CellState state)
		{
			//	Spécifie l'état d'une ligne.
			if (this.columns == null)
			{
				return;
			}

			this.columns[column].SetLineState(row-this.firstVisibleRow, state);
		}

		public StringList.CellState GetLineState(int column, int row)
		{
			//	Retourne l'état d'une ligne.
			if (this.columns == null)
			{
				return StringList.CellState.Normal;
			}

			return this.columns[column].GetLineState(row-this.firstVisibleRow);
		}

		public void SetLineColor(int column, int row, Color color)
		{
			//	Spécifie la couleur de fond d'une ligne.
			if (this.columns == null)
			{
				return;
			}

			this.columns[column].SetLineColor(row-this.firstVisibleRow, color);
		}

		public Color GetLineColor(int column, int row)
		{
			//	Retourne la couleur de fond d'une ligne.
			if (this.columns == null)
			{
				return Color.Empty;
			}

			return this.columns[column].GetLineColor(row-this.firstVisibleRow);
		}

		public void SetDynamicToolTips(int column, bool state)
		{
			//	Spécifie si une colonne génère des tooltips dynamiques.
			this.columns[column].IsDynamicToolTips = state;
		}

		public bool GetDynamicToolTips(int column)
		{
			//	Retourne si une colonne génère des tooltips dynamiques.
			return this.columns[column].IsDynamicToolTips;
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
					this.isDirtyScroller = true;
					this.scroller.Invalidate();
				}
			}
		}

		public bool AllowMultipleSelection
		{
			//	Indique si les sélections multiples de lignes sont possibles.
			//	En mode 'true' la sélection multiple est forcée, c'est-à-dire qu'il
			//	n'est pas nécessaire d'utiliser la touche Ctrl.
			get
			{
				return this.allowMultipleSelection;
			}

			set
			{
				if (this.allowMultipleSelection != value)
				{
					this.allowMultipleSelection = value;

					if (this.allowMultipleSelection)
					{
						this.selectedRows = new List<int>();
					}
					else
					{
						this.selectedRows = null;
					}
					
					for (int i=0; i<this.columns.Length; i++)
					{
						this.columns[i].AllowMultipleSelection = value;
						this.columns[i].SelectedCells = this.selectedRows;
					}
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
				this.AllowMultipleSelection = false;
				this.SetSelectedRow(value, -1);
			}
		}

		public List<int> SelectedRows
		{
			//	Lignes sélectionnées. L'ordre obtenu dépend de l'ordre dans lequel
			//	l'utilisateur a cliqué sur les lignes.
			get
			{
				return this.selectedRows;
			}

			set
			{
				this.AllowMultipleSelection = true;
				this.selectedRows = value;
				this.isDirtySelected = true;
				this.Invalidate();
			}
		}

		public void SetSelectedRow(int row, int column)
		{
			//	Sélectionne une ligne, en indiquant également la colonne cliquée, pour SelectedColumn.
			this.selectedColumn = column;

			row = System.Math.Max(row, -1);
			row = System.Math.Min(row, this.TotalRows-1);

			if (this.selectedRow != row)
			{
				this.selectedRow = row;
				this.isDirtySelected = true;
				this.Invalidate();
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
					this.isDirtyScroller = true;
					this.isDirtySelected = true;
					this.OnUpdateCellContent();
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
			//	Met à jour la ligne sélectionnée, si nécessaire.
			if (this.isDirtySelected)
			{
				this.isDirtySelected = false;

				if (this.allowMultipleSelection)
				{
					List<int> sels = new List<int>();
					foreach (int sel in this.selectedRows)
					{
						sels.Add(sel-this.firstVisibleRow);
					}

					for (int i=0; i<this.columns.Length; i++)
					{
						this.columns[i].SelectedCells = sels;
					}
				}
				else
				{
					for (int i=0; i<this.columns.Length; i++)
					{
						this.columns[i].SelectedCell = this.selectedRow-this.firstVisibleRow;
					}
				}
			}
		}

		protected void UpdateScroller()
		{
			//	Met à jour l'ascenseur, si nécessaire.
			int count = this.LineCount;
			if (this.lastLineCount != count)
			{
				this.lastLineCount = count;
				this.isDirtyScroller = true;
			}

			if (this.isDirtyScroller)
			{
				this.isDirtyScroller = false;
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
		}


		internal bool ProcessListMessage(StringList list, Message message, Point pos)
		{
			//	Avant que StringList ne traite ses messages, il nous appelle afin
			//	que nous ayons une chance de détecter le drag sur les colonnes de
			//	séparation... avant que StringList ne nous mange l'événement sous
			//	notre nez.
			this.ProcessMessage(message, list.MapClientToParent(pos));
			
			if (message.Consumer == this)
			{
				return true;
			}
			
			return false;
		}
		
		protected override void ProcessMessage(Message message, Point pos)
		{
			if (this.IsEnabled == false)
			{
				return;
			}

			if (message.MessageType == MessageType.MouseWheel)
			{
				if (message.Wheel > 0)  this.FirstVisibleRow -= 3;
				if (message.Wheel < 0)  this.FirstVisibleRow += 3;
			}

			if (message.MessageType == MessageType.MouseDown)
			{
				this.WidthDraggingBegin(pos);
				if (this.widthDraggingRank != -1)
				{
					message.Captured = true;
					message.Consumer = this;
					return;
				}
			}

			if (message.MessageType == MessageType.MouseMove)
			{
				if (this.widthDraggingRank == -1)
				{
					if (this.WidthDraggingDetect(pos) != -1)
					{
						this.MouseCursor = MouseCursor.AsVSplit;
						message.Consumer = this;
						return;
					}
					else
					{
						this.MouseCursor = MouseCursor.AsArrow;
					}
				}
				else
				{
					this.WidthDraggingMove(pos);
					message.Captured = true;
					message.Consumer = this;
					return;
				}
			}

			if (message.MessageType == MessageType.MouseUp)
			{
				if (this.widthDraggingRank != -1)
				{
					this.WidthDraggingEnd(pos);
					message.Captured = true;
					message.Consumer = this;
					return;
				}
			}

			if (message.MessageType == MessageType.KeyDown)
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
			if (this.widthDraggingRank == -1)
			{
				return false;
			}

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
				if (pos.X > x-StringList.WidthDraggingDetectMargin && pos.X < x+StringList.WidthDraggingDetectMargin)
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

			if (this.columns == null)
			{
				return;
			}

			if (this.absoluteColumn != -1 && this.columns.Length > 1)  // une position absolue à imposer ?
			{
				//	Cherche toutes les largeurs absolues actuelles.
				double[] aw = new double[this.columns.Length];
				for (int c=0; c<this.columns.Length; c++)
				{
					aw[c] = this.GetColumnsAbsoluteWidth(c);
				}

				//	Calcule toutes les largeurs absolues futures.
				double delta = (aw[this.absoluteColumn]-this.absoluteWidth) / (this.columns.Length-1);
				double total = 0;
				for (int c=0; c<this.columns.Length; c++)
				{
					if (c == this.absoluteColumn)
					{
						aw[c] = this.absoluteWidth;  // met la largeur imposée
					}
					else
					{
						aw[c] += delta;  // modifie la largeur actuelle proportionnellement
					}

					total += aw[c];
				}

				//	Modifie toute les largeurs relatives, en fonction des largeurs absolues souhaitées.
				for (int c=0; c<this.columns.Length; c++)
				{
					this.SetColumnsRelativeWidth(c, aw[c]/total);
				}

				this.absoluteColumn = -1;  // c'est fait
			}

			Rectangle rect = this.Client.Bounds;

			for (int i=0; i<this.columns.Length; i++)
			{
				if (i < this.columns.Length-1)
				{
					rect.Width = this.GetColumnsAbsoluteWidth(i) + 1;
				}
				else
				{
					rect.Right = this.Client.Bounds.Right-this.scroller.PreferredWidth;
				}

				this.columns[i].SetManualBounds(rect);

				rect.Left = rect.Right-1;
			}

			rect = this.Client.Bounds;
			rect.Left = rect.Right-this.scroller.PreferredWidth;
			this.scroller.SetManualBounds(rect);

			this.OnColumnsWidthChanged();
			this.OnUpdateCellContent ();
			this.isDirtyGeometry = false;
		}


		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (this.isDirtyGeometry)
			{
				this.UpdateClientGeometry();
			}

			this.UpdateScroller();
			this.UpdateSelectedRow();

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
		private void HandleCellCountChanged(object sender)
		{
			this.OnCellCountChanged();
		}

		private void HandleDraggingCellSelectionChanged(object sender)
		{
			StringList array = sender as StringList;
			int sel = array.SelectedCell;

			for (int i=0; i<this.columns.Length; i++)
			{
				this.columns[i].SelectedCell = sel;
			}
		}

		private void HandleFinalCellSelectionChanged(object sender)
		{
			StringList array = sender as StringList;

			if (this.allowMultipleSelection)
			{
				List<int> sels = array.SelectedCells;

				this.selectedRows.Clear();
				foreach (int sel in sels)
				{
					int row = this.firstVisibleRow+sel;
					if (row < this.TotalRows)
					{
						this.selectedRows.Add(row);
					}
				}

				for (int i=0; i<this.columns.Length; i++)
				{
					this.columns[i].UpdateSelectedCell();
				}

				this.OnSelectedRowChanged();
			}
			else
			{
				int sel = array.SelectedCell;
				int row = this.firstVisibleRow+sel;
				if (row < this.TotalRows)
				{
					int column = -1;

					for (int i=0; i<this.columns.Length; i++)
					{
						this.columns[i].SelectedCell = sel;

						if (this.columns[i] == sender)
						{
							column = i;
						}
					}

					this.SetSelectedRow (this.firstVisibleRow+sel, column);
				}
				else
				{
					this.selectedRow = -2;  // pour forcer la mise à jour !
					this.SetSelectedRow (-1, -1);
				}
			}
		}

		private void HandleDoubleClicked(object sender, MessageEventArgs e)
		{
			this.OnSelectedRowDoubleClicked();
		}

		private void HandleScrollerValueChanged(object sender)
		{
			this.FirstVisibleRow = (int) System.Math.Floor(this.scroller.Value+0.5M);
		}
		#endregion


		#region Events handler
		protected virtual void OnColumnsWidthChanged()
		{
			//	Génère un événement pour dire que la largeur de colonnes a changé.
			var handler = this.GetUserEventHandler("ColumnsWidthChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler ColumnsWidthChanged
		{
			add
			{
				this.AddUserEventHandler("ColumnsWidthChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ColumnsWidthChanged", value);
			}
		}


		protected virtual void OnCellCountChanged()
		{
			//	Génère un événement pour dire que le nombre de cellules a changé.
			var handler = this.GetUserEventHandler("CellCountChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler CellCountChanged
		{
			add
			{
				this.AddUserEventHandler("CellCountChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("CellCountChanged", value);
			}
		}


		protected virtual void OnUpdateCellContent()
		{
			//	Génère un événement pour dire que le contenu des cellules a changé.
			var handler = this.GetUserEventHandler ("UpdateCellContent");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler UpdateCellContent
		{
			add
			{
				this.AddUserEventHandler ("UpdateCellContent", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("UpdateCellContent", value);
			}
		}


		protected virtual void OnSelectedRowChanged()
		{
			//	Génère un événement pour dire que la ligne sélectionnée a changé.
			var handler = this.GetUserEventHandler("SelectedRowChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler SelectedRowChanged
		{
			add
			{
				this.AddUserEventHandler("SelectedRowChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("SelectedRowChanged", value);
			}
		}


		protected virtual void OnSelectedRowDoubleClicked()
		{
			//	Génère un événement pour dire que la ligne sélectionnée a été double cliquée.
			var handler = this.GetUserEventHandler("SelectedRowDoubleClicked");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler SelectedRowDoubleClicked
		{
			add
			{
				this.AddUserEventHandler("SelectedRowDoubleClicked", value);
			}
			remove
			{
				this.RemoveUserEventHandler("SelectedRowDoubleClicked", value);
			}
		}
		#endregion


		protected StringList[]				columns;
		protected VScroller					scroller;
		protected int						totalRows;
		protected bool						allowMultipleSelection = false;
		protected int						selectedRow = -1;
		protected int						selectedColumn = -1;
		protected List<int>					selectedRows;
		protected int						firstVisibleRow = 0;
		protected bool						ignoreChange = false;
		protected bool						isDirtyScroller = true;
		protected bool						isDirtySelected = true;
		protected bool						isDirtyGeometry = true;
		protected int						lastLineCount = 0;
		protected int						absoluteColumn = -1;
		protected double					absoluteWidth;

		protected int						widthDraggingRank = -1;
		protected double[]					widthDraggingAbsolutes;
		protected double					widthDraggingMin;
		protected double					widthDraggingMax;
		protected double					widthDraggingPos;
	}
}
