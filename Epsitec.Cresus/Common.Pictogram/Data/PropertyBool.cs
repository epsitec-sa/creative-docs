using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyBool représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyBool : AbstractProperty
	{
		public PropertyBool()
		{
		}

		// Valeur de la propriété.
		[XmlAttribute]
		public bool Bool
		{
			get { return this.boolValue; }
			set { this.boolValue = value; }
		}

		// Indique si un changement de cette propriété modifie la bbox de l'objet.
		[XmlIgnore]
		public override bool AlterBoundingBox
		{
			get { return ( this.type == PropertyType.PolyClose ); }
		}

		// Indique si cette propriété peut faire l'objet d'un style.
		[XmlIgnore]
		public override bool StyleAbility
		{
			get { return false; }
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyBool p = property as PropertyBool;
			p.Bool = this.boolValue;
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyBool p = property as PropertyBool;
			if ( p.Bool != this.boolValue )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel()
		{
			return new PanelBool();
		}

		protected bool			boolValue = false;
	}
}
