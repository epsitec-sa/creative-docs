using Epsitec.Common.Pictogram.Widgets;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe GlobalModifier permet de modifier une sélection d'objets.
	/// </summary>
	public class GlobalModifier : Epsitec.Common.Widgets.Widget
	{
		public GlobalModifier()
		{
			this.data = new GlobalModifierData();
			this.data.Visible = false;

			this.h1 = new Handle();
			this.h2 = new Handle();
			this.h3 = new Handle();
			this.h4 = new Handle();
			this.center = new Handle();
			this.rotate = new Handle();

			this.h1.IsSelected = true;
			this.h2.IsSelected = true;
			this.h3.IsSelected = true;
			this.h4.IsSelected = true;
			this.center.IsSelected = true;
			this.rotate.IsSelected = true;
			this.center.Type = HandleType.Center;
			this.rotate.Type = HandleType.Rotate;
		}

		public GlobalModifierData Data
		{
			get { return this.data; }
			set { this.data = value; }
		}

		public GlobalModifierData CloneData()
		{
			GlobalModifierData copy = new GlobalModifierData();
			this.data.CopyTo(copy);
			return copy;
		}

		public bool Visible
		{
			get { return this.data.Visible; }
			set { this.data.Visible = value; }
		}

		public void Initialize(Drawing.Rectangle rect)
		{
			this.data.P1 = rect.BottomLeft;
			this.data.P2 = rect.TopRight;
			this.data.Center = rect.Center;
			this.data.Angle = 0.0;
			this.UpdateHandle();
		}

		// Retourne une position d'une poignée.
		public Drawing.Point Position(int rank)
		{
			if ( rank == 0 )  return this.h1.Position;
			if ( rank == 1 )  return this.h1.Position;
			if ( rank == 2 )  return this.h2.Position;
			if ( rank == 3 )  return this.h3.Position;
			if ( rank == 4 )  return this.h4.Position;
			if ( rank == 5 )  return this.center.Position;
			if ( rank == 6 )  return this.rotate.Position;
			return new Drawing.Point(0, 0);
		}

		// Détecte si la souris est dans le modificateur.
		public bool Detect(Drawing.Point mouse, out int rank)
		{
			if ( this.data.Visible )
			{
				if ( this.rotate.Detect(mouse) )  { rank = 6;  return true; }
				if ( this.center.Detect(mouse) )  { rank = 5;  return true; }

				if ( this.h4.Detect(mouse) )  { rank = 4;  return true; }
				if ( this.h3.Detect(mouse) )  { rank = 3;  return true; }
				if ( this.h2.Detect(mouse) )  { rank = 2;  return true; }
				if ( this.h1.Detect(mouse) )  { rank = 1;  return true; }

				Drawing.Rectangle rect = new Drawing.Rectangle(h1.Position, h2.Position);
				if ( rect.Contains(mouse) )  { rank = 0;  return true; }
			}
			rank = -1;
			return false;
		}

		// Met en évidence la poignée survollée.
		public void HiliteHandle(int rank, IconContext iconContext, ref Drawing.Rectangle bbox)
		{
			this.HiliteHandle(this.h1,     (rank==1), iconContext, ref bbox);
			this.HiliteHandle(this.h2,     (rank==2), iconContext, ref bbox);
			this.HiliteHandle(this.h3,     (rank==3), iconContext, ref bbox);
			this.HiliteHandle(this.h4,     (rank==4), iconContext, ref bbox);
			this.HiliteHandle(this.center, (rank==5), iconContext, ref bbox);
			this.HiliteHandle(this.rotate, (rank==6), iconContext, ref bbox);
		}

		// Met en évidence une poignée.
		protected void HiliteHandle(Handle handle, bool hilite, IconContext iconContext, ref Drawing.Rectangle bbox)
		{
			if ( handle.IsHilited == hilite )  return;
			handle.IsHilited = hilite;
			handle.BoundingBox(iconContext, ref bbox);
		}

		// Déplace tout le modificateur.
		public void MoveAll(Drawing.Point move)
		{
			this.data.P1 += move;
			this.data.P2 += move;
			this.UpdateHandle();
		}

		// Une poignée du modificateur sera déplacée.
		public void MoveStarting(int rank, Drawing.Point pos, IconContext iconContext)
		{
			if ( rank == 0 )  // global ?
			{
				iconContext.ConstrainFixStarting(pos);
			}
			else if ( rank == 5 )  // center ?
			{
				iconContext.ConstrainFixStarting(this.Position(5), ConstrainType.Line);
			}
			else if ( rank == 6 )  // rotate ?
			{
				iconContext.ConstrainFixStarting(this.Position(5));
			}
			else
			{
				Drawing.Point origin = new Epsitec.Common.Drawing.Point(0, 0);
				if ( rank == 1 )  origin = this.data.P2;  // inf/gauche ?
				if ( rank == 2 )  origin = this.data.P1;  // sup/droite ?
				if ( rank == 3 )  origin = this.data.P4;  // sup/gauche ?
				if ( rank == 4 )  origin = this.data.P3;  // inf/droite ?
				iconContext.ConstrainFixStarting(origin, this.Position(rank), ConstrainType.Scale);
			}

			this.moveStart = pos;
			this.moveOffset = pos-this.Position(rank);
		}

		// Déplace une poignée du modificateur.
		public void MoveProcess(int rank, Drawing.Point pos, IconContext iconContext)
		{
			if ( rank != 6 )  // pas rotate ?
			{
				iconContext.SnapGrid(this.moveStart, ref pos);
			}

			if ( rank == 0 )  // global ?
			{
				iconContext.ConstrainSnapPos(ref pos);
				pos -= this.moveOffset;
			}
			else
			{
				pos -= this.moveOffset;
				iconContext.ConstrainSnapPos(ref pos);
			}

			if ( rank == 0 )  // tout ?
			{
				Drawing.Point dim = this.data.P2-this.data.P1;
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
				this.data.Angle = Drawing.Point.ComputeAngleDeg(this.data.Center, pos) - 90;
			}

			this.UpdateHandle();
		}

		// Retourne la bbox du modificateur.
		public Drawing.Rectangle BoundingBox()
		{
			Drawing.Rectangle bbox = new Drawing.Rectangle(this.data.P1, this.data.P2);

			Drawing.Rectangle circle = new Drawing.Rectangle(this.data.Center, this.data.Center);
			circle.Inflate(this.data.Radius);
			bbox.MergeWith(circle);

			return bbox;
		}

		// Met à jour la position des poignées en fonction des données.
		public void UpdateHandle()
		{
			this.h1.Position = this.data.P1;
			this.h2.Position = this.data.P2;
			this.h3.Position = this.data.P3;
			this.h4.Position = this.data.P4;

			this.center.Position = this.data.Center;

			double radius = this.data.Radius;
			this.rotate.Position = this.data.Center + Drawing.Transform.RotatePointDeg(this.data.Angle, new Drawing.Point(0, radius));
		}

		// Dessine le modificateur.
		public void Draw(Drawing.Graphics graphics, IconContext iconContext)
		{
			if ( !this.data.Visible )  return;

			Drawing.Rectangle rect = new Drawing.Rectangle(this.data.P1, this.data.P2);
			graphics.LineWidth = 1.0/iconContext.ScaleX;
			rect.Inflate(-0.5/iconContext.ScaleX, -0.5/iconContext.ScaleY);

			Drawing.Color filledColor = iconContext.HiliteOutlineColor;
			filledColor.A *= 0.2;
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(filledColor);

			graphics.AddRectangle(rect);
			graphics.RenderSolid(iconContext.HiliteOutlineColor);

			Drawing.Point p1 = this.data.Center;
			Drawing.Point p2 = this.rotate.Position;
			this.PaintCircle(graphics, p1, this.data.Radius, this.data.Radius, iconContext.HiliteOutlineColor);
			graphics.AddLine(p1, p2);
			graphics.AddLine(p2, this.ComputeExtremity(p1, p2, 0.4, 0.2, 0));
			graphics.AddLine(p2, this.ComputeExtremity(p1, p2, 0.4, 0.2, 1));  // flèche
			graphics.RenderSolid(iconContext.HiliteOutlineColor);

			h1.Draw(graphics, iconContext);
			h2.Draw(graphics, iconContext);
			h3.Draw(graphics, iconContext);
			h4.Draw(graphics, iconContext);
			center.Draw(graphics, iconContext);
			rotate.Draw(graphics, iconContext);
		}

		// Dessine un cercle complet.
		protected void PaintCircle(Drawing.Graphics graphics,
								   Drawing.Point c, double rx, double ry,
								   Drawing.Color color)
		{
			Drawing.Path path = new Drawing.Path();
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
		protected Drawing.Point ComputeExtremity(Drawing.Point p1, Drawing.Point p2, double para, double perp, int rank)
		{
			double distPara = Drawing.Point.Distance(p1, p2)*para;
			double distPerp = Drawing.Point.Distance(p1, p2)*perp;
			Drawing.Point c = Drawing.Point.Move(p2, p1, distPara);
			Drawing.Point p = Drawing.Point.Move(c, Drawing.Point.Symmetry(p2, p1), distPerp);
			double angle = (rank==0) ? System.Math.PI/2 : -System.Math.PI/2;
			return Drawing.Transform.RotatePointRad(c, angle, p);
		}

		protected GlobalModifierData	data;
		protected Handle				h1;
		protected Handle				h2;
		protected Handle				h3;
		protected Handle				h4;
		protected Handle				center;
		protected Handle				rotate;
		protected Drawing.Point			moveStart;
		protected Drawing.Point			moveOffset;
	}


	/// <summary>
	/// La classe GlobalModifierData contient les définitions du modificateur global.
	/// </summary>
	public class GlobalModifierData
	{
		public GlobalModifierData()
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

		public Drawing.Point P1
		{
			get
			{
				return new Drawing.Point(p1.X, p1.Y);
			}

			set
			{
				this.p1.X = value.X;
				this.p1.Y = value.Y;
			}
		}

		public Drawing.Point P2
		{
			get
			{
				return new Drawing.Point(p2.X, p2.Y);
			}

			set
			{
				this.p2.X = value.X;
				this.p2.Y = value.Y;
			}
		}

		public Drawing.Point P3
		{
			get
			{
				return new Drawing.Point(p1.X, p2.Y);
			}

			set
			{
				this.p1.X = value.X;
				this.p2.Y = value.Y;
			}
		}

		public Drawing.Point P4
		{
			get
			{
				return new Drawing.Point(p2.X, p1.Y);
			}

			set
			{
				this.p2.X = value.X;
				this.p1.Y = value.Y;
			}
		}

		public Drawing.Point Center
		{
			get
			{
				return this.p1+(this.p2-this.p1).ScaleMul(this.center);
			}

			set
			{
				this.center = (value-this.p1).ScaleDiv(this.p2-this.p1);
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

		public void CopyTo(GlobalModifierData dest)
		{
			dest.visible = this.visible;
			dest.p1      = this.p1;
			dest.p2      = this.p2;
			dest.center  = this.center;
			dest.angle   = this.angle;
		}

		// Transforme un point.
		static public Drawing.Point Transform(GlobalModifierData initial, GlobalModifierData final, Drawing.Point pos)
		{
			Drawing.Point f = (pos-initial.P1).ScaleDiv(initial.P2-initial.P1);
			pos = f.ScaleMul(final.P2-final.P1)+final.P1;

			double rot = final.Angle-initial.Angle;
			if ( rot != 0 )
			{
				rot = rot*System.Math.PI/180.0;  // en radians
				pos = Drawing.Transform.RotatePointRad(final.Center, rot, pos);
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
		static protected Drawing.Point Deform(Drawing.Point s0, Drawing.Point s2,
											  Drawing.Point d0, Drawing.Point d1, Drawing.Point d2, Drawing.Point d3,
											  Drawing.Point p)
		{
			Drawing.Point	q = new Epsitec.Common.Drawing.Point();
			Drawing.Point	pp = new Epsitec.Common.Drawing.Point();

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
		protected Drawing.Point			p1;			// un coin quelconque
		protected Drawing.Point			p2;			// le coin opposé
		protected Drawing.Point			center;		// [0..1]
		protected double				angle;		// en degrés
	}

}
