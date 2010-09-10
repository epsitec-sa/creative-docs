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
	/// <summary>
	/// Ce contrôleur supervise une zone de prévisualisation et une barre d'outils. La zone de prévisualisation
	/// contiendra un ou plusieurs Widgets.EntityPreviewer, et la barre d'outil tout ce qu'il faut pour choisir
	/// la ou les pages à afficher, ainsi que le zoom (réduction ou agrandissement).
	/// </summary>
	public class PreviewerController
	{
		public PreviewerController(Printers.AbstractEntityPrinter entityPrinter, IEnumerable<AbstractEntity> entities)
		{
			this.entityPrinter = entityPrinter;
			this.entities      = entities;

			this.currentZoom = 1;

			this.pagePreviewers = new List<Widgets.EntityPreviewer> ();
			this.printerUnitList = Printers.PrinterSettings.GetPrinterUnitList ();

			this.entityPrinter.IsPreview = true;
			this.entityPrinter.BuildSections ();
		}


		public void CreateUI(FrameBox previewBox, FrameBox toolbarBox)
		{
			//	Crée l'interface dans deux boîtes, l'une pour le ou les aperçus et l'autre pour choisir
			//	la page et le zoom.
			this.previewBox = previewBox;
			this.toolbarBox = toolbarBox;

			this.previewFrame = new Scrollable
			{
				Parent = this.previewBox,
				Dock = DockStyle.Fill,
			};

			this.previewFrame.Viewport.IsAutoFitting = true;

			this.previewFrame.SizeChanged += delegate
			{
				this.UpdatePagePreviewsGeometry ();
			};

			{
				var frame = UIBuilder.CreateMiniToolbar (this.toolbarBox, 24);
				frame.PreferredWidth = 60;
				frame.Dock = DockStyle.Left;

				this.pageRank = new StaticText
				{
					Parent = frame,
					ContentAlignment = ContentAlignment.MiddleCenter,
					Dock = DockStyle.Fill,
				};
			}

			{
				var frame = UIBuilder.CreateMiniToolbar (this.toolbarBox, 24);
				frame.Margins = new Margins (-1, 0, 0, 0);
				frame.Dock = DockStyle.Fill;

				this.pageSlider = new HSlider
				{
					Parent = frame,
					UseArrowGlyphs = true,
					Dock = DockStyle.Fill,
					Margins = new Margins (2),
				};
			}

			if (this.entityPrinter.EntityPrintingSettings.DocumentTypeSelected == DocumentType.Debug1 ||
				this.entityPrinter.EntityPrintingSettings.DocumentTypeSelected == DocumentType.Debug2)
			{
				{
					var frame = UIBuilder.CreateMiniToolbar (this.toolbarBox, 24);
					frame.PreferredWidth = 70;
					frame.Margins = new Margins (1, 10, 0, 0);
					frame.Dock = DockStyle.Right;

					this.debugPrevButton2 = new GlyphButton
					{
						Parent = frame,
						GlyphShape = GlyphShape.Minus,
						PreferredWidth = 20,
						PreferredHeight = 20,
						Dock = DockStyle.Left,
					};

					this.debugParam2 = new StaticText
					{
						Parent = frame,
						ContentAlignment = ContentAlignment.MiddleCenter,
						PreferredWidth = 30,
						PreferredHeight = 20,
						Dock = DockStyle.Fill,
					};

					this.debugNextButton2 = new GlyphButton
					{
						Parent = frame,
						GlyphShape = GlyphShape.Plus,
						PreferredWidth = 20,
						PreferredHeight = 20,
						Dock = DockStyle.Right,
					};
				}

				{
					var frame = UIBuilder.CreateMiniToolbar (this.toolbarBox, 24);
					frame.PreferredWidth = 70;
					frame.Margins = new Margins (0, 0, 0, 0);
					frame.Dock = DockStyle.Right;

					this.debugPrevButton1 = new GlyphButton
					{
						Parent = frame,
						GlyphShape = GlyphShape.Minus,
						PreferredWidth = 20,
						PreferredHeight = 20,
						Dock = DockStyle.Left,
					};

					this.debugParam1 = new StaticText
					{
						Parent = frame,
						ContentAlignment = ContentAlignment.MiddleCenter,
						PreferredWidth = 30,
						PreferredHeight = 20,
						Dock = DockStyle.Fill,
					};

					this.debugNextButton1 = new GlyphButton
					{
						Parent = frame,
						GlyphShape = GlyphShape.Plus,
						PreferredWidth = 20,
						PreferredHeight = 20,
						Dock = DockStyle.Right,
					};
				}

				this.UpdateDebug ();
			}

			{
				var frame = UIBuilder.CreateMiniToolbar (this.toolbarBox, 24);
				frame.Margins = new Margins (2, 0, 0, 0);
				frame.Dock = DockStyle.Right;

				this.zoom18Button = new Button
				{
					Parent = frame,
					ButtonStyle = ButtonStyle.ToolItem,
					AutoFocus = false,
					Text = "÷8",
					PreferredWidth = 20,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
				};

				this.zoom14Button = new Button
				{
					Parent = frame,
					ButtonStyle = ButtonStyle.ToolItem,
					AutoFocus = false,
					Text = "÷4",
					PreferredWidth = 20,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
				};

				this.zoom11Button = new Button
				{
					Parent = frame,
					ButtonStyle = ButtonStyle.ToolItem,
					AutoFocus = false,
					Text = "×1",
					PreferredWidth = 20,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
				};

				this.zoom21Button = new Button
				{
					Parent = frame,
					ButtonStyle = ButtonStyle.ToolItem,
					AutoFocus = false,
					Text = "×2",
					PreferredWidth = 20,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
				};

				this.zoom41Button = new Button
				{
					Parent = frame,
					ButtonStyle = ButtonStyle.ToolItem,
					AutoFocus = false,
					Text = "×4",
					PreferredWidth = 20,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
				};

				ToolTip.Default.SetToolTip (this.zoom18Button, "Montre jusqu'à 8 pages simultanément");
				ToolTip.Default.SetToolTip (this.zoom14Button, "Montre jusqu'à 4 pages simultanément");
				ToolTip.Default.SetToolTip (this.zoom11Button, "Montre une page intégralement");
				ToolTip.Default.SetToolTip (this.zoom21Button, "Montre une page agrandie 2 fois");
				ToolTip.Default.SetToolTip (this.zoom41Button, "Montre une page agrandie 4 fois");
			}

			this.placer = new Dialogs.PreviewOptimalPlacer (this.pagePreviewers, this.entityPrinter.PageSize);

			this.pageSlider.ValueChanged += delegate
			{
				this.currentPage = (int) this.pageSlider.Value;
				this.UpdatePages ();
			};

			this.zoom18Button.Clicked += delegate
			{
				this.currentZoom = 1.0/8.0;
				this.UpdatePages ();
			};

			this.zoom14Button.Clicked += delegate
			{
				this.currentZoom = 1.0/4.0;
				this.UpdatePages ();
			};

			this.zoom11Button.Clicked += delegate
			{
				this.currentZoom = 1;
				this.UpdatePages ();
			};

			this.zoom21Button.Clicked += delegate
			{
				this.currentZoom = 2;
				this.UpdatePages ();
			};

			this.zoom41Button.Clicked += delegate
			{
				this.currentZoom = 4;
				this.UpdatePages ();
			};

			if (this.debugPrevButton1 != null)
			{
				this.debugPrevButton1.Clicked += new EventHandler<MessageEventArgs> (this.HandleDebugPrevButton1Clicked);
				this.debugNextButton1.Clicked += new EventHandler<MessageEventArgs> (this.HandleDebugNextButton1Clicked);
				this.debugPrevButton2.Clicked += new EventHandler<MessageEventArgs> (this.HandleDebugPrevButton2Clicked);
				this.debugNextButton2.Clicked += new EventHandler<MessageEventArgs> (this.HandleDebugNextButton2Clicked);
			}

			this.UpdatePages ();
		}

		public void Update()
		{
			//	Met à jour le ou les aperçus. Le nombre et pages et leurs contenus peuvent avoir
			//	été changés.
			if (this.HasDocumentTypeSelected)
			{
				this.entityPrinter.BuildSections ();
			}

			this.UpdatePages ();
		}


		private void UpdatePages()
		{
			this.previewBox.Visibility = this.HasDocumentTypeSelected;
			this.toolbarBox.Visibility = this.HasDocumentTypeSelected;

			this.UpdateZoom ();
			this.UpdatePreview ();
			this.UpdateButtons ();
		}

		private void UpdateZoom()
		{
			if (!this.HasDocumentTypeSelected)
			{
				return;
			}

			this.zoom18Button.ActiveState = this.currentZoom == 1.0/8.0 ? ActiveState.Yes : ActiveState.No;
			this.zoom14Button.ActiveState = this.currentZoom == 1.0/4.0 ? ActiveState.Yes : ActiveState.No;
			this.zoom11Button.ActiveState = this.currentZoom == 1       ? ActiveState.Yes : ActiveState.No;
			this.zoom21Button.ActiveState = this.currentZoom == 2       ? ActiveState.Yes : ActiveState.No;
			this.zoom41Button.ActiveState = this.currentZoom == 4       ? ActiveState.Yes : ActiveState.No;
		}

		private void UpdatePreview()
		{
			if (!this.HasDocumentTypeSelected)
			{
				return;
			}

			this.showedPageCount = (this.currentZoom < 1) ? (int) (1.0/this.currentZoom) : 1;

			this.currentPage = System.Math.Min (this.currentPage + this.showedPageCount, this.entityPrinter.PageCount ());
			this.currentPage = System.Math.Max (this.currentPage - this.showedPageCount, 0);

			this.pagePreviewers.Clear ();
			this.previewFrame.Viewport.Children.Clear ();

			int pageRank = this.currentPage;

			for (int i = 0; i < this.showedPageCount; i++)
			{
				if (pageRank >= this.entityPrinter.PageCount ())
				{
					break;
				}

				var preview = new Widgets.EntityPreviewer
				{
					Parent = this.previewFrame.Viewport,
					EntityPrinter = this.entityPrinter,
					Description = this.GetPrintersUsedDescription (pageRank),
					CurrentPage = pageRank++,
				};

				this.pagePreviewers.Add (preview);
			}

			this.UpdatePagePreviewsGeometry ();
			this.UpdatePageSlider ();
		}

		private void UpdatePagePreviewsGeometry()
		{
			//	Positionne tous les Widgets.EntityPreviewer, selon le parent this.previewFrame.
			if (this.currentZoom > 1)  // agrandissement ?
			{
				this.previewFrame.HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways;
				this.previewFrame.VerticalScrollerMode   = ScrollableScrollerMode.ShowAlways;
				this.previewFrame.PaintViewportFrame = true;

				this.pagePreviewers[0].PreferredSize = this.placer.AdjustRatioPageSize (this.previewFrame.Client.Bounds.Size * this.currentZoom);
				this.pagePreviewers[0].Dock = DockStyle.Left | DockStyle.Bottom;
			}
			else  // 1:1 ou réduction ?
			{
				this.previewFrame.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
				this.previewFrame.VerticalScrollerMode   = ScrollableScrollerMode.HideAlways;
				this.previewFrame.PaintViewportFrame = false;

				this.placer.AvailableSize = this.previewFrame.Client.Bounds.Size;
				this.placer.PageCount = this.pagePreviewers.Count;
				this.placer.UpdateGeometry ();
			}
		}

		private void UpdatePageSlider()
		{
			this.pageSlider.MinValue = 0;
			this.pageSlider.MaxValue = System.Math.Max (this.entityPrinter.PageCount () - this.showedPageCount, 0);
			this.pageSlider.Resolution = 1;
			//?this.pageSlider.VisibleRangeRatio = System.Math.Min ((decimal) this.showedPageCount / (decimal) this.entityPrinter.PageCount (), 1);
			this.pageSlider.SmallChange = 1;
			this.pageSlider.LargeChange = 10;
			this.pageSlider.Value = this.currentPage;
		}

		private void UpdateButtons()
		{
			if (!this.HasDocumentTypeSelected)
			{
				return;
			}

			int t = this.entityPrinter.PageCount ();
			int p = this.currentPage+1;

			if (this.showedPageCount <= 1)
			{
				this.pageRank.Text = string.Format ("{0} / {1}", p.ToString (), t.ToString ());

				ToolTip.Default.SetToolTip (this.pageRank,   "Page visible / Nombre total de pages");
				ToolTip.Default.SetToolTip (this.pageSlider, "Montre une autre page");
			}
			else
			{
				int q = System.Math.Min (p + this.showedPageCount-1, t);
				this.pageRank.Text = string.Format ("{0}..{1} / {2}", p.ToString (), q.ToString (), t.ToString ());

				ToolTip.Default.SetToolTip (this.pageRank,   "Pages visibles / Nombre total de pages");
				ToolTip.Default.SetToolTip (this.pageSlider, "Montre d'autres pages");
			}

			this.zoom14Button.Enable = t > 1;
			this.zoom18Button.Enable = t > 4;
		}

		private void UpdateDebug()
		{
			this.debugParam1.Text = this.entityPrinter.DebugParam1.ToString ();
			this.debugParam2.Text = this.entityPrinter.DebugParam2.ToString ();

			if (this.pagePreviewers.Count != 0)
			{
				this.pagePreviewers[0].Invalidate ();
			}
		}

	
		private string GetPrintersUsedDescription(int page)
		{
			Dictionary<string, int> dico = this.GetPrintersUsed (page);

			if (dico.Count == 0)
			{
				return "<i>Cette page ne sera pas imprimée</i>";  // texte en rouge
			}
			else
			{
				System.Text.StringBuilder builder = new System.Text.StringBuilder ();
				int i = 0;

				foreach (var pair in dico)
				{
					if (i > 0)
					{
						if (i < dico.Count-1)
						{
							builder.Append (", ");
						}
						else
						{
							builder.Append (" et ");
						}
					}

					builder.Append (pair.Key);

					if (pair.Value > 1)
					{
						builder.Append (" (");
						builder.Append (pair.Value.ToString ());  // par exemple: "Brouillon (2×)"
						builder.Append ("×)");
					}

					i++;
				}

				string printerUnit = (dico.Count > 1) ? "les unités d'impression" : "l'unité d'impression";

				return string.Format ("Cette page sera imprimée avec {0} {1}", printerUnit, builder.ToString ());
			}
		}

		private Dictionary<string, int> GetPrintersUsed(int page)
		{
			Dictionary<string, int> dico = new Dictionary<string, int> ();

			PageType pageType = this.entityPrinter.GetPageType (page);

			DocumentTypeDefinition documentType = this.entityPrinter.DocumentTypeSelected;
			List<DocumentPrinter> documentPrinters = documentType.DocumentPrinters;

			foreach (DocumentPrinter documentPrinter in documentPrinters)
			{
				if (!string.IsNullOrEmpty (documentPrinter.LogicalPrinterName))
				{
					if (Printers.Common.IsPrinterAndPageMatching (documentPrinter.PrinterFunction, pageType))
					{
						var pu = this.printerUnitList.Where (x => x.LogicalName == documentPrinter.LogicalPrinterName).FirstOrDefault ();
						int copies = (pu == null) ? 1 : pu.Copies;

						if (dico.ContainsKey (documentPrinter.LogicalPrinterName))
						{
							dico[documentPrinter.LogicalPrinterName] += copies;
						}
						else
						{
							dico.Add (documentPrinter.LogicalPrinterName, copies);
						}
					}
				}
			}

			return dico;
		}


		private void HandleDebugPrevButton1Clicked(object sender, MessageEventArgs e)
		{
			this.entityPrinter.DebugParam1 -= GetStep (e);
			this.UpdateDebug ();
		}

		private void HandleDebugNextButton1Clicked(object sender, MessageEventArgs e)
		{
			this.entityPrinter.DebugParam1 += GetStep (e);
			this.UpdateDebug ();
		}

		private void HandleDebugPrevButton2Clicked(object sender, MessageEventArgs e)
		{
			this.entityPrinter.DebugParam2 -= GetStep (e);
			this.UpdateDebug ();
		}

		private void HandleDebugNextButton2Clicked(object sender, MessageEventArgs e)
		{
			this.entityPrinter.DebugParam2 += GetStep (e);
			this.UpdateDebug ();
		}

		private static int GetStep(MessageEventArgs e)
		{
			int step = 1;

			if ((e.Message.ModifierKeys & ModifierKeys.Control) != 0)
			{
				step *= 10;
			}

			if ((e.Message.ModifierKeys & ModifierKeys.Shift) != 0)
			{
				step *= 100;
			}

			return step;
		}


		private bool HasDocumentTypeSelected
		{
			get
			{
				return this.entityPrinter.EntityPrintingSettings.DocumentTypeSelected != DocumentType.None;
			}
		}


		private readonly IEnumerable<AbstractEntity>	entities;
		private readonly Printers.AbstractEntityPrinter	entityPrinter;

		private FrameBox								previewBox;
		private FrameBox								toolbarBox;

		private Scrollable								previewFrame;
		private List<Widgets.EntityPreviewer>			pagePreviewers;
		private Dialogs.PreviewOptimalPlacer			placer;

		private StaticText								pageRank;
		private HSlider									pageSlider;

		private GlyphButton								debugPrevButton1;
		private StaticText								debugParam1;
		private GlyphButton								debugNextButton1;

		private GlyphButton								debugPrevButton2;
		private StaticText								debugParam2;
		private GlyphButton								debugNextButton2;

		private Button									zoom18Button;
		private Button									zoom14Button;
		private Button									zoom11Button;
		private Button									zoom21Button;
		private Button									zoom41Button;

		private double									currentZoom;
		private List<PrinterUnit>						printerUnitList;

		private int										currentPage;
		private int										showedPageCount;
	}
}
