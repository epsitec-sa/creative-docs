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
		public System.Collections.ArrayList Objects
		{
			get { return this.objects; }
			set { this.objects = value; }
		}


		// Nom de l'ic�ne.
		public virtual string IconName
		{
			get { return @""; }
		}


		// Nombre de poign�es, sans compter celles des propri�t�s.
		public int TotalHandle
		{
			get { return this.handles.Count; }
		}

		// Nombre de poign�es, avec celles des propri�t�s.
		public int TotalHandleProperties
		{
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

		// Ajoute une poign�e.
		public void HandleAdd(Drawing.Point pos, HandleType type)
		{
			Handle handle = new Handle();
			handle.Position = pos;
			handle.Type = type;
			this.handles.Add(handle);
			this.dirtyBbox = true;
		}

		// Ins�re une poign�e.
		public void HandleInsert(int rank, Handle handle)
		{
			this.handles.Insert(rank, handle);
			this.dirtyBbox = true;
		}

		// Supprime une poign�e.
		public void HandleDelete(int rank)
		{
			this.handles.RemoveAt(rank);
			this.dirtyBbox = true;
		}

		// Donne une poign�e de l'objet.
		public Handle Handle(int rank)
		{
			if ( rank < this.handles.Count )  // poign�e de l'objet ?
			{
				System.Diagnostics.Debug.Assert(this.handles[rank] != null);
				return this.handles[rank] as Handle;
			}
			else	// poign�e d'une propri�t� ?
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

		// D�tecte la poign�e point�e par la souris.
		public virtual int DetectHandle(Drawing.Point pos)
		{
			int total = this.TotalHandleProperties;
			//for ( int i=0 ; i<total ; i++ )
			for ( int i=total-1 ; i>=0 ; i-- )
			{
				if ( this.Handle(i).Detect(pos) )  return i;
			}
			return -1;
		}

		// D�but du d�placement d'une poign�e.
		public virtual void MoveHandleStarting(int rank, Drawing.Point pos, IconContext iconContext)
		{
			if ( rank < this.handles.Count )  // poign�e de l'objet ?
			{
				iconContext.ConstrainFixStarting(pos);
			}
			else	// poign�e d'une propri�t� ?
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

		// D�place une poign�e.
		public virtual void MoveHandleProcess(int rank, Drawing.Point pos, IconContext iconContext)
		{
			if ( rank < this.handles.Count )  // poign�e de l'objet ?
			{
				iconContext.ConstrainSnapPos(ref pos);
				iconContext.SnapGrid(ref pos);
				this.Handle(rank).Position = pos;
				this.dirtyBbox = true;
			}
			else	// poign�e d'une propri�t� ?
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

		// D�place un coin tout en conservant une forme rectangulaire.
		protected void MoveCorner(Drawing.Point pc, int corner, int left, int right, int opposite)
		{
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

		// Indique si le d�placement d'une poign�e doit se r�percuter sur les propri�t�s.
		public virtual bool IsMoveHandlePropertyChanged(int rank)
		{
			return false;
		}

		// Retourne la propri�t� modifi�e en d�pla�ant une poign�e.
		public virtual AbstractProperty MoveHandleProperty(int rank)
		{
			if ( rank >= this.handles.Count )  // poign�e d'une propri�t� ?
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

		// D�place tout l'objet.
		public virtual void MoveAll(Drawing.Point move, bool all)
		{
			foreach ( Handle handle in this.handles )
			{
				if ( all || handle.IsSelected )
				{
					handle.Position += move;
				}
			}
			this.dirtyBbox = true;
		}


		// D�place globalement l'objet.
		public virtual void MoveGlobal(GlobalModifierData initial, GlobalModifierData final, bool all)
		{
			foreach ( Handle handle in this.handles )
			{
				if ( all || handle.IsSelected )
				{
					handle.Position = GlobalModifierData.Transform(initial, final, handle.Position);
				}
			}
			this.dirtyBbox = true;
		}


		// D�but du d�placement pendant l'�dition.
		public virtual void MoveEditStarting(Drawing.Point pos, IconContext iconContext)
		{
		}

		// D�placement pendant l'�dition.
		public virtual void MoveEditProcess(Drawing.Point pos, IconContext iconContext)
		{
		}


		// D�tecte la cellule point�e par la souris.
		public virtual int DetectCell(Drawing.Point pos)
		{
			return -1;
		}

		// D�but du d�placement d'une cellule.
		public virtual void MoveCellStarting(int rank, Drawing.Point pos,
											 bool isShift, bool isCtrl, int downCount,
											 IconContext iconContext)
		{
		}

		// D�place une cellule.
		public virtual void MoveCellProcess(int rank, Drawing.Point pos,
											bool isShift, bool isCtrl,
											IconContext iconContext)
		{
		}


		// Rectangle englobant l'objet.
		public Drawing.Rectangle BoundingBox
		{
			get
			{
				if ( this.IsSelected() )  return this.BoundingBoxFull;
				else                      return this.BoundingBoxGeom;
			}
		}

		// Rectangle englobant la g�om�trie de l'objet, sans tenir compte
		// de l'�paisseur des traits.
		public Drawing.Rectangle BoundingBoxThin
		{
			get
			{
				if ( this.dirtyBbox )  // est-ce que la bbox n'est plus � jour ?
				{
					this.UpdateBoundingBox();  // on la recalcule
					this.dirtyBbox = false;  // elle est de nouveau � jour
				}
				return this.bboxThin;
			}
		}

		// Rectangle englobant la g�om�trie de l'objet, en tenant compte
		// de l'�paisseur des traits.
		public Drawing.Rectangle BoundingBoxGeom
		{
			get
			{
				if ( this.dirtyBbox )  // est-ce que la bbox n'est plus � jour ?
				{
					this.UpdateBoundingBox();  // on la recalcule
					this.dirtyBbox = false;  // elle est de nouveau � jour
				}
				return this.bboxGeom;
			}
		}

		// Rectangle englobant complet de l'objet, pendant une s�lection.
		public Drawing.Rectangle BoundingBoxFull
		{
			get
			{
				if ( this.dirtyBbox )  // est-ce que la bbox n'est plus � jour ?
				{
					this.UpdateBoundingBox();  // on la recalcule
					this.dirtyBbox = false;  // elle est de nouveau � jour
				}
				return this.bboxFull;
			}
		}

		// Calcule le rectangle englobant l'objet. Chaque objet se charge de
		// ce calcul, selon sa g�om�trie, l'�paisseur de son trait, etc.
		// Il faut calculer :
		//	this.bboxThin  bo�te selon la g�om�trie de l'objet, sans les traits
		//	this.bboxGeom  bo�te selon la g�om�trie de l'objet, avec les traits
		//	this.bboxFull  bo�te compl�te lorsque l'objet est s�lectionn�
		protected virtual void UpdateBoundingBox()
		{
		}


		// Etat survol� de l'objet.
		[XmlIgnore]
		public bool IsHilite
		{
			get
			{
				return this.isHilite;
			}

			set
			{
				this.isHilite = value;
			}
		}

		// Etat cach� de l'objet.
		[XmlIgnore]
		public bool IsHide
		{
			get
			{
				return this.isHide;
			}

			set
			{
				this.isHide = value;
			}
		}


		// S�lectionne toutes les poign�es de l'objet.
		public void Select()
		{
			this.Select(true, false);
		}

		// D�s�lectionne toutes les poign�es de l'objet.
		public void Deselect()
		{
			this.Select(false, false);
		}

		// S�lectionne ou d�s�lectionne toutes les poign�es de l'objet.
		public void Select(bool select)
		{
			this.Select(select, false);
		}

		// S�lectionne ou d�s�lectionne toutes les poign�es de l'objet.
		public virtual void Select(bool select, bool edit)
		{
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

		// S�lectionne toutes les poign�es de l'objet dans un rectangle.
		public virtual void Select(Drawing.Rectangle rect)
		{
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

		// Indique que l'objet est s�lectionn� globalement (avec GlobalModifier).
		public void GlobalSelect(bool global)
		{
			int total = this.TotalHandleProperties;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				handle.IsGlobalSelected = handle.IsSelected && global;
			}
			this.globalSelected = global;
		}

		// Indique si l'objet est s�lectionn�.
		public bool IsSelected()
		{
			return this.selected;
		}

		// Indique si l'objet est s�lectionn� globalement (avec GlobalModifier).
		public bool IsGlobalSelected()
		{
			return this.globalSelected;
		}

		// Adapte une poign�e � la s�lection globale.
		protected void GlobalHandleAdapt(int rank)
		{
			this.Handle(rank).IsGlobalSelected = this.globalSelected && this.Handle(rank).IsSelected;
		}


		// Indique si l'objet est �ditable.
		public virtual bool IsEditable()
		{
			return false;
		}

		// Indique si l'objet est en cours d'�dition.
		public bool IsEdited()
		{
			return this.edited;
		}

		
		// Indique si les propri�t�s de l'objet sont en cours d'�dition.
		[XmlIgnore]
		public bool EditProperties
		{
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


		// Nombre de prori�t�s.
		public int TotalProperty
		{
			get { return this.properties.Count; }
		}

		// Indique si une propri�t� existe.
		public bool ExistProperty(int rank)
		{
			return ( rank < this.properties.Count );
		}

		// Ajoute une prori�t� � l'objet.
		public void AddProperty(AbstractProperty property)
		{
			this.properties.Add(property);
		}

		// Donne une propri�t� de l'objet.
		public AbstractProperty Property(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as AbstractProperty;
		}

		// Ajoute toutes les propri�t�s de l'objet dans une liste.
		// Un type de propri�t� donn� n'est qu'une fois dans la liste.
		public virtual void PropertiesList(System.Collections.ArrayList list)
		{
			foreach ( AbstractProperty property in this.properties )
			{
				this.PropertyAllList(list, property);
			}
		}

		// Ajoute intelligemment une propri�t� dans la liste.
		protected void PropertyAllList(System.Collections.ArrayList list, AbstractProperty property)
		{
			AbstractProperty existing = property.Search(list);
			if ( existing == null )  // pas trouv� dans la liste ?
			{
				property.Multi = false;
				list.Add(property);
			}
			else	// d�j� dans la liste ?
			{
				if ( !property.Compare(existing) )
				{
					existing.Multi = true;
				}
			}
		}

		// Donne une propri�t� de l'objet.
		public PropertyBool PropertyBool(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyBool;
		}

		// Donne une propri�t� de l'objet.
		public PropertyDouble PropertyDouble(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyDouble;
		}

		// Donne une propri�t� de l'objet.
		public PropertyColor PropertyColor(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyColor;
		}

		// Donne une propri�t� de l'objet.
		public PropertyGradient PropertyGradient(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyGradient;
		}

		// Donne une propri�t� de l'objet.
		public PropertyShadow PropertyShadow(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyShadow;
		}

		// Donne une propri�t� de l'objet.
		public PropertyLine PropertyLine(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyLine;
		}

		// Donne une propri�t� de l'objet.
		public PropertyString PropertyString(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyString;
		}

		// Donne une propri�t� de l'objet.
		public PropertyList PropertyList(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyList;
		}

		// Donne une propri�t� de l'objet.
		public PropertyCombo PropertyCombo(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyCombo;
		}

		// Donne une propri�t� de l'objet.
		public PropertyFont PropertyFont(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyFont;
		}

		// Donne une propri�t� de l'objet.
		public PropertyJustif PropertyJustif(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyJustif;
		}

		// Donne une propri�t� de l'objet.
		public PropertyTextLine PropertyTextLine(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyTextLine;
		}

		// Donne une propri�t� de l'objet.
		public PropertyArrow PropertyArrow(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyArrow;
		}

		// Donne une propri�t� de l'objet.
		public PropertyCorner PropertyCorner(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyCorner;
		}

		// Donne une propri�t� de l'objet.
		public PropertyRegular PropertyRegular(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyRegular;
		}

		// Donne une propri�t� de l'objet.
		public PropertyImage PropertyImage(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyImage;
		}

		// Donne une propri�t� de l'objet.
		public PropertyModColor PropertyModColor(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyModColor;
		}

		// Cherche une propri�t� d'apr�s son type.
		protected virtual AbstractProperty SearchProperty(PropertyType type)
		{
			foreach ( AbstractProperty property in this.properties )
			{
				if ( property.Type == type )  return property;
			}
			return null;
		}

		// Cherche si une propri�t� est li�e � un style.
		public virtual bool IsLinkProperty(AbstractProperty property)
		{
			if ( property.StyleID == 0 )  return false;
			AbstractProperty actual = this.SearchProperty(property.Type);
			if ( actual == null )  return false;
			return ( actual.StyleID == property.StyleID );
		}

		// Retourne une copie d'une propri�t�.
		public virtual AbstractProperty GetProperty(PropertyType type)
		{
			AbstractProperty actual = this.SearchProperty(type);
			if ( actual == null )  return null;

			AbstractProperty copy = AbstractProperty.NewProperty(type);
			if ( copy == null )  return null;
			actual.CopyTo(copy);
			return copy;
		}

		// Modifie une propri�t�.
		public virtual void SetPropertyBase(AbstractProperty property)
		{
			AbstractProperty actual = this.SearchProperty(property.Type);
			if ( actual == null )  return;
			property.CopyTo(actual);
		}

		// Modifie une propri�t�.
		public virtual void SetProperty(AbstractProperty property)
		{
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

		// M�morise l'�tat �tendu d'une propri�t�.
		// Seul Drawer.objectMemory s'occupe de m�moriser cet �tat.
		public void SetPropertyExtended(PropertyType type, bool extended)
		{
			AbstractProperty property = this.SearchProperty(type);
			if ( property == null )  return;
			property.ExtendedSize = extended;
		}

		// Retourne l'�tat �tendu d'une propri�t�.
		// Seul Drawer.objectMemory s'occupe de m�moriser cet �tat.
		public bool GetPropertyExtended(PropertyType type)
		{
			AbstractProperty property = this.SearchProperty(type);
			if ( property == null )  return false;
			return property.ExtendedSize;
		}

		// Initialise l'�tat �tendu selon Drawer.objectMemory a toutes
		// les propri�t�s de l'objet.
		public void SetPropertyExtended(AbstractObject objectMemory)
		{
			foreach ( AbstractProperty property in this.properties )
			{
				AbstractProperty p = objectMemory.SearchProperty(property.Type);
				if ( p == null )  continue;
				property.ExtendedSize = p.ExtendedSize;
			}
		}

		// Reprend toutes les propri�t�s d'un objet source.
		public virtual void CloneProperties(AbstractObject src)
		{
			if ( src == null )  return;
			foreach ( AbstractProperty property in this.properties )
			{
				AbstractProperty p = src.SearchProperty(property.Type);
				if ( p == null )  continue;
				p.CopyTo(property);
			}
		}

		// Adapte les styles de l'objet coll�, qui peut provenir d'un autre fichier,
		// donc d'une autre collection de styles. On se base sur le nom des styles
		// (StyleName) pour faire la correspondance.
		// Si on trouve un nom identique -> le style de l'objet coll� est modifi�
		// en fonction du style existant.
		// Si on ne trouve pas un nom identique -> on cr�e un nouveau style, en
		// modifiant bien entendu l'identificateur (StyleID) de l'objet coll�.
		public virtual void PasteAdaptStyles(StylesCollection stylesCollection)
		{
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


		// D�tecte si la souris est sur un objet.
		public virtual bool Detect(Drawing.Point pos)
		{
			return false;
		}

		// D�tecte si l'objet est dans un rectangle.
		// all = true  -> toutes les poign�es doivent �tre dans le rectangle
		// all = false -> une seule poign�e doit �tre dans le rectangle
		public virtual bool Detect(Drawing.Rectangle rect, bool all)
		{
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

		
		// D�tecte si la souris est sur l'objet pour l'�diter.
		public virtual bool DetectEdit(Drawing.Point pos)
		{
			return false;
		}


		// Donne le contenu du menu contextuel.
		public virtual void ContextMenu(System.Collections.ArrayList list, Drawing.Point pos, int handleRank)
		{
		}

		// Ex�cute une commande du menu contextuel.
		public virtual void ContextCommand(string cmd, Drawing.Point pos, int handleRank)
		{
		}


		// D�but de la cr�ation d'un objet.
		public virtual void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainFixStarting(pos);
		}

		// D�placement pendant la cr�ation d'un objet.
		public virtual void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			this.dirtyBbox = true;
		}

		// Fin de la cr�ation d'un objet.
		public virtual void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainDelStarting();
		}

		// Indique si la cr�ation de l'objet est termin�e.
		public virtual bool CreateIsEnding(IconContext iconContext)
		{
			return true;
		}

		// Indique si l'objet peut exister. Retourne false si l'objet ne peut
		// pas exister et doit �tre d�truit.
		public virtual bool CreateIsExist(IconContext iconContext)
		{
			return true;
		}

		// Termine la cr�ation de l'objet. Retourne false si l'objet ne peut
		// pas exister et doit �tre d�truit.
		public virtual bool CreateEnding(IconContext iconContext)
		{
			return false;
		}

		// Retourne un bouton d'action pendant la cr�ation.
		public virtual bool CreateAction(int rank, out string cmd, out string name, out string text)
		{
			cmd  = "";
			name = "";
			text = "";
			return false;
		}

		// Indique s'il faut s�lectionner l'objet apr�s sa cr�ation.
		public virtual bool SelectAfterCreation()
		{
			return false;
		}


		// Cr�e une instance de l'objet.
		protected abstract AbstractObject CreateNewObject();

		// Effectue une copie de l'objet courant.
		public bool DuplicateObject(ref AbstractObject newObject)
		{
			newObject = this.CreateNewObject();
			newObject.CloneObject(this);
			return true;
		}

		// Reprend toutes les caract�ristiques d'un objet.
		public virtual void CloneObject(AbstractObject src)
		{
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


		// Retourne true si l'objet est compl�tement cach�.
		protected virtual bool IsFullHide(IconContext iconContext)
		{
			if ( this.isHide )  // objet cach� ?
			{
				if ( !iconContext.HideHalfActive )  return true;
				iconContext.IsDimmed = true;
			}
			return false;
		}

		// Dessine la g�om�trie de l'objet.
		public virtual void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			if ( this.IsFullHide(iconContext) )  return;

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

		// Dessine les poign�es de l'objet.
		public virtual void DrawHandle(Drawing.Graphics graphics, IconContext iconContext)
		{
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


		// Arrange un objet apr�s sa d�s�rialisation. Il faut supprimer les
		// propri�t�s cr��es � double dans le constructeur et lors de la d�s�rialisation.
		// Le tableau contient d'abord la propri�t� cr��e dans le constructeur,
		// puis celle qui a �t� d�s�rialis�e. Il faut donc remplacer la 1�re par
		// la 2�me, et d�truire la 2�me.
		public virtual void ArrangeAfterRead()
		{
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


		// D�tecte si la souris est sur le trait d'un chemin.
		protected static bool DetectOutline(Drawing.Path path, double width, Drawing.Point pos)
		{
			return (AbstractObject.DetectOutlineRank(path, width, pos) != -1 );
		}

		// D�tecte sur quel trait d'un chemin est la souris.
		// Retourne le rang du trait (0..1), ou -1.
		protected static int DetectOutlineRank(Drawing.Path path, double width, Drawing.Point pos)
		{
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
						if ( Drawing.Point.Detect(current,p1, pos, width) )  return rank;
						rank ++;
						current = p1;
						break;

					case Drawing.PathElement.Curve3:
						p1 = points[i++];
						p2 = points[i++];
						if ( Drawing.Point.Detect(current,p1,p1,p2, pos, width) )  return rank;
						rank ++;
						current = p2;
						break;

					case Drawing.PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						if ( Drawing.Point.Detect(current,p1,p2,p3, pos, width) )  return rank;
						rank ++;
						current = p3;
						break;

					default:
						if ( (elements[i] & Drawing.PathElement.FlagClose) != 0 )
						{
							if ( Drawing.Point.Detect(current,start, pos, width) )  return rank;
							rank ++;
						}
						i ++;
						break;
				}
			}
			return -1;
		}

		// D�tecte si la souris est dans un chemin.
		protected static bool DetectSurface(Drawing.Path path, Drawing.Point pos)
		{
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

		// Calcule la bbox qui englobe exactement un chemin quelconque.
		protected static Drawing.Rectangle ComputeBoundingBox(Drawing.Path path)
		{
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

		// Ajoute un courbe de B�zier dans la bbox.
		protected static void BoundingBoxAddBezier(ref Drawing.Rectangle bbox, Drawing.Point p1, Drawing.Point s1, Drawing.Point s2, Drawing.Point p2)
		{
			double step = 1.0/10.0;  // nombre arbitraire de 10 subdivisions
			for ( double t=0 ; t<=1.0 ; t+=step )
			{
				bbox.MergeWith(Drawing.Point.Bezier(p1, s1, s2, p2, t));
			}
		}


		// Teste si l'objet est rectangulaire.
		protected static bool IsRectangular(Drawing.Point p0, Drawing.Point p1, Drawing.Point p2, Drawing.Point p3)
		{
			if ( !AbstractObject.IsRight(p3, p0, p2) )  return false;
			if ( !AbstractObject.IsRight(p0, p2, p1) )  return false;
			if ( !AbstractObject.IsRight(p2, p1, p3) )  return false;
			if ( !AbstractObject.IsRight(p1, p3, p0) )  return false;
			return true;
		}

		// Teste si 3 points forment un angle droit.
		protected static bool IsRight(Drawing.Point p1, Drawing.Point p2, Drawing.Point p3)
		{
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
		protected bool							dirtyBbox = true;
		protected Drawing.Rectangle				bboxThin = new Drawing.Rectangle();
		protected Drawing.Rectangle				bboxGeom = new Drawing.Rectangle();
		protected Drawing.Rectangle				bboxFull = new Drawing.Rectangle();

		protected System.Collections.ArrayList	properties = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	handles = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	objects = null;
	}
}
