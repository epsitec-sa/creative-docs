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
	/// Dialogue pour monter un aperçu d'une page avant l'impression. On peut naviguer dans les différentes
	/// pages du document.
	/// </summary>
	class PreviewDialog : AbstractDialog, IAttachedDialog
	{
		public PreviewDialog(CoreApplication application, Printers.AbstractEntityPrinter entityPrinter, IEnumerable<AbstractEntity> entities)
		{
			this.IsApplicationWindow = true;  // pour avoir les boutons Minimize/Maximize/Close !

			this.application   = application;
			this.entityPrinter = entityPrinter;
			this.entities      = entities;

			this.currentZoom = 1;

			this.application.AttachDialog (this);

			this.pagePreviews = new List<Widgets.PreviewEntity> ();
			this.printerUnitList = Printers.PrinterSettings.GetPrinterUnitList ();

			this.entityPrinter.IsPreview = true;
			this.entityPrinter.BuildSections ();
		}


		protected override Window CreateWindow()
		{
			Window window = new Window ();

			this.SetupWindow  (window);
			this.SetupWidgets (window);
			this.SetupEvents  (window);

			window.AdjustWindowSize ();

			return window;
		}

		protected void SetupWindow(Window window)
		{
			this.OwnerWindow = this.application.Window;

			var pageSize = this.entityPrinter.PageSize;
			string path = System.IO.Path.Combine (Globals.Directories.ExecutableRoot, "app.ico");

			window.Icon = this.application.Window.Icon;
			window.Text = "Aperçu avant impression";
			window.ClientSize = new Size (pageSize.Width*3+10+10, pageSize.Height*3+10+40);
			window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;  // pour avoir les boutons Minimize/Maximize/Close !

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		protected void SetupWidgets(Window window)
		{
			this.previewFrame = new Scrollable
			{
				Parent = window.Root,
				Anchor = AnchorStyles.All,
				Margins = new Margins (10, 10, 10, 40),
			};

			this.previewFrame.Viewport.IsAutoFitting = true;

			this.previewFrame.SizeChanged += delegate
			{
				this.UpdatePagePreviewsGeometry ();
			};

			this.footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
				Margins = new Margins (10, 10, 0, 10),
			};

			this.updateButton = new Button
			{
				Parent = this.footer,
				Text = "Mettre à jour",
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
			};

			{
				var frame = UIBuilder.CreateMiniToolbar (this.footer, 24);
				frame.PreferredWidth = 50;
				frame.Margins = new Margins (20, 0, 0, 0);
				frame.Dock = DockStyle.Left;

				this.pageRank = new StaticText
				{
					Parent = frame,
					ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
					Dock = DockStyle.Fill,
				};
			}

			{
				var frame = UIBuilder.CreateMiniToolbar (this.footer, 24);
				frame.Margins = new Margins (-1, 0, 0, 0);
				frame.Dock = DockStyle.Fill;

				this.pageScroller = new HScroller
				{
					Parent = frame,
					Dock = DockStyle.Fill,
				};
			}

			ToolTip.Default.SetToolTip (this.pageRank,     "Page(s) visible(s)");
			ToolTip.Default.SetToolTip (this.pageScroller, "Montre une autre page");

			this.closeButton = new Button ()
			{
				Parent = this.footer,
				Text = "Fermer",
				Dock = DockStyle.Right,
				TabIndex = 1,
			};

			this.printButton = new Button ()
			{
				Parent = this.footer,
				Text = "Tâches d'impression",
				PreferredWidth = 120,
				Dock = DockStyle.Right,
				Margins = new Margins (0, 10, 0, 0),
				TabIndex = 1,
			};

			if (this.entityPrinter.EntityPrintingSettings.DocumentTypeSelected == DocumentType.Debug1 ||
				this.entityPrinter.EntityPrintingSettings.DocumentTypeSelected == DocumentType.Debug2)
			{
				{
					var frame = UIBuilder.CreateMiniToolbar (this.footer, 24);
					frame.PreferredWidth = 70;
					frame.Margins = new Margins (1, 10, 0, 0);
					frame.Dock = DockStyle.Right;

					this.debugPrevButton2 = new GlyphButton
					{
						Parent = frame,
						GlyphShape = Common.Widgets.GlyphShape.Minus,
						PreferredWidth = 20,
						PreferredHeight = 20,
						Dock = DockStyle.Left,
					};

					this.debugParam2 = new StaticText
					{
						Parent = frame,
						ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
						PreferredWidth = 30,
						PreferredHeight = 20,
						Dock = DockStyle.Fill,
					};

					this.debugNextButton2 = new GlyphButton
					{
						Parent = frame,
						GlyphShape = Common.Widgets.GlyphShape.Plus,
						PreferredWidth = 20,
						PreferredHeight = 20,
						Dock = DockStyle.Right,
					};
				}

				{
					var frame = UIBuilder.CreateMiniToolbar (this.footer, 24);
					frame.PreferredWidth = 70;
					frame.Margins = new Margins (0, 0, 0, 0);
					frame.Dock = DockStyle.Right;

					this.debugPrevButton1 = new GlyphButton
					{
						Parent = frame,
						GlyphShape = Common.Widgets.GlyphShape.Minus,
						PreferredWidth = 20,
						PreferredHeight = 20,
						Dock = DockStyle.Left,
					};

					this.debugParam1 = new StaticText
					{
						Parent = frame,
						ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
						PreferredWidth = 30,
						PreferredHeight = 20,
						Dock = DockStyle.Fill,
					};

					this.debugNextButton1 = new GlyphButton
					{
						Parent = frame,
						GlyphShape = Common.Widgets.GlyphShape.Plus,
						PreferredWidth = 20,
						PreferredHeight = 20,
						Dock = DockStyle.Right,
					};
				}

				this.UpdateDebug ();
			}

			{
				var frame = UIBuilder.CreateMiniToolbar (this.footer, 24);
				frame.Margins = new Margins (2, 20, 0, 0);
				frame.Dock = DockStyle.Right;

				this.zoom18Button = new Button
				{
					Parent = frame,
					ButtonStyle = Common.Widgets.ButtonStyle.ToolItem,
					AutoFocus = false,
					Text = "÷8",
					PreferredWidth = 20,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
				};

				this.zoom14Button = new Button
				{
					Parent = frame,
					ButtonStyle = Common.Widgets.ButtonStyle.ToolItem,
					AutoFocus = false,
					Text = "÷4",
					PreferredWidth = 20,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
				};

				this.zoom11Button = new Button
				{
					Parent = frame,
					ButtonStyle = Common.Widgets.ButtonStyle.ToolItem,
					AutoFocus = false,
					Text = "×1",
					PreferredWidth = 20,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
				};

				this.zoom21Button = new Button
				{
					Parent = frame,
					ButtonStyle = Common.Widgets.ButtonStyle.ToolItem,
					AutoFocus = false,
					Text = "×2",
					PreferredWidth = 20,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
				};

				this.zoom41Button = new Button
				{
					Parent = frame,
					ButtonStyle = Common.Widgets.ButtonStyle.ToolItem,
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

			this.placer = new PreviewOptimalPlacer (this.pagePreviews, this.entityPrinter.PageSize);

			this.UpdatePages ();
		}

		protected void SetupEvents(Window window)
		{
			this.pageScroller.ValueChanged += delegate
			{
				this.currentPage = (int) this.pageScroller.Value;
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
				this.debugPrevButton1.Clicked += new EventHandler<MessageEventArgs> (debugPrevButton1_Clicked);
				this.debugNextButton1.Clicked += new EventHandler<MessageEventArgs> (debugNextButton1_Clicked);
				this.debugPrevButton2.Clicked += new EventHandler<MessageEventArgs> (debugPrevButton2_Clicked);
				this.debugNextButton2.Clicked += new EventHandler<MessageEventArgs> (debugNextButton2_Clicked);
			}

			this.updateButton.Clicked += delegate
			{
				this.Update ();
			};

			this.printButton.Clicked += delegate
			{
				this.CloseDialog ();

				PrintEngine.Print (this.entities, this.entityPrinter);
			};

			this.closeButton.Clicked += delegate
			{
				this.CloseDialog ();
			};

			this.closeButton.Clicked += (sender, e) => this.CloseDialog ();
		}

		private void debugPrevButton1_Clicked(object sender, MessageEventArgs e)
		{
			this.entityPrinter.DebugParam1 -= GetStep(e);
			this.UpdateDebug ();
		}

		private void debugNextButton1_Clicked(object sender, MessageEventArgs e)
		{
			this.entityPrinter.DebugParam1 += GetStep (e);
			this.UpdateDebug ();
		}

		private void debugPrevButton2_Clicked(object sender, MessageEventArgs e)
		{
			this.entityPrinter.DebugParam2 -= GetStep (e);
			this.UpdateDebug ();
		}

		private void debugNextButton2_Clicked(object sender, MessageEventArgs e)
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

		private void UpdatePages()
		{
			this.UpdateZoom ();
			this.UpdatePreview ();
			this.UpdateButtons ();
		}

		private void UpdateZoom()
		{
			this.zoom18Button.ActiveState = this.currentZoom == 1.0/8.0 ? ActiveState.Yes : ActiveState.No;
			this.zoom14Button.ActiveState = this.currentZoom == 1.0/4.0 ? ActiveState.Yes : ActiveState.No;
			this.zoom11Button.ActiveState = this.currentZoom == 1       ? ActiveState.Yes : ActiveState.No;
			this.zoom21Button.ActiveState = this.currentZoom == 2       ? ActiveState.Yes : ActiveState.No;
			this.zoom41Button.ActiveState = this.currentZoom == 4       ? ActiveState.Yes : ActiveState.No;
		}

		private void UpdatePreview()
		{
			this.showedPageCount = (this.currentZoom < 1) ? (int) (1.0/this.currentZoom) : 1;

			this.currentPage = System.Math.Min (this.currentPage + this.showedPageCount, this.entityPrinter.PageCount ());
			this.currentPage = System.Math.Max (this.currentPage - this.showedPageCount, 0);

			this.pagePreviews.Clear ();
			this.previewFrame.Viewport.Children.Clear ();

			int pageRank = this.currentPage;

			for (int i = 0; i < this.showedPageCount; i++)
			{
				if (pageRank >= this.entityPrinter.PageCount ())
				{
					break;
				}

				var preview = new Widgets.PreviewEntity
				{
					Parent = this.previewFrame.Viewport,
					EntityPrinter = this.entityPrinter,
					Description = this.GetPrintersUsedDescription (pageRank),
					CurrentPage = pageRank++,
				};

				this.pagePreviews.Add (preview);
			}

			this.UpdatePagePreviewsGeometry ();

			this.pageScroller.MinValue = 0;
			this.pageScroller.MaxValue = System.Math.Max (this.entityPrinter.PageCount () - this.showedPageCount, 0);
			this.pageScroller.Resolution = 1;
			this.pageScroller.VisibleRangeRatio = System.Math.Min ((decimal) this.showedPageCount / (decimal) this.entityPrinter.PageCount (), 1);
			this.pageScroller.SmallChange = 1;
			this.pageScroller.LargeChange = 10;
			this.pageScroller.Value = this.currentPage;
		}

		private void UpdatePagePreviewsGeometry()
		{
			//	Positionne tous les Widgets.PreviewEntity, selon le parent this.previewFrame.
			if (this.currentZoom > 1)  // agrandissement ?
			{
				this.previewFrame.HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways;
				this.previewFrame.VerticalScrollerMode   = ScrollableScrollerMode.ShowAlways;
				this.previewFrame.PaintViewportFrame = true;

				this.pagePreviews[0].PreferredSize = this.placer.AdjustRatioPageSize (this.previewFrame.Client.Bounds.Size * this.currentZoom);
				this.pagePreviews[0].Dock = DockStyle.Left | DockStyle.Bottom;
			}
			else  // 1:1 ou réduction ?
			{
				this.previewFrame.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
				this.previewFrame.VerticalScrollerMode   = ScrollableScrollerMode.HideAlways;
				this.previewFrame.PaintViewportFrame = false;

				this.placer.AvailableSize = this.previewFrame.Client.Bounds.Size;
				this.placer.PageCount = this.pagePreviews.Count;
				this.placer.UpdateGeometry ();
			}
		}

		private void UpdateButtons()
		{
			int t = this.entityPrinter.PageCount ();
			int p = this.currentPage+1;

			if (this.showedPageCount <= 1)
			{

				this.pageRank.Text = string.Format ("{0} / {1}", p.ToString (), t.ToString ());
			}
			else
			{
				int q = System.Math.Min (p + this.showedPageCount-1, t);

				this.pageRank.Text = string.Format ("{0}..{1} / {2}", p.ToString (), q.ToString (), t.ToString ());
			}

	
			this.zoom14Button.Enable = t > 1;
			this.zoom18Button.Enable = t > 4;
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
						var pu = this.printerUnitList.Where (x => x.LogicalName == documentPrinter.LogicalPrinterName).FirstOrDefault();
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


		private void UpdateDebug()
		{
			this.debugParam1.Text = this.entityPrinter.DebugParam1.ToString ();
			this.debugParam2.Text = this.entityPrinter.DebugParam2.ToString ();

			if (this.pagePreviews.Count != 0)
			{
				this.pagePreviews[0].Invalidate ();
			}
		}


		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();

			this.application.DetachDialog (this);
		}



		#region IAttachedDialog Members

		public IEnumerable<AbstractEntity> Entities
		{
			get
			{
				return this.entities;
			}
		}

		public void Update()
		{
			//?this.entityPrinter.Clear ();
			//?this.entityPrinter.BuildSections ();

			//?this.preview.CurrentPage = System.Math.Min (this.preview.CurrentPage, this.entityPrinter.PageCount ()-1);
			//?this.UpdatePage ();
		}

		#endregion


		private readonly CoreApplication				application;
		private readonly IEnumerable<AbstractEntity>	entities;
		private readonly Printers.AbstractEntityPrinter	entityPrinter;

		private Scrollable								previewFrame;
		private List<Widgets.PreviewEntity>				pagePreviews;
		private PreviewOptimalPlacer					placer;

		private FrameBox								footer;

		private HScroller								pageScroller;
		private StaticText								pageRank;

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

		private Button									updateButton;
		private Button									printButton;
		private Button									closeButton;

		private double									currentZoom;
		private List<PrinterUnit>						printerUnitList;

		private int										currentPage;
		private int										showedPageCount;
	}
}
