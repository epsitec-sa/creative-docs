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

		public string GetListName()
		{
			//	Détermine le nom de la propriété dans la liste (Lister).
			return this.stringValue;
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
			PropertyName p = property as PropertyName;
			p.String = this.stringValue;
		}

		public override bool Compare(AbstractProperty property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			PropertyName p = property as PropertyName;
			if ( p.String != this.stringValue )  return false;

			return true;
		}

		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			return new PanelName(drawer);
		}

		protected string			stringValue = "";
	}
}
