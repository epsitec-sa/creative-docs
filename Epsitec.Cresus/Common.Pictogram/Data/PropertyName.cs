using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyName repr�sente une propri�t� d'un objet graphique.
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

		// D�termine le nom de la propri�t� dans la liste (Lister).
		public string GetListName()
		{
			return this.stringValue;
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
			PropertyName p = property as PropertyName;
			p.String = this.stringValue;
		}

		// Compare deux propri�t�s.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyName p = property as PropertyName;
			if ( p.String != this.stringValue )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override AbstractPanel CreatePanel()
		{
			return new PanelName();
		}

		protected string			stringValue = "";
	}
}
