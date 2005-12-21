using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyColor représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyColor : AbstractProperty
	{
		public PropertyColor()
		{
			this.color = Drawing.Color.FromBrightness(0.0);
		}

		public Drawing.Color Color
		{
			//	Couleur de la propriété.
			get { return this.color; }
			set { this.color = value; }
		}

		public override void CopyTo(AbstractProperty property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			PropertyColor p = property as PropertyColor;
			p.Color = this.color;
		}

		public override bool Compare(AbstractProperty property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			PropertyColor p = property as PropertyColor;
			if ( p.Color != this.color )  return false;

			return true;
		}

		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			return new PanelColor(drawer);
		}


		public bool PaintColor(Printing.PrintPort port, IconContext iconContext)
		{
			//	Définition de la couleur pour l'impression.
			if ( !this.color.IsOpaque )  return false;

			port.Color = this.color;
			return true;
		}


		[XmlAttribute]
		protected Drawing.Color			color = Drawing.Color.FromBrightness(0);
	}
}
