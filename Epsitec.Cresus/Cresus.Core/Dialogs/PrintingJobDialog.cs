//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Printers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour voir les jobs avant impression réelle.
	/// </summary>
	class PrintingJobDialog : AbstractDialog
	{
		public PrintingJobDialog(CoreApplication application, List<JobToPrint> jobs)
		{
			this.IsApplicationWindow = true;  // pour avoir les boutons Minimize/Maximize/Close !

			this.application = application;
			this.jobs        = jobs;

			this.checkButtons   = new List<CheckButton> ();
			this.previewButtons = new List<GlyphButton> ();
		}


		protected override Window CreateWindow()
		{
			this.window = new Window ();

			this.SetupWindow ();
			this.SetupWidgets ();
			this.SetupEvents ();
			this.UpdateWidgets ();
			this.UpdatePreview ();

			this.window.AdjustWindowSize ();

			return this.window;
		}

		private void SetupWindow()
		{
			this.OwnerWindow = this.application.Window;
			this.window.Icon = this.application.Window.Icon;
			this.window.Text = "Tâches d'impression";
			this.window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;  // pour avoir les boutons Minimize/Maximize/Close !

			this.UpdateWindowSize ();

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		private void UpdateWindowSize()
		{
			double width = PrintingJobDialog.previewWidth;
			double height = (int) (width*297/210);  // place pour une page A4 verticale

			this.window.ClientSize = new Size (10 + PrintingJobDialog.panelWidth + 20 + PrintingJobDialog.previewWidth, 10 + height + 40);
			this.window.AdjustWindowSize ();
		}

		private void SetupWidgets()
		{
			this.checkButtons.Clear ();
			this.previewButtons.Clear ();

			var frame = new FrameBox
			{
				Parent = this.window.Root,
				Anchor = AnchorStyles.All,
				ContainerLayoutMode = Common.Widgets.ContainerLayoutMode.HorizontalFlow,
				Margins = new Margins (10, 10, 10, 40),
			};

			//	Crée les 2 panneaux côte-à-côte.
			var leftFrame = new FrameBox
			{
				Parent = frame,
				DrawFullFrame = true,
				PreferredWidth = PrintingJobDialog.panelWidth,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
			};

			var title = new StaticText
			{
				Parent = leftFrame,
				Text = "<font size=\"16\">Liste des tâches d'impression</font>",
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Margins = new Margins (10, 10, 10, 10),
			};

			var scrollable = new Scrollable
			{
				Parent = leftFrame,
				Dock = DockStyle.Fill,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode = ScrollableScrollerMode.Auto,
				PaintViewportFrame = false,
			};

			scrollable.Viewport.IsAutoFitting = true;

			int jobIndex = 0;
			foreach (var job in this.jobs)
			{
				this.SetupJobWidgets (scrollable.Viewport, job, jobIndex++);
			}

			this.previewFrame = new Widgets.PreviewEntity
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 0, 0, 0),
			};

			//	Rempli le pied de page.
			var footer = new FrameBox
			{
				Parent = this.window.Root,
				PreferredHeight = 20,
				Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
				Margins = new Margins (10, 10, 0, 10),
			};

			this.cancelButton = new Button ()
			{
				Parent = footer,
				Text = "Annuler",
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
				TabIndex = 2,
			};

			this.acceptButton = new Button ()
			{
				Parent = footer,
				Text = "Imprimer",
				Dock = DockStyle.Right,
				TabIndex = 1,
			};

			this.informationText = new StaticText
			{
				Parent = footer,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleRight,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 10, 0, 0),
			};

			this.invertButton = new Button
			{
				Parent = footer,
				Text = "Inverser la sélection",
				PreferredWidth = 120,
				Dock = DockStyle.Left,
			};

			this.PreviewButtonsReset ();

			//	Montre l'aperçu de la première section.
			if (this.jobs.Count > 0 && this.jobs[0].Sections.Count > 0)
			{
				this.previewedSection = this.jobs[0].Sections[0];
				PrintingJobDialog.SetPreviewButtonState (this.previewButtons[0], true);
			}

			this.UpdateWidgets ();
			this.UpdatePreview ();
			this.UpdateInformation ();
		}

		private void SetupJobWidgets(Widget parent, JobToPrint job, int jobIndex)
		{
			var box = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, -1),
				Padding = new Margins (10),
			};

			var title = new StaticText
			{
				Parent = box,
				Text = string.Format ("<font size=\"22\"><b>{0}.{1} </b></font><font size=\"14\">{2}</font>", (job.Sections[0].EntityRank+1).ToString (), (jobIndex+1).ToString (), job.Sections[0].Printer.PhysicalPrinterName),
				PreferredHeight = 30,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 10),
			};

			foreach (var section in job.Sections)
			{
				this.SetupSectionWidgets (box, section);
			}
		}

		private void SetupSectionWidgets(FrameBox parent, SectionToPrint section)
		{
			double size = 18;

			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, -1),
			};

			var check = new CheckButton
			{
				Parent = box,
				ActiveState = Common.Widgets.ActiveState.Yes,
				PreferredWidth = 20,
				Dock = DockStyle.Left,
			};

			this.checkButtons.Add (check);

			check.ActiveStateChanged += delegate
			{
				section.PrintThisSection = check.ActiveState == ActiveState.Yes;
				this.UpdateInformation ();
				this.UpdateWidgets ();
			};

			{
				var rectangle = new FrameBox
				{
					Parent = box,
					DrawFullFrame = true,
					PreferredHeight = size,
					PreferredWidth = 80,
					Dock = DockStyle.Left,
				};

				var text = new StaticText
				{
					Parent = rectangle,
					Text = section.PagesDescription,
					ContentAlignment = Common.Drawing.ContentAlignment.MiddleLeft,
					Dock = DockStyle.Fill,
					Margins = new Margins (5, 5, 0, 0),
				};
			}

			{
				var rectangle = new FrameBox
				{
					Parent = box,
					DrawFullFrame = true,
					PreferredHeight = size,
					Dock = DockStyle.Fill,
					Margins = new Margins (-1, 1, 0, 0),
				};

				var tray = new StaticText
				{
					Parent = rectangle,
					Text = string.Format ("Imprimante {0}", section.Printer.NiceDescription),
					ContentAlignment = Common.Drawing.ContentAlignment.MiddleLeft,
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Fill,
					Margins = new Margins (5, 5, 0, 0),
				};
			}

			var button = new GlyphButton
			{
				Parent = box,
				PreferredHeight = size,
				PreferredWidth = size,
				Dock = DockStyle.Right,
			};

			this.previewButtons.Add (button);

			button.Clicked += delegate
			{
				this.PreviewButtonsReset ();
				PrintingJobDialog.SetPreviewButtonState (button, true);

				this.previewedSection = section;
				this.UpdatePreview ();
			};
		}

		private void SetupEvents()
		{
			this.invertButton.Clicked += delegate
			{
				this.InvertSelection ();
			};

			this.acceptButton.Clicked += delegate
			{
				this.Result = DialogResult.Accept;
				this.CloseDialog ();
			};

			this.cancelButton.Clicked += delegate
			{
				this.Result = DialogResult.Cancel;
				this.CloseDialog ();
			};
		}

		private void UpdateWidgets()
		{
			int count = this.PageCountToPrint;

			this.acceptButton.Enable = count != 0;
		}

		private void UpdateInformation()
		{
			int count = this.PageCountToPrint;

			if (count == 0)
			{
				this.informationText.Text = "Aucune page ne sera imprimée";
			}
			else
			{
				string accord1 = (count <= 1) ? "" : "s";
				string accord2 = (count <= 1) ? "a" : "ont";
				this.informationText.Text = string.Format ("{0} page{1} ser{2} imprimée{1}", count.ToString (), accord1, accord2);
			}
		}

		private void UpdatePreview()
		{
			if (this.previewedSection != null)
			{
				this.previewedSection.EntityPrinter.Clear ();
				this.previewFrame.BuildSections (this.previewedSection.EntityPrinter);
				this.previewedSection.EntityPrinter.CurrentPage = this.previewedSection.FirstPage;
				this.previewFrame.Invalidate ();  // pour forcer le dessin
			}
		}

		private int PageCountToPrint
		{
			get
			{
				int count = 0;

				foreach (var job in this.jobs)
				{
					foreach (var section in job.Sections)
					{
						if (section.PrintThisSection)
						{
							count += section.PageCount;
						}
					}
				}

				return count;
			}
		}

		private void InvertSelection()
		{
			foreach (var button in this.checkButtons)
			{
				if (button.ActiveState == ActiveState.Yes)
				{
					button.ActiveState = ActiveState.No;
				}
				else
				{
					button.ActiveState = ActiveState.Yes;
				}
			}
		}

		private void PreviewButtonsReset()
		{
			foreach (var button in this.previewButtons)
			{
				PrintingJobDialog.SetPreviewButtonState (button, false);
			}
		}

		private static void SetPreviewButtonState(GlyphButton button, bool state)
		{
			button.GlyphShape = state ? GlyphShape.ArrowRight : GlyphShape.None;
		}


		private static readonly double panelWidth   = 450;
		private static readonly double previewWidth = 350;

		private readonly CoreApplication				application;
		private readonly List<JobToPrint>				jobs;

		private Window									window;
		private Widgets.PreviewEntity					previewFrame;
		private StaticText								informationText;
		private Button									invertButton;
		private Button									acceptButton;
		private Button									cancelButton;
		private SectionToPrint							previewedSection;
		private List<CheckButton>						checkButtons;
		private List<GlyphButton>						previewButtons;
	}
}
