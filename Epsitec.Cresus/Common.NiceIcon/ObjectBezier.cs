using System.Xml.Serialization;

namespace Epsitec.Common.NiceIcon
{
	/// <summary>
	/// La classe ObjectBezier est la classe de l'objet graphique "courbes de B�zier".
	/// </summary>
	public class ObjectBezier : AbstractObject
	{
		public ObjectBezier()
		{
		}

		public override void CreateProperties()
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


		// Nom de l'ic�ne.
		public override string IconName
		{
			get { return @"manifest:Epsitec.Common.NiceIcon/Images.bezier.png"; }
		}


		// D�tecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			if ( this.DetectOutline(pos) != -1 )  return true;
			if ( this.DetectFill(pos) )  return true;
			return false;
		}

		// D�tecte si la souris est sur le pourtour de l'objet.
		// Retourne le rank de la poign�e de d�part, ou -1
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
				if ( Widgets.Math.Detect(p1,s1,s2,p2, pos, width) )  return i;
			}
			if ( this.PropertyBool(3).Bool )  // ferm� ?
			{
				Drawing.Point p1 = this.Handle(total-2).Position;
				Drawing.Point s1 = this.Handle(total-1).Position;
				Drawing.Point s2 = this.Handle(0).Position;
				Drawing.Point p2 = this.Handle(1).Position;
				if ( Widgets.Math.Detect(p1,s1,s2,p2, pos, width) )  return total-3;
			}
			return -1;
		}

		// D�tecte si la souris est dans la surface de l'objet.
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
			if ( this.PropertyBool(3).Bool )  // ferm� ?
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

		// Ajoute un courbe de B�zier dans la bbox.
		protected void BboxBezier(ref Drawing.Rectangle bbox, Drawing.Point p1, Drawing.Point s1, Drawing.Point s2, Drawing.Point p2)
		{
			double step = 1.0/10.0;  // nombre arbitraire de 10 subdivisions
			for ( double t=0 ; t<=1.0 ; t+=step )
			{
				Drawing.Point a = Widgets.Math.Bezier(p1, s1, s2, p2, t);
				bbox.Left   = System.Math.Min(bbox.Left,   a.X);
				bbox.Right  = System.Math.Max(bbox.Right,  a.X);
				bbox.Bottom = System.Math.Min(bbox.Bottom, a.Y);
				bbox.Top    = System.Math.Max(bbox.Top,    a.Y);
			}
		}

		// D�tecte si l'objet est dans un rectangle.
		public override bool Detect(Drawing.Rectangle rect)
		{
			int total = this.TotalHandle;
			if ( total < 3 )  return false;

			Drawing.Rectangle bbox = new Drawing.Rectangle();
			Drawing.Point p = this.Handle(1).Position;
			bbox.Left   = p.X;
			bbox.Right  = p.X;
			bbox.Bottom = p.Y;
			bbox.Top    = p.Y;

			for ( int i=0 ; i<total-3 ; i+=3 )
			{
				Drawing.Point p1 = this.Handle(i+1).Position;
				Drawing.Point s1 = this.Handle(i+2).Position;
				Drawing.Point s2 = this.Handle(i+3).Position;
				Drawing.Point p2 = this.Handle(i+4).Position;
				this.BboxBezier(ref bbox, p1, s1, s2, p2);
			}
			if ( this.PropertyBool(3).Bool )  // ferm� ?
			{
				Drawing.Point p1 = this.Handle(total-2).Position;
				Drawing.Point s1 = this.Handle(total-1).Position;
				Drawing.Point s2 = this.Handle(0).Position;
				Drawing.Point p2 = this.Handle(1).Position;
				this.BboxBezier(ref bbox, p1, s1, s2, p2);
			}

			double width = System.Math.Max(this.PropertyLine(0).Width/2, this.minimalWidth);
			bbox.Inflate(width, width);
			return rect.Contains(bbox);
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
				list.Add(item);  // s�parateur

				if ( this.Handle(rank+2).Type == HandleType.Hide )
				{
					item = new ContextMenuItem();
					item.Name = "curve";
					item.Text = "Courbe";
					list.Add(item);
				}
				else
				{
					item = new ContextMenuItem();
					item.Name = "line";
					item.Text = "Droit";
					list.Add(item);
				}

				item = new ContextMenuItem();
				list.Add(item);  // s�parateur

				item = new ContextMenuItem();
				item.Name = "handleAdd";
				item.Icon = @"manifest:Epsitec.Common.NiceIcon/Images.add.png";
				item.Text = "Ajouter un point";
				list.Add(item);
			}
			else	// sur un point ?
			{
				if ( handleRank%3 == 1 )  // poign�e principale ?
				{
					if ( this.Handle(handleRank-1).Type != HandleType.Hide && this.Handle(handleRank+1).Type != HandleType.Hide )
					{
						item = new ContextMenuItem();
						list.Add(item);  // s�parateur

						HandleConstrainType type = this.Handle(handleRank).ConstrainType;

						item = new ContextMenuItem();
						item.Name = "handleSym";
						if ( type == HandleConstrainType.Symetric )  item.Icon = @"manifest:Epsitec.Common.NiceIcon/Images.check.png";
						item.Text = "Symetrique";
						list.Add(item);

						item = new ContextMenuItem();
						item.Name = "handleSmooth";
						if ( type == HandleConstrainType.Smooth )  item.Icon = @"manifest:Epsitec.Common.NiceIcon/Images.check.png";
						item.Text = "Lisse";
						list.Add(item);

						item = new ContextMenuItem();
						item.Name = "handleCorner";
						if ( type == HandleConstrainType.Corner )  item.Icon = @"manifest:Epsitec.Common.NiceIcon/Images.check.png";
						item.Text = "Anguleux";
						list.Add(item);
					}
					else if ( this.Handle(handleRank-1).Type != HandleType.Hide || this.Handle(handleRank+1).Type != HandleType.Hide )
					{
						item = new ContextMenuItem();
						list.Add(item);  // s�parateur

						HandleConstrainType type = this.Handle(handleRank).ConstrainType;

						item = new ContextMenuItem();
						item.Name = "handleSmooth";
						if ( type == HandleConstrainType.Smooth )  item.Icon = @"manifest:Epsitec.Common.NiceIcon/Images.check.png";
						item.Text = "En ligne";
						list.Add(item);

						item = new ContextMenuItem();
						item.Name = "handleCorner";
						if ( type != HandleConstrainType.Smooth )  item.Icon = @"manifest:Epsitec.Common.NiceIcon/Images.check.png";
						item.Text = "Libre";
						list.Add(item);
					}

					if ( this.handles.Count >= 3*3 )
					{
						item = new ContextMenuItem();
						list.Add(item);  // s�parateur

						item = new ContextMenuItem();
						item.Name = "handleDelete";
						item.Icon = @"manifest:Epsitec.Common.NiceIcon/Images.sub.png";
						item.Text = "Enlever le point";
						list.Add(item);
					}
				}
			}
		}

		// Ex�cute une commande du menu contextuel.
		public override void ContextCommand(string cmd, Drawing.Point pos, int handleRank)
		{
			int rank = this.DetectOutline(pos);

			if ( cmd == "line" )
			{
				if ( rank == -1 )  return;
				this.ContextToLine(rank);
			}

			if ( cmd == "curve" )
			{
				if ( rank == -1 )  return;
				this.ContextToCurve(rank);
			}

			if ( cmd == "handleAdd" )
			{
				if ( rank == -1 )  return;
				this.ContextAddHandle(pos, rank);
			}

			if ( cmd == "handleSym" )
			{
				this.ContextSym(handleRank);
			}

			if ( cmd == "handleSmooth" )
			{
				this.ContextSmooth(handleRank);
			}

			if ( cmd == "handleCorner" )
			{
				this.ContextCorner(handleRank);
			}

			if ( cmd == "handleDelete" )
			{
				this.ContextSubHandle(pos, handleRank);
			}
		}

		// Passe le point en mode sym�trique.
		protected void ContextSym(int rank)
		{
			this.Handle(rank).ConstrainType = HandleConstrainType.Symetric;
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

		// Ajoute une poign�e sans changer l'aspect de la courbe.
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
				pos = Widgets.Math.Projection(this.Handle(prev+1).Position, this.Handle(next+1).Position, pos);
				this.Handle(curr+0).Position = pos;
				this.Handle(curr+1).Position = pos;
				this.Handle(curr+2).Position = pos;
				this.Handle(curr+0).Type = HandleType.Hide;
				this.Handle(curr+2).Type = HandleType.Hide;
			}
			else
			{
				double t = Widgets.Math.Bezier(this.Handle(prev+1).Position, this.Handle(prev+2).Position, this.Handle(next+0).Position, this.Handle(next+1).Position, pos);
				this.Handle(curr+1).Position = Widgets.Math.Bezier(this.Handle(prev+1).Position, this.Handle(prev+2).Position, this.Handle(next+0).Position, this.Handle(next+1).Position, t);
				pos = Widgets.Math.VectorMul(this.Handle(prev+2).Position, this.Handle(next+0).Position, t);
				this.Handle(prev+2).Position = Widgets.Math.VectorMul(this.Handle(prev+1).Position, this.Handle(prev+2).Position, t);
				this.Handle(next+0).Position = Widgets.Math.VectorMul(this.Handle(next+1).Position, this.Handle(next+0).Position, 1-t);
				this.Handle(curr+0).Position = Widgets.Math.VectorMul(this.Handle(prev+2).Position, pos, t);
				this.Handle(curr+2).Position = Widgets.Math.VectorMul(this.Handle(next+0).Position, pos, 1-t);

				this.Handle(curr+1).ConstrainType = HandleConstrainType.Smooth;
				if ( this.Handle(prev+1).ConstrainType == HandleConstrainType.Symetric )  this.Handle(prev+1).ConstrainType = HandleConstrainType.Smooth;
				if ( this.Handle(next+1).ConstrainType == HandleConstrainType.Symetric )  this.Handle(next+1).ConstrainType = HandleConstrainType.Smooth;
			}
		}

		// Supprime une poign�e sans changer l'aspect de la courbe.
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
			this.Handle(rank+2).Position = Widgets.Math.VectorMul(this.Handle(rank+1).Position, this.Handle(next+1).Position, 0.25);
			this.Handle(next+0).Position = Widgets.Math.VectorMul(this.Handle(next+1).Position, this.Handle(rank+1).Position, 0.25);
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

			double dist = Widgets.Math.Distance(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position);
			Drawing.Point pos = new Drawing.Point();
			pos = Widgets.Math.Avance(this.Handle(rankPrimary).Position, this.Handle(rankExtremity).Position, dist);
			pos = Widgets.Math.Symetry(this.Handle(rankPrimary).Position, pos);
			this.Handle(rankSecondary).Position = pos;
		}

		// D�place une poign�e primaire selon les contraintes.
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

		// D�place une poign�e secondaire selon les contraintes.
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
					this.Handle(rankSecondary).Position = Widgets.Math.Projection(this.Handle(rankPrimary).Position, this.Handle(rankOpposite).Position, pos);
				}
			}
			else	// courbe ?
			{
				if ( type == HandleConstrainType.Symetric )
				{
					this.Handle(rankOpposite).Position = Widgets.Math.Symetry(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position);
				}

				if ( type == HandleConstrainType.Smooth )
				{
					double dist = Widgets.Math.Distance(this.Handle(rankPrimary).Position, this.Handle(rankOpposite).Position);
					Drawing.Point p = Widgets.Math.Avance(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position, dist);
					this.Handle(rankOpposite).Position = Widgets.Math.Symetry(this.Handle(rankPrimary).Position, p);
				}
			}
		}

		// D�place une poign�e.
		public override void MoveHandle(int rank, Drawing.Point pos)
		{
			if ( rank%3 == 0 )  // poign�e secondaire ?
			{
				this.MoveSecondary(rank+1, rank, rank+2, pos);
			}

			if ( rank%3 == 1 )  // poign�e principale ?
			{
				this.MovePrimary(rank, pos);
			}

			if ( rank%3 == 2 )  // poign�e secondaire ?
			{
				this.MoveSecondary(rank-1, rank, rank-2, pos);
			}
		}


		// D�but de la cr�ation d'un objet.
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
				double len = Widgets.Math.Distance(pos, this.Handle(1).Position);
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
						this.Handle(rank+2).Position = Widgets.Math.VectorMul(this.Handle(rank+1).Position, pos, 0.5);
					}
				}
				this.Handle(this.TotalHandle-2).IsSelected = true;
			}

			this.mouseDown = true;
		}

		// D�placement pendant la cr�ation d'un objet.
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
				this.Handle(rank-1).Position = Widgets.Math.Symetry(this.Handle(rank).Position, pos);
			}
			else
			{
				if ( this.TotalHandle > 0 )
				{
					double len = Widgets.Math.Distance(pos, this.Handle(1).Position);
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

		// Fin de la cr�ation d'un objet.
		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			int rank = this.TotalHandle;
			double len = Widgets.Math.Distance(this.Handle(rank-1).Position, this.Handle(rank-2).Position);
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

		// Indique si la cr�ation de l'objet est termin�e.
		public override bool CreateIsEnding(IconContext iconContext)
		{
			if ( this.lastPoint )
			{
				this.Handle(1).Type = HandleType.Primary;
				this.DeselectObject();
				return true;
			}
			else
			{
				return false;
			}
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit �tre d�truit.
		public override bool CreateIsExist(IconContext iconContext)
		{
			return true;
		}

		// Termine la cr�ation de l'objet. Retourne false si l'objet ne peut
		// pas exister et doit �tre d�truit.
		public override bool CreateEnding(IconContext iconContext)
		{
			int total = this.TotalHandle;
			if ( total <= 3 )  return false;

			this.Handle(1).Type = HandleType.Primary;
			this.DeselectObject();
			return true;
		}

		
		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			base.DrawGeometry(graphics, iconContext);

			int total = this.TotalHandle;
			if ( total < 3 )  return;

			bool isSelected = this.IsSelected();

			Drawing.Path path = new Drawing.Path();
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
			if ( this.PropertyBool(3).Bool )  // ferm� ?
			{
				path.CurveTo(this.Handle(total-1).Position, this.Handle(0).Position, this.Handle(1).Position);
				path.Close();
			}

			graphics.Rasterizer.AddSurface(path);
			Drawing.Rectangle rect = path.ComputeBounds();
			this.PropertyGradient(2).Render(graphics, iconContext, rect);

			graphics.Rasterizer.AddOutline(path, this.PropertyLine(0).Width, this.PropertyLine(0).Cap, this.PropertyLine(0).Join);
			graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(1).Color));

			if ( this.IsHilite && iconContext.IsEditable )
			{
				graphics.Rasterizer.AddOutline(path, this.PropertyLine(0).Width+iconContext.HiliteSize, this.PropertyLine(0).Cap, this.PropertyLine(0).Join);
				graphics.RenderSolid(iconContext.HiliteColor);
			}

			if ( isSelected && iconContext.IsEditable )
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
