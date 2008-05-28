using System.Collections.Generic;
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
		public PanelsContext(DesignerApplication designerApplication)
		{
			this.designerApplication = designerApplication;
			this.extendedProxies = new Dictionary<string, bool>();
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

		public bool ShowAttachment
		{
			get
			{
				return this.showAttachment;
			}
			set
			{
				this.showAttachment = value;
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

		public double MinimalSize
		{
			get
			{
				return this.minimalSize;
			}
			set
			{
				this.minimalSize = value;
			}
		}

		public double GroupOutline
		{
			get
			{
				return this.groupOutline;
			}
			set
			{
				this.groupOutline = value;
			}
		}

		public double ZOrderThickness
		{
			get
			{
				return this.zOrderThickness;
			}
			set
			{
				this.zOrderThickness = value;
			}
		}

		public double SizeMarkThickness
		{
			get
			{
				return this.sizeMarkThickness;
			}
			set
			{
				this.sizeMarkThickness = value;
			}
		}

		public double DockedTriangleThickness
		{
			get
			{
				return this.dockedTriangleThickness;
			}
			set
			{
				this.dockedTriangleThickness = value;
			}
		}

		public double DockedTriangleLength
		{
			get
			{
				return this.dockedTriangleLength;
			}
			set
			{
				this.dockedTriangleLength = value;
			}
		}


		#region ExtendedProxies
		public bool IsExtendedProxies(string name)
		{
			//	Indique si un panneau pour un proxy est étendu ou non.
			bool defaultState;
			if (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Window)
			{
				name = string.Concat("W.", name);
				defaultState = true;
			}
			else
			{
				name = string.Concat("N.", name);
				defaultState = false;
			}

			if (!this.extendedProxies.ContainsKey(name))
			{
				this.extendedProxies.Add(name, defaultState);
			}

			return this.extendedProxies[name];
		}

		public void SetExtendedProxies(string name, bool extended)
		{
			//	Modifie l'état étendu ou non d'un panneau pour un proxy.
			if (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Window)
			{
				name = string.Concat("W.", name);
			}
			else
			{
				name = string.Concat("N.", name);
			}

			if (!this.extendedProxies.ContainsKey(name))
			{
				this.extendedProxies.Add(name, false);
			}

			this.extendedProxies[name] = extended;
		}
		#endregion


		#region Static colors
		static public Color ColorHiliteOutline
		{
			//	Couleur lorsqu'un objet est survolé par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return Color.FromColor(adorner.ColorCaption, 0.8);
			}
		}

		static public Color ColorHiliteSurface
		{
			//	Couleur lorsqu'un objet est survolé par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return Color.FromColor(adorner.ColorCaption, 0.4);
			}
		}

		static public Color ColorHiliteParent
		{
			//	Couleur lorsqu'un objet parent est survolé par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return adorner.ColorCaption;
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

		static public Color ColorGridCellOutline
		{
			//	Couleur pour représenter une mise en évidence de cellule.
			get
			{
				return Color.FromRgb(0.0/255.0, 0.0/255.0, 0.0/255.0);
			}
		}

		static public Color ColorGridCellSurface
		{
			//	Couleur pour représenter une mise en évidence de cellule.
			get
			{
				return Color.FromAlphaRgb(0.2, 255.0/255.0, 255.0/255.0, 0.0/255.0);
			}
		}

		static public Color ColorAttachment
		{
			//	Couleur pour représenter un ressort.
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

		static public Color ColorHandleNormal
		{
			//	Couleur pour une poignée.
			get
			{
				return Color.FromRgb(1, 0, 0);
			}
		}

		static public Color ColorHandleHilited
		{
			//	Couleur lorsqu'un objet est survolé par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return adorner.ColorCaption;
			}
		}

		static public Color ColorSizeMarkLight
		{
			//	Couleur pour un marqueur de taille préférentielle.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return Color.FromColor(adorner.ColorCaption, 0.2);
			}
		}

		static public Color ColorSizeMarkDark
		{
			//	Couleur pour un marqueur de taille préférentielle.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return Color.FromColor(adorner.ColorCaption, 0.5);
			}
		}

		static public Color ColorSizeMarkLine
		{
			//	Couleur pour un marqueur de taille préférentielle.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return adorner.ColorCaption;
			}
		}

		static public Color ColorOutsideForeground
		{
			//	Couleur superposée à une DragWindow lorsqu'on est hors de la fenêtre.
			get
			{
				return Color.FromAlphaRgb(0.3, 1,0,0);
			}
		}
		#endregion


		protected DesignerApplication		designerApplication;
		protected string					tool = "ToolSelect";
		protected bool						showGrid = false;
		protected bool						showConstrain = true;
		protected bool						showAttachment = true;
		protected bool						showExpand = false;
		protected bool						showZOrder = false;
		protected bool						showTabIndex = false;
		protected double					gridStep = 10;
		protected double					constrainMargin = 5;
		protected Size						constrainSpacing = new Size(10, 5);
		protected Margins					constrainGroupMargins = new Margins(12, 12, 12+7, 12);
		protected double					leading = 30;
		protected double					minimalSize = 3;
		protected double					groupOutline = 5;
		protected double					zOrderThickness = 2;
		protected double					sizeMarkThickness = 8;
		protected double					dockedTriangleThickness = 10;
		protected double					dockedTriangleLength = 10;
		protected Dictionary<string, bool>	extendedProxies;	
	}
}
