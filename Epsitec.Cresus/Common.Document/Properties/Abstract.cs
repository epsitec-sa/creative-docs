using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	// ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	// sous peine de plantée lors de la désérialisation.
	public enum Type
	{
		None         = 0,		// aucune
		Name         = 1,		// nom de l'objet
		LineColor    = 2,		// couleur du trait
		LineMode     = 3,		// mode du trait
		Arrow        = 4,		// extrémité des segments
		FillGradient = 5,		// dégradé de remplissage
		BackColor    = 6,		// texte: couleur de fond
		Shadow       = 7,		// ombre sous l'objet
		PolyClose    = 8,		// mode de fermeture des polygones
		Corner       = 9,		// coins des rectangles
		Regular      = 10,		// définitions du polygone régulier
		Arc          = 11,		// arc de cercle ou d'ellipse
		TextFont     = 12,		// texte: police
		TextJustif   = 13,		// texte: justification
		TextLine     = 14,		// texte: justification
		Image        = 15,		// nom de l'image bitmap
		ModColor     = 16,		// modification de couleur
	}

	/// <summary>
	/// La classe Property représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public abstract class Abstract : System.IComparable, ISerializable
	{
		public Abstract(Document document, Type type)
		{
			this.document = document;
			this.type = type;
			this.owners = new UndoableList(this.document, UndoableListType.ObjectsInsideProperty);
		}

		// Crée une nouvelle propriété.
		static public Abstract NewProperty(Document document, Type type)
		{
			Abstract property = null;
			switch ( type )
			{
				case Type.Name:          property = new Name(document, type);      break;
				case Type.LineColor:     property = new Gradient(document, type);  break;
				case Type.LineMode:      property = new Line(document, type);      break;
				case Type.Arrow:         property = new Arrow(document, type);     break;
				case Type.FillGradient:  property = new Gradient(document, type);  break;
				case Type.BackColor:     property = new Color(document, type);     break;
				case Type.Shadow:        property = new Shadow(document, type);    break;
				case Type.PolyClose:     property = new Bool(document, type);      break;
				case Type.Corner:        property = new Corner(document, type);    break;
				case Type.Regular:       property = new Regular(document, type);   break;
				case Type.Arc:           property = new Arc(document, type);       break;
				case Type.TextFont:      property = new Font(document, type);      break;
				case Type.TextJustif:    property = new Justif(document, type);    break;
				case Type.TextLine:      property = new TextLine(document, type);  break;
				case Type.Image:         property = new Image(document, type);     break;
				case Type.ModColor:      property = new ModColor(document, type);  break;
			}
			return property;
		}

		// Retourne le nom d'un type de propriété.
		static public string TypeName(Type type)
		{
			return type.ToString();
		}

		// Retourne le type de propriété d'après son nom.
		static public Type TypeName(string typeName)
		{
			return (Type) System.Enum.Parse(typeof(Type), typeName);
		}

		// Retourne un numéro d'ordre pour le tri.
		static public int SortOrder(Type type)
		{
			switch ( type )
			{
				case Type.Name:          return 1;
				case Type.LineMode:      return 2;
				case Type.Arrow:         return 3;
				case Type.LineColor:     return 4;
				case Type.FillGradient:  return 5;
				case Type.BackColor:     return 6;
				case Type.Shadow:        return 7;
				case Type.PolyClose:     return 8;
				case Type.Corner:        return 9;
				case Type.Regular:       return 10;
				case Type.Arc:           return 11;
				case Type.TextFont:      return 12;
				case Type.TextJustif:    return 13;
				case Type.TextLine:      return 14;
				case Type.Image:         return 15;
				case Type.ModColor:      return 16;
			}
			return 0;
		}

		// Liste des propriétaires. Normalement, un propriétaire est un Objects.Abstract.
		// Mais une propriété "isMulti" contient une liste de propriétaires de type
		// Abstract !
		public UndoableList Owners
		{
			get
			{
				return this.owners;
			}
		}

		// Type de la propriété.
		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		// Type de la propriété.
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

		// Indique si la propriété ne sert qu'à créer de nouveaux objets.
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

		// Indique si la propriété est un style.
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

		// Indique si la propriété est flottante.
		// Une propriété flottante n'est référencée par personne et elle n'est pas
		// dans la liste des propriétés du document. ObjectPoly crée un ObjectLine
		// avec des propriétés flottantes, pendant la création.
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

		// Intensité pour le fond du panneau.
		public static double BackgroundIntensity(Type type)
		{
			switch ( type )
			{
				case Type.Name:          return 0.70;
				case Type.LineColor:     return 0.85;
				case Type.LineMode:      return 0.85;
				case Type.Arrow:         return 0.85;
				case Type.FillGradient:  return 0.95;
				case Type.BackColor:     return 0.95;
				case Type.Shadow:        return 0.80;
				case Type.PolyClose:     return 0.90;
				case Type.Corner:        return 0.90;
				case Type.Regular:       return 0.90;
				case Type.Arc:           return 0.90;
				case Type.TextFont:      return 0.80;
				case Type.TextJustif:    return 0.80;
				case Type.TextLine:      return 0.80;
				case Type.Image:         return 0.90;
				case Type.ModColor:      return 0.95;
			}
			return 0.0;
		}

		// Nom de la propriété.
		public static string Text(Type type)
		{
			switch ( type )
			{
				case Type.Name:          return "Nom";
				case Type.LineColor:     return "Couleur trait";
				case Type.LineMode:      return "Epaisseur trait";
				case Type.FillGradient:  return "Couleur intérieure";
				case Type.Shadow:        return "Ombre";
				case Type.PolyClose:     return "Contour fermé";
				case Type.Arrow:         return "Extrémités";
				case Type.Corner:        return "Coins";
				case Type.Regular:       return "Nombre de côtés";
				case Type.Arc:           return "Arc";
				case Type.BackColor:     return "Couleur fond";
				case Type.TextFont:      return "Police";
				case Type.TextJustif:    return "Mise en page";
				case Type.TextLine:      return "Position texte";
				case Type.Image:         return "Image";
				case Type.ModColor:      return "Couleur calque";
			}
			return "";
		}

		// Nom de la propriété ou du style si c'en est un.
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
					return Abstract.Text(this.type);
				}
			}
		}

		// Indique si la propriété est utilisée par un objet sélectionnée.
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

		// Mode de déploiement du panneau associé.
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

		// Représentation de plusieurs propriétés contradictoires.
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


		// Indique si un changement de cette propriété modifie la bbox de l'objet.
		public virtual bool AlterBoundingBox
		{
			get { return false; }
		}

		// Retourne la valeur d'engraissement pour la bbox.
		public virtual double WidthInflateBoundingBox()
		{
			return 0.0;
		}

		// Engraisse la bbox en fonction de la propriété.
		public virtual void InflateBoundingBox(Rectangle bbox, ref Rectangle bboxFull)
		{
		}


		// Indique si cette propriété peut faire l'objet d'un style.
		public static bool StyleAbility(Type type)
		{
			return ( type != Type.None      &&
					 type != Type.Name      &&
					 type != Type.Shadow    &&
					 type != Type.ModColor  &&
					 type != Type.PolyClose );
		}

		// Reprend une propriété d'un objet modèle.
		public void PickerProperty(Abstract model)
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

		// Effectue une copie complète de la propriété.
		public void DeepCopyTo(Abstract property)
		{
			this.CopyTo(property);

			this.owners.CopyTo(property.owners);
			property.isSelected     = this.isSelected;
			property.isMulti        = this.isMulti;
			property.isExtendedSize = this.isExtendedSize;
		}

		// Effectue une copie de la propriété.
		public virtual void CopyTo(Abstract property)
		{
			property.type      = this.type;
			property.styleName = this.styleName;
			property.isStyle   = this.isStyle;
		}

		// Détermine si Compare doit tenir compte du nom du style.
		public bool IsCompareStyleName
		{
			get { return this.isCompareStyleName; }
			set { this.isCompareStyleName = value; }
		}

		// Compare deux propriétés.
		// Il ne faut surtout pas comparer isStyle, car les styles et les
		// propriétés sont dans des listes différentes.
		public virtual bool Compare(Abstract property)
		{
			if ( property.type != this.type )  return false;
			if ( this.isCompareStyleName && property.styleName != this.styleName )  return false;
			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public virtual Panels.Abstract CreatePanel(Document document)
		{
			return null;
		}


		// Nombre de poignées.
		public virtual int TotalHandle(Objects.Abstract obj)
		{
			return 0;
		}

		// Indique si une poignée est visible.
		public virtual bool IsHandleVisible(Objects.Abstract obj, int rank)
		{
			return false;
		}

		// Retourne la position d'une poignée.
		public virtual Point GetHandlePosition(Objects.Abstract obj, int rank)
		{
			return new Point();
		}
		
		// Modifie la position d'une poignée.
		public virtual void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			this.document.Notifier.NotifyPropertyChanged(this);
		}
		
		// Dessine les traits de construction avant les poignées.
		public virtual void DrawEdit(Graphics graphics, DrawingContext drawingContext, Objects.Abstract obj)
		{
		}


		// Initialise le zoom par défaut d'un chemin.
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


		#region Notify
		// Signale que tous les propriétaires vont changer.
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
						// Normalement, un propriétaire est un Objects.Abstract.
						// Mais une propriété "isMulti" contient une liste de propriétaires
						// de type Abstract !
						Abstract realProp = this.owners[i] as Abstract;
						realProp.NotifyBefore(oplet);
					}
				}
				else
				{
					if ( this.AlterBoundingBox )
					{
						Objects.Abstract obj = this.owners[i] as Objects.Abstract;
						this.document.Notifier.NotifyArea(obj.BoundingBox);
					}
				}
			}
		}

		// Signale que tous les propriétaires ont changé.
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
						Abstract realProp = this.owners[i] as Abstract;
						this.CopyTo(realProp);
						realProp.NotifyAfter(oplet);
					}
				}
				else
				{
					Objects.Abstract obj = this.owners[i] as Objects.Abstract;
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

			this.document.IsDirtySerialize = true;
		}
		#endregion


		#region IComparable Members
		public int CompareTo(object obj)
		{
			if ( obj is Abstract )
			{
				Abstract property = obj as Abstract;
				int order1 = Abstract.SortOrder(this.type);
				int order2 = Abstract.SortOrder(property.type);
				int eq = order1.CompareTo(order2);
				if ( eq != 0 )  return eq;

				if ( this.owners.Count == 0 || property.owners.Count == 0 )  return eq;

				// Attention: lors d'une sélection de plusieurs objets ayant des
				// propriétés différentes, les propriétaires de la propriété ne sont
				// pas des Objects.Abstract, mais des Abstract (isMulti).
				Objects.Abstract obj1 = this.owners[0] as Objects.Abstract;
				int id1 = -1;  if ( obj1 != null )  id1 = obj1.UniqueId;

				Objects.Abstract obj2 = property.owners[0] as Objects.Abstract;
				int id2 = -1;  if ( obj2 != null )  id2 = obj2.UniqueId;

				return id1.CompareTo(id2);
			}
			throw new System.ArgumentException("object is not a Abstract");
		}
		#endregion


		#region OpletProperty
		// Ajoute un oplet pour mémoriser la propriété.
		protected void InsertOpletProperty()
		{
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletProperty oplet = new OpletProperty(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		// Mémorise toute la propriété.
		protected class OpletProperty : AbstractOplet
		{
			public OpletProperty(Abstract host)
			{
				this.host = host;

				this.copy = Abstract.NewProperty(this.host.document, this.host.type);
				this.host.DeepCopyTo(this.copy);
			}

			public Abstract Property
			{
				get { return this.host; }
			}

			protected void Swap()
			{
				this.host.NotifyBefore(false);

				Abstract temp = Abstract.NewProperty(this.host.document, this.host.type);
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

			protected Abstract				host;
			protected Abstract				copy;
		}
		#endregion

		
		#region Serialization
		// Sérialise la propriété.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Type", this.type);
			info.AddValue("IsStyle", this.isStyle);
			if ( this.isStyle )
			{
				info.AddValue("StyleName", this.styleName);
			}
		}

		// Constructeur qui désérialise la propriété.
		protected Abstract(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;
			this.type = (Type) info.GetValue("Type", typeof(Type));
			this.isStyle = info.GetBoolean("IsStyle");
			if ( this.isStyle )
			{
				this.styleName = info.GetString("StyleName");
			}
			this.owners = new UndoableList(this.document, UndoableListType.ObjectsInsideProperty);
		}
		#endregion


		protected Document						document;
		protected Type							type = Type.None;
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
