using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyString repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	public class PropertyString : AbstractProperty
	{
		public PropertyString()
		{
		}

		[XmlAttribute]
		public string String
		{
			get { return this.stringValue; }
			set { this.stringValue = value; }
		}

		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		[XmlIgnore]
		public override bool AlterBoundingBox
		{
			get { return ( this.type == PropertyType.TextString ); }
		}

		// Effectue une copie de la propri�t�.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyString p = property as PropertyString;
			p.String = this.stringValue;
		}

		// Compare deux propri�t�s.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyString p = property as PropertyString;
			if ( p.String != this.stringValue )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			return new PanelString(drawer);
		}

		protected string			stringValue = "";
	}
}
