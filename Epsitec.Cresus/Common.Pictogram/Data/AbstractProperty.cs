using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
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
		Arrow,				// extr�mit� des segments
		Corner,				// coins des rectangles
		Regular,			// d�finitions du polygone r�gulier
		TextString,			// texte
		TextFontName,		// police
		TextFontOptical,	// police
		ModColor,			// modification de couleur
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
				case PropertyType.Arrow:            property = new PropertyArrow();     break;
				case PropertyType.Corner:           property = new PropertyCorner();    break;
				case PropertyType.Regular:          property = new PropertyRegular();   break;
				case PropertyType.TextString:       property = new PropertyString();    break;
				case PropertyType.TextFontName:     property = new PropertyCombo();     break;
				case PropertyType.TextFontOptical:  property = new PropertyCombo();     break;
				case PropertyType.ModColor:         property = new PropertyModColor();  break;
			}
			if ( property == null )  return null;
			property.Type = type;
			return property;
		}

		// Retourne le nom d'un type de propri�t�.
		static public string TypeName(PropertyType type)
		{
			switch ( type )
			{
				case PropertyType.LineColor:        return "LineColor";
				case PropertyType.LineMode:         return "LineMode";
				case PropertyType.FillGradient:     return "FillGradient";
				case PropertyType.Shadow:           return "Shadow";
				case PropertyType.PolyClose:        return "PolyClose";
				case PropertyType.Arrow:            return "Arrow";
				case PropertyType.Corner:           return "Corner";
				case PropertyType.Regular:          return "Regular";
				case PropertyType.TextString:       return "TextString";
				case PropertyType.TextFontName:     return "TextFontName";
				case PropertyType.TextFontOptical:  return "TextFontOptical";
				case PropertyType.ModColor:         return "ModColor";
			}
			return "";
		}

		// Retourne le type de propri�t� d'apr�s son nom.
		static public PropertyType TypeName(string typeName)
		{
			switch ( typeName )
			{
				case "LineColor":        return PropertyType.LineColor;
				case "LineMode":         return PropertyType.LineMode;
				case "FillGradient":     return PropertyType.FillGradient;
				case "Shadow":           return PropertyType.Shadow;
				case "PolyClose":        return PropertyType.PolyClose;
				case "Arrow":            return PropertyType.Arrow;
				case "Corner":           return PropertyType.Corner;
				case "Regular":          return PropertyType.Regular;
				case "TextString":       return PropertyType.TextString;
				case "TextFontName":     return PropertyType.TextFontName;
				case "TextFontOptical":  return PropertyType.TextFontOptical;
				case "ModColor":         return PropertyType.ModColor;

			}
			return PropertyType.None;
		}

		// Type de la propri�t�.
		[XmlAttribute]
		public PropertyType Type
		{
			get { return this.type; }
			set { this.type = value; }
		}

		// Type de la propri�t�.
		[XmlAttribute]
		public string StyleName
		{
			get { return this.styleName; }
			set { this.styleName = value; }
		}

		// Type de la propri�t�.
		[XmlAttribute]
		public int StyleID
		{
			get { return this.styleID; }
			set { this.styleID = value; }
		}

		// Nom de la propri�t�.
		[XmlIgnore]
		public string Text
		{
			get { return this.text; }
			set { this.text = value; }
		}

		// Nom de la propri�t� ou du style si c'en est un.
		[XmlIgnore]
		public string TextStyle
		{
			get
			{
				if ( this.styleName == "" )
				{
					return this.text;
				}
				else
				{
					return string.Format("<b>{0}</b>", this.styleName);
				}
			}
		}

		// Couleur de fond du panneau associ�.
		[XmlIgnore]
		public double BackgroundIntensity
		{
			get { return this.backgroundIntensity; }
			set { this.backgroundIntensity = value; }
		}

		// Mode de d�ploiement du panneau associ�.
		[XmlIgnore]
		public bool ExtendedSize
		{
			get { return this.extendedSize; }
			set { this.extendedSize = value; }
		}

		// Repr�sentation de plusieurs propri�t�s contradictoires.
		[XmlIgnore]
		public bool Multi
		{
			get { return this.multi; }
			set { this.multi = value; }
		}

		// Indique s'il faut �diter les propri�t�s.
		[XmlIgnore]
		public bool EditProperties
		{
			get { return this.editProperties; }
			set { this.editProperties = value; }
		}

		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		[XmlIgnore]
		public virtual bool AlterBoundingBox
		{
			get { return false; }
		}

		// Indique si cette propri�t� peut faire l'objet d'un style.
		[XmlIgnore]
		public virtual bool StyleAbility
		{
			get { return true; }
		}

		// Effectue une copie de la propri�t�.
		public virtual void CopyTo(AbstractProperty property)
		{
			this.CopyInfoTo(property);
			property.styleName      = this.styleName;
			property.styleID        = this.styleID;
			property.editProperties = this.editProperties;
		}

		// Effectue une copie des informations de base de la propri�t�.
		public virtual void CopyInfoTo(AbstractProperty property)
		{
			property.type                = this.type;
			property.text                = this.text;
			property.backgroundIntensity = this.backgroundIntensity;
			property.extendedSize        = this.extendedSize;
			property.multi               = this.multi;
		}

		// Compare deux propri�t�s.
		public virtual bool Compare(AbstractProperty property)
		{
			if ( property.type      != this.type      )  return false;
			if ( property.styleName != this.styleName )  return false;
			if ( property.styleID   != this.styleID   )  return false;
			return true;
		}

		// Cherche une propri�t� de m�me type dans une liste.
		public AbstractProperty Search(System.Collections.ArrayList list)
		{
			foreach ( AbstractProperty property in list )
			{
				if ( property.Type == this.type )  return property;
			}
			return null;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public virtual AbstractPanel CreatePanel()
		{
			return null;
		}


		// Nombre de poign�es.
		public virtual int TotalHandle
		{
			get { return this.handles.Count; }
		}

		// Donne une poign�e de la propri�t�.
		public virtual Handle Handle(int rank, Drawing.Rectangle bbox)
		{
			return null;
		}

		// D�but du d�placement d'une poign�e de la propri�t�.
		public virtual void MoveHandleStarting(int rank, Drawing.Point pos, Drawing.Rectangle bbox, IconContext iconContext)
		{
			iconContext.ConstrainFixStarting(pos);
		}

		// D�place une poign�e de la propri�t�.
		public virtual void MoveHandleProcess(int rank, Drawing.Point pos, Drawing.Rectangle bbox, IconContext iconContext)
		{
		}

		// Indique si les poign�es sont visibles.
		public virtual bool IsHandleVisible()
		{
			return false;
		}

		// Dessine les traits de construction avant les poign�es.
		public virtual void DrawEdit(Drawing.Graphics graphics, IconContext iconContext, Drawing.Rectangle bbox)
		{
		}


		// G�n�re un �v�nement pour dire que la propri�t� a chang�.
		// En fait, ce sont les objets qui vont �couter cet �v�nement, pour
		// �ventuellement modifier les poign�es qui refl�tent les propri�t�s.
		protected virtual void OnChanged()
		{
			if ( this.Changed != null )  // qq'un �coute ?
			{
				this.Changed(this);
			}
		}

		public event EventHandler Changed;


		protected string						text;
		protected double						backgroundIntensity;
		protected bool							extendedSize = false;
		protected bool							multi = false;
		protected bool							editProperties = false;

		protected PropertyType					type;
		protected string						styleName = "";
		protected int							styleID = 0;
		protected System.Collections.ArrayList	handles = new System.Collections.ArrayList();
	}
}
