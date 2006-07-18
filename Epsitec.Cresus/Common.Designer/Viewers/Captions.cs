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
	public class Captions : Abstract
	{
		public Captions(Module module, PanelsContext context, ResourceAccess access) : base(module, context, access)
		{
			int tabIndex = 0;

			//	Crée les 2 parties gauche/droite séparées par un splitter.
			Widget left = new Widget(this);
			left.MinWidth = 80;
			left.MaxWidth = 400;
			left.PreferredWidth = 200;
			left.Dock = DockStyle.Left;
			left.Padding = new Margins(10, 10, 10, 10);

			VSplitter splitter = new VSplitter(this);
			splitter.Dock = DockStyle.Left;
			VSplitter.SetAutoCollapseEnable(left, true);

			Widget right = new Widget(this);
			right.MinWidth = 200;
			right.Dock = DockStyle.Fill;
			right.Padding = new Margins(1, 1, 1, 1);
			
			//	Crée la partie gauche.			
			this.labelEdit = new TextFieldEx(left);
			this.labelEdit.Name = "LabelEdit";
			this.labelEdit.Margins = new Margins(0, 0, 10, 0);
			this.labelEdit.Dock = DockStyle.Bottom;
			this.labelEdit.EditionAccepted += new EventHandler(this.HandleTextChanged);
			this.labelEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.labelEdit.TabIndex = tabIndex++;
			this.labelEdit.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.labelEdit.Visibility = (this.module.Mode == DesignerMode.Build);

			this.array = new MyWidgets.StringArray(left);
			this.array.Columns = 1;
			this.array.SetColumnsRelativeWidth(0, 1.00);
			this.array.SetDynamicsToolTips(0, true);
			this.array.Margins = new Margins(0, 0, 0, 0);
			this.array.Dock = DockStyle.Fill;
			this.array.CellCountChanged += new EventHandler(this.HandleArrayCellCountChanged);
			this.array.CellsContentChanged += new EventHandler(this.HandleArrayCellsContentChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
			this.array.TabIndex = tabIndex++;
			this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Crée la partie droite, bande supérieure pour les boutons des cultures.
			Widget sup = new Widget(right);
			sup.PreferredHeight = 40;
			sup.Padding = new Margins(11, 27, 10, 0);
			sup.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			sup.Dock = DockStyle.Top;

			this.primaryCulture = new IconButtonMark(sup);
			this.primaryCulture.ButtonStyle = ButtonStyle.ActivableIcon;
			this.primaryCulture.SiteMark = SiteMark.OnBottom;
			this.primaryCulture.MarkDimension = 10;
			this.primaryCulture.PreferredHeight = 35;
			this.primaryCulture.ActiveState = ActiveState.Yes;
			this.primaryCulture.AutoFocus = false;
			this.primaryCulture.TabIndex = tabIndex++;
			this.primaryCulture.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.primaryCulture.Margins = new Margins(0, 10, 0, 0);
			this.primaryCulture.Dock = DockStyle.StackFill;

			this.secondaryCultureGroup = new Widget(sup);
			this.secondaryCultureGroup.Margins = new Margins(10, 0, 0, 0);
			this.secondaryCultureGroup.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			this.secondaryCultureGroup.Dock = DockStyle.StackFill;

			//	Crée la partie droite, bande inférieure pour la zone d'étition scrollable.
			this.scrollable = new Scrollable(right);
			this.scrollable.MinWidth = 100;
			this.scrollable.MinHeight = 100;
			this.scrollable.Margins = new Margins(1, 1, 1, 1);
			this.scrollable.Dock = DockStyle.Fill;
			this.scrollable.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
			this.scrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.scrollable.Panel.IsAutoFitting = true;
			this.scrollable.IsForegroundFrame = true;

			this.scrollable.Panel.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;

			this.leftContainers = new List<MyWidgets.StackedPanel>();
			this.rightContainers = new List<MyWidgets.StackedPanel>();
			this.intensityContainers = new List<double>();

			MyWidgets.StackedPanel leftContainer, rightContainer;

			//	Textes.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.Labels, 0.5);

			this.primaryLabels = new MyWidgets.StringCollection(leftContainer.Container);
			this.primaryLabels.Dock = DockStyle.StackBegin;
			this.primaryLabels.StringTextChanged += new EventHandler(this.HandleStringTextCollectionChanged);
			this.primaryLabels.StringFocusChanged += new EventHandler(this.HandleStringFocusCollectionChanged);
			this.primaryLabels.TabIndex = tabIndex++;
			this.primaryLabels.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.secondaryLabels = new MyWidgets.StringCollection(rightContainer.Container);
			this.secondaryLabels.Dock = DockStyle.StackBegin;
			this.secondaryLabels.StringTextChanged += new EventHandler(this.HandleStringTextCollectionChanged);
			this.secondaryLabels.StringFocusChanged += new EventHandler(this.HandleStringFocusCollectionChanged);
			this.secondaryLabels.TabIndex = tabIndex++;
			this.secondaryLabels.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Description.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.Description, 0.3);

			this.primaryDescription = new TextFieldMulti(leftContainer.Container);
			this.primaryDescription.PreferredHeight = 70;
			this.primaryDescription.Dock = DockStyle.StackBegin;
			this.primaryDescription.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryDescription.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryDescription.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryDescription.TabIndex = tabIndex++;
			this.primaryDescription.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.secondaryDescription = new TextFieldMulti(rightContainer.Container);
			this.secondaryDescription.PreferredHeight = 70;
			this.secondaryDescription.Dock = DockStyle.StackBegin;
			this.secondaryDescription.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryDescription.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryDescription.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryDescription.TabIndex = tabIndex++;
			this.secondaryDescription.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Icône.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.Icon, 0.1);

			this.primaryIcon = new IconButton(leftContainer.Container);
			this.primaryIcon.PreferredHeight = 30;
			this.primaryIcon.PreferredWidth = 30;
			this.primaryIcon.ButtonStyle = ButtonStyle.ActivableIcon;
			this.primaryIcon.Anchor = AnchorStyles.TopLeft;
			this.primaryIcon.TabIndex = tabIndex++;
			this.primaryIcon.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.secondaryIcon = new IconButton(rightContainer.Container);
			this.secondaryIcon.PreferredHeight = 30;
			this.secondaryIcon.PreferredWidth = 30;
			this.secondaryIcon.ButtonStyle = ButtonStyle.ActivableIcon;
			this.secondaryIcon.Anchor = AnchorStyles.TopLeft;
			this.secondaryIcon.TabIndex = tabIndex++;
			this.secondaryIcon.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Commentaires.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.About, 0.7);

			this.primaryAbout = new TextFieldMulti(leftContainer.Container);
			this.primaryAbout.PreferredHeight = 50;
			this.primaryAbout.Dock = DockStyle.StackBegin;
			this.primaryAbout.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryAbout.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryAbout.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryAbout.TabIndex = tabIndex++;
			this.primaryAbout.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.secondaryAbout = new TextFieldMulti(rightContainer.Container);
			this.secondaryAbout.PreferredHeight = 50;
			this.secondaryAbout.Dock = DockStyle.StackBegin;
			this.secondaryAbout.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryAbout.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryAbout.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryAbout.TabIndex = tabIndex++;
			this.secondaryAbout.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;


			this.UpdateCultures();
			this.Update();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.array.CellCountChanged -= new EventHandler(this.HandleArrayCellCountChanged);
				this.array.CellsContentChanged -= new EventHandler(this.HandleArrayCellsContentChanged);
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);

				this.labelEdit.EditionAccepted -= new EventHandler(this.HandleTextChanged);
				this.labelEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

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
			}

			base.Dispose(disposing);
		}


		protected override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Captions;
			}
		}

		public override ResourceAccess.Type BundleType
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
			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		protected void UpdateColor()
		{
			//	Met à jour les culeurs dans toutes les bandes.
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
				this.SetTextField(this.labelEdit, 0, null, null);

				this.SetTextField(this.primaryLabels, 0, null, null);
				this.SetTextField(this.primaryDescription, 0, null, null);
				this.SetTextField(this.primaryIcon, 0, null, null);
				this.SetTextField(this.primaryAbout, 0, null, null);
				
				this.SetTextField(this.secondaryLabels, 0, null, null);
				this.SetTextField(this.secondaryDescription, 0, null, null);
				this.SetTextField(this.secondaryIcon, 0, null, null);
				this.SetTextField(this.secondaryAbout, 0, null, null);
			}
			else
			{
				int index = this.access.AccessIndex;

				this.SetTextField(this.labelEdit, index, null, "Name");
				this.SetTextField(this.primaryLabels, index, null, "Labels");
				this.SetTextField(this.primaryDescription, index, null, "Description");
				this.SetTextField(this.primaryIcon, index, null, "Icon");
				this.SetTextField(this.primaryAbout, index, null, "About");

				if (this.secondaryCulture == null)
				{
					this.SetTextField(this.secondaryLabels, 0, null, null);
					this.SetTextField(this.secondaryDescription, 0, null, null);
					this.SetTextField(this.secondaryIcon, 0, null, null);
					this.SetTextField(this.secondaryAbout, 0, null, null);
				}
				else
				{
					this.SetTextField(this.secondaryLabels, index, this.secondaryCulture, "Labels");
					this.SetTextField(this.secondaryDescription, index, this.secondaryCulture, "Description");
					this.SetTextField(this.secondaryIcon, index, this.secondaryCulture, "Icon");
					this.SetTextField(this.secondaryAbout, index, this.secondaryCulture, "About");
				}

				this.labelEdit.Focus();
				this.labelEdit.SelectAll();
			}

			this.ignoreChange = iic;

			this.UpdateCommands();
		}

		public override void UpdateCommands()
		{
			//	Met à jour les commandes en fonction de la ressource sélectionnée.
			base.UpdateCommands();

			int sel = this.access.AccessIndex;
			int count = this.access.AccessCount;
			bool build = (this.module.Mode == DesignerMode.Build);
			bool search = this.module.MainWindow.DialogSearch.IsActionsEnabled;
			bool newCulture = (this.access.CultureCount < Misc.Cultures.Length);

			bool all = false;
			bool modified = false;
			if (sel != -1 && this.secondaryCulture != null)
			{
				ResourceAccess.ModificationState state = this.access.GetModification(sel, this.secondaryCulture);
				modified = (state != ResourceAccess.ModificationState.Normal);
			}

			this.GetCommandState("NewCulture").Enable = newCulture;
			this.GetCommandState("DeleteCulture").Enable = true;

			this.GetCommandState("Search").Enable = true;
			this.GetCommandState("SearchPrev").Enable = search;
			this.GetCommandState("SearchNext").Enable = search;

			this.GetCommandState("ModificationPrev").Enable = true;
			this.GetCommandState("ModificationNext").Enable = true;
			this.GetCommandState("ModificationAll").Enable = (sel != -1 && all);
			this.GetCommandState("ModificationClear").Enable = (sel != -1 && modified);

			this.GetCommandState("FontBold").Enable = (sel != -1);
			this.GetCommandState("FontItalic").Enable = (sel != -1);
			this.GetCommandState("FontUnderlined").Enable = (sel != -1);
			this.GetCommandState("Glyphs").Enable = (sel != -1);

			this.GetCommandState("PanelDelete").Enable = false;
			this.GetCommandState("PanelDuplicate").Enable = false;
			this.GetCommandState("PanelDeselectAll").Enable = false;
			this.GetCommandState("PanelSelectAll").Enable = false;
			this.GetCommandState("PanelSelectInvert").Enable = false;

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

			this.GetCommandState("OrderUpAll").Enable = false;
			this.GetCommandState("OrderDownAll").Enable = false;
			this.GetCommandState("OrderUpOne").Enable = false;
			this.GetCommandState("OrderDownOne").Enable = false;

			this.GetCommandState("TabIndexClear").Enable = false;
			this.GetCommandState("TabIndexRenum").Enable = false;
			this.GetCommandState("TabIndexLast").Enable = false;
			this.GetCommandState("TabIndexPrev").Enable = false;
			this.GetCommandState("TabIndexNext").Enable = false;
			this.GetCommandState("TabIndexFirst").Enable = false;

			this.module.MainWindow.UpdateInfoCurrentModule();
			this.module.MainWindow.UpdateInfoAccess();
			this.module.MainWindow.UpdateInfoViewer();
		}


		protected void CreateBand(out MyWidgets.StackedPanel leftContainer, out MyWidgets.StackedPanel rightContainer, string title, double backgroundIntensity)
		{
			//	Crée une bande horizontale avec deux containers gauche/droite pour les
			//	ressources primaire/secondaire.
			Widget band = new Widget(this.scrollable.Panel);
			band.Dock = DockStyle.StackBegin;
			band.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			leftContainer = new MyWidgets.StackedPanel(band);
			leftContainer.Title = title;
			leftContainer.IsLeftPart = true;
			leftContainer.MinWidth = 100;
			leftContainer.Dock = DockStyle.StackFill;
			this.leftContainers.Add(leftContainer);

			rightContainer = new MyWidgets.StackedPanel(band);
			rightContainer.Title = title;
			rightContainer.IsLeftPart = false;
			rightContainer.MinWidth = 100;
			rightContainer.Dock = DockStyle.StackFill;
			this.rightContainers.Add(rightContainer);

			this.intensityContainers.Add(backgroundIntensity);
		}

		protected void ColoriseBands(ResourceAccess.ModificationState state1, ResourceAccess.ModificationState state2)
		{
			//	Colorise toutes les bandes horizontales.
			for (int i=0; i<this.leftContainers.Count; i++)
			{
				MyWidgets.StackedPanel lc = this.leftContainers[i];
				MyWidgets.StackedPanel rc = this.rightContainers[i];

				lc.BackgroundColor = Abstract.GetBackgroundColor(state1, this.intensityContainers[i]);

				rc.BackgroundColor = Abstract.GetBackgroundColor(state2, this.intensityContainers[i]);
				rc.Visibility = (this.secondaryCulture != null);
			}
		}


		#region FixFilter
		protected static string AddFilter(string name)
		{
			//	Ajoute le filtre fixe si nécessaire.
			if (!Captions.HasFixFilter(name))
			{
				return Captions.FixFilter + name;
			}

			return name;
		}

		public static string SubFilter(string name)
		{
			//	Supprime le filtre fixe si nécessaire.
			if (Captions.HasFixFilter(name))
			{
				return name.Substring(Captions.FixFilter.Length);
			}

			return name;
		}

		protected static bool HasFixFilter(string name)
		{
			//	Indique si un nom commence par le filtre fixe.
			return name.StartsWith(Captions.FixFilter);
		}

		protected static string FixFilter = "Cap.";
		#endregion


		void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateArray();
		}

		void HandleArrayCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a changé.
			this.UpdateArray();
		}

		void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.access.AccessIndex = this.array.SelectedRow;
			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		void HandleTextChanged(object sender)
		{
			//	Un texte éditable a changé.
			if ( this.ignoreChange )  return;

			AbstractTextField edit = sender as AbstractTextField;
			string text = edit.Text;
			int sel = this.access.AccessIndex;

			if (edit == this.labelEdit)
			{
				this.UpdateFieldName(edit, sel);
			}

			if (edit == this.primaryDescription)
			{
				this.access.SetField(sel, null, "Description", new ResourceAccess.Field(text));
			}

			if (edit == this.secondaryDescription)
			{
				this.access.SetField(sel, this.secondaryCulture, "Description", new ResourceAccess.Field(text));
			}

			if (edit == this.primaryAbout)
			{
				this.access.SetField(sel, null, "About", new ResourceAccess.Field(text));
			}

			if (edit == this.secondaryAbout)
			{
				this.access.SetField(sel, this.secondaryCulture, "About", new ResourceAccess.Field(text));
			}

			this.UpdateColor();
			this.UpdateModificationsCulture();
		}

		void HandleStringTextCollectionChanged(object sender)
		{
			//	Une collection de textes a changé.
			if ( this.ignoreChange )  return;

			MyWidgets.StringCollection sc = sender as MyWidgets.StringCollection;
			int sel = this.access.AccessIndex;

			if (sc == this.primaryLabels)
			{
				this.access.SetField(sel, null, "Labels", new ResourceAccess.Field(sc.Collection));
			}

			if (sc == this.secondaryLabels)
			{
				this.access.SetField(sel, this.secondaryCulture, "Labels", new ResourceAccess.Field(sc.Collection));
			}

			this.UpdateColor();
			this.UpdateModificationsCulture();
		}

		void HandleStringFocusCollectionChanged(object sender)
		{
			//	Le focus a changé dans une collection.
			if ( this.ignoreChange )  return;

			MyWidgets.StringCollection sc = sender as MyWidgets.StringCollection;
			this.currentTextField = sc.FocusedTextField;
		}

		void HandleCursorChanged(object sender)
		{
			//	Le curseur a été déplacé dans un texte éditable.
			if ( this.ignoreChange )  return;

			this.lastActionIsReplace = false;
		}


		protected Widget						secondaryCultureGroup;
		protected TextFieldEx					labelEdit;
		protected Scrollable					scrollable;
		protected List<MyWidgets.StackedPanel>	leftContainers;
		protected List<MyWidgets.StackedPanel>	rightContainers;
		protected List<double>					intensityContainers;
		protected MyWidgets.StringCollection	primaryLabels;
		protected MyWidgets.StringCollection	secondaryLabels;
		protected TextFieldMulti				primaryDescription;
		protected TextFieldMulti				secondaryDescription;
		protected IconButton					primaryIcon;
		protected IconButton					secondaryIcon;
		protected TextFieldMulti				primaryAbout;
		protected TextFieldMulti				secondaryAbout;
	}
}
