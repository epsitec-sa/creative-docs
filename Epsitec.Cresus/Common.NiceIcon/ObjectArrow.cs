using System.Xml.Serialization;

namespace Epsitec.Common.NiceIcon
{
	/// <summary>
	/// La classe ObjectArrow est la classe de l'objet graphique "flèche".
	/// </summary>
	public class ObjectArrow : AbstractObject
	{
		public ObjectArrow()
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
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"manifest:Epsitec.Common.NiceIcon/Images.arrow.png"; }
		}


		[XmlAttribute]
		public double DistPara
		{
			get { return this.distPara; }
			set { this.distPara = value; }
		}

		[XmlAttribute]
		public double DistPerp
		{
			get { return this.distPerp; }
			set { this.distPerp = value; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;
			double width = System.Math.Max(this.PropertyLine(0).Width/2, this.minimalWidth);
			if ( Drawing.Point.Detect(p1,p2, pos, width) )  return true;

			p1 = this.ComputeExtremity(0);
			if ( Drawing.Point.Detect(p1,p2, pos, width) )  return true;

			p1 = this.ComputeExtremity(1);
			if ( Drawing.Point.Detect(p1,p2, pos, width) )  return true;

			return false;
		}


		// Déplace une poignée.
		public override void MoveHandle(int rank, Drawing.Point pos)
		{
			if ( rank == 2 )
			{
				this.ComputeDistances(pos);
				this.Handle(2).Position = this.ComputeExtremity(0);
			}
			else
			{
				this.Handle(rank).Position = pos;
				this.Handle(2).Position = this.ComputeExtremity(0);
			}
		}

		// Déplace tout l'objet.
		public override void MoveAll(Drawing.Point move)
		{
			this.Handle(0).Position += move;
			this.Handle(1).Position += move;
			this.Handle(2).Position = this.ComputeExtremity(0);
		}

		
		// Début de la création d'un objet.
		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainFixStarting(pos);
			this.HandleAdd(pos, HandleType.Primary);
			this.HandleAdd(pos, HandleType.Primary);
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
			iconContext.ConstrainDelStarting();

			// Crée la 3ème poignée.
			this.HandleAdd(this.ComputeExtremity(0), HandleType.Primary);
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(IconContext iconContext)
		{
			double len = Drawing.Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > this.minimalSize );
		}


		// Calcule l'extrémité gauche ou droite de la flèche.
		protected Drawing.Point ComputeExtremity(int rank)
		{
			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;
			Drawing.Point c = Drawing.Point.Move(p2, p1, this.distPara);
			Drawing.Point p = Drawing.Point.Move(c, p2, this.distPerp);
			double angle = (rank==0) ? System.Math.PI/2 : -System.Math.PI/2;
			return Drawing.Transform.RotatePoint(c, angle, p);
		}

		// Calcule les distances en fonction de la position de l'extrémité.
		protected void ComputeDistances(Drawing.Point pos)
		{
			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;
			Drawing.Point p = Drawing.Point.Projection(p1, p2, pos);
			this.distPara = Drawing.Point.Distance(p2, p);
			this.distPerp = Drawing.Point.Distance(p, pos);
		}


		// Reprend toutes les caractéristiques d'un objet.
		public override void CloneObject(AbstractObject src)
		{
			base.CloneObject(src);
			this.ComputeDistances(this.Handle(2).Position);
		}


		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			base.DrawGeometry(graphics, iconContext);

			if ( this.TotalHandle < 2 )  return;

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(this.Handle(0).Position);
			path.LineTo(this.Handle(1).Position);

			path.MoveTo(this.ComputeExtremity(0));
			path.LineTo(this.Handle(1).Position);
			path.LineTo(this.ComputeExtremity(1));

			graphics.Rasterizer.AddOutline(path, this.PropertyLine(0).Width, this.PropertyLine(0).Cap, this.PropertyLine(0).Join);
			graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(1).Color));

			if ( this.IsHilite && iconContext.IsEditable )
			{
				path = new Drawing.Path();
				path.MoveTo(this.Handle(0).Position);
				path.LineTo(this.Handle(1).Position);

				path.MoveTo(this.ComputeExtremity(0));
				path.LineTo(this.Handle(1).Position);
				path.LineTo(this.ComputeExtremity(1));

				graphics.Rasterizer.AddOutline(path, this.PropertyLine(0).Width+iconContext.HiliteSize, this.PropertyLine(0).Cap, this.PropertyLine(0).Join);
				graphics.RenderSolid(iconContext.HiliteColor);
			}
		}


		protected double			distPara = 10;
		protected double			distPerp = 5;
	}
}
