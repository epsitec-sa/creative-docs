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
			this.lastGroup.Visibility = (this.mainWindow.DisplayModeState != MainWindow.DisplayMode.FullScreen);

			this.hsplitter = new HSplitter(this.lastPane);
			this.hsplitter.Dock = DockStyle.Top;
			this.hsplitter.Visibility = (this.mainWindow.DisplayModeState != MainWindow.DisplayMode.FullScreen);

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
			ToolTip.Default.SetToolTip(this.editor, "*");  // pour voir les tooltips dynamiques

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
			this.buttonSubViewA = new MyWidgets.EntitySubView(this.toolbar);
			this.buttonSubViewA.Text = "A";
			this.buttonSubViewA.PreferredWidth = this.buttonSubViewA.PreferredHeight;
			this.buttonSubViewA.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonSubViewA.AutoFocus = false;
			this.buttonSubViewA.Dock = DockStyle.Left;
			this.buttonSubViewA.Clicked += new MessageEventHandler(this.HandleButtonSubViewClicked);
			this.buttonSubViewA.DragStarting += new EventHandler(this.HandleButtonSubViewDragStarting);
			this.buttonSubViewA.DragEnding += new EventHandler(this.HandleButtonSubViewDragEnding);
			ToolTip.Default.SetToolTip(this.buttonSubViewA, "Vue A");

			this.buttonSubViewB = new MyWidgets.EntitySubView(this.toolbar);
			this.buttonSubViewB.Text = "B";
			this.buttonSubViewB.PreferredWidth = this.buttonSubViewB.PreferredHeight;
			this.buttonSubViewB.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonSubViewB.AutoFocus = false;
			this.buttonSubViewB.Dock = DockStyle.Left;
			this.buttonSubViewB.Clicked += new MessageEventHandler(this.HandleButtonSubViewClicked);
			this.buttonSubViewB.DragStarting += new EventHandler(this.HandleButtonSubViewDragStarting);
			this.buttonSubViewB.DragEnding += new EventHandler(this.HandleButtonSubViewDragEnding);
			ToolTip.Default.SetToolTip(this.buttonSubViewB, "Vue B");

			this.buttonSubViewC = new MyWidgets.EntitySubView(this.toolbar);
			this.buttonSubViewC.Text = "C";
			this.buttonSubViewC.PreferredWidth = this.buttonSubViewC.PreferredHeight;
			this.buttonSubViewC.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonSubViewC.AutoFocus = false;
			this.buttonSubViewC.Dock = DockStyle.Left;
			this.buttonSubViewC.Clicked += new MessageEventHandler(this.HandleButtonSubViewClicked);
			this.buttonSubViewC.DragStarting += new EventHandler(this.HandleButtonSubViewDragStarting);
			this.buttonSubViewC.DragEnding += new EventHandler(this.HandleButtonSubViewDragEnding);
			ToolTip.Default.SetToolTip(this.buttonSubViewC, "Vue C");

			this.buttonSubViewT = new MyWidgets.EntitySubView(this.toolbar);
			this.buttonSubViewT.Text = "T";
			this.buttonSubViewT.PreferredWidth = this.buttonSubViewT.PreferredHeight;
			this.buttonSubViewT.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonSubViewT.AutoFocus = false;
			this.buttonSubViewT.Dock = DockStyle.Left;
			this.buttonSubViewT.Margins = new Margins(2, 0, 0, 0);
			this.buttonSubViewT.Clicked += new MessageEventHandler(this.HandleButtonSubViewClicked);
			this.buttonSubViewT.DragStarting += new EventHandler(this.HandleButtonSubViewDragStarting);
			this.buttonSubViewT.DragEnding += new EventHandler(this.HandleButtonSubViewDragEnding);
			ToolTip.Default.SetToolTip(this.buttonSubViewT, "Vue temporaire");

			IconSeparator sep = new IconSeparator(this.toolbar);
			sep.Dock = DockStyle.Left;

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
			this.UpdateSubView();
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

				this.buttonSubViewA.Clicked -= new MessageEventHandler(this.HandleButtonSubViewClicked);
				this.buttonSubViewA.DragStarting -= new EventHandler(this.HandleButtonSubViewDragStarting);
				this.buttonSubViewA.DragEnding -= new EventHandler(this.HandleButtonSubViewDragEnding);
				this.buttonSubViewB.Clicked -= new MessageEventHandler(this.HandleButtonSubViewClicked);
				this.buttonSubViewB.DragStarting -= new EventHandler(this.HandleButtonSubViewDragStarting);
				this.buttonSubViewB.DragEnding -= new EventHandler(this.HandleButtonSubViewDragEnding);
				this.buttonSubViewC.Clicked -= new MessageEventHandler(this.HandleButtonSubViewClicked);
				this.buttonSubViewC.DragStarting -= new EventHandler(this.HandleButtonSubViewDragStarting);
				this.buttonSubViewC.DragEnding -= new EventHandler(this.HandleButtonSubViewDragEnding);
				this.buttonSubViewT.Clicked -= new MessageEventHandler(this.HandleButtonSubViewClicked);
				this.buttonSubViewT.DragStarting -= new EventHandler(this.HandleButtonSubViewDragStarting);
				this.buttonSubViewT.DragEnding -= new EventHandler(this.HandleButtonSubViewDragEnding);
				
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

		public int SubView
		{
			//	Sous-vue utilisée pour représenter les boîtes et les liaisons.
			get
			{
				return Entities.subView;
			}
			set
			{
				if (Entities.subView != value)
				{
					this.mainWindow.Terminate();

					Entities.subView = value;

					this.UpdateSubView();
					this.UpdateTitle();
					this.UpdateEdit();
					this.UpdateColor();
					this.UpdateModificationsCulture();
					this.UpdateCommands();
				}
			}
		}

		protected string SubViewName(int subView)
		{
			//	Retourne le nom de la sous-vue utilisée.
			switch (subView)
			{
				case 0:   return "A";
				case 1:   return "B";
				case 2:   return "C";
				default:  return "T";
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

			if (!this.Deserialize())
			{
				CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
				if (item != null)
				{
					EntitiesEditor.ObjectBox box = new EntitiesEditor.ObjectBox(this.editor);
					box.IsRoot = true;  // la première boîte est toujours la boîte racine
					box.SetContent(item);
					this.editor.AddBox(box);
				}

				this.editor.CreateConnections();
				this.editor.UpdateAfterGeometryChanged(null);
			}
		}

		protected void UpdateSubView()
		{
			//	Met à jour le bouton sélectionné pour la sous-vue.
			this.buttonSubViewA.ActiveState = (this.SubView == 0) ? ActiveState.Yes : ActiveState.No;
			this.buttonSubViewB.ActiveState = (this.SubView == 1) ? ActiveState.Yes : ActiveState.No;
			this.buttonSubViewC.ActiveState = (this.SubView == 2) ? ActiveState.Yes : ActiveState.No;
			this.buttonSubViewT.ActiveState = (this.SubView == 3) ? ActiveState.Yes : ActiveState.No;
		}

		public override void Terminate()
		{
			//	Termine le travail sur une ressource, avant de passer à une autre.
			if (this.editor.DirtySerialization)
			{
				this.editor.DirtySerialization = false;

				if (this.SubView != 3)  // pas la sous-vue temporaire ?
				{
					string question = string.Format("Voulez-vous conserver les modifications de l'entité <b>{0}</b><br/>effectuées dans la vue <b>{1}</b> ?", this.nameToSerialize, this.SubViewName(Entities.subView));
					if (this.module.MainWindow.DialogQuestion(question) != Epsitec.Common.Dialogs.DialogResult.Yes)
					{
						return;
					}
				}

				this.Serialize();
			}
		}

		protected void DragSubView(int srcSubView, int dstSubView)
		{
			//	Effectue le drag & drop d'une sous-vue dans une autre.
			string question = string.Format("Voulez-vous copier la vue <b>{0}</b> vers <b>{1}</b> ?", this.SubViewName(srcSubView), this.SubViewName(dstSubView));
			if (this.module.MainWindow.DialogQuestion(question) != Epsitec.Common.Dialogs.DialogResult.Yes)
			{
				return;
			}

			if (srcSubView == this.SubView)  // drag de la sous-vue courante ?
			{
				string data = this.editor.Serialize();
				this.SetSerializeData(this.CurrentDruid, dstSubView, data);
			}
			else if (dstSubView == this.SubView)  // drag dans la sous-vue courante ?
			{
				string data = this.GetSerializeData(this.CurrentDruid, srcSubView);
				this.SetSerializeData(this.CurrentDruid, dstSubView, data);
				this.UpdateEdit();
			}
			else  // drag d'une sous-vue cachée vers une autre cachée ?
			{
				string data = this.GetSerializeData(this.CurrentDruid, srcSubView);
				this.SetSerializeData(this.CurrentDruid, dstSubView, data);
			}
		}

		protected void Serialize()
		{
			//	Sérialise les données.
			if (this.druidToSerialize.IsValid)
			{
				string data = this.editor.Serialize();
				this.SetSerializeData(this.druidToSerialize, this.SubView, data);
			}
		}

		protected bool Deserialize()
		{
			//	Désérialise les données sérialisées. Retourne false s'il n'existe aucune donnée sérialisée.
			this.nameToSerialize = this.CurrentName;
			this.druidToSerialize = this.CurrentDruid;
			this.editor.DirtySerialization = false;

			string data = this.GetSerializeData(this.druidToSerialize, this.SubView);
			if (data == null)
			{
				return false;
			}
			else
			{
				this.editor.Deserialize(data);
				return true;
			}
		}

		protected void SetSerializeData(Druid druid, int subView, string data)
		{
			//	Sérialise des données. data vaut null s'il faut effacer les données sérialisées.
			if (druid.IsValid)
			{
				string key = string.Concat(subView.ToString(System.Globalization.CultureInfo.InvariantCulture), ":", druid.ToString());

				if (data == null)
				{
					if (Entities.serial.ContainsKey(key))
					{
						Entities.serial.Remove(key);
					}
				}
				else
				{
					if (Entities.serial.ContainsKey(key))
					{
						Entities.serial[key] = data;
					}
					else
					{
						Entities.serial.Add(key, data);
					}
				}
			}
		}

		protected string GetSerializeData(Druid druid, int subView)
		{
			//	Désérialise des données. Retourne null s'il n'existe aucune donnée sérialisée.
			if (druid.IsValid)
			{
				string key = string.Concat(subView.ToString(System.Globalization.CultureInfo.InvariantCulture), ":", druid.ToString());
				
				if (Entities.serial.ContainsKey(key))
				{
					return Entities.serial[key];
				}
			}
			return null;
		}


		protected Druid CurrentDruid
		{
			get
			{
				CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
				if (item == null)
				{
					return Druid.Empty;
				}
				else
				{
					return item.Id;
				}
			}
		}

		protected string CurrentName
		{
			get
			{
				CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
				if (item == null)
				{
					return null;
				}
				else
				{
					return item.Name;
				}
			}
		}

		protected int GetSubView(object widget)
		{
			//	Retourne le rang d'une sous-vue correspondant à un widget.
			if (widget == this.buttonSubViewA)
			{
				return 0;
			}

			if (widget == this.buttonSubViewB)
			{
				return 1;
			}

			if (widget == this.buttonSubViewC)
			{
				return 2;
			}

			if (widget == this.buttonSubViewT)
			{
				return 3;
			}

			return -1;
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

		private void HandleButtonSubViewClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsqu'un bouton de vue (A, B, C ou T) est cliqué.
			this.SubView = this.GetSubView(sender);
		}

		private void HandleButtonSubViewDragStarting(object sender)
		{
			//	Appelé lorsqu'un bouton de vue (A, B, C ou T) commencer à être draggé sur un autre.
			this.dragStarting = this.GetSubView(sender);
		}

		private void HandleButtonSubViewDragEnding(object sender)
		{
			//	Appelé lorsqu'un bouton de vue (A, B, C ou T) a été draggé sur un autre.
			int dragEnding = this.GetSubView(sender);
			if (this.dragStarting != -1 && dragEnding != -1)
			{
				this.DragSubView(this.dragStarting, dragEnding);
			}
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

		protected static int subView = 0;
		protected static double zoom = Entities.zoomDefault;
		protected static Dictionary<string, string> serial = new Dictionary<string, string>();

		protected HSplitter hsplitter;
		protected EntitiesEditor.Editor editor;
		protected VScroller vscroller;
		protected HScroller hscroller;
		protected Size areaSize;
		protected HToolBar toolbar;
		protected MyWidgets.EntitySubView buttonSubViewA;
		protected MyWidgets.EntitySubView buttonSubViewB;
		protected MyWidgets.EntitySubView buttonSubViewC;
		protected MyWidgets.EntitySubView buttonSubViewT;
		protected IconButton buttonZoomPage;
		protected IconButton buttonZoomMin;
		protected IconButton buttonZoomDefault;
		protected IconButton buttonZoomMax;
		protected StatusField fieldZoom;
		protected HSlider sliderZoom;
		protected Druid druidToSerialize;
		protected string nameToSerialize;
		protected int dragStarting;
	}
}
