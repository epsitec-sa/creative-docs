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
		}


		public StringList() : base()
		{
			this.AutoEngage = false;
			this.AutoFocus  = true;

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
			//	Largeur relative (dans n'importe quelle unité) de la colonne.
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

		public int LineCount
		{
			//	Nombre total de ligne en fonction de la hauteur du widget et de la hauteur d'une ligne.
			get
			{
				if ( this.cells == null )  return 0;
				return this.cells.Length;
			}
		}

		public void SetLineString(int index, string text)
		{
			//	Spécifie le texte contenu dans une ligne.
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
			//	Spécifie l'état d'une ligne.
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
			//	Retourne l'état d'une ligne.
			if ( this.cells == null )  return CellState.Normal;
			if ( index < 0 || index >= this.cells.Length )  return CellState.Normal;
			return this.cells[index].State;
		}

		public int CellSelected
		{
			//	Cellule sélectionnée.
			get
			{
				for (int i=0; i<this.cells.Length; i++)
				{
					if ( this.cells[i].Selected )  return i;
				}
				return -1;
			}

			set
			{
				for (int i=0; i<this.cells.Length; i++)
				{
					bool selected = (i == value);
					if (this.cells[i].Selected != selected)
					{
						this.cells[i].Selected = selected;
						this.Invalidate();
					}
				}
			}
		}

		public bool IsDynamicsToolTips
		{
			//	Faut-il générer les tooltips dynamiques ?
			get
			{
				return this.isDynamicsToolTips;
			}

			set
			{
				this.isDynamicsToolTips = value;
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
				if (message.Type == MessageType.MouseDown)
				{
					int cell = this.Detect(pos, true);
					if (this.CellSelected != cell)
					{
						if (cell != -1)
						{
							this.CellSelected = cell;
							this.OnDraggingCellSelectionChanged();
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

				if (message.Type == MessageType.MouseMove)
				{
					if (this.isDragging)
					{
						int cell = this.Detect(pos, false);
						if (this.CellSelected != cell)
						{
							this.CellSelected = cell;
							this.OnDraggingCellSelectionChanged();
						}
						message.Captured = true;
						message.Consumer = this;
						return;
					}
				}

				if (message.Type == MessageType.MouseUp)
				{
					int cell = this.Detect(pos, false);
					this.CellSelected = cell;
					this.OnFinalCellSelectionChanged();
					this.isDragging = false;
					message.Captured = true;
					message.Consumer = this;
					return;
				}

				if (message.Type == MessageType.MouseLeave)
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
			//	Détecte la cellule visée par la souris.
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
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			int length = (int) (this.Client.Bounds.Height/this.lineHeight);
			length = System.Math.Max(length, 1);
			if ( this.cells == null || this.cells.Length != length )
			{
				this.cells = new Cell[length];
				for ( int i=0 ; i<this.cells.Length ; i++ )
				{
					this.cells[i] = new Cell();
					this.cells[i].TextLayout = new TextLayout();
					this.cells[i].TextLayout.Alignment = ContentAlignment.MiddleLeft;
					this.cells[i].State = CellState.Normal;
					this.cells[i].Selected = false;
				}

				this.OnCellsQuantityChanged();
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

				WidgetState state = this.PaintState;
				if (this.cells[i].Selected)
				{
					state |= WidgetState.Selected;
				}
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
					graphics.RenderSolid(adorner.ColorText(WidgetState.None));
				}

				if (this.cells[i].TextLayout.Text != null)
				{
					state = WidgetState.Enabled;
					if (this.cells[i].Selected)
					{
						state |= WidgetState.Selected;
					}
					Color color = adorner.ColorText(state);

					this.cells[i].TextLayout.LayoutSize = new Size(rect.Width-5, rect.Height);
					this.cells[i].TextLayout.Paint(new Point(rect.Left+5, rect.Bottom), graphics, Rectangle.MaxValue, color, GlyphPaintStyle.Normal);
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
			if ( !this.isDynamicsToolTips )  return null;
			return this.GetTooltipEditedText(pos);
		}

		protected string GetTooltipEditedText(Point pos)
		{
			//	Donne le texte du tooltip en fonction de la position.
			int row = this.Detect(pos, false);
			if ( row != -1 )
			{
				return this.GetLineString(row);
			}

			return null;  // pas de tooltip
		}
		#endregion


		#region Events handler
		protected virtual void OnCellsQuantityChanged()
		{
			//	Génère un événement pour dire que le nombre de cellules a changé.
			if (this.CellsQuantityChanged != null)  // qq'un écoute ?
			{
				this.CellsQuantityChanged(this);
			}
		}

		public event Support.EventHandler CellsQuantityChanged;


		protected virtual void OnDraggingCellSelectionChanged()
		{
			//	Génère un événement pour dire qu'une cellule a été sélectionnée.
			if (this.DraggingCellSelectionChanged != null)  // qq'un écoute ?
			{
				this.DraggingCellSelectionChanged(this);
			}
		}

		public event Support.EventHandler DraggingCellSelectionChanged;


		protected virtual void OnFinalCellSelectionChanged()
		{
			//	Génère un événement pour dire qu'une cellule a été sélectionnée.
			if (this.FinalCellSelectionChanged != null)  // qq'un écoute ?
			{
				this.FinalCellSelectionChanged(this);
			}
		}

		public event Support.EventHandler FinalCellSelectionChanged;
		#endregion


		#region Cell class
		protected class Cell
		{
			public TextLayout				TextLayout;
			public CellState				State;
			public bool						Selected;
		}
		#endregion


		public static readonly double		WidthDraggingDetectMargin = 2;

		protected double					lineHeight = 20;
		protected double					relativeWidth = 0;
		protected Cell[]					cells;
		protected bool						isDynamicsToolTips = false;
		protected bool						isDragging = false;
	}
}
