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
	public class Entities : Abstract2
	{
		public Entities(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			this.scrollable.Visibility = false;

			//	Crée les grands blocs de widgets.
			Widget band = new Widget(this.lastPane);
			band.Dock = DockStyle.Fill;

			this.editor = new EntitiesEditor.Editor(band);
			this.editor.Dock = DockStyle.Fill;
			this.editor.AreaSize = this.areaSize;
			this.editor.Zoom = this.zoom;
			this.editor.SizeChanged += new EventHandler<DependencyPropertyChangedEventArgs>(this.HandleEditorSizeChanged);

			this.vscroler = new VScroller(band);
			this.vscroler.IsInverted = true;
			this.vscroler.Dock = DockStyle.Right;

			this.toolbar = new HToolBar(this.lastPane);
			this.toolbar.Dock = DockStyle.Bottom;
			this.toolbar.Margins = new Margins(0, 0, 5, 0);

			this.hscroler = new HScroller(this.lastPane);
			this.hscroler.Margins = new Margins(0, this.vscroler.PreferredWidth, 0, 0);
			this.hscroler.Dock = DockStyle.Bottom;

			//	Peuple la toolbar.
			StaticText stz = new StaticText(this.toolbar);
			stz.Text = "Zoom";
			stz.ContentAlignment = ContentAlignment.MiddleRight;
			stz.PreferredWidth = 40;
			stz.Margins = new Margins(0, 5, 0, 0);
			stz.Dock = DockStyle.Left;

			this.fieldZoom = new TextFieldUpDown(this.toolbar);
			this.fieldZoom.MinValue = (decimal) 0.2;
			this.fieldZoom.MaxValue = (decimal) 1.0;
			this.fieldZoom.Step = (decimal) 0.1;
			this.fieldZoom.Resolution = (decimal) 0.01;
			this.fieldZoom.PreferredWidth = 50;
			this.fieldZoom.Margins = new Margins(0, 5, 1, 1);
			this.fieldZoom.Dock = DockStyle.Left;
			this.fieldZoom.ValueChanged += new EventHandler(this.HandleFieldZoomValueChanged);

			this.sliderZoom = new HSlider(this.toolbar);
			this.sliderZoom.ShowMinMaxButtons = true;
			this.sliderZoom.MinValue = (decimal) 0.2;
			this.sliderZoom.MaxValue = (decimal) 1.0;
			this.sliderZoom.SmallChange = (decimal) 0.1;
			this.sliderZoom.LargeChange = (decimal) 0.2;
			this.sliderZoom.Resolution = (decimal) 0.01;
			this.sliderZoom.PreferredWidth = 120;
			this.sliderZoom.Margins = new Margins(0, 0, 4, 4);
			this.sliderZoom.Dock = DockStyle.Left;
			this.sliderZoom.ValueChanged += new EventHandler(this.HandleSliderZoomValueChanged);

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

		protected double Zoom
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

					this.fieldZoom.Value = (decimal) this.zoom;
					this.sliderZoom.Value = (decimal) this.zoom;

					this.UpdateScroller();
				}
			}
		}

		protected void UpdateScroller()
		{
			double w = this.areaSize.Width*this.zoom - this.editor.Client.Size.Width;
			if (w <= 0)
			{
				this.hscroler.Enable = false;
			}
			else
			{
				this.hscroler.Enable = true;
				this.hscroler.MinValue = (decimal) 0;
				this.hscroler.MaxValue = (decimal) w;
				this.hscroler.VisibleRangeRatio = (decimal) (this.editor.Client.Size.Width / (this.areaSize.Width*this.zoom));
			}

			double h = this.areaSize.Height*this.zoom - this.editor.Client.Size.Height;
			if (h <= 0)
			{
				this.vscroler.Enable = false;
			}
			else
			{
				this.vscroler.Enable = true;
				this.vscroler.MinValue = (decimal) 0;
				this.vscroler.MaxValue = (decimal) h;
				this.vscroler.VisibleRangeRatio = (decimal) (this.editor.Client.Size.Height / (this.areaSize.Height*this.zoom));
			}
		}


		private void HandleEditorSizeChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.UpdateScroller();
		}

		private void HandleFieldZoomValueChanged(object sender)
		{
			TextFieldUpDown field = sender as TextFieldUpDown;
			this.Zoom = (double) field.Value;
		}

		private void HandleSliderZoomValueChanged(object sender)
		{
			HSlider slider = sender as HSlider;
			this.Zoom = (double) slider.Value;
		}


		protected EntitiesEditor.Editor editor;
		protected VScroller vscroler;
		protected HScroller hscroler;
		protected Size areaSize;
		protected double zoom;
		protected HToolBar toolbar;
		protected TextFieldUpDown fieldZoom;
		protected HSlider sliderZoom;
	}
}
