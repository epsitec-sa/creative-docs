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

			this.buttonNew = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.LayerNew.icon");
			this.buttonNew.Clicked += new MessageEventHandler(this.HandleButtonNew);
			this.toolBar.Items.Add(this.buttonNew);
			ToolTip.Default.SetToolTip(this.buttonNew, "Nouveau calque <b>dessus</b> le calque courant");

			this.buttonDuplicate = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.Duplicate.icon");
			this.buttonDuplicate.Clicked += new MessageEventHandler(this.HandleButtonDuplicate);
			this.toolBar.Items.Add(this.buttonDuplicate);
			ToolTip.Default.SetToolTip(this.buttonDuplicate, "Dupliquer le calque");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonUp = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.Up.icon");
			this.buttonUp.Clicked += new MessageEventHandler(this.HandleButtonUp);
			this.toolBar.Items.Add(this.buttonUp);
			ToolTip.Default.SetToolTip(this.buttonUp, "Calque dessus");

			this.buttonDown = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.Down.icon");
			this.buttonDown.Clicked += new MessageEventHandler(this.HandleButtonDown);
			this.toolBar.Items.Add(this.buttonDown);
			ToolTip.Default.SetToolTip(this.buttonDown, "Calque dessous");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDelete = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.Delete.icon");
			this.buttonDelete.Clicked += new MessageEventHandler(this.HandleButtonDelete);
			this.toolBar.Items.Add(this.buttonDelete);
			ToolTip.Default.SetToolTip(this.buttonDelete, "Supprimer le calque");

			this.table = new CellTable(this);
			this.table.Dock = DockStyle.Fill;
			this.table.SelectionChanged += new EventHandler(this.HandleTableSelectionChanged);
			this.table.StyleH  = CellArrayStyle.ScrollNorm;
			this.table.StyleH |= CellArrayStyle.Header;
			this.table.StyleH |= CellArrayStyle.Separator;
			this.table.StyleH |= CellArrayStyle.Mobile;
			this.table.StyleV  = CellArrayStyle.ScrollNorm;
			this.table.StyleV |= CellArrayStyle.Separator;
			this.table.StyleV |= CellArrayStyle.SelectLine;
			this.table.DefHeight = 16;

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
			this.panelMisc.Height = 152;
			
			this.panelRadio = new Widget(this.panelMisc);
			this.panelRadio.Dock = DockStyle.Top;
			this.panelRadio.DockMargins = new Margins(0, 0, 5, 0);
			this.panelRadio.Height = 64;
			
			this.radioGroupType = new GroupBox(this.panelRadio);
			this.radioGroupType.Dock = DockStyle.Left;
			this.radioGroupType.DockMargins = new Margins(0, 0, 0, 0);
			this.radioGroupType.Width = 116;
			this.radioGroupType.Height = this.panelRadio.Height;
			this.radioGroupType.Text = "Si calque inactif :";

			this.radioShowType = new RadioButton(this.radioGroupType);
			this.radioShowType.Dock = DockStyle.Top;
			this.radioShowType.DockMargins = new Margins(10, 10, 0, 0);
			this.radioShowType.Text = "Afficher";
			this.radioShowType.Clicked += new MessageEventHandler(this.HandleRadioTypeClicked);

			this.radioDimmedType = new RadioButton(this.radioGroupType);
			this.radioDimmedType.Dock = DockStyle.Top;
			this.radioDimmedType.DockMargins = new Margins(10, 10, 0, 0);
			this.radioDimmedType.Text = "Estomper";
			this.radioDimmedType.Clicked += new MessageEventHandler(this.HandleRadioTypeClicked);

			this.radioHideType = new RadioButton(this.radioGroupType);
			this.radioHideType.Dock = DockStyle.Top;
			this.radioHideType.DockMargins = new Margins(10, 10, 0, 0);
			this.radioHideType.Text = "Cacher";
			this.radioHideType.Clicked += new MessageEventHandler(this.HandleRadioTypeClicked);

			this.radioGroupPrint = new GroupBox(this.panelRadio);
			this.radioGroupPrint.Dock = DockStyle.Right;
			this.radioGroupPrint.DockMargins = new Margins(0, 0, 0, 0);
			this.radioGroupPrint.Width = 116;
			this.radioGroupPrint.Height = this.panelRadio.Height;
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

			this.buttonShow = new Button(this.panelMisc);
			this.buttonShow.Dock = DockStyle.Top;
			this.buttonShow.DockMargins = new Margins(0, 0, 3, 0);
			this.buttonShow.Text = "Afficher normalement tous les calques";
			this.buttonShow.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonDimmed = new Button(this.panelMisc);
			this.buttonDimmed.Dock = DockStyle.Top;
			this.buttonDimmed.DockMargins = new Margins(0, 0, 3, 0);
			this.buttonDimmed.Text = "Afficher estompé les autres calques";
			this.buttonDimmed.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonHide = new Button(this.panelMisc);
			this.buttonHide.Dock = DockStyle.Top;
			this.buttonHide.DockMargins = new Margins(0, 0, 3, 0);
			this.buttonHide.Text = "Cacher les autres calques";
			this.buttonHide.Clicked += new MessageEventHandler(this.HandleButtonClicked);
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
		

		// Effectue la mise à jour du contenu.
		protected override void DoUpdateContent()
		{
			this.UpdateTable();
			this.UpdateRadio();
			this.UpdatePanel();
			this.UpdateToolBar();
		}

		// Effectue la mise à jour d'un objet.
		protected override void DoUpdateObject(Objects.Abstract obj)
		{
			Objects.Layer layer = obj as Objects.Layer;
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Page page = context.RootObject(1) as Objects.Page;
			int rank = page.Objects.IndexOf(obj);
			int i = context.TotalLayers()-rank-1;
			this.TableUpdateRow(i, layer);

			if ( rank == context.CurrentLayer )
			{
				this.UpdatePanel();
			}
		}

		// Met à jour les boutons de la toolbar.
		protected void UpdateToolBar()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int total = this.table.Rows;
			int sel = context.CurrentLayer;

			this.buttonDuplicate.SetEnabled(sel != -1);
			this.buttonUp.SetEnabled(sel != -1 && sel < total-1);
			this.buttonDown.SetEnabled(sel != -1 && sel > 0);
			this.buttonDelete.SetEnabled(sel != -1 && total > 1);
		}

		// Met à jour le contenu de la table.
		protected void UpdateTable()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;

			int rows = context.TotalLayers();
			int initialColumns = this.table.Columns;
			this.table.SetArraySize(3, rows);

			if ( initialColumns == 0 )
			{
				this.table.SetWidthColumn(0, 20);
				this.table.SetWidthColumn(1, 50);
				this.table.SetWidthColumn(2, 142);
			}

			this.table.SetHeaderTextH(0, "");
			this.table.SetHeaderTextH(1, "Position");
			this.table.SetHeaderTextH(2, "Nom");

			Objects.Page page = context.RootObject(1) as Objects.Page;
			for ( int i=0 ; i<rows ; i++ )
			{
				int ii = context.TotalLayers()-i-1;
				Objects.Layer layer = page.Objects[ii] as Objects.Layer;
				this.TableFillRow(i);
				this.TableUpdateRow(i, layer);
			}
		}

		// Peuple une ligne de la table, si nécessaire.
		protected void TableFillRow(int row)
		{
			for ( int column=0 ; column<this.table.Columns ; column++ )
			{
				if ( this.table[column, row].IsEmpty )
				{
					StaticText st = new StaticText();
					st.Alignment = (column==0) ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
					st.Dock = DockStyle.Fill;
					this.table[column, row].Insert(st);
				}
			}
		}

		// Met à jour le contenu d'une ligne de la table.
		protected void TableUpdateRow(int row, Objects.Layer layer)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			StaticText st;

			st = this.table[0, row].Children[0] as StaticText;
			int n = context.TotalLayers()-row-1;
			st.Text = ((char)('A'+n)).ToString();

			st = this.table[1, row].Children[0] as StaticText;
			st.Text = Objects.Layer.LayerPositionName(n, context.TotalLayers());

			st = this.table[2, row].Children[0] as StaticText;
			st.Text = layer.Name;

			this.table.SelectRow(row, n==context.CurrentLayer);
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


		// Crée un nouveau calque.
		private void HandleButtonNew(object sender, MessageEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			this.document.Modifier.LayerCreate(sel+1, "");
		}

		// Duplique un calque.
		private void HandleButtonDuplicate(object sender, MessageEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			this.document.Modifier.LayerDuplicate(sel, "");
		}

		// Monte d'une ligne le calque sélectionné.
		private void HandleButtonUp(object sender, MessageEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			this.document.Modifier.LayerSwap(sel, sel+1);
		}

		// Descend d'une ligne le calque sélectionné.
		private void HandleButtonDown(object sender, MessageEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			this.document.Modifier.LayerSwap(sel, sel-1);
		}

		// Supprime le calque sélectionné.
		private void HandleButtonDelete(object sender, MessageEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			this.document.Modifier.LayerDelete(sel);
		}

		// Liste cliquée.
		private void HandleTableSelectionChanged(object sender)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			context.CurrentLayer = context.TotalLayers()-this.table.SelectedRow-1;

			this.UpdateToolBar();
		}


		// Le bouton pour étendre/réduire le panneau a été cliqué.
		private void ExtendedButtonClicked(object sender, MessageEventArgs e)
		{
			this.isExtended = !this.isExtended;
			this.UpdateExtended();
		}

		// 
		protected void UpdateExtended()
		{
			this.extendedButton.GlyphShape = this.isExtended ? GlyphShape.ArrowDown : GlyphShape.ArrowUp;

			this.panelMisc.SetVisible(this.isExtended);
			this.panelModColor.SetVisible(this.isExtended);
		}

		// Un bouton radio a été cliqué.
		private void HandleRadioTypeClicked(object sender, MessageEventArgs e)
		{
			using ( this.document.Modifier.OpletQueueBeginAction() )
			{
				DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
				Objects.LayerType type = Objects.LayerType.None;
				if ( sender == this.radioShowType   )  type = Objects.LayerType.Show;
				if ( sender == this.radioDimmedType )  type = Objects.LayerType.Dimmed;
				if ( sender == this.radioHideType   )  type = Objects.LayerType.Hide;
				int sel = context.CurrentLayer;
				Objects.Page page = context.RootObject(1) as Objects.Page;
				Objects.Layer layer = page.Objects[sel] as Objects.Layer;

				layer.Type = type;

				this.document.Modifier.OpletQueueValidateAction();
			}
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

			Objects.LayerType type = layer.Type;
			this.radioShowType.ActiveState   = (type == Objects.LayerType.Show  ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.radioDimmedType.ActiveState = (type == Objects.LayerType.Dimmed) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.radioHideType.ActiveState   = (type == Objects.LayerType.Hide  ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;

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
				this.UpdateRadio();
				this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);

				this.document.Modifier.OpletQueueValidateAction();
			}
		}


		protected HToolBar				toolBar;
		protected IconButton			buttonNew;
		protected IconButton			buttonDuplicate;
		protected IconButton			buttonUp;
		protected IconButton			buttonDown;
		protected IconButton			buttonDelete;
		protected CellTable				table;
		protected Panels.LayerName		panelLayerName;
		protected GlyphButton			extendedButton;
		protected Widget				panelMisc;
		protected Widget				panelRadio;
		protected GroupBox				radioGroupType;
		protected RadioButton			radioShowType;
		protected RadioButton			radioDimmedType;
		protected RadioButton			radioHideType;
		protected GroupBox				radioGroupPrint;
		protected RadioButton			radioShowPrint;
		protected RadioButton			radioDimmedPrint;
		protected RadioButton			radioHidePrint;
		protected Button				buttonShow;
		protected Button				buttonDimmed;
		protected Button				buttonHide;
		protected Panels.ModColor		panelModColor;
		protected bool					isExtended = false;
		protected bool					ignoreChanged = false;
	}
}
