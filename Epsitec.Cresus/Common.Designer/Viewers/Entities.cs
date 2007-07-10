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
		public Entities(Module module, PanelsContext context, ResourceAccess access, DesignerApplication designerApplication) : base(module, context, access, designerApplication)
		{
			this.lastGroup.Dock = DockStyle.Top;
			this.lastGroup.Visibility = (this.designerApplication.DisplayModeState != DesignerApplication.DisplayMode.FullScreen);

			this.hsplitter = new HSplitter(this.lastPane);
			this.hsplitter.Dock = DockStyle.Top;
			this.hsplitter.Visibility = (this.designerApplication.DisplayModeState != DesignerApplication.DisplayMode.FullScreen);

			Widget editorGroup = new Widget(this.lastPane);
			editorGroup.Padding = new Margins(10, 10, 10, 10);
			editorGroup.Dock = DockStyle.Fill;

			//	Crée les grands blocs de widgets.
			Widget band = new Widget(editorGroup);
			band.Dock = DockStyle.Fill;

			this.editor = new EntitiesEditor.Editor(band);
			this.editor.Entities = this;
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
					if (!this.designerApplication.Terminate())
					{
						return;
					}

					Entities.subView = value;
					this.designerApplication.LocatorFix();

					this.UpdateSubView();
					this.UpdateTitle();
					this.UpdateEdit();
					this.UpdateColor();
					this.UpdateModificationsCulture();
					this.UpdateCommands();
				}
			}
		}

		public static string SubViewName(int subView)
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
				if (Entities.isZoomPage)
				{
					return this.ZoomPage;
				}
				else
				{
					return Entities.zoom;
				}
			}
			set
			{
				bool isZoomPage = (value == 0 || value == this.ZoomPage);

				if (Entities.zoom != value || Entities.isZoomPage != isZoomPage)
				{
					Entities.zoom = value;
					Entities.isZoomPage = isZoomPage;

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

			this.ignoreChange = true;
			this.sliderZoom.Value = (decimal) this.Zoom;
			this.ignoreChange = false;

			this.buttonZoomPage.ActiveState    = (Entities.isZoomPage              ) ? ActiveState.Yes : ActiveState.No;
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

			this.Zoom = this.Zoom;
		}

		protected void UpdateSubView()
		{
			//	Met à jour le bouton sélectionné pour la sous-vue.
			this.buttonSubViewA.ActiveState = (this.SubView == 0) ? ActiveState.Yes : ActiveState.No;
			this.buttonSubViewB.ActiveState = (this.SubView == 1) ? ActiveState.Yes : ActiveState.No;
			this.buttonSubViewC.ActiveState = (this.SubView == 2) ? ActiveState.Yes : ActiveState.No;
			this.buttonSubViewT.ActiveState = (this.SubView == 3) ? ActiveState.Yes : ActiveState.No;
		}

		public override bool Terminate(bool soft)
		{
			//	Termine le travail sur une ressource, avant de passer à une autre.
			//	Si soft = true, on sérialise temporairement sans poser de question.
			//	Retourne false si l'utilisateur a choisi "annuler".
			base.Terminate(soft);

			if (this.module.AccessEntities.IsLocalDirty)
			{
				if (soft)
				{
					if (this.druidToSerialize.IsValid)
					{
						Entities.softSerialize = this.editor.Serialize();
					}
					else
					{
						Entities.softSerialize = null;
					}

					Entities.softDirtySerialization = this.module.AccessEntities.IsLocalDirty;
				}
				else
				{
#if false
					Common.Dialogs.DialogResult result = Epsitec.Common.Dialogs.DialogResult.Answer1;
					if (this.SubView != 3)  // pas la sous-vue temporaire ?
					{
						string header = string.Format("Que voulez-vous faire des modifications de la disposition de la vue <b>{0}</b> de l'entité <b>{1}</b> ?", Entities.SubViewName(Entities.subView), this.nameToSerialize);

						List<string> questions = new List<string>();
						questions.Add(ConfirmationButton.FormatContent("Conserver", string.Format("La disposition de la vue <b>{0}</b> est conservée.", Entities.SubViewName(Entities.subView))));
						questions.Add(ConfirmationButton.FormatContent("Copier vers T", string.Format("La disposition de la vue <b>{0}</b> est copiée dans la vue temporaire <b>T</b>.", Entities.SubViewName(Entities.subView))));
						questions.Add(ConfirmationButton.FormatContent("Ne pas conserver", string.Format("La disposition de la vue <b>{0}</b> est perdue.", Entities.SubViewName(Entities.subView))));
						
						result = this.designerApplication.DialogConfirmation(header, questions, true);
					}

					if (result == Epsitec.Common.Dialogs.DialogResult.Cancel)  // annuler ?
					{
						return false;
					}

					if (result == Epsitec.Common.Dialogs.DialogResult.Answer1)  // conserver ?
					{
						this.Serialize();
					}

					if (result == Epsitec.Common.Dialogs.DialogResult.Answer2)  // copier ?
					{
						string data = this.editor.Serialize();
						Entities.SetSerializedData(this.access.Accessor, this.druidToSerialize, 3, data);  // copie les données dans la sous-vue T
					}

					this.editor.DirtySerialization = false;
#else
					this.Serialize();
					this.module.AccessEntities.ClearLocalDirty();
#endif
				}
			}

			return true;
		}

		protected void DragSubView(int srcSubView, int dstSubView)
		{
			//	Effectue le drag & drop d'une sous-vue dans une autre.
			string header = string.Format("Quelle opération voulez-vous effectuer sur l'entité <b>{0}</b> ?", this.nameToSerialize);

			List<string> questions = new List<string>();
			questions.Add(ConfirmationButton.FormatContent("Copier", string.Format("La vue <b>{0}</b> est copiée dans la vue <b>{1}</b>.", Entities.SubViewName(srcSubView), Entities.SubViewName(dstSubView))));
			questions.Add(ConfirmationButton.FormatContent("Permuter", string.Format("Les vues <b>{0}</b> et <b>{1}</b> sont permutées.", Entities.SubViewName(srcSubView), Entities.SubViewName(dstSubView))));
			
			Common.Dialogs.DialogResult result = this.designerApplication.DialogConfirmation(header, questions, true);

			if (result == Epsitec.Common.Dialogs.DialogResult.Cancel)
			{
				return;
			}

			if (result == Epsitec.Common.Dialogs.DialogResult.Answer1)  // copier ?
			{
				if (srcSubView == this.SubView)  // drag de la sous-vue courante ?
				{
					string data = this.editor.Serialize();
					Entities.SetSerializedData(this.access.Accessor, this.CurrentDruid, dstSubView, data);
				}
				else if (dstSubView == this.SubView)  // drag dans la sous-vue courante ?
				{
					string data = Entities.GetSerializedData(this.access.Accessor, this.CurrentDruid, srcSubView);
					Entities.SetSerializedData(this.access.Accessor, this.CurrentDruid, dstSubView, data);
					this.UpdateEdit();
				}
				else  // drag d'une sous-vue cachée vers une autre cachée ?
				{
					string data = Entities.GetSerializedData(this.access.Accessor, this.CurrentDruid, srcSubView);
					Entities.SetSerializedData(this.access.Accessor, this.CurrentDruid, dstSubView, data);
				}
			}

			if (result == Epsitec.Common.Dialogs.DialogResult.Answer2)  // permuter ?
			{
				if (srcSubView == this.SubView)  // drag de la sous-vue courante ?
				{
					string srcData = this.editor.Serialize();
					string dstData = Entities.GetSerializedData(this.access.Accessor, this.CurrentDruid, dstSubView);
					Entities.SetSerializedData(this.access.Accessor, this.CurrentDruid, dstSubView, srcData);
					Entities.SetSerializedData(this.access.Accessor, this.CurrentDruid, srcSubView, dstData);
				}
				else if (dstSubView == this.SubView)  // drag dans la sous-vue courante ?
				{
					string srcData = Entities.GetSerializedData(this.access.Accessor, this.CurrentDruid, srcSubView);
					string dstData = this.editor.Serialize();
					Entities.SetSerializedData(this.access.Accessor, this.CurrentDruid, dstSubView, srcData);
					Entities.SetSerializedData(this.access.Accessor, this.CurrentDruid, srcSubView, dstData);
					this.UpdateEdit();
				}
				else  // drag d'une sous-vue cachée vers une autre cachée ?
				{
					string srcData = Entities.GetSerializedData(this.access.Accessor, this.CurrentDruid, srcSubView);
					string dstData = Entities.GetSerializedData(this.access.Accessor, this.CurrentDruid, dstSubView);
					Entities.SetSerializedData(this.access.Accessor, this.CurrentDruid, dstSubView, srcData);
					Entities.SetSerializedData(this.access.Accessor, this.CurrentDruid, srcSubView, dstData);
				}
			}

			this.SubView = dstSubView;
		}

		protected void Serialize()
		{
			//	Sérialise les données.
			if (this.druidToSerialize.IsValid)
			{
				string data = this.editor.Serialize();
				Entities.SetSerializedData(this.access.Accessor, this.druidToSerialize, this.SubView, data);
			}
		}

		protected bool Deserialize()
		{
			//	Désérialise les données sérialisées. Retourne false s'il n'existe aucune donnée sérialisée.
			this.nameToSerialize = this.CurrentName;
			this.druidToSerialize = this.CurrentDruid;

			if (Entities.softSerialize == null)
			{
				string data = Entities.GetSerializedData(this.access.Accessor, this.druidToSerialize, this.SubView);
				if (data == null)
				{
					this.module.AccessEntities.ClearLocalDirty();
					return false;
				}
				else
				{
					this.editor.Deserialize(data);
					this.module.AccessEntities.ClearLocalDirty();
					return true;
				}
			}
			else
			{
				if (this.module.AccessEntities.IsLocalDirty)
				{
					this.module.AccessEntities.SetLocalDirty();
				}
				else
				{
					this.module.AccessEntities.ClearLocalDirty();
				}

				this.editor.Deserialize(Entities.softSerialize);

				Entities.softDirtySerialization = false;
				Entities.softSerialize = null;
				return true;
			}
		}

		private static void SetSerializedData(IResourceAccessor accessor, Druid druid, int subView, string data)
		{
			//	Sérialise des données. data vaut null s'il faut effacer les données sérialisées.
			CultureMap resource = accessor.Collection[druid];
			
			if (resource != null)
			{
				StructuredData record = resource.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				string key = subView.ToString(System.Globalization.CultureInfo.InvariantCulture);
				Dictionary<string, string> dict = Entities.GetSerializedLayouts(record);
				
				if (string.IsNullOrEmpty(data))
				{
					if (dict.ContainsKey(key))
					{
						dict.Remove(key);
					}
				}
				else
				{
					//	Supprime l'en-tête XML <?xml...?> qui est inutile ici; on le regénèrera
					//	au besoin à la désérialisation :

					if (data.StartsWith(EntitiesEditor.Xml.XmlHeader))
					{
						data = data.Substring(EntitiesEditor.Xml.XmlHeader.Length);
					}

					dict[key] = data;
				}

				Entities.SetSerializedLayouts(record, dict);
			}
		}

		private static string GetSerializedData(IResourceAccessor accessor, Druid druid, int subView)
		{
			//	Désérialise des données. Retourne null s'il n'existe aucune donnée sérialisée.
			CultureMap resource = accessor.Collection[druid];
			
			if (resource != null)
			{
				StructuredData record = resource.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				string key = subView.ToString(System.Globalization.CultureInfo.InvariantCulture);
				Dictionary<string, string> dict = Entities.GetSerializedLayouts(record);
				
				if (dict.ContainsKey(key))
				{
					string data = dict[key];

					//	Si les données ont été purgées de leur en-tête <?xml ...?>, alors on
					//	leur en remet un artificiellement :
					if (data.StartsWith("<?xml"))
					{
						return data;
					}
					else
					{
						return EntitiesEditor.Xml.XmlHeader + data;
					}
				}
			}
			return null;
		}

		private static Dictionary<string, string> GetSerializedLayouts(StructuredData record)
		{
			Dictionary<string, string> dict = new Dictionary<string, string>();
			string data = record.GetValue(Support.Res.Fields.ResourceStructuredType.SerializedDesignerLayouts) as string;

			string openElementPrefix = "<"+EntitiesEditor.Xml.Layout;
			string closeElement      = "</"+EntitiesEditor.Xml.Layout+">";
			
			while (!string.IsNullOrEmpty(data))
			{
				System.Diagnostics.Debug.Assert(data.StartsWith(openElementPrefix));

				//										//	<Layout id="1">...</Layout><Layout id="2">...
				int pos1 = data.IndexOf('"')+1;			//              ^: :  :        :
				int pos2 = data.IndexOf('"', pos1);		//	             ^ :  :        :
				int pos3 = data.IndexOf('>')+1;			//	               ^  :        :
				int pos4 = data.IndexOf(closeElement);	//	                  ^        :
				int pos5 = pos4 + closeElement.Length;	//	                           ^
				
				string id   = data.Substring(pos1, pos2-pos1);
				string node = data.Substring(pos3, pos4-pos3);

				dict[id] = node;

				data = data.Substring(pos5);
			}
			
			return dict;
		}

		private static void SetSerializedLayouts(StructuredData record, Dictionary<string, string> dict)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();

			foreach (KeyValuePair<string, string> pair in dict)
			{
				buffer.Append("<");
				buffer.Append(EntitiesEditor.Xml.Layout);
				buffer.Append(@" id=""");
				buffer.Append(pair.Key);
				buffer.Append(@""">");

				buffer.Append(pair.Value);

				buffer.Append("</");
				buffer.Append(EntitiesEditor.Xml.Layout);
				buffer.Append(">");
			}

			record.SetValue(Support.Res.Fields.ResourceStructuredType.SerializedDesignerLayouts, buffer.ToString());
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
				this.Zoom = 0;
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
			if (this.ignoreChange)
			{
				return;
			}

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
		protected static bool isZoomPage = false;
		protected static string softSerialize = null;
		protected static bool softDirtySerialization = false;

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
		protected int dragStarting;
		protected Druid druidToSerialize;
		protected string nameToSerialize;
	}
}
