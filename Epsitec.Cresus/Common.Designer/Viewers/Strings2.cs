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

		protected enum ResourceState
		{
			Normal,			//	défini normalement
			Empty,			//	vide ou indéfini (fond rouge)
			Modified,		//	modifié (fond jaune)
		}


		public Strings2(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			this.twoLettersSecondaryCulture = "en";

			this.accessor = new Support.ResourceAccessors.StringResourceAccessor();
			this.accessor.Load(access.ResourceManager);

			StructuredType cultureMapType = new StructuredType();
			cultureMapType.Fields.Add("Name", StringType.Default);
			cultureMapType.Fields.Add("Primary", StringType.Default);
			cultureMapType.Fields.Add("Secondary", StringType.Default);

			this.collectionView = new CollectionView(this.accessor.Collection);
//-			this.collectionView.SortDescriptions.Clear();
//-			this.collectionView.SortDescriptions.Add(new SortDescription("Name"));  // TODO: pourquoi ça ne fonctionne pas ?

			this.itemViewFactory = new ItemViewFactory(this);
			
			//	Crée les deux volets gauche/droite séparés d'un splitter.
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
			
			//	Crée la partie gauche.			
			this.labelEdit = new MyWidgets.TextFieldExName(this.left);
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

			this.table.ColumnHeader.SetColumnSort(0, ListSortDirection.Ascending);

			//	Crée la partie droite, bande supérieure pour les boutons des cultures.
			Widget sup = new Widget(this.right);
			sup.Name = "Sup";
			sup.PreferredHeight = 35;
			sup.Padding = new Margins(1, 18, 10, 0);
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

			//	Crée la partie droite, bande inférieure pour la zone d'étition scrollable.
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

			//	Résumé des captions.
			this.buttonCaptionExtend = this.CreateBand(out leftContainer, out rightContainer, "Résumé", BandMode.CaptionSummary, GlyphShape.ArrowDown, false, 0.2);
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
			this.UpdateButtonsCultures();
			this.UpdateTitle();
			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateButtonsModificationsCulture();
			this.UpdateCommands();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.splitter.SplitterDragged -= new EventHandler(this.HandleSplitterDragged);

				this.labelEdit.EditionAccepted -= new EventHandler(this.HandleTextChanged);
				this.labelEdit.EditionRejected -= new EventHandler(this.HandleTextRejected);
				this.labelEdit.CursorChanged -= new EventHandler(this.HandleCursorChanged);
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
			//	Met à jour le contenu du Viewer.
			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateButtonsModificationsCulture();
			this.UpdateCommands();
		}

		protected void UpdateDisplayMode()
		{
			//	Met à jour le mode d'affichage des bandes.
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
			//	Met à jour tout le contenu du tableau.
			this.table.ItemPanel.Refresh();
		}

		protected void UpdateTitle()
		{
			//	Met à jour le titre en dessus de la zone scrollable.
			CultureMap item = this.collectionView.CurrentItem as CultureMap;
			string name = item.Name;
			this.titleText.Text = string.Concat("<font size=\"150%\">", name, "</font>");
		}

		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			this.primarySummary.Text = this.GetSummary(this.GetTwoLetters(0));
			this.secondarySummary.Text = this.GetSummary(this.GetTwoLetters(1));

			CultureMap item = this.collectionView.CurrentItem as CultureMap;

			this.labelEdit.Text = item.Name;

			StructuredData data;

			data = item.GetCultureData(this.GetTwoLetters(0));
			this.primaryText.Text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;
			this.primaryComment.Text = data.GetValue(Support.Res.Fields.Resource.Comment) as string;

			data = item.GetCultureData(this.GetTwoLetters(1));
			this.secondaryText.Text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;
			this.secondaryComment.Text = data.GetValue(Support.Res.Fields.Resource.Comment) as string;

			this.ignoreChange = iic;

			this.UpdateCommands();
		}

		protected void UpdateColor()
		{
			//	Met à jour les couleurs dans toutes les bandes.
			ResourceState state1 = this.GetResourceState(this.GetTwoLetters(0));
			ResourceState state2 = this.GetResourceState(this.GetTwoLetters(1));
			this.ColoriseBands(state1, state2);
		}


		protected void UpdateButtonsModificationsCulture()
		{
			//	Met à jour les pastilles dans les boutons des cultures.
			if (this.secondaryButtonsCulture == null)  // pas de culture secondaire ?
			{
				return;
			}

			foreach (IconButtonMark button in this.secondaryButtonsCulture)
			{
				ResourceState state = this.GetResourceState(button.Name);

				if (state == ResourceState.Normal)
				{
					button.BulletColor = Color.Empty;
				}
				else
				{
					button.BulletColor = Strings2.GetBackgroundColor(state, 1.0);
				}
			}
		}

		protected void UpdateButtonsSelectedCulture()
		{
			//	Sélectionne le bouton correspondant à la culture secondaire.
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

		protected void UpdateButtonsCultures()
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

			this.primaryButtonCulture.Text = string.Format(Res.Strings.Viewers.Strings.Reference, Strings2.CultureName(this.GetTwoLetters(0)));

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
					this.secondaryButtonsCulture[i].Text = Strings2.CultureName(list[i]);
					this.secondaryButtonsCulture[i].AutoFocus = false;
					this.secondaryButtonsCulture[i].Dock = DockStyle.Fill;
					this.secondaryButtonsCulture[i].Clicked += new MessageEventHandler(this.HandleButtonSecondaryCultureClicked);
					//?ToolTip.Default.SetToolTip(this.secondaryButtonsCulture[i], Strings2.CultureLongName(bundle.Culture));
				}

				this.TwoLettersSecondaryCulture = list[0];
			}
			else
			{
				this.TwoLettersSecondaryCulture = null;
			}
		}

		static protected string CultureName(string twoLetter)
		{
			return twoLetter.ToUpper();  // TODO: transformer "en" en "English"
		}


		protected string TwoLettersSecondaryCulture
		{
			//	Culture secondaire utilisée.
			get
			{
				return this.twoLettersSecondaryCulture;
			}
			set
			{
				if (this.twoLettersSecondaryCulture != value)
				{
					this.twoLettersSecondaryCulture = value;

					this.UpdateButtonsSelectedCulture();
					this.UpdateArray();
				}
			}
		}

		protected string GetTwoLetters(int row)
		{
			//	Retourne la culture primaire ou secondaire utilisée.
			System.Diagnostics.Debug.Assert(row == 0 || row == 1);
			return (row == 0) ? Resources.DefaultTwoLetterISOLanguageName : this.twoLettersSecondaryCulture;
		}

		protected ResourceState GetResourceState(string twoLettersCulture)
		{
			//	Retourne l'état d'une ressource.
			CultureMap item = this.collectionView.CurrentItem as CultureMap;
			return this.GetResourceState(item, twoLettersCulture);
		}

		protected ResourceState GetResourceState(CultureMap item, string twoLettersCulture)
		{
			//	Retourne l'état d'une ressource.
			StructuredData data = item.GetCultureData(twoLettersCulture);

			if (data.IsEmpty)
			{
				return ResourceState.Empty;
			}

			string text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;
			if (string.IsNullOrEmpty(text))
			{
				return ResourceState.Empty;
			}

			if (twoLettersCulture != this.GetTwoLetters(0))  // culture secondaire ?
			{
				StructuredData primaryData = item.GetCultureData(this.GetTwoLetters(0));
				int pmod = (int) primaryData.GetValue(Support.Res.Fields.Resource.ModificationId);
				int cmod = (int)        data.GetValue(Support.Res.Fields.Resource.ModificationId);
				if (pmod > cmod)
				{
					return ResourceState.Modified;
				}
			}

			return ResourceState.Normal;
		}

		protected string GetSummary(string twoLettersCulture)
		{
			//	Retourne le texte résumé de la ressource sélectionnée.
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();

			CultureMap item = this.collectionView.CurrentItem as CultureMap;
			StructuredData data = item.GetCultureData(twoLettersCulture);

			string text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;
			if (string.IsNullOrEmpty(text))
			{
				buffer.Append(Misc.Italic("(indéfini)"));
			}
			else
			{
				buffer.Append(text);
			}

			string comment = data.GetValue(Support.Res.Fields.Resource.Comment) as string;
			if (!string.IsNullOrEmpty(comment))
			{
				buffer.Append("<br/>");
				buffer.Append(Misc.Italic(comment));
			}

			return buffer.ToString();
		}


		protected double GetColomnWidth(int column)
		{
			//	Retourne la largeur d'une colonne.
			return this.table.Columns[column].Width.Value;
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

		protected void ColoriseBands(ResourceState state1, ResourceState state2)
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
					rc.Visibility = (this.GetTwoLetters(1) != null);
				}
			}
		}

		protected static Color GetBackgroundColor(ResourceState state, double intensity)
		{
			//	Donne une couleur pour un fond de panneau.
			if (intensity == 0.0)
			{
				return Color.Empty;
			}

			switch (state)
			{
				case ResourceState.Empty:
					return Color.FromAlphaRgb(intensity, 0.91, 0.40, 0.40);  // rouge

				case ResourceState.Modified:
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


		private void HandleSplitterDragged(object sender)
		{
			//	Le splitter a été bougé.
			Abstract.leftArrayWidth = this.left.ActualWidth;
		}

		private void HandleTableSelectionChanged(object sender)
		{
			this.UpdateTitle();
			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateButtonsModificationsCulture();
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

		private void HandleButtonSecondaryCultureClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton pour changer de culture secondaire a été cliqué.
			IconButtonMark button = sender as IconButtonMark;
			this.TwoLettersSecondaryCulture = button.Name;

			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateCommands();
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

			CultureMap item = this.collectionView.CurrentItem as CultureMap;

			if (edit == this.labelEdit)
			{
				item.Name = text;
			}

			if (edit == this.primaryText)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(0));
				data.SetValue(Support.Res.Fields.ResourceString.Text, text);
			}

			if (edit == this.secondaryText)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(1));
				data.SetValue(Support.Res.Fields.ResourceString.Text, text);
			}

			if (edit == this.primaryComment)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(0));
				data.SetValue(Support.Res.Fields.Resource.Comment, text);
			}

			if (edit == this.secondaryComment)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(1));
				data.SetValue(Support.Res.Fields.Resource.Comment, text);
			}

			this.UpdateButtonsModificationsCulture();
		}

		private void HandleTextRejected(object sender)
		{
			TextFieldEx edit = sender as TextFieldEx;

			if (edit != null)
			{
				edit.RejectEdition();  // TODO: devrait être inutile
			}
		}

		private void HandleLabelKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appelé lorsque la ligne éditable pour le label voit son focus changer.
			TextFieldEx field = sender as TextFieldEx;
			field.AcceptEdition();
			this.HandleEditKeyboardFocusChanged(sender, e);
		}

		private void HandleCursorChanged(object sender)
		{
			//	Le curseur a été déplacé dans un texte éditable.
			if (this.ignoreChange)
			{
				return;
			}

			this.lastActionIsReplace = false;
		}

		protected void HandleButtonCompactOrExtendClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton pour changer le mode d'affichage a été cliqué.
			if (sender == this.buttonCaptionCompact)
			{
				Strings2.captionExtended = false;
			}

			if (sender == this.buttonCaptionExtend)
			{
				Strings2.captionExtended = true;
			}

			this.UpdateDisplayMode();
			this.UpdateEdit();  // pour que le résumé prenne en compte les modifications
		}


		protected UI.IItemViewFactory ItemViewFactoryGetter(UI.ItemView itemView)
		{
			//	Retourne le "factory" a utiliser pour les éléments représentés dans cet ItemTable/ItemPanel.
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
				return this.CreateContent(item, this.owner.GetTwoLetters(0));
			}

			private Widget CreateSecondary(CultureMap item)
			{
				return this.CreateContent(item, this.owner.GetTwoLetters(1));
			}
			
			private Widget CreateContent(CultureMap item, string twoLettersCulture)
			{
				//	Crée le contenu pour une colonne primaire ou secondaire.
				//	Par optimisation, un seul widget est créé s'il n'y a pas de couleur de fond.
				StaticText main, text;
				ResourceState state = this.owner.GetResourceState(item, twoLettersCulture);

				if (state == ResourceState.Normal)
				{
					main = text = new StaticText();
				}
				else
				{
					main = new StaticText();
					main.BackColor = Strings2.GetBackgroundColor(state, 0.7);

					text = new StaticText(main);
					text.Dock = DockStyle.Fill;
				}

				StructuredData data = item.GetCultureData(twoLettersCulture);
				string value = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;

				text.Margins = new Margins(5, 5, 0, 0);
				text.Text = TextLayout.ConvertToTaggedText(value);
				text.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

				return main;
			}
			

			Strings2 owner;
		}


		protected static bool					captionExtended = false;

		protected Support.ResourceAccessors.StringResourceAccessor accessor;
		protected CollectionView				collectionView;
		private ItemViewFactory					itemViewFactory;
		protected string						twoLettersSecondaryCulture;

		protected Widget						left;
		protected Widget						right;
		protected VSplitter						splitter;
		protected UI.ItemTable					table;
		protected MyWidgets.TextFieldExName		labelEdit;
		protected IconButtonMark				primaryButtonCulture;
		protected Widget						secondaryButtonsCultureGroup;
		protected IconButtonMark[]				secondaryButtonsCulture;
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
