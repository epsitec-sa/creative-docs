using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe Selector permet de modifier une sélection d'objets.
	/// </summary>
	public class Selector
	{
		public Selector(Document document)
		{
			this.document = document;

			this.data = new SelectorData();
			this.data.Visible = false;
			this.data.Handles = false;

			this.h1     = new Objects.Handle(this.document);
			this.h2     = new Objects.Handle(this.document);
			this.h3     = new Objects.Handle(this.document);
			this.h4     = new Objects.Handle(this.document);
			this.center = new Objects.Handle(this.document);
			this.rotate = new Objects.Handle(this.document);

			this.center.Type = Objects.HandleType.Center;
			this.rotate.Type = Objects.HandleType.Rotate;
		}

		public SelectorData Data
		{
			get { return this.data; }
			set { this.data = value; }
		}

		public SelectorData CloneData()
		{
			SelectorData copy = new SelectorData();
			this.data.CopyTo(copy);
			return copy;
		}

		public bool Visible
		{
			get
			{
				return this.data.Visible;
			}

			set
			{
				if ( this.data.Visible != value )
				{
					this.OpletQueueInsert();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
					this.data.Visible = value;
					this.UpdateHandleVisible();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
				}
			}
		}

		public bool Handles
		{
			get
			{
				return this.data.Handles;
			}

			set
			{
				if ( this.data.Handles != value )
				{
					this.OpletQueueInsert();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
					this.data.Handles = value;
					this.UpdateHandleVisible();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
					this.document.Notifier.NotifySelectionChanged();
				}
			}
		}

		// Fixe le départ pour un rectangle de sélection simple (sans poignées).
		public void FixStarting(Point pos)
		{
			this.OpletQueueInsert();
			this.data.P1 = pos;
			this.data.P2 = pos;
			this.data.Center = pos;
			this.data.Angle = 0.0;
			this.UpdateHandlePos();
			this.Visible = true;
			this.Handles = false;
		}

		// Fixe l'arrivée pour un rectangle de sélection simple (sans poignées).
		public void FixEnding(Point pos)
		{
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.data.P2 = pos;
			this.data.Center = (this.data.P1+this.data.P2)/2;
			this.data.Angle = 0.0;
			this.UpdateHandlePos();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// Initialise le sélectionneur complexe (avec poignées).
		public void Initialize(Rectangle rect)
		{
			this.OpletQueueInsert();
			this.data.P1 = rect.BottomLeft;
			this.data.P2 = rect.TopRight;
			this.data.Center = rect.Center;
			this.data.Angle = 0.0;
			this.UpdateHandlePos();
			this.Visible = true;
			this.Handles = true;
		}

		// Retourne une position d'une poignée.
		public Point Position(int rank)
		{
			if ( rank == 0 )  return this.h1.Position;
			if ( rank == 1 )  return this.h1.Position;
			if ( rank == 2 )  return this.h2.Position;
			if ( rank == 3 )  return this.h3.Position;
			if ( rank == 4 )  return this.h4.Position;
			if ( rank == 5 )  return this.center.Position;
			if ( rank == 6 )  return this.rotate.Position;
			return new Point(0, 0);
		}

		// Détecte si la souris est dans le modificateur.
		public bool Detect(Point mouse, bool global, out int rank)
		{
			if ( this.data.Visible )
			{
				if ( this.rotate.Detect(mouse) )  { rank = 6;  return true; }
				if ( this.center.Detect(mouse) )  { rank = 5;  return true; }

				if ( this.h4.Detect(mouse) )  { rank = 4;  return true; }
				if ( this.h3.Detect(mouse) )  { rank = 3;  return true; }
				if ( this.h2.Detect(mouse) )  { rank = 2;  return true; }
				if ( this.h1.Detect(mouse) )  { rank = 1;  return true; }

				if ( global )
				{
					Rectangle rect = new Rectangle(h1.Position, h2.Position);
					if ( rect.Contains(mouse) )  { rank = 0;  return true; }
				}
			}
			rank = -1;
			return false;
		}

		// Met en évidence la poignée survollée.
		public void HiliteHandle(int rank)
		{
			this.h1.IsHilited     = (rank==1);
			this.h2.IsHilited     = (rank==2);
			this.h3.IsHilited     = (rank==3);
			this.h4.IsHilited     = (rank==4);
			this.center.IsHilited = (rank==5);
			this.rotate.IsHilited = (rank==6);
		}

		// Une poignée du modificateur sera déplacée.
		public void MoveStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			this.OpletQueueInsert();

			if ( rank == 0 )  // global ?
			{
				drawingContext.ConstrainFixStarting(pos);
				drawingContext.ConstrainFixType(ConstrainType.Normal);
			}
			else if ( rank == 5 )  // center ?
			{
				drawingContext.ConstrainFixStarting(this.Position(5));
				drawingContext.ConstrainFixType(ConstrainType.Line);
			}
			else if ( rank == 6 )  // rotate ?
			{
				drawingContext.ConstrainFixStarting(this.Position(5));
				drawingContext.ConstrainFixType(ConstrainType.Rotate);
			}
			else
			{
				Point origin = new Point(0, 0);
				if ( rank == 1 )  origin = this.data.P2;  // inf/gauche ?
				if ( rank == 2 )  origin = this.data.P1;  // sup/droite ?
				if ( rank == 3 )  origin = this.data.P4;  // sup/gauche ?
				if ( rank == 4 )  origin = this.data.P3;  // inf/droite ?
				drawingContext.ConstrainFixStarting(origin, this.Position(rank));
				drawingContext.ConstrainFixType(ConstrainType.Scale);
			}

			this.moveStart = pos;
			this.moveOffset = pos-this.Position(rank);
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// Déplace une poignée du modificateur.
		public void MoveProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);

			if ( rank != 6 )  // pas rotate ?
			{
				drawingContext.SnapGrid(this.moveStart, ref pos);
			}

			if ( rank == 0 )  // global ?
			{
				drawingContext.ConstrainSnapPos(ref pos);
				pos -= this.moveOffset;
			}
			else
			{
				pos -= this.moveOffset;
				drawingContext.ConstrainSnapPos(ref pos);
			}

			if ( rank == 0 )  // tout ?
			{
				Point dim = this.data.P2-this.data.P1;
				this.data.P1 = pos;
				this.data.P2 = pos+dim;
			}

			if ( rank == 1 )  // inf/gauche ?
			{
				this.data.P1 = pos;
			}

			if ( rank == 2 )  // sup/droite ?
			{
				this.data.P2 = pos;
			}

			if ( rank == 3 )  // sup/gauche ?
			{
				this.data.P3 = pos;
			}

			if ( rank == 4 )  // inf/droite ?
			{
				this.data.P4 = pos;
			}

			if ( rank == 5 )  // center ?
			{
				this.data.Center = pos;
			}

			if ( rank == 6 )  // rotate ?
			{
				this.data.Angle = Point.ComputeAngleDeg(this.data.Center, pos)-90;
			}

			this.UpdateHandlePos();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}


		// Effectue une opération de miroir.
		public void OperMirror(bool horizontal)
		{
			this.OpletQueueInsert();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Modifier.ActiveViewer.MoveGlobalStarting();
			SelectorData initial = this.CloneData();
			Point p3 = this.data.P3;
			Point p4 = this.data.P4;
			this.data.P1 = horizontal ? p4 : p3;
			this.data.P2 = horizontal ? p3 : p4;
			this.document.Modifier.ActiveViewer.MoveGlobalProcess(initial, this.Data);
			this.UpdateHandlePos();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// Effectue une opération de déplacement.
		public void OperMove(Point move)
		{
			this.OpletQueueInsert();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Modifier.ActiveViewer.MoveGlobalStarting();
			SelectorData initial = this.CloneData();
			this.data.P1 += move;
			this.data.P2 += move;
			this.document.Modifier.ActiveViewer.MoveGlobalProcess(initial, this.Data);
			this.UpdateHandlePos();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// Effectue une opération d'agrandissement.
		public void OperZoom(double zoom)
		{
			this.OpletQueueInsert();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Modifier.ActiveViewer.MoveGlobalStarting();
			SelectorData initial = this.CloneData();
			Point center = this.data.Center;
			this.data.P1 = Point.Scale(center, this.data.P1, zoom);
			this.data.P2 = Point.Scale(center, this.data.P2, zoom);
			this.document.Modifier.ActiveViewer.MoveGlobalProcess(initial, this.Data);
			this.UpdateHandlePos();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// Effectue une opération de rotation.
		public void OperRotate(double angle)
		{
			this.OpletQueueInsert();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Modifier.ActiveViewer.MoveGlobalStarting();
			SelectorData initial = this.CloneData();
			this.data.Angle += angle;
			this.document.Modifier.ActiveViewer.MoveGlobalProcess(initial, this.Data);
			this.UpdateHandlePos();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}


		// Retourne la bbox du modificateur.
		public Rectangle BoundingBox
		{
			get
			{
				if ( !this.data.Visible )  return Rectangle.Empty;

				Rectangle bbox = this.data.Rectangle;

				if ( this.data.Handles )
				{
					Rectangle circle = new Rectangle(this.data.Center, this.data.Center);
					circle.Inflate(this.data.Radius);
					bbox.MergeWith(circle);
				}

				return bbox;
			}
		}

		// Met à jour l'état des poignées en fonction des données.
		protected void UpdateHandleVisible()
		{
			bool visible = this.Visible && this.Handles;
			this.h1.IsVisible     = visible;
			this.h2.IsVisible     = visible;
			this.h3.IsVisible     = visible;
			this.h4.IsVisible     = visible;
			this.center.IsVisible = visible;
			this.rotate.IsVisible = visible;
		}

		// Met à jour la position des poignées en fonction des données.
		protected void UpdateHandlePos()
		{
			this.h1.Position = this.data.P1;
			this.h2.Position = this.data.P2;
			this.h3.Position = this.data.P3;
			this.h4.Position = this.data.P4;

			this.center.Position = this.data.Center;

			double radius = this.data.Radius;
			double angle  = this.data.Angle;
			this.rotate.Position = this.data.Center + Transform.RotatePointDeg(angle, new Point(0, radius));
		}

		// Dessine le modificateur.
		public void Draw(Graphics graphics, DrawingContext drawingContext)
		{
			if ( !this.data.Visible )  return;

			Rectangle rect = this.data.Rectangle;
			graphics.LineWidth = 1.0/drawingContext.ScaleX;
			rect.Deflate(0.5/drawingContext.ScaleX, 0.5/drawingContext.ScaleY);

			if ( !rect.IsSurfaceZero )
			{
				if ( this.data.Handles )
				{
					Color filledColor = drawingContext.HiliteOutlineColor;
					filledColor.A *= 0.2;
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(filledColor);
				}

				graphics.AddRectangle(rect);
				graphics.RenderSolid(drawingContext.HiliteOutlineColor);
			}

			if ( this.data.Handles )
			{
				Point p1 = this.data.Center;
				Point p2 = this.rotate.Position;
				this.PaintCircle(graphics, p1, this.data.Radius, this.data.Radius, drawingContext.HiliteOutlineColor);
				graphics.AddLine(p1, p2);
				graphics.AddLine(p2, this.ComputeExtremity(p1, p2, 0.4, 0.2, 0));
				graphics.AddLine(p2, this.ComputeExtremity(p1, p2, 0.4, 0.2, 1));  // flèche
				graphics.RenderSolid(drawingContext.HiliteOutlineColor);

				h1.Draw(graphics, drawingContext);
				h2.Draw(graphics, drawingContext);
				h3.Draw(graphics, drawingContext);
				h4.Draw(graphics, drawingContext);
				center.Draw(graphics, drawingContext);
				rotate.Draw(graphics, drawingContext);
			}
		}

		// Dessine un cercle complet.
		protected void PaintCircle(Graphics graphics,
								   Point c, double rx, double ry,
								   Color color)
		{
			Path path = new Path();
			path.MoveTo(c.X-rx, c.Y);
			path.CurveTo(c.X-rx, c.Y+ry*0.56, c.X-rx*0.56, c.Y+ry, c.X, c.Y+ry);
			path.CurveTo(c.X+rx*0.56, c.Y+ry, c.X+rx, c.Y+ry*0.56, c.X+rx, c.Y);
			path.CurveTo(c.X+rx, c.Y-ry*0.56, c.X+rx*0.56, c.Y-ry, c.X, c.Y-ry);
			path.CurveTo(c.X-rx*0.56, c.Y-ry, c.X-rx, c.Y-ry*0.56, c.X-rx, c.Y);
			path.Close();
			graphics.Rasterizer.AddOutline(path, graphics.LineWidth);
			graphics.RenderSolid(color);
		}

		// Calcule l'extrémité gauche ou droite de la flèche.
		protected Point ComputeExtremity(Point p1, Point p2, double para, double perp, int rank)
		{
			double distPara = Point.Distance(p1, p2)*para;
			double distPerp = Point.Distance(p1, p2)*perp;
			Point c = Point.Move(p2, p1, distPara);
			Point p = Point.Move(c, Point.Symmetry(p2, p1), distPerp);
			double angle = (rank==0) ? 90 : -90;
			return Transform.RotatePointDeg(c, angle, p);
		}


		#region OpletSelector
		protected void OpletQueueInsert()
		{
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletSelector oplet = new OpletSelector(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		protected class OpletSelector : AbstractOplet
		{
			public OpletSelector(Selector host)
			{
				this.host = host;
				this.data = new SelectorData();
				this.host.data.CopyTo(this.data);
			}

			protected void Swap()
			{
				this.host.document.Notifier.NotifyArea(this.host.BoundingBox);
				this.host.data.Swap(this.data);
				this.host.UpdateHandleVisible();
				this.host.UpdateHandlePos();
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

			protected Selector				host;
			protected SelectorData			data;
		}
		#endregion

		protected Document			document;
		protected SelectorData		data;
		protected Objects.Handle	h1;
		protected Objects.Handle	h2;
		protected Objects.Handle	h3;
		protected Objects.Handle	h4;
		protected Objects.Handle	center;
		protected Objects.Handle	rotate;
		protected Point				moveStart;
		protected Point				moveOffset;
	}


	/// <summary>
	/// La classe SelectorData contient les définitions du modificateur global.
	/// </summary>
	public class SelectorData
	{
		public SelectorData()
		{
			this.center.X = 0.5;
			this.center.Y = 0.5;
			this.angle = 0.0;
		}

		public bool Visible
		{
			get { return this.visible; }
			set { this.visible = value; }
		}

		public bool Handles
		{
			get { return this.handles; }
			set { this.handles = value; }
		}

		public Rectangle Rectangle
		{
			get
			{
				return new Rectangle(this.p1, this.p2);
			}
		}

		public Point P1
		{
			get
			{
				return new Point(p1.X, p1.Y);
			}

			set
			{
				this.p1.X = value.X;
				this.p1.Y = value.Y;
			}
		}

		public Point P2
		{
			get
			{
				return new Point(p2.X, p2.Y);
			}

			set
			{
				this.p2.X = value.X;
				this.p2.Y = value.Y;
			}
		}

		public Point P3
		{
			get
			{
				return new Point(p1.X, p2.Y);
			}

			set
			{
				this.p1.X = value.X;
				this.p2.Y = value.Y;
			}
		}

		public Point P4
		{
			get
			{
				return new Point(p2.X, p1.Y);
			}

			set
			{
				this.p2.X = value.X;
				this.p1.Y = value.Y;
			}
		}

		public Point Center
		{
			get
			{
				return this.p1+Point.ScaleMul(this.p2-this.p1, this.center);
			}

			set
			{
				//this.center = Point.ScaleDiv(value-this.p1, this.p2-this.p1);
				Point a = value-this.p1;
				Point b = this.p2-this.p1;

				if ( b.X == 0 )  this.center.X = this.p1.X;
				else             this.center.X = a.X / b.X;

				if ( b.Y == 0 )  this.center.Y = this.p1.Y;
				else             this.center.Y = a.Y / b.Y;
			}
		}

		public double Angle
		{
			get { return this.angle; }
			set { this.angle = value; }
		}

		public double Radius
		{
			get
			{
				double width = System.Math.Abs(this.p1.X-this.p2.X);
				double height = System.Math.Abs(this.p1.Y-this.p2.Y);
				//return System.Math.Max(width, height)/2.0;
				//return (width+height)/4.0;
				return System.Math.Min(width, height)*0.4;
			}
		}

		public void CopyTo(SelectorData dest)
		{
			dest.visible = this.visible;
			dest.handles = this.handles;
			dest.p1      = this.p1;
			dest.p2      = this.p2;
			dest.center  = this.center;
			dest.angle   = this.angle;
		}

		public void Swap(SelectorData dest)
		{
			Misc.Swap(ref dest.visible, ref this.visible);
			Misc.Swap(ref dest.handles, ref this.handles);
			Misc.Swap(ref dest.p1,      ref this.p1);
			Misc.Swap(ref dest.p2,      ref this.p2);
			Misc.Swap(ref dest.center,  ref this.center);
			Misc.Swap(ref dest.angle,   ref this.angle);
		}

		// Transforme un point.
		static public Point DotTransform(SelectorData initial, SelectorData final, Point pos)
		{
			Point f = Point.ScaleDiv(pos-initial.P1, initial.P2-initial.P1);
			pos = Point.ScaleMul(f, final.P2-final.P1)+final.P1;

			double rot = final.Angle-initial.Angle;
			if ( rot != 0 )
			{
				pos = Transform.RotatePointDeg(final.Center, rot, pos);
			}

			return pos;
		}

#if false
		// Déforme un point dans un quadrillatère quelconque.
		// s0                       d0         d1
		//  o----------o             o---------o
		//  |          |     ---\    |          \
		//  |          |     ---/    |           \
		//  o----------o             o------------o
		//             s2           d3            d2
		static protected Point Deform(Point s0, Point s2,
									  Point d0, Point d1, Point d2, Point d3,
									  Point p)
		{
			Point	q = new Point();
			Point	pp = new Point();

			q.X = p.X - s0.X;
			q.Y = p.Y - s0.Y;

			pp.X  = d0.X;
			pp.X += (d3.X-d0.X)/(s2.Y-s0.Y)*q.Y;
			pp.X += (d1.X-d0.X)/(s2.X-s0.X)*q.X;
			pp.X += (d2.X-d3.X-d1.X+d0.X)*q.X*q.Y/(s2.X-s0.X)/(s2.Y-s0.Y);

			pp.Y  = d0.Y;
			pp.Y += (d3.Y-d0.Y)/(s2.Y-s0.Y)*q.Y;
			pp.Y += (d1.Y-d0.Y)/(s2.X-s0.X)*q.X;
			pp.Y += (d2.Y-d3.Y-d1.Y+d0.Y)*q.X*q.Y/(s2.X-s0.X)/(s2.Y-s0.Y);

			return pp;
		}
#endif


		protected bool					visible;
		protected bool					handles;
		protected Point					p1;			// un coin quelconque
		protected Point					p2;			// le coin opposé
		protected Point					center;		// [0..1]
		protected double				angle;		// en degrés
	}
}
