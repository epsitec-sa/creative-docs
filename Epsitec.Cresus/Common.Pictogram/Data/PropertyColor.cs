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

		// Couleur de la propri�t�.
		public Drawing.Color Color
		{
			get { return this.color; }
			set { this.color = value; }
		}

		// Effectue une copie de la propri�t�.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyColor p = property as PropertyColor;
			p.Color = this.color;
		}

		// Compare deux propri�t�s.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyColor p = property as PropertyColor;
			if ( p.Color != this.color )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			return new PanelColor(drawer);
		}

		[XmlAttribute]
		protected Drawing.Color			color = Drawing.Color.FromBrightness(0);
	}
}
