using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	public enum PropertyType
	{
		None,				// aucune
		LineColor,			// couleur du trait
		LineMode,			// mode du trait
		FillGradient,		// d�grad� de remplissage
		Shadow,				// ombre sous l'objet
		PolyClose,			// mode de fermeture des polygones
		RoundRect,			// rayons des rectangles arrondis
		RegularFaces,		// polygone r�gulier: nb de faces
		RegularStar,		// polygone r�gulier: �toile ?
		RegularShape,		// polygone r�gulier: forme
		TextString,			// texte
		TextFontName,		// police
		TextFontOptical,	// police
	}

	/// <summary>
	/// La classe Property repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	public abstract class AbstractProperty
	{
		public AbstractProperty()
		{
		}

		// Cr�e une nouvelle propri�t�.
		static public AbstractProperty NewProperty(PropertyType type)
		{
			AbstractProperty property = null;
			switch ( type )
			{
				case PropertyType.LineColor:        property = new PropertyColor();     break;
				case PropertyType.LineMode:         property = new PropertyLine();      break;
				case PropertyType.FillGradient:     property = new PropertyGradient();  break;
				case PropertyType.Shadow:           property = new PropertyShadow();    break;
				case PropertyType.PolyClose:        property = new PropertyBool();      break;
				case PropertyType.RoundRect:        property = new PropertyDouble();    break;
				case PropertyType.RegularFaces:     property = new PropertyDouble();    break;
				case PropertyType.RegularStar:      property = new PropertyBool();      break;
				case PropertyType.RegularShape:     property = new PropertyDouble();    break;
				case PropertyType.TextString:       property = new PropertyString();    break;
				case PropertyType.TextFontName:     property = new PropertyList();      break;
				case PropertyType.TextFontOptical:  property = new PropertyList();      break;
			}
			if ( property == null )  return null;
			property.Type = type;
			return property;
		}

		// Type de la propri�t�.
		[XmlAttribute]
		public PropertyType Type
		{
			get
			{
				return this.type;
			}

			set
			{
				this.type = value;
			}
		}

		// Nom de la propri�t�.
		[XmlIgnore]
		public string Text
		{
			get
			{
				return this.text;
			}

			set
			{
				this.text = value;
			}
		}

		// Couleur de fond du panneau associ�.
		[XmlIgnore]
		public Drawing.Color BackgroundColor
		{
			get
			{
				return this.backgroundColor;
			}

			set
			{
				this.backgroundColor = value;
			}
		}

		// Mode de d�ploiement du panneau associ�.
		[XmlIgnore]
		public bool ExtendedSize
		{
			get
			{
				return this.extendedSize;
			}

			set
			{
				this.extendedSize = value;
			}
		}

		// Repr�sentation de plusieurs propri�t�s contradictoires.
		[XmlIgnore]
		public bool Multi
		{
			get
			{
				return this.multi;
			}

			set
			{
				this.multi = value;
			}
		}

		// Effectue une copie de la propri�t�.
		public virtual void CopyTo(AbstractProperty property)
		{
			property.type            = this.type;
			property.text            = this.text;
			property.backgroundColor = this.backgroundColor;
			property.extendedSize    = this.extendedSize;
			property.multi           = this.multi;
		}

		// Effectue une copie des informations de base de la propri�t�.
		public void CopyInfoTo(AbstractProperty property)
		{
			property.type            = this.type;
			property.text            = this.text;
			property.backgroundColor = this.backgroundColor;
			property.extendedSize    = this.extendedSize;
			property.multi           = this.multi;
		}

		// Compare deux propri�t�s.
		public virtual bool Compare(AbstractProperty property)
		{
			if ( property.type != this.type )  return false;
			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public virtual AbstractPanel CreatePanel()
		{
			return null;
		}

		protected string			text;
		protected Drawing.Color		backgroundColor;
		protected bool				extendedSize = false;
		protected bool				multi = false;

		protected PropertyType		type;
	}
}
