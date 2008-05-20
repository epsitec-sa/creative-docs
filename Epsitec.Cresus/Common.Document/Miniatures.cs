using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Cette classe gère le panneau des vues des pages miniatures.
	/// </summary>
	public class Miniatures
	{
		public Miniatures(Document document)
		{
			this.document = document;

			this.panelWidth = 160;
			this.pages = new List<int>();
		}


		public void CreateInterface(PanePage parentPanel, PaneBook parentPane)
		{
			//	Crée l'interface nécessaire pour les pages miniatures.
			this.parentPanel = parentPanel;

			StaticText helpText = new StaticText(this.parentPanel);
			helpText.Text = Res.Strings.Miniatures.Title;
			helpText.Dock = DockStyle.Top;
			helpText.Margins = new Margins(0, 0, 4, 7);

			HToolBar toolbar = new HToolBar(this.parentPanel);
			toolbar.Dock = DockStyle.Top;
			toolbar.Margins = new Margins(0, 0, 0, 5);

			this.buttonAll = new IconButton();
			this.buttonAll.Clicked += new MessageEventHandler(this.HandleButtonAll);
			toolbar.Items.Add(this.buttonAll);

			GlyphButton buttonMenu = new GlyphButton();
			buttonMenu.ButtonStyle = ButtonStyle.ComboItem;
			buttonMenu.GlyphShape = GlyphShape.Menu;
			buttonMenu.AutoFocus = false;
			buttonMenu.Clicked += new MessageEventHandler(this.HandleButtonMenu);
			ToolTip.Default.SetToolTip(buttonMenu, Res.Strings.Miniatures.Button.Menu.Tooltip);
			toolbar.Items.Add(buttonMenu);

			this.slider = new HSlider();
			this.slider.MinValue = 1M;
			this.slider.MaxValue = 4M;
			this.slider.SmallChange = 1M;
			this.slider.LargeChange = 1M;
			this.slider.Resolution = 0.01M;
			this.slider.Value = 2M;
			this.slider.PreferredWidth = 70;
			this.slider.PreferredHeight = 22-4-4;
			this.slider.Margins = new Margins(8, 8, 4, 4);
			this.slider.ValueChanged += new EventHandler(this.HandleSliderValueChanged);
			ToolTip.Default.SetToolTip(this.slider, Res.Strings.Miniatures.Slider.Tooltip);
			toolbar.Items.Add(this.slider);

			this.panel = new Widgets.FlowPanel(this.parentPanel);
			this.panel.Dock = DockStyle.Fill;

			this.buttonExpand = new GlyphButton(parentPane);
			this.buttonExpand.GlyphShape = GlyphShape.ArrowLeft;
			this.buttonExpand.PreferredSize = new Size(14, 14);
			this.buttonExpand.Anchor = AnchorStyles.TopRight;
			this.buttonExpand.Clicked += new MessageEventHandler(this.HandleButtonExpand);
			ToolTip.Default.SetToolTip(this.buttonExpand, Res.Strings.Miniatures.Button.Expand.Tooltip);

			this.UpdateButtons();
		}


		public void UpdatePageAfterChanging()
		{
			//	Adapte les miniatures après un changement de page (création d'une
			//	nouvelle page, suppression d'une page, etc.).
			int total = this.TotalPages;

			if (this.showAllPages)
			{
				this.pages.Clear();
				for (int i=0; i<total; i++)
				{
					this.pages.Add(i);
				}
			}
			else
			{
				while (this.pages.Count > total)  // trop de miniatures ?
				{
					this.pages.RemoveAt(this.pages.Count-1);
				}
			}

			this.Create();
		}


		private void HandleButtonExpand(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton "<" ou ">" pour montrer/cacher le panneau des miniatures est cliqué.
			if (this.parentPanel.PaneAbsoluteSize == 0)  // panneau caché ?
			{
				this.parentPanel.PaneAbsoluteSize = this.panelWidth;  // remet la dernière largeur
				this.buttonExpand.GlyphShape = GlyphShape.ArrowRight;
			}
			else
			{
				this.panelWidth = this.parentPanel.PaneAbsoluteSize;  // mémorise la largeur
				this.parentPanel.PaneAbsoluteSize = 0;  // cache le panneau
				this.buttonExpand.GlyphShape = GlyphShape.ArrowLeft;
			}
		}

		private void HandleButtonMenu(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton pour ajouter une nouvelle miniature est cliqué.
			AbstractButton button = sender as AbstractButton;
			if (button == null)  return;

			UndoableList pages = this.document.DocumentObjects;  // liste des pages
			VMenu menu = Objects.Page.CreateMenu(pages, this.pages, null, this.HandleButtonMenuPressed);
			menu.Host = button.Window;
			TextFieldCombo.AdjustComboSize(button, menu, false);
			menu.ShowAsComboList(button, Point.Zero, button);
		}

		private void HandleButtonMenuPressed(object sender, MessageEventArgs e)
		{
			//	Appelé lorsqu'une page est choisie dans le menu des pages miniatures.
			MenuItem item = sender as MenuItem;
			int page = System.Convert.ToInt32(item.Name);

			if (this.pages.Contains(page))
			{
				this.pages.Remove(page);  // ne montre plus cette page
			}
			else
			{
				this.pages.Add(page);  // montre cette page
				this.pages.Sort();
			}

			this.showAllPages = false;

			this.Create();
		}

		private void HandleButtonAll(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton pour montrer/cacher toutes les miniatures est cliqué.
			if (this.pages.Count == 0)  // montrer tout ?
			{
				int total = this.TotalPages;
				for (int i=0; i<total; i++)
				{
					this.pages.Add(i);
				}

				this.showAllPages = true;
			}
			else  // cacher tout ?
			{
				this.pages.Clear();
				this.showAllPages = false;
			}

			this.Create();
		}

		private void HandleSliderValueChanged(object sender)
		{
			//	Appelé lorsque la taille des miniatures a changé.
			this.Create();
		}


		protected void Create()
		{
			//	Crée toutes les pages miniatures.
			this.Clear();  // supprime les miniatures existantes
			double zoom = (double) this.slider.Value;

			foreach (int page in this.pages)
			{
				Size pageSize = this.document.GetPageSize(page);
				double h = 50*zoom;
				double w = System.Math.Ceiling(h*pageSize.Width/pageSize.Height);

				FrameBox box = new FrameBox(this.panel);
				box.PreferredSize = new Size(w, h+12);

				//	Crée la vue de la page miniature.
				Viewer viewer = new Viewer(this.document);
				viewer.SetParent(box);
				viewer.Dock = DockStyle.Fill;
				viewer.IsDocumentPreview = true;
				viewer.DrawingContext.LayerDrawingMode = LayerDrawingMode.ShowInactive;
				viewer.Margins = new Margins(1, 1, 1, 1);
				viewer.PreferredSize = new Size(w, h);
				viewer.DrawingContext.InternalPageLayer(page, 0);
				viewer.Window.ForceLayout();
				viewer.DrawingContext.ZoomPageAndCenter();
				viewer.Clicked += new MessageEventHandler(this.HandleViewerClicked);
				this.document.Modifier.AttachViewer(viewer);
				this.document.Notifier.NotifyArea(viewer);

				//	Crée la légende en bas, avec le numéro+nom de la page.
				StaticText label = new StaticText(box);
				label.PreferredSize = new Size(w, 12);
				label.Dock = DockStyle.Bottom;
				label.ContentAlignment = ContentAlignment.TopCenter;
				label.Text = string.Concat("<font size=\"75%\">", this.PageName(page), "</font>");
			}

			this.UpdateButtons();
		}

		protected void Clear()
		{
			//	Supprime toutes les miniatures.
			List<Viewer> viewers = new List<Viewer>();
			foreach (Viewer viewer in this.document.Modifier.Viewers)
			{
				if (viewer.IsDocumentPreview)
				{
					viewers.Add(viewer);
				}
			}

			foreach (Viewer viewer in viewers)
			{
				this.document.Modifier.DetachViewer(viewer);
			}

			this.panel.Children.Clear();
		}


		private void HandleViewerClicked(object sender, MessageEventArgs e)
		{
			//	Une page miniature a été cliquée.
			Viewer viewer = sender as Viewer;
			int page = viewer.DrawingContext.CurrentPage;

			this.document.Modifier.ActiveViewer.DrawingContext.CurrentPage = page;
		}


		protected void UpdateButtons()
		{
			//	Met à jour les boutons.
			if (this.pages.Count == 0)
			{
				this.buttonAll.IconName = Misc.Icon("MiniaturesAllShow");
				ToolTip.Default.SetToolTip(this.buttonAll, Res.Strings.Miniatures.Button.AllShow.Tooltip);
			}
			else
			{
				this.buttonAll.IconName = Misc.Icon("MiniaturesAllHide");
				ToolTip.Default.SetToolTip(this.buttonAll, Res.Strings.Miniatures.Button.AllHide.Tooltip);
			}
		}


		protected string PageName(int rank)
		{
			//	Retourne le nom le plus complet possible d'une page.
			UndoableList doc = this.document.DocumentObjects;
			Objects.Page page = doc[rank] as Objects.Page;

			string longName = page.LongName;
			if (string.IsNullOrEmpty(longName))
			{
				return page.ShortName;
			}
			else
			{
				return string.Concat(page.ShortName, " ", page.LongName);
			}
		}

		protected int TotalPages
		{
			//	Retourne le nombre total de pages.
			get
			{
				return this.document.Modifier.ActiveViewer.DrawingContext.TotalPages();
			}
		}


		protected Document						document;
		protected List<int>						pages;
		protected PanePage						parentPanel;
		protected IconButton					buttonAll;
		protected GlyphButton					buttonExpand;
		protected HSlider						slider;
		protected Widgets.FlowPanel				panel;
		protected double						panelWidth;
		protected bool							showAllPages;
	}
}
