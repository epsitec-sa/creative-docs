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

			if ( this.document != null && this.document.Modifier != null )
			{
				this.uniqueId = this.document.GetNextUniqueObjectId();
			}

			this.properties = new UndoableList(this.document, UndoableListType.PropertiesInsideObject);
			this.surfaceAnchor = new SurfaceAnchor(this.document, this);
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
			if ( model != null && model.aggregate != null )
			{
				this.aggregate = model.aggregate;
				this.document.Modifier.AggregateToDocument(this.aggregate);
			}

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
				case "ObjectPoly":       obj = new Poly(document, model);       break;
				case "ObjectBezier":     obj = new Bezier(document, model);     break;
				case "ObjectRegular":    obj = new Regular(document, model);    break;
				case "ObjectSurface":    obj = new Surface(document, model);    break;
				case "ObjectVolume":     obj = new Volume(document, model);     break;
				case "ObjectTextLine":   obj = new TextLine(document, model);   break;
				case "ObjectTextBox":    obj = new TextBox(document, model);    break;
				case "ObjectImage":      obj = new Image(document, model);      break;
				case "ObjectDimension":  obj = new Dimension(document, model);  break;
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


		// Direction de l'objet.
		public double Direction
		{
			get
			{
				return this.direction;
			}

			set
			{
				if ( this.direction != value )
				{
					this.direction = value;
					this.surfaceAnchor.SetDirty();
				}
			}
		}

		// Donne la surface.
		public SurfaceAnchor SurfaceAnchor
		{
			get
			{
				return this.surfaceAnchor;
			}
		}


		#region HotSpot
		// Retourne la position du point chaud.
		public Point HotSpotPosition
		{
			get
			{
				if ( this.hotSpotRank != -1 && this.hotSpotRank < this.TotalMainHandle )
				{
					Handle handle = this.handles[this.hotSpotRank] as Handle;
					return handle.Position;
				}
				else
				{
					return this.BoundingBoxThin.Center;
				}
			}
		}

		// Utilise le point chaud suivant ou précédent.
		public void ChangeHotSpot(int dir)
		{
			for ( int i=0 ; i<1000 ; i++ )
			{
				if ( dir > 0 )
				{
					this.hotSpotRank ++;
					if ( this.hotSpotRank >= this.TotalMainHandle )
					{
						this.hotSpotRank = -1;
					}
				}
				else
				{
					this.hotSpotRank --;
					if ( this.hotSpotRank < -1 )
					{
						this.hotSpotRank = this.TotalMainHandle-1;
					}
				}

				if ( this.hotSpotRank == -1 )  break;

				Handle handle = this.handles[this.hotSpotRank] as Handle;
				if ( handle.Type != HandleType.Bezier    &&
					 handle.Type != HandleType.Secondary &&
					 handle.Type != HandleType.Hide      )  break;
			}
		}
		#endregion


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
					this.SetDirtyBbox();
				}
			}
		}

		// Met à jour toutes les poignées des propriétés.
		public void HandlePropertiesUpdate()
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
					handle.Position = property.GetHandlePosition(this, handle.PropertyRank);
					this.SetDirtyBbox();
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
			this.SetDirtyBbox();
		}

		// Insère une poignée.
		public void HandleInsert(int rank, Handle handle)
		{
			this.handles.Insert(rank, handle);
			this.SetDirtyBbox();
		}

		// Supprime une poignée.
		public void HandleDelete(int rank)
		{
			this.handles.RemoveAt(rank);
			this.SetDirtyBbox();
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
			double min = 1000000.0;
			int rank = -1;
			for ( int i=total-1 ; i>=0 ; i-- )
			{
				if ( this.Handle(i).Detect(pos) )
				{
					double distance = Point.Distance(this.Handle(i).Position, pos);
					if ( distance < min && System.Math.Abs(distance-min) > 0.00001 )
					{
						min = distance;
						rank = i;
					}
				}
			}
			return rank;
		}

		// Début du déplacement d'une poignée.
		public virtual void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank < this.TotalMainHandle )  // poignée de l'objet, pas propriété ?
			{
				this.InsertOpletGeometry();
			}
		}

		// Déplace une poignée.
		public virtual void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				drawingContext.SnapPos(ref pos);

				Handle handle = this.Handle(rank);
				handle.Position = pos;

				if ( handle.PropertyType != Properties.Type.None )
				{
					Properties.Abstract property = this.Property(handle.PropertyType);
					property.SetHandlePosition(this, handle.PropertyRank, pos);
				}

				this.SetDirtyBbox();
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Fin du déplacement d'une poignée.
		public virtual void MoveHandleEnding(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				drawingContext.MagnetClearStarting();
				this.document.Modifier.TextInfoModif = "";
			}
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

			this.HandlePropertiesUpdate();
			this.SetDirtyBbox();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Début du déplacement global de l'objet.
		public virtual void MoveGlobalStarting()
		{
			this.InsertOpletGeometry();

			foreach ( Handle handle in this.handles )
			{
				handle.InitialPosition = handle.Position;
			}

			this.initialDirection = this.direction;
		}

		// Effectue le déplacement global de l'objet.
		// Un objet désélectionné est déplacé entièrement, car il s'agit forcément
		// du fils d'un objet sélectionné.
		public virtual void MoveGlobalProcess(Selector selector)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			bool allHandle = !this.IsSelected;
			foreach ( Handle handle in this.handles )
			{
				if ( allHandle || handle.IsVisible )
				{
					handle.Position = selector.DotTransform(handle.InitialPosition);
				}
			}

			this.direction = this.initialDirection + selector.GetTransformAngle;

			this.SetDirtyBbox();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Début du déplacement global des propriétés de l'objet.
		public virtual void MoveGlobalStartingProperties()
		{
			for ( int i=0 ; i<this.properties.Count ; i++ )
			{
				Properties.Abstract property = this.properties[i] as Properties.Abstract;
				if ( property.IsStyle )  continue;

				property.MoveGlobalStarting();
			}
		}

		// Effectue le déplacement global des propriétés de l'objet.
		public void MoveGlobalProcessProperties(Selector selector)
		{
			for ( int i=0 ; i<this.properties.Count ; i++ )
			{
				Properties.Abstract property = this.properties[i] as Properties.Abstract;
				if ( property.IsStyle )  continue;

				property.MoveGlobalProcess(selector);
			}
		}

		// Aligne l'objet sur la grille.
		public virtual void AlignGrid(DrawingContext drawingContext)
		{
			this.InsertOpletGeometry();
			this.document.Notifier.NotifyArea(this.BoundingBox);

			foreach ( Handle handle in this.handles )
			{
				if ( handle.IsVisible )
				{
					Point pos = handle.Position;
					drawingContext.SnapGridForce(ref pos);
					handle.Position = pos;
				}
			}

			this.SetDirtyBbox();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Texte des informations de modification pour un objet segment de ligne.
		protected void TextInfoModifLine()
		{
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			double angle = Point.ComputeAngleDeg(this.Handle(0).Position, this.Handle(1).Position);
			string text = string.Format(Res.Strings.Object.Abstract.InfoLine, this.document.Modifier.RealToString(len), this.document.Modifier.AngleToString(angle));
			this.document.Modifier.TextInfoModif = text;
		}

		// Texte des informations de modification pour un objet rectangulaire.
		protected void TextInfoModifRect()
		{
			double width  = 0.0;
			double height = 0.0;

			if ( this.handles.Count < 4 )
			{
				Point p1 = this.Handle(0).Position;
				Point p2 = this.Handle(1).Position;
				width = System.Math.Abs(p1.X-p2.X);
				height = System.Math.Abs(p1.Y-p2.Y);
			}
			else
			{
				Point p1 = this.Handle(0).Position;
				Point p2 = this.Handle(1).Position;
				Point p3 = this.Handle(2).Position;
				Point p4 = this.Handle(3).Position;
				if ( Geometry.IsRectangular(p1, p2, p3, p4) )
				{
					width  = (Point.Distance(p1,p4)+Point.Distance(p3,p2))/2.0;
					height = (Point.Distance(p1,p3)+Point.Distance(p4,p2))/2.0;
				}
			}

			if ( width == 0.0 && height == 0.0 )
			{
				this.document.Modifier.TextInfoModif = "";
			}
			else
			{
				string text = string.Format(Res.Strings.Object.Abstract.InfoRectangle, this.document.Modifier.RealToString(width), this.document.Modifier.RealToString(height));
				this.document.Modifier.TextInfoModif = text;
			}
		}

		// Texte des informations de modification pour un objet circulaire.
		protected void TextInfoModifCircle()
		{
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			string text = string.Format(Res.Strings.Object.Abstract.InfoCircle, this.document.Modifier.RealToString(len));
			this.document.Modifier.TextInfoModif = text;
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

		// Coupe le texte sélectionné pendant l'édition.
		public virtual bool EditCut()
		{
			return false;
		}

		// Copie le texte sélectionné pendant l'édition.
		public virtual bool EditCopy()
		{
			return false;
		}

		// Met en gras pendant l'édition.
		public virtual bool EditBold()
		{
			return false;
		}

		// Met en italique pendant l'édition.
		public virtual bool EditItalic()
		{
			return false;
		}

		// Souligne pendant l'édition.
		public virtual bool EditUnderlined()
		{
			return false;
		}

		// Colle du texte pendant l'édition.
		public virtual bool EditPaste()
		{
			return false;
		}

		// Sélectionne tout le texte pendant l'édition.
		public virtual bool EditSelectAll()
		{
			return false;
		}

		// Insère un glyphe dans le pavé en édition.
		public virtual bool EditInsertGlyph(string text)
		{
			return false;
		}

		// Donne la fonte actullement utilisée.
		public virtual string EditGetFontName()
		{
			return "";
		}

		// Donne la zone contenant le curseur d'édition.
		public virtual Drawing.Rectangle EditCursorBox
		{
			get
			{
				return Drawing.Rectangle.Empty;
			}
		}

		// Donne la zone contenant le texte sélectionné.
		public virtual Drawing.Rectangle EditSelectBox
		{
			get
			{
				return Drawing.Rectangle.Empty;
			}
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
			this.surfaceAnchor.SetDirty();
		}

		// Rectangle englobant l'objet.
		public Drawing.Rectangle BoundingBox
		{
			get
			{
				if ( this.IsSelected || this.isCreating )
				{
					return this.BoundingBoxFull;
				}
				else
				{
					return this.BoundingBoxGeom;
				}
			}
		}

		// Rectangle englobant l'objet complet ou partiel.
		public Drawing.Rectangle BoundingBoxPartial
		{
			get
			{
				if ( this.allSelected )
				{
					return this.BoundingBoxDetect;
				}
				else
				{
					Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
					int total = this.TotalHandle;
					for ( int i=0 ; i<total ; i++ )
					{
						Handle handle = this.Handle(i);
						if ( handle.PropertyType != Properties.Type.None )  break;
						if ( !handle.IsVisible )  continue;
						if ( handle.Type == HandleType.Bezier )  continue;
						bbox.MergeWith(handle.Position);
					}
					Properties.Line line = this.PropertyLineMode;
					if ( line != null )
					{
						bbox.Inflate(line.Width/2.0);
					}
					return bbox;
				}
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

		// Calcule les 2 rectangles pour SurfaceAnchor, qui sont les bbox
		// (thin et geom) de l'objet lorsqu'il n'est pas tourné.
		public void UpdateSurfaceBox(out Drawing.Rectangle surfThin, out Drawing.Rectangle surfGeom)
		{
			if ( this.direction == 0.0 )
			{
				surfThin = this.bboxThin;
				surfGeom = this.bboxGeom;
			}
			else
			{
				Drawing.Rectangle initThin = this.bboxThin;
				Drawing.Rectangle initGeom = this.bboxGeom;
				Drawing.Rectangle initFull = this.bboxFull;

				this.document.IsSurfaceRotation = true;
				this.document.SurfaceRotationAngle = -this.direction;  // comme si droit
				this.UpdateBoundingBox();
				this.document.IsSurfaceRotation = false;
				this.document.SurfaceRotationAngle = 0.0;
				surfThin = this.bboxThin;
				surfGeom = this.bboxGeom;

				this.bboxThin = initThin;
				this.bboxGeom = initGeom;
				this.bboxFull = initFull;
			}
		}

		// Constuit les formes de l'objet.
		public virtual Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			return null;
		}

		// Calcule le rectangle englobant l'objet. Chaque objet se charge de
		// ce calcul, selon sa géométrie, l'épaisseur de son trait, etc.
		// Il faut calculer :
		//	this.bboxThin  boîte selon la géométrie de l'objet, sans les traits
		//	this.bboxGeom  boîte selon la géométrie de l'objet, avec les traits
		//	this.bboxFull  boîte complète lorsque l'objet est sélectionné
		protected void UpdateBoundingBox()
		{
			Shape[] shapes = this.ShapesBuild(null, null, false);
			this.ComputeBoundingBox(shapes);

			for ( int i=0 ; i<this.TotalHandle ; i++ )
			{
				this.InflateBoundingBox(this.Handle(i).Position, true);
			}
		}

		// Calcule toutes les bbox de l'objet en fonction des formes.
		protected void ComputeBoundingBox(params Shape[] shapes)
		{
			if ( shapes == null )  return;

			this.bboxThin = Drawing.Rectangle.Empty;
			this.bboxGeom = Drawing.Rectangle.Empty;

			foreach ( Shape shape in shapes )
			{
				if ( shape == null || shape.Path == null )
				{
					continue;
				}
				
				if ( shape.Aspect != Aspect.Normal       &&
					 shape.Aspect != Aspect.Additional   &&
					 shape.Aspect != Aspect.InvisibleBox )
				{
					continue;
				}

				Drawing.Rectangle bbox = Geometry.ComputeBoundingBox(shape.Path);
				this.bboxThin.MergeWith(bbox);

				double width = 0.0;

				if ( shape.Type == Type.Stroke )
				{
					width += Properties.Gradient.InflateBoundingBoxWidth(shape);
					width += Properties.Line.InflateBoundingBoxWidth(shape);
					width *= Properties.Line.InflateBoundingBoxFactor(shape);
				}

				if ( shape.Type == Type.Surface )
				{
					width += Properties.Gradient.InflateBoundingBoxWidth(shape);
				}

				bbox.Inflate(width);
				this.bboxGeom.MergeWith(bbox);
			}

			this.bboxFull = this.bboxGeom;

			if ( !this.document.IsSurfaceRotation )
			{
				foreach ( Shape shape in shapes )
				{
					if ( shape == null )  continue;
					if ( shape.Aspect != Aspect.Normal )  continue;

					bool initialLineUse = this.surfaceAnchor.LineUse;

					if ( shape.Type == Type.Surface && shape.PropertySurface != null )
					{
						this.surfaceAnchor.LineUse = false;
						shape.PropertySurface.InflateBoundingBox(this.surfaceAnchor, ref this.bboxFull);
					}

					if ( shape.Type == Type.Stroke && shape.PropertySurface != null )
					{
						this.surfaceAnchor.LineUse = true;
						shape.PropertySurface.InflateBoundingBox(this.surfaceAnchor, ref this.bboxFull);
					}

					this.surfaceAnchor.LineUse = initialLineUse;
				}
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


		// Gestion de la marque.
		public bool Mark
		{
			get { return this.mark; }
			set { this.mark = value; }
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
		public void Select(bool select, bool edit)
		{
			this.InsertOpletSelection();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);

			this.selected = select;
			this.edited = edit;
			this.globalSelected = false;
			this.allSelected = true;
			this.SplitProperties();

			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( handle.PropertyType != Properties.Type.None )  break;

				handle.IsVisible = select && !edit;
				handle.IsGlobalSelected = false;
			}
			this.HandlePropertiesUpdate();
			this.SetDirtyBbox();

			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Notifier.NotifySelectionChanged();
		}

		// Sélectionne toutes les poignées de l'objet dans un rectangle.
		public virtual void Select(Drawing.Rectangle rect)
		{
			this.InsertOpletSelection();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);

			int sel = 0;
			int total = this.TotalMainHandle;
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
			this.allSelected = (sel == total);
			this.HandlePropertiesUpdate();
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
			this.HandlePropertiesUpdate();

			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// Indique si l'objet est en cours de création.
		public bool IsCreating
		{
			get { return this.isCreating; }
		}

		// Indique si l'objet est sélectionné.
		public bool IsSelected
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
			if ( model != null )
			{
				Properties.Abstract original = model.Property(type);
				if ( original != null && original.IsStyle )
				{
					// L'objet utilise directement la propriété du style, et
					// surtout pas une copie !
					original.Owners.Add(this);  // l'objet est un propriétaire de l'original
					this.properties.Add(original);  // ajoute dans la liste de l'objet
					return;
				}
			}

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
				if ( original != null )
				{
					original.CopyTo(property);
				}
			}

			if ( floating )
			{
				property.IsFloating = true;
				return;
			}

			property.IsSelected = this.selected;

			Properties.Abstract idem = this.SearchProperty(property, property.IsSelected);
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

		// Transfert toutes les propriétés d'un ObjectMemory source qui n'existent
		// pas dans l'objet courant.
		public void PropertiesXferMemory(Objects.Abstract src)
		{
			bool ie = this.document.Modifier.OpletQueueEnable;
			this.document.Modifier.OpletQueueEnable = false;

			for ( int i=0 ; i<src.properties.Count ; i++ )
			{
				Properties.Abstract property = src.properties[i] as Properties.Abstract;
				if ( this.ExistProperty(property.Type) )  continue;

				property.Owners.Clear();
				property.Owners.Add(this);  // l'objet est un propriétaire de cette propriété
				this.properties.Add(property);  // ajoute dans la liste de l'objet
			}

			this.document.Modifier.OpletQueueEnable = ie;
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

		// Retourne la liste de toutes les types utilisant un style.
		public Properties.Type[] PropertiesStyle()
		{
			int total = 0;
			foreach ( Properties.Abstract property in this.properties )
			{
				if ( property.IsStyle )  total ++;
			}

			Properties.Type[] list = new Properties.Type[total];
			int i = 0;
			foreach ( Properties.Abstract property in this.properties )
			{
				if ( property.IsStyle )  list[i++] = property.Type;
			}

			return list;
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

		public Properties.Line PropertyLineDimension
		{
			get { return this.Property(Properties.Type.LineDimension) as Properties.Line; }
		}

		public Properties.Gradient PropertyFillGradient
		{
			get { return this.Property(Properties.Type.FillGradient) as Properties.Gradient; }
		}

		public Properties.Gradient PropertyFillGradientVT
		{
			get { return this.Property(Properties.Type.FillGradientVT) as Properties.Gradient; }
		}

		public Properties.Gradient PropertyFillGradientVL
		{
			get { return this.Property(Properties.Type.FillGradientVL) as Properties.Gradient; }
		}

		public Properties.Gradient PropertyFillGradientVR
		{
			get { return this.Property(Properties.Type.FillGradientVR) as Properties.Gradient; }
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

		public Properties.Arrow PropertyDimensionArrow
		{
			get { return this.Property(Properties.Type.DimensionArrow) as Properties.Arrow; }
		}

		public Properties.Dimension PropertyDimension
		{
			get { return this.Property(Properties.Type.Dimension) as Properties.Dimension; }
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

		public Properties.Surface PropertySurface
		{
			get { return this.Property(Properties.Type.Surface) as Properties.Surface; }
		}

		public Properties.Volume PropertyVolume
		{
			get { return this.Property(Properties.Type.Volume) as Properties.Volume; }
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
		public void PropertiesList(System.Collections.ArrayList list,
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
		public void PropertiesList(System.Collections.ArrayList list,
								   Objects.Abstract filter)
		{
			foreach ( Properties.Abstract property in this.properties )
			{
				if ( property.Type == Properties.Type.ModColor )  continue;
				if ( property.Type == Properties.Type.BackColor )  continue;
				if ( filter != null && !filter.ExistingProperty(property.Type) )  continue;
				list.Add(property);
			}
		}

		// Cherche si l'objet utilise une propriété.
		public bool PropertyExist(Properties.Abstract search)
		{
			return this.properties.Contains(search);
		}

		// Indique si une impression complexe est nécessaire.
		public bool IsComplexPrinting
		{
			get
			{
				foreach ( Properties.Abstract property in this.properties )
				{
					if ( property.IsComplexPrinting )  return true;
				}
				return false;
			}
		}

		// Reprend toutes les propriétés d'un objet source.
		public void PickerProperties(Objects.Abstract model)
		{
			if ( this is Objects.Memory )
			{
				this.AggregateFree();

				foreach ( Properties.Abstract mp in model.properties )
				{
					Properties.Abstract property = this.Property(mp.Type);
					if ( property == null )  continue;

					if ( mp.IsStyle )
					{
						this.ChangeProperty(mp);
					}
					else
					{
						Properties.Abstract style = Properties.Abstract.NewProperty(this.document, mp.Type);
						mp.CopyTo(style);
						this.ChangeProperty(style);
					}
				}
				this.Aggregate = model.aggregate;
			}
			else
			{
				foreach ( Properties.Abstract mp in model.properties )
				{
					if ( !mp.IsStyle )
					{
						if ( model.IsSelected )
						{
							this.UseProperty(mp);
						}
						else
						{
							this.PickerProperty(mp);
						}
					}
				}
				this.AggregateUse(model.aggregate);
				this.Aggregate = model.aggregate;
				this.SetDirtyBbox();
			}
		}

		// Modifie la propriété PolyClose de l'objet en cours de création.
		// Rappel: un objet en cours de création n'est pas sélectionné.
		public void ChangePropertyPolyClose(bool close)
		{
			Properties.Bool pb = this.PropertyPolyClose;
			if ( pb == null )  return;
			if ( pb.BoolValue == close )  return;  // on ne veut rien changer

			bool oqe;
			UndoableList properties = this.document.PropertiesAuto;
			foreach ( Properties.Abstract property in properties )
			{
				Properties.Bool existing = property as Properties.Bool;
				if ( existing == null )  continue;
				if ( existing == pb )  continue;  // soi-même ?

				if ( existing.BoolValue == close )
				{
					oqe = this.document.Modifier.OpletQueueEnable;
					this.document.Modifier.OpletQueueEnable = true;

					this.MergeProperty(existing, pb);
					
					this.document.Modifier.OpletQueueEnable = oqe;
					return;
				}
			}

			oqe = this.document.Modifier.OpletQueueEnable;
			this.document.Modifier.OpletQueueEnable = true;

			Properties.Abstract dst = Properties.Abstract.NewProperty(this.document, pb.Type);
			pb.CopyTo(dst);
			Properties.Bool npb = dst as Properties.Bool;
			npb.BoolValue = close;
			this.document.Modifier.PropertyAdd(dst);

			this.MergeProperty(npb, pb);

			this.document.Modifier.OpletQueueEnable = oqe;
		}

		// Utilise un style donné.
		public void UseProperty(Properties.Abstract style)
		{
			Properties.Abstract actual = this.Property(style.Type);
			if ( actual == null )  return;

			this.MergeProperty(style, actual);
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Prend une propriété à un objet désélectionné.
		protected void PickerProperty(Properties.Abstract property)
		{
			Properties.Abstract src = this.Property(property.Type);
			if ( src == null )  return;

			Properties.Abstract dst = Properties.Abstract.NewProperty(this.document, property.Type);
			property.CopyTo(dst);
			dst.IsSelected = true;  // nouvel état
			this.document.Modifier.PropertyAdd(dst);
			this.MergeProperty(dst, src);
			this.document.Notifier.NotifyArea(this.BoundingBox);
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
			Properties.Abstract dst = this.SearchProperty(property, selected);
			if ( dst == null )  // la propriété détachée sera seule ?
			{
				// Crée une nouvelle instance pour la propriété dans son nouvel
				// état, car elle est seule à être comme cela.
				dst = Properties.Abstract.NewProperty(this.document, property.Type);
				property.CopyTo(dst);
				dst.IsSelected = selected;  // nouvel état
				dst.IsStyle = false;
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
		protected Properties.Abstract SearchProperty(Properties.Abstract item, bool selected)
		{
			UndoableList properties = this.document.Modifier.PropertyList(selected);
			
			foreach ( Properties.Abstract property in properties )
			{
				if ( property == item )  continue;  // soi-même ?
				if ( property.Type != item.Type )  continue;

				if ( property.Compare(item) )  return property;
			}
			return null;
		}


		// Agrégat utilisé par l'objet.
		public Properties.Aggregate Aggregate
		{
			get
			{
				return this.aggregate;
			}

			set
			{
				if ( this.aggregate != value )
				{
					this.InsertOpletAggregate();
					this.aggregate = value;
					this.SetDirtyBbox();
					this.document.Notifier.NotifyArea(this.BoundingBox);
				}
			}
		}

		// Nom de l'agrégat utilisé par l'objet.
		public string AggregateName
		{
			get
			{
				if ( this.aggregate == null )  return "";
				return this.aggregate.AggregateName;
			}
		}

		// Adapte l'objet en fonction des changements dans l'agrégat.
		public void AggregateAdapt(Properties.Aggregate agg)
		{
			if ( agg.IsUsedByObject(this) )
			{
				this.AggregateFree();
				this.AggregateUse(this.aggregate);
			}
		}

		// Utilise toutes les propriétés d'un agrégat.
		public void AggregateUse(Properties.Aggregate agg)
		{
			if ( agg != null )
			{
				if ( this is Objects.Memory )
				{
					for ( int i=0 ; i<this.properties.Count ; i++ )
					{
						Properties.Abstract property = this.properties[i] as Properties.Abstract;
						Properties.Abstract style = agg.PropertyDeep(property.Type);
						if ( style != null )
						{
							this.ChangeProperty(style);
						}
					}
				}
				else
				{
					for ( int i=0 ; i<this.properties.Count ; i++ )
					{
						Properties.Abstract property = this.properties[i] as Properties.Abstract;
						Properties.Abstract style = agg.PropertyDeep(property.Type);
						if ( style != null )
						{
							this.UseProperty(style);
						}
					}
				}
			}
		}

		// Libère toutes les propriétés d'un agrégat.
		public void AggregateFree()
		{
			for ( int i=0 ; i<this.properties.Count ; i++ )
			{
				Properties.Abstract property = this.properties[i] as Properties.Abstract;
				this.AggregateFree(property);
			}
		}

		// Libère une propriété d'un agrégat.
		public void AggregateFree(Properties.Abstract property)
		{
			if ( this is Objects.Memory )
			{
				if ( property.IsStyle )
				{
					Properties.Abstract free = Properties.Abstract.NewProperty(this.document, property.Type);
					property.CopyTo(free);
					free.IsOnlyForCreation = true;
					free.IsStyle = false;
					this.ChangeProperty(free);
				}
			}
			else
			{
				if ( property.IsStyle )
				{
					this.FreeProperty(property);
				}
			}
		}

		// Libère toutes les propriétés d'un agrégat qui sera détruit.
		public void AggregateDelete(Properties.Aggregate agg)
		{
			if ( agg == this.aggregate )
			{
				this.AggregateFree();
				this.Aggregate = null;
			}
			else
			{
				this.AggregateFree();
				if ( this.aggregate != null )
				{
					this.AggregateUse(this.aggregate);
				}
			}
		}


		// Détecte si la souris est sur un objet.
		public bool Detect(Point pos)
		{
			if ( this.isHide )  return false;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;

			Drawing.Rectangle bbox = this.BoundingBox;
			bbox.Inflate(context.MinimalWidth);
			if ( !bbox.Contains(pos) )  return false;

			Shape[] shapes = this.ShapesBuild(null, null, false);
			return context.Drawer.Detect(pos, context, shapes);
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
					if ( handle.Type != HandleType.Primary  &&
						 handle.Type != HandleType.Starting )
					{
						continue;
					}

					if ( rect.Contains(handle.Position) )
					{
						return true;
					}
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
			drawingContext.ConstrainFlush();
			drawingContext.ConstrainAddHV(pos);
			drawingContext.MagnetFixStarting(pos);
		}

		// Déplacement pendant la création d'un objet.
		public virtual void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			this.SetDirtyBbox();
		}

		// Fin de la création d'un objet.
		public virtual void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			drawingContext.ConstrainDelStarting();
			drawingContext.MagnetClearStarting();
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


		// Ajoute toutes les fontes utilisées par l'objet dans une liste.
		public virtual void FillFontFaceList(System.Collections.ArrayList list)
		{
		}

		// Ajoute tous les caractères utilisés par l'objet dans une table.
		public virtual void FillOneCharList(System.Collections.Hashtable table)
		{
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

			this.isHide              = src.isHide;
			this.selected            = src.selected;
			this.globalSelected      = src.globalSelected;
			this.allSelected         = src.allSelected;
			this.edited              = src.edited;
			this.dirtyBbox           = src.dirtyBbox;
			this.bboxThin            = src.bboxThin;
			this.bboxGeom            = src.bboxGeom;
			this.bboxFull            = src.bboxFull;
			this.name                = src.name;
			this.totalPropertyHandle = src.totalPropertyHandle;
			this.mark                = src.mark;
			this.direction           = src.direction;

			this.surfaceAnchor.SetDirty();
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
		public void DrawGeometry(IPaintPort port, DrawingContext drawingContext)
		{
			// Un objet ne doit jamais être sélectionné ET caché !
			System.Diagnostics.Debug.Assert(!this.selected || !this.isHide);

			// Dessine les formes.
			Shape[] shapes = this.ShapesBuild(port, drawingContext, false);
			if ( shapes != null )
			{
				drawingContext.Drawer.DrawShapes(port, drawingContext, this, shapes);

				if ( port is Graphics )
				{
					if ( this.IsHilite && drawingContext.IsActive )
					{
						Shape.Hilited(port, shapes);
						drawingContext.Drawer.DrawShapes(port, drawingContext, this, shapes);
					}

					if ( this.IsOverDash(drawingContext) )
					{
						Shape.OverDashed(port, shapes);
						drawingContext.Drawer.DrawShapes(port, drawingContext, this, shapes);
					}
				}
			}

			if ( port is Graphics )
			{
				Graphics graphics = port as Graphics;

				// Dessine les bbox en mode debug.
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
		}

		// Dessine le texte.
		public virtual void DrawText(IPaintPort port, DrawingContext drawingContext)
		{
		}

		// Dessine l'image.
		public virtual void DrawImage(IPaintPort port, DrawingContext drawingContext)
		{
		}

		// Dessine le nom de l'objet.
		public void DrawLabel(Graphics graphics, DrawingContext drawingContext)
		{
			if ( this.isHide )  return;

			Properties.Name name = this.PropertyName;
			if ( name == null || name.String == "" )  return;

			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/drawingContext.ScaleX;

			Drawing.Rectangle bbox = this.BoundingBox;
			graphics.Align(ref bbox);
			bbox.Inflate(0.5/drawingContext.ScaleX);

			string s = TextLayout.ConvertToSimpleText(name.String);
			Point pos = bbox.TopLeft;
			Font font = Misc.GetFont("Tahoma");
			double fs = 9.5/drawingContext.ScaleX;
			double ta = font.GetTextAdvance(s)*fs;
			double fa = font.Ascender*fs;
			double fd = font.Descender*fs;
			double h = fa-fd;
			double m = 2.0/drawingContext.ScaleX;

			Color lineColor = drawingContext.HiliteOutlineColor;
			if ( this.isHilite )  // survolé par la souris ?
			{
				lineColor = Color.FromRGB(1,0,0);  // rouge
			}

			Color textColor = Color.FromBrightness(0);  // noir
			Color shadowColor = Color.FromBrightness(1);  // blanc
			if ( lineColor.GetBrightness() < 0.5 )  // couleur foncée ?
			{
				textColor = Color.FromBrightness(1);  // blanc
				shadowColor = Color.FromBrightness(0);  // noir
			}

			if ( m+ta+m <= bbox.Width )
			{
				Drawing.Rectangle rect = new Drawing.Rectangle(bbox.Left, bbox.Top-h, m+ta+m, h);
				graphics.Align(ref rect);
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(lineColor);

				pos.X += m;
				pos.Y -= fa-1.0/drawingContext.ScaleX;

				graphics.AddText(pos.X+1.0/drawingContext.ScaleX, pos.Y-1.0/drawingContext.ScaleX, s, font, fs);
				graphics.RenderSolid(shadowColor);

				graphics.AddText(pos.X, pos.Y, s, font, fs);
				graphics.RenderSolid(textColor);
			}

			graphics.AddRectangle(bbox);
			graphics.RenderSolid(lineColor);

			graphics.LineWidth = initialWidth;
		}

		// Dessine le nom du style.
		public void DrawAggregate(Graphics graphics, DrawingContext drawingContext)
		{
			if ( this.isHide )  return;

			if ( this.aggregate == null )  return;
			string name = TextLayout.ConvertToSimpleText(this.aggregate.AggregateName);
			if ( name == "" )  return;

			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/drawingContext.ScaleX;

			Drawing.Rectangle bbox = this.BoundingBox;
			graphics.Align(ref bbox);
			bbox.Inflate(0.5/drawingContext.ScaleX);

			Point pos = bbox.BottomLeft;
			Font font = Misc.GetFont("Tahoma");
			double fs = 9.5/drawingContext.ScaleX;
			double ta = font.GetTextAdvance(name)*fs;
			double fa = font.Ascender*fs;
			double fd = font.Descender*fs;
			double h = fa-fd;
			double m = 2.0/drawingContext.ScaleX;

			Color lineColor = drawingContext.HiliteOutlineColor;
			if ( this.isHilite )  // survolé par la souris ?
			{
				lineColor = Color.FromRGB(1.0, 0.8, 0.0);  // jaune-orange
			}

			Color textColor = Color.FromBrightness(0);  // noir
			Color shadowColor = Color.FromBrightness(1);  // blanc
			if ( lineColor.GetBrightness() < 0.5 )  // couleur foncée ?
			{
				textColor = Color.FromBrightness(1);  // blanc
				shadowColor = Color.FromBrightness(0);  // noir
			}

			if ( m+ta+m <= bbox.Width )
			{
				Drawing.Rectangle rect = new Drawing.Rectangle(bbox.Left, bbox.Bottom, m+ta+m, h);
				graphics.Align(ref rect);
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(lineColor);

				pos.X += m;
				pos.Y += m;

				graphics.AddText(pos.X+1.0/drawingContext.ScaleX, pos.Y-1.0/drawingContext.ScaleX, name, font, fs);
				graphics.RenderSolid(shadowColor);

				graphics.AddText(pos.X, pos.Y, name, font, fs);
				graphics.RenderSolid(textColor);
			}

			graphics.AddRectangle(bbox);
			graphics.RenderSolid(lineColor);

			graphics.LineWidth = initialWidth;
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

		// Indique s'il faut dessiner le pointillé forcé lorsqu'il n'y a pas de trait.
		protected bool IsOverDash(DrawingContext context)
		{
			if ( !context.IsActive )
			{
				return false;
			}

			if ( this.isCreating || this.IsHilite )
			{
				return true;
			}

			if ( this.IsSelected )
			{
				return !context.PreviewActive;
			}

			return false;
		}

		// Donne la liste des propriétés qui utilisent des surfaces complexes.
		public System.Collections.ArrayList GetComplexSurfacesPDF(IPaintPort port)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			foreach ( Properties.Abstract property in this.properties )
			{
				for ( int i=0 ; i<2 ; i++ )
				{
					Properties.Abstract surface = null;
					if ( i == 0 )  surface = property as Properties.Gradient;
					if ( i == 1 )  surface = property as Properties.Font;
					if ( surface == null )  continue;

					PDF.Type type = surface.TypeComplexSurfacePDF(port);
					bool isSmooth = surface.IsSmoothSurfacePDF(port);

					if ( type == PDF.Type.None )  continue;
					if ( type == PDF.Type.OpaqueRegular && !isSmooth )  continue;

					list.Add(surface);
				}
			}
			return list;
		}


		// Calcule le scroll éventuel nécessaire pour rendre le cursur visible.
		protected void ComputeAutoScroll(Point c1, Point c2)
		{
			if ( !this.autoScrollOneShot )  return;
			this.autoScrollOneShot = false;

			Drawing.Rectangle view = this.document.Modifier.ActiveViewer.ScrollRectangle;
			Drawing.Rectangle cursor = new Drawing.Rectangle(c1, c2);
			if ( view.Contains(cursor) )  return;

			if ( cursor.Width > view.Width || cursor.Height > view.Height )
			{
				cursor = new Drawing.Rectangle(cursor.Center, cursor.Center);
			}

			Point move = new Point(0,0);

			if ( cursor.Right > view.Right )
			{
				move.X = cursor.Right-view.Right;
			}
			if ( cursor.Left < view.Left )
			{
				move.X = cursor.Left-view.Left;
			}
			if ( cursor.Top > view.Top )
			{
				move.Y = cursor.Top-view.Top;
			}
			if ( cursor.Bottom < view.Bottom )
			{
				move.Y = cursor.Bottom-view.Bottom;
			}

			this.document.Modifier.ActiveViewer.AutoScroll(move);
		}


		// Retourne le chemin géométrique de l'objet pour les constructions
		// magnétiques. Généralement, ce chemin est identique à celui rendu
		// par GetPath, mais certains objets peuvent retourner un chemin plus
		// simple (comme Line, Poly, TextLine, TextBox et Dimension).
		// L'idée est d'ignorer les propriétés Corner et Arrow, par exemple.
		public virtual Path GetMagnetPath()
		{
			return this.GetPath(0);
		}

		// Retourne le chemin géométrique de l'objet.
		public virtual Path GetPath(int rank)
		{
			return null;
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
				this.allSelected    = host.allSelected;

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
				Misc.Swap(ref this.allSelected,    ref host.allSelected   );

				if ( this.list.Count == this.host.handles.Count )
				{
					int total = this.list.Count;
					for ( int i=0 ; i<total ; i++ )
					{
						Handle hObj  = this.host.handles[i] as Handle;
						Handle hCopy = this.list[i] as Handle;
						hObj.SwapSelection(hCopy);
					}
				}
				else
				{
					System.Collections.ArrayList temp = this.host.handles;
					this.host.handles = this.list;
					this.list = temp;
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
			protected bool							allSelected;
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
				this.direction = host.direction;

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

				Misc.Swap(ref this.direction, ref host.direction);

				this.host.SetDirtyBbox();
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
			protected double						direction;
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


		#region OpletAggregate
		// Ajoute un oplet pour mémoriser l'agrégat de l'objet.
		protected void InsertOpletAggregate()
		{
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletAggregate oplet = new OpletAggregate(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		// Mémorise l'agrégat de l'objet.
		protected class OpletAggregate : AbstractOplet
		{
			public OpletAggregate(Objects.Abstract host)
			{
				this.host = host;
				this.aggregate = host.aggregate;
			}

			protected void Swap()
			{
				Properties.Aggregate temp = host.aggregate;
				host.aggregate = this.aggregate;  // host.aggregate <-> this.aggregate
				this.aggregate = temp;
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
			protected Properties.Aggregate			aggregate;
		}
		#endregion


		#region Serialization
		// Sérialise l'objet.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("UniqueId", this.uniqueId);
			info.AddValue("Name", this.name);
			info.AddValue("Properties", this.properties);

			// Ne sérialise que les poignées des objets, sans celles des propriétés.
			System.Collections.ArrayList objHandles = new System.Collections.ArrayList();
			for ( int i=0 ; i<this.TotalMainHandle ; i++ )
			{
				objHandles.Add(this.handles[i]);
			}
			info.AddValue("Handles", objHandles);

			info.AddValue("Objects", this.objects);
			info.AddValue("Direction", this.direction);
			info.AddValue("Aggregate", this.aggregate);
		}

		// Constructeur qui désérialise l'objet.
		protected Abstract(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;
			this.uniqueId = info.GetInt32("UniqueId");
			this.name = info.GetString("Name");
			this.properties = (UndoableList) info.GetValue("Properties", typeof(UndoableList));
			this.surfaceAnchor = new SurfaceAnchor(this.document, this);

			this.handles = (System.Collections.ArrayList) info.GetValue("Handles", typeof(System.Collections.ArrayList));
			this.HandlePropertiesCreate();  // crée les poignées des propriétés

			this.objects = (UndoableList) info.GetValue("Objects", typeof(UndoableList));

			if ( this.document.IsRevisionGreaterOrEqual(1,0,17) )
			{
				this.direction = info.GetDouble("Direction");
			}
			else
			{
				this.direction = 0.0;
			}

			if ( this.document.IsRevisionGreaterOrEqual(1,0,24) )
			{
				this.aggregate = (Properties.Aggregate) info.GetValue("Aggregate", typeof(Properties.Aggregate));
			}
			else
			{
				this.aggregate = null;
			}
		}

		// Adapte l'objet après une désérialisation.
		public void ReadFinalize()
		{
			foreach ( Properties.Abstract property in this.properties )
			{
				property.Owners.Add(this);
			}
		}

		// Vérifie si tous les fichiers existent.
		public virtual void ReadCheckWarnings(Font.FaceInfo[] fonts, System.Collections.ArrayList warnings)
		{
		}

		// Vérifie si toutes les fontes d'un TextLayout existent.
		protected static void ReadCheckFonts(Font.FaceInfo[] fonts, System.Collections.ArrayList warnings, TextLayout textLayout)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			textLayout.FillFontFaceList(list);
			foreach ( string face in list )
			{
				if ( !Abstract.ReadSearchFont(fonts, face) )
				{
					string message = string.Format("La fonte <b>{0}</b> n'existe pas.", face);
					if ( !warnings.Contains(message) )
					{
						warnings.Add(message);
					}
				}
			}
		}

		// Cherche si une fonte existe dans la liste des fontes.
		protected static bool ReadSearchFont(Font.FaceInfo[] fonts, string face)
		{
			foreach ( Font.FaceInfo info in fonts )
			{
				if ( info.IsLatin )
				{
					if ( info.Name == face )  return true;
				}
			}
			return false;
		}
		#endregion


		protected Document						document;
		protected int							uniqueId;
		protected bool							isHilite = false;
		protected bool							isHide = false;
		protected bool							mark = false;
		protected bool							selected = false;
		protected bool							edited = false;
		protected bool							globalSelected = false;
		protected bool							allSelected = false;
		protected bool							isCreating = false;
		protected bool							dirtyBbox = true;
		protected bool							autoScrollOneShot = false;
		protected Drawing.Rectangle				bboxThin = Drawing.Rectangle.Empty;
		protected Drawing.Rectangle				bboxGeom = Drawing.Rectangle.Empty;
		protected Drawing.Rectangle				bboxFull = Drawing.Rectangle.Empty;
		protected int							hotSpotRank = -1;

		protected string						name = "";
		protected UndoableList					properties;
		protected System.Collections.ArrayList	handles = new System.Collections.ArrayList();
		protected UndoableList					objects = null;
		protected int							totalPropertyHandle = 0;
		protected double						direction = 0.0;
		protected double						initialDirection = 0.0;
		protected SurfaceAnchor					surfaceAnchor;
		protected Properties.Aggregate			aggregate = null;
	}
}
