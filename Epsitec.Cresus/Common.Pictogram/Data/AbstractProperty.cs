using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	public enum PropertyType
	{
		None,				// aucune
		Name,				// nom de l'objet
		LineColor,			// couleur du trait
		LineMode,			// mode du trait
		FillGradient,		// d�grad� de remplissage
		Shadow,				// ombre sous l'objet
		PolyClose,			// mode de fermeture des polygones
		Arrow,				// extr�mit� des segments
		Corner,				// coins des rectangles
		Regular,			// d�finitions du polygone r�gulier
		Arc,				// arc de cercle ou d'ellipse
		BackColor,			// texte: couleur de fond
		TextFont,			// texte: police
		TextJustif,			// texte: justification
		TextLine,			// texte: justification
		Image,				// nom de l'image bitmap
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
				case PropertyType.Name:             property = new PropertyName();      break;
				case PropertyType.LineColor:        property = new PropertyColor();     break;
				case PropertyType.LineMode:         property = new PropertyLine();      break;
				case PropertyType.FillGradient:     property = new PropertyGradient();  break;
				case PropertyType.Shadow:           property = new PropertyShadow();    break;
				case PropertyType.PolyClose:        property = new PropertyBool();      break;
				case PropertyType.Arrow:            property = new PropertyArrow();     break;
				case PropertyType.Corner:           property = new PropertyCorner();    break;
				case PropertyType.Regular:          property = new PropertyRegular();   break;
				case PropertyType.Arc:              property = new PropertyArc();       break;
				case PropertyType.BackColor:        property = new PropertyColor();     break;
				case PropertyType.TextFont:         property = new PropertyFont();      break;
				case PropertyType.TextJustif:       property = new PropertyJustif();    break;
				case PropertyType.TextLine:         property = new PropertyTextLine();  break;
				case PropertyType.Image:            property = new PropertyImage();     break;
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
				case PropertyType.Name:             return "Name";
				case PropertyType.LineColor:        return "LineColor";
				case PropertyType.LineMode:         return "LineMode";
				case PropertyType.FillGradient:     return "FillGradient";
				case PropertyType.Shadow:           return "Shadow";
				case PropertyType.PolyClose:        return "PolyClose";
				case PropertyType.Arrow:            return "Arrow";
				case PropertyType.Corner:           return "Corner";
				case PropertyType.Regular:          return "Regular";
				case PropertyType.Arc:              return "Arc";
				case PropertyType.BackColor:        return "BackColor";
				case PropertyType.TextFont:         return "TextFont";
				case PropertyType.TextJustif:       return "TextJustif";
				case PropertyType.TextLine:         return "TextLine";
				case PropertyType.Image:            return "Image";
				case PropertyType.ModColor:         return "ModColor";
			}
			return "";
		}

		// Retourne le type de propri�t� d'apr�s son nom.
		static public PropertyType TypeName(string typeName)
		{
			switch ( typeName )
			{
				case "Name":             return PropertyType.Name;
				case "LineColor":        return PropertyType.LineColor;
				case "LineMode":         return PropertyType.LineMode;
				case "FillGradient":     return PropertyType.FillGradient;
				case "Shadow":           return PropertyType.Shadow;
				case "PolyClose":        return PropertyType.PolyClose;
				case "Arrow":            return PropertyType.Arrow;
				case "Corner":           return PropertyType.Corner;
				case "Regular":          return PropertyType.Regular;
				case "Arc":              return PropertyType.Arc;
				case "BackColor":        return PropertyType.BackColor;
				case "TextFont":         return PropertyType.TextFont;
				case "TextJustif":       return PropertyType.TextJustif;
				case "TextLine":         return PropertyType.TextLine;
				case "Image":            return PropertyType.Image;
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

		// Intensit� pour le fond du panneau.
		[XmlIgnore]
		public double BackgroundIntensity
		{
			get
			{
				switch ( this.type )
				{
					case PropertyType.Name:             return 0.70;
					case PropertyType.LineColor:        return 0.85;
					case PropertyType.LineMode:         return 0.85;
					case PropertyType.FillGradient:     return 0.95;
					case PropertyType.Shadow:           return 0.80;
					case PropertyType.PolyClose:        return 0.90;
					case PropertyType.Arrow:            return 0.85;
					case PropertyType.Corner:           return 0.90;
					case PropertyType.Regular:          return 0.90;
					case PropertyType.Arc:              return 0.90;
					case PropertyType.BackColor:        return 0.80;
					case PropertyType.TextFont:         return 0.80;
					case PropertyType.TextJustif:       return 0.80;
					case PropertyType.TextLine:         return 0.80;
					case PropertyType.Image:            return 0.90;
					case PropertyType.ModColor:         return 0.95;
				}
				return 0.0;
			}
		}

		// Nom de la propri�t�.
		[XmlIgnore]
		public string Text
		{
			get
			{
				switch ( this.type )
				{
					case PropertyType.Name:             return "Nom";
					case PropertyType.LineColor:        return "Couleur trait";
					case PropertyType.LineMode:         return "Epaisseur trait";
					case PropertyType.FillGradient:     return "Couleur int�rieure";
					case PropertyType.Shadow:           return "Ombre";
					case PropertyType.PolyClose:        return "Contour ferm�";
					case PropertyType.Arrow:            return "Extr�mit�s";
					case PropertyType.Corner:           return "Coins";
					case PropertyType.Regular:          return "Nombre de c�t�s";
					case PropertyType.Arc:              return "Arc";
					case PropertyType.BackColor:        return "Couleur fond";
					case PropertyType.TextFont:         return "Police";
					case PropertyType.TextJustif:       return "Mise en page";
					case PropertyType.TextLine:         return "Position texte";
					case PropertyType.Image:            return "Image";
					case PropertyType.ModColor:         return "Transformation de couleur :";
				}
				return "";
			}
		}

		// Nom de la propri�t� ou du style si c'en est un.
		[XmlIgnore]
		public string TextStyle
		{
			get
			{
				if ( this.styleName == "" )
				{
					return this.Text;
				}
				else
				{
					return string.Format("<b>{0}</b>", this.styleName);
				}
			}
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
			property.type           = this.type;
			property.multi          = this.multi;
			property.styleName      = this.styleName;
			property.styleID        = this.styleID;
			property.editProperties = this.editProperties;
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
		public virtual AbstractPanel CreatePanel(Drawer drawer)
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


		// Initialise le zoom par d�faut d'un chemin.
		static public double DefaultZoom(IconContext iconContext)
		{
			if ( iconContext == null )
			{
				return 2.0;
			}
			else
			{
				return iconContext.ScaleX;
			}
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


		protected PropertyType					type = PropertyType.None;
		protected string						styleName = "";
		protected int							styleID = 0;
		protected bool							multi = false;
		protected bool							editProperties = false;
		protected bool							extendedSize = false;

		protected System.Collections.ArrayList	handles = new System.Collections.ArrayList();
	}
}
