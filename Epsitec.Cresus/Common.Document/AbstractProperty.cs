using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	public enum PropertyType
	{
		None,				// aucune
		Name,				// nom de l'objet
		LineColor,			// couleur du trait
		LineMode,			// mode du trait
		Arrow,				// extr�mit� des segments
		FillGradient,		// d�grad� de remplissage
		BackColor,			// texte: couleur de fond
		Shadow,				// ombre sous l'objet
		PolyClose,			// mode de fermeture des polygones
		Corner,				// coins des rectangles
		Regular,			// d�finitions du polygone r�gulier
		Arc,				// arc de cercle ou d'ellipse
		TextFont,			// texte: police
		TextJustif,			// texte: justification
		TextLine,			// texte: justification
		Image,				// nom de l'image bitmap
		ModColor,			// modification de couleur
	}

	/// <summary>
	/// La classe Property repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public abstract class AbstractProperty : System.IComparable, ISerializable
	{
		public AbstractProperty(Document document)
		{
			this.document = document;
			this.owners = new UndoableList(this.document, UndoableListType.ObjectsInsideProperty);
		}

		// Cr�e une nouvelle propri�t�.
		static public AbstractProperty NewProperty(Document document, PropertyType type)
		{
			AbstractProperty property = null;
			switch ( type )
			{
				case PropertyType.Name:             property = new PropertyName(document);      break;
				case PropertyType.LineColor:        property = new PropertyColor(document);     break;
				case PropertyType.LineMode:         property = new PropertyLine(document);      break;
				case PropertyType.FillGradient:     property = new PropertyGradient(document);  break;
				case PropertyType.Shadow:           property = new PropertyShadow(document);    break;
				case PropertyType.PolyClose:        property = new PropertyBool(document);      break;
				case PropertyType.Arrow:            property = new PropertyArrow(document);     break;
				case PropertyType.Corner:           property = new PropertyCorner(document);    break;
				case PropertyType.Regular:          property = new PropertyRegular(document);   break;
				case PropertyType.Arc:              property = new PropertyArc(document);       break;
				case PropertyType.BackColor:        property = new PropertyColor(document);     break;
				case PropertyType.TextFont:         property = new PropertyFont(document);      break;
				case PropertyType.TextJustif:       property = new PropertyJustif(document);    break;
				case PropertyType.TextLine:         property = new PropertyTextLine(document);  break;
				case PropertyType.Image:            property = new PropertyImage(document);     break;
				case PropertyType.ModColor:         property = new PropertyModColor(document);  break;
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

		// Liste des propri�taires. Normalement, un propri�taire est un AbstractObject.
		// Mais une propri�t� "isMulti" contient une liste de propri�taires de type
		// AbstractProperty !
		public UndoableList Owners
		{
			get
			{
				return this.owners;
			}
		}

		// Type de la propri�t�.
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

		// Type de la propri�t�.
		public string StyleName
		{
			get
			{
				return this.styleName;
			}
			
			set
			{
				if ( this.styleName != value )
				{
					this.InsertOpletProperty();
					this.styleName = value;
				}
			}
		}

		// Indique si la propri�t� ne sert qu'� cr�er de nouveaux objets.
		public bool IsOnlyForCreation
		{
			get
			{
				return this.isOnlyForCreation;
			}
			
			set
			{
				this.isOnlyForCreation = value;
			}
		}

		// Indique si la propri�t� est un style.
		public bool IsStyle
		{
			get
			{
				return this.isStyle;
			}
			
			set
			{
				if ( this.isStyle != value )
				{
					this.InsertOpletProperty();
					this.isStyle = value;
					this.document.Notifier.NotifyStyleChanged();
					this.document.Notifier.NotifySelectionChanged();
				}
			}
		}

		// Indique si la propri�t� est flottante.
		// Une propri�t� flottante n'est r�f�renc�e par personne et elle n'est pas
		// dans la liste des propri�t�s du document. ObjectPoly cr�e un ObjectLine
		// avec des propri�t�s flottantes, pendant la cr�ation.
		public bool IsFloating
		{
			get
			{
				return this.isFloating;
			}
			
			set
			{
				this.isFloating = value;
			}
		}

		// Intensit� pour le fond du panneau.
		public static double BackgroundIntensity(PropertyType type)
		{
			switch ( type )
			{
				case PropertyType.Name:             return 0.70;
				case PropertyType.LineColor:        return 0.85;
				case PropertyType.LineMode:         return 0.85;
				case PropertyType.Arrow:            return 0.85;
				case PropertyType.FillGradient:     return 0.95;
				case PropertyType.BackColor:        return 0.95;
				case PropertyType.Shadow:           return 0.80;
				case PropertyType.PolyClose:        return 0.90;
				case PropertyType.Corner:           return 0.90;
				case PropertyType.Regular:          return 0.90;
				case PropertyType.Arc:              return 0.90;
				case PropertyType.TextFont:         return 0.80;
				case PropertyType.TextJustif:       return 0.80;
				case PropertyType.TextLine:         return 0.80;
				case PropertyType.Image:            return 0.90;
				case PropertyType.ModColor:         return 0.95;
			}
			return 0.0;
		}

		// Nom de la propri�t�.
		public static string Text(PropertyType type)
		{
			switch ( type )
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
				case PropertyType.ModColor:         return "Couleur calque";
			}
			return "";
		}

		// Nom de la propri�t� ou du style si c'en est un.
		public string TextStyle
		{
			get
			{
				if ( this.isStyle )
				{
					return string.Format("<b>{0}</b>", this.styleName);
				}
				else
				{
					return AbstractProperty.Text(this.type);
				}
			}
		}

		// Indique si la propri�t� est utilis�e par un objet s�lectionn�e.
		public bool IsSelected
		{
			get
			{
				return this.isSelected;
			}
			
			set
			{
				if ( this.isSelected != value )
				{
					this.isSelected = value;
				}
			}
		}

		// Mode de d�ploiement du panneau associ�.
		public bool IsExtendedSize
		{
			get
			{
				return this.isExtendedSize;
			}
			
			set
			{
				this.isExtendedSize = value;
			}
		}

		// Repr�sentation de plusieurs propri�t�s contradictoires.
		public bool IsMulti
		{
			get
			{
				return this.isMulti;
			}
			
			set
			{
				this.isMulti = value;
			}
		}

		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		public virtual bool AlterBoundingBox
		{
			get { return false; }
		}

		// Indique si cette propri�t� peut faire l'objet d'un style.
		public static bool StyleAbility(PropertyType type)
		{
			return ( type != PropertyType.None      &&
					 type != PropertyType.Name      &&
					 type != PropertyType.Shadow    &&
					 type != PropertyType.ModColor  &&
					 type != PropertyType.PolyClose );
		}

		// Reprend une propri�t� d'un objet mod�le.
		public void PickerProperty(AbstractProperty model)
		{
			System.Diagnostics.Debug.Assert(this.isStyle);
			System.Diagnostics.Debug.Assert(this.Type == model.Type);
			this.NotifyBefore();
			string name = this.styleName;
			model.CopyTo(this);
			this.isStyle = true;
			this.styleName = name;
			this.NotifyAfter();
			this.document.Notifier.NotifyPropertyChanged(this);
		}

		// Effectue une copie compl�te de la propri�t�.
		public void DeepCopyTo(AbstractProperty property)
		{
			this.CopyTo(property);

			this.owners.CopyTo(property.owners);
			property.isSelected     = this.isSelected;
			property.isMulti        = this.isMulti;
			property.isExtendedSize = this.isExtendedSize;
		}

		// Effectue une copie de la propri�t�.
		public virtual void CopyTo(AbstractProperty property)
		{
			property.type      = this.type;
			property.styleName = this.styleName;
			property.isStyle   = this.isStyle;
		}

		// D�termine si Compare doit tenir compte du nom du style.
		public bool IsCompareStyleName
		{
			get { return this.isCompareStyleName; }
			set { this.isCompareStyleName = value; }
		}

		// Compare deux propri�t�s.
		// Il ne faut surtout pas comparer isStyle, car les styles et les
		// propri�t�s sont dans des listes diff�rentes.
		public virtual bool Compare(AbstractProperty property)
		{
			if ( property.type != this.type )  return false;
			if ( this.isCompareStyleName && property.styleName != this.styleName )  return false;
			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public virtual AbstractPanel CreatePanel(Document document)
		{
			return null;
		}


		// Nombre de poign�es.
		public virtual int TotalHandle(AbstractObject obj)
		{
			return 0;
		}

		// Indique si une poign�e est visible.
		public virtual bool IsHandleVisible(AbstractObject obj, int rank)
		{
			return false;
		}

		// Retourne la position d'une poign�e.
		public virtual Point GetHandlePosition(AbstractObject obj, int rank)
		{
			return new Point();
		}
		
		// Modifie la position d'une poign�e.
		public virtual void SetHandlePosition(AbstractObject obj, int rank, Point pos)
		{
			this.document.Notifier.NotifyPropertyChanged(this);
		}
		
		// Dessine les traits de construction avant les poign�es.
		public virtual void DrawEdit(Graphics graphics, DrawingContext drawingContext, AbstractObject obj)
		{
		}


		// Initialise le zoom par d�faut d'un chemin.
		static public double DefaultZoom(DrawingContext drawingContext)
		{
			if ( drawingContext == null )
			{
				return 2.0;
			}
			else
			{
				return drawingContext.ScaleX;
			}
		}


		#region IComparable Members
		public int CompareTo(object obj)
		{
			if ( obj is AbstractProperty )
			{
				AbstractProperty property = obj as AbstractProperty;
				int eq = this.type.CompareTo(property.type);
				if ( eq != 0 )  return eq;

				if ( this.owners.Count == 0 || property.owners.Count == 0 )  return eq;

				// Attention: lors d'une s�lection de plusieurs objets ayant des
				// propri�t�s diff�rentes, les propri�taires de la propri�t� ne sont
				// pas des AbstractObject, mais des AbstractProperty (isMulti).
				AbstractObject obj1 = this.owners[0] as AbstractObject;
				int id1 = -1;  if ( obj1 != null )  id1 = obj1.UniqueId;

				AbstractObject obj2 = property.owners[0] as AbstractObject;
				int id2 = -1;  if ( obj2 != null )  id2 = obj2.UniqueId;

				return id1.CompareTo(id2);
			}
			throw new System.ArgumentException("object is not a AbstractProperty");
		}
		#endregion


		// Signale que tous les propri�taires vont changer.
		protected void NotifyBefore()
		{
			this.NotifyBefore(true);
		}
		
		protected virtual void NotifyBefore(bool oplet)
		{
			if ( this.isOnlyForCreation )  return;

			if ( oplet )  this.InsertOpletProperty();

			int total = this.owners.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				if ( this.isMulti )
				{
					if ( oplet )
					{
						// Normalement, un propri�taire est un AbstractObject.
						// Mais une propri�t� "isMulti" contient une liste de propri�taires
						// de type AbstractProperty !
						AbstractProperty realProp = this.owners[i] as AbstractProperty;
						realProp.NotifyBefore(oplet);
					}
				}
				else
				{
					if ( this.AlterBoundingBox )
					{
						AbstractObject obj = this.owners[i] as AbstractObject;
						this.document.Notifier.NotifyArea(obj.BoundingBox);
					}
				}
			}
		}

		// Signale que tous les propri�taires ont chang�.
		protected void NotifyAfter()
		{
			this.NotifyAfter(true);
		}

		protected virtual void NotifyAfter(bool oplet)
		{
			if ( this.isOnlyForCreation )  return;

			int total = this.owners.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				if ( this.isMulti )
				{
					if ( oplet )
					{
						AbstractProperty realProp = this.owners[i] as AbstractProperty;
						this.CopyTo(realProp);
						realProp.NotifyAfter(oplet);
					}
				}
				else
				{
					AbstractObject obj = this.owners[i] as AbstractObject;
					obj.HandlePropertiesUpdateVisible();
					obj.HandlePropertiesUpdatePosition();

					if ( this.AlterBoundingBox )
					{
						obj.SetDirtyBbox();
					}

					if ( this.isStyle )
					{
						this.document.Notifier.NotifyPropertyChanged(this);
					}

					this.document.Notifier.NotifyArea(obj.BoundingBox);
				}
			}
		}


		#region OpletProperty
		// Ajoute un oplet pour m�moriser la propri�t�.
		protected void InsertOpletProperty()
		{
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletProperty oplet = new OpletProperty(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		// M�morise toute la propri�t�.
		protected class OpletProperty : AbstractOplet
		{
			public OpletProperty(AbstractProperty host)
			{
				this.host = host;

				this.copy = AbstractProperty.NewProperty(this.host.document, this.host.type);
				this.host.DeepCopyTo(this.copy);
			}

			public AbstractProperty Property
			{
				get { return this.host; }
			}

			protected void Swap()
			{
				this.host.NotifyBefore(false);

				AbstractProperty temp = AbstractProperty.NewProperty(this.host.document, this.host.type);
				this.host.DeepCopyTo(temp);
				this.copy.DeepCopyTo(this.host);  // this.host <-> this.copy
				temp.DeepCopyTo(this.copy);

				if ( this.host.isStyle != this.copy.isStyle )
				{
					this.host.document.Notifier.NotifyStyleChanged();
					this.host.document.Notifier.NotifySelectionChanged();
				}
				this.host.NotifyAfter(false);
			}

			public override IOplet Undo()
			{
				this.Swap();
				return this;
			}

			public override IOplet Redo()
			{
				this.Swap();
				return this;
			}

			protected AbstractProperty				host;
			protected AbstractProperty				copy;
		}
		#endregion

		
		#region Serialization
		// S�rialise la propri�t�.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Type", this.type);
			info.AddValue("IsStyle", this.isStyle);
			if ( this.isStyle )
			{
				info.AddValue("StyleName", this.styleName);
			}
		}

		// Constructeur qui d�s�rialise la propri�t�.
		protected AbstractProperty(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;
			this.type = (PropertyType) info.GetValue("Type", typeof(PropertyType));
			this.isStyle = info.GetBoolean("IsStyle");
			if ( this.isStyle )
			{
				this.styleName = info.GetString("StyleName");
			}

			this.owners = new UndoableList(this.document, UndoableListType.ObjectsInsideProperty);
		}
		#endregion


		protected Document						document;
		protected PropertyType					type = PropertyType.None;
		protected UndoableList					owners;
		protected string						styleName = "";
		protected bool							isOnlyForCreation = false;
		protected bool							isStyle = false;
		protected bool							isFloating = false;
		protected bool							isMulti = false;
		protected bool							isExtendedSize = false;
		protected bool							isSelected = false;
		protected bool							isCompareStyleName = true;
	}
}
