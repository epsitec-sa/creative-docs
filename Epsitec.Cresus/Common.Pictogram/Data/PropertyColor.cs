using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyColor repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	public class PropertyColor : AbstractProperty
	{
		public PropertyColor()
		{
			this.color = Drawing.Color.FromBrightness(0.0);
		}

		public Drawing.Color Color
		{
			//	Couleur de la propri�t�.
			get { return this.color; }
			set { this.color = value; }
		}

		public override void CopyTo(AbstractProperty property)
		{
			//	Effectue une copie de la propri�t�.
			base.CopyTo(property);
			PropertyColor p = property as PropertyColor;
			p.Color = this.color;
		}

		public override bool Compare(AbstractProperty property)
		{
			//	Compare deux propri�t�s.
			if ( !base.Compare(property) )  return false;

			PropertyColor p = property as PropertyColor;
			if ( p.Color != this.color )  return false;

			return true;
		}

		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			//	Cr�e le panneau permettant d'�diter la propri�t�.
			return new PanelColor(drawer);
		}


		public bool PaintColor(Printing.PrintPort port, IconContext iconContext)
		{
			//	D�finition de la couleur pour l'impression.
			if ( !this.color.IsOpaque )  return false;

			port.Color = this.color;
			return true;
		}


		[XmlAttribute]
		protected Drawing.Color			color = Drawing.Color.FromBrightness(0);
	}
}
