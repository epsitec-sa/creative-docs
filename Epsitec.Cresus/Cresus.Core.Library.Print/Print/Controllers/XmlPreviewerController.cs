//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Print.Serialization;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Print.Controllers
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


		public XmlPreviewerController(IBusinessContext businessContext, List<DeserializedJob> jobs)
		{
			this.businessContext = businessContext;
			this.coreData = this.businessContext.Data;
			this.jobs = jobs;
			this.pages = Print.Common.GetDeserializedPages (this.jobs).ToList ();

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
				this.UpdatePreview ();
				this.UpdateGroups ();
				this.UpdateButtons ();
			};

			//	PagesToolbarBox.
			this.pagesToolbarBox.PreferredHeight = 24;

			{
				this.groupsToolbarBox = Library.UI.Toolkit.CreateMiniToolbar (this.pagesToolbarBox, 24);
				this.groupsToolbarBox.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
				this.groupsToolbarBox.Dock = DockStyle.Fill;
			}

			{
				var frame = Library.UI.Toolkit.CreateMiniToolbar (this.pagesToolbarBox, 24);
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

				ToolTip.Default.SetToolTip (this.zoom18Button, "Réduction 8 fois");
				ToolTip.Default.SetToolTip (this.zoom14Button, "Réduction 4 fois");
				ToolTip.Default.SetToolTip (this.zoom11Button, "Echelle optimale");
				ToolTip.Default.SetToolTip (this.zoom21Button, "Agrandissement 2 fois");
				ToolTip.Default.SetToolTip (this.zoom41Button, "Agrandissement 4 fois");
			}

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
			this.UpdateGroups ();
			this.UpdateButtons ();
			this.UpdateZoom ();
		}

		private void UpdatePreview()
		{
			int minimalHope = (this.currentZoom < 1) ? (int) (1.0/this.currentZoom) : 1;
			minimalHope = System.Math.Min (minimalHope, this.pages.Count);
			var additionnalSize = new Size (0, Widgets.XmlPrintedPagePreviewer.titleHeight);
			var placer = new Dialogs.OptimalPreviewPlacer (this.previewFrame.Client.Bounds, this.BoundsPageSize, additionnalSize, 5, minimalHope);
			this.showedPageCount = (this.currentZoom > 1) ? 1 : System.Math.Max (placer.Total, 1);

			this.currentPage = this.currentPage /this.showedPageCount * this.showedPageCount;

			this.pagePreviewers.Clear ();
			this.previewFrame.Viewport.Children.Clear ();

			for (int i = 0; i < this.showedPageCount; i++)
			{
				int pageRank = this.currentPage+i;

				if (pageRank >= this.pages.Count)
				{
					break;
				}

				var preview = new Widgets.XmlPrintedPagePreviewer (this.businessContext)
				{
					Parent = this.previewFrame.Viewport,
					Page = this.pages[pageRank],
				};

				this.pagePreviewers.Add (preview);

				pageRank++;
			}

			//	Positionne tous les Widgets.EntityPreviewer, selon le parent this.previewFrame.
			if (this.currentZoom > 1)  // agrandissement ?
			{
				this.previewFrame.HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways;
				this.previewFrame.VerticalScrollerMode   = ScrollableScrollerMode.ShowAlways;
				this.previewFrame.PaintViewportFrame = true;

				this.pagePreviewers[0].PreferredSize = placer.GetZoomedSize (this.pages[this.currentPage].ParentSection.PageSize, this.currentZoom);
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

		private void UpdateGroups()
		{
			this.groupsToolbarBox.Children.Clear ();  // supprime les boutons précédents

			if (this.showedPageCount != 0)
			{
				int total = System.Math.Max (((this.pages.Count+this.showedPageCount-1) / this.showedPageCount), 0);

				if (total > 1)
				{
					//	Met le bouton '<'.
					if (this.currentPage > 0)
					{
						var button = new GlyphButton
						{
							Parent = this.groupsToolbarBox,
							GlyphShape = GlyphShape.ArrowLeft,
							ButtonStyle = ButtonStyle.ToolItem,
							Dock = DockStyle.Left,
						};

						button.Clicked += delegate
						{
							this.currentPage -= this.showedPageCount;
							this.UpdatePages (rebuild: false);
						};

						ToolTip.Default.SetToolTip (button, (this.showedPageCount == 1) ? "Montre la page précédente" : "Montre les pages précédentes");
					}

					//	Met les boutons pour les groupes de pages.
					for (int i = 0; i < total; i++)
					{
						int from = i*this.showedPageCount+1;
						int to = System.Math.Min (i*this.showedPageCount+this.showedPageCount, this.pages.Count);

						string textSep    = (from == to-1) ? ", " : "..";  // "4, 5" ou "4..7"
						string tooltipSep = (from == to-1) ? "et" : "à";   // "4 et 5" ou "4 à 7"

						string text, tooltip;

						if (from == to)
						{
							text    = from.ToString ();
							tooltip = string.Format ("Montre la page {0}", from.ToString ());
						}
						else
						{
							text    = string.Concat (from.ToString (), textSep, to.ToString ());
							tooltip = string.Format ("Montre les pages {0} {1} {2}", from.ToString (), tooltipSep, to.ToString ());
						}

						int cp = i*this.showedPageCount;

						var button = new Button
						{
							Parent = this.groupsToolbarBox,
							Text = text,
							Name = cp.ToString (System.Globalization.CultureInfo.InvariantCulture),
							ButtonStyle = ButtonStyle.ToolItem,
							AutoFocus = false,
							ActiveState = (cp == this.currentPage) ? ActiveState.Yes : ActiveState.No,
							Dock = DockStyle.Fill,
						};

						button.Clicked += delegate
						{
							this.currentPage = int.Parse (button.Name);
							this.UpdatePages (rebuild: false);
						};

						ToolTip.Default.SetToolTip (button, tooltip);
					}

					//	Met le bouton '>'.
					if (this.currentPage < this.pages.Count-this.showedPageCount)
					{
						var button = new GlyphButton
						{
							Parent = this.groupsToolbarBox,
							GlyphShape = GlyphShape.ArrowRight,
							ButtonStyle = ButtonStyle.ToolItem,
							Dock = DockStyle.Right,
						};

						button.Clicked += delegate
						{
							this.currentPage += this.showedPageCount;
							this.UpdatePages (rebuild: false);
						};

						ToolTip.Default.SetToolTip (button, (this.showedPageCount == 1) ? "Montre la page suivante" : "Montre les pages suivantes");
					}
				}
			}
		}

		private void UpdateButtons()
		{
			this.zoom14Button.Enable = this.pages.Count > 1;
			this.zoom18Button.Enable = this.pages.Count > 4;
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


		private readonly IBusinessContext						businessContext;
		private readonly CoreData								coreData;
		private readonly List<DeserializedJob>					jobs;
		private readonly List<Widgets.XmlPrintedPagePreviewer>	pagePreviewers;

		private List<DeserializedPage>							pages;

		private FrameBox										previewBox;
		private FrameBox										pagesToolbarBox;

		private Scrollable										previewFrame;

		private FrameBox										groupsToolbarBox;

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
