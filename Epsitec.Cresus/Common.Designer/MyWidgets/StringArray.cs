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
				}
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if ( this.stringList == null )  return;

			Rectangle rect = this.Client.Bounds;
			rect.Height = this.lineHeight;
			rect.Offset(0, this.lineHeight*(this.stringList.Length-1));

			for ( int i=0 ; i<this.stringList.Length ; i++ )
			{
				if ( this.stringList[i].Text != null )
				{
					this.stringList[i].LayoutSize = rect.Size;
					this.stringList[i].Paint(rect.BottomLeft, graphics);
				}

				if ( i == 0 )
				{
					graphics.AddLine(rect.Left, rect.Top+0.5, rect.Right, rect.Top+0.5);
				}

				graphics.AddLine(rect.Left+0.5, rect.Bottom, rect.Left+0.5, rect.Top);
				graphics.AddLine(rect.Right-0.5, rect.Bottom, rect.Right-0.5, rect.Top);
				graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);

				graphics.RenderSolid(Color.FromBrightness(0));

				rect.Offset(0, -this.lineHeight);
			}
		}


		protected double					lineHeight = 20;
		protected TextLayout[]				stringList;
	}
}
