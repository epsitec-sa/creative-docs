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

		public virtual void CreateProperties()
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
		[XmlArrayItem("String",   Type=typeof(PropertyString))]
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

		[XmlArrayItem("Arrow",     Type=typeof(ObjectArrow))]
		[XmlArrayItem("Bezier",    Type=typeof(ObjectBezier))]
		[XmlArrayItem("Circle",    Type=typeof(ObjectCircle))]
		[XmlArrayItem("Ellipse",   Type=typeof(ObjectEllipse))]
		[XmlArrayItem("Group",     Type=typeof(ObjectGroup))]
		[XmlArrayItem("Line",      Type=typeof(ObjectLine))]
		[XmlArrayItem("Polyline",  Type=typeof(ObjectPoly))]
		[XmlArrayItem("Rectangle", Type=typeof(ObjectRectangle))]
		[XmlArrayItem("Polygon",   Type=typeof(ObjectRegular))]
		[XmlArrayItem("Text",      Type=typeof(ObjectText))]
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
			get
			{
				return this.handles.Count;
			}
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
			this.durtyBbox = true;
		}

		// Ins�re une poign�e.
		public void HandleInsert(int rank, Handle handle)
		{
			this.handles.Insert(rank, handle);
			this.durtyBbox = true;
		}

		// Supprime une poign�e.
		public void HandleDelete(int rank)
		{
			this.handles.RemoveAt(rank);
			this.durtyBbox = true;
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
						return property.Handle(rank-index, this.bbox);
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

		// D�but du d�placement une poign�e.
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
						property.MoveHandleStarting(rank-index, pos, this.bbox, iconContext);
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
				this.Handle(rank).Position = pos;
				this.durtyBbox = true;
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
						property.MoveHandleProcess(rank-index, pos, this.bbox, iconContext);
						break;
					}
					index += count;
				}
			}
		}

		// D�place tout l'objet.
		public virtual void MoveAll(Drawing.Point move)
		{
			foreach ( Handle handle in this.handles )
			{
				handle.Position += move;
			}
			this.bbox.Offset(move);
		}


		// Met � jour le rectangle englobant l'objet.
		public virtual void UpdateBoundingBox()
		{
		}

		// Rectangle englobant l'objet.
		public virtual Drawing.Rectangle BoundingBox
		{
			get
			{
				if ( this.durtyBbox )
				{
					this.UpdateBoundingBox();
					this.durtyBbox = false;
				}
				return this.bbox;
			}
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


		// S�lectionne toutes les poign�es de l'objet.
		public void Select()
		{
			this.Select(true);
		}

		// D�s�lectionne toutes les poign�es de l'objet.
		public void Deselect()
		{
			this.Select(false);
		}

		// S�lectionne ou d�s�lectionne toutes les poign�es de l'objet.
		public void Select(bool select)
		{
			this.selected = select;

			int total = this.TotalHandleProperties;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				handle.IsSelected = select;
			}
		}

		// Indique si l'objet est s�lectionn�.
		public bool IsSelected()
		{
			return this.selected;
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
			get
			{
				return this.properties.Count;
			}
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

		// Cherche une propri�t� d'apr�s son type.
		protected AbstractProperty SearchProperty(PropertyType type)
		{
			foreach ( AbstractProperty property in this.properties )
			{
				if ( property.Type == type )  return property;
			}
			return null;
		}

		// Retourne une propri�t�.
		public AbstractProperty GetProperty(PropertyType type)
		{
			AbstractProperty actual = this.SearchProperty(type);
			if ( actual == null )  return null;

			AbstractProperty copy = AbstractProperty.NewProperty(type);
			if ( copy == null )  return null;

			actual.CopyTo(copy);
			return copy;
		}

		// Modifie une propri�t�.
		public void SetProperty(AbstractProperty property)
		{
			AbstractProperty actual = this.SearchProperty(property.Type);
			if ( actual == null )  return;

			property.CopyTo(actual);
		}

		// Modifie juste l'�tat "�tendu" d'une propri�t�.
		public void SetPropertyExtended(AbstractProperty property)
		{
			AbstractProperty actual = this.SearchProperty(property.Type);
			if ( actual == null )  return;

			actual.ExtendedSize = property.ExtendedSize;
		}

		// Reprend toutes les propri�t�s d'un objet source.
		public void CloneProperties(AbstractObject src)
		{
			if ( src == null )  return;
			foreach ( AbstractProperty property in this.properties )
			{
				AbstractProperty p = src.SearchProperty(property.Type);
				if ( p == null )  continue;
				p.CopyTo(property);
			}
		}

		// Reprend toutes les informations des propri�t�s d'un objet source.
		public void CloneInfoProperties(AbstractObject src)
		{
			if ( src == null )  return;
			foreach ( AbstractProperty property in this.properties )
			{
				AbstractProperty p = src.SearchProperty(property.Type);
				if ( p == null )  continue;
				p.CopyInfoTo(property);
			}
		}


		// D�tecte si la souris est sur un objet.
		public virtual bool Detect(Drawing.Point pos)
		{
			return false;
		}

		// D�tecte si l'objet est dans un rectangle.
		public virtual bool Detect(Drawing.Rectangle rect)
		{
			foreach ( Handle handle in this.handles )
			{
				if ( !rect.Contains(handle.Position) )  return false;
			}
			return true;
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
			this.durtyBbox = true;
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


		// Cr�e une instance de l'objet.
		protected abstract AbstractObject CreateNewObject();

		// Effectue une copie de l'objet courant.
		public bool DuplicateObject(ref AbstractObject newObject)
		{
			newObject = this.CreateNewObject();
			newObject.CreateProperties();
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

			this.selected       = src.selected;
			this.editProperties = src.editProperties;
			this.durtyBbox      = src.durtyBbox;
			this.bbox           = src.bbox;
		}


		// Dessine la g�om�trie de l'objet.
		public virtual void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			if ( iconContext.IsEditable )
			{
				this.scaleX       = iconContext.ScaleX;
				this.scaleY       = iconContext.ScaleY;
				this.minimalSize  = iconContext.MinimalSize;
				this.minimalWidth = iconContext.MinimalWidth;
				this.closeMargin  = iconContext.CloseMargin;
			}
		}

		// Dessine les poign�es de l'objet.
		public virtual void DrawHandle(Drawing.Graphics graphics, IconContext iconContext)
		{
			foreach ( AbstractProperty property in this.properties )
			{
				property.DrawEdit(graphics, iconContext, this.bbox);
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

		// Retourne l'origine de l'objet.
		public virtual Drawing.Point Origin
		{
			get
			{
				return new Drawing.Point(0, 0);
			}
		}


		protected bool							isHilite;
		protected double						scaleX;
		protected double						scaleY;
		protected double						minimalSize;
		protected double						minimalWidth;
		protected double						closeMargin;
		protected bool							selected = false;
		protected bool							editProperties = false;
		protected bool							durtyBbox = true;
		protected Drawing.Rectangle				bbox = new Drawing.Rectangle();

		[XmlAttribute]
		protected System.Collections.ArrayList	properties = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	handles = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	objects = null;
	}
}
