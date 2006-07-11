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
			this.primaryCulture.Padding = new Margins(0, 1, 0, 0);
			this.primaryCulture.Dock = DockStyle.StackFill;

			this.secondaryCulture = new Widget(sup);
			this.secondaryCulture.Padding = new Margins(1, 0, 0, 0);
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

			this.scrollable.Panel.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			Widget leftContainer = new Widget(this.scrollable.Panel);
			leftContainer.MinWidth = 100;
			leftContainer.Dock = DockStyle.StackFill;

			Widget rightContainer = new Widget(this.scrollable.Panel);
			rightContainer.MinWidth = 100;
			rightContainer.Dock = DockStyle.StackFill;

			this.primaryDescription = new TextFieldMulti(leftContainer);
			this.primaryDescription.PreferredHeight = 100;
			this.primaryDescription.Margins = new Margins(10, 1, 10, 10);
			this.primaryDescription.Dock = DockStyle.StackBegin;

			this.secondaryDescription = new TextFieldMulti(rightContainer);
			this.secondaryDescription.PreferredHeight = 100;
			this.secondaryDescription.Margins = new Margins(1, 10, 10, 10);
			this.secondaryDescription.Dock = DockStyle.StackBegin;




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
		}

		public override void DoCount(string search, Searcher.SearchingMode mode)
		{
			//	Effectue un comptage.
		}

		public override void DoReplace(string search, string replace, Searcher.SearchingMode mode)
		{
			//	Effectue un remplacement.
		}

		public override void DoReplaceAll(string search, string replace, Searcher.SearchingMode mode)
		{
			//	Effectue un 'remplacer tout'.
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
			this.module.Modifier.Delete(Module.BundleType.Captions, druid);

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
			Druid newDruid = this.module.Modifier.Duplicate(Module.BundleType.Captions, druid, newName, duplicate);

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
			if ( !this.module.Modifier.Move(Module.BundleType.Captions, druid, direction) )  return;
		
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
			ResourceBundle bundle = this.module.NewCulture(name, Module.BundleType.Captions);

			this.UpdateCultures();
			this.Update();
			this.module.Modifier.IsDirty = true;
		}

		public override void DoDeleteCulture()
		{
			//	Supprime la culture courante.
#if false
			string question = string.Format(Res.Strings.Dialog.DeleteCulture.Question, Misc.CultureName(this.secondaryBundle.Culture));
			Common.Dialogs.DialogResult result = this.module.MainWindow.DialogQuestion(question);
			if ( result != Epsitec.Common.Dialogs.DialogResult.Yes )  return;

			this.module.DeleteCulture(this.secondaryBundle, Module.BundleType.Captions);

			this.UpdateCultures();
			if (this.secondaryBundle != null)
			{
				this.UpdateSelectedCulture(Misc.CultureName(this.secondaryBundle.Culture));
			}
			this.UpdateArray();
			this.UpdateCommands();
			this.module.Modifier.IsDirty = true;
#endif
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
			this.UpdateCaption();
			this.UpdateCommands();
		}

		protected void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			int sel = this.array.SelectedRow;
			int column = this.array.SelectedColumn;

			if (sel >= this.druidsIndex.Count)
			{
				sel = -1;
				column = -1;
			}

			if ( sel == -1 )
			{
				this.labelEdit.Enable = false;
				this.labelEdit.Text = "";
			}
			else
			{
				this.labelEdit.Enable = true;

				Druid druid = this.druidsIndex[sel];
				this.SetTextField(this.labelEdit, Captions.SubFilter(this.primaryBundle[druid].Name));

				this.labelEdit.Focus();
				this.labelEdit.SelectAll();
			}

			this.ignoreChange = iic;

			this.UpdateCommands();
		}

		protected void UpdateCaption()
		{
			int sel = this.array.SelectedRow;

			if (sel >= this.druidsIndex.Count)
			{
				sel = -1;
			}

			this.secondaryDescription.Visibility = (this.secondaryBundle != null);

			if (sel == -1)
			{
				this.primaryDescription.Enable = false;
				this.primaryDescription.Text = "";

				this.secondaryDescription.Enable = false;
				this.secondaryDescription.Text = "";
			}
			else
			{
				Druid druid = this.druidsIndex[sel];

#if true
				ResourceBundle.Field primaryField = this.primaryBundle[druid];
				string primaryText = primaryField.AsString;
				primaryText = TextLayout.ConvertToTaggedText(primaryText);

				string secondaryText = null;
				if (this.secondaryBundle != null)
				{
					ResourceBundle.Field secondaryField = this.secondaryBundle[druid];
					secondaryText = secondaryField.AsString;
					secondaryText = TextLayout.ConvertToTaggedText(secondaryText);
				}
#else
				Command cmd = Widgets.Command.Get(druid);
				string primaryText = cmd.Description;
				string secondaryText = "???";
#endif

				this.primaryDescription.Enable = true;
				this.primaryDescription.Text = primaryText;

				if (secondaryText == null)
				{
					this.secondaryDescription.Enable = false;
					this.secondaryDescription.Text = "";
				}
				else
				{
					this.secondaryDescription.Enable = true;
					this.secondaryDescription.Text = secondaryText;
				}
			}
		}

		public override void UpdateCommands()
		{
			//	Met à jour les commandes en fonction de la ressource sélectionnée.
			base.UpdateCommands();

			int sel = this.array.SelectedRow;
			int count = this.druidsIndex.Count;
			bool build = (this.module.Mode == DesignerMode.Build);
			bool newCulture = (this.module.GetBundles(Module.BundleType.Captions).Count < Misc.Cultures.Length);

			this.GetCommandState("NewCulture").Enable = newCulture;
			this.GetCommandState("DeleteCulture").Enable = true;

			this.GetCommandState("Search").Enable = false;
			this.GetCommandState("SearchPrev").Enable = false;
			this.GetCommandState("SearchNext").Enable = false;

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
			ResourceBundleCollection bundles = this.module.GetBundles(Module.BundleType.Captions);

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
			ResourceBundleCollection bundles = this.module.GetBundles(Module.BundleType.Captions);

			this.secondaryBundle = this.module.GetCulture(name, Module.BundleType.Captions);
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

		protected static string SubFilter(string name)
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
			this.UpdateCaption();
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

				this.module.Modifier.Rename(Module.BundleType.Captions, druid, Captions.AddFilter(text));
				this.array.SetLineString(0, sel, text);
			}

			this.module.Modifier.IsDirty = true;
		}


		protected IconButtonMark			primaryCulture;
		protected Widget					secondaryCulture;
		protected IconButtonMark[]			secondaryCultures;
		protected ResourceBundle			primaryBundle;
		protected ResourceBundle			secondaryBundle;
		protected TextFieldEx				labelEdit;
		protected Scrollable				scrollable;
		protected TextFieldMulti			primaryDescription;
		protected TextFieldMulti			secondaryDescription;
	}
}
