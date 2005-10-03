using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Document
{
	public enum SelectorType
	{
		None,
		Auto,			// mode automatique
		Individual,		// objets individuels
		Scaler,			// mise � l'�chelle et rotation
		Stretcher,		// d�formation
	}

	public enum SelectorTypeStretch
	{
		Free,			// libre
		TrapezeH,		// trap�ze horizontal
		TrapezeV,		// trap�ze vertical
		ParallelH,		// parall�logramme horizontal
		ParallelV,		// parall�logramme vertical
	}

	/// <summary>
	/// La classe Selector permet de modifier une s�lection d'objets.
	/// </summary>
	public class Selector
	{
		public Selector(Document document)
		{
			this.document = document;

			this.initialData = new SelectorData();
			this.initialData.Visible = false;
			this.initialData.Handles = false;
			this.initialBBoxThin = Rectangle.Empty;

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

		public SelectorTypeStretch TypeStretch
		{
			get
			{
				return this.finalData.TypeStretch;
			}

			set
			{
				if ( this.finalData.TypeStretch != value )
				{
					this.OpletQueueInsert();
					this.finalData.TypeStretch = value;
					this.UpdateHandleVisible();
				}
			}
		}

		public Rectangle InitialBBoxThin
		{
			set
			{
				this.initialBBoxThin = value;
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

		// Fixe le d�part pour un rectangle de s�lection simple (sans poign�es).
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

		// Fixe l'arriv�e pour un rectangle de s�lection simple (sans poign�es).
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

		// Initialise le s�lectionneur complexe (avec poign�es).
		public void Initialize(Rectangle rect, double angle)
		{
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.OpletQueueInsert();
			this.finalData.P1 = rect.BottomLeft;
			this.finalData.P2 = rect.TopRight;
			this.finalData.P3 = rect.TopLeft;
			this.finalData.P4 = rect.BottomRight;
			this.finalData.Center = rect.Center;
			this.finalData.Angle = angle;
			this.UpdateHandlePos();
			this.Visible = true;
			this.Handles = true;
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// Initialise le s�lectionneur pour effectuer une mise � l'�chelle rapide
		// (c'est-�-dire non interactif).
		public void QuickScale(Rectangle rInitial, Rectangle rFinal)
		{
			this.initialData.P1 = rInitial.BottomLeft;
			this.initialData.P2 = rInitial.TopRight;
			this.initialData.P3 = rInitial.TopLeft;
			this.initialData.P4 = rInitial.BottomRight;
			this.initialData.Center = rInitial.Center;
			this.initialData.Angle = 0.0;

			this.finalData.TypeInUse = SelectorType.Scaler;
			this.finalData.P1 = rFinal.BottomLeft;
			this.finalData.P2 = rFinal.TopRight;
			this.finalData.P3 = rFinal.TopLeft;
			this.finalData.P4 = rFinal.BottomRight;
			this.finalData.Center = rFinal.Center;
			this.finalData.Angle = 0.0;
		}

		// Retourne une position d'une poign�e.
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

		// D�tecte si la souris est dans le modificateur.
		public bool Detect(Point mouse, bool global, out int rank)
		{
			if ( this.finalData.Visible )
			{
				if ( this.finalData.TypeInUse == SelectorType.Scaler )
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
					InsideSurface s = new InsideSurface(mouse, 4);
					s.AddLine(h1.Position, h3.Position);
					s.AddLine(h3.Position, h2.Position);
					s.AddLine(h2.Position, h4.Position);
					s.AddLine(h4.Position, h1.Position);
					if ( s.IsInside() )  { rank = 0;  return true; }
				}
			}
			rank = -1;
			return false;
		}

		// Met en �vidence la poign�e survoll�e.
		public void HiliteHandle(int rank)
		{
			this.h1.IsHilited     = (rank==1);
			this.h2.IsHilited     = (rank==2);
			this.h3.IsHilited     = (rank==3);
			this.h4.IsHilited     = (rank==4);
			this.center.IsHilited = (rank==5);
			this.rotate.IsHilited = (rank==6);
		}

		// Nomme l'action pour les undo/redo.
		public void MoveNameAction(int rank)
		{
			if ( rank == 0 )  // global ?
			{
				this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.Selector.Move);
			}
			else if ( rank == 5 )  // center ?
			{
				this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.Selector.Center);
			}
			else if ( rank == 6 )  // rotate ?
			{
				this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.Selector.Rotate);
			}
			else
			{
				if ( this.finalData.TypeInUse == SelectorType.Stretcher )
				{
					this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.Selector.Stretcher);
				}
				else
				{
					this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.Selector.Scaler);
				}
			}
		}

		// Une poign�e du modificateur sera d�plac�e.
		public void MoveStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			this.OpletQueueInsert();

			drawingContext.ConstrainFlush();
			if ( rank == 0 )  // global ?
			{
				drawingContext.ConstrainAddHV(pos);
			}
			else if ( rank == 5 )  // center ?
			{
				drawingContext.ConstrainAddHV(this.Position(5));
			}
			else if ( rank == 6 )  // rotate ?
			{
				drawingContext.ConstrainAddCenter(this.Position(5));
			}
			else
			{
				Point origin = new Point(0, 0);
				if ( rank == 1 )  origin = this.finalData.P2;  // inf/gauche ?
				if ( rank == 2 )  origin = this.finalData.P1;  // sup/droite ?
				if ( rank == 3 )  origin = this.finalData.P4;  // sup/gauche ?
				if ( rank == 4 )  origin = this.finalData.P3;  // inf/droite ?
				drawingContext.ConstrainAddHV(this.Position(rank));
				drawingContext.ConstrainAddLine(this.Position(rank), origin);
			}

			this.MoveTextInfoModif(rank);
			this.moveStart = pos;
			this.moveOffset = pos-this.Position(rank);
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// D�place une poign�e du modificateur.
		public void MoveProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);

			if ( rank != 6 )  // pas rotate ?
			{
				if ( this.initialBBoxThin.IsEmpty )
				{
					drawingContext.SnapGrid(ref pos);
				}
				else
				{
					Rectangle bbox = this.initialBBoxThin;
					bbox.Offset(pos-this.moveStart);
					drawingContext.SnapGrid(ref pos, -this.moveStart, bbox);
				}
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

				if ( this.finalData.TypeInUse == SelectorType.Scaler )
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

			if ( rank >= 1 && rank <= 4 )  // coin ?
			{
				Point initialP1 = this.finalData.P1;
				Point initialP2 = this.finalData.P2;
				Point initialP3 = this.finalData.P3;
				Point initialP4 = this.finalData.P4;

				this.MoveCorner(rank, pos);

				if ( !this.IsCorrectGeometry() )
				{
					this.finalData.P1 = initialP1;
					this.finalData.P2 = initialP2;
					this.finalData.P3 = initialP3;
					this.finalData.P4 = initialP4;
				}
			}

			if ( rank == 5 )  // center ?
			{
				this.finalData.Center = pos;
			}

			if ( rank == 6 )  // rotate ?
			{
				this.finalData.Angle = Point.ComputeAngleDeg(this.finalData.Center, pos)-90;
			}

			this.MoveTextInfoModif(rank);
			this.UpdateHandlePos();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// D�place un coin.
		protected void MoveCorner(int rank, Point pos)
		{
			Point move = pos-this.finalData.GetCorner(rank);
			this.finalData.SetCorner(rank, pos);

			if ( this.finalData.TypeInUse != SelectorType.Stretcher )  return;
			if ( this.finalData.TypeStretch == SelectorTypeStretch.Free )  return;

			int rankH = Selector.CornerH(rank);
			int rankV = Selector.CornerV(rank);
			int rankO = Selector.CornerO(rank);

			if ( this.finalData.TypeStretch == SelectorTypeStretch.ParallelH )
			{
				this.finalData.SetCorner(rankH, this.finalData.GetCorner(rankH)+move);
			}

			if ( this.finalData.TypeStretch == SelectorTypeStretch.ParallelV )
			{
				this.finalData.SetCorner(rankV, this.finalData.GetCorner(rankV)+move);
			}

			if ( this.finalData.TypeStretch == SelectorTypeStretch.TrapezeH )
			{
				move.X = -move.X;
				this.finalData.SetCorner(rankH, this.finalData.GetCorner(rankH)+move);
			}

			if ( this.finalData.TypeStretch == SelectorTypeStretch.TrapezeV )
			{
				move.Y = -move.Y;
				this.finalData.SetCorner(rankV, this.finalData.GetCorner(rankV)+move);
			}
		}

		protected static int CornerH(int rank)
		{
			switch ( rank )
			{
				case 1:  return 4;  // inf/gauche ?
				case 2:  return 3;  // sup/droite ?
				case 3:  return 2;  // sup/gauche ?
				case 4:  return 1;  // inf/droite ?
			}
			return -1;
		}

		protected static int CornerV(int rank)
		{
			switch ( rank )
			{
				case 1:  return 3;  // inf/gauche ?
				case 2:  return 4;  // sup/droite ?
				case 3:  return 1;  // sup/gauche ?
				case 4:  return 2;  // inf/droite ?
			}
			return -1;
		}

		protected static int CornerO(int rank)
		{
			switch ( rank )
			{
				case 1:  return 2;  // inf/gauche ?
				case 2:  return 1;  // sup/droite ?
				case 3:  return 4;  // sup/gauche ?
				case 4:  return 3;  // inf/droite ?
			}
			return -1;
		}

		// Fin du d�placement.
		public void MoveEnding(int rank, Point pos, DrawingContext drawingContext)
		{
			this.document.Modifier.TextInfoModif = "";
		}

		// G�n�re le texte d'information.
		protected void MoveTextInfoModif(int rank)
		{
			string text = "";

			if ( rank == 0 )  // tout ?
			{
				double dx = this.finalData.P1.X - this.initialData.P1.X;
				double dy = this.finalData.P1.Y - this.initialData.P1.Y;
				text = string.Format(Res.Strings.Action.Selector.Info.Move, this.document.Modifier.RealToString(dx), this.document.Modifier.RealToString(dy));
			}
			else if ( rank <= 4 )  // coin ?
			{
				double idx = this.initialData.P2.X - this.initialData.P1.X;
				double idy = this.initialData.P2.Y - this.initialData.P1.Y;
				double fdx = this.finalData.P2.X - this.finalData.P1.X;
				double fdy = this.finalData.P2.Y - this.finalData.P1.Y;
				double zx = (idx==0) ? 100.0 : fdx/idx*100.0;
				double zy = (idy==0) ? 100.0 : fdy/idy*100.0;
				text = string.Format(Res.Strings.Action.Selector.Info.Scale, zx.ToString("F1"), zy.ToString("F1"));
			}
			else if ( rank == 5 )  // center ?
			{
				double dx = this.finalData.Center.X - this.initialData.Center.X;
				double dy = this.finalData.Center.Y - this.initialData.Center.Y;
				text = string.Format(Res.Strings.Action.Selector.Info.Center, this.document.Modifier.RealToString(dx), this.document.Modifier.RealToString(dy));
			}
			else if ( rank == 6 )  // rotate ?
			{
				double a = this.finalData.Angle;
				if ( a < 0.0 )  a += 360.0;
				text = string.Format(Res.Strings.Action.Selector.Info.Rotate, this.document.Modifier.AngleToString(a));
			}

			this.document.Modifier.TextInfoModif = text;
		}


		// Effectue une op�ration de miroir.
		public void OperMirror(bool horizontal)
		{
			this.OpletQueueInsert();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Modifier.ActiveViewer.MoveGlobalStarting();

			Point p1 = this.finalData.P1;
			Point p2 = this.finalData.P2;
			Point center = this.finalData.Center;
			if ( horizontal )
			{
				this.finalData.P1 = new Point(center.X*2-p1.X, p1.Y);
				this.finalData.P2 = new Point(center.X*2-p2.X, p2.Y);
			}
			else
			{
				this.finalData.P1 = new Point(p1.X, center.Y*2-p1.Y);
				this.finalData.P2 = new Point(p2.X, center.Y*2-p2.Y);
			}

			this.document.Modifier.ActiveViewer.MoveGlobalProcess(this);
			this.UpdateHandlePos();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// Effectue une op�ration de d�placement.
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

		// Effectue une op�ration de mise � l'�chelle.
		public void OperScale(double scale)
		{
			this.OpletQueueInsert();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Modifier.ActiveViewer.MoveGlobalStarting();
			Point center = this.finalData.Center;
			this.finalData.P1 = Point.Scale(center, this.finalData.P1, scale);
			this.finalData.P2 = Point.Scale(center, this.finalData.P2, scale);
			this.document.Modifier.ActiveViewer.MoveGlobalProcess(this);
			this.UpdateHandlePos();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}

		// Effectue une op�ration de rotation.
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

		// Met � jour l'�tat des poign�es en fonction des donn�es.
		protected void UpdateHandleVisible()
		{
			bool visible = this.Visible && this.Handles;
			this.h1.IsVisible = visible;
			this.h2.IsVisible = visible;
			this.h3.IsVisible = visible;
			this.h4.IsVisible = visible;

			if ( this.finalData.TypeInUse == SelectorType.Scaler )
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

		// Met � jour la position des poign�es en fonction des donn�es.
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

			if ( this.finalData.TypeInUse == SelectorType.Scaler )
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
				if ( this.finalData.TypeInUse == SelectorType.Scaler )
				{
					Point c = this.finalData.Center;
					Point p = this.rotate.Position;
					graphics.LineWidth = 1.0/drawingContext.ScaleX;
					graphics.AddCircle(c, this.finalData.Radius);
					graphics.AddLine(c, p);
					graphics.AddLine(p, this.ComputeExtremity(c,p, 0.4, 0.2, 0));
					graphics.AddLine(p, this.ComputeExtremity(c,p, 0.4, 0.2, 1));  // fl�che
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

		// Calcule l'extr�mit� gauche ou droite de la fl�che.
		protected Point ComputeExtremity(Point p1, Point p2, double para, double perp, int rank)
		{
			double distPara = Point.Distance(p1, p2)*para;
			double distPerp = Point.Distance(p1, p2)*perp;
			Point c = Point.Move(p2, p1, distPara);
			Point p = Point.Move(c, Point.Symmetry(p2, p1), distPerp);
			double angle = (rank==0) ? 90 : -90;
			return Transform.RotatePointDeg(c, angle, p);
		}

		// Lors d'une d�formation, la forme finale ne doit jamais contenir d'angle
		// aigu, car Stretcher.Reverse ne s'en sort pas dans ce as.
		protected bool IsCorrectGeometry()
		{
			if ( this.finalData.TypeInUse != SelectorType.Stretcher )  return true;

			InsideSurface i;
			
			i = new InsideSurface(this.finalData.P1, 4);
			i.AddLine(this.finalData.P3, this.finalData.P4);
			i.AddLine(this.finalData.P4, this.finalData.P2);
			i.AddLine(this.finalData.P2, this.finalData.P3);
			if ( i.IsInside() )  return false;

			i = new InsideSurface(this.finalData.P2, 4);
			i.AddLine(this.finalData.P4, this.finalData.P3);
			i.AddLine(this.finalData.P3, this.finalData.P1);
			i.AddLine(this.finalData.P1, this.finalData.P4);
			if ( i.IsInside() )  return false;

			i = new InsideSurface(this.finalData.P3, 4);
			i.AddLine(this.finalData.P2, this.finalData.P1);
			i.AddLine(this.finalData.P1, this.finalData.P4);
			i.AddLine(this.finalData.P4, this.finalData.P2);
			if ( i.IsInside() )  return false;

			i = new InsideSurface(this.finalData.P4, 4);
			i.AddLine(this.finalData.P1, this.finalData.P2);
			i.AddLine(this.finalData.P2, this.finalData.P3);
			i.AddLine(this.finalData.P3, this.finalData.P1);
			if ( i.IsInside() )  return false;

			return true;
		}


		// Transforme un point.
		public Point DotTransform(Point pos)
		{
			double sx=0.5, sy=0.5;

			if ( this.initialData.P1.X != this.initialData.P2.X )
			{
				sx = (pos.X-this.initialData.P1.X) / (this.initialData.P2.X-this.initialData.P1.X);
			}

			if ( this.initialData.P1.Y != this.initialData.P2.Y )
			{
				sy = (pos.Y-this.initialData.P1.Y) / (this.initialData.P2.Y-this.initialData.P1.Y);
			}

			if ( this.finalData.TypeInUse == SelectorType.Stretcher )
			{
				Stretcher s = new Stretcher();

				if ( this.initialData.P3.X == this.initialData.P1.X &&
					 this.initialData.P3.Y == this.initialData.P2.Y &&
					 this.initialData.P4.X == this.initialData.P2.X &&
					 this.initialData.P4.Y == this.initialData.P1.Y )
				{
					// Si la forme initiale est rectangulaire, il suffit d'une
					// simple transformation normale dans la forme de destination.
					s.InitialRectangle = new Rectangle(this.initialData.P1, this.initialData.P2);
					this.finalData.ToStretcher(s);
					pos = s.Transform(pos);
				}
				else
				{
					// Si la forme initiale est quelconque, il faut d'abord effectuer
					// une transformation inverse dans un rectangle arbitraire unit�,
					// puis ensuite effectuer une transformation normale de ce
					// rectangle dans la forme de destination.
					s.InitialRectangle = new Rectangle(0, 0, 1, 1);
					this.initialData.ToStretcher(s);
					pos = s.Reverse(pos);

					this.finalData.ToStretcher(s);
					pos = s.Transform(pos);
				}
			}
			else
			{
				pos.X = this.finalData.P1.X + sx*(this.finalData.P2.X-this.finalData.P1.X);
				pos.Y = this.finalData.P1.Y + sy*(this.finalData.P2.Y-this.finalData.P1.Y);

				double rot = this.finalData.Angle-this.initialData.Angle;
				if ( rot != 0 )
				{
					pos = Transform.RotatePointDeg(this.finalData.Center, rot, pos);
				}
			}

			return pos;
		}

		// Retourne l'angle de rotation.
		public double GetTransformAngle
		{
			get
			{
				return this.finalData.Angle-this.initialData.Angle;
			}
		}

		// Retourne le facteur de mise � l'�chelle moyen.
		public double GetTransformScale
		{
			get
			{
				return (this.GetTransformScaleX+this.GetTransformScaleY)/2.0;
			}
		}

		// Retourne le facteur de mise � l'�chelle horizontal.
		public double GetTransformScaleX
		{
			get
			{
				double di = this.initialData.P2.X-this.initialData.P1.X;
				double df = this.finalData.P2.X-this.finalData.P1.X;
				return (di == 0) ? 1.0 : System.Math.Abs(df/di);
			}
		}

		// Retourne le facteur de mise � l'�chelle vertical.
		public double GetTransformScaleY
		{
			get
			{
				double di = this.initialData.P2.Y-this.initialData.P1.Y;
				double df = this.finalData.P2.Y-this.finalData.P1.Y;
				return (di == 0) ? 1.0 : System.Math.Abs(df/di);
			}
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
		/// La classe SelectorData contient les d�finitions du modificateur global.
		/// </summary>
		protected class SelectorData
		{
			public SelectorData()
			{
				this.typeChoice = SelectorType.Auto;
				this.typeInUse = SelectorType.Scaler;
				this.TypeStretch = SelectorTypeStretch.Free;
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

			public SelectorTypeStretch TypeStretch
			{
				get { return this.typeStretch; }
				set { this.typeStretch = value; }
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

			public Point GetCorner(int rank)
			{
				switch ( rank )
				{
					case 1:  return this.p1;
					case 2:  return this.p2;
					case 3:  return this.p3;
					case 4:  return this.p4;
				}
				return new Point(0.0, 0.0);
			}

			public void SetCorner(int rank, Point pos)
			{
				switch ( rank )
				{
					case 1:  this.P1 = pos;  break;
					case 2:  this.P2 = pos;  break;
					case 3:  this.P3 = pos;  break;
					case 4:  this.P4 = pos;  break;
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

					if ( this.typeInUse == SelectorType.Scaler )
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

					if ( this.typeInUse == SelectorType.Scaler )
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

					if ( this.typeInUse == SelectorType.Scaler )
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

					if ( this.typeInUse == SelectorType.Scaler )
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

			public void ToStretcher(Stretcher stretcher)
			{
				stretcher.FinalBottomLeft  = this.p1;
				stretcher.FinalBottomRight = this.p4;
				stretcher.FinalTopLeft     = this.p3;
				stretcher.FinalTopRight    = this.p2;
			}

			public void CopyTo(SelectorData dest)
			{
				dest.typeChoice  = this.typeChoice;
				dest.typeInUse   = this.typeInUse;
				dest.typeStretch = this.typeStretch;
				dest.visible     = this.visible;
				dest.handles     = this.handles;
				dest.p1          = this.p1;
				dest.p2          = this.p2;
				dest.p3          = this.p3;
				dest.p4          = this.p4;
				dest.center      = this.center;
				dest.angle       = this.angle;
			}

			public void Swap(SelectorData dest)
			{
				SelectorType th = dest.typeChoice;
				dest.typeChoice = this.typeChoice;
				this.typeChoice = th;

				SelectorType tu = dest.typeInUse;
				dest.typeInUse = this.typeInUse;
				this.typeInUse = tu;

				SelectorTypeStretch ts = dest.typeStretch;
				dest.typeStretch = this.typeStretch;
				this.typeStretch = ts;

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
			protected SelectorTypeStretch	typeStretch;
			protected bool					visible;
			protected bool					handles;
			protected Point					p1;			// un coin quelconque
			protected Point					p2;			// le coin oppos�
			protected Point					p3;			// en mode Stretcher
			protected Point					p4;			// en mode Stretcher
			protected Point					center;		// [0..1]
			protected double				angle;		// en degr�s
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
		protected Rectangle			initialBBoxThin;
	}
}
