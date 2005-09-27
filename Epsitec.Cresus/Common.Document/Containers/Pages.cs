using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Pages contient tous les panneaux des pages.
	/// </summary>
	[SuppressBundleSupport]
	public class Pages : Abstract
	{
		public Pages(Document document) : base(document)
		{
			this.toolBar = new HToolBar(this);
			this.toolBar.Dock = DockStyle.Top;
			this.toolBar.DockMargins = new Margins(0, 0, 0, -1);
			System.Diagnostics.Debug.Assert(this.toolBar.CommandDispatcher != null);

			this.buttonNew = new IconButton("PageNew", Misc.Icon("PageNew"));
			this.toolBar.Items.Add(this.buttonNew);
			ToolTip.Default.SetToolTip(this.buttonNew, Res.Strings.Action.PageNewLong);
			this.Synchro(this.buttonNew);

			this.buttonDuplicate = new IconButton("PageDuplicate", Misc.Icon("DuplicateItem"));
			this.toolBar.Items.Add(this.buttonDuplicate);
			ToolTip.Default.SetToolTip(this.buttonDuplicate, Res.Strings.Action.PageDuplicate);
			this.Synchro(this.buttonDuplicate);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonUp = new IconButton("PageUp", Misc.Icon("Up"));
			this.toolBar.Items.Add(this.buttonUp);
			ToolTip.Default.SetToolTip(this.buttonUp, Res.Strings.Action.PageUp);
			this.Synchro(this.buttonUp);

			this.buttonDown = new IconButton("PageDown", Misc.Icon("Down"));
			this.toolBar.Items.Add(this.buttonDown);
			ToolTip.Default.SetToolTip(this.buttonDown, Res.Strings.Action.PageDown);
			this.Synchro(this.buttonDown);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDelete = new IconButton("PageDelete", Misc.Icon("DeleteItem"));
			this.toolBar.Items.Add(this.buttonDelete);
			ToolTip.Default.SetToolTip(this.buttonDelete, Res.Strings.Action.PageDelete);
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
			this.table.DefHeight = 16;

			// --- Début panelMisc
			this.buttonPageStack = new Button(this);
			this.buttonPageStack.Dock = DockStyle.Bottom;
			this.buttonPageStack.DockMargins = new Margins(0, 0, 0, 0);
			this.buttonPageStack.Command = "PageStack";
			this.buttonPageStack.Text = Res.Strings.Container.Pages.Button.PageStack;

			
			this.panelMisc = new Widget(this);
			this.panelMisc.Dock = DockStyle.Bottom;
			this.panelMisc.DockMargins = new Margins(0, 0, 5, 0);
			this.panelMisc.Height = 160;

			this.radioMasterGroup = new GroupBox(this.panelMisc);
			this.radioMasterGroup.Dock = DockStyle.Bottom;
			this.radioMasterGroup.DockMargins = new Margins(0, 0, 0, 4);
			this.radioMasterGroup.Height = 130;
			this.radioMasterGroup.Text = Res.Strings.Container.Pages.Button.MasterGroup;

			this.radioAll = new RadioButton(this.radioMasterGroup);
			this.radioAll.Dock = DockStyle.Top;
			this.radioAll.DockMargins = new Margins(10, 10, 5, 0);
			this.radioAll.Text = Res.Strings.Container.Pages.Button.MasterAll;
			this.radioAll.Clicked += new MessageEventHandler(this.HandleRadioClicked);	//@@
			this.radioAll.Index = 1;

			this.radioOdd = new RadioButton(this.radioMasterGroup);
			this.radioOdd.Dock = DockStyle.Top;
			this.radioOdd.DockMargins = new Margins(10, 10, 0, 0);
			this.radioOdd.Text = Res.Strings.Container.Pages.Button.MasterOdd;
			this.radioOdd.Clicked += new MessageEventHandler(this.HandleRadioClicked);	//@@
			this.radioOdd.Index = 2;

			this.radioEven = new RadioButton(this.radioMasterGroup);
			this.radioEven.Dock = DockStyle.Top;
			this.radioEven.DockMargins = new Margins(10, 10, 0, 0);
			this.radioEven.Text = Res.Strings.Container.Pages.Button.MasterEven;
			this.radioEven.Clicked += new MessageEventHandler(this.HandleRadioClicked);	//@@
			this.radioEven.Index = 3;

			this.radioNone = new RadioButton(this.radioMasterGroup);
			this.radioNone.Dock = DockStyle.Top;
			this.radioNone.DockMargins = new Margins(10, 10, 0, 0);
			this.radioNone.Text = Res.Strings.Container.Pages.Button.MasterNone;
			this.radioNone.Clicked += new MessageEventHandler(this.HandleRadioClicked);	//@@
			this.radioNone.Index = 4;

			this.checkAutoStop = new CheckButton(this.radioMasterGroup);
			this.checkAutoStop.Dock = DockStyle.Top;
			this.checkAutoStop.DockMargins = new Margins(10, 10, 4, 0);
			this.checkAutoStop.Text = Res.Strings.Container.Pages.Button.MasterStop;
			this.checkAutoStop.Clicked += new MessageEventHandler(this.HandleCheckClicked);

			this.specificGroup = new Widget(this.radioMasterGroup);
			this.specificGroup.Dock = DockStyle.Top;
			this.specificGroup.DockMargins = new Margins(10, 10, 2, 0);
			this.specificGroup.Width = 170;

			this.checkSpecific = new CheckButton(this.specificGroup);
			this.checkSpecific.Width = 160;
			this.checkSpecific.Dock = DockStyle.Left;
			this.checkSpecific.DockMargins = new Margins(0, 0, 0, 0);
			this.checkSpecific.Text = Res.Strings.Container.Pages.Button.MasterSpecific;
			this.checkSpecific.Clicked += new MessageEventHandler(this.HandleCheckClicked);

			this.specificMasterPage = new TextFieldCombo(this.specificGroup);
			this.specificMasterPage.Width = 50;
			this.specificMasterPage.IsReadOnly = true;
			this.specificMasterPage.Dock = DockStyle.Left;
			this.specificMasterPage.DockMargins = new Margins(0, 0, 0, 0);
			this.specificMasterPage.OpeningCombo += new CancelEventHandler(this.HandleOpeningCombo);
			this.specificMasterPage.ClosedCombo += new EventHandler(this.HandleClosedCombo);


			this.radioSlaveGroup = new GroupBox(this.panelMisc);
			this.radioSlaveGroup.Dock = DockStyle.Bottom;
			this.radioSlaveGroup.DockMargins = new Margins(0, 0, 0, 4);
			this.radioSlaveGroup.Height = 130;
			this.radioSlaveGroup.Text = Res.Strings.Container.Pages.Button.SlaveGroup;

			this.radioGroupLeft = new Widget(this.radioSlaveGroup);
			this.radioGroupLeft.Dock = DockStyle.Left;
			this.radioGroupLeft.DockMargins = new Margins(0, 0, 5, 0);
			this.radioGroupLeft.Width = 170;

			this.radioNever = new RadioButton(this.radioGroupLeft);
			this.radioNever.Dock = DockStyle.Top;
			this.radioNever.DockMargins = new Margins(10, 0, 0, 0);
			this.radioNever.Text = Res.Strings.Container.Pages.Button.SlaveNever;
			this.radioNever.Clicked += new MessageEventHandler(this.HandleRadioClicked);	//@@
			this.radioNever.Index = 1;

			this.radioDefault = new RadioButton(this.radioGroupLeft);
			this.radioDefault.Dock = DockStyle.Top;
			this.radioDefault.DockMargins = new Margins(10, 0, 0, 0);
			this.radioDefault.Text = Res.Strings.Container.Pages.Button.SlaveDefault;
			this.radioDefault.Clicked += new MessageEventHandler(this.HandleRadioClicked);	//@@
			this.radioDefault.Index = 2;

			this.radioSpecific = new RadioButton(this.radioGroupLeft);
			this.radioSpecific.Dock = DockStyle.Top;
			this.radioSpecific.DockMargins = new Margins(10, 0, 0, 0);
			this.radioSpecific.Text = Res.Strings.Container.Pages.Button.SlaveSpecific;
			this.radioSpecific.Clicked += new MessageEventHandler(this.HandleRadioClicked);	//@@
			this.radioSpecific.Index = 3;

			this.checkGuides = new CheckButton(this.radioGroupLeft);
			this.checkGuides.Dock = DockStyle.Top;
			this.checkGuides.DockMargins = new Margins(10, 0, 4, 0);
			this.checkGuides.Text = Res.Strings.Container.Pages.Button.SlaveGuides;
			this.checkGuides.Clicked += new MessageEventHandler(this.HandleCheckClicked);

			this.radioGroupRight = new Widget(this.radioSlaveGroup);
			this.radioGroupRight.Dock = DockStyle.Left;
			this.radioGroupRight.DockMargins = new Margins(0, 0, 5, 0);
			this.radioGroupRight.Width = 50;

			this.specificSlavePage = new TextFieldCombo(this.radioGroupRight);
			this.specificSlavePage.IsReadOnly = true;
			this.specificSlavePage.Dock = DockStyle.Bottom;
			this.specificSlavePage.DockMargins = new Margins(0, 0, 0, 61);
			this.specificSlavePage.OpeningCombo += new CancelEventHandler(this.HandleOpeningCombo);
			this.specificSlavePage.ClosedCombo += new EventHandler(this.HandleClosedCombo);


			this.radioGroup = new Widget(this.panelMisc);
			this.radioGroup.Dock = DockStyle.Bottom;
			this.radioGroup.DockMargins = new Margins(0, 0, 0, 4);
			this.radioGroup.Height = 20;

			this.radioSlave = new RadioButton(this.radioGroup);
			this.radioSlave.Width = 100;
			this.radioSlave.Dock = DockStyle.Left;
			this.radioSlave.DockMargins = new Margins(10, 10, 0, 0);
			this.radioSlave.Text = Res.Strings.Container.Pages.Button.Slave;
			this.radioSlave.Clicked += new MessageEventHandler(this.HandleRadioClicked);	//@@
			this.radioSlave.Index = 1;

			this.radioMaster = new RadioButton(this.radioGroup);
			this.radioMaster.Width = 100;
			this.radioMaster.Dock = DockStyle.Left;
			this.radioMaster.DockMargins = new Margins(10, 10, 0, 0);
			this.radioMaster.Text = Res.Strings.Container.Pages.Button.Master;
			this.radioMaster.Clicked += new MessageEventHandler(this.HandleRadioClicked);	//@@
			this.radioMaster.Index = 2;
			// --- Fin panelMisc
			
			this.extendedButton = new GlyphButton(this);
			this.extendedButton.Dock = DockStyle.Bottom;
			this.extendedButton.DockMargins = new Margins(0, 0, 5, 0);
			this.extendedButton.ButtonStyle = ButtonStyle.Icon;
			this.extendedButton.Clicked += new MessageEventHandler(this.ExtendedButtonClicked);
			this.extendedButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.extendedButton, Res.Strings.Dialog.Button.More);
			

			this.toolBarName = new HToolBar(this);
			this.toolBarName.Dock = DockStyle.Bottom;
			this.toolBarName.DockMargins = new Margins(0, 0, 0, 0);
			this.toolBarName.TabIndex = 100;
			this.toolBarName.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;

			StaticText st = new StaticText();
			st.Width = 80;
			st.Text = Res.Strings.Panel.PageName.Label.Name;
			this.toolBarName.Items.Add(st);

			this.name = new TextField();
			this.name.Width = 140;
			this.name.DockMargins = new Margins(0, 0, 1, 1);
			this.name.TextChanged += new EventHandler(this.HandleNameTextChanged);
			this.toolBarName.Items.Add(this.name);
			ToolTip.Default.SetToolTip(this.name, Res.Strings.Panel.PageName.Tooltip.Name);

			
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
			this.UpdatePanel();
		}

		// Effectue la mise à jour d'un objet.
		protected override void DoUpdateObject(Objects.Abstract obj)
		{
			Objects.Page page = obj as Objects.Page;
			UndoableList pages = this.document.GetObjects;
			int rank = pages.IndexOf(obj);
			this.TableUpdateRow(rank, page);
		}

		// Met à jour le contenu de la table.
		protected void UpdateTable()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentPage;

			int rows = context.TotalPages();
			int initialColumns = this.table.Columns;
			this.table.SetArraySize(2, rows);

			if ( initialColumns == 0 )
			{
				this.table.SetWidthColumn(0, 40);
				this.table.SetWidthColumn(1, 177);
			}

			this.table.SetHeaderTextH(0, Res.Strings.Container.Pages.Header.Number);
			this.table.SetHeaderTextH(1, Res.Strings.Container.Pages.Header.Name);

			UndoableList doc = this.document.GetObjects;
			for ( int i=0 ; i<rows ; i++ )
			{
				Objects.Page page = doc[i] as Objects.Page;
				this.TableFillRow(i);
				this.TableUpdateRow(i, page);
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
					st.DockMargins = new Margins(4, 4, 0, 0);
					this.table[column, row].Insert(st);
				}
			}
		}

		// Met à jour le contenu d'une ligne de la table.
		protected void TableUpdateRow(int row, Objects.Page page)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			StaticText st;

			st = this.table[0, row].Children[0] as StaticText;
			st.Text = page.ShortName;

			st = this.table[1, row].Children[0] as StaticText;
			st.Text = page.Name;

			this.table.SelectRow(row, row==context.CurrentPage);
		}

		// Met à jour le panneau pour éditer la propriété sélectionnée.
		protected void UpdatePanel()
		{
			this.UpdatePageName();

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Page page = context.RootObject(1) as Objects.Page;

			this.radioSlave.ActiveState  = (page.MasterType == Objects.MasterType.Slave) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.radioMaster.ActiveState = (page.MasterType != Objects.MasterType.Slave) ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			this.radioSlaveGroup.SetVisible (page.MasterType == Objects.MasterType.Slave);
			this.radioMasterGroup.SetVisible(page.MasterType != Objects.MasterType.Slave);

			this.radioAll.ActiveState =  (page.MasterType == Objects.MasterType.All ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.radioEven.ActiveState = (page.MasterType == Objects.MasterType.Even) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.radioOdd.ActiveState =  (page.MasterType == Objects.MasterType.Odd ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.radioNone.ActiveState = (page.MasterType == Objects.MasterType.None) ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			this.radioNever.ActiveState    = (page.MasterUse == Objects.MasterUse.Never   ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.radioDefault.ActiveState  = (page.MasterUse == Objects.MasterUse.Default ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.radioSpecific.ActiveState = (page.MasterUse == Objects.MasterUse.Specific) ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			this.checkGuides.ActiveState = page.MasterGuides ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			this.specificSlavePage.SetEnabled(page.MasterUse == Objects.MasterUse.Specific);
			if ( page.MasterPageToUse == null ||
				 page.MasterPageToUse.MasterType == Objects.MasterType.Slave )
			{
				this.specificSlavePage.Text = "";
			}
			else
			{
				this.specificSlavePage.Text = page.MasterPageToUse.ShortName;
			}

			this.checkAutoStop.ActiveState = page.MasterAutoStop ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.checkSpecific.ActiveState = page.MasterSpecific ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			this.specificMasterPage.SetEnabled(page.MasterSpecific);
			if ( page.MasterPageToUse == null || !page.MasterSpecific )
			{
				this.specificMasterPage.Text = "";
			}
			else
			{
				this.specificMasterPage.Text = page.MasterPageToUse.ShortName;
			}
		}

		// Met à jour le panneau pour éditer le nom de la page sélectionnée.
		protected void UpdatePageName()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentPage;
			string text = this.document.Modifier.PageName(sel);

			this.ignoreChanged = true;
			this.name.Text = text;
			this.ignoreChanged = false;
		}


		// Liste cliquée.
		private void HandleTableSelectionChanged(object sender)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			context.CurrentPage = this.table.SelectedRow;
		}

		// Liste double-cliquée.
		private void HandleTableDoubleClicked(object sender, MessageEventArgs e)
		{
			this.name.SelectAll();
			this.name.Focus();
		}

		// Le nom de la page a changé.
		private void HandleNameTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentPage;

			if ( this.document.Modifier.PageName(sel) != this.name.Text )
			{
				this.document.Modifier.PageName(sel, this.name.Text);
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
			this.buttonPageStack.SetVisible(this.isExtended);
		}

		// Un bouton radio a été cliqué.
		private void HandleRadioClicked(object sender, MessageEventArgs e)
		{
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.PageChangeStatus);

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Page page = context.RootObject(1) as Objects.Page;

			if ( sender == this.radioSlave )
			{
				page.MasterType = Objects.MasterType.Slave;
			}
			if ( sender == this.radioMaster )
			{
				page.MasterType = Objects.MasterType.All;
			}

			if ( sender == this.radioAll )
			{
				page.MasterType = Objects.MasterType.All;
			}
			if ( sender == this.radioEven )
			{
				page.MasterType = Objects.MasterType.Even;
			}
			if ( sender == this.radioOdd )
			{
				page.MasterType = Objects.MasterType.Odd;
			}
			if ( sender == this.radioNone )
			{
				page.MasterType = Objects.MasterType.None;
			}

			if ( sender == this.radioNever )
			{
				page.MasterUse = Objects.MasterUse.Never;
			}
			if ( sender == this.radioDefault )
			{
				page.MasterUse = Objects.MasterUse.Default;
			}
			if ( sender == this.radioSpecific )
			{
				page.MasterUse = Objects.MasterUse.Specific;
			}

			this.UpdatePanel();
			this.document.Notifier.NotifyPagesChanged();
			this.document.Modifier.OpletQueueValidateAction();
		}

		// Un bouton à cocher a été cliqué.
		private void HandleCheckClicked(object sender, MessageEventArgs e)
		{
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.PageChangeStatus);

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Page page = context.RootObject(1) as Objects.Page;

			if ( sender == this.checkGuides )
			{
				page.MasterGuides = !page.MasterGuides;
			}

			if ( sender == this.checkAutoStop )
			{
				page.MasterAutoStop = !page.MasterAutoStop;
			}

			if ( sender == this.checkSpecific )
			{
				page.MasterSpecific = !page.MasterSpecific;
			}

			this.UpdatePanel();
			this.document.Notifier.NotifyPagesChanged();
			this.document.Modifier.OpletQueueValidateAction();
		}

		// Combo ouvert.
		private void HandleOpeningCombo(object sender, CancelEventArgs e)
		{
			TextFieldCombo field = sender as TextFieldCombo;
			field.Items.Clear();

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			UndoableList doc = this.document.GetObjects;
			Objects.Page currentPage = context.RootObject(1) as Objects.Page;
			int total = context.TotalPages();
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Page page = doc[i] as Objects.Page;
				if ( page == currentPage )  continue;
				if ( page.MasterType != Objects.MasterType.Slave )
				{
					field.Items.Add(page.ShortName);
				}
			}
		}

		// Combo fermé.
		private void HandleClosedCombo(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextFieldCombo field = sender as TextFieldCombo;
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			UndoableList doc = this.document.GetObjects;
			int total = context.TotalPages();
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Page page = doc[i] as Objects.Page;
				if ( page.ShortName == field.Text )
				{
					this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.PageChangeStatus);
					Objects.Page currentPage = context.RootObject(1) as Objects.Page;
					currentPage.MasterPageToUse = page;
					this.document.Notifier.NotifyPagesChanged();
					this.document.Modifier.OpletQueueValidateAction();
					return;
				}
			}
		}


		protected HToolBar				toolBar;
		protected IconButton			buttonNew;
		protected IconButton			buttonDuplicate;
		protected IconButton			buttonUp;
		protected IconButton			buttonDown;
		protected IconButton			buttonDelete;
		protected CellTable				table;
		protected HToolBar				toolBarName;
		protected TextField				name;
		protected GlyphButton			extendedButton;
		protected Widget				panelMisc;

		protected Widget				radioGroup;
		protected RadioButton			radioSlave;
		protected RadioButton			radioMaster;

		protected GroupBox				radioMasterGroup;
		protected RadioButton			radioAll;
		protected RadioButton			radioEven;
		protected RadioButton			radioOdd;
		protected RadioButton			radioNone;
		protected CheckButton			checkAutoStop;
		protected Widget				specificGroup;
		protected CheckButton			checkSpecific;
		protected TextFieldCombo		specificMasterPage;

		protected GroupBox				radioSlaveGroup;
		protected Widget				radioGroupLeft;
		protected Widget				radioGroupRight;
		protected RadioButton			radioNever;
		protected RadioButton			radioDefault;
		protected RadioButton			radioSpecific;
		protected CheckButton			checkGuides;
		protected TextFieldCombo		specificSlavePage;

		protected Button				buttonPageStack;

		protected bool					isExtended = false;
		protected bool					ignoreChanged = false;
	}
}
