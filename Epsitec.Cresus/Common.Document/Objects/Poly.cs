using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Poly est la classe de l'objet graphique "polygone".
	/// </summary>
	[System.Serializable()]
	public class Poly : Objects.Abstract
	{
		public Poly(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.Name )  return true;
			if ( type == Properties.Type.LineMode )  return true;
			if ( type == Properties.Type.LineColor )  return true;
			if ( type == Properties.Type.Arrow )  return true;
			if ( type == Properties.Type.FillGradient )  return true;
			if ( type == Properties.Type.PolyClose )  return true;
			if ( type == Properties.Type.Corner )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Poly(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/poly.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
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

			this.Handle(rank).Position = pos;

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


		// Donne le contenu du menu contextuel.
		public override void ContextMenu(System.Collections.ArrayList list, Point pos, int handleRank)
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
				item.Icon = @"file:images/add.icon";
				item.Text = "Ajouter un point";
				list.Add(item);
			}
			else
			{
				if ( this.TotalMainHandle > 2 )
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
					item.Text = "Coin quelconque";
					list.Add(item);

					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "HandleSimply";
					item.IconActiveNo = @"file:images/activeno.icon";
					item.IconActiveYes = @"file:images/activeyes.icon";
					item.Active = ( type == HandleConstrainType.Simply );
					item.Text = "Coin toujours droit";
					list.Add(item);

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

		// Exécute une commande du menu contextuel.
		public override void ContextCommand(string cmd, Point pos, int handleRank)
		{
			if ( cmd == "HandleAdd" )
			{
				int rank = this.DetectOutline(pos);
				if ( rank == -1 )  return;

				int next = rank+1;
				if ( next >= this.TotalMainHandle )  next = 0;
				Point p = Point.Projection(this.Handle(rank).Position, this.Handle(next).Position, pos);

				Handle handle = new Handle(this.document);
				handle.Position = p;
				handle.Type = HandleType.Primary;
				handle.IsVisible = true;
				this.HandleInsert(rank+1, handle);
				this.HandlePropertiesUpdateVisible();
				this.HandlePropertiesUpdatePosition();
			}

			if ( cmd == "HandleDelete" )
			{
				this.HandleDelete(handleRank);
				this.HandlePropertiesUpdateVisible();
				this.HandlePropertiesUpdatePosition();
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

		// Détecte si la souris est sur le pourtour de l'objet.
		// Retourne le rank de la poignée de départ, ou -1
		protected int DetectOutline(Point pos)
		{
			this.document.Modifier.OpletQueueEnable = false;
			Properties.Corner corner = this.PropertyCorner;
			Properties.CornerType type = corner.CornerType;
			corner.CornerType = Properties.CornerType.Right;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			corner.CornerType = type;
			this.document.Modifier.OpletQueueEnable = true;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			double width = System.Math.Max(this.PropertyLineMode.Width/2, context.MinimalWidth);
			return Geometry.DetectOutlineRank(pathLine, width, pos);
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			drawingContext.ConstrainSnapPos(ref pos);
			drawingContext.ConstrainFixType(ConstrainType.Normal);
			drawingContext.SnapGrid(ref pos);

			if ( this.TotalHandle == 0 )
			{
				this.ChangePropertyPolyClose(false);
				this.HandleAdd(pos, HandleType.Starting);
				this.Handle(0).IsVisible = true;
			}
			else
			{
				this.HandleAdd(pos, HandleType.Primary);
				this.Handle(this.TotalHandle-1).IsVisible = true;
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

			this.isCreating = true;

			int rank = this.TotalHandle-1;
			if ( rank > 0 )
			{
				double len = Point.Distance(this.Handle(0).Position, pos);
				if ( len <= drawingContext.CloseMargin )
				{
					this.Handle(0).Type = HandleType.Ending;
				}
				else
				{
					this.Handle(0).Type = HandleType.Starting;
				}
			}

			this.TempMove(pos, drawingContext);

			if ( this.mouseDown )
			{
				this.Handle(rank).Position = pos;
			}
			this.dirtyBbox = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapGrid(ref pos);
			drawingContext.ConstrainSnapPos(ref pos);
			int rank = this.TotalHandle-1;
			this.Handle(rank).Position = pos;
			drawingContext.ConstrainDelStarting();
			drawingContext.ConstrainFixStarting(pos);
			this.mouseDown = false;
			this.TempCreate(pos, drawingContext);
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Indique si la création de l'objet est terminée.
		public override bool CreateIsEnding(DrawingContext drawingContext)
		{
			if ( this.TotalHandle < 2 )  return false;

			int rank = this.TotalHandle-1;
			double len = Point.Distance(this.Handle(0).Position, this.Handle(rank).Position);
			if ( len > drawingContext.CloseMargin )  return false;  // pas fini

			this.isCreating = false;
			this.HandleDelete(rank);
			this.ChangePropertyPolyClose(true);

			this.TempDelete();
			this.Handle(0).Type = HandleType.Primary;
			this.Deselect();
			drawingContext.ConstrainDelStarting();

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdatePosition();
			return true;
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			return ( this.TotalHandle >= 2 );
		}

		// Termine la création de l'objet. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateEnding(DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			this.isCreating = false;

			if ( this.TotalHandle < 2 )  return false;

			this.TempDelete();
			this.Handle(0).Type = HandleType.Primary;
			this.Deselect();
			drawingContext.ConstrainDelStarting();

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

		// Crée l'objet temporaire pour montrer le nouveau segment.
		protected void TempCreate(Point pos, DrawingContext drawingContext)
		{
			this.document.Modifier.OpletQueueEnable = false;

			if ( this.tempLine == null )
			{
				this.tempLine = new Objects.Line(this.document, this, true);
			}

			Properties.Gradient pg = this.tempLine.PropertyLineColor;
			pg.Color1 = Color.FromARGB(0.2, pg.Color1.R, pg.Color1.G, pg.Color1.B);
			pg.Color2 = Color.FromARGB(0.2, pg.Color2.R, pg.Color2.G, pg.Color2.B);

			Properties.Line pl = this.tempLine.PropertyLineMode;
			if ( pl.Width == 0 )  pl.Width = 1.0/drawingContext.ScaleX;

			Properties.Arrow pa = this.tempLine.PropertyArrow;
			pa.ArrowType1 = Properties.ArrowType.Right;
			pa.ArrowType2 = Properties.ArrowType.Right;

			this.document.Modifier.OpletQueueEnable = true;

			this.tempLine.CreateMouseDown(pos, drawingContext);
		}

		// Déplace l'objet temporaire pour montrer le nouveau segment.
		protected void TempMove(Point pos, DrawingContext drawingContext)
		{
			if ( this.tempLine != null )
			{
				this.tempLine.CreateMouseMove(pos, drawingContext);
			}
		}

		// Détruit l'objet temporaire pour montrer le nouveau segment.
		protected void TempDelete()
		{
			if ( this.tempLine != null )  this.tempLine.Dispose();
			this.tempLine = null;
		}

		
		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			Path pathTemp = null;
			if ( this.tempLine != null )
			{
				pathTemp = new Path();
				pathTemp.MoveTo(this.tempLine.Handle(0).Position);
				pathTemp.LineTo(this.tempLine.Handle(1).Position);
			}

			Path[] paths = new Path[4];
			paths[0] = pathLine;
			paths[1] = pathStart;
			paths[2] = pathEnd;
			paths[3] = pathTemp;

			bool[] lineModes = new bool[4];
			lineModes[0] = true;
			lineModes[1] = outlineStart;
			lineModes[2] = outlineEnd;
			lineModes[3] = true;

			bool[] lineColors = new bool[4];
			lineColors[0] = true;
			lineColors[1] = surfaceStart;
			lineColors[2] = surfaceEnd;
			lineColors[3] = true;

			bool[] fillGradients = new bool[4];
			fillGradients[0] = true;
			fillGradients[1] = false;
			fillGradients[2] = false;
			fillGradients[3] = false;

			this.ComputeBoundingBox(paths, lineModes, lineColors, fillGradients);

			for ( int i=0 ; i<this.TotalMainHandle ; i++ )
			{
				this.InflateBoundingBox(this.Handle(i).Position, true);
			}
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

			double zoom = Properties.Abstract.DefaultZoom(drawingContext);
			pathStart.DefaultZoom = zoom;
			pathEnd.DefaultZoom = zoom;
			pathLine.DefaultZoom = zoom;

			int total = this.TotalMainHandle;
			if ( total < 2 )
			{
				outlineStart = false;
				surfaceStart = false;
				outlineEnd   = false;
				surfaceEnd   = false;
				return;
			}

			Point p1, p2, pp1, pp2, s;
			double w = this.PropertyLineMode.Width;
			CapStyle cap = this.PropertyLineMode.Cap;
			p1 = this.Handle(0).Position;
			p2 = this.Handle(1).Position;
			pp1 = this.PropertyArrow.PathExtremity(pathStart, 0, w,cap, p1,p2, out outlineStart, out surfaceStart);
			p1 = this.Handle(total-1).Position;
			p2 = this.Handle(total-2).Position;
			pp2 = this.PropertyArrow.PathExtremity(pathEnd,   1, w,cap, p1,p2, out outlineEnd,   out surfaceEnd);

			bool close = ( this.PropertyPolyClose.BoolValue && total > 2 );
			Properties.Corner corner = this.PropertyCorner;
			if ( corner.CornerType == Properties.CornerType.Right )  // coins droits ?
			{
				for ( int i=0 ; i<total ; i++ )
				{
					p1 = this.Handle(i).Position;

					if ( i == 0 )  // premier point ?
					{
						pathLine.MoveTo(pp1);
					}
					else if ( i < total-1 )  // point intermédiaire ?
					{
						pathLine.LineTo(p1);
					}
					else	// dernier point ?
					{
						pathLine.LineTo(pp2);
					}
				}
				if ( close )
				{
					pathLine.LineTo(pp1);
					pathLine.Close();
				}
			}
			else	// coins spéciaux ?
			{
				for ( int i=0 ; i<total ; i++ )
				{
					p1 = this.Handle(i).Position;
					int prev = i-1;  if ( prev < 0 )  prev = total-1;
					int next = i+1;  if ( next >= total )  next = 0;
					bool simply = ( this.Handle(i).ConstrainType == HandleConstrainType.Simply );

					if ( i == 0 )  // premier point ?
					{
						if ( outlineStart || surfaceStart || !close )
						{
							pathLine.MoveTo(pp1);
						}
						else
						{
							p1 = this.Handle(prev).Position;
							s  = this.Handle(i).Position;
							p2 = this.Handle(next).Position;
							this.PathCorner(pathLine, p1,s,p2, corner, simply);
						}
					}
					else if ( i < total-1 )  // point intermédiaire ?
					{
						p1 = this.Handle(prev).Position;
						s  = this.Handle(i).Position;
						p2 = this.Handle(next).Position;
						this.PathCorner(pathLine, p1,s,p2, corner, simply);
					}
					else	// dernier point ?
					{
						if ( outlineEnd || surfaceEnd || !close )
						{
							pathLine.LineTo(pp2);
						}
						else
						{
							p1 = this.Handle(prev).Position;
							s  = this.Handle(i).Position;
							p2 = this.Handle(next).Position;
							this.PathCorner(pathLine, p1,s,p2, corner, simply);
						}
					}
				}
				if ( close )
				{
					pathLine.Close();
				}
			}
		}

		// Crée le chemin d'un coin.
		protected void PathCorner(Path path, Point p1, Point s, Point p2, Properties.Corner corner, bool simply)
		{
			if ( simply )
			{
				if ( path.IsEmpty )  path.MoveTo(s);
				else                 path.LineTo(s);
			}
			else
			{
				double l1 = Point.Distance(p1, s);
				double l2 = Point.Distance(p2, s);
				double radius = System.Math.Min(corner.Radius, System.Math.Min(l1,l2)/2);
				Point c1 = Point.Move(s, p1, radius);
				Point c2 = Point.Move(s, p2, radius);
				if ( path.IsEmpty )  path.MoveTo(c1);
				else                 path.LineTo(c1);
				corner.PathCorner(path, c1,s,c2, radius);
			}
		}

		// Dessine l'objet.
		public override void DrawGeometry(Graphics graphics, DrawingContext drawingContext)
		{
			base.DrawGeometry(graphics, drawingContext);

			if ( this.TotalHandle < 1 )  return;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(drawingContext,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			this.PropertyFillGradient.RenderSurface(graphics, drawingContext, pathLine, this.BoundingBoxThin);

			if ( outlineStart )
			{
				this.PropertyLineMode.DrawPath(graphics, drawingContext, pathStart, this.PropertyLineColor, this.BoundingBoxGeom);
			}
			if ( surfaceStart )
			{
				this.PropertyLineColor.RenderSurface(graphics, drawingContext, pathStart, this.BoundingBoxThin);
			}

			if ( outlineEnd )
			{
				this.PropertyLineMode.DrawPath(graphics, drawingContext, pathEnd, this.PropertyLineColor, this.BoundingBoxGeom);
			}
			if ( surfaceEnd )
			{
				this.PropertyLineColor.RenderSurface(graphics, drawingContext, pathEnd, this.BoundingBoxThin);
			}

			this.PropertyLineMode.DrawPath(graphics, drawingContext, pathLine, this.PropertyLineColor, this.BoundingBoxGeom);

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

			if ( this.tempLine != null )
			{
				this.tempLine.DrawGeometry(graphics, drawingContext);
			}
		}

		// Imprime l'objet.
		public override void PrintGeometry(Printing.PrintPort port, DrawingContext drawingContext)
		{
			base.PrintGeometry(port, drawingContext);

			if ( this.TotalHandle < 1 )  return;

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
		protected Poly(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion

		
		protected bool				mouseDown = false;
		protected Objects.Line		tempLine;
	}
}
