using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectPoly est la classe de l'objet graphique "polygone".
	/// </summary>
	public class ObjectPoly : AbstractObject
	{
		public ObjectPoly()
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

			PropertyCorner corner = new PropertyCorner();
			corner.Type = PropertyType.Corner;
			this.AddProperty(corner);
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectPoly();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/poly1.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Drawing.Path path = this.PathBuild();

			double width = System.Math.Max(this.PropertyLine(0).Width/2, this.minimalWidth);
			if ( base.DetectOutline(path, width, pos) )  return true;

			if ( this.PropertyGradient(2).IsVisible() )
			{
				path.Close();
				if ( base.DetectFill(path, pos) )  return true;
			}
			return false;
		}

		// Détecte si la souris est sur le pourtour de l'objet.
		// Retourne le rank de la poignée de départ, ou -1
		protected int DetectOutline(Drawing.Point pos)
		{
			int total = this.TotalHandle;
			double width = System.Math.Max(this.PropertyLine(0).Width/2, this.minimalWidth);
			for ( int i=0 ; i<total-1 ; i++ )
			{
				Drawing.Point p1 = this.Handle(i+0).Position;
				Drawing.Point p2 = this.Handle(i+1).Position;
				if ( Drawing.Point.Detect(p1,p2, pos, width) )  return i;
			}
			if ( this.PropertyBool(3).Bool && total > 2 )  // fermé ?
			{
				Drawing.Point p1 = this.Handle(0).Position;
				Drawing.Point p2 = this.Handle(total-1).Position;
				if ( Drawing.Point.Detect(p1,p2, pos, width) )  return total-1;
			}
			return -1;
		}

		// Détecte si la souris est dans la surface de l'objet.
		protected bool DetectFill(Drawing.Point pos)
		{
			int total = this.TotalHandle;
			if ( !this.PropertyGradient(2).IsVisible() )  return false;
			InsideSurface surf = new InsideSurface(pos, total);
			for ( int i=0 ; i<total-1 ; i++ )
			{
				Drawing.Point p1 = this.Handle(i+0).Position;
				Drawing.Point p2 = this.Handle(i+1).Position;
				surf.AddLine(p1, p2);
			}
			if ( true )  // toujours comme si fermé
			{
				Drawing.Point p1 = this.Handle(0).Position;
				Drawing.Point p2 = this.Handle(total-1).Position;
				surf.AddLine(p1, p2);
			}
			return surf.IsInside();
		}


		// Donne le contenu du menu contextuel.
		public override void ContextMenu(System.Collections.ArrayList list, Drawing.Point pos, int handleRank)
		{
			ContextMenuItem item;

			if ( handleRank == -1 )
			{
				if ( this.DetectOutline(pos) == -1 )  return;

				item = new ContextMenuItem();
				list.Add(item);  // séparateur

				item = new ContextMenuItem();
				item.Command = "Object";
				item.Name = "HandleAdd";
				item.Icon = @"file:images/add1.icon";
				item.Text = "Ajouter un point";
				list.Add(item);
			}
			else
			{
				if ( this.handles.Count > 2 )
				{
					item = new ContextMenuItem();
					list.Add(item);  // séparateur

					HandleConstrainType type = this.Handle(handleRank).ConstrainType;

					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "HandleSym";
					item.IconActiveNo = @"file:images/activeno1.icon";
					item.IconActiveYes = @"file:images/activeyes1.icon";
					item.Active = ( type == HandleConstrainType.Symmetric );
					item.Text = "Coin quelconque";
					list.Add(item);

					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "HandleSimply";
					item.IconActiveNo = @"file:images/activeno1.icon";
					item.IconActiveYes = @"file:images/activeyes1.icon";
					item.Active = ( type == HandleConstrainType.Simply );
					item.Text = "Coin toujours droit";
					list.Add(item);

					item = new ContextMenuItem();
					list.Add(item);  // séparateur

					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "HandleDelete";
					item.Icon = @"file:images/sub1.icon";
					item.Text = "Enlever le point";
					list.Add(item);
				}
			}
		}

		// Exécute une commande du menu contextuel.
		public override void ContextCommand(string cmd, Drawing.Point pos, int handleRank)
		{
			if ( cmd == "HandleAdd" )
			{
				int rank = this.DetectOutline(pos);
				if ( rank == -1 )  return;

				int next = rank+1;
				if ( next >= this.handles.Count )  next = 0;
				Drawing.Point p = Drawing.Point.Projection(this.Handle(rank).Position, this.Handle(next).Position, pos);

				Handle handle = new Handle();
				handle.Position = p;
				handle.Type = HandleType.Primary;
				handle.IsSelected = true;
				this.HandleInsert(rank+1, handle);
			}

			if ( cmd == "HandleDelete" )
			{
				this.HandleDelete(handleRank);
			}

			if ( cmd == "HandleSym" )
			{
				this.Handle(handleRank).ConstrainType = HandleConstrainType.Symmetric;
			}

			if ( cmd == "HandleSimply" )
			{
				this.Handle(handleRank).ConstrainType = HandleConstrainType.Simply;
			}
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);

			if ( this.TotalHandle == 0 )
			{
				this.PropertyBool(3).Bool = false;
				this.HandleAdd(pos, HandleType.Starting);
				this.Handle(0).IsSelected = true;
			}
			else
			{
				this.HandleAdd(pos, HandleType.Primary);
				this.Handle(this.TotalHandle-1).IsSelected = true;
			}

			this.mouseDown = true;
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);

			int rank = this.TotalHandle-1;
			if ( rank > 0 )
			{
				double len = Drawing.Point.Distance(this.Handle(0).Position, pos);
				if ( len <= this.closeMargin )
				{
					this.Handle(0).Type = HandleType.Ending;
				}
				else
				{
					this.Handle(0).Type = HandleType.Starting;
				}
			}

			this.TempMove(pos, iconContext);

			if ( this.mouseDown )
			{
				this.Handle(rank).Position = pos;
				this.dirtyBbox = true;
			}
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			int rank = this.TotalHandle-1;
			this.Handle(rank).Position = pos;
			iconContext.ConstrainFixStarting(pos);
			this.mouseDown = false;
			this.TempCreate(pos, iconContext);
		}

		// Indique si la création de l'objet est terminée.
		public override bool CreateIsEnding(IconContext iconContext)
		{
			if ( this.TotalHandle < 2 )  return false;

			int rank = this.TotalHandle-1;
			double len = Drawing.Point.Distance(this.Handle(0).Position, this.Handle(rank).Position);
			if ( len > this.closeMargin )  return false;  // pas fini

			this.HandleDelete(rank);
			this.PropertyBool(3).Bool = true;

			this.TempDelete();
			this.Handle(0).Type = HandleType.Primary;
			this.Deselect();
			iconContext.ConstrainDelStarting();
			return true;
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(IconContext iconContext)
		{
			return ( this.TotalHandle >= 2 );
		}

		// Termine la création de l'objet. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateEnding(IconContext iconContext)
		{
			if ( this.TotalHandle < 2 )  return false;

			this.TempDelete();
			this.Handle(0).Type = HandleType.Primary;
			this.Deselect();
			iconContext.ConstrainDelStarting();
			return true;
		}


		// Crée l'objet temporaire pour montrer le nouveau segment.
		protected void TempCreate(Drawing.Point pos, IconContext iconContext)
		{
			this.TempDelete();

			this.tempLine = new ObjectLine();
			this.tempLine.CreateProperties();
			this.tempLine.CloneProperties(this);

			AbstractProperty ap = this.tempLine.GetProperty(PropertyType.LineColor);
			PropertyColor pc = ap as PropertyColor;
			pc.Color = Drawing.Color.FromARGB(0.2, pc.Color.R, pc.Color.G, pc.Color.B);
			this.tempLine.SetProperty(pc);

			ap = this.tempLine.GetProperty(PropertyType.LineMode);
			PropertyLine pl = ap as PropertyLine;
			if ( pl.Width == 0 )  pl.Width = 1.0*this.scaleX;
			this.tempLine.SetProperty(pl);

			this.tempLine.CreateMouseDown(pos, iconContext);
		}

		// Déplace l'objet temporaire pour montrer le nouveau segment.
		protected void TempMove(Drawing.Point pos, IconContext iconContext)
		{
			if ( this.tempLine != null )
			{
				this.tempLine.CreateMouseMove(pos, iconContext);
			}
		}

		// Détruit l'objet temporaire pour montrer le nouveau segment.
		protected void TempDelete()
		{
			this.tempLine = null;
		}

		
		// Met à jour le rectangle englobant l'objet.
		public override void UpdateBoundingBox()
		{
			Drawing.Path path = this.PathBuild();
			this.bbox = path.ComputeBounds();
			double width = this.PropertyLine(0).Width/2.0;
			this.bbox.Inflate(width, width);
		}

		// Crée le chemin de l'objet.
		protected Drawing.Path PathBuild()
		{
			Drawing.Path path = new Drawing.Path();

			PropertyCorner corner = this.PropertyCorner(4);
			if ( corner.CornerType == CornerType.Right )
			{
				int total = this.TotalHandle;
				for ( int i=0 ; i<total ; i++ )
				{
					if ( i == 0 )  path.MoveTo(this.Handle(i).Position);
					else           path.LineTo(this.Handle(i).Position);
				}
				if ( this.PropertyBool(3).Bool && total > 2 )
				{
					path.LineTo(this.Handle(0).Position);
					path.Close();
				}
			}
			else
			{
				int total = this.TotalHandle;
				for ( int i=0 ; i<total ; i++ )
				{
					int prev = i-1;  if ( prev < 0 )  prev = total-1;
					int next = i+1;  if ( next >= total )  next = 0;
					Drawing.Point p1 = this.Handle(prev).Position;
					Drawing.Point s  = this.Handle(i).Position;
					Drawing.Point p2 = this.Handle(next).Position;
					bool simply = ( this.Handle(i).ConstrainType == HandleConstrainType.Simply );
					this.PathCorner(path, p1,s,p2, corner, simply);
				}
				if ( this.PropertyBool(3).Bool && total > 2 )
				{
					path.Close();
				}
			}

			return path;
		}

		// Crée le chemin d'un coin.
		protected void PathCorner(Drawing.Path path, Drawing.Point p1, Drawing.Point s, Drawing.Point p2, PropertyCorner corner, bool simply)
		{
			if ( simply )
			{
				if ( path.IsEmpty )  path.MoveTo(s);
				else                 path.LineTo(s);
			}
			else
			{
				double l1 = Drawing.Point.Distance(p1, s);
				double l2 = Drawing.Point.Distance(p2, s);
				double radius = System.Math.Min(corner.Radius, System.Math.Min(l1,l2)/2);
				Drawing.Point c1 = Drawing.Point.Move(s, p1, radius);
				Drawing.Point c2 = Drawing.Point.Move(s, p2, radius);
				if ( path.IsEmpty )  path.MoveTo(c1);
				else                 path.LineTo(c1);
				corner.PathCorner(path, c1,s,c2, radius);
			}
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			base.DrawGeometry(graphics, iconContext);

			if ( this.TotalHandle < 1 )  return;

			Drawing.Path path = this.PathBuild();
			this.PropertyGradient(2).Render(graphics, iconContext, path, this.BoundingBox);

			graphics.Rasterizer.AddOutline(path, this.PropertyLine(0).Width, this.PropertyLine(0).Cap, this.PropertyLine(0).Join);
			graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(1).Color));

			if ( this.IsHilite && iconContext.IsEditable )
			{
				if ( this.PropertyGradient(2).IsVisible() )
				{
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(iconContext.HiliteSurfaceColor);
				}

				graphics.Rasterizer.AddOutline(path, this.PropertyLine(0).Width+iconContext.HiliteSize, this.PropertyLine(0).Cap, this.PropertyLine(0).Join);
				graphics.RenderSolid(iconContext.HiliteOutlineColor);
			}

			if ( this.tempLine != null )
			{
				this.tempLine.DrawGeometry(graphics, iconContext);
			}
		}


		protected bool				mouseDown = false;
		protected ObjectLine		tempLine;
	}
}
