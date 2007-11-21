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
			this.middle.Padding = new Margins(2, 5, 5, 5);
			this.middle.Dock = DockStyle.Fill;

			FrameBox drawing = new FrameBox(this.middle);  // conteneur pour vToolBar et scrollable
			drawing.Dock = DockStyle.Fill;

			this.vToolBar = new VToolBar(drawing);
			this.vToolBar.Dock = DockStyle.Left;
			this.VToolBarAdd(Widgets.Command.Get("ToolSelect"));
			this.VToolBarAdd(Widgets.Command.Get("ToolGlobal"));
			this.VToolBarAdd(null);
			this.VToolBarAdd(Widgets.Command.Get("ObjectHLine"));
			this.VToolBarAdd(Widgets.Command.Get("ObjectStatic"));

			FrameBox container = new FrameBox(drawing);
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

			//	Crée le panneau pour la table des champs.
			this.panelField = new FrameBox(this.right);
			this.panelField.Dock = DockStyle.Fill;
			this.panelField.Margins = new Margins(5, 5, 5, 5);

			this.fieldToolbar = new HToolBar(this.panelField);
			this.fieldToolbar.Dock = DockStyle.Top;
			this.fieldToolbar.Margins = new Margins(0, 0, 0, 5);

			this.fieldButtonUse = new IconButton();
			this.fieldButtonUse.AutoFocus = false;
			this.fieldButtonUse.CaptionId = Res.Captions.Editor.Forms.Use.Id;
			this.fieldButtonUse.Clicked += new MessageEventHandler(this.HandleFieldButtonClicked);
			this.fieldToolbar.Items.Add(this.fieldButtonUse);

			this.fieldToolbar.Items.Add(new IconSeparator());

			this.fieldButtonLine = new IconButton();
			this.fieldButtonLine.AutoFocus = false;
			this.fieldButtonLine.CaptionId = Res.Captions.Editor.Forms.Line.Id;
			this.fieldButtonLine.Clicked += new MessageEventHandler(this.HandleFieldButtonClicked);
			this.fieldToolbar.Items.Add(this.fieldButtonLine);

			this.fieldButtonTitle = new IconButton();
			this.fieldButtonTitle.AutoFocus = false;
			this.fieldButtonTitle.CaptionId = Res.Captions.Editor.Forms.Title.Id;
			this.fieldButtonTitle.Clicked += new MessageEventHandler(this.HandleFieldButtonClicked);
			this.fieldToolbar.Items.Add(this.fieldButtonTitle);

			this.fieldButtonBox = new IconButton();
			this.fieldButtonBox.AutoFocus = false;
			this.fieldButtonBox.CaptionId = Res.Captions.Editor.Forms.Box.Id;
			this.fieldButtonBox.Clicked += new MessageEventHandler(this.HandleFieldButtonClicked);
			this.fieldToolbar.Items.Add(this.fieldButtonBox);

			this.fieldToolbar.Items.Add(new IconSeparator());

			this.fieldButtonPrev = new IconButton();
			this.fieldButtonPrev.AutoFocus = false;
			this.fieldButtonPrev.CaptionId = Res.Captions.Editor.Forms.Prev.Id;
			this.fieldButtonPrev.Clicked += new MessageEventHandler(this.HandleFieldButtonClicked);
			this.fieldToolbar.Items.Add(this.fieldButtonPrev);

			this.fieldButtonNext = new IconButton();
			this.fieldButtonNext.AutoFocus = false;
			this.fieldButtonNext.CaptionId = Res.Captions.Editor.Forms.Next.Id;
			this.fieldButtonNext.Clicked += new MessageEventHandler(this.HandleFieldButtonClicked);
			this.fieldToolbar.Items.Add(this.fieldButtonNext);

			this.fieldToolbar.Items.Add(new IconSeparator());

			this.fieldButtonGoto = new IconButton();
			this.fieldButtonGoto.AutoFocus = false;
			this.fieldButtonGoto.CaptionId = Res.Captions.Editor.LocatorGoto.Id;
			this.fieldButtonGoto.Clicked += new MessageEventHandler(this.HandleFieldButtonClicked);
			this.fieldToolbar.Items.Add(this.fieldButtonGoto);

			this.fieldTable = new MyWidgets.StringArray(this.panelField);
			this.fieldTable.Columns = 2;
			this.fieldTable.SetColumnsRelativeWidth(0, 0.05);
			this.fieldTable.SetColumnsRelativeWidth(1, 0.95);
			this.fieldTable.SetColumnAlignment(0, ContentAlignment.MiddleCenter);
			this.fieldTable.SetColumnAlignment(1, ContentAlignment.MiddleLeft);
			this.fieldTable.SetColumnBreakMode(1, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
			this.fieldTable.AllowMultipleSelection = true;
			this.fieldTable.LineHeight = 16;
			this.fieldTable.Dock = DockStyle.Fill;
			this.fieldTable.CellCountChanged += new EventHandler(this.HandleFieldTableCellCountChanged);
			this.fieldTable.CellsContentChanged += new EventHandler(this.HandleFieldTableCellsContentChanged);
			this.fieldTable.SelectedRowChanged += new EventHandler(this.HandleFieldTableSelectedRowChanged);

			//	Crée le tabbook pour les onglets.
			this.tabBook = new TabBook(this.right);
			this.tabBook.Arrows = TabBookArrows.Stretch;
			this.tabBook.PreferredHeight = 200;
			this.tabBook.Dock = DockStyle.Bottom;
			this.tabBook.Margins = new Margins(5, 5, 5, 5);
			this.tabBook.Padding = new Margins(1, 1, 1, 1);
			this.tabBook.ActivePageChanged += new EventHandler<CancelEventArgs>(this.HandleTabBookActivePageChanged);

			//	Crée l'onglet 'propriétés'.
			this.tabPageProperties = new TabPage();
			this.tabPageProperties.TabTitle = Res.Strings.Viewers.Panels.TabProperties;
			this.tabPageProperties.Padding = new Margins(4, 4, 4, 4);
			this.tabBook.Items.Add(this.tabPageProperties);

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
			this.tabBook.Items.Add(this.tabPageCultures);

			this.CreateCultureButtons();

			this.tabBook.ActivePage = this.tabPageProperties;

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
				this.fieldButtonUse.Clicked -= new MessageEventHandler(this.HandleFieldButtonClicked);
				this.fieldButtonLine.Clicked -= new MessageEventHandler(this.HandleFieldButtonClicked);
				this.fieldButtonTitle.Clicked -= new MessageEventHandler(this.HandleFieldButtonClicked);
				this.fieldButtonBox.Clicked -= new MessageEventHandler(this.HandleFieldButtonClicked);
				this.fieldButtonPrev.Clicked -= new MessageEventHandler(this.HandleFieldButtonClicked);
				this.fieldButtonNext.Clicked -= new MessageEventHandler(this.HandleFieldButtonClicked);
				this.fieldButtonGoto.Clicked -= new MessageEventHandler(this.HandleFieldButtonClicked);

				this.fieldTable.CellCountChanged -= new EventHandler(this.HandleFieldTableCellCountChanged);
				this.fieldTable.CellsContentChanged -= new EventHandler(this.HandleFieldTableCellsContentChanged);
				this.fieldTable.SelectedRowChanged -= new EventHandler(this.HandleFieldTableSelectedRowChanged);
				
				this.tabBook.ActivePageChanged -= new EventHandler<CancelEventArgs>(this.HandleTabBookActivePageChanged);
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


		public override void DoTool(string name)
		{
			//	Choix de l'outil.
			base.DoTool(name);
			this.formEditor.AdaptAfterToolChanged();
			this.RegenerateProxies();
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
			this.UpdateFieldTable(true);
			this.UpdateButtons();
			this.ignoreChange = iic;
		}

		protected void UpdateFieldTable(bool newContent)
		{
			//	Met à jour la table des champs.
			this.formEditor.ObjectModifier.UpdateTableContent(this.druidToSerialize, this.entityDruidsPath);

			int first = this.fieldTable.FirstVisibleRow;
			for (int i=0; i<this.fieldTable.LineCount; i++)
			{
				if (first+i >= this.formEditor.ObjectModifier.TableContent.Count)
				{
					this.fieldTable.SetLineString(0, first+i, "");
					this.fieldTable.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);
					this.fieldTable.SetLineColor(0, first+i, Color.Empty);

					this.fieldTable.SetLineString(1, first+i, "");
					this.fieldTable.SetLineState(1, first+i, MyWidgets.StringList.CellState.Disabled);
					this.fieldTable.SetLineColor(1, first+i, Color.Empty);
				}
				else
				{
					FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[first+i];

					string name = this.module.AccessFields.GetFieldNames(item.DruidsPath);
					if (name == null)
					{
						FieldDescription field = this.formEditor.ObjectModifier.GetFormDescription(item);
						if (field != null)
						{
							name = Misc.Bold(Misc.Italic(field.Description));
						}
					}

					Color color = item.Used ? Color.FromRgb(0,1,0) : Color.FromRgb(1,0,0);

					this.fieldTable.SetLineString(0, first+i, "");
					this.fieldTable.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);
					this.fieldTable.SetLineColor(0, first+i, color);

					this.fieldTable.SetLineString(1, first+i, name);
					this.fieldTable.SetLineState(1, first+i, MyWidgets.StringList.CellState.Normal);
					this.fieldTable.SetLineColor(1, first+i, Color.Empty);
				}
			}

			this.fieldTable.TotalRows = this.formEditor.ObjectModifier.TableContent.Count;

			if (newContent)
			{
				this.fieldTable.FirstVisibleRow = 0;
			}
		}

		protected void UpdateButtons()
		{
			//	Met à jour les boutons.
			int useCounter = 0;
			int freeCounter = 0;
			bool isPrev = false;
			bool isNext = false;

			if (!this.designerApplication.IsReadonly)
			{
				List<int> sels = this.fieldTable.SelectedRows;
				if (sels != null && sels.Count > 0)
				{
					isPrev = true;
					isNext = true;

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
					}
				}
			}

			this.fieldButtonUse.Enable = (useCounter == 0 && freeCounter > 0) || (useCounter > 0 && freeCounter == 0);
			if (this.fieldButtonUse.Enable)
			{
				this.fieldButtonUse.IconName = (useCounter > 0) ? Misc.Icon("Delete") : Misc.Icon("Create");
			}
			else
			{
				this.fieldButtonUse.IconName = Misc.Icon("ActiveNo");
			}

			this.fieldButtonLine.Enable = (useCounter > 0 && freeCounter == 0);
			this.fieldButtonTitle.Enable = (useCounter > 0 && freeCounter == 0);
			this.fieldButtonBox.Enable = (useCounter > 0 && freeCounter == 0);

			this.fieldButtonPrev.Enable = isPrev;
			this.fieldButtonNext.Enable = isNext;
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

				this.entityDruidsPath = null;
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
				this.panelContainer = engine.CreateForm(form);
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
				}
				else
				{
					this.formEditor.DeselectAll();
				}

				this.entityDruidsPath = this.module.AccessEntities.GetEntityDruidsPath(this.entityId);
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
			this.UpdateArray();
			this.UpdateEdit();
			this.UpdateFieldTable(true);
			this.UpdateButtons();
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


		protected Widget VToolBarAdd(Command command)
		{
			//	Ajoute une icône dans la toolbar verticale.
			if (command == null)
			{
				//	N'utilise pas un IconSeparator, afin d'éviter les confusions
				//	avec les objets HSeparator et VSeparator !
				Widget sep = new Widget();
				sep.PreferredWidth = 20;
				sep.PreferredHeight = 20;
				this.vToolBar.Items.Add(sep);
				return sep;
			}
			else
			{
				IconButton button = new IconButton(command);
				button.PreferredWidth = 20;
				this.vToolBar.Items.Add(button);
				ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(command));
				return button;
			}
		}


		public override void UpdateViewer(Viewers.Changing oper)
		{
			//	Met à jour le statut du visualisateur en cours, en fonction de la sélection.
			//	Met également à jour l'arbre des objets, s'il est visible.
			if (oper == Changing.Selection)
			{
				this.ReflectSelectionToList();
				this.UpdateButtons();
			}

			if (oper == Changing.Create || oper == Changing.Delete || oper == Changing.Move || oper == Changing.Regenerate)
			{
				//	Régénère le panneau contenant le masque de saisie.
				this.SetForm(this.form, this.druidToSerialize, oper == Changing.Regenerate);
				this.UpdateFieldTable(false);
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
			this.fieldTable.SelectedRows = sels;
			this.ignoreChange = false;
		}

		protected void ReflectSelectionToEditor()
		{
			//	Reflète les sélections effectuées dans la liste des champs dans l'éditeur de Forms.
			//	Liste des champs -> Editeur de Forms.
			List<int> sels = this.fieldTable.SelectedRows;

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
			List<int> sels = this.fieldTable.SelectedRows;
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

			this.SetForm(this.form, this.druidToSerialize, false);
			this.UpdateFieldTable(false);

			sels.Clear();
			foreach (System.Guid guid in guids)
			{
				int index = this.formEditor.ObjectModifier.GetTableContentIndex(guid);
				if (index != -1)
				{
					sels.Add(index);
				}
			}
			this.fieldTable.SelectedRows = sels;
			this.ReflectSelectionToEditor();

			this.UpdateButtons();
		}

		protected void SelectedFieldsLine()
		{
			//	Insère une ligne avant le champ sélectionné.
			List<int> sels = this.fieldTable.SelectedRows;
			sels.Sort();

			FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sels[0]];
			int index = this.formEditor.ObjectModifier.GetFormDescriptionIndex(item.Guid);

			FieldDescription field = new FieldDescription(FieldDescription.FieldType.Line);
			this.form.Fields.Insert(index, field);

			this.SetForm(this.form, this.druidToSerialize, false);
			this.UpdateFieldTable(false);

			sels.Clear();
			sels.Add(index);
			this.fieldTable.SelectedRows = sels;
			this.ReflectSelectionToEditor();

			this.UpdateButtons();
		}

		protected void SelectedFieldsTitle()
		{
			//	Insère un titre avant le champ sélectionné.
			List<int> sels = this.fieldTable.SelectedRows;
			sels.Sort();

			FormEditor.ObjectModifier.TableItem item = this.formEditor.ObjectModifier.TableContent[sels[0]];
			int index = this.formEditor.ObjectModifier.GetFormDescriptionIndex(item.Guid);

			FieldDescription field = new FieldDescription(FieldDescription.FieldType.Title);
			this.form.Fields.Insert(index, field);

			this.SetForm(this.form, this.druidToSerialize, false);
			this.UpdateFieldTable(false);

			sels.Clear();
			sels.Add(index);
			this.fieldTable.SelectedRows = sels;
			this.ReflectSelectionToEditor();

			this.UpdateButtons();
		}

		protected void SelectedFieldsBox()
		{
			//	Groupe/sépare les champs sélectionnés.
		}

		protected void SelectedFieldsMove(int direction)
		{
			//	Déplace les champs sélectionnés vers le haut ou vers le bas.
			List<int> sels = this.fieldTable.SelectedRows;
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
			this.UpdateFieldTable(false);
			this.ReflectSelectionToList();
			this.UpdateButtons();
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

		private void HandleTabBookActivePageChanged(object sender, CancelEventArgs e)
		{
			//	Changement de l'onglet visible.
		}

		private void HandleFieldButtonClicked(object sender, MessageEventArgs e)
		{
			if (sender == this.fieldButtonUse)
			{
				this.SelectedFieldsUse();
			}

			if (sender == this.fieldButtonLine)
			{
				this.SelectedFieldsLine();
			}

			if (sender == this.fieldButtonTitle)
			{
				this.SelectedFieldsTitle();
			}

			if (sender == this.fieldButtonBox)
			{
				this.SelectedFieldsBox();
			}

			if (sender == this.fieldButtonPrev)
			{
				this.SelectedFieldsMove(-1);
			}

			if (sender == this.fieldButtonNext)
			{
				this.SelectedFieldsMove(1);
			}

			if (sender == this.fieldButtonGoto)
			{
			}
		}

		private void HandleFieldTableCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateFieldTable(false);
		}

		private void HandleFieldTableCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a changé.
			this.UpdateFieldTable(false);
		}

		private void HandleFieldTableSelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			if (this.ignoreChange)
			{
				return;
			}

			this.ReflectSelectionToEditor();
			this.UpdateButtons();
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
		protected VToolBar						vToolBar;
		protected FrameBox						panelContainerParent;
		protected UI.Panel						panelContainer;
		protected Druid							entityId;
		protected FormDescription				form;
		protected List<string>					entityDruidsPath;
		protected FormEditor.Editor				formEditor;
		protected FrameBox						right;
		protected HSplitter						splitter3;

		protected FrameBox						panelField;
		protected HToolBar						fieldToolbar;
		protected IconButton					fieldButtonUse;
		protected IconButton					fieldButtonLine;
		protected IconButton					fieldButtonTitle;
		protected IconButton					fieldButtonBox;
		protected IconButton					fieldButtonPrev;
		protected IconButton					fieldButtonNext;
		protected IconButton					fieldButtonGoto;
		protected MyWidgets.StringArray			fieldTable;

		protected TabBook						tabBook;
		protected TabPage						tabPageProperties;
		protected Scrollable					propertiesScrollable;

		protected TabPage						tabPageCultures;
		protected List<IconButton>				cultureButtonList;

		protected UI.PanelMode					panelMode = UI.PanelMode.Default;
		protected Druid							druidToSerialize;
	}
}
