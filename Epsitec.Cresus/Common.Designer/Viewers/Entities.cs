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
			this.editor.Module = this.module;
			this.editor.Dock = DockStyle.Fill;
			this.editor.AreaSize = this.areaSize;
			this.editor.Zoom = this.Zoom;
			this.editor.SizeChanged += new EventHandler<DependencyPropertyChangedEventArgs>(this.HandleEditorSizeChanged);
			this.editor.AreaSizeChanged += new EventHandler(this.HandleEditorAreaSizeChanged);
			this.editor.AreaOffsetChanged += new EventHandler(this.HandleEditorAreaOffsetChanged);
			this.editor.ZoomChanged += new EventHandler(this.HandleEditorZoomChanged);

			this.vscroller = new VScroller(band);
			this.vscroller.IsInverted = true;
			this.vscroller.Dock = DockStyle.Right;
			this.vscroller.ValueChanged += new EventHandler(this.HandleScrollerValueChanged);
			this.editor.VScroller = this.vscroller;

			this.toolbar = new HToolBar(editorGroup);
			this.toolbar.Dock = DockStyle.Bottom;
			this.toolbar.Margins = new Margins(0, 0, 5, 0);

			this.hscroller = new HScroller(editorGroup);
			this.hscroller.Margins = new Margins(0, this.vscroller.PreferredWidth, 0, 0);
			this.hscroller.Dock = DockStyle.Bottom;
			this.hscroller.ValueChanged += new EventHandler(this.HandleScrollerValueChanged);

			//	Peuple la toolbar.
			this.buttonZoomPage = new IconButton(this.toolbar);
			this.buttonZoomPage.IconName = Misc.Icon("ZoomPage");
			this.buttonZoomPage.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonZoomPage.AutoFocus = false;
			this.buttonZoomPage.Dock = DockStyle.Left;
			this.buttonZoomPage.Clicked += new MessageEventHandler(this.HandleButtonZoomClicked);
			ToolTip.Default.SetToolTip(this.buttonZoomPage, "Zoom pleine page");

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
			this.sliderZoom.MinValue = (decimal) Entities.zoomMin;
			this.sliderZoom.MaxValue = (decimal) Entities.zoomMax;
			this.sliderZoom.SmallChange = (decimal) 0.1;
			this.sliderZoom.LargeChange = (decimal) 0.2;
			this.sliderZoom.Resolution = (decimal) 0.01;
			this.sliderZoom.PreferredWidth = 90;
			this.sliderZoom.Margins = new Margins(0, 0, 4, 4);
			this.sliderZoom.Dock = DockStyle.Left;
			this.sliderZoom.ValueChanged += new EventHandler(this.HandleSliderZoomValueChanged);
			ToolTip.Default.SetToolTip(this.sliderZoom, "Choix du zoom");

			this.AreaSize = new Size(100, 100);

			this.editor.UpdateGeometry();
			this.UpdateZoom();
			this.UpdateAll();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.editor.SizeChanged -= new EventHandler<DependencyPropertyChangedEventArgs>(this.HandleEditorSizeChanged);
				this.editor.AreaSizeChanged -= new EventHandler(this.HandleEditorAreaSizeChanged);
				this.editor.AreaOffsetChanged -= new EventHandler(this.HandleEditorAreaOffsetChanged);
				this.editor.ZoomChanged -= new EventHandler(this.HandleEditorZoomChanged);
				this.vscroller.ValueChanged -= new EventHandler(this.HandleScrollerValueChanged);
				this.hscroller.ValueChanged -= new EventHandler(this.HandleScrollerValueChanged);
				this.buttonZoomPage.Clicked -= new MessageEventHandler(this.HandleButtonZoomClicked);
				this.buttonZoomMin.Clicked -= new MessageEventHandler(this.HandleButtonZoomClicked);
				this.buttonZoomDefault.Clicked -= new MessageEventHandler(this.HandleButtonZoomClicked);
				this.buttonZoomMax.Clicked -= new MessageEventHandler(this.HandleButtonZoomClicked);
				this.fieldZoom.Clicked -= new MessageEventHandler(this.HandleFieldZoomClicked);
				this.sliderZoom.ValueChanged -= new EventHandler(this.HandleSliderZoomValueChanged);
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
				return Entities.zoom;
			}
			set
			{
				if (Entities.zoom != value)
				{
					Entities.zoom = value;

					this.UpdateZoom();
					this.UpdateScroller();
				}
			}
		}

		protected double ZoomPage
		{
			//	Retourne le zoom permettant de voir toute la surface de travail.
			get
			{
				double zx = this.editor.Client.Bounds.Width  / this.editor.AreaSize.Width;
				double zy = this.editor.Client.Bounds.Height / this.editor.AreaSize.Height;
				double zoom = System.Math.Min(zx, zy);

				zoom = System.Math.Max(zoom, Entities.zoomMin);
				zoom = System.Math.Min(zoom, Entities.zoomMax);
				
				zoom = System.Math.Floor(zoom*100)/100;  // 45.8% -> 46%
				return zoom;
			}
		}

		protected void UpdateZoom()
		{
			//	Met à jour tout ce qui dépend du zoom.
			this.editor.Zoom = this.Zoom;

			this.fieldZoom.Text = string.Concat(System.Math.Floor(this.Zoom*100).ToString(), "%");
			this.sliderZoom.Value = (decimal) this.Zoom;

			this.buttonZoomPage.ActiveState    = (this.Zoom == this.ZoomPage       ) ? ActiveState.Yes : ActiveState.No;
			this.buttonZoomMin.ActiveState     = (this.Zoom == Entities.zoomMin    ) ? ActiveState.Yes : ActiveState.No;
			this.buttonZoomDefault.ActiveState = (this.Zoom == Entities.zoomDefault) ? ActiveState.Yes : ActiveState.No;
			this.buttonZoomMax.ActiveState     = (this.Zoom == Entities.zoomMax    ) ? ActiveState.Yes : ActiveState.No;
		}

		protected void UpdateScroller()
		{
			//	Met à jour les ascenseurs, en fonction du zoom courant et de la taille de l'éditeur.
			double w = this.areaSize.Width*this.Zoom - this.editor.Client.Size.Width;
			if (w <= 0 || this.editor.Client.Size.Width <= 0)
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
				this.hscroller.VisibleRangeRatio = (decimal) (this.editor.Client.Size.Width / (this.areaSize.Width*this.Zoom));
			}

			double h = this.areaSize.Height*this.Zoom - this.editor.Client.Size.Height;
			if (h <= 0 || this.editor.Client.Size.Height <= 0)
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
				this.vscroller.VisibleRangeRatio = (decimal) (this.editor.Client.Size.Height / (this.areaSize.Height*this.Zoom));
			}

			this.editor.IsScrollerEnable = this.hscroller.Enable || this.vscroller.Enable;
			this.HandleScrollerValueChanged(null);
		}


		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			base.UpdateEdit();

			this.editor.Clear();

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			if (item != null)
			{
				EntitiesEditor.ObjectBox box = new EntitiesEditor.ObjectBox(this.editor);
				box.IsRoot = true;
				box.Title = item.Name;
				box.SetContent(item);
				this.editor.AddBox(box);
			}

			this.editor.CreateConnections();
			this.editor.UpdateAfterGeometryChanged(null);
		}


		private void HandleEditorSizeChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			//	Appelé lorsque la taille de la fenêtre de l'éditeur change.
			this.UpdateScroller();
			this.UpdateZoom();
		}

		private void HandleEditorAreaSizeChanged(object sender)
		{
			//	Appelé lorsque les dimensions de la zone de travail ont changé.
			this.AreaSize = this.editor.AreaSize;
			this.UpdateZoom();
		}

		private void HandleEditorAreaOffsetChanged(object sender)
		{
			//	Appelé lorsque l'offset de la zone de travail a changé.
			Point offset = this.editor.AreaOffset;

			if (this.hscroller.Enable)
			{
				offset.X = System.Math.Max(offset.X, (double) this.hscroller.MinValue/this.Zoom);
				offset.X = System.Math.Min(offset.X, (double) this.hscroller.MaxValue/this.Zoom);
			}
			else
			{
				offset.X = 0;
			}

			if (this.vscroller.Enable)
			{
				offset.Y = System.Math.Max(offset.Y, (double) this.vscroller.MinValue/this.Zoom);
				offset.Y = System.Math.Min(offset.Y, (double) this.vscroller.MaxValue/this.Zoom);
			}
			else
			{
				offset.Y = 0;
			}

			this.editor.AreaOffset = offset;

			this.hscroller.Value = (decimal) (offset.X*this.Zoom);
			this.vscroller.Value = (decimal) (offset.Y*this.Zoom);
		}

		private void HandleScrollerValueChanged(object sender)
		{
			//	Appelé lorsqu'un ascenseur a été bougé.
			double ox = 0;
			if (this.hscroller.IsEnabled)
			{
				ox = (double) this.hscroller.Value/this.Zoom;
			}

			double oy = 0;
			if (this.vscroller.IsEnabled)
			{
				oy = (double) this.vscroller.Value/this.Zoom;
			}

			this.editor.AreaOffset = new Point(ox, oy);
		}

		private void HandleButtonZoomClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsqu'un bouton de zoom prédéfini est cliqué.
			if (sender == this.buttonZoomPage)
			{
				this.Zoom = this.ZoomPage;
			}

			if (sender == this.buttonZoomMin)
			{
				this.Zoom = Entities.zoomMin;
			}

			if (sender == this.buttonZoomDefault)
			{
				this.Zoom = Entities.zoomDefault;
			}
			
			if (sender == this.buttonZoomMax)
			{
				this.Zoom = Entities.zoomMax;
			}
		}

		private void HandleFieldZoomClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le champ du zoom a été cliqué.
			StatusField sf = sender as StatusField;
			if (sf == null)  return;
			VMenu menu = EntitiesEditor.ZoomMenu.CreateZoomMenu(Entities.zoomDefault, this.Zoom, this.ZoomPage, null);
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

		private void HandleEditorZoomChanged(object sender)
		{
			//	Appelé lorsque le zoom a changé depuis l'éditeur.
			this.Zoom = this.editor.Zoom;
		}


		public static readonly double zoomMin = 0.2;
		public static readonly double zoomMax = 2.0;
		protected static readonly double zoomDefault = 1.0;

		protected static double zoom = Entities.zoomDefault;

		protected HSplitter hsplitter;
		protected EntitiesEditor.Editor editor;
		protected VScroller vscroller;
		protected HScroller hscroller;
		protected Size areaSize;
		protected HToolBar toolbar;
		protected IconButton buttonZoomPage;
		protected IconButton buttonZoomMin;
		protected IconButton buttonZoomDefault;
		protected IconButton buttonZoomMax;
		protected StatusField fieldZoom;
		protected HSlider sliderZoom;
	}
}
