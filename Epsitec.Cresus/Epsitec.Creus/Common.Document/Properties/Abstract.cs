using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	//	ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	//	sous peine de plantée lors de la désérialisation.
	public enum Type
	{
		None            = 0,		// aucune
		Name            = 1,		// nom de l'objet
		LineColor       = 2,		// couleur du trait
		LineMode        = 3,		// mode du trait
		Arrow           = 4,		// extrémité des segments
		FillGradient    = 5,		// dégradé de remplissage
		BackColor       = 6,		// texte: couleur de fond (plus utilisé)
		Shadow          = 7,		// ombre sous l'objet
		PolyClose       = 8,		// mode de fermeture des polygones
		Corner          = 9,		// coins des rectangles
		Regular         = 10,		// définitions du polygone régulier
		Arc             = 11,		// arc de cercle ou d'ellipse
		TextFont        = 12,		// texte: police
		TextJustif      = 13,		// texte: justification
		TextLine        = 14,		// texte: justification
		Image           = 15,		// nom de l'image bitmap
		ModColor        = 16,		// modification de couleur
		Surface         = 17,		// surface 2d
		Volume          = 18,		// volume 3d
		FillGradientVT  = 19,		// dégradé de remplissage volume top
		FillGradientVL  = 20,		// dégradé de remplissage volume left
		FillGradientVR  = 21,		// dégradé de remplissage volume right
		DimensionArrow  = 22,		// extrémité des cotes
		Dimension       = 23,		// cotes
		LineDimension   = 24,		// lignes secondaires d'une cote
		Tension         = 25,		// tension d'un trait à main levée
		Frame           = 26,		// cadre

		//	Les propriétés additionnelles ne sont jamais sérialisées.
		//	Elles sont créées à la volée en cas de besoin, et jamais détruites.
		FrameStroke     = 100,		// propriété additionnelle: trait du cadre
		FrameSurface    = 101,		// propriété additionnelle: surface du cadre
		FrameBackground = 102,		// propriété additionnelle: fond du cadre
		FrameShadow     = 103,		// propriété additionnelle: ombre du cadre
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
			this.Initialize();
		}

		protected virtual void Initialize()
		{
		}

		static public Abstract NewProperty(Document document, Type type)
		{
			//	Crée une nouvelle propriété.
			Abstract property = null;
			switch ( type )
			{
				case Type.Name:             property = new Name(document, type);      break;
				case Type.LineColor:        property = new Gradient(document, type);  break;
				case Type.LineMode:         property = new Line(document, type);      break;
				case Type.LineDimension:    property = new Line(document, type);      break;
				case Type.Arrow:            property = new Arrow(document, type);     break;
				case Type.DimensionArrow:   property = new Arrow(document, type);     break;
				case Type.Dimension:        property = new Dimension(document, type); break;
				case Type.FillGradient:     property = new Gradient(document, type);  break;
				case Type.FillGradientVT:   property = new Gradient(document, type);  break;
				case Type.FillGradientVL:   property = new Gradient(document, type);  break;
				case Type.FillGradientVR:   property = new Gradient(document, type);  break;
				case Type.BackColor:        property = new Color(document, type);     break;
				case Type.Shadow:           property = new Shadow(document, type);    break;
				case Type.PolyClose:        property = new Bool(document, type);      break;
				case Type.Corner:           property = new Corner(document, type);    break;
				case Type.Regular:          property = new Regular(document, type);   break;
				case Type.Tension:          property = new Tension(document, type);   break;
				case Type.Arc:              property = new Arc(document, type);       break;
				case Type.Surface:          property = new Surface(document, type);   break;
				case Type.Volume:           property = new Volume(document, type);    break;
				case Type.Frame:            property = new Frame(document, type);     break;
				case Type.FrameStroke:      property = new Line(document, type);      break;
				case Type.FrameSurface:     property = new Gradient(document, type);  break;
				case Type.FrameBackground:  property = new Gradient(document, type);  break;
				case Type.FrameShadow:      property = new Gradient(document, type);  break;
				case Type.TextFont:         property = new Font(document, type);      break;
				case Type.TextJustif:       property = new Justif(document, type);    break;
				case Type.TextLine:         property = new TextLine(document, type);  break;
				case Type.Image:            property = new Image(document, type);     break;
				case Type.ModColor:         property = new ModColor(document, type);  break;
			}
			return property;
		}

		static public string TypeName(Type type)
		{
			//	Retourne le nom d'un type de propriété.
			return type.ToString();
		}

		static public Type TypeName(string typeName)
		{
			//	Retourne le type de propriété d'après son nom.
			if ( typeName == null )
			{
				return Type.None;
			}

			try
			{
				return (Type) System.Enum.Parse(typeof(Type), typeName);
			}
			catch
			{
				return Type.None;
			}
		}

		static public int SortOrder(Type type)
		{
			//	Retourne un numéro d'ordre pour le tri.
			//	Ceci détermine l'ordre d'empilement vertical dans le panneau des propriétés.
			switch ( type )
			{
				case Type.Name:            return 1;
				case Type.LineMode:        return 2;
				case Type.LineDimension:   return 3;
				case Type.Arrow:           return 4;
				case Type.DimensionArrow:  return 5;
				case Type.LineColor:       return 6;
				case Type.FillGradient:    return 7;
				case Type.FillGradientVT:  return 8;
				case Type.FillGradientVL:  return 9;
				case Type.FillGradientVR:  return 10;
				case Type.BackColor:       return 11;
				case Type.Shadow:          return 12;
				case Type.PolyClose:       return 13;
				case Type.Frame:           return 14;
				case Type.Corner:          return 15;
				case Type.Regular:         return 16;
				case Type.Arc:             return 17;
				case Type.Surface:         return 18;
				case Type.Volume:          return 19;
				case Type.TextFont:        return 20;
				case Type.TextJustif:      return 21;
				case Type.TextLine:        return 22;
				case Type.Image:           return 23;
				case Type.ModColor:        return 24;
				case Type.Dimension:       return 25;
				case Type.Tension:         return 26;
			}
			return 0;
		}

		static public Type SortOrder(int index)
		{
			//	Retourne le type selon un numéro d'ordre de tri.
			foreach ( int value in System.Enum.GetValues(typeof(Type)) )
			{
				Type type = (Type) value;
				if ( index == Abstract.SortOrder(type) )  return type;
			}
			return Type.None;
		}

		public UndoableList Owners
		{
			//	Liste des propriétaires. Normalement, un propriétaire est un Objects.Abstract.
			//	Mais une propriété "isMulti" contient une liste de propriétaires de type
			//	Abstract !
			get
			{
				return this.owners;
			}
		}

		public Type Type
		{
			//	Type de la propriété.
			get
			{
				return this.type;
			}
		}

		public virtual Objects.HandleType GetHandleType(int rank)
		{
			return Objects.HandleType.Property;
		}

		public bool IsStrokingGradient
		{
			//	Indique s'il s'agit d'une propriété de surface pour un trait.
			get
			{
				return ( this.type == Type.LineColor );
			}
		}

		public bool IsOnlyForCreation
		{
			//	Indique si la propriété ne sert qu'à créer de nouveaux objets.
			get
			{
				return this.isOnlyForCreation;
			}
			
			set
			{
				this.isOnlyForCreation = value;
			}
		}

		public bool IsStyle
		{
			//	Indique si la propriété est un style.
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

		public bool IsFloating
		{
			//	Indique si la propriété est flottante.
			//	Une propriété flottante n'est référencée par personne et elle n'est pas
			//	dans la liste des propriétés du document. ObjectPoly crée un ObjectLine
			//	avec des propriétés flottantes, pendant la création.
			get
			{
				return this.isFloating;
			}
			
			set
			{
				this.isFloating = value;
			}
		}

		public string OldStyleName
		{
			//	Retourne le nom de l'ancien style.
			get
			{
				return this.oldStyleName;
			}
		}

		public static double BackgroundIntensity(Type type)
		{
			//	Intensité pour le fond du panneau.
			switch ( type )
			{
				case Type.Name:            return 0.70;
				case Type.LineColor:       return 0.85;
				case Type.LineMode:        return 0.85;
				case Type.LineDimension:   return 0.85;
				case Type.Arrow:           return 0.85;
				case Type.DimensionArrow:  return 0.85;
				case Type.Dimension:       return 0.85;
				case Type.FillGradient:    return 0.95;
				case Type.FillGradientVT:  return 0.95;
				case Type.FillGradientVL:  return 0.95;
				case Type.FillGradientVR:  return 0.95;
				case Type.BackColor:       return 0.95;
				case Type.Shadow:          return 0.80;
				case Type.PolyClose:       return 0.90;
				case Type.Corner:          return 0.90;
				case Type.Regular:         return 0.90;
				case Type.Arc:             return 0.90;
				case Type.Surface:         return 0.90;
				case Type.Volume:          return 0.90;
				case Type.TextFont:        return 0.80;
				case Type.TextJustif:      return 0.80;
				case Type.TextLine:        return 0.80;
				case Type.Image:           return 0.90;
				case Type.ModColor:        return 0.95;
				case Type.Tension:         return 0.90;
				case Type.Frame:           return 0.90;
			}
			return 0.0;
		}

		public static string Text(Type type)
		{
			//	Nom de la propriété.
			switch ( type )
			{
				case Type.Name:            return Res.Strings.Property.Abstract.Name;
				case Type.LineColor:       return Res.Strings.Property.Abstract.LineColor;
				case Type.LineMode:        return Res.Strings.Property.Abstract.LineMode;
				case Type.LineDimension:   return Res.Strings.Property.Abstract.LineDimension;
				case Type.FillGradient:    return Res.Strings.Property.Abstract.FillGradient;
				case Type.FillGradientVT:  return Res.Strings.Property.Abstract.FillGradientVT;
				case Type.FillGradientVL:  return Res.Strings.Property.Abstract.FillGradientVL;
				case Type.FillGradientVR:  return Res.Strings.Property.Abstract.FillGradientVR;
				case Type.Shadow:          return Res.Strings.Property.Abstract.Shadow;
				case Type.PolyClose:       return Res.Strings.Property.Abstract.PolyClose;
				case Type.Arrow:           return Res.Strings.Property.Abstract.Arrow;
				case Type.DimensionArrow:  return Res.Strings.Property.Abstract.DimensionArrow;
				case Type.Dimension:       return Res.Strings.Property.Abstract.Dimension;
				case Type.Corner:          return Res.Strings.Property.Abstract.Corner;
				case Type.Regular:         return Res.Strings.Property.Abstract.Regular;
				case Type.Tension:         return Res.Strings.Property.Abstract.Tension;
				case Type.Arc:             return Res.Strings.Property.Abstract.Arc;
				case Type.Surface:         return Res.Strings.Property.Abstract.Surface;
				case Type.Volume:          return Res.Strings.Property.Abstract.Volume;
				case Type.BackColor:       return Res.Strings.Property.Abstract.BackColor;
				case Type.TextFont:        return Res.Strings.Property.Abstract.TextFont;
				case Type.TextJustif:      return Res.Strings.Property.Abstract.TextJustif;
				case Type.TextLine:        return Res.Strings.Property.Abstract.TextLine;
				case Type.Image:           return Res.Strings.Property.Abstract.Image;
				case Type.ModColor:        return Res.Strings.Property.Abstract.ModColor;
				case Type.Frame:           return Res.Strings.Property.Abstract.Frame;
			}
			return "";
		}

		public static string IconText(Type type)
		{
			//	Nom de l'icône de la propriété.
			switch ( type )
			{
				case Type.Name:            return "PropertyName";
				case Type.LineColor:       return "PropertyLineColor";
				case Type.LineMode:        return "PropertyLineMode";
				case Type.LineDimension:   return "PropertyLineDimension";
				case Type.FillGradient:    return "PropertyFillGradient";
				case Type.FillGradientVT:  return "PropertyFillGradientVT";
				case Type.FillGradientVL:  return "PropertyFillGradientVL";
				case Type.FillGradientVR:  return "PropertyFillGradientVR";
				case Type.Shadow:          return "PropertyShadow";
				case Type.PolyClose:       return "PropertyPolyClose";
				case Type.Arrow:           return "PropertyArrow";
				case Type.DimensionArrow:  return "PropertyDimensionArrow";
				case Type.Dimension:       return "PropertyDimension";
				case Type.Corner:          return "PropertyCorner";
				case Type.Regular:         return "PropertyRegular";
				case Type.Tension:         return "PropertyTension";
				case Type.Arc:             return "PropertyArc";
				case Type.Surface:         return "PropertySurface";
				case Type.Volume:          return "PropertyVolume";
				case Type.BackColor:       return "PropertyBackColor";
				case Type.TextFont:        return "PropertyTextFont";
				case Type.TextJustif:      return "PropertyTextJustif";
				case Type.TextLine:        return "PropertyTextLine";
				case Type.Image:           return "PropertyImage";
				case Type.ModColor:        return "PropertyModColor";
				case Type.Frame:           return "PropertyFrame";
			}
			return "";
		}

		public virtual string SampleText
		{
			//	Donne le petit texte pour les échantillons.
			get
			{
				return "";
			}
		}

		public string TextStyle
		{
			//	Nom de la propriété ou du style si c'en est un.
			get
			{
				return Abstract.Text(this.type);
			}
		}

		#region StyleBrief
		public virtual void PutStyleBrief(System.Text.StringBuilder builder)
		{
			//	Construit le texte résumé d'un style pour une propriété.
			this.PutStyleBriefPrefix(builder);
			this.PutStyleBriefPostfix(builder);
		}

		protected void PutStyleBriefPrefix(System.Text.StringBuilder builder)
		{
			builder.Append(Misc.Image(Abstract.IconText(this.type)));
			builder.Append("  ");
		}

		protected void PutStyleBriefPostfix(System.Text.StringBuilder builder)
		{
			builder.Append("<br/>");
		}
		#endregion

		public bool IsSelected
		{
			//	Indique si la propriété est utilisée par un objet sélectionnée.
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

		public bool IsExtendedSize
		{
			//	Mode de déploiement du panneau associé.
			get
			{
				return this.isExtendedSize;
			}
			
			set
			{
				this.isExtendedSize = value;
			}
		}

		public bool IsMulti
		{
			//	Représentation de plusieurs propriétés contradictoires.
			get
			{
				return this.isMulti;
			}
			
			set
			{
				this.isMulti = value;
			}
		}

		public virtual bool IsComplexPrinting
		{
			//	Indique si une impression complexe est nécessaire.
			get
			{
				return false;
			}
		}


		public virtual bool AlterBoundingBox
		{
			//	Indique si un changement de cette propriété modifie la bbox de l'objet.
			get { return false; }
		}

		public virtual void InflateBoundingBox(SurfaceAnchor sa, ref Rectangle bboxFull)
		{
			//	Engraisse la bbox en fonction de la propriété.
		}

		public virtual bool IsVisible(IPaintPort port)
		{
			//	Indique si la propriété est visible.
			return true;
		}


		public static bool StyleAbility(Type type)
		{
			//	Indique si cette propriété peut faire l'objet d'un style.
			return ( type != Type.None       &&
					 type != Type.Name       &&
					 type != Type.Shadow     &&
					 type != Type.BackColor  &&
					 type != Type.ModColor   &&
					 type != Type.PolyClose  &&
					 type != Type.TextJustif &&
					 type != Type.TextLine   );
		}

		public void PickerProperty(Abstract model)
		{
			//	Reprend une propriété d'un objet modèle.
			System.Diagnostics.Debug.Assert(this.isStyle);
			System.Diagnostics.Debug.Assert(this.Type == model.Type);
			this.NotifyBefore();
			model.CopyTo(this);
			this.isStyle = true;
			this.NotifyAfter();
			this.document.Notifier.NotifyPropertyChanged(this);
		}

		public void DeepCopyTo(Abstract property)
		{
			//	Effectue une copie complète de la propriété.
			this.CopyTo(property);

			this.owners.CopyTo(property.owners);
			property.isSelected     = this.isSelected;
			property.isMulti        = this.isMulti;
			property.isExtendedSize = this.isExtendedSize;
		}

		public virtual void CopyTo(Abstract property)
		{
			//	Effectue une copie de la propriété.
			property.type    = this.type;
			property.isStyle = this.isStyle;
		}

		public virtual bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			//	Il ne faut surtout pas comparer isStyle, car les styles et les
			//	propriétés sont dans des listes différentes.
			if ( property.type != this.type )  return false;
			return true;
		}

		public virtual Panels.Abstract CreatePanel(Document document)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			return null;
		}


		public virtual void MoveHandleStarting(Objects.Abstract obj, int rank, Point pos, DrawingContext drawingContext)
		{
			//	Début du déplacement d'une poignée.
		}

		public virtual int TotalHandle(Objects.Abstract obj)
		{
			//	Nombre de poignées.
			return 0;
		}

		public virtual bool IsHandleVisible(Objects.Abstract obj, int rank)
		{
			//	Indique si une poignée est visible.
			return false;
		}

		public virtual Point GetHandlePosition(Objects.Abstract obj, int rank)
		{
			//	Retourne la position d'une poignée.
			return new Point();
		}
		
		public virtual void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			//	Modifie la position d'une poignée.
			//	Si la position est hors limite, les méthodes override n'auront pas
			//	modifié la propriété. Donc, le NotifyAfter qui s'occupe d'appeler
			//	HandlePropertiesUpdate n'aura pas été appelé. C'est pourquoi il
			//	est appelé ici encore une fois, afin d'être certain qu'il soit
			//	exécuté au moins une fois !
			obj.HandlePropertiesUpdate();

			this.document.Notifier.NotifyPropertyChanged(this);
		}

		public virtual void MoveGlobalStarting()
		{
			//	Début du déplacement global de la propriété.
		}
		
		public virtual void MoveGlobalProcess(Selector selector)
		{
			//	Effectue le déplacement global de la propriété.
		}
		
		public virtual void DrawEdit(Graphics graphics, DrawingContext drawingContext, Objects.Abstract obj)
		{
			//	Dessine les traits de construction avant les poignées.
		}


		public virtual void OpenOrClosePopupInterface(Objects.Abstract obj)
		{
		}

		public virtual void ClosePopupInterface(Objects.Abstract obj)
		{
		}


		static public double DefaultZoom(DrawingContext drawingContext)
		{
			//	Initialise le zoom par défaut d'un chemin.
			if ( drawingContext == null )
			{
				return 2.0;
			}
			else
			{
				return drawingContext.ScaleX;
			}
		}


		public virtual bool IsSmoothSurfacePDF(IPaintPort port)
		{
			//	Indique si la surface PDF est floue.
			return false;
		}
		
		public virtual PDF.Type TypeComplexSurfacePDF(IPaintPort port)
		{
			//	Donne le type PDF de la surface complexe.
			return PDF.Type.None;
		}


		public virtual bool ChangeColorSpace(ColorSpace cs)
		{
			//	Modifie l'espace des couleurs.
			return false;
		}

		public virtual bool ChangeColor(double adjust, bool stroke)
		{
			//	Modifie les couleurs.
			return false;
		}


		#region Notify
		protected void NotifyBefore()
		{
			//	Signale que tous les propriétaires vont changer.
			this.NotifyBefore(true);
		}
		
		protected virtual void NotifyBefore(bool oplet)
		{
			if ( this.isOnlyForCreation )  return;
			if ( !oplet )  return;

			this.InsertOpletProperty();

			int total = this.owners.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				if ( this.isMulti )
				{
					//	Normalement, un propriétaire est un Objects.Abstract.
					//	Mais une propriété "isMulti" contient une liste de propriétaires
					//	de type Abstract !
					Abstract realProp = this.owners[i] as Abstract;
					realProp.NotifyBefore(oplet);
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

		protected void NotifyAfter()
		{
			//	Signale que tous les propriétaires ont changé.
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

					if ( oplet )
					{
						obj.HandlePropertiesUpdate();

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
					else
					{
						// Il ne faut surtout pas recalculer les bbox ici, car le nombre de recalculs
						// serait beaucoup trop important. Si deux objets A et B utilisent la même
						// propriété P, le NotifyAfter fait pour la propriété de l'objet A va demander
						// de recalculer les bbox des objets l'utilisant, soit A et B. De même pour
						// l'objet B !
						// A la place, le Modifier.AccumulateObject garde la trace de tous les objets
						// modifiés, pour les traiter globalement en une seule fois, lors du
						// Modifier.AccumulateEnding.
						this.document.Modifier.AccumulateObject(obj);
					}
				}
			}

			this.document.SetDirtySerialize(this.isStyle ? CacheBitmapChanging.All : CacheBitmapChanging.Local);
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

				//	Attention: lors d'une sélection de plusieurs objets ayant des
				//	propriétés différentes, les propriétaires de la propriété ne sont
				//	pas des Objects.Abstract, mais des Abstract (isMulti).
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
		protected void InsertOpletProperty()
		{
			//	Ajoute un oplet pour mémoriser la propriété.
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletProperty oplet = new OpletProperty(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		//	Mémorise toute la propriété.
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
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise la propriété.
			info.AddValue("Type", this.type);
			info.AddValue("IsStyle", this.isStyle);
		}

		protected Abstract(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui désérialise la propriété.
			this.document = Document.ReadDocument;
			this.Initialize();
			this.type = (Type) info.GetValue("Type", typeof(Type));
			this.isStyle = info.GetBoolean("IsStyle");
			if ( this.isStyle && !this.document.IsRevisionGreaterOrEqual(1,0,24) )
			{
				this.oldStyleName = info.GetString("StyleName");
			}
			this.owners = new UndoableList(this.document, UndoableListType.ObjectsInsideProperty);
		}
		#endregion


		protected Document						document;
		protected Type							type = Type.None;
		protected UndoableList					owners;
		protected string						oldStyleName = "";
		protected bool							isOnlyForCreation = false;
		protected bool							isStyle = false;
		protected bool							isFloating = false;
		protected bool							isMulti = false;
		protected bool							isExtendedSize = false;
		protected bool							isSelected = false;
	}
}
