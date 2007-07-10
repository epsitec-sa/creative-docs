using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public abstract class Abstract2 : Abstract
	{
		protected enum BandMode
		{
			MainSummary,
			MainView,
			Separator,
			SuiteSummary,
			SuiteView,
		}


		public Abstract2(Module module, PanelsContext context, ResourceAccess access, DesignerApplication designerApplication) : base(module, context, access, designerApplication)
		{
			this.itemViewFactory = new ItemViewFactory(this);
			
			//	Crée les deux volets séparés d'un splitter.
			this.firstPane = new Widget(this);
			this.firstPane.Name = "FirstPane";
			if (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Horizontal)
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
			this.firstPane.Dock = (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Horizontal) ? DockStyle.Left : DockStyle.Top;
			this.firstPane.Padding = new Margins(10, 10, 10, 10);
			this.firstPane.TabIndex = this.tabIndex++;
			this.firstPane.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			this.firstPane.Visibility = (this.designerApplication.DisplayModeState != DesignerApplication.DisplayMode.FullScreen);

			if (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Horizontal)
			{
				this.splitter = new VSplitter(this);
				this.splitter.Dock = DockStyle.Left;
			}
			else
			{
				this.splitter = new HSplitter(this);
				this.splitter.Dock = DockStyle.Top;
			}
			this.splitter.SplitterDragged += new EventHandler(this.HandleSplitterDragged);
			this.splitter.Visibility = (this.designerApplication.DisplayModeState != DesignerApplication.DisplayMode.FullScreen);
			AbstractSplitter.SetAutoCollapseEnable(this.firstPane, true);

			this.lastPane = new Widget(this);
			this.lastPane.Name = "LastPane";
			if (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Horizontal)
			{
				this.lastPane.MinWidth = 200;
			}
			else
			{
				this.lastPane.MinHeight = 50;
			}
			this.lastPane.Dock = DockStyle.Fill;
			this.lastPane.TabIndex = this.tabIndex++;
			this.lastPane.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			
			//	Crée la première partie (gauche ou supérieure).
			this.labelEdit = new MyWidgets.TextFieldExName(this.firstPane);
			this.labelEdit.Name = "LabelEdit";
			this.labelEdit.Margins = new Margins(0, 0, 10, 0);
			this.labelEdit.Dock = DockStyle.Bottom;
			this.labelEdit.ButtonShowCondition = ShowCondition.WhenModified;
			this.labelEdit.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.labelEdit.EditionAccepted += new EventHandler(this.HandleTextChanged);
			this.labelEdit.EditionRejected += new EventHandler(this.HandleTextRejected);
			this.labelEdit.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.labelEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);
			this.labelEdit.TabIndex = this.tabIndex++;
			this.labelEdit.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.labelEdit.Visibility = (this.module.Mode == DesignerMode.Build);
			this.currentTextField = this.labelEdit;

			this.table = new UI.ItemTable(this.firstPane);
			this.table.ItemPanel.CustomItemViewFactoryGetter = this.ItemViewFactoryGetter;
			this.table.Items = this.access.CollectionView;
			this.InitializeTable();
			this.table.HorizontalScrollMode = (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Horizontal) ? UI.ItemTableScrollMode.Linear : UI.ItemTableScrollMode.None;
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
			this.lastGroup = new Widget(this.lastPane);
			this.lastGroup.Padding = new Margins(10, 10, 10, 10);
			this.lastGroup.Dock = DockStyle.Fill;

			Widget sup = new Widget(this.lastGroup);
			sup.Name = "Sup";
			sup.PreferredHeight = 26;
			sup.Padding = new Margins(0, 17, 1, 0);
			sup.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			sup.Dock = DockStyle.Top;
			sup.TabIndex = this.tabIndex++;
			sup.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			
			this.primaryButtonCulture = new IconButtonMark(sup);
			this.primaryButtonCulture.ButtonStyle = ButtonStyle.ActivableIcon;
			this.primaryButtonCulture.SiteMark = ButtonMarkDisposition.Below;
			this.primaryButtonCulture.MarkDimension = 5;
			this.primaryButtonCulture.PreferredHeight = 25;
			this.primaryButtonCulture.ActiveState = ActiveState.Yes;
			this.primaryButtonCulture.AutoFocus = false;
			this.primaryButtonCulture.Margins = new Margins(0, 1, 0, 0);
			this.primaryButtonCulture.Dock = DockStyle.Fill;

			this.secondaryButtonsCultureGroup = new Widget(sup);
			this.secondaryButtonsCultureGroup.Margins = new Margins(1, 0, 0, 0);
			this.secondaryButtonsCultureGroup.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			this.secondaryButtonsCultureGroup.Dock = DockStyle.Fill;
			this.secondaryButtonsCultureGroup.TabIndex = this.tabIndex++;
			this.secondaryButtonsCultureGroup.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

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
			this.scrollable.Panel.IsAutoFitting = true;
			this.scrollable.PaintForegroundFrame = true;
			this.scrollable.Panel.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
			this.scrollable.TabIndex = this.tabIndex++;
			this.scrollable.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.bands = new List<Band>();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.buttonMainExtend != null)
				{
					this.buttonMainExtend.Clicked -= new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);
					this.buttonMainCompact.Clicked -= new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);
				}

				if (this.buttonSuiteExtend != null)
				{
					this.buttonSuiteExtend.Clicked -= new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);
					this.buttonSuiteCompact.Clicked -= new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);
				}

				this.splitter.SplitterDragged -= new EventHandler(this.HandleSplitterDragged);

				this.labelEdit.EditionAccepted -= new EventHandler(this.HandleTextChanged);
				this.labelEdit.EditionRejected -= new EventHandler(this.HandleTextRejected);
				this.labelEdit.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.labelEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);

				this.table.ItemPanel.SelectionChanged -= new EventHandler<UI.ItemPanelSelectionChangedEventArgs>(this.HandleTableSelectionChanged);
				this.table.SizeChanged -= this.HandleTableSizeChanged;
				this.table.ColumnHeader.ColumnWidthChanged -= this.HandleColumnHeaderColumnWidthChanged;
			}

			base.Dispose(disposing);
		}


		protected virtual void InitializeTable()
		{
			//	Initialise la table.
			StructuredType cultureMapType = new StructuredType();
			cultureMapType.Fields.Add("Name", StringType.Default);
			cultureMapType.Fields.Add("Primary", StringType.Default);
			cultureMapType.Fields.Add("Secondary", StringType.Default);
			cultureMapType.Fields.Add("Druid", StringType.Default);
			cultureMapType.Fields.Add("Local", StringType.Default);
			cultureMapType.Fields.Add("Identity", StringType.Default);

			this.table.SourceType = cultureMapType;

			this.table.Columns.Add(new UI.ItemTableColumn("Name", new Widgets.Layouts.GridLength(this.GetColumnWidth(0), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Primary", new Widgets.Layouts.GridLength(this.GetColumnWidth(1), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Secondary", new Widgets.Layouts.GridLength(this.GetColumnWidth(2), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Druid", new Widgets.Layouts.GridLength(this.GetColumnWidth(3), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Local", new Widgets.Layouts.GridLength(this.GetColumnWidth(4), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Identity", new Widgets.Layouts.GridLength(this.GetColumnWidth(5), Widgets.Layouts.GridUnitType.Proportional)));

			this.table.ColumnHeader.SetColumnComparer(1, this.ComparePrimary);
			this.table.ColumnHeader.SetColumnComparer(2, this.CompareSecondary);
			this.table.ColumnHeader.SetColumnComparer(3, this.CompareDruid);
			this.table.ColumnHeader.SetColumnComparer(4, this.CompareLocal);
			this.table.ColumnHeader.SetColumnComparer(5, this.CompareIdentity);

			this.table.ColumnHeader.SetColumnText(0, "Nom");
			this.table.ColumnHeader.SetColumnText(3, "Druid");
			this.table.ColumnHeader.SetColumnText(4, "Local");
			this.table.ColumnHeader.SetColumnText(5, "Identité");

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


		public override int SelectedRow
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


		public override void Update()
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

		protected override void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			//	TODO: à supprimer le jour où une modification dans la liste ObservableList se refraichit automatiquement !
			this.ignoreChange = true;
			this.table.ItemPanel.Refresh();
			this.ignoreChange = false;
		}

		public override void ShowSelectedRow()
		{
			//	Montre la ressource sélectionnée dans le tableau.
			if (this.table != null)
			{
				int pos = this.access.CollectionView.CurrentPosition;
				UI.ItemView item = this.table.ItemPanel.GetItemView(pos);
				this.table.ItemPanel.Show(item);
			}
		}

		protected void UpdateTitle()
		{
			//	Met à jour le titre en dessus de la zone scrollable.
			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			if (item != null)
			{
				string name = item.FullName;
				this.titleText.Text = string.Concat("<font size=\"150%\">", name, "</font>");
			}
		}


		protected override void UpdateCultures()
		{
			//	Met à jour les boutons des cultures en fonction des cultures existantes.
			if (this.secondaryButtonsCulture != null)
			{
				foreach (IconButtonMark button in this.secondaryButtonsCulture)
				{
					button.Clicked -= new MessageEventHandler(this.HandleButtonSecondaryCultureClicked);
					button.Dispose();
				}
				this.secondaryButtonsCulture = null;
			}

			this.primaryButtonCulture.Text = string.Format(Res.Strings.Viewers.Strings.Reference, Misc.CultureName(this.access.GetBaseCultureName()));

			List<string> list = this.access.GetSecondaryCultureNames();  // TODO:
			if (list.Count > 0)
			{
				this.secondaryButtonsCulture = new IconButtonMark[list.Count];
				for (int i=0; i<list.Count; i++)
				{
					this.secondaryButtonsCulture[i] = new IconButtonMark(this.secondaryButtonsCultureGroup);
					this.secondaryButtonsCulture[i].ButtonStyle = ButtonStyle.ActivableIcon;
					this.secondaryButtonsCulture[i].SiteMark = ButtonMarkDisposition.Below;
					this.secondaryButtonsCulture[i].MarkDimension = 5;
					this.secondaryButtonsCulture[i].Name = list[i];
					this.secondaryButtonsCulture[i].Text = Misc.CultureName(list[i]);
					this.secondaryButtonsCulture[i].AutoFocus = false;
					this.secondaryButtonsCulture[i].Dock = DockStyle.Fill;
					this.secondaryButtonsCulture[i].Margins = new Margins(0, (i==list.Count-1)?1:0, 0, 0);
					this.secondaryButtonsCulture[i].Clicked += new MessageEventHandler(this.HandleButtonSecondaryCultureClicked);
				}

				this.TwoLettersSecondaryCulture = list[0];
			}
			else
			{
				this.TwoLettersSecondaryCulture = null;
			}
		}

		protected override void UpdateModificationsCulture()
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

		protected override void UpdateSelectedCulture()
		{
			//	Sélectionne le bouton correspondant à la culture secondaire.
			this.table.ColumnHeader.SetColumnText(this.PrimaryColumn, Misc.CultureName(this.access.GetBaseCultureName()));
			this.table.ColumnHeader.SetColumnText(this.SecondaryColumn, Misc.CultureName(this.GetTwoLetters(1)));

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
						this.bands[i].bandContainer.Visibility = !Abstract2.mainExtended;
						break;

					case BandMode.MainView:
						this.bands[i].bandContainer.Visibility = Abstract2.mainExtended;
						break;

					case BandMode.SuiteSummary:
						this.bands[i].bandContainer.Visibility = !Abstract2.suiteExtended;
						break;

					case BandMode.SuiteView:
						this.bands[i].bandContainer.Visibility = Abstract2.suiteExtended;
						break;
				}
			}
		}

		protected override void UpdateModificationsState()
		{
			//	Met à jour en fonction des modifications (fonds de couleur, etc).
			this.UpdateColor();
			this.UpdateModificationsCulture();
		}

		protected void UpdateColor()
		{
			//	Met à jour les couleurs dans toutes les bandes.
			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			ResourceAccess.ModificationState state1 = this.access.GetModification(item, this.GetTwoLetters(0));
			ResourceAccess.ModificationState state2 = this.access.GetModification(item, this.GetTwoLetters(1));
			this.ColoriseBands(state1, state2);
		}

		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			base.UpdateEdit();

			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			if (this.primarySummary != null)
			{
				this.primarySummary.Text = this.GetSummary(this.GetTwoLetters(0));
				this.secondarySummary.Text = this.GetSummary(this.GetTwoLetters(1));
			}

			this.labelEdit.Enable = !this.designerApplication.IsReadonly;

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			if (item != null)
			{
				this.labelEdit.Text = item.Name;
			}

			this.ignoreChange = iic;
		}

		protected virtual string GetSummary(string twoLettersCulture)
		{
			//	Retourne le texte résumé de la ressource sélectionnée.
			return null;
		}


		protected void SetValue(CultureMap item, StructuredData data, Druid id, object value, bool update)
		{
			//	Méthode appelée pour modifier un champ.
			ResourceAccess.SetStructuredDataValue(this.access.Accessor, item, data, id.ToString(), value);
			this.access.SetGlobalDirty();
			
			this.UpdateColor();
			//?this.access.Accessor.PersistChanges();
			this.access.SetLocalDirty();

			if (update)
			{
				//?this.access.CollectionView.Refresh();  // TODO: ne mettre à jour que la ligne modifiée
			}
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
		

		#region Band
		protected GlyphButton CreateBand(out MyWidgets.StackedPanel leftContainer, out MyWidgets.StackedPanel rightContainer, string title, BandMode mode, GlyphShape extendShape, bool isNewSection, double backgroundIntensity)
		{
			//	Crée une bande horizontale avec deux containers gauche/droite pour les
			//	ressources primaire/secondaire.
			Widget band = new Widget(this.scrollable.Panel);
			band.Name = "BandForLeftAndRight";
			band.Dock = DockStyle.StackBegin;
			band.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			band.TabIndex = this.tabIndex++;
			band.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

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
			rightContainer.MinWidth = 100;
			rightContainer.Dock = DockStyle.StackFill;
			rightContainer.TabIndex = this.tabIndex++;
			rightContainer.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.bands.Add(new Band(band, leftContainer, rightContainer, mode, backgroundIntensity));

			return leftContainer.ExtendButton;
		}

		protected GlyphButton CreateBand(out MyWidgets.StackedPanel leftContainer, string title, BandMode mode, GlyphShape extendShape, bool isNewSection, double backgroundIntensity)
		{
			//	Crée une bande horizontale avec un seul container gauche pour la
			//	ressource primaire.
			Widget band = new Widget(this.scrollable.Panel);
			band.Name = "BandForLeft";
			band.Dock = DockStyle.StackBegin;
			band.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			band.TabIndex = this.tabIndex++;
			band.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

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

			return leftContainer.ExtendButton;
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
					rc.Visibility = (this.GetTwoLetters(1) != null);
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
			public ItemViewFactory(Abstract2 owner)
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

					case "Type":
						return this.CreateType(item, shape);

					case "Entity":
						return this.CreateEntity(item, shape);

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
				}

				return null;
			}

			private Widget CreateName(CultureMap item, UI.ItemViewShape shape)
			{
				string text = (shape == UI.ItemViewShape.ToolTip) ? item.FullName : item.Name;
				if (shape == UI.ItemViewShape.ToolTip && string.IsNullOrEmpty(text))
				{
					return null;
				}

				UI.ItemViewText widget = new UI.ItemViewText();
				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = TextLayout.ConvertToTaggedText(text);
				widget.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				widget.PreferredSize = widget.GetBestFitSize();

				return widget;
			}

			private Widget CreateType(CultureMap item, UI.ItemViewShape shape)
			{
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

				UI.ItemViewText widget = new UI.ItemViewText();
				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = TextLayout.ConvertToTaggedText(text);
				widget.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				widget.PreferredSize = widget.GetBestFitSize();

				return widget;
			}

			private Widget CreateEntity(CultureMap item, UI.ItemViewShape shape)
			{
				if (shape == UI.ItemViewShape.ToolTip)
				{
					return null;
				}

				string text = item.Prefix;

				UI.ItemViewText widget = new UI.ItemViewText();
				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = TextLayout.ConvertToTaggedText(text);
				widget.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				widget.PreferredSize = widget.GetBestFitSize();

				return widget;
			}

			private Widget CreatePrimary(CultureMap item, UI.ItemViewShape shape)
			{
				return this.CreateContent(item, shape, this.owner.GetTwoLetters(0));
			}

			private Widget CreateSecondary(CultureMap item, UI.ItemViewShape shape)
			{
				return this.CreateContent(item, shape, this.owner.GetTwoLetters(1));
			}
			
			private Widget CreateContent(CultureMap item, UI.ItemViewShape shape, string twoLettersCulture)
			{
				//	Crée le contenu pour une colonne primaire ou secondaire.
				//	Par optimisation, un seul widget est créé s'il n'y a pas de couleur de fond.
				string text = this.owner.GetColumnText(item, twoLettersCulture);
				if (shape == UI.ItemViewShape.ToolTip && string.IsNullOrEmpty(text))
				{
					return null;
				}

				UI.ItemViewText main, st;
				ResourceAccess.ModificationState state = this.owner.access.GetModification(item, twoLettersCulture);

				if (state == ResourceAccess.ModificationState.Normal)
				{
					main = st = new UI.ItemViewText();
				}
				else
				{
					main = new UI.ItemViewText();
					main.BackColor = Abstract.GetBackgroundColor(state, 0.7);

					st = new UI.ItemViewText(main);
					st.Dock = DockStyle.Fill;
				}

				st.Margins = new Margins(5, 5, 0, 0);
				st.Text = text;
				st.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split;
				st.PreferredSize = st.GetBestFitSize();

				return main;
			}
			
			private Widget CreateDruid(CultureMap item, UI.ItemViewShape shape)
			{
				string text = this.owner.GetDruidText(item);

				if (shape == UI.ItemViewShape.ToolTip)
				{
					text = string.Concat(text, " ", this.owner.GetLocalText(item), " ", this.owner.GetIdentityText(item));
				}

				if (shape == UI.ItemViewShape.ToolTip && string.IsNullOrEmpty(text))
				{
					return null;
				}

				UI.ItemViewText widget = new UI.ItemViewText();
				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = TextLayout.ConvertToTaggedText(text);
				widget.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				widget.PreferredSize = widget.GetBestFitSize();

				return widget;
			}

			private Widget CreateLocal(CultureMap item, UI.ItemViewShape shape)
			{
				if (shape == UI.ItemViewShape.ToolTip)
				{
					return null;
				}

				string text = this.owner.GetLocalText(item);

				UI.ItemViewText widget = new UI.ItemViewText();
				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = TextLayout.ConvertToTaggedText(text);
				widget.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				widget.ContentAlignment = ContentAlignment.MiddleRight;
				widget.PreferredSize = widget.GetBestFitSize();

				return widget;
			}

			private Widget CreateIdentity(CultureMap item, UI.ItemViewShape shape)
			{
				if (shape == UI.ItemViewShape.ToolTip)
				{
					return null;
				}

				string text = this.owner.GetIdentityText(item);

				UI.ItemViewText widget = new UI.ItemViewText();
				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = TextLayout.ConvertToTaggedText(text);
				widget.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				widget.PreferredSize = widget.GetBestFitSize();

				return widget;
			}


			private Abstract2 owner;
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

		public virtual string GetColumnText(CultureMap item, string twoLettersCulture)
		{
			//	Retourne le texte pour une colonne primaire ou secondaire.
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
			if (item.Id.Developer == 0)
			{
				return "Dieu";
			}

			Identity.IdentityCard card = Identity.IdentityRepository.Default.FindIdentityCard(item.Id.Developer);
			if (card != null)
			{
				return card.UserName;
			}

			return string.Format("Dev {0}", item.Id.Developer.ToString());
		}


		private void HandleSplitterDragged(object sender)
		{
			//	Le splitter a été bougé.
			if (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Horizontal)
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

			if (sender == this.buttonMainCompact)
			{
				Abstract2.mainExtended = false;
			}

			if (sender == this.buttonMainExtend)
			{
				Abstract2.mainExtended = true;
			}

			if (sender == this.buttonSuiteCompact)
			{
				Abstract2.suiteExtended = false;
			}

			if (sender == this.buttonSuiteExtend)
			{
				Abstract2.suiteExtended = true;
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
				//?this.access.Accessor.PersistChanges();
				this.access.SetLocalDirty();
				this.access.CollectionView.Refresh();
			}

			this.UpdateModificationsCulture();
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


		private ItemViewFactory					itemViewFactory;

		protected static bool					mainExtended = false;
		protected static bool					suiteExtended = false;

		protected Widget						firstPane;
		protected Widget						lastPane;
		protected AbstractSplitter				splitter;
		protected UI.ItemTable					table;
		protected MyWidgets.TextFieldExName		labelEdit;

		protected Widget						lastGroup;
		protected FrameBox						titleBox;
		protected StaticText					titleText;
		protected Scrollable					scrollable;
		protected List<Band>					bands;

		protected IconButtonMark				primaryButtonCulture;
		protected Widget						secondaryButtonsCultureGroup;
		protected IconButtonMark[]				secondaryButtonsCulture;

		protected StaticText					primarySummary;
		protected StaticText					secondarySummary;
		protected GlyphButton					buttonMainExtend;
		protected GlyphButton					buttonMainCompact;
		protected GlyphButton					buttonSuiteExtend;
		protected GlyphButton					buttonSuiteCompact;
	}
}
