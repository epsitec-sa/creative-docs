using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectBezier est la classe de l'objet graphique "courbes de Bézier".
	/// </summary>
	public class ObjectBezier : AbstractObject
	{
		public ObjectBezier()
		{
			PropertyLine lineMode = new PropertyLine();
			lineMode.Type = PropertyType.LineMode;
			this.AddProperty(lineMode);

			PropertyColor lineColor = new PropertyColor();
			lineColor.Type = PropertyType.LineColor;
			this.AddProperty(lineColor);

			PropertyGradient fillGradient = new PropertyGradient();
			fillGradient.Type = PropertyType.FillGradient;
			this.AddProperty(fillGradient);

			PropertyBool fillClose = new PropertyBool();
			fillClose.Type = PropertyType.PolyClose;
			this.AddProperty(fillClose);
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectBezier();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/bezier.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			if ( this.isHide )  return false;
			if ( this.DetectOutline(pos) != -1 )  return true;
			if ( this.DetectFill(pos) )  return true;
			return false;
		}

		// Détecte si la souris est sur le pourtour de l'objet.
		// Retourne le rank de la poignée de départ, ou -1
		protected int DetectOutline(Drawing.Point pos)
		{
			int total = this.TotalHandle;
			if ( total < 3 )  return -1;
			double width = System.Math.Max(this.PropertyLine(0).Width/2, this.minimalWidth);
			for ( int i=0 ; i<total-3 ; i+=3 )
			{
				Drawing.Point p1 = this.Handle(i+1).Position;
				Drawing.Point s1 = this.Handle(i+2).Position;
				Drawing.Point s2 = this.Handle(i+3).Position;
				Drawing.Point p2 = this.Handle(i+4).Position;
				if ( Drawing.Point.Detect(p1,s1,s2,p2, pos, width) )  return i;
			}
			if ( this.PropertyBool(3).Bool )  // fermé ?
			{
				Drawing.Point p1 = this.Handle(total-2).Position;
				Drawing.Point s1 = this.Handle(total-1).Position;
				Drawing.Point s2 = this.Handle(0).Position;
				Drawing.Point p2 = this.Handle(1).Position;
				if ( Drawing.Point.Detect(p1,s1,s2,p2, pos, width) )  return total-3;
			}
			return -1;
		}

		// Détecte si la souris est dans la surface de l'objet.
		protected bool DetectFill(Drawing.Point pos)
		{
			int total = this.TotalHandle;
			if ( total < 3 )  return false;
			if ( !this.PropertyGradient(2).IsVisible() )  return false;
			InsideSurface surf = new InsideSurface(pos, (total/3)*InsideSurface.bezierStep);
			for ( int i=0 ; i<total-3 ; i+=3 )
			{
				Drawing.Point p1 = this.Handle(i+1).Position;
				Drawing.Point s1 = this.Handle(i+2).Position;
				Drawing.Point s2 = this.Handle(i+3).Position;
				Drawing.Point p2 = this.Handle(i+4).Position;
				surf.AddBezier(p1, s1, s2, p2);
			}
			if ( this.PropertyBool(3).Bool )  // fermé ?
			{
				Drawing.Point p1 = this.Handle(total-2).Position;
				Drawing.Point s1 = this.Handle(total-1).Position;
				Drawing.Point s2 = this.Handle(0).Position;
				Drawing.Point p2 = this.Handle(1).Position;
				surf.AddBezier(p1, s1, s2, p2);
			}
			else
			{
				Drawing.Point p1 = this.Handle(total-2).Position;
				Drawing.Point p2 = this.Handle(1).Position;
				surf.AddLine(p1, p2);
			}
			return surf.IsInside();
		}

		// Déplace tout l'objet.
		public override void MoveAll(Drawing.Point move, bool all)
		{
			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( all || this.Handle(i+1).IsSelected )
				{
					this.Handle(i+0).Position += move;
					this.Handle(i+1).Position += move;
					this.Handle(i+2).Position += move;
				}
			}

			this.dirtyBbox = true;
		}

		// Sélectionne toutes les poignées de l'objet dans un rectangle.
		public override void Select(Drawing.Rectangle rect)
		{
			int total = this.TotalHandleProperties;
			int sel = 0;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( rect.Contains(this.Handle(i+1).Position) )
				{
					this.Handle(i+0).IsSelected = true;
					this.Handle(i+1).IsSelected = true;
					this.Handle(i+2).IsSelected = true;
					sel += 3;
				}
				else
				{
					this.Handle(i+0).IsSelected = false;
					this.Handle(i+1).IsSelected = false;
					this.Handle(i+2).IsSelected = false;
				}
			}
			this.selected = ( sel > 0 );
		}


		// Donne le contenu du menu contextuel.
		public override void ContextMenu(System.Collections.ArrayList list, Drawing.Point pos, int handleRank)
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
					item.Text = "Courbe";
					list.Add(item);
				}
				else
				{
					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "Line";
					item.Text = "Droit";
					list.Add(item);
				}

				item = new ContextMenuItem();
				list.Add(item);  // séparateur

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

					if ( this.handles.Count >= 3*3 )
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
		public override void ContextCommand(string cmd, Drawing.Point pos, int handleRank)
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
		}

		// Passe le point en mode anguleux.
		protected void ContextCorner(int rank)
		{
			this.Handle(rank).ConstrainType = HandleConstrainType.Corner;
		}

		// Ajoute une poignée sans changer l'aspect de la courbe.
		protected void ContextAddHandle(Drawing.Point pos, int rank)
		{
			for ( int i=0 ; i<3 ; i++ )
			{
				Handle handle = new Handle();
				handle.Position = pos;
				handle.Type = (i==1) ? HandleType.Primary : HandleType.Secondary;
				handle.IsSelected = true;
				this.HandleInsert(rank+3, handle);
			}

			int prev = rank+0;
			int curr = rank+3;
			int next = rank+6;
			if ( next >= this.handles.Count )  next = 0;

			if ( this.Handle(prev+2).Type == HandleType.Hide && this.Handle(next+0).Type == HandleType.Hide )
			{
				pos = Drawing.Point.Projection(this.Handle(prev+1).Position, this.Handle(next+1).Position, pos);
				this.Handle(curr+0).Position = pos;
				this.Handle(curr+1).Position = pos;
				this.Handle(curr+2).Position = pos;
				this.Handle(curr+0).Type = HandleType.Hide;
				this.Handle(curr+2).Type = HandleType.Hide;
			}
			else
			{
				double t = Drawing.Point.Bezier(this.Handle(prev+1).Position, this.Handle(prev+2).Position, this.Handle(next+0).Position, this.Handle(next+1).Position, pos);
				this.Handle(curr+1).Position = Drawing.Point.Bezier(this.Handle(prev+1).Position, this.Handle(prev+2).Position, this.Handle(next+0).Position, this.Handle(next+1).Position, t);
				pos = Drawing.Point.Scale(this.Handle(prev+2).Position, this.Handle(next+0).Position, t);
				this.Handle(prev+2).Position = Drawing.Point.Scale(this.Handle(prev+1).Position, this.Handle(prev+2).Position, t);
				this.Handle(next+0).Position = Drawing.Point.Scale(this.Handle(next+1).Position, this.Handle(next+0).Position, 1-t);
				this.Handle(curr+0).Position = Drawing.Point.Scale(this.Handle(prev+2).Position, pos, t);
				this.Handle(curr+2).Position = Drawing.Point.Scale(this.Handle(next+0).Position, pos, 1-t);

				this.Handle(curr+1).ConstrainType = HandleConstrainType.Smooth;
				if ( this.Handle(prev+1).ConstrainType == HandleConstrainType.Symmetric )  this.Handle(prev+1).ConstrainType = HandleConstrainType.Smooth;
				if ( this.Handle(next+1).ConstrainType == HandleConstrainType.Symmetric )  this.Handle(next+1).ConstrainType = HandleConstrainType.Smooth;
			}
		}

		// Supprime une poignée sans changer l'aspect de la courbe.
		protected void ContextSubHandle(Drawing.Point pos, int rank)
		{
			this.HandleDelete(rank-1);
			this.HandleDelete(rank-1);
			this.HandleDelete(rank-1);

			int prev = rank-4;
			if ( prev < 0 )  prev = this.handles.Count-3;
			int next = rank-1;
			if ( next >= this.handles.Count )  next = 0;

			if ( this.Handle(prev+2).Type == HandleType.Hide || this.Handle(next+0).Type == HandleType.Hide )
			{
				if ( this.Handle(prev+2).Type != this.Handle(next+0).Type )
				{
					this.ContextToCurve(prev);
				}
			}
		}

		// Conversion d'un segement en ligne droite.
		protected void ContextToLine(int rank)
		{
			int next = rank+3;
			if ( next >= this.handles.Count )  next = 0;
			this.Handle(rank+2).Position = this.Handle(rank+1).Position;
			this.Handle(next+0).Position = this.Handle(next+1).Position;
			this.Handle(rank+2).Type = HandleType.Hide;
			this.Handle(next+0).Type = HandleType.Hide;
			this.Handle(rank+1).ConstrainType = HandleConstrainType.Corner;
			this.Handle(next+1).ConstrainType = HandleConstrainType.Corner;
		}

		// Conversion d'un segement en courbe.
		protected void ContextToCurve(int rank)
		{
			int next = rank+3;
			if ( next >= this.handles.Count )  next = 0;
			this.Handle(rank+2).Position = Drawing.Point.Scale(this.Handle(rank+1).Position, this.Handle(next+1).Position, 0.25);
			this.Handle(next+0).Position = Drawing.Point.Scale(this.Handle(next+1).Position, this.Handle(rank+1).Position, 0.25);
			this.Handle(rank+2).Type = HandleType.Secondary;
			this.Handle(next+0).Type = HandleType.Secondary;
			this.Handle(rank+1).ConstrainType = HandleConstrainType.Corner;
			this.Handle(next+1).ConstrainType = HandleConstrainType.Corner;
		}


		// Adapte le point secondaire s'il est en mode "en ligne".
		protected void AdaptPrimaryLine(int rankPrimary, int rankSecondary, out int rankExtremity)
		{
			rankExtremity = rankPrimary - (rankSecondary-rankPrimary)*3;
			if ( rankExtremity < 0 )  rankExtremity = this.handles.Count-2;
			if ( rankExtremity >= this.handles.Count )  rankExtremity = 1;

			if ( this.Handle(rankPrimary).ConstrainType != HandleConstrainType.Smooth )  return;
			int rankOpposite = rankPrimary - (rankSecondary-rankPrimary);
			if ( this.Handle(rankOpposite).Type != HandleType.Hide )  return;

			double dist = Drawing.Point.Distance(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position);
			Drawing.Point pos = new Drawing.Point();
			pos = Drawing.Point.Move(this.Handle(rankPrimary).Position, this.Handle(rankExtremity).Position, dist);
			pos = Drawing.Point.Symmetry(this.Handle(rankPrimary).Position, pos);
			this.Handle(rankSecondary).Position = pos;
		}

		// Déplace une poignée primaire selon les contraintes.
		protected void MovePrimary(int rank, Drawing.Point pos)
		{
			Drawing.Point move = pos-this.Handle(rank).Position;
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
		}

		// Déplace une poignée secondaire selon les contraintes.
		protected void MoveSecondary(int rankPrimary, int rankSecondary, int rankOpposite, Drawing.Point pos)
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
						if ( rankOpposite >= this.handles.Count )  rankOpposite = 1;
					}
					else
					{
						rankOpposite -= 2;
						if ( rankOpposite < 0 )  rankOpposite = this.handles.Count-2;
					}
					this.Handle(rankSecondary).Position = Drawing.Point.Projection(this.Handle(rankPrimary).Position, this.Handle(rankOpposite).Position, pos);
				}
			}
			else	// courbe ?
			{
				if ( type == HandleConstrainType.Symmetric )
				{
					this.Handle(rankOpposite).Position = Drawing.Point.Symmetry(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position);
				}

				if ( type == HandleConstrainType.Smooth )
				{
					double dist = Drawing.Point.Distance(this.Handle(rankPrimary).Position, this.Handle(rankOpposite).Position);
					Drawing.Point p = Drawing.Point.Move(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position, dist);
					this.Handle(rankOpposite).Position = Drawing.Point.Symmetry(this.Handle(rankPrimary).Position, p);
				}
			}
		}

		// Début du déplacement une poignée.
		public override void MoveHandleStarting(int rank, Drawing.Point pos, IconContext iconContext)
		{
			pos = this.Handle((rank/3)*3+1).Position;
			iconContext.ConstrainFixStarting(pos);
		}

		// Déplace une poignée.
		public override void MoveHandleProcess(int rank, Drawing.Point pos, IconContext iconContext)
		{
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				base.MoveHandleProcess(rank, pos, iconContext);
				return;
			}

			iconContext.ConstrainSnapPos(ref pos);

			if ( rank%3 == 0 )  // poignée secondaire ?
			{
				this.MoveSecondary(rank+1, rank, rank+2, pos);
			}

			if ( rank%3 == 1 )  // poignée principale ?
			{
				this.MovePrimary(rank, pos);
			}

			if ( rank%3 == 2 )  // poignée secondaire ?
			{
				this.MoveSecondary(rank-1, rank, rank-2, pos);
			}
			this.dirtyBbox = true;
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainFixStarting(pos);

			if ( this.TotalHandle == 0 )
			{
				this.lastPoint = false;
				this.PropertyBool(3).Bool = false;
			}
			else
			{
				double len = Drawing.Point.Distance(pos, this.Handle(1).Position);
				if ( len <= this.closeMargin )
				{
					pos = this.Handle(1).Position;
					this.lastPoint = true;
					this.PropertyBool(3).Bool = true;
				}
			}

			if ( !this.lastPoint )
			{
				if ( this.TotalHandle == 0 )
				{
					this.HandleAdd(pos, HandleType.Secondary);
					this.HandleAdd(pos, HandleType.Starting);
					this.HandleAdd(pos, HandleType.Secondary);
				}
				else
				{
					this.HandleAdd(pos, HandleType.Secondary);
					this.HandleAdd(pos, HandleType.Primary);
					this.HandleAdd(pos, HandleType.Secondary);

					int rank = this.TotalHandle-6;
					if ( this.Handle(rank).Type == HandleType.Hide )
					{
						this.Handle(rank+2).Position = Drawing.Point.Scale(this.Handle(rank+1).Position, pos, 0.5);
					}
				}
				this.Handle(this.TotalHandle-2).IsSelected = true;
			}

			this.mouseDown = true;
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);

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
				this.Handle(rank-1).Position = Drawing.Point.Symmetry(this.Handle(rank).Position, pos);
				this.dirtyBbox = true;
			}
			else
			{
				if ( this.TotalHandle > 0 )
				{
					double len = Drawing.Point.Distance(pos, this.Handle(1).Position);
					if ( len <= this.closeMargin )
					{
						this.Handle(1).Type = HandleType.Ending;
					}
					else
					{
						this.Handle(1).Type = HandleType.Starting;
					}
				}
			}
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			int rank = this.TotalHandle;
			double len = Drawing.Point.Distance(this.Handle(rank-1).Position, this.Handle(rank-2).Position);
			if ( rank <= 3 )
			{
				if ( len <= this.minimalSize )
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
				if ( len <= this.minimalSize )
				{
					this.ContextToLine(rank-6);
				}
			}

			iconContext.ConstrainDelStarting();
			this.mouseDown = false;
		}

		// Indique si la création de l'objet est terminée.
		public override bool CreateIsEnding(IconContext iconContext)
		{
			if ( this.lastPoint )
			{
				this.Handle(1).Type = HandleType.Primary;
				this.Deselect();
				return true;
			}
			else
			{
				return false;
			}
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(IconContext iconContext)
		{
			return true;
		}

		// Termine la création de l'objet. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateEnding(IconContext iconContext)
		{
			int total = this.TotalHandle;
			if ( total <= 3 )  return false;

			this.Handle(1).Type = HandleType.Primary;
			this.Deselect();
			return true;
		}

		
		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			this.bboxThin = Drawing.Rectangle.Empty;
			this.bboxGeom = Drawing.Rectangle.Empty;
			this.bboxFull = Drawing.Rectangle.Empty;
			if ( this.TotalHandle < 3 )  return;

			this.bboxThin = this.RealBoundingBox();

			this.bboxGeom = this.bboxThin;
			this.PropertyLine(0).InflateBoundingBox(ref this.bboxGeom);

			this.bboxFull = this.FullBoundingBox();

			this.bboxGeom.MergeWith(this.PropertyGradient(2).BoundingBoxGeom(this.bboxThin));
			this.bboxFull.MergeWith(this.PropertyGradient(2).BoundingBoxFull(this.bboxThin));
			this.bboxFull.MergeWith(this.bboxGeom);
		}

		// Calcule la bbox qui englobe l'objet et les poignées secondaires.
		protected Drawing.Rectangle FullBoundingBox()
		{
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				bbox.MergeWith(this.Handle(i).Position);
			}
			return bbox;
		}

		// Calcule la bbox qui englobe exactement l'objet géométrique.
		protected Drawing.Rectangle RealBoundingBox()
		{
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			int total = this.TotalHandle;
			for ( int i=0 ; i<total-3 ; i+=3 )
			{
				Drawing.Point p1 = this.Handle(i+1).Position;
				Drawing.Point s1 = this.Handle(i+2).Position;
				Drawing.Point s2 = this.Handle(i+3).Position;
				Drawing.Point p2 = this.Handle(i+4).Position;
				ObjectBezier.BboxBezier(ref bbox, p1, s1, s2, p2);
			}
			if ( this.PropertyBool(3).Bool )  // fermé ?
			{
				Drawing.Point p1 = this.Handle(total-2).Position;
				Drawing.Point s1 = this.Handle(total-1).Position;
				Drawing.Point s2 = this.Handle(0).Position;
				Drawing.Point p2 = this.Handle(1).Position;
				ObjectBezier.BboxBezier(ref bbox, p1, s1, s2, p2);
			}
			return bbox;
		}

		// Ajoute un courbe de Bézier dans la bbox.
		static protected void BboxBezier(ref Drawing.Rectangle bbox, Drawing.Point p1, Drawing.Point s1, Drawing.Point s2, Drawing.Point p2)
		{
			double step = 1.0/10.0;  // nombre arbitraire de 10 subdivisions
			for ( double t=0 ; t<=1.0 ; t+=step )
			{
				bbox.MergeWith(Drawing.Point.Bezier(p1, s1, s2, p2, t));
			}
		}

		// Crée le chemin de l'objet.
		protected Drawing.Path PathBuild()
		{
			Drawing.Path path = new Drawing.Path();

			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( i == 0 )
				{
					path.MoveTo(this.Handle(1).Position);
				}
				else
				{
					path.CurveTo(this.Handle(i-1).Position, this.Handle(i).Position, this.Handle(i+1).Position);
				}
			}
			if ( this.PropertyBool(3).Bool )  // fermé ?
			{
				path.CurveTo(this.Handle(total-1).Position, this.Handle(0).Position, this.Handle(1).Position);
				path.Close();
			}

			return path;
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			if ( this.isHide )  return;
			base.DrawGeometry(graphics, iconContext);

			int total = this.TotalHandle;
			if ( total < 3 )  return;

			Drawing.Path path = this.PathBuild();
			this.PropertyGradient(2).Render(graphics, iconContext, path, this.BoundingBoxThin);

			graphics.Rasterizer.AddOutline(path, this.PropertyLine(0).Width, this.PropertyLine(0).Cap, this.PropertyLine(0).Join, this.PropertyLine(0).Limit);
			graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(1).Color));

			if ( this.IsHilite && iconContext.IsEditable )
			{
				if ( this.PropertyGradient(2).IsVisible() )
				{
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(iconContext.HiliteSurfaceColor);
				}

				graphics.Rasterizer.AddOutline(path, this.PropertyLine(0).Width+iconContext.HiliteSize, this.PropertyLine(0).Cap, this.PropertyLine(0).Join, this.PropertyLine(0).Limit);
				graphics.RenderSolid(iconContext.HiliteOutlineColor);
			}

			if ( this.IsSelected() && iconContext.IsEditable && !this.IsGlobalSelected() )
			{
				double initialWidth = graphics.LineWidth;
				graphics.LineWidth = 1.0/iconContext.ScaleX;

				for ( int i=0 ; i<total ; i+=3 )
				{
					graphics.AddLine(this.Handle(i+0).Position, this.Handle(i+1).Position);
					graphics.AddLine(this.Handle(i+1).Position, this.Handle(i+2).Position);
					graphics.RenderSolid(Drawing.Color.FromBrightness(0.6));
				}

				graphics.LineWidth = initialWidth;
			}
		}


		protected bool				mouseDown = false;
		protected bool				lastPoint;
	}
}
