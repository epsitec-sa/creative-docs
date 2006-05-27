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
