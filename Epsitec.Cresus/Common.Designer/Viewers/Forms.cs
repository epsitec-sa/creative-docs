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
			this.drawingScrollable.Panel.IsAutoFitting = true;
			this.drawingScrollable.PaintForegroundFrame = true;

			FrameBox container = new FrameBox(this.drawingScrollable.Panel);
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
			this.right.PreferredWidth = 240;
			this.right.MaxWidth = 400;
			this.right.Dock = DockStyle.Right;

			//	Crée le tabbook primaire pour les onglets.
			this.tabBookPrimary = new TabBook(this.right);
			this.tabBookPrimary.Arrows = TabBookArrows.Stretch;
			this.tabBookPrimary.Dock = DockStyle.Fill;
			this.tabBookPrimary.Margins = new Margins(5, 5, 5, 5);
			this.tabBookPrimary.Padding = new Margins(1, 1, 1, 1);

			//	Crée l'onglet 'champs'.
			this.tabPageFields = new TabPage();
			this.tabPageFields.TabTitle = Res.Strings.Viewers.Panels.TabFields;
			this.tabPageFields.Padding = new Margins(4, 4, 4, 4);
			this.tabBookPrimary.Items.Add(this.tabPageFields);

			this.fieldsToolbar = new HToolBar(this.tabPageFields);
			this.fieldsToolbar.Dock = DockStyle.Top;
			this.fieldsToolbar.Margins = new Margins(0, 0, 0, 5);

			this.fieldsButtonUse = new IconButton();
			this.fieldsButtonUse.AutoFocus = false;
			this.fieldsButtonUse.CaptionId = Res.Captions.Editor.Forms.Use.Id;
			this.fieldsButtonUse.Clicked += new MessageEventHandler(this.HandleFieldsButtonClicked);
			this.fieldsToolbar.Items.Add(this.fieldsButtonUse);

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

			this.fieldsTable = new MyWidgets.StringArray(this.tabPageFields);
			this.fieldsTable.Columns = 2;
			this.fieldsTable.SetColumnsRelativeWidth(0, 0.05);
			this.fieldsTable.SetColumnsRelativeWidth(1, 0.95);
			this.fieldsTable.SetColumnAlignment(0, ContentAlignment.MiddleCenter);
			this.fieldsTable.SetColumnAlignment(1, ContentAlignment.MiddleLeft);
			this.fieldsTable.SetColumnBreakMode(1, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
			this.fieldsTable.AllowMultipleSelection = true;
			this.fieldsTable.LineHeight = 16;
			this.fieldsTable.Dock = DockStyle.Fill;
			this.fieldsTable.CellCountChanged += new EventHandler(this.HandleFieldTableCellCountChanged);
			this.fieldsTable.CellsContentChanged += new EventHandler(this.HandleFieldTableCellsContentChanged);
			this.fieldsTable.SelectedRowChanged += new EventHandler(this.HandleFieldTableSelectedRowChanged);

			//	Crée l'onglet 'relations'.
			this.tabPageRelations = new TabPage();
			this.tabPageRelations.TabTitle = Res.Strings.Viewers.Panels.TabRelations;
			this.tabPageRelations.Padding = new Margins(4, 4, 4, 4);
			this.tabBookPrimary.Items.Add(this.tabPageRelations);

			this.relationsToolbar = new HToolBar(this.tabPageRelations);
			this.relationsToolbar.Dock = DockStyle.Top;
			this.relationsToolbar.Margins = new Margins(0, 0, 0, 5);

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

			this.relationsTable = new MyWidgets.StringArray(this.tabPageRelations);
			this.relationsTable.Columns = 2;
			this.relationsTable.SetColumnsRelativeWidth(0, 0.90);
			this.relationsTable.SetColumnsRelativeWidth(1, 0.10);
			this.relationsTable.SetColumnAlignment(0, ContentAlignment.MiddleLeft);
			this.relationsTable.SetColumnAlignment(1, ContentAlignment.MiddleCenter);
			this.relationsTable.SetColumnBreakMode(0, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
			this.relationsTable.AllowMultipleSelection = false;
			this.relationsTable.LineHeight = 16;
			this.relationsTable.Dock = DockStyle.Fill;
			this.relationsTable.CellCountChanged += new EventHandler(this.HandleRelationsTableCellCountChanged);
			this.relationsTable.CellsContentChanged += new EventHandler(this.HandleRelationsTableCellsContentChanged);
			this.relationsTable.SelectedRowChanged += new EventHandler(this.HandleRelationsTableSelectedRowChanged);

			//	Crée le tabbook secondaire pour les onglets.
			this.tabBookSecondary = new TabBook(this.right);
			this.tabBookSecondary.Arrows = TabBookArrows.Stretch;
			this.tabBookSecondary.PreferredHeight = 200;
			this.tabBookSecondary.Dock = DockStyle.Bottom;
			this.tabBookSecondary.Margins = new Margins(5, 5, 5, 5);
			this.tabBookSecondary.Padding = new Margins(1, 1, 1, 1);

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
			this.propertiesScrollable.Panel.IsAutoFitting = true;
			this.propertiesScrollable.Panel.Margins = new Margins(10, 10, 10, 10);
			this.propertiesScrollable.PaintForegroundFrame = true;

			//	Crée l'onglet 'cultures'.
			this.tabPageCultures = new TabPage();
			this.tabPageCultures.TabTitle = Res.Strings.Viewers.Panels.TabCultures;
			this.tabPageCultures.Padding = new Margins(10, 10, 10, 10);
			this.tabBookSecondary.Items.Add(this.tabPageCultures);

			this.CreateCultureButtons();

			this.tabBookPrimary.ActivePage = this.tabPageFields;
			this.tabBookSecondary.ActivePage = this.tabPageProperties;

			this.splitter2 = new VSplitter(surface);
			this.splitter2.Dock = DockStyle.Right;
			this.splitter2.Margins = new Margins(0, 0, 1, 1);

			this.splitter3 = new HSplitter(this.right);
			this.splitter3.Dock = DockStyle.Bottom;
			this.splitter3.Margins = new Margins(0, 1, 0, 0);

			this.UpdateAll();
			this.UpdateViewer(Viewers.Changing.Show);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.fieldsButtonUse.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonGlue.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonLine.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonTitle.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonBox.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonPrev.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonNext.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);
				this.fieldsButtonGoto.Clicked -= new MessageEventHandler(this.HandleFieldsButtonClicked);

				this.fieldsTable.CellCountChanged -= new EventHandler(this.HandleFieldTableCellCountChanged);
				this.fieldsTable.CellsContentChanged -= new EventHandler(this.HandleFieldTableCellsContentChanged);
				this.fieldsTable.SelectedRowChanged -= new EventHandler(this.HandleFieldTableSelectedRowChanged);

				this.relationsButtonExpand.Clicked -= new MessageEventHandler(this.HandleRelationsButtonClicked);
				this.relationsButtonCompact.Clicked -= new MessageEventHandler(this.HandleRelationsButtonClicked);

				this.relationsTable.CellCountChanged -= new EventHandler(this.HandleRelationsTableCellCountChanged);
				this.relationsTable.CellsContentChanged -= new EventHandler(this.HandleRelationsTableCellsContentChanged);
				this.relationsTable.SelectedRowChanged -= new EventHandler(this.HandleRelationsTableSelectedRowChanged);
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

			this.table.SourceType = cultureMapType;
			this.table.Columns.Add(new UI.ItemTableColumn("Name", new Widgets.Layouts.GridLength(this.GetColumnWidth(0), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.ColumnHeader.SetColumnText(0, Res.Strings.Viewers.Column.Name);
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
				return (this.formEditor.SelectedObjects.Count != 0);
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

		protected void UpdateFieldsTable(bool newContent)
		{
			//	Met à jour la table des champs.
			//?this.formEditor.ObjectModifier.UpdateTableContent(this.druidToSerialize, this.entityFields);
			this.formEditor.ObjectModifier.UpdateTableContent(this.druidToSerialize, this.formEditor.ObjectModifier.TableRelations);

			int first = this.fieldsTable.FirstVisibleRow;
			for (int i=0; i<this.fieldsTable.LineCount; i++)
			{
				if (first+i >= this.formEditor.ObjectModifier.TableContent.Count)
				{
					this.fieldsTable.SetLineString(0, first+i, "");
					this.fieldsTable.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);
					this.fieldsTable.SetLineColor(0, first+i, Color.Empty);

					this.fieldsTable.SetLineString(1, first+i, "");
					this.fieldsTable.SetLineState(1, first+i, MyWidgets.StringList.CellState.Disabled);
					this.fieldsTable.SetLineColor(1, first+i, Color.Empty);
				}
				else
				{
					FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[first+i];

					string name = this.formEditor.ObjectModifier.GetTableContentDescription(item);
					Color color = item.Used ? Color.FromRgb(0,1,0) : Color.FromRgb(1,0,0);

					this.fieldsTable.SetLineString(0, first+i, "");
					this.fieldsTable.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);
					this.fieldsTable.SetLineColor(0, first+i, color);

					this.fieldsTable.SetLineString(1, first+i, name);
					this.fieldsTable.SetLineState(1, first+i, MyWidgets.StringList.CellState.Normal);
					this.fieldsTable.SetLineColor(1, first+i, Color.Empty);
				}
			}

			this.fieldsTable.TotalRows = this.formEditor.ObjectModifier.TableContent.Count;

			if (newContent)
			{
				this.fieldsTable.FirstVisibleRow = 0;
			}
		}

		protected void UpdateFieldsButtons()
		{
			//	Met à jour les boutons dans l'onglet des champs.
			int useCounter = 0;
			int freeCounter = 0;
			int boxBegin = 0;
			int boxEnd = 0;
			bool isPrev = false;
			bool isNext = false;
			bool isGoto = false;

			if (!this.designerApplication.IsReadonly)
			{
				List<int> sels = this.fieldsTable.SelectedRows;

				if (sels != null && sels.Count > 0)
				{
					sels.Sort();

					isPrev = true;
					isNext = true;
					isGoto = (sels.Count == 1);

					foreach (int sel in sels)
					{
						FormEditor.ObjectModifier.TableItem prev = Common.Designer.FormEditor.ObjectModifier.TableItem.Empty;
						FormEditor.ObjectModifier.TableItem curr = Common.Designer.FormEditor.ObjectModifier.TableItem.Empty;
						FormEditor.ObjectModifier.TableItem next = Common.Designer.FormEditor.ObjectModifier.TableItem.Empty;

						if (sel > 0)
						{
							prev = this.formEditor.ObjectModifier.TableContent[sel-1];
						}

						curr = this.formEditor.ObjectModifier.TableContent[sel];

						if (sel < this.formEditor.ObjectModifier.TableContent.Count-1)
						{
							next = this.formEditor.ObjectModifier.TableContent[sel+1];
						}

						if (curr.FieldType == FieldDescription.FieldType.BoxBegin)
						{
							boxBegin++;
						}

						if (curr.FieldType == FieldDescription.FieldType.BoxEnd)
						{
							boxEnd++;
						}

						if (!curr.Used || !prev.Used)  // premier champ utilisé ?
						{
							isPrev = false;
						}

						if (!curr.Used || !next.Used)  // dernier champ utilisé ?
						{
							isNext = false;
						}

						if (curr.Used)  // champ utilisé ?
						{
							useCounter++;
						}
						else  // champ inutilisé ?
						{
							freeCounter++;
							isPrev = false;
							isNext = false;
						}

						if (curr.FieldType != FieldDescription.FieldType.Field)
						{
							isGoto = false;
						}
					}
				}
			}

			if (boxBegin == 0 && boxEnd == 0)
			{
				this.fieldsButtonUse.Enable = (useCounter == 0 && freeCounter > 0) || (useCounter > 0 && freeCounter == 0);
			}
			else
			{
				this.fieldsButtonUse.Enable = false;
			}

			if (this.fieldsButtonUse.Enable)
			{
				this.fieldsButtonUse.IconName = (useCounter > 0) ? Misc.Icon("Delete") : Misc.Icon("Create");
			}
			else
			{
				this.fieldsButtonUse.IconName = Misc.Icon("ActiveNo");
			}

			this.fieldsButtonGlue.Enable = (useCounter > 0 && freeCounter == 0);
			this.fieldsButtonLine.Enable = (useCounter > 0 && freeCounter == 0);
			this.fieldsButtonTitle.Enable = (useCounter > 0 && freeCounter == 0);
			this.fieldsButtonBox.Enable = (useCounter > 0 && freeCounter == 0);

			this.fieldsButtonPrev.Enable = isPrev;
			this.fieldsButtonNext.Enable = isNext;
			this.fieldsButtonGoto.Enable = isGoto;
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
				}
				else
				{
					string name = this.formEditor.ObjectModifier.GetTableRelationDescription(first+i);
					string icon = this.formEditor.ObjectModifier.GetTableRelationIcon(first+i);

					this.relationsTable.SetLineString(0, first+i, name);
					this.relationsTable.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);
					this.relationsTable.SetLineColor(0, first+i, Color.Empty);

					this.relationsTable.SetLineString(1, first+i, icon);
					this.relationsTable.SetLineState(1, first+i, MyWidgets.StringList.CellState.Normal);
					this.relationsTable.SetLineColor(1, first+i, Color.Empty);
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

			this.relationsButtonExpand.Enable = this.formEditor.ObjectModifier.IsTableRelationExpandable(sel);
			this.relationsButtonCompact.Enable = this.formEditor.ObjectModifier.IsTableRelationCompactable(sel);
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
					Forms.softSerialize = this.FormToXml(this.GetForm());
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
			this.access.SetForm(this.druidToSerialize, this.GetForm());
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
				FormDescription form = this.access.GetForm(this.druidToSerialize);
				this.SetForm(form, this.druidToSerialize, false);
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
				this.SetForm(form, this.druidToSerialize, false);

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
			return FormEngine.Serialization.DeserializeForm(xml, this.module.ResourceManager);
		}

		protected FormDescription GetForm()
		{
			//	Retourne le masque de saisie en cours d'édition.
			return this.form;
		}

		protected void SetForm(FormDescription form, Druid druid, bool keepSelection)
		{
			//	Spécifie le masque de saisie en cours d'édition.
			//	Construit physiquement le masque de saisie (UI.Panel contenant des widgets) sur la
			//	base de sa description FormDescription.
			this.form = form;

			if (this.panelContainer != null)
			{
				this.panelContainer.SetParent(null);
				this.panelContainer = null;
			}

			if (form == null || druid.IsEmpty)
			{
				this.entityId = Druid.Empty;

				this.panelContainer = new UI.Panel();
				this.InitializePanel();

				this.formEditor.Panel = this.panelContainer;
				this.formEditor.Druid = druid;
				this.formEditor.IsEditEnabled = false;
				this.formEditor.Form = null;

				this.entityFields = null;
			}
			else
			{
				this.entityId = form.EntityId;

				List<System.Guid> guids = null;
				if (keepSelection)
				{
					guids = this.formEditor.GetSelectedGuids();  // Guid des objets sélectionnés
				}

				FormEngine.Engine engine = new FormEngine.Engine(this.module.ResourceManager);
				this.panelContainer = engine.CreateForm(form, true);
				if (this.panelContainer == null)
				{
					this.panelContainer = new UI.Panel();
				}
				this.InitializePanel();

				this.formEditor.Panel = this.panelContainer;
				this.formEditor.Druid = druid;
				this.formEditor.IsEditEnabled = !this.designerApplication.IsReadonly;
				this.formEditor.Form = form;

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
				this.SetForm(this.form, this.druidToSerialize, oper == Changing.Regenerate);
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


		protected void SelectedFieldsUse()
		{
			//	Utilise ou supprime les champs sélectionnés.
			List<int> sels = this.fieldsTable.SelectedRows;
			sels.Sort();

			List<System.Guid> guids = new List<System.Guid>();

			foreach (int sel in sels)
			{
				FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sel];

				if (item.Used)  // supprime le champ du masque ?
				{
					int index = this.formEditor.ObjectModifier.GetFormDescriptionIndex(item.Guid);
					if (index != -1)
					{
						this.form.Fields.RemoveAt(index);
					}
				}
				else  // ajoute le champ dans le masque ?
				{
					FieldDescription field = new FieldDescription(FieldDescription.FieldType.Field);
					field.SetFields(item.DruidsPath);

					this.form.Fields.Add(field);
				}

				guids.Add(item.Guid);
			}

			this.SetForm(this.form, this.druidToSerialize, true);
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
			this.module.AccessForms.SetLocalDirty();
		}

		protected void SelectedFieldsGlue()
		{
			//	Insère une "glue" avant le champ sélectionné.
			List<int> sels = this.fieldsTable.SelectedRows;
			sels.Sort();

			FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sels[0]];
			int index = this.formEditor.ObjectModifier.GetFormDescriptionIndex(item.Guid);

			FieldDescription field = new FieldDescription(FieldDescription.FieldType.Glue);
			field.ColumnsRequired = 0;
			this.form.Fields.Insert(index, field);

			this.SetForm(this.form, this.druidToSerialize, true);
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
			int index = this.formEditor.ObjectModifier.GetFormDescriptionIndex(item.Guid);

			FieldDescription field = new FieldDescription(FieldDescription.FieldType.Line);
			this.form.Fields.Insert(index, field);

			this.SetForm(this.form, this.druidToSerialize, true);
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
			int index = this.formEditor.ObjectModifier.GetFormDescriptionIndex(item.Guid);

			FieldDescription field = new FieldDescription(FieldDescription.FieldType.Title);
			this.form.Fields.Insert(index, field);

			this.SetForm(this.form, this.druidToSerialize, true);
			this.UpdateFieldsTable(false);

			sels.Clear();
			sels.Add(index);
			this.fieldsTable.SelectedRows = sels;
			this.ReflectSelectionToEditor();

			this.UpdateFieldsButtons();
			this.module.AccessForms.SetLocalDirty();
		}

		protected void SelectedFieldsBox()
		{
			//	Groupe/sépare les champs sélectionnés.
			List<int> sels = this.fieldsTable.SelectedRows;
			sels.Sort();

			FormEditor.ObjectModifier.TableItem firstItem = this.formEditor.ObjectModifier.TableContent[sels[0]];
			int first = this.formEditor.ObjectModifier.GetFormDescriptionIndex(firstItem.Guid);

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
								int last = this.formEditor.ObjectModifier.GetFormDescriptionIndex(item.Guid);
								this.form.Fields.RemoveAt(last);  // enlève le BoxEnd
								this.form.Fields.RemoveAt(first);  // enlève le BoxBegin

								this.SetForm(this.form, this.druidToSerialize, true);
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

				FieldDescription field = this.form.Fields[sel];
				this.form.Fields.RemoveAt(sel);
				content.Insert(0, field);
			}

			int index = first;

			FieldDescription box = new FieldDescription(FieldDescription.FieldType.BoxBegin);
			box.BoxFrameState = FrameState.All;
			this.form.Fields.Insert(index++, box);

			foreach (FieldDescription field in content)
			{
				this.form.Fields.Insert(index++, field);
			}

			box = new FieldDescription(FieldDescription.FieldType.BoxEnd);
			this.form.Fields.Insert(index++, box);

			this.SetForm(this.form, this.druidToSerialize, true);
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

			foreach (int sel in sels)
			{
				FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sel];
				int index = this.formEditor.ObjectModifier.GetFormDescriptionIndex(item.Guid);
				if (index != -1)
				{
					FieldDescription field = this.form.Fields[index];
					this.form.Fields.RemoveAt(index);
					this.form.Fields.Insert(index+direction, field);
				}
			}

			this.SetForm(this.form, this.druidToSerialize, true);
			this.UpdateFieldsTable(false);
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
			FieldDescription field = this.formEditor.ObjectModifier.GetFormDescription(item);
			Druid druid = field.FieldIds[field.FieldIds.Count-1];

			Module module = this.designerApplication.SearchModule(druid);
			if (module == null)
			{
				return;
			}

			this.designerApplication.LocatorGoto(module.ModuleId.Name, ResourceAccess.Type.Fields, -1, druid, this.Window.FocusedWidget);
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


		#region Proxies
		protected void DefineProxies(IEnumerable<Widget> widgets)
		{
			//	Crée les proxies et l'interface utilisateur pour les widgets sélectionnés.
			this.ClearProxies();
			this.proxyManager.SetSelection(widgets);
			this.proxyManager.CreateUserInterface(this.propertiesScrollable.Panel);
		}

		protected void ClearProxies()
		{
			//	Supprime l'interface utilisateur pour les widgets sélectionnés.
			this.proxyManager.ClearUserInterface(this.propertiesScrollable.Panel);
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
				this.proxyManager.CreateUserInterface(this.propertiesScrollable.Panel);
			}
			this.module.AccessForms.SetLocalDirty();
		}

		public void RegenerateProxiesAndForm()
		{
			//	Régénère les proxies et le masque de saisie.
			if (this.proxyManager.RegenerateProxies())
			{
				this.ClearProxies();
				this.proxyManager.CreateUserInterface(this.propertiesScrollable.Panel);
			}

			this.formEditor.RegenerateForm();
			this.module.AccessForms.SetLocalDirty();
		}
		#endregion


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
		}

		private void HandleFieldsButtonClicked(object sender, MessageEventArgs e)
		{
			if (sender == this.fieldsButtonUse)
			{
				this.SelectedFieldsUse();
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
		}

		private void HandleRelationsButtonClicked(object sender, MessageEventArgs e)
		{
			if (sender == this.relationsButtonExpand)
			{
				this.SelectedRelationsExpand();
			}

			if (sender == this.relationsButtonCompact)
			{
				this.SelectedRelationsCompact();
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


		protected static readonly double		treeButtonWidth = 22;
		protected static double					treeBranchesHeight = 30;

		private static double[]					columnWidthHorizontal = {200};
		private static double[]					columnWidthVertical = {250};

		protected static string					softSerialize = null;
		protected static bool					softDirtySerialization = false;

		protected FormEditor.ProxyManager		proxyManager;
		protected VSplitter						splitter2;
		protected Widget						middle;
		protected Scrollable					drawingScrollable;
		protected FrameBox						panelContainerParent;
		protected UI.Panel						panelContainer;
		protected Druid							entityId;
		protected FormDescription				form;
		protected IList<StructuredData>			entityFields;
		protected FormEditor.Editor				formEditor;
		protected FrameBox						right;
		protected HSplitter						splitter3;

		protected TabBook						tabBookPrimary;
		protected TabBook						tabBookSecondary;

		protected TabPage						tabPageFields;
		protected HToolBar						fieldsToolbar;
		protected IconButton					fieldsButtonUse;
		protected IconButton					fieldsButtonGlue;
		protected IconButton					fieldsButtonLine;
		protected IconButton					fieldsButtonTitle;
		protected IconButton					fieldsButtonBox;
		protected IconButton					fieldsButtonPrev;
		protected IconButton					fieldsButtonNext;
		protected IconButton					fieldsButtonGoto;
		protected MyWidgets.StringArray			fieldsTable;

		protected TabPage						tabPageRelations;
		protected HToolBar						relationsToolbar;
		protected IconButton					relationsButtonExpand;
		protected IconButton					relationsButtonCompact;
		protected MyWidgets.StringArray			relationsTable;

		protected TabPage						tabPageProperties;
		protected Scrollable					propertiesScrollable;

		protected TabPage						tabPageCultures;
		protected List<IconButton>				cultureButtonList;

		protected UI.PanelMode					panelMode = UI.PanelMode.Default;
		protected Druid							druidToSerialize;
	}
}
