using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Viewers
{
	public enum Changing
	{
		Show,			// changement d'interface
		Selection,		// sélection ou désélection
		Create,			// création d'un nouvel objet
		Delete,			// suppression d'un objet
		Move,			// déplacement d'un objet dans l'arbre (mais pas un déplacement géométrique)
		Regenerate,		// régénération du contenu
	}


	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public abstract class Abstract : AbstractGroup
	{
		protected enum BandMode
		{
			MainSummary,
			MainView,
			Separator,
			SuiteSummary,
			SuiteView,
		}


		public Abstract(Module module, PanelsContext context, ResourceAccess access, DesignerApplication designerApplication)
		{
			this.module = module;
			this.context = context;
			this.access = access;
			this.designerApplication = designerApplication;
			this.access.ResourceType = this.ResourceType;
			this.itemViewFactory = new ItemViewFactory(this);
			
			//	Crée les deux volets séparés d'un splitter.
			this.firstPane = new FrameBox(this);
			this.firstPane.Name = "FirstPane";
			if (this.IsDisplayModeHorizontal)
			{
				this.firstPane.MinWidth = 80;
				this.firstPane.MaxWidth = 600;
				this.firstPane.PreferredWidth = Abstract.leftArrayWidth;
			}
			else
			{
				this.firstPane.MinHeight = 100;
				this.firstPane.MaxHeight = 600;
				this.firstPane.PreferredHeight = Abstract.topArrayHeight;
			}
			this.firstPane.Dock = this.IsDisplayModeHorizontal ? DockStyle.Left : DockStyle.Top;
			this.firstPane.Padding = new Margins(10, 10, 10, 10);
			this.firstPane.TabIndex = this.tabIndex++;
			this.firstPane.Visibility = (this.designerApplication.DisplayModeState != DesignerApplication.DisplayMode.FullScreen);

			if (this.IsDisplayModeHorizontal)
			{
				this.splitter = new VSplitter(this);
				this.splitter.Dock = DockStyle.Left;
			}
			else
			{
				this.splitter = new HSplitter(this);
				this.splitter.Dock = DockStyle.Top;
			}
			this.splitter.SplitterDragged += this.HandleSplitterDragged;
			this.splitter.Visibility = (this.designerApplication.DisplayModeState != DesignerApplication.DisplayMode.FullScreen);
			AbstractSplitter.SetAutoCollapseEnable(this.firstPane, true);

			this.lastPane = new FrameBox(this);
			this.lastPane.Name = "LastPane";
			if (this.IsDisplayModeHorizontal)
			{
				this.lastPane.MinWidth = 200;
			}
			else
			{
				this.lastPane.MinHeight = 50;
			}
			this.lastPane.Dock = DockStyle.Fill;
			this.lastPane.TabIndex = this.tabIndex++;
			
			//	Crée la première partie (gauche ou supérieure).
			this.labelEdit = new MyWidgets.TextFieldExName(this.firstPane);
			this.labelEdit.Name = "LabelEdit";
			this.labelEdit.Margins = new Margins(0, 0, 10, 0);
			this.labelEdit.Dock = DockStyle.Bottom;
			this.labelEdit.ButtonShowCondition = ButtonShowCondition.WhenModified;
			this.labelEdit.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.labelEdit.EditionAccepted += this.HandleTextChanged;
			this.labelEdit.EditionRejected += this.HandleTextRejected;
			this.labelEdit.CursorChanged += this.HandleCursorChanged;
			this.labelEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);
			this.labelEdit.TabIndex = this.tabIndex++;
			this.labelEdit.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.labelEdit.Visibility = (this.module.Mode == DesignerMode.Build);
			this.currentTextField = this.labelEdit;

			this.table = new UI.ItemTable(this.firstPane);
			this.table.ItemPanel.CustomItemViewFactoryGetter = this.ItemViewFactoryGetter;
			this.table.Items = this.access.CollectionView;
			this.InitializeTable();
			this.table.HorizontalScrollMode = this.IsDisplayModeHorizontal ? UI.ItemTableScrollMode.Linear : UI.ItemTableScrollMode.None;
			this.table.VerticalScrollMode = UI.ItemTableScrollMode.ItemBased;
			this.table.HeaderVisibility = true;
			this.table.FrameVisibility = true;
			this.table.ItemPanel.Layout = UI.ItemPanelLayout.VerticalList;
			this.table.ItemPanel.ItemSelectionMode = UI.ItemPanelSelectionMode.ExactlyOne;
			this.table.ItemPanel.CurrentItemTrackingMode = UI.CurrentItemTrackingMode.AutoSelect;
			this.table.ItemPanel.SelectionChanged += new EventHandler<UI.ItemPanelSelectionChangedEventArgs>(this.HandleTableSelectionChanged);
			this.table.SizeChanged += this.HandleTableSizeChanged;
			this.table.ColumnHeader.ColumnWidthChanged += this.HandleColumnHeaderColumnWidthChanged;
			//?this.table.ColumnHeader.SetColumnSort(0, ListSortDirection.Ascending);
			this.table.Dock = Widgets.DockStyle.Fill;
			this.table.Margins = Drawing.Margins.Zero;

			//	Crée la dernière partie (droite ou inférieure), bande supérieure pour les boutons des cultures.
			this.lastGroup = new FrameBox(this.lastPane);
			this.lastGroup.Padding = new Margins(10, 10, 10, 10);
			this.lastGroup.Dock = DockStyle.Fill;
			this.lastGroup.TabIndex = this.tabIndex++;

			Widget sup = new FrameBox(this.lastGroup);
			sup.Name = "Sup";
			sup.PreferredHeight = 26;
			sup.Padding = new Margins(0, 0, 1, 0);
			sup.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			sup.Dock = DockStyle.Top;
			sup.TabIndex = this.tabIndex++;
			
			this.primaryButtonCulture = new IconButtonMark(sup);
			this.primaryButtonCulture.ButtonStyle = ButtonStyle.ActivableIcon;
			this.primaryButtonCulture.MarkDisposition = ButtonMarkDisposition.Below;
			this.primaryButtonCulture.MarkLength = 5;
			this.primaryButtonCulture.PreferredHeight = 25;
			this.primaryButtonCulture.ActiveState = ActiveState.Yes;
			this.primaryButtonCulture.AutoFocus = false;
			this.primaryButtonCulture.Margins = new Margins(0, 1, 0, 0);
			this.primaryButtonCulture.Dock = DockStyle.Fill;

			this.secondaryButtonsCultureGroup = new FrameBox(sup);
			this.secondaryButtonsCultureGroup.Margins = new Margins(1, 0, 0, 0);
			this.secondaryButtonsCultureGroup.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			this.secondaryButtonsCultureGroup.Dock = DockStyle.Fill;
			this.secondaryButtonsCultureGroup.TabIndex = this.tabIndex++;

			this.cultureMenuButton = new GlyphButton(sup);
			this.cultureMenuButton.GlyphShape = GlyphShape.Menu;
			this.cultureMenuButton.ButtonStyle = ButtonStyle.ToolItem;
			this.cultureMenuButton.AutoFocus = false;
			this.cultureMenuButton.Clicked += this.HandleCultureMenuButtonClicked;
			this.cultureMenuButton.Margins = new Margins(1, 0, 2, 7);
			this.cultureMenuButton.Dock = DockStyle.Right;

			//	Crée le titre.
			this.titleBox = new FrameBox(this.lastGroup);
			this.titleBox.DrawFullFrame = true;
			this.titleBox.PreferredHeight = 26;
			this.titleBox.Dock = DockStyle.Top;
			this.titleBox.Margins = new Margins(0, 0, 1, -1);

			this.titleText = new StaticText(this.titleBox);
			this.titleText.ContentAlignment = ContentAlignment.MiddleCenter;
			this.titleText.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			this.titleText.Dock = DockStyle.Fill;
			this.titleText.Margins = new Margins(4, 4, 0, 0);

			//	Crée la dernière partie (droite ou inférieure), bande inférieure pour la zone d'étition scrollable.
			this.scrollable = new Scrollable(this.lastGroup);
			this.scrollable.Name = "Scrollable";
			this.scrollable.MinWidth = 100;
			this.scrollable.MinHeight = 39;
			this.scrollable.Margins = new Margins(0, 0, 0, 0);
			this.scrollable.Dock = DockStyle.Fill;
			this.scrollable.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
			this.scrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.scrollable.Viewport.IsAutoFitting = true;
			this.scrollable.PaintViewportFrame = true;
			this.scrollable.ViewportPadding = new Margins (-1);
			this.scrollable.Viewport.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
			this.scrollable.TabIndex = this.tabIndex++;
			this.scrollable.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.bands = new List<Band>();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.buttonMainExtendLeft != null)
				{
					this.buttonMainExtendLeft.Clicked -= this.HandleButtonCompactOrExtendClicked;
				}

				if (this.buttonMainCompactLeft != null)
				{
					this.buttonMainCompactLeft.Clicked -= this.HandleButtonCompactOrExtendClicked;
				}

				if (this.buttonMainExtendRight != null)
				{
					this.buttonMainExtendRight.Clicked -= this.HandleButtonCompactOrExtendClicked;
				}

				if (this.buttonMainCompactRight != null)
				{
					this.buttonMainCompactRight.Clicked -= this.HandleButtonCompactOrExtendClicked;
				}

				if (this.buttonSuiteExtendLeft != null)
				{
					this.buttonSuiteExtendLeft.Clicked -= this.HandleButtonCompactOrExtendClicked;
				}

				if (this.buttonSuiteCompactLeft != null)
				{
					this.buttonSuiteCompactLeft.Clicked -= this.HandleButtonCompactOrExtendClicked;
				}

				if (this.buttonSuiteExtendRight != null)
				{
					this.buttonSuiteExtendRight.Clicked -= this.HandleButtonCompactOrExtendClicked;
				}

				if (this.buttonSuiteCompactRight != null)
				{
					this.buttonSuiteCompactRight.Clicked -= this.HandleButtonCompactOrExtendClicked;
				}

				this.splitter.SplitterDragged -= this.HandleSplitterDragged;

				this.labelEdit.EditionAccepted -= this.HandleTextChanged;
				this.labelEdit.EditionRejected -= this.HandleTextRejected;
				this.labelEdit.CursorChanged -= this.HandleCursorChanged;
				this.labelEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);

				this.table.ItemPanel.SelectionChanged -= new EventHandler<UI.ItemPanelSelectionChangedEventArgs>(this.HandleTableSelectionChanged);
				this.table.SizeChanged -= this.HandleTableSizeChanged;
				this.table.ColumnHeader.ColumnWidthChanged -= this.HandleColumnHeaderColumnWidthChanged;

				this.cultureMenuButton.Clicked -= this.HandleCultureMenuButtonClicked;
			}

			base.Dispose(disposing);
		}


		protected virtual void InitializeTable()
		{
			//	Initialise la table.
			StructuredType cultureMapType = new StructuredType();
			cultureMapType.Fields.Add("Name", StringType.NativeDefault);
			cultureMapType.Fields.Add("Primary", StringType.NativeDefault);
			cultureMapType.Fields.Add("Secondary", StringType.NativeDefault);
			cultureMapType.Fields.Add("Source", StringType.NativeDefault);
			cultureMapType.Fields.Add("Druid", StringType.NativeDefault);
			cultureMapType.Fields.Add("Local", StringType.NativeDefault);
			cultureMapType.Fields.Add("Identity", StringType.NativeDefault);
			cultureMapType.Fields.Add("PatchLevel", StringType.NativeDefault);

			this.table.SourceType = cultureMapType;

			this.table.Columns.Add(new UI.ItemTableColumn("Name", new Widgets.Layouts.GridLength(this.GetColumnWidth(0), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Primary", new Widgets.Layouts.GridLength(this.GetColumnWidth(1), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Secondary", new Widgets.Layouts.GridLength(this.GetColumnWidth(2), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Source", new Widgets.Layouts.GridLength(this.GetColumnWidth(3), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Druid", new Widgets.Layouts.GridLength(this.GetColumnWidth(4), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Local", new Widgets.Layouts.GridLength(this.GetColumnWidth(5), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Identity", new Widgets.Layouts.GridLength(this.GetColumnWidth(6), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("PatchLevel", new Widgets.Layouts.GridLength(this.GetColumnWidth(7), Widgets.Layouts.GridUnitType.Proportional)));

			this.table.ColumnHeader.SetColumnComparer(1, this.ComparePrimary);
			this.table.ColumnHeader.SetColumnComparer(2, this.CompareSecondary);
			this.table.ColumnHeader.SetColumnComparer(3, this.CompareSource);
			this.table.ColumnHeader.SetColumnComparer(4, this.CompareDruid);
			this.table.ColumnHeader.SetColumnComparer(5, this.CompareLocal);
			this.table.ColumnHeader.SetColumnComparer(6, this.CompareIdentity);
			this.table.ColumnHeader.SetColumnComparer(7, this.ComparePatchLevel);

			this.table.ColumnHeader.SetColumnText(0, Res.Strings.Viewers.Column.Name);
			this.table.ColumnHeader.SetColumnText(4, Res.Strings.Viewers.Column.Druid);
			this.table.ColumnHeader.SetColumnText(5, Res.Strings.Viewers.Column.Local);
			this.table.ColumnHeader.SetColumnText(6, Res.Strings.Viewers.Column.Identity);
			this.table.ColumnHeader.SetColumnText(7, Res.Strings.Viewers.Column.PatchLevel);

			this.table.ColumnHeader.SetColumnSort(0, ListSortDirection.Ascending);
		}

		protected virtual int PrimaryColumn
		{
			//	Retourne le rang de la colonne pour la culture principale.
			get
			{
				return 1;
			}
		}

		protected virtual int SecondaryColumn
		{
			//	Retourne le rang de la colonne pour la culture secondaire.
			get
			{
				return 2;
			}
		}


		protected string TwoLettersSecondaryCulture
		{
			//	Culture secondaire utilisée.
			get
			{
				return this.secondaryCulture;
			}
			set
			{
				this.secondaryCulture = value;

				this.UpdateSelectedCulture();
				this.UpdateArray();
			}
		}

		public string GetTwoLetters(int row)
		{
			//	Retourne la culture primaire ou secondaire utilisée.
			System.Diagnostics.Debug.Assert(row == 0 || row == 1);
			return (row == 0) ? Resources.DefaultTwoLetterISOLanguageName : this.secondaryCulture;
		}



		public abstract ResourceAccess.Type ResourceType
		{
			get;
		}

		public static Abstract Create(ResourceAccess.Type type, Module module, PanelsContext context, ResourceAccess access, DesignerApplication designerApplication)
		{
			//	Crée un Viewer d'un type donné.
			if (type == ResourceAccess.Type.Strings )  return new Strings (module, context, access, designerApplication);
			if (type == ResourceAccess.Type.Captions)  return new Captions(module, context, access, designerApplication);
			if (type == ResourceAccess.Type.Fields  )  return new Fields  (module, context, access, designerApplication);
			if (type == ResourceAccess.Type.Commands)  return new Commands(module, context, access, designerApplication);
			if (type == ResourceAccess.Type.Types   )  return new Types   (module, context, access, designerApplication);
			if (type == ResourceAccess.Type.Values  )  return new Values  (module, context, access, designerApplication);
			if (type == ResourceAccess.Type.Entities)  return new Entities(module, context, access, designerApplication);
			if (type == ResourceAccess.Type.Panels  )  return new Panels  (module, context, access, designerApplication);
			if (type == ResourceAccess.Type.Forms   )  return new Forms   (module, context, access, designerApplication);
			return null;
		}


		public PanelsContext PanelsContext
		{
			get
			{
				return this.context;
			}
		}

		public DesignerApplication DesignerApplication
		{
			get
			{
				return this.designerApplication;
			}
		}


		public virtual bool HasUsefulViewerWindow
		{
			//	Indique si cette vue a l'utilité d'une fenêtre supplémentaire.
			get
			{
				return false;
			}
		}


		public virtual AbstractTextField CurrentTextField
		{
			//	Retourne le texte éditable en cours d'édition.
			get
			{
				return this.currentTextField;
			}
		}

		public void DoSearch(string search, Searcher.SearchingMode mode, List<int> filter)
		{
			//	Effectue une recherche.
			Searcher searcher = this.SearchBegin(mode, filter, false);
			if (searcher == null)
			{
				return;
			}

			if (searcher.Search(search))
			{
				this.lastActionIsReplace = false;
				this.access.AccessIndex = searcher.Row;
				this.SelectedRow = this.access.AccessIndex;

				AbstractTextField edit = this.IndexToTextField(searcher.Field, searcher.Subfield);
				if (edit != null && edit.Visibility)
				{
					this.ignoreChange = true;

					this.currentTextField = edit;
					this.Window.MakeActive();
					edit.Focus();
					edit.CursorFrom  = edit.TextLayout.FindIndexFromOffset(searcher.Index);
					edit.CursorTo    = edit.TextLayout.FindIndexFromOffset(searcher.Index+searcher.Length);
					edit.CursorAfter = false;

					this.ignoreChange = false;
				}
			}
			else
			{
				this.designerApplication.DialogError(Res.Strings.Dialog.Search.Message.Error);
			}
		}

		public void DoCount(string search, Searcher.SearchingMode mode, List<int> filter)
		{
			//	Effectue un comptage.
			Searcher searcher = this.SearchBegin(mode, filter, false);
			if (searcher == null)
			{
				return;
			}

			int count = searcher.Count(search);
			if (count == 0)
			{
				this.designerApplication.DialogError(Res.Strings.Dialog.Search.Message.Error);
			}
			else
			{
				string message = string.Format(Res.Strings.Dialog.Search.Message.Count, count.ToString());
				this.designerApplication.DialogMessage(message);
			}
		}

		public void DoReplace(string search, string replace, Searcher.SearchingMode mode, List<int> filter)
		{
			//	Effectue un remplacement.
			Searcher searcher = this.SearchBegin(mode, filter, true);
			if (searcher == null)
			{
				return;
			}

			if (searcher.Replace(search, false))
			{
				this.lastActionIsReplace = true;

				string text = this.ReplaceDo(searcher, replace);
				if (text == null)
				{
					this.access.AccessIndex = searcher.Row;
					this.SelectedRow = this.access.AccessIndex;
					return;
				}

				AbstractTextField edit = this.IndexToTextField(searcher.Field, searcher.Subfield);
				if (edit != null && edit.Visibility)
				{
					this.ignoreChange = true;

					this.currentTextField = edit;
					this.Window.MakeActive();
					edit.Focus();
					edit.Text = text;
					edit.CursorFrom  = edit.TextLayout.FindIndexFromOffset(searcher.Index);
					edit.CursorTo    = edit.TextLayout.FindIndexFromOffset(searcher.Index+replace.Length);
					edit.CursorAfter = false;

					this.ignoreChange = false;
				}

				if (searcher.Field == 0)  // remplacement de 'Name' ?
				{
					this.access.AccessIndex = searcher.Row;
					this.UpdateArray();
					this.ShowSelectedRow();
				}
			}
			else
			{
				this.designerApplication.DialogError(Res.Strings.Dialog.Search.Message.Error);
			}
		}

		public void DoReplaceAll(string search, string replace, Searcher.SearchingMode mode, List<int> filter)
		{
			//	Effectue un 'remplacer tout'.
			Searcher searcher = this.SearchBegin(mode, filter, true);
			if (searcher == null)
			{
				return;
			}

			this.access.SortDefer();
			int count = 0;
			bool fromBeginning = true;
			while (searcher.Replace(search, fromBeginning))
			{
				fromBeginning = false;
				count++;

				string text = this.ReplaceDo(searcher, replace);
				if (text == null)
				{
					this.access.SortUndefer();
					return;
				}

				searcher.Skip(replace.Length);  // saute les caractères sélectionnés
			}
			this.access.SortUndefer();

			if (count == 0)
			{
				this.designerApplication.DialogError(Res.Strings.Dialog.Search.Message.Error);
			}
			else
			{
				this.UpdateArray();
				this.ShowSelectedRow();
				this.UpdateEdit();
				this.UpdateCommands();

				string text = string.Format(Res.Strings.Dialog.Search.Message.Replace, count.ToString());
				this.designerApplication.DialogMessage(text);
			}
		}

		protected Searcher SearchBegin(Searcher.SearchingMode mode, List<int> filter, bool replace)
		{
			//	Initialisation pour une recherche.
			if (replace && this.module.Mode == DesignerMode.Translate)
			{
				if (filter.Contains(0))  // remplacement dans 'Name' ?
				{
					filter.Remove(0);  // interdi en mode 'Translate'
				}
			}

			if (filter.Count == 0)  // tout filtré ?
			{
				return null;
			}

			Searcher searcher = new Searcher(this.access);

			int field, subfield;
			this.TextFieldToIndex(this.currentTextField, out field, out subfield);
			if (field == -1)
			{
				return null;
			}

			searcher.FixStarting(mode, filter, this.SelectedRow, field, subfield, this.currentTextField, this.secondaryCulture, false);
			return searcher;
		}

		protected string ReplaceDo(Searcher searcher, string replace)
		{
			//	Effectue le remplacement.
			//	Retourne la chaîne complète contenant le remplacement.
			string cultureName;
			ResourceAccess.FieldType fieldType;
			this.access.SearcherIndexToAccess(searcher.Field, this.secondaryCulture, out cultureName, out fieldType);
			ResourceAccess.Field field = this.access.GetField(searcher.Row, cultureName, fieldType);
			System.Diagnostics.Debug.Assert(field != null);

			string text = "";

			if (field.FieldType == ResourceAccess.Field.Type.String)
			{
				text = field.String;
				text = text.Remove(searcher.Index, searcher.Length);
				text = text.Insert(searcher.Index, replace);

				if (fieldType == ResourceAccess.FieldType.Name)
				{
					string initialName = field.String;

					//	Met un nom dont on est certain qu'il est valide et qu'il n'existe pas !
					this.access.SetField(searcher.Row, cultureName, fieldType, new ResourceAccess.Field("wXrfGjkleWEuio"));

					string err = this.access.CheckNewName(null, ref text);
					if (err != null)
					{
						this.access.SetField(searcher.Row, cultureName, fieldType, new ResourceAccess.Field(initialName));

						this.designerApplication.DialogError(err);
						return null;
					}
				}

				this.access.SetField(searcher.Row, cultureName, fieldType, new ResourceAccess.Field(text));
			}

			if (field.FieldType == ResourceAccess.Field.Type.StringCollection)
			{
				string[] array = new string[field.StringCollection.Count];
				field.StringCollection.CopyTo(array, 0);

				text = array[searcher.Subfield];
				text = text.Remove(searcher.Index, searcher.Length);
				text = text.Insert(searcher.Index, replace);
				array[searcher.Subfield] = text;

				this.access.SetField(searcher.Row, cultureName, fieldType, new ResourceAccess.Field(array));
			}

			return text;
		}

		public void DoFilter(string filter, Searcher.SearchingMode mode)
		{
			//	Change le filtre des ressources visibles.
			this.access.SetFilter(filter, mode);

			this.UpdateArray();
			this.SelectedRow = this.access.AccessIndex;
			this.UpdateCommands();
		}

		public void DoAccess(string name)
		{
			//	Change la ressource visible.
			int sel = this.access.AccessIndex;

			if (name == "AccessFirst")  sel = 0;
			if (name == "AccessPrev" )  sel --;
			if (name == "AccessNext" )  sel ++;
			if (name == "AccessLast" )  sel = 1000000;

			this.access.AccessIndex = sel;
			this.SelectedRow = this.access.AccessIndex;
			this.UpdateCommands();

			if (this.currentTextField != null)
			{
				this.currentTextField.SelectAll();
			}
		}

		public void DoModification(string name)
		{
			//	Change la ressource modifiée visible.
			int sel = this.access.AccessIndex;

			if (name == "ModificationAll")
			{
				if (sel == -1)  return;
				this.access.ModificationSetAll(sel);

				this.UpdateArray();
				this.UpdateModificationsState();
				this.UpdateModificationsCulture();
				this.UpdateCommands();
			}
			else if (name == "ModificationClear")
			{
				if (sel == -1)  return;
				this.access.ModificationClear(sel, this.secondaryCulture);

				this.UpdateArray();
				this.UpdateModificationsState();
				this.UpdateModificationsCulture();
				this.UpdateCommands();
			}
			else
			{
				if (sel == -1)
				{
					sel = (name == "ModificationPrev") ? 0 : this.access.AccessCount-1;
				}

				bool secondary = false;
				int dir = (name == "ModificationPrev") ? -1 : 1;

				for (int i=0; i<this.access.AccessCount; i++)
				{
					sel += dir;

					if (sel >= this.access.AccessCount)
					{
						sel = 0;
					}

					if (sel < 0)
					{
						sel = this.access.AccessCount-1;
					}

					ResourceAccess.ModificationState state = this.access.GetModification(sel, null);
					if (state != ResourceAccess.ModificationState.Normal)
					{
						break;
					}

					if (this.secondaryCulture != null)
					{
						state = this.access.GetModification(sel, this.secondaryCulture);
						if (state != ResourceAccess.ModificationState.Normal)
						{
							secondary = true;
							break;
						}
					}
				}

				this.access.AccessIndex = sel;
				this.SelectedRow = sel;
				this.SelectEdit(secondary);
			}
		}

		public void DoDelete()
		{
			//	Supprime la ressource sélectionnée.
			if (this.IsDeleteOrDuplicateForViewer)
			{
				this.DoCommand("PanelDelete");
			}
			else
			{
				ResourceAccess.Field field = this.access.GetField(this.access.AccessIndex, null, ResourceAccess.FieldType.Name);
				string question = string.Format(Res.Strings.Dialog.Delete.Question, field.String);
				if (this.designerApplication.DialogQuestion(question) == Epsitec.Common.Dialogs.DialogResult.Yes)
				{
					this.PrepareForDelete();
					this.access.Delete();

					this.UpdateArray();
					this.UpdateViewer(Changing.Delete);
					this.SelectedRow = this.access.AccessIndex;
					this.UpdateCommands();
				}
			}
		}

		public void DoDuplicate(bool duplicate)
		{
			//	Duplique la ressource sélectionnée.
			if (this.IsDeleteOrDuplicateForViewer)
			{
				if (duplicate)
				{
					this.DoCommand("PanelDuplicate");
				}
				else
				{
					//	Rien d'intelligent à faire pour l'instant !
				}
			}
			else
			{
				string newName = "New";
				if (this.access.AccessIndex >= 0 && this.access.AccessIndex < this.access.AccessCount)
				{
					ResourceAccess.Field field = this.access.GetField(this.access.AccessIndex, null, ResourceAccess.FieldType.Name);
					newName = field.String;
				}

				newName = this.access.GetDuplicateName(newName);
				if (this.access.Duplicate(newName, duplicate))
				{
					this.UpdateArray();
					this.SelectedRow = this.access.AccessIndex;
					this.UpdateEdit();
					this.access.SetLocalDirty();  // nécessaire, car UpdateEdit à fait un ClearLocalDirty !
					this.UpdateCommands();
					this.designerApplication.LocatorFix();

					if (this.currentTextField != null)
					{
						this.currentTextField.SelectAll();
						this.currentTextField.Focus();
					}
				}
			}
		}

		public void DoCopyToModule(Module destModule)
		{
			//	Copie la ressource dans un autre module.
			if (!this.IsDeleteOrDuplicateForViewer)
			{
				ResourceAccess.Field field = this.access.GetField(this.access.AccessIndex, null, ResourceAccess.FieldType.Name);

				//	TODO: à finir...
				//destModule.AccessStrings.SetField();
			}
		}

		public void DoNewCulture()
		{
			//	Crée une nouvelle culture.
			string name = this.designerApplication.DlgNewCulture(this.access);
			if (name == null)  return;
			this.access.CreateCulture(name);

			this.UpdateCultures();
			this.UpdateArray();
			this.UpdateEdit();
			this.UpdateModificationsState();
			this.UpdateModificationsCulture();
			this.UpdateClientGeometry();
			this.UpdateCommands();
		}

		public void DoDeleteCulture()
		{
			//	Supprime la culture courante.
			System.Globalization.CultureInfo culture = this.access.GetCulture(this.secondaryCulture);
			string question = string.Format(Res.Strings.Dialog.DeleteCulture.Question, Misc.CultureName(culture));
			var result = this.designerApplication.DialogQuestion (question);
			if (result != Epsitec.Common.Dialogs.DialogResult.Yes)  return;

			this.access.DeleteCulture(this.secondaryCulture);

			this.UpdateCultures();
			if (this.secondaryCulture != null)
			{
				this.UpdateSelectedCulture();
			}
			this.UpdateArray();
			this.UpdateEdit();
			this.UpdateModificationsState();
			this.UpdateModificationsCulture();
			this.UpdateClientGeometry();
			this.UpdateCommands();
		}

		public void DoClipboard(string name)
		{
			//	Effectue une action avec le bloc-notes.
			if (this.currentTextField == null)
			{
				return;
			}

			if (name == "Cut")
			{
				this.currentTextField.ProcessCut();
			}

			if (name == "Copy")
			{
				this.currentTextField.ProcessCopy();
			}

			if (name == "Paste")
			{
				this.currentTextField.ProcessPaste();
			}
		}

		public void DoFont(string name)
		{
			//	Effectue une modification de typographie.
			if (this.currentTextField == null)
			{
				return;
			}

			if (name == "FontBold")
			{
				this.currentTextField.TextNavigator.SelectionBold = !this.currentTextField.TextNavigator.SelectionBold;
			}

			if (name == "FontItalic")
			{
				this.currentTextField.TextNavigator.SelectionItalic = !this.currentTextField.TextNavigator.SelectionItalic;
			}

			if (name == "FontUnderline")
			{
				this.currentTextField.TextNavigator.SelectionUnderline = !this.currentTextField.TextNavigator.SelectionUnderline;
			}

			//?this.HandleTextChanged(this.currentTextField);
		}

		public virtual void DoTool(string name)
		{
			//	Choix de l'outil.
			this.context.Tool = name;
			this.UpdateCommands();
		}

		public virtual void DoCommand(string name)
		{
			//	Exécute une commande.
			if (name == "ShowBothCulture")
			{
				Abstract.showPrimaryCulture = true;
				Abstract.showSecondaryCulture = true;
				this.ShowBands();
				return;
			}

			if (name == "ShowPrimaryCulture")
			{
				Abstract.showPrimaryCulture = true;
				Abstract.showSecondaryCulture = false;
				this.ShowBands();
				return;
			}

			if (name == "ShowSecondaryCulture")
			{
				Abstract.showPrimaryCulture = false;
				Abstract.showSecondaryCulture = true;
				this.ShowBands();
				return;
			}

			this.UpdateCommands();
		}


		public virtual int SelectedRow
		{
			//	Ligne sélectionnée dans la table.
			get
			{
				return this.access.CollectionView.CurrentPosition;
			}
			set
			{
				this.access.CollectionView.MoveCurrentToPosition(value);
				this.ShowSelectedRow();
			}
		}


		public virtual void UpdateViewer(Viewers.Changing oper)
		{
			//	Met à jour le visualisateur en cours.
		}

		public virtual string InfoViewerText
		{
			//	Donne le texte d'information sur le visualisateur en cours.
			get
			{
				return "";
			}
		}

		public string InfoAccessText
		{
			//	Donne le texte d'information sur l'accès en cours.
			get
			{
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

				int sel = this.access.AccessIndex;
				if (sel == -1)
				{
					builder.Append("-");
				}
				else
				{
					ResourceAccess.Field field = this.access.GetField(sel, null, ResourceAccess.FieldType.Name);
					if (field != null)
					{
						builder.Append(field.String);
						builder.Append(": ");
						builder.Append((sel+1).ToString());
					}
				}

				builder.Append("/");
				builder.Append(this.access.AccessCount.ToString());

				int total = this.access.TotalCount;
				if (this.access.AccessCount < total)
				{
					builder.Append(" (");
					builder.Append(total.ToString());
					builder.Append(")");
				}

				return builder.ToString();
			}
		}


		protected virtual bool IsDeleteOrDuplicateForViewer
		{
			//	Indique s'il faut aiguiller ici une opération delete ou duplicate.
			get
			{
				return false;
			}
		}

		protected virtual bool HasDeleteOrDuplicate
		{
			get
			{
				return true;
			}
		}

		protected virtual void PrepareForDelete()
		{
			//	Préparation en vue d'une suppression.
		}

		public void UpdateWhenModuleUsed()
		{
			//	Met à jour les ressources lorsque le module est utilisé.
			//	Il faut mettre à jour le contenu de la ressource en cours d'édition,
			//	car un Name peut avoir changé (par exemple dans le tableau Structured).
			this.ClearCache();
			this.UpdateArray();
			this.UpdateEdit();
			this.UpdateCommands();
			this.ShowBands();
		}

		protected virtual void ClearCache()
		{
			//	Force une nouvelle mise à jour lors du prochain Update.
		}

		protected virtual void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			//	TODO: à supprimer le jour où une modification dans la liste ObservableList se rafraichit automatiquement !
			this.ignoreChange = true;
			this.table.ItemPanel.Refresh();
			this.ignoreChange = false;

			this.UpdateTitle();
		}

		public virtual void ShowSelectedRow()
		{
			//	Montre la ressource sélectionnée dans le tableau.
			if (this.table != null)
			{
				int pos = this.access.CollectionView.CurrentPosition;
				UI.ItemView item = this.table.ItemPanel.GetItemView(pos);
				this.table.ItemPanel.Show(item);
			}
		}

		protected virtual void UpdateTitle()
		{
			//	Met à jour le titre en dessus de la zone scrollable.
			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;

			if (item == null)
			{
				this.titleText.Text = "";
				this.titleBox.BackColor = Color.Empty;
			}
			else
			{
				Color backColor = Color.Empty;

				string name = item.FullName;
				string based = null;
				string mode = null;

				if (this.access.ResourceType == ResourceAccess.Type.Forms)
				{
					FormEngine.FormDescription form = this.access.GetForm(item.Id);
					if (form != null)
					{
						string formName = null;
						string entityName = null;

						if (form.IsDelta)
						{
							formName = this.module.AccessForms.GetFormName(form.DeltaBaseFormId);
							backColor = Misc.SourceColor(CultureMapSource.DynamicMerge);
						}

						entityName = this.module.AccessEntities.GetEntityName(form.EntityId);

						if (!string.IsNullOrEmpty(formName))
						{
							if (string.IsNullOrEmpty(entityName))
							{
								based = string.Format(Res.Strings.Viewers.Forms.Title.Delta, formName);
							}
							else
							{
								based = string.Format(Res.Strings.Viewers.Forms.Title.DeltaNormal, formName, entityName);
							}
						}
						else
						{
							if (!string.IsNullOrEmpty(entityName))
							{
								based = string.Format(Res.Strings.Viewers.Forms.Title.Normal, entityName);
							}
						}
					}
				}

				if (this.module.IsPatch)
				{
					CultureMapSource source = this.access.GetCultureMapSource(item);
					backColor = Misc.SourceColor(source);
					mode = Misc.SourceText(source);
					if (!string.IsNullOrEmpty(mode))
					{
						mode = string.Concat(" (", mode, ")");
					}
				}

				this.titleText.Text = string.Concat("<font size=\"150%\">", name, "</font>", based, mode);
				this.titleBox.BackColor = backColor;
			}

			//	S'il existe une fenêtre supplémentaire, affiche son titre.
			if (this.designerApplication.ViewersWindow != null)
			{
				string title = string.Concat(Res.Strings.Application.Title, " - ", this.designerApplication.CurrentModule.ModuleId.Name, " - ", ResourceAccess.TypeDisplayName(this.access.ResourceType));

				if (item != null)
				{
					title = string.Concat(title, " - ", item.FullName);
				}

				this.designerApplication.ViewersWindowUpdate(title, this.HasUsefulViewerWindow);
			}
		}

		protected void UpdateColor()
		{
			//	Met à jour les couleurs dans toutes les bandes.
			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			ResourceAccess.ModificationState state1 = this.access.GetModification(item, this.GetTwoLetters(0));
			ResourceAccess.ModificationState state2 = this.access.GetModification(item, this.GetTwoLetters(1));
			this.ColoriseBands(state1, state2);
		}

		protected virtual void UpdateModificationsState()
		{
			//	Met à jour en fonction des modifications (fonds de couleur, etc).
			this.UpdateColor();
			this.UpdateModificationsCulture();
		}

		protected virtual void UpdateModificationsCulture()
		{
			//	Met à jour les pastilles dans les boutons des cultures.
			if (this.secondaryButtonsCulture == null)  // pas de culture secondaire ?
			{
				return;
			}

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;

			foreach (IconButtonMark button in this.secondaryButtonsCulture)
			{
				ResourceAccess.ModificationState state = this.access.GetModification(item, button.Name);

				if (state == ResourceAccess.ModificationState.Normal)
				{
					button.BulletColor = Color.Empty;
				}
				else
				{
					button.BulletColor = Abstract.GetBackgroundColor(state, 1.0);
				}
			}
		}

		protected virtual void UpdateSelectedCulture()
		{
			//	Sélectionne le bouton correspondant à la culture secondaire.
			if (this.PrimaryColumn != -1)
			{
				this.table.ColumnHeader.SetColumnText(this.PrimaryColumn, this.access.GetCultureName(this.access.GetPrimaryCultureName()));
			}

			if (this.SecondaryColumn != -1)
			{
				this.table.ColumnHeader.SetColumnText(this.SecondaryColumn, this.access.GetCultureName(this.GetTwoLetters(1)));
			}

			if (this.secondaryButtonsCulture == null)
			{
				return;
			}

			for (int i=0; i<this.secondaryButtonsCulture.Length; i++)
			{
				if (this.secondaryButtonsCulture[i].Name == this.GetTwoLetters(1))
				{
					this.secondaryButtonsCulture[i].ActiveState = ActiveState.Yes;
				}
				else
				{
					this.secondaryButtonsCulture[i].ActiveState = ActiveState.No;
				}
			}
		}

		protected void UpdateDisplayMode()
		{
			//	Met à jour le mode d'affichage des bandes.
			for (int i=0; i<this.bands.Count; i++)
			{
				switch (bands[i].bandMode)
				{
					case BandMode.MainSummary:
						this.bands[i].bandContainer.Visibility = !Abstract.mainExtended;
						break;

					case BandMode.MainView:
						this.bands[i].bandContainer.Visibility = Abstract.mainExtended;
						break;

					case BandMode.SuiteSummary:
						this.bands[i].bandContainer.Visibility = !Abstract.suiteExtended;
						break;

					case BandMode.SuiteView:
						this.bands[i].bandContainer.Visibility = Abstract.suiteExtended;
						break;
				}
			}
		}

		protected void UpdateFieldName(AbstractTextField edit, int sel)
		{
			//	Change le 'Name' d'une ressource, en gérant les diverses impossibilités.
			sel = this.access.SortDefer(sel);

			string editedName = edit.Text;
			string prefix = null;
			string initialName = this.access.GetField(sel, null, ResourceAccess.FieldType.Name).String;

			CultureMap item = this.access.CollectionView.Items[sel] as CultureMap;
			if (item != null && !string.IsNullOrEmpty(item.Prefix))
			{
				//	Si on est sur sur ressource de type 'Field', il faut tenir compte du préfixe.
				//	Par exemple, le champ Client de l'entité Facture a un préfixe 'Facture' et il
				//	faut comparer le nom 'Facture.Client', pour éviter de confondre avec les champs
				//	d'une autre entité comme 'Adresse.Client'.
				prefix = item.Prefix;
			}

			//	Met un nom dont on est certain qu'il est valide et qu'il n'existe pas !
			this.access.SetField(sel, null, ResourceAccess.FieldType.Name, new ResourceAccess.Field("wXrfGjkleWEuio"));

			string err = this.access.CheckNewName(prefix, ref editedName);
			if (err != null)
			{
				this.access.SetField(sel, null, ResourceAccess.FieldType.Name, new ResourceAccess.Field(initialName));
				this.access.SortUndefer();

				this.ignoreChange = true;
				edit.Text = initialName;
				edit.SelectAll();
				this.ignoreChange = false;

				this.designerApplication.DialogError(err);
				return;
			}

			this.ignoreChange = true;
			edit.Text = editedName;
			edit.SelectAll();
			this.ignoreChange = false;

			this.access.SetField(sel, null, ResourceAccess.FieldType.Name, new ResourceAccess.Field(editedName));

			this.access.AccessIndex = this.access.SortUndefer(sel);
			this.UpdateArray();
			this.ShowSelectedRow();
		}

		protected virtual Widget CultureParentWidget
		{
			//	Retourne le parent à utiliser pour les boutons des cultures.
			get
			{
				return this;
			}
		}

		protected virtual void UpdateCultures()
		{
			//	Met à jour les boutons des cultures en fonction des cultures existantes.
			if (this.secondaryButtonsCulture != null)
			{
				foreach (IconButtonMark button in this.secondaryButtonsCulture)
				{
					button.Clicked -= this.HandleButtonSecondaryCultureClicked;
					button.Dispose();
				}
				this.secondaryButtonsCulture = null;
			}

			this.primaryButtonCulture.Text = string.Format(Res.Strings.Viewers.Strings.Reference, this.access.GetCultureName(this.access.GetPrimaryCultureName()));

			List<string> list = this.access.GetSecondaryCultureNames();  // TODO:
			if (list.Count > 0)
			{
				this.secondaryButtonsCulture = new IconButtonMark[list.Count];
				for (int i=0; i<list.Count; i++)
				{
					this.secondaryButtonsCulture[i] = new IconButtonMark(this.secondaryButtonsCultureGroup);
					this.secondaryButtonsCulture[i].ButtonStyle = ButtonStyle.ActivableIcon;
					this.secondaryButtonsCulture[i].MarkDisposition = ButtonMarkDisposition.Below;
					this.secondaryButtonsCulture[i].MarkLength = 5;
					this.secondaryButtonsCulture[i].Name = list[i];
					this.secondaryButtonsCulture[i].Text = this.access.GetCultureName(list[i]);
					this.secondaryButtonsCulture[i].AutoFocus = false;
					this.secondaryButtonsCulture[i].Dock = DockStyle.Fill;
					this.secondaryButtonsCulture[i].Margins = new Margins(0, (i==list.Count-1)?1:0, 0, 0);
					this.secondaryButtonsCulture[i].Clicked += this.HandleButtonSecondaryCultureClicked;
				}

				this.TwoLettersSecondaryCulture = list[0];
			}
			else
			{
				this.TwoLettersSecondaryCulture = null;
			}
		}

		protected virtual void SelectEdit(bool secondary)
		{
		}

		protected virtual void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			if (this.primarySummary != null)
			{
				this.primarySummary.Text = this.GetSummary(this.GetTwoLetters(0));
				this.secondarySummary.Text = this.GetSummary(this.GetTwoLetters(1));
			}

			bool readOlny = this.access.IsNameReadOnly(this.access.AccessIndex);
			this.labelEdit.Enable = !this.designerApplication.IsReadonly && !readOlny;

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			if (item != null)
			{
				if (!this.labelEdit.IsEditing)
				{
					if (this.labelEdit.Text != item.Name)
					{
						this.labelEdit.Text = item.Name;
						this.labelEdit.SelectAll();
					}
				}
			}

			this.ignoreChange = iic;
		}

		public virtual void UpdateList()
		{
			//	Met à jour le contenu de la liste de gauche.
			this.access.CollectionView.Refresh();
			this.UpdateTitle();
		}

		public virtual void Update()
		{
			//	Met à jour le contenu du Viewer.
			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		protected void UpdateAll()
		{
			this.UpdateDisplayMode();
			this.UpdateCultures();
			this.UpdateTitle();
			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		public void UpdateCommands()
		{
			//	Met à jour les commandes en fonction de la ressource sélectionnée.
			int sel = this.access.AccessIndex;
			int count = this.access.AccessCount;
			bool build = this.module.Mode == DesignerMode.Build;
			bool patch = this.module.IsPatch;
			bool reference = this.access.GetCultureMapSource(sel) == CultureMapSource.ReferenceModule;

			this.UpdateCommandTool("ToolSelect");
			this.UpdateCommandTool("ToolGlobal");
			this.UpdateCommandTool("ToolGrid");
			this.UpdateCommandTool("ToolEdit");
			this.UpdateCommandTool("ToolZoom");
			this.UpdateCommandTool("ToolHand");
			this.UpdateCommandTool("ObjectVLine");
			this.UpdateCommandTool("ObjectHLine");
			this.UpdateCommandTool("ObjectSquareButton");
			this.UpdateCommandTool("ObjectRectButton");
			this.UpdateCommandTool("ObjectText");
			this.UpdateCommandTool("ObjectTable");
			this.UpdateCommandTool("ObjectStatic");
			this.UpdateCommandTool("ObjectGroup");
			this.UpdateCommandTool("ObjectGroupFrame");
			this.UpdateCommandTool("ObjectGroupBox");
			this.UpdateCommandTool("ObjectPanel");

			this.GetCommandState("Save").Enable = this.module.IsGlobalDirty;
			this.GetCommandState("SaveAs").Enable = true;

			if (this.designerApplication.IsReadonly || this is Panels || this is Forms)
			{
				this.GetCommandState("NewCulture").Enable = false;
				this.GetCommandState("DeleteCulture").Enable = false;
			}
			else
			{
				this.GetCommandState("NewCulture").Enable = (this.access.CultureCount < Misc.Cultures.Length);
				this.GetCommandState("DeleteCulture").Enable = (this.access.CultureCount > 1);
			}

			bool search = this.designerApplication.DialogSearch.IsActionsEnabled;
			this.GetCommandState("Search").Enable = true;
			this.GetCommandState("SearchPrev").Enable = search;
			this.GetCommandState("SearchNext").Enable = search;

			this.GetCommandState("Filter").Enable = true;

			this.GetCommandState("AccessFirst").Enable = (sel != -1 && sel > 0);
			this.GetCommandState("AccessPrev").Enable = (sel != -1 && sel > 0);
			this.GetCommandState("AccessLast").Enable = (sel != -1 && sel < count-1);
			this.GetCommandState("AccessNext").Enable = (sel != -1 && sel < count-1);

			this.GetCommandState("DisplayHorizontal").Enable = true;
			this.GetCommandState("DisplayVertical").Enable = true;
			this.GetCommandState("DisplayFullScreen").Enable = true;

			//?this.GetCommandState("EditLocked").ActiveState = this.designerApplication.IsEditLocked ? ActiveState.Yes : ActiveState.No;
			this.GetCommandState("EditOk").Enable = this.module.IsLocalDirty;
			this.GetCommandState("EditCancel").Enable = this.module.IsLocalDirty;

			if (!this.IsDeleteOrDuplicateForViewer)
			{
				if (this.HasDeleteOrDuplicate && !this.designerApplication.IsReadonly)
				{
					this.GetCommandState("Delete").Enable = (sel != -1 && build && (!patch || !reference));
					this.GetCommandState("Create").Enable = (build);
					this.GetCommandState("Duplicate").Enable = (sel != -1 && build);
				}
				else
				{
					this.GetCommandState("Delete").Enable = false;
					this.GetCommandState("Create").Enable = false;
					this.GetCommandState("Duplicate").Enable = false;
				}
				this.GetCommandState("CopyToModule").Enable = (sel != -1 && build && !this.designerApplication.IsReadonly);
			}

			if (this is Panels || this is Forms)
			{
				this.GetCommandState("ModificationPrev").Enable = false;
				this.GetCommandState("ModificationNext").Enable = false;
				this.GetCommandState("ModificationAll").Enable = false;
				this.GetCommandState("ModificationClear").Enable = false;
			}
			else
			{
				bool all = false;
				bool modified = false;
				if (sel != -1 && this.secondaryCulture != null)
				{
					all = this.access.IsModificationAll(sel);
					ResourceAccess.ModificationState state = this.access.GetModification(sel, this.secondaryCulture);
					modified = (state == ResourceAccess.ModificationState.Modified);
				}

				this.GetCommandState("ModificationPrev").Enable = true;
				this.GetCommandState("ModificationNext").Enable = true;
				this.GetCommandState("ModificationAll").Enable = all;
				this.GetCommandState("ModificationClear").Enable = modified;
			}

			if (this.designerApplication.IsReadonly || this is Panels || this is Forms)
			{
				this.GetCommandState("FontBold").Enable = false;
				this.GetCommandState("FontItalic").Enable = false;
				this.GetCommandState("FontUnderline").Enable = false;
				this.GetCommandState("DesignerGlyphs").Enable = false;
			}
			else
			{
				this.GetCommandState("FontBold").Enable = (sel != -1);
				this.GetCommandState("FontItalic").Enable = (sel != -1);
				this.GetCommandState("FontUnderline").Enable = (sel != -1);
				this.GetCommandState("DesignerGlyphs").Enable = (sel != -1);
			}

			if (this is Panels)
			{
				int objSelected, objCount;
				bool isRoot;
				Panels panels = this as Panels;
				panels.PanelEditor.GetSelectionInfo(out objSelected, out objCount, out isRoot);

				if (this.IsDeleteOrDuplicateForViewer)
				{
					this.GetCommandState("Delete").Enable = (objSelected != 0);
					this.GetCommandState("Create").Enable = false;
					this.GetCommandState("Duplicate").Enable = false;
					this.GetCommandState("CopyToModule").Enable = false;
				}

				this.GetCommandState("PanelDeselectAll").Enable = (objSelected != 0);
				this.GetCommandState("PanelSelectAll").Enable = (objSelected < objCount);
				this.GetCommandState("PanelSelectInvert").Enable = (objCount > 0);
				this.GetCommandState("PanelSelectRoot").Enable = !isRoot;
				this.GetCommandState("PanelSelectParent").Enable = (objCount > 0 && !isRoot);

				this.GetCommandState("PanelShowGrid").Enable = true;
				this.GetCommandState("PanelShowConstrain").Enable = true;
				this.GetCommandState("PanelShowAttachment").Enable = true;
				this.GetCommandState("PanelShowExpand").Enable = true;
				this.GetCommandState("PanelShowZOrder").Enable = true;
				this.GetCommandState("PanelShowTabIndex").Enable = true;
				this.GetCommandState("PanelRun").Enable = (sel != -1);
				this.GetCommandState("PanelShowGrid").ActiveState = this.context.ShowGrid ? ActiveState.Yes : ActiveState.No;
				this.GetCommandState("PanelShowConstrain").ActiveState = this.context.ShowConstrain ? ActiveState.Yes : ActiveState.No;
				this.GetCommandState("PanelShowAttachment").ActiveState = this.context.ShowAttachment ? ActiveState.Yes : ActiveState.No;
				this.GetCommandState("PanelShowExpand").ActiveState = this.context.ShowExpand ? ActiveState.Yes : ActiveState.No;
				this.GetCommandState("PanelShowZOrder").ActiveState = this.context.ShowZOrder ? ActiveState.Yes : ActiveState.No;
				this.GetCommandState("PanelShowTabIndex").ActiveState = this.context.ShowTabIndex ? ActiveState.Yes : ActiveState.No;

				this.GetCommandState("MoveLeft").Enable = (objSelected != 0);
				this.GetCommandState("MoveRight").Enable = (objSelected != 0);
				this.GetCommandState("MoveDown").Enable = (objSelected != 0);
				this.GetCommandState("MoveUp").Enable = (objSelected != 0);

				this.GetCommandState("AlignLeft").Enable = (objSelected >= 2);
				this.GetCommandState("AlignCenterX").Enable = (objSelected >= 2);
				this.GetCommandState("AlignRight").Enable = (objSelected >= 2);
				this.GetCommandState("AlignTop").Enable = (objSelected >= 2);
				this.GetCommandState("AlignCenterY").Enable = (objSelected >= 2);
				this.GetCommandState("AlignBottom").Enable = (objSelected >= 2);
				this.GetCommandState("AlignBaseLine").Enable = (objSelected >= 2);
				this.GetCommandState("AdjustWidth").Enable = (objSelected >= 2);
				this.GetCommandState("AdjustHeight").Enable = (objSelected >= 2);
				this.GetCommandState("AlignGrid").Enable = (objSelected != 0);

				this.GetCommandState("PanelOrderUpAll").Enable = (objSelected != 0 && objCount >= 2);
				this.GetCommandState("PanelOrderDownAll").Enable = (objSelected != 0 && objCount >= 2);
				this.GetCommandState("PanelOrderUpOne").Enable = (objSelected != 0 && objCount >= 2);
				this.GetCommandState("PanelOrderDownOne").Enable = (objSelected != 0 && objCount >= 2);

				this.GetCommandState("TabIndexClear").Enable = (objSelected != 0);
				this.GetCommandState("TabIndexRenum").Enable = (objCount != 0 && !this.designerApplication.IsReadonly);
				this.GetCommandState("TabIndexLast").Enable = (objSelected != 0);
				this.GetCommandState("TabIndexPrev").Enable = (objSelected != 0);
				this.GetCommandState("TabIndexNext").Enable = (objSelected != 0);
				this.GetCommandState("TabIndexFirst").Enable = (objSelected != 0);
			}
			else if (this is Forms)
			{
				int objSelected, objCount;
				bool isRoot;
				Forms forms = this as Forms;
				forms.FormEditor.GetSelectionInfo(out objSelected, out objCount, out isRoot);

				if (this.IsDeleteOrDuplicateForViewer)
				{
					this.GetCommandState("Delete").Enable = false;
					this.GetCommandState("Create").Enable = false;
					this.GetCommandState("Duplicate").Enable = false;
					this.GetCommandState("CopyToModule").Enable = false;
				}

				this.GetCommandState("PanelDeselectAll").Enable = (objSelected != 0);
				this.GetCommandState("PanelSelectAll").Enable = (objSelected < objCount);
				this.GetCommandState("PanelSelectInvert").Enable = (objCount > 0);
				this.GetCommandState("PanelSelectRoot").Enable = !isRoot;
				this.GetCommandState("PanelSelectParent").Enable = (objCount > 0 && !isRoot);

				this.GetCommandState("PanelShowGrid").Enable = true;
				this.GetCommandState("PanelShowConstrain").Enable = false;
				this.GetCommandState("PanelShowAttachment").Enable = false;
				this.GetCommandState("PanelShowExpand").Enable = false;
				this.GetCommandState("PanelShowZOrder").Enable = false;
				this.GetCommandState("PanelShowTabIndex").Enable = true;
				this.GetCommandState("PanelRun").Enable = (sel != -1);
				this.GetCommandState("PanelShowGrid").ActiveState = this.context.ShowGrid ? ActiveState.Yes : ActiveState.No;
				this.GetCommandState("PanelShowConstrain").ActiveState = ActiveState.No;
				this.GetCommandState("PanelShowAttachment").ActiveState = ActiveState.No;
				this.GetCommandState("PanelShowExpand").ActiveState = ActiveState.No;
				this.GetCommandState("PanelShowZOrder").ActiveState = ActiveState.No;
				this.GetCommandState("PanelShowTabIndex").ActiveState = this.context.ShowTabIndex ? ActiveState.Yes : ActiveState.No;

				this.GetCommandState("MoveLeft").Enable = false;
				this.GetCommandState("MoveRight").Enable = false;
				this.GetCommandState("MoveDown").Enable = false;
				this.GetCommandState("MoveUp").Enable = false;

				this.GetCommandState("AlignLeft").Enable = false;
				this.GetCommandState("AlignCenterX").Enable = false;
				this.GetCommandState("AlignRight").Enable = false;
				this.GetCommandState("AlignTop").Enable = false;
				this.GetCommandState("AlignCenterY").Enable = false;
				this.GetCommandState("AlignBottom").Enable = false;
				this.GetCommandState("AlignBaseLine").Enable = false;
				this.GetCommandState("AdjustWidth").Enable = false;
				this.GetCommandState("AdjustHeight").Enable = false;
				this.GetCommandState("AlignGrid").Enable = false;

				this.GetCommandState("PanelOrderUpAll").Enable = false;
				this.GetCommandState("PanelOrderDownAll").Enable = false;
				this.GetCommandState("PanelOrderUpOne").Enable = false;
				this.GetCommandState("PanelOrderDownOne").Enable = false;

				this.GetCommandState("TabIndexClear").Enable = false;
				this.GetCommandState("TabIndexRenum").Enable = false;
				this.GetCommandState("TabIndexLast").Enable = false;
				this.GetCommandState("TabIndexPrev").Enable = false;
				this.GetCommandState("TabIndexNext").Enable = false;
				this.GetCommandState("TabIndexFirst").Enable = false;
			}
			else
			{
				this.GetCommandState("PanelDeselectAll").Enable = false;
				this.GetCommandState("PanelSelectAll").Enable = false;
				this.GetCommandState("PanelSelectInvert").Enable = false;
				this.GetCommandState("PanelSelectRoot").Enable = false;
				this.GetCommandState("PanelSelectParent").Enable = false;

				this.GetCommandState("PanelShowGrid").Enable = false;
				this.GetCommandState("PanelShowConstrain").Enable = false;
				this.GetCommandState("PanelShowAttachment").Enable = false;
				this.GetCommandState("PanelShowExpand").Enable = false;
				this.GetCommandState("PanelShowZOrder").Enable = false;
				this.GetCommandState("PanelShowTabIndex").Enable = false;
				this.GetCommandState("PanelRun").Enable = false;

				this.GetCommandState("MoveLeft").Enable = false;
				this.GetCommandState("MoveRight").Enable = false;
				this.GetCommandState("MoveDown").Enable = false;
				this.GetCommandState("MoveUp").Enable = false;

				this.GetCommandState("AlignLeft").Enable = false;
				this.GetCommandState("AlignCenterX").Enable = false;
				this.GetCommandState("AlignRight").Enable = false;
				this.GetCommandState("AlignTop").Enable = false;
				this.GetCommandState("AlignCenterY").Enable = false;
				this.GetCommandState("AlignBottom").Enable = false;
				this.GetCommandState("AlignBaseLine").Enable = false;
				this.GetCommandState("AdjustWidth").Enable = false;
				this.GetCommandState("AdjustHeight").Enable = false;
				this.GetCommandState("AlignGrid").Enable = false;

				this.GetCommandState("PanelOrderUpAll").Enable = false;
				this.GetCommandState("PanelOrderDownAll").Enable = false;
				this.GetCommandState("PanelOrderUpOne").Enable = false;
				this.GetCommandState("PanelOrderDownOne").Enable = false;

				this.GetCommandState("TabIndexClear").Enable = false;
				this.GetCommandState("TabIndexRenum").Enable = false;
				this.GetCommandState("TabIndexLast").Enable = false;
				this.GetCommandState("TabIndexPrev").Enable = false;
				this.GetCommandState("TabIndexNext").Enable = false;
				this.GetCommandState("TabIndexFirst").Enable = false;
			}

			this.UpdateUndoRedoCommands();

			this.designerApplication.UpdateInfoCurrentModule();
			this.designerApplication.UpdateInfoAccess();
			this.designerApplication.UpdateInfoViewer();
		}

		protected void UpdateUndoRedoCommands()
		{
			//	Met à jour les commandes undo/redo.
			this.GetCommandState("Undo").Enable = this.IsUndoEnable;
			this.GetCommandState("Redo").Enable = this.IsRedoEnable;
			this.GetCommandState("UndoRedoList").Enable = this.IsUndoRedoListEnable;
		}

		protected void UpdateCommandTool(string name)
		{
			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;

			if (this.designerApplication.IsReadonly || item == null)  // mode bloqué ou aucune ressource ?
			{
				this.GetCommandState(name).ActiveState = ActiveState.No;
				this.GetCommandState(name).Enable = false;
			}
			else
			{
				this.GetCommandState(name).ActiveState = (this.context.Tool == name) ? ActiveState.Yes : ActiveState.No;
				this.GetCommandState(name).Enable = true;
			}
		}

		protected CommandState GetCommandState(string command)
		{
			return this.designerApplication.GetCommandState(command);
		}

		protected virtual string GetSummary(string twoLettersCulture)
		{
			//	Retourne le texte résumé de la ressource sélectionnée.
			return null;
		}


		#region UndoRedo
		public virtual void Undo()
		{
			//	Annule la dernière action.
		}

		public virtual void Redo()
		{
			//	Refait la dernière action.
		}

		public virtual VMenu UndoRedoCreateMenu(Support.EventHandler<MessageEventArgs> message)
		{
			//	Crée le menu undo/redo.
			return null;
		}

		public virtual void UndoRedoGoto(int index)
		{
			//	Annule ou refait quelques actions, selon le menu.
		}

		public virtual void UndoFlush()
		{
			//	Les commandes annuler/refaire ne seront plus possibles.
		}

		protected virtual bool IsUndoEnable
		{
			//	Retourne true si la commande "Undo" doit être active.
			get
			{
				return false;
			}
		}

		protected virtual bool IsRedoEnable
		{
			//	Retourne true si la commande "Redo" doit être active.
			get
			{
				return false;
			}
		}

		protected virtual bool IsUndoRedoListEnable
		{
			//	Retourne true si la commande "UndoRedoList" pour le menu doit être active.
			get
			{
				return false;
			}
		}
		#endregion


		protected void SetValue(CultureMap item, StructuredData data, Druid id, object value, bool update)
		{
			//	Méthode appelée pour modifier un champ.
			ResourceAccess.SetStructuredDataValue(this.access.Accessor, item, data, id.ToString(), value);
			this.access.SetLocalDirty();
			this.UpdateColor();
			this.UpdateEdit();
		}


		protected void SetTextField(MyWidgets.StringCollection collection, IList<string> list)
		{
			if (list == null)
			{
				collection.Enable = false;
				collection.Collection = null;
			}
			else
			{
				collection.Enable = true;
				collection.Collection = new List<string>(list);
			}
		}
		


		public virtual bool Terminate(bool soft)
		{
			//	Termine le travail sur une ressource, avant de passer à une autre.
			//	Si soft = true, on sérialise temporairement sans poser de question.
			//	Retourne false si l'utilisateur a choisi "annuler".
			if (!soft)
			{
				this.UndoFlush();
			}

			if (!soft && this.access.IsLocalDirty)
			{
				this.PersistChanges();
			}
			
			return true;
		}

		protected virtual void PersistChanges()
		{
			//	Accepte les changements effectués dans les ressources.
			if (this.access.IsLocalDirty)
			{
				this.access.PersistChanges();
				this.access.ClearLocalDirty();
				this.UpdateList();  // met à jour la liste de gauche avec les données modifiées
			}
		}

		public void RevertChanges()
		{
			//	Annule les changements effectués dans les ressources.
			this.UndoFlush();

			if (this.access.IsLocalDirty)
			{
				if (this.access.ResourceType == ResourceAccess.Type.Entities)
				{
					this.module.AccessFields.RevertChanges();
					this.module.AccessFields.ClearLocalDirty();
				}

				this.access.RevertChanges();
				this.access.ClearLocalDirty();
				this.UpdateList();  // met à jour la liste de gauche avec les données modifiées
				this.Update();  // met à jour la partie éditable centrale avec les données initiales
			}
		}


		protected static Color GetBackgroundColor(ResourceAccess.ModificationState state, double intensity)
		{
			//	Donne une couleur pour un fond de panneau.
			if (intensity == 0.0)
			{
				return Color.Empty;
			}

			switch (state)
			{
				case ResourceAccess.ModificationState.Empty:
					return Color.FromAlphaRgb(intensity, 0.91, 0.40, 0.40);  // rouge

				case ResourceAccess.ModificationState.Modified:
					return Color.FromAlphaRgb(intensity, 0.91, 0.81, 0.41);  // jaune

				default:
					IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
					Color cap = adorner.ColorCaption;
					return Color.FromAlphaRgb(intensity, 0.4+cap.R*0.6, 0.4+cap.G*0.6, 0.4+cap.B*0.6);
			}
		}


		protected virtual void TextFieldToIndex(AbstractTextField textField, out int field, out int subfield)
		{
			//	Cherche les index correspondant à un texte éditable.
			field = -1;
			subfield = -1;
		}

		protected virtual AbstractTextField IndexToTextField(int field, int subfield)
		{
			//	Cherche le TextField permettant d'éditer des index.
			return null;
		}

		public static void SearchCreateFilterGroup(AbstractGroup parent, EventHandler handler, ResourceAccess.Type type)
		{
			//	Crée le contenu du groupe 'filtre'.
			switch (type)
			{
				case ResourceAccess.Type.Strings:
					Strings.SearchCreateFilterGroup(parent, handler);
					break;

				case ResourceAccess.Type.Captions:
				case ResourceAccess.Type.Commands:
					Captions.SearchCreateFilterGroup(parent, handler);
					break;
			}
		}

		public static List<int> SearchGetFilterGroup(AbstractGroup parent, ResourceAccess.Type type)
		{
			//	Donne le résultat du groupe 'filtre', sous forme d'une liste des index autorisés.
			List<int> filter = new List<int>();

			foreach (Widget widget in parent.Children)
			{
				if (widget is CheckButton)
				{
					CheckButton check = widget as CheckButton;
					if (check.ActiveState == ActiveState.Yes)
					{
						int index = int.Parse(check.Name);
						filter.Add(index);
					}
				}
			}

			return filter;
		}


		protected virtual double GetColumnWidth(int column)
		{
			//	Retourne la largeur à utiliser pour une colonne de la liste de gauche.
			if (this.IsDisplayModeHorizontal)
			{
				return Abstract.columnWidthHorizontal[column];
			}
			else
			{
				return Abstract.columnWidthVertical[column];
			}
		}

		protected virtual void SetColumnWidth(int column, double value)
		{
			//	Mémorise la largeur à utiliser pour une colonne de la liste de gauche.
			if (this.IsDisplayModeHorizontal)
			{
				Abstract.columnWidthHorizontal[column] = value;
			}
			else
			{
				Abstract.columnWidthVertical[column] = value;
			}
		}

		protected bool IsDisplayModeHorizontal
		{
			//	Retourne true si le mode s'apparentele plus possible à une disposition horizontale normale.
			get
			{
				return (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Horizontal ||
						this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Window);
			}
		}


		#region Band
		protected void CreateBand(out MyWidgets.StackedPanel leftContainer, out MyWidgets.StackedPanel rightContainer, string title, BandMode mode, GlyphShape extendShape, bool isNewSection, double backgroundIntensity)
		{
			//	Crée une bande horizontale avec deux containers gauche/droite pour les
			//	ressources primaire/secondaire.
			FrameBox band = new FrameBox(this.scrollable.Viewport);
			band.Name = "BandForLeftAndRight";
			band.Dock = DockStyle.StackBegin;
			band.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			band.TabIndex = this.tabIndex++;

			leftContainer = new MyWidgets.StackedPanel(band);
			leftContainer.Name = "LeftContainer";
			leftContainer.Title = title;
			leftContainer.IsLeftPart = true;
			leftContainer.IsNewSection = isNewSection;
			leftContainer.ExtendShape = extendShape;
			leftContainer.MinWidth = 100;
			leftContainer.Dock = DockStyle.StackFill;
			leftContainer.TabIndex = this.tabIndex++;
			leftContainer.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			rightContainer = new MyWidgets.StackedPanel(band);
			rightContainer.Name = "RightContainer";
			rightContainer.Title = title;
			rightContainer.IsLeftPart = false;
			rightContainer.IsNewSection = isNewSection;
			rightContainer.ExtendShape = extendShape;
			rightContainer.MinWidth = 100;
			rightContainer.Dock = DockStyle.StackFill;
			rightContainer.TabIndex = this.tabIndex++;
			rightContainer.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.bands.Add(new Band(band, leftContainer, rightContainer, mode, backgroundIntensity));
		}

		protected void CreateBand(out MyWidgets.StackedPanel leftContainer, string title, BandMode mode, GlyphShape extendShape, bool isNewSection, double backgroundIntensity)
		{
			//	Crée une bande horizontale avec un seul container gauche pour la
			//	ressource primaire.
			FrameBox band = new FrameBox(this.scrollable.Viewport);
			band.Name = "BandForLeft";
			band.Dock = DockStyle.StackBegin;
			band.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			band.TabIndex = this.tabIndex++;

			leftContainer = new MyWidgets.StackedPanel(band);
			leftContainer.Name = "LeftContainer";
			leftContainer.Title = title;
			leftContainer.IsLeftPart = true;
			leftContainer.IsNewSection = isNewSection;
			leftContainer.ExtendShape = extendShape;
			leftContainer.MinWidth = 100;
			leftContainer.Dock = DockStyle.StackFill;
			leftContainer.TabIndex = this.tabIndex++;
			leftContainer.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.bands.Add(new Band(band, leftContainer, null, mode, backgroundIntensity));
		}

		protected void ColoriseBands(ResourceAccess.ModificationState state1, ResourceAccess.ModificationState state2)
		{
			//	Colorise toutes les bandes horizontales.
			for (int i=0; i<this.bands.Count; i++)
			{
				MyWidgets.StackedPanel lc = this.bands[i].leftContainer;
				MyWidgets.StackedPanel rc = this.bands[i].rightContainer;

				lc.BackgroundColor = Abstract.GetBackgroundColor(state1, this.bands[i].intensityContainer);

				if (rc != null)
				{
					rc.BackgroundColor = Abstract.GetBackgroundColor(state2, this.bands[i].intensityContainer);
				}
			}

			this.ShowBands();
		}

		protected void ShowBands()
		{
			//	Montre ou cache les parties gauche et droite dans le panneau principal, en fonction
			//	des cultures que l'utilisateur a choisi de voir.
			this.primaryButtonCulture.Visibility = Abstract.showPrimaryCulture;
			this.secondaryButtonsCultureGroup.Visibility = Abstract.showSecondaryCulture;

			foreach (Band band in this.bands)
			{
				MyWidgets.StackedPanel lc = band.leftContainer;
				MyWidgets.StackedPanel rc = band.rightContainer;

				if (rc == null)  // pas de panneau à droite ?
				{
					lc.Visibility = true;  // panneau unique traversant, toujours visible
					lc.IsLeftPart = true;
				}
				else
				{
					lc.Visibility = Abstract.showPrimaryCulture;
					rc.Visibility = Abstract.showSecondaryCulture && (this.GetTwoLetters(1) != null);

					if (lc.Visibility)
					{
						lc.IsLeftPart = true;
						rc.IsLeftPart = false;
					}
					else
					{
						lc.IsLeftPart = false;
						rc.IsLeftPart = true;
					}
				}
			}
		}

		protected struct Band
		{
			public Band(Widget band, MyWidgets.StackedPanel left, MyWidgets.StackedPanel right, BandMode mode, double intensity)
			{
				this.bandContainer = band;
				this.leftContainer = left;
				this.rightContainer = right;
				this.bandMode = mode;
				this.intensityContainer = intensity;
			}

			public Widget						bandContainer;
			public MyWidgets.StackedPanel		leftContainer;
			public MyWidgets.StackedPanel		rightContainer;
			public BandMode						bandMode;
			public double						intensityContainer;
		}
		#endregion


		protected UI.IItemViewFactory ItemViewFactoryGetter(UI.ItemView itemView)
		{
			//	Retourne le "factory" a utiliser pour les éléments représentés dans cet ItemTable/ItemPanel.
			if (itemView.Item == null)
			{
				return null;
			}
			else
			{
				return this.itemViewFactory;
			}
		}

		/// <summary>
		/// Cette classe peuple les colonnes du tableau. Elle résoud tous les types de colonnes, afin
		/// de pouvoir être utilisée par tous les Viewers.
		/// </summary>
		private class ItemViewFactory : UI.AbstractItemViewFactory
		{
			public ItemViewFactory(Abstract owner)
			{
				this.owner = owner;
			}

			protected override Widget CreateElement(string name, UI.ItemPanel panel, UI.ItemView view, UI.ItemViewShape shape)
			{
				CultureMap item = view.Item as CultureMap;

				switch (name)
				{
					case "Name":
						return this.CreateName(item, shape);

					case "Source":
						return this.CreateSource(item, shape);

					case "Type":
						return this.CreateType(item, shape);

					case "Prefix":
						return this.CreatePrefix(item, shape);

					case "Primary":
						return this.CreatePrimary(item, shape);

					case "Secondary":
						return this.CreateSecondary(item, shape);

					case "Druid":
						return this.CreateDruid(item, shape);

					case "Local":
						return this.CreateLocal(item, shape);

					case "Identity":
						return this.CreateIdentity(item, shape);

					case "PatchLevel":
						return this.CreatePatchLevel(item, shape);
				}

				return null;
			}

			private Widget CreateName(CultureMap item, UI.ItemViewShape shape)
			{
				//	Crée le contenu pour le nom de la ressource.
				string text = (shape == UI.ItemViewShape.ToolTip) ? item.FullName : item.Name;
				if (shape == UI.ItemViewShape.ToolTip && string.IsNullOrEmpty(text))
				{
					return null;
				}

				text = TextLayout.ConvertToTaggedText(text);
				return this.CreateItemViewText(item, text, ContentAlignment.MiddleLeft, Color.Empty);
			}

			private Widget CreateSource(CultureMap item, UI.ItemViewShape shape)
			{
				//	Crée le contenu pour la source de la ressource.
				string text = this.owner.GetSourceText(item);
				return this.CreateItemViewText(item, text, ContentAlignment.MiddleCenter, Color.Empty);
			}

			private Widget CreateType(CultureMap item, UI.ItemViewShape shape)
			{
				//	Crée le contenu pour le type de la ressource.
				StructuredData data = item.GetCultureData(Support.Resources.DefaultTwoLetterISOLanguageName);
				object typeCodeValue = data.GetValue(Support.Res.Fields.ResourceBaseType.TypeCode);

				string text = "";
				if (!UndefinedValue.IsUndefinedValue(typeCodeValue) && !UnknownValue.IsUnknownValue(typeCodeValue))
				{
					text = typeCodeValue.ToString();
				}

				if (shape == UI.ItemViewShape.ToolTip && string.IsNullOrEmpty(text))
				{
					return null;
				}

				text = TextLayout.ConvertToTaggedText(text);
				return this.CreateItemViewText(item, text, ContentAlignment.MiddleLeft, Color.Empty);
			}

			private Widget CreatePrefix(CultureMap item, UI.ItemViewShape shape)
			{
				//	Crée le contenu pour le préfixe de la ressource.
				if (shape == UI.ItemViewShape.ToolTip)
				{
					return null;
				}

				string text = TextLayout.ConvertToTaggedText(item.Prefix);
				return this.CreateItemViewText(item, text, ContentAlignment.MiddleLeft, Color.Empty);
			}

			private Widget CreatePrimary(CultureMap item, UI.ItemViewShape shape)
			{
				//	Crée le contenu pour la colonne primaire.
				return this.CreateContent(item, shape, this.owner.GetTwoLetters(0));
			}

			private Widget CreateSecondary(CultureMap item, UI.ItemViewShape shape)
			{
				//	Crée le contenu pour la colonne secondaire.
				return this.CreateContent(item, shape, this.owner.GetTwoLetters(1));
			}
			
			private Widget CreateContent(CultureMap item, UI.ItemViewShape shape, string twoLettersCulture)
			{
				//	Crée le contenu pour une colonne primaire ou secondaire.
				string text = this.owner.GetColumnText(item, twoLettersCulture) ?? ResourceBundle.Field.Null;
				if (shape == UI.ItemViewShape.ToolTip && string.IsNullOrEmpty(text))
				{
					return null;
				}

				if (!string.IsNullOrEmpty(text))
				{
					text = text.Replace("<br/>", ", ");  // pour afficher un texte multi-lignes sur une seule
					text = text.Replace(ResourceBundle.Field.Null, @"<img src=""manifest:Epsitec.Common.Widgets.Images.DefaultValue.icon""/>");  // 
				}

				Color backColor = Color.Empty;
				ResourceAccess.ModificationState state = this.owner.access.GetModification(item, twoLettersCulture);

				if (state != ResourceAccess.ModificationState.Normal)
				{
					backColor = Abstract.GetBackgroundColor(state, 0.7);
				}

				return this.CreateItemViewText(item, text, ContentAlignment.MiddleLeft, backColor);
			}
			
			private Widget CreateDruid(CultureMap item, UI.ItemViewShape shape)
			{
				//	Crée le contenu pour le druid de la ressource.
				string text = this.owner.GetDruidText(item);

				if (shape == UI.ItemViewShape.ToolTip)
				{
					text = string.Concat(text, " ", this.owner.GetLocalText(item), " ", this.owner.GetIdentityText(item));
				}

				if (shape == UI.ItemViewShape.ToolTip && string.IsNullOrEmpty(text))
				{
					return null;
				}

				text = TextLayout.ConvertToTaggedText(text);
				return this.CreateItemViewText(item, text, ContentAlignment.MiddleLeft, Color.Empty);
			}

			private Widget CreateLocal(CultureMap item, UI.ItemViewShape shape)
			{
				//	Crée le contenu pour le numéro local du druid de la ressource.
				if (shape == UI.ItemViewShape.ToolTip)
				{
					return null;
				}

				string text = TextLayout.ConvertToTaggedText(this.owner.GetLocalText(item));
				return this.CreateItemViewText(item, text, ContentAlignment.MiddleRight, Color.Empty);
			}

			private Widget CreateIdentity(CultureMap item, UI.ItemViewShape shape)
			{
				//	Crée le contenu pour l'identité du créateur de la ressource.
				if (shape == UI.ItemViewShape.ToolTip)
				{
					return null;
				}

				string text = TextLayout.ConvertToTaggedText(this.owner.GetIdentityText(item));
				return this.CreateItemViewText(item, text, ContentAlignment.MiddleLeft, Color.Empty);
			}

			private Widget CreatePatchLevel(CultureMap item, UI.ItemViewShape shape)
			{
				//	Crée le contenu pour le niveau de patch du créateur de la ressource.
				if (shape == UI.ItemViewShape.ToolTip)
				{
					return null;
				}

				string text = TextLayout.ConvertToTaggedText(this.owner.GetPatchLevelText(item));
				return this.CreateItemViewText(item, text, ContentAlignment.MiddleRight, Color.Empty);
			}

			private UI.ItemViewText CreateItemViewText(CultureMap item, string text, ContentAlignment alignment, Color backColor)
			{
				//	Crée un ou deux UI.ItemViewText, avec éventuellement un fond coloré.
				//	S'il y a un fond coloré, il faut créer deux widgets, afin que la couleur remplisse toute
				//	la surface, y compris les marges.
				//	Par optimisation, un seul widget est créé s'il n'y a pas de couleur de fond.
				UI.ItemViewText main, st;

				if (backColor.IsEmpty && this.owner.module.IsPatch)  // module de patch ?
				{
					CultureMapSource source = this.owner.access.GetCultureMapSource(item);
					backColor = Misc.SourceColor(source);
				}

				if (backColor.IsEmpty)
				{
					main = st = new UI.ItemViewText();
				}
				else
				{
					main = new UI.ItemViewText();
					main.BackColor = backColor;

					st = new UI.ItemViewText(main);
					st.Dock = DockStyle.Fill;
				}

				st.Margins = new Margins(5, 5, 0, 0);
				st.Text = text;
				st.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				st.ContentAlignment = alignment;
				st.PreferredSize = st.GetBestFitSize();

				return main;
			}


			private Abstract owner;
		}

		
		protected int CompareSource(object a, object b)
		{
			CultureMap itemA = a as CultureMap;
			CultureMap itemB = b as CultureMap;

			string sA = this.GetSourceText(itemA);
			string sB = this.GetSourceText(itemB);

			return sA.CompareTo(sB);
		}

		protected int ComparePrimary(object a, object b)
		{
			CultureMap itemA = a as CultureMap;
			CultureMap itemB = b as CultureMap;

			string sA = this.GetColumnText(itemA, this.GetTwoLetters(0));
			string sB = this.GetColumnText(itemB, this.GetTwoLetters(0));

			return sA.CompareTo(sB);
		}

		protected int CompareSecondary(object a, object b)
		{
			CultureMap itemA = a as CultureMap;
			CultureMap itemB = b as CultureMap;

			string sA = this.GetColumnText(itemA, this.GetTwoLetters(1));
			string sB = this.GetColumnText(itemB, this.GetTwoLetters(1));

			return sA.CompareTo(sB);
		}

		protected int CompareDruid(object a, object b)
		{
			CultureMap itemA = a as CultureMap;
			CultureMap itemB = b as CultureMap;

			string dA = itemA.Id.ToString();
			string dB = itemB.Id.ToString();

			return dA.CompareTo(dB);
		}

		protected int CompareLocal(object a, object b)
		{
			CultureMap itemA = a as CultureMap;
			CultureMap itemB = b as CultureMap;

			int iA = itemA.Id.Local;
			int iB = itemB.Id.Local;

			return iA.CompareTo(iB);
		}

		protected int CompareIdentity(object a, object b)
		{
			CultureMap itemA = a as CultureMap;
			CultureMap itemB = b as CultureMap;

			int iA = itemA.Id.Developer;
			int iB = itemB.Id.Developer;

			return iA.CompareTo(iB);
		}

		protected int ComparePatchLevel(object a, object b)
		{
			CultureMap itemA = a as CultureMap;
			CultureMap itemB = b as CultureMap;

			int iA = itemA.Id.PatchLevel;
			int iB = itemB.Id.PatchLevel;

			return iA.CompareTo(iB);
		}

		public virtual string GetColumnText(CultureMap item, string twoLettersCulture)
		{
			//	Retourne le texte pour une colonne primaire ou secondaire.
			return "";
		}

		public string GetSourceText(CultureMap item)
		{
			//	Retourne le texte pour la source.
			CultureMapSource source = this.access.GetCultureMapSource(item);

			switch (source)
			{
				case CultureMapSource.ReferenceModule:
					return Res.Strings.Viewers.Source.Short.Reference;

				case CultureMapSource.PatchModule:
					return Res.Strings.Viewers.Source.Short.Patch;

				case CultureMapSource.DynamicMerge:
					return Res.Strings.Viewers.Source.Short.Merge;
			}

			return "";
		}

		public string GetDruidText(CultureMap item)
		{
			//	Retourne le texte pour la colonne Druid.
			return item.Id.ToString();
		}

		public string GetLocalText(CultureMap item)
		{
			//	Retourne le texte pour la colonne Local.
			return item.Id.Local.ToString();
		}

		public string GetIdentityText(CultureMap item)
		{
			//	Retourne le texte pour la colonne Identity.
			if (item.Id.DeveloperAndPatchLevel == 0)
			{
				return Res.Strings.Viewers.Identity.God;
			}

			Identity.IdentityCard card = Identity.IdentityRepository.Default.FindIdentityCard(item.Id.Developer);
			if (card != null)
			{
				return card.UserName;
			}

			return string.Format(Res.Strings.Viewers.Identity.Developer, item.Id.Developer.ToString());
		}

		public string GetPatchLevelText(CultureMap item)
		{
			//	Retourne le texte pour la colonne PatchLevel.
			return item.Id.PatchLevel.ToString();
		}


		public void ColorizeResetBox(MyWidgets.ResetBox box, CultureMapSource source, bool usesOriginalData)
		{
			//	Colore la boîte si on est dans un module de patch avec redéfinition de la donnée.
			if (!box.IsPatch || source != CultureMapSource.DynamicMerge || usesOriginalData)
			{
				box.BackColor = Color.Empty;
				box.ResetButton.Enable = false;
			}
			else
			{
				box.BackColor = Misc.SourceColor(CultureMapSource.DynamicMerge);
				box.ResetButton.Enable = !this.designerApplication.IsReadonly;
			}
		}


		protected VMenu CreateCultureMenu()
		{
			//	Crée le petit menu associé au bouton "v" des cultures.
			VMenu menu = new VMenu();
			MenuItem item;

			item = new MenuItem("ShowBothCulture", Misc.GetMenuIconRadioState(Abstract.showPrimaryCulture && Abstract.showSecondaryCulture), "Afficher les deux cultures", "", "ShowBothCulture");
			menu.Items.Add(item);

			item = new MenuItem("ShowPrimaryCulture", Misc.GetMenuIconRadioState(Abstract.showPrimaryCulture && !Abstract.showSecondaryCulture), "Afficher seulement la culture de référence", "", "ShowPrimaryCulture");
			menu.Items.Add(item);

			item = new MenuItem("ShowSecondaryCulture", Misc.GetMenuIconRadioState(!Abstract.showPrimaryCulture && Abstract.showSecondaryCulture), "Afficher seulement la culture secondaire", "", "ShowSecondaryCulture");
			menu.Items.Add(item);

			return menu;
		}

	
		#region Handle methods
		private void HandleCultureMenuButtonClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton "v" pour le menu est cliqué.
			AbstractButton button = sender as AbstractButton;

			VMenu menu = this.CreateCultureMenu();
			menu.Host = button.Window;
			TextFieldCombo.AdjustComboSize(button, menu, false);
			menu.ShowAsComboList(button, Point.Zero, button);
		}

		protected void HandleEditKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appelé lorsqu'une ligne éditable voit son focus changer.
			bool focused = (bool) e.NewValue;

			if (focused)
			{
				this.currentTextField = sender as AbstractTextField;
			}
		}

		private void HandleSplitterDragged(object sender)
		{
			//	Le splitter a été bougé.
			if (this.IsDisplayModeHorizontal)
			{
				Abstract.leftArrayWidth = this.firstPane.ActualWidth;
			}
			else
			{
				Abstract.topArrayHeight = this.firstPane.ActualHeight;
			}
		}

		private void HandleButtonSecondaryCultureClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton pour changer de culture secondaire a été cliqué.
			IconButtonMark button = sender as IconButtonMark;
			this.TwoLettersSecondaryCulture = button.Name;

			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateCommands();
		}

		private void HandleTableSelectionChanged(object sender, UI.ItemPanelSelectionChangedEventArgs e)
		{
			//	La ligne sélectionnée dans le tableau a changé.
			if (this.ignoreChange)
			{
				return;
			}

			if (!this.designerApplication.Terminate())
			{
				e.Cancel = true;  // revient à la sélection précédente
				return;
			}

			this.designerApplication.LocatorFix();

			this.UpdateTitle();
			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		private void HandleTableSizeChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Les dimensions du tableau ont changé.
#if false
			UI.ItemTable table = (UI.ItemTable) sender;
			Drawing.Size size = (Drawing.Size) e.NewValue;

			double width = size.Width - table.GetPanelPadding().Width;
			//?table.ColumnHeader.SetColumnWidth(0, width);

			table.ItemPanel.ItemViewDefaultSize = new Size(width, 20);
#endif
		}

		private void HandleColumnHeaderColumnWidthChanged(object sender, UI.ColumnWidthChangeEventArgs e)
		{
			//	La largeur d'une colonne du tableau a changé.
			this.SetColumnWidth(e.Column, e.NewWidth);
		}

		protected void HandleButtonCompactOrExtendClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton pour changer le mode d'affichage a été cliqué.
			if (!this.designerApplication.Terminate(true))
			{
				return;
			}

			if (sender == this.buttonMainCompactLeft || sender == this.buttonMainCompactRight)
			{
				Abstract.mainExtended = false;
			}

			if (sender == this.buttonMainExtendLeft || sender == this.buttonMainExtendRight)
			{
				Abstract.mainExtended = true;
			}

			if (sender == this.buttonSuiteCompactLeft || sender == this.buttonSuiteCompactRight)
			{
				Abstract.suiteExtended = false;
			}

			if (sender == this.buttonSuiteExtendLeft || sender == this.buttonSuiteExtendRight)
			{
				Abstract.suiteExtended = true;
			}

			this.UpdateDisplayMode();
			this.UpdateEdit();  // pour que le résumé prenne en compte les modifications
		}

		private void HandleTextChanged(object sender)
		{
			//	Un texte éditable a changé.
			if (this.ignoreChange)
			{
				return;
			}

			AbstractTextField edit = sender as AbstractTextField;
			string text = edit.Text;

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;

			if (edit == this.labelEdit)
			{
				this.UpdateFieldName(edit, this.access.CollectionView.CurrentPosition);
				this.access.SetLocalDirty();
			}

			this.UpdateModificationsCulture();
			this.UpdateTitle();
		}

		private void HandleTextRejected(object sender)
		{
			TextFieldEx edit = sender as TextFieldEx;

			if (edit != null)
			{
				edit.RejectEdition();  // TODO: devrait être inutile
			}
		}

		protected void HandleLabelKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appelé lorsque la ligne éditable pour le label voit son focus changer.
			TextFieldEx field = sender as TextFieldEx;
			field.AcceptEdition();
			this.HandleEditKeyboardFocusChanged(sender, e);
		}

		protected void HandleCursorChanged(object sender)
		{
			//	Le curseur a été déplacé dans un texte éditable.
			if (this.ignoreChange)
			{
				return;
			}

			this.lastActionIsReplace = false;
		}
		#endregion


		protected static double					leftArrayWidth = 439;
		protected static double					topArrayHeight = 220;
		protected static bool					mainExtended = false;
		protected static bool					suiteExtended = false;
		protected static bool					showPrimaryCulture = true;
		protected static bool					showSecondaryCulture = true;
		private static double[]					columnWidthHorizontal = { 200, 100, 100, 20, 80, 50, 100, 50 };
		private static double[]					columnWidthVertical = { 250, 300, 300, 20, 80, 50, 100, 50 };

		protected Module						module;
		protected PanelsContext					context;
		protected ResourceAccess				access;
		protected DesignerApplication			designerApplication;
		protected string						secondaryCulture;  // two letters

		private ItemViewFactory					itemViewFactory;

		protected FrameBox						firstPane;
		protected FrameBox						lastPane;
		protected AbstractSplitter				splitter;
		protected UI.ItemTable					table;
		protected MyWidgets.TextFieldExName		labelEdit;

		protected FrameBox						lastGroup;
		protected FrameBox						titleBox;
		protected StaticText					titleText;
		protected Scrollable					scrollable;
		protected List<Band>					bands;

		protected IconButtonMark				primaryButtonCulture;
		protected FrameBox						secondaryButtonsCultureGroup;
		protected IconButtonMark[]				secondaryButtonsCulture;
		protected GlyphButton					cultureMenuButton;

		protected StaticText					primarySummary;
		protected StaticText					secondarySummary;
		protected GlyphButton					buttonMainExtendLeft;
		protected GlyphButton					buttonMainExtendRight;
		protected GlyphButton					buttonMainCompactLeft;
		protected GlyphButton					buttonMainCompactRight;
		protected GlyphButton					buttonSuiteExtendLeft;
		protected GlyphButton					buttonSuiteExtendRight;
		protected GlyphButton					buttonSuiteCompactLeft;
		protected GlyphButton					buttonSuiteCompactRight;

		protected bool							ignoreChange = false;
		protected bool							lastActionIsReplace = false;
		protected AbstractTextField				currentTextField;
		protected int							tabIndex = 1;
	}
}
