using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Layers contient tous les panneaux des calques.
	/// </summary>
	public class Layers : Abstract
	{
		public Layers(Document document) : base(document)
		{
			this.helpText = new StaticText(this);
			this.helpText.Text = Res.Strings.Container.Help.Layers;
			this.helpText.Dock = DockStyle.Top;
			this.helpText.Margins = new Margins(0, 0, -2, 7);

			this.toolBar = new HToolBar(this);
			this.toolBar.Dock = DockStyle.Top;
			this.toolBar.Margins = new Margins(0, 0, 0, -1);
			this.toolBar.TabIndex = 1;
			this.toolBar.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			int index = 0;

			this.buttonNew = new IconButton("LayerNew", Misc.Icon("LayerNew"));
			this.toolBar.Items.Add(this.buttonNew);
			this.buttonNew.TabIndex = index++;
			this.buttonNew.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonNew, Res.Strings.Action.LayerNewLong);
			this.Synchro(this.buttonNew);

			this.buttonDuplicate = new IconButton("LayerDuplicate", Misc.Icon("DuplicateItem"));
			this.toolBar.Items.Add(this.buttonDuplicate);
			this.buttonDuplicate.TabIndex = index++;
			this.buttonDuplicate.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonDuplicate, Res.Strings.Action.LayerDuplicate);
			this.Synchro(this.buttonDuplicate);

			this.buttonNewSel = new IconButton("LayerNewSel", Misc.Icon("LayerNewSel"));
			this.toolBar.Items.Add(this.buttonNewSel);
			this.buttonNewSel.TabIndex = index++;
			this.buttonNewSel.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonNewSel, Res.Strings.Action.LayerNewSel);
			this.Synchro(this.buttonNewSel);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonMergeUp = new IconButton("LayerMergeUp", Misc.Icon("LayerMergeUp"));
			this.toolBar.Items.Add(this.buttonMergeUp);
			this.buttonMergeUp.TabIndex = index++;
			this.buttonMergeUp.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonMergeUp, Res.Strings.Action.LayerMergeUp);
			this.Synchro(this.buttonMergeUp);

			this.buttonMergeDown = new IconButton("LayerMergeDown", Misc.Icon("LayerMergeDown"));
			this.toolBar.Items.Add(this.buttonMergeDown);
			this.buttonMergeDown.TabIndex = index++;
			this.buttonMergeDown.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonMergeDown, Res.Strings.Action.LayerMergeDown);
			this.Synchro(this.buttonMergeDown);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonUp = new IconButton("LayerUp", Misc.Icon("Up"));
			this.toolBar.Items.Add(this.buttonUp);
			this.buttonUp.TabIndex = index++;
			this.buttonUp.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonUp, Res.Strings.Action.LayerUp);
			this.Synchro(this.buttonUp);

			this.buttonDown = new IconButton("LayerDown", Misc.Icon("Down"));
			this.toolBar.Items.Add(this.buttonDown);
			this.buttonDown.TabIndex = index++;
			this.buttonDown.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonDown, Res.Strings.Action.LayerDown);
			this.Synchro(this.buttonDown);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDelete = new IconButton("LayerDelete", Misc.Icon("DeleteItem"));
			this.toolBar.Items.Add(this.buttonDelete);
			this.buttonDelete.TabIndex = index++;
			this.buttonDelete.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonDelete, Res.Strings.Action.LayerDelete);
			this.Synchro(this.buttonDelete);

			this.table = new CellTable(this);
			this.table.Dock = DockStyle.Fill;
			this.table.SelectionChanged += this.HandleTableSelectionChanged;
			this.table.DoubleClicked += this.HandleTableDoubleClicked;
			this.table.StyleH  = CellArrayStyles.ScrollNorm;
			this.table.StyleH |= CellArrayStyles.Header;
			this.table.StyleH |= CellArrayStyles.Separator;
			this.table.StyleH |= CellArrayStyles.Mobile;
			this.table.StyleV  = CellArrayStyles.ScrollNorm;
			this.table.StyleV |= CellArrayStyles.Separator;
			this.table.StyleV |= CellArrayStyles.SelectLine;
			this.table.DefHeight = 18;
			this.table.TabIndex = 2;
			this.table.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			Panels.Abstract.StaticDocument = this.document;
			this.panelModColor = new Panels.ModColor(this.document);
			this.panelModColor.IsExtendedSize = true;
			this.panelModColor.IsLayoutDirect = true;
			this.panelModColor.Dock = DockStyle.Bottom;
			this.panelModColor.Margins = new Margins(0, 0, 5, 0);
			this.panelModColor.SetParent(this);
			this.panelModColor.TabIndex = 100;
			this.panelModColor.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			//	--- Début panelMisc
			this.panelMisc = new Widget(this);
			this.panelMisc.Dock = DockStyle.Bottom;
			this.panelMisc.Margins = new Margins(0, 0, 5, 0);
			this.panelMisc.PreferredHeight = 70;
			this.panelMisc.TabIndex = 98;
			this.panelMisc.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			
			this.panelButton = new Widget(this.panelMisc);
			this.panelButton.Dock = DockStyle.Left;
			this.panelButton.Margins = new Margins(0, 0, 0, 0);
			this.panelButton.PreferredWidth = 126;
			this.panelButton.PreferredHeight = this.panelMisc.PreferredHeight;
			this.panelButton.TabIndex = 1;
			this.panelButton.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			
			this.buttonShow = new Button(this.panelButton);
			this.buttonShow.Dock = DockStyle.Top;
			this.buttonShow.Margins = new Margins(0, 0, 0, 0);
			this.buttonShow.Text = Res.Strings.Container.Layers.Button.Show;
			this.buttonShow.Clicked += this.HandleButtonClicked;
			this.buttonShow.TabIndex = 1;
			this.buttonShow.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonShow, Res.Strings.Container.Layers.Button.HelpShow);

			this.buttonDimmed = new Button(this.panelButton);
			this.buttonDimmed.Dock = DockStyle.Top;
			this.buttonDimmed.Margins = new Margins(0, 0, 0, 0);
			this.buttonDimmed.Text = Res.Strings.Container.Layers.Button.Dimmed;
			this.buttonDimmed.Clicked += this.HandleButtonClicked;
			this.buttonDimmed.TabIndex = 2;
			this.buttonDimmed.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonDimmed, Res.Strings.Container.Layers.Button.HelpDimmed);

			this.buttonHide = new Button(this.panelButton);
			this.buttonHide.Dock = DockStyle.Top;
			this.buttonHide.Margins = new Margins(0, 0, 0, 0);
			this.buttonHide.Text = Res.Strings.Container.Layers.Button.Hide;
			this.buttonHide.Clicked += this.HandleButtonClicked;
			this.buttonHide.TabIndex = 3;
			this.buttonHide.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonHide, Res.Strings.Container.Layers.Button.HelpHide);

			this.radioGroupPrint = new GroupBox(this.panelMisc);
			this.radioGroupPrint.Dock = DockStyle.Right;
			this.radioGroupPrint.Margins = new Margins(0, 0, 0, 4);
			this.radioGroupPrint.PreferredWidth = 106;
			this.radioGroupPrint.PreferredHeight = this.panelMisc.PreferredHeight;
			this.radioGroupPrint.Text = Res.Strings.Container.Layers.Button.PrintGroup;
			this.radioGroupPrint.TabIndex = 2;
			this.radioGroupPrint.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.radioShowPrint = new RadioButton(this.radioGroupPrint);
			this.radioShowPrint.Dock = DockStyle.Top;
			this.radioShowPrint.Margins = new Margins(10, 10, 0, 2);
			this.radioShowPrint.Text = Res.Strings.Container.Layers.Button.PrintShow;
			this.radioShowPrint.ActiveStateChanged += this.HandleRadioPrintChanged;
			this.radioShowPrint.Index = 1;
			this.radioShowPrint.TabIndex = 1;
			this.radioShowPrint.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.radioDimmedPrint = new RadioButton(this.radioGroupPrint);
			this.radioDimmedPrint.Dock = DockStyle.Top;
			this.radioDimmedPrint.Margins = new Margins(10, 10, 0, 2);
			this.radioDimmedPrint.Text = Res.Strings.Container.Layers.Button.PrintDimmed;
			this.radioDimmedPrint.ActiveStateChanged += this.HandleRadioPrintChanged;
			this.radioDimmedPrint.Index = 2;
			this.radioDimmedPrint.TabIndex = 2;
			this.radioDimmedPrint.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.radioHidePrint = new RadioButton(this.radioGroupPrint);
			this.radioHidePrint.Dock = DockStyle.Top;
			this.radioHidePrint.Margins = new Margins(10, 10, 0, 2);
			this.radioHidePrint.Text = Res.Strings.Container.Layers.Button.PrintHide;
			this.radioHidePrint.ActiveStateChanged += this.HandleRadioPrintChanged;
			this.radioHidePrint.Index = 3;
			this.radioHidePrint.TabIndex = 3;
			this.radioHidePrint.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			//	--- Fin panelMisc
			
			this.extendedButton = new GlyphButton(this);
			this.extendedButton.Dock = DockStyle.Bottom;
			this.extendedButton.Margins = new Margins(0, 0, 5, 0);
			this.extendedButton.ButtonStyle = ButtonStyle.Icon;
			this.extendedButton.Clicked += this.ExtendedButtonClicked;
			this.extendedButton.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.extendedButton.TabIndex = 97;
			this.extendedButton.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.extendedButton, Res.Strings.Dialog.Button.More);

			
			this.toolBarName = new HToolBar(this);
			this.toolBarName.Dock = DockStyle.Bottom;
			this.toolBarName.Margins = new Margins(0, 0, 0, 0);
			this.toolBarName.TabIndex = 96;
			this.toolBarName.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			StaticText st = new StaticText();
			st.PreferredWidth = 80;
			st.Text = Res.Strings.Panel.LayerName.Label.Name;
			this.toolBarName.Items.Add(st);

			this.name = new TextField();
			this.name.PreferredWidth = 140;
			this.name.Margins = new Margins(0, 0, 1, 1);
			this.name.TextChanged += this.HandleNameTextChanged;
			this.name.TabIndex = 1;
			this.name.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.toolBarName.Items.Add(this.name);
			ToolTip.Default.SetToolTip(this.name, Res.Strings.Panel.LayerName.Tooltip.Name);


			this.UpdateExtended();
		}

		protected void Synchro(Widget widget)
		{
			//	Synchronise avec l'état de la commande.
			//	TODO: devrait être inutile, à supprimer donc !!!
#if false //#fix
			widget.Enable = (this.toolBar.CommandDispatcher[widget.Command].Enabled);
#endif
		}

		
		protected override void DoUpdateContent()
		{
			//	Effectue la mise à jour du contenu.
			this.helpText.Visibility = this.document.GlobalSettings.LabelProperties;
			this.UpdateTable();
			this.UpdateRadio();
			this.UpdatePanel();
		}

		protected override void DoUpdateObject(Objects.Abstract obj)
		{
			//	Effectue la mise à jour d'un objet.
			Objects.Layer layer = obj as Objects.Layer;
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Page page = context.RootObject(1) as Objects.Page;
			int rank = page.Objects.IndexOf(obj);
			int i = context.TotalLayers()-rank-1;
			this.TableUpdateRow(i, layer, true);

			if ( rank == context.CurrentLayer )
			{
				this.UpdatePanel();
			}
		}

		protected void UpdateTable()
		{
			//	Met à jour le contenu de la table.
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;

			int rows = context.TotalLayers();
			int initialColumns = this.table.Columns;
			this.table.SetArraySize(5, rows);

			if ( initialColumns == 0 )
			{
				this.table.SetWidthColumn(0, 20);
				this.table.SetWidthColumn(1, 60);
				this.table.SetWidthColumn(2, 101);
				this.table.SetWidthColumn(3, 18);
				this.table.SetWidthColumn(4, 18);
			}

			this.table.SetHeaderTextH(0, "");
			this.table.SetHeaderTextH(1, Res.Strings.Container.Layers.Header.Position);
			this.table.SetHeaderTextH(2, Res.Strings.Container.Layers.Header.Name);
			this.table.SetHeaderTextH(3, Res.Strings.Container.Layers.Header.Magnet);
			this.table.SetHeaderTextH(4, Res.Strings.Container.Layers.Header.Show);

			Objects.Page page = context.RootObject(1) as Objects.Page;
			for ( int i=0 ; i<rows ; i++ )
			{
				int ii = context.TotalLayers()-i-1;
				Objects.Layer layer = page.Objects[ii] as Objects.Layer;
				this.TableFillRow(i);
				this.TableUpdateRow(i, layer, ii==sel);
			}
		}

		protected void TableFillRow(int row)
		{
			//	Peuple une ligne de la table, si nécessaire.
			for ( int column=0 ; column<this.table.Columns ; column++ )
			{
				if ( this.table[column, row].IsEmpty )
				{
					if ( column == 3 )
					{
						IconButton ib = new IconButton();
						ib.Name = row.ToString();
						ib.IconUri = Misc.Icon("MagnetLayer");
						ib.PreferredIconSize = new Size(15, 15);  // petite taille spéciale
						ib.ButtonStyle = ButtonStyle.ActivableIcon;
						ib.Dock = DockStyle.Fill;
						ib.Margins = new Margins(-1, -1, -1, -1);
						ib.Clicked += this.HandleButtonMagnetLayerClicked;
						ToolTip.Default.SetToolTip(ib, Res.Strings.Action.MagnetLayer);
						this.table[column, row].Insert(ib);
					}
					else if ( column == 4 )
					{
						CheckButton bt = new CheckButton();
						bt.Name = row.ToString();
						bt.AcceptThreeState = true;
						bt.Dock = DockStyle.Fill;
						bt.Margins = new Margins(2, 0, 0, 0);
						bt.ActiveStateChanged += this.HandleCheckActiveStateChanged;
						ToolTip.Default.SetToolTip(bt, Res.Strings.Container.Layers.Tooltip.Show);
						this.table[column, row].Insert(bt);
					}
					else
					{
						StaticText st = new StaticText();
						st.ContentAlignment = (column==0) ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
						st.Dock = DockStyle.Fill;
						st.Margins = new Margins(4, 4, 0, 0);
						this.table[column, row].Insert(st);
					}
				}
			}
		}

		protected void TableUpdateRow(int row, Objects.Layer layer, bool select)
		{
			//	Met à jour le contenu d'une ligne de la table.
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			StaticText st;
			IconButton ib;
			CheckButton bt;

			this.ignoreChanged = true;

			st = this.table[0, row].Children[0] as StaticText;
			int n = context.TotalLayers()-row-1;
			st.Text = Objects.Layer.ShortName(n);

			st = this.table[1, row].Children[0] as StaticText;
			st.Text = Objects.Layer.LayerPositionName(n, context.TotalLayers());

			st = this.table[2, row].Children[0] as StaticText;
			st.Text = layer.Name;

			ib = this.table[3, row].Children[0] as IconButton;
			ib.Name = row.ToString();
			ib.ActiveState = layer.Magnet ? ActiveState.Yes : ActiveState.No;

			bt = this.table[4, row].Children[0] as CheckButton;
			bt.Name = row.ToString();
			ActiveState state = ActiveState.No;
			if ( layer.Type == Objects.LayerType.Show   )  state = ActiveState.Yes;
			if ( layer.Type == Objects.LayerType.Dimmed )  state = ActiveState.Maybe;
			if ( select )  state = ActiveState.Yes;
			bt.ActiveState = state;
			bt.Enable = (!select);

			this.table.SelectRow(row, n==context.CurrentLayer);

			this.ignoreChanged = false;
		}

		protected void UpdatePanel()
		{
			//	Met à jour le panneau pour éditer le calque sélectionné.
			this.UpdateLayerName();

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			Objects.Page page = context.RootObject(1) as Objects.Page;
			Objects.Layer layer = page.Objects[sel] as Objects.Layer;
			this.panelModColor.Property = layer.PropertyModColor;
		}

		protected void UpdateLayerName()
		{
			//	Met à jour le panneau pour éditer le nom du calque sélectionné.
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			string text = this.document.Modifier.LayerName(sel);

			this.ignoreChanged = true;
			this.name.Text = text;
			this.ignoreChanged = false;
		}


		private void HandleTableSelectionChanged(object sender)
		{
			//	Liste cliquée.
			if ( this.table.SelectedRow == -1 )  return;
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			context.CurrentLayer = context.TotalLayers()-this.table.SelectedRow-1;
		}

		private void HandleTableDoubleClicked(object sender, MessageEventArgs e)
		{
			//	Liste double-cliquée.
			this.name.SelectAll();
			this.name.Focus();
		}

		private void HandleNameTextChanged(object sender)
		{
			//	Le nom du calque a changé.
			if ( this.ignoreChanged )  return;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			if ( sel == -1 )  return;

			if ( this.document.Modifier.LayerName(sel) != this.name.Text )
			{
				this.document.Modifier.LayerName(sel, this.name.Text);
			}
		}

		private void HandleButtonMagnetLayerClicked(object sender, MessageEventArgs e)
		{
			//	Bouton "objets du calque magnétiques" dans la liste cliqué.
			if ( this.ignoreChanged )  return;

			IconButton ib = sender as IconButton;
			int sel = System.Convert.ToInt32(ib.Name);
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			if ( sel < 0 || sel >= context.TotalLayers() )  return;
			sel = context.TotalLayers()-sel-1;
			Objects.Page page = context.RootObject(1) as Objects.Page;
			Objects.Layer layer = page.Objects[sel] as Objects.Layer;

			using ( this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.LayerChangeMagnet) )
			{
				layer.Magnet = !layer.Magnet;

				this.document.Notifier.NotifyMagnetChanged();
				this.document.Notifier.NotifyPagesChanged();
				this.document.Notifier.NotifyLayersChanged();
				this.document.Modifier.OpletQueueValidateAction();
			}
		}

		private void HandleCheckActiveStateChanged(object sender)
		{
			//	Bouton "check" à 3 états dans la liste cliqué.
			if ( this.ignoreChanged )  return;

			CheckButton bt = sender as CheckButton;
			int sel = System.Convert.ToInt32(bt.Name);
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			if ( sel < 0 || sel >= context.TotalLayers() )  return;
			sel = context.TotalLayers()-sel-1;
			Objects.Page page = context.RootObject(1) as Objects.Page;
			Objects.Layer layer = page.Objects[sel] as Objects.Layer;

			using ( this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.LayerChangeShowOne) )
			{
				Objects.LayerType type = Objects.LayerType.None;
				if ( bt.ActiveState == ActiveState.Yes   )  type = Objects.LayerType.Show;
				if ( bt.ActiveState == ActiveState.No    )  type = Objects.LayerType.Hide;
				if ( bt.ActiveState == ActiveState.Maybe )  type = Objects.LayerType.Dimmed;
				layer.Type = type;

				this.document.Notifier.NotifyMagnetChanged();
				this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
				this.document.Modifier.OpletQueueValidateAction();
			}
		}


		private void ExtendedButtonClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton pour étendre/réduire le panneau a été cliqué.
			this.isExtended = !this.isExtended;
			this.UpdateExtended();
		}

		protected void UpdateExtended()
		{
			//	Met à jour l'état réduit/étendu du panneau.
			this.extendedButton.GlyphShape = this.isExtended ? GlyphShape.ArrowDown : GlyphShape.ArrowUp;

			this.panelMisc.Visibility = (this.isExtended);
			this.panelModColor.Visibility = (this.isExtended);
		}

		private void HandleRadioPrintChanged(object sender)
		{
			//	Un bouton radio a changé.
			if ( this.ignoreChanged )  return;

			RadioButton radio = sender as RadioButton;
			if ( radio == null )  return;
			if ( radio.ActiveState != ActiveState.Yes )  return;

			using ( this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.LayerChangePrint) )
			{
				DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
				Objects.LayerPrint print = Objects.LayerPrint.None;
				if ( sender == this.radioShowPrint   )  print = Objects.LayerPrint.Show;
				if ( sender == this.radioDimmedPrint )  print = Objects.LayerPrint.Dimmed;
				if ( sender == this.radioHidePrint   )  print = Objects.LayerPrint.Hide;
				int sel = context.CurrentLayer;
				Objects.Page page = context.RootObject(1) as Objects.Page;
				Objects.Layer layer = page.Objects[sel] as Objects.Layer;

				layer.Print = print;

				this.document.Notifier.NotifyPagesChanged();
				this.document.Notifier.NotifyMagnetChanged();
				this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
				this.document.Modifier.OpletQueueValidateAction();
			}
		}

		private void UpdateRadio()
		{
			//	Met à jour les boutons radio.
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			Objects.Page page = context.RootObject(1) as Objects.Page;
			Objects.Layer layer = page.Objects[sel] as Objects.Layer;

			Objects.LayerPrint print = layer.Print;
			this.radioShowPrint.ActiveState   = (print == Objects.LayerPrint.Show  ) ? ActiveState.Yes : ActiveState.No;
			this.radioDimmedPrint.ActiveState = (print == Objects.LayerPrint.Dimmed) ? ActiveState.Yes : ActiveState.No;
			this.radioHidePrint.ActiveState   = (print == Objects.LayerPrint.Hide  ) ? ActiveState.Yes : ActiveState.No;
		}


		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton a été cliqué.
			using ( this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.LayerChangeShowAll) )
			{
				DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
				Objects.LayerType type = Objects.LayerType.None;
				if ( sender == this.buttonShow   )  type = Objects.LayerType.Show;
				if ( sender == this.buttonDimmed )  type = Objects.LayerType.Dimmed;
				if ( sender == this.buttonHide   )  type = Objects.LayerType.Hide;
				Objects.Page page = context.RootObject(1) as Objects.Page;
				int total = context.TotalLayers();
				for ( int i=0 ; i<total ; i++ )
				{
					Objects.Layer layer = page.Objects[i] as Objects.Layer;
					layer.Type = type;
				}
				this.UpdateTable();
				this.document.Notifier.NotifyMagnetChanged();
				this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);

				this.document.Modifier.OpletQueueValidateAction();
			}
		}


		protected StaticText				helpText;
		protected HToolBar					toolBar;
		protected IconButton				buttonNew;
		protected IconButton				buttonDuplicate;
		protected IconButton				buttonNewSel;
		protected IconButton				buttonMergeUp;
		protected IconButton				buttonMergeDown;
		protected IconButton				buttonUp;
		protected IconButton				buttonDown;
		protected IconButton				buttonDelete;
		protected CellTable					table;
		protected HToolBar					toolBarName;
		protected TextField					name;
		protected GlyphButton				extendedButton;
		protected Widget					panelMisc;
		protected Widget					panelButton;
		protected Button					buttonShow;
		protected Button					buttonDimmed;
		protected Button					buttonHide;
		protected GroupBox					radioGroupPrint;
		protected RadioButton				radioShowPrint;
		protected RadioButton				radioDimmedPrint;
		protected RadioButton				radioHidePrint;
		protected Panels.ModColor			panelModColor;
		protected bool						isExtended = false;
		protected bool						ignoreChanged = false;
	}
}
