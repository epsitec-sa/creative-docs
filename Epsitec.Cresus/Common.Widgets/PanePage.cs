namespace Epsitec.Common.Widgets
{
	using BundleAttribute  = Support.BundleAttribute;
	
	/// <summary>
	/// La classe PanePage repr�sente une page du PaneBook.
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

		
		public double							AbsoluteOrder
		{
			get
			{
				double ret = this.paneAbsoluteOrder;
				this.paneAbsoluteOrder = System.Double.NaN;
				return ret;
			}
		}
		
		public double							AbsoluteSize
		{
			get
			{
				return this.paneAbsoluteSize;
			}
			set
			{
				this.paneAbsoluteSize = value;
			}
		}
		
		[Bundle]	public double				PaneRelativeSize
		{
			// Largeur ou hauteur relative du panneau.
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

		[Bundle]	public double				PaneAbsoluteSize
		{
			// Largeur ou hauteur absolue (en points) du panneau.
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

		[Bundle]	public double				PaneMinSize
		{
			// Largeur ou hauteur minimale du panneau en points.
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

		[Bundle]	public double				PaneMaxSize
		{
			// Largeur ou hauteur maximale du panneau en points.
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

		[Bundle]	public double				PaneHideSize
		{
			// Largeur ou hauteur maximale en dessous de laquelle le contenu est cach�.
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

		[Bundle]	public double				PaneElasticity
		{
			// Elasticit� du panneau (0=fixe, 1=�lastique).
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

		[Bundle]	public bool					PaneToggle
		{
			// Mode du panneau.
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

		public PaneButton						PaneButton
		{
			// Retourne le bouton associ�.
			get
			{
				return this.paneButton;
			}
		}
		
		public GlyphButton						GlyphButton
		{
			// Retourne le bouton associ�.
			get
			{
				return this.glyphButton;
			}
		}
		
		public PaneBook							Book
		{
			// Retourne le PaneBook parent.
			get
			{
				PaneBook book = this.Parent as PaneBook;
				return book;
			}
		}

		public bool								IsDirty
		{
			// Indique si le PanePage doit �tre recalcul�.
			get
			{
				return this.isDirty;
			}

			set
			{
				this.isDirty = value;
			}
		}
		
		public int								Rank
		{
			// Rang facultatif du panneau.
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

		
		public event System.EventHandler		RankChanged;
		
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
