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
using System;

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
			this.pagePreviews   = new List<Widgets.PreviewEntity> ();
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

			this.previewFrame = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 0, 0, 0),
			};

			this.previewFrame.SizeChanged += delegate
			{
				this.UpdatePagePreviewsGeometry ();
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

#if false
			//	Montre l'aperçu de la première section.
			if (this.jobs.Count > 0 && this.jobs[0].Sections.Count > 0)
			{
				this.previewedSection = this.jobs[0].Sections[0];
				PrintingJobDialog.SetPreviewButtonState (this.previewButtons[0], true);
			}
#endif

			Size pageSize = this.jobs[0].Sections[0].EntityPrinter.PageSize;
			this.placer = new PreviewOptimalPlacer (this.pagePreviews, pageSize);

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
				Text = string.Format ("<font size=\"16\"><b>{0} </b></font>sur l'imprimante {1}", job.JobFullName, job.Sections[0].PrinterUnit.PhysicalPrinterName),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				PreferredHeight = 20,
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
				section.Enable = check.ActiveState == ActiveState.Yes;
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
					Text = string.Format ("Unité d'impression {0}", section.PrinterUnit.NiceDescription),
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

			ToolTip.Default.SetToolTip (button, "Montre les pages correspondantes");

			this.previewButtons.Add (button);

			button.Clicked += delegate
			{
				this.PreviewButtonsReset ();
				PrintingJobDialog.SetPreviewButtonState (button, true);

				this.previewedSection = section;
				this.previewedSection.EntityPrinter.IsPreview = true;
				this.previewedSection.EntityPrinter.BuildSections ();

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
			//	Crée tous les Widgets.PreviewEntity, sans s'occuper de les positionner.
			this.pagePreviews.Clear ();
			this.previewFrame.Children.Clear ();

			if (this.previewedSection == null)
			{
				this.previewFrame.DrawFullFrame = true;

				var label = new StaticText
				{
					Parent = this.previewFrame,
					Text = "<font size=\"24\" color=\"#ffffff\"><i>Cliquez sur un petit bouton bleu à gauche pour voir la ou les pages correspondantes</i></font>",
					ContentAlignment = ContentAlignment.MiddleCenter,
					Margins = new Margins (10),
					Dock = DockStyle.Fill,
				};
			}
			else
			{
				this.previewFrame.DrawFullFrame = false;

				int count = this.previewedSection.PageCount;
				int pageRank = this.previewedSection.FirstPage;

				for (int i = 0; i < count; i++)
				{
					var preview = new Widgets.PreviewEntity
					{
						Parent = this.previewFrame,
						EntityPrinter = this.previewedSection.EntityPrinter,
						CurrentPage = pageRank++,
					};

					this.pagePreviews.Add (preview);
				}

				this.UpdatePagePreviewsGeometry ();
			}
		}

		private void UpdatePagePreviewsGeometry()
		{
			//	Positionne tous les Widgets.PreviewEntity, selon le parent this.previewFrame.
			if (this.previewedSection != null)
			{
				this.placer.AvailableSize = this.previewFrame.Client.Bounds.Size;
				this.placer.PageCount = this.previewedSection.PageCount;
				this.placer.UpdateGeometry ();
			}
		}

		private int PageCountToPrint
		{
			//	Retourne le nombre de pages à imprimer.
			get
			{
				int count = 0;

				foreach (var job in this.jobs)
				{
					foreach (var section in job.Sections)
					{
						if (section.Enable)
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
		private PreviewOptimalPlacer					placer;
		private FrameBox								previewFrame;
		private StaticText								informationText;
		private Button									invertButton;
		private Button									acceptButton;
		private Button									cancelButton;
		private SectionToPrint							previewedSection;
		private List<CheckButton>						checkButtons;
		private List<GlyphButton>						previewButtons;
		private List<Widgets.PreviewEntity>				pagePreviews;
	}
}
