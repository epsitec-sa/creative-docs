using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// Cette classe gère le panneau des vues des calques miniatures.
	/// </summary>
	public class LayerMiniatures
	{
		public LayerMiniatures(Document document)
		{
			this.document = document;

			this.layers = new List<int>();
		}


		public void CreateInterface(FrameBox parentPanel)
		{
			//	Crée l'interface nécessaire pour les calques miniatures dans un panneau dédié.
			this.parentPanel = parentPanel;

			//	Crée puis peuple la barre d'outils.
			HToolBar toolbar = new HToolBar(this.parentPanel);
			toolbar.Dock = DockStyle.Top;
			toolbar.Margins = new Margins(0, 0, 0, -1);

			this.slider = new HSlider(toolbar);
			this.slider.MinValue = 1M;
			this.slider.MaxValue = 4M;
			this.slider.SmallChange = 1M;
			this.slider.LargeChange = 1M;
			this.slider.Resolution = 0.01M;
			this.slider.Value = 2M;
			this.slider.PreferredWidth = 70;
			this.slider.PreferredHeight = 22-4-4;
			this.slider.Margins = new Margins(0, 0, 4, 4);
			this.slider.Dock = DockStyle.Right;
			this.slider.ValueChanged += new EventHandler(this.HandleSliderValueChanged);
			ToolTip.Default.SetToolTip(this.slider, Res.Strings.Miniatures.Slider.Tooltip);

			//	Crée la zone contenant les miniatures.
			this.scrollable = new Scrollable(this.parentPanel);
			this.scrollable.Dock = DockStyle.Fill;
			this.scrollable.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
			this.scrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.scrollable.Viewport.IsAutoFitting = true;
			this.scrollable.PaintForegroundFrame = true;
			this.scrollable.ForegroundFrameMargins = new Margins(0, 1, 0, 0);
		}


		public void UpdateLayerAfterChanging()
		{
			//	Adapte les miniatures après un changement de calque (création d'un
			//	nouveau calque, suppression d'un calque, etc.).
			this.layers.Clear();

			int total = this.TotalLayers;
			for (int i=total-1; i>=0; i--)
			{
				this.layers.Add(i);
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
			//	Crée tous les calques miniatures.
			if (this.parentPanel.Window == null)
			{
				return;
			}

			double offsetY = this.scrollable.ViewportOffsetY;

			this.Clear();  // supprime les miniatures existantes
			double zoom = (double) this.slider.Value;
			int currentPage = this.CurrentPage;
			int currentLayer = this.CurrentLayer;
			Size pageSize = this.document.GetPageSize(currentPage);
			double posY = 0;
			double requiredWidth = 0;

			foreach (int layer in this.layers)
			{
				double w = System.Math.Ceiling(zoom*pageSize.Width*50/2970);
				double h = System.Math.Ceiling(zoom*pageSize.Height*50/2970);

				string layerName = this.LayerName(layer);

				FrameBox box = new FrameBox(this.scrollable.Viewport);
				box.Index = layer;
				box.PreferredSize = new Size(w, h+LayerMiniatures.labelHeight);
				box.Padding = new Margins(2, 2, 2, 2);
				box.Anchor = AnchorStyles.TopLeft;
				box.Margins = new Margins(0, 0, posY, 0);
				box.Clicked += new MessageEventHandler(this.HandleLayerBoxClicked);
				ToolTip.Default.SetToolTip(box, layerName);

				//	Crée la vue du calque miniature.
				Viewer viewer = new Viewer(this.document);
				viewer.SetParent(box);
				viewer.Dock = DockStyle.Fill;
				viewer.PreferredSize = new Size(w, h);
				viewer.IsDocumentPreview = true;
				viewer.DrawingContext.LayerDrawingMode = LayerDrawingMode.ShowInactive;
				viewer.DrawingContext.InternalPageLayer(currentPage, layer);
				viewer.DrawingContext.GridShow = false;
				viewer.DrawingContext.GuidesShow = false;
				viewer.Window.ForceLayout();
				viewer.DrawingContext.ZoomPageAndCenter();
				this.document.Modifier.AttachViewer(viewer);
				this.document.Notifier.NotifyArea(viewer);

				//	Crée la légende en bas, avec le numéro+nom du calque.
				StaticText label = new StaticText(box);
				label.PreferredSize = new Size(w, LayerMiniatures.labelHeight);
				label.Dock = DockStyle.Bottom;
				label.ContentAlignment = ContentAlignment.TopCenter;
				label.Text = Misc.FontSize(layerName, 0.75);

				if (layer == currentLayer)
				{
					box.BackColor = viewer.DrawingContext.HiliteOutlineColor;
				}

				posY += h+LayerMiniatures.labelHeight+2;
				requiredWidth = System.Math.Max(requiredWidth, w);
			}

			this.scrollable.ViewportOffsetY = offsetY;
			this.parentPanel.PreferredWidth = requiredWidth+3+23;
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

			this.scrollable.Viewport.Children.Clear();
		}


		private void HandleLayerBoxClicked(object sender, MessageEventArgs e)
		{
			//	Un calque miniature a été cliquée.
			FrameBox box = sender as FrameBox;
			this.CurrentLayer = box.Index;  // rend le calque cliqué actif
		}


		protected string LayerName(int rank)
		{
			//	Retourne le nom le plus complet possible du calque, constitué du numéro
			//	suivi de son éventuel nom.
			return Objects.Layer.ShortName(rank);
		}

		protected int TotalLayers
		{
			//	Retourne le nombre total de calques.
			get
			{
				return this.document.Modifier.ActiveViewer.DrawingContext.TotalLayers();
			}
		}

		protected int CurrentPage
		{
			//	Retourne le rang de la page courante.
			get
			{
				return this.document.Modifier.ActiveViewer.DrawingContext.CurrentPage;
			}
		}

		protected int CurrentLayer
		{
			//	Retourne le rang du calque courant.
			get
			{
				return this.document.Modifier.ActiveViewer.DrawingContext.CurrentLayer;
			}
			set
			{
				this.document.Modifier.ActiveViewer.DrawingContext.CurrentLayer = value;
			}
		}


		protected static readonly double		labelHeight = 12;

		protected Document						document;
		protected List<int>						layers;
		protected List<Objects.Layer>			layersObject;
		protected FrameBox						parentPanel;
		protected HSlider						slider;
		protected Scrollable					scrollable;
	}
}
