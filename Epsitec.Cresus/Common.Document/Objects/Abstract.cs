using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Objects.Abstract est la classe de base des objets graphiques.
	/// </summary>
	[System.Serializable()]
	public abstract class Abstract : ISerializable
	{
		// Constructeur.
		// Si document = null, on crée un objet factice, c'est-à-dire
		// sans propriétés. On utilise un objet factice pour appeler
		// la méthode ExistingProperty.
		public Abstract(Document document, Objects.Abstract model)
		{
			this.document = document;
			this.uniqueId = Modifier.GetNextUniqueObjectId();
			this.properties = new UndoableList(this.document, UndoableListType.PropertiesInsideObject);
		}

		public virtual void Dispose()
		{
			foreach ( Properties.Abstract property in this.properties )
			{
				property.Owners.Remove(this);

				if ( !property.IsStyle &&
					 !property.IsFloating &&
					 property.Owners.Count == 0 )
				{
					this.document.Modifier.PropertyRemove(property);
				}

				if ( property.IsStyle )
				{
					this.document.Notifier.NotifyStyleChanged();
				}
			}
		}


		// Indique si l'objet a besoin de cette propriété.
		protected virtual bool ExistingProperty(Properties.Type type)
		{
			return false;
		}

		// Crée toutes les propriétés dont l'objet a besoin. Cette méthode est
		// appelée par les constructeurs de tous les objets.
		protected void CreateProperties(Objects.Abstract model, bool floating)
		{
			System.Diagnostics.Debug.Assert(this.document != null);
			foreach ( int value in System.Enum.GetValues(typeof(Properties.Type)) )
			{
				Properties.Type type = (Properties.Type)value;
				if ( this.ExistingProperty(type) )
				{
					this.AddProperty(type, model, floating);
				}
			}
		}

		// Crée un nouvel objet selon l'outil sélectionné.
		public static Objects.Abstract CreateObject(Document document, string name, Objects.Abstract model)
		{
			Objects.Abstract obj = null;
			switch ( name )
			{
				case "ObjectLine":       obj = new Line(document, model);       break;
				case "ObjectRectangle":  obj = new Rectangle(document, model);  break;
				case "ObjectCircle":     obj = new Circle(document, model);     break;
				case "ObjectEllipse":    obj = new Ellipse(document, model);    break;
				case "ObjectRegular":    obj = new Regular(document, model);    break;
				case "ObjectPoly":       obj = new Poly(document, model);       break;
				case "ObjectBezier":     obj = new Bezier(document, model);     break;
				case "ObjectTextLine":   obj = new TextLine(document, model);   break;
				case "ObjectTextBox":    obj = new TextBox(document, model);    break;
				case "ObjectArray":      obj = new Array(document, model);      break;
				case "ObjectImage":      obj = new Image(document, model);      break;
			}
			System.Diagnostics.Debug.Assert(obj != null);
			return obj;
		}

		
		public System.Collections.ArrayList Handles
		{
			get { return this.handles; }
			set { this.handles = value; }
		}

		public UndoableList Objects
		{
			get { return this.objects; }
			set { this.objects = value; }
		}

		public int UniqueId
		{
			get { return this.uniqueId; }
		}


		// Nom de l'icône.
		public virtual string IconName
		{
			get { return @""; }
		}


		// Nom de l'objet, utilisé pour les pages et les calques.
		public string Name
		{
			get
			{
				return this.name;
			}
			
			set
			{
				if ( this.name != value )
				{
					this.InsertOpletName();
					this.name = value;
				}
			}
		}


		// Nombre total de poignées, avec celles des propriétés.
		public int TotalHandle
		{
			get { return this.handles.Count; }
		}

		// Nombre total de poignées, sans celles des propriétés.
		public int TotalMainHandle
		{
			get { return this.handles.Count-this.totalPropertyHandle; }
		}

		// Nombre total de poignées des propriétés.
		public int TotalPropertyHandle
		{
			get { return this.totalPropertyHandle; }
		}

		// Crée toutes les poignées pour les propriétés.
		protected void HandlePropertiesCreate()
		{
			foreach ( Properties.Abstract property in this.properties )
			{
				int total = property.TotalHandle(this);
				for ( int i=0 ; i<total ; i++ )
				{
					Handle handle = new Handle(this.document);
					handle.Type = HandleType.Property;
					handle.PropertyType = property.Type;
					handle.PropertyRank = i;
					this.handles.Add(handle);
					this.totalPropertyHandle ++;
					this.dirtyBbox = true;
				}
			}
		}

		// Met à jour toutes les poignées des propriétés.
		public void HandlePropertiesUpdateVisible()
		{
			bool sel = this.selected && !this.edited && !this.globalSelected;
			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( handle.PropertyType != Properties.Type.None )
				{
					Properties.Abstract property = this.Property(handle.PropertyType);
					handle.IsVisible = property.IsHandleVisible(this, handle.PropertyRank) && sel;
					handle.IsGlobalSelected = this.globalSelected && handle.IsVisible;
					this.dirtyBbox = true;
				}
			}
		}

		// Met à jour toutes les poignées des propriétés.
		public void HandlePropertiesUpdatePosition()
		{
			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( handle.PropertyType != Properties.Type.None )
				{
					Properties.Abstract property = this.Property(handle.PropertyType);
					handle.Position = property.GetHandlePosition(this, handle.PropertyRank);
				}
			}
		}

		// Ajoute une poignée.
		public void HandleAdd(Point pos, HandleType type)
		{
			Handle handle = new Handle(this.document);
			handle.Position = pos;
			handle.Type = type;
			this.handles.Add(handle);
			this.dirtyBbox = true;
		}

		// Insère une poignée.
		public void HandleInsert(int rank, Handle handle)
		{
			this.handles.Insert(rank, handle);
			this.dirtyBbox = true;
		}

		// Supprime une poignée.
		public void HandleDelete(int rank)
		{
			this.handles.RemoveAt(rank);
			this.dirtyBbox = true;
		}

		// Donne la position d'une poignée.
		public Point GetHandlePosition(int rank)
		{
			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				System.Diagnostics.Debug.Assert(this.handles[rank] != null);
				Handle handle = this.handles[rank] as Handle;
				return handle.Position;
			}
			return new Point(0,0);
		}

		// Modifie l'état "survollé" d'une poignée.
		public void HandleHilite(int rank, bool hilite)
		{
			Handle handle = this.Handle(rank);
			handle.IsHilited = hilite;
		}

		// Donne une poignée de l'objet.
		public Handle Handle(int rank)
		{
			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				System.Diagnostics.Debug.Assert(this.handles[rank] != null);
				return this.handles[rank] as Handle;
			}
			return null;
		}

		// Détecte la poignée pointée par la souris.
		public virtual int DetectHandle(Point pos)
		{
			int total = this.TotalHandle;
			for ( int i=total-1 ; i>=0 ; i-- )
			{
				if ( this.Handle(i).Detect(pos) )  return i;
			}
			return -1;
		}

		// Début du déplacement d'une poignée.
		public virtual void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				this.InsertOpletGeometry();
				drawingContext.ConstrainFixStarting(pos);
			}
		}

		// Déplace une poignée.
		public virtual void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				drawingContext.ConstrainSnapPos(ref pos);
				drawingContext.SnapGrid(ref pos);

				Handle handle = this.Handle(rank);
				handle.Position = pos;

				if ( handle.PropertyType != Properties.Type.None )
				{
					Properties.Abstract property = this.Property(handle.PropertyType);
					property.SetHandlePosition(this, handle.PropertyRank, pos);
				}

				this.dirtyBbox = true;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Déplace un coin tout en conservant une forme rectangulaire.
		protected void MoveCorner(Point pc, int corner, int left, int right, int opposite)
		{
			if ( Geometry.IsRectangular(this.Handle(0).Position, this.Handle(1).Position, this.Handle(2).Position, this.Handle(3).Position) )
			{
				this.Handle(corner).Position = pc;

				Point pl = this.Handle(left).Position;
				Point pr = this.Handle(right).Position;
				Point po = this.Handle(opposite).Position;

				if ( pl == pr )
				{
					this.Handle(left ).Position = new Point(pc.X, po.Y);
					this.Handle(right).Position = new Point(po.X, pc.Y);
				}
				else if ( pl == po )
				{
					this.Handle(right).Position = Point.Projection(pr, po, pc);
					this.Handle(left ).Position = po+(pc-this.Handle(right).Position);
				}
				else if ( pr == po )
				{
					this.Handle(left ).Position = Point.Projection(pl, po, pc);
					this.Handle(right).Position = po+(pc-this.Handle(left).Position);
				}
				else
				{
					this.Handle(left ).Position = Point.Projection(pl, po, pc);
					this.Handle(right).Position = Point.Projection(pr, po, pc);
				}
			}
			else
			{
				this.Handle(corner).Position = pc;
			}
		}


		// Début du déplacement de tout l'objet.
		public virtual void MoveAllStarting()
		{
			this.InsertOpletGeometry();
		}

		// Effectue le déplacement de tout l'objet.
		// Un objet désélectionné est déplacé entièrement, car il s'agit forcément
		// du fils d'un objet sélectionné.
		public virtual void MoveAllProcess(Point move)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			bool allHandle = !this.IsSelected;
			foreach ( Handle handle in this.handles )
			{
				if ( allHandle || handle.IsVisible )
				{
					handle.Position += move;
				}
			}

			this.dirtyBbox = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Début du déplacement global de l'objet.
		public virtual void MoveGlobalStarting()
		{
			this.InsertOpletGeometry();
		}

		// Effectue le déplacement global de l'objet.
		// Un objet désélectionné est déplacé entièrement, car il s'agit forcément
		// du fils d'un objet sélectionné.
		public virtual void MoveGlobalProcess(SelectorData initial, SelectorData final)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			bool allHandle = !this.IsSelected;
			foreach ( Handle handle in this.handles )
			{
				if ( allHandle || handle.IsVisible )
				{
					handle.Position = SelectorData.DotTransform(initial, final, handle.Position);
				}
			}

			this.dirtyBbox = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Gestion d'un événement pendant l'édition.
		public virtual bool EditProcessMessage(Message message, Point pos)
		{
			return false;
		}

		// Gestion d'un événement pendant l'édition.
		public virtual void EditMouseDownMessage(Point pos)
		{
		}

		// Détecte la cellule pointée par la souris.
		public virtual int DetectCell(Point pos)
		{
			return -1;
		}

		// Début du déplacement d'une cellule.
		public virtual void MoveCellStarting(int rank, Point pos,
											 bool isShift, bool isCtrl, int downCount,
											 DrawingContext drawingContext)
		{
		}

		// Déplace une cellule.
		public virtual void MoveCellProcess(int rank, Point pos, DrawingContext drawingContext)
		{
		}


		// Indique qu'il faudra refaire les bbox.
		public void SetDirtyBbox()
		{
			this.dirtyBbox = true;
		}

		// Rectangle englobant l'objet.
		public Drawing.Rectangle BoundingBox
		{
			get
			{
				if ( this.IsSelected )  return this.BoundingBoxFull;
				else                    return this.BoundingBoxGeom;
			}
		}

		// Rectangle englobant l'objet pour les détections.
		public Drawing.Rectangle BoundingBoxDetect
		{
			get
			{
				Drawing.Rectangle bbox = this.BoundingBoxThin;
				Properties.Line line = this.PropertyLineMode;
				if ( line != null )
				{
					bbox.Inflate(line.Width/2.0);
				}
				return bbox;
			}
		}

		// Rectangle englobant l'objet pour les groupes.
		public Drawing.Rectangle BoundingBoxGroup
		{
			get
			{
				Drawing.Rectangle bbox = this.BoundingBoxThin;
				Properties.Line line = this.PropertyLineMode;
				if ( line != null )
				{
					bbox.Inflate(line.Width/2.0);
				}
				return bbox;
			}
		}

		// Rectangle englobant la géométrie de l'objet, sans tenir compte
		// de l'épaisseur des traits.
		public Drawing.Rectangle BoundingBoxThin
		{
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

		// Rectangle englobant la géométrie de l'objet, en tenant compte
		// de l'épaisseur des traits.
		public Drawing.Rectangle BoundingBoxGeom
		{
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

		// Rectangle englobant complet de l'objet, pendant une sélection.
		public Drawing.Rectangle BoundingBoxFull
		{
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

		// Calcule le rectangle englobant l'objet. Chaque objet se charge de
		// ce calcul, selon sa géométrie, l'épaisseur de son trait, etc.
		// Il faut calculer :
		//	this.bboxThin  boîte selon la géométrie de l'objet, sans les traits
		//	this.bboxGeom  boîte selon la géométrie de l'objet, avec les traits
		//	this.bboxFull  boîte complète lorsque l'objet est sélectionné
		protected virtual void UpdateBoundingBox()
		{
		}

		// Calcule toutes les bbox de l'objet en fonction des propriétés.
		protected void ComputeBoundingBox(Path[] paths, bool[] outlines, bool[] surfaces)
		{
			System.Diagnostics.Debug.Assert(paths.Length == outlines.Length);
			System.Diagnostics.Debug.Assert(paths.Length == surfaces.Length);

			Properties.Line     line    = this.PropertyLineMode;
			Properties.Gradient outline = this.PropertyLineColor;
			Properties.Gradient surface = this.PropertyFillGradient;

			this.bboxThin = Drawing.Rectangle.Empty;
			this.bboxGeom = Drawing.Rectangle.Empty;
			for ( int i=0 ; i<paths.Length ; i++ )
			{
				if ( paths[i] == null )  continue;

				Drawing.Rectangle bbox = Geometry.ComputeBoundingBox(paths[i]);
				this.bboxThin.MergeWith(bbox);

				double width1 = 0.0;
				double width2 = 0.0;

				if ( outlines[i] )
				{
					if ( line != null )
					{
						width1 += line.WidthInflateBoundingBox();
					}

					if ( outline != null )
					{
						width1 += outline.WidthInflateBoundingBox();
					}
				}

				if ( surfaces[i] )
				{
					if ( surface != null )
					{
						width2 += surface.WidthInflateBoundingBox();
					}
				}

				double width = System.Math.Max(width1, width2);
				bbox.Inflate(width);
				this.bboxGeom.MergeWith(bbox);
			}

			this.bboxFull = this.bboxGeom;

			if ( outline != null )
			{
				outline.InflateBoundingBox(this.bboxGeom, ref this.bboxFull);
			}

			if ( surface != null )
			{
				surface.InflateBoundingBox(this.bboxThin, ref this.bboxFull);
			}
		}

		// Agrandit toutes les bbox en fonction d'un point supplémentaire.
		protected void InflateBoundingBox(Point pos, bool onlyFull)
		{
			if ( !onlyFull )
			{
				this.bboxThin.MergeWith(pos);
				this.bboxGeom.MergeWith(pos);
			}
			this.bboxFull.MergeWith(pos);
		}


		// Etat survolé de l'objet.
		public bool IsHilite
		{
			get
			{
				return this.isHilite;
			}

			set
			{
				if ( this.isHilite != value )
				{
					this.isHilite = value;
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
				}
			}
		}

		// Etat caché de l'objet.
		public bool IsHide
		{
			get
			{
				return this.isHide;
			}

			set
			{
				if ( this.isHide != value )
				{
					this.InsertOpletSelection();
					this.isHide = value;
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
					this.document.Notifier.NotifySelectionChanged();
				}
			}
		}


		// Sélectionne toutes les poignées de l'objet.
		public void Select()
		{
			this.Select(true, false);
		}

		// Désélectionne toutes les poignées de l'objet.
		public void Deselect()
		{
			this.Select(false, false);
		}

		// Sélectionne ou désélectionne toutes les poignées de l'objet.
		public void Select(bool select)
		{
			this.Select(select, false);
		}

		// Sélectionne ou désélectionne toutes les poignées de l'objet.
		public virtual void Select(bool select, bool edit)
		{
			this.InsertOpletSelection();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);

			this.selected = select;
			this.edited = edit;
			this.globalSelected = false;
			this.SplitProperties();

			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( handle.PropertyType != Properties.Type.None )  break;

				handle.IsVisible = select && !edit;
				handle.IsGlobalSelected = false;
			}
			this.HandlePropertiesUpdateVisible();
			this.dirtyBbox = true;

			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Notifier.NotifySelectionChanged();
		}

		// Sélectionne toutes les poignées de l'objet dans un rectangle.
		public virtual void Select(Drawing.Rectangle rect)
		{
			this.InsertOpletSelection();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);

			int sel = 0;
			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( handle.PropertyType != Properties.Type.None )  break;

				if ( rect.Contains(handle.Position) )
				{
					handle.IsVisible = true;
					handle.IsGlobalSelected = false;
					sel ++;
				}
				else
				{
					handle.IsVisible = false;
					handle.IsGlobalSelected = false;
				}
			}
			this.selected = ( sel > 0 );
			this.edited = false;
			this.globalSelected = false;
			this.HandlePropertiesUpdateVisible();
			this.SplitProperties();

			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Notifier.NotifySelectionChanged();
		}

		// Indique que l'objet est sélectionné globalement (avec Selector).
		public void GlobalSelect(bool global)
		{
			this.InsertOpletSelection();

			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( handle.PropertyType != Properties.Type.None )  break;

				handle.IsGlobalSelected = handle.IsVisible && global;
			}
			this.globalSelected = global;
			this.HandlePropertiesUpdateVisible();

			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// Indique si l'objet est sélectionné.
		public virtual bool IsSelected
		{
			get { return this.selected; }
		}

		// Indique si l'objet est sélectionné globalement (avec Selector).
		public bool IsGlobalSelected
		{
			get { return this.globalSelected; }
		}

		// Adapte une poignée à la sélection globale.
		protected void GlobalHandleAdapt(int rank)
		{
			this.Handle(rank).IsGlobalSelected = this.globalSelected && this.Handle(rank).IsVisible;
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}


		// Indique si l'objet est éditable.
		public virtual bool IsEditable
		{
			get { return false; }
		}

		// Indique si l'objet est en cours d'édition.
		public bool IsEdited
		{
			get { return this.edited; }
		}

		// Lie l'objet éditable à une règle.
		public virtual bool EditRulerLink(TextRuler ruler, DrawingContext drawingContext)
		{
			return false;
		}

		
		// Ajoute une nouvelle propriété à l'objet.
		// Une propriété flottante n'est référencée par personne et elle n'est pas
		// dans la liste des propriétés du document. ObjectPoly crée un ObjectLine
		// avec des propriétés flottantes, pendant la création.
		protected void AddProperty(Properties.Type type, Objects.Abstract model, bool floating)
		{
			Properties.Abstract property = Properties.Abstract.NewProperty(this.document, type);

			property.Owners.Add(this);  // l'objet est un propriétaire de cette propriété
			this.properties.Add(property);  // ajoute dans la liste de l'objet

			if ( this is Objects.Memory )
			{
				// Les propriétés de ObjectMemory sont marquées "IsOnlyForCreation".
				// De plus, elles ne sont pas dans la liste des propriétés du document.
				property.IsOnlyForCreation = true;
				return;
			}

			if ( model != null )
			{
				Properties.Abstract original = model.Property(type);
				original.CopyTo(property);
			}

			if ( floating )
			{
				property.IsFloating = true;
				return;
			}

			property.IsSelected = this.selected;

			Properties.Abstract idem = this.SearchProperty(property, property.IsStyle, property.IsSelected);
			if ( idem == null || property is Properties.ModColor )
			{
				this.document.Modifier.PropertyAdd(property);
			}
			else
			{
				property.Owners.Remove(this);
				idem.Owners.Add(this);
				this.ChangeProperty(idem);  // l'objet utilise désormais la propriété destination
			}
		}

		// Nombre de proriétés.
		public int TotalProperty
		{
			get { return this.properties.Count; }
		}

		// Indique si une propriété existe.
		public bool ExistProperty(Properties.Type type)
		{
			return ( this.Property(type) != null );
		}

		// Change une propriété de l'objet.
		public void ChangeProperty(Properties.Abstract property)
		{
			int i = this.PropertyIndex(property.Type);
			System.Diagnostics.Debug.Assert(i != -1);
			this.properties[i] = property;

			if ( property.IsStyle )
			{
				this.document.Notifier.NotifyStyleChanged();
			}
		}

		// Donne l'index d'une propriété de l'objet.
		public int PropertyIndex(Properties.Type type)
		{
			for ( int i=0 ; i<this.properties.Count ; i++ )
			{
				Properties.Abstract property = this.properties[i] as Properties.Abstract;
				if ( property.Type == type )  return i;
			}
			return -1;
		}

		// Donne une propriété de l'objet.
		public Properties.Abstract Property(Properties.Type type)
		{
			foreach ( Properties.Abstract property in this.properties )
			{
				if ( property.Type == type )  return property;
			}
			return null;
		}

		public Properties.Name PropertyName
		{
			get { return this.Property(Properties.Type.Name) as Properties.Name; }
		}

		public Properties.Gradient PropertyLineColor
		{
			get { return this.Property(Properties.Type.LineColor) as Properties.Gradient; }
		}

		public Properties.Line PropertyLineMode
		{
			get { return this.Property(Properties.Type.LineMode) as Properties.Line; }
		}

		public Properties.Gradient PropertyFillGradient
		{
			get { return this.Property(Properties.Type.FillGradient) as Properties.Gradient; }
		}

		public Properties.Shadow PropertyShadow
		{
			get { return this.Property(Properties.Type.Shadow) as Properties.Shadow; }
		}

		public Properties.Bool PropertyPolyClose
		{
			get { return this.Property(Properties.Type.PolyClose) as Properties.Bool; }
		}

		public Properties.Arrow PropertyArrow
		{
			get { return this.Property(Properties.Type.Arrow) as Properties.Arrow; }
		}

		public Properties.Corner PropertyCorner
		{
			get { return this.Property(Properties.Type.Corner) as Properties.Corner; }
		}

		public Properties.Regular PropertyRegular
		{
			get { return this.Property(Properties.Type.Regular) as Properties.Regular; }
		}

		public Properties.Arc PropertyArc
		{
			get { return this.Property(Properties.Type.Arc) as Properties.Arc; }
		}

		public Properties.Color PropertyBackColor
		{
			get { return this.Property(Properties.Type.BackColor) as Properties.Color; }
		}

		public Properties.Font PropertyTextFont
		{
			get { return this.Property(Properties.Type.TextFont) as Properties.Font; }
		}

		public Properties.Justif PropertyTextJustif
		{
			get { return this.Property(Properties.Type.TextJustif) as Properties.Justif; }
		}

		public Properties.TextLine PropertyTextLine
		{
			get { return this.Property(Properties.Type.TextLine) as Properties.TextLine; }
		}

		public Properties.Image PropertyImage
		{
			get { return this.Property(Properties.Type.Image) as Properties.Image; }
		}

		public Properties.ModColor PropertyModColor
		{
			get { return this.Property(Properties.Type.ModColor) as Properties.ModColor; }
		}

		// Cherche une propriété d'un type donné dans une liste.
		protected static int PropertySearch(System.Collections.ArrayList list, Properties.Type type)
		{
			for ( int i=0 ; i<list.Count ; i++ )
			{
				Properties.Abstract property = list[i] as Properties.Abstract;
				if ( property.Type == type )  return i;
			}
			return -1;
		}

		// Ajoute toutes les propriétés de l'objet dans une liste.
		public virtual void PropertiesList(System.Collections.ArrayList list,
										   bool propertiesDetail)
		{
			foreach ( Properties.Abstract property in this.properties )
			{
				//?if ( propertiesDetail || property.IsStyle )
				if ( propertiesDetail )
				{
					if ( !list.Contains(property) )
					{
						list.Add(property);
					}
				}
				else
				{
					int index = Abstract.PropertySearch(list, property.Type);
					if ( index == -1 )
					{
						list.Add(property);
					}
					else
					{
						Properties.Abstract idem = list[index] as Properties.Abstract;

						if ( !property.Compare(idem) )
						{
							if ( idem.IsMulti == false )
							{
								Properties.Abstract multi = Properties.Abstract.NewProperty(this.document, idem.Type);
								idem.CopyTo(multi);
								multi.IsMulti = true;
								multi.Owners.Add(idem);
								multi.Owners.Add(property);  // proptiétaires de type Abstract !
								list[index] = multi;  // remplace par multi
							}
							else
							{
								if ( !idem.Owners.Contains(property) )
								{
									idem.Owners.Add(property);  // proptiétaires de type Abstract !
								}
							}
						}
					}
				}
			}
		}

		// Ajoute toutes les propriétés de l'objet dans une liste.
		public virtual void PropertiesList(System.Collections.ArrayList list,
										   Objects.Abstract filter)
		{
			foreach ( Properties.Abstract property in this.properties )
			{
				if ( filter != null && !filter.ExistingProperty(property.Type) )  continue;
				list.Add(property);
			}
		}

		// Cherche si l'objet utilise une propriété.
		public bool PropertyExist(Properties.Abstract search)
		{
			return this.properties.Contains(search);
		}

		// Reprend toutes les propriétés d'un objet source.
		public void PickerProperties(Objects.Abstract model)
		{
			if ( this is Objects.Memory )
			{
				foreach ( Properties.Abstract mp in model.properties )
				{
					Properties.Abstract property = this.Property(mp.Type);
					if ( property == null )  continue;

					if ( property.IsStyle == mp.IsStyle )
					{
						mp.CopyTo(property);
					}
					else
					{
						Properties.Abstract style = Properties.Abstract.NewProperty(this.document, mp.Type);
						mp.CopyTo(style);
						this.document.Modifier.OpletQueueEnable = false;
						this.ChangeProperty(style);
						this.document.Modifier.OpletQueueEnable = true;
					}
				}
			}
			else
			{
				foreach ( Properties.Abstract mp in model.properties )
				{
					this.UseProperty(mp);
				}
				this.dirtyBbox = true;
			}
		}

		// Utilise un style donné.
		public void UseProperty(Properties.Abstract style)
		{
			Properties.Abstract actual = this.Property(style.Type);
			if ( actual == null )  return;

			this.MergeProperty(style, actual);
		}

		// Libère un style (style -> propriété).
		public void FreeProperty(Properties.Abstract search)
		{
			for ( int i=0 ; i<this.properties.Count ; i++ )
			{
				Properties.Abstract property = this.properties[i] as Properties.Abstract;

				if ( search.IsMulti )
				{
					if ( property.Type == search.Type )
					{
						if ( property.IsStyle )
						{
							this.SplitProperty(property, this.selected);
						}
						return;
					}
				}
				else
				{
					if ( property == search )
					{
						this.SplitProperty(property, this.selected);
						return;
					}
				}
			}
		}

		// Détache les propriétés sélectionnées des propriétés désélectionnées.
		// C'est nécessaire au cas où une propriété est modifiée.
		protected void SplitProperties()
		{
			for ( int i=0 ; i<this.properties.Count ; i++ )
			{
				Properties.Abstract property = this.properties[i] as Properties.Abstract;

				if ( property.IsStyle )  continue;
				if ( property.IsSelected == this.selected )  continue;

				this.SplitProperty(property, this.selected);
			}
		}

		// Détache une propriété.
		protected void SplitProperty(Properties.Abstract property, bool selected)
		{
			Properties.Abstract dst = this.SearchProperty(property, false, selected);
			if ( dst == null )  // la propriété détachée sera seule ?
			{
				// Crée une nouvelle instance pour la propriété dans son nouvel
				// état, car elle est seule à être comme cela.
				dst = Properties.Abstract.NewProperty(this.document, property.Type);
				property.CopyTo(dst);
				dst.IsSelected = selected;  // nouvel état
				dst.IsStyle = false;
				dst.StyleName = "";
				this.document.Modifier.PropertyAdd(dst);
			}
			this.MergeProperty(dst, property);  // fusionne év. à une même propriété
		}

		// Essaie de fusionner une propriété avec une même.
		protected void MergeProperty(Properties.Abstract dst, Properties.Abstract src)
		{
			if ( dst == null )  return;

			src.Owners.Remove(this);
			dst.Owners.Add(this);
			this.ChangeProperty(dst);  // l'objet utilise désormais la propriété destination

			if ( src.Owners.Count == 0 && !src.IsStyle )  // propriété source plus utilisée ?
			{
				this.document.Modifier.PropertyRemove(src);
			}
		}

		// Cherche une propriété identique dans une collection du document.
		protected Properties.Abstract SearchProperty(Properties.Abstract item, bool style, bool selected)
		{
			UndoableList properties = this.document.Modifier.PropertyList(style, selected);
			foreach ( Properties.Abstract property in properties )
			{
				if ( property == item )  continue;  // soi-même ?
				if ( property.Type != item.Type )  continue;

				// Lorsqu'on cherche à libèrer un style (FreeProperty), il faut chercher
				// une propriété automatique équivalente au style (item), mais sans tenir
				// compte du nom du style.
				property.IsCompareStyleName = false;
				bool eq = property.Compare(item);
				property.IsCompareStyleName = true;
				if ( eq )  return property;
			}
			return null;
		}


		// Détecte si la souris est sur un objet.
		public virtual bool Detect(Point pos)
		{
			return false;
		}

		// Détecte si l'objet est dans un rectangle.
		// partial = false -> toutes les poignées doivent être dans le rectangle
		// partial = true  -> une seule poignée doit être dans le rectangle
		public virtual bool Detect(Drawing.Rectangle rect, bool partial)
		{
			if ( this.isHide )  return false;

			if ( partial )
			{
				foreach ( Handle handle in this.handles )
				{
					if ( handle.Type != HandleType.Primary )  continue;
					if ( rect.Contains(handle.Position) )  return true;
				}
				return false;
			}
			else
			{
				return rect.Contains(this.BoundingBoxDetect);
			}
		}

		
		// Détecte si la souris est sur l'objet pour l'éditer.
		public virtual bool DetectEdit(Point pos)
		{
			return false;
		}


		// Donne le contenu du menu contextuel.
		public virtual void ContextMenu(System.Collections.ArrayList list, Point pos, int handleRank)
		{
		}

		// Exécute une commande du menu contextuel.
		public virtual void ContextCommand(string cmd, Point pos, int handleRank)
		{
		}


		// Début de la création d'un objet.
		public virtual void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			drawingContext.ConstrainFixStarting(pos);
		}

		// Déplacement pendant la création d'un objet.
		public virtual void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			this.dirtyBbox = true;
		}

		// Fin de la création d'un objet.
		public virtual void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			drawingContext.ConstrainDelStarting();
		}

		// Indique si la création de l'objet est terminée.
		public virtual bool CreateIsEnding(DrawingContext drawingContext)
		{
			return true;
		}

		// Indique si l'objet peut exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public virtual bool CreateIsExist(DrawingContext drawingContext)
		{
			return true;
		}

		// Termine la création de l'objet. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public virtual bool CreateEnding(DrawingContext drawingContext)
		{
			return false;
		}

		// Retourne un bouton d'action pendant la création.
		public virtual bool CreateAction(int rank, out string cmd, out string name, out string text)
		{
			cmd  = "";
			name = "";
			text = "";
			return false;
		}

		// Indique s'il faut sélectionner l'objet après sa création.
		public virtual bool SelectAfterCreation()
		{
			return false;
		}

		// Indique s'il faut éditer l'objet après sa création.
		public virtual bool EditAfterCreation()
		{
			return false;
		}


		// Crée une instance de l'objet.
		protected abstract Objects.Abstract CreateNewObject(Document document, Objects.Abstract model);

		// Effectue une copie de l'objet courant.
		public bool DuplicateObject(Document document, ref Objects.Abstract newObject)
		{
			newObject = this.CreateNewObject(document, this);
			newObject.CloneObject(this);
			return true;
		}

		// Reprend toutes les caractéristiques d'un objet.
		public virtual void CloneObject(Objects.Abstract src)
		{
			this.handles.Clear();
			int total = src.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle srcHandle = src.Handle(i);
				Handle newHandle = new Handle(this.document);
				srcHandle.CopyTo(newHandle);
				this.handles.Add(newHandle);
			}

			this.isHide         = src.isHide;
			this.selected       = src.selected;
			this.globalSelected = src.globalSelected;
			this.edited         = src.edited;
			this.dirtyBbox      = src.dirtyBbox;
			this.bboxThin       = src.bboxThin;
			this.bboxGeom       = src.bboxGeom;
			this.bboxFull       = src.bboxFull;
			this.name           = src.name;
			this.totalPropertyHandle = src.totalPropertyHandle;

			this.SplitProperties();
		}

		// Adapte un objet qui vient d'être copié.
		public void DuplicateAdapt()
		{
#if false
			if ( this.properties.Count > 0 )
			{
				PropertyName name = this.properties[0] as PropertyName;
				if ( name != null && name.String != "" )
				{
					name.String = Misc.CopyName(name.String);
				}
			}
#endif
		}


		// Dessine la géométrie de l'objet.
		public virtual void DrawGeometry(Graphics graphics, DrawingContext drawingContext)
		{
			// Un objet ne doit jamais être sélectionné ET caché !
			System.Diagnostics.Debug.Assert(!this.selected || !this.isHide);

			if ( drawingContext.IsDrawBoxThin )
			{
				double initialWidth = graphics.LineWidth;
				graphics.LineWidth = 1.0/drawingContext.ScaleX;

				graphics.AddRectangle(this.BoundingBoxThin);
				graphics.RenderSolid(Color.FromARGB(0.5, 0,1,1));

				graphics.LineWidth = initialWidth;
			}

			if ( drawingContext.IsDrawBoxGeom )
			{
				double initialWidth = graphics.LineWidth;
				graphics.LineWidth = 1.0/drawingContext.ScaleX;

				graphics.AddRectangle(this.BoundingBoxGeom);
				graphics.RenderSolid(Color.FromARGB(0.5, 0,1,0));

				graphics.LineWidth = initialWidth;
			}

			if ( drawingContext.IsDrawBoxFull )
			{
				double initialWidth = graphics.LineWidth;
				graphics.LineWidth = 1.0/drawingContext.ScaleX;

				graphics.AddRectangle(this.BoundingBoxFull);
				graphics.RenderSolid(Color.FromARGB(0.5, 1,1,0));

				graphics.LineWidth = initialWidth;
			}
		}

		// Dessine les poignées de l'objet.
		public virtual void DrawHandle(Graphics graphics, DrawingContext drawingContext)
		{
			if ( this.isHide )  return;

			foreach ( Properties.Abstract property in this.properties )
			{
				property.DrawEdit(graphics, drawingContext, this);
			}

			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);

				if ( handle.Type != HandleType.Hide )
				{
					handle.Draw(graphics, drawingContext);
				}
			}
		}

		// Imprime la géométrie de l'objet.
		public virtual void PrintGeometry(Printing.PrintPort port, DrawingContext drawingContext)
		{
		}


		#region OpletSelection
		// Ajoute un oplet pour mémoriser les informations de sélection de l'objet.
		protected void InsertOpletSelection()
		{
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletSelection oplet = new OpletSelection(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		// Mémorise toutes les informations de sélection de l'objet.
		protected class OpletSelection : AbstractOplet
		{
			public OpletSelection(Objects.Abstract host)
			{
				this.host = host;

				this.isHide         = host.isHide;
				this.selected       = host.selected;
				this.edited         = host.edited;
				this.globalSelected = host.globalSelected;

				this.list = new System.Collections.ArrayList();
				foreach ( Handle hObj in this.host.handles )
				{
					Handle hCopy = new Handle(host.document);
					hObj.CopyTo(hCopy);
					this.list.Add(hCopy);
				}
			}

			protected void Swap()
			{
				this.host.document.Notifier.NotifyArea(this.host.BoundingBox);

				Misc.Swap(ref this.isHide,         ref host.isHide        );
				Misc.Swap(ref this.selected,       ref host.selected      );
				Misc.Swap(ref this.edited,         ref host.edited        );
				Misc.Swap(ref this.globalSelected, ref host.globalSelected);

				int total = this.list.Count;
				for ( int i=0 ; i<total ; i++ )
				{
					Handle hObj  = this.host.handles[i] as Handle;
					Handle hCopy = this.list[i] as Handle;
					hObj.SwapSelection(hCopy);
				}

				this.host.document.Modifier.DirtyCounters();
				this.host.document.Notifier.NotifyArea(this.host.BoundingBox);
				this.host.document.Notifier.NotifySelectionChanged();
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

			protected Objects.Abstract				host;
			protected bool							isHide;
			protected bool							selected;
			protected bool							edited;
			protected bool							globalSelected;
			protected System.Collections.ArrayList	list;
		}
		#endregion


		#region OpletGeometry
		// Ajoute un oplet pour mémoriser la géométrie de l'objet.
		protected void InsertOpletGeometry()
		{
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletGeometry oplet = new OpletGeometry(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		// Mémorise toutes les informations sur la géométrie de l'objet.
		protected class OpletGeometry : AbstractOplet
		{
			public OpletGeometry(Objects.Abstract host)
			{
				this.host = host;
				this.list = new System.Collections.ArrayList();

				foreach ( Handle handle in this.host.handles )
				{
					Handle copy = new Handle(host.document);
					handle.CopyTo(copy);
					this.list.Add(copy);
				}
			}

			protected void Swap()
			{
				this.host.document.Notifier.NotifyArea(this.host.BoundingBox);

				System.Collections.ArrayList temp = this.host.handles;
				this.host.handles = this.list;
				this.list = temp;

				this.host.dirtyBbox = true;
				this.host.document.Notifier.NotifyArea(this.host.BoundingBox);
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

			protected Objects.Abstract				host;
			protected System.Collections.ArrayList	list;
		}
		#endregion

		
		#region OpletName
		// Ajoute un oplet pour mémoriser le nom de l'objet.
		protected void InsertOpletName()
		{
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletName oplet = new OpletName(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		// Mémorise le nom de l'objet.
		protected class OpletName : AbstractOplet
		{
			public OpletName(Objects.Abstract host)
			{
				this.host = host;
				this.name = host.name;
			}

			protected void Swap()
			{
				string temp = host.name;
				host.name = this.name;  // host.name <-> this.name
				this.name = temp;

				if ( this.host is Objects.Page || this.host is Objects.Layer )
				{
					this.host.document.Notifier.NotifyPagesChanged();
					this.host.document.Notifier.NotifyLayersChanged();
				}
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

			protected Objects.Abstract				host;
			protected string						name;
		}
		#endregion

		
		#region Serialization
		// Sérialise l'objet.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("UniqueId", this.uniqueId);
			info.AddValue("Name", this.name);
			info.AddValue("Properties", this.properties);
			info.AddValue("Handles", this.handles);
			info.AddValue("TotalPropertyHandle", this.totalPropertyHandle);
			info.AddValue("Objects", this.objects);
		}

		// Constructeur qui désérialise l'objet.
		protected Abstract(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;
			this.uniqueId = info.GetInt32("UniqueId");
			this.name = info.GetString("Name");
			this.properties = (UndoableList) info.GetValue("Properties", typeof(UndoableList));
			this.handles = (System.Collections.ArrayList) info.GetValue("Handles", typeof(System.Collections.ArrayList));
			this.totalPropertyHandle = info.GetInt32("TotalPropertyHandle");
			this.objects = (UndoableList) info.GetValue("Objects", typeof(UndoableList));
		}

		// Adapte l'objet après une désérialisation.
		public void ReadFinalize()
		{
			foreach ( Properties.Abstract property in this.properties )
			{
				property.Owners.Add(this);
			}
		}
		#endregion


		protected Document						document;
		protected int							uniqueId;
		protected bool							isHilite = false;
		protected bool							isHide = false;
		protected bool							selected = false;
		protected bool							edited = false;
		protected bool							globalSelected = false;
		protected bool							dirtyBbox = true;
		protected Drawing.Rectangle				bboxThin = Drawing.Rectangle.Empty;
		protected Drawing.Rectangle				bboxGeom = Drawing.Rectangle.Empty;
		protected Drawing.Rectangle				bboxFull = Drawing.Rectangle.Empty;

		protected string						name = "";
		protected UndoableList					properties;
		protected System.Collections.ArrayList	handles = new System.Collections.ArrayList();
		protected UndoableList					objects = null;
		protected int							totalPropertyHandle = 0;
	}
}
