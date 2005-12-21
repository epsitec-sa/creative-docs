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
		FillGradient,		// dégradé de remplissage
		Shadow,				// ombre sous l'objet
		PolyClose,			// mode de fermeture des polygones
		Arrow,				// extrémité des segments
		Corner,				// coins des rectangles
		Regular,			// définitions du polygone régulier
		Arc,				// arc de cercle ou d'ellipse
		BackColor,			// texte: couleur de fond
		TextFont,			// texte: police
		TextJustif,			// texte: justification
		TextLine,			// texte: justification
		Image,				// nom de l'image bitmap
		ModColor,			// modification de couleur
	}

	/// <summary>
	/// La classe Property représente une propriété d'un objet graphique.
	/// </summary>
	public abstract class AbstractProperty
	{
		public AbstractProperty()
		{
		}

		static public AbstractProperty NewProperty(PropertyType type)
		{
			//	Crée une nouvelle propriété.
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

		static public string TypeName(PropertyType type)
		{
			//	Retourne le nom d'un type de propriété.
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

		static public PropertyType TypeName(string typeName)
		{
			//	Retourne le type de propriété d'après son nom.
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

		[XmlAttribute]
		public PropertyType Type
		{
			//	Type de la propriété.
			get { return this.type; }
			set { this.type = value; }
		}

		[XmlAttribute]
		public string StyleName
		{
			//	Type de la propriété.
			get { return this.styleName; }
			set { this.styleName = value; }
		}

		[XmlAttribute]
		public int StyleID
		{
			//	Style de la propriété (0 = pas un style).
			get { return this.styleID; }
			set { this.styleID = value; }
		}

		[XmlIgnore]
		public double BackgroundIntensity
		{
			//	Intensité pour le fond du panneau.
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

		[XmlIgnore]
		public string Text
		{
			//	Nom de la propriété.
			get
			{
				switch ( this.type )
				{
					case PropertyType.Name:             return "Nom";
					case PropertyType.LineColor:        return "Couleur trait";
					case PropertyType.LineMode:         return "Epaisseur trait";
					case PropertyType.FillGradient:     return "Couleur intérieure";
					case PropertyType.Shadow:           return "Ombre";
					case PropertyType.PolyClose:        return "Contour fermé";
					case PropertyType.Arrow:            return "Extrémités";
					case PropertyType.Corner:           return "Coins";
					case PropertyType.Regular:          return "Nombre de côtés";
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

		[XmlIgnore]
		public string TextStyle
		{
			//	Nom de la propriété ou du style si c'en est un.
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

		[XmlIgnore]
		public bool ExtendedSize
		{
			//	Mode de déploiement du panneau associé.
			get { return this.extendedSize; }
			set { this.extendedSize = value; }
		}

		[XmlIgnore]
		public bool Multi
		{
			//	Représentation de plusieurs propriétés contradictoires.
			get { return this.multi; }
			set { this.multi = value; }
		}

		[XmlIgnore]
		public bool EditProperties
		{
			//	Indique s'il faut éditer les propriétés.
			get { return this.editProperties; }
			set { this.editProperties = value; }
		}

		[XmlIgnore]
		public virtual bool AlterBoundingBox
		{
			//	Indique si un changement de cette propriété modifie la bbox de l'objet.
			get { return false; }
		}

		[XmlIgnore]
		public virtual bool StyleAbility
		{
			//	Indique si cette propriété peut faire l'objet d'un style.
			get { return true; }
		}

		public virtual void CopyTo(AbstractProperty property)
		{
			//	Effectue une copie de la propriété.
			property.type           = this.type;
			property.multi          = this.multi;
			property.styleName      = this.styleName;
			property.styleID        = this.styleID;
			property.editProperties = this.editProperties;
		}

		public virtual bool Compare(AbstractProperty property)
		{
			//	Compare deux propriétés.
			if ( property.type      != this.type      )  return false;
			if ( property.styleName != this.styleName )  return false;
			if ( property.styleID   != this.styleID   )  return false;
			return true;
		}

		public AbstractProperty Search(System.Collections.ArrayList list)
		{
			//	Cherche une propriété de même type dans une liste.
			foreach ( AbstractProperty property in list )
			{
				if ( property.Type == this.type )  return property;
			}
			return null;
		}

		public virtual AbstractPanel CreatePanel(Drawer drawer)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			return null;
		}


		public virtual int TotalHandle
		{
			//	Nombre de poignées.
			get { return this.handles.Count; }
		}

		public virtual Handle Handle(int rank, Drawing.Rectangle bbox)
		{
			//	Donne une poignée de la propriété.
			return null;
		}

		public virtual void MoveHandleStarting(int rank, Drawing.Point pos, Drawing.Rectangle bbox, IconContext iconContext)
		{
			//	Début du déplacement d'une poignée de la propriété.
			iconContext.ConstrainFixStarting(pos);
		}

		public virtual void MoveHandleProcess(int rank, Drawing.Point pos, Drawing.Rectangle bbox, IconContext iconContext)
		{
			//	Déplace une poignée de la propriété.
		}

		public virtual bool IsHandleVisible()
		{
			//	Indique si les poignées sont visibles.
			return false;
		}

		public virtual void DrawEdit(Drawing.Graphics graphics, IconContext iconContext, Drawing.Rectangle bbox)
		{
			//	Dessine les traits de construction avant les poignées.
		}


		static public double DefaultZoom(IconContext iconContext)
		{
			//	Initialise le zoom par défaut d'un chemin.
			if ( iconContext == null )
			{
				return 2.0;
			}
			else
			{
				return iconContext.ScaleX;
			}
		}


		protected virtual void OnChanged()
		{
			//	Génère un événement pour dire que la propriété a changé.
			//	En fait, ce sont les objets qui vont écouter cet événement, pour
			//	éventuellement modifier les poignées qui reflètent les propriétés.
			if ( this.Changed != null )  // qq'un écoute ?
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
