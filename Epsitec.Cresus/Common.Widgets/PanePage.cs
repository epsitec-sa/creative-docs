namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe PanePage représente une page du PaneBook.
	/// </summary>
	public class PanePage : AbstractGroup
	{
		public PanePage()
		{
			this.paneButton = new PaneButton(null);
			this.paneButton.Alignment = Drawing.ContentAlignment.MiddleCenter;

			this.glyphButton = new GlyphButton(this);
			this.glyphButton.ButtonStyle = ButtonStyle.Icon;
			this.glyphButton.Hide();
		}
		
		public PanePage(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		// Largeur ou hauteur relative du panneau.
		[ Support.Bundle ("rs") ] public double PaneRelativeSize
		{
			get
			{
				return this.paneRelativeSize;
			}

			set
			{
				if ( this.paneRelativeSize != value )
				{
					this.paneRelativeSize = value;
					this.isDirty = true;
				}
			}
		}

		// Largeur ou hauteur absolue (en points) du panneau.
		[ Support.Bundle ("as") ] public double PaneAbsoluteSize
		{
			get
			{
				return this.paneAbsoluteSize;
			}

			set
			{
				if ( this.paneAbsoluteSize != value )
				{
					this.paneAbsoluteSize = value;
					this.paneAbsoluteOrder = value;
					this.isDirty = true;
				}
			}
		}

		public double GetAbsoluteOrder()
		{
			double ret = this.paneAbsoluteOrder;
			this.paneAbsoluteOrder = System.Double.NaN;
			return ret;
		}

		public void SetAbsoluteSize(double value)
		{
			this.paneAbsoluteSize = value;
		}

		// Largeur ou hauteur minimale du panneau en points.
		[ Support.Bundle ("min") ] public double PaneMinSize
		{
			get
			{
				return this.paneMinSize;
			}

			set
			{
				if ( this.paneMinSize != value )
				{
					this.paneMinSize = value;
					this.isDirty = true;
				}
			}
		}

		// Largeur ou hauteur maximale du panneau en points.
		[ Support.Bundle ("max") ] public double PaneMaxSize
		{
			get
			{
				return this.paneMaxSize;
			}

			set
			{
				if ( this.paneMaxSize != value )
				{
					this.paneMaxSize = value;
					this.isDirty = true;
				}
			}
		}

		// Largeur ou hauteur maximale en dessous de laquelle le contenu est caché.
		[ Support.Bundle ("hs") ] public double PaneHideSize
		{
			get
			{
				return this.paneHideSize;
			}

			set
			{
				if ( this.paneHideSize != value )
				{
					this.paneHideSize = value;
					this.isDirty = true;
				}
			}
		}

		// Elasticité du panneau (0=fixe, 1=élastique).
		[ Support.Bundle ("e") ] public double PaneElasticity
		{
			get
			{
				return this.paneElasticity;
			}

			set
			{
				if ( this.paneElasticity != value )
				{
					this.paneElasticity = value;
					this.isDirty = true;
				}
			}
		}

		// Mode du panneau.
		[ Support.Bundle ("toggle") ] public bool PaneToggle
		{
			get
			{
				return this.paneToggle;
			}

			set
			{
				if ( this.paneToggle != value )
				{
					this.paneToggle = value;
					this.isDirty = true;
				}
			}
		}

		// Retourne le bouton associé.
		public PaneButton PaneButton
		{
			get
			{
				return this.paneButton;
			}
		}
		
		// Retourne le bouton associé.
		public GlyphButton GlyphButton
		{
			get
			{
				return this.glyphButton;
			}
		}
		
		// Retourne le PaneBook parent.
		public PaneBook Book
		{
			get
			{
				PaneBook book = this.Parent as PaneBook;
				return book;
			}
		}

		// Indique si le PanePage doit être recalculé.
		public bool IsDirty
		{
			get
			{
				return this.isDirty;
			}

			set
			{
				this.isDirty = value;
			}
		}
		
		// Rang facultatif du panneau.
		public int Rank
		{
			get
			{
				return this.rank;
			}

			set
			{
				if ( this.rank != value )
				{
					this.rank = value;
					this.OnRankChanged(System.EventArgs.Empty);
				}
			}
		}

		
		public event System.EventHandler RankChanged;
		
		protected virtual void OnRankChanged(System.EventArgs e)
		{
			if ( this.RankChanged != null )
			{
				this.RankChanged(this, e);
			}
		}
		

		protected int						rank;
		protected double					paneRelativeSize = 0;
		protected double					paneAbsoluteSize = 0;
		protected double					paneAbsoluteOrder = System.Double.NaN;
		protected double					paneMinSize = 0;
		protected double					paneMaxSize = 1000000;
		protected double					paneHideSize = 0;
		protected double					paneElasticity = 1;
		protected bool						paneToggle;
		protected PaneButton				paneButton;
		protected GlyphButton				glyphButton;
		protected bool						isDirty = true;
	}
}
