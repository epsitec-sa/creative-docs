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
		static PreviewerController()
		{
			PrintEngine.Setup ();
		}


		public PreviewerController(Printers.AbstractEntityPrinter entityPrinter, IEnumerable<AbstractEntity> entities)
		{
			this.entityPrinter = entityPrinter;
			this.entities      = entities;

			this.currentZoom = 1;

			this.pagePreviewers = new List<Widgets.PrintedPagePreviewer> ();
			this.printerUnitsUsed = new List<Dictionary<PrinterUnit, int>> ();
			this.printerUnitList = Printers.PrinterApplicationSettings.GetPrinterUnitList ();
			this.printerUnitFieldList = new List<PrinterUnit> ();
			this.filteredPages = new List<int> ();

			this.UpdateFilteredPages ();

			this.entityPrinter.PreviewMode = PreviewMode.PagedPreview;
			this.SetPrinterUnits ();
			this.entityPrinter.BuildSections ();
		}


		public bool ShowNotPrinting
		{
			get;
			set;
		}


		public void CreateUI(FrameBox previewBox, FrameBox pagesToolbarBox, FrameBox printerUnitsToolbarBox, bool compact=false)
		{
			//	Crée l'interface dans deux boîtes, l'une pour le ou les aperçus (previewBox) et l'autre pour choisir
			//	la page et le zoom (toolbarBox).
			this.previewBox  = previewBox;
			this.pagesToolbarBox = pagesToolbarBox;
			this.printerUnitsToolbarBox = printerUnitsToolbarBox;

			this.previewFrame = new Scrollable
			{
				Parent = this.previewBox,
				ScrollWithHand = true,
				Dock = DockStyle.Fill,
			};

			this.previewFrame.Viewport.IsAutoFitting = true;

			this.previewFrame.SizeChanged += delegate
			{
				this.UpdatePreview ();
				this.UpdatePageSlider ();
				this.UpdateButtons ();
			};

			//	PagesToolbarBox.
			this.pagesToolbarBox.PreferredHeight = 24;

			{
				var frame = UIBuilder.CreateMiniToolbar (this.pagesToolbarBox, 24);
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
				var frame = UIBuilder.CreateMiniToolbar (this.pagesToolbarBox, 24);
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

			if (this.entityPrinter.EntityPrintingSettings.DocumentTypeSelected == Business.DocumentType.Debug1 ||
				this.entityPrinter.EntityPrintingSettings.DocumentTypeSelected == Business.DocumentType.Debug2)
			{
				{
					var frame = UIBuilder.CreateMiniToolbar (this.pagesToolbarBox, 24);
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
					var frame = UIBuilder.CreateMiniToolbar (this.pagesToolbarBox, 24);
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
				var frame = UIBuilder.CreateMiniToolbar (this.pagesToolbarBox, 24);
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

			this.pageSlider.ValueChanged += delegate
			{
				this.currentPage = (int) this.pageSlider.Value * this.showedPageCount;
				this.UpdatePages (rebuild: false);
			};

			this.zoom18Button.Clicked += delegate
			{
				this.currentZoom = 1.0/8.0;
				this.UpdatePages (rebuild: false);
			};

			this.zoom14Button.Clicked += delegate
			{
				this.currentZoom = 1.0/4.0;
				this.UpdatePages (rebuild: false);
			};

			this.zoom11Button.Clicked += delegate
			{
				this.currentZoom = 1;
				this.UpdatePages (rebuild: false);
			};

			this.zoom21Button.Clicked += delegate
			{
				this.currentZoom = 2;
				this.UpdatePages (rebuild: false);
			};

			this.zoom41Button.Clicked += delegate
			{
				this.currentZoom = 4;
				this.UpdatePages (rebuild: false);
			};

			if (this.debugPrevButton1 != null)
			{
				this.debugPrevButton1.Clicked += new EventHandler<MessageEventArgs> (this.HandleDebugPrevButton1Clicked);
				this.debugNextButton1.Clicked += new EventHandler<MessageEventArgs> (this.HandleDebugNextButton1Clicked);
				this.debugPrevButton2.Clicked += new EventHandler<MessageEventArgs> (this.HandleDebugPrevButton2Clicked);
				this.debugNextButton2.Clicked += new EventHandler<MessageEventArgs> (this.HandleDebugNextButton2Clicked);
			}

			//	PrinterUnitsToolbarBox.
			this.printerUnitsToolbarBox.PreferredHeight = 22;

			{
				if (!compact)
				{
					var label = new StaticText
					{
						Parent = this.printerUnitsToolbarBox,
						Text = "Unité d'impression",
						PreferredWidth = 100,
						Dock = DockStyle.Left,
					};
				}

				this.printerUnitField = new TextFieldCombo
				{
					Parent = this.printerUnitsToolbarBox,
					IsReadOnly = true,
					Dock = DockStyle.Fill,
				};

				this.printerUnitField.SelectedItemChanged += delegate
				{
					int sel = this.printerUnitField.SelectedItemIndex;

					if (sel <= 0)
					{
						this.ChangePrinterUnit (null);
					}
					else
					{
						this.ChangePrinterUnit (this.printerUnitFieldList[sel-1]);
					}
				};

				ToolTip.Default.SetToolTip (this.printerUnitField, "Choix de l'unité d'impression pour les aperçus");
			}

			this.UpdatePages (rebuild: false);
		}

		public void Update()
		{
			//	Met à jour le ou les aperçus. Le nombre de pages et leurs contenus peuvent avoir
			//	été changés.
			this.UpdatePages (rebuild: true);
		}


		private void UpdatePages(bool rebuild)
		{
			this.previewBox.Visibility = this.HasDocumentTypeSelected;
			this.pagesToolbarBox.Visibility = this.HasDocumentTypeSelected;

			if (this.HasDocumentTypeSelected && rebuild)
			{
				this.SetPrinterUnits ();
				this.entityPrinter.BuildSections ();
			}

			this.UpdateFilteredPages ();
			this.UpdatePreview ();
			this.UpdatePageSlider ();
			this.UpdatePrinterUnitField ();
			this.UpdateButtons ();
			this.UpdateZoom ();
		}

		private void UpdateFilteredPages()
		{
			//	Construit la liste des numéros de pages en accord avec une unité d'impression donnée.
			this.filteredPages.Clear ();

			for (int page = 0; page < this.entityPrinter.PageCount (); page++)
			{
				if (this.selectedPrinterUnit == null)
				{
					this.filteredPages.Add (page);
				}
				else
				{
					var documentPrinter = this.entityPrinter.GetDocumentPrinter (page);
					var printersUsed = this.GetPrintersUsed (documentPrinter, page);

					if (Common.InsidePageSize (this.selectedPrinterUnit.PhysicalPaperSize, documentPrinter.MinimalPageSize, documentPrinter.MaximalPageSize) &&
					printersUsed.Keys.Contains (this.selectedPrinterUnit))
					{
						this.filteredPages.Add (page);
					}
				}
			}
		}

		private void UpdatePreview()
		{
			if (!this.HasDocumentTypeSelected)
			{
				return;
			}

			int minimalHope = (this.currentZoom < 1) ? (int) (1.0/this.currentZoom) : 1;
			minimalHope = System.Math.Min (minimalHope, this.filteredPages.Count);
			var placer = new Dialogs.OptimalPreviewPlacer (this.previewFrame.Client.Bounds, this.entityPrinter.BoundsPageSize, Size.Zero, 5, minimalHope);
			this.showedPageCount = System.Math.Max (placer.Total, 1);

			this.currentPage = this.currentPage /this.showedPageCount * this.showedPageCount;

			this.pagePreviewers.Clear ();
			this.printerUnitsUsed.Clear ();
			this.previewFrame.Viewport.Children.Clear ();

			for (int i = 0; i < this.showedPageCount; i++)
			{
				if (this.currentPage+i >= this.filteredPages.Count)
				{
					break;
				}

				int pageRank = this.filteredPages[this.currentPage+i];

				var documentPrinter = this.entityPrinter.GetDocumentPrinter (pageRank);
				var printersUsed = this.GetPrintersUsed (documentPrinter, pageRank);

				bool hasForcingOptions = PreviewerController.HasForcingOptions (printersUsed);

				FormattedText description;
				bool notPrinting, hasManyOptions;

				if (this.ShowNotPrinting)  // montre les pages non imprimées ?
				{
					description = this.GetPrintersUsedDescription (printersUsed);
					notPrinting = (printersUsed.Count == 0);
					hasManyOptions = this.selectedPrinterUnit == null && hasForcingOptions;
				}
				else  // ne montre pas les pages non imprimées ?
				{
					description = null;
					notPrinting = false;
					hasManyOptions = false;
				}

				if (hasManyOptions)
				{
					description = FormattedText.Concat (description, "<br/>L'aspect peut varier selon l'unité d'impression.");
				}

				var preview = new Widgets.PrintedPagePreviewer
				{
					Parent = this.previewFrame.Viewport,
					DocumentPrinter = documentPrinter,
					Description = description,
					NotPrinting = notPrinting,
					HasManyOptions = hasManyOptions,
					CurrentPage = this.entityPrinter.GetPageRelative (pageRank),
				};

				this.pagePreviewers.Add (preview);
				this.printerUnitsUsed.Add (printersUsed);

				pageRank++;
			}

			//	Positionne tous les Widgets.EntityPreviewer, selon le parent this.previewFrame.
			if (this.currentZoom > 1)  // agrandissement ?
			{
				this.previewFrame.HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways;
				this.previewFrame.VerticalScrollerMode   = ScrollableScrollerMode.ShowAlways;
				this.previewFrame.PaintViewportFrame = true;

				var documentPrinter = this.entityPrinter.GetDocumentPrinter (this.RealPage (this.currentPage));
				if (documentPrinter == null)
				{
					this.pagePreviewers[0].PreferredSize = placer.Size * this.currentZoom;
				}
				else
				{
					this.pagePreviewers[0].PreferredSize = placer.GetZoomedSize (documentPrinter.RequiredPageSize, this.currentZoom);
				}

				this.pagePreviewers[0].Dock = DockStyle.Left | DockStyle.Bottom;
			}
			else  // 1:1 ou réduction ?
			{
				this.previewFrame.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
				this.previewFrame.VerticalScrollerMode   = ScrollableScrollerMode.HideAlways;
				this.previewFrame.PaintViewportFrame = false;

				placer.UpdateGeometry (this.pagePreviewers);
			}
		}

		private void UpdatePageSlider()
		{
			this.pageSlider.MinValue = 0;
			this.pageSlider.MaxValue = (this.showedPageCount == 0) ? 0 : System.Math.Max (((this.filteredPages.Count+this.showedPageCount-1) / this.showedPageCount)-1, 0);
			this.pageSlider.Resolution = 1;
			this.pageSlider.SmallChange = 1;
			this.pageSlider.LargeChange = 1;
			this.pageSlider.Value = (this.showedPageCount == 0) ? 0 : this.currentPage / this.showedPageCount;
		}

		private void UpdateButtons()
		{
			if (!this.HasDocumentTypeSelected)
			{
				return;
			}

			int t = this.entityPrinter.PageCount ();
			int p = this.RealPage (this.currentPage) + 1;
			int q = this.RealPage (this.currentPage+this.showedPageCount-1) + 1;

			if (this.showedPageCount <= 1 || p == q)
			{
				this.pageRank.Text = string.Format ("{0} / {1}", p.ToString (), t.ToString ());

				ToolTip.Default.SetToolTip (this.pageRank,   "Page visible / Nombre total de pages");
				ToolTip.Default.SetToolTip (this.pageSlider, "Montre une autre page");
			}
			else
			{
				this.pageRank.Text = string.Format ("{0}..{1} / {2}", p.ToString (), q.ToString (), t.ToString ());

				ToolTip.Default.SetToolTip (this.pageRank,   "Pages visibles / Nombre total de pages");
				ToolTip.Default.SetToolTip (this.pageSlider, "Montre d'autres pages");
			}

			this.zoom14Button.Enable = t > 1;
			this.zoom18Button.Enable = t > 4;
		}

		private int RealPage(int rank)
		{
			rank = System.Math.Min (rank, this.filteredPages.Count-1);

			if (rank < 0)
			{
				return 0;
			}
			else
			{
				return this.filteredPages[rank];
			}
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

		private void UpdatePrinterUnitField()
		{
			//	Met à jour le menu déroulant pour le choix de l'unité d'impression avec toutes les unités d'impression
			//	définies pour ce document.
			this.printerUnitField.Items.Clear ();
			this.printerUnitFieldList.Clear ();

			this.printerUnitField.Items.Add ("Toutes");

			DocumentTypeDefinition documentTypeDefinition = this.entityPrinter.SelectedDocumentTypeDefinition;

			if (documentTypeDefinition != null)
			{
				foreach (var documentPrinterFunction in documentTypeDefinition.DocumentPrinterFunctions)
				{
					if (!string.IsNullOrEmpty (documentPrinterFunction.LogicalPrinterName))
					{
						var printerUnit = this.printerUnitList.Where (x => x.LogicalName == documentPrinterFunction.LogicalPrinterName).FirstOrDefault ();

						if (printerUnit != null)
						{
							string description = printerUnit.NiceDescription;

							if (!this.printerUnitField.Items.Contains (description))
							{
								this.printerUnitField.Items.Add (description);
								this.printerUnitFieldList.Add (printerUnit);
							}
						}
					}
				}
			}

			int sel = this.printerUnitFieldList.IndexOf (this.selectedPrinterUnit);
			this.printerUnitField.SelectedItemIndex = sel+1;

			this.printerUnitsToolbarBox.Visibility = (this.printerUnitField.Items.Count > 1);
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


		private void ChangePrinterUnit(PrinterUnit printerUnit)
		{
			this.selectedPrinterUnit = printerUnit;

			if (this.selectedPrinterUnit == null)
			{
				this.SetPrinterUnits ();
				this.entityPrinter.BuildSections ();
			}
			else
			{
				this.entityPrinter.SetPrinterUnit (this.selectedPrinterUnit);
				this.entityPrinter.BuildSections (this.selectedPrinterUnit.ForcingOptionsToClear, this.selectedPrinterUnit.ForcingOptionsToSet);
			}

			this.UpdatePages (rebuild: true);
		}


		private void SetPrinterUnits()
		{
			//	On spécifie les unités d'impressions pour tous les documents.
			//	L'astuce consiste à fouiller dans toutes les unités d'impressions utilisées pour un document, et à voir
			//	si elles utilisent toutes la même taille de page. Si oui, on peut spécifier n'importe laquelle de ces
			//	unités d'impressions, puisque AbstractDocumentPrinter.SetPrinterUnit ne sert qu'à déterminer la taille
			//	des pages.
			foreach (var documentPrinter in this.entityPrinter.DocumentPrinters)
			{
				var printerUnits = this.GetPrinterUnits (documentPrinter);

				Size pageSize = PreviewerController.GetCommonPageSize (printerUnits);

				if (pageSize.IsEmpty)
				{
					documentPrinter.SetPrinterUnit (null);
				}
				else
				{
					documentPrinter.SetPrinterUnit (printerUnits[0]);
				}
			}
		}

		private List<PrinterUnit> GetPrinterUnits(AbstractDocumentPrinter documentPrinter)
		{
			//	Retourne la liste des unités d'impression utilisées pour un AbstractDocumentPrinter donné.
			var list = new List<PrinterUnit> ();

			DocumentTypeDefinition documentTypeDefinition = this.entityPrinter.SelectedDocumentTypeDefinition;

			if (documentTypeDefinition != null)
			{
				foreach (var documentPrinterFunction in documentTypeDefinition.DocumentPrinterFunctions)
				{
					if (!string.IsNullOrEmpty (documentPrinterFunction.LogicalPrinterName))
					{
						var printerUnit = this.printerUnitList.Where (x => x.LogicalName == documentPrinterFunction.LogicalPrinterName).FirstOrDefault ();

						if (printerUnit != null &&
							Common.InsidePageSize (printerUnit.PhysicalPaperSize, documentPrinter.MinimalPageSize, documentPrinter.MaximalPageSize))
						{
							list.Add (printerUnit);
						}
					}
				}
			}

			return list;
		}

		private static Size GetCommonPageSize(List<PrinterUnit> printerUnits)
		{
			//	Retourne la taille commune à plusieurs unités d'impression, ou Size.Empty si les
			//	tailles diffèrent.
			Size size = Size.Empty;

			foreach (var printerUnit in printerUnits)
			{
				if (size.IsEmpty)
				{
					size = printerUnit.PhysicalPaperSize;
				}
				else
				{
					if (size != printerUnit.PhysicalPaperSize)
					{
						return Size.Empty;
					}
				}
			}

			return size;
		}


		private static bool HasForcingOptions(Dictionary<PrinterUnit, int> printersUsed)
		{
			//	Retourne true si une unité d'impression a des options forcées.
			foreach (var printerUsed in printersUsed)
			{
				PrinterUnit printerUnit = printerUsed.Key;

				if (printerUnit.ForcingOptionsToClear.Count > 0 || printerUnit.ForcingOptionsToSet.Count > 0)
				{
					return true;
				}
			}

			return false;
		}


		private FormattedText GetPrintersUsedDescription(Dictionary<PrinterUnit, int> printerUsed)
		{
			if (printerUsed.Count == 0)
			{
				return "<i>Cette page ne sera pas imprimée</i>";
			}
			else
			{
				var builder = new TextBuilder ();
				int i = 0;

				foreach (var pair in printerUsed)
				{
					if (i > 0)
					{
						if (i < printerUsed.Count-1)
						{
							builder.Append (", ");
						}
						else
						{
							builder.Append (" et ");
						}
					}

					builder.Append (pair.Key.LogicalName);

					if (pair.Value > 1)
					{
						builder.Append (" (");
						builder.Append (pair.Value.ToString ());  // par exemple: "Brouillon (2×)"
						builder.Append ("×)");
					}

					i++;
				}

				FormattedText printerUnit = (printerUsed.Count > 1) ? "les unités d'impression" : "l'unité d'impression";

				return FormattedText.Concat ("Cette page sera imprimée avec ", printerUnit, " ", builder.ToFormattedText (), ".");
			}
		}

		private Dictionary<PrinterUnit, int> GetPrintersUsed(AbstractDocumentPrinter documentPrinter, int page)
		{
			//	Retourne un dictionnaire avec le nom logique de l'unité d'impression et le nombre de copies correspondantes.
			var dico = new Dictionary<PrinterUnit, int> ();

			PageType pageType = this.entityPrinter.GetPageType (page);

			DocumentTypeDefinition documentType = this.entityPrinter.SelectedDocumentTypeDefinition;
			List<DocumentPrinterFunction> documentPrinterFunctions = documentType.DocumentPrinterFunctions;

			foreach (DocumentPrinterFunction documentPrinterFunction in documentPrinterFunctions)
			{
				if (!string.IsNullOrEmpty (documentPrinterFunction.LogicalPrinterName))
				{
					if (Printers.Common.IsPrinterAndPageMatching (documentPrinterFunction.PrinterFunction, pageType))
					{
						var printerUnit = this.printerUnitList.Where (x => x.LogicalName == documentPrinterFunction.LogicalPrinterName).FirstOrDefault ();

						if (printerUnit != null &&
							Common.InsidePageSize (printerUnit.PhysicalPaperSize, documentPrinter.MinimalPageSize, documentPrinter.MaximalPageSize))
						{
							if (dico.ContainsKey (printerUnit))
							{
								dico[printerUnit] += printerUnit.Copies;
							}
							else
							{
								dico.Add (printerUnit, printerUnit.Copies);
							}
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
				return this.entityPrinter.EntityPrintingSettings.DocumentTypeSelected != Business.DocumentType.None;
			}
		}


		private readonly IEnumerable<AbstractEntity>		entities;
		private readonly Printers.AbstractEntityPrinter		entityPrinter;
		private readonly List<Widgets.PrintedPagePreviewer>	pagePreviewers;
		private readonly List<Dictionary<PrinterUnit, int>>	printerUnitsUsed;
		private readonly List<PrinterUnit>					printerUnitList;
		private readonly List<PrinterUnit>					printerUnitFieldList;
		private readonly List<int>							filteredPages;

		private FrameBox									previewBox;
		private FrameBox									pagesToolbarBox;
		private FrameBox									printerUnitsToolbarBox;

		private Scrollable									previewFrame;

		private StaticText									pageRank;
		private HSlider										pageSlider;

		private GlyphButton									debugPrevButton1;
		private StaticText									debugParam1;
		private GlyphButton									debugNextButton1;

		private GlyphButton									debugPrevButton2;
		private StaticText									debugParam2;
		private GlyphButton									debugNextButton2;

		private Button										zoom18Button;
		private Button										zoom14Button;
		private Button										zoom11Button;
		private Button										zoom21Button;
		private Button										zoom41Button;

		private TextFieldCombo								printerUnitField;

		private double										currentZoom;
			
		private int											currentPage;
		private int											showedPageCount;

		private PrinterUnit									selectedPrinterUnit;
	}
}
