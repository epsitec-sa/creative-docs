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
		public Forms(Module module, PanelsContext context, ResourceAccess access, DesignerApplication designerApplication) : base(module, context, access, designerApplication)
		{
			this.engine = new FormEngine.Engine(this.module.FormResourceProvider);

			this.scrollable.Visibility = false;

			FrameBox surface = new FrameBox(this.lastGroup);
			surface.DrawFullFrame = true;
			surface.Margins = new Margins(0, 0, 5, 0);
			surface.Dock = DockStyle.Fill;

			//	Crée le groupe central.
			this.middle = new FrameBox(surface);
			this.middle.Padding = new Margins(5, 5, 5, 5);
			this.middle.Dock = DockStyle.Fill;

			FrameBox drawing = new FrameBox(this.middle);  // conteneur pour scrollable
			drawing.Dock = DockStyle.Fill;

			this.drawingScrollable = new Scrollable(drawing);
			this.drawingScrollable.MinWidth = 100;
			this.drawingScrollable.MinHeight = 100;
			this.drawingScrollable.Margins = new Margins(1, 1, 1, 1);
			this.drawingScrollable.Dock = DockStyle.Fill;
			this.drawingScrollable.HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.drawingScrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.drawingScrollable.Viewport.IsAutoFitting = true;
			this.drawingScrollable.PaintForegroundFrame = true;

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
			this.formEditor.Initialize(this.module, this.context, this.panelContainer);
			this.formEditor.MinWidth = 100;
			this.formEditor.MinHeight = 100;
			this.formEditor.Anchor = AnchorStyles.All;
			this.formEditor.ChildrenAdded += new EventHandler(this.HandleFormEditorChildrenAdded);
			this.formEditor.ChildrenSelected += new EventHandler(this.HandleFormEditorChildrenSelected);
			this.formEditor.ChildrenGeometryChanged += new EventHandler(this.HandleFormEditorChildrenGeometryChanged);
			this.formEditor.UpdateCommands += new EventHandler(this.HandleFormEditorUpdateCommands);

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
			this.fieldsButtonRemove.Clicked += new MessageEventHandler(this.HandleFieldsButtonClicked);
			this.fieldsToolbar.Items.Add(this.fieldsButtonRemove);

			this.fieldsButtonReset = new IconButton();
			this.fieldsButtonReset.AutoFocus = false;
			this.fieldsButtonReset.CaptionId = Res.Captions.Editor.Forms.Reset.Id;
			this.fieldsButtonReset.Clicked += new MessageEventHandler(this.HandleFieldsButtonClicked);
			this.fieldsToolbar.Items.Add(this.fieldsButtonReset);

			this.fieldsToolbar.Items.Add(new IconSeparator());

			this.fieldsButtonGlue = new IconButton();
			this.fieldsButtonGlue.AutoFocus = false;
			this.fieldsButtonGlue.CaptionId = Res.Captions.Editor.Forms.Glue.Id;
			this.fieldsButtonGlue.Clicked += new MessageEventHandler(this.HandleFieldsButtonClicked);
			this.fieldsToolbar.Items.Add(this.fieldsButtonGlue);

			this.fieldsButtonLine = new IconButton();
			this.fieldsButtonLine.AutoFocus = false;
			this.fieldsButtonLine.CaptionId = Res.Captions.Editor.Forms.Line.Id;
			this.fieldsButtonLine.Clicked += new MessageEventHandler(this.HandleFieldsButtonClicked);
			this.fieldsToolbar.Items.Add(this.fieldsButtonLine);

			this.fieldsButtonTitle = new IconButton();
			this.fieldsButtonTitle.AutoFocus = false;
			this.fieldsButtonTitle.CaptionId = Res.Captions.Editor.Forms.Title.Id;
			this.fieldsButtonTitle.Clicked += new MessageEventHandler(this.HandleFieldsButtonClicked);
			this.fieldsToolbar.Items.Add(this.fieldsButtonTitle);

			this.fieldsButtonBox = new IconButton();
			this.fieldsButtonBox.AutoFocus = false;
			this.fieldsButtonBox.CaptionId = Res.Captions.Editor.Forms.Box.Id;
			this.fieldsButtonBox.Clicked += new MessageEventHandler(this.HandleFieldsButtonClicked);
			this.fieldsToolbar.Items.Add(this.fieldsButtonBox);

			this.fieldsButtonForm = new IconButton();
			this.fieldsButtonForm.AutoFocus = false;
			this.fieldsButtonForm.CaptionId = Res.Captions.Editor.Forms.Form.Id;
			this.fieldsButtonForm.Clicked += new MessageEventHandler(this.HandleFieldsButtonClicked);
			this.fieldsToolbar.Items.Add(this.fieldsButtonForm);

			this.fieldsToolbar.Items.Add(new IconSeparator());

			this.fieldsButtonPrev = new IconButton();
			this.fieldsButtonPrev.AutoFocus = false;
			this.fieldsButtonPrev.CaptionId = Res.Captions.Editor.Forms.Prev.Id;
			this.fieldsButtonPrev.Clicked += new MessageEventHandler(this.HandleFieldsButtonClicked);
			this.fieldsToolbar.Items.Add(this.fieldsButtonPrev);

			this.fieldsButtonNext = new IconButton();
			this.fieldsButtonNext.AutoFocus = false;
			this.fieldsButtonNext.CaptionId = Res.Captions.Editor.Forms.Next.Id;
			this.fieldsButtonNext.Clicked += new MessageEventHandler(this.HandleFieldsButtonClicked);
			this.fieldsToolbar.Items.Add(this.fieldsButtonNext);

			this.fieldsToolbar.Items.Add(new IconSeparator());

			this.fieldsButtonGoto = new IconButton();
			this.fieldsButtonGoto.AutoFocus = false;
			this.fieldsButtonGoto.CaptionId = Res.Captions.Editor.LocatorGoto.Id;
			this.fieldsButtonGoto.Clicked += new MessageEventHandler(this.HandleFieldsButtonClicked);
			this.fieldsToolbar.Items.Add(this.fieldsButtonGoto);

			this.fieldsButtonMenu = new GlyphButton(topToolBar);
			this.fieldsButtonMenu.GlyphShape = GlyphShape.Menu;
			this.fieldsButtonMenu.ButtonStyle = ButtonStyle.ToolItem;
			this.fieldsButtonMenu.AutoFocus = false;
			this.fieldsButtonMenu.Clicked += new MessageEventHandler(this.HandleFieldsButtonClicked);
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
			this.fieldsTable.CellCountChanged += new EventHandler(this.HandleFieldTableCellCountChanged);
			this.fieldsTable.CellsContentChanged += new EventHandler(this.HandleFieldTableCellsContentChanged);
			this.fieldsTable.SelectedRowChanged += new EventHandler(this.HandleFieldTableSelectedRowChanged);
			this.UpdateFieldsTableColumns();

			//	Crée l'onglet 'relations'.
			this.tabPageRelations = new TabPage();
			this.tabPageRelations.TabTitle = Res.Strings.Viewers.Panels.TabRelations;
			this.tabPageRelations.Padding = new Margins(4, 4, 4, 4);
			this.tabBookSecondary.Items.Add(this.tabPageRelations);

			this.relationsToolbar = new HToolBar(this.tabPageRelations);
			this.relationsToolbar.Dock = DockStyle.Top;
			this.relationsToolbar.Margins = new Margins(0, 0, 0, 5);

			this.relationsButtonUse = new IconButton();
			this.relationsButtonUse.AutoFocus = false;
			this.relationsButtonUse.CaptionId = Res.Captions.Editor.Forms.Use.Id;
			this.relationsButtonUse.Clicked += new MessageEventHandler(this.HandleRelationsButtonClicked);
			this.relationsToolbar.Items.Add(this.relationsButtonUse);

			this.relationsToolbar.Items.Add(new IconSeparator());

			this.relationsButtonExpand = new IconButton();
			this.relationsButtonExpand.AutoFocus = false;
			this.relationsButtonExpand.CaptionId = Res.Captions.Editor.Forms.Expand.Id;
			this.relationsButtonExpand.Clicked += new MessageEventHandler(this.HandleRelationsButtonClicked);
			this.relationsToolbar.Items.Add(this.relationsButtonExpand);

			this.relationsButtonCompact = new IconButton();
			this.relationsButtonCompact.AutoFocus = false;
			this.relationsButtonCompact.CaptionId = Res.Captions.Editor.Forms.Compact.Id;
			this.relationsButtonCompact.Clicked += new MessageEventHandler(this.HandleRelationsButtonClicked);
			this.relationsToolbar.Items.Add(this.relationsButtonCompact);

			this.relationsToolbar.Items.Add(new IconSeparator());

			this.relationsButtonAuto = new IconButton();
			this.relationsButtonAuto.AutoFocus = false;
			this.relationsButtonAuto.CaptionId = Res.Captions.Editor.Forms.Auto.Id;
			this.relationsButtonAuto.Clicked += new MessageEventHandler(this.HandleRelationsButtonClicked);
			this.relationsToolbar.Items.Add(this.relationsButtonAuto);

			this.relationsTable = new MyWidgets.StringArray(this.tabPageRelations);
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
			this.relationsTable.CellCountChanged += new EventHandler(this.HandleRelationsTableCellCountChanged);
			this.relationsTable.CellsContentChanged += new EventHandler(this.HandleRelationsTableCellsContentChanged);
			this.relationsTable.SelectedRowChanged += new EventHandler(this.HandleRelationsTableSelectedRowChanged);
			this.relationsTable.SelectedRowDoubleClicked += new EventHandler(this.HandleRelationsTableSelectedRowDoubleClicked);

			//	Crée l'onglet 'propriétés'.
			this.tabPageProperties = new TabPage();
			this.tabPageProperties.TabTitle = Res.Strings.Viewers.Panels.TabProperties;
			this.tabPageProperties.Padding = new Margins(4, 4, 4, 4);
			this.tabBookSecondary.Items.Add(this.tabPageProperties);

			this.proxyManager = new FormEditor.ProxyManager(this);

			this.propertiesScrollable = new Scrollable(this.tabPageProperties);
			this.propertiesScrollable.Dock = DockStyle.Fill;
			this.propertiesScrollable.HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.propertiesScrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.propertiesScrollable.Viewport.IsAutoFitting = true;
			this.propertiesScrollable.Viewport.Margins = new Margins(10, 10, 10, 10);
			this.propertiesScrollable.PaintForegroundFrame = true;

			//	Crée l'onglet 'cultures'.
			this.tabPageCultures = new TabPage();
			this.tabPageCultures.TabTitle = Res.Strings.Viewers.Panels.TabCultures;
			this.tabPageCultures.Padding = new Margins(10, 10, 10, 10);
			this.tabBookSecondary.Items.Add(this.tabPageCultures);

			this.CreateCultureButtons();

			this.tabBookSecondary.ActivePage = this.tabPageRelations;

			this.splitter2 = new VSplitter(surface);
			this.splitter2.Dock = DockStyle.Right;
			this.splitter2.Margins = new Margins(0, 0, 1, 1);
			this.splitter2.SplitterDragged += new EventHandler(this.HandleSplitterDragged);

			this.splitter3 = new HSplitter(this.right);
			this.splitter3.Dock = DockStyle.Bottom;
			this.splitter3.Margins = new Margins(0, 1, 0, 0);
			this.splitter3.SplitterDragged += new EventHandler(this.HandleSplitterDragged);

			this.UpdateAll();
			this.UpdateViewer(Viewers.Changing.Show);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.fieldsButtonRemove.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonReset.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonGlue.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonLine.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonTitle.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonBox.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonForm.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonPrev.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonNext.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonGoto.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonMenu.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);

				this.fieldsTable.CellCountChanged -= new EventHandler(this.HandleFieldTableCellCountChanged);
				this.fieldsTable.CellsContentChanged -= new EventHandler(this.HandleFieldTableCellsContentChanged);
				this.fieldsTable.SelectedRowChanged -= new EventHandler(this.HandleFieldTableSelectedRowChanged);

				this.relationsButtonUse.Clicked -= new MessageEventHandler(this.HandleRelationsButtonClicked);
				this.relationsButtonExpand.Clicked -= new MessageEventHandler(this.HandleRelationsButtonClicked);
				this.relationsButtonCompact.Clicked -= new MessageEventHandler(this.HandleRelationsButtonClicked);
				this.relationsButtonAuto.Clicked -= new MessageEventHandler(this.HandleRelationsButtonClicked);

				this.relationsTable.CellCountChanged -= new EventHandler(this.HandleRelationsTableCellCountChanged);
				this.relationsTable.CellsContentChanged -= new EventHandler(this.HandleRelationsTableCellsContentChanged);
				this.relationsTable.SelectedRowChanged -= new EventHandler(this.HandleRelationsTableSelectedRowChanged);
				this.relationsTable.SelectedRowDoubleClicked -= new EventHandler(this.HandleRelationsTableSelectedRowDoubleClicked);

				this.splitter2.SplitterDragged -= new EventHandler(this.HandleSplitterDragged);
				this.splitter3.SplitterDragged -= new EventHandler(this.HandleSplitterDragged);
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


		public FormEditor.Editor FormEditor
		{
			get
			{
				return this.formEditor;
			}
		}


		protected override void InitializeTable()
		{
			//	Initialise la table.
			StructuredType cultureMapType = new StructuredType();
			cultureMapType.Fields.Add("Name", StringType.Default);
			cultureMapType.Fields.Add("Druid", StringType.Default);
			cultureMapType.Fields.Add("Local", StringType.Default);
			cultureMapType.Fields.Add("Identity", StringType.Default);

			this.table.SourceType = cultureMapType;

			this.table.Columns.Add(new UI.ItemTableColumn("Name", new Widgets.Layouts.GridLength(this.GetColumnWidth(0), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Druid", new Widgets.Layouts.GridLength(this.GetColumnWidth(1), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Local", new Widgets.Layouts.GridLength(this.GetColumnWidth(2), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Identity", new Widgets.Layouts.GridLength(this.GetColumnWidth(3), Widgets.Layouts.GridUnitType.Proportional)));

			this.table.ColumnHeader.SetColumnComparer(1, this.CompareDruid);
			this.table.ColumnHeader.SetColumnComparer(2, this.CompareLocal);
			this.table.ColumnHeader.SetColumnComparer(3, this.CompareIdentity);

			this.table.ColumnHeader.SetColumnText(0, Res.Strings.Viewers.Column.Name);
			this.table.ColumnHeader.SetColumnText(1, Res.Strings.Viewers.Column.Druid);
			this.table.ColumnHeader.SetColumnText(2, Res.Strings.Viewers.Column.Local);
			this.table.ColumnHeader.SetColumnText(3, Res.Strings.Viewers.Column.Identity);

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
			if (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Horizontal)
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
			if (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Horizontal)
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
			bool isDelta = this.formEditor.ObjectModifier.IsDelta;

			if (!this.designerApplication.IsReadonly)
			{
				List<int> sels = this.fieldsTable.SelectedRows;

				if (sels != null && sels.Count > 0)
				{
					isSel = true;
					isPrev = true;
					isNext = true;
					isGoto = (sels.Count == 1);

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
					}
				}

				if (sels != null && sels.Count == 1)
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
					}
				}
			}

			this.fieldsButtonRemove.Enable = isSel;
			this.fieldsButtonReset.Enable = isSel;
			this.fieldsButtonGlue.Enable = isSel;
			this.fieldsButtonLine.Enable = isSel;
			this.fieldsButtonTitle.Enable = isSel;
			this.fieldsButtonBox.Enable = isSel && !isDelta;

			this.fieldsButtonForm.Enable = isForm && !isDelta;
			this.fieldsButtonPrev.Enable = isPrev;
			this.fieldsButtonNext.Enable = isNext;
			this.fieldsButtonGoto.Enable = isGoto;

			this.fieldsButtonRemove.IconName = isDelta ? Misc.Icon("FormDeltaHide") : Misc.Icon("Delete");
			this.fieldsButtonBox.IconName = isUnbox ? Misc.Icon("FormUnbox") : Misc.Icon("FormBox");
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


		public override bool Terminate(bool soft)
		{
			//	Termine le travail sur une ressource, avant de passer à une autre.
			//	Si soft = true, on sérialise temporairement sans poser de question.
			//	Retourne false si l'utilisateur a choisi "annuler".
			
			base.Terminate(soft);

			if (this.module.AccessForms.IsLocalDirty)
			{
				System.Diagnostics.Debug.Assert (soft);
				
				if (this.druidToSerialize.IsValid)
				{
					//?Forms.softSerialize = this.FormToXml(this.GetForm());
				}
				else
				{
					Forms.softSerialize = null;
				}

				Forms.softDirtySerialization = this.module.AccessForms.IsLocalDirty;
			}

			return true;
		}

		protected override void PersistChanges()
		{
			//	Stocke la version XML (sérialisée) du masque de saisie dans l'accesseur
			//	s'il y a eu des modifications.
			//?this.GetForm();
			this.access.SetForm(this.druidToSerialize, this.workingForm);
			base.PersistChanges();
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

			if (Forms.softSerialize == null)
			{
				//?FormDescription form = this.access.GetForm(this.druidToSerialize);
				//?this.SetForm(form, this.druidToSerialize, false);
				this.access.GetForm(this.druidToSerialize, out this.workingForm, out this.baseFields, out this.finalFields, out this.entityId);
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

				FormDescription form = this.XmlToForm(Forms.softSerialize);
				//?this.SetForm(form, this.druidToSerialize, false);

				Forms.softDirtySerialization = false;
				Forms.softSerialize = null;
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
					this.DefineProxies(this.formEditor.SelectedObjects);  // met à jour les proxies
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
			this.UpdateCommands();
		}


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
				button.Clicked += new MessageEventHandler(this.HandleCultureButtonClicked);
				ToolTip.Default.SetToolTip(button, Misc.CultureLongName(culture));

				this.cultureButtonList.Add(button);
			}

			this.UpdateCultureButtons();
		}

		void HandleCultureButtonClicked(object sender, MessageEventArgs e)
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
			List<int> sels = this.fieldsTable.SelectedRows;
			sels.Sort();

			List<System.Guid> guids = new List<System.Guid>();

			if (this.formEditor.ObjectModifier.IsDelta)  // masque delta ?
			{
				foreach (int sel in sels)
				{
					FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sel];
					FieldDescription field = this.formEditor.ObjectModifier.GetFieldDescription(item);

					int index = FormEngine.Arrange.IndexOfGuid(this.workingForm.Fields, item.Guid);
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

						if (!actual.DeltaMoved && !actual.DeltaModified && !actual.DeltaHidden && !actual.DeltaShowed && !actual.DeltaBrokenAttach)
						{
							this.workingForm.Fields.RemoveAt(index);
						}
					}

					guids.Add(item.Guid);
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
						this.workingForm.Fields.RemoveAt(index);
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

		protected void SelectedFieldsGlue()
		{
			//	Insère une "glue" avant le champ sélectionné.
			List<int> sels = this.fieldsTable.SelectedRows;
			sels.Sort();

			FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sels[0]];
			int index = this.formEditor.ObjectModifier.GetFieldDescriptionIndex(item.Guid);
			FieldDescription field;

			if (this.formEditor.ObjectModifier.IsDelta)  // masque delta ?
			{
				System.Guid ag = System.Guid.Empty;
				if (sels[0] > 0)
				{
					ag = this.formEditor.ObjectModifier.TableContent[sels[0]-1].Guid;
				}

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

		protected void SelectedFieldsLine()
		{
			//	Insère une ligne avant le champ sélectionné.
			List<int> sels = this.fieldsTable.SelectedRows;
			sels.Sort();

			FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sels[0]];
			int index = this.formEditor.ObjectModifier.GetFieldDescriptionIndex(item.Guid);
			FieldDescription field;

			if (this.formEditor.ObjectModifier.IsDelta)  // masque delta ?
			{
				System.Guid ag = System.Guid.Empty;
				if (sels[0] > 0)
				{
					ag = this.formEditor.ObjectModifier.TableContent[sels[0]-1].Guid;
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

		protected void SelectedFieldsTitle()
		{
			//	Insère un titre avant le champ sélectionné.
			List<int> sels = this.fieldsTable.SelectedRows;
			sels.Sort();

			FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sels[0]];
			int index = this.formEditor.ObjectModifier.GetFieldDescriptionIndex(item.Guid);
			FieldDescription field;

			if (this.formEditor.ObjectModifier.IsDelta)  // masque delta ?
			{
				System.Guid ag = System.Guid.Empty;
				if (sels[0] > 0)
				{
					ag = this.formEditor.ObjectModifier.TableContent[sels[0]-1].Guid;
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
			bool isPrivateRelation = false;
			Module module = this.designerApplication.SearchModule(this.druidToSerialize);
			StructuredTypeClass typeClass = StructuredTypeClass.None;
			Common.Dialogs.DialogResult result = this.designerApplication.DlgResourceSelector(Dialogs.ResourceSelector.Operation.Form, module, ResourceAccess.Type.Forms, ref typeClass, ref druid, ref isNullable, ref isPrivateRelation, null, typeId);
			if (result != Common.Dialogs.DialogResult.Yes)
			{
				return;
			}

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
						this.designerApplication.DialogError("Il n'existe aucun masque pour cette relation.");
						return;
					}

					field = new FieldDescription(FieldDescription.FieldType.SubForm);
					field.BoxFrameState = FrameState.All;
					field.SetFields(item.DruidsPath);
					field.SubFormId = formId;
				}

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


		#region Proxies
		protected void DefineProxies(IEnumerable<Widget> widgets)
		{
			//	Crée les proxies et l'interface utilisateur pour les widgets sélectionnés.
			this.ClearProxies();
			this.proxyManager.SetSelection(widgets);
			this.proxyManager.CreateUserInterface(this.propertiesScrollable.Viewport);
		}

		protected void ClearProxies()
		{
			//	Supprime l'interface utilisateur pour les widgets sélectionnés.
			this.proxyManager.ClearUserInterface(this.propertiesScrollable.Viewport);
		}

		protected void UpdateProxies()
		{
			//	Met à jour les proxies et l'interface utilisateur (panneaux), sans changer
			//	le nombre de propriétés visibles par panneau.
			this.proxyManager.UpdateUserInterface();
			this.module.AccessForms.SetLocalDirty();
		}

		public void RegenerateProxies()
		{
			//	Régénère la liste des proxies et met à jour les panneaux de l'interface
			//	utilisateur s'il y a eu un changement dans le nombre de propriétés visibles
			//	par panneau.
			if (this.proxyManager.RegenerateProxies())
			{
				this.ClearProxies();
				this.proxyManager.CreateUserInterface(this.propertiesScrollable.Viewport);
			}
			this.module.AccessForms.SetLocalDirty();
		}

		public void RegenerateProxiesAndForm()
		{
			//	Régénère les proxies et le masque de saisie.
			if (this.proxyManager.RegenerateProxies())
			{
				this.ClearProxies();
				this.proxyManager.CreateUserInterface(this.propertiesScrollable.Viewport);
			}

			this.formEditor.RegenerateForm();
			this.module.AccessForms.SetLocalDirty();
		}
		#endregion


		protected VMenu CreateFieldsMenu()
		{
			//	Crée le petit menu contextuel associé au bouton "v" de la liste des champs.
			VMenu menu = new VMenu();
			MenuItem item;

			item = new MenuItem("FormFieldsShowPrefix", Misc.GetMenuIconCheckState(Forms.showPrefix), Res.Strings.Viewers.Forms.Menu.ShowPrefix, "", "FormFieldsShowPrefix");
			menu.Items.Add(item);

#if true
			item = new MenuItem("FormFieldsShowGuid", Misc.GetMenuIconCheckState(Forms.showGuid), "Afficher les Guids (debug)", "", "FormFieldsShowGuid");
			menu.Items.Add(item);
#endif

			item = new MenuItem("FormFieldsShowColumn1", Misc.GetMenuIconCheckState(Forms.showColumn1), "Afficher la deuxième colonne", "", "FormFieldsShowColumn1");
			menu.Items.Add(item);

			item = new MenuItem("FormFieldsShowColumn2", Misc.GetMenuIconCheckState(Forms.showColumn2), "Afficher la troisième colonne", "", "FormFieldsShowColumn2");
			menu.Items.Add(item);

			if (this.formEditor.ObjectModifier.IsDelta && !this.designerApplication.IsReadonly)
			{
				menu.Items.Add(new MenuSeparator());

				item = new MenuItem("FormFieldsClearDelta", Misc.Icon("Delete"), "Effacer tout le masque correctif", "", "FormFieldsClearDelta");
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
			this.DefineProxies(this.formEditor.SelectedObjects);
		}

		private void HandleFormEditorChildrenGeometryChanged(object sender)
		{
			this.UpdateProxies();
		}

		private void HandleFormEditorUpdateCommands(object sender)
		{
			this.UpdateCommands();
			this.UpdateFieldsButtons();
		}

		private void HandleFieldsButtonClicked(object sender, MessageEventArgs e)
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

			if (sender == this.fieldsButtonGlue)
			{
				this.SelectedFieldsGlue();
			}

			if (sender == this.fieldsButtonLine)
			{
				this.SelectedFieldsLine();
			}

			if (sender == this.fieldsButtonTitle)
			{
				this.SelectedFieldsTitle();
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
		}

		private void HandleRelationsButtonClicked(object sender, MessageEventArgs e)
		{
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


		protected static double					rightPanelWidth = 280;
		protected static double					bottomPanelHeight = 200;

		private static double[]					columnWidthHorizontal = { 200, 80, 50, 100 };
		private static double[]					columnWidthVertical = { 250, 80, 50, 100 };

		protected static string					softSerialize = null;
		protected static bool					softDirtySerialization = false;
		protected static bool					showPrefix = false;
		protected static bool					showGuid = false;
		protected static bool					showColumn1 = true;
		protected static bool					showColumn2 = true;

		protected FormEngine.Engine				engine;
		protected FormEditor.ProxyManager		proxyManager;
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
		protected IconButton					fieldsButtonGlue;
		protected IconButton					fieldsButtonLine;
		protected IconButton					fieldsButtonTitle;
		protected IconButton					fieldsButtonBox;
		protected IconButton					fieldsButtonForm;
		protected IconButton					fieldsButtonPrev;
		protected IconButton					fieldsButtonNext;
		protected IconButton					fieldsButtonGoto;
		protected GlyphButton					fieldsButtonMenu;
		protected MyWidgets.StringArray			fieldsTable;

		protected TabBook						tabBookSecondary;

		protected TabPage						tabPageRelations;
		protected HToolBar						relationsToolbar;
		protected IconButton					relationsButtonUse;
		protected IconButton					relationsButtonExpand;
		protected IconButton					relationsButtonCompact;
		protected IconButton					relationsButtonAuto;
		protected MyWidgets.StringArray			relationsTable;

		protected TabPage						tabPageProperties;
		protected Scrollable					propertiesScrollable;

		protected TabPage						tabPageCultures;
		protected List<IconButton>				cultureButtonList;

		protected UI.PanelMode					panelMode = UI.PanelMode.Default;
		protected Druid							druidToSerialize;
	}
}
