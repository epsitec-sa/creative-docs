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
			System.Diagnostics.Debug.Assert(this.toolBar.CommandDispatcher != null);

			this.buttonNew = new IconButton("LayerNew", "manifest:Epsitec.App.DocumentEditor.Images.LayerNew.icon");
			this.toolBar.Items.Add(this.buttonNew);
			ToolTip.Default.SetToolTip(this.buttonNew, "Nouveau calque <b>dessus</b> le calque courant");
			this.Synchro(this.buttonNew);

			this.buttonDuplicate = new IconButton("LayerDuplicate", "manifest:Epsitec.App.DocumentEditor.Images.DuplicateItem.icon");
			this.toolBar.Items.Add(this.buttonDuplicate);
			ToolTip.Default.SetToolTip(this.buttonDuplicate, "Dupliquer le calque");
			this.Synchro(this.buttonDuplicate);

			this.buttonNewSel = new IconButton("LayerNewSel", "manifest:Epsitec.App.DocumentEditor.Images.LayerNewSel.icon");
			this.toolBar.Items.Add(this.buttonNewSel);
			ToolTip.Default.SetToolTip(this.buttonNewSel, "Sélection dans un nouveau calque");
			this.Synchro(this.buttonNewSel);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonMergeUp = new IconButton("LayerMergeUp", "manifest:Epsitec.App.DocumentEditor.Images.LayerMergeUp.icon");
			this.toolBar.Items.Add(this.buttonMergeUp);
			ToolTip.Default.SetToolTip(this.buttonMergeUp, "Fusionne avec le calque dessus");
			this.Synchro(this.buttonMergeUp);

			this.buttonMergeDown = new IconButton("LayerMergeDown", "manifest:Epsitec.App.DocumentEditor.Images.LayerMergeDown.icon");
			this.toolBar.Items.Add(this.buttonMergeDown);
			ToolTip.Default.SetToolTip(this.buttonMergeDown, "Fusionne avec le calque dessous");
			this.Synchro(this.buttonMergeDown);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonUp = new IconButton("LayerUp", "manifest:Epsitec.App.DocumentEditor.Images.Up.icon");
			this.toolBar.Items.Add(this.buttonUp);
			ToolTip.Default.SetToolTip(this.buttonUp, "Calque dessus");
			this.Synchro(this.buttonUp);

			this.buttonDown = new IconButton("LayerDown", "manifest:Epsitec.App.DocumentEditor.Images.Down.icon");
			this.toolBar.Items.Add(this.buttonDown);
			ToolTip.Default.SetToolTip(this.buttonDown, "Calque dessous");
			this.Synchro(this.buttonDown);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDelete = new IconButton("LayerDelete", "manifest:Epsitec.App.DocumentEditor.Images.DeleteItem.icon");
			this.toolBar.Items.Add(this.buttonDelete);
			ToolTip.Default.SetToolTip(this.buttonDelete, "Supprimer le calque");
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

			this.panelModColor = new Panels.ModColor(this.document);
			this.panelModColor.IsExtendedSize = true;
			this.panelModColor.IsLayoutDirect = true;
			this.panelModColor.TabIndex = 101;
			this.panelModColor.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			this.panelModColor.Dock = DockStyle.Bottom;
			this.panelModColor.DockMargins = new Margins(0, 0, 5, 0);
			this.panelModColor.Parent = this;

			// --- Début panelMisc
			this.panelMisc = new Widget(this);
			this.panelMisc.Dock = DockStyle.Bottom;
			this.panelMisc.DockMargins = new Margins(0, 0, 5, 0);
			this.panelMisc.Height = 70;
			
			this.panelButton = new Widget(this.panelMisc);
			this.panelButton.Dock = DockStyle.Left;
			this.panelButton.DockMargins = new Margins(0, 0, 0, 0);
			this.panelButton.Width = 126;
			this.panelButton.Height = this.panelMisc.Height;
			
			this.buttonShow = new Button(this.panelButton);
			this.buttonShow.Dock = DockStyle.Top;
			this.buttonShow.DockMargins = new Margins(0, 0, 0, 0);
			this.buttonShow.Text = "Afficher tous";
			this.buttonShow.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			ToolTip.Default.SetToolTip(this.buttonShow, "Afficher normalement tous les calques");

			this.buttonDimmed = new Button(this.panelButton);
			this.buttonDimmed.Dock = DockStyle.Top;
			this.buttonDimmed.DockMargins = new Margins(0, 0, 0, 0);
			this.buttonDimmed.Text = "Estomper les autres";
			this.buttonDimmed.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			ToolTip.Default.SetToolTip(this.buttonDimmed, "Afficher estompé les autres calques");

			this.buttonHide = new Button(this.panelButton);
			this.buttonHide.Dock = DockStyle.Top;
			this.buttonHide.DockMargins = new Margins(0, 0, 0, 0);
			this.buttonHide.Text = "Cacher les autres";
			this.buttonHide.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			ToolTip.Default.SetToolTip(this.buttonHide, "Cacher les autres calques");

			this.radioGroupPrint = new GroupBox(this.panelMisc);
			this.radioGroupPrint.Dock = DockStyle.Right;
			this.radioGroupPrint.DockMargins = new Margins(0, 0, 0, 4);
			this.radioGroupPrint.Width = 106;
			this.radioGroupPrint.Height = this.panelMisc.Height;
			this.radioGroupPrint.Text = "Si impression :";

			this.radioShowPrint = new RadioButton(this.radioGroupPrint);
			this.radioShowPrint.Dock = DockStyle.Top;
			this.radioShowPrint.DockMargins = new Margins(10, 10, 0, 0);
			this.radioShowPrint.Text = "Imprimer";
			this.radioShowPrint.Clicked += new MessageEventHandler(this.HandleRadioPrintClicked);

			this.radioDimmedPrint = new RadioButton(this.radioGroupPrint);
			this.radioDimmedPrint.Dock = DockStyle.Top;
			this.radioDimmedPrint.DockMargins = new Margins(10, 10, 0, 0);
			this.radioDimmedPrint.Text = "Estomper";
			this.radioDimmedPrint.Clicked += new MessageEventHandler(this.HandleRadioPrintClicked);

			this.radioHidePrint = new RadioButton(this.radioGroupPrint);
			this.radioHidePrint.Dock = DockStyle.Top;
			this.radioHidePrint.DockMargins = new Margins(10, 10, 0, 0);
			this.radioHidePrint.Text = "Cacher";
			this.radioHidePrint.Clicked += new MessageEventHandler(this.HandleRadioPrintClicked);
			// --- Fin panelMisc
			
			this.extendedButton = new GlyphButton(this);
			this.extendedButton.Dock = DockStyle.Bottom;
			this.extendedButton.DockMargins = new Margins(0, 0, 5, 0);
			this.extendedButton.ButtonStyle = ButtonStyle.Icon;
			this.extendedButton.Clicked += new MessageEventHandler(this.ExtendedButtonClicked);
			this.extendedButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.extendedButton, "Etend ou réduit le panneau");
			
			this.panelLayerName = new Panels.LayerName(this.document);
			this.panelLayerName.IsExtendedSize = false;
			this.panelLayerName.IsLayoutDirect = true;
			this.panelLayerName.TabIndex = 100;
			this.panelLayerName.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			this.panelLayerName.Dock = DockStyle.Bottom;
			this.panelLayerName.DockMargins = new Margins(0, 0, 5, 0);
			this.panelLayerName.Parent = this;

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
				this.table.SetWidthColumn(1, 50);
				this.table.SetWidthColumn(2, 124);
				this.table.SetWidthColumn(3, 18);
			}

			this.table.SetHeaderTextH(0, "");
			this.table.SetHeaderTextH(1, "Position");
			this.table.SetHeaderTextH(2, "Nom");
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
			WidgetState state = WidgetState.ActiveNo;
			if ( layer.Type == Objects.LayerType.Show   )  state = WidgetState.ActiveYes;
			if ( layer.Type == Objects.LayerType.Dimmed )  state = WidgetState.ActiveMaybe;
			bt.ActiveState = state;
			bt.SetEnabled(!select);

			this.table.SelectRow(row, n==context.CurrentLayer);

			this.ignoreChanged = false;
		}

		// Met à jour le panneau pour éditer la propriété sélectionnée.
		protected void UpdatePanel()
		{
			this.panelLayerName.UpdateValues();

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			Objects.Page page = context.RootObject(1) as Objects.Page;
			Objects.Layer layer = page.Objects[sel] as Objects.Layer;
			this.panelModColor.Property = layer.PropertyModColor;
		}


		// Liste cliquée.
		private void HandleTableSelectionChanged(object sender)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			context.CurrentLayer = context.TotalLayers()-this.table.SelectedRow-1;
		}

		// Liste double-cliquée.
		private void HandleTableDoubleClicked(object sender, MessageEventArgs e)
		{
			this.panelLayerName.SetDefaultFocus();
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

			using ( this.document.Modifier.OpletQueueBeginAction() )
			{
				Objects.LayerType type = Objects.LayerType.None;
				if ( bt.ActiveState == WidgetState.ActiveYes   )  type = Objects.LayerType.Show;
				if ( bt.ActiveState == WidgetState.ActiveNo    )  type = Objects.LayerType.Hide;
				if ( bt.ActiveState == WidgetState.ActiveMaybe )  type = Objects.LayerType.Dimmed;
				layer.Type = type;

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
			this.panelModColor.SetVisible(this.isExtended);
		}

		// Un bouton radio a été cliqué.
		private void HandleRadioPrintClicked(object sender, MessageEventArgs e)
		{
			using ( this.document.Modifier.OpletQueueBeginAction() )
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
			this.radioShowPrint.ActiveState   = (print == Objects.LayerPrint.Show  ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.radioDimmedPrint.ActiveState = (print == Objects.LayerPrint.Dimmed) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.radioHidePrint.ActiveState   = (print == Objects.LayerPrint.Hide  ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}


		// Un bouton a été cliqué.
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			using ( this.document.Modifier.OpletQueueBeginAction() )
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
				this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);

				this.document.Modifier.OpletQueueValidateAction();
			}
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
		protected Panels.LayerName		panelLayerName;
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
		protected Panels.ModColor		panelModColor;
		protected bool					isExtended = false;
		protected bool					ignoreChanged = false;
	}
}
