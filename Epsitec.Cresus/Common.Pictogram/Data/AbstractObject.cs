using Epsitec.Common.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe AbstractObject est la classe de base des objets graphiques.
	/// </summary>
	public abstract class AbstractObject
	{
		public AbstractObject()
		{
		}

		public virtual void Dispose()
		{
		}

		[XmlArrayItem("Name",     Type=typeof(PropertyName))]
		[XmlArrayItem("Bool",     Type=typeof(PropertyBool))]
		[XmlArrayItem("Color",    Type=typeof(PropertyColor))]
		[XmlArrayItem("Double",   Type=typeof(PropertyDouble))]
		[XmlArrayItem("Gradient", Type=typeof(PropertyGradient))]
		[XmlArrayItem("Shadow",   Type=typeof(PropertyShadow))]
		[XmlArrayItem("Line",     Type=typeof(PropertyLine))]
		[XmlArrayItem("List",     Type=typeof(PropertyList))]
		[XmlArrayItem("Combo",    Type=typeof(PropertyCombo))]
		[XmlArrayItem("Font",     Type=typeof(PropertyFont))]
		[XmlArrayItem("Justif",   Type=typeof(PropertyJustif))]
		[XmlArrayItem("TextLine", Type=typeof(PropertyTextLine))]
		[XmlArrayItem("String",   Type=typeof(PropertyString))]
		[XmlArrayItem("Arrow",    Type=typeof(PropertyArrow))]
		[XmlArrayItem("Corner",   Type=typeof(PropertyCorner))]
		[XmlArrayItem("Regular",  Type=typeof(PropertyRegular))]
		[XmlArrayItem("Image",    Type=typeof(PropertyImage))]
		[XmlArrayItem("Arc",      Type=typeof(PropertyArc))]
		[XmlArrayItem("ModColor", Type=typeof(PropertyModColor))]
		public System.Collections.ArrayList Properties
		{
			get { return this.properties; }
			set { this.properties = value; }
		}

		[XmlArrayItem(Type=typeof(Handle))]
		public System.Collections.ArrayList Handles
		{
			get { return this.handles; }
			set { this.handles = value; }
		}

		[XmlArrayItem("Bezier",    Type=typeof(ObjectBezier))]
		[XmlArrayItem("Circle",    Type=typeof(ObjectCircle))]
		[XmlArrayItem("Ellipse",   Type=typeof(ObjectEllipse))]
		[XmlArrayItem("Group",     Type=typeof(ObjectGroup))]
		[XmlArrayItem("Layer",     Type=typeof(ObjectLayer))]
		[XmlArrayItem("Line",      Type=typeof(ObjectLine))]
		[XmlArrayItem("Page",      Type=typeof(ObjectPage))]
		[XmlArrayItem("Polyline",  Type=typeof(ObjectPoly))]
		[XmlArrayItem("Rectangle", Type=typeof(ObjectRectangle))]
		[XmlArrayItem("Polygon",   Type=typeof(ObjectRegular))]
		[XmlArrayItem("TextLine",  Type=typeof(ObjectTextLine))]
		[XmlArrayItem("TextBox",   Type=typeof(ObjectTextBox))]
		[XmlArrayItem("Array",     Type=typeof(ObjectArray))]
		[XmlArrayItem("Image",     Type=typeof(ObjectImage))]
		public UndoList Objects
		{
			get { return this.objects; }
			set { this.objects = value; }
		}


		public virtual string IconName
		{
			//	Nom de l'icône.
			get { return @""; }
		}


		public int TotalHandle
		{
			//	Nombre de poignées, sans compter celles des propriétés.
			get { return this.handles.Count; }
		}

		public int TotalHandleProperties
		{
			//	Nombre de poignées, avec celles des propriétés.
			get
			{
				int total = 0;
				foreach ( AbstractProperty property in this.properties )
				{
					total += property.TotalHandle;
				}
				return this.handles.Count + total;
			}
		}

		public void HandleAdd(Drawing.Point pos, HandleType type)
		{
			//	Ajoute une poignée.
			Handle handle = new Handle();
			handle.Position = pos;
			handle.Type = type;
			this.handles.Add(handle);
			this.dirtyBbox = true;
		}

		public void HandleInsert(int rank, Handle handle)
		{
			//	Insère une poignée.
			this.handles.Insert(rank, handle);
			this.dirtyBbox = true;
		}

		public void HandleDelete(int rank)
		{
			//	Supprime une poignée.
			this.handles.RemoveAt(rank);
			this.dirtyBbox = true;
		}

		public Handle Handle(int rank)
		{
			//	Donne une poignée de l'objet.
			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				System.Diagnostics.Debug.Assert(this.handles[rank] != null);
				return this.handles[rank] as Handle;
			}
			else	// poignée d'une propriété ?
			{
				rank -= this.handles.Count;
				int index = 0;
				foreach ( AbstractProperty property in this.properties )
				{
					int count = property.TotalHandle;
					if ( rank >= index && rank < index+count )
					{
						return property.Handle(rank-index, this.bboxThin);
					}
					index += count;
				}
				return null;
			}
		}

		public virtual int DetectHandle(Drawing.Point pos)
		{
			//	Détecte la poignée pointée par la souris.
			int total = this.TotalHandleProperties;
			//for ( int i=0 ; i<total ; i++ )
			for ( int i=total-1 ; i>=0 ; i-- )
			{
				if ( this.Handle(i).Detect(pos) )  return i;
			}
			return -1;
		}

		public virtual void MoveHandleStarting(int rank, Drawing.Point pos, IconContext iconContext)
		{
			//	Début du déplacement d'une poignée.
			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				iconContext.ConstrainFixStarting(pos);
			}
			else	// poignée d'une propriété ?
			{
				rank -= this.handles.Count;
				int index = 0;
				foreach ( AbstractProperty property in this.properties )
				{
					int count = property.TotalHandle;
					if ( rank >= index && rank < index+count )
					{
						property.MoveHandleStarting(rank-index, pos, this.bboxThin, iconContext);
						break;
					}
					index += count;
				}
			}
		}

		public virtual void MoveHandleProcess(int rank, Drawing.Point pos, IconContext iconContext)
		{
			//	Déplace une poignée.
			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				iconContext.ConstrainSnapPos(ref pos);
				iconContext.SnapGrid(ref pos);
				this.Handle(rank).Position = pos;
				this.dirtyBbox = true;
			}
			else	// poignée d'une propriété ?
			{
				rank -= this.handles.Count;
				int index = 0;
				foreach ( AbstractProperty property in this.properties )
				{
					int count = property.TotalHandle;
					if ( rank >= index && rank < index+count )
					{
						property.MoveHandleProcess(rank-index, pos, this.bboxThin, iconContext);
						break;
					}
					index += count;
				}
				this.dirtyBbox = true;
			}
		}

		protected void MoveCorner(Drawing.Point pc, int corner, int left, int right, int opposite)
		{
			//	Déplace un coin tout en conservant une forme rectangulaire.
			if ( AbstractObject.IsRectangular(this.Handle(0).Position, this.Handle(1).Position, this.Handle(2).Position, this.Handle(3).Position) )
			{
				this.Handle(corner).Position = pc;

				Drawing.Point pl = this.Handle(left).Position;
				Drawing.Point pr = this.Handle(right).Position;
				Drawing.Point po = this.Handle(opposite).Position;

				if ( pl == pr )
				{
					this.Handle(left ).Position = new Drawing.Point(pc.X, po.Y);
					this.Handle(right).Position = new Drawing.Point(po.X, pc.Y);
				}
				else if ( pl == po )
				{
					this.Handle(right).Position = Drawing.Point.Projection(pr, po, pc);
					this.Handle(left ).Position = po+(pc-this.Handle(right).Position);
				}
				else if ( pr == po )
				{
					this.Handle(left ).Position = Drawing.Point.Projection(pl, po, pc);
					this.Handle(right).Position = po+(pc-this.Handle(left).Position);
				}
				else
				{
					this.Handle(left ).Position = Drawing.Point.Projection(pl, po, pc);
					this.Handle(right).Position = Drawing.Point.Projection(pr, po, pc);
				}
			}
			else
			{
				this.Handle(corner).Position = pc;
			}
		}

		public virtual bool IsMoveHandlePropertyChanged(int rank)
		{
			//	Indique si le déplacement d'une poignée doit se répercuter sur les propriétés.
			return false;
		}

		public virtual AbstractProperty MoveHandleProperty(int rank)
		{
			//	Retourne la propriété modifiée en déplaçant une poignée.
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				rank -= this.handles.Count;
				int index = 0;
				foreach ( AbstractProperty property in this.properties )
				{
					int count = property.TotalHandle;
					if ( rank >= index && rank < index+count )
					{
						return property;
					}
					index += count;
				}
			}
			return null;
		}

		public virtual void MoveAll(Drawing.Point move, bool all)
		{
			//	Déplace tout l'objet.
			foreach ( Handle handle in this.handles )
			{
				if ( all || handle.IsSelected )
				{
					handle.Position += move;
				}
			}
			this.dirtyBbox = true;
		}


		public virtual void MoveGlobal(GlobalModifierData initial, GlobalModifierData final, bool all)
		{
			//	Déplace globalement l'objet.
			foreach ( Handle handle in this.handles )
			{
				if ( all || handle.IsSelected )
				{
					handle.Position = GlobalModifierData.Transform(initial, final, handle.Position);
				}
			}
			this.dirtyBbox = true;
		}


		public virtual bool EditProcessMessage(Message message, Drawing.Point pos)
		{
			//	Gestion d'un événement pendant l'édition.
			return false;
		}

		public virtual void EditMouseDownMessage(Drawing.Point pos)
		{
			//	Gestion d'un événement pendant l'édition.
		}

		public virtual int DetectCell(Drawing.Point pos)
		{
			//	Détecte la cellule pointée par la souris.
			return -1;
		}

		public virtual void MoveCellStarting(int rank, Drawing.Point pos,
											 bool isShift, bool isCtrl, int downCount,
											 IconContext iconContext)
		{
			//	Début du déplacement d'une cellule.
		}

		public virtual void MoveCellProcess(int rank, Drawing.Point pos,
											bool isShift, bool isCtrl,
											IconContext iconContext)
		{
			//	Déplace une cellule.
		}


		public void DeletePattern(int rank)
		{
			//	Adapte l'objet après la supression d'un pattern.
			PropertyLine line = this.SearchProperty(PropertyType.LineMode) as PropertyLine;
			if ( line == null )  return;
			if ( !line.AdaptDeletePattern(rank) )  return;
			this.UpdateBoundingBox();
			this.dirtyBbox = false;
		}

		public void DeletePattern()
		{
			//	Adapte l'objet après la supression de tous les patterns.
			PropertyLine line = this.SearchProperty(PropertyType.LineMode) as PropertyLine;
			if ( line == null )  return;
			if ( !line.AdaptDeletePattern() )  return;
			this.UpdateBoundingBox();
			this.dirtyBbox = false;
		}


		public Drawing.Rectangle BoundingBox
		{
			//	Rectangle englobant l'objet.
			get
			{
				if ( this.IsSelected() )  return this.BoundingBoxFull;
				else                      return this.BoundingBoxGeom;
			}
		}

		public void PatternUpdateBoundingBox(IconObjects iconObjects)
		{
			//	Recalcule la bbox en fonction des patterns.
			PropertyLine line = this.SearchProperty(PropertyType.LineMode) as PropertyLine;
			if ( line == null || line.PatternId == 0 )  return;
			line.PatternBbox = iconObjects.RetPatternBbox(line.PatternId);
			this.UpdateBoundingBox();
			this.dirtyBbox = false;
		}

		public Drawing.Rectangle BoundingBoxThin
		{
			//	Rectangle englobant la géométrie de l'objet, sans tenir compte
			//	de l'épaisseur des traits.
			get
			{
				if ( this.dirtyBbox )  // est-ce que la bbox n'est plus à jour ?
				{
					this.UpdateBoundingBox();  // on la recalcule
					this.dirtyBbox = false;  // elle est de nouveau à jour
				}
				return this.bboxThin;
			}
		}

		public Drawing.Rectangle BoundingBoxGeom
		{
			//	Rectangle englobant la géométrie de l'objet, en tenant compte
			//	de l'épaisseur des traits.
			get
			{
				if ( this.dirtyBbox )  // est-ce que la bbox n'est plus à jour ?
				{
					this.UpdateBoundingBox();  // on la recalcule
					this.dirtyBbox = false;  // elle est de nouveau à jour
				}
				return this.bboxGeom;
			}
		}

		public Drawing.Rectangle BoundingBoxFull
		{
			//	Rectangle englobant complet de l'objet, pendant une sélection.
			get
			{
				if ( this.dirtyBbox )  // est-ce que la bbox n'est plus à jour ?
				{
					this.UpdateBoundingBox();  // on la recalcule
					this.dirtyBbox = false;  // elle est de nouveau à jour
				}
				return this.bboxFull;
			}
		}

		protected virtual void UpdateBoundingBox()
		{
			//	Calcule le rectangle englobant l'objet. Chaque objet se charge de
			//	ce calcul, selon sa géométrie, l'épaisseur de son trait, etc.
			//	Il faut calculer :
			//	this.bboxThin  boîte selon la géométrie de l'objet, sans les traits
			//	this.bboxGeom  boîte selon la géométrie de l'objet, avec les traits
			//	this.bboxFull  boîte complète lorsque l'objet est sélectionné
		}


		[XmlIgnore]
		public bool IsHilite
		{
			//	Etat survolé de l'objet.
			get
			{
				return this.isHilite;
			}

			set
			{
				this.isHilite = value;
			}
		}

		[XmlIgnore]
		public bool IsHide
		{
			//	Etat caché de l'objet.
			get
			{
				return this.isHide;
			}

			set
			{
				this.isHide = value;
			}
		}


		[XmlIgnore]
		public bool UndoStamp
		{
			//	Etat "modifié par le undo" de l'objet.
			get
			{
				return this.undoStamp;
			}

			set
			{
				this.undoStamp = value;
			}
		}


		public void Select()
		{
			//	Sélectionne toutes les poignées de l'objet.
			this.Select(true, false);
		}

		public void Deselect()
		{
			//	Désélectionne toutes les poignées de l'objet.
			this.Select(false, false);
		}

		public void Select(bool select)
		{
			//	Sélectionne ou désélectionne toutes les poignées de l'objet.
			this.Select(select, false);
		}

		public virtual void Select(bool select, bool edit)
		{
			//	Sélectionne ou désélectionne toutes les poignées de l'objet.
			this.selected = select;
			this.edited = edit;
			this.globalSelected = false;

			int total = this.TotalHandleProperties;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				handle.IsSelected = select && !edit;
				handle.IsGlobalSelected = false;
			}
			this.dirtyBbox = true;
		}

		public virtual void Select(Drawing.Rectangle rect)
		{
			//	Sélectionne toutes les poignées de l'objet dans un rectangle.
			int total = this.TotalHandleProperties;
			int sel = 0;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( rect.Contains(handle.Position) )
				{
					handle.IsSelected = true;
					handle.IsGlobalSelected = false;
					sel ++;
				}
				else
				{
					handle.IsSelected = false;
					handle.IsGlobalSelected = false;
				}
			}
			this.selected = ( sel > 0 );
			this.edited = false;
			this.globalSelected = false;
		}

		public void GlobalSelect(bool global)
		{
			//	Indique que l'objet est sélectionné globalement (avec GlobalModifier).
			int total = this.TotalHandleProperties;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				handle.IsGlobalSelected = handle.IsSelected && global;
			}
			this.globalSelected = global;
		}

		public bool IsSelected()
		{
			//	Indique si l'objet est sélectionné.
			return this.selected;
		}

		public bool IsGlobalSelected()
		{
			//	Indique si l'objet est sélectionné globalement (avec GlobalModifier).
			return this.globalSelected;
		}

		protected void GlobalHandleAdapt(int rank)
		{
			//	Adapte une poignée à la sélection globale.
			this.Handle(rank).IsGlobalSelected = this.globalSelected && this.Handle(rank).IsSelected;
		}


		public virtual bool IsEditable()
		{
			//	Indique si l'objet est éditable.
			return false;
		}

		public bool IsEdited()
		{
			//	Indique si l'objet est en cours d'édition.
			return this.edited;
		}

		public virtual bool EditRulerLink(TextRuler ruler, IconContext iconContext)
		{
			//	Lie l'objet éditable à une règle.
			return false;
		}

		
		[XmlIgnore]
		public bool EditProperties
		{
			//	Indique si les propriétés de l'objet sont en cours d'édition.
			get
			{
				return this.editProperties;
			}

			set
			{
				this.editProperties = value;

				foreach ( AbstractProperty property in this.properties )
				{
					property.EditProperties = editProperties;
				}
			}
		}


		public int TotalProperty
		{
			//	Nombre de proriétés.
			get { return this.properties.Count; }
		}

		public bool ExistProperty(int rank)
		{
			//	Indique si une propriété existe.
			return ( rank < this.properties.Count );
		}

		public void AddProperty(AbstractProperty property)
		{
			//	Ajoute une proriété à l'objet.
			this.properties.Add(property);
		}

		public AbstractProperty Property(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as AbstractProperty;
		}

		public virtual void PropertiesList(System.Collections.ArrayList list, bool firstLevel)
		{
			//	Ajoute toutes les propriétés de l'objet dans une liste.
			//	Un type de propriété donné n'est qu'une fois dans la liste.
			foreach ( AbstractProperty property in this.properties )
			{
				if ( !firstLevel && property.Type == PropertyType.Name )  continue;
				this.PropertyAllList(list, property);
			}
		}

		protected void PropertyAllList(System.Collections.ArrayList list, AbstractProperty property)
		{
			//	Ajoute intelligemment une propriété dans la liste.
			AbstractProperty existing = property.Search(list);
			if ( existing == null )  // pas trouvé dans la liste ?
			{
				property.Multi = false;
				list.Add(property);
			}
			else	// déjà dans la liste ?
			{
				if ( !property.Compare(existing) )
				{
					existing.Multi = true;
				}
			}
		}

		public PropertyBool PropertyBool(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyBool;
		}

		public PropertyDouble PropertyDouble(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyDouble;
		}

		public PropertyColor PropertyColor(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyColor;
		}

		public PropertyGradient PropertyGradient(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyGradient;
		}

		public PropertyShadow PropertyShadow(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyShadow;
		}

		public PropertyLine PropertyLine(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyLine;
		}

		public PropertyString PropertyString(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyString;
		}

		public PropertyList PropertyList(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyList;
		}

		public PropertyCombo PropertyCombo(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyCombo;
		}

		public PropertyFont PropertyFont(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyFont;
		}

		public PropertyJustif PropertyJustif(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyJustif;
		}

		public PropertyTextLine PropertyTextLine(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyTextLine;
		}

		public PropertyArrow PropertyArrow(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyArrow;
		}

		public PropertyCorner PropertyCorner(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyCorner;
		}

		public PropertyRegular PropertyRegular(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyRegular;
		}

		public PropertyImage PropertyImage(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyImage;
		}

		public PropertyArc PropertyArc(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyArc;
		}

		public PropertyModColor PropertyModColor(int rank)
		{
			//	Donne une propriété de l'objet.
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyModColor;
		}

		protected virtual AbstractProperty SearchProperty(PropertyType type)
		{
			//	Cherche une propriété d'après son type.
			foreach ( AbstractProperty property in this.properties )
			{
				if ( property.Type == type )  return property;
			}
			return null;
		}

		public virtual bool IsLinkProperty(AbstractProperty property)
		{
			//	Cherche si une propriété est liée à un style.
			if ( property.StyleID == 0 )  return false;
			AbstractProperty actual = this.SearchProperty(property.Type);
			if ( actual == null )  return false;
			return ( actual.StyleID == property.StyleID );
		}

		public void PropertySpread(AbstractProperty property)
		{
			//	Répand une propriété faisant partie d'un style.
			if ( property.StyleID == 0 )  return;
			AbstractProperty actual = this.SearchProperty(property.Type);
			if ( actual == null )  return;
			if ( actual.StyleID != property.StyleID )  return;
			property.CopyTo(actual);
		}

		public virtual AbstractProperty GetProperty(PropertyType type)
		{
			//	Retourne une copie d'une propriété.
			AbstractProperty actual = this.SearchProperty(type);
			if ( actual == null )  return null;

			AbstractProperty copy = AbstractProperty.NewProperty(type);
			if ( copy == null )  return null;
			actual.CopyTo(copy);
			return copy;
		}

		public virtual void SetPropertyBase(AbstractProperty property)
		{
			//	Modifie une propriété.
			AbstractProperty actual = this.SearchProperty(property.Type);
			if ( actual == null )  return;
			property.CopyTo(actual);
		}

		public virtual void SetProperty(AbstractProperty property)
		{
			//	Modifie une propriété.
			AbstractProperty actual = this.SearchProperty(property.Type);
			if ( actual == null )
			{
				if ( this is ObjectGroup )
				{
					this.dirtyBbox = true;  // un objet du groupe peut modifier la bbox !
				}
				return;
			}

			property.CopyTo(actual);

			if ( actual.AlterBoundingBox )
			{
				this.dirtyBbox = true;
			}
		}

		public void SetPropertyExtended(PropertyType type, bool extended)
		{
			//	Mémorise l'état étendu d'une propriété.
			//	Seul Drawer.objectMemory s'occupe de mémoriser cet état.
			AbstractProperty property = this.SearchProperty(type);
			if ( property == null )  return;
			property.ExtendedSize = extended;
		}

		public bool GetPropertyExtended(PropertyType type)
		{
			//	Retourne l'état étendu d'une propriété.
			//	Seul Drawer.objectMemory s'occupe de mémoriser cet état.
			AbstractProperty property = this.SearchProperty(type);
			if ( property == null )  return false;
			return property.ExtendedSize;
		}

		public void SetPropertyExtended(AbstractObject objectMemory)
		{
			//	Initialise l'état étendu selon Drawer.objectMemory a toutes
			//	les propriétés de l'objet.
			foreach ( AbstractProperty property in this.properties )
			{
				AbstractProperty p = objectMemory.SearchProperty(property.Type);
				if ( p == null )  continue;
				property.ExtendedSize = p.ExtendedSize;
			}
		}

		public virtual void CloneProperties(AbstractObject src)
		{
			//	Reprend toutes les propriétés d'un objet source.
			if ( src == null )  return;
			foreach ( AbstractProperty property in this.properties )
			{
				AbstractProperty p = src.SearchProperty(property.Type);
				if ( p == null )  continue;
				p.CopyTo(property);
			}
		}

		public virtual void PasteAdaptStyles(StylesCollection stylesCollection)
		{
			//	Adapte les styles de l'objet collé, qui peut provenir d'un autre fichier,
			//	donc d'une autre collection de styles. On se base sur le nom des styles
			//	(StyleName) pour faire la correspondance.
			//	Si on trouve un nom identique -> le style de l'objet collé est modifié
			//	en fonction du style existant.
			//	Si on ne trouve pas un nom identique -> on crée un nouveau style, en
			//	modifiant bien entendu l'identificateur (StyleID) de l'objet collé.
			int total = this.TotalProperty;
			for ( int i=0 ; i<total ; i++ )
			{
				AbstractProperty property = this.Property(i);
				if ( property == null )  break;

				if ( property.StyleID == 0 )  continue;  // n'utilise pas un style ?

				AbstractProperty style = stylesCollection.SearchProperty(property);
				if ( style == null )
				{
					int rank = stylesCollection.AddProperty(property);
					style = stylesCollection.GetProperty(rank);
				}
				style.CopyTo(property);
			}
		}

		public virtual void PasteAdaptProperties(bool isPatternPossible)
		{
			//	Adapte les propriétés de l'objet collé, pour empêcher d'utiliser un
			//	pattern dans un pattern. On empêche aussi d'utiliser un style dans
			//	un pattern.
			if ( isPatternPossible )  return;

			PropertyLine line = this.SearchProperty(PropertyType.LineMode) as PropertyLine;
			if ( line != null )
			{
				line.PatternId = 0;
			}

			int total = this.TotalProperty;
			for ( int i=0 ; i<total ; i++ )
			{
				AbstractProperty property = this.Property(i);
				if ( property == null )  break;

				property.StyleID = 0;
			}
		}


		public virtual bool Detect(Drawing.Point pos)
		{
			//	Détecte si la souris est sur un objet.
			return false;
		}

		public virtual bool Detect(Drawing.Rectangle rect, bool all)
		{
			//	Détecte si l'objet est dans un rectangle.
			//	all = true  -> toutes les poignées doivent être dans le rectangle
			//	all = false -> une seule poignée doit être dans le rectangle
			if ( this.isHide )  return false;

			if ( all )
			{
				return rect.Contains(this.BoundingBoxGeom);
			}
			else
			{
				foreach ( Handle handle in this.handles )
				{
					if ( handle.Type != HandleType.Primary )  continue;
					if ( rect.Contains(handle.Position) )  return true;
				}
				return false;
			}
		}

		
		public virtual bool DetectEdit(Drawing.Point pos)
		{
			//	Détecte si la souris est sur l'objet pour l'éditer.
			return false;
		}


		public virtual void ContextMenu(System.Collections.ArrayList list, Drawing.Point pos, int handleRank)
		{
			//	Donne le contenu du menu contextuel.
		}

		public virtual void ContextCommand(string cmd, Drawing.Point pos, int handleRank)
		{
			//	Exécute une commande du menu contextuel.
		}


		public virtual void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			//	Début de la création d'un objet.
			iconContext.ConstrainFixStarting(pos);
		}

		public virtual void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			//	Déplacement pendant la création d'un objet.
			this.dirtyBbox = true;
		}

		public virtual void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			//	Fin de la création d'un objet.
			iconContext.ConstrainDelStarting();
		}

		public virtual bool CreateIsEnding(IconContext iconContext)
		{
			//	Indique si la création de l'objet est terminée.
			return true;
		}

		public virtual bool CreateIsExist(IconContext iconContext)
		{
			//	Indique si l'objet peut exister. Retourne false si l'objet ne peut
			//	pas exister et doit être détruit.
			return true;
		}

		public virtual bool CreateEnding(IconContext iconContext)
		{
			//	Termine la création de l'objet. Retourne false si l'objet ne peut
			//	pas exister et doit être détruit.
			return false;
		}

		public virtual bool CreateAction(int rank, out string cmd, out string name, out string text)
		{
			//	Retourne un bouton d'action pendant la création.
			cmd  = "";
			name = "";
			text = "";
			return false;
		}

		public virtual bool SelectAfterCreation()
		{
			//	Indique s'il faut sélectionner l'objet après sa création.
			return false;
		}

		public virtual bool EditAfterCreation()
		{
			//	Indique s'il faut éditer l'objet après sa création.
			return false;
		}


		protected abstract AbstractObject CreateNewObject();
		public bool DuplicateObject(ref AbstractObject newObject)
		{
			//	Crée une instance de l'objet.
	
			//	Effectue une copie de l'objet courant.
			newObject = this.CreateNewObject();
			newObject.CloneObject(this);
			return true;
		}

		public virtual void CloneObject(AbstractObject src)
		{
			//	Reprend toutes les caractéristiques d'un objet.
			this.handles.Clear();
			int total = src.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle srcHandle = src.Handle(i);
				Handle newHandle = new Handle();
				srcHandle.CopyTo(newHandle);
				this.handles.Add(newHandle);
			}

			this.CloneProperties(src);

			this.isHide         = src.isHide;
			this.selected       = src.selected;
			this.globalSelected = src.globalSelected;
			this.edited         = src.edited;
			this.editProperties = src.editProperties;
			this.dirtyBbox      = src.dirtyBbox;
			this.bboxThin       = src.bboxThin;
			this.bboxGeom       = src.bboxGeom;
			this.bboxFull       = src.bboxFull;
		}

		public void DuplicateAdapt()
		{
			//	Adapte un objet qui vient d'être copié.
			if ( this.properties.Count > 0 )
			{
				PropertyName name = this.properties[0] as PropertyName;
				if ( name != null && name.String != "" )
				{
					name.String = Misc.CopyName(name.String);
				}
			}
		}


		protected virtual bool IsFullHide(IconContext iconContext)
		{
			//	Retourne true si l'objet est complètement caché.
			if ( this.isHide )  // objet caché ?
			{
				if ( !iconContext.HideHalfActive )  return true;
				iconContext.IsDimmed = true;
			}
			return false;
		}

		public virtual void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects)
		{
			//	Dessine la géométrie de l'objet.
			if ( this.IsFullHide(iconContext) )  return;

			Widgets.Drawer.totalObjectDraw ++;

			if ( iconContext.IsEditable )
			{
				this.scaleX       = iconContext.ScaleX;
				this.scaleY       = iconContext.ScaleY;
				this.minimalSize  = iconContext.MinimalSize;
				this.minimalWidth = iconContext.MinimalWidth;
				this.closeMargin  = iconContext.CloseMargin;
			}

			if ( iconContext.IsDrawBoxThin )
			{
				double initialWidth = graphics.LineWidth;
				graphics.LineWidth = 1.0/iconContext.ScaleX;

				graphics.AddRectangle(this.bboxThin);
				graphics.RenderSolid(Drawing.Color.FromARGB(0.5, 0,1,1));

				graphics.LineWidth = initialWidth;
			}

			if ( iconContext.IsDrawBoxGeom )
			{
				double initialWidth = graphics.LineWidth;
				graphics.LineWidth = 1.0/iconContext.ScaleX;

				graphics.AddRectangle(this.bboxGeom);
				graphics.RenderSolid(Drawing.Color.FromARGB(0.5, 0,1,0));

				graphics.LineWidth = initialWidth;
			}

			if ( iconContext.IsDrawBoxFull )
			{
				double initialWidth = graphics.LineWidth;
				graphics.LineWidth = 1.0/iconContext.ScaleX;

				graphics.AddRectangle(this.bboxFull);
				graphics.RenderSolid(Drawing.Color.FromARGB(0.5, 1,1,0));

				graphics.LineWidth = initialWidth;
			}
		}

		public virtual void DrawHandle(Drawing.Graphics graphics, IconContext iconContext)
		{
			//	Dessine les poignées de l'objet.
			if ( this.isHide )  return;

			foreach ( AbstractProperty property in this.properties )
			{
				property.DrawEdit(graphics, iconContext, this.bboxThin);
			}

			int total = this.TotalHandleProperties;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);

				if ( handle.Type == HandleType.Property )
				{
					handle.IsSelected = this.editProperties;
				}

				if ( handle.Type != HandleType.Hide )
				{
					handle.Draw(graphics, iconContext);
				}
			}
		}

		public virtual void PrintGeometry(Printing.PrintPort port, IconContext iconContext, IconObjects iconObjects)
		{
			//	Imprime la géométrie de l'objet.
		}


		public virtual void ArrangeAfterRead()
		{
			//	Arrange un objet après sa désérialisation. Il faut supprimer les
			//	propriétés créées à double dans le constructeur et lors de la désérialisation.
			//	Le tableau contient d'abord la propriété créée dans le constructeur,
			//	puis celle qui a été désérialisée. Il faut donc remplacer la 1ère par
			//	la 2ème, et détruire la 2ème.
			int total = this.properties.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				AbstractProperty firstProp = this.properties[i] as AbstractProperty;
				for ( int j=i+1 ; j<total ; j++ )
				{
					AbstractProperty secProp = this.properties[j] as AbstractProperty;
					if ( secProp.Type != firstProp.Type )  continue;
					this.properties[i] = this.properties[j];
					this.properties.RemoveAt(j);
					total = this.properties.Count;
				}
			}
		}


		protected static bool DetectOutline(Drawing.Path path, double width, Drawing.Point pos)
		{
			//	Détecte si la souris est sur le trait d'un chemin.
			return (AbstractObject.DetectOutlineRank(path, width, pos) != -1 );
		}

		protected static int DetectOutlineRank(Drawing.Path path, double width, Drawing.Point pos)
		{
			//	Détecte sur quel trait d'un chemin est la souris.
			//	Retourne le rang du trait (0..1), ou -1.
			Drawing.PathElement[] elements;
			Drawing.Point[] points;
			path.GetElements(out elements, out points);

			Drawing.Point start = new Drawing.Point(0, 0);
			Drawing.Point current = new Drawing.Point(0, 0);
			Drawing.Point p1 = new Drawing.Point(0, 0);
			Drawing.Point p2 = new Drawing.Point(0, 0);
			Drawing.Point p3 = new Drawing.Point(0, 0);
			int i = 0;
			int rank = 0;
			while ( i < elements.Length )
			{
				switch ( elements[i] & Drawing.PathElement.MaskCommand )
				{
					case Drawing.PathElement.MoveTo:
						current = points[i++];
						start = current;
						break;

					case Drawing.PathElement.LineTo:
						p1 = points[i++];
						if ( Drawing.Point.DetectSegment(current,p1, pos, width) )  return rank;
						rank ++;
						current = p1;
						break;

					case Drawing.PathElement.Curve3:
						p1 = points[i++];
						p2 = points[i++];
						if ( Drawing.Point.DetectBezier(current,p1,p1,p2, pos, width) )  return rank;
						rank ++;
						current = p2;
						break;

					case Drawing.PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						if ( Drawing.Point.DetectBezier(current,p1,p2,p3, pos, width) )  return rank;
						rank ++;
						current = p3;
						break;

					default:
						if ( (elements[i] & Drawing.PathElement.FlagClose) != 0 )
						{
							if ( Drawing.Point.DetectSegment(current,start, pos, width) )  return rank;
							rank ++;
						}
						i ++;
						break;
				}
			}
			return -1;
		}

		protected static bool DetectSurface(Drawing.Path path, Drawing.Point pos)
		{
			//	Détecte si la souris est dans un chemin.
			Drawing.PathElement[] elements;
			Drawing.Point[] points;
			path.GetElements(out elements, out points);

			int total = 0;
			int i = 0;
			while ( i < elements.Length )
			{
				switch ( elements[i] & Drawing.PathElement.MaskCommand )
				{
					case Drawing.PathElement.MoveTo:
						i += 1;
						break;

					case Drawing.PathElement.LineTo:
						i += 1;
						total += 1;
						break;

					case Drawing.PathElement.Curve3:
						i += 2;
						total += InsideSurface.bezierStep;
						break;

					case Drawing.PathElement.Curve4:
						i += 3;
						total += InsideSurface.bezierStep;
						break;

					default:
						if ( (elements[i] & Drawing.PathElement.FlagClose) != 0 )
						{
							total += 1;
						}
						i ++;
						break;
				}
			}
			InsideSurface surf = new InsideSurface(pos, total+10);

			Drawing.Point start = new Drawing.Point(0, 0);
			Drawing.Point current = new Drawing.Point(0, 0);
			Drawing.Point p1 = new Drawing.Point(0, 0);
			Drawing.Point p2 = new Drawing.Point(0, 0);
			Drawing.Point p3 = new Drawing.Point(0, 0);
			i = 0;
			while ( i < elements.Length )
			{
				switch ( elements[i] & Drawing.PathElement.MaskCommand )
				{
					case Drawing.PathElement.MoveTo:
						current = points[i++];
						start = current;
						break;

					case Drawing.PathElement.LineTo:
						p1 = points[i++];
						surf.AddLine(current, p1);
						current = p1;
						break;

					case Drawing.PathElement.Curve3:
						p1 = points[i++];
						p2 = points[i++];
						surf.AddBezier(current, p1, p1, p2);
						current = p2;
						break;

					case Drawing.PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						surf.AddBezier(current, p1, p2, p3);
						current = p3;
						break;

					default:
						if ( (elements[i] & Drawing.PathElement.FlagClose) != 0 )
						{
							surf.AddLine(current, start);
						}
						i ++;
						break;
				}
			}
			return surf.IsInside();
		}

		protected static Drawing.Rectangle ComputeBoundingBox(Drawing.Path path)
		{
			//	Calcule la bbox qui englobe exactement un chemin quelconque.
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			Drawing.PathElement[] elements;
			Drawing.Point[] points;
			path.GetElements(out elements, out points);

			Drawing.Point start = new Drawing.Point(0, 0);
			Drawing.Point current = new Drawing.Point(0, 0);
			Drawing.Point p1 = new Drawing.Point(0, 0);
			Drawing.Point p2 = new Drawing.Point(0, 0);
			Drawing.Point p3 = new Drawing.Point(0, 0);
			int i = 0;
			while ( i < elements.Length )
			{
				switch ( elements[i] & Drawing.PathElement.MaskCommand )
				{
					case Drawing.PathElement.MoveTo:
						current = points[i++];
						start = current;
						break;

					case Drawing.PathElement.LineTo:
						p1 = points[i++];
						bbox.MergeWith(current);
						bbox.MergeWith(p1);
						current = p1;
						break;

					case Drawing.PathElement.Curve3:
						p1 = points[i++];
						p2 = points[i++];
						AbstractObject.BoundingBoxAddBezier(ref bbox, current,p1,p1,p2);
						current = p2;
						break;

					case Drawing.PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						AbstractObject.BoundingBoxAddBezier(ref bbox, current,p1,p2,p3);
						current = p3;
						break;

					default:
						if ( (elements[i] & Drawing.PathElement.FlagClose) != 0 )
						{
							bbox.MergeWith(current);
							bbox.MergeWith(start);
						}
						i ++;
						break;
				}
			}
			return bbox;
		}

		protected static void BoundingBoxAddBezier(ref Drawing.Rectangle bbox, Drawing.Point p1, Drawing.Point s1, Drawing.Point s2, Drawing.Point p2)
		{
			//	Ajoute un courbe de Bézier dans la bbox.
			double step = 1.0/10.0;  // nombre arbitraire de 10 subdivisions
			for ( double t=0 ; t<=1.0 ; t+=step )
			{
				bbox.MergeWith(Drawing.Point.FromBezier(p1, s1, s2, p2, t));
			}
		}


		protected static bool IsRectangular(Drawing.Point p0, Drawing.Point p1, Drawing.Point p2, Drawing.Point p3)
		{
			//	Teste si l'objet est rectangulaire.
			if ( !AbstractObject.IsRight(p3, p0, p2) )  return false;
			if ( !AbstractObject.IsRight(p0, p2, p1) )  return false;
			if ( !AbstractObject.IsRight(p2, p1, p3) )  return false;
			if ( !AbstractObject.IsRight(p1, p3, p0) )  return false;
			return true;
		}

		protected static bool IsRight(Drawing.Point p1, Drawing.Point p2, Drawing.Point p3)
		{
			//	Teste si 3 points forment un angle droit.
			Drawing.Point p = Drawing.Point.Projection(p1, p2, p3);
			return Drawing.Point.Distance(p, p2) < 0.00001;
		}


		protected bool							isHilite = false;
		protected bool							isHide = false;
		protected double						scaleX;
		protected double						scaleY;
		protected double						minimalSize;
		protected double						minimalWidth;
		protected double						closeMargin;
		protected bool							selected = false;
		protected bool							edited = false;
		protected bool							globalSelected = false;
		protected bool							editProperties = false;
		protected bool							undoStamp = false;
		protected bool							dirtyBbox = true;
		protected Drawing.Rectangle				bboxThin = new Drawing.Rectangle();
		protected Drawing.Rectangle				bboxGeom = new Drawing.Rectangle();
		protected Drawing.Rectangle				bboxFull = new Drawing.Rectangle();

		protected System.Collections.ArrayList	properties = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	handles = new System.Collections.ArrayList();
		protected UndoList						objects = null;
	}
}
