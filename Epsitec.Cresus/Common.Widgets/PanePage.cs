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
		}
		
		public PanePage(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		// Largeur ou hauteur relative du panneau.
		public double PaneRelativeSize
		{
			get
			{
				return this.paneRelativeSize;
			}

			set
			{
				this.paneRelativeSize = value;
			}
		}

		// Largeur ou hauteur minimale du panneau en points.
		public double PaneMinSize
		{
			get
			{
				return this.paneMinSize;
			}

			set
			{
				this.paneMinSize = value;
			}
		}

		// Largeur ou hauteur maximale du panneau en points.
		public double PaneMaxSize
		{
			get
			{
				return this.paneMaxSize;
			}

			set
			{
				this.paneMaxSize = value;
			}
		}

		// Elasticité du panneau (0=fixe, 1=elastique).
		public double PaneElasticity
		{
			get
			{
				return this.paneElasticity;
			}

			set
			{
				this.paneElasticity = value;
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
		
		// Retourne le PaneBook parent.
		public PaneBook Book
		{
			get
			{
				PaneBook book = this.Parent as PaneBook;
				return book;
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
		

		protected int					rank;
		protected double				paneRelativeSize = 0;
		protected double				paneMinSize = 0;
		protected double				paneMaxSize = 1000000;
		protected double				paneElasticity = 1;
		protected PaneButton			paneButton;
	}
}
