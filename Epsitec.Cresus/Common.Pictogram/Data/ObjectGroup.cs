using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectGroup est la classe de l'objet graphique "groupe".
	/// </summary>
	public class ObjectGroup : AbstractObject
	{
		public ObjectGroup()
		{
			this.objects = new System.Collections.ArrayList();
		}

		public override void CreateProperties()
		{
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectGroup();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/objgroup1.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;

			Drawing.Rectangle rect = new Epsitec.Common.Drawing.Rectangle();
			rect.Left   = System.Math.Min(p1.X, p2.X);
			rect.Right  = System.Math.Max(p1.X, p2.X);
			rect.Bottom = System.Math.Min(p1.Y, p2.Y);
			rect.Top    = System.Math.Max(p1.Y, p2.Y);
			return rect.Contains(pos);
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainFixStarting(pos, ConstrainType.Square);
			this.HandleAdd(pos, HandleType.Hide);  // rang = 0
			this.HandleAdd(pos, HandleType.Hide);  // rang = 1
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
			this.durtyBbox = true;
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
			iconContext.ConstrainDelStarting();
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(IconContext iconContext)
		{
			double len = Drawing.Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > this.minimalSize );
		}

		
		// Modifie le rectangle englobant.
		public void SetBoundingBox(Drawing.Rectangle bbox)
		{
			if ( this.handles.Count == 0 )
			{
				this.HandleAdd(new Drawing.Point(0,0), HandleType.Hide);
				this.HandleAdd(new Drawing.Point(0,0), HandleType.Hide);
			}
			this.Handle(0).Position = bbox.BottomLeft;
			this.Handle(1).Position = bbox.TopRight;
			this.bbox = bbox;
		}

		// Met à jour le rectangle englobant l'objet.
		public override void UpdateBoundingBox()
		{
			this.bbox = new Drawing.Rectangle(this.Handle(0).Position, this.Handle(1).Position);
		}

		// Crée le chemin d'un rectangle.
		protected Drawing.Path PathRectangle(Drawing.Rectangle rect)
		{
			double ox = System.Math.Min(rect.Left, rect.Right);
			double oy = System.Math.Min(rect.Bottom, rect.Top);
			double dx = System.Math.Abs(rect.Width);
			double dy = System.Math.Abs(rect.Height);

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(ox, oy);
			path.LineTo(ox+dx, oy);
			path.LineTo(ox+dx, oy+dy);
			path.LineTo(ox, oy+dy);
			path.Close();
			return path;
		}

		// Crée le chemin des coins d'un rectangle.
		protected Drawing.Path PathCorners(Drawing.Rectangle rect)
		{
			Drawing.Point p1 = new Drawing.Point(rect.Left, rect.Bottom);
			Drawing.Point p2 = new Drawing.Point(rect.Left, rect.Top);
			Drawing.Point p3 = new Drawing.Point(rect.Right, rect.Top);
			Drawing.Point p4 = new Drawing.Point(rect.Right, rect.Bottom);
			double dim = System.Math.Min(rect.Width*0.25, rect.Height*0.25);

			Drawing.Path path = new Drawing.Path();

			path.MoveTo(Drawing.Point.Move(p1, p4, dim));
			path.LineTo(p1);
			path.LineTo(Drawing.Point.Move(p1, p2, dim));

			path.MoveTo(Drawing.Point.Move(p2, p1, dim));
			path.LineTo(p2);
			path.LineTo(Drawing.Point.Move(p2, p3, dim));

			path.MoveTo(Drawing.Point.Move(p3, p2, dim));
			path.LineTo(p3);
			path.LineTo(Drawing.Point.Move(p3, p4, dim));

			path.MoveTo(Drawing.Point.Move(p4, p3, dim));
			path.LineTo(p4);
			path.LineTo(Drawing.Point.Move(p4, p1, dim));

			return path;
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			base.DrawGeometry(graphics, iconContext);

			if ( this.TotalHandle < 2 )  return;
			if ( !iconContext.IsEditable || iconContext.IsDimmed )  return;

			Drawing.Rectangle rect = new Drawing.Rectangle();
			rect.Left   = this.Handle(0).Position.X;
			rect.Bottom = this.Handle(0).Position.Y;
			rect.Right  = this.Handle(1).Position.X;
			rect.Top    = this.Handle(1).Position.Y;
			rect.Normalise();
			graphics.Align(ref rect);
			rect.Inflate(0.5/iconContext.ScaleX, 0.5/iconContext.ScaleX);

			Drawing.Path path = this.PathCorners(rect);
			this.bbox = path.ComputeBounds();

			Drawing.Color color = Drawing.Color.FromBrightness(0.7);
			if ( this.IsSelected() )  color = Drawing.Color.FromRGB(1,0,0);

			graphics.Rasterizer.AddOutline(path, 1.0/iconContext.ScaleX);
			graphics.RenderSolid(color);

			if ( this.IsHilite )
			{
				path = this.PathRectangle(rect);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(iconContext.HiliteColor);
			}
		}

		// Retourne l'origine de l'objet.
		public override Drawing.Point Origin
		{
			get
			{
				return this.Handle(0).Position;
			}
		}
	}
}
