//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public class ContinuousController
	{
		public ContinuousController(AbstractEntityPrinter entityPrinter)
		{
			System.Diagnostics.Debug.Assert (entityPrinter != null);
			this.entityPrinter = entityPrinter;

			this.zoom = 1;
		}

		public void CreateUI(Widget parent)
		{
			System.Diagnostics.Debug.Assert (parent != null);
			this.parent = parent;

			double scrollBreadth = 20;

			var main = new FrameBox ()
			{
				Parent = this.parent,
				Dock   = DockStyle.Fill,
			};

			var footer = new FrameBox ()
			{
				Parent  = this.parent,
				Dock    = DockStyle.Bottom,
				Padding = new Margins (0, scrollBreadth, 0, 0),
			};

			//	Crée le visualisateur de page ContinuousPagePreviewer.
			this.previewer = new Widgets.ContinuousPagePreviewer ()
			{
				Parent          = main,
				DocumentPrinter = this.entityPrinter.GetDocumentPrinter (0),
				Dock            = DockStyle.Fill,
				Margins         = new Margins (0, 1, 0, 1),
			};

			//	Crée l'ascenseur vertical.
			this.vScroller = new VScroller ()
			{
				Parent         = main,
				PreferredWidth = scrollBreadth,
				Dock           = DockStyle.Right,
			};

			//	Crée les boutons dans le pied de page.
			var toolbar = UIBuilder.CreateMiniToolbar (footer);
			toolbar.PreferredWidth = 10;  // sera étendu
			toolbar.PreferredHeight = scrollBreadth;
			toolbar.Margins = new Margins (0, 2, 0, 0);
			toolbar.Dock = DockStyle.Left;

			this.zoom1Button = new Button
			{
				Parent          = toolbar,
				ButtonStyle     = ButtonStyle.ToolItem,
				AutoFocus       = false,
				Text            = "×1",
				PreferredWidth  = 20,
				PreferredHeight = scrollBreadth-4,
				Dock            = DockStyle.Left,
			};

			this.zoom2Button = new Button
			{
				Parent          = toolbar,
				ButtonStyle     = ButtonStyle.ToolItem,
				AutoFocus       = false,
				Text            = "×2",
				PreferredWidth  = 20,
				PreferredHeight = scrollBreadth-4,
				Dock            = DockStyle.Left,
			};

			ToolTip.Default.SetToolTip (this.zoom1Button, "Visualise toute la largeur");
			ToolTip.Default.SetToolTip (this.zoom2Button, "Agrandissement ×2");

			//	Crée l'ascenseur horizontal dans le pied de page.
			this.hScroller = new HScroller ()
			{
				Parent          = footer,
				PreferredHeight = scrollBreadth,
				Dock            = DockStyle.Fill,
			};

			//	Connecte les événements.
			this.previewer.SizeChanged += delegate
			{
				this.UpdateScroller ();
			};

			this.vScroller.ValueChanged += delegate
			{
				this.UpdatePagePreview ();
			};

			this.hScroller.ValueChanged += delegate
			{
				this.UpdatePagePreview ();
			};

			this.zoom1Button.Clicked += delegate
			{
				this.ChangeZoom (1);
			};

			this.zoom2Button.Clicked += delegate
			{
				this.ChangeZoom (2);
			};

			this.UpdateScroller ();
			this.UpdateButtons ();
		}


		private void ChangeZoom(double zoom)
		{
			if (this.zoom != zoom)
			{
				this.zoom = zoom;
				this.previewer.Zoom = zoom;
			}

			this.UpdateScroller ();
			this.UpdateButtons ();
		}

		private void UpdateScroller()
		{
			double totalHeight   = this.previewer.TotalHeight;
			double visibleHeight = this.previewer.Client.Size.Height;
			double maxHeight     = System.Math.Max (totalHeight-visibleHeight, 0);

			if (maxHeight == 0)
			{
				this.vScroller.MaxValue = 0;
				this.vScroller.Value    = 0;
			}
			else
			{
				this.vScroller.MaxValue          = (decimal) maxHeight;
				this.vScroller.Value             = (decimal) maxHeight;
				this.vScroller.VisibleRangeRatio = (decimal) (visibleHeight/totalHeight);
				this.vScroller.SmallChange       = (decimal) 10;
				this.vScroller.LargeChange       = (decimal) (visibleHeight*0.5);
			}

			double totalWidth   = this.previewer.TotalWidth;
			double visibleWidth = this.previewer.Client.Size.Width;
			double maxWidth     = System.Math.Max (totalWidth-visibleWidth, 0);

			if (zoom == 1)
			{
				this.hScroller.MaxValue = 0;
				this.hScroller.Value    = 0;
			}
			else
			{
				this.hScroller.MaxValue          = (decimal) maxWidth;
				this.hScroller.Value             = (decimal) 0;
				this.hScroller.VisibleRangeRatio = (decimal) (visibleWidth/totalWidth);
				this.hScroller.SmallChange       = (decimal) 10;
				this.hScroller.LargeChange       = (decimal) (visibleWidth*0.5);
			}
		}

		private void UpdatePagePreview()
		{
			this.previewer.VerticalOffset   = (double) this.vScroller.Value;
			this.previewer.HorizontalOffset = (double) this.hScroller.Value;
		}

		private void UpdateButtons()
		{
			this.zoom1Button.ActiveState = (this.zoom == 1) ? ActiveState.Yes : ActiveState.No;
			this.zoom2Button.ActiveState = (this.zoom == 2) ? ActiveState.Yes : ActiveState.No;
		}


		private readonly AbstractEntityPrinter			entityPrinter;

		private Widget									parent;
		private double									zoom;
		private Widgets.ContinuousPagePreviewer			previewer;
		private VScroller								vScroller;
		private HScroller								hScroller;
		private Button									zoom1Button;
		private Button									zoom2Button;
	}
}
