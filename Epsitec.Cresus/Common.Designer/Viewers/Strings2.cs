using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de repr�senter les ressources d'un module.
	/// </summary>
	public class Strings2 : Abstract
	{
		protected enum BandMode
		{
			CaptionSummary,
			CaptionView,
			Separator,
			SuiteSummary,
			SuiteView,
		}

		protected enum ModificationState
		{
			Normal,			//	d�fini normalement
			Empty,			//	vide (fond rouge)
			Modified,		//	modifi� (fond jaune)
		}


		public Strings2(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			this.accessor = new Support.ResourceAccessors.StringResourceAccessor();
			this.accessor.Load(access.ResourceManager);

			StructuredType cultureMapType = new StructuredType();
			cultureMapType.Fields.Add("Name", StringType.Default);
			cultureMapType.Fields.Add("Primary", StringType.Default);
			cultureMapType.Fields.Add("Secondary", StringType.Default);

			this.collectionView = new CollectionView(this.accessor.Collection);
			this.collectionView.SortDescriptions.Clear();
			this.collectionView.SortDescriptions.Add(new SortDescription("Name"));  // TODO: pourquoi �a ne fonctionne pas ?

			this.itemViewFactory = new ItemViewFactory(this);
			
			//	Cr�e les deux volets gauche/droite s�par�s d'un splitter.
			this.left = new Widget(this);
			this.left.Name = "Left";
			this.left.MinWidth = 80;
			this.left.MaxWidth = 500;
			this.left.PreferredWidth = Abstract.leftArrayWidth;
			this.left.Dock = DockStyle.Left;
			this.left.Padding = new Margins(10, 10, 10, 10);
			this.left.TabIndex = this.tabIndex++;
			this.left.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.splitter = new VSplitter(this);
			this.splitter.Dock = DockStyle.Left;
			this.splitter.SplitterDragged += new EventHandler(this.HandleSplitterDragged);
			VSplitter.SetAutoCollapseEnable(this.left, true);

			this.right = new Widget(this);
			this.right.Name = "Right";
			this.right.MinWidth = 200;
			this.right.Dock = DockStyle.Fill;
			this.right.Padding = new Margins(1, 1, 1, 1);
			this.right.TabIndex = this.tabIndex++;
			this.right.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			
			//	Cr�e la partie gauche.			
			this.labelEdit = new MyWidgets.TextFieldExName(this.left);
			this.labelEdit.Name = "LabelEdit";
			this.labelEdit.Margins = new Margins(0, 0, 10, 0);
			this.labelEdit.Dock = DockStyle.Bottom;
			this.labelEdit.ButtonShowCondition = ShowCondition.WhenModified;
			this.labelEdit.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.labelEdit.EditionAccepted += new EventHandler(this.HandleTextChanged);
			this.labelEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);
			this.labelEdit.TabIndex = this.tabIndex++;
			this.labelEdit.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.labelEdit.Visibility = (this.module.Mode == DesignerMode.Build);
			this.currentTextField = this.labelEdit;

			this.table = new UI.ItemTable(this.left);
			this.table.ItemPanel.CustomItemViewFactoryGetter = this.ItemViewFactoryGetter;
			this.table.SourceType = cultureMapType;
			this.table.Items = this.collectionView;
			this.table.Columns.Add(new UI.ItemTableColumn("Name", new Widgets.Layouts.GridLength(200, Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Primary", new Widgets.Layouts.GridLength(100, Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Secondary", new Widgets.Layouts.GridLength(100, Widgets.Layouts.GridUnitType.Proportional)));
			this.table.HorizontalScrollMode = UI.ItemTableScrollMode.Linear;
			this.table.VerticalScrollMode = UI.ItemTableScrollMode.ItemBased;
			this.table.HeaderVisibility = false;
			this.table.FrameVisibility = false;
			this.table.ItemPanel.Layout = UI.ItemPanelLayout.VerticalList;
			this.table.ItemPanel.ItemSelectionMode = UI.ItemPanelSelectionMode.ExactlyOne;
			this.table.ItemPanel.SelectionChanged += new EventHandler(this.HandleTableSelectionChanged);
			this.table.SizeChanged += this.HandleTableSizeChanged;
			this.table.ColumnHeader.ColumnWidthChanged += this.HandleColumnHeaderColumnWidthChanged;
			this.table.Dock = Widgets.DockStyle.Fill;
			this.table.Margins = new Drawing.Margins(0, 0, 0, 0);

			//	Cr�e la partie droite, bande sup�rieure pour les boutons des cultures.
			Widget sup = new Widget(this.right);
			sup.Name = "Sup";
			sup.PreferredHeight = 35;
			sup.Padding = new Margins(1, 18, 10, 0);
			sup.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			sup.Dock = DockStyle.Top;
			sup.TabIndex = this.tabIndex++;
			sup.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			
			this.primaryCulture = new IconButtonMark(sup);
			this.primaryCulture.ButtonStyle = ButtonStyle.ActivableIcon;
			this.primaryCulture.SiteMark = ButtonMarkDisposition.Below;
			this.primaryCulture.MarkDimension = 5;
			this.primaryCulture.PreferredHeight = 25;
			this.primaryCulture.ActiveState = ActiveState.Yes;
			this.primaryCulture.AutoFocus = false;
			this.primaryCulture.Margins = new Margins(0, 1, 0, 0);
			this.primaryCulture.Dock = DockStyle.Fill;

			this.secondaryCultureGroup = new Widget(sup);
			this.secondaryCultureGroup.Name = "SecondaryCultureGroup";
			this.secondaryCultureGroup.Margins = new Margins(1, 0, 0, 0);
			this.secondaryCultureGroup.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			this.secondaryCultureGroup.Dock = DockStyle.Fill;
			this.secondaryCultureGroup.TabIndex = this.tabIndex++;
			this.secondaryCultureGroup.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			//	Cr�e le titre.
			this.titleBox = new FrameBox(this.right);
			this.titleBox.DrawFullFrame = true;
			this.titleBox.PreferredHeight = 26;
			this.titleBox.Dock = DockStyle.Top;
			this.titleBox.Margins = new Margins(1, 1, 1, -1);

			this.titleText = new StaticText(this.titleBox);
			this.titleText.ContentAlignment = ContentAlignment.MiddleCenter;
			this.titleText.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			this.titleText.Dock = DockStyle.Fill;
			this.titleText.Margins = new Margins(4, 4, 0, 0);

			//	Cr�e la partie droite, bande inf�rieure pour la zone d'�tition scrollable.
			this.scrollable = new Scrollable(this.right);
			this.scrollable.Name = "Scrollable";
			this.scrollable.MinWidth = 100;
			this.scrollable.MinHeight = 100;
			this.scrollable.Margins = new Margins(1, 1, 0, 1);
			this.scrollable.Dock = DockStyle.Fill;
			this.scrollable.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
			this.scrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.scrollable.Panel.IsAutoFitting = true;
			this.scrollable.PaintForegroundFrame = true;
			this.scrollable.Panel.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
			this.scrollable.TabIndex = this.tabIndex++;
			this.scrollable.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.bands = new List<Band>();
			MyWidgets.StackedPanel leftContainer, rightContainer;

			//	R�sum� des captions.
			this.buttonCaptionExtend = this.CreateBand(out leftContainer, out rightContainer, "R�sum�", BandMode.CaptionSummary, GlyphShape.ArrowDown, false, 0.2);
			this.buttonCaptionExtend.Clicked += new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);

			this.primarySummary = new StaticText(leftContainer.Container);
			this.primarySummary.MinHeight = 30;
			this.primarySummary.Dock = DockStyle.Fill;

			this.secondarySummary = new StaticText(rightContainer.Container);
			this.secondarySummary.MinHeight = 30;
			this.secondarySummary.Dock = DockStyle.Fill;

			//	Textes.
			this.buttonCaptionCompact = this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.Labels.Title, BandMode.CaptionView, GlyphShape.ArrowUp, false, 0.2);
			this.buttonCaptionCompact.Clicked += new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);

			this.primaryText = new TextFieldMulti(leftContainer.Container);
			this.primaryText.PreferredHeight = 10+14*6;
			this.primaryText.Dock = DockStyle.StackBegin;
			this.primaryText.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryText.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryText.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryText.TabIndex = this.tabIndex++;
			this.primaryText.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.secondaryText = new TextFieldMulti(rightContainer.Container);
			this.secondaryText.PreferredHeight = 10+14*6;
			this.secondaryText.Dock = DockStyle.StackBegin;
			this.secondaryText.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryText.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryText.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryText.TabIndex = this.tabIndex++;
			this.secondaryText.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Commentaires.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.About.Title, BandMode.CaptionView, GlyphShape.None, false, 0.2);

			this.primaryComment = new TextFieldMulti(leftContainer.Container);
			this.primaryComment.PreferredHeight = 10+14*4;
			this.primaryComment.Dock = DockStyle.StackBegin;
			this.primaryComment.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryComment.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryComment.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryComment.TabIndex = this.tabIndex++;
			this.primaryComment.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.secondaryComment = new TextFieldMulti(rightContainer.Container);
			this.secondaryComment.PreferredHeight = 10+14*4;
			this.secondaryComment.Dock = DockStyle.StackBegin;
			this.secondaryComment.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryComment.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryComment.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryComment.TabIndex = this.tabIndex++;
			this.secondaryComment.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.UpdateDisplayMode();
			this.UpdateCultures();
			this.UpdateTitle();
			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.splitter.SplitterDragged -= new EventHandler(this.HandleSplitterDragged);

				this.labelEdit.EditionAccepted -= new EventHandler(this.HandleTextChanged);
				this.labelEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);

				this.table.ItemPanel.SelectionChanged -= new EventHandler(this.HandleTableSelectionChanged);
				this.table.SizeChanged -= this.HandleTableSizeChanged;
				this.table.ColumnHeader.ColumnWidthChanged -= this.HandleColumnHeaderColumnWidthChanged;

				this.buttonCaptionExtend.Clicked -= new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);
				this.buttonCaptionCompact.Clicked -= new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);

				this.primaryText.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.primaryText.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.primaryText.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryText.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.secondaryText.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.secondaryText.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.primaryComment.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.primaryComment.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.primaryComment.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryComment.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.secondaryComment.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.secondaryComment.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			}

			base.Dispose(disposing);
		}


		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Strings2;
			}
		}

		
		public override void Update()
		{
			//	Met � jour le contenu du Viewer.
			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		protected void UpdateDisplayMode()
		{
			//	Met� jour le mode d'affichage des bandes.
			for (int i=0; i<this.bands.Count; i++)
			{
				switch (bands[i].bandMode)
				{
					case BandMode.CaptionSummary:
						this.bands[i].bandContainer.Visibility = !Strings2.captionExtended;
						break;

					case BandMode.CaptionView:
						this.bands[i].bandContainer.Visibility = Strings2.captionExtended;
						break;
				}
			}
		}

		protected override void UpdateArray()
		{
			//	Met � jour tout le contenu du tableau.
		}

		protected void UpdateTitle()
		{
			//	Met � jour le titre en dessus de la zone scrollable.
			CultureMap item = this.collectionView.CurrentItem as CultureMap;
			string name = item.Name;
			this.titleText.Text = string.Concat("<font size=\"150%\">", name, "</font>");
		}

		protected override void UpdateEdit()
		{
			//	Met � jour les lignes �ditables en fonction de la s�lection dans le tableau.
			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			this.primarySummary.Text = this.GetSummary(Resources.DefaultTwoLetterISOLanguageName);
			this.secondarySummary.Text = this.GetSummary("en");  // TODO:

			CultureMap item = this.collectionView.CurrentItem as CultureMap;

			this.labelEdit.Text = item.Name;

			StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			this.primaryText.Text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;
			this.primaryComment.Text = data.GetValue(Support.Res.Fields.ResourceString.Comment) as string;

			data = item.GetCultureData("en");  // TODO:
			this.secondaryText.Text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;
			this.secondaryComment.Text = data.GetValue(Support.Res.Fields.ResourceString.Comment) as string;

			this.ignoreChange = iic;

			this.UpdateCommands();
		}

		protected void UpdateColor()
		{
			//	Met � jour les couleurs dans toutes les bandes.
			CultureMap item = this.collectionView.CurrentItem as CultureMap;

			ModificationState state1 = ModificationState.Normal;
			if (!item.IsCultureDefined(Resources.DefaultTwoLetterISOLanguageName))
			{
				state1 = ModificationState.Empty;
			}

			ModificationState state2 = ModificationState.Normal;
			if (!item.IsCultureDefined("en"))
			{
				state2 = ModificationState.Empty;
			}

			this.ColoriseBands(state1, state2);
		}


		protected string GetSummary(string twoLettersCulture)
		{
			//	Retourne le texte r�sum� de la ressource s�lectionn�e.
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();

			CultureMap item = this.collectionView.CurrentItem as CultureMap;
			StructuredData data = item.GetCultureData(twoLettersCulture);

			string text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;
			if (string.IsNullOrEmpty(text))
			{
				buffer.Append(Misc.Italic("(ind�fini)"));
			}
			else
			{
				buffer.Append(text);
			}

			string comment = data.GetValue(Support.Res.Fields.ResourceString.Comment) as string;
			if (!string.IsNullOrEmpty(comment))
			{
				buffer.Append("<br/>");
				buffer.Append(Misc.Italic(comment));
			}

			return buffer.ToString();
		}


		#region Band
		protected GlyphButton CreateBand(out MyWidgets.StackedPanel leftContainer, out MyWidgets.StackedPanel rightContainer, string title, BandMode mode, GlyphShape extendShape, bool isNewSection, double backgroundIntensity)
		{
			//	Cr�e une bande horizontale avec deux containers gauche/droite pour les
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
			//	Cr�e une bande horizontale avec un seul container gauche pour la
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

		protected void ColoriseBands(ModificationState state1, ModificationState state2)
		{
			//	Colorise toutes les bandes horizontales.
			for (int i=0; i<this.bands.Count; i++)
			{
				MyWidgets.StackedPanel lc = this.bands[i].leftContainer;
				MyWidgets.StackedPanel rc = this.bands[i].rightContainer;

				lc.BackgroundColor = Strings2.GetBackgroundColor(state1, this.bands[i].intensityContainer);

				if (rc != null)
				{
					rc.BackgroundColor = Strings2.GetBackgroundColor(state2, this.bands[i].intensityContainer);
					rc.Visibility = (this.secondaryCulture != null);
				}
			}
		}

		protected static Color GetBackgroundColor(ModificationState state, double intensity)
		{
			//	Donne une couleur pour un fond de panneau.
			if (intensity == 0.0)
			{
				return Color.Empty;
			}

			switch (state)
			{
				case ModificationState.Empty:
					return Color.FromAlphaRgb(intensity, 0.91, 0.40, 0.40);  // rouge

				case ModificationState.Modified:
					return Color.FromAlphaRgb(intensity, 0.91, 0.81, 0.41);  // jaune

				default:
					IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
					Color cap = adorner.ColorCaption;
					return Color.FromAlphaRgb(intensity, 0.4+cap.R*0.6, 0.4+cap.G*0.6, 0.4+cap.B*0.6);
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


		protected override Widget CultureParentWidget
		{
			//	Retourne le parent � utiliser pour les boutons des cultures.
			get
			{
				return this.secondaryCultureGroup;
			}
		}


		protected double GetColomnWidth(int column)
		{
			//	Retourne la largeur d'une colonne.
			return this.table.Columns[column].Width.Value;
		}

		
		private void HandleSplitterDragged(object sender)
		{
			//	Le splitter a �t� boug�.
			Abstract.leftArrayWidth = this.left.ActualWidth;
		}

		private void HandleTableSelectionChanged(object sender)
		{
			this.UpdateTitle();
			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		private void HandleTableSizeChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			UI.ItemTable table = (UI.ItemTable) sender;
			Drawing.Size size = (Drawing.Size) e.NewValue;

			double width = size.Width - table.GetPanelPadding().Width;
			//?table.ColumnHeader.SetColumnWidth(0, width);

			table.ItemPanel.ItemViewDefaultSize = new Size(width, 20);
		}

		private void HandleColumnHeaderColumnWidthChanged(object sender, UI.ColumnWidthChangeEventArgs e)
		{
		}

		private void HandleTextChanged(object sender)
		{
			//	Un texte �ditable a chang�.
			if (this.ignoreChange)
			{
				return;
			}

			AbstractTextField edit = sender as AbstractTextField;
			string text = edit.Text;
		}

		private void HandleLabelKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appel� lorsque la ligne �ditable pour le label voit son focus changer.
			TextFieldEx field = sender as TextFieldEx;
			field.AcceptEdition();
			this.HandleEditKeyboardFocusChanged(sender, e);
		}

		private void HandleCursorChanged(object sender)
		{
			//	Le curseur a �t� d�plac� dans un texte �ditable.
			if (this.ignoreChange)
			{
				return;
			}

			this.lastActionIsReplace = false;
		}

		protected void HandleButtonCompactOrExtendClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton pour changer le mode d'affichage a �t� cliqu�.
			if (sender == this.buttonCaptionCompact)
			{
				Strings2.captionExtended = false;
			}

			if (sender == this.buttonCaptionExtend)
			{
				Strings2.captionExtended = true;
			}

			this.UpdateDisplayMode();
			this.UpdateEdit();  // pour que le r�sum� prenne en compte les modifications
		}


		protected UI.IItemViewFactory ItemViewFactoryGetter(UI.ItemView itemView)
		{
			//	Retourne le "factory" a utiliser pour les �l�ments repr�sent�s dans cet ItemTable/ItemPanel.
			if (itemView.Item == null || itemView.Item.GetType() != typeof(CultureMap))
			{
				return null;
			}
			else
			{
				return this.itemViewFactory;
			}
		}


		private class ItemViewFactory : UI.AbstractItemViewFactory
		{
			public ItemViewFactory(Strings2 owner)
			{
				this.owner = owner;
			}

			protected override Widget CreateElement(string name, UI.ItemPanel panel, UI.ItemView view, UI.ItemViewShape shape)
			{
				CultureMap item = view.Item as CultureMap;

				switch (name)
				{
					case "Name":
						return this.CreateName(item);

					case "Primary":
						return this.CreatePrimary(item);

					case "Secondary":
						return this.CreateSecondary(item);
				}

				return null;
			}

			private Widget CreateName(CultureMap item)
			{
				StaticText widget = new StaticText();

				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = TextLayout.ConvertToTaggedText(item.Name);
				widget.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

				return widget;
			}

			private Widget CreatePrimary(CultureMap item)
			{
				StaticText widget = new StaticText();
				StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				string text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;

				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = TextLayout.ConvertToTaggedText(text);
				widget.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

				return widget;
			}

			private Widget CreateSecondary(CultureMap item)
			{
				StaticText widget = new StaticText();
				StructuredData data = item.GetCultureData("en"); // TODO: choisir ici la culture secondaire qui a �t� s�lectionn�e
				string text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;

				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = TextLayout.ConvertToTaggedText(text);
				widget.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

				return widget;
			}
			

			Strings2 owner;
		}


		protected static bool					captionExtended = false;

		protected Support.ResourceAccessors.StringResourceAccessor accessor;
		protected UI.ItemTable					table;
		protected CollectionView				collectionView;
		private ItemViewFactory					itemViewFactory;

		protected Widget						left;
		protected Widget						right;
		protected VSplitter						splitter;
		protected Widget						secondaryCultureGroup;
		protected MyWidgets.TextFieldExName		labelEdit;
		protected FrameBox						titleBox;
		protected StaticText					titleText;
		protected Scrollable					scrollable;
		protected List<Band>					bands;
		protected GlyphButton					buttonCaptionExtend;
		protected GlyphButton					buttonCaptionCompact;
		protected StaticText					primarySummary;
		protected StaticText					secondarySummary;
		protected TextFieldMulti				primaryText;
		protected TextFieldMulti				secondaryText;
		protected TextFieldMulti				primaryComment;
		protected TextFieldMulti				secondaryComment;
	}
}
