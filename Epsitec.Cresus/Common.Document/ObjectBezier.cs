using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe ObjectBezier est la classe de l'objet graphique "courbes de Bézier".
	/// </summary>
	[System.Serializable()]
	public class ObjectBezier : AbstractObject
	{
		public ObjectBezier(Document document, AbstractObject model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
		}

		protected override bool ExistingProperty(PropertyType type)
		{
			if ( type == PropertyType.Name )  return true;
			if ( type == PropertyType.LineMode )  return true;
			if ( type == PropertyType.LineColor )  return true;
			if ( type == PropertyType.Arrow )  return true;
			if ( type == PropertyType.FillGradient )  return true;
			if ( type == PropertyType.PolyClose )  return true;
			return false;
		}

		protected override AbstractObject CreateNewObject(Document document, AbstractObject model)
		{
			return new ObjectBezier(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/bezier.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Point pos)
		{
			if ( this.isHide )  return false;

			Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			double width = System.Math.Max(this.PropertyLineMode.Width/2, context.MinimalWidth);
			
			if (                 Geometry.DetectOutline(pathLine,  width, pos) )  return true;
			if ( outlineStart && Geometry.DetectOutline(pathStart, width, pos) )  return true;
			if ( outlineEnd   && Geometry.DetectOutline(pathEnd,   width, pos) )  return true;

			if ( surfaceStart && Geometry.DetectSurface(pathStart, pos) )  return true;
			if ( surfaceEnd   && Geometry.DetectSurface(pathEnd,   pos) )  return true;

			if ( this.PropertyFillGradient.IsVisible() )
			{
				pathLine.Close();
				if ( Geometry.DetectSurface(pathLine, pos) )  return true;
			}

			return false;
		}

		// Détecte si la souris est sur le pourtour de l'objet.
		// Retourne le rank de la poignée de départ, ou -1
		protected int DetectOutline(Point pos)
		{
			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			double width = System.Math.Max(this.PropertyLineMode.Width/2, context.MinimalWidth);
			int rank = Geometry.DetectOutlineRank(pathLine, width, pos);
			if ( rank != -1 )  rank *= 3;
			return rank;
		}

		// Déplace tout l'objet.
		public override void MoveAllProcess(Point move)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			bool allHandle = !this.IsSelected;
			int total = this.TotalMainHandle;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( allHandle || this.Handle(i+1).IsVisible )
				{
					this.Handle(i+0).Position += move;
					this.Handle(i+1).Position += move;
					this.Handle(i+2).Position += move;
				}
			}
			this.HandlePropertiesUpdatePosition();
			this.dirtyBbox = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Sélectionne toutes les poignées de l'objet dans un rectangle.
		public override void Select(Rectangle rect)
		{
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);

			int total = this.TotalMainHandle;
			int sel = 0;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( rect.Contains(this.Handle(i+1).Position) )
				{
					this.Handle(i+0).IsVisible = true;
					this.Handle(i+1).IsVisible = true;
					this.Handle(i+2).IsVisible = true;
					sel += 3;
				}
				else
				{
					this.Handle(i+0).IsVisible = false;
					this.Handle(i+1).IsVisible = false;
					this.Handle(i+2).IsVisible = false;
				}
			}
			this.selected = ( sel > 0 );

			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Notifier.NotifySelectionChanged();
		}

		// Indique si l'objet est sélectionné.
		public override bool IsSelected
		{
			get
			{
				return ( this.selected || this.isCreating );
			}
		}


		// Donne le contenu du menu contextuel.
		public override void ContextMenu(System.Collections.ArrayList list, Point pos, int handleRank)
		{
			ContextMenuItem item;

			if ( handleRank == -1 )  // sur un segment ?
			{
				int rank = this.DetectOutline(pos);
				if ( rank == -1 )  return;

				item = new ContextMenuItem();
				list.Add(item);  // séparateur

				if ( this.Handle(rank+2).Type == HandleType.Hide )
				{
					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "Curve";
					item.Icon = @"file:images/tocurve.icon";
					item.Text = "Courbe";
					list.Add(item);
				}
				else
				{
					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "Line";
					item.Icon = @"file:images/toline.icon";
					item.Text = "Droit";
					list.Add(item);
				}

				item = new ContextMenuItem();
				item.Command = "Object";
				item.Name = "HandleAdd";
				item.Icon = @"file:images/add.icon";
				item.Text = "Ajouter un point";
				list.Add(item);
			}
			else	// sur un point ?
			{
				if ( handleRank%3 == 1 )  // poignée principale ?
				{
					if ( this.Handle(handleRank-1).Type != HandleType.Hide && this.Handle(handleRank+1).Type != HandleType.Hide )
					{
						item = new ContextMenuItem();
						list.Add(item);  // séparateur

						HandleConstrainType type = this.Handle(handleRank).ConstrainType;

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleSym";
						item.IconActiveNo = @"file:images/activeno.icon";
						item.IconActiveYes = @"file:images/activeyes.icon";
						item.Active = ( type == HandleConstrainType.Symmetric );
						item.Text = "Symetrique";
						list.Add(item);

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleSmooth";
						item.IconActiveNo = @"file:images/activeno.icon";
						item.IconActiveYes = @"file:images/activeyes.icon";
						item.Active = ( type == HandleConstrainType.Smooth );
						item.Text = "Lisse";
						list.Add(item);

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleCorner";
						item.IconActiveNo = @"file:images/activeno.icon";
						item.IconActiveYes = @"file:images/activeyes.icon";
						item.Active = ( type == HandleConstrainType.Corner );
						item.Text = "Anguleux";
						list.Add(item);
					}
					else if ( this.Handle(handleRank-1).Type != HandleType.Hide || this.Handle(handleRank+1).Type != HandleType.Hide )
					{
						item = new ContextMenuItem();
						list.Add(item);  // séparateur

						HandleConstrainType type = this.Handle(handleRank).ConstrainType;

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleSmooth";
						item.IconActiveNo = @"file:images/activeno.icon";
						item.IconActiveYes = @"file:images/activeyes.icon";
						item.Active = ( type == HandleConstrainType.Smooth );
						item.Text = "En ligne";
						list.Add(item);

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleCorner";
						item.IconActiveNo = @"file:images/activeno.icon";
						item.IconActiveYes = @"file:images/activeyes.icon";
						item.Active = ( type != HandleConstrainType.Smooth );
						item.Text = "Libre";
						list.Add(item);
					}

					if ( this.TotalMainHandle >= 3*3 )
					{
						item = new ContextMenuItem();
						list.Add(item);  // séparateur

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleDelete";
						item.Icon = @"file:images/sub.icon";
						item.Text = "Enlever le point";
						list.Add(item);
					}
				}
			}
		}

		// Exécute une commande du menu contextuel.
		public override void ContextCommand(string cmd, Point pos, int handleRank)
		{
			int rank = this.DetectOutline(pos);

			if ( cmd == "Line" )
			{
				if ( rank == -1 )  return;
				this.ContextToLine(rank);
			}

			if ( cmd == "Curve" )
			{
				if ( rank == -1 )  return;
				this.ContextToCurve(rank);
			}

			if ( cmd == "HandleAdd" )
			{
				if ( rank == -1 )  return;
				this.ContextAddHandle(pos, rank);
			}

			if ( cmd == "HandleSym" )
			{
				this.ContextSym(handleRank);
			}

			if ( cmd == "HandleSmooth" )
			{
				this.ContextSmooth(handleRank);
			}

			if ( cmd == "HandleCorner" )
			{
				this.ContextCorner(handleRank);
			}

			if ( cmd == "HandleDelete" )
			{
				this.ContextSubHandle(pos, handleRank);
			}
		}

		// Passe le point en mode symétrique.
		protected void ContextSym(int rank)
		{
			this.Handle(rank).ConstrainType = HandleConstrainType.Symmetric;
			this.MoveSecondary(rank, rank-1, rank+1, this.Handle(rank-1).Position);
			this.dirtyBbox = true;
		}

		// Passe le point en mode lisse.
		protected void ContextSmooth(int rank)
		{
			this.Handle(rank).ConstrainType = HandleConstrainType.Smooth;

			if ( this.Handle(rank-1).Type == HandleType.Hide || this.Handle(rank+1).Type == HandleType.Hide )
			{
				this.MovePrimary(rank, this.Handle(rank).Position);
			}
			else
			{
				this.MoveSecondary(rank, rank-1, rank+1, this.Handle(rank-1).Position);
			}
			this.dirtyBbox = true;
		}

		// Passe le point en mode anguleux.
		protected void ContextCorner(int rank)
		{
			this.Handle(rank).ConstrainType = HandleConstrainType.Corner;
		}

		// Ajoute une poignée sans changer l'aspect de la courbe.
		protected void ContextAddHandle(Point pos, int rank)
		{
			for ( int i=0 ; i<3 ; i++ )
			{
				Handle handle = new Handle(this.document);
				handle.Position = pos;
				handle.Type = (i==1) ? HandleType.Primary : HandleType.Bezier;
				handle.IsVisible = true;
				this.HandleInsert(rank+3, handle);
			}

			int prev = rank+0;
			int curr = rank+3;
			int next = rank+6;
			if ( next >= this.TotalMainHandle )  next = 0;

			if ( this.Handle(prev+2).Type == HandleType.Hide && this.Handle(next+0).Type == HandleType.Hide )
			{
				pos = Point.Projection(this.Handle(prev+1).Position, this.Handle(next+1).Position, pos);
				this.Handle(curr+0).Position = pos;
				this.Handle(curr+1).Position = pos;
				this.Handle(curr+2).Position = pos;
				this.Handle(curr+0).Type = HandleType.Hide;
				this.Handle(curr+2).Type = HandleType.Hide;
			}
			else
			{
				double t = Point.FindBezierParameter(this.Handle(prev+1).Position, this.Handle(prev+2).Position, this.Handle(next+0).Position, this.Handle(next+1).Position, pos);
				this.Handle(curr+1).Position = Point.FromBezier(this.Handle(prev+1).Position, this.Handle(prev+2).Position, this.Handle(next+0).Position, this.Handle(next+1).Position, t);
				pos = Point.Scale(this.Handle(prev+2).Position, this.Handle(next+0).Position, t);
				this.Handle(prev+2).Position = Point.Scale(this.Handle(prev+1).Position, this.Handle(prev+2).Position, t);
				this.Handle(next+0).Position = Point.Scale(this.Handle(next+1).Position, this.Handle(next+0).Position, 1-t);
				this.Handle(curr+0).Position = Point.Scale(this.Handle(prev+2).Position, pos, t);
				this.Handle(curr+2).Position = Point.Scale(this.Handle(next+0).Position, pos, 1-t);

				this.Handle(curr+1).ConstrainType = HandleConstrainType.Smooth;
				if ( this.Handle(prev+1).ConstrainType == HandleConstrainType.Symmetric )  this.Handle(prev+1).ConstrainType = HandleConstrainType.Smooth;
				if ( this.Handle(next+1).ConstrainType == HandleConstrainType.Symmetric )  this.Handle(next+1).ConstrainType = HandleConstrainType.Smooth;
			}
			this.dirtyBbox = true;
			this.HandlePropertiesUpdatePosition();
		}

		// Supprime une poignée sans changer l'aspect de la courbe.
		protected void ContextSubHandle(Point pos, int rank)
		{
			this.HandleDelete(rank-1);
			this.HandleDelete(rank-1);
			this.HandleDelete(rank-1);

			int prev = rank-4;
			if ( prev < 0 )  prev = this.TotalMainHandle-3;
			int next = rank-1;
			if ( next >= this.TotalMainHandle )  next = 0;

			if ( this.Handle(prev+2).Type == HandleType.Hide || this.Handle(next+0).Type == HandleType.Hide )
			{
				if ( this.Handle(prev+2).Type != this.Handle(next+0).Type )
				{
					this.ContextToCurve(prev);
				}
			}
			this.dirtyBbox = true;
			this.HandlePropertiesUpdatePosition();
		}

		// Conversion d'un segement en ligne droite.
		protected void ContextToLine(int rank)
		{
			int next = rank+3;
			if ( next >= this.TotalMainHandle )  next = 0;
			this.Handle(rank+2).Position = this.Handle(rank+1).Position;
			this.Handle(next+0).Position = this.Handle(next+1).Position;
			this.Handle(rank+2).Type = HandleType.Hide;
			this.Handle(next+0).Type = HandleType.Hide;
			this.Handle(rank+1).ConstrainType = HandleConstrainType.Corner;
			this.Handle(next+1).ConstrainType = HandleConstrainType.Corner;
			this.dirtyBbox = true;
			this.HandlePropertiesUpdatePosition();
		}

		// Conversion d'un segement en courbe.
		protected void ContextToCurve(int rank)
		{
			int next = rank+3;
			if ( next >= this.TotalMainHandle )  next = 0;
			this.Handle(rank+2).Position = Point.Scale(this.Handle(rank+1).Position, this.Handle(next+1).Position, 0.25);
			this.Handle(next+0).Position = Point.Scale(this.Handle(next+1).Position, this.Handle(rank+1).Position, 0.25);
			this.Handle(rank+2).Type = HandleType.Bezier;
			this.Handle(next+0).Type = HandleType.Bezier;
			this.Handle(rank+1).ConstrainType = HandleConstrainType.Corner;
			this.Handle(next+1).ConstrainType = HandleConstrainType.Corner;
			this.dirtyBbox = true;
			this.HandlePropertiesUpdatePosition();
		}


		// Adapte le point secondaire s'il est en mode "en ligne".
		protected void AdaptPrimaryLine(int rankPrimary, int rankSecondary, out int rankExtremity)
		{
			rankExtremity = rankPrimary - (rankSecondary-rankPrimary)*3;
			if ( rankExtremity < 0 )  rankExtremity = this.TotalMainHandle-2;
			if ( rankExtremity >= this.TotalMainHandle )  rankExtremity = 1;

			if ( this.Handle(rankPrimary).ConstrainType != HandleConstrainType.Smooth )  return;
			int rankOpposite = rankPrimary - (rankSecondary-rankPrimary);
			if ( this.Handle(rankOpposite).Type != HandleType.Hide )  return;

			double dist = Point.Distance(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position);
			Point pos = new Point();
			pos = Point.Move(this.Handle(rankPrimary).Position, this.Handle(rankExtremity).Position, dist);
			pos = Point.Symmetry(this.Handle(rankPrimary).Position, pos);
			this.Handle(rankSecondary).Position = pos;
			this.dirtyBbox = true;
			this.HandlePropertiesUpdatePosition();
		}

		// Déplace une poignée primaire selon les contraintes.
		protected void MovePrimary(int rank, Point pos)
		{
			Point move = pos-this.Handle(rank).Position;
			this.Handle(rank).Position = pos;
			this.Handle(rank-1).Position += move;
			this.Handle(rank+1).Position += move;
			if ( this.Handle(rank-1).Type == HandleType.Hide )  this.Handle(rank-1).Position = pos;
			if ( this.Handle(rank+1).Type == HandleType.Hide )  this.Handle(rank+1).Position = pos;

			int rankExtremity;
			this.AdaptPrimaryLine(rank, rank-1, out rankExtremity);
			this.AdaptPrimaryLine(rankExtremity, rankExtremity+1, out rankExtremity);
			this.AdaptPrimaryLine(rank, rank+1, out rankExtremity);
			this.AdaptPrimaryLine(rankExtremity, rankExtremity-1, out rankExtremity);
			this.dirtyBbox = true;
		}

		// Déplace une poignée secondaire selon les contraintes.
		protected void MoveSecondary(int rankPrimary, int rankSecondary, int rankOpposite, Point pos)
		{
			HandleConstrainType type = this.Handle(rankPrimary).ConstrainType;

			this.Handle(rankSecondary).Position = pos;

			if ( this.Handle(rankOpposite).Type == HandleType.Hide )  // droite?
			{
				if ( type == HandleConstrainType.Smooth )
				{
					if ( rankOpposite > rankSecondary )
					{
						rankOpposite += 2;
						if ( rankOpposite >= this.TotalMainHandle )  rankOpposite = 1;
					}
					else
					{
						rankOpposite -= 2;
						if ( rankOpposite < 0 )  rankOpposite = this.TotalMainHandle-2;
					}
					this.Handle(rankSecondary).Position = Point.Projection(this.Handle(rankPrimary).Position, this.Handle(rankOpposite).Position, pos);
				}
			}
			else	// courbe ?
			{
				if ( type == HandleConstrainType.Symmetric )
				{
					this.Handle(rankOpposite).Position = Point.Symmetry(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position);
				}

				if ( type == HandleConstrainType.Smooth )
				{
					double dist = Point.Distance(this.Handle(rankPrimary).Position, this.Handle(rankOpposite).Position);
					Point p = Point.Move(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position, dist);
					this.Handle(rankOpposite).Position = Point.Symmetry(this.Handle(rankPrimary).Position, p);
				}
			}
		}

		// Début du déplacement une poignée.
		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank < this.TotalMainHandle )
			{
				this.InsertOpletGeometry();
				pos = this.Handle((rank/3)*3+1).Position;
				drawingContext.ConstrainFixStarting(pos);
			}
		}

		// Déplace une poignée.
		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank >= this.TotalMainHandle )  // poignée d'une propriété ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.ConstrainSnapPos(ref pos);
			drawingContext.SnapGrid(ref pos);

			if ( this.Handle(rank).Type == HandleType.Primary )  // principale ?
			{
				this.MovePrimary(rank, pos);
			}
			else if ( this.Handle(rank).Type == HandleType.Bezier )  // secondaire ?
			{
				if ( rank%3 == 0 )  // poignée secondaire ?
				{
					this.MoveSecondary(rank+1, rank, rank+2, pos);
				}
				if ( rank%3 == 2 )  // poignée secondaire ?
				{
					this.MoveSecondary(rank-1, rank, rank-2, pos);
				}
			}

			this.HandlePropertiesUpdatePosition();
			this.dirtyBbox = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Déplace globalement l'objet.
		public override void MoveGlobalProcess(SelectorData initial, SelectorData final)
		{
			base.MoveGlobalProcess(initial, final);
			this.HandlePropertiesUpdatePosition();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			drawingContext.ConstrainFixStarting(pos);
			drawingContext.ConstrainFixType(ConstrainType.Normal);

			this.isCreating = true;

			if ( this.TotalHandle == 0 )
			{
				this.lastPoint = false;
				this.PropertyPolyClose.Bool = false;
			}
			else
			{
				double len = Point.Distance(pos, this.Handle(1).Position);
				if ( len <= drawingContext.CloseMargin )
				{
					pos = this.Handle(1).Position;
					this.lastPoint = true;
					this.PropertyPolyClose.Bool = true;
				}
			}

			if ( !this.lastPoint )
			{
				if ( this.TotalHandle == 0 )
				{
					this.HandleAdd(pos, HandleType.Bezier);
					this.HandleAdd(pos, HandleType.Starting);
					this.HandleAdd(pos, HandleType.Bezier);
				}
				else
				{
					this.HandleAdd(pos, HandleType.Bezier);
					this.HandleAdd(pos, HandleType.Primary);
					this.HandleAdd(pos, HandleType.Bezier);

					int rank = this.TotalHandle-6;
					if ( this.Handle(rank).Type == HandleType.Hide )
					{
						this.Handle(rank+2).Position = Point.Scale(this.Handle(rank+1).Position, pos, 0.5);
					}
				}
				this.Handle(this.TotalHandle-2).IsVisible = true;
			}

			this.mouseDown = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapGrid(ref pos);
			drawingContext.ConstrainSnapPos(ref pos);

			if ( this.mouseDown )
			{
				int rank;
				if ( this.lastPoint )
				{
					rank = 1;
				}
				else
				{
					rank = this.TotalHandle-2;
				}
				this.Handle(rank+1).Position = pos;
				this.Handle(rank-1).Position = Point.Symmetry(this.Handle(rank).Position, pos);
				this.dirtyBbox = true;
			}
			else
			{
				if ( this.TotalHandle > 0 )
				{
					double len = Point.Distance(pos, this.Handle(1).Position);
					if ( len <= drawingContext.CloseMargin )
					{
						this.Handle(1).Type = HandleType.Ending;
					}
					else
					{
						this.Handle(1).Type = HandleType.Starting;
					}
				}
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			int rank = this.TotalHandle;
			double len = Point.Distance(this.Handle(rank-1).Position, this.Handle(rank-2).Position);
			if ( rank <= 3 )
			{
				if ( len <= drawingContext.MinimalSize )
				{
					rank -= 3;
					this.Handle(rank+0).Position = this.Handle(rank+1).Position;
					this.Handle(rank+2).Position = this.Handle(rank+1).Position;
					this.Handle(rank+0).Type = HandleType.Hide;
					this.Handle(rank+2).Type = HandleType.Hide;
					this.Handle(rank+1).ConstrainType = HandleConstrainType.Corner;
				}
			}
			else
			{
				if ( len <= drawingContext.MinimalSize )
				{
					this.ContextToLine(rank-6);
				}
			}

			drawingContext.ConstrainDelStarting();
			this.mouseDown = false;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Indique si la création de l'objet est terminée.
		public override bool CreateIsEnding(DrawingContext drawingContext)
		{
			if ( this.lastPoint )
			{
				this.Handle(1).Type = HandleType.Primary;
				this.Deselect();
				this.HandlePropertiesCreate();
				this.HandlePropertiesUpdatePosition();
				this.isCreating = false;
				return true;
			}
			else
			{
				return false;
			}
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			return true;
		}

		// Termine la création de l'objet. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateEnding(DrawingContext drawingContext)
		{
			this.isCreating = false;

			int total = this.TotalHandle;
			if ( total <= 3 )  return false;

			this.Handle(1).Type = HandleType.Primary;
			this.Deselect();
			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdatePosition();
			return true;
		}

		// Retourne un bouton d'action pendant la création.
		public override bool CreateAction(int rank, out string cmd, out string name, out string text)
		{
			if ( rank == 0 )
			{
				cmd  = "Object";
				name = "CreateEnding";
				text = "Terminer la création";
				return true;
			}
			if ( rank == 1 )
			{
				cmd  = "Object";
				name = "CreateAndSelect";
				text = "Terminer et sélectionner";
				return true;
			}
			return base.CreateAction(rank, out cmd, out name, out text);
		}

		
		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			if ( this.TotalMainHandle < 3 )  return;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			Rectangle bboxStart = Geometry.ComputeBoundingBox(pathStart);
			Rectangle bboxEnd   = Geometry.ComputeBoundingBox(pathEnd);
			Rectangle bboxLine  = Geometry.ComputeBoundingBox(pathLine);

			this.bboxThin = bboxLine;
			this.bboxThin.MergeWith(this.Handle(1).Position);
			this.bboxThin.MergeWith(this.Handle(this.TotalMainHandle-2).Position);

			this.PropertyLineMode.InflateBoundingBox(ref bboxLine);
			this.bboxGeom = bboxLine;

			if ( outlineStart )  this.PropertyLineMode.InflateBoundingBox(ref bboxStart);
			this.bboxGeom.MergeWith(bboxStart);

			if ( outlineEnd )  this.PropertyLineMode.InflateBoundingBox(ref bboxEnd);
			this.bboxGeom.MergeWith(bboxEnd);

			this.bboxGeom.MergeWith(this.bboxThin);
			this.bboxFull = this.bboxGeom;
			this.bboxFull.MergeWith(this.FullBoundingBox());

			this.bboxGeom.MergeWith(this.PropertyFillGradient.BoundingBoxGeom(this.bboxThin));
			this.bboxFull.MergeWith(this.PropertyFillGradient.BoundingBoxFull(this.bboxThin));
			this.bboxFull.MergeWith(this.bboxGeom);
		}

		// Calcule la bbox qui englobe l'objet et les poignées secondaires.
		protected Rectangle FullBoundingBox()
		{
			Rectangle bbox = Rectangle.Empty;
			int total = this.TotalMainHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				bbox.MergeWith(this.Handle(i).Position);
			}
			return bbox;
		}

		// Calcule les points de la droite à une extrémité de la courbe.
		public void ComputeExtremity(bool start, out Point a, out Point b)
		{
			int r1 = start ? 1 : this.TotalMainHandle-2;
			int r2 = start ? 2 : this.TotalMainHandle-3;

			if ( this.Handle(r2).Type == HandleType.Hide )
			{
				r2 = start ? 4 : this.TotalMainHandle-5;
			}

			a = this.Handle(r1).Position;
			b = this.Handle(r2).Position;
		}

		// Crée les chemins de l'objet.
		protected void PathBuild(DrawingContext drawingContext,
								 out Path pathStart, out bool outlineStart, out bool surfaceStart,
								 out Path pathEnd,   out bool outlineEnd,   out bool surfaceEnd,
								 out Path pathLine)
		{
			pathStart = new Path();
			pathEnd   = new Path();
			pathLine  = new Path();

			double zoom = AbstractProperty.DefaultZoom(drawingContext);
			pathStart.DefaultZoom = zoom;
			pathEnd.DefaultZoom = zoom;
			pathLine.DefaultZoom = zoom;

			int total = this.TotalMainHandle;
			if ( total < 6 )
			{
				outlineStart = false;
				surfaceStart = false;
				outlineEnd   = false;
				surfaceEnd   = false;
				return;
			}

			Point p1, p2, pp1, pp2;
			double w = this.PropertyLineMode.Width;
			CapStyle cap = this.PropertyLineMode.Cap;
			this.ComputeExtremity(true, out p1, out p2);
			pp1 = this.PropertyArrow.PathExtremity(pathStart, 0, w,cap, p1,p2, out outlineStart, out surfaceStart);
			this.ComputeExtremity(false, out p1, out p2);
			pp2 = this.PropertyArrow.PathExtremity(pathEnd,   1, w,cap, p1,p2, out outlineEnd,   out surfaceEnd);

			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( i == 0 )  // premier point ?
				{
					pathLine.MoveTo(pp1);
				}
				else if ( i < total-3 )  // point intermédiaire ?
				{
					pathLine.CurveTo(this.Handle(i-1).Position, this.Handle(i).Position, this.Handle(i+1).Position);
				}
				else	// dernier point ?
				{
					pathLine.CurveTo(this.Handle(i-1).Position, this.Handle(i).Position, pp2);
				}
			}
			if ( this.PropertyPolyClose.Bool )  // fermé ?
			{
				pathLine.CurveTo(this.Handle(total-1).Position, this.Handle(0).Position, pp1);
				pathLine.Close();
			}
		}

		// Dessine l'objet.
		public override void DrawGeometry(Graphics graphics, DrawingContext drawingContext)
		{
			base.DrawGeometry(graphics, drawingContext);

			int total = this.TotalMainHandle;
			if ( total < 3 )  return;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(drawingContext,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			this.PropertyFillGradient.Render(graphics, drawingContext, pathLine, this.BoundingBoxThin);

			if ( outlineStart )
			{
				this.PropertyLineMode.AddOutline(graphics, pathStart, 0.0);
				graphics.RenderSolid(drawingContext.AdaptColor(this.PropertyLineColor.Color));
			}
			if ( surfaceStart )
			{
				graphics.Rasterizer.AddSurface(pathStart);
				graphics.RenderSolid(drawingContext.AdaptColor(this.PropertyLineColor.Color));
			}

			if ( outlineEnd )
			{
				this.PropertyLineMode.AddOutline(graphics, pathEnd, 0.0);
				graphics.RenderSolid(drawingContext.AdaptColor(this.PropertyLineColor.Color));
			}
			if ( surfaceEnd )
			{
				graphics.Rasterizer.AddSurface(pathEnd);
				graphics.RenderSolid(drawingContext.AdaptColor(this.PropertyLineColor.Color));
			}

			this.PropertyLineMode.DrawPath(graphics, drawingContext, pathLine, this.PropertyLineColor.Color);

			if ( this.IsHilite && drawingContext.IsActive )
			{
				if ( this.PropertyFillGradient.IsVisible() )
				{
					graphics.Rasterizer.AddSurface(pathLine);
					graphics.RenderSolid(drawingContext.HiliteSurfaceColor);
				}

				if ( outlineStart )
				{
					this.PropertyLineMode.AddOutline(graphics, pathStart, drawingContext.HiliteSize);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}
				if ( surfaceStart )
				{
					graphics.Rasterizer.AddSurface(pathStart);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}

				if ( outlineEnd )
				{
					this.PropertyLineMode.AddOutline(graphics, pathEnd, drawingContext.HiliteSize);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}
				if ( surfaceEnd )
				{
					graphics.Rasterizer.AddSurface(pathEnd);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}

				this.PropertyLineMode.AddOutline(graphics, pathLine, drawingContext.HiliteSize);
				graphics.RenderSolid(drawingContext.HiliteOutlineColor);
			}

			if ( this.IsSelected && drawingContext.IsActive && !this.IsGlobalSelected )
			{
				double initialWidth = graphics.LineWidth;
				graphics.LineWidth = 1.0/drawingContext.ScaleX;

				for ( int i=0 ; i<total ; i+=3 )
				{
					graphics.AddLine(this.Handle(i+0).Position, this.Handle(i+1).Position);
					graphics.AddLine(this.Handle(i+1).Position, this.Handle(i+2).Position);
					graphics.RenderSolid(Color.FromBrightness(0.6));
				}

				graphics.LineWidth = initialWidth;
			}
		}

		// Imprime l'objet.
		public override void PrintGeometry(Printing.PrintPort port, DrawingContext drawingContext)
		{
			base.PrintGeometry(port, drawingContext);

			int total = this.TotalMainHandle;
			if ( total < 3 )  return;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(drawingContext,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			if ( this.PropertyFillGradient.PaintColor(port, drawingContext) )
			{
				port.PaintSurface(pathLine);
			}

			if ( this.PropertyLineColor.PaintColor(port, drawingContext) )
			{
				if ( outlineStart )
				{
					this.PropertyLineMode.PaintOutline(port, drawingContext, pathStart);
				}
				if ( surfaceStart )
				{
					port.PaintSurface(pathStart);
				}

				if ( outlineEnd )
				{
					this.PropertyLineMode.PaintOutline(port, drawingContext, pathEnd);
				}
				if ( surfaceEnd )
				{
					port.PaintSurface(pathEnd);
				}

				this.PropertyLineMode.PaintOutline(port, drawingContext, pathLine);
			}
		}


		#region Serialization
		// Sérialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui désérialise l'objet.
		protected ObjectBezier(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion

		
		protected bool				mouseDown = false;
		protected bool				lastPoint;
		protected bool				isCreating;
	}
}
