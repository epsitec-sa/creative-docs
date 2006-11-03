using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public abstract class AbstractCaptions : Abstract
	{
		protected enum BandMode
		{
			CaptionSummary,
			CaptionView,
			Separator,
			SuiteSummary,
			SuiteView,
		}

		public AbstractCaptions(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			//	Crée les 2 parties gauche/droite séparées par un splitter.
			this.left = new Widget(this);
			this.left.Name = "Left";
			this.left.MinWidth = 80;
			this.left.MaxWidth = 400;
			this.left.PreferredWidth = Abstract.leftArrayWidth;
			this.left.Dock = DockStyle.Left;
			this.left.Padding = new Margins(10, 10, 10, 10);
			this.left.TabIndex = this.tabIndex++;
			this.left.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

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
			this.right.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			
			//	Crée la partie gauche.			
			this.labelEdit = new MyWidgets.TextFieldExName(this.left);
			this.labelEdit.Name = "LabelEdit";
			this.labelEdit.Margins = new Margins(0, 0, 10, 0);
			this.labelEdit.Dock = DockStyle.Bottom;
			this.labelEdit.ButtonShowCondition = ShowCondition.WhenModified;
			this.labelEdit.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.labelEdit.EditionAccepted += new EventHandler(this.HandleTextChanged);
			this.labelEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);
			this.labelEdit.TabIndex = this.tabIndex++;
			this.labelEdit.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.labelEdit.Visibility = (this.module.Mode == DesignerMode.Build);
			this.currentTextField = this.labelEdit;

			this.array = new MyWidgets.StringArray(this.left);
			this.array.Columns = 1;
			this.array.SetColumnsRelativeWidth(0, 1.00);
			this.array.SetColumnBreakMode(0, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
			this.array.SetDynamicToolTips(0, true);
			this.array.Margins = new Margins(0, 0, 0, 0);
			this.array.Dock = DockStyle.Fill;
			this.array.CellCountChanged += new EventHandler(this.HandleArrayCellCountChanged);
			this.array.CellsContentChanged += new EventHandler(this.HandleArrayCellsContentChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
			this.array.TabIndex = this.tabIndex++;
			this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Crée la partie droite, bande supérieure pour les boutons des cultures.
			Widget sup = new Widget(this.right);
			sup.Name = "Sup";
			sup.PreferredHeight = 35;
			sup.Padding = new Margins(11, 27, 10, 0);
			sup.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			sup.Dock = DockStyle.Top;
			sup.TabIndex = this.tabIndex++;
			sup.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.primaryCulture = new IconButtonMark(sup);
			this.primaryCulture.ButtonStyle = ButtonStyle.ActivableIcon;
			this.primaryCulture.SiteMark = ButtonMarkDisposition.Below;
			this.primaryCulture.MarkDimension = 5;
			this.primaryCulture.PreferredHeight = 25;
			this.primaryCulture.ActiveState = ActiveState.Yes;
			this.primaryCulture.AutoFocus = false;
			this.primaryCulture.TabIndex = this.tabIndex++;
			this.primaryCulture.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.primaryCulture.Margins = new Margins(0, 10, 0, 0);
			this.primaryCulture.Dock = DockStyle.StackFill;

			this.secondaryCultureGroup = new Widget(sup);
			this.secondaryCultureGroup.Name = "SecondaryCultureGroup";
			this.secondaryCultureGroup.Margins = new Margins(10, 0, 0, 0);
			this.secondaryCultureGroup.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			this.secondaryCultureGroup.Dock = DockStyle.StackFill;
			this.secondaryCultureGroup.TabIndex = this.tabIndex++;
			this.secondaryCultureGroup.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

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
			this.scrollable.IsForegroundFrame = true;
			this.scrollable.Panel.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
			this.scrollable.TabIndex = this.tabIndex++;
			this.scrollable.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.bandContainers = new List<Widget>();
			this.leftContainers = new List<MyWidgets.StackedPanel>();
			this.rightContainers = new List<MyWidgets.StackedPanel>();
			this.modeContainers = new List<BandMode>();
			this.intensityContainers = new List<double>();

			MyWidgets.StackedPanel leftContainer, rightContainer;

			//	Résumé des captions.
			this.buttonCaptionExtend = this.CreateBand(out leftContainer, out rightContainer, "Résumé", BandMode.CaptionSummary, GlyphShape.ArrowDown, false, 0.2);
			this.buttonCaptionExtend.Clicked += new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);

			this.primarySummary = new StaticText(leftContainer.Container);
			this.primarySummary.MinHeight = 30;
			this.primarySummary.Dock = DockStyle.Fill;

			this.primarySummaryIcon = new IconButton(leftContainer.Container);
			this.primarySummaryIcon.MinSize = new Size(30, 30);
			this.primarySummaryIcon.Dock = DockStyle.Right;

			this.secondarySummary = new StaticText(rightContainer.Container);
			this.secondarySummary.MinHeight = 30;
			this.secondarySummary.Dock = DockStyle.Fill;

			this.secondarySummaryIcon = new IconButton(rightContainer.Container);
			this.secondarySummaryIcon.MinSize = new Size(30, 30);
			this.secondarySummaryIcon.Dock = DockStyle.Right;

			//	Textes.
			this.buttonCaptionCompact = this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.Labels.Title, BandMode.CaptionView, GlyphShape.ArrowUp, false, 0.2);
			this.buttonCaptionCompact.Clicked += new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);

			this.primaryLabels = new MyWidgets.StringCollection(leftContainer.Container);
			this.primaryLabels.Dock = DockStyle.StackBegin;
			this.primaryLabels.StringTextChanged += new EventHandler(this.HandleStringTextCollectionChanged);
			this.primaryLabels.StringFocusChanged += new EventHandler(this.HandleStringFocusCollectionChanged);
			this.primaryLabels.TabIndex = this.tabIndex++;
			this.primaryLabels.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.secondaryLabels = new MyWidgets.StringCollection(rightContainer.Container);
			this.secondaryLabels.Dock = DockStyle.StackBegin;
			this.secondaryLabels.StringTextChanged += new EventHandler(this.HandleStringTextCollectionChanged);
			this.secondaryLabels.StringFocusChanged += new EventHandler(this.HandleStringFocusCollectionChanged);
			this.secondaryLabels.TabIndex = this.tabIndex++;
			this.secondaryLabels.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			//	Description.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.Description.Title, BandMode.CaptionView, GlyphShape.None, false, 0.2);

			this.primaryDescription = new TextFieldMulti(leftContainer.Container);
			this.primaryDescription.PreferredHeight = 50;
			this.primaryDescription.Dock = DockStyle.StackBegin;
			this.primaryDescription.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryDescription.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryDescription.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryDescription.TabIndex = this.tabIndex++;
			this.primaryDescription.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.secondaryDescription = new TextFieldMulti(rightContainer.Container);
			this.secondaryDescription.PreferredHeight = 50;
			this.secondaryDescription.Dock = DockStyle.StackBegin;
			this.secondaryDescription.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryDescription.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryDescription.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryDescription.TabIndex = this.tabIndex++;
			this.secondaryDescription.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Icône.
			this.CreateBand(out leftContainer, Res.Strings.Viewers.Captions.Icon.Title, BandMode.CaptionView, GlyphShape.None, false, 0.2);

			StaticText label = new StaticText(leftContainer.Container);
			label.Text = Res.Strings.Viewers.Captions.Icon.Title;
			label.MinHeight = 30;  // attention, très important !
			label.PreferredHeight = 30;
			label.PreferredWidth = 30;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Margins = new Margins(0, 5, 0, 0);
			label.Dock = DockStyle.Left;

			this.primaryIcon = new IconButton(leftContainer.Container);
			this.primaryIcon.MinHeight = 30;  // attention, très important !
			this.primaryIcon.PreferredHeight = 30;
			this.primaryIcon.PreferredWidth = 30;
			this.primaryIcon.ButtonStyle = ButtonStyle.ActivableIcon;
			this.primaryIcon.Dock = DockStyle.Left;
			this.primaryIcon.TabIndex = this.tabIndex++;
			this.primaryIcon.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.primaryIcon.Clicked += new MessageEventHandler(this.HandlePrimaryIconClicked);

			this.primaryIconInfo = new StaticText(leftContainer.Container);
			this.primaryIconInfo.PreferredHeight = 30;
			this.primaryIconInfo.PreferredWidth = 300;
			this.primaryIconInfo.Margins = new Margins(10, 0, 0, 0);
			this.primaryIconInfo.Dock = DockStyle.Left;

			//	Commentaires.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.About.Title, BandMode.CaptionView, GlyphShape.None, false, 0.2);

			this.primaryAbout = new TextFieldMulti(leftContainer.Container);
			this.primaryAbout.PreferredHeight = 36;
			this.primaryAbout.Dock = DockStyle.StackBegin;
			this.primaryAbout.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryAbout.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryAbout.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryAbout.TabIndex = this.tabIndex++;
			this.primaryAbout.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.secondaryAbout = new TextFieldMulti(rightContainer.Container);
			this.secondaryAbout.PreferredHeight = 36;
			this.secondaryAbout.Dock = DockStyle.StackBegin;
			this.secondaryAbout.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryAbout.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryAbout.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryAbout.TabIndex = this.tabIndex++;
			this.secondaryAbout.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Séparateur.
			this.CreateBand(out leftContainer, "", BandMode.Separator, GlyphShape.None, false, 0.0);

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

				this.array.CellCountChanged -= new EventHandler(this.HandleArrayCellCountChanged);
				this.array.CellsContentChanged -= new EventHandler(this.HandleArrayCellsContentChanged);
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);

				this.labelEdit.EditionAccepted -= new EventHandler(this.HandleTextChanged);
				this.labelEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);

				this.primaryLabels.StringTextChanged -= new EventHandler(this.HandleStringTextCollectionChanged);
				this.primaryLabels.StringFocusChanged -= new EventHandler(this.HandleStringFocusCollectionChanged);

				this.secondaryLabels.StringTextChanged -= new EventHandler(this.HandleStringTextCollectionChanged);
				this.secondaryLabels.StringFocusChanged -= new EventHandler(this.HandleStringFocusCollectionChanged);

				this.primaryDescription.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.primaryDescription.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.primaryDescription.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryDescription.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.secondaryDescription.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.secondaryDescription.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.primaryAbout.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.primaryAbout.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.primaryAbout.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryAbout.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.secondaryAbout.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.secondaryAbout.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				if (this.buttonCaptionExtend != null)
				{
					this.buttonCaptionExtend.Clicked -= new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);
				}

				if (this.buttonCaptionCompact != null)
				{
					this.buttonCaptionCompact.Clicked -= new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);
				}

				if (this.buttonSuiteExtend != null)
				{
					this.buttonSuiteExtend.Clicked -= new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);
				}

				if (this.buttonSuiteCompact != null)
				{
					this.buttonSuiteCompact.Clicked -= new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);
				}
			}

			base.Dispose(disposing);
		}


		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Captions;
			}
		}


		public override void Update()
		{
			//	Met à jour le contenu du Viewer.
			this.UpdateArray();
			this.UpdateTitle();
			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		protected void UpdateColor()
		{
			//	Met à jour les couleurs dans toutes les bandes.
			int sel = this.access.AccessIndex;

			if (sel >= this.access.AccessCount)
			{
				sel = -1;
			}

			ResourceAccess.ModificationState state1 = ResourceAccess.ModificationState.Normal;
			ResourceAccess.ModificationState state2 = ResourceAccess.ModificationState.Normal;
			if (sel != -1)
			{
				state1 = this.access.GetModification(sel, null);
				state2 = this.access.GetModification(sel, this.secondaryCulture);
			}
			this.ColoriseBands(state1, state2);
		}

		protected override Widget CultureParentWidget
		{
			//	Retourne le parent à utiliser pour les boutons des cultures.
			get
			{
				return this.secondaryCultureGroup;
			}
		}

		protected void UpdateDisplayMode()
		{
			//	Metà jour le mode d'affichage des bandes.
			for (int i=0; i<this.modeContainers.Count; i++)
			{
				if (this.modeContainers[i] == BandMode.CaptionSummary)
				{
					this.bandContainers[i].Visibility = !AbstractCaptions.captionExtended;
				}

				if (this.modeContainers[i] == BandMode.CaptionView)
				{
					this.bandContainers[i].Visibility = AbstractCaptions.captionExtended;
				}

				if (this.modeContainers[i] == BandMode.SuiteSummary)
				{
					this.bandContainers[i].Visibility = !AbstractCaptions.suiteExtended;
				}

				if (this.modeContainers[i] == BandMode.SuiteView)
				{
					this.bandContainers[i].Visibility = AbstractCaptions.suiteExtended;
				}
			}
		}

		protected void UpdateTitle()
		{
			//	Met à jour le titre en dessus de la zone scrollable.
			this.titleText.Text = string.Concat("<font size=\"150%\">", this.RetTitle, "</font>");
		}

		protected virtual string RetTitle
		{
			//	Retourne le texte à utiliser pour le titre en dessus de la zone scrollable.
			get
			{
				int sel = this.access.AccessIndex;

				if (sel == -1)
				{
					return "";
				}
				else
				{
					ResourceAccess.Field field = this.access.GetField(sel, null, ResourceAccess.FieldType.Name);
					return field.String;
				}
			}
		}

		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			int sel = this.access.AccessIndex;

			if (sel >= this.access.AccessCount)
			{
				sel = -1;
			}

			if ( sel == -1 )
			{
				this.SetTextField(this.labelEdit, 0, null, ResourceAccess.FieldType.None);

				this.primarySummary.Text = "";
				this.secondarySummary.Text = "";
				this.SetTextField(this.primarySummaryIcon, 0, null, ResourceAccess.FieldType.None);
				this.SetTextField(this.secondarySummaryIcon, 0, null, ResourceAccess.FieldType.None);

				this.SetTextField(this.primaryLabels, 0, null, ResourceAccess.FieldType.None);
				this.SetTextField(this.primaryDescription, 0, null, ResourceAccess.FieldType.None);
				this.SetTextField(this.primaryIcon, 0, null, ResourceAccess.FieldType.None);
				this.primaryIconInfo.Text = "";
				this.SetTextField(this.primaryAbout, 0, null, ResourceAccess.FieldType.None);

				this.SetTextField(this.secondaryLabels, 0, null, ResourceAccess.FieldType.None);
				this.SetTextField(this.secondaryDescription, 0, null, ResourceAccess.FieldType.None);
				this.SetTextField(this.secondaryAbout, 0, null, ResourceAccess.FieldType.None);
			}
			else
			{
				Common.Types.Caption caption;

				caption = this.access.GetField(sel, null, ResourceAccess.FieldType.Caption).Caption;
				this.primarySummary.Text = ResourceAccess.GetCaptionNiceDescription(caption, 30);

				caption = this.access.GetField(sel, this.secondaryCulture, ResourceAccess.FieldType.Caption).Caption;
				this.secondarySummary.Text = ResourceAccess.GetCaptionNiceDescription(caption, 30);

				this.SetTextField(this.primarySummaryIcon, sel, null, ResourceAccess.FieldType.Icon);
				this.SetTextField(this.secondarySummaryIcon, sel, null, ResourceAccess.FieldType.Icon);

				this.SetTextField(this.labelEdit, sel, null, ResourceAccess.FieldType.Name);
				this.SetTextField(this.primaryLabels, sel, null, ResourceAccess.FieldType.Labels);
				this.SetTextField(this.primaryDescription, sel, null, ResourceAccess.FieldType.Description);
				this.SetTextField(this.primaryIcon, sel, null, ResourceAccess.FieldType.Icon);
				this.UpdateIconInfo();
				this.SetTextField(this.primaryAbout, sel, null, ResourceAccess.FieldType.About);

				if (this.secondaryCulture == null)
				{
					this.SetTextField(this.secondaryLabels, 0, null, ResourceAccess.FieldType.None);
					this.SetTextField(this.secondaryDescription, 0, null, ResourceAccess.FieldType.None);
					this.SetTextField(this.secondaryAbout, 0, null, ResourceAccess.FieldType.None);
				}
				else
				{
					this.SetTextField(this.secondaryLabels, sel, this.secondaryCulture, ResourceAccess.FieldType.Labels);
					this.SetTextField(this.secondaryDescription, sel, this.secondaryCulture, ResourceAccess.FieldType.Description);
					this.SetTextField(this.secondaryAbout, sel, this.secondaryCulture, ResourceAccess.FieldType.About);
				}

				this.labelEdit.SelectAll();
				this.labelEdit.Focus();
			}

			this.ignoreChange = iic;

			this.UpdateCommands();
		}

		protected void UpdateIconInfo()
		{
			ResourceAccess.Field field = this.access.GetField(this.access.AccessIndex, null, ResourceAccess.FieldType.Icon);

			string module, name;
			Misc.GetIconNames(field.String, out module, out name);
			
			if (string.IsNullOrEmpty(name))
			{
				this.primaryIconInfo.Text = Res.Strings.Dialog.Icon.None;
			}
			else
			{
				this.primaryIconInfo.Text = string.Format("{0}<br/>{1}", module, name);
			}
		}


		protected GlyphButton CreateBand(out MyWidgets.StackedPanel leftContainer, out MyWidgets.StackedPanel rightContainer, string title, BandMode mode, GlyphShape extendShape, bool isNewSection, double backgroundIntensity)
		{
			//	Crée une bande horizontale avec deux containers gauche/droite pour les
			//	ressources primaire/secondaire.
			Widget band = new Widget(this.scrollable.Panel);
			band.Name = "BandForLeftAndRight";
			band.Dock = DockStyle.StackBegin;
			band.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			band.TabIndex = this.tabIndex++;
			band.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			this.bandContainers.Add(band);

			leftContainer = new MyWidgets.StackedPanel(band);
			leftContainer.Name = "LeftContainer";
			leftContainer.Title = title;
			leftContainer.IsLeftPart = true;
			leftContainer.IsNewSection = isNewSection;
			leftContainer.ExtendShape = extendShape;
			leftContainer.MinWidth = 100;
			leftContainer.Dock = DockStyle.StackFill;
			leftContainer.TabIndex = this.tabIndex++;
			leftContainer.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			this.leftContainers.Add(leftContainer);

			rightContainer = new MyWidgets.StackedPanel(band);
			rightContainer.Name = "RightContainer";
			rightContainer.Title = title;
			rightContainer.IsLeftPart = false;
			rightContainer.MinWidth = 100;
			rightContainer.Dock = DockStyle.StackFill;
			rightContainer.TabIndex = this.tabIndex++;
			rightContainer.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			this.rightContainers.Add(rightContainer);

			this.modeContainers.Add(mode);
			this.intensityContainers.Add(backgroundIntensity);

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
			band.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			this.bandContainers.Add(band);

			leftContainer = new MyWidgets.StackedPanel(band);
			leftContainer.Name = "LeftContainer";
			leftContainer.Title = title;
			leftContainer.IsLeftPart = true;
			leftContainer.IsNewSection = isNewSection;
			leftContainer.ExtendShape = extendShape;
			leftContainer.MinWidth = 100;
			leftContainer.Dock = DockStyle.StackFill;
			leftContainer.TabIndex = this.tabIndex++;
			leftContainer.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			this.leftContainers.Add(leftContainer);

			this.rightContainers.Add(null);  // pour synchroniser les parties gauche/droite

			this.modeContainers.Add(mode);
			this.intensityContainers.Add(backgroundIntensity);

			return leftContainer.ExtendButton;
		}

		protected void ColoriseBands(ResourceAccess.ModificationState state1, ResourceAccess.ModificationState state2)
		{
			//	Colorise toutes les bandes horizontales.
			for (int i=0; i<this.leftContainers.Count; i++)
			{
				MyWidgets.StackedPanel lc = this.leftContainers[i];
				MyWidgets.StackedPanel rc = this.rightContainers[i];

				lc.BackgroundColor = Abstract.GetBackgroundColor(state1, this.intensityContainers[i]);

				if (rc != null)
				{
					rc.BackgroundColor = Abstract.GetBackgroundColor(state2, this.intensityContainers[i]);
					rc.Visibility = (this.secondaryCulture != null);
				}
			}
		}


		protected override void TextFieldToIndex(AbstractTextField textField, out int field, out int subfield)
		{
			//	Cherche les index correspondant à un texte éditable.
			if (textField == this.labelEdit)
			{
				field = 0;
				subfield = 0;
				return;
			}

			subfield = this.primaryLabels.GetIndex(textField);
			if (subfield != -1)
			{
				field = 1;
				return;
			}

			if (textField == this.primaryDescription)
			{
				field = 3;
				subfield = 0;
				return;
			}

			if (textField == this.primaryAbout)
			{
				field = 5;
				subfield = 0;
				return;
			}

			if (this.secondaryCulture != null)
			{
				subfield = this.secondaryLabels.GetIndex(textField);
				if (subfield != -1)
				{
					field = 2;
					return;
				}

				if (textField == this.secondaryDescription)
				{
					field = 4;
					subfield = 0;
					return;
				}

				if (textField == this.secondaryAbout)
				{
					field = 6;
					subfield = 0;
					return;
				}
			}

			field = -1;
			subfield = -1;
		}

		protected override AbstractTextField IndexToTextField(int field, int subfield)
		{
			//	Cherche le TextField permettant d'éditer des index.
			if (subfield == 0)
			{
				switch (field)
				{
					case 0:
						return this.labelEdit;

					case 3:
						return this.primaryDescription;

					case 4:
						return this.secondaryDescription;

					case 5:
						return this.primaryAbout;

					case 6:
						return this.secondaryAbout;

				}
			}

			if (field == 1)
			{
				return this.primaryLabels.GetTextField(subfield);
			}

			if (field == 2)
			{
				return this.secondaryLabels.GetTextField(subfield);
			}

			return null;
		}

		public static void SearchCreateFilterGroup(AbstractGroup parent, EventHandler handler)
		{
			StaticText label;
			CheckButton check;

			label = new StaticText(parent);
			label.PreferredWidth = 80;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Text = Res.Strings.Viewers.Captions.Labels.Title;
			label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			label.Margins = new Margins(0, 0, 0, 0);

			label = new StaticText(parent);
			label.PreferredWidth = 80;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Text = Res.Strings.Viewers.Captions.Description.Short;
			label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			label.Margins = new Margins(0, 0, 16, 0);

			label = new StaticText(parent);
			label.PreferredWidth = 80;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Text = Res.Strings.Viewers.Captions.About.Title;
			label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			label.Margins = new Margins(0, 0, 32, 0);

			check = new CheckButton(parent);
			check.Name = "0";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*0, 0, 0, 0);
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.Label);

			check = new CheckButton(parent);
			check.Name = "1";  // (*)
			check.ActiveState = ActiveState.Yes;
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 0, 0);
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryLabels);

			check = new CheckButton(parent);
			check.Name = "2";  // (*)
			check.ActiveState = ActiveState.Yes;
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 0, 0);
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryLabels);

			check = new CheckButton(parent);
			check.Name = "3";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 16, 0);
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryDescription);

			check = new CheckButton(parent);
			check.Name = "4";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 16, 0);
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryDescription);

			check = new CheckButton(parent);
			check.Name = "5";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 32, 0);
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryAbout);

			check = new CheckButton(parent);
			check.Name = "6";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 32, 0);
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryAbout);

			// (*)	Ce numéro correspond à field dans ResourceAccess.SearcherIndexToAccess !
		}


		protected void HandleButtonCompactOrExtendClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton pour changer le mode d'affichage a été cliqué.
			if (sender == this.buttonCaptionCompact)
			{
				AbstractCaptions.captionExtended = false;
			}

			if (sender == this.buttonCaptionExtend)
			{
				AbstractCaptions.captionExtended = true;
			}

			if (sender == this.buttonSuiteCompact)
			{
				AbstractCaptions.suiteExtended = false;
			}

			if (sender == this.buttonSuiteExtend)
			{
				AbstractCaptions.suiteExtended = true;
			}

			this.UpdateDisplayMode();
		}

		private void HandleSplitterDragged(object sender)
		{
			//	Le splitter a été bougé.
			Abstract.leftArrayWidth = this.left.ActualWidth;
		}

		private void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateArray();
			this.array.ShowSelectedRow();
		}

		private void HandleArrayCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a changé.
			this.UpdateArray();
		}

		private void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.access.AccessIndex = this.array.SelectedRow;
			this.UpdateTitle();
			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		private void HandleLabelKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appelé lorsque la ligne éditable pour le label voit son focus changer.
			TextFieldEx field = sender as TextFieldEx;
			field.AcceptEdition();
			this.HandleEditKeyboardFocusChanged(sender, e);
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
			int sel = this.access.AccessIndex;

			if (edit == this.labelEdit)
			{
				this.UpdateFieldName(edit, sel);
				this.UpdateTitle();
			}

			if (edit == this.primaryDescription)
			{
				this.access.SetField(sel, null, ResourceAccess.FieldType.Description, new ResourceAccess.Field(text));
			}

			if (edit == this.secondaryDescription)
			{
				this.access.SetField(sel, this.secondaryCulture, ResourceAccess.FieldType.Description, new ResourceAccess.Field(text));
			}

			if (edit == this.primaryAbout)
			{
				this.access.SetField(sel, null, ResourceAccess.FieldType.About, new ResourceAccess.Field(text));
			}

			if (edit == this.secondaryAbout)
			{
				this.access.SetField(sel, this.secondaryCulture, ResourceAccess.FieldType.About, new ResourceAccess.Field(text));
			}

			this.UpdateColor();
			this.UpdateModificationsCulture();
		}

		private void HandleStringTextCollectionChanged(object sender)
		{
			//	Une collection de textes a changé.
			if (this.ignoreChange)
			{
				return;
			}

			MyWidgets.StringCollection sc = sender as MyWidgets.StringCollection;
			int sel = this.access.AccessIndex;

			if (sc == this.primaryLabels)
			{
				this.access.SetField(sel, null, ResourceAccess.FieldType.Labels, new ResourceAccess.Field(sc.Collection));
			}

			if (sc == this.secondaryLabels)
			{
				this.access.SetField(sel, this.secondaryCulture, ResourceAccess.FieldType.Labels, new ResourceAccess.Field(sc.Collection));
			}

			this.UpdateColor();
			this.UpdateModificationsCulture();
		}

		private void HandleStringFocusCollectionChanged(object sender)
		{
			//	Le focus a changé dans une collection.
			if (this.ignoreChange)
			{
				return;
			}

			MyWidgets.StringCollection sc = sender as MyWidgets.StringCollection;
			this.currentTextField = sc.FocusedTextField;
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

		private void HandlePrimaryIconClicked(object sender, MessageEventArgs e)
		{
			//	Le boutons pour choisir l'icône a été cliqué.
			ResourceAccess.Field field = this.access.GetField(this.access.AccessIndex, null, ResourceAccess.FieldType.Icon);
			string initialIcon = field.String;

			string icon = this.module.MainWindow.DlgIcon(this.module.ResourceManager, initialIcon);

			if (icon != initialIcon)
			{
				this.access.SetField(this.access.AccessIndex, null, ResourceAccess.FieldType.Icon, new ResourceAccess.Field(icon));

				this.SetTextField(this.primaryIcon, this.access.AccessIndex, null, ResourceAccess.FieldType.Icon);
				this.UpdateIconInfo();
			}
		}


		protected static bool					captionExtended = false;
		protected static bool					suiteExtended = false;

		protected Widget						left;
		protected Widget						right;
		protected VSplitter						splitter;
		protected Widget						secondaryCultureGroup;
		protected MyWidgets.TextFieldExName		labelEdit;
		protected FrameBox						titleBox;
		protected StaticText					titleText;
		protected Scrollable					scrollable;
		protected List<Widget>					bandContainers;
		protected List<MyWidgets.StackedPanel>	leftContainers;
		protected List<MyWidgets.StackedPanel>	rightContainers;
		protected List<BandMode>				modeContainers;
		protected List<double>					intensityContainers;
		protected StaticText					primarySummary;
		protected IconButton					primarySummaryIcon;
		protected StaticText					secondarySummary;
		protected IconButton					secondarySummaryIcon;
		protected MyWidgets.StringCollection	primaryLabels;
		protected MyWidgets.StringCollection	secondaryLabels;
		protected TextFieldMulti				primaryDescription;
		protected TextFieldMulti				secondaryDescription;
		protected IconButton					primaryIcon;
		protected StaticText					primaryIconInfo;
		protected TextFieldMulti				primaryAbout;
		protected TextFieldMulti				secondaryAbout;
		protected GlyphButton					buttonCaptionExtend;
		protected GlyphButton					buttonCaptionCompact;
		protected GlyphButton					buttonSuiteExtend;
		protected GlyphButton					buttonSuiteCompact;
	}
}
