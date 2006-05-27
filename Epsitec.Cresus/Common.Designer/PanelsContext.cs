using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Contient le contexte commun à tous les Viewers.
	/// </summary>
	public class PanelsContext
	{
		public PanelsContext()
		{
		}


		public string Tool
		{
			get
			{
				return this.tool;
			}
			set
			{
				this.tool = value;
			}
		}

		public bool ShowGrid
		{
			get
			{
				return this.showGrid;
			}
			set
			{
				this.showGrid = value;
			}
		}

		public bool ShowConstrain
		{
			get
			{
				return this.showConstrain;
			}
			set
			{
				this.showConstrain = value;
			}
		}

		public bool ShowAnchor
		{
			get
			{
				return this.showAnchor;
			}
			set
			{
				this.showAnchor = value;
			}
		}

		public bool ShowExpand
		{
			get
			{
				return this.showExpand;
			}
			set
			{
				this.showExpand = value;
			}
		}

		public bool ShowZOrder
		{
			get
			{
				return this.showZOrder;
			}
			set
			{
				this.showZOrder = value;

				if (this.showZOrder)
				{
					this.showTabIndex = false;
				}
			}
		}

		public bool ShowTabIndex
		{
			get
			{
				return this.showTabIndex;
			}
			set
			{
				this.showTabIndex = value;

				if (this.showTabIndex)
				{
					this.showZOrder = false;
				}
			}
		}

		public double GridStep
		{
			get
			{
				return this.gridStep;
			}
			set
			{
				this.gridStep = value;
			}
		}

		public double ConstrainMargin
		{
			get
			{
				return this.constrainMargin;
			}
			set
			{
				this.constrainMargin = value;
			}
		}

		public Size ConstrainSpacing
		{
			get
			{
				return this.constrainSpacing;
			}
			set
			{
				this.constrainSpacing = value;
			}
		}

		public double Leading
		{
			get
			{
				return this.leading;
			}
			set
			{
				this.leading = value;
			}
		}

		public Margins ConstrainGroupMargins
		{
			get
			{
				return this.constrainGroupMargins;
			}
			set
			{
				this.constrainGroupMargins = value;
			}
		}


		#region Static colors
		static public Color HiliteOutlineColor
		{
			//	Couleur lorsqu'un objet est survolé par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return Color.FromColor(adorner.ColorCaption, 0.8);
			}
		}

		static public Color HiliteSurfaceColor
		{
			//	Couleur lorsqu'un objet est survolé par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return Color.FromColor(adorner.ColorCaption, 0.4);
			}
		}

		static public Color ColorOutsurface
		{
			//	Couleur pour la surface hors du panneau.
			get
			{
				return Color.FromAlphaRgb(0.2, 0.5, 0.5, 0.5);
			}
		}

		static public Color ColorZOrder
		{
			//	Couleur pour les chiffres du ZOrder.
			get
			{
				return Color.FromRgb(1, 0, 0);
			}
		}

		static public Color ColorTabIndex
		{
			//	Couleur pour les chiffres de l'ordre pour la touche Tab.
			get
			{
				return Color.FromRgb(0, 0, 1);
			}
		}

		static public Color ColorAnchor
		{
			//	Couleur pour représenter un ancrage.
			get
			{
				return Color.FromRgb(0, 0, 1);
			}
		}

		static public Color ColorGrid1
		{
			//	Couleur pour la grille magnétique primaire (division principale).
			get
			{
				return Color.FromAlphaRgb(0.2, 0.4, 0.4, 0.4);
			}
		}

		static public Color ColorGrid2
		{
			//	Couleur pour la grille magnétique secondaire (subdivision).
			get
			{
				return Color.FromAlphaRgb(0.2, 0.7, 0.7, 0.7);
			}
		}
		#endregion


		protected string				tool = "ToolSelect";
		protected bool					showGrid = false;
		protected bool					showConstrain = true;
		protected bool					showAnchor = true;
		protected bool					showExpand = false;
		protected bool					showZOrder = false;
		protected bool					showTabIndex = false;
		protected double				gridStep = 10;
		protected double				constrainMargin = 5;
		protected Size					constrainSpacing = new Size(10, 5);
		protected double				leading = 30;
		protected Margins				constrainGroupMargins = new Margins(12, 12, 12+7, 12);
	}
}
