using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Tableau d'une colonne de TextLayout (vus comme des string).
	/// </summary>
	public class StringList : Widget, Widgets.Helpers.IToolTipHost
	{
		public enum CellState
		{
			Normal,
			Warning,
			Modified,
			Disabled,
			Unused,
		}


		public StringList() : base()
		{
			this.AutoEngage = false;
			this.AutoFocus = true;
			this.AutoDoubleClick = true;

			this.InternalState |= InternalState.Focusable;
			this.InternalState |= InternalState.Engageable;
		}

		public StringList(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public double LineHeight
		{
			//	Hauteur d'une ligne.
			get
			{
				return this.lineHeight;
			}

			set
			{
				if ( this.lineHeight != value )
				{
					this.lineHeight = value;
					this.UpdateClientGeometry();
				}
			}
		}

		public double RelativeWidth
		{
			//	Largeur relative (dans n'importe quelle unit�) de la colonne.
			get
			{
				return this.relativeWidth;
			}

			set
			{
				if ( this.relativeWidth != value )
				{
					this.relativeWidth = value;
				}
			}
		}

		public ContentAlignment Alignment
		{
			//	Alignement des textes pour la colonne.
			get
			{
				return this.alignment;
			}

			set
			{
				this.alignment = value;
			}
		}

		public TextBreakMode BreakMode
		{
			//	C�sure des textes pour la colonne.
			get
			{
				return this.breakMode;
			}

			set
			{
				this.breakMode = value;
			}
		}

		public int LineCount
		{
			//	Nombre total de ligne en fonction de la hauteur du widget et de la hauteur d'une ligne.
			get
			{
				if (this.cells == null)
				{
					this.UpdateClientGeometry();
				}

				if (this.cells == null)
				{
					return 0;
				}

				return this.cells.Length;
			}
		}

		public void SetLineString(int index, string text)
		{
			//	Sp�cifie le texte contenu dans une ligne.
			if ( this.cells == null )  return;
			if ( index < 0 || index >= this.cells.Length )  return;

			if ( this.cells[index].TextLayout.Text != text )
			{
				this.cells[index].TextLayout.Text = text;
				this.Invalidate();
			}
		}

		public string GetLineString(int index)
		{
			//	Retourne le texte contenu dans une ligne.
			if ( this.cells == null )  return null;
			if ( index < 0 || index >= this.cells.Length )  return null;
			return this.cells[index].TextLayout.Text;
		}

		public void SetLineState(int index, CellState state)
		{
			//	Sp�cifie l'�tat d'une ligne.
			if ( this.cells == null )  return;
			if ( index < 0 || index >= this.cells.Length )  return;

			if ( this.cells[index].State != state )
			{
				this.cells[index].State = state;
				this.Invalidate();
			}
		}

		public CellState GetLineState(int index)
		{
			//	Retourne l'�tat d'une ligne.
			if ( this.cells == null )  return CellState.Normal;
			if ( index < 0 || index >= this.cells.Length )  return CellState.Normal;
			return this.cells[index].State;
		}

		public bool AllowMultipleSelection
		{
			//	Indique si les s�lections multiples sont possibles.
			//	En mode 'true' la s�lection multiple est forc�e, c'est-�-dire qu'il
			//	n'est pas n�cessaire d'utiliser la touche Ctrl.
			get
			{
				return this.allowMultipleSelection;
			}

			set
			{
				this.allowMultipleSelection = value;
			}
		}

		public int SelectedCell
		{
			//	Cellule s�lectionn�e.
			//	Attention, les cellules sont compt�s ici � partir de z�ro, contraitement �
			//	StringArray qui compte des lignes en tenant compte de la premi�re ligne
			//	visible.
			get
			{
				return this.selectedCell;
			}

			set
			{
				this.selectedCell = value;
				this.UpdateSelectedCell();
			}
		}

		public List<int> SelectedCells
		{
			//	Cellules s�lectionn�es.
			//	Attention, les cellules sont compt�s ici � partir de z�ro, contraitement �
			//	StringArray qui compte des lignes en tenant compte de la premi�re ligne
			//	visible.
			get
			{
				return this.selectedCells;
			}

			set
			{
				this.selectedCells = value;
				this.UpdateSelectedCell();
			}
		}

		public bool IsDynamicToolTips
		{
			//	Faut-il g�n�rer les tooltips dynamiques ?
			get
			{
				return this.isDynamicToolTips;
			}

			set
			{
				this.isDynamicToolTips = value;
			}
		}


		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			if (this.IsEnabled == false)
			{
				return;
			}

			if (message.IsMouseType)
			{
				StringArray array = this.Parent as StringArray;

				if (array != null)
				{
					if (array.ProcessListMessage (this, message, pos))
					{
						return;
					}
				}
				
				if (message.MessageType == MessageType.MouseDown)
				{
					int cell = this.Detect(pos, true);

					if (this.allowMultipleSelection)
					{
						if (this.selectedCells.Contains(cell))
						{
							this.selectedCells.Remove(cell);
						}
						else
						{
							this.selectedCells.Add(cell);
						}

						this.UpdateSelectedCell();
						this.OnFinalCellSelectionChanged();
					}
					else
					{
						if (this.SelectedCell != cell)
						{
							if (cell != -1)
							{
								this.SelectedCell = cell;
								this.OnDraggingCellSelectionChanged();
							}
						}
					}

					if (cell != -1)
					{
						this.isDragging = true;
						message.Captured = true;
						message.Consumer = this;
						return;
					}
				}

				if (message.MessageType == MessageType.MouseMove)
				{
					if (this.isDragging)
					{
						int cell = this.Detect(pos, false);
						if (!this.allowMultipleSelection && this.SelectedCell != cell)
						{
							this.SelectedCell = cell;
							this.OnDraggingCellSelectionChanged();
						}
						message.Captured = true;
						message.Consumer = this;
						return;
					}
				}

				if (message.MessageType == MessageType.MouseUp)
				{
					int cell = this.Detect(pos, false);

					if (!this.allowMultipleSelection)
					{
						this.SelectedCell = cell;
						this.OnFinalCellSelectionChanged();
					}

					this.isDragging = false;
					message.Captured = true;
					message.Consumer = this;
					return;
				}

				if (message.MessageType == MessageType.MouseLeave)
				{
					message.Captured = true;
					message.Consumer = this;
					return;
				}
			}

			base.ProcessMessage(message, pos);
		}

		protected int Detect(Point pos, bool margins)
		{
			//	D�tecte la cellule vis�e par la souris.
			Rectangle box = this.Client.Bounds;
			
			if (margins)
			{
				box.Left  += StringList.WidthDraggingDetectMargin;
				box.Right -= StringList.WidthDraggingDetectMargin;
			}

			if ( !box.Contains(pos) )  return -1;
			double py = box.Height-(pos.Y-box.Bottom);
			double h = box.Height/this.cells.Length;
			return (int) (py/h);
		}

		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			int length = (int) (this.Client.Bounds.Height/this.lineHeight);
			length = System.Math.Max(length, 1);
			if ( this.cells == null || this.cells.Length != length )
			{
				this.CreateCells(length);
				this.OnCellCountChanged();
			}
		}

		private void CreateCells(int length)
		{
			this.cells = new Cell[length];
			for (int i=0; i<this.cells.Length; i++)
			{
				this.cells[i] = new Cell ();
				this.cells[i].TextLayout = new TextLayout ();
				this.cells[i].TextLayout.Alignment = this.alignment;
				this.cells[i].TextLayout.BreakMode = this.breakMode;
				this.cells[i].State = CellState.Normal;
				this.cells[i].Selected = this.IsSelectedCell(i);
			}
		}

		public void UpdateSelectedCell()
		{
			if (this.cells != null)
			{
				for (int i=0; i<this.cells.Length; i++)
				{
					bool selected = this.IsSelectedCell(i);
					if (this.cells[i].Selected != selected)
					{
						this.cells[i].Selected = selected;
						this.Invalidate();
					}
				}
			}
		}

		protected bool IsSelectedCell(int cell)
		{
			//	Indique si une cellule doit �tre s�lectionn�e.
			if (this.allowMultipleSelection)
			{
				return this.selectedCells != null && this.selectedCells.Contains(cell);
			}
			else
			{
				return cell == this.selectedCell;
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if ( this.cells == null )  return;

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(adorner.ColorTextBackground);

			double h = rect.Height/this.cells.Length;
			rect.Bottom = rect.Top-h;

			for (int i=0; i<this.cells.Length; i++)
			{
				Rectangle cell = rect;
				graphics.Align(ref cell);

				WidgetPaintState state = this.PaintState;
				if (this.cells[i].Selected)
				{
					state |= WidgetPaintState.Selected;
				}
				state &= ~WidgetPaintState.Entered;
				adorner.PaintCellBackground(graphics, cell, state);

				if (this.cells[i].State == CellState.Warning)
				{
					graphics.AddFilledRectangle(cell);
					graphics.RenderSolid(Color.FromAlphaRgb(0.4, 1, 0, 0));  // rouge semi-transparent
				}

				if (this.cells[i].State == CellState.Modified)
				{
					graphics.AddFilledRectangle(cell);
					graphics.RenderSolid(Color.FromAlphaRgb(0.5, 1, 0.8, 0));  // jaune semi-transparent
				}

				if (this.cells[i].State == CellState.Disabled)
				{
					graphics.AddFilledRectangle(cell);
					graphics.RenderSolid(adorner.ColorText(WidgetPaintState.None));
				}

				if (this.cells[i].State == CellState.Unused)
				{
					graphics.AddFilledRectangle(cell);
					graphics.RenderSolid(Color.FromAlphaRgb(0.1, 0, 0, 0));  // gris tr�s clair
				}

				if (this.cells[i].TextLayout.Text != null)
				{
					state = WidgetPaintState.Enabled;
					if (this.cells[i].Selected)
					{
						state |= WidgetPaintState.Selected;
					}
					Color color = adorner.ColorText(state);

					double leftMargin = 0;
					if (this.alignment == ContentAlignment.MiddleLeft || this.alignment == ContentAlignment.TopLeft || this.alignment == ContentAlignment.BottomLeft)
					{
						leftMargin = rect.Left+5;
					}

					this.cells[i].TextLayout.LayoutSize = new Size(rect.Width-5, rect.Height);
					this.cells[i].TextLayout.Paint(new Point(leftMargin, rect.Bottom), graphics, Rectangle.MaxValue, color, GlyphPaintStyle.Normal);
				}

				rect.Offset(0, -h);
			}

			rect = this.Client.Bounds;
			rect.Bottom = rect.Top-h;

			for (int i=0; i<this.cells.Length-1; i++)
			{
				Point p1 = new Point(rect.Left, rect.Bottom);
				Point p2 = new Point(rect.Right, rect.Bottom);
				graphics.Align(ref p1);
				graphics.Align(ref p2);
				p1.Y += 0.5;
				p2.Y += 0.5;
				graphics.AddLine(p1, p2);

				rect.Offset(0, -h);
			}

			rect = this.Client.Bounds;
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(adorner.ColorBorder);
		}


		#region Helpers.IToolTipHost
		public object GetToolTipCaption(Point pos)
		{
			//	Donne l'objet (string ou widget) pour le tooltip en fonction de la position.
			if ( !this.isDynamicToolTips )  return null;
			return this.GetTooltipEditedText(pos);
		}

		protected string GetTooltipEditedText(Point pos)
		{
			//	Donne le texte du tooltip en fonction de la position.
			int row = this.Detect(pos, false);
			if ( row != -1 )
			{
				string text = this.GetLineString(row);
				if (text != "")
				{
					return text;
				}
			}

			return null;  // pas de tooltip
		}
		#endregion


		#region Events handler
		protected virtual void OnCellCountChanged()
		{
			//	G�n�re un �v�nement pour dire que le nombre de cellules a chang�.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("CellCountChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler CellCountChanged
		{
			add
			{
				this.AddUserEventHandler ("CellCountChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("CellCountChanged", value);
			}
		}


		protected virtual void OnDraggingCellSelectionChanged()
		{
			//	G�n�re un �v�nement pour dire qu'une cellule a �t� s�lectionn�e.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("DraggingCellSelectionChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler DraggingCellSelectionChanged
		{
			add
			{
				this.AddUserEventHandler("DraggingCellSelectionChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("DraggingCellSelectionChanged", value);
			}
		}


		protected virtual void OnFinalCellSelectionChanged()
		{
			//	G�n�re un �v�nement pour dire qu'une cellule a �t� s�lectionn�e.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("FinalCellSelectionChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler FinalCellSelectionChanged
		{
			add
			{
				this.AddUserEventHandler("FinalCellSelectionChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("FinalCellSelectionChanged", value);
			}
		}
		#endregion


		#region Cell class
		protected class Cell
		{
			public TextLayout				TextLayout;
			public CellState				State;
			public bool						Selected;
		}
		#endregion


		public static readonly double		WidthDraggingDetectMargin = 3;

		protected double					lineHeight = 20;
		protected double					relativeWidth = 0;
		protected ContentAlignment			alignment = ContentAlignment.MiddleLeft;
		protected TextBreakMode				breakMode = TextBreakMode.None;
		protected Cell[]					cells;
		protected bool						isDynamicToolTips = false;
		protected bool						isDragging = false;
		protected bool						allowMultipleSelection = false;
		protected int						selectedCell = -1;
		protected List<int>					selectedCells;
	}
}
