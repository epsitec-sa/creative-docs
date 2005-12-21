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

		public override void CopyTo(AbstractProperty property)
		{
			//	Effectue une copie de la propri�t�.
			base.CopyTo(property);
			PropertyString p = property as PropertyString;
			p.String = this.stringValue;
		}

		public override bool Compare(AbstractProperty property)
		{
			//	Compare deux propri�t�s.
			if ( !base.Compare(property) )  return false;

			PropertyString p = property as PropertyString;
			if ( p.String != this.stringValue )  return false;

			return true;
		}

		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			//	Cr�e le panneau permettant d'�diter la propri�t�.
			return new PanelString(drawer);
		}

		protected string			stringValue = "";
	}
}
