using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.FormEngine;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Forms : Abstract
	{
		public Forms(Module module, PanelsContext context, ResourceAccess access, DesignerApplication designerApplication)
			: base (module, context, access, designerApplication)
		{
			bool isWindow = (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Window);

			this.engine = new FormEngine.Engine(this.module.FormResourceProvider);
			this.undoEngine = new Undo.Engine();

			this.scrollable.Visibility = false;

			FrameBox surface = new FrameBox(this.lastGroup);
			surface.DrawFullFrame = true;
			surface.Margins = new Margins(0, 0, 5, 0);
			surface.Dock = DockStyle.Fill;

			//	Crée le groupe central.
			this.middle = new FrameBox(surface);
			this.middle.Padding = new Margins(5, 5, 5, 5);
			this.middle.Dock = DockStyle.Fill;

			FrameBox drawing;
			if (isWindow)
			{
				drawing = new FrameBox(this.designerApplication.ViewersWindow.Root);
				drawing.Dock = DockStyle.Fill;
			}
			else
			{
				drawing = new FrameBox(this.middle);  // conteneur pour scrollable
				drawing.Dock = DockStyle.Fill;
			}

			this.drawingScrollable = new Scrollable(drawing);
			this.drawingScrollable.MinWidth = 100;
			this.drawingScrollable.MinHeight = 100;
			this.drawingScrollable.Margins = new Margins(1, 1, 1, 1);
			this.drawingScrollable.Dock = DockStyle.Fill;
			this.drawingScrollable.HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.drawingScrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.drawingScrollable.Viewport.IsAutoFitting = true;
			this.drawingScrollable.Viewport.TabNavigationMode = TabNavigationMode.None;
			this.drawingScrollable.PaintViewportFrame = true;
			this.drawingScrollable.ViewportPadding = new Margins (-1);

			FrameBox container = new FrameBox(this.drawingScrollable.Viewport);
			container.MinWidth = 100;
			container.Dock = DockStyle.Fill;

			//	Sous-conteneur qui a des marges, pour permettre de voir les cotes (Dimension*)
			//	du FormEditor qui s'affiche par-dessus.
			this.panelContainerParent = new FrameBox(container);
			this.panelContainerParent.Dock = DockStyle.Fill;

			//	Le UI.Panel est dans le sous-contenur qui a des marges.
			this.panelContainer = new UI.Panel();

			//	Le FormEditor est par-dessus le UI.Panel. Il occupe toute la surface (il déborde
			//	donc des marges) et tient compte lui-même du décalage. C'est le seul moyen pour
			//	pouvoir dessiner dans les marges ET y détecter les événements souris.
			this.formEditor = new FormEditor.Editor(container);
			this.formEditor.Initialize(this, this.module, this.context, this.panelContainer);
			this.formEditor.MinWidth = 100;
			this.formEditor.MinHeight = 100;
			this.formEditor.Anchor = AnchorStyles.All;
			this.formEditor.ChildrenAdded += this.HandleFormEditorChildrenAdded;
			this.formEditor.ChildrenSelected += this.HandleFormEditorChildrenSelected;
			this.formEditor.UpdateCommands += this.HandleFormEditorUpdateCommands;

			this.InitializePanel();

			//	Crée le groupe droite.
			this.right = new FrameBox(surface);
			this.right.MinWidth = 150;
			this.right.PreferredWidth = Forms.rightPanelWidth;
			this.right.MaxWidth = 400;
			this.right.Dock = DockStyle.Right;

			//	Crée le tabbook primaire pour les onglets.
			FrameBox top = new FrameBox(this.right);
			top.Dock = DockStyle.Fill;
			top.Margins = new Margins(5, 5, 5, 5);
			top.Padding = new Margins(1, 1, 1, 1);

			//	Crée le tabbook secondaire pour les onglets.
			this.tabBookSecondary = new TabBook(this.right);
			this.tabBookSecondary.Arrows = TabBookArrows.Stretch;
			this.tabBookSecondary.Dock = DockStyle.Bottom;
			this.tabBookSecondary.Margins = new Margins(5, 5, 5, 5);
			this.tabBookSecondary.Padding = new Margins(1, 1, 1, 1);
			this.tabBookSecondary.PreferredHeight = Forms.bottomPanelHeight;

			//	Crée l'onglet 'champs'.
			FrameBox topToolBar = new FrameBox(top);
			topToolBar.Dock = DockStyle.Top;
			topToolBar.Margins = new Margins(0, 0, 0, 5);

			this.fieldsToolbar = new HToolBar(topToolBar);
			this.fieldsToolbar.Dock = DockStyle.Fill;

			this.fieldsButtonRemove = new IconButton();
			this.fieldsButtonRemove.AutoFocus = false;
			this.fieldsButtonRemove.CaptionId = Res.Captions.Editor.Forms.Remove.Id;
			this.fieldsButtonRemove.Clicked += this.HandleButtonClicked;
			this.fieldsToolbar.Items.Add(this.fieldsButtonRemove);

			this.fieldsButtonReset = new IconButton();
			this.fieldsButtonReset.AutoFocus = false;
			this.fieldsButtonReset.CaptionId = Res.Captions.Editor.Forms.Reset.Id;
			this.fieldsButtonReset.Clicked += this.HandleButtonClicked;
			this.fieldsToolbar.Items.Add(this.fieldsButtonReset);

			this.fieldsToolbar.Items.Add(new IconSeparator());

			this.fieldsButtonBox = new IconButton();
			this.fieldsButtonBox.AutoFocus = false;
			this.fieldsButtonBox.CaptionId = Res.Captions.Editor.Forms.Box.Id;
			this.fieldsButtonBox.Clicked += this.HandleButtonClicked;
			this.fieldsToolbar.Items.Add(this.fieldsButtonBox);

			this.fieldsButtonForm = new IconButton();
			this.fieldsButtonForm.AutoFocus = false;
			this.fieldsButtonForm.CaptionId = Res.Captions.Editor.Forms.Form.Id;
			this.fieldsButtonForm.Clicked += this.HandleButtonClicked;
			this.fieldsToolbar.Items.Add(this.fieldsButtonForm);

			this.fieldsToolbar.Items.Add(new IconSeparator());

			this.fieldsButtonPrev = new IconButton();
			this.fieldsButtonPrev.AutoFocus = false;
			this.fieldsButtonPrev.CaptionId = Res.Captions.Editor.Forms.Prev.Id;
			this.fieldsButtonPrev.Clicked += this.HandleButtonClicked;
			this.fieldsToolbar.Items.Add(this.fieldsButtonPrev);

			this.fieldsButtonNext = new IconButton();
			this.fieldsButtonNext.AutoFocus = false;
			this.fieldsButtonNext.CaptionId = Res.Captions.Editor.Forms.Next.Id;
			this.fieldsButtonNext.Clicked += this.HandleButtonClicked;
			this.fieldsToolbar.Items.Add(this.fieldsButtonNext);

			this.fieldsToolbar.Items.Add(new IconSeparator());

			this.fieldsButtonGoto = new IconButton();
			this.fieldsButtonGoto.AutoFocus = false;
			this.fieldsButtonGoto.CaptionId = Res.Captions.Editor.LocatorGoto.Id;
			this.fieldsButtonGoto.Clicked += this.HandleButtonClicked;
			this.fieldsToolbar.Items.Add(this.fieldsButtonGoto);

			this.fieldsButtonMenu = new GlyphButton(topToolBar);
			this.fieldsButtonMenu.GlyphShape = GlyphShape.Menu;
			this.fieldsButtonMenu.ButtonStyle = ButtonStyle.ToolItem;
			this.fieldsButtonMenu.AutoFocus = false;
			this.fieldsButtonMenu.Clicked += this.HandleButtonClicked;
			this.fieldsButtonMenu.Margins = new Margins(0, 0, 3, 3);
			this.fieldsButtonMenu.Dock = DockStyle.Right;

			this.fieldsTable = new MyWidgets.StringArray(top);
			this.fieldsTable.Columns = 3;
			this.fieldsTable.SetColumnAlignment(0, ContentAlignment.MiddleLeft);
			this.fieldsTable.SetColumnAlignment(1, ContentAlignment.MiddleCenter);
			this.fieldsTable.SetColumnAlignment(2, ContentAlignment.MiddleCenter);
			this.fieldsTable.SetColumnBreakMode(0, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
			this.fieldsTable.AllowMultipleSelection = true;
			this.fieldsTable.LineHeight = 16;
			this.fieldsTable.Dock = DockStyle.Fill;
			this.fieldsTable.CellCountChanged += this.HandleFieldTableCellCountChanged;
			this.fieldsTable.CellsContentChanged += this.HandleFieldTableCellsContentChanged;
			this.fieldsTable.SelectedRowChanged += this.HandleFieldTableSelectedRowChanged;
			this.UpdateFieldsTableColumns();

			//	Crée l'onglet 'propriétés'.
			if (!isWindow)
			{
				this.tabPageProperties = new TabPage();
				this.tabPageProperties.TabTitle = Res.Strings.Viewers.Forms.TabProperties;
				this.tabPageProperties.Padding = new Margins(4, 4, 4, 4);
				this.tabBookSecondary.Items.Add(this.tabPageProperties);
			}

			Widget parentProperties = isWindow ? this.middle : this.tabPageProperties;

			this.otherToolbar = new HToolBar(parentProperties);
			this.otherToolbar.Dock = DockStyle.Top;
			this.otherToolbar.Margins = new Margins(0, 0, 0, 5);

			this.otherButtonCommand = new IconButton();
			this.otherButtonCommand.AutoFocus = false;
			this.otherButtonCommand.CaptionId = Res.Captions.Editor.Forms.Command.Id;
			this.otherButtonCommand.Clicked += this.HandleButtonClicked;
			this.otherToolbar.Items.Add(this.otherButtonCommand);

			this.otherToolbar.Items.Add(new IconSeparator());

			this.otherButtonLine = new IconButton();
			this.otherButtonLine.AutoFocus = false;
			this.otherButtonLine.CaptionId = Res.Captions.Editor.Forms.Line.Id;
			this.otherButtonLine.Clicked += this.HandleButtonClicked;
			this.otherToolbar.Items.Add(this.otherButtonLine);

			this.otherButtonTitle = new IconButton();
			this.otherButtonTitle.AutoFocus = false;
			this.otherButtonTitle.CaptionId = Res.Captions.Editor.Forms.Title.Id;
			this.otherButtonTitle.Clicked += this.HandleButtonClicked;
			this.otherToolbar.Items.Add(this.otherButtonTitle);

			this.otherToolbar.Items.Add(new IconSeparator());

			this.otherButtonGlue = new IconButton();
			this.otherButtonGlue.AutoFocus = false;
			this.otherButtonGlue.CaptionId = Res.Captions.Editor.Forms.Glue.Id;
			this.otherButtonGlue.Clicked += this.HandleButtonClicked;
			this.otherToolbar.Items.Add(this.otherButtonGlue);

			Proxies.ObjectManagerForm objectManager = new Proxies.ObjectManagerForm(this.designerApplication, this.formEditor.ObjectModifier);
			Proxies.ProxyForm proxy = new Proxies.ProxyForm(this);
			this.proxyManager = new Proxies.ProxyManager(objectManager, proxy);

			this.propertiesScrollable = new Scrollable(parentProperties);
			this.propertiesScrollable.Dock = DockStyle.Fill;
			this.propertiesScrollable.HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.propertiesScrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.propertiesScrollable.Viewport.IsAutoFitting = true;
			this.propertiesScrollable.Viewport.Margins = new Margins(10, 10, 10, 10);
			this.propertiesScrollable.PaintViewportFrame = true;
			this.propertiesScrollable.ViewportPadding = new Margins (-1);

			//	Crée l'onglet 'source'.
			this.tabPageSource = new TabPage();
			this.tabPageSource.TabTitle = Res.Strings.Viewers.Forms.TabRelations;
			this.tabPageSource.Padding = new Margins(4, 4, 4, 4);
			this.tabBookSecondary.Items.Add(this.tabPageSource);

			this.relationsToolbar = new HToolBar(this.tabPageSource);
			this.relationsToolbar.Dock = DockStyle.Top;
			this.relationsToolbar.Margins = new Margins(0, 0, 0, 5);

			this.relationsButtonUse = new IconButton();
			this.relationsButtonUse.AutoFocus = false;
			this.relationsButtonUse.CaptionId = Res.Captions.Editor.Forms.Use.Id;
			this.relationsButtonUse.Clicked += this.HandleButtonClicked;
			this.relationsToolbar.Items.Add(this.relationsButtonUse);

			this.relationsToolbar.Items.Add(new IconSeparator());

			this.relationsButtonExpand = new IconButton();
			this.relationsButtonExpand.AutoFocus = false;
			this.relationsButtonExpand.CaptionId = Res.Captions.Editor.Forms.Expand.Id;
			this.relationsButtonExpand.Clicked += this.HandleButtonClicked;
			this.relationsToolbar.Items.Add(this.relationsButtonExpand);

			this.relationsButtonCompact = new IconButton();
			this.relationsButtonCompact.AutoFocus = false;
			this.relationsButtonCompact.CaptionId = Res.Captions.Editor.Forms.Compact.Id;
			this.relationsButtonCompact.Clicked += this.HandleButtonClicked;
			this.relationsToolbar.Items.Add(this.relationsButtonCompact);

			this.relationsToolbar.Items.Add(new IconSeparator());

			this.relationsButtonAuto = new IconButton();
			this.relationsButtonAuto.AutoFocus = false;
			this.relationsButtonAuto.CaptionId = Res.Captions.Editor.Forms.Auto.Id;
			this.relationsButtonAuto.Clicked += this.HandleButtonClicked;
			this.relationsToolbar.Items.Add(this.relationsButtonAuto);

			this.relationsTable = new MyWidgets.StringArray(this.tabPageSource);
			this.relationsTable.Columns = 3;
			this.relationsTable.SetColumnsRelativeWidth(0, 0.10);
			this.relationsTable.SetColumnsRelativeWidth(1, 0.80);
			this.relationsTable.SetColumnsRelativeWidth(2, 0.10);
			this.relationsTable.SetColumnAlignment(0, ContentAlignment.MiddleCenter);
			this.relationsTable.SetColumnAlignment(1, ContentAlignment.MiddleLeft);
			this.relationsTable.SetColumnAlignment(2, ContentAlignment.MiddleCenter);
			this.relationsTable.SetColumnBreakMode(1, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
			this.relationsTable.AllowMultipleSelection = false;
			this.relationsTable.LineHeight = 16;
			this.relationsTable.Dock = DockStyle.Fill;
			this.relationsTable.CellCountChanged += this.HandleRelationsTableCellCountChanged;
			this.relationsTable.CellsContentChanged += this.HandleRelationsTableCellsContentChanged;
			this.relationsTable.SelectedRowChanged += this.HandleRelationsTableSelectedRowChanged;
			this.relationsTable.SelectedRowDoubleClicked += this.HandleRelationsTableSelectedRowDoubleClicked;

			//	Crée l'onglet 'divers'.
			this.tabPageMisc = new TabPage();
			this.tabPageMisc.TabTitle = Res.Strings.Viewers.Forms.TabMisc;
			this.tabPageMisc.Padding = new Margins(10, 10, 10, 10);
			this.tabBookSecondary.Items.Add(this.tabPageMisc);

			this.CreateMiscPage();

			//	Crée l'onglet 'cultures'.
			this.tabPageCultures = new TabPage();
			this.tabPageCultures.TabTitle = Res.Strings.Viewers.Forms.TabCultures;
			this.tabPageCultures.Padding = new Margins(10, 10, 10, 10);
			this.tabBookSecondary.Items.Add(this.tabPageCultures);

			this.CreateCultureButtons();

			this.tabBookSecondary.ActivePage = isWindow ? this.tabPageSource : this.tabPageProperties;

			this.splitter2 = new VSplitter(surface);
			this.splitter2.Dock = DockStyle.Right;
			this.splitter2.Margins = new Margins(0, 0, 1, 1);
			this.splitter2.SplitterDragged += this.HandleSplitterDragged;

			this.splitter3 = new HSplitter(this.right);
			this.splitter3.Dock = DockStyle.Bottom;
			this.splitter3.Margins = new Margins(0, 1, 0, 0);
			this.splitter3.SplitterDragged += this.HandleSplitterDragged;

			this.UpdateAll();
			this.UpdateViewer(Viewers.Changing.Show);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.fieldsButtonRemove.Clicked -= this.HandleButtonClicked;
				this.fieldsButtonReset.Clicked -= this.HandleButtonClicked;
				this.fieldsButtonBox.Clicked -= this.HandleButtonClicked;
				this.fieldsButtonForm.Clicked -= this.HandleButtonClicked;
				this.fieldsButtonPrev.Clicked -= this.HandleButtonClicked;
				this.fieldsButtonNext.Clicked -= this.HandleButtonClicked;
				this.fieldsButtonGoto.Clicked -= this.HandleButtonClicked;
				this.fieldsButtonMenu.Clicked -= this.HandleButtonClicked;
				this.relationsButtonUse.Clicked -= this.HandleButtonClicked;
				this.relationsButtonExpand.Clicked -= this.HandleButtonClicked;
				this.relationsButtonCompact.Clicked -= this.HandleButtonClicked;
				this.relationsButtonAuto.Clicked -= this.HandleButtonClicked;
				this.otherButtonCommand.Clicked -= this.HandleButtonClicked;
				this.otherButtonLine.Clicked -= this.HandleButtonClicked;
				this.otherButtonTitle.Clicked -= this.HandleButtonClicked;
				this.otherButtonGlue.Clicked -= this.HandleButtonClicked;

				this.fieldsTable.CellCountChanged -= this.HandleFieldTableCellCountChanged;
				this.fieldsTable.CellsContentChanged -= this.HandleFieldTableCellsContentChanged;
				this.fieldsTable.SelectedRowChanged -= this.HandleFieldTableSelectedRowChanged;

				this.relationsTable.CellCountChanged -= this.HandleRelationsTableCellCountChanged;
				this.relationsTable.CellsContentChanged -= this.HandleRelationsTableCellsContentChanged;
				this.relationsTable.SelectedRowChanged -= this.HandleRelationsTableSelectedRowChanged;
				this.relationsTable.SelectedRowDoubleClicked -= this.HandleRelationsTableSelectedRowDoubleClicked;

				this.splitter2.SplitterDragged -= this.HandleSplitterDragged;
				this.splitter3.SplitterDragged -= this.HandleSplitterDragged;
			}

			base.Dispose(disposing);
		}


		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Forms;
			}
		}

		public override bool HasUsefulViewerWindow
		{
			//	Indique si cette vue a l'utilité d'une fenêtre supplémentaire.
			get
			{
				return true;
			}
		}


		public FormEditor.Editor FormEditor
		{
			get
			{
				return this.formEditor;
			}
		}

		public Proxies.ProxyManager ProxyManager
		{
			get
			{
				return this.proxyManager;
			}
		}


		protected override void InitializeTable()
		{
			//	Initialise la table.
			StructuredType cultureMapType = new StructuredType();
			cultureMapType.Fields.Add("Name", StringType.NativeDefault);
			cultureMapType.Fields.Add("Druid", StringType.NativeDefault);
			cultureMapType.Fields.Add("Local", StringType.NativeDefault);
			cultureMapType.Fields.Add("Identity", StringType.NativeDefault);
			cultureMapType.Fields.Add("PatchLevel", StringType.NativeDefault);

			this.table.SourceType = cultureMapType;

			this.table.Columns.Add(new UI.ItemTableColumn("Name", new Widgets.Layouts.GridLength(this.GetColumnWidth(0), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Druid", new Widgets.Layouts.GridLength(this.GetColumnWidth(1), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Local", new Widgets.Layouts.GridLength(this.GetColumnWidth(2), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Identity", new Widgets.Layouts.GridLength(this.GetColumnWidth(3), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("PatchLevel", new Widgets.Layouts.GridLength(this.GetColumnWidth(4), Widgets.Layouts.GridUnitType.Proportional)));

			this.table.ColumnHeader.SetColumnComparer(1, this.CompareDruid);
			this.table.ColumnHeader.SetColumnComparer(2, this.CompareLocal);
			this.table.ColumnHeader.SetColumnComparer(3, this.CompareIdentity);
			this.table.ColumnHeader.SetColumnComparer(4, this.ComparePatchLevel);

			this.table.ColumnHeader.SetColumnText(0, Res.Strings.Viewers.Column.Name);
			this.table.ColumnHeader.SetColumnText(1, Res.Strings.Viewers.Column.Druid);
			this.table.ColumnHeader.SetColumnText(2, Res.Strings.Viewers.Column.Local);
			this.table.ColumnHeader.SetColumnText(3, Res.Strings.Viewers.Column.Identity);
			this.table.ColumnHeader.SetColumnText(4, Res.Strings.Viewers.Column.PatchLevel);

			this.table.ColumnHeader.SetColumnSort(0, ListSortDirection.Ascending);
		}

		protected override int PrimaryColumn
		{
			//	Retourne le rang de la colonne pour la culture principale.
			get
			{
				return -1;
			}
		}

		protected override int SecondaryColumn
		{
			//	Retourne le rang de la colonne pour la culture secondaire.
			get
			{
				return -1;
			}
		}

		protected override double GetColumnWidth(int column)
		{
			//	Retourne la largeur à utiliser pour une colonne de la liste de gauche.
			if (this.IsDisplayModeHorizontal)
			{
				return Forms.columnWidthHorizontal[column];
			}
			else
			{
				return Forms.columnWidthVertical[column];
			}
		}

		protected override void SetColumnWidth(int column, double value)
		{
			//	Mémorise la largeur à utiliser pour une colonne de la liste de gauche.
			if (this.IsDisplayModeHorizontal)
			{
				Forms.columnWidthHorizontal[column] = value;
			}
			else
			{
				Forms.columnWidthVertical[column] = value;
			}
		}

		
		public override void PaintHandler(Graphics graphics, Rectangle repaint, IPaintFilter paintFilter)
		{
			if (paintFilter == null)
			{
				paintFilter = this.formEditor;
			}
			
			base.PaintHandler(graphics, repaint, paintFilter);
		}


		public override void DoCommand(string name)
		{
			//	Exécute une commande.
			if (name == "PanelRun")
			{
				this.module.DesignerApplication.ActiveButton("PanelRun", true);
				this.Terminate(false);
				this.module.RunForm(this.access.AccessIndex);
				this.module.DesignerApplication.ActiveButton("PanelRun", false);
				return;
			}

			if (name == "FormFieldsShowPrefix")  // affiche/cache les préfixes ?
			{
				Forms.showPrefix = !Forms.showPrefix;
				this.UpdateFieldsTable(false);
				return;
			}

			if (name == "FormFieldsShowGuid")  // affiche/cache les Guids ?
			{
				Forms.showGuid = !Forms.showGuid;
				this.UpdateFieldsTable(false);
				return;
			}

			if (name == "FormFieldsShowColumn1")  // affiche/cache la 2ème colonne ?
			{
				Forms.showColumn1 = !Forms.showColumn1;
				this.UpdateFieldsTableColumns();
				return;
			}

			if (name == "FormFieldsShowColumn2")  // affiche/cache la 3ème colonne ?
			{
				Forms.showColumn2 = !Forms.showColumn2;
				this.UpdateFieldsTableColumns();
				return;
			}

			if (name == "FormFieldsClearDelta")  // efface tout le masque delta ?
			{
				this.workingForm.Fields.Clear();
				this.SetForm(false);
				this.UpdateFieldsTable(true);
				this.ReflectSelectionToList();
				this.UpdateFieldsButtons();
				this.UpdateRelationsTable(false);
				this.UpdateRelationsButtons();
				this.module.AccessForms.SetLocalDirty();
				return;
			}

			this.formEditor.DoCommand(name);
			base.DoCommand(name);
		}

		public override string InfoViewerText
		{
			//	Donne le texte d'information sur le visualisateur en cours.
			get
			{
				return this.formEditor.SelectionInfo;
			}
		}


		protected override bool IsDeleteOrDuplicateForViewer
		{
			//	Indique s'il faut aiguiller ici une opération delete ou duplicate.
			get
			{
				return false;
			}
		}

		protected override void PrepareForDelete()
		{
			//	Préparation en vue de la suppression de l'interface.
			this.formEditor.PrepareForDelete();
		}


		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			base.UpdateEdit();

			bool iic = this.ignoreChange;
			this.ignoreChange = true;
			this.Deserialize();
			this.UpdateFieldsTable(true);
			this.UpdateFieldsButtons();
			this.UpdateRelationsTable(true);
			this.UpdateRelationsButtons();
			this.UpdateMiscPage();
			this.DefineProxies();  // met à jour les proxies
			this.ignoreChange = iic;
		}

		protected void UpdateFieldsTableColumns()
		{
			//	Met à jour la largeur des colonnes de la table des champs.
			double w1 = Forms.showColumn1 ? 0.10 : 0.00;
			double w2 = Forms.showColumn2 ? 0.10 : 0.00;

			this.fieldsTable.SetColumnsRelativeWidth(0, 1.00-w1-w2);
			this.fieldsTable.SetColumnsRelativeWidth(1, w1);
			this.fieldsTable.SetColumnsRelativeWidth(2, w2);
		}

		protected void UpdateFieldsTable(bool newContent)
		{
			//	Met à jour la table des champs.
			this.formEditor.ObjectModifier.UpdateTableContent();

			int first = this.fieldsTable.FirstVisibleRow;
			for (int i=0; i<this.fieldsTable.LineCount; i++)
			{
				if (first+i >= this.formEditor.ObjectModifier.TableContent.Count)
				{
					this.fieldsTable.SetLineString(0, first+i, "");
					this.fieldsTable.SetLineTooltip(0, first+i, "");
					this.fieldsTable.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);
					this.fieldsTable.SetLineColor(0, first+i, Color.Empty);

					this.fieldsTable.SetLineString(1, first+i, "");
					this.fieldsTable.SetLineState(1, first+i, MyWidgets.StringList.CellState.Disabled);
					this.fieldsTable.SetLineColor(1, first+i, Color.Empty);

					this.fieldsTable.SetLineString(2, first+i, "");
					this.fieldsTable.SetLineState(2, first+i, MyWidgets.StringList.CellState.Disabled);
					this.fieldsTable.SetLineColor(2, first+i, Color.Empty);
				}
				else
				{
					FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[first+i];
					string name = this.formEditor.ObjectModifier.GetTableContentDescription(item, true, Forms.showPrefix, Forms.showGuid);
					string icon1 = this.formEditor.ObjectModifier.GetTableContentIcon1(item);
					string icon2 = this.formEditor.ObjectModifier.GetTableContentIcon2(item);
					Color color = this.formEditor.ObjectModifier.GetTableContentUseColor(item);

					this.fieldsTable.SetLineString(0, first+i, name);
					this.fieldsTable.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);
					this.fieldsTable.SetLineColor(0, first+i, color);

					if (Forms.showPrefix)
					{
						this.fieldsTable.SetLineTooltip(0, first+i, null);
					}
					else
					{
						string tooltip = this.formEditor.ObjectModifier.GetTableContentDescription(item, false, true, false);
						this.fieldsTable.SetLineTooltip(0, first+i, tooltip);
					}

					this.fieldsTable.SetLineString(1, first+i, icon1);
					this.fieldsTable.SetLineState(1, first+i, MyWidgets.StringList.CellState.Normal);
					this.fieldsTable.SetLineColor(1, first+i, color);

					this.fieldsTable.SetLineString(2, first+i, icon2);
					this.fieldsTable.SetLineState(2, first+i, MyWidgets.StringList.CellState.Normal);
					this.fieldsTable.SetLineColor(2, first+i, color);
				}
			}

			this.fieldsTable.TotalRows = this.formEditor.ObjectModifier.TableContent.Count;

			if (newContent)
			{
				this.fieldsTable.FirstVisibleRow = 0;
				this.fieldsTable.SelectedRows = new List<int>();  // désélectionne tout
			}

			this.fieldsTable.SetDynamicToolTips(0, !Forms.showPrefix);
		}

		protected void UpdateFieldsButtons()
		{
			//	Met à jour les boutons dans l'onglet des champs.
			bool isSel = false;
			bool isPrev = false;
			bool isNext = false;
			bool isGoto = false;
			bool isUnbox = false;
			bool isForm = false;
			bool isDeletable = false;
			bool isDelta = this.formEditor.ObjectModifier.IsDelta;
			bool isGlue = false;

			if (!this.designerApplication.IsReadonly)
			{
				List<int> sels = this.fieldsTable.SelectedRows;

				if (sels != null && sels.Count > 0)  // sélection multiple ?
				{
					isSel = true;
					isPrev = true;
					isNext = true;
					isGoto = (sels.Count == 1);
					isDeletable = true;

					foreach (int sel in sels)
					{
						if (sel >= this.formEditor.ObjectModifier.TableContent.Count)  // ancienne sélection parasite ?
						{
							continue;
						}

						if (sel <= 0)
						{
							isPrev = false;
						}

						if (sel >= this.formEditor.ObjectModifier.TableContent.Count-1)
						{
							isNext = false;
						}

						if (this.formEditor.ObjectModifier.TableContent[sel].FieldType != FieldDescription.FieldType.Field)
						{
							isGoto = false;
						}

						if (!this.IsDeletableField(sel))
						{
							isDeletable = false;
						}
					}
				}

				if (sels != null && sels.Count == 1)  // sélection simple ?
				{
					int sel = sels[0];

					if (sel < this.formEditor.ObjectModifier.TableContent.Count)
					{
						if (this.formEditor.ObjectModifier.TableContent[sel].FieldType == FieldDescription.FieldType.BoxBegin)
						{
							isUnbox = true;
						}

						if (this.formEditor.ObjectModifier.TableContent[sel].FieldType == FieldDescription.FieldType.SubForm)
						{
							isForm = true;
						}

						if (this.IsDeletableField(sel))
						{
							isDeletable = true;
						}
					}
				}

				if (sels != null && sels.Count == 2)  // sélection double ?
				{
					if (sels[0] == sels[1]+1 || sels[0] == sels[1]-1)
					{
						isGlue = true;
					}
				}
			}

			this.fieldsButtonRemove.Enable = isSel;
			this.fieldsButtonReset.Enable  = isSel;
			this.fieldsButtonBox.Enable    = isSel && !isDelta;

			this.fieldsButtonForm.Enable = isForm && !isDelta;
			this.fieldsButtonPrev.Enable = isPrev;
			this.fieldsButtonNext.Enable = isNext;
			this.fieldsButtonGoto.Enable = isGoto;

			this.otherButtonCommand.Enable = !this.designerApplication.IsReadonly;
			this.otherButtonLine.Enable    = !this.designerApplication.IsReadonly;
			this.otherButtonTitle.Enable   = !this.designerApplication.IsReadonly;
			this.otherButtonGlue.Enable    = isGlue;

			this.fieldsButtonRemove.IconUri = isDeletable ? Misc.Icon("Delete") : Misc.Icon("FormDeltaHide");
			this.fieldsButtonBox.IconUri    = isUnbox ? Misc.Icon("FormUnbox") : Misc.Icon("FormBox");
		}

		protected bool IsDeletableField(int sel)
		{
			//	Indique si un élément peut être véritablement supprimé, et non simplement caché.
			FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sel];
			int index = FormEngine.Arrange.IndexOfGuid(this.workingForm.Fields, item.Guid);

			if (index == -1)
			{
				return false;
			}

			FieldDescription field = this.workingForm.Fields[index];

			if (this.formEditor.ObjectModifier.IsDelta)
			{
				return field.DeltaInserted;
			}
			else
			{
				return field.Type == FieldDescription.FieldType.Command ||
					   field.Type == FieldDescription.FieldType.Line ||
					   field.Type == FieldDescription.FieldType.Title ||
					   field.Type == FieldDescription.FieldType.Glue;
			}
		}

		protected void UpdateRelationsTable(bool newContent)
		{
			//	Met à jour la table des relations.
			List<FormEditor.ObjectModifier.RelationItem> list = this.formEditor.ObjectModifier.TableRelations;

			int first = this.relationsTable.FirstVisibleRow;
			for (int i=0; i<this.relationsTable.LineCount; i++)
			{
				if (first+i >= list.Count)
				{
					this.relationsTable.SetLineString(0, first+i, "");
					this.relationsTable.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);
					this.relationsTable.SetLineColor(0, first+i, Color.Empty);

					this.relationsTable.SetLineString(1, first+i, "");
					this.relationsTable.SetLineState(1, first+i, MyWidgets.StringList.CellState.Disabled);
					this.relationsTable.SetLineColor(1, first+i, Color.Empty);

					this.relationsTable.SetLineString(2, first+i, "");
					this.relationsTable.SetLineState(2, first+i, MyWidgets.StringList.CellState.Disabled);
					this.relationsTable.SetLineColor(2, first+i, Color.Empty);
				}
				else
				{
					string name = this.formEditor.ObjectModifier.GetTableRelationDescription(first+i);
					string icon = this.formEditor.ObjectModifier.GetTableRelationRelIcon(first+i);
					string used = this.formEditor.ObjectModifier.GetTableRelationUseIcon(first+i);
					Color color = this.formEditor.ObjectModifier.GetTableRelationUseColor(first+i);

					this.relationsTable.SetLineString(0, first+i, used);
					this.relationsTable.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);
					this.relationsTable.SetLineColor(0, first+i, color);

					this.relationsTable.SetLineString(1, first+i, name);
					this.relationsTable.SetLineState(1, first+i, MyWidgets.StringList.CellState.Normal);
					this.relationsTable.SetLineColor(1, first+i, Color.Empty);

					this.relationsTable.SetLineString(2, first+i, icon);
					this.relationsTable.SetLineState(2, first+i, MyWidgets.StringList.CellState.Normal);
					this.relationsTable.SetLineColor(2, first+i, Color.Empty);
				}
			}

			this.relationsTable.TotalRows = list.Count;

			if (newContent)
			{
				this.relationsTable.FirstVisibleRow = 0;
				this.relationsTable.SelectedRow = -1;
			}
			else
			{
				if (this.relationsTable.TotalRows <= this.relationsTable.LineCount)
				{
					this.relationsTable.FirstVisibleRow = 0;
				}
			}
		}

		protected void UpdateRelationsButtons()
		{
			//	Met à jour les boutons dans l'onglet des relations.
			int sel = this.relationsTable.SelectedRow;

			this.relationsButtonUse.Enable = this.formEditor.ObjectModifier.IsTableRelationUseable(sel);
			this.relationsButtonExpand.Enable = this.formEditor.ObjectModifier.IsTableRelationExpandable(sel);
			this.relationsButtonCompact.Enable = this.formEditor.ObjectModifier.IsTableRelationCompactable(sel);

			this.relationsButtonAuto.Enable = !this.designerApplication.IsReadonly && this.finalFields != null;
		}


		#region UndoRedo
		public override void Undo()
		{
			//	Annule la dernière action.
			Undo.Shapshot snapshot = this.undoEngine.Undo(this.UndoCurrentSnapshot(null));
			this.UndoRestore(snapshot);
			this.UpdateUndoRedoCommands();
		}

		public override void Redo()
		{
			//	Refait la dernière action.
			Undo.Shapshot snapshot = this.undoEngine.Redo();
			this.UndoRestore(snapshot);
			this.UpdateUndoRedoCommands();
		}

		public override VMenu UndoRedoCreateMenu(Support.EventHandler<MessageEventArgs> message)
		{
			//	Crée le menu undo/redo.
			return this.undoEngine.CreateMenu(message);
		}

		public override void UndoRedoGoto(int index)
		{
			//	Annule ou refait quelques actions, selon le menu.
			Undo.Shapshot snapshot = this.undoEngine.Goto(index, this.UndoCurrentSnapshot(null));
			this.UndoRestore(snapshot);
			this.UpdateUndoRedoCommands();
		}

		public override void UndoFlush()
		{
			//	Les commandes annuler/refaire ne seront plus possibles.
			this.undoEngine.Flush();
			this.UpdateUndoRedoCommands();
		}

		protected override bool IsUndoEnable
		{
			//	Retourne true si la commande "Undo" doit être active.
			get
			{
				return this.undoEngine.IsUndoEnable;
			}
		}

		protected override bool IsRedoEnable
		{
			//	Retourne true si la commande "Redo" doit être active.
			get
			{
				return this.undoEngine.IsRedoEnable;
			}
		}

		protected override bool IsUndoRedoListEnable
		{
			//	Retourne true si la commande "UndoRedoList" pour le menu doit être active.
			get
			{
				return this.undoEngine.IsUndoRedoListEnable;
			}
		}

		public void UndoMemorize(string snapshotName, bool merge)
		{
			//	Mémorise l'état actuel, avant d'effectuer une modification dans this.workingForm.
			//	Si merge = true et que la dernière photographie avait le même nom, on conserve la dernière
			//	photographie mémorisée.
			this.undoEngine.Memorize(this.UndoCurrentSnapshot(snapshotName), merge);
			this.UpdateUndoRedoCommands();
		}

		protected Undo.Shapshot UndoCurrentSnapshot(string snapshotName)
		{
			//	Retourne la photographie courante, prête à être mémorisée dans this.undoEngine.
			Undo.Shapshot snapshot = new Undo.Shapshot();

			snapshot.Name = snapshotName;
			snapshot.SerializedData = this.FormToXml(this.workingForm);

			foreach (int sel in this.fieldsTable.SelectedRows)
			{
				snapshot.Selection.Add(sel);
			}

			return snapshot;
		}

		protected void UndoRestore(Undo.Shapshot snapshot)
		{
			//	Remet l'éditeur de masques dans un état précédent.
			FormDescription inputForm = this.XmlToForm(snapshot.SerializedData);
			this.access.GetForm(this.druidToSerialize, inputForm, out this.workingForm, out this.baseFields, out this.finalFields, out this.entityId);
			this.SetForm(false);

			this.formEditor.DeselectAll();
			this.UpdateFieldsTable(false);

			List<int> sels = new List<int>();
			foreach (int sel in snapshot.Selection)
			{
				sels.Add(sel);
			}
			this.fieldsTable.SelectedRows = sels;
			this.ReflectSelectionToEditor();

			this.relationsTable.SelectedRow = -1;
			
			this.UpdateFieldsButtons();
			this.UpdateRelationsTable(false);
			this.UpdateRelationsButtons();
			this.UpdateMiscPage();
		}
		#endregion

	
		public override bool Terminate(bool soft)
		{
			//	Termine le travail sur une ressource, avant de passer à une autre.
			//	Si soft = true, on sérialise temporairement sans poser de question.
			//	Retourne false si l'utilisateur a choisi "annuler".
			base.Terminate(soft);

			if (this.module.AccessForms.IsLocalDirty)
			{
				System.Diagnostics.Debug.Assert(soft);
				
				if (this.druidToSerialize.IsValid)
				{
					Forms.softWorkingForm = this.workingForm;  // voir (*)
					Forms.softBaseFields  = this.baseFields;
					Forms.softFinalFields = this.finalFields;
					Forms.softEntityId    = this.entityId;

					// (*)	Il ne faut surtout pas faire une copie comme ceci:
					//		Forms.softWorkingForm = new FormDescription(this.workingForm);
					//		this.finalFields est parfois la liste directement dans this.workingForm (mais pas toujours).
					//		Une copie fait qu'une modification dans this.workingForm n'est plus reportée dans this.finalFields !
				}
				else
				{
					Forms.softWorkingForm = null;
					Forms.softBaseFields  = null;
					Forms.softFinalFields = null;
					Forms.softEntityId    = Druid.Empty;
				}
			}

			return true;
		}

		protected override void PersistChanges()
		{
			//	Stocke la version XML (sérialisée) du masque de saisie dans l'accesseur
			//	s'il y a eu des modifications.
			if (this.access.SetForm(this.druidToSerialize, this.workingForm))
			{
				base.PersistChanges();
			}
		}

		protected void Deserialize()
		{
			//	Désérialise les données sérialisées.
			int sel = this.access.AccessIndex;
			this.druidToSerialize = Druid.Empty;

			if (sel != -1)
			{
				this.druidToSerialize = this.access.AccessDruid(sel);
			}

			if (Forms.softWorkingForm == null)
			{
				this.access.GetForm(this.druidToSerialize, null, out this.workingForm, out this.baseFields, out this.finalFields, out this.entityId);
				this.SetForm(false);
			}
			else
			{
				if (this.module.AccessForms.IsLocalDirty)
				{
					this.module.AccessForms.SetLocalDirty();
				}
				else
				{
					this.module.AccessForms.ClearLocalDirty();
				}

				this.workingForm = Forms.softWorkingForm;
				this.baseFields  = Forms.softBaseFields;
				this.finalFields = Forms.softFinalFields;
				this.entityId    = Forms.softEntityId;
				this.SetForm(false);

				Forms.softWorkingForm = null;
				Forms.softBaseFields  = null;
				Forms.softFinalFields = null;
				Forms.softEntityId    = Druid.Empty;
			}
		}

		protected string FormToXml(FormDescription form)
		{
			//	form -> xml.
			return FormEngine.Serialization.SerializeForm(form);
		}

		protected FormDescription XmlToForm(string xml)
		{
			//	xml -> form.
			return FormEngine.Serialization.DeserializeForm(xml);
		}

		protected void SetForm(bool keepSelection)
		{
			//	Spécifie le masque de saisie en cours d'édition.
			//	Construit physiquement le masque de saisie (UI.Panel contenant des widgets) sur la
			//	base de sa description FormDescription.
			if (this.panelContainer != null)
			{
				this.panelContainer.SetParent(null);
				this.panelContainer = null;
			}

			if (this.finalFields == null)
			{
				this.panelContainer = new UI.Panel();
				this.InitializePanel();

				this.formEditor.Panel = this.panelContainer;
				this.formEditor.Druid = this.druidToSerialize;
				this.formEditor.IsEditEnabled = false;
				this.formEditor.WorkingForm = null;
				this.formEditor.BaseFields = null;
				this.formEditor.FinalFields = null;

				this.entityFields = null;
			}
			else
			{
				List<System.Guid> guids = null;
				if (keepSelection)
				{
					guids = this.formEditor.GetSelectedGuids();  // Guid des objets sélectionnés
				}

				if (this.workingForm.IsDelta)
				{
					this.finalFields = this.engine.Arrange.Merge(this.baseFields, this.workingForm.Fields);
				}

				this.panelContainer = this.engine.CreateForm(this.finalFields, this.entityId, true);

				if (this.panelContainer == null)
				{
					this.panelContainer = new UI.Panel();
				}
				else
				{
					this.UpdateMiscPagePanel();
				}
				
				this.InitializePanel();

				this.formEditor.Panel = this.panelContainer;
				this.formEditor.Druid = this.druidToSerialize;
				this.formEditor.IsEditEnabled = !this.designerApplication.IsReadonly;
				this.formEditor.WorkingForm = this.workingForm;
				this.formEditor.BaseFields = this.baseFields;
				this.formEditor.FinalFields = this.finalFields;

				if (keepSelection)
				{
					this.formEditor.SetSelectedGuids(guids);  // resélectionne les mêmes objets

					//	Il ne faut surtout pas régénérer les proxies ici, car cela cause de gros problèmes
					//	pendant le drag d'un slider, par exemple !
					//	this.DefineProxies();  // met à jour les proxies
				}
				else
				{
					this.formEditor.DeselectAll();
				}

				this.entityFields = this.module.AccessEntities.GetEntityDruidsPath(this.entityId);
			}

			if (!keepSelection)
			{
				this.formEditor.ObjectModifier.UpdateTableRelation(this.entityId, this.entityFields);
			}
		}

		protected void InitializePanel()
		{
			//	Initialise le panneau contenant le masque pour pouvoir être édité.
			this.panelContainer.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Stacked;
			this.panelContainer.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
			this.panelContainer.Dock = DockStyle.Fill;
			this.panelContainer.Margins = new Margins(Common.Designer.FormEditor.Editor.margin, Common.Designer.FormEditor.Editor.margin, Common.Designer.FormEditor.Editor.margin, Common.Designer.FormEditor.Editor.margin);
			this.panelContainer.DrawDesignerFrame = true;
			this.panelContainer.SetParent(this.panelContainerParent);
			this.panelContainer.ZOrder = this.formEditor.ZOrder+1;
		}


		public override void Update()
		{
			//	Met à jour le contenu du Viewer.
			this.formEditor.DeselectAll();
			this.UpdateArray();
			this.UpdateEdit();
			this.UpdateFieldsTable(true);
			this.UpdateFieldsButtons();
			this.UpdateRelationsTable(true);
			this.UpdateRelationsButtons();
			this.UpdateMiscPage();
			this.UpdateCommands();
		}


		#region MiscPage
		protected void CreateMiscPage()
		{
			//	Crée tous les widgets pour la page "divers".
			int index = 1;

			FrameBox widthBox = new FrameBox(this.tabPageMisc);
			widthBox.TabIndex = index++;
			widthBox.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			widthBox.Margins = new Margins(0, 0, 0, 2);
			widthBox.Dock = DockStyle.Top;

			this.miscWidthButton = new CheckButton(widthBox);
			this.miscWidthButton.AutoToggle = false;
			this.miscWidthButton.PreferredWidth = 140;
			this.miscWidthButton.Text = Res.Strings.Viewers.Forms.MiscPage.Width;
			this.miscWidthButton.TabIndex = index++;
			this.miscWidthButton.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.miscWidthButton.Dock = DockStyle.Left;
			this.miscWidthButton.Clicked += this.HandleMiscButtonClicked;

			this.miscWidthField = new TextFieldUpDown(widthBox);
			this.miscWidthField.Resolution = 1.0M;
			this.miscWidthField.Step = 1.0M;
			this.miscWidthField.MinValue = 10.0M;
			this.miscWidthField.MaxValue = 2000.0M;
			this.miscWidthField.PreferredWidth = 60;
			this.miscWidthField.TabIndex = index++;
			this.miscWidthField.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.miscWidthField.Dock = DockStyle.Left;
			this.miscWidthField.TextChanged += this.HandleMiscFieldTextChanged;

			FrameBox heightBox = new FrameBox(this.tabPageMisc);
			heightBox.TabIndex = index++;
			heightBox.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			heightBox.Margins = new Margins(0, 0, 0, 2);
			heightBox.Dock = DockStyle.Top;

			this.miscHeightButton = new CheckButton(heightBox);
			this.miscHeightButton.AutoToggle = false;
			this.miscHeightButton.PreferredWidth = 140;
			this.miscHeightButton.Text = Res.Strings.Viewers.Forms.MiscPage.Height;
			this.miscHeightButton.TabIndex = index++;
			this.miscHeightButton.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.miscHeightButton.Dock = DockStyle.Left;
			this.miscHeightButton.Clicked += this.HandleMiscButtonClicked;

			this.miscHeightField = new TextFieldUpDown(heightBox);
			this.miscHeightField.Resolution = 1.0M;
			this.miscHeightField.Step = 1.0M;
			this.miscHeightField.MinValue = 10.0M;
			this.miscHeightField.MaxValue = 2000.0M;
			this.miscHeightField.PreferredWidth = 60;
			this.miscHeightField.TabIndex = index++;
			this.miscHeightField.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.miscHeightField.Dock = DockStyle.Left;
			this.miscHeightField.TextChanged += this.HandleMiscFieldTextChanged;
		}

		private void HandleMiscButtonClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton pour la largeur ou la hauteur préférentielle a changé.
			if (this.ignoreChange || this.designerApplication.IsReadonly)
			{
				return;
			}

			CheckButton button = sender as CheckButton;
			button.ActiveState = (button.ActiveState == ActiveState.No) ? ActiveState.Yes : ActiveState.No;
			Size defaultSize = this.workingForm.DefaultSize;

			if (button == this.miscWidthButton)
			{
				if (button.ActiveState == ActiveState.No)
				{
					defaultSize.Width = double.NaN;
				}
				else
				{
					defaultSize.Width = 800;
				}
			}

			if (button == this.miscHeightButton)
			{
				if (button.ActiveState == ActiveState.No)
				{
					defaultSize.Height = double.NaN;
				}
				else
				{
					defaultSize.Height = 600;
				}
			}

			this.UndoMemorize(Res.Strings.Undo.Action.DefaultSize, true);
			this.workingForm.DefaultSize = defaultSize;
			this.module.AccessForms.SetLocalDirty();
			this.UpdateMiscPage();
			this.UpdateMiscPagePanel();
		}

		private void HandleMiscFieldTextChanged(object sender)
		{
			//	Le texte pour la largeur ou la hauteur préférentielle a changé.
			if (this.ignoreChange || this.designerApplication.IsReadonly)
			{
				return;
			}

			TextFieldUpDown field = sender as TextFieldUpDown;
			Size defaultSize = this.workingForm.DefaultSize;

			if (field == this.miscWidthField)
			{
				double value = double.NaN;

				if (!string.IsNullOrEmpty(field.Text))
				{
					value = (double) field.Value;
				}

				defaultSize.Width = value;
			}

			if (field == this.miscHeightField)
			{
				double value = double.NaN;

				if (!string.IsNullOrEmpty(field.Text))
				{
					value = (double) field.Value;
				}

				defaultSize.Height = value;
			}

			if (this.workingForm.DefaultSize != defaultSize)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.DefaultSize, true);
				this.workingForm.DefaultSize = defaultSize;
				this.UpdateMiscPagePanel();
				this.module.AccessForms.SetLocalDirty();
			}
		}

		protected void UpdateMiscPage()
		{
			//	Met à jour tous les widgets pour la page "divers".
			this.ignoreChange = true;

			Size defaultSize = new Size(double.NaN, double.NaN);
			if (this.workingForm != null)
			{
				defaultSize = this.workingForm.DefaultSize;
			}

			if (double.IsNaN(defaultSize.Width))
			{
				this.miscWidthButton.ActiveState = ActiveState.No;
				this.miscWidthButton.Enable = !this.designerApplication.IsReadonly;
				this.miscWidthField.Text = "";
				this.miscWidthField.Enable = false;
			}
			else
			{
				this.miscWidthButton.ActiveState = ActiveState.Yes;
				this.miscWidthButton.Enable = !this.designerApplication.IsReadonly;
				this.miscWidthField.Value = (decimal) defaultSize.Width;
				this.miscWidthField.Enable = !this.designerApplication.IsReadonly;
			}

			if (double.IsNaN(defaultSize.Height))
			{
				this.miscHeightButton.ActiveState = ActiveState.No;
				this.miscHeightButton.Enable = !this.designerApplication.IsReadonly;
				this.miscHeightField.Text = "";
				this.miscHeightField.Enable = false;
			}
			else
			{
				this.miscHeightButton.ActiveState = ActiveState.Yes;
				this.miscHeightButton.Enable = !this.designerApplication.IsReadonly;
				this.miscHeightField.Value = (decimal) defaultSize.Height;
				this.miscHeightField.Enable = !this.designerApplication.IsReadonly;
			}

			this.ignoreChange = false;
		}

		protected void UpdateMiscPagePanel()
		{
			//	Met à jour le vrai panneau (this.panelContainer) en fonction des réglages de
			//	l'onglet "divers", dans le but de représenter le panneau avec la taille que
			//	l'utilisateur spécifie.
			Size defaultSize = this.workingForm.DefaultSize;

			if (double.IsNaN(defaultSize.Width))
			{
				this.panelContainer.MinWidth = 0.0;
				this.panelContainer.MaxWidth = double.PositiveInfinity;
				this.panelContainer.HorizontalAlignment = HorizontalAlignment.Stretch;
			}
			else
			{
				this.panelContainer.MinWidth = defaultSize.Width;
				this.panelContainer.MaxWidth = defaultSize.Width;
				this.panelContainer.HorizontalAlignment = HorizontalAlignment.Left;
			}

			if (double.IsNaN(defaultSize.Height))
			{
				this.panelContainer.MinHeight = 0.0;
				this.panelContainer.MaxHeight = double.PositiveInfinity;
				this.panelContainer.VerticalAlignment = VerticalAlignment.Stretch;
			}
			else
			{
				this.panelContainer.MinHeight = defaultSize.Height;
				this.panelContainer.MaxHeight = defaultSize.Height;
				this.panelContainer.VerticalAlignment = VerticalAlignment.Top;
			}
		}
		#endregion


		#region CultureButtons
		protected void CreateCultureButtons()
		{
			//	Crée tous les boutons pour les cultures.
			this.cultureButtonList = new List<IconButton>();

			int tabIndex = 1;
			foreach (string name in Misc.Cultures)
			{
				System.Globalization.CultureInfo culture = Resources.FindSpecificCultureInfo(name);

				IconButton button = new IconButton(this.tabPageCultures);
				button.Name = name;
				button.Text = Misc.CultureName(culture);
				button.ButtonStyle = ButtonStyle.ActivableIcon;
				button.Dock = DockStyle.Top;
				button.Margins = new Margins(0, 0, 0, 2);
				button.TabIndex = tabIndex++;
				button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				button.Clicked += this.HandleCultureButtonClicked;
				ToolTip.Default.SetToolTip(button, Misc.CultureLongName(culture));

				this.cultureButtonList.Add(button);
			}

			this.UpdateCultureButtons();
		}

		private void HandleCultureButtonClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton pour changer de culture a été cliqué.
			IconButton button = sender as IconButton;
			this.module.ResourceManager.ActiveCulture = Resources.FindSpecificCultureInfo(button.Name);
			this.panelContainer.UpdateDisplayCaptions();
			this.UpdateCultureButtons();
			this.SetForm(true);
		}

		protected void UpdateCultureButtons()
		{
			//	Met à jour les boutons pour les cultures.
			string culture = Misc.CultureBaseName(this.module.ResourceManager.ActiveCulture);
			foreach (IconButton button in this.cultureButtonList)
			{
				bool active = (button.Name == culture);
				button.ActiveState = active ? ActiveState.Yes : ActiveState.No;
			}
		}
		#endregion


		public override void UpdateViewer(Viewers.Changing oper)
		{
			//	Met à jour le statut du visualisateur en cours, en fonction de la sélection.
			//	Met également à jour l'arbre des objets, s'il est visible.
			if (oper == Changing.Selection)
			{
				this.ReflectSelectionToList();
				this.UpdateFieldsButtons();
			}

			if (oper == Changing.Create || oper == Changing.Delete || oper == Changing.Move || oper == Changing.Regenerate)
			{
				//	Régénère le panneau contenant le masque de saisie.
				this.SetForm(oper == Changing.Regenerate);
				this.UpdateFieldsTable(false);
			}
		}


		protected void ReflectSelectionToList()
		{
			//	Reflète les sélections effectuées dans l'éditeur de Forms dans la liste des champs.
			//	Editeur de Forms -> Liste des champs.
			List<System.Guid> guids = this.formEditor.GetSelectedGuids();
			List<int> sels = new List<int>();

			foreach (System.Guid guid in guids)
			{
				int sel = this.formEditor.ObjectModifier.GetTableContentIndex(guid);
				if (sel != -1)
				{
					sels.Add(sel);
				}
			}

			this.ignoreChange = true;
			this.fieldsTable.SelectedRows = sels;
			this.ignoreChange = false;
		}

		protected void ReflectSelectionToEditor()
		{
			//	Reflète les sélections effectuées dans la liste des champs dans l'éditeur de Forms.
			//	Liste des champs -> Editeur de Forms.
			List<int> sels = this.fieldsTable.SelectedRows;

			if (sels == null || sels.Count == 0)
			{
				this.formEditor.DeselectAll();
			}
			else
			{
				List<System.Guid> guids = new List<System.Guid>();
				foreach (int sel in sels)
				{
					if (sel != -1)
					{
						guids.Add(this.formEditor.ObjectModifier.TableContent[sel].Guid);
					}
				}

				this.formEditor.SelectListObject(guids);
			}
		}


		protected void SelectedFieldsRemove()
		{
			//	Utilise ou supprime les champs sélectionnés.
			this.UndoMemorize(Res.Strings.Undo.Action.FieldRemove, false);

			List<int> sels = this.fieldsTable.SelectedRows;
			sels.Sort();

			List<System.Guid> guids = new List<System.Guid>();

			foreach (int sel in sels)
			{
				FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sel];
				FieldDescription field = this.formEditor.ObjectModifier.GetFieldDescription(item);
				int index = FormEngine.Arrange.IndexOfGuid(this.workingForm.Fields, item.Guid);

				if (this.IsDeletableField(sel))  // élément à supprimer réellement ?
				{
					if (index != -1)
					{
						this.workingForm.Fields.RemoveAt(index);
					}
				}
				else  // élément à montrer/cacher ?
				{
					if (this.formEditor.ObjectModifier.IsDelta)  // masque delta ?
					{
						if (index == -1)
						{
							if (field.DeltaHidden)
							{
								FieldDescription copy = new FieldDescription(field);
								copy.DeltaShowed = true;
								copy.DeltaHidden = false;
								this.workingForm.Fields.Add(copy);  // ajoute l'élément pour dire "montré"
							}
							else
							{
								FieldDescription copy = new FieldDescription(field);
								copy.DeltaHidden = true;
								copy.DeltaShowed = false;
								this.workingForm.Fields.Add(copy);  // ajoute l'élément pour dire "caché"
							}
						}
						else
						{
							FieldDescription actual = this.workingForm.Fields[index];

							if (this.formEditor.ObjectModifier.IsTableContentInheritHidden(item))
							{
								if (field.DeltaHidden)
								{
									actual.DeltaShowed = true;
									actual.DeltaHidden = false;
								}
								else
								{
									actual.DeltaHidden = true;
									actual.DeltaShowed = false;
								}
							}
							else
							{
								if (actual.DeltaShowed)  // champ montré ?
								{
									actual.DeltaShowed = false;  // cet élément sera supprimé, et c'est donc le parent DeltaHidden qui dira de cacher
								}
								else if (actual.DeltaHidden)  // champ caché ?
								{
									actual.DeltaHidden = false;  // rend le champ visible
								}
								else  // champ visible ?
								{
									actual.DeltaHidden = true;  // cache le champ
								}

								if (!actual.Delta)
								{
									this.workingForm.Fields.RemoveAt(index);
								}
							}
						}
					}
					else  // masque normal ?
					{
						if (index != -1)
						{
							this.workingForm.Fields[index].DeltaHidden = !this.workingForm.Fields[index].DeltaHidden;
						}
					}

					guids.Add(item.Guid);
				}
			}

			this.SetForm(true);
			this.UpdateFieldsTable(false);

			sels.Clear();
			foreach (System.Guid guid in guids)
			{
				int index = this.formEditor.ObjectModifier.GetTableContentIndex(guid);
				if (index != -1)
				{
					sels.Add(index);
				}
			}
			this.fieldsTable.SelectedRows = sels;
			this.ReflectSelectionToEditor();

			this.UpdateFieldsButtons();
			this.UpdateRelationsTable(false);
			this.UpdateRelationsButtons();
			this.module.AccessForms.SetLocalDirty();
		}

		protected void SelectedFieldsReset()
		{
			//	Remet à zéro les champs sélectionnés, dans un masque delta.
			this.UndoMemorize(Res.Strings.Undo.Action.FieldReset, false);

			List<int> sels = this.fieldsTable.SelectedRows;
			sels.Sort();

			List<System.Guid> guids = new List<System.Guid>();

			foreach (int sel in sels)
			{
				FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sel];
				int index = FormEngine.Arrange.IndexOfGuid(this.workingForm.Fields, item.Guid);
				if (index != -1)
				{
					if (this.formEditor.ObjectModifier.IsDelta)  // masque delta ?
					{
						this.workingForm.Fields.RemoveAt(index);
					}
					else  // masque normal ?
					{
						FieldDescription field = this.workingForm.Fields[index];
						field.Reset();
					}
				}

				guids.Add(item.Guid);
			}

			this.SetForm(true);
			this.UpdateFieldsTable(false);

			sels.Clear();
			foreach (System.Guid guid in guids)
			{
				int index = this.formEditor.ObjectModifier.GetTableContentIndex(guid);
				if (index != -1)
				{
					sels.Add(index);
				}
			}
			this.fieldsTable.SelectedRows = sels;
			this.ReflectSelectionToEditor();

			this.UpdateFieldsButtons();
			this.UpdateRelationsTable(false);
			this.UpdateRelationsButtons();
			this.module.AccessForms.SetLocalDirty();
		}

		protected void SelectedFieldsForm()
		{
			//	Choix du sous-masque à utiliser pour la relation.
			List<int> sels = this.fieldsTable.SelectedRows;
			if (sels == null || sels.Count == 0)
			{
				return;
			}
			sels.Sort();

			FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sels[0]];
			FieldDescription field = this.formEditor.ObjectModifier.GetFieldDescription(item);

			Druid druid = field.SubFormId;
			Druid typeId = this.access.FormRelationEntity(this.entityId, field.GetPath(null));
			bool isNullable = false;
			Module module = this.designerApplication.SearchModule(this.druidToSerialize);
			if (module == null)
			{
				return;
			}
			StructuredTypeClass typeClass = StructuredTypeClass.None;
			var result = this.designerApplication.DlgResourceSelector(Dialogs.ResourceSelector.Operation.Form, module, ResourceAccess.Type.Forms, ref typeClass, ref druid, ref isNullable, null, typeId);
			if (result != Common.Dialogs.DialogResult.Yes)
			{
				return;
			}

			this.UndoMemorize(Res.Strings.Undo.Action.FieldForm, false);
			field.SubFormId = druid;
			
			this.SetForm(true);
			this.UpdateFieldsTable(false);
			this.UpdateFieldsButtons();
			this.module.AccessForms.SetLocalDirty();
		}

		protected void SelectedFieldsBox()
		{
			//	Groupe/sépare les champs sélectionnés.
			List<int> sels = this.fieldsTable.SelectedRows;
			sels.Sort();

			FormEditor.ObjectModifier.TableItem firstItem = this.formEditor.ObjectModifier.TableContent[sels[0]];
			int first = this.formEditor.ObjectModifier.GetFieldDescriptionIndex(firstItem.Guid);

			if (sels.Count == 1)
			{
				if (firstItem.FieldType == FieldDescription.FieldType.BoxBegin)
				{
					//	Sépare le groupe sélectionné.
					this.UndoMemorize(Res.Strings.Undo.Action.FieldBoxUnlink, false);

					int level = 0;
					for (int i=sels[0]; i<this.formEditor.ObjectModifier.TableContent.Count; i++)
					{
						FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[i];

						if (item.FieldType == FieldDescription.FieldType.BoxBegin)
						{
							level++;
						}

						if (item.FieldType == FieldDescription.FieldType.BoxEnd)
						{
							level--;
							if (level == 0)
							{
								int last = this.formEditor.ObjectModifier.GetFieldDescriptionIndex(item.Guid);
								this.workingForm.Fields.RemoveAt(last);  // enlève le BoxEnd
								this.workingForm.Fields.RemoveAt(first);  // enlève le BoxBegin

								this.SetForm(true);
								this.UpdateFieldsTable(false);

								sels.Clear();
								sels.Add(first);
								this.fieldsTable.SelectedRows = sels;
								this.ReflectSelectionToEditor();

								this.UpdateFieldsButtons();
								this.module.AccessForms.SetLocalDirty();
							}
						}
					}
					return;
				}
			}

			//	Groupe les champs sélectionnés.
			this.UndoMemorize(Res.Strings.Undo.Action.FieldBoxLink, false);

			List<FieldDescription> content = new List<FieldDescription>();
			for (int i=sels.Count-1; i>=0; i--)
			{
				int sel = sels[i];

				FieldDescription field = this.workingForm.Fields[sel];
				this.workingForm.Fields.RemoveAt(sel);
				content.Insert(0, field);
			}

			int index = first;

			FieldDescription box = new FieldDescription(FieldDescription.FieldType.BoxBegin);
			box.BoxFrameState = FrameState.All;
			this.workingForm.Fields.Insert(index++, box);

			foreach (FieldDescription field in content)
			{
				this.workingForm.Fields.Insert(index++, field);
			}

			box = new FieldDescription(FieldDescription.FieldType.BoxEnd);
			this.workingForm.Fields.Insert(index++, box);

			this.SetForm(true);
			this.UpdateFieldsTable(false);

			sels.Clear();
			sels.Add(first);
			this.fieldsTable.SelectedRows = sels;
			this.ReflectSelectionToEditor();

			this.UpdateFieldsButtons();
			this.module.AccessForms.SetLocalDirty();
		}

		protected void SelectedFieldsMove(int direction)
		{
			//	Déplace les champs sélectionnés vers le haut ou vers le bas.
			this.UndoMemorize(Res.Strings.Undo.Action.FieldMove, false);

			List<int> sels = this.fieldsTable.SelectedRows;
			sels.Sort();
			if (direction > 0)
			{
				sels.Reverse();
			}

			if (this.formEditor.ObjectModifier.IsDelta)  // masque delta ?
			{
				List<FormEditor.ObjectModifier.TableItem> items = new List<Epsitec.Common.Designer.FormEditor.ObjectModifier.TableItem>();

				foreach (int sel in sels)
				{
					items.Add(this.formEditor.ObjectModifier.TableContent[sel]);
				}

				foreach (FormEditor.ObjectModifier.TableItem item in items)
				{
					int index = this.formEditor.ObjectModifier.GetFieldDescriptionIndex(item.Guid);
					this.formEditor.ObjectModifier.FormDeltaMove(index, direction);

					this.SetForm(true);
					this.UpdateFieldsTable(false);
				}
			}
			else  // masque normal ?
			{
				foreach (int sel in sels)
				{
					FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sel];
					int index = this.formEditor.ObjectModifier.GetFieldDescriptionIndex(item.Guid);
					if (index != -1)
					{
						FieldDescription field = this.workingForm.Fields[index];
						this.workingForm.Fields.RemoveAt(index);
						this.workingForm.Fields.Insert(index+direction, field);
					}
				}

				this.SetForm(true);
				this.UpdateFieldsTable(false);
			}

			this.ReflectSelectionToList();
			this.UpdateFieldsButtons();
			this.module.AccessForms.SetLocalDirty();
		}

		protected void SelectedFieldsGoto()
		{
			//	Va sur la définition du champ sélectionné.
			List<int> sels = this.fieldsTable.SelectedRows;
			if (sels.Count != 1)
			{
				return;
			}

			FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sels[0]];
			FieldDescription field = this.formEditor.ObjectModifier.GetFieldDescription(item);
			Druid druid = field.FieldIds[field.FieldIds.Count-1];

			Module module = this.designerApplication.SearchModule(druid);
			if (module == null)
			{
				return;
			}

			this.designerApplication.LocatorGoto(module.ModuleId.Name, ResourceAccess.Type.Fields, -1, druid, this.Window.FocusedWidget);
		}

		protected void SelectedRelationsUse()
		{
			//	Utilise la relation sélectionnée.
			int sel = this.relationsTable.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			this.UndoMemorize(Res.Strings.Undo.Action.RelationUse, false);

			FormEditor.ObjectModifier.RelationItem item = this.formEditor.ObjectModifier.TableRelations[sel];
			FieldDescription field;

			if (this.formEditor.ObjectModifier.IsDelta)  // masque delta ?
			{
				FormEditor.ObjectModifier.TableItem ti = this.formEditor.ObjectModifier.TableContent[this.fieldsTable.TotalRows-1];
					
				field = new FieldDescription(FieldDescription.FieldType.Field);
				field.SetFields(item.DruidsPath);
				field.DeltaInserted = true;
				field.DeltaAttachGuid = ti.Guid;
				this.workingForm.Fields.Add(field);
			}
			else  // masque normal ?
			{
#if true
				//	Tous les champs, quels que soient leur type, sont traités comme
				//	tels; c'est en aval, dans FormEngine, que se prennent le bonnes
				//	décisions.
				field = new FieldDescription (FieldDescription.FieldType.Field);
				field.SetFields (item.DruidsPath);
#else
				if (item.Relation == FieldRelation.None)  // champ normal ?
				{
					field = new FieldDescription(FieldDescription.FieldType.Field);
					field.SetFields(item.DruidsPath);
				}
				else  // relation ?
				{
					Druid formId = this.module.AccessForms.FormSearch(item.typeId);

					if (formId.IsEmpty)
					{
						this.designerApplication.DialogError(Res.Strings.Viewers.Forms.Error.RelationUse);
						return;
					}

					field = new FieldDescription(FieldDescription.FieldType.SubForm);
					field.BoxFrameState = FrameState.All;
					field.SetFields(item.DruidsPath);
					field.SubFormId = formId;
				}
#endif

				this.workingForm.Fields.Add(field);
			}

			this.SetForm(true);
			this.UpdateFieldsTable(false);

			List<int> sels = new List<int>();
			sels.Add(this.formEditor.ObjectModifier.GetFieldCount-1);
			this.fieldsTable.SelectedRows = sels;
			this.ReflectSelectionToEditor();

			this.UpdateFieldsButtons();
			this.UpdateRelationsTable(false);
			this.UpdateRelationsButtons();
			this.module.AccessForms.SetLocalDirty();
		}

		protected void SelectedRelationsExpand()
		{
			//	Etend la relation sélectionnée.
			int sel = this.relationsTable.SelectedRow;
			this.formEditor.ObjectModifier.TableRelationExpand(sel);

			this.UpdateRelationsTable(false);
			this.UpdateRelationsButtons();
			this.UpdateFieldsTable(false);
			this.UpdateFieldsButtons();
		}

		protected void SelectedRelationsCompact()
		{
			//	Compacte la relation sélectionnée.
			int sel = this.relationsTable.SelectedRow;
			this.formEditor.ObjectModifier.TableRelationCompact(sel);

			this.UpdateRelationsTable(false);
			this.UpdateRelationsButtons();
			this.UpdateFieldsTable(false);
			this.UpdateFieldsButtons();
		}

		protected void SelectedRelationsAuto()
		{
			//	Etend automatiquement les champs utilisés.
			this.formEditor.ObjectModifier.UpdateTableRelation(this.entityId, this.entityFields, this.workingForm);

			this.UpdateRelationsTable(false);
			this.UpdateRelationsButtons();
			this.UpdateFieldsTable(false);
			this.UpdateFieldsButtons();
		}

		protected void SelectedOtherCommand()
		{
			//	Insère une commande sous forme d'un MetaButton.
			StructuredTypeClass typeClass = StructuredTypeClass.None;
			Druid druid = Druid.Empty;
			bool isNullable = false;
			var result = this.designerApplication.DlgResourceSelector(Epsitec.Common.Designer.Dialogs.ResourceSelector.Operation.Selection, this.module, ResourceAccess.Type.Commands, ref typeClass, ref druid, ref isNullable, null, Druid.Empty);
			if (result != Common.Dialogs.DialogResult.Yes)  // annuler ?
			{
				return;
			}

			this.UndoMemorize(Res.Strings.Undo.Action.CommandInsert, false);

			List<int> sels = this.fieldsTable.SelectedRows;
			sels.Sort();

			FieldDescription field;
			int index;

			if (sels.Count == 0)
			{
				index = this.formEditor.ObjectModifier.GetFieldCount;
			}
			else
			{
				FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sels[0]];
				index = this.formEditor.ObjectModifier.GetFieldDescriptionIndex(item.Guid);
			}

			if (this.formEditor.ObjectModifier.IsDelta)  // masque delta ?
			{
				System.Guid ag = System.Guid.Empty;
				if (sels.Count == 0)
				{
					ag = this.formEditor.ObjectModifier.TableContent[this.fieldsTable.TotalRows-1].Guid;
				}
				else
				{
					if (sels[0] > 0)
					{
						ag = this.formEditor.ObjectModifier.TableContent[sels[0]-1].Guid;
					}
				}

				field = new FieldDescription(FieldDescription.FieldType.Command);
				field.CommandButtonClassValue = FieldDescription.CommandButtonClass.RichDialogButton;
				field.SetField(druid);
				field.DeltaInserted = true;
				field.DeltaAttachGuid = ag;

				this.workingForm.Fields.Add(field);
			}
			else  // masque normal ?
			{
				field = new FieldDescription(FieldDescription.FieldType.Command);
				field.CommandButtonClassValue = FieldDescription.CommandButtonClass.RichDialogButton;
				field.SetField(druid);

				this.workingForm.Fields.Insert(index, field);
			}

			this.SetForm(true);
			this.UpdateFieldsTable(false);

			sels.Clear();
			sels.Add(index);
			this.fieldsTable.SelectedRows = sels;
			this.ReflectSelectionToEditor();

			this.UpdateFieldsButtons();
			this.module.AccessForms.SetLocalDirty();
		}

		protected void SelectedOtherLine()
		{
			//	Insère une ligne avant le champ sélectionné.
			this.UndoMemorize(Res.Strings.Undo.Action.FieldLine, false);

			List<int> sels = this.fieldsTable.SelectedRows;
			sels.Sort();

			FieldDescription field;
			int index;

			if (sels.Count == 0)
			{
				index = this.formEditor.ObjectModifier.GetFieldCount;
			}
			else
			{
				FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sels[0]];
				index = this.formEditor.ObjectModifier.GetFieldDescriptionIndex(item.Guid);
			}

			if (this.formEditor.ObjectModifier.IsDelta)  // masque delta ?
			{
				System.Guid ag = System.Guid.Empty;
				if (sels.Count == 0)
				{
					ag = this.formEditor.ObjectModifier.TableContent[this.fieldsTable.TotalRows-1].Guid;
				}
				else
				{
					if (sels[0] > 0)
					{
						ag = this.formEditor.ObjectModifier.TableContent[sels[0]-1].Guid;
					}
				}

				field = new FieldDescription(FieldDescription.FieldType.Line);
				field.DeltaInserted = true;
				field.DeltaAttachGuid = ag;
				this.workingForm.Fields.Add(field);
			}
			else  // masque normal ?
			{
				field = new FieldDescription(FieldDescription.FieldType.Line);
				this.workingForm.Fields.Insert(index, field);
			}

			this.SetForm(true);
			this.UpdateFieldsTable(false);

			sels.Clear();
			sels.Add(index);
			this.fieldsTable.SelectedRows = sels;
			this.ReflectSelectionToEditor();

			this.UpdateFieldsButtons();
			this.module.AccessForms.SetLocalDirty();
		}

		protected void SelectedOtherTitle()
		{
			//	Insère un titre avant le champ sélectionné.
			this.UndoMemorize(Res.Strings.Undo.Action.FieldTitle, false);

			List<int> sels = this.fieldsTable.SelectedRows;
			sels.Sort();

			FieldDescription field;
			int index;

			if (sels.Count == 0)
			{
				index = this.formEditor.ObjectModifier.GetFieldCount;
			}
			else
			{
				FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sels[0]];
				index = this.formEditor.ObjectModifier.GetFieldDescriptionIndex(item.Guid);
			}

			if (this.formEditor.ObjectModifier.IsDelta)  // masque delta ?
			{
				System.Guid ag = System.Guid.Empty;
				if (sels.Count == 0)
				{
					ag = this.formEditor.ObjectModifier.TableContent[this.fieldsTable.TotalRows-1].Guid;
				}
				else
				{
					if (sels[0] > 0)
					{
						ag = this.formEditor.ObjectModifier.TableContent[sels[0]-1].Guid;
					}
				}

				field = new FieldDescription(FieldDescription.FieldType.Title);
				field.DeltaInserted = true;
				field.DeltaAttachGuid = ag;
				this.workingForm.Fields.Add(field);
			}
			else  // masque normal ?
			{
				field = new FieldDescription(FieldDescription.FieldType.Title);
				this.workingForm.Fields.Insert(index, field);
			}

			this.SetForm(true);
			this.UpdateFieldsTable(false);

			sels.Clear();
			sels.Add(index);
			this.fieldsTable.SelectedRows = sels;
			this.ReflectSelectionToEditor();

			this.UpdateFieldsButtons();
			this.module.AccessForms.SetLocalDirty();
		}

		protected void SelectedOtherGlue()
		{
			//	Insère une "glue" avant le champ sélectionné.
			List<int> sels = this.fieldsTable.SelectedRows;
			if (sels == null || sels.Count != 2)
			{
				return;
			}
			sels.Sort();

			this.UndoMemorize(Res.Strings.Undo.Action.FieldGlue, false);

			FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sels[1]];
			int index = this.formEditor.ObjectModifier.GetFieldDescriptionIndex(item.Guid);
			FieldDescription field;

			if (this.formEditor.ObjectModifier.IsDelta)  // masque delta ?
			{
				System.Guid ag = this.formEditor.ObjectModifier.TableContent[sels[0]].Guid;

				field = new FieldDescription(FieldDescription.FieldType.Glue);
				field.ColumnsRequired = 0;
				field.DeltaInserted = true;
				field.DeltaAttachGuid = ag;
				this.workingForm.Fields.Add(field);
			}
			else  // masque normal ?
			{
				field = new FieldDescription(FieldDescription.FieldType.Glue);
				field.ColumnsRequired = 0;
				this.workingForm.Fields.Insert(index, field);
			}

			this.SetForm(true);
			this.UpdateFieldsTable(false);

			sels.Clear();
			sels.Add(index);
			this.fieldsTable.SelectedRows = sels;
			this.ReflectSelectionToEditor();

			this.UpdateFieldsButtons();
			this.module.AccessForms.SetLocalDirty();
		}


		public void ChangeForwardTab(System.Guid fieldGuid, System.Guid forwardTabGuid)
		{
			//	Modifie l'élément suivant pour la navigation avec Tab.
			this.UndoMemorize(Res.Strings.Undo.Action.ForwardTab, false);

			FieldDescription field = this.formEditor.ObjectModifier.GetFieldDescription(fieldGuid);
			int index = FormEngine.Arrange.IndexOfGuid(this.formEditor.WorkingForm.Fields, fieldGuid);

			if (this.formEditor.ObjectModifier.IsDelta)  // masque delta ?
			{
				if (index == -1)
				{
					if (forwardTabGuid != System.Guid.Empty)
					{
						FieldDescription copy = new FieldDescription(field);
						copy.DeltaForwardTab = true;
						copy.ForwardTabGuid = forwardTabGuid;

						this.workingForm.Fields.Add(copy);  // met l'élément à la fin de la liste delta
					}
				}
				else
				{
					FieldDescription actual = this.workingForm.Fields[index];

					actual.DeltaForwardTab = (forwardTabGuid != System.Guid.Empty);
					actual.ForwardTabGuid = forwardTabGuid;

					if (!actual.Delta)
					{
						this.workingForm.Fields.RemoveAt(index);  // supprime l'élément dans la liste delta
					}
				}
			}
			else  // masque normal ?
			{
				field.ForwardTabGuid = forwardTabGuid;
			}

			this.SetForm(true);
			this.UpdateFieldsTable(false);
			this.ReflectSelectionToList();
			this.UpdateFieldsButtons();
			this.module.AccessForms.SetLocalDirty();
		}


		protected void DefineProxies()
		{
			//	Crée les proxies et l'interface utilisateur pour les widgets sélectionnés.
			this.propertiesScrollable.Viewport.Children.Clear();  // supprime les panneaux existants
			this.proxyManager.CreateInterface(this.propertiesScrollable.Viewport, this.formEditor.SelectedObjects);
		}


		protected VMenu CreateFieldsMenu()
		{
			//	Crée le petit menu contextuel associé au bouton "v" de la liste des champs.
			VMenu menu = new VMenu();
			MenuItem item;

			item = new MenuItem("FormFieldsShowPrefix", Misc.GetMenuIconCheckState(Forms.showPrefix), Res.Strings.Viewers.Forms.Menu.ShowPrefix, "", "FormFieldsShowPrefix");
			menu.Items.Add(item);

#if true
			item = new MenuItem("FormFieldsShowGuid", Misc.GetMenuIconCheckState(Forms.showGuid), Res.Strings.Viewers.Forms.Menu.ShowGuid, "", "FormFieldsShowGuid");
			menu.Items.Add(item);
#endif

			item = new MenuItem("FormFieldsShowColumn1", Misc.GetMenuIconCheckState(Forms.showColumn1), Res.Strings.Viewers.Forms.Menu.ShowColumn1, "", "FormFieldsShowColumn1");
			menu.Items.Add(item);

			item = new MenuItem("FormFieldsShowColumn2", Misc.GetMenuIconCheckState(Forms.showColumn2), Res.Strings.Viewers.Forms.Menu.ShowColumn2, "", "FormFieldsShowColumn2");
			menu.Items.Add(item);

			if (this.formEditor.ObjectModifier.IsDelta && !this.designerApplication.IsReadonly)
			{
				menu.Items.Add(new MenuSeparator());

				item = new MenuItem("FormFieldsClearDelta", Misc.Icon("Delete"), Res.Strings.Viewers.Forms.Menu.ClearDelta, "", "FormFieldsClearDelta");
				menu.Items.Add(item);
			}

			return menu;
		}


		private void HandleSplitterDragged(object sender)
		{
			//	Un splitter a été bougé.
			if (sender == this.splitter2)
			{
				Forms.rightPanelWidth = this.right.ActualWidth;
			}

			if (sender == this.splitter3)
			{
				Forms.bottomPanelHeight = this.tabBookSecondary.ActualHeight;
			}
		}

		private void HandleFormEditorChildrenAdded(object sender)
		{
			this.UpdateCommands();
		}

		private void HandleFormEditorChildrenSelected(object sender)
		{
			this.UpdateCommands();
			this.DefineProxies();
		}

		private void HandleFormEditorUpdateCommands(object sender)
		{
			this.UpdateCommands();
			this.UpdateFieldsButtons();
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			AbstractButton button = sender as AbstractButton;

			if (sender == this.fieldsButtonRemove)
			{
				this.SelectedFieldsRemove();
			}

			if (sender == this.fieldsButtonReset)
			{
				this.SelectedFieldsReset();
			}

			if (sender == this.fieldsButtonBox)
			{
				this.SelectedFieldsBox();
			}

			if (sender == this.fieldsButtonForm)
			{
				this.SelectedFieldsForm();
			}

			if (sender == this.fieldsButtonPrev)
			{
				this.SelectedFieldsMove(-1);
			}

			if (sender == this.fieldsButtonNext)
			{
				this.SelectedFieldsMove(1);
			}

			if (sender == this.fieldsButtonGoto)
			{
				this.SelectedFieldsGoto();
			}

			if (sender == this.fieldsButtonMenu)
			{
				VMenu menu = this.CreateFieldsMenu();
				menu.Host = button.Window;
				TextFieldCombo.AdjustComboSize(button, menu, false);
				menu.ShowAsComboList(button, Point.Zero, button);
			}

			if (sender == this.relationsButtonUse)
			{
				this.SelectedRelationsUse();
			}

			if (sender == this.relationsButtonExpand)
			{
				this.SelectedRelationsExpand();
			}

			if (sender == this.relationsButtonCompact)
			{
				this.SelectedRelationsCompact();
			}

			if (sender == this.relationsButtonAuto)
			{
				this.SelectedRelationsAuto();
			}

			if (sender == this.otherButtonCommand)
			{
				this.SelectedOtherCommand();
			}

			if (sender == this.otherButtonLine)
			{
				this.SelectedOtherLine();
			}

			if (sender == this.otherButtonTitle)
			{
				this.SelectedOtherTitle();
			}

			if (sender == this.otherButtonGlue)
			{
				this.SelectedOtherGlue();
			}
		}

		private void HandleRelationsTableCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateRelationsTable(false);
		}

		private void HandleRelationsTableCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a changé.
			this.UpdateRelationsTable(false);
		}

		private void HandleRelationsTableSelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			if (this.ignoreChange)
			{
				return;
			}

			this.UpdateRelationsButtons();
		}

		private void HandleRelationsTableSelectedRowDoubleClicked(object sender)
		{
			//	La ligne sélectionnée a été double-cliquée.
			int sel = this.relationsTable.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			if (this.formEditor.ObjectModifier.IsTableRelationExpandable(sel))
			{
				this.SelectedRelationsExpand();
			}
			else if (this.formEditor.ObjectModifier.IsTableRelationCompactable(sel))
			{
				this.SelectedRelationsCompact();
			}
			else if (this.formEditor.ObjectModifier.IsTableRelationUseable(sel))
			{
				this.SelectedRelationsUse();
			}
		}

		private void HandleFieldTableCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateFieldsTable(false);
		}

		private void HandleFieldTableCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a changé.
			this.UpdateFieldsTable(false);
		}

		private void HandleFieldTableSelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			if (this.ignoreChange)
			{
				return;
			}

			this.ReflectSelectionToEditor();
			this.UpdateFieldsButtons();
		}


		protected static double					rightPanelWidth = 240;
		protected static double					bottomPanelHeight = 200;

		private static double[]					columnWidthHorizontal = { 200, 80, 50, 100, 50 };
		private static double[]					columnWidthVertical = { 250, 80, 50, 100, 50 };

		protected static FormDescription		softWorkingForm = null;
		protected static List<FieldDescription>	softBaseFields  = null;
		protected static List<FieldDescription>	softFinalFields = null;
		protected static Druid					softEntityId    = Druid.Empty;

		protected static bool					showPrefix = false;
		protected static bool					showGuid = false;
		protected static bool					showColumn1 = true;
		protected static bool					showColumn2 = true;

		protected FormEngine.Engine				engine;
		protected Proxies.ProxyManager			proxyManager;
		protected VSplitter						splitter2;
		protected Widget						middle;
		protected Scrollable					drawingScrollable;
		protected FrameBox						panelContainerParent;
		protected UI.Panel						panelContainer;
		protected FormDescription				workingForm;
		protected List<FieldDescription>		baseFields;
		protected List<FieldDescription>		finalFields;
		protected Druid							entityId;
		protected IList<StructuredData>			entityFields;
		protected FormEditor.Editor				formEditor;
		protected FrameBox						right;
		protected HSplitter						splitter3;

		protected TabPage						tabPageFields;
		protected HToolBar						fieldsToolbar;
		protected IconButton					fieldsButtonRemove;
		protected IconButton					fieldsButtonReset;
		protected IconButton					fieldsButtonBox;
		protected IconButton					fieldsButtonForm;
		protected IconButton					fieldsButtonPrev;
		protected IconButton					fieldsButtonNext;
		protected IconButton					fieldsButtonGoto;
		protected GlyphButton					fieldsButtonMenu;
		protected MyWidgets.StringArray			fieldsTable;

		protected TabBook						tabBookSecondary;

		protected TabPage						tabPageSource;
		protected HToolBar						relationsToolbar;
		protected IconButton					relationsButtonUse;
		protected IconButton					relationsButtonExpand;
		protected IconButton					relationsButtonCompact;
		protected IconButton					relationsButtonAuto;
		protected MyWidgets.StringArray			relationsTable;

		protected TabPage						tabPageProperties;
		protected HToolBar						otherToolbar;
		protected IconButton					otherButtonCommand;
		protected IconButton					otherButtonLine;
		protected IconButton					otherButtonTitle;
		protected IconButton					otherButtonGlue;
		protected Scrollable					propertiesScrollable;

		protected TabPage						tabPageMisc;
		protected CheckButton					miscWidthButton;
		protected TextFieldUpDown				miscWidthField;
		protected CheckButton					miscHeightButton;
		protected TextFieldUpDown				miscHeightField;

		protected TabPage						tabPageCultures;
		protected List<IconButton>				cultureButtonList;

		protected UI.PanelMode					panelMode = UI.PanelMode.Default;
		protected Druid							druidToSerialize;

		protected Undo.Engine					undoEngine;
	}
}
