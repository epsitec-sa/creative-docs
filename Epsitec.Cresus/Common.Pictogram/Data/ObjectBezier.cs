using Epsitec.Common.Support;
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
			PropertyName name = new PropertyName();
			name.Type = PropertyType.Name;
			this.AddProperty(name);

			PropertyLine lineMode = new PropertyLine();
			lineMode.Type = PropertyType.LineMode;
			this.AddProperty(lineMode);

			PropertyColor lineColor = new PropertyColor();
			lineColor.Type = PropertyType.LineColor;
			this.AddProperty(lineColor);

			PropertyArrow arrow = new PropertyArrow();
			arrow.Type = PropertyType.Arrow;
			arrow.Changed += new EventHandler(this.HandleChanged);
			this.AddProperty(arrow);

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

		public override void Dispose()
		{
			if ( this.ExistProperty(3) )  this.PropertyArrow(3).Changed -= new EventHandler(this.HandleChanged);
			base.Dispose();
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

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Drawing.Path pathStart;  bool outlineStart, surfaceStart;
			Drawing.Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Drawing.Path pathLine;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			double width = System.Math.Max(this.PropertyLine(1).PatternWidth/2, this.minimalWidth);
			
			if (                 AbstractObject.DetectOutline(pathLine,  width, pos) )  return true;
			if ( outlineStart && AbstractObject.DetectOutline(pathStart, width, pos) )  return true;
			if ( outlineEnd   && AbstractObject.DetectOutline(pathEnd,   width, pos) )  return true;

			if ( surfaceStart && AbstractObject.DetectSurface(pathStart, pos) )  return true;
			if ( surfaceEnd   && AbstractObject.DetectSurface(pathEnd,   pos) )  return true;

			if ( this.PropertyGradient(4).IsVisible() )
			{
				pathLine.Close();
				if ( AbstractObject.DetectSurface(pathLine, pos) )  return true;
			}

			return false;
		}

		// Détecte si la souris est sur le pourtour de l'objet.
		// Retourne le rank de la poignée de départ, ou -1
		protected int DetectOutline(Drawing.Point pos)
		{
			Drawing.Path pathStart;  bool outlineStart, surfaceStart;
			Drawing.Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Drawing.Path pathLine;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			double width = System.Math.Max(this.PropertyLine(1).PatternWidth/2, this.minimalWidth);
			int rank = AbstractObject.DetectOutlineRank(pathLine, width, pos);
			if ( rank != -1 )  rank *= 3;
			return rank;
		}

		// Déplace tout l'objet.
		public override void MoveAll(Drawing.Point move, bool all)
		{
			int total = this.TotalHandlePrimary;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( all || this.Handle(i+1).IsSelected )
				{
					this.Handle(i+0).Position += move;
					this.Handle(i+1).Position += move;
					this.Handle(i+2).Position += move;
				}
			}
			this.UpdateHandle();
			this.dirtyBbox = true;
		}

		// Sélectionne toutes les poignées de l'objet dans un rectangle.
		public override void Select(Drawing.Rectangle rect)
		{
			int total = this.TotalHandlePrimary;
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
		protected void ContextAddHandle(Drawing.Point pos, int rank)
		{
			for ( int i=0 ; i<3 ; i++ )
			{
				Handle handle = new Handle();
				handle.Position = pos;
				handle.Type = (i==1) ? HandleType.Primary : HandleType.Bezier;
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
				double t = Drawing.Point.FindBezierParameter(this.Handle(prev+1).Position, this.Handle(prev+2).Position, this.Handle(next+0).Position, this.Handle(next+1).Position, pos);
				this.Handle(curr+1).Position = Drawing.Point.FromBezier(this.Handle(prev+1).Position, this.Handle(prev+2).Position, this.Handle(next+0).Position, this.Handle(next+1).Position, t);
				pos = Drawing.Point.Scale(this.Handle(prev+2).Position, this.Handle(next+0).Position, t);
				this.Handle(prev+2).Position = Drawing.Point.Scale(this.Handle(prev+1).Position, this.Handle(prev+2).Position, t);
				this.Handle(next+0).Position = Drawing.Point.Scale(this.Handle(next+1).Position, this.Handle(next+0).Position, 1-t);
				this.Handle(curr+0).Position = Drawing.Point.Scale(this.Handle(prev+2).Position, pos, t);
				this.Handle(curr+2).Position = Drawing.Point.Scale(this.Handle(next+0).Position, pos, 1-t);

				this.Handle(curr+1).ConstrainType = HandleConstrainType.Smooth;
				if ( this.Handle(prev+1).ConstrainType == HandleConstrainType.Symmetric )  this.Handle(prev+1).ConstrainType = HandleConstrainType.Smooth;
				if ( this.Handle(next+1).ConstrainType == HandleConstrainType.Symmetric )  this.Handle(next+1).ConstrainType = HandleConstrainType.Smooth;
			}
			this.dirtyBbox = true;
			this.UpdateHandle();
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
			this.dirtyBbox = true;
			this.UpdateHandle();
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
			this.dirtyBbox = true;
		}

		// Conversion d'un segement en courbe.
		protected void ContextToCurve(int rank)
		{
			int next = rank+3;
			if ( next >= this.handles.Count )  next = 0;
			this.Handle(rank+2).Position = Drawing.Point.Scale(this.Handle(rank+1).Position, this.Handle(next+1).Position, 0.25);
			this.Handle(next+0).Position = Drawing.Point.Scale(this.Handle(next+1).Position, this.Handle(rank+1).Position, 0.25);
			this.Handle(rank+2).Type = HandleType.Bezier;
			this.Handle(next+0).Type = HandleType.Bezier;
			this.Handle(rank+1).ConstrainType = HandleConstrainType.Corner;
			this.Handle(next+1).ConstrainType = HandleConstrainType.Corner;
			this.dirtyBbox = true;
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
			this.dirtyBbox = true;
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
			this.dirtyBbox = true;
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
			if ( rank < this.TotalHandlePrimary )
			{
				pos = this.Handle((rank/3)*3+1).Position;
				iconContext.ConstrainFixStarting(pos);
			}
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
			iconContext.SnapGrid(ref pos);

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
			else
			{
				if ( rank == this.HandleArrowRank(0) )  // pp1 ?
				{
					double d = Drawing.Point.Distance(this.Handle(1).Position, pos);
					this.PropertyArrow(3).Length1 = d;
				}

				if ( rank == this.HandleArrowRank(1) )  // pp2 ?
				{
					double d = Drawing.Point.Distance(this.Handle(this.TotalHandlePrimary-2).Position, pos);
					this.PropertyArrow(3).Length2 = d;
				}
			}
			this.UpdateHandle();
			this.dirtyBbox = true;
		}

		// Indique si le déplacement d'une poignée doit se répercuter sur les propriétés.
		public override bool IsMoveHandlePropertyChanged(int rank)
		{
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				return base.IsMoveHandlePropertyChanged(rank);
			}
			return ( rank >= this.TotalHandlePrimary );
		}

		// Retourne la propriété modifiée en déplaçant une poignée.
		public override AbstractProperty MoveHandleProperty(int rank)
		{
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				return base.MoveHandleProperty(rank);
			}
			if ( rank >= this.TotalHandlePrimary )  return this.PropertyArrow(3);
			return null;
		}


		// Déplace globalement l'objet.
		public override void MoveGlobal(GlobalModifierData initial, GlobalModifierData final, bool all)
		{
			base.MoveGlobal(initial, final, all);
			this.UpdateHandle();
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainFixStarting(pos);

			if ( this.TotalHandle == 0 )
			{
				this.lastPoint = false;
				this.PropertyBool(5).Bool = false;
			}
			else
			{
				double len = Drawing.Point.Distance(pos, this.Handle(1).Position);
				if ( len <= this.closeMargin )
				{
					pos = this.Handle(1).Position;
					this.lastPoint = true;
					this.PropertyBool(5).Bool = true;
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
			iconContext.SnapGrid(ref pos);

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
				this.UpdateHandle();
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
			this.UpdateHandle();
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

		
		private void HandleChanged(object sender)
		{
			this.UpdateHandle();
		}

		// Met à jour les poignées pour les profondeurs des flèches.
		protected void UpdateHandle()
		{
			int total = this.TotalHandlePrimary;
			if ( total < 6 )  return;

			Drawing.Point p1, p2, pp1, pp2;
			p1 = this.Handle(1).Position;
			p2 = this.Handle(2).Position;
			pp1 = Drawing.Point.Move(p1, p2, this.PropertyArrow(3).GetLength(0));
			p1 = this.Handle(total-2).Position;
			p2 = this.Handle(total-3).Position;
			pp2 = Drawing.Point.Move(p1, p2, this.PropertyArrow(3).GetLength(1));
			int r1 = this.HandleArrowRank(0);
			int r2 = this.HandleArrowRank(1);
			total += ((r1==-1)?0:1) + ((r2==-1)?0:1);

			// Supprime les poignées en trop.
			while ( this.handles.Count > total )
			{
				this.HandleDelete(this.handles.Count-1);
			}

			// Ajoute les poignées manquantes.
			while ( this.handles.Count < total )
			{
				this.HandleAdd(pp1, HandleType.Secondary);
			}

			if ( r1 != -1 )
			{
				this.Handle(r1).Position = pp1;
				this.Handle(r1).IsSelected = this.Handle(0).IsSelected;
			}

			if ( r2 != -1 )
			{
				this.Handle(r2).Position = pp2;
				this.Handle(r2).IsSelected = this.Handle(this.TotalHandlePrimary-1).IsSelected;
			}
		}

		// Retourne le rang d'une poignée secondaire.
		protected int HandleArrowRank(int extremity)
		{
			if ( this.PropertyArrow(3).GetArrowType(extremity) == ArrowType.Right )  return -1;

			int total = this.TotalHandlePrimary;
			if ( extremity == 0 )
			{
				return total;
			}
			else
			{
				if ( this.PropertyArrow(3).GetArrowType(0) == ArrowType.Right )  return total;
				return total+1;
			}
		}


		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			Drawing.Path pathStart;  bool outlineStart, surfaceStart;
			Drawing.Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Drawing.Path pathLine;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			Drawing.Rectangle bboxStart = AbstractObject.ComputeBoundingBox(pathStart);
			Drawing.Rectangle bboxEnd   = AbstractObject.ComputeBoundingBox(pathEnd);
			Drawing.Rectangle bboxLine  = AbstractObject.ComputeBoundingBox(pathLine);

			this.bboxThin = bboxLine;
			this.bboxThin.MergeWith(this.Handle(1).Position);
			this.bboxThin.MergeWith(this.Handle(this.TotalHandlePrimary-2).Position);

			this.PropertyLine(1).InflateBoundingBox(ref bboxLine);
			this.bboxGeom = bboxLine;

			if ( outlineStart )  this.PropertyLine(1).InflateBoundingBox(ref bboxStart);
			this.bboxGeom.MergeWith(bboxStart);

			if ( outlineEnd )  this.PropertyLine(1).InflateBoundingBox(ref bboxEnd);
			this.bboxGeom.MergeWith(bboxEnd);

			this.bboxGeom.MergeWith(this.bboxThin);
			this.bboxFull = this.bboxGeom;
			this.bboxFull.MergeWith(this.FullBoundingBox());

			this.bboxGeom.MergeWith(this.PropertyGradient(4).BoundingBoxGeom(this.bboxThin));
			this.bboxFull.MergeWith(this.PropertyGradient(4).BoundingBoxFull(this.bboxThin));
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

		// Retourne le nombre de poignées principales.
		// Ne compte pas les 1 ou 2 poignées secondaires à la fin, utilisées
		// pour PropertyArrow.
		protected int TotalHandlePrimary
		{
			get
			{
				int total = this.handles.Count;
				while ( total > 0 && this.Handle(total-1).Type == HandleType.Secondary )
				{
					total --;
				}
				return total;
			}
		}

		// Crée les chemins de l'objet.
		protected void PathBuild(IconContext iconContext,
								 out Drawing.Path pathStart, out bool outlineStart, out bool surfaceStart,
								 out Drawing.Path pathEnd,   out bool outlineEnd,   out bool surfaceEnd,
								 out Drawing.Path pathLine)
		{
			pathStart = new Drawing.Path();
			pathEnd   = new Drawing.Path();
			pathLine  = new Drawing.Path();

			double zoom = AbstractProperty.DefaultZoom(iconContext);
			pathStart.DefaultZoom = zoom;
			pathEnd.DefaultZoom = zoom;
			pathLine.DefaultZoom = zoom;

			int total = this.TotalHandlePrimary;
			if ( total < 6 )
			{
				outlineStart = false;
				surfaceStart = false;
				outlineEnd   = false;
				surfaceEnd   = false;
				return;
			}

			Drawing.Point p1, p2, pp1, pp2;
			double w = this.PropertyLine(1).Width;
			Drawing.CapStyle cap = this.PropertyLine(1).Cap;
			p1 = this.Handle(1).Position;
			p2 = this.Handle(2).Position;
			pp1 = this.PropertyArrow(3).PathExtremity(pathStart, 0, w,cap, p1,p2, out outlineStart, out surfaceStart);
			p1 = this.Handle(total-2).Position;
			p2 = this.Handle(total-3).Position;
			pp2 = this.PropertyArrow(3).PathExtremity(pathEnd,   1, w,cap, p1,p2, out outlineEnd,   out surfaceEnd);

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
			if ( this.PropertyBool(5).Bool )  // fermé ?
			{
				pathLine.CurveTo(this.Handle(total-1).Position, this.Handle(0).Position, pp1);
				pathLine.Close();
			}
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects)
		{
			if ( base.IsFullHide(iconContext) )  return;
			base.DrawGeometry(graphics, iconContext, iconObjects);

			int total = this.TotalHandlePrimary;
			if ( total < 3 )  return;

			Drawing.Path pathStart;  bool outlineStart, surfaceStart;
			Drawing.Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Drawing.Path pathLine;
			this.PathBuild(iconContext,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			this.PropertyGradient(4).Render(graphics, iconContext, pathLine, this.BoundingBoxThin);

			if ( outlineStart )
			{
				this.PropertyLine(1).AddOutline(graphics, pathStart, 0.0);
				graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(2).Color));
			}
			if ( surfaceStart )
			{
				graphics.Rasterizer.AddSurface(pathStart);
				graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(2).Color));
			}

			if ( outlineEnd )
			{
				this.PropertyLine(1).AddOutline(graphics, pathEnd, 0.0);
				graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(2).Color));
			}
			if ( surfaceEnd )
			{
				graphics.Rasterizer.AddSurface(pathEnd);
				graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(2).Color));
			}

			this.PropertyLine(1).DrawPath(graphics, iconContext, iconObjects, pathLine, this.PropertyColor(2).Color);

			if ( this.IsHilite && iconContext.IsEditable )
			{
				if ( this.PropertyGradient(4).IsVisible() )
				{
					graphics.Rasterizer.AddSurface(pathLine);
					graphics.RenderSolid(iconContext.HiliteSurfaceColor);
				}

				if ( outlineStart )
				{
					this.PropertyLine(1).AddOutline(graphics, pathStart, iconContext.HiliteSize);
					graphics.RenderSolid(iconContext.HiliteOutlineColor);
				}
				if ( surfaceStart )
				{
					graphics.Rasterizer.AddSurface(pathStart);
					graphics.RenderSolid(iconContext.HiliteOutlineColor);
				}

				if ( outlineEnd )
				{
					this.PropertyLine(1).AddOutline(graphics, pathEnd, iconContext.HiliteSize);
					graphics.RenderSolid(iconContext.HiliteOutlineColor);
				}
				if ( surfaceEnd )
				{
					graphics.Rasterizer.AddSurface(pathEnd);
					graphics.RenderSolid(iconContext.HiliteOutlineColor);
				}

				this.PropertyLine(1).AddOutline(graphics, pathLine, iconContext.HiliteSize);
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
