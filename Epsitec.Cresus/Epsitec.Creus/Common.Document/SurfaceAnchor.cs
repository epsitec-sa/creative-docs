using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	public class SurfaceAnchor
	{
		public SurfaceAnchor(Document document, Objects.Abstract obj)
		{
			//	Constructeur d'une surface liée à un document.
			this.document = document;
			this.obj = obj;
			this.dirty = true;
			this.surfaceThin = Rectangle.Empty;
			this.surfaceGeom = Rectangle.Empty;
			this.lineUse = false;
		}

		public SurfaceAnchor(Rectangle box)
		{
			//	Constructeur d'une surface simple rectangulaire indépendante d'un objet.
			this.document = null;
			this.obj = null;
			this.dirty = false;
			this.surfaceThin = box;
			this.surfaceGeom = box;
			this.lineUse = false;
		}

		public void SetSurface(Rectangle rect)
		{
			//	Force la surface à utiliser.
			this.surfaceThin = rect;
			this.surfaceGeom = rect;
			this.dirty = false;
		}


		public void Move(Point move)
		{
			//	Déplace la surface.
			if ( this.dirty )  return;

			this.surfaceThin.Offset(move);
			this.surfaceGeom.Offset(move);
		}

		public void SetDirty()
		{
			//	Indique que l'objet a été modifié.
			this.dirty = true;
		}

		public bool LineUse
		{
			//	Indique s'il faut tenir compte de l'épaisseur du trait lors
			//	des conversions ToAbs et ToRel.
			get
			{
				return this.lineUse;
			}

			set
			{
				this.lineUse = value;
			}
		}

		public bool IsSurfaceZero
		{
			//	Indique si la surface est nulle.
			get
			{
				this.Update();
				return this.Surface.IsSurfaceZero;
			}
		}

		public Point Center
		{
			//	Retourne le centre de la surface.
			get
			{
				return this.ToAbs(new Point(0.5, 0.5));
			}
		}

		public double Width
		{
			//	Retourne la largeur.
			get
			{
				this.Update();
				return this.Surface.Width;
			}
		}

		public double Height
		{
			//	Retourne la hauteur.
			get
			{
				this.Update();
				return this.Surface.Height;
			}
		}

		public double Direction
		{
			//	Retourne la direction de l'objet lié.
			get
			{
				if ( this.obj == null )
				{
					return 0.0;
				}
				else
				{
					return obj.Direction;
				}
			}
		}

		public double RotateRadius
		{
			//	Retourne le rayon à utiliser pour la poignée des rotations.
			get
			{
				this.Update();
				return System.Math.Min(this.Surface.Width, this.Surface.Height)*0.45;
			}
		}

		public Rectangle BoundingBox
		{
			//	Retourne la bbox rectangulaire qui inclu les 4 coins.
			get
			{
				Rectangle box = Rectangle.Empty;
				box.MergeWith(this.ToAbs(new Point(0.0, 0.0)));
				box.MergeWith(this.ToAbs(new Point(1.0, 0.0)));
				box.MergeWith(this.ToAbs(new Point(0.0, 1.0)));
				box.MergeWith(this.ToAbs(new Point(1.0, 1.0)));
				return box;
			}
		}

		public Point ToAbs(Point rel)
		{
			//	Conversion d'une coordonnée relative en coordonnée absolue.
			this.Update();

			Rectangle surface = this.Surface;
			rel.X = surface.Left   + surface.Width*rel.X;
			rel.Y = surface.Bottom + surface.Height*rel.Y;

			if ( this.obj == null )
			{
				return rel;
			}
			else
			{
				return Transform.RotatePointDeg(this.obj.Direction, rel);
			}
		}

		public Point ToRel(Point abs)
		{
			//	Conversion d'une coordonnée absolue en coordonnée relative.
			this.Update();

			if ( this.obj != null )
			{
				abs = Transform.RotatePointDeg(-this.obj.Direction, abs);
			}

			Rectangle surface = this.Surface;

			if ( surface.Width == 0.0 )
			{
				abs.X = 0.0;
			}
			else
			{
				abs.X = (abs.X-surface.Left)/surface.Width;
			}

			if ( surface.Height == 0.0 )
			{
				abs.Y = 0.0;
			}
			else
			{
				abs.Y = (abs.Y-surface.Bottom)/surface.Height;
			}

			return abs;
		}

		protected Rectangle Surface
		{
			//	Retourne la surface rectangulaire à utiliser.
			get
			{
				if ( this.lineUse )
				{
					return this.surfaceGeom;
				}
				else
				{
					return this.surfaceThin;
				}
			}
		}

		protected void Update()
		{
			//	Met à jour les surfaces en fonction de l'objet.
			if ( !this.dirty || this.obj == null )  return;

			this.obj.UpdateSurfaceBox(out this.surfaceThin, out this.surfaceGeom);
			//?System.Diagnostics.Debug.Assert(!this.surfaceThin.IsEmpty);
			this.dirty = false;
		}


		protected Document				document;
		protected Objects.Abstract		obj;
		protected bool					dirty;
		protected bool					lineUse;
		protected Rectangle				surfaceThin;
		protected Rectangle				surfaceGeom;
	}
}
