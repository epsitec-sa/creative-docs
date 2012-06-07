using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	public enum DetectEditType
	{
		Out,
		Body,
		HandleFlowPrev,
		HandleFlowNext,
	}

	/// <summary>
	/// La classe Objects.Abstract est la classe de base des objets graphiques.
	/// </summary>
	[System.Serializable()]
	public abstract class Abstract : ISerializable
	{
		public Abstract(Document document, Objects.Abstract model)
		{
			//	Constructeur.
			//	Si document = null, on crée un objet factice, c'est-à-dire
			//	sans propriétés. On utilise un objet factice pour appeler
			//	la méthode ExistingProperty.
			this.document = document;

			if ( this.document != null && this.document.Modifier != null )
			{
				this.uniqueId = this.document.GetNextUniqueObjectId();
			}

			this.properties = new UndoableList(this.document, UndoableListType.PropertiesInsideObject);
			this.aggregates = new UndoableList(this.document, UndoableListType.AggregatesInsideObject);
			this.surfaceAnchor = new SurfaceAnchor(this.document, this);
		}

		public virtual void Dispose()
		{
			foreach ( Properties.Abstract property in this.properties )
			{
				property.ClosePopupInterface (this);
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


		protected virtual bool ExistingProperty(Properties.Type type)
		{
			//	Indique si l'objet a besoin de cette propriété.
			return false;
		}

		protected void CreateProperties(Objects.Abstract model, bool floating)
		{
			//	Crée toutes les propriétés dont l'objet a besoin. Cette méthode est
			//	appelée par les constructeurs de tous les objets.
			System.Diagnostics.Debug.Assert(this.document != null);
			if ( model != null && model.aggregates != null && model.aggregates.Count != 0 )
			{
				foreach ( Properties.Aggregate agg in model.aggregates )
				{
					this.aggregates.Add(agg);
					this.document.Modifier.AggregateToDocument(agg);
				}
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

		public static Objects.Abstract CreateObject(Document document, string name, Objects.Abstract model)
		{
			//	Crée un nouvel objet selon l'outil sélectionné.
			Objects.Abstract obj = null;
			switch ( name )
			{
				case "ObjectLine":       obj = new Line(document, model);       break;
				case "ObjectRectangle":  obj = new Rectangle(document, model);  break;
				case "ObjectCircle":     obj = new Circle(document, model);     break;
				case "ObjectEllipse":    obj = new Ellipse(document, model);    break;
				case "ObjectPoly":       obj = new Poly(document, model);       break;
				case "ObjectFree":       obj = new Free(document, model);       break;
				case "ObjectBezier":     obj = new Bezier(document, model);     break;
				case "ObjectRegular":    obj = new Regular(document, model);    break;
				case "ObjectSurface":    obj = new Surface(document, model);    break;
				case "ObjectVolume":     obj = new Volume(document, model);     break;
				case "ObjectTextLine":   obj = new TextLine(document, model);   break;
				case "ObjectTextLine2":  obj = new TextLine2(document, model);  break;
				case "ObjectTextBox":    obj = new TextBox(document, model);    break;
				case "ObjectTextBox2":   obj = new TextBox2(document, model);   break;
				case "ObjectImage":      obj = new Image(document, model);      break;
				case "ObjectDimension":  obj = new Dimension(document, model);  break;
			}
			System.Diagnostics.Debug.Assert(obj != null);
			return obj;
		}



		public Widget PopupInterfaceFrame
		{
			get
			{
				return this.popupInterfaceFrame;
			}
			set
			{
				this.popupInterfaceFrame = value;
			}
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


		public virtual string IconUri
		{
			//	Nom de l'icône.
			get { return @""; }
		}


		public string Name
		{
			//	Nom de l'objet, utilisé pour les pages et les calques.
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

		public int DebugId
		{
			//	Identificateur utilisé pour le debug.
			get
			{
				if ( this.debugId == 0 )
				{
					this.debugId = Abstract.nextDebugId++;
				}
				
				return this.debugId;
			}
		}

		public double Direction
		{
			//	Direction de l'objet.
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

		public SurfaceAnchor SurfaceAnchor
		{
			//	Donne la surface.
			get
			{
				return this.surfaceAnchor;
			}
		}


		#region HotSpot
		public Point HotSpotPosition
		{
			//	Retourne la position du point chaud.
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

		public void ChangeHotSpot(int dir)
		{
			//	Utilise le point chaud suivant ou précédent.
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


		public virtual Polygon PropertyHandleSupport
		{
			//	Retourne le polygone de support pour les poignées des propriétés.
			get
			{
				return null;
			}
		}


		public int TotalHandle
		{
			//	Nombre total de poignées, avec celles des propriétés.
			get
			{
				return this.handles.Count;
			}
		}

		public int TotalMainHandle
		{
			//	Nombre total de poignées, sans celles des propriétés.
			get
			{
				return this.handles.Count-this.totalPropertyHandle;
			}
		}

		public virtual int CreationLastHandle
		{
			//	Retourne le rang de la dernière poignée créée.
			get
			{
				return this.TotalMainHandle-1;
			}
		}

		public int TotalPropertyHandle
		{
			//	Nombre total de poignées des propriétés.
			get
			{
				return this.totalPropertyHandle;
			}
		}

		protected void HandlePropertiesCreate()
		{
			//	Crée toutes les poignées pour les propriétés.
			foreach ( Properties.Abstract property in this.properties )
			{
				int total = property.TotalHandle(this);
				for ( int i=0 ; i<total ; i++ )
				{
					Handle handle = new Handle(this.document);
					handle.Type = property.GetHandleType (i);
					handle.PropertyType = property.Type;
					handle.PropertyRank = i;
					this.handles.Add(handle);
					this.totalPropertyHandle ++;
					this.SetDirtyBbox();
				}
			}
		}

		public void HandlePropertiesUpdate()
		{
			//	Met à jour toutes les poignées des propriétés.
			bool sel = this.selected && !this.edited && !this.globalSelected && !this.document.Modifier.IsToolShaper;
			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( handle.PropertyType != Properties.Type.None )
				{
					Properties.Abstract property = this.Property(handle.PropertyType);
					if ( property == null )  continue;

					bool isVisible = property.IsHandleVisible(this, handle.PropertyRank) && sel;
					bool isGlobalSelected = this.globalSelected && handle.IsVisible;
					bool isShaperDeselected = false;
					handle.Modify(isVisible, isGlobalSelected, false, isShaperDeselected);

					handle.Position = property.GetHandlePosition(this, handle.PropertyRank);
					this.SetDirtyBbox();
				}
			}

			this.UpdatePopupInterface ();
		}

		private void UpdatePopupInterface()
		{
			foreach (Properties.Abstract property in this.properties)
			{
				property.OpenOrClosePopupInterface (this);
			}
		}

		public void HandleAdd(Point pos, HandleType type)
		{
			//	Ajoute une poignée.
			this.HandleAdd(pos, type, HandleConstrainType.Symmetric);
		}

		public void HandleAdd(Point pos, HandleType type, HandleConstrainType constrain)
		{
			//	Ajoute une poignée.
			Handle handle = new Handle(this.document);
			handle.Position = pos;
			handle.Type = type;
			handle.ConstrainType = constrain;
			this.handles.Add(handle);
			this.SetDirtyBbox();
		}

		public void HandleInsert(int rank, Handle handle)
		{
			//	Insère une poignée.
			this.handles.Insert(rank, handle);
			this.SetDirtyBbox();
		}

		public void HandleDelete(int rank)
		{
			//	Supprime une poignée.
			this.handles.RemoveAt(rank);
			this.SetDirtyBbox();
		}

		public Point GetHandlePosition(int rank)
		{
			//	Donne la position d'une poignée.
			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				System.Diagnostics.Debug.Assert(this.handles[rank] != null);
				Handle handle = this.handles[rank] as Handle;
				return handle.Position;
			}
			return new Point(0,0);
		}

		public void HandleHilite(int rank, bool hilite)
		{
			//	Modifie l'état "survollé" d'une poignée.
			Handle handle = this.Handle(rank);
			if (handle != null)
			{
				handle.IsHilited = hilite;
			}
		}

		public Handle Handle(int rank)
		{
			//	Donne une poignée de l'objet.
			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				System.Diagnostics.Debug.Assert(this.handles[rank] != null);
				return this.handles[rank] as Handle;
			}
			return null;
		}

		public int DetectHandle(Point pos)
		{
			//	Détecte la poignée pointée par la souris.
			return this.DetectHandle(pos, -1);
		}

		public int DetectHandle(Point pos, int excludeRank)
		{
			//	Détecte la poignée pointée par la souris, en excluant une poignée.
			int total = this.TotalHandle;
			double min = 1000000.0;
			int rank = -1;
			for ( int i=total-1 ; i>=0 ; i-- )
			{
				bool detected = false;

				if (i == excludeRank && this.moveHandleRank != -1)
				{
					DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
					double size = context.HandleSize + 1.0/context.ScaleX;
					detected = Common.Document.Objects.Handle.Detect(pos, this.moveHandlePos, size);
				}
				else if (i != excludeRank)
				{
					detected = this.Handle(i).Detect(pos);
				}

				if (detected)
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

		public int DetectSelectedSegmentHandle(Point pos)
		{
			//	Détecte la poignée d'un segment sélectionné pointée par la souris.
			int rank = -1;
			if ( this.selectedSegments != null && this.selectedSegments.Count != 0 )
			{
				double min = 1000000.0;
				for ( int i=0 ; i<this.selectedSegments.Count ; i++ )
				{
					SelectedSegment ss = this.selectedSegments[i] as SelectedSegment;
					if ( ss.Detect(pos) )
					{
						double distance = Point.Distance(ss.Position, pos);
						if ( distance < min && System.Math.Abs(distance-min) > 0.00001 )
						{
							min = distance;
							rank = i;
						}
					}
				}
			}
			return rank;
		}

		public bool IsSelectedSegments()
		{
			//	Indique s'il existe un segment sélectionné.
			return ( this.selectedSegments != null && this.selectedSegments.Count != 0 );
		}

		public bool IsShaperHandleSelected(int rank)
		{
			//	Indique si une poignée est sélectionnée par le modeleur.
			Handle handle = this.Handle(rank);
			return handle.IsVisible && !handle.IsShaperDeselected;
		}

		public virtual bool IsShaperHandleSelected()
		{
			//	Indique si au moins une poignée est sélectionnée par le modeleur.
			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( !handle.IsVisible )  continue;

				if ( !handle.IsShaperDeselected )  return true;
			}
			return false;
		}

		public virtual int TotalShaperHandleSelected()
		{
			//	Donne le nombre de poignées sélectionnées par le modeleur.
			int count = 0;
			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( !handle.IsVisible )  continue;

				if ( !handle.IsShaperDeselected )  count ++;
			}
			return count;
		}

		public void SelectHandle(int rank, bool add)
		{
			//	Sélectionne une poignée avec le modeleur.
			if ( !add )
			{
				this.SelectedSegmentClear();
			}

			if ( !add && rank != -1 )
			{
				Handle handle = this.Handle(rank);
				if ( !handle.IsShaperDeselected )  return;  // poignée déjà sélectionnée ?
			}

			this.InsertOpletSelection();

			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( !handle.IsVisible )  continue;

				if ( i == rank )
				{
					if ( add )
					{
						handle.IsShaperDeselected = !handle.IsShaperDeselected;
					}
					else
					{
						handle.IsShaperDeselected = false;
					}
				}
				else
				{
					if ( !add )
					{
						handle.IsShaperDeselected = true;
					}
				}
			}
		}

		public int ShaperDetectSegment(Point mouse)
		{
			//	Détecte le segment pour le modeleur survolé par la souris.
			if ( !this.IsSelectedSegmentPossible )  return -1;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Path path = this.GetShaperPath();
			double width = this.SelectedSegmentWidth(context, true);
			return Geometry.DetectOutlineRank(path, width/2, mouse);
		}

		public void ShaperHiliteSegment(bool hilite, Point mouse)
		{
			//	Met en évidence un segment pour le modeleur, lorsque l'objet est survolé et sélectionné.
			int hilitedSegment = -1;

			if ( hilite )
			{
				hilitedSegment = this.ShaperDetectSegment(mouse);
			}

			if ( this.hilitedSegment != hilitedSegment )
			{
				this.hilitedSegment = hilitedSegment;
				this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			}
		}

		public void ShaperHiliteHandles(bool hilite)
		{
			//	Mise en évidence de toutes les poignées pour le modeleur, lorsque l'objet est survolé
			//	sans être sélectionné.
			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( handle.PropertyType != Properties.Type.None )  continue;
				if ( handle.Type == HandleType.Secondary )  continue;
				if ( handle.Type == HandleType.Bezier )  continue;

				handle.Modify(hilite, false, false, hilite);
			}
		}

		public virtual bool IsSelectedSegmentPossible
		{
			//	Indique si cet objet peut avoir des segments sélectionnés.
			get { return false; }
		}

		public void SelectedSegmentClear()
		{
			//	Désélectionne tous les segments de l'objet.
			if ( this.selectedSegments != null )
			{
				this.selectedSegments.Clear();
				this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			}
		}

		public Point GetSelectedSegmentPosition(int rank)
		{
			//	Retourne la position de la poignée d'un segment sélectionné.
			SelectedSegment ss = this.selectedSegments[rank] as SelectedSegment;
			return ss.Position;
		}

		public int SelectedSegmentAdd(int rank, ref Point pos, bool add)
		{
			//	Sélectionne un segment de l'objet.
			if ( !add )
			{
				this.SelectHandle(-1, false);
			}

			if ( this.selectedSegments == null )
			{
				this.selectedSegments = new UndoableList(this.document, UndoableListType.SelectedSegments);
			}

			if ( !add )
			{
				this.selectedSegments.Clear();
			}

			SelectedSegment ss = new SelectedSegment(this.document, this, rank, pos);
			int i = this.selectedSegments.Add(ss);
			pos = ss.Position;

			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			return i;
		}


		public virtual void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Début du déplacement d'une poignée.
			if ( rank < this.TotalMainHandle )  // poignée de l'objet, pas propriété ?
			{
				this.moveHandleRank = rank;
				this.moveHandlePos = this.Handle(rank).Position;
				this.InsertOpletGeometry();
			}
		}

		public virtual void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Déplace une poignée.
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

		public virtual void MoveHandleEnding(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Fin du déplacement d'une poignée.
			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				this.moveHandleRank = -1;
				drawingContext.MagnetClearStarting();
				this.document.Modifier.TextInfoModif = "";
			}

			this.document.Notifier.NotifyGeometryChanged();
		}


		public virtual void MoveSelectedSegmentStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Début du déplacement d'une poignée d'un segment sélectionné.
			this.InsertOpletGeometry();
		}

		public virtual void MoveSelectedSegmentProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Déplace une poignée d'un segment sélectionné.
		}

		public virtual void MoveSelectedSegmentEnding(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Fin du déplacement d'une poignée d'un segment sélectionné.
			drawingContext.ConstrainDelStarting();
			drawingContext.MagnetClearStarting();
		}


		public virtual void MoveSelectedHandlesStarting(Point mouse, DrawingContext drawingContext)
		{
			//	Retourne la liste des positions des poignées sélectionnées par le modeleur.
			drawingContext.SnapPos(ref mouse);
			this.moveSelectedHandleStart = mouse;

			this.moveSelectedHandleList = new System.Collections.ArrayList();
			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( !handle.IsVisible )  continue;

				if ( !handle.IsShaperDeselected )
				{
					this.MoveSelectedHandlesAdd(i);
				}
			}

			if ( this.selectedSegments != null && this.selectedSegments.Count != 0 )
			{
				foreach ( SelectedSegment ss in this.selectedSegments )
				{
					this.MoveSelectedHandlesAdd(ss.Rank+0);
					this.MoveSelectedHandlesAdd(ss.Rank+1);
				}
			}

			if ( this.moveSelectedHandleList.Count == 0 )
			{
				this.moveSelectedHandleList = null;
				return;
			}

			this.InsertOpletGeometry();

			if ( this.selectedSegments != null && this.selectedSegments.Count != 0 )
			{
				SelectedSegment.InsertOpletGeometry(this.selectedSegments, this);
			}
		}

		protected void MoveSelectedHandlesAdd(int rank)
		{
			foreach ( MoveSelectedHandle h in this.moveSelectedHandleList )
			{
				if ( h.rank == rank )  return;
			}

			MoveSelectedHandle nh = new MoveSelectedHandle();
			nh.rank = rank;
			nh.position = this.Handle(rank).Position;
			this.moveSelectedHandleList.Add(nh);
		}

		public virtual void MoveSelectedHandlesProcess(Point mouse, DrawingContext drawingContext)
		{
			//	Déplace toutes les poignées sélectionnées par le modeleur.
			if ( this.moveSelectedHandleList == null )  return;

			this.document.Notifier.NotifyArea(this.BoundingBox);

			drawingContext.SnapPos(ref mouse);
			Point move = mouse-moveSelectedHandleStart;

			foreach ( MoveSelectedHandle h in this.moveSelectedHandleList )
			{
				Handle handle = this.Handle(h.rank);
				handle.Position = h.position+move;
			}

			if ( this.selectedSegments != null && this.selectedSegments.Count != 0 )
			{
				SelectedSegment.Update(this.selectedSegments, this);
			}

			this.SetDirtyBbox();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public virtual void MoveSelectedHandlesEnding(Point move, DrawingContext drawingContext)
		{
			//	Fin du déplacement de toutes les poignées sélectionnées par le modeleur.
			this.moveSelectedHandleList = null;
		}


		protected void MoveCorner(Point pc, int corner, int left, int right, int opposite)
		{
			//	Déplace un coin tout en conservant une forme rectangulaire.
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


		public virtual void MoveAllStarting()
		{
			//	Début du déplacement de tout l'objet.
			this.InsertOpletGeometry();
		}

		public virtual void MoveAllProcess(Point move)
		{
			//	Effectue le déplacement de tout l'objet.
			//	Un objet désélectionné est déplacé entièrement, car il s'agit forcément
			//	du fils d'un objet sélectionné.
			this.document.Notifier.NotifyArea(this.BoundingBox);

			bool allHandle = !this.IsSelected;
			bool global = true;
			foreach ( Handle handle in this.handles )
			{
				if ( handle.Type == HandleType.Property )
				{
					handle.Position += move;
				}
				else
				{
					if ( allHandle || handle.IsVisible )
					{
						handle.Position += move;
					}
					else
					{
						global = false;
					}
				}
			}

			if ( global )
			{
				this.MoveBbox(move);
			}
			else
			{
				this.SetDirtyBbox();
			}

			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		public virtual void MoveGlobalStarting()
		{
			//	Début du déplacement global de l'objet.
			this.InsertOpletGeometry();

			foreach ( Handle handle in this.handles )
			{
				handle.InitialPosition = handle.Position;
			}

			this.initialDirection = this.direction;
		}

		public virtual void MoveGlobalProcess(Selector selector)
		{
			//	Effectue le déplacement global de l'objet.
			//	Un objet désélectionné est déplacé entièrement, car il s'agit forcément
			//	du fils d'un objet sélectionné.
			this.document.Notifier.NotifyArea(this.BoundingBox);

			bool allHandle = !this.IsSelected;
			bool global = true;
			bool firstHandle = true;
			Point move = new Point(0,0);
			foreach ( Handle handle in this.handles )
			{
				if ( handle.Type == HandleType.Property )
				{
					handle.Position = selector.DotTransform(handle.InitialPosition);
				}
				else
				{
					if ( allHandle || handle.IsVisible )
					{
						if ( firstHandle )
						{
							Point initial = handle.Position;
							handle.Position = selector.DotTransform(handle.InitialPosition);
							move = handle.Position-initial;
							firstHandle = false;
						}
						else
						{
							handle.Position = selector.DotTransform(handle.InitialPosition);
						}
					}
					else
					{
						global = false;
					}
				}
			}

			this.direction = this.initialDirection + selector.GetTransformAngle;

			if ( global && !firstHandle && selector.IsTranslate )
			{
				this.MoveBbox(move);
			}
			else
			{
				this.SetDirtyBbox();
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public virtual void MoveGlobalStartingProperties()
		{
			//	Début du déplacement global des propriétés de l'objet.
			for ( int i=0 ; i<this.properties.Count ; i++ )
			{
				Properties.Abstract property = this.properties[i] as Properties.Abstract;
				if ( property.IsStyle )  continue;

				property.MoveGlobalStarting();
			}
		}

		public void MoveGlobalProcessProperties(Selector selector)
		{
			//	Effectue le déplacement global des propriétés de l'objet.
			for ( int i=0 ; i<this.properties.Count ; i++ )
			{
				Properties.Abstract property = this.properties[i] as Properties.Abstract;
				if ( property.IsStyle )  continue;

				property.MoveGlobalProcess(selector);
			}
		}

		public virtual void AlignGrid(DrawingContext drawingContext)
		{
			//	Aligne l'objet sur la grille.
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

		public virtual void Reset()
		{
			//	Remet l'objet droit et d'équerre.
		}


		protected void TextInfoModifLine()
		{
			//	Texte des informations de modification pour un objet segment de ligne.
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			double angle = Point.ComputeAngleDeg(this.Handle(0).Position, this.Handle(1).Position);
			string text = string.Format(Res.Strings.Object.Abstract.InfoLine, this.document.Modifier.RealToString(len), this.document.Modifier.AngleToString(angle));
			this.document.Modifier.TextInfoModif = text;
		}

		protected void TextInfoModifRect()
		{
			//	Texte des informations de modification pour un objet rectangulaire.
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

		protected void TextInfoModifCircle()
		{
			//	Texte des informations de modification pour un objet circulaire.
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			string text = string.Format(Res.Strings.Object.Abstract.InfoCircle, this.document.Modifier.RealToString(len));
			this.document.Modifier.TextInfoModif = text;
		}


		public virtual int DetectCell(Point pos)
		{
			//	Détecte la cellule pointée par la souris.
			return -1;
		}

		public virtual void MoveCellStarting(int rank, Point pos,
											 bool isShift, bool isCtrl, int downCount,
											 DrawingContext drawingContext)
		{
			//	Début du déplacement d'une cellule.
		}

		public virtual void MoveCellProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Déplace une cellule.
		}


		protected void MoveBbox(Point move)
		{
			//	Déplace toutes les bbox de l'objet.
			if ( this.dirtyBbox )  return;

			this.bboxThin.Offset(move);
			this.bboxGeom.Offset(move);
			this.bboxFull.Offset(move);
			this.surfaceAnchor.Move(move);
		}

		public void SetDirtyBbox()
		{
			//	Indique qu'il faudra refaire les bbox.
			this.dirtyBbox = true;
			this.dirtyBboxBase = true;
			this.surfaceAnchor.SetDirty();
		}

		public Drawing.Rectangle BoundingBox
		{
			//	Rectangle englobant l'objet.
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

		public Drawing.Rectangle BoundingBoxPartial
		{
			//	Rectangle englobant l'objet complet ou partiel.
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

		public Drawing.Rectangle BoundingBoxDetect
		{
			//	Rectangle englobant l'objet pour les détections.
			get
			{
				return this.BoundingBoxGeom;
			}
		}

		public Drawing.Rectangle BoundingBoxGroup
		{
			//	Rectangle englobant l'objet pour les groupes.
			get
			{
				return this.BoundingBoxGeom;
			}
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

				Drawing.Rectangle box = this.bboxFull;

				if ( this is TextBox2 || this is TextLine2 )
				{
					box = Drawing.Rectangle.Union(box, this.bboxLast);
				}

				if ( this.edited )  // édition en cours ?
				{
					double sx = this.document.Modifier.ActiveViewer.DrawingContext.ScaleX;
					box.Inflate(Abstract.EditFlowHandleSize*1.42/sx);
				}

				return box;
			}
		}

		public void UpdateSurfaceBox(out Drawing.Rectangle surfThin, out Drawing.Rectangle surfGeom)
		{
			//	Calcule les 2 rectangles pour SurfaceAnchor, qui sont les bbox
			//	(Thin et Geom) de l'objet lorsqu'il n'est pas tourné.
			if (this.dirtyBboxBase)  // est-ce que les bbox Thin et Geom ne sont pas à jour ?
			{
				this.UpdateBoundingBox();  // on les recalcule (toutes, même Full)
				this.dirtyBbox = false;  // elles sont de nouveau à jour
			}

			if (this.direction == 0.0)
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

		public virtual Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Constuit les formes de l'objet.
			return null;
		}

		private void UpdateBoundingBox()
		{
			//	Calcule le rectangle englobant l'objet. Chaque objet se charge de
			//	ce calcul, selon sa géométrie, l'épaisseur de son trait, etc.
			//	Il faut calculer :
			//	this.bboxThin  boîte selon la géométrie de l'objet, sans les traits
			//	this.bboxGeom  boîte selon la géométrie de l'objet, avec les traits
			//	this.bboxFull  boîte complète lorsque l'objet est sélectionné
			Shape[] shapes = this.ShapesBuild(null, null, false);
			this.ComputeBoundingBox(shapes);

			for ( int i=0 ; i<this.TotalHandle ; i++ )
			{
				Handle handle = this.handles[i] as Handle;
				if ( handle.Type != HandleType.Property || handle.IsVisible )
				{
					this.InflateBoundingBox(this.Handle(i).Position, true);
				}
			}
		}

		protected void ComputeBoundingBox(params Shape[] shapes)
		{
			//	Calcule toutes les bbox de l'objet en fonction des formes.
			if ( shapes == null )  return;

			this.bboxLast = this.bboxFull;
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

				bbox = Geometry.ComputeBoundingBox(shape);
				this.bboxGeom.MergeWith(bbox);
				this.bboxGeom.MergeWith(this.RealBoundingBox());
			}

			this.bboxFull = this.bboxGeom;
			this.dirtyBboxBase = false;  // les bbox Thin et Geom sont déjà à jour (Full pas encore)

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
						shape.PropertySurface.InflateBoundingBox (this.surfaceAnchor, ref this.bboxFull);
					}

					this.surfaceAnchor.LineUse = initialLineUse;
				}
			}
		}

		protected void InflateBoundingBox(Point pos, bool onlyFull)
		{
			//	Agrandit toutes les bbox en fonction d'un point supplémentaire.
			if ( !onlyFull )
			{
				this.bboxThin.MergeWith(pos);
				this.bboxGeom.MergeWith(pos);
			}
			this.bboxFull.MergeWith(pos);
		}


		public bool IsHilite
		{
			//	Etat survolé de l'objet.
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

		public bool IsHide
		{
			//	Etat caché de l'objet.
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


		public bool Mark
		{
			//	Gestion de la marque.
			get { return this.mark; }
			set { this.mark = value; }
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

		public void Select(bool select, bool edit)
		{
			//	Sélectionne ou désélectionne toutes les poignées de l'objet.
			this.InsertOpletSelection();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);

			this.selected = select;
			this.SetEdited(edit);
			this.globalSelected = false;
			this.allSelected = true;
			this.SplitProperties();

			bool shaper = this.document.Modifier.IsToolShaper;

			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( handle.PropertyType != Properties.Type.None )  break;

				if ( shaper )
				{
					handle.Modify(select, false, false, select);
				}
				else
				{
					handle.Modify(select && !edit, false, false, false);
				}
			}
			this.SelectedSegmentClear();
			this.SetDirtyBbox();
			this.HandlePropertiesUpdate();

			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Notifier.NotifySelectionChanged();
		}

		public virtual void Select(Drawing.Rectangle rect)
		{
			//	Sélectionne toutes les poignées de l'objet dans un rectangle.
			this.InsertOpletSelection();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);

			bool shaper = this.document.Modifier.IsToolShaper;

			int sel = 0;
			int total = this.TotalMainHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( handle.PropertyType != Properties.Type.None )  break;

				if ( rect.Contains(handle.Position) )
				{
					handle.Modify(true, false, false, false);
					sel ++;
				}
				else
				{
					if ( shaper )
					{
						handle.Modify(true, false, false, true);
					}
					else
					{
						handle.Modify(false, false, false, false);
					}
				}
			}
			this.selected = ( sel > 0 );
			this.SetEdited(false);
			this.globalSelected = false;
			this.allSelected = (sel == total);
			this.HandlePropertiesUpdate();
			this.SplitProperties();

			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Notifier.NotifySelectionChanged();
		}

		protected virtual void SetEdited(bool state)
		{
			//	Modifie le mode d'édition. Il faut obligatoirement utiliser cet appel
			//	pour modifier this.edited !
			System.Diagnostics.Debug.Assert(state == false);
		}

		protected virtual void EditWrappersAttach()
		{
			//	Attache l'objet au différents wrappers.
		}

		protected virtual void UpdateTextRulers()
		{
			//	Met à jour les règles pour le texte en édition.
		}

		public void GlobalSelect(bool global, bool many)
		{
			//	Indique que l'objet est sélectionné globalement (avec Selector).
			this.InsertOpletSelection();

			if ( this.document.Modifier.IsToolShaper )
			{
				global = true;
				many = false;
			}

			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle handle = this.Handle(i);
				if ( handle.PropertyType != Properties.Type.None )  break;

				if ( many )
				{
					handle.IsGlobalSelected = false;
					handle.IsManySelected = handle.IsVisible && global;
				}
				else
				{
					handle.IsGlobalSelected = handle.IsVisible && global;
					handle.IsManySelected = false;
					handle.IsShaperDeselected = false;
				}
			}
			this.globalSelected = global;
			this.HandlePropertiesUpdate();

			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		public bool IsCreating
		{
			//	Indique si l'objet est en cours de création.
			get { return this.isCreating; }
		}

		public bool IsSelected
		{
			//	Indique si l'objet est sélectionné.
			get { return this.selected; }
		}

		public bool IsGlobalSelected
		{
			//	Indique si l'objet est sélectionné globalement (avec Selector).
			get { return this.globalSelected; }
		}


		public virtual bool IsEditable
		{
			//	Indique si l'objet est éditable.
			get { return false; }
		}

		public bool IsEdited
		{
			//	Indique si l'objet est en cours d'édition.
			get { return this.edited; }
		}

		
		protected void AddProperty(Properties.Type type, Objects.Abstract model, bool floating)
		{
			//	Ajoute une nouvelle propriété à l'objet.
			//	Une propriété flottante n'est référencée par personne et elle n'est pas
			//	dans la liste des propriétés du document. ObjectPoly crée un ObjectLine
			//	avec des propriétés flottantes, pendant la création.
			if (this.document.Type == DocumentType.Pictogram && type == Properties.Type.Frame)
			{
				return;
			}

			if ( model != null )
			{
				Properties.Abstract original = model.Property(type);
				if ( original != null && original.IsStyle )
				{
					//	L'objet utilise directement la propriété du style, et
					//	surtout pas une copie !
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
				//	Les propriétés de ObjectMemory sont marquées "IsOnlyForCreation".
				//	De plus, elles ne sont pas dans la liste des propriétés du document.
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

		public int TotalProperty
		{
			//	Nombre de proriétés.
			get { return this.properties.Count; }
		}

		public bool ExistProperty(Properties.Type type)
		{
			//	Indique si une propriété existe.
			return ( this.Property(type) != null );
		}

		public void ChangeProperty(Properties.Abstract property)
		{
			//	Change une propriété de l'objet.
			int i = this.PropertyIndex(property.Type);
			System.Diagnostics.Debug.Assert(i != -1);
			this.properties[i] = property;

			if ( property.IsStyle )
			{
				this.document.Notifier.NotifyStyleChanged();
			}
		}

		public void PropertiesXferMemory(Objects.Abstract src)
		{
			//	Transfert toutes les propriétés d'un ObjectMemory source qui n'existent
			//	pas dans l'objet courant.
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

		public int PropertyIndex(Properties.Type type)
		{
			//	Donne l'index d'une propriété de l'objet.
			for ( int i=0 ; i<this.properties.Count ; i++ )
			{
				Properties.Abstract property = this.properties[i] as Properties.Abstract;
				if ( property.Type == type )  return i;
			}
			return -1;
		}

		public Properties.Abstract Property(Properties.Type type)
		{
			//	Donne une propriété de l'objet.
			foreach ( Properties.Abstract property in this.properties )
			{
				if ( property.Type == type )  return property;
			}
			return null;
		}

		public Properties.Type[] PropertiesStyle()
		{
			//	Retourne la liste de toutes les types utilisant un style.
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

		public Properties.Tension PropertyTension
		{
			get { return this.Property(Properties.Type.Tension) as Properties.Tension; }
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
			get { return this.Property (Properties.Type.Volume) as Properties.Volume; }
		}

		public Properties.Frame PropertyFrame
		{
			get { return this.Property(Properties.Type.Frame) as Properties.Frame; }
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


		public Properties.Line AdditionnalPropertyFrameStroke
		{
			get
			{
				return this.GetAdditionnalProperty (Properties.Type.FrameStroke) as Properties.Line;
			}
		}

		public Properties.Gradient AdditionnalPropertyFrameSurface
		{
			get
			{
				return this.GetAdditionnalProperty (Properties.Type.FrameSurface) as Properties.Gradient;
			}
		}

		public Properties.Gradient AdditionnalPropertyFrameBackground
		{
			get
			{
				return this.GetAdditionnalProperty (Properties.Type.FrameBackground) as Properties.Gradient;
			}
		}

		public Properties.Gradient AdditionnalPropertyFrameShadow
		{
			get
			{
				return this.GetAdditionnalProperty (Properties.Type.FrameShadow) as Properties.Gradient;
			}
		}

		private Properties.Abstract GetAdditionnalProperty(Properties.Type type)
		{
			//	Retourne une propriété additionnelle. Si elle n'existe pas, elle est créée.
			//	Les propriétés additionnelles ne sont jamais sérialisées.
			//	Elles sont créées à la volée en cas de besoin, et jamais détruites.
			//	Cela n'est pas un problème, dans la mesure où la quantité de mémoire utilisée
			//	est petite. Imaginons un exemple:
			//		1.	Un objet Rectangle utilise un cadre (les propriétés additionnelles
			//			sont créées).
			//		2.	L'objet n'utilise plus de cadre (les propriétés additionnelles
			//			continuent d'exister pour des prunes).
			//		3.	Le document est sérialisé (les propriétés additionnelles ne sont
			//			pas sérialisées).
			//		4.	A la désérialisation, les propriétés additionnelles n'existent plus.

			if (this.additionnalProperties == null)
			{
				//	Crée la liste une fois pour toutes, si elle n'existe pas.
				this.additionnalProperties = new List<Properties.Abstract> ();
			}

			//	Cherche si la propriété existe.
			foreach (var property in this.additionnalProperties)
			{
				if (property.Type == type)
				{
					return property;  // bingo, la propriété existe
				}
			}

			//	Crée la propriété une fois pour toutes.
			var np = Properties.Abstract.NewProperty (this.document, type);
			this.additionnalProperties.Add (np);
			return np;
		}


		protected static int PropertySearch(System.Collections.ArrayList list, Properties.Type type)
		{
			//	Cherche une propriété d'un type donné dans une liste.
			for ( int i=0 ; i<list.Count ; i++ )
			{
				Properties.Abstract property = list[i] as Properties.Abstract;
				if ( property.Type == type )  return i;
			}
			return -1;
		}

		public void PropertiesList(System.Collections.ArrayList list, bool propertiesDetail)
		{
			//	Ajoute toutes les propriétés de l'objet dans une liste.
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

		public void PropertiesList(System.Collections.ArrayList list, Objects.Abstract filter)
		{
			//	Ajoute toutes les propriétés de l'objet dans une liste.
			foreach ( Properties.Abstract property in this.properties )
			{
				if ( property.Type == Properties.Type.ModColor )  continue;
				if ( property.Type == Properties.Type.BackColor )  continue;
				if ( filter != null && !filter.ExistingProperty(property.Type) )  continue;
				list.Add(property);
			}
		}

		public bool PropertyExist(Properties.Abstract search)
		{
			//	Cherche si l'objet utilise une propriété.
			return this.properties.Contains(search);
		}

		public bool IsComplexPrinting
		{
			//	Indique si une impression complexe est nécessaire.
			get
			{
				foreach (Properties.Abstract property in this.properties)
				{
					if (property.IsComplexPrinting)
					{
						return true;
					}
				}

				if (this.additionnalProperties != null)
				{
					foreach (Properties.Abstract property in this.additionnalProperties)
					{
						if (property.IsComplexPrinting)
						{
							return true;
						}
					}
				}

				return false;
			}
		}

		public void PickerProperties(Objects.Abstract model)
		{
			//	Reprend toutes les propriétés d'un objet source.
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
				model.aggregates.UndoableCopyTo(this.aggregates);
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
				model.aggregates.UndoableCopyTo(this.aggregates);
				this.AggregateUse();
				this.SetDirtyBbox();
			}
		}

		public void ChangePropertyPolyClose(bool close)
		{
			//	Modifie la propriété PolyClose de l'objet en cours de création.
			//	Rappel: un objet en cours de création n'est pas sélectionné.
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

		public void UseProperty(Properties.Abstract style)
		{
			//	Utilise un style donné.
			Properties.Abstract actual = this.Property(style.Type);
			if ( actual == null )  return;

			this.MergeProperty(style, actual);
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		protected void PickerProperty(Properties.Abstract property)
		{
			//	Prend une propriété à un objet désélectionné.
			Properties.Abstract src = this.Property(property.Type);
			if ( src == null )  return;

			Properties.Abstract dst = Properties.Abstract.NewProperty(this.document, property.Type);
			property.CopyTo(dst);
			dst.IsSelected = true;  // nouvel état
			this.document.Modifier.PropertyAdd(dst);
			this.MergeProperty(dst, src);
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public void FreeProperty(Properties.Abstract search)
		{
			//	Libère un style (style -> propriété).
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

		protected void SplitProperties()
		{
			//	Détache les propriétés sélectionnées des propriétés désélectionnées.
			//	C'est nécessaire au cas où une propriété est modifiée.
			for ( int i=0 ; i<this.properties.Count ; i++ )
			{
				Properties.Abstract property = this.properties[i] as Properties.Abstract;

				if ( property.IsStyle )  continue;
				if ( property.IsSelected == this.selected )  continue;

				this.SplitProperty(property, this.selected);
			}
		}

		protected void SplitProperty(Properties.Abstract property, bool selected)
		{
			//	Détache une propriété.
			Properties.Abstract dst = this.SearchProperty(property, selected);
			if ( dst == null )  // la propriété détachée sera seule ?
			{
				//	Crée une nouvelle instance pour la propriété dans son nouvel
				//	état, car elle est seule à être comme cela.
				dst = Properties.Abstract.NewProperty(this.document, property.Type);
				property.CopyTo(dst);
				dst.IsSelected = selected;  // nouvel état
				dst.IsStyle = false;
				this.document.Modifier.PropertyAdd(dst);
			}
			this.MergeProperty(dst, property);  // fusionne év. à une même propriété
		}

		protected void MergeProperty(Properties.Abstract dst, Properties.Abstract src)
		{
			//	Essaie de fusionner une propriété avec une même.
			if ( dst == null )  return;

			src.Owners.Remove(this);
			dst.Owners.Add(this);
			this.ChangeProperty(dst);  // l'objet utilise désormais la propriété destination

			if ( src.Owners.Count == 0 && !src.IsStyle )  // propriété source plus utilisée ?
			{
				this.document.Modifier.PropertyRemove(src);
			}
		}

		protected Properties.Abstract SearchProperty(Properties.Abstract item, bool selected)
		{
			//	Cherche une propriété identique dans une collection du document.
			UndoableList properties = this.document.Modifier.PropertyList(selected);
			
			foreach ( Properties.Abstract property in properties )
			{
				if ( property == item )  continue;  // soi-même ?
				if ( property.Type != item.Type )  continue;

				if ( property.Compare(item) )  return property;
			}
			return null;
		}


		public UndoableList Aggregates
		{
			//	Liste des agrégats utilisés par l'objet.
			get
			{
				return this.aggregates;
			}
		}

		public void AggregateAdd(Properties.Aggregate agg)
		{
			//	Ajoute un agrégat au sommet de la liste de l'objet.
			this.aggregates.Insert(0, agg);

			if ( agg.IsUsedByObject(this) )
			{
				this.AggregateUse();
			}

			this.SetDirtyBbox();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public string AggregateName
		{
			//	Nom de l'agrégat utilisé par l'objet.
			get
			{
				if ( this.aggregates.Count == 0 )  return "";

				Properties.Aggregate agg = this.aggregates[0] as Properties.Aggregate;
				return agg.AggregateName;
			}
		}

		public System.Collections.ArrayList AggregateNames
		{
			//	Liste des noms des agrégats utilisés par l'objet.
			get
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList();
				foreach ( Properties.Aggregate agg in this.aggregates )
				{
					list.Add(agg.AggregateName);
				}
				return list;
			}
		}

		public void AggregateAdapt(Properties.Aggregate agg)
		{
			//	Adapte l'objet en fonction des changements dans l'agrégat.
			if ( agg.IsUsedByObject(this) )
			{
				this.AggregateFree();
				this.AggregateUse();
			}
		}

		public void AggregateUse()
		{
			//	Utilise toutes les propriétés de la liste d'agrégats.
			if ( this is Objects.Memory )
			{
				for ( int i=0 ; i<this.properties.Count ; i++ )
				{
					Properties.Abstract property = this.properties[i] as Properties.Abstract;
					foreach ( Properties.Aggregate agg in this.aggregates )
					{
						Properties.Abstract style = agg.PropertyDeep(property.Type);
						if ( style != null )
						{
							this.ChangeProperty(style);
							break;
						}
					}
				}
			}
			else
			{
				for ( int i=0 ; i<this.properties.Count ; i++ )
				{
					Properties.Abstract property = this.properties[i] as Properties.Abstract;
					foreach ( Properties.Aggregate agg in this.aggregates )
					{
						Properties.Abstract style = agg.PropertyDeep(property.Type);
						if ( style != null )
						{
							this.UseProperty(style);
							break;
						}
					}
				}
			}
		}

		public void AggregateFree()
		{
			//	Libère toutes les propriétés des agrégats.
			for ( int i=0 ; i<this.properties.Count ; i++ )
			{
				Properties.Abstract property = this.properties[i] as Properties.Abstract;
				this.AggregateFree(property);
			}
		}

		public void AggregateFree(Properties.Aggregate agg)
		{
			//	Libère toutes les propriétés d'un agrégat donné.
			for ( int i=0 ; i<this.properties.Count ; i++ )
			{
				Properties.Abstract property = this.properties[i] as Properties.Abstract;
				if ( agg.Contains(property) )
				{
					this.AggregateFree(property);
				}
			}
		}

		public void AggregateFree(Properties.Abstract property)
		{
			//	Libère une propriété.
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

		public void AggregateDelete(Properties.Aggregate agg)
		{
			//	Libère toutes les propriétés d'un agrégat qui sera détruit.
			if ( this.aggregates.Contains(agg) )
			{
				this.AggregateFree();
				this.aggregates.Remove(agg);
			}
			else
			{
				this.AggregateFree();
				this.AggregateUse();
			}
		}


		public bool Detect(Point pos)
		{
			//	Détecte si la souris est sur un objet.
			if ( this.isHide )  return false;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;

			Drawing.Rectangle bbox = this.BoundingBox;
			bbox.Inflate(context.MinimalWidth);
			if ( !bbox.Contains(pos) )  return false;

			Shape[] shapes = this.ShapesBuild(null, null, false);
			return context.Drawer.Detect(pos, context, shapes);
		}

		public virtual bool Detect(Drawing.Rectangle rect, bool partial)
		{
			//	Détecte si l'objet est dans un rectangle.
			//	partial = false -> toutes les poignées doivent être dans le rectangle
			//	partial = true  -> une seule poignée doit être dans le rectangle
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

		
		public virtual DetectEditType DetectEdit(Point pos)
		{
			//	Détecte si la souris est sur l'objet pour l'éditer.
			return DetectEditType.Out;
		}


		public virtual void PutCommands(List<string> list)
		{
			//	Met les commandes pour l'objet dans une liste.
			if ( this.document.Modifier.IsToolShaper )
			{
				if ( !this.IsSelectedSegmentPossible )
				{
					this.PutCommands(list, "ToBezier");
					this.PutCommands(list, "ToPoly");
					this.PutCommands(list, "");
				}

				this.PutCommands(list, "ShaperHandleAdd");
				this.PutCommands(list, "ShaperHandleContinue");
				this.PutCommands(list, "ShaperHandleSub");
				this.PutCommands(list, "");
				this.PutCommands(list, "ShaperHandleToLine");
				this.PutCommands(list, "ShaperHandleToCurve");
				this.PutCommands(list, "");
				this.PutCommands(list, "ShaperHandleSym");
				this.PutCommands(list, "ShaperHandleSmooth");
				this.PutCommands(list, "ShaperHandleDis");
				this.PutCommands(list, "");
				this.PutCommands(list, "ShaperHandleInline");
				this.PutCommands(list, "ShaperHandleFree");
				this.PutCommands(list, "");
				this.PutCommands(list, "ShaperHandleCorner");
				this.PutCommands(list, "ShaperHandleSimply");
				this.PutCommands(list, "");
				this.PutCommands(list, "ShaperHandleSharp");
				this.PutCommands(list, "ShaperHandleRound");
				this.PutCommands(list, "");
			}
		}

		protected void PutCommands(List<string> list, string cmd)
		{
			//	Met une commande pour l'objet dans une liste, si nécessaire.
			this.document.Modifier.ActiveViewer.MiniBarAdd(list, cmd);
		}

		public virtual bool ShaperHandleState(string family, ref bool enable, System.Collections.ArrayList actives)
		{
			//	Donne l'état d'une commande ShaperHandle*.
			return false;
		}

		protected static void ShaperHandleStateAdd(System.Collections.ArrayList actives, string state)
		{
			if ( !actives.Contains(state) )
			{
				actives.Add(state);
			}
		}

		public virtual bool ShaperHandleCommand(string cmd)
		{
			//	Exécute une commande ShaperHandle*.
			return false;
		}


		public virtual void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			//	Début de la création d'un objet.
			drawingContext.ConstrainClear();
			drawingContext.ConstrainAddHV(pos, false, 0);
			drawingContext.MagnetFixStarting(pos);
		}

		public virtual void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			//	Déplacement pendant la création d'un objet.
			this.SetDirtyBbox();
		}

		public virtual void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			//	Fin de la création d'un objet.
			drawingContext.ConstrainDelStarting();
			drawingContext.MagnetClearStarting();
		}

		public virtual void CreateProcessMessage(Message message, Point pos)
		{
			//	Gestion du clavier pendant la création d'un objet.
		}

		public virtual bool CreateIsEnding(DrawingContext drawingContext)
		{
			//	Indique si la création de l'objet est terminée.
			return true;
		}

		public virtual bool CreateIsExist(DrawingContext drawingContext)
		{
			//	Indique si l'objet peut exister. Retourne false si l'objet ne peut
			//	pas exister et doit être détruit.
			return true;
		}

		public virtual bool CreateEnding(DrawingContext drawingContext)
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

		protected static string CreateAction(string icon, string text)
		{
			return string.Format("     {0}   {1}", Misc.Image(icon), text);
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


		public virtual void FillFontFaceList(List<OpenType.FontName> list)
		{
			//	Ajoute toutes les fontes utilisées par l'objet dans une liste.
		}

		public virtual void FillOneCharList(IPaintPort port, DrawingContext drawingContext, System.Collections.Hashtable table)
		{
			//	Ajoute tous les caractères utilisés par l'objet dans une table.
		}

		public virtual Drawing.Rectangle RealBoundingBox()
		{
			//	Retourne la bounding réelle, en fonction des caractères contenus.
			return Drawing.Rectangle.Empty;
		}


		//	Crée une instance de l'objet.
		protected abstract Objects.Abstract CreateNewObject(Document document, Objects.Abstract model);

		public bool DuplicateObject(Document document, ref Objects.Abstract newObject)
		{
			//	Effectue une copie de l'objet courant.
			newObject = this.CreateNewObject(document, this);
			newObject.CloneObject(this);
			return true;
		}

		public virtual void CloneObject(Objects.Abstract src)
		{
			//	Reprend toutes les caractéristiques d'un objet.
			this.handles.Clear();
			int total = src.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle srcHandle = src.Handle(i);
				Handle newHandle = new Handle(this.document);
				srcHandle.CopyTo(newHandle);
				this.handles.Add(newHandle);
				newHandle.IsHilited = false;
			}

			this.isHide              = src.isHide;
			this.selected            = src.selected;
			this.globalSelected      = src.globalSelected;
			this.allSelected         = src.allSelected;
			this.edited              = src.edited;
			this.dirtyBbox           = src.dirtyBbox;
			this.dirtyBboxBase       = src.dirtyBboxBase;
			this.bboxThin            = src.bboxThin;
			this.bboxGeom            = src.bboxGeom;
			this.bboxFull            = src.bboxFull;
			this.name                = src.name;
			this.totalPropertyHandle = src.totalPropertyHandle;
			this.mark                = src.mark;
			this.direction           = src.direction;
			this.isDirtyPageAndLayerNumbers = src.isDirtyPageAndLayerNumbers;
			this.pageNumber          = src.pageNumber;
			this.layerNumber         = src.layerNumber;

			this.surfaceAnchor.SetDirty();
			this.SplitProperties();
		}

		public void DuplicateAdapt()
		{
			//	Adapte un objet qui vient d'être copié.
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


		public void DrawGeometry(IPaintPort port, DrawingContext drawingContext)
		{
			//	Dessine la géométrie de l'objet.
			//	Un objet ne doit jamais être sélectionné ET caché !
			System.Diagnostics.Debug.Assert(!this.selected || !this.isHide);

			//	Dessine les formes.
			Shape[] shapes = this.ShapesBuild(port, drawingContext, false);
			if ( shapes != null )
			{
				bool skipText = drawingContext.Drawer.DrawShapes(port, drawingContext, this, Drawer.DrawShapesMode.NoText, shapes);

				if ( port is Graphics )
				{
					if ( this.IsHilite && drawingContext.IsActive )
					{
						double initialHiliteSize = drawingContext.HiliteSize;

						if ( skipText )
						{
							drawingContext.HiliteSize = 0.5/drawingContext.ScaleX;
						}

						Shape.Hilited(port, shapes);
						drawingContext.Drawer.DrawShapes(port, drawingContext, this, Drawer.DrawShapesMode.All, shapes);

						drawingContext.HiliteSize = initialHiliteSize;
					}

					if ( this.IsOverDash(drawingContext) )
					{
						Shape.OverDashed(port, shapes);
						drawingContext.Drawer.DrawShapes(port, drawingContext, this, Drawer.DrawShapesMode.All, shapes);
					}
				}

				if ( skipText )
				{
					drawingContext.Drawer.DrawShapes(port, drawingContext, this, Drawer.DrawShapesMode.OnlyText, shapes);
				}
			}

			if ( port is Graphics )
			{
				Graphics graphics = port as Graphics;

				//	Dessine les segments sélectionnés ou survolés.
				if ( this.selectedSegments != null || this.hilitedSegment != -1 )
				{
					Path path = this.GetShaperPath();
					double width = this.SelectedSegmentWidth(drawingContext, false);

					if ( this.selectedSegments != null )
					{
						foreach ( SelectedSegment ss in this.selectedSegments )
						{
							Path selPath = Geometry.PathExtract(path, ss.Rank);
							graphics.Rasterizer.AddOutline(selPath, width);
							graphics.FinalRenderSolid(DrawingContext.ColorSelectedSegment);
						}
					}

					if ( this.hilitedSegment != -1 )
					{
						Path hilitePath = Geometry.PathExtract(path, this.hilitedSegment);
						graphics.Rasterizer.AddOutline(hilitePath, width);
						graphics.FinalRenderSolid(drawingContext.HiliteOutlineColor);
					}

					if ( this.selectedSegments != null )
					{
						foreach ( SelectedSegment ss in this.selectedSegments )
						{
							ss.Draw(graphics, drawingContext);
						}
					}
				}

				//	Dessine les bbox en mode debug.
				if ( drawingContext.IsDrawBoxThin )
				{
					double initialWidth = graphics.LineWidth;
					graphics.LineWidth = 1.0/drawingContext.ScaleX;

					graphics.AddRectangle(this.BoundingBoxThin);
					graphics.RenderSolid(Color.FromAlphaRgb(0.5, 0,0.5,1));  // bleu

					graphics.LineWidth = initialWidth;
				}

				if ( drawingContext.IsDrawBoxGeom )
				{
					double initialWidth = graphics.LineWidth;
					graphics.LineWidth = 1.0/drawingContext.ScaleX;

					graphics.AddRectangle(this.BoundingBoxGeom);
					graphics.RenderSolid(Color.FromAlphaRgb(0.5, 0,1,0));  // vert

					graphics.LineWidth = initialWidth;
				}

				if ( drawingContext.IsDrawBoxFull )
				{
					double initialWidth = graphics.LineWidth;
					graphics.LineWidth = 1.0/drawingContext.ScaleX;

					graphics.AddRectangle(this.BoundingBoxFull);
					graphics.RenderSolid(Color.FromAlphaRgb(0.5, 1,0.5,0));  // orange

					graphics.LineWidth = initialWidth;
				}
			}
		}

		protected double SelectedSegmentWidth(DrawingContext context, bool detect)
		{
			//	Retourne la largeur pour les segments sélectionnés.
			double width = 0.0;
			Properties.Line line = this.PropertyLineMode;
			if ( line != null )
			{
				width = line.Width;
			}
			return System.Math.Max(width, detect ? context.MinimalWidth : 3.0/context.ScaleX);
		}

		protected virtual string NameToDisplay
		{
			//	Retourne le nom de l'objet à afficher (Label) en haut à gauche.
			get
			{
				Properties.Name name = this.PropertyName;
				if (name == null || string.IsNullOrEmpty(name.String))
				{
					return null;
				}

				return name.String;
			}
		}

		public virtual void DrawText(IPaintPort port, DrawingContext drawingContext)
		{
			//	Dessine le texte.
		}

		public virtual void DrawImage(IPaintPort port, DrawingContext drawingContext)
		{
			//	Dessine l'image.
		}

		public void DrawLabel(Graphics graphics, DrawingContext drawingContext)
		{
			//	Dessine le nom de l'objet.
			if ( this.isHide )  return;

			string name = this.NameToDisplay;
			if (string.IsNullOrEmpty(name))
			{
				return;
			}

			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/drawingContext.ScaleX;

			Drawing.Rectangle bbox = graphics.Align(this.BoundingBox);
			bbox.Inflate(0.5/drawingContext.ScaleX);

			string s = TextLayout.ConvertToSimpleText(name);
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
				lineColor = Color.FromRgb(1,0,0);  // rouge
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
				rect = graphics.Align (rect);
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

		public void DrawAggregate(Graphics graphics, DrawingContext drawingContext)
		{
			//	Dessine le nom du style.
			if ( this.isHide )  return;

			if ( this.aggregates.Count == 0 )  return;
			string name = TextLayout.ConvertToSimpleText(this.AggregateName);
			if ( name == "" )  return;

			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/drawingContext.ScaleX;

			Drawing.Rectangle bbox = graphics.Align(this.BoundingBox);
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
				lineColor = Color.FromRgb(1.0, 0.8, 0.0);  // jaune-orange
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
				rect = graphics.Align (rect);
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

		public virtual void DrawHandle(Graphics graphics, DrawingContext drawingContext)
		{
			//	Dessine les poignées de l'objet.
			if ( this.isHide )  return;
			if ( !drawingContext.VisibleHandles )  return;

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

		protected bool IsOverDash(DrawingContext context)
		{
			//	Indique s'il faut dessiner le pointillé forcé lorsqu'il n'y a pas de trait.
			if ( !context.IsActive )
			{
				return false;
			}

			if ( this.isCreating || this.IsHilite )
			{
				return true;
			}

			return !context.PreviewActive;
		}

		public IList<Properties.Abstract> GetPdfComplexSurfaces(IPaintPort port)
		{
			//	Donne la liste des propriétés qui utilisent des surfaces complexes.
			List<Properties.Abstract> list = new List<Properties.Abstract> ();

			foreach (Properties.Abstract property in this.properties)
			{
				this.GetPdfComplexSurface (port, property, list);
			}

			if (this.additionnalProperties != null)
			{
				foreach (Properties.Abstract property in this.additionnalProperties)
				{
					this.GetPdfComplexSurface (port, property, list);
				}
			}

			return list;
		}

		private void GetPdfComplexSurface(IPaintPort port, Properties.Abstract property, IList<Properties.Abstract> list)
		{
			for (int i=0; i<2; i++)
			{
				Properties.Abstract surface = null;

				switch (i)
				{
					case 0:
						surface = property as Properties.Gradient;
						break;

					case 1:
						surface = property as Properties.Font;
						break;
				}

				if (surface == null)
				{
					continue;
				}

				PDF.Type type = surface.TypeComplexSurfacePDF (port);
				bool isSmooth = surface.IsSmoothSurfacePDF (port);

				if (type == PDF.Type.None)
				{
					continue;
				}
				if (type == PDF.Type.OpaqueRegular && !isSmooth)
				{
					continue;
				}

				list.Add (surface);
			}
		}


		public void SetAutoScroll()
		{
			//	Permet au scroll de se faire à la prochaine occasion.
			this.autoScrollOneShot = true;
		}
		
		protected void ComputeAutoScroll(Point c1, Point c2)
		{
			//	Calcule le scroll éventuel nécessaire pour rendre le cursur visible.
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
			if ( cursor.Right  > view.Right  )  move.X = cursor.Right  - view.Right;
			if ( cursor.Left   < view.Left   )  move.X = cursor.Left   - view.Left;
			if ( cursor.Top    > view.Top    )  move.Y = cursor.Top    - view.Top;
			if ( cursor.Bottom < view.Bottom )  move.Y = cursor.Bottom - view.Bottom;

			this.document.Modifier.ActiveViewer.AutoScroll(move);
		}


		public virtual Path GetMagnetPath()
		{
			//	Retourne le chemin géométrique de l'objet pour les constructions
			//	magnétiques. Généralement, ce chemin est identique à celui rendu
			//	par GetPath, mais certains objets peuvent retourner un chemin plus
			//	simple (comme Line, Poly, TextLine, TextBox et Dimension).
			//	L'idée est d'ignorer les propriétés Corner et Arrow, par exemple.
			return this.GetPath();
		}

		public virtual Path GetShaperPath()
		{
			//	Retourne le chemin géométrique de l'objet pour le modeleur.
			//	Généralement, ce chemin est identique à celui rendu
			//	par GetPath, mais certains objets peuvent retourner un chemin plus
			//	simple (comme Line, Poly, TextLine, TextBox et Dimension).
			//	L'idée est d'ignorer les propriétés Corner et Arrow, par exemple.
			//	Lorsqu'une courbe est ouverte, un segment null est ajouté pour ne
			//	pas perturber le compte des segments (ShaperDetectSegment).
			//	Il faut utiliser GetShaperPath lors de l'utilisation de
			//	Geometry.PathExtract et Geometry.DetectOutlineRank.
			return this.GetPath();
		}

		public virtual Path[] GetPaths()
		{
			//	Retourne les chemins géométriques de l'objet.
			//	Si l'objet retourne plus d'un chemin, il faut surcharger cette méthode !
			Path path = this.GetPath();
			if ( path == null )  return null;

			Path[] paths = new Path[1];
			paths[0] = path;
			return paths;
		}

		protected virtual Path GetPath()
		{
			//	Retourne le chemin géométrique de l'objet.
			return null;
		}


		#region CacheBitmap
		public Bitmap CacheBitmap
		{
			//	Retourne le bitmap caché.
			get
			{
				if (this.cacheBitmap == null && !this.cacheBitmapSize.IsEmpty)
				{
					this.CacheBitmapCreate();
				}

				return this.cacheBitmap;
			}
		}

		public Drawing.Size CacheBitmapSize
		{
			//	Dimensions souhaitées pour le bitmap caché.
			get
			{
				return this.cacheBitmapSize;
			}
			set
			{
				if (this.cacheBitmapSize != value)
				{
					this.cacheBitmapSize = value;
					this.CacheBitmapDirty();
				}
			}
		}

		public void CacheBitmapDirty()
		{
			//	Le bitmap caché n'est plus valide.
#if false
			if (this is Page)
			{
				System.Diagnostics.Debug.WriteLine(string.Format("CacheBitmapDirty page #{0}", this.PageNumber));
			}
			if (this is Layer)
			{
				System.Diagnostics.Debug.WriteLine(string.Format("CacheBitmapDirty layer #{0}", this.LayerNumber));
			}
#endif
			this.cacheBitmap = null;
		}

		protected virtual void CacheBitmapCreate()
		{
			//	Crée le bitmap caché.
		}
		#endregion


		#region CreateFromPath
		public bool CreatePolyFromPath(Path path, int subPath)
		{
			//	Crée un polygone à partir d'un chemin quelconque.
			PathElement[] elements;
			Point[] points;
			path.GetElements(out elements, out points);
			if ( elements.Length > 1000 )  return false;

			int firstHandle = this.TotalMainHandle;
			Point start = new Point(0, 0);
			Point current = new Point(0, 0);
			Point p1 = new Point(0, 0);
			Point p2 = new Point(0, 0);
			Point p3 = new Point(0, 0);
			bool close = false;
			bool bDo = false;
			int subRank = -1;
			int i = 0;
			while ( i < elements.Length )
			{
				switch ( elements[i] & PathElement.MaskCommand )
				{
					case PathElement.MoveTo:
						subRank ++;
						current = points[i++];
						firstHandle = this.TotalMainHandle;
						if ( subPath == -1 || subPath == subRank )
						{
							this.HandleAdd(current, HandleType.Starting);
							bDo = true;
						}
						start = current;
						break;

					case PathElement.LineTo:
						p1 = points[i++];
						if ( subPath == -1 || subPath == subRank )
						{
							if ( Geometry.Compare(p1, start) )
							{
								close = true;
								firstHandle = this.TotalMainHandle;
							}
							else
							{
								this.HandleAdd(p1, HandleType.Primary);
								bDo = true;
							}
						}
						current = p1;
						break;

					case PathElement.Curve3:
						p1 = points[i];
						p2 = points[i++];
						p3 = points[i++];
						Geometry.BezierS1ToS2(current, ref p1, ref p2, p3);
						if ( subPath == -1 || subPath == subRank )
						{
							if ( Geometry.Compare(p3, start) )
							{
								close = true;
								firstHandle = this.TotalMainHandle;
							}
							else
							{
								this.HandleAdd(p3, HandleType.Primary);
								bDo = true;
							}
						}
						current = p3;
						break;

					case PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						if ( subPath == -1 || subPath == subRank )
						{
							if ( Geometry.Compare(p3, start) )
							{
								close = true;
								firstHandle = this.TotalMainHandle;
							}
							else
							{
								this.HandleAdd(p3, HandleType.Primary);
								bDo = true;
							}
						}
						current = p3;
						break;

					default:
						if ( (elements[i] & PathElement.FlagClose) != 0 )
						{
							close = true;
						}
						i ++;
						break;
				}
			}
			this.PropertyPolyClose.BoolValue = close;

			return bDo;
		}

		public bool CreateBezierFromPath(Path path, int subPath)
		{
			//	Crée une courbe de Bézier à partir d'un chemin quelconque.
			PathElement[] elements;
			Point[] points;
			path.GetElements(out elements, out points);
			if ( elements.Length > 1000 )  return false;

			int firstHandle = this.TotalMainHandle;
			Point start = new Point(0, 0);
			Point current = new Point(0, 0);
			Point p1 = new Point(0, 0);
			Point p2 = new Point(0, 0);
			Point p3 = new Point(0, 0);
			bool close = false;
			bool bDo = false;
			int subRank = -1;
			int i = 0;
			while ( i < elements.Length )
			{
				switch ( elements[i] & PathElement.MaskCommand )
				{
					case PathElement.MoveTo:
						subRank ++;
						current = points[i++];
						firstHandle = this.TotalMainHandle;
						if ( subPath == -1 || subPath == subRank )
						{
							this.HandleAdd(current, HandleType.Bezier);
							this.HandleAdd(current, HandleType.Starting);
							this.HandleAdd(current, HandleType.Bezier);
							bDo = true;
						}
						start = current;
						break;

					case PathElement.LineTo:
						p1 = points[i++];
						if ( subPath == -1 || subPath == subRank )
						{
							if ( Geometry.Compare(p1, start) )
							{
								close = true;
								this.PathAdjust(firstHandle);
								firstHandle = this.TotalMainHandle;
							}
							else
							{
								this.HandleAdd(p1, HandleType.Bezier);
								this.HandleAdd(p1, HandleType.Primary);
								this.HandleAdd(p1, HandleType.Bezier);
								bDo = true;
							}
						}
						current = p1;
						break;

					case PathElement.Curve3:
						p1 = points[i];
						p2 = points[i++];
						p3 = points[i++];
						Geometry.BezierS1ToS2(current, ref p1, ref p2, p3);
						if ( subPath == -1 || subPath == subRank )
						{
							if ( Geometry.Compare(p3, start) )
							{
								close = true;
								this.Handle(this.TotalMainHandle-1).Position = p1;
								this.Handle(firstHandle).Position = p2;
								this.PathAdjust(firstHandle);
								firstHandle = this.TotalMainHandle;
							}
							else
							{
								this.Handle(this.TotalMainHandle-1).Position = p1;
								this.HandleAdd(p2, HandleType.Bezier);
								this.HandleAdd(p3, HandleType.Primary);
								this.HandleAdd(p3, HandleType.Bezier);
								bDo = true;
							}
						}
						current = p3;
						break;

					case PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						if ( subPath == -1 || subPath == subRank )
						{
							if ( Geometry.Compare(p3, start) )
							{
								close = true;
								this.Handle(this.TotalMainHandle-1).Position = p1;
								this.Handle(firstHandle).Position = p2;
								this.PathAdjust(firstHandle);
								firstHandle = this.TotalMainHandle;
							}
							else
							{
								this.Handle(this.TotalMainHandle-1).Position = p1;
								this.HandleAdd(p2, HandleType.Bezier);
								this.HandleAdd(p3, HandleType.Primary);
								this.HandleAdd(p3, HandleType.Bezier);
								bDo = true;
							}
						}
						current = p3;
						break;

					default:
						if ( (elements[i] & PathElement.FlagClose) != 0 )
						{
							close = true;
							this.PathAdjust(firstHandle);
						}
						i ++;
						break;
				}
			}
			this.PathAdjust(firstHandle);

			if ( this.PropertyPolyClose != null )
			{
				this.PropertyPolyClose.BoolValue = close;
			}

			return bDo;
		}

		protected void PathAdjust(int firstHandle)
		{
			//	Ajuste le chemin.
			//	Transforme les courbes en droite si nécessaire.
			int total = this.TotalMainHandle;
			for ( int i=firstHandle ; i<total ; i+= 3 )
			{
				if ( this.Handle(i+0).Position == this.Handle(i+1).Position )
				{
					this.Handle(i+0).Type = HandleType.Hide;
				}

				if ( this.Handle(i+2).Position == this.Handle(i+1).Position )
				{
					this.Handle(i+2).Type = HandleType.Hide;
				}
			}

			//	Mets les bonnes contraintes aux points principaux.
			for ( int i=firstHandle ; i<total ; i+= 3 )
			{
				this.Handle(i+1).ConstrainType = this.ComputeConstrain(firstHandle, i);
			}
		}

		protected HandleConstrainType ComputeConstrain(int firstHandle, int rank)
		{
			//	Devine le type de contrainte.
			Point p = this.Handle(rank+1).Position;

			Point s1 = this.Handle(rank+0).Position;
			if ( this.Handle(rank+0).Type == HandleType.Hide )  // droite ?
			{
				int r = rank-2;
				if ( r < 0 )  r = this.TotalMainHandle-2;
				s1 = this.Handle(r).Position;
			}

			Point s2 = this.Handle(rank+2).Position;
			if ( this.Handle(rank+2).Type == HandleType.Hide )  // droite ?
			{
				int r = rank+4;
				if ( r > this.TotalMainHandle )  r = firstHandle+1;
				s2 = this.Handle(r).Position;
			}

			return Abstract.ComputeConstrain(s1, p, s2);
		}

		protected static HandleConstrainType ComputeConstrain(Point s1, Point p, Point s2)
		{
			//	Devine le type de contrainte en fonction d'un point principal et
			//	des 2 points secondaires de part et d'autre.
			double dx = System.Math.Abs((p.X-s1.X)-(s2.X-p.X));
			double dy = System.Math.Abs((p.Y-s1.Y)-(s2.Y-p.Y));
			if ( dx < 0.0001 && dy < 0.0001 )
			{
				return HandleConstrainType.Symmetric;
			}
			
			double a1 = Point.ComputeAngleDeg(p, s1);
			double a2 = Point.ComputeAngleDeg(p, s2);
			if ( System.Math.Abs(System.Math.Abs(a1-a2)      ) < 0.1 ||
				System.Math.Abs(System.Math.Abs(a1-a2)-180.0) < 0.1 )
			{
				return HandleConstrainType.Smooth;
			}

			return HandleConstrainType.Corner;
		}
		#endregion


		#region PageAndLayerNumbers
		public int PageNumber
		{
			//	Donne le numéro de la page à laquelle appartient l'objet.
			//	Le numéro de page est toujours ici l'index 0..n de la page dans le document,
			//	en comptant les pages maîtres comme les autres.
			get
			{
				if ( this.isDirtyPageAndLayerNumbers )
				{
					this.document.Modifier.UpdatePageAndLayerNumbers();
					System.Diagnostics.Debug.Assert(this.isDirtyPageAndLayerNumbers == false);
				}

				return this.pageNumber;
			}

			set
			{
				this.pageNumber = value;
				this.isDirtyPageAndLayerNumbers = false;
				this.UpdatePageAndLayerNumbers();
			}
		}

		public int LayerNumber
		{
			//	Donne le numéro du calque auquel appartient l'objet.
			//	Le numéro de calque est toujours ici l'index 0..n du calque.
			get
			{
				if (this.isDirtyPageAndLayerNumbers)
				{
					this.document.Modifier.UpdatePageAndLayerNumbers();
					System.Diagnostics.Debug.Assert(this.isDirtyPageAndLayerNumbers == false);
				}

				return this.layerNumber;
			}

			set
			{
				this.layerNumber = value;
				this.isDirtyPageAndLayerNumbers = false;
				this.UpdatePageAndLayerNumbers();
			}
		}

		protected virtual void UpdatePageAndLayerNumbers()
		{
			//	Met à jour d'autres éléments de l'objet dépendants du numéro de page.
		}
		#endregion


		#region OpletSelection
		protected void InsertOpletSelection()
		{
			//	Ajoute un oplet pour mémoriser les informations de sélection de l'objet.
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletSelection oplet = new OpletSelection(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		//	Mémorise toutes les informations de sélection de l'objet.
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

				Misc.Swap(ref this.isHide,         ref this.host.isHide        );
				Misc.Swap(ref this.globalSelected, ref this.host.globalSelected);
				Misc.Swap(ref this.allSelected,    ref this.host.allSelected   );

				if (this.edited != this.host.edited)
				{
					bool ed = this.edited;
					this.edited = this.host.edited;
					this.host.SetEdited (ed);
				}

				if (this.selected != this.host.selected)
				{
					Misc.Swap (ref this.selected, ref this.host.selected);
					this.host.UpdatePopupInterface ();
				}

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
		protected void InsertOpletGeometry()
		{
			//	Ajoute un oplet pour mémoriser la géométrie de l'objet.
			if ( !this.document.Modifier.OpletQueueEnable )  return;
//-			System.Diagnostics.Debug.WriteLine("InsertOpletGeometry");
			OpletGeometry oplet = new OpletGeometry(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		//	Mémorise toutes les informations sur la géométrie de l'objet.
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

				if ( this.host is AbstractText )
				{
					AbstractText text = this.host as AbstractText;
					text.UpdateGeometry();
				}

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
		protected void InsertOpletName()
		{
			//	Ajoute un oplet pour mémoriser le nom de l'objet.
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletName oplet = new OpletName(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		//	Mémorise le nom de l'objet.
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
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			info.AddValue("UniqueId", this.uniqueId);
			info.AddValue("Name", this.name);
			info.AddValue("Properties", this.properties);

			//	Ne sérialise que les poignées des objets, sans celles des propriétés.
			System.Collections.ArrayList objHandles = new System.Collections.ArrayList();
			for ( int i=0 ; i<this.TotalMainHandle ; i++ )
			{
				objHandles.Add(this.handles[i]);
			}
			info.AddValue("Handles", objHandles);

			info.AddValue("Objects", this.objects);
			info.AddValue("Direction", this.direction);
			info.AddValue("Aggregates", this.aggregates);
		}

		protected Abstract(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui désérialise l'objet.
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

			if ( this.document.IsRevisionGreaterOrEqual(1,0,26) )
			{
				this.aggregates = (UndoableList) info.GetValue("Aggregates", typeof(UndoableList));
			}
			else if ( this.document.IsRevisionGreaterOrEqual(1,0,24) )
			{
				this.aggregates = new UndoableList(this.document, UndoableListType.AggregatesInsideObject);
				Properties.Aggregate agg = (Properties.Aggregate) info.GetValue("Aggregate", typeof(Properties.Aggregate));
				if ( agg != null )
				{
					this.aggregates.Add(agg);
				}
			}
			else
			{
				this.aggregates = new UndoableList(this.document, UndoableListType.AggregatesInsideObject);
			}

			this.CreateMissingProperties ();
		}

		private void CreateMissingProperties()
		{
			//	Crée toutes les propriétés dont l'objet a besoin et qui n'étaient pas sérialisées.
			//	Cela arrive par exemple lorsqu'on ouvre un document créé avant que la propriété 'Frame' existe.
			if (this.document.Type != DocumentType.Pictogram)  // pour gagner du temps lors de la génération d'icônes
			{
				foreach (int value in System.Enum.GetValues (typeof (Properties.Type)))
				{
					Properties.Type type = (Properties.Type) value;
					if (this.ExistingProperty (type) &&  // propriété utilisée par ce type d'objet ?
						!this.ExistProperty (type))      // propriété n'existe pas ?
					{
						this.AddProperty (type, null, false);
					}
				}
			}
		}

		public virtual void ReadFinalize()
		{
			//	Adapte l'objet après une désérialisation.
			foreach ( Properties.Abstract property in this.properties )
			{
				property.Owners.Add(this);
			}
		}

		public virtual void ReadCheckWarnings(System.Collections.ArrayList warnings)
		{
			//	Vérifie si tous les fichiers existent.
		}

		protected static void ReadCheckFonts(System.Collections.ArrayList warnings, TextLayout textLayout)
		{
			//	Vérifie si toutes les fontes d'un TextLayout existent.
			List<OpenType.FontName> list = new List<OpenType.FontName>();
			textLayout.FillFontFaceList(list);
			foreach ( OpenType.FontName fontName in list )
			{
				if (!Misc.IsExistingFont(fontName))
				{
					string message = string.Format(Res.Strings.Object.Text.Error, fontName.FullName);
					if ( !warnings.Contains(message) )
					{
						warnings.Add(message);
					}
				}
			}
		}

		public virtual void ReadFinalizeFlowReady(TextFlow flow)
		{
		}
		#endregion


		protected struct MoveSelectedHandle
		{
			public int				rank;
			public Point			position;
		}

		protected static double EditFlowHandleSize
		{
			//	Taille des "poignées" pour choisir le flux du texte.
			get { return 10.0; }
		}


		protected Document						document;
		protected int							uniqueId;
		protected bool							isHilite;
		protected bool							isHide;
		protected bool							mark;
		protected bool							selected;
		protected bool							edited;
		protected bool							globalSelected;
		protected bool							allSelected;
		protected bool							isCreating;
		protected bool							dirtyBbox = true;
		protected bool							dirtyBboxBase = true;
		protected bool							autoScrollOneShot;
		protected Drawing.Rectangle				bboxThin = Drawing.Rectangle.Empty;
		protected Drawing.Rectangle				bboxGeom = Drawing.Rectangle.Empty;
		protected Drawing.Rectangle				bboxFull = Drawing.Rectangle.Empty;
		protected Drawing.Rectangle				bboxLast = Drawing.Rectangle.Empty;
		protected int							hotSpotRank = -1;
		protected int							hilitedSegment = -1;
		protected Point							moveSelectedHandleStart;
		protected System.Collections.ArrayList	moveSelectedHandleList;
		protected int							moveHandleRank = -1;
		protected Point							moveHandlePos;

		protected string						name = "";
		protected UndoableList					properties;
		protected List<Properties.Abstract>		additionnalProperties;
		protected System.Collections.ArrayList	handles = new System.Collections.ArrayList();
		protected UndoableList					selectedSegments = null;
		protected UndoableList					objects = null;
		protected int							totalPropertyHandle;
		protected double						direction = 0.0;
		protected double						initialDirection = 0.0;
		protected SurfaceAnchor					surfaceAnchor;
		protected UndoableList					aggregates = null;

		protected bool							isDirtyPageAndLayerNumbers = true;
		protected int							pageNumber = -1;
		protected int							layerNumber = -1;
		protected int							debugId;

		protected Bitmap						cacheBitmap;
		protected Drawing.Size					cacheBitmapSize;
		
		private static int						nextDebugId = 1;
		private Widget							popupInterfaceFrame;
	}
}
