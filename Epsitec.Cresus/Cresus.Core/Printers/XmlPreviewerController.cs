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
	/// contiendra un ou plusieurs Widgets.XmlEntityPreviewer, et la barre d'outil tout ce qu'il faut pour choisir
	/// la ou les pages à afficher, ainsi que le zoom (réduction ou agrandissement).
	/// </summary>
	public class XmlPreviewerController
	{
		static XmlPreviewerController()
		{
			PrintEngine.Setup ();
		}


		public XmlPreviewerController(List<DeserializedJob> jobs)
		{
			this.jobs = jobs;
			this.pages = Printers.Common.GetDeserializedPages (this.jobs).ToList ();

			this.currentZoom = 1;

			this.pagePreviewers = new List<Widgets.XmlPrintedPagePreviewer> ();
		}


		public void CreateUI(FrameBox previewBox, FrameBox pagesToolbarBox)
		{
			//	Crée l'interface dans deux boîtes, l'une pour le ou les aperçus (previewBox) et l'autre pour choisir
			//	la page et le zoom (toolbarBox).
			this.previewBox  = previewBox;
			this.pagesToolbarBox = pagesToolbarBox;

			this.previewFrame = new Scrollable
			{
				Parent = this.previewBox,
				ScrollWithHand = true,
				Dock = DockStyle.Fill,
			};

			this.previewFrame.Viewport.IsAutoFitting = true;

			this.previewFrame.SizeChanged += delegate
			{
				this.UpdatePagePreviewsGeometry ();
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
				this.currentPage = (int) this.pageSlider.Value;
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
			this.UpdatePreview ();
			this.UpdatePagePreviewsGeometry ();
			this.UpdatePageSlider ();
			this.UpdateButtons ();
			this.UpdateZoom ();
		}

		private void UpdatePreview()
		{
			this.showedPageCount = (this.currentZoom < 1) ? (int) (1.0/this.currentZoom) : 1;

			this.currentPage = System.Math.Min (this.currentPage + this.showedPageCount, this.pages.Count);
			this.currentPage = System.Math.Max (this.currentPage - this.showedPageCount, 0);

			this.pagePreviewers.Clear ();
			this.previewFrame.Viewport.Children.Clear ();

			for (int i = 0; i < this.showedPageCount; i++)
			{
				int pageRank = this.currentPage+i;

				if (pageRank >= this.pages.Count)
				{
					break;
				}

				var preview = new Widgets.XmlPrintedPagePreviewer
				{
					Parent = this.previewFrame.Viewport,
					Page = this.pages[pageRank],
				};

				this.pagePreviewers.Add (preview);

				pageRank++;
			}
		}

		private void UpdatePagePreviewsGeometry()
		{
			//	Positionne tous les Widgets.EntityPreviewer, selon le parent this.previewFrame.
			this.placer = new Dialogs.OptimalPreviewPlacer<Widgets.XmlPrintedPagePreviewer> (this.pagePreviewers);
			this.placer.PageSize = this.BoundsPageSize;

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
			this.pageSlider.MaxValue = System.Math.Max (this.pages.Count - this.showedPageCount, 0);
			this.pageSlider.Resolution = 1;
			//?this.pageSlider.VisibleRangeRatio = System.Math.Min ((decimal) this.showedPageCount / (decimal) this.pages.Count, 1);
			this.pageSlider.SmallChange = 1;
			this.pageSlider.LargeChange = 10;
			this.pageSlider.Value = this.currentPage;
		}

		private void UpdateButtons()
		{
			int t = this.pages.Count;
			int p = this.currentPage + 1;
			int q = this.currentPage+this.showedPageCount-1 + 1;

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

		private void UpdateZoom()
		{
			this.zoom18Button.ActiveState = this.currentZoom == 1.0/8.0 ? ActiveState.Yes : ActiveState.No;
			this.zoom14Button.ActiveState = this.currentZoom == 1.0/4.0 ? ActiveState.Yes : ActiveState.No;
			this.zoom11Button.ActiveState = this.currentZoom == 1       ? ActiveState.Yes : ActiveState.No;
			this.zoom21Button.ActiveState = this.currentZoom == 2       ? ActiveState.Yes : ActiveState.No;
			this.zoom41Button.ActiveState = this.currentZoom == 4       ? ActiveState.Yes : ActiveState.No;
		}

		private Size BoundsPageSize
		{
			get
			{
				double maxWidth  = 0;
				double maxHeight = 0;

				foreach (var page in this.pages)
				{
					if (maxWidth < page.ParentSection.PageSize.Width)
					{
						maxWidth = page.ParentSection.PageSize.Width;
					}

					if (maxHeight < page.ParentSection.PageSize.Height)
					{
						maxHeight = page.ParentSection.PageSize.Height;
					}
				}

				return new Size (maxWidth, maxHeight);
			}
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


		private readonly List<DeserializedJob>					jobs;
		private readonly List<Widgets.XmlPrintedPagePreviewer>	pagePreviewers;

		private List<Printers.DeserializedPage>					pages;

		private FrameBox										previewBox;
		private FrameBox										pagesToolbarBox;

		private Scrollable										previewFrame;
		private Dialogs.OptimalPreviewPlacer<Widgets.XmlPrintedPagePreviewer> placer;

		private StaticText										pageRank;
		private HSlider											pageSlider;

		private Button											zoom18Button;
		private Button											zoom14Button;
		private Button											zoom11Button;
		private Button											zoom21Button;
		private Button											zoom41Button;

		private double											currentZoom;
		private int												currentPage;
		private int												showedPageCount;
	}
}
