using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Layers contient tous les panneaux des calques.
	/// </summary>
	[SuppressBundleSupport]
	public class Layers : Abstract
	{
		public Layers(Document document) : base(document)
		{
			this.toolBar = new HToolBar(this);
			this.toolBar.Dock = DockStyle.Top;
			this.toolBar.DockMargins = new Margins(0, 0, 0, -1);
			this.toolBar.TabIndex = 1;
			this.toolBar.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			System.Diagnostics.Debug.Assert(this.toolBar.CommandDispatcher != null);

			int index = 0;

			this.buttonNew = new IconButton("LayerNew", Misc.Icon("LayerNew"));
			this.toolBar.Items.Add(this.buttonNew);
			this.buttonNew.TabIndex = index++;
			this.buttonNew.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonNew, Res.Strings.Action.LayerNewLong);
			this.Synchro(this.buttonNew);

			this.buttonDuplicate = new IconButton("LayerDuplicate", Misc.Icon("DuplicateItem"));
			this.toolBar.Items.Add(this.buttonDuplicate);
			this.buttonDuplicate.TabIndex = index++;
			this.buttonDuplicate.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonDuplicate, Res.Strings.Action.LayerDuplicate);
			this.Synchro(this.buttonDuplicate);

			this.buttonNewSel = new IconButton("LayerNewSel", Misc.Icon("LayerNewSel"));
			this.toolBar.Items.Add(this.buttonNewSel);
			this.buttonNewSel.TabIndex = index++;
			this.buttonNewSel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonNewSel, Res.Strings.Action.LayerNewSel);
			this.Synchro(this.buttonNewSel);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonMergeUp = new IconButton("LayerMergeUp", Misc.Icon("LayerMergeUp"));
			this.toolBar.Items.Add(this.buttonMergeUp);
			this.buttonMergeUp.TabIndex = index++;
			this.buttonMergeUp.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonMergeUp, Res.Strings.Action.LayerMergeUp);
			this.Synchro(this.buttonMergeUp);

			this.buttonMergeDown = new IconButton("LayerMergeDown", Misc.Icon("LayerMergeDown"));
			this.toolBar.Items.Add(this.buttonMergeDown);
			this.buttonMergeDown.TabIndex = index++;
			this.buttonMergeDown.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonMergeDown, Res.Strings.Action.LayerMergeDown);
			this.Synchro(this.buttonMergeDown);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonUp = new IconButton("LayerUp", Misc.Icon("Up"));
			this.toolBar.Items.Add(this.buttonUp);
			this.buttonUp.TabIndex = index++;
			this.buttonUp.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonUp, Res.Strings.Action.LayerUp);
			this.Synchro(this.buttonUp);

			this.buttonDown = new IconButton("LayerDown", Misc.Icon("Down"));
			this.toolBar.Items.Add(this.buttonDown);
			this.buttonDown.TabIndex = index++;
			this.buttonDown.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonDown, Res.Strings.Action.LayerDown);
			this.Synchro(this.buttonDown);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDelete = new IconButton("LayerDelete", Misc.Icon("DeleteItem"));
			this.toolBar.Items.Add(this.buttonDelete);
			this.buttonDelete.TabIndex = index++;
			this.buttonDelete.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonDelete, Res.Strings.Action.LayerDelete);
			this.Synchro(this.buttonDelete);

			this.table = new CellTable(this);
			this.table.Dock = DockStyle.Fill;
			this.table.SelectionChanged += new EventHandler(this.HandleTableSelectionChanged);
			this.table.DoubleClicked += new MessageEventHandler(this.HandleTableDoubleClicked);
			this.table.StyleH  = CellArrayStyle.ScrollNorm;
			this.table.StyleH |= CellArrayStyle.Header;
			this.table.StyleH |= CellArrayStyle.Separator;
			this.table.StyleH |= CellArrayStyle.Mobile;
			this.table.StyleV  = CellArrayStyle.ScrollNorm;
			this.table.StyleV |= CellArrayStyle.Separator;
			this.table.StyleV |= CellArrayStyle.SelectLine;
			this.table.DefHeight = 18;
			this.table.TabIndex = 2;
			this.table.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			Panels.Abstract.StaticDocument = this.document;
			this.panelModColor = new Panels.ModColor(this.document);
			this.panelModColor.IsExtendedSize = true;
			this.panelModColor.IsLayoutDirect = true;
			this.panelModColor.Dock = DockStyle.Bottom;
			this.panelModColor.DockMargins = new Margins(0, 0, 5, 0);
			this.panelModColor.SetParent(this);
			this.panelModColor.TabIndex = 100;
			this.panelModColor.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.checkMagnet = new CheckButton(this);
			this.checkMagnet.Text = Res.Strings.Container.Layers.Button.Magnet;
			this.checkMagnet.Dock = DockStyle.Bottom;
			this.checkMagnet.DockMargins = new Margins(0, 0, 5, 5);
			this.checkMagnet.Clicked += new MessageEventHandler(this.HandleCheckMagnetClicked);
			this.checkMagnet.TabIndex = 99;
			this.checkMagnet.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			// --- Début panelMisc
			this.panelMisc = new Widget(this);
			this.panelMisc.Dock = DockStyle.Bottom;
			this.panelMisc.DockMargins = new Margins(0, 0, 5, 0);
			this.panelMisc.Height = 70;
			this.panelMisc.TabIndex = 98;
			this.panelMisc.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			
			this.panelButton = new Widget(this.panelMisc);
			this.panelButton.Dock = DockStyle.Left;
			this.panelButton.DockMargins = new Margins(0, 0, 0, 0);
			this.panelButton.Width = 126;
			this.panelButton.Height = this.panelMisc.Height;
			this.panelButton.TabIndex = 1;
			this.panelButton.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			
			this.buttonShow = new Button(this.panelButton);
			this.buttonShow.Dock = DockStyle.Top;
			this.buttonShow.DockMargins = new Margins(0, 0, 0, 0);
			this.buttonShow.Text = Res.Strings.Container.Layers.Button.Show;
			this.buttonShow.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.buttonShow.TabIndex = 1;
			this.buttonShow.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonShow, Res.Strings.Container.Layers.Button.HelpShow);

			this.buttonDimmed = new Button(this.panelButton);
			this.buttonDimmed.Dock = DockStyle.Top;
			this.buttonDimmed.DockMargins = new Margins(0, 0, 0, 0);
			this.buttonDimmed.Text = Res.Strings.Container.Layers.Button.Dimmed;
			this.buttonDimmed.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.buttonDimmed.TabIndex = 2;
			this.buttonDimmed.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonDimmed, Res.Strings.Container.Layers.Button.HelpDimmed);

			this.buttonHide = new Button(this.panelButton);
			this.buttonHide.Dock = DockStyle.Top;
			this.buttonHide.DockMargins = new Margins(0, 0, 0, 0);
			this.buttonHide.Text = Res.Strings.Container.Layers.Button.Hide;
			this.buttonHide.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.buttonHide.TabIndex = 3;
			this.buttonHide.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonHide, Res.Strings.Container.Layers.Button.HelpHide);

			this.radioGroupPrint = new GroupBox(this.panelMisc);
			this.radioGroupPrint.Dock = DockStyle.Right;
			this.radioGroupPrint.DockMargins = new Margins(0, 0, 0, 4);
			this.radioGroupPrint.Width = 106;
			this.radioGroupPrint.Height = this.panelMisc.Height;
			this.radioGroupPrint.Text = Res.Strings.Container.Layers.Button.PrintGroup;
			this.radioGroupPrint.TabIndex = 2;
			this.radioGroupPrint.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.radioShowPrint = new RadioButton(this.radioGroupPrint);
			this.radioShowPrint.Dock = DockStyle.Top;
			this.radioShowPrint.DockMargins = new Margins(10, 10, 0, 0);
			this.radioShowPrint.Text = Res.Strings.Container.Layers.Button.PrintShow;
			this.radioShowPrint.ActiveStateChanged += new EventHandler(this.HandleRadioPrintChanged);
			this.radioShowPrint.Index = 1;
			this.radioShowPrint.TabIndex = 1;
			this.radioShowPrint.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.radioDimmedPrint = new RadioButton(this.radioGroupPrint);
			this.radioDimmedPrint.Dock = DockStyle.Top;
			this.radioDimmedPrint.DockMargins = new Margins(10, 10, 0, 0);
			this.radioDimmedPrint.Text = Res.Strings.Container.Layers.Button.PrintDimmed;
			this.radioDimmedPrint.ActiveStateChanged += new EventHandler(this.HandleRadioPrintChanged);
			this.radioDimmedPrint.Index = 2;
			this.radioDimmedPrint.TabIndex = 2;
			this.radioDimmedPrint.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.radioHidePrint = new RadioButton(this.radioGroupPrint);
			this.radioHidePrint.Dock = DockStyle.Top;
			this.radioHidePrint.DockMargins = new Margins(10, 10, 0, 0);
			this.radioHidePrint.Text = Res.Strings.Container.Layers.Button.PrintHide;
			this.radioHidePrint.ActiveStateChanged += new EventHandler(this.HandleRadioPrintChanged);
			this.radioHidePrint.Index = 3;
			this.radioHidePrint.TabIndex = 3;
			this.radioHidePrint.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			// --- Fin panelMisc
			
			this.extendedButton = new GlyphButton(this);
			this.extendedButton.Dock = DockStyle.Bottom;
			this.extendedButton.DockMargins = new Margins(0, 0, 5, 0);
			this.extendedButton.ButtonStyle = ButtonStyle.Icon;
			this.extendedButton.Clicked += new MessageEventHandler(this.ExtendedButtonClicked);
			this.extendedButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.extendedButton.TabIndex = 97;
			this.extendedButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.extendedButton, Res.Strings.Dialog.Button.More);

			
			this.toolBarName = new HToolBar(this);
			this.toolBarName.Dock = DockStyle.Bottom;
			this.toolBarName.DockMargins = new Margins(0, 0, 0, 0);
			this.toolBarName.TabIndex = 96;
			this.toolBarName.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			StaticText st = new StaticText();
			st.Width = 80;
			st.Text = Res.Strings.Panel.LayerName.Label.Name;
			this.toolBarName.Items.Add(st);

			this.name = new TextField();
			this.name.Width = 140;
			this.name.DockMargins = new Margins(0, 0, 1, 1);
			this.name.TextChanged += new EventHandler(this.HandleNameTextChanged);
			this.name.TabIndex = 1;
			this.name.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toolBarName.Items.Add(this.name);
			ToolTip.Default.SetToolTip(this.name, Res.Strings.Panel.LayerName.Tooltip.Name);


			this.UpdateExtended();
		}

		// Synchronise avec l'état de la commande.
		// TODO: devrait être inutile, à supprimer donc !!!
		protected void Synchro(Widget widget)
		{
			widget.SetEnabled(this.toolBar.CommandDispatcher[widget.Command].Enabled);
		}

		
		// Effectue la mise à jour du contenu.
		protected override void DoUpdateContent()
		{
			this.UpdateTable();
			this.UpdateRadio();
			this.UpdateMagnet();
			this.UpdatePanel();
		}

		// Effectue la mise à jour d'un objet.
		protected override void DoUpdateObject(Objects.Abstract obj)
		{
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

		// Met à jour le contenu de la table.
		protected void UpdateTable()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;

			int rows = context.TotalLayers();
			int initialColumns = this.table.Columns;
			this.table.SetArraySize(4, rows);

			if ( initialColumns == 0 )
			{
				this.table.SetWidthColumn(0, 20);
				this.table.SetWidthColumn(1, 60);
				this.table.SetWidthColumn(2, 119);
				this.table.SetWidthColumn(3, 18);
			}

			this.table.SetHeaderTextH(0, "");
			this.table.SetHeaderTextH(1, Res.Strings.Container.Layers.Header.Position);
			this.table.SetHeaderTextH(2, Res.Strings.Container.Layers.Header.Name);
			this.table.SetHeaderTextH(3, "");

			Objects.Page page = context.RootObject(1) as Objects.Page;
			for ( int i=0 ; i<rows ; i++ )
			{
				int ii = context.TotalLayers()-i-1;
				Objects.Layer layer = page.Objects[ii] as Objects.Layer;
				this.TableFillRow(i);
				this.TableUpdateRow(i, layer, ii==sel);
			}
		}

		// Peuple une ligne de la table, si nécessaire.
		protected void TableFillRow(int row)
		{
			for ( int column=0 ; column<this.table.Columns ; column++ )
			{
				if ( this.table[column, row].IsEmpty )
				{
					if ( column == 3 )
					{
						CheckButton bt = new CheckButton();
						bt.Name = row.ToString();
						bt.AcceptThreeState = true;
						bt.Dock = DockStyle.Fill;
						bt.DockMargins = new Margins(2, 0, 0, 0);
						bt.ActiveStateChanged += new EventHandler(this.HandleCheckActiveStateChanged);
						this.table[column, row].Insert(bt);
					}
					else
					{
						StaticText st = new StaticText();
						st.Alignment = (column==0) ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
						st.Dock = DockStyle.Fill;
						st.DockMargins = new Margins(4, 4, 0, 0);
						this.table[column, row].Insert(st);
					}
				}
			}
		}

		// Met à jour le contenu d'une ligne de la table.
		protected void TableUpdateRow(int row, Objects.Layer layer, bool select)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			StaticText st;
			CheckButton bt;

			this.ignoreChanged = true;

			st = this.table[0, row].Children[0] as StaticText;
			int n = context.TotalLayers()-row-1;
			st.Text = Objects.Layer.ShortName(n);

			st = this.table[1, row].Children[0] as StaticText;
			st.Text = Objects.Layer.LayerPositionName(n, context.TotalLayers());

			st = this.table[2, row].Children[0] as StaticText;
			st.Text = layer.Name;

			bt = this.table[3, row].Children[0] as CheckButton;
			bt.Name = row.ToString();
			ActiveState state = ActiveState.No;
			if ( layer.Type == Objects.LayerType.Show   )  state = ActiveState.Yes;
			if ( layer.Type == Objects.LayerType.Dimmed )  state = ActiveState.Maybe;
			if ( select )  state = ActiveState.Yes;
			bt.ActiveState = state;
			bt.SetEnabled(!select);

			this.table.SelectRow(row, n==context.CurrentLayer);

			this.ignoreChanged = false;
		}

		// Met à jour le panneau pour éditer la propriété sélectionnée.
		protected void UpdatePanel()
		{
			this.UpdateLayerName();

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			Objects.Page page = context.RootObject(1) as Objects.Page;
			Objects.Layer layer = page.Objects[sel] as Objects.Layer;
			this.panelModColor.Property = layer.PropertyModColor;
		}

		// Met à jour le panneau pour éditer le nom du calque sélectionné.
		protected void UpdateLayerName()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			string text = this.document.Modifier.LayerName(sel);

			this.ignoreChanged = true;
			this.name.Text = text;
			this.ignoreChanged = false;
		}


		// Liste cliquée.
		private void HandleTableSelectionChanged(object sender)
		{
			if ( this.table.SelectedRow == -1 )  return;
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			context.CurrentLayer = context.TotalLayers()-this.table.SelectedRow-1;
		}

		// Liste double-cliquée.
		private void HandleTableDoubleClicked(object sender, MessageEventArgs e)
		{
			this.name.SelectAll();
			this.name.Focus();
		}

		// Le nom du calque a changé.
		private void HandleNameTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			if ( sel == -1 )  return;

			if ( this.document.Modifier.LayerName(sel) != this.name.Text )
			{
				this.document.Modifier.LayerName(sel, this.name.Text);
			}
		}

		// Bouton "check" à 3 états dans la liste cliqué.
		private void HandleCheckActiveStateChanged(object sender)
		{
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


		// Le bouton pour étendre/réduire le panneau a été cliqué.
		private void ExtendedButtonClicked(object sender, MessageEventArgs e)
		{
			this.isExtended = !this.isExtended;
			this.UpdateExtended();
		}

		// Met à jour l'état réduit/étendu du panneau.
		protected void UpdateExtended()
		{
			this.extendedButton.GlyphShape = this.isExtended ? GlyphShape.ArrowDown : GlyphShape.ArrowUp;

			this.panelMisc.SetVisible(this.isExtended);
			this.checkMagnet.SetVisible(this.isExtended);
			this.panelModColor.SetVisible(this.isExtended);
		}

		// Un bouton radio a changé.
		private void HandleRadioPrintChanged(object sender)
		{
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
				this.document.Modifier.OpletQueueValidateAction();
			}
		}

		// Met à jour les boutons radio.
		private void UpdateRadio()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			Objects.Page page = context.RootObject(1) as Objects.Page;
			Objects.Layer layer = page.Objects[sel] as Objects.Layer;

			Objects.LayerPrint print = layer.Print;
			this.radioShowPrint.ActiveState   = (print == Objects.LayerPrint.Show  ) ? ActiveState.Yes : ActiveState.No;
			this.radioDimmedPrint.ActiveState = (print == Objects.LayerPrint.Dimmed) ? ActiveState.Yes : ActiveState.No;
			this.radioHidePrint.ActiveState   = (print == Objects.LayerPrint.Hide  ) ? ActiveState.Yes : ActiveState.No;
		}


		// Un bouton a été cliqué.
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
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

		// Le bouton "magnétique" a été cliqué.
		private void HandleCheckMagnetClicked(object sender, MessageEventArgs e)
		{
			using ( this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.LayerChangeMagnet) )
			{
				DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
				int sel = context.CurrentLayer;
				Objects.Page page = context.RootObject(1) as Objects.Page;
				Objects.Layer layer = page.Objects[sel] as Objects.Layer;
				layer.Magnet = !layer.Magnet;

				this.document.Notifier.NotifyMagnetChanged();
				this.document.Notifier.NotifyPagesChanged();
				this.document.Modifier.OpletQueueValidateAction();
			}
		}

		// Met à jour le bouton "magnétique".
		private void UpdateMagnet()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			Objects.Page page = context.RootObject(1) as Objects.Page;
			Objects.Layer layer = page.Objects[sel] as Objects.Layer;

			this.checkMagnet.ActiveState = layer.Magnet ? ActiveState.Yes : ActiveState.No;
		}


		protected HToolBar				toolBar;
		protected IconButton			buttonNew;
		protected IconButton			buttonDuplicate;
		protected IconButton			buttonNewSel;
		protected IconButton			buttonMergeUp;
		protected IconButton			buttonMergeDown;
		protected IconButton			buttonUp;
		protected IconButton			buttonDown;
		protected IconButton			buttonDelete;
		protected CellTable				table;
		protected HToolBar				toolBarName;
		protected TextField				name;
		protected GlyphButton			extendedButton;
		protected Widget				panelMisc;
		protected Widget				panelButton;
		protected Button				buttonShow;
		protected Button				buttonDimmed;
		protected Button				buttonHide;
		protected GroupBox				radioGroupPrint;
		protected RadioButton			radioShowPrint;
		protected RadioButton			radioDimmedPrint;
		protected RadioButton			radioHidePrint;
		protected CheckButton			checkMagnet;
		protected Panels.ModColor		panelModColor;
		protected bool					isExtended = false;
		protected bool					ignoreChanged = false;
	}
}
