using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Pages contient tous les panneaux des pages.
	/// </summary>
	public class Pages : Abstract
	{
		public Pages(Document document) : base(document)
		{
			this.helpText = new StaticText(this);
			this.helpText.Text = Res.Strings.Container.Help.Pages;
			this.helpText.Dock = DockStyle.Top;
			this.helpText.Margins = new Margins(0, 0, -2, 7);

			this.toolBar = new HToolBar(this);
			this.toolBar.Dock = DockStyle.Top;
			this.toolBar.Margins = new Margins(0, 0, 0, -1);
			this.toolBar.TabIndex = 1;
			this.toolBar.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			int index = 0;

			this.buttonNew = new IconButton("PageNew", Misc.Icon("PageNew"));
			this.toolBar.Items.Add(this.buttonNew);
			this.buttonNew.TabIndex = index++;
			this.buttonNew.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonNew, Res.Strings.Action.PageNewLong);
			this.Synchro(this.buttonNew);

			this.buttonDuplicate = new IconButton("PageDuplicate", Misc.Icon("DuplicateItem"));
			this.toolBar.Items.Add(this.buttonDuplicate);
			this.buttonDuplicate.TabIndex = index++;
			this.buttonDuplicate.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonDuplicate, Res.Strings.Action.PageDuplicate);
			this.Synchro(this.buttonDuplicate);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonUp = new IconButton("PageUp", Misc.Icon("Up"));
			this.toolBar.Items.Add(this.buttonUp);
			this.buttonUp.TabIndex = index++;
			this.buttonUp.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonUp, Res.Strings.Action.PageUp);
			this.Synchro(this.buttonUp);

			this.buttonDown = new IconButton("PageDown", Misc.Icon("Down"));
			this.toolBar.Items.Add(this.buttonDown);
			this.buttonDown.TabIndex = index++;
			this.buttonDown.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonDown, Res.Strings.Action.PageDown);
			this.Synchro(this.buttonDown);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDelete = new IconButton("PageDelete", Misc.Icon("DeleteItem"));
			this.toolBar.Items.Add(this.buttonDelete);
			this.buttonDelete.TabIndex = index++;
			this.buttonDelete.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonDelete, Res.Strings.Action.PageDelete);
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
			this.table.DefHeight = 16;
			this.table.TabIndex = 2;
			this.table.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	--- Début panelMisc
			this.buttonPageStack = new Button(this);
			this.buttonPageStack.Dock = DockStyle.Bottom;
			this.buttonPageStack.Margins = new Margins(0, 0, 0, 0);
			this.buttonPageStack.CommandObject = Command.Get("PageStack");
			this.buttonPageStack.Text = Res.Strings.Container.Pages.Button.PageStack;
			this.buttonPageStack.TabIndex = 100;
			this.buttonPageStack.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			
			this.panelMisc = new Widget(this);
			this.panelMisc.Dock = DockStyle.Bottom;
			this.panelMisc.Margins = new Margins(0, 0, 5, 0);
			this.panelMisc.PreferredHeight = (this.document.Type == DocumentType.Pictogram) ? 186+24+24 : 186;
			this.panelMisc.TabIndex = 99;
			this.panelMisc.TabNavigationMode = TabNavigationMode.ForwardTabPassive;


			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.languageGroup = new Widget(this.panelMisc);
				this.languageGroup.Dock = DockStyle.Bottom;
				this.languageGroup.Margins = new Margins(0, 0, 0, 4);
				this.languageGroup.PreferredHeight = 20;
				this.languageGroup.TabIndex = 4;
				this.languageGroup.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

				StaticText labelLanguage = new StaticText(this.languageGroup);
				labelLanguage.Text = Res.Strings.Container.Pages.Language.Label;
				labelLanguage.ContentAlignment = ContentAlignment.MiddleRight;
				labelLanguage.PreferredWidth = 94;
				labelLanguage.Dock = DockStyle.Left;
				labelLanguage.Margins = new Margins(0, 4, 0, 0);

				this.languageField = new TextFieldCombo(this.languageGroup);
				this.languageField.PreferredWidth = 120;
				this.languageField.Items.Add("fr");
				this.languageField.Items.Add("en");
				this.languageField.Items.Add("de");
				this.languageField.Items.Add("it");
				this.languageField.Dock = DockStyle.Left;
				this.languageField.Margins = new Margins(0, 0, 0, 0);
				this.languageField.TextChanged += this.HandleLanguageChanged;

			
				this.styleGroup = new Widget(this.panelMisc);
				this.styleGroup.Dock = DockStyle.Bottom;
				this.styleGroup.Margins = new Margins(0, 0, 0, 4);
				this.styleGroup.PreferredHeight = 20;
				this.styleGroup.TabIndex = 4;
				this.styleGroup.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

				StaticText labelStyle = new StaticText(this.styleGroup);
				labelStyle.Text = Res.Strings.Container.Pages.Style.Label;
				labelStyle.ContentAlignment = ContentAlignment.MiddleRight;
				labelStyle.PreferredWidth = 94;
				labelStyle.Dock = DockStyle.Left;
				labelStyle.Margins = new Margins(0, 4, 0, 0);

				this.styleField = new TextFieldCombo(this.styleGroup);
				this.styleField.PreferredWidth = 120;
				this.styleField.Items.Add("");
				this.styleField.Items.Add("Normal");
				this.styleField.Items.Add("Cursor");
				this.styleField.Items.Add("Active");
				this.styleField.Dock = DockStyle.Left;
				this.styleField.Margins = new Margins(0, 0, 0, 0);
				this.styleField.TextChanged += this.HandleStyleChanged;
			}

			this.pageSizeGroup = new Widget(this.panelMisc);
			this.pageSizeGroup.Dock = DockStyle.Bottom;
			this.pageSizeGroup.Margins = new Margins(0, 0, 0, 4);
			this.pageSizeGroup.PreferredHeight = 22;
			this.pageSizeGroup.TabIndex = 3;
			this.pageSizeGroup.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			StaticText labelSize = new StaticText(this.pageSizeGroup);
			labelSize.Text = Res.Strings.Container.Pages.Size.Label;
			labelSize.ContentAlignment = ContentAlignment.MiddleRight;
			labelSize.PreferredWidth = 94;
			labelSize.Dock = DockStyle.Left;
			labelSize.Margins = new Margins(0, 4, 0, 0);

			this.pageSizeWidth = new TextFieldReal(this.pageSizeGroup);
			this.pageSizeWidth.PreferredWidth = 54;
			this.pageSizeWidth.Dock = DockStyle.Left;
			this.pageSizeWidth.Margins = new Margins(0, 0, 0, 0);
			this.pageSizeWidth.FactorMinRange = 0.01M;
			this.pageSizeWidth.FactorMaxRange = 1.0M;
			this.pageSizeWidth.FactorStep     = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.pageSizeWidth);
			this.pageSizeWidth.TabIndex = 1;
			this.pageSizeWidth.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.pageSizeWidth.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.pageSizeWidth.AutoSelectOnFocus = true;
			this.pageSizeWidth.SwallowEscapeOnRejectEdition = true;
			this.pageSizeWidth.SwallowReturnOnAcceptEdition = true;
			this.pageSizeWidth.EditionAccepted += this.HandlePageWidthEditionAccepted;
			ToolTip.Default.SetToolTip(this.pageSizeWidth, Res.Strings.Container.Pages.Size.Width);

			this.pageSizeSwap = new IconButton(this.pageSizeGroup);
			this.pageSizeSwap.PreferredWidth = 12;
			this.pageSizeSwap.AutoFocus = false;
			this.pageSizeSwap.IconUri = Misc.Icon("SwapDataV");
			this.pageSizeSwap.Dock = DockStyle.Left;
			this.pageSizeSwap.Margins = new Margins(0, 0, 0, 0);
			this.pageSizeSwap.TabIndex = 2;
			this.pageSizeSwap.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.pageSizeSwap.Clicked += this.HandlePageSwapClicked;
			ToolTip.Default.SetToolTip(this.pageSizeSwap, Res.Strings.Container.Pages.Size.Swap);

			this.pageSizeHeight = new TextFieldReal(this.pageSizeGroup);
			this.pageSizeHeight.PreferredWidth = 54;
			this.pageSizeHeight.Dock = DockStyle.Left;
			this.pageSizeHeight.Margins = new Margins(0, 0, 0, 0);
			this.pageSizeHeight.FactorMinRange = 0.01M;
			this.pageSizeHeight.FactorMaxRange = 1.0M;
			this.pageSizeHeight.FactorStep     = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.pageSizeHeight);
			this.pageSizeHeight.TabIndex = 3;
			this.pageSizeHeight.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.pageSizeHeight.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.pageSizeHeight.AutoSelectOnFocus = true;
			this.pageSizeHeight.SwallowEscapeOnRejectEdition = true;
			this.pageSizeHeight.SwallowReturnOnAcceptEdition = true;
			this.pageSizeHeight.EditionAccepted += this.HandlePageHeightEditionAccepted;
			ToolTip.Default.SetToolTip(this.pageSizeHeight, Res.Strings.Container.Pages.Size.Height);

			this.pageSizeClear = new IconButton(this.pageSizeGroup);
			this.pageSizeClear.AutoFocus = false;
			this.pageSizeClear.IconUri = Misc.Icon("Nothing");
			this.pageSizeClear.Dock = DockStyle.Left;
			this.pageSizeClear.Margins = new Margins(0, 0, 0, 0);
			this.pageSizeClear.TabIndex = 4;
			this.pageSizeClear.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.pageSizeClear.Clicked += this.HandlePageClearClicked;
			ToolTip.Default.SetToolTip(this.pageSizeClear, Res.Strings.Container.Pages.Size.Clear);

			
			this.radioMasterGroup = new GroupBox(this.panelMisc);
			this.radioMasterGroup.Dock = DockStyle.Bottom;
			this.radioMasterGroup.Margins = new Margins(0, 0, 0, 4);
			this.radioMasterGroup.PreferredHeight = 138;
			this.radioMasterGroup.Text = Res.Strings.Container.Pages.Button.MasterGroup;
			this.radioMasterGroup.TabIndex = 2;
			this.radioMasterGroup.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.radioAll = new RadioButton(this.radioMasterGroup);
			this.radioAll.Dock = DockStyle.Top;
			this.radioAll.Margins = new Margins(10, 10, 5, 2);
			this.radioAll.Text = Res.Strings.Container.Pages.Button.MasterAll;
			this.radioAll.ActiveStateChanged += this.HandleRadioChanged;
			this.radioAll.Index = 1;
			this.radioAll.TabIndex = 1;
			this.radioAll.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.radioOdd = new RadioButton(this.radioMasterGroup);
			this.radioOdd.Dock = DockStyle.Top;
			this.radioOdd.Margins = new Margins(10, 10, 0, 2);
			this.radioOdd.Text = Res.Strings.Container.Pages.Button.MasterOdd;
			this.radioOdd.ActiveStateChanged += this.HandleRadioChanged;
			this.radioOdd.Index = 2;
			this.radioOdd.TabIndex = 2;
			this.radioOdd.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.radioEven = new RadioButton(this.radioMasterGroup);
			this.radioEven.Dock = DockStyle.Top;
			this.radioEven.Margins = new Margins(10, 10, 0, 2);
			this.radioEven.Text = Res.Strings.Container.Pages.Button.MasterEven;
			this.radioEven.ActiveStateChanged += this.HandleRadioChanged;
			this.radioEven.Index = 3;
			this.radioEven.TabIndex = 3;
			this.radioEven.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.radioNone = new RadioButton(this.radioMasterGroup);
			this.radioNone.Dock = DockStyle.Top;
			this.radioNone.Margins = new Margins(10, 10, 0, 2);
			this.radioNone.Text = Res.Strings.Container.Pages.Button.MasterNone;
			this.radioNone.ActiveStateChanged += this.HandleRadioChanged;
			this.radioNone.Index = 4;
			this.radioNone.TabIndex = 4;
			this.radioNone.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.checkAutoStop = new CheckButton(this.radioMasterGroup);
			this.checkAutoStop.Dock = DockStyle.Top;
			this.checkAutoStop.Margins = new Margins(10, 10, 4, 0);
			this.checkAutoStop.Text = Res.Strings.Container.Pages.Button.MasterStop;
			this.checkAutoStop.Clicked += this.HandleCheckClicked;
			this.checkAutoStop.TabIndex = 5;
			this.checkAutoStop.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.specificGroup = new Widget(this.radioMasterGroup);
			this.specificGroup.Dock = DockStyle.Top;
			this.specificGroup.Margins = new Margins(10, 10, 2, 0);
			this.specificGroup.PreferredWidth = 170;
			this.specificGroup.TabIndex = 6;
			this.specificGroup.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.checkSpecific = new CheckButton(this.specificGroup);
			this.checkSpecific.PreferredWidth = 160;
			this.checkSpecific.Dock = DockStyle.Left;
			this.checkSpecific.Margins = new Margins(0, 0, 0, 0);
			this.checkSpecific.Text = Res.Strings.Container.Pages.Button.MasterSpecific;
			this.checkSpecific.Clicked += this.HandleCheckClicked;
			this.checkSpecific.TabIndex = 1;
			this.checkSpecific.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.specificMasterPage = new TextFieldCombo(this.specificGroup);
			this.specificMasterPage.PreferredWidth = 50;
			this.specificMasterPage.IsReadOnly = true;
			this.specificMasterPage.Dock = DockStyle.Left;
			this.specificMasterPage.Margins = new Margins(0, 0, 0, 0);
			this.specificMasterPage.ComboOpening += new EventHandler<CancelEventArgs> (this.HandleComboOpening);
			this.specificMasterPage.ComboClosed += this.HandleComboClosed;
			this.specificMasterPage.TabIndex = 2;
			this.specificMasterPage.TabNavigationMode = TabNavigationMode.ActivateOnTab;


			this.radioSlaveGroup = new GroupBox(this.panelMisc);
			this.radioSlaveGroup.Dock = DockStyle.Bottom;
			this.radioSlaveGroup.Margins = new Margins(0, 0, 0, 4);
			this.radioSlaveGroup.PreferredHeight = 138;
			this.radioSlaveGroup.Text = Res.Strings.Container.Pages.Button.SlaveGroup;
			this.radioSlaveGroup.TabIndex = 3;
			this.radioSlaveGroup.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.radioGroupLeft = new Widget(this.radioSlaveGroup);
			this.radioGroupLeft.Dock = DockStyle.Left;
			this.radioGroupLeft.Margins = new Margins(0, 0, 5, 0);
			this.radioGroupLeft.PreferredWidth = 170;
			this.radioGroupLeft.TabIndex = 1;
			this.radioGroupLeft.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.radioNever = new RadioButton(this.radioGroupLeft);
			this.radioNever.Dock = DockStyle.Top;
			this.radioNever.Margins = new Margins(10, 0, 0, 2);
			this.radioNever.Text = Res.Strings.Container.Pages.Button.SlaveNever;
			this.radioNever.ActiveStateChanged += this.HandleRadioChanged;
			this.radioNever.Index = 1;
			this.radioNever.TabIndex = 1;
			this.radioNever.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.radioDefault = new RadioButton(this.radioGroupLeft);
			this.radioDefault.Dock = DockStyle.Top;
			this.radioDefault.Margins = new Margins(10, 0, 0, 2);
			this.radioDefault.Text = Res.Strings.Container.Pages.Button.SlaveDefault;
			this.radioDefault.ActiveStateChanged += this.HandleRadioChanged;
			this.radioDefault.Index = 2;
			this.radioDefault.TabIndex = 2;
			this.radioDefault.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.radioSpecific = new RadioButton(this.radioGroupLeft);
			this.radioSpecific.Dock = DockStyle.Top;
			this.radioSpecific.Margins = new Margins(10, 0, 0, 2);
			this.radioSpecific.Text = Res.Strings.Container.Pages.Button.SlaveSpecific;
			this.radioSpecific.ActiveStateChanged += this.HandleRadioChanged;
			this.radioSpecific.Index = 3;
			this.radioSpecific.TabIndex = 3;
			this.radioSpecific.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.checkGuides = new CheckButton(this.radioGroupLeft);
			this.checkGuides.Dock = DockStyle.Top;
			this.checkGuides.Margins = new Margins(10, 0, 4, 0);
			this.checkGuides.Text = Res.Strings.Container.Pages.Button.SlaveGuides;
			this.checkGuides.Clicked += this.HandleCheckClicked;
			this.checkGuides.TabIndex = 4;
			this.checkGuides.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.radioGroupRight = new Widget(this.radioSlaveGroup);
			this.radioGroupRight.Dock = DockStyle.Left;
			this.radioGroupRight.Margins = new Margins(0, 0, 5, 0);
			this.radioGroupRight.PreferredWidth = 50;
			this.radioGroupRight.TabIndex = 2;
			this.radioGroupRight.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.specificSlavePage = new TextFieldCombo(this.radioGroupRight);
			this.specificSlavePage.PreferredWidth = 50;
			this.specificSlavePage.IsReadOnly = true;
			this.specificSlavePage.Dock = DockStyle.Top;
			this.specificSlavePage.Margins = new Margins(0, 0, 30, 0);
			this.specificSlavePage.ComboOpening += new EventHandler<CancelEventArgs> (this.HandleComboOpening);
			this.specificSlavePage.ComboClosed += this.HandleComboClosed;
			this.specificSlavePage.TabIndex = 1;
			this.specificSlavePage.TabNavigationMode = TabNavigationMode.ForwardTabPassive;


			this.radioGroup = new Widget(this.panelMisc);
			this.radioGroup.Dock = DockStyle.Bottom;
			this.radioGroup.Margins = new Margins(0, 0, 0, 4);
			this.radioGroup.PreferredHeight = 20;
			this.radioGroup.TabIndex = 1;
			this.radioGroup.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.radioSlave = new RadioButton(this.radioGroup);
			this.radioSlave.PreferredWidth = 100;
			this.radioSlave.Dock = DockStyle.Left;
			this.radioSlave.Margins = new Margins(10, 10, 0, 0);
			this.radioSlave.Text = Res.Strings.Container.Pages.Button.Slave;
			this.radioSlave.ActiveStateChanged += this.HandleRadioChanged;
			this.radioSlave.Index = 1;
			this.radioSlave.TabIndex = 1;
			this.radioSlave.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.radioMaster = new RadioButton(this.radioGroup);
			this.radioMaster.PreferredWidth = 100;
			this.radioMaster.Dock = DockStyle.Left;
			this.radioMaster.Margins = new Margins(10, 10, 0, 0);
			this.radioMaster.Text = Res.Strings.Container.Pages.Button.Master;
			this.radioMaster.ActiveStateChanged += this.HandleRadioChanged;
			this.radioMaster.Index = 2;
			this.radioMaster.TabIndex = 2;
			this.radioMaster.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			//	--- Fin panelMisc
			
			this.extendedButton = new GlyphButton(this);
			this.extendedButton.Dock = DockStyle.Bottom;
			this.extendedButton.Margins = new Margins(0, 0, 5, 0);
			this.extendedButton.ButtonStyle = ButtonStyle.Icon;
			this.extendedButton.Clicked += this.ExtendedButtonClicked;
			this.extendedButton.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.extendedButton.TabIndex = 98;
			this.extendedButton.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.extendedButton, Res.Strings.Dialog.Button.More);
			

			this.toolBarName = new HToolBar(this);
			this.toolBarName.Dock = DockStyle.Bottom;
			this.toolBarName.Margins = new Margins(0, 0, 0, 0);
			this.toolBarName.TabIndex = 97;
			this.toolBarName.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			StaticText st = new StaticText();
			st.PreferredWidth = 80;
			st.Text = Res.Strings.Panel.PageName.Label.Name;
			this.toolBarName.Items.Add(st);

			this.name = new TextField();
			this.name.PreferredWidth = 140;
			this.name.Margins = new Margins(0, 0, 1, 1);
			this.name.TextChanged += this.HandleNameTextChanged;
			this.name.TabIndex = 1;
			this.name.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.toolBarName.Items.Add(this.name);
			ToolTip.Default.SetToolTip(this.name, Res.Strings.Panel.PageName.Tooltip.Name);

			
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
			this.UpdatePanel();
		}

		protected override void DoUpdateObject(Objects.Abstract obj)
		{
			//	Effectue la mise à jour d'un objet.
			Objects.Page page = obj as Objects.Page;
			UndoableList pages = this.document.DocumentObjects;
			int rank = pages.IndexOf(obj);
			this.TableUpdateRow(rank, page);
		}

		protected void UpdateTable()
		{
			//	Met à jour le contenu de la table.
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

			UndoableList doc = this.document.DocumentObjects;
			for ( int i=0 ; i<rows ; i++ )
			{
				Objects.Page page = doc[i] as Objects.Page;
				this.TableFillRow(i);
				this.TableUpdateRow(i, page);
			}
		}

		protected void TableFillRow(int row)
		{
			//	Peuple une ligne de la table, si nécessaire.
			for ( int column=0 ; column<this.table.Columns ; column++ )
			{
				if ( this.table[column, row].IsEmpty )
				{
					StaticText st = new StaticText();
					st.ContentAlignment = (column==0) ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
					st.Dock = DockStyle.Fill;
					st.Margins = new Margins(4, 4, 0, 0);
					this.table[column, row].Insert(st);
				}
			}
		}

		protected void TableUpdateRow(int row, Objects.Page page)
		{
			//	Met à jour le contenu d'une ligne de la table.
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			StaticText st;

			st = this.table[0, row].Children[0] as StaticText;
			st.Text = page.ShortName;

			st = this.table[1, row].Children[0] as StaticText;
			st.Text = page.LongName;

			this.table.SelectRow(row, row==context.CurrentPage);
		}

		protected void UpdatePanel()
		{
			//	Met à jour le panneau pour éditer la page sélectionnée.
			this.UpdatePageName();

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Page page = context.RootObject(1) as Objects.Page;

			Size size = page.PageSize;
			string language = page.Language;
			string style = page.PageStyle;

			bool widthDefined = true;
			if ( size.Width == 0 )
			{
				size.Width = this.document.DocumentSize.Width;
				widthDefined = false;
			}

			bool heightDefined = true;
			if ( size.Height == 0 )
			{
				size.Height = this.document.DocumentSize.Height;
				heightDefined = false;
			}

			this.ignoreChanged = true;

			if ( this.pageSizeWidth != null )
			{
				this.pageSizeWidth.InternalValue = (decimal) size.Width;
				this.pageSizeWidth.TextDisplayMode = widthDefined ? TextFieldDisplayMode.OverriddenValue : TextFieldDisplayMode.InheritedValue;
			}

			if ( this.pageSizeHeight != null )
			{
				this.pageSizeHeight.InternalValue = (decimal) size.Height;
				this.pageSizeHeight.TextDisplayMode = heightDefined ? TextFieldDisplayMode.OverriddenValue : TextFieldDisplayMode.InheritedValue;
			}

			if ( this.languageField != null )
			{
				this.languageField.Text = language;
			}

			if ( this.styleField != null )
			{
				this.styleField.Text = style;
			}

			this.radioSlave.ActiveState  = (page.MasterType == Objects.MasterType.Slave) ? ActiveState.Yes : ActiveState.No;
			this.radioMaster.ActiveState = (page.MasterType != Objects.MasterType.Slave) ? ActiveState.Yes : ActiveState.No;

			this.radioSlaveGroup.Visibility = (page.MasterType == Objects.MasterType.Slave);
			this.radioMasterGroup.Visibility = (page.MasterType != Objects.MasterType.Slave);

			this.radioAll.ActiveState =  (page.MasterType == Objects.MasterType.All ) ? ActiveState.Yes : ActiveState.No;
			this.radioEven.ActiveState = (page.MasterType == Objects.MasterType.Even) ? ActiveState.Yes : ActiveState.No;
			this.radioOdd.ActiveState =  (page.MasterType == Objects.MasterType.Odd ) ? ActiveState.Yes : ActiveState.No;
			this.radioNone.ActiveState = (page.MasterType == Objects.MasterType.None) ? ActiveState.Yes : ActiveState.No;

			this.radioNever.ActiveState    = (page.MasterUse == Objects.MasterUse.Never   ) ? ActiveState.Yes : ActiveState.No;
			this.radioDefault.ActiveState  = (page.MasterUse == Objects.MasterUse.Default ) ? ActiveState.Yes : ActiveState.No;
			this.radioSpecific.ActiveState = (page.MasterUse == Objects.MasterUse.Specific) ? ActiveState.Yes : ActiveState.No;

			this.checkGuides.ActiveState = page.MasterGuides ? ActiveState.Yes : ActiveState.No;

			this.specificSlavePage.Enable = (page.MasterUse == Objects.MasterUse.Specific);
			if ( page.MasterPageToUse == null ||
				 page.MasterPageToUse.MasterType == Objects.MasterType.Slave )
			{
				this.specificSlavePage.Text = "";
			}
			else
			{
				this.specificSlavePage.Text = page.MasterPageToUse.ShortName;
			}

			this.checkAutoStop.ActiveState = page.MasterAutoStop ? ActiveState.Yes : ActiveState.No;
			this.checkSpecific.ActiveState = page.MasterSpecific ? ActiveState.Yes : ActiveState.No;

			this.specificMasterPage.Enable = (page.MasterSpecific);
			if ( page.MasterPageToUse == null || !page.MasterSpecific )
			{
				this.specificMasterPage.Text = "";
			}
			else
			{
				this.specificMasterPage.Text = page.MasterPageToUse.ShortName;
			}

			this.ignoreChanged = false;
		}

		protected void UpdatePageName()
		{
			//	Met à jour le panneau pour éditer le nom de la page sélectionnée.
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentPage;
			string text = this.document.Modifier.PageName(sel);

			this.ignoreChanged = true;
			this.name.Text = text;
			this.ignoreChanged = false;
		}


		private void HandleTableSelectionChanged(object sender)
		{
			//	Liste cliquée.
			if ( this.table.SelectedRow == -1 )  return;
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			context.CurrentPage = this.table.SelectedRow;
		}

		private void HandleTableDoubleClicked(object sender, MessageEventArgs e)
		{
			//	Liste double-cliquée.
			this.name.SelectAll();
			this.name.Focus();
		}

		private void HandleNameTextChanged(object sender)
		{
			//	Le nom de la page a changé.
			if ( this.ignoreChanged )  return;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentPage;

			if ( this.document.Modifier.PageName(sel) != this.name.Text )
			{
				this.document.Modifier.PageName(sel, this.name.Text);
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
			this.buttonPageStack.Visibility = (this.isExtended);
		}

		private void HandleLanguageChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Container.Pages.Language.Label, "SpecialPageLanguage");

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Page page = context.RootObject(1) as Objects.Page;

			page.Language = this.languageField.Text;

			this.document.Modifier.ActiveViewer.DrawingContext.ZoomPageAndCenter();
			this.document.Notifier.NotifyPagesChanged();
			this.document.Modifier.OpletQueueValidateAction();
		}

		private void HandleStyleChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Container.Pages.Style.Label, "SpecialPageStyle");

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Page page = context.RootObject(1) as Objects.Page;

			page.PageStyle = this.styleField.Text;

			this.document.Modifier.ActiveViewer.DrawingContext.ZoomPageAndCenter();
			this.document.Notifier.NotifyPagesChanged();
			this.document.Modifier.OpletQueueValidateAction();
		}

		private void HandlePageWidthEditionAccepted(object sender)
		{
			if ( this.ignoreChanged )  return;

			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Container.Pages.Size.Label, "SpecialPageSize");

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Page page = context.RootObject(1) as Objects.Page;

			Size size = page.PageSize;
			size.Width = (double) this.pageSizeWidth.InternalValue;
			page.PageSize = size;

			this.document.Modifier.ActiveViewer.DrawingContext.ZoomPageAndCenter();
			this.document.Notifier.NotifyPagesChanged();
			this.document.Modifier.OpletQueueValidateAction();
		}

		private void HandlePageHeightEditionAccepted(object sender)
		{
			if ( this.ignoreChanged )  return;

			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Container.Pages.Size.Label, "SpecialPageSize");

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Page page = context.RootObject(1) as Objects.Page;

			Size size = page.PageSize;
			size.Height = (double) this.pageSizeHeight.InternalValue;
			page.PageSize = size;

			this.document.Modifier.ActiveViewer.DrawingContext.ZoomPageAndCenter();
			this.document.Notifier.NotifyPagesChanged();
			this.document.Modifier.OpletQueueValidateAction();
		}

		private void HandlePageSwapClicked(object sender, MessageEventArgs e)
		{
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Container.Pages.Size.Swap);

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Page page = context.RootObject(1) as Objects.Page;

			Size size = new Size(page.PageSize.Height, page.PageSize.Width);
			if ( size.Width == 0 && size.Height == 0 )
			{
				size.Width  = this.document.DocumentSize.Height;
				size.Height = this.document.DocumentSize.Width;
			}
			page.PageSize = size;

			this.document.Modifier.ActiveViewer.DrawingContext.ZoomPageAndCenter();
			this.document.Notifier.NotifyPagesChanged();
			this.document.Modifier.OpletQueueValidateAction();
		}

		private void HandlePageClearClicked(object sender, MessageEventArgs e)
		{
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Container.Pages.Size.Clear);

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Page page = context.RootObject(1) as Objects.Page;

			page.PageSize = new Size(0,0);

			this.document.Modifier.ActiveViewer.DrawingContext.ZoomPageAndCenter();
			this.document.Notifier.NotifyPagesChanged();
			this.document.Modifier.OpletQueueValidateAction();
		}

		private void HandleRadioChanged(object sender)
		{
			//	Un bouton radio a été cliqué.
			if ( this.ignoreChanged )  return;

			RadioButton radio = sender as RadioButton;
			if ( radio == null )  return;
			if ( radio.ActiveState != ActiveState.Yes )  return;

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

		private void HandleCheckClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton à cocher a été cliqué.
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

		private void HandleComboOpening(object sender, CancelEventArgs e)
		{
			//	Combo ouvert.
			TextFieldCombo field = sender as TextFieldCombo;
			field.Items.Clear();

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			UndoableList doc = this.document.DocumentObjects;
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

		private void HandleComboClosed(object sender)
		{
			//	Combo fermé.
			if ( this.ignoreChanged )  return;
			TextFieldCombo field = sender as TextFieldCombo;
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			UndoableList doc = this.document.DocumentObjects;
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


		protected StaticText				helpText;

		protected HToolBar					toolBar;
		protected IconButton				buttonNew;
		protected IconButton				buttonDuplicate;
		protected IconButton				buttonUp;
		protected IconButton				buttonDown;
		protected IconButton				buttonDelete;
		protected CellTable					table;
		protected HToolBar					toolBarName;
		protected TextField					name;
		protected GlyphButton				extendedButton;
		protected Widget					panelMisc;

		protected Widget					pageSizeGroup;
		protected TextFieldReal				pageSizeWidth;
		protected TextFieldReal				pageSizeHeight;
		protected IconButton				pageSizeSwap;
		protected IconButton				pageSizeClear;

		protected Widget					languageGroup;
		protected TextFieldCombo			languageField;

		protected Widget					styleGroup;
		protected TextFieldCombo			styleField;

		protected Widget					radioGroup;
		protected RadioButton				radioSlave;
		protected RadioButton				radioMaster;

		protected GroupBox					radioMasterGroup;
		protected RadioButton				radioAll;
		protected RadioButton				radioEven;
		protected RadioButton				radioOdd;
		protected RadioButton				radioNone;
		protected CheckButton				checkAutoStop;
		protected Widget					specificGroup;
		protected CheckButton				checkSpecific;
		protected TextFieldCombo			specificMasterPage;
	
		protected GroupBox					radioSlaveGroup;
		protected Widget					radioGroupLeft;
		protected Widget					radioGroupRight;
		protected RadioButton				radioNever;
		protected RadioButton				radioDefault;
		protected RadioButton				radioSpecific;
		protected CheckButton				checkGuides;
		protected TextFieldCombo			specificSlavePage;

		protected Button					buttonPageStack;

		protected bool						isExtended = false;
		protected bool						ignoreChanged = false;
	}
}
