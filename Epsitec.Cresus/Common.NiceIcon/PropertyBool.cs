using Epsitec.Common.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.NiceIcon
{
	/// <summary>
	/// La classe PropertyBool représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyBool : AbstractProperty
	{
		public PropertyBool()
		{
		}

		// Type de la propriété.
		[XmlAttribute]
		public bool Bool
		{
			get
			{
				return this.boolValue;
			}

			set
			{
				this.boolValue = value;
			}
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
