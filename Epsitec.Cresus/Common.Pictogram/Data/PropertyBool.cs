using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyBool repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	public class PropertyBool : AbstractProperty
	{
		public PropertyBool()
		{
		}

		// Valeur de la propri�t�.
		[XmlAttribute]
		public bool Bool
		{
			get { return this.boolValue; }
			set { this.boolValue = value; }
		}

		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		[XmlIgnore]
		public override bool AlterBoundingBox
		{
			get { return ( this.type == PropertyType.PolyClose ); }
		}

		// Indique si cette propri�t� peut faire l'objet d'un style.
		[XmlIgnore]
		public override bool StyleAbility
		{
			get { return false; }
		}

		// Effectue une copie de la propri�t�.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyBool p = property as PropertyBool;
			p.Bool = this.boolValue;
		}

		// Compare deux propri�t�s.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyBool p = property as PropertyBool;
			if ( p.Bool != this.boolValue )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override AbstractPanel CreatePanel()
		{
			return new PanelBool();
		}

		protected bool			boolValue = false;
	}
}
