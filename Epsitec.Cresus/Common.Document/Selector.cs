using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Document
{
	public enum SelectorType
	{
		None,
		Auto,			// mode automatique
		Individual,		// objets individuels
		Zoomer,			// zoom et rotation
		Stretcher,		// déformation
	}

	/// <summary>
	/// La classe Selector permet de modifier une sélection d'objets.
	/// </summary>
	public class Selector
	{
		public Selector(Document document)
		{
			this.document = document;

			this.initialData = new SelectorData();
			this.initialData.Visible = false;
			this.initialData.Handles = false;

			this.finalData = new SelectorData();
			this.finalData.Visible = false;
			this.finalData.Handles = false;

			this.h1     = new Objects.Handle(this.document);
			this.h2     = new Objects.Handle(this.document);
			this.h3     = new Objects.Handle(this.document);
			this.h4     = new Objects.Handle(this.document);
			this.center = new Objects.Handle(this.document);
			this.rotate = new Objects.Handle(this.document);

			this.center.Type = Objects.HandleType.Center;
			this.rotate.Type = Objects.HandleType.Rotate;
		}

		public SelectorType TypeChoice
		{
			get
			{
				return this.finalData.TypeChoice;
			}

			set
			{
				if ( this.finalData.TypeChoice != value )
				{
					this.OpletQueueInsert();
					this.finalData.TypeChoice = value;
					this.UpdateHandleVisible();
				}
			}
		}

		public SelectorType TypeInUse
		{
			get
			{
				return this.finalData.TypeInUse;
			}

			set
			{
				if ( this.finalData.TypeInUse != value )
				{
					this.OpletQueueInsert();
					this.finalData.TypeInUse = value;
					this.UpdateHandleVisible();
				}
			}
		}

		public void FinalToInitialData()
		{
			this.finalData.CopyTo(this.initialData);
		}

		public bool Visible
		{
			get
			{
				return this.finalData.Visible;
			}

			set
			{
				if ( this.finalData.Visible != value )
				{
					this.OpletQueueInsert();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
					this.finalData.Visible = value;
					this.UpdateHandleVisible();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
				}
			}
		}

		public bool Handles
		{
			get
			{
				return this.finalData.Handles;
			}

			set
			{
				if ( this.finalData.Handles != value )
				{
					this.OpletQueueInsert();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
					this.finalData.Handles = value;
					this.UpdateHandleVisible();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
					this.document.Notifier.NotifySelectionChanged();
				}
			}
		}

		public Rectangle Rectangle
		{
			get
			{
				return this.finalData.Rectangle;
			}
		}

		// Fixe le départ pour un rectangle de sélection simple (sans poignées).
		public void FixStarting(Point pos)
		{
			this.OpletQueueInsert();
			this.finalData.P1 = pos;
			this.finalData.P2 = pos;
			this.finalData.P3 = pos;
			this.finalData.P4 = pos;
			this.finalData.Center = pos;
			this.finalData.Angle = 0.0;
			this.UpdateHandlePos();
			this.Visible = true;
			this.Handles = false;
		}

		// Fixe l'arrivée pour un rectangle de sélection simple (sans poignées).
		public void FixEnding(Point pos)
		{
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.finalData.P2 = pos;
			this.finalData.P3 = new Point(this.finalData.P1.X, this.finalData.P2.Y);
			this.finalData.P4 = new Point(this.finalData.P2.X, this.finalData.P1.Y);
			this.finalData.Center = (this.finalData.P1+this.finalData.P2)/2;
			this.finalData.Angle = 0.0;
			this.UpdateHandlePos();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// Initialise le sélectionneur complexe (avec poignées).
		public void Initialize(Rectangle rect)
		{
			this.OpletQueueInsert();
			this.finalData.P1 = rect.BottomLeft;
			this.finalData.P2 = rect.TopRight;
			this.finalData.P3 = rect.TopLeft;
			this.finalData.P4 = rect.BottomRight;
			this.finalData.Center = rect.Center;
			this.finalData.Angle = 0.0;
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
			if ( this.finalData.Visible )
			{
				if ( this.finalData.TypeInUse == SelectorType.Zoomer )
				{
					if ( this.rotate.Detect(mouse) )  { rank = 6;  return true; }
					if ( this.center.Detect(mouse) )  { rank = 5;  return true; }
				}

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
				if ( rank == 1 )  origin = this.finalData.P2;  // inf/gauche ?
				if ( rank == 2 )  origin = this.finalData.P1;  // sup/droite ?
				if ( rank == 3 )  origin = this.finalData.P4;  // sup/gauche ?
				if ( rank == 4 )  origin = this.finalData.P3;  // inf/droite ?
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
				Point ip1 = this.finalData.P1;

				if ( this.finalData.TypeInUse == SelectorType.Zoomer )
				{
					Point dim = this.finalData.P2-this.finalData.P1;
					this.finalData.P1 = pos;
					this.finalData.P2 = pos+dim;
				}
				else
				{
					Point move = pos-this.finalData.P1;
					this.finalData.P1 += move;
					this.finalData.P2 += move;
					this.finalData.P3 += move;
					this.finalData.P4 += move;
				}

				this.document.Modifier.AddMoveAfterDuplicate(this.finalData.P1-ip1);
			}
			else
			{
				this.document.Modifier.FlushMoveAfterDuplicate();
			}

			if ( rank == 1 )  // inf/gauche ?
			{
				this.finalData.P1 = pos;
			}

			if ( rank == 2 )  // sup/droite ?
			{
				this.finalData.P2 = pos;
			}

			if ( rank == 3 )  // sup/gauche ?
			{
				this.finalData.P3 = pos;
			}

			if ( rank == 4 )  // inf/droite ?
			{
				this.finalData.P4 = pos;
			}

			if ( rank == 5 )  // center ?
			{
				this.finalData.Center = pos;
			}

			if ( rank == 6 )  // rotate ?
			{
				this.finalData.Angle = Point.ComputeAngleDeg(this.finalData.Center, pos)-90;
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
			Point p3 = this.finalData.P3;
			Point p4 = this.finalData.P4;
			this.finalData.P1 = horizontal ? p4 : p3;
			this.finalData.P2 = horizontal ? p3 : p4;
			this.document.Modifier.ActiveViewer.MoveGlobalProcess(this);
			this.UpdateHandlePos();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// Effectue une opération de déplacement.
		public void OperMove(Point move)
		{
			this.OpletQueueInsert();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Modifier.ActiveViewer.MoveGlobalStarting();
			this.finalData.P1 += move;
			this.finalData.P2 += move;
			this.document.Modifier.ActiveViewer.MoveGlobalProcess(this);
			this.UpdateHandlePos();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// Effectue une opération d'agrandissement.
		public void OperZoom(double zoom)
		{
			this.OpletQueueInsert();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Modifier.ActiveViewer.MoveGlobalStarting();
			Point center = this.finalData.Center;
			this.finalData.P1 = Point.Scale(center, this.finalData.P1, zoom);
			this.finalData.P2 = Point.Scale(center, this.finalData.P2, zoom);
			this.document.Modifier.ActiveViewer.MoveGlobalProcess(this);
			this.UpdateHandlePos();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// Effectue une opération de rotation.
		public void OperRotate(double angle)
		{
			this.OpletQueueInsert();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Modifier.ActiveViewer.MoveGlobalStarting();
			this.finalData.Angle += angle;
			this.document.Modifier.ActiveViewer.MoveGlobalProcess(this);
			this.UpdateHandlePos();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}


		// Retourne la bbox du modificateur.
		public Rectangle BoundingBox
		{
			get
			{
				if ( !this.finalData.Visible )  return Rectangle.Empty;

				Rectangle bbox = this.finalData.Rectangle;

				if ( this.finalData.Handles )
				{
					Rectangle circle = new Rectangle(this.finalData.Center, this.finalData.Center);
					circle.Inflate(this.finalData.Radius);
					bbox.MergeWith(circle);
				}

				return bbox;
			}
		}

		// Met à jour l'état des poignées en fonction des données.
		protected void UpdateHandleVisible()
		{
			bool visible = this.Visible && this.Handles;
			this.h1.IsVisible = visible;
			this.h2.IsVisible = visible;
			this.h3.IsVisible = visible;
			this.h4.IsVisible = visible;

			if ( this.finalData.TypeInUse == SelectorType.Zoomer )
			{
				this.h1.ConstrainType = Objects.HandleConstrainType.Simply;
				this.h2.ConstrainType = Objects.HandleConstrainType.Simply;
				this.h3.ConstrainType = Objects.HandleConstrainType.Simply;
				this.h4.ConstrainType = Objects.HandleConstrainType.Simply;
			}
			else
			{
				this.h1.ConstrainType = Objects.HandleConstrainType.Corner;
				this.h2.ConstrainType = Objects.HandleConstrainType.Corner;
				this.h3.ConstrainType = Objects.HandleConstrainType.Corner;
				this.h4.ConstrainType = Objects.HandleConstrainType.Corner;
				visible = false;
			}
			this.center.IsVisible = visible;
			this.rotate.IsVisible = visible;
		}

		// Met à jour la position des poignées en fonction des données.
		protected void UpdateHandlePos()
		{
			this.h1.Position = this.finalData.P1;
			this.h2.Position = this.finalData.P2;
			this.h3.Position = this.finalData.P3;
			this.h4.Position = this.finalData.P4;

			this.center.Position = this.finalData.Center;

			double radius = this.finalData.Radius;
			double angle  = this.finalData.Angle;
			this.rotate.Position = this.finalData.Center + Transform.RotatePointDeg(angle, new Point(0, radius));
		}

		// Dessine le modificateur.
		public void Draw(Graphics graphics, DrawingContext drawingContext)
		{
			if ( !this.finalData.Visible )  return;

			Point p1 = this.finalData.P1;
			Point p2 = this.finalData.P2;
			Point p3 = this.finalData.P3;
			Point p4 = this.finalData.P4;

			if ( this.finalData.TypeInUse == SelectorType.Zoomer )
			{
				Point adj = new Point(0.5/drawingContext.ScaleX, 0.5/drawingContext.ScaleY);
				p1 += adj;
				p2 += adj;
				p3 += adj;
				p4 += adj;
			}

			Path path = new Path();
			path.MoveTo(p1);
			path.LineTo(p3);
			path.LineTo(p2);
			path.LineTo(p4);
			path.Close();

			if ( this.finalData.Handles && !drawingContext.PreviewActive )
			{
				Color filledColor = drawingContext.HiliteOutlineColor;
				filledColor.A *= 0.2;
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(filledColor);
			}

			graphics.Rasterizer.AddOutline(path, 1.0/drawingContext.ScaleX);
			graphics.RenderSolid(drawingContext.HiliteOutlineColor);

			if ( this.finalData.Handles )
			{
				if ( this.finalData.TypeInUse == SelectorType.Zoomer )
				{
					Point c = this.finalData.Center;
					Point p = this.rotate.Position;
					graphics.LineWidth = 1.0/drawingContext.ScaleX;
					graphics.AddCircle(c, this.finalData.Radius);
					graphics.AddLine(c, p);
					graphics.AddLine(p, this.ComputeExtremity(c,p, 0.4, 0.2, 0));
					graphics.AddLine(p, this.ComputeExtremity(c,p, 0.4, 0.2, 1));  // flèche
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}

				if ( this.finalData.TypeInUse == SelectorType.Stretcher )
				{
					graphics.LineWidth = 1.0/drawingContext.ScaleX;
					graphics.AddLine(p1, p2);
					graphics.AddLine(p3, p4);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}

				h1.Draw(graphics, drawingContext);
				h2.Draw(graphics, drawingContext);
				h3.Draw(graphics, drawingContext);
				h4.Draw(graphics, drawingContext);
				center.Draw(graphics, drawingContext);
				rotate.Draw(graphics, drawingContext);
			}
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


		// Transforme un point.
		static public Point DotTransform(Selector selector, Point pos)
		{
			double sx=0.5, sy=0.5;

			if ( selector.initialData.P1.X != selector.initialData.P2.X )
			{
				sx = (pos.X-selector.initialData.P1.X) / (selector.initialData.P2.X-selector.initialData.P1.X);
			}

			if ( selector.initialData.P1.Y != selector.initialData.P2.Y )
			{
				sy = (pos.Y-selector.initialData.P1.Y) / (selector.initialData.P2.Y-selector.initialData.P1.Y);
			}

			if ( selector.finalData.TypeInUse == SelectorType.Stretcher )
			{
				Point p14 = Point.Scale(selector.finalData.P1, selector.finalData.P4, sx);
				Point p32 = Point.Scale(selector.finalData.P3, selector.finalData.P2, sx);
				pos = Point.Scale(p14, p32, sy);
			}
			else
			{
				pos.X = selector.finalData.P1.X + sx*(selector.finalData.P2.X-selector.finalData.P1.X);
				pos.Y = selector.finalData.P1.Y + sy*(selector.finalData.P2.Y-selector.finalData.P1.Y);

				double rot = selector.finalData.Angle-selector.initialData.Angle;
				if ( rot != 0 )
				{
					pos = Transform.RotatePointDeg(selector.finalData.Center, rot, pos);
				}
			}

			return pos;
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

				this.initialData = new SelectorData();
				this.host.initialData.CopyTo(this.initialData);

				this.finalData = new SelectorData();
				this.host.finalData.CopyTo(this.finalData);
			}

			protected void Swap()
			{
				this.host.document.Notifier.NotifyArea(this.host.BoundingBox);
				this.host.initialData.Swap(this.initialData);
				this.host.finalData.Swap(this.finalData);
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
			protected SelectorData			initialData;
			protected SelectorData			finalData;
		}
		#endregion


		#region SelectorData
		/// <summary>
		/// La classe SelectorData contient les définitions du modificateur global.
		/// </summary>
		protected class SelectorData
		{
			public SelectorData()
			{
				this.typeChoice = SelectorType.Auto;
				this.typeInUse = SelectorType.Zoomer;
				this.center.X = 0.5;
				this.center.Y = 0.5;
				this.angle = 0.0;
			}

			public SelectorType TypeChoice
			{
				get { return this.typeChoice; }
				set { this.typeChoice = value; }
			}

			public SelectorType TypeInUse
			{
				get { return this.typeInUse; }
				set { this.typeInUse = value; }
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
					Rectangle rect = new Rectangle(this.p1, this.p2);
					rect.MergeWith(this.p3);
					rect.MergeWith(this.p4);
					return rect;
				}
			}

			public Point P1
			{
				get
				{
					return this.p1;
				}

				set
				{
					this.p1 = value;

					if ( this.typeInUse == SelectorType.Zoomer )
					{
						this.p3.X = value.X;
						this.p4.Y = value.Y;
					}
				}
			}

			public Point P2
			{
				get
				{
					return this.p2;
				}

				set
				{
					this.p2 = value;

					if ( this.typeInUse == SelectorType.Zoomer )
					{
						this.p4.X = value.X;
						this.p3.Y = value.Y;
					}
				}
			}

			public Point P3
			{
				get
				{
					return this.p3;
				}

				set
				{
					this.p3 = value;

					if ( this.typeInUse == SelectorType.Zoomer )
					{
						this.p1.X = value.X;
						this.p2.Y = value.Y;
					}
				}
			}

			public Point P4
			{
				get
				{
					return this.p4;
				}

				set
				{
					this.p4 = value;

					if ( this.typeInUse == SelectorType.Zoomer )
					{
						this.p2.X = value.X;
						this.p1.Y = value.Y;
					}
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
				dest.typeChoice = this.typeChoice;
				dest.typeInUse  = this.typeInUse;
				dest.visible    = this.visible;
				dest.handles    = this.handles;
				dest.p1         = this.p1;
				dest.p2         = this.p2;
				dest.p3         = this.p3;
				dest.p4         = this.p4;
				dest.center     = this.center;
				dest.angle      = this.angle;
			}

			public void Swap(SelectorData dest)
			{
				SelectorType th = dest.typeChoice;
				dest.typeChoice = this.typeChoice;
				this.typeChoice = th;

				SelectorType tu = dest.typeInUse;
				dest.typeInUse = this.typeInUse;
				this.typeInUse = tu;

				Misc.Swap(ref dest.visible, ref this.visible);
				Misc.Swap(ref dest.handles, ref this.handles);
				Misc.Swap(ref dest.p1,      ref this.p1);
				Misc.Swap(ref dest.p2,      ref this.p2);
				Misc.Swap(ref dest.p3,      ref this.p3);
				Misc.Swap(ref dest.p4,      ref this.p4);
				Misc.Swap(ref dest.center,  ref this.center);
				Misc.Swap(ref dest.angle,   ref this.angle);
			}


			protected SelectorType			typeChoice;
			protected SelectorType			typeInUse;
			protected bool					visible;
			protected bool					handles;
			protected Point					p1;			// un coin quelconque
			protected Point					p2;			// le coin opposé
			protected Point					p3;			// en mode Stretcher
			protected Point					p4;			// en mode Stretcher
			protected Point					center;		// [0..1]
			protected double				angle;		// en degrés
		}
		#endregion


		protected Document			document;
		protected SelectorData		initialData;
		protected SelectorData		finalData;
		protected Objects.Handle	h1;
		protected Objects.Handle	h2;
		protected Objects.Handle	h3;
		protected Objects.Handle	h4;
		protected Objects.Handle	center;
		protected Objects.Handle	rotate;
		protected Point				moveStart;
		protected Point				moveOffset;
	}
}
