namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractRuler implémente la classe de base des règles
	/// HRuler et VRuler.
	/// </summary>
	[Support.SuppressBundleSupport]
	public abstract class AbstractRuler : Widget
	{
		protected AbstractRuler(bool vertical)
		{
			this.isVertical = vertical;
		}

		protected AbstractRuler(Widget embedder, bool vertical) : this(vertical)
		{
			this.SetEmbedder(embedder);
		}


		public double							PPM
		{
			// Nombre de points/millimètres pour la graduation.
			get
			{
				return this.ppm;
			}

			set
			{
				if ( this.ppm != value )
				{
					this.ppm = value;
					this.Invalidate();
				}
			}
		}

		public double							Starting
		{
			// Début de la graduation.
			get
			{
				return this.starting;
			}

			set
			{
				if ( this.starting != value )
				{
					this.starting = value;
					this.Invalidate();
				}
			}
		}

		public double							Ending
		{
			// Fin de la graduation.
			get
			{
				return this.ending;
			}

			set
			{
				if ( this.ending != value )
				{
					this.ending = value;
					this.Invalidate();
				}
			}
		}

		public double							Marker
		{
			// Position du marqueur.
			get
			{
				return this.marker;
			}

			set
			{
				if ( this.marker != value )
				{
					this.marker = value;
					this.Invalidate();
				}
			}
		}

		public bool								MarkerVisible
		{
			// Visibilité du marqueur.
			get
			{
				return this.markerVisible;
			}

			set
			{
				if ( this.markerVisible != value )
				{
					this.markerVisible = value;
					this.Invalidate();
				}
			}
		}


		protected static readonly double	defaultBreadth = 11;

		private bool						isVertical;
		protected double					ppm = 10.0;
		protected double					starting = 0.0;
		protected double					ending = 100.0;
		protected double					marker = 0.0;
		protected bool						markerVisible = false;
	}
}
