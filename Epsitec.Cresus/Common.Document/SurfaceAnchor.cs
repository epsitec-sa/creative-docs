using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	public class SurfaceAnchor
	{
		// Constructeur d'une surface li�e � un document.
		public SurfaceAnchor(Document document, Objects.Abstract obj)
		{
			this.document = document;
			this.obj = obj;
			this.dirty = true;
			this.surfaceThin = Rectangle.Empty;
			this.surfaceGeom = Rectangle.Empty;
			this.lineUse = false;
		}

		// Constructeur d'une surface simple rectangulaire ind�pendante d'un objet.
		public SurfaceAnchor(Rectangle box)
		{
			this.document = null;
			this.obj = null;
			this.dirty = false;
			this.surfaceThin = box;
			this.surfaceGeom = box;
			this.lineUse = false;
		}

		// Force la surface � utiliser.
		public void SetSurface(Rectangle rect)
		{
			this.surfaceThin = rect;
			this.surfaceGeom = rect;
			this.dirty = false;
		}


		// Indique que l'objet a �t� modifi�.
		public void SetDirty()
		{
			this.dirty = true;
		}

		// Indique s'il faut tenir compte de l'�paisseur du trait lors
		// des conversions ToAbs et ToRel.
		public bool LineUse
		{
			get
			{
				return this.lineUse;
			}

			set
			{
				this.lineUse = value;
			}
		}

		// Indique si la surface est nulle.
		public bool IsSurfaceZero
		{
			get
			{
				this.Update();
				return this.Surface.IsSurfaceZero;
			}
		}

		// Retourne le centre de la surface.
		public Point Center
		{
			get
			{
				return this.ToAbs(new Point(0.5, 0.5));
			}
		}

		// Retourne la largeur.
		public double Width
		{
			get
			{
				this.Update();
				return this.Surface.Width;
			}
		}

		// Retourne la hauteur.
		public double Height
		{
			get
			{
				this.Update();
				return this.Surface.Height;
			}
		}

		// Retourne la direction de l'objet li�.
		public double Direction
		{
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

		// Retourne le rayon � utiliser pour la poign�e des rotations.
		public double RotateRadius
		{
			get
			{
				this.Update();
				return System.Math.Min(this.Surface.Width, this.Surface.Height)*0.45;
			}
		}

		// Retourne la bbox rectangulaire qui inclu les 4 coins.
		public Rectangle BoundingBox
		{
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

		// Conversion d'une coordonn�e relative en coordonn�e absolue.
		public Point ToAbs(Point rel)
		{
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

		// Conversion d'une coordonn�e absolue en coordonn�e relative.
		public Point ToRel(Point abs)
		{
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

		// Retourne la surface rectangulaire � utiliser.
		protected Rectangle Surface
		{
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

		// Met � jour les surfaces en fonction de l'objet.
		protected void Update()
		{
			if ( !this.dirty || this.obj == null )  return;

			this.obj.UpdateSurfaceBox(out this.surfaceThin, out this.surfaceGeom);
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
