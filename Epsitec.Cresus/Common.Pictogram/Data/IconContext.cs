using Epsitec.Common.Widgets;

namespace Epsitec.Common.Pictogram.Data
{
	public enum ConstrainType
	{
		None,			// aucune
		Normal,			// horizontal, vertical et 45 degrés
		Square,			// uniquement 45 degrés
		Line,			// uniquement horizontal et vertical
	}

	/// <summary>
	/// La classe IconContext contient le "device contexte" pour les icônes.
	/// </summary>
	public class IconContext
	{
		public IconContext()
		{
		}

		public IAdorner Adorner
		{
			get { return this.adorner; }
			set { this.adorner = value; }
		}

		public Drawing.Color UniqueColor
		{
			get { return this.uniqueColor; }
			set { this.uniqueColor = value; }
		}

		public double ScaleX
		{
			get { return this.scaleX; }
			set { this.scaleX = value; }
		}

		public double ScaleY
		{
			get { return this.scaleY; }
			set { this.scaleY = value; }
		}

		public double Zoom
		{
			get { return this.zoom; }
			set { this.zoom = value; }
		}

		public double OriginX
		{
			get { return this.originX; }
			set { this.originX = value; }
		}

		public double OriginY
		{
			get { return this.originY; }
			set { this.originY = value; }
		}

		// Indique si l'objet Drawer est éditable.
		public bool IsEditable
		{
			get { return this.isEditable; }
			set { this.isEditable = value; }
		}

		// Indique si l'icône est estompée.
		public bool IsDimmed
		{
			get { return this.isDimmed; }
			set { this.isDimmed = value; }
		}

		// Taille minimale que doit avoir un objet à sa création.
		public double MinimalSize
		{
			get { return this.minimalSize/this.scaleX; }
		}

		// Epaisseur minimale d'un objet pour la détection du coutour.
		public double MinimalWidth
		{
			get { return this.minimalWidth/this.scaleX; }
		}

		// Marge pour fermer un polygone.
		public double CloseMargin
		{
			get { return this.closeMargin/this.scaleX; }
		}

		// Taille supplémentaire lorsqu'un objet est survolé par la souris.
		public double HiliteSize
		{
			get { return this.hiliteSize/this.scaleX; }
		}

		// Taille d'une poignée.
		public double HandleSize
		{
			get { return this.handleSize/this.scaleX; }
		}

		// Adapte une couleur en fonction de l'état de l'icône.
		public Drawing.Color AdaptColor(Drawing.Color color)
		{
			if ( !this.uniqueColor.IsEmpty )  // desabled (n/b) ?
			{
				if ( this.adorner != null )
				{
					this.adorner.AdaptDisabledTextColor(ref color, this.uniqueColor);
				}
			}
			if ( this.isDimmed )  // estompé (hors groupe) ?
			{
				double alpha = color.A;
				double intensity = color.GetBrightness ();
				intensity = 0.5+(intensity-0.5)*0.05;  // diminue le contraste
				intensity = System.Math.Min(intensity+0.1, 1.0);  // augmente l'intensité
				color = Drawing.Color.FromBrightness(intensity);
				color.A = alpha*0.2;  // très transparent
			}
			return color;
		}

		// Couleur lorsqu'un objet est survolé par la souris.
		public Drawing.Color HiliteColor
		{
#if false
			get { return Drawing.Color.FromARGB(0.3, 1,1,0); }
#else
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;
				Drawing.Color color = adorner.ColorCaption;
				return Drawing.Color.FromARGB(0.8, color.R, color.G, color.B);
			}
#endif
		}


		// Indique si la touche Ctrl est pressés.
		public bool IsCtrl
		{
			get { return this.isCtrl; }
			set { this.isCtrl = value; }
		}

		// Fixe le point initial pour les contraintes.
		public void ConstrainFixStarting(Drawing.Point pos)
		{
			this.constrainStarting = pos;
			this.constrainType = ConstrainType.Normal;
		}

		// Fixe le point initial pour les contraintes.
		public void ConstrainFixStarting(Drawing.Point pos, ConstrainType type)
		{
			this.constrainStarting = pos;
			this.constrainType = type;
		}

		// Retourne une position éventuellement contrainte.
		public void ConstrainSnapPos(ref Drawing.Point pos)
		{
			if ( this.constrainType == ConstrainType.None || !this.isCtrl )  return;

			if ( this.constrainType == ConstrainType.Normal )
			{
				double angle = Drawing.Point.ComputeAngle(this.constrainStarting, pos);
				double dist = Drawing.Point.Distance(pos, this.constrainStarting);
				angle = System.Math.Floor((angle+System.Math.PI/8)/(System.Math.PI/4))*(System.Math.PI/4);
				pos = Drawing.Transform.RotatePoint(this.constrainStarting, angle, this.constrainStarting+new Drawing.Point(dist,0));
			}

			if ( this.constrainType == ConstrainType.Square )
			{
				double angle = Drawing.Point.ComputeAngle(this.constrainStarting, pos);
				double dist = Drawing.Point.Distance(pos, this.constrainStarting);
				angle += System.Math.PI/4;
				angle = System.Math.Floor((angle+System.Math.PI/4)/(System.Math.PI/2))*(System.Math.PI/2);
				angle -= System.Math.PI/4;
				pos = Drawing.Transform.RotatePoint(this.constrainStarting, angle, this.constrainStarting+new Drawing.Point(dist,0));
			}

			if ( this.constrainType == ConstrainType.Line )
			{
				if ( System.Math.Abs(pos.X-this.constrainStarting.X) < System.Math.Abs(pos.Y-this.constrainStarting.Y) )
				{
					pos.X = this.constrainStarting.X;
				}
				else
				{
					pos.Y = this.constrainStarting.Y;
				}
			}
		}

		// Enlève le point initial pour les contraintes.
		public void ConstrainDelStarting()
		{
			this.constrainType = ConstrainType.None;
		}

		// Donne le point de contrainte initial s'il est en vigeur.
		public bool ConstrainGetStarting(out Drawing.Point pos, out ConstrainType type)
		{
			pos = this.constrainStarting;
			type = this.constrainType;
			return ( this.constrainType != ConstrainType.None && this.isCtrl );
		}


		protected IAdorner			adorner;
		protected Drawing.Color		uniqueColor;
		protected double			scaleX = 1;
		protected double			scaleY = 1;
		protected double			zoom = 1;
		protected double			originX = 0;
		protected double			originY = 0;
		protected bool				isEditable = false;
		protected bool				isDimmed = false;
		protected double			minimalSize = 3;
		protected double			minimalWidth = 5;
		protected double			closeMargin = 10;
		protected double			hiliteSize = 6;
		protected double			handleSize = 10;
		protected bool				isCtrl = false;
		protected Drawing.Point		constrainStarting;
		protected ConstrainType		constrainType;
	}
}
