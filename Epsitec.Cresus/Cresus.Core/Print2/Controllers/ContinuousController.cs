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
using Epsitec.Cresus.Core.Print2.EntityPrinters;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Print2.Controllers
{
	public class ContinuousController
	{
		public ContinuousController(CoreData coreData, DocumentMetadataEntity metadoc, Business.DocumentType documentType)
		{
			var entities = new List<AbstractEntity> ();
			entities.Add (metadoc);

			var options = OptionsDictionary.GetDefault ();

			this.documentPrinter = AbstractPrinter.CreateDocumentPrinter (coreData, entities, options, null);
			System.Diagnostics.Debug.Assert (this.documentPrinter != null);

			this.documentPrinter.SetContinuousPreviewMode ();
			this.documentPrinter.BuildSections ();

			this.zoom = 1;
		}

		public void CreateUI(Widget parent)
		{
			System.Diagnostics.Debug.Assert (parent != null);
			this.parent = parent;

			double scrollBreadth = AbstractScroller.DefaultBreadth;

			var main = new FrameBox ()
			{
				Parent = this.parent,
				Dock   = DockStyle.Fill,
			};

			var footer = new FrameBox ()
			{
				Parent          = this.parent,
				Dock            = DockStyle.Bottom,
				PreferredHeight = scrollBreadth,
				Padding         = new Margins (0, scrollBreadth, 0, 0),
			};

			//	Crée le visualisateur de page ContinuousPagePreviewer.
			this.previewer = new Widgets.ContinuousPagePreviewer ()
			{
				Parent          = main,
				DocumentPrinter = this.documentPrinter,
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
			this.zoom1Button = new Button
			{
				Parent          = footer,
				ButtonStyle     = ButtonStyle.ToolItem,
				AutoFocus       = false,
				Text            = "×1",
				PreferredWidth  = 20,
				PreferredHeight = scrollBreadth,
				Dock            = DockStyle.Left,
			};

			this.zoom2Button = new Button
			{
				Parent          = footer,
				ButtonStyle     = ButtonStyle.ToolItem,
				AutoFocus       = false,
				Text            = "×2",
				PreferredWidth  = 20,
				PreferredHeight = scrollBreadth,
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
				Margins         = new Margins (2, 0, 0, 0),
			};

			//	Connecte les événements.
			this.previewer.SizeChanged += delegate
			{
				this.UpdateScroller ();
			};

			this.previewer.CurrentValueChanged += delegate
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

			this.previewer.CurrentValue = new Point (0, 1000000);  // montre la partie supérieure

			this.UpdateScroller ();
			this.UpdateButtons ();
		}

		public void CloseUI()
		{
			if (this.previewer != null)
			{
				this.previewer.CloseUI ();
			}
		}

		public void Update()
		{
			//	Met à jour le contenu de la page.
			this.documentPrinter.BuildSections ();
			this.previewer.Update ();
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
			if ((this.previewer == null) ||
				(this.previewer.DocumentPrinter == null))
			{
				return;
			}

			var min = this.previewer.MinValue;
			var max = this.previewer.MaxValue;
			var vrt = this.previewer.VisibleRangeRatio;
			var val = this.previewer.CurrentValue;
			var std = this.previewer.ScreenToDocumentScale;

			//	Met à jour l'ascenseur horizontal.
			if (System.Math.Abs (vrt.Width-1) < 0.00001)
			{
				this.hScroller.Enable = false;

				this.hScroller.MinValue = 0;
				this.hScroller.MaxValue = 0;
				this.hScroller.Value    = 0;
			}
			else
			{
				this.hScroller.Enable = true;

				this.hScroller.MinValue          = (decimal) min.X;
				this.hScroller.MaxValue          = (decimal) max.X;
				this.hScroller.Value             = (decimal) val.X;
				this.hScroller.VisibleRangeRatio = (decimal) vrt.Width;
				this.hScroller.SmallChange       = (decimal) (10.0*std);
				this.hScroller.LargeChange       = (decimal) (this.previewer.Client.Bounds.Width*0.5*std);
			}

			//	Met à jour l'ascenseur vertical.
			if (System.Math.Abs (vrt.Height-1) < 0.00001)
			{
				this.vScroller.Enable = false;

				this.vScroller.MinValue = 0;
				this.vScroller.MaxValue = 0;
				this.vScroller.Value    = 0;
			}
			else
			{
				this.vScroller.Enable = true;

				this.vScroller.MinValue          = (decimal) min.Y;
				this.vScroller.MaxValue          = (decimal) max.Y;
				this.vScroller.Value             = (decimal) val.Y;
				this.vScroller.VisibleRangeRatio = (decimal) vrt.Height;
				this.vScroller.SmallChange       = (decimal) (10.0*std);
				this.vScroller.LargeChange       = (decimal) (this.previewer.Client.Bounds.Height*0.5*std);
			}
		}

		private void UpdatePagePreview()
		{
			this.previewer.CurrentValue = new Point((double) this.hScroller.Value,
													(double) this.vScroller.Value);
		}

		private void UpdateButtons()
		{
			this.zoom1Button.ActiveState = (this.zoom == 1) ? ActiveState.Yes : ActiveState.No;
			this.zoom2Button.ActiveState = (this.zoom == 2) ? ActiveState.Yes : ActiveState.No;
		}


		private readonly AbstractPrinter				documentPrinter;

		private Widget									parent;
		private double									zoom;
		private Widgets.ContinuousPagePreviewer			previewer;
		private VScroller								vScroller;
		private HScroller								hScroller;
		private Button									zoom1Button;
		private Button									zoom2Button;
	}
}
