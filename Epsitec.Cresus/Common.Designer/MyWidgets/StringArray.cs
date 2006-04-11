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
				if ( this.stringList == null )  return 0;
				return this.stringList.Length;
			}
		}

		public void SetLineString(int index, string text)
		{
			//	Spécifie le texte contenu dans une ligne.
			if ( this.stringList == null )  return;
			if ( index < 0 || index >= this.stringList.Length )  return;
			this.stringList[index].Text = text;
		}

		public string GetLineString(int index)
		{
			//	Retourne le texte contenu dans une ligne.
			if (this.stringList == null)
				return null;
			if ( index < 0 || index >= this.stringList.Length )  return null;
			return this.stringList[index].Text;
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			int length = (int) (this.Client.Bounds.Height/this.lineHeight);
			length = System.Math.Max(length, 1);
			if ( this.stringList == null || this.stringList.Length != length )
			{
				this.stringList = new TextLayout[length];
				for ( int i=0 ; i<this.stringList.Length ; i++ )
				{
					this.stringList[i] = new TextLayout();
					this.stringList[i].Alignment = ContentAlignment.MiddleLeft;
				}

				this.OnSizeChanged();
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if ( this.stringList == null )  return;

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;
			double h = rect.Height/this.stringList.Length;
			rect.Bottom = rect.Top-h;

			for ( int i=0 ; i<this.stringList.Length ; i++ )
			{
				if ( this.stringList[i].Text != null )
				{
					this.stringList[i].LayoutSize = new Size(rect.Width-5, rect.Height);
					this.stringList[i].Paint(new Point(rect.Left+5, rect.Bottom), graphics);
				}

				if ( i < this.stringList.Length-1 )
				{
					Point p1 = new Point(rect.Left, rect.Bottom);
					Point p2 = new Point(rect.Right, rect.Bottom);
					graphics.Align(ref p1);
					graphics.Align(ref p2);
					p1.Y += 0.5;
					p2.Y += 0.5;
					graphics.AddLine(p1, p2);
				}

				rect.Offset(0, -h);
			}

			rect = this.Client.Bounds;
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(adorner.ColorBorder);
		}


		protected virtual void OnSizeChanged()
		{
			//	Génère un événement pour dire que la taille a changé.
			if ( this.SizeChanged != null )  // qq'un écoute ?
			{
				this.SizeChanged(this);
			}
		}

		public event Support.EventHandler SizeChanged;

		
		protected double					lineHeight = 20;
		protected TextLayout[]				stringList;
	}
}
