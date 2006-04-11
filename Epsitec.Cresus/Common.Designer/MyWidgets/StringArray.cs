using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Tableau d'une colonne de TextLayout.
	/// </summary>
	public class StringArray : Widget
	{
		public enum CellState
		{
			Normal,
			Warning,
		}


		public StringArray() : base()
		{
		}

		public StringArray(Widget embedder) : this()
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
			this.cells[index].TextLayout.Text = text;
			this.Invalidate();
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
			this.cells[index].State = state;
			this.Invalidate();
		}

		public CellState GetLineState(int index)
		{
			//	Retourne l'état d'une ligne.
			if ( this.cells == null )  return CellState.Normal;
			if ( index < 0 || index >= this.cells.Length )  return CellState.Normal;
			return this.cells[index].State;
		}

		public void SetLineSelected(int index, bool selected)
		{
			//	Spécifie l'état d'une ligne.
			if ( this.cells == null )  return;
			if ( index < 0 || index >= this.cells.Length )  return;
			this.cells[index].Selected = selected;
			this.Invalidate();
		}

		public bool GetLineSelected(int index)
		{
			//	Retourne l'état d'une ligne.
			if ( this.cells == null )  return false;
			if ( index < 0 || index >= this.cells.Length )  return false;
			return this.cells[index].Selected;
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
			double h = rect.Height/this.cells.Length;
			rect.Bottom = rect.Top-h;

			for ( int i=0 ; i<this.cells.Length ; i++ )
			{
				Color backColor = adorner.ColorTextBackground;
				if ( this.cells[i].Selected )
				{
					backColor = adorner.ColorCaption;
				}
				else if ( this.cells[i].State == CellState.Warning )
				{
					backColor = Color.FromRgb(1, 0.5, 0.5);
				}

				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(backColor);

				if ( this.cells[i].TextLayout.Text != null )
				{
					this.cells[i].TextLayout.LayoutSize = new Size(rect.Width-5, rect.Height);
					this.cells[i].TextLayout.Paint(new Point(rect.Left+5, rect.Bottom), graphics);
				}

				if ( i < this.cells.Length-1 )
				{
					Point p1 = new Point(rect.Left, rect.Bottom);
					Point p2 = new Point(rect.Right, rect.Bottom);
					graphics.Align(ref p1);
					graphics.Align(ref p2);
					p1.Y += 0.5;
					p2.Y += 0.5;
					graphics.AddLine(p1, p2);
					graphics.RenderSolid(adorner.ColorBorder);
				}

				rect.Offset(0, -h);
			}

			rect = this.Client.Bounds;
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(adorner.ColorBorder);
		}


		protected virtual void OnCellsQuantityChanged()
		{
			//	Génère un événement pour dire que la taille a changé.
			if (this.CellsQuantityChanged != null)  // qq'un écoute ?
			{
				this.CellsQuantityChanged(this);
			}
		}

		public event Support.EventHandler CellsQuantityChanged;


		#region Cell class
		protected class Cell
		{
			public TextLayout				TextLayout;
			public CellState				State;
			public bool						Selected;
		}
		#endregion


		protected double					lineHeight = 20;
		protected Cell[]					cells;
	}
}
