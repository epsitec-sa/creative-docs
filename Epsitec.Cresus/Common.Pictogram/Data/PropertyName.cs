using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyName représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyName : AbstractProperty
	{
		public PropertyName()
		{
		}

		[XmlAttribute]
		public string String
		{
			get { return this.stringValue; }
			set { this.stringValue = value; }
		}

		// Détermine le nom de la propriété dans la liste (Lister).
		public string GetListName()
		{
			return this.stringValue;
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
			PropertyName p = property as PropertyName;
			p.String = this.stringValue;
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyName p = property as PropertyName;
			if ( p.String != this.stringValue )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel()
		{
			return new PanelName();
		}

		protected string			stringValue = "";
	}
}
