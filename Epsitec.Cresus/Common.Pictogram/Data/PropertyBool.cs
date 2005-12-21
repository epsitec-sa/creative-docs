using Epsitec.Common.Widgets;
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

		[XmlAttribute]
		public bool Bool
		{
			//	Valeur de la propriété.
			get { return this.boolValue; }
			set { this.boolValue = value; }
		}

		[XmlIgnore]
		public override bool AlterBoundingBox
		{
			//	Indique si un changement de cette propriété modifie la bbox de l'objet.
			get { return ( this.type == PropertyType.PolyClose ); }
		}

		[XmlIgnore]
		public override bool StyleAbility
		{
			//	Indique si cette propriété peut faire l'objet d'un style.
			get { return false; }
		}

		public override void CopyTo(AbstractProperty property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			PropertyBool p = property as PropertyBool;
			p.Bool = this.boolValue;
		}

		public override bool Compare(AbstractProperty property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			PropertyBool p = property as PropertyBool;
			if ( p.Bool != this.boolValue )  return false;

			return true;
		}

		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			return new PanelBool(drawer);
		}

		protected bool			boolValue = false;
	}
}
