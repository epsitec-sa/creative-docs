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
		public Captions(Module module, PanelsContext context) : base(module, context)
		{
			MyWidgets.StackedPanel panel;
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

			this.secondaryCulture = new Widget(sup);
			this.secondaryCulture.Margins = new Margins(10-1, 0, 0, 0);
			this.secondaryCulture.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			this.secondaryCulture.Dock = DockStyle.StackFill;

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

			this.leftContainers = new List<Widget>();
			this.rightContainers = new List<Widget>();

			Widget leftContainer, rightContainer;

			//	Textes.
			this.CreateBand(out leftContainer, out rightContainer, 0.4);

			panel = new MyWidgets.StackedPanel(leftContainer);
			panel.IsLeftPart = true;
			panel.Title = Res.Strings.Viewers.Captions.Labels;
			panel.Dock = DockStyle.StackBegin;

			this.primaryLabels = new MyWidgets.StringCollection(panel.Container);
			this.primaryLabels.Dock = DockStyle.StackBegin;
			this.primaryLabels.StringChanged += new EventHandler(this.HandleStringCollectionChanged);
			this.primaryLabels.TabIndex = tabIndex++;
			this.primaryLabels.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			panel = new MyWidgets.StackedPanel(rightContainer);
			panel.IsLeftPart = false;
			panel.Title = Res.Strings.Viewers.Captions.Labels;
			panel.Dock = DockStyle.StackBegin;

			this.secondaryLabels = new MyWidgets.StringCollection(panel.Container);
			this.secondaryLabels.Dock = DockStyle.StackBegin;
			this.secondaryLabels.StringChanged += new EventHandler(this.HandleStringCollectionChanged);
			this.secondaryLabels.TabIndex = tabIndex++;
			this.secondaryLabels.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Description.
			this.CreateBand(out leftContainer, out rightContainer, 0.2);

			panel = new MyWidgets.StackedPanel(leftContainer);
			panel.IsLeftPart = true;
			panel.Title = Res.Strings.Viewers.Captions.Description;
			panel.Dock = DockStyle.StackBegin;

			this.primaryDescription = new TextFieldMulti(panel.Container);
			this.primaryDescription.PreferredHeight = 70;
			this.primaryDescription.Dock = DockStyle.StackBegin;
			this.primaryDescription.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryDescription.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryDescription.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryDescription.TabIndex = tabIndex++;
			this.primaryDescription.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			panel = new MyWidgets.StackedPanel(rightContainer);
			panel.IsLeftPart = false;
			panel.Title = Res.Strings.Viewers.Captions.Description;
			panel.Dock = DockStyle.StackBegin;

			this.secondaryDescription = new TextFieldMulti(panel.Container);
			this.secondaryDescription.PreferredHeight = 70;
			this.secondaryDescription.Dock = DockStyle.StackBegin;
			this.secondaryDescription.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryDescription.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryDescription.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryDescription.TabIndex = tabIndex++;
			this.secondaryDescription.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Icône.
			this.CreateBand(out leftContainer, out rightContainer, 0.0);

			panel = new MyWidgets.StackedPanel(leftContainer);
			panel.IsLeftPart = true;
			panel.Title = Res.Strings.Viewers.Captions.Icon;
			panel.Dock = DockStyle.StackBegin;

			this.primaryIcon = new IconButton(panel.Container);
			this.primaryIcon.PreferredHeight = 30;
			this.primaryIcon.PreferredWidth = 30;
			this.primaryIcon.ButtonStyle = ButtonStyle.ActivableIcon;
			this.primaryIcon.Anchor = AnchorStyles.TopLeft;
			this.primaryIcon.TabIndex = tabIndex++;
			this.primaryIcon.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			panel = new MyWidgets.StackedPanel(rightContainer);
			panel.IsLeftPart = false;
			panel.Title = Res.Strings.Viewers.Captions.Icon;
			panel.Dock = DockStyle.StackBegin;

			this.secondaryIcon = new IconButton(panel.Container);
			this.secondaryIcon.PreferredHeight = 30;
			this.secondaryIcon.PreferredWidth = 30;
			this.secondaryIcon.ButtonStyle = ButtonStyle.ActivableIcon;
			this.secondaryIcon.Anchor = AnchorStyles.TopLeft;
			this.secondaryIcon.TabIndex = tabIndex++;
			this.secondaryIcon.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Commentaires.
			this.CreateBand(out leftContainer, out rightContainer, 0.6);

			panel = new MyWidgets.StackedPanel(leftContainer);
			panel.IsLeftPart = true;
			panel.Title = Res.Strings.Viewers.Captions.About;
			panel.Dock = DockStyle.StackBegin;

			this.primaryAbout = new TextFieldMulti(panel.Container);
			this.primaryAbout.PreferredHeight = 50;
			this.primaryAbout.Dock = DockStyle.StackBegin;
			this.primaryAbout.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryAbout.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryAbout.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryAbout.TabIndex = tabIndex++;
			this.primaryAbout.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			panel = new MyWidgets.StackedPanel(rightContainer);
			panel.IsLeftPart = false;
			panel.Title = Res.Strings.Viewers.Captions.About;
			panel.Dock = DockStyle.StackBegin;

			this.secondaryAbout = new TextFieldMulti(panel.Container);
			this.secondaryAbout.PreferredHeight = 50;
			this.secondaryAbout.Dock = DockStyle.StackBegin;
			this.secondaryAbout.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryAbout.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryAbout.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryAbout.TabIndex = tabIndex++;
			this.secondaryAbout.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Ligne horizontale pour terminer proprement.
			Separator sep = new Separator(this.scrollable.Panel);
			sep.PreferredHeight = 1;
			sep.Dock = DockStyle.StackBegin;

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

				this.primaryLabels.StringChanged -= new EventHandler(this.HandleStringCollectionChanged);

				this.secondaryLabels.StringChanged -= new EventHandler(this.HandleStringCollectionChanged);

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


		public override Module.BundleType BundleType
		{
			get
			{
				return Module.BundleType.Captions;
			}
		}


		public override void DoSearch(string search, Searcher.SearchingMode mode)
		{
			//	Effectue une recherche.
			Searcher searcher = new Searcher(this.druidsIndex, this.primaryBundle, this.secondaryBundle, this.BundleType);
			searcher.FixStarting(mode, this.array.SelectedRow, this.currentTextField, false);

			if (searcher.Search(search))
			{
				this.lastActionIsReplace = false;

				this.array.SelectedRow = searcher.Row;
				this.array.ShowSelectedRow();

				AbstractTextField edit = null;
				if (searcher.Field == 0)  edit = this.labelEdit;
				if (searcher.Field == 1)  edit = this.primaryDescription;
				if (searcher.Field == 2)  edit = this.secondaryDescription;
				if (searcher.Field == 3)  edit = this.primaryAbout;
				if (searcher.Field == 4)  edit = this.secondaryAbout;
				if (edit != null && edit.Visibility)
				{
					this.ignoreChange = true;

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
				this.module.MainWindow.DialogError(Res.Strings.Dialog.Search.Message.Error);
			}
		}

		public override void DoCount(string search, Searcher.SearchingMode mode)
		{
			//	Effectue un comptage.
			Searcher searcher = new Searcher(this.druidsIndex, this.primaryBundle, this.secondaryBundle, this.BundleType);
			searcher.FixStarting(mode, this.array.SelectedRow, this.currentTextField, false);

			int count = searcher.Count(search);
			if (count == 0)
			{
				this.module.MainWindow.DialogError(Res.Strings.Dialog.Search.Message.Error);
			}
			else
			{
				string message = string.Format(Res.Strings.Dialog.Search.Message.Count, count.ToString());
				this.module.MainWindow.DialogMessage(message);
			}
		}

		public override void DoReplace(string search, string replace, Searcher.SearchingMode mode)
		{
			//	Effectue un remplacement.
			if (this.module.Mode == DesignerMode.Translate)
			{
				mode &= ~Searcher.SearchingMode.SearchInLabel;
			}

			Searcher searcher = new Searcher(this.druidsIndex, this.primaryBundle, this.secondaryBundle, this.BundleType);
			searcher.FixStarting(mode, this.array.SelectedRow, this.currentTextField, this.lastActionIsReplace);

			if (searcher.Replace(search, false))
			{
				this.lastActionIsReplace = true;

				this.array.SelectedRow = searcher.Row;
				this.array.ShowSelectedRow();

				Druid druid = this.druidsIndex[searcher.Row];
				string text = "";

				if (searcher.Field == 0)
				{
					string validReplace = replace;
					if (!Misc.IsValidLabel(ref validReplace))
					{
						this.module.MainWindow.DialogError(Res.Strings.Error.InvalidLabel);
						return;
					}

					if (this.IsExistingName(validReplace))
					{
						this.module.MainWindow.DialogError(Res.Strings.Error.NameAlreadyExist);
						return;
					}

					text = this.primaryBundle[druid].AsString;
					text = text.Remove(searcher.Index, searcher.Length);
					text = text.Insert(searcher.Index, validReplace);

					this.module.Modifier.Rename(this.BundleType, druid, text);
					this.array.SetLineString(0, searcher.Row, text);
				}

				if (searcher.Field == 1 && this.secondaryBundle != null)
				{
					text = this.primaryBundle[druid].AsString;
					text = text.Remove(searcher.Index, searcher.Length);
					text = text.Insert(searcher.Index, replace);
					this.primaryBundle[druid].SetStringValue(text);
				}

				if (searcher.Field == 2 && this.secondaryBundle != null)
				{
					text = this.secondaryBundle[druid].AsString;
					text = text.Remove(searcher.Index, searcher.Length);
					text = text.Insert(searcher.Index, replace);
					this.secondaryBundle[druid].SetStringValue(text);
				}

				if (searcher.Field == 3)
				{
					text = this.primaryBundle[druid].About;
					text = text.Remove(searcher.Index, searcher.Length);
					text = text.Insert(searcher.Index, replace);
					this.primaryBundle[druid].SetAbout(text);
				}

				if (searcher.Field == 4 && this.secondaryBundle != null)
				{
					text = this.secondaryBundle[druid].About;
					text = text.Remove(searcher.Index, searcher.Length);
					text = text.Insert(searcher.Index, replace);
					this.secondaryBundle[druid].SetAbout(text);
				}

				AbstractTextField edit = null;
				if (searcher.Field == 0)  edit = this.labelEdit;
				if (searcher.Field == 1)  edit = this.primaryDescription;
				if (searcher.Field == 2)  edit = this.secondaryDescription;
				if (searcher.Field == 3)  edit = this.primaryAbout;
				if (searcher.Field == 4)  edit = this.secondaryAbout;
				if (edit != null && edit.Visibility)
				{
					this.ignoreChange = true;

					this.Window.MakeActive();
					edit.Focus();
					edit.Text = text;
					edit.CursorFrom  = edit.TextLayout.FindIndexFromOffset(searcher.Index);
					edit.CursorTo    = edit.TextLayout.FindIndexFromOffset(searcher.Index+replace.Length);
					edit.CursorAfter = false;

					this.ignoreChange = false;
				}

				this.module.Modifier.IsDirty = true;
			}
			else
			{
				this.module.MainWindow.DialogError(Res.Strings.Dialog.Search.Message.Error);
			}
		}

		public override void DoReplaceAll(string search, string replace, Searcher.SearchingMode mode)
		{
			//	Effectue un 'remplacer tout'.
			if (this.module.Mode == DesignerMode.Translate)
			{
				mode &= ~Searcher.SearchingMode.SearchInLabel;
			}

			Searcher searcher = new Searcher(this.druidsIndex, this.primaryBundle, this.secondaryBundle, this.BundleType);
			searcher.FixStarting(mode, this.array.SelectedRow, this.currentTextField, false);

			int count = 0;
			bool fromBeginning = true;
			while (searcher.Replace(search, fromBeginning))
			{
				fromBeginning = false;
				count ++;

				Druid druid = this.druidsIndex[searcher.Row];
				string text = "";

				if (searcher.Field == 0)
				{
					text = this.primaryBundle[druid].Name;
					text = text.Remove(searcher.Index, searcher.Length);
					text = text.Insert(searcher.Index, replace);

					this.module.Modifier.Rename(this.BundleType, druid, text);
				}

				if (searcher.Field == 1)
				{
					text = this.primaryBundle[druid].AsString;
					text = text.Remove(searcher.Index, searcher.Length);
					text = text.Insert(searcher.Index, replace);
					this.primaryBundle[druid].SetStringValue(text);
				}

				if (searcher.Field == 2 && this.secondaryBundle != null)
				{
					text = this.secondaryBundle[druid].AsString;
					text = text.Remove(searcher.Index, searcher.Length);
					text = text.Insert(searcher.Index, replace);
					this.secondaryBundle[druid].SetStringValue(text);
				}

				if (searcher.Field == 3)
				{
					text = this.primaryBundle[druid].About;
					text = text.Remove(searcher.Index, searcher.Length);
					text = text.Insert(searcher.Index, replace);
					this.primaryBundle[druid].SetAbout(text);
				}

				if (searcher.Field == 4 && this.secondaryBundle != null)
				{
					text = this.secondaryBundle[druid].About;
					text = text.Remove(searcher.Index, searcher.Length);
					text = text.Insert(searcher.Index, replace);
					this.secondaryBundle[druid].SetAbout(text);
				}

				searcher.Skip(replace.Length);  // saute les caractères sélectionnés
			}

			if (count == 0)
			{
				this.module.MainWindow.DialogError(Res.Strings.Dialog.Search.Message.Error);
			}
			else
			{
				this.UpdateArray();
				this.UpdateEdit();
				this.UpdateCommands();
				this.module.Modifier.IsDirty = true;

				string text = string.Format(Res.Strings.Dialog.Search.Message.Replace, count.ToString());
				this.module.MainWindow.DialogMessage(text);
			}
		}

		public override void DoModification(string name)
		{
			//	Change la ressource modifiée visible.
		}

		public override void DoDelete()
		{
			//	Supprime la ressource sélectionnée.
			int sel = this.array.SelectedRow;
			if ( sel == -1 )  return;

			Druid druid = this.druidsIndex[sel];
			this.module.Modifier.Delete(this.BundleType, druid);

			this.druidsIndex.RemoveAt(sel);
			this.UpdateArray();

			sel = System.Math.Min(sel, this.druidsIndex.Count-1);
			this.array.SelectedRow = sel;
			this.array.ShowSelectedRow();
			this.UpdateCommands();
			this.module.Modifier.IsDirty = true;
		}

		public override void DoDuplicate(bool duplicate)
		{
			//	Duplique la ressource sélectionnée.
			int sel = this.array.SelectedRow;
			if ( sel == -1 )  return;

			Druid druid = this.druidsIndex[sel];
			string newName = this.GetDuplicateName(this.primaryBundle[druid].Name);
			Druid newDruid = this.module.Modifier.Duplicate(this.BundleType, druid, newName, duplicate);

			int newSel = sel+1;
			this.druidsIndex.Insert(newSel, newDruid);
			this.UpdateArray();

			this.array.SelectedRow = newSel;
			this.array.ShowSelectedRow();
			this.UpdateCommands();
			this.module.Modifier.IsDirty = true;
		}

		public override void DoMove(int direction)
		{
			//	Déplace la ressource sélectionnée.
			int sel = this.array.SelectedRow;
			if ( sel == -1 )  return;

			Druid druid = this.druidsIndex[sel];
			if ( !this.module.Modifier.Move(this.BundleType, druid, direction) )  return;
		
			int newSel = sel+direction;
			this.druidsIndex.RemoveAt(sel);
			this.druidsIndex.Insert(newSel, druid);
			this.UpdateArray();

			this.array.SelectedRow = newSel;
			this.array.ShowSelectedRow();
			this.UpdateCommands();
			this.module.Modifier.IsDirty = true;
		}

		public override void DoNewCulture()
		{
			//	Crée une nouvelle culture.
			string name = this.module.MainWindow.DlgNewCulture();
			if ( name == null )  return;
			ResourceBundle bundle = this.module.NewCulture(name, this.BundleType);

			this.UpdateCultures();
			this.Update();
			this.module.Modifier.IsDirty = true;
		}

		public override void DoDeleteCulture()
		{
			//	Supprime la culture courante.
			string question = string.Format(Res.Strings.Dialog.DeleteCulture.Question, Misc.CultureName(this.secondaryBundle.Culture));
			Common.Dialogs.DialogResult result = this.module.MainWindow.DialogQuestion(question);
			if ( result != Epsitec.Common.Dialogs.DialogResult.Yes )  return;

			this.module.DeleteCulture(this.secondaryBundle, this.BundleType);

			this.UpdateCultures();
			if (this.secondaryBundle != null)
			{
				this.UpdateSelectedCulture(Misc.CultureName(this.secondaryBundle.Culture));
			}
			this.Update();
			this.module.Modifier.IsDirty = true;
		}

		public override void DoClipboard(string name)
		{
			//	Effectue une action avec le bloc-notes.
		}

		public override void DoFont(string name)
		{
			//	Effectue une modification de typographie.
		}


		public override void Update()
		{
			//	Met à jour le contenu du Viewer.
			this.UpdateArray();
			this.UpdateEdit();
			this.UpdateCommands();
		}

		protected void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			int sel = this.array.SelectedRow;

			if (sel >= this.druidsIndex.Count)
			{
				sel = -1;
			}

			foreach (Widget container in this.rightContainers)
			{
				container.Visibility = (this.secondaryBundle != null);
			}

			if ( sel == -1 )
			{
				this.labelEdit.Enable = false;
				this.labelEdit.Text = "";

				this.primaryLabels.Enable = false;
				this.primaryLabels.Collection = null;

				this.primaryDescription.Enable = false;
				this.primaryDescription.Text = "";

				this.primaryIcon.Enable = false;
				this.primaryIcon.IconName = null;

				this.primaryAbout.Enable = false;
				this.primaryAbout.Text = "";

				this.secondaryLabels.Enable = false;
				this.secondaryLabels.Collection = null;

				this.secondaryDescription.Enable = false;
				this.secondaryDescription.Text = "";

				this.secondaryIcon.Enable = false;
				this.secondaryIcon.IconName = null;

				this.secondaryAbout.Enable = false;
				this.secondaryAbout.Text = "";
			}
			else
			{
				Druid druid = this.druidsIndex[sel];

				this.labelEdit.Enable = true;
				this.SetTextField(this.labelEdit, this.primaryBundle[druid].Name);

				this.primaryLabels.Enable = true;
				this.primaryLabels.Collection = this.GetCaptionLabels(this.primaryBundle, druid);

				this.primaryDescription.Enable = true;
				this.SetTextField(this.primaryDescription, this.GetCaptionDescription(this.primaryBundle, druid));

				this.primaryIcon.Enable = true;
				this.primaryIcon.IconName = this.GetCaptionIcon(this.primaryBundle, druid);

				this.primaryAbout.Enable = true;
				this.SetTextField(this.primaryAbout, this.primaryBundle[druid].About);

				if (this.secondaryBundle == null)
				{
					this.secondaryLabels.Enable = false;
					this.secondaryLabels.Collection = null;

					this.secondaryDescription.Enable = false;
					this.secondaryDescription.Text = "";

					this.secondaryIcon.Enable = false;
					this.secondaryIcon.IconName = null;

					this.secondaryAbout.Enable = false;
					this.secondaryAbout.Text = "";
				}
				else
				{
					this.secondaryLabels.Enable = true;
					this.secondaryLabels.Collection = this.GetCaptionLabels(this.secondaryBundle, druid);

					this.secondaryDescription.Enable = true;
					this.SetTextField(this.secondaryDescription, this.GetCaptionDescription(this.secondaryBundle, druid));

					this.secondaryIcon.Enable = true;
					this.secondaryIcon.IconName = this.GetCaptionIcon(this.secondaryBundle, druid);

					this.secondaryAbout.Enable = true;
					this.SetTextField(this.secondaryAbout, this.secondaryBundle[druid].About);
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

			int sel = this.array.SelectedRow;
			int count = this.druidsIndex.Count;
			bool build = (this.module.Mode == DesignerMode.Build);
			bool newCulture = (this.module.GetBundles(this.BundleType).Count < Misc.Cultures.Length);

			this.GetCommandState("NewCulture").Enable = newCulture;
			this.GetCommandState("DeleteCulture").Enable = true;

			this.GetCommandState("Search").Enable = true;
			this.GetCommandState("SearchPrev").Enable = true;
			this.GetCommandState("SearchNext").Enable = true;

			this.GetCommandState("ModificationPrev").Enable = false;
			this.GetCommandState("ModificationNext").Enable = false;
			this.GetCommandState("ModificationAll").Enable = false;
			this.GetCommandState("ModificationClear").Enable = false;

			this.GetCommandState("FontBold").Enable = false;
			this.GetCommandState("FontItalic").Enable = false;
			this.GetCommandState("FontUnderlined").Enable = false;
			this.GetCommandState("Glyphs").Enable = false;

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


		public override string InfoAccessText
		{
			//	Donne le texte d'information sur l'accès en cours.
			get
			{
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

				int sel = this.array.SelectedRow;
				if (sel == -1)
				{
					builder.Append("-");
				}
				else
				{
					ResourceBundleCollection bundles = this.module.GetBundles(Module.BundleType.Strings);
					ResourceBundle bundle = bundles[ResourceLevel.Default];
					ResourceBundle.Field field = bundle[this.druidsIndex[sel]];

					builder.Append(field.Name);
					builder.Append(": ");
					builder.Append((sel+1).ToString());
				}

				builder.Append("/");
				builder.Append(this.druidsIndex.Count.ToString());

				if (this.druidsIndex.Count < this.InfoAccessTotalCount)
				{
					builder.Append(" (");
					builder.Append(this.InfoAccessTotalCount.ToString());
					builder.Append(")");
				}

				return builder.ToString();
			}
		}

		protected override int InfoAccessTotalCount
		{
			get
			{
				return this.primaryBundle.FieldCount;
			}
		}

		
		protected void UpdateCultures()
		{
			//	Met à jour les widgets pour les cultures.
			ResourceBundleCollection bundles = this.module.GetBundles(this.BundleType);

			if (this.secondaryCultures != null)
			{
				foreach (IconButtonMark button in this.secondaryCultures)
				{
					button.Clicked -= new MessageEventHandler(this.HandleSecondaryCultureClicked);
					button.Dispose();
				}
				this.secondaryCultures = null;
			}

			this.primaryBundle = bundles[ResourceLevel.Default];
			this.primaryCulture.Text = string.Format(Res.Strings.Viewers.Strings.Reference, Misc.CultureName(this.primaryBundle.Culture));

			this.secondaryBundle = null;

			if (bundles.Count-1 > 0)
			{
				List<CultureInfo> list = new List<CultureInfo>();

				for (int b=0; b<bundles.Count; b++)
				{
					ResourceBundle bundle = bundles[b];
					if (bundle != this.primaryBundle)
					{
						CultureInfo info = new CultureInfo(bundle.Culture);
						list.Add(info);

						if (this.secondaryBundle == null)
						{
							this.secondaryBundle = bundle;
						}
					}
				}

				list.Sort(new Comparers.CultureName());
				
				this.secondaryCultures = new IconButtonMark[list.Count];
				for (int i=0; i<list.Count; i++)
				{
					this.secondaryCultures[i] = new IconButtonMark(this.secondaryCulture);
					this.secondaryCultures[i].ButtonStyle = ButtonStyle.ActivableIcon;
					this.secondaryCultures[i].SiteMark = SiteMark.OnBottom;
					this.secondaryCultures[i].MarkDimension = 10;
					this.secondaryCultures[i].PreferredHeight = 35;
					this.secondaryCultures[i].Name = list[i].Name;
					this.secondaryCultures[i].Text = list[i].Name;
					this.secondaryCultures[i].AutoFocus = false;
					this.secondaryCultures[i].Margins = new Margins(0, (i==list.Count-1)?1:0, 0, 0);
					this.secondaryCultures[i].Dock = DockStyle.StackFill;
					this.secondaryCultures[i].Clicked += new MessageEventHandler(this.HandleSecondaryCultureClicked);
					ToolTip.Default.SetToolTip(this.secondaryCultures[i], list[i].Tooltip);
				}
			}

			if (this.secondaryBundle != null)
			{
				this.UpdateSelectedCulture(Misc.CultureName(this.secondaryBundle.Culture));
			}

			this.UpdateDruidsIndex("", Searcher.SearchingMode.None);
		}

		protected void UpdateSelectedCulture(string name)
		{
			//	Sélectionne le widget correspondant à la culture secondaire.
			ResourceBundleCollection bundles = this.module.GetBundles(this.BundleType);

			this.secondaryBundle = this.module.GetCulture(name, this.BundleType);
			if (this.secondaryCultures == null)  return;

			for (int i=0; i<this.secondaryCultures.Length; i++)
			{
				if (this.secondaryCultures[i].Name == name)
				{
					this.secondaryCultures[i].ActiveState = ActiveState.Yes;
				}
				else
				{
					this.secondaryCultures[i].ActiveState = ActiveState.No;
				}
			}
		}

		protected override void UpdateDruidsIndex(string filter, Searcher.SearchingMode mode)
		{
			//	Construit l'index en fonction des ressources primaires.
			this.druidsIndex.Clear();

			if ((mode&Searcher.SearchingMode.CaseSensitive) == 0)
			{
				filter = Searcher.RemoveAccent(filter.ToLower());
			}

			Regex regex = null;
			if ((mode&Searcher.SearchingMode.Jocker) != 0)
			{
				regex = RegexFactory.FromSimpleJoker(filter, RegexFactory.Options.None);
			}

			foreach (ResourceBundle.Field field in this.primaryBundle.Fields)
			{
				if (!Captions.HasFixFilter(field.Name))  continue;
				string name = Captions.SubFilter(field.Name);

				if (filter != "")
				{
					if ((mode&Searcher.SearchingMode.Jocker) != 0)
					{
						string text = name;
						if ((mode&Searcher.SearchingMode.CaseSensitive) == 0)
						{
							text = Searcher.RemoveAccent(text.ToLower());
						}
						if (!regex.IsMatch(text))  continue;
					}
					else
					{
						int index = Searcher.IndexOf(name, filter, 0, mode);
						if (index == -1)  continue;
						if ((mode&Searcher.SearchingMode.AtBeginning) != 0 && index != 0)  continue;
					}
				}

				Druid fullDruid = new Druid(field.Druid, this.primaryBundle.Module.Id);
				this.druidsIndex.Add(fullDruid);
			}
		}

		protected override void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			this.array.TotalRows = this.druidsIndex.Count;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.druidsIndex.Count)
				{
					ResourceBundle.Field primaryField = this.primaryBundle[this.druidsIndex[first+i]];

					this.array.SetLineString(0, first+i, Captions.SubFilter(primaryField.Name));
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);
				}
				else
				{
					this.array.SetLineString(0, first+i, "");
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);
				}
			}
		}


		protected void CreateBand(out Widget leftContainer, out Widget rightContainer, double backgroundIntensity)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
			Color cap = adorner.ColorCaption;
			Color color = Color.FromAlphaRgb(backgroundIntensity, 0.5+cap.R*0.5, 0.5+cap.G*0.5, 0.5+cap.B*0.5);

			Separator band = new Separator(this.scrollable.Panel);
			band.Color = color;
			band.Alpha = color.A;
			band.Dock = DockStyle.StackBegin;
			band.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			leftContainer = new Widget(band);
			leftContainer.MinWidth = 100;
			leftContainer.Dock = DockStyle.StackFill;
			this.leftContainers.Add(leftContainer);

			rightContainer = new Widget(band);
			rightContainer.MinWidth = 100;
			rightContainer.Dock = DockStyle.StackFill;
			rightContainer.Margins = new Margins(-1, 0, 0, 0);
			this.rightContainers.Add(rightContainer);
		}


		#region Caption modifiers
		protected string GetCaptionDescription(ResourceBundle bundle, Druid druid)
		{
			//	Donne la description d'un caption.
			Common.Types.Caption caption = this.CaptionDeserialize(bundle, druid);

			if (string.IsNullOrEmpty(caption.Description))
			{
				return "";
			}

			return caption.Description;
		}

		protected void SetCaptionDescription(ResourceBundle bundle, Druid druid, string text)
		{
			//	Modifie la description d'un caption.
			Common.Types.Caption caption = this.CaptionDeserialize(bundle, druid);
			caption.Description = text;
			this.CaptionSerialize(bundle, druid, caption);
		}

		protected List<string> GetCaptionLabels(ResourceBundle bundle, Druid druid)
		{
			//	Retourne la liste des labels d'un caption.
			Common.Types.Caption caption = this.CaptionDeserialize(bundle, druid);

			ICollection<string> collection = caption.Labels;
			string[] strings = new string[collection.Count];
			collection.CopyTo(strings, 0);
			List<string> list = new List<string>();
			list.AddRange(strings);

			return list;
		}

		protected void SetCaptionLabels(ResourceBundle bundle, Druid druid, List<string> labels)
		{
			//	Modifie les labels d'un caption.
			Common.Types.Caption caption = this.CaptionDeserialize(bundle, druid);

			ICollection<string> collection = caption.Labels;
			collection.Clear();
			foreach (string text in labels)
			{
				collection.Add(text);
			}

			this.CaptionSerialize(bundle, druid, caption);
		}

		protected string GetCaptionIcon(ResourceBundle bundle, Druid druid)
		{
			//	Donne l'icône d'un caption.
			Common.Types.Caption caption = this.CaptionDeserialize(bundle, druid);

			if (string.IsNullOrEmpty(caption.Icon))
			{
				return null;
			}

			return caption.Icon;
		}

		protected void SetCaptionIcon(ResourceBundle bundle, Druid druid, string text)
		{
			//	Modifie l'icône d'un caption.
			Common.Types.Caption caption = this.CaptionDeserialize(bundle, druid);
			caption.Icon = text;
			this.CaptionSerialize(bundle, druid, caption);
		}

		protected Common.Types.Caption CaptionDeserialize(ResourceBundle bundle, Druid druid)
		{
			//	Désérialise un caption.
			Common.Types.Caption caption = new Common.Types.Caption();

			string s = bundle[druid].AsString;
			if (!string.IsNullOrEmpty(s))
			{
				caption.DeserializeFromString(s);
			}

			return caption;
		}

		protected void CaptionSerialize(ResourceBundle bundle, Druid druid, Common.Types.Caption caption)
		{
			//	Sérialise un caption.
			bundle[druid].SetStringValue(caption.SerializeToString());
		}
		#endregion


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
			this.UpdateEdit();
			this.UpdateCommands();
		}

		void HandleSecondaryCultureClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton pour changer de culture secondaire a été cliqué.
			IconButtonMark button = sender as IconButtonMark;
			this.UpdateSelectedCulture(button.Name);
			this.UpdateArray();
			this.UpdateEdit();
			this.UpdateCommands();
		}

		void HandleTextChanged(object sender)
		{
			//	Un texte éditable a changé.
			if ( this.ignoreChange )  return;

			AbstractTextField edit = sender as AbstractTextField;
			string text = edit.Text;
			int sel = this.array.SelectedRow;
			Druid druid = this.druidsIndex[sel];

			if (edit == this.labelEdit)
			{
				if (!Misc.IsValidLabel(ref text))
				{
					this.ignoreChange = true;
					edit.Text = Captions.SubFilter(this.primaryBundle[this.druidsIndex[sel]].Name);
					edit.SelectAll();
					this.ignoreChange = false;

					this.module.MainWindow.DialogError(Res.Strings.Error.InvalidLabel);
					return;
				}

				this.ignoreChange = true;
				edit.Text = text;
				edit.SelectAll();
				this.ignoreChange = false;

				if (Captions.SubFilter(this.primaryBundle[this.druidsIndex[sel]].Name) == text)  // label inchangé ?
				{
					return;
				}

				if (this.IsExistingName(Captions.AddFilter(text)))
				{
					this.ignoreChange = true;
					edit.Text = Captions.SubFilter(this.primaryBundle[this.druidsIndex[sel]].Name);
					edit.SelectAll();
					this.ignoreChange = false;

					this.module.MainWindow.DialogError(Res.Strings.Error.NameAlreadyExist);
					return;
				}

				this.module.Modifier.Rename(this.BundleType, druid, Captions.AddFilter(text));
				this.array.SetLineString(0, sel, text);
			}

			if (edit == this.primaryDescription)
			{
				this.SetCaptionDescription(this.primaryBundle, druid, text);
			}

			if (edit == this.secondaryDescription && this.secondaryBundle != null)
			{
				this.module.Modifier.CreateIfNecessary(this.BundleType, this.secondaryBundle, druid);
				this.SetCaptionDescription(this.secondaryBundle, druid, text);
			}

			if (edit == this.primaryAbout)
			{
				this.primaryBundle[druid].SetAbout(text);
			}

			if (edit == this.secondaryAbout && this.secondaryBundle != null)
			{
				this.module.Modifier.CreateIfNecessary(this.BundleType, this.secondaryBundle, druid);
				this.secondaryBundle[druid].SetAbout(text);
			}

			this.module.Modifier.IsDirty = true;
		}

		void HandleStringCollectionChanged(object sender)
		{
			//	Une collection de textes a changé.
			if ( this.ignoreChange )  return;

			MyWidgets.StringCollection sc = sender as MyWidgets.StringCollection;
			int sel = this.array.SelectedRow;
			Druid druid = this.druidsIndex[sel];

			if (sc == this.primaryLabels)
			{
				this.SetCaptionLabels(this.primaryBundle, druid, sc.Collection);
			}

			if (sc == this.secondaryLabels)
			{
				this.module.Modifier.CreateIfNecessary(this.BundleType, this.secondaryBundle, druid);
				this.SetCaptionLabels(this.secondaryBundle, druid, sc.Collection);
			}

			this.module.Modifier.IsDirty = true;
		}

		void HandleCursorChanged(object sender)
		{
			//	Le curseur a été déplacé dans un texte éditable.
			if ( this.ignoreChange )  return;

			this.lastActionIsReplace = false;
		}


		protected IconButtonMark				primaryCulture;
		protected Widget						secondaryCulture;
		protected IconButtonMark[]				secondaryCultures;
		protected ResourceBundle				primaryBundle;
		protected ResourceBundle				secondaryBundle;
		protected TextFieldEx					labelEdit;
		protected Scrollable					scrollable;
		protected List<Widget>					leftContainers;
		protected List<Widget>					rightContainers;
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
