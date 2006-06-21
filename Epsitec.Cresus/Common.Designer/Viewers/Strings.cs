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
	public class Strings : Abstract
	{
		public Strings(Module module, PanelsContext context) : base(module, context)
		{
			int tabIndex = 0;

			this.primaryCulture = new IconButtonMark(this);
			this.primaryCulture.ButtonStyle = ButtonStyle.ActivableIcon;
			this.primaryCulture.SiteMark = SiteMark.OnBottom;
			this.primaryCulture.MarkDimension = 5;
			this.primaryCulture.ActiveState = ActiveState.Yes;
			this.primaryCulture.AutoFocus = false;
			this.primaryCulture.TabIndex = tabIndex++;
			this.primaryCulture.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.array = new MyWidgets.StringArray(this);
			this.array.Columns = 3;
			this.array.SetColumnsRelativeWidth(0, 0.30);
			this.array.SetColumnsRelativeWidth(1, 0.35);
			this.array.SetColumnsRelativeWidth(2, 0.35);
			this.array.SetDynamicsToolTips(0, true);
			this.array.SetDynamicsToolTips(1, false);
			this.array.SetDynamicsToolTips(2, false);
			this.array.ColumnsWidthChanged += new EventHandler(this.HandleArrayColumnsWidthChanged);
			this.array.CellCountChanged += new EventHandler (this.HandleArrayCellCountChanged);
			this.array.CellsContentChanged += new EventHandler(this.HandleArrayCellsContentChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
			this.array.TabIndex = tabIndex++;
			this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.labelStatic = new StaticText(this);
			this.labelStatic.ContentAlignment = ContentAlignment.MiddleRight;
			this.labelStatic.Text = Res.Strings.Viewers.Strings.Edit;
			this.labelStatic.Visibility = (this.module.Mode != DesignerMode.Build);

			this.labelEdit = new TextFieldEx(this);
			this.labelEdit.Name = "LabelEdit";
			this.labelEdit.EditionAccepted += new EventHandler(this.HandleTextChanged);
			this.labelEdit.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.labelEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.labelEdit.TabIndex = tabIndex++;
			this.labelEdit.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.labelEdit.Visibility = (this.module.Mode == DesignerMode.Build);

			this.primaryEdit = new TextFieldMulti(this);
			this.primaryEdit.Name = "PrimaryEdit";
			this.primaryEdit.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryEdit.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryEdit.TabIndex = tabIndex++;
			this.primaryEdit.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.secondaryEdit = new TextFieldMulti(this);
			this.secondaryEdit.Name = "SecondaryEdit";
			this.secondaryEdit.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryEdit.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryEdit.TabIndex = tabIndex++;
			this.secondaryEdit.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.labelAbout = new StaticText(this);
			this.labelAbout.ContentAlignment = ContentAlignment.MiddleRight;
			this.labelAbout.Text = Res.Strings.Viewers.Strings.About;

			this.primaryAbout = new TextFieldMulti(this);
			this.primaryAbout.Name = "PrimaryAbout";
			this.primaryAbout.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryAbout.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryAbout.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryAbout.TabIndex = tabIndex++;
			this.primaryAbout.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.secondaryAbout = new TextFieldMulti(this);
			this.secondaryAbout.Name = "SecondaryAbout";
			this.secondaryAbout.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryAbout.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryAbout.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryAbout.TabIndex = tabIndex++;
			this.secondaryAbout.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.UpdateCultures();
			this.UpdateEdit();
			this.UpdateModifiers();
			this.UpdateCommands();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.array.ColumnsWidthChanged -= new EventHandler(this.HandleArrayColumnsWidthChanged);
				this.array.CellCountChanged -= new EventHandler (this.HandleArrayCellCountChanged);
				this.array.CellsContentChanged -= new EventHandler(this.HandleArrayCellsContentChanged);
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);

				this.labelEdit.EditionAccepted -= new EventHandler(this.HandleTextChanged);
				this.labelEdit.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.labelEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.primaryEdit.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.primaryEdit.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.primaryEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryEdit.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.secondaryEdit.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.secondaryEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.primaryAbout.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.primaryAbout.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.primaryAbout.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryAbout.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.secondaryAbout.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.secondaryAbout.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			}

			base.Dispose(disposing);
		}


		public override void DoSearch(string search, Searcher.SearchingMode mode)
		{
			//	Effectue une recherche.
			Searcher searcher = new Searcher(this.druidsIndex, this.primaryBundle, this.secondaryBundle);
			searcher.FixStarting(mode, this.array.SelectedRow, this.currentTextField, false);

			if (searcher.Search(search))
			{
				this.lastActionIsReplace = false;

				this.array.SelectedRow = searcher.Row;
				this.array.ShowSelectedRow();

				AbstractTextField edit = null;
				if (searcher.Field == 0)  edit = this.labelEdit;
				if (searcher.Field == 1)  edit = this.primaryEdit;
				if (searcher.Field == 2)  edit = this.secondaryEdit;
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
			Searcher searcher = new Searcher(this.druidsIndex, this.primaryBundle, this.secondaryBundle);
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

			Searcher searcher = new Searcher(this.druidsIndex, this.primaryBundle, this.secondaryBundle);
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

					this.module.Modifier.Rename(druid, text);
					this.array.SetLineString(0, searcher.Row, text);
				}

				if (searcher.Field == 1)
				{
					text = this.primaryBundle[druid].AsString;
					text = text.Remove(searcher.Index, searcher.Length);
					text = text.Insert(searcher.Index, replace);
					this.primaryBundle[druid].SetStringValue(text);

					this.UpdateArrayField(1, searcher.Row, this.primaryBundle[druid], this.secondaryBundle[druid]);
				}

				if (searcher.Field == 2)
				{
					text = this.secondaryBundle[druid].AsString;
					text = text.Remove(searcher.Index, searcher.Length);
					text = text.Insert(searcher.Index, replace);
					this.secondaryBundle[druid].SetStringValue(text);

					this.UpdateArrayField(2, searcher.Row, this.secondaryBundle[druid], this.primaryBundle[druid]);
				}

				if (searcher.Field == 3)
				{
					text = this.primaryBundle[druid].About;
					text = text.Remove(searcher.Index, searcher.Length);
					text = text.Insert(searcher.Index, replace);
					this.primaryBundle[druid].SetAbout(text);
				}

				if (searcher.Field == 4)
				{
					text = this.secondaryBundle[druid].About;
					text = text.Remove(searcher.Index, searcher.Length);
					text = text.Insert(searcher.Index, replace);
					this.secondaryBundle[druid].SetAbout(text);
				}

				AbstractTextField edit = null;
				if (searcher.Field == 0)  edit = this.labelEdit;
				if (searcher.Field == 1)  edit = this.primaryEdit;
				if (searcher.Field == 2)  edit = this.secondaryEdit;
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

			Searcher searcher = new Searcher(this.druidsIndex, this.primaryBundle, this.secondaryBundle);
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

					this.module.Modifier.Rename(druid, text);
				}

				if (searcher.Field == 1)
				{
					text = this.primaryBundle[druid].AsString;
					text = text.Remove(searcher.Index, searcher.Length);
					text = text.Insert(searcher.Index, replace);
					this.primaryBundle[druid].SetStringValue(text);
				}

				if (searcher.Field == 2)
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

				if (searcher.Field == 4)
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
			int sel = this.array.SelectedRow;

			if (name == "ModificationAll")
			{
				if (sel == -1)  return;
				Druid druid = this.druidsIndex[sel];
				ResourceBundle.Field field1 = this.primaryBundle[druid];
				if (field1.IsEmpty)  return;
				field1.SetModificationId(field1.ModificationId+1);

				this.UpdateArray();
				this.UpdateModifiers();
				this.UpdateCommands();
				this.module.Modifier.IsDirty = true;
			}
			else if (name == "ModificationClear")
			{
				if (sel == -1)  return;
				Druid druid = this.druidsIndex[sel];
				ResourceBundle.Field field1 = this.primaryBundle[druid];
				ResourceBundle.Field field2 = this.secondaryBundle[druid];
				if (field1.IsEmpty || field2.IsEmpty)  return;
				field2.SetModificationId(field1.ModificationId);

				this.UpdateArray();
				this.UpdateModifiers();
				this.UpdateCommands();
				this.module.Modifier.IsDirty = true;
			}
			else
			{
				if (sel == -1)
				{
					sel = (name == "ModificationPrev") ? 0 : this.druidsIndex.Count-1;
				}

				int column = -1;
				int dir = (name == "ModificationPrev") ? -1 : 1;

				for (int i=0; i<this.druidsIndex.Count; i++)
				{
					sel += dir;

					if (sel >= this.druidsIndex.Count)
					{
						sel = 0;
					}

					if (sel < 0)
					{
						sel = this.druidsIndex.Count-1;
					}

					Druid druid = this.druidsIndex[sel];
					ResourceBundle.Field field1 = this.primaryBundle[druid];
					ResourceBundle.Field field2 = this.secondaryBundle[druid];
					bool state1 = field1.IsEmpty || string.IsNullOrEmpty(field1.AsString);
					bool state2 = field2.IsEmpty || string.IsNullOrEmpty(field2.AsString);

					if (state1 || state2)
					{
						column = 2;
						break;
					}

					if (!state1 && !state2 && field1.ModificationId > field2.ModificationId)
					{
						column = 2;
						break;
					}
				}

				this.array.SelectedRow = sel;
				this.array.ShowSelectedRow();

				AbstractTextField edit = null;
				if (column == 1)  edit = this.primaryEdit;
				if (column == 2)  edit = this.secondaryEdit;
				if (edit != null)
				{
					this.Window.MakeActive();
					edit.Focus();
					edit.SelectAll();
				}
			}
		}

		public override void DoDelete()
		{
			//	Supprime la ressource sélectionnée.
			int sel = this.array.SelectedRow;
			if ( sel == -1 )  return;

			Druid druid = this.druidsIndex[sel];
			this.module.Modifier.Delete(druid);

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
			Druid newDruid = this.module.Modifier.Duplicate(druid, newName, duplicate);

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
			if ( !this.module.Modifier.Move(druid, direction) )  return;
		
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
			ResourceBundle bundle = this.module.NewCulture(name);

			this.UpdateCultures();
			this.UpdateArray();
			this.UpdateModifiers();
			this.UpdateClientGeometry();
			this.UpdateCommands();
			this.module.Modifier.IsDirty = true;
		}

		public override void DoDeleteCulture()
		{
			//	Supprime la culture courante.
			string question = string.Format(Res.Strings.Dialog.DeleteCulture.Question, Misc.CultureName(this.secondaryBundle.Culture));
			Common.Dialogs.DialogResult result = this.module.MainWindow.DialogQuestion(question);
			if ( result != Epsitec.Common.Dialogs.DialogResult.Yes )  return;

			this.module.DeleteCulture(this.secondaryBundle);

			this.UpdateCultures();
			if (this.secondaryBundle != null)
			{
				this.UpdateSelectedCulture(Misc.CultureName(this.secondaryBundle.Culture));
			}
			this.UpdateArray();
			this.UpdateModifiers();
			this.UpdateClientGeometry();
			this.UpdateCommands();
			this.module.Modifier.IsDirty = true;
		}

		public override void DoClipboard(string name)
		{
			//	Effectue une action avec le bloc-notes.
			if ( this.currentTextField == null )  return;

			if ( name == "Cut" )
			{
				this.currentTextField.ProcessCut();
			}

			if ( name == "Copy" )
			{
				this.currentTextField.ProcessCopy();
			}

			if ( name == "Paste" )
			{
				this.currentTextField.ProcessPaste();
			}
		}

		public override void DoFont(string name)
		{
			//	Effectue une modification de typographie.
			if ( this.currentTextField == null )  return;

			if ( name == "FontBold" )
			{
				this.currentTextField.TextNavigator.SelectionBold = !this.currentTextField.TextNavigator.SelectionBold;
			}

			if ( name == "FontItalic" )
			{
				this.currentTextField.TextNavigator.SelectionItalic = !this.currentTextField.TextNavigator.SelectionItalic;
			}

			if ( name == "FontUnderlined" )
			{
				this.currentTextField.TextNavigator.SelectionUnderlined = !this.currentTextField.TextNavigator.SelectionUnderlined;
			}

			this.HandleTextChanged(this.currentTextField);
		}


		protected void UpdateCultures()
		{
			//	Met à jour les widgets pour les cultures.
			ResourceBundleCollection bundles = this.module.Bundles;

			if (this.secondaryCultures != null)
			{
				foreach (IconButtonMark button in this.secondaryCultures)
				{
					button.Clicked -= new MessageEventHandler(this.HandleSecondaryCultureClicked);
					button.Dispose();
				}
				this.secondaryCultures = null;
			}

			if (this.secondaryModifiers != null)
			{
				foreach (ColorSample sample in this.secondaryModifiers)
				{
					sample.Dispose();
				}
				this.secondaryModifiers = null;
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
					this.secondaryCultures[i] = new IconButtonMark(this);
					this.secondaryCultures[i].ButtonStyle = ButtonStyle.ActivableIcon;
					this.secondaryCultures[i].SiteMark = SiteMark.OnBottom;
					this.secondaryCultures[i].MarkDimension = 5;
					this.secondaryCultures[i].Name = list[i].Name;
					this.secondaryCultures[i].Text = list[i].Name;
					this.secondaryCultures[i].AutoFocus = false;
					this.secondaryCultures[i].Clicked += new MessageEventHandler(this.HandleSecondaryCultureClicked);
					ToolTip.Default.SetToolTip(this.secondaryCultures[i], list[i].Tooltip);
				}

				this.secondaryModifiers = new ColorSample[list.Count];
				for (int i=0; i<list.Count; i++)
				{
					this.secondaryModifiers[i] = new ColorSample(this);
					this.secondaryModifiers[i].Name = list[i].Name;
					this.secondaryModifiers[i].AutoFocus = false;
					this.secondaryModifiers[i].Passive = true;
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
			ResourceBundleCollection bundles = this.module.Bundles;

			this.secondaryBundle = this.module.GetCulture(name);
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
				if (filter != "")
				{
					if ((mode&Searcher.SearchingMode.Jocker) != 0)
					{
						string text = field.Name;
						if ((mode&Searcher.SearchingMode.CaseSensitive) == 0)
						{
							text = Searcher.RemoveAccent(text.ToLower());
						}
						if (!regex.IsMatch(text))  continue;
					}
					else
					{
						int index = Searcher.IndexOf(field.Name, filter, 0, mode);
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
					ResourceBundle.Field primaryField   = this.primaryBundle[this.druidsIndex[first+i]];
					ResourceBundle.Field secondaryField = this.secondaryBundle[this.druidsIndex[first+i]];

					this.array.SetLineString(0, first+i, primaryField.Name);
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);
					this.UpdateArrayField(1, first+i, primaryField, secondaryField);
					this.UpdateArrayField(2, first+i, secondaryField, primaryField);
				}
				else
				{
					this.array.SetLineString(0, first+i, "");
					this.array.SetLineString(1, first+i, "");
					this.array.SetLineString(2, first+i, "");
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Disabled);
					this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Disabled);
				}
			}
		}

		protected void UpdateArrayField(int column, int row, ResourceBundle.Field field, ResourceBundle.Field secondaryField)
		{
			if (field != null)
			{
				string text = field.AsString;
				if (text != null && text != "")
				{
					this.array.SetLineString(column, row, text);

					int primaryId = field.ModificationId;
					int secondaryId = primaryId;
					if (secondaryField != null)
					{
						secondaryId = secondaryField.ModificationId;
					}

					MyWidgets.StringList.CellState state = MyWidgets.StringList.CellState.Normal;
					if (primaryId < secondaryId)  // éventuellement pas à jour (fond jaune) ?
					{
						state = MyWidgets.StringList.CellState.Modified;
					}

					if (this.array.GetLineState(column, row) != state)
					{
						this.array.SetLineState(column, row, state);
						this.UpdateModifiers();
					}

					return;
				}
			}

			this.array.SetLineString(column, row, "");
			this.array.SetLineState(column, row, MyWidgets.StringList.CellState.Warning);
		}

		protected void UpdateModifiers()
		{
			//	Met à jour les indicateurs de modifications.
			int sel = this.array.SelectedRow;
			Druid druid = Druid.Empty;
			if (sel != -1)
			{
				druid = this.druidsIndex[sel];
			}

			ResourceBundle defaultBundle = this.module.Bundles[ResourceLevel.Default];

			for (int i=0; i<this.secondaryModifiers.Length; i++)
			{
				ColorSample sample = this.secondaryModifiers[i];

				bool modified = false;

				ResourceBundle bundle = this.module.GetCulture(sample.Name);
				if (bundle != null && !druid.IsEmpty)
				{
					modified = (defaultBundle[druid].ModificationId > bundle[druid].ModificationId);
				}

				RichColor color = RichColor.FromBrightness(1);
				if (modified)
				{
					color = RichColor.FromRgb(0.91, 0.81, 0.41);  // jaune
				}
				sample.Color = color;
			}
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
				this.primaryEdit.Enable = false;
				this.secondaryEdit.Enable = false;
				this.primaryAbout.Enable = false;
				this.secondaryAbout.Enable = false;

				this.labelEdit.Text = "";
				this.primaryEdit.Text = "";
				this.secondaryEdit.Text = "";
				this.primaryAbout.Text = "";
				this.secondaryAbout.Text = "";
			}
			else
			{
				this.labelEdit.Enable = true;
				this.primaryEdit.Enable = true;
				this.secondaryEdit.Enable = true;
				this.primaryAbout.Enable = true;
				this.secondaryAbout.Enable = true;

				Druid druid = this.druidsIndex[sel];

				this.SetTextField(this.labelEdit, this.primaryBundle[druid].Name);

				this.SetTextField(this.primaryEdit, this.primaryBundle[druid].AsString);
				this.SetTextField(this.secondaryEdit, this.secondaryBundle[druid].AsString);

				this.SetTextField(this.primaryAbout, this.primaryBundle[druid].About);
				this.SetTextField(this.secondaryAbout, this.secondaryBundle[druid].About);

				AbstractTextField edit = null;
				if (column == 0)  edit = this.labelEdit;
				if (column == 1)  edit = this.primaryEdit;
				if (column == 2)  edit = this.secondaryEdit;
				if (edit != null && edit.Visibility)
				{
					edit.Focus();
					edit.SelectAll();
				}
				else
				{
					this.labelEdit.Cursor = 100000;
					this.primaryEdit.Cursor = 100000;
					this.secondaryEdit.Cursor = 100000;
				}
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

			bool all = false;
			bool modified = false;
			if (sel != -1)
			{
				Druid druid = this.druidsIndex[sel];
				all = this.module.Modifier.IsModificationAll(druid);
				ResourceBundle.Field field1 = this.primaryBundle[druid];
				ResourceBundle.Field field2 = this.secondaryBundle[druid];
				if (!field1.IsEmpty && !field2.IsEmpty)
				{
					modified = (field1.ModificationId > field2.ModificationId);
				}
			}

			bool search = this.module.MainWindow.DialogSearch.IsActionsEnabled;
			
			bool newCulture = (this.module.Bundles.Count < Misc.Cultures.Length);

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
					ResourceBundleCollection bundles = this.module.Bundles;
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

		
		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.primaryCulture == null )  return;

			Rectangle box = this.Client.Bounds;
			box.Deflate(10);
			box.Top -= 4;
			Rectangle rect, r;

			int lines = System.Math.Max((int)box.Height/50, 4);
			int editLines = lines*2/3;
			int aboutLines = lines-editLines;
			double cultureHeight = 20;
			double editHeight = editLines*13+8;
			double aboutHeight = aboutLines*13+8;

			//	Il faut obligatoirement s'occuper d'abord de this.array, puisque les autres
			//	widgets dépendent des largeurs relatives de ses colonnes.
			rect = box;
			rect.Top -= cultureHeight+5;
			rect.Bottom += editHeight+5+aboutHeight+5;
			this.array.SetManualBounds(rect);

			rect = box;
			rect.Bottom = rect.Top-cultureHeight-5;
			rect.Left += this.array.GetColumnsAbsoluteWidth(0);
			rect.Width = this.array.GetColumnsAbsoluteWidth(1)-1;
			this.primaryCulture.SetManualBounds(rect);

			if (this.secondaryCultures != null)
			{
				rect.Left = rect.Right+2;
				rect.Width = this.array.GetColumnsAbsoluteWidth(2);
				double w = System.Math.Floor(rect.Width/this.secondaryCultures.Length);
				for (int i=0; i<this.secondaryCultures.Length; i++)
				{
					r = rect;
					r.Left += w*i;
					r.Width = w;
					if (i == this.secondaryCultures.Length-1)
					{
						r.Right = rect.Right;
					}
					this.secondaryCultures[i].SetManualBounds(r);

					r.Bottom = r.Top-1;
					r.Height = 6;
					r.Width ++;
					this.secondaryModifiers[i].SetManualBounds(r);
				}
			}

			rect = box;
			rect.Top = rect.Bottom+editHeight+aboutHeight+5;
			rect.Bottom = rect.Top-editHeight;
			rect.Width = this.array.GetColumnsAbsoluteWidth(0)-5;
			this.labelStatic.SetManualBounds(rect);
			rect.Width += 5+1;
			r = rect;
			r.Bottom = r.Top-21;
			this.labelEdit.SetManualBounds(r);
			rect.Left += this.array.GetColumnsAbsoluteWidth(0);
			rect.Width = this.array.GetColumnsAbsoluteWidth(1)+1;
			this.primaryEdit.SetManualBounds(rect);
			rect.Left = rect.Right-1;
			rect.Width = this.array.GetColumnsAbsoluteWidth(2);
			this.secondaryEdit.SetManualBounds(rect);

			rect = box;
			rect.Top = rect.Bottom+aboutHeight;
			rect.Bottom = rect.Top-aboutHeight;
			rect.Width = this.array.GetColumnsAbsoluteWidth(0)-5;
			this.labelAbout.SetManualBounds(rect);
			rect.Left += this.array.GetColumnsAbsoluteWidth(0);
			rect.Width = this.array.GetColumnsAbsoluteWidth(1)+1;
			this.primaryAbout.SetManualBounds(rect);
			rect.Left = rect.Right-1;
			rect.Width = this.array.GetColumnsAbsoluteWidth(2);
			this.secondaryAbout.SetManualBounds(rect);
		}


		protected void SetTextField(AbstractTextField field, string text)
		{
			if (text == null)
			{
				field.Text = "";
			}
			else
			{
				field.Text = text;
			}
		}

		protected bool IsExistingName(string baseName)
		{
			//	Indique si un nom existe.
			ResourceBundleCollection bundles = this.module.Bundles;
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];

			ResourceBundle.Field field = defaultBundle[baseName];
			return (field != null && field.Name != null);
		}

		protected string GetDuplicateName(string baseName)
		{
			//	Retourne le nom à utiliser lorsqu'un nom existant est dupliqué.
			ResourceBundleCollection bundles = this.module.Bundles;

			int numberLength = 0;
			while (baseName.Length > 0)
			{
				char last = baseName[baseName.Length-1-numberLength];
				if (last >= '0' && last <= '9')
				{
					numberLength ++;
				}
				else
				{
					break;
				}
			}

			int nextNumber = 2;
			if (numberLength > 0)
			{
				nextNumber = int.Parse(baseName.Substring(baseName.Length-numberLength))+1;
				baseName = baseName.Substring(0, baseName.Length-numberLength);
			}

			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];
			string newName = baseName;
			for (int i=nextNumber; i<nextNumber+100; i++)
			{
				newName = string.Concat(baseName, i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				ResourceBundle.Field field = defaultBundle[newName];
				if (field.IsEmpty)  break;
			}

			return newName;
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

		void HandleArrayColumnsWidthChanged(object sender)
		{
			//	La largeur des colonnes a changé.
			this.UpdateClientGeometry();
		}

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
			this.UpdateModifiers();
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
					edit.Text = this.primaryBundle[this.druidsIndex[sel]].Name;
					edit.SelectAll();
					this.ignoreChange = false;

					this.module.MainWindow.DialogError(Res.Strings.Error.InvalidLabel);
					return;
				}

				this.ignoreChange = true;
				edit.Text = text;
				edit.SelectAll();
				this.ignoreChange = false;

				if (this.primaryBundle[this.druidsIndex[sel]].Name == text)  // label inchangé ?
				{
					return;
				}

				if (this.IsExistingName(text))
				{
					this.ignoreChange = true;
					edit.Text = this.primaryBundle[this.druidsIndex[sel]].Name;
					edit.SelectAll();
					this.ignoreChange = false;

					this.module.MainWindow.DialogError(Res.Strings.Error.NameAlreadyExist);
					return;
				}

				this.module.Modifier.Rename(druid, text);
				this.array.SetLineString(0, sel, text);
			}

			if (edit == this.primaryEdit)
			{
				this.primaryBundle[druid].SetStringValue(text);
				this.UpdateArrayField(1, sel, this.primaryBundle[druid], this.secondaryBundle[druid]);
			}

			if (edit == this.secondaryEdit)
			{
				this.module.Modifier.CreateIfNecessary(this.secondaryBundle, druid);
				this.secondaryBundle[druid].SetStringValue(text);
				this.UpdateArrayField(2, sel, this.secondaryBundle[druid], this.primaryBundle[druid]);
			}

			if (edit == this.primaryAbout)
			{
				this.primaryBundle[druid].SetAbout(text);
			}

			if (edit == this.secondaryAbout)
			{
				this.module.Modifier.CreateIfNecessary(this.secondaryBundle, druid);
				this.secondaryBundle[druid].SetAbout(text);
			}

			this.module.Modifier.IsDirty = true;
		}

		void HandleCursorChanged(object sender)
		{
			//	Le curseur a été déplacé dans un texte éditable.
			if ( this.ignoreChange )  return;

			this.lastActionIsReplace = false;
		}


		#region CultureInfo
		public class CultureInfo
		{
			public CultureInfo(System.Globalization.CultureInfo culture)
			{
				this.name = Misc.CultureName(culture);
				this.tooltip = Misc.CultureLongName(culture);
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public string Tooltip
			{
				get
				{
					return this.tooltip;
				}
			}

			protected string			name;
			protected string			tooltip;
		}
		#endregion


		protected bool						lastActionIsReplace = false;

		protected IconButtonMark			primaryCulture;
		protected IconButtonMark[]			secondaryCultures;
		protected ColorSample[]				secondaryModifiers;
		protected ResourceBundle			primaryBundle;
		protected ResourceBundle			secondaryBundle;
		protected StaticText				labelStatic;
		protected TextFieldEx				labelEdit;
		protected TextFieldMulti			primaryEdit;
		protected TextFieldMulti			secondaryEdit;
		protected StaticText				labelAbout;
		protected TextFieldMulti			primaryAbout;
		protected TextFieldMulti			secondaryAbout;
	}
}
