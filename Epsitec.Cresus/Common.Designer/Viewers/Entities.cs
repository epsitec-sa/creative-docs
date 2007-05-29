using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Entities : AbstractCaptions2
	{
		public Entities(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			this.lastGroup.Dock = DockStyle.Top;

			this.hsplitter = new HSplitter(this.lastPane);
			this.hsplitter.Dock = DockStyle.Top;

			Widget editorGroup = new Widget(this.lastPane);
			editorGroup.Padding = new Margins(10, 10, 10, 10);
			editorGroup.Dock = DockStyle.Fill;

			//	Crée les grands blocs de widgets.
			Widget band = new Widget(editorGroup);
			band.Dock = DockStyle.Fill;

			this.editor = new EntitiesEditor.Editor(band);
			this.editor.Dock = DockStyle.Fill;
			this.editor.AreaSize = this.areaSize;
			this.editor.Zoom = this.zoom;
			this.editor.SizeChanged += new EventHandler<DependencyPropertyChangedEventArgs>(this.HandleEditorSizeChanged);

			this.vscroller = new VScroller(band);
			this.vscroller.IsInverted = true;
			this.vscroller.Dock = DockStyle.Right;
			this.vscroller.ValueChanged += new EventHandler(this.HandleScrollerValueChanged);

			this.toolbar = new HToolBar(editorGroup);
			this.toolbar.Dock = DockStyle.Bottom;
			this.toolbar.Margins = new Margins(0, 0, 5, 0);

			this.hscroller = new HScroller(editorGroup);
			this.hscroller.Margins = new Margins(0, this.vscroller.PreferredWidth, 0, 0);
			this.hscroller.Dock = DockStyle.Bottom;
			this.hscroller.ValueChanged += new EventHandler(this.HandleScrollerValueChanged);

			//	Peuple la toolbar.
			this.buttonZoomMin = new IconButton(this.toolbar);
			this.buttonZoomMin.IconName = Misc.Icon("ZoomMin");
			this.buttonZoomMin.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonZoomMin.AutoFocus = false;
			this.buttonZoomMin.Dock = DockStyle.Left;
			this.buttonZoomMin.Clicked += new MessageEventHandler(this.HandleButtonZoomClicked);
			ToolTip.Default.SetToolTip(this.buttonZoomMin, "Zoom minimal");

			this.buttonZoomDefault = new IconButton(this.toolbar);
			this.buttonZoomDefault.IconName = Misc.Icon("ZoomDefault");
			this.buttonZoomDefault.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonZoomDefault.AutoFocus = false;
			this.buttonZoomDefault.Dock = DockStyle.Left;
			this.buttonZoomDefault.Clicked += new MessageEventHandler(this.HandleButtonZoomClicked);
			ToolTip.Default.SetToolTip(this.buttonZoomDefault, "Zoom par défaut (1:1)");

			this.buttonZoomMax = new IconButton(this.toolbar);
			this.buttonZoomMax.IconName = Misc.Icon("ZoomMax");
			this.buttonZoomMax.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonZoomMax.AutoFocus = false;
			this.buttonZoomMax.Dock = DockStyle.Left;
			this.buttonZoomMax.Clicked += new MessageEventHandler(this.HandleButtonZoomClicked);
			ToolTip.Default.SetToolTip(this.buttonZoomMax, "Zoom maximal");

			this.fieldZoom = new StatusField(this.toolbar);
			this.fieldZoom.PreferredWidth = 50;
			this.fieldZoom.Margins = new Margins(5, 5, 1, 1);
			this.fieldZoom.Dock = DockStyle.Left;
			this.fieldZoom.Clicked += new MessageEventHandler(this.HandleFieldZoomClicked);
			ToolTip.Default.SetToolTip(this.fieldZoom, "Cliquez pour choisir le zoom dans un menu");

			this.sliderZoom = new HSlider(this.toolbar);
			this.sliderZoom.MinValue = (decimal) 0.2;
			this.sliderZoom.MaxValue = (decimal) 2.0;
			this.sliderZoom.SmallChange = (decimal) 0.1;
			this.sliderZoom.LargeChange = (decimal) 0.2;
			this.sliderZoom.Resolution = (decimal) 0.01;
			this.sliderZoom.PreferredWidth = 90;
			this.sliderZoom.Margins = new Margins(0, 0, 4, 4);
			this.sliderZoom.Dock = DockStyle.Left;
			this.sliderZoom.ValueChanged += new EventHandler(this.HandleSliderZoomValueChanged);
			ToolTip.Default.SetToolTip(this.sliderZoom, "Choix du zoom");

			this.AreaSize = new Size(1000, 1000);
			this.Zoom = 1;

			//	Provisoire:
			EntitiesEditor.ObjectBox box;

			box = new EntitiesEditor.ObjectBox(this.editor);
			box.Title = "Facture";
			box.SetContent("Numéro de facture;Date;Client;Articles;TVA;Rabais;Frais de port");
			this.editor.AddBox(box);

			box = new EntitiesEditor.ObjectBox(this.editor);
			box.Title = "Client";
			box.SetContent("Numéro de client;Titre;Nom;Prénom;Entreprise;Adresse;NPA;Ville;Pays;Téléphone professionnel;Téléphone privé;Téléphone mobile;E-mail professionnel;E-mail privé;Site web");
			this.editor.AddBox(box);

			box = new EntitiesEditor.ObjectBox(this.editor);
			box.Title = "Article";
			box.SetContent("Numéro d'article;Désignation;Quantité;Prix d'achat;Prix de vente");
			this.editor.AddBox(box);

			box = new EntitiesEditor.ObjectBox(this.editor);
			box.Title = "Rabais";
			box.SetContent("Normal;Revendeur;Grossiste");
			this.editor.AddBox(box);

			this.editor.AddConnection(new EntitiesEditor.ObjectConnection(this.editor));
			this.editor.AddConnection(new EntitiesEditor.ObjectConnection(this.editor));
			this.editor.AddConnection(new EntitiesEditor.ObjectConnection(this.editor));

			this.editor.UpdateGeometry();
			this.UpdateAll();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}


		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Entities;
			}
		}


		protected Size AreaSize
		{
			//	Dimensions de la surface pour représenter les boîtes et les liaisons.
			get
			{
				return this.areaSize;
			}
			set
			{
				if (this.areaSize != value)
				{
					this.areaSize = value;

					this.editor.AreaSize = this.areaSize;
					this.UpdateScroller();
				}
			}
		}

		public double Zoom
		{
			//	Zoom pour représenter les boîtes et les liaisons.
			get
			{
				return this.zoom;
			}
			set
			{
				if (this.zoom != value)
				{
					this.zoom = value;

					this.editor.Zoom = this.zoom;

					this.fieldZoom.Text = string.Concat(System.Math.Floor(this.zoom*100).ToString(), "%");
					this.sliderZoom.Value = (decimal) this.zoom;

					this.buttonZoomMin.ActiveState     = (this.zoom == 0.2) ? ActiveState.Yes : ActiveState.No;
					this.buttonZoomDefault.ActiveState = (this.zoom == 1.0) ? ActiveState.Yes : ActiveState.No;
					this.buttonZoomMax.ActiveState     = (this.zoom == 2.0) ? ActiveState.Yes : ActiveState.No;

					this.UpdateScroller();
				}
			}
		}

		protected void UpdateScroller()
		{
			double w = this.areaSize.Width*this.zoom - this.editor.Client.Size.Width;
			if (w <= 0)
			{
				this.hscroller.Enable = false;
			}
			else
			{
				this.hscroller.Enable = true;
				this.hscroller.MinValue = (decimal) 0;
				this.hscroller.MaxValue = (decimal) w;
				this.hscroller.SmallChange = (decimal) (w/10);
				this.hscroller.LargeChange = (decimal) (w/5);
				this.hscroller.VisibleRangeRatio = (decimal) (this.editor.Client.Size.Width / (this.areaSize.Width*this.zoom));
			}

			double h = this.areaSize.Height*this.zoom - this.editor.Client.Size.Height;
			if (h <= 0)
			{
				this.vscroller.Enable = false;
			}
			else
			{
				this.vscroller.Enable = true;
				this.vscroller.MinValue = (decimal) 0;
				this.vscroller.MaxValue = (decimal) h;
				this.vscroller.SmallChange = (decimal) (h/10);
				this.vscroller.LargeChange = (decimal) (h/5);
				this.vscroller.VisibleRangeRatio = (decimal) (this.editor.Client.Size.Height / (this.areaSize.Height*this.zoom));
			}

			this.HandleScrollerValueChanged(null);
		}


		private void HandleEditorSizeChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			//	Appelé lorsque la taille de la fenêtre de l'éditeur change.
			this.UpdateScroller();
		}

		private void HandleScrollerValueChanged(object sender)
		{
			//	Appelé lorsqu'un ascenseur a été bougé.
			double ox = 0;
			if (this.hscroller.IsEnabled)
			{
				ox = (double) this.hscroller.Value/this.zoom;
			}

			double oy = 0;
			if (this.vscroller.IsEnabled)
			{
				oy = (double) this.vscroller.Value/this.zoom;
			}

			this.editor.AreaOffset = new Point(ox, oy);
		}

		private void HandleButtonZoomClicked(object sender, MessageEventArgs e)
		{
			if (sender == this.buttonZoomMin)
			{
				this.Zoom = 0.2;
			}

			if (sender == this.buttonZoomDefault)
			{
				this.Zoom = 1.0;
			}
			
			if (sender == this.buttonZoomMax)
			{
				this.Zoom = 2.0;
			}
		}

		private void HandleFieldZoomClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le champ du zoom a été cliqué.
			StatusField sf = sender as StatusField;
			if (sf == null)  return;
			VMenu menu = EntitiesEditor.ZoomMenu.CreateZoomMenu(this.zoom, 1.0, null);
			menu.Host = sf.Window;
			TextFieldCombo.AdjustComboSize(sf, menu, false);
			menu.ShowAsComboList(sf, Point.Zero, sf);
		}

		private void HandleSliderZoomValueChanged(object sender)
		{
			//	Appelé lorsque le slider du zoom a été bougé.
			HSlider slider = sender as HSlider;
			this.Zoom = (double) slider.Value;
		}


		protected HSplitter hsplitter;
		protected EntitiesEditor.Editor editor;
		protected VScroller vscroller;
		protected HScroller hscroller;
		protected Size areaSize;
		protected double zoom;
		protected HToolBar toolbar;
		protected IconButton buttonZoomMin;
		protected IconButton buttonZoomDefault;
		protected IconButton buttonZoomMax;
		protected StatusField fieldZoom;
		protected HSlider sliderZoom;
	}
}
