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
	public abstract class Abstract : Widget
	{
		public Abstract(Module module, PanelsContext context, ResourceAccess access)
		{
			this.module = module;
			this.context = context;
			this.access = access;
			this.access.ResourceType = this.ResourceType;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}


		public abstract ResourceAccess.Type ResourceType
		{
			get;
		}

		public static Abstract Create(ResourceAccess.Type type, Module module, PanelsContext context, ResourceAccess access)
		{
			//	Crée un Viewer d'un type donné.
			if (type == ResourceAccess.Type.Strings)  return new Strings(module, context, access);
			if (type == ResourceAccess.Type.Captions)  return new Captions(module, context, access);
			if (type == ResourceAccess.Type.Commands)  return new Commands(module, context, access);
			if (type == ResourceAccess.Type.Types)  return new Types(module, context, access);
			if (type == ResourceAccess.Type.Values)  return new Values(module, context, access);
			if (type == ResourceAccess.Type.Panels)  return new Panels(module, context, access);
			if (type == ResourceAccess.Type.Scripts)  return new Scripts(module, context, access);
			return null;
		}


		public PanelsContext PanelsContext
		{
			get
			{
				return this.context;
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
				this.array.SelectedRow = this.access.AccessIndex;
				this.array.ShowSelectedRow();

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
				this.module.MainWindow.DialogError(Res.Strings.Dialog.Search.Message.Error);
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
				this.module.MainWindow.DialogError(Res.Strings.Dialog.Search.Message.Error);
			}
			else
			{
				string message = string.Format(Res.Strings.Dialog.Search.Message.Count, count.ToString());
				this.module.MainWindow.DialogMessage(message);
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

				this.access.AccessIndex = searcher.Row;
				this.array.SelectedRow = this.access.AccessIndex;
				this.array.ShowSelectedRow();

				string text = this.ReplaceDo(searcher, replace);
				if (text == null)
				{
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
					this.UpdateArray();
				}
			}
			else
			{
				this.module.MainWindow.DialogError(Res.Strings.Dialog.Search.Message.Error);
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

			int count = 0;
			bool fromBeginning = true;
			while (searcher.Replace(search, fromBeginning))
			{
				fromBeginning = false;
				count ++;

				string text = this.ReplaceDo(searcher, replace);
				if (text == null)
				{
					return;
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

				string text = string.Format(Res.Strings.Dialog.Search.Message.Replace, count.ToString());
				this.module.MainWindow.DialogMessage(text);
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

			searcher.FixStarting(mode, filter, this.array.SelectedRow, field, subfield, this.currentTextField, this.secondaryCulture, false);
			return searcher;
		}

		protected string ReplaceDo(Searcher searcher, string replace)
		{
			//	Effectue le remplacement.
			//	Retourne la chaîne complète contenant le remplacement.
			string cultureName, fieldName;
			this.access.SearcherIndexToAccess(searcher.Field, this.secondaryCulture, out cultureName, out fieldName);
			ResourceAccess.Field field = this.access.GetField(searcher.Row, cultureName, fieldName);
			System.Diagnostics.Debug.Assert(field != null);

			string text = "";

			if (field.FieldType == ResourceAccess.Field.Type.String)
			{
				text = field.String;
				text = text.Remove(searcher.Index, searcher.Length);
				text = text.Insert(searcher.Index, replace);

				if (fieldName == "Name")
				{
					if (!Misc.IsValidLabel(ref text))
					{
						this.module.MainWindow.DialogError(Res.Strings.Error.InvalidLabel);
						return null;
					}

					if (this.access.IsExistingName(text))
					{
						this.module.MainWindow.DialogError(Res.Strings.Error.NameAlreadyExist);
						return null;
					}
				}

				this.access.SetField(searcher.Row, cultureName, fieldName, new ResourceAccess.Field(text));
			}

			if (field.FieldType == ResourceAccess.Field.Type.StringCollection)
			{
				string[] array = new string[field.StringCollection.Count];
				field.StringCollection.CopyTo(array, 0);

				text = array[searcher.Subfield];
				text = text.Remove(searcher.Index, searcher.Length);
				text = text.Insert(searcher.Index, replace);
				array[searcher.Subfield] = text;

				this.access.SetField(searcher.Row, cultureName, fieldName, new ResourceAccess.Field(array));
			}

			return text;
		}

		public void DoFilter(string filter, Searcher.SearchingMode mode)
		{
			//	Change le filtre des ressources visibles.
			this.access.SetFilter(filter, mode);

			this.UpdateArray();
			this.array.SelectedRow = this.access.AccessIndex;
			this.array.ShowSelectedRow();

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

			this.array.SelectedRow = this.access.AccessIndex;
			this.array.ShowSelectedRow();
			this.UpdateCommands();
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
				this.UpdateModificationsCulture();
				this.UpdateCommands();
			}
			else if (name == "ModificationClear")
			{
				if (sel == -1)  return;
				this.access.ModificationClear(sel, this.secondaryCulture);

				this.UpdateArray();
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
				this.array.SelectedRow = sel;
				this.array.ShowSelectedRow();
				this.SelectEdit(secondary);
			}
		}

		public void DoDelete()
		{
			//	Supprime la ressource sélectionnée.
			this.access.Delete();

			this.UpdateArray();
			this.array.SelectedRow = this.access.AccessIndex;
			this.array.ShowSelectedRow();
			this.UpdateCommands();
		}

		public void DoDuplicate(bool duplicate)
		{
			//	Duplique la ressource sélectionnée.
			ResourceAccess.Field field = this.access.GetField(this.access.AccessIndex, null, "Name");
			string newName = this.access.GetDuplicateName(field.String);
			this.access.Duplicate(newName, duplicate);

			this.UpdateArray();
			this.array.SelectedRow = this.access.AccessIndex;
			this.array.ShowSelectedRow();
			this.UpdateCommands();
		}

		public void DoMove(int direction)
		{
			//	Déplace la ressource sélectionnée.
			this.access.Move(direction);

			this.UpdateArray();
			this.array.SelectedRow = this.access.AccessIndex;
			this.array.ShowSelectedRow();
			this.UpdateCommands();
		}

		public void DoNewCulture()
		{
			//	Crée une nouvelle culture.
			string name = this.module.MainWindow.DlgNewCulture(this.access);
			if ( name == null )  return;
			this.access.CreateCulture(name);

			this.UpdateCultures();
			this.UpdateArray();
			this.UpdateModificationsCulture();
			this.UpdateClientGeometry();
			this.UpdateCommands();
		}

		public void DoDeleteCulture()
		{
			//	Supprime la culture courante.
			ResourceBundle bundle = this.access.GetCultureBundle(this.secondaryCulture);
			string question = string.Format(Res.Strings.Dialog.DeleteCulture.Question, Misc.CultureName(bundle.Culture));
			Common.Dialogs.DialogResult result = this.module.MainWindow.DialogQuestion(question);
			if ( result != Epsitec.Common.Dialogs.DialogResult.Yes )  return;

			this.access.DeleteCulture(this.secondaryCulture);

			this.UpdateCultures();
			if (this.secondaryCulture != null)
			{
				this.UpdateSelectedCulture();
			}
			this.UpdateArray();
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

			if (name == "FontUnderlined")
			{
				this.currentTextField.TextNavigator.SelectionUnderlined = !this.currentTextField.TextNavigator.SelectionUnderlined;
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
			this.UpdateCommands();
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
					ResourceAccess.Field field = this.access.GetField(sel, null, "Name");
					if (field != null)
					{
						builder.Append(field.String);
						builder.Append(": ");
						builder.Append((sel+1).ToString());
					}
				}

				builder.Append("/");
				builder.Append(this.access.AccessCount.ToString());

				if (this.access.AccessCount < this.access.TotalCount)
				{
					builder.Append(" (");
					builder.Append(this.access.TotalCount.ToString());
					builder.Append(")");
				}

				return builder.ToString();
			}
		}


		public void UpdateExternalDirtyList()
		{
			//	Met à jour la liste des ressources si elle a été modifiée depuis l'extérieur.
			if (this.access.IsExternalDirtyList)
			{
				this.UpdateArray();
				this.access.IsExternalDirtyList = false;
			}
		}

		protected virtual void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			//	Cette version convient pour un tableau d'une seule colonne (Captions, Panels, etc.).
			//	Seul Strings et ses trois colonnes doit implémenter une autre version.
			this.array.TotalRows = this.access.AccessCount;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.access.AccessCount)
				{
					this.UpdateArrayField(0, first+i, null, "Name");
				}
				else
				{
					this.UpdateArrayField(0, first+i, null, null);
				}
			}

			this.array.SelectedRow = this.access.AccessIndex;
		}

		protected void UpdateModificationsCulture()
		{
			//	Met à jour les pastilles dans les boutons des cultures.
			if (this.secondaryCultures == null)  // pas de culture secondaire ?
			{
				return;
			}

			int sel = this.access.AccessIndex;

			foreach (IconButtonMark button in this.secondaryCultures)
			{
				ResourceAccess.ModificationState state = this.access.GetModification(sel, button.Name);

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

		protected void UpdateSelectedCulture()
		{
			//	Sélectionne le bouton correspondant à la culture secondaire.
			for (int i=0; i<this.secondaryCultures.Length; i++)
			{
				if (this.secondaryCultures[i].Name == this.secondaryCulture)
				{
					this.secondaryCultures[i].ActiveState = ActiveState.Yes;
				}
				else
				{
					this.secondaryCultures[i].ActiveState = ActiveState.No;
				}
			}
		}

		protected virtual Widget CultureParentWidget
		{
			//	Retourne le parent à utiliser pour les boutons des cultures.
			get
			{
				return this;
			}
		}

		protected void UpdateCultures()
		{
			//	Met à jour les boutons des cultures en fonction des cultures existantes.
			if (this.secondaryCultures != null)
			{
				foreach (IconButtonMark button in this.secondaryCultures)
				{
					button.Clicked -= new MessageEventHandler(this.HandleSecondaryCultureClicked);
					button.Dispose();
				}
				this.secondaryCultures = null;
			}

			ResourceBundle bundle = this.access.GetCultureBundle(this.access.GetBaseCultureName());
			this.primaryCulture.Text = string.Format(Res.Strings.Viewers.Strings.Reference, Misc.CultureName(bundle.Culture));

			Widget parent = this.CultureParentWidget;
			bool isCaptions = !(parent is Strings);

			List<string> list = this.access.GetSecondaryCultureNames();
			if (list.Count > 0)
			{
				this.secondaryCultures = new IconButtonMark[list.Count];
				for (int i=0; i<list.Count; i++)
				{
					bundle = this.access.GetCultureBundle(list[i]);

					this.secondaryCultures[i] = new IconButtonMark(parent);
					this.secondaryCultures[i].ButtonStyle = ButtonStyle.ActivableIcon;
					this.secondaryCultures[i].SiteMark = SiteMark.OnBottom;
					this.secondaryCultures[i].MarkDimension = isCaptions ? 10 : 5;
					this.secondaryCultures[i].Name = list[i];
					this.secondaryCultures[i].Text = Misc.CultureName(bundle.Culture);
					this.secondaryCultures[i].AutoFocus = false;
					this.secondaryCultures[i].TabIndex = i;
					this.secondaryCultures[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
					if (isCaptions)
					{
						this.secondaryCultures[i].Margins = new Margins(0, (i==list.Count-1)?1:0, 0, 0);
						this.secondaryCultures[i].Dock = DockStyle.StackFill;
					}
					this.secondaryCultures[i].Clicked += new MessageEventHandler(this.HandleSecondaryCultureClicked);
					ToolTip.Default.SetToolTip(this.secondaryCultures[i], Misc.CultureLongName(bundle.Culture));
				}

				this.secondaryCulture = list[0];
				this.UpdateSelectedCulture();
			}
			else
			{
				this.secondaryCulture = null;
			}
		}

		protected virtual void SelectEdit(bool secondary)
		{
		}

		protected virtual void UpdateEdit()
		{
		}

		public virtual void Update()
		{
			//	Met à jour le contenu du Viewer.
		}

		public void UpdateCommands()
		{
			//	Met à jour les commandes en fonction de la ressource sélectionnée.
			int sel = this.access.AccessIndex;
			int count = this.access.AccessCount;
			bool build = (this.module.Mode == DesignerMode.Build);

			this.UpdateCommandTool("ToolSelect");
			this.UpdateCommandTool("ToolGlobal");
			this.UpdateCommandTool("ToolGrid");
			this.UpdateCommandTool("ToolEdit");
			this.UpdateCommandTool("ToolZoom");
			this.UpdateCommandTool("ToolHand");
			this.UpdateCommandTool("ObjectVLine");
			this.UpdateCommandTool("ObjectHLine");
			this.UpdateCommandTool("ObjectButton");
			this.UpdateCommandTool("ObjectText");
			this.UpdateCommandTool("ObjectStatic");
			this.UpdateCommandTool("ObjectGroup");
			this.UpdateCommandTool("ObjectGroupBox");

			this.GetCommandState("Save").Enable = this.module.IsDirty;
			this.GetCommandState("SaveAs").Enable = true;

			if (this is Panels)
			{
				this.GetCommandState("NewCulture").Enable = false;
				this.GetCommandState("DeleteCulture").Enable = false;
			}
			else
			{
				this.GetCommandState("NewCulture").Enable = (this.access.CultureCount < Misc.Cultures.Length);
				this.GetCommandState("DeleteCulture").Enable = (this.access.CultureCount > 1);
			}

			bool search = this.module.MainWindow.DialogSearch.IsActionsEnabled;
			this.GetCommandState("Search").Enable = true;
			this.GetCommandState("SearchPrev").Enable = search;
			this.GetCommandState("SearchNext").Enable = search;

			this.GetCommandState("Filter").Enable = true;

			this.GetCommandState("AccessFirst").Enable = (sel != -1 && sel > 0);
			this.GetCommandState("AccessPrev").Enable = (sel != -1 && sel > 0);
			this.GetCommandState("AccessLast").Enable = (sel != -1 && sel < count-1);
			this.GetCommandState("AccessNext").Enable = (sel != -1 && sel < count-1);

			this.GetCommandState("Delete").Enable = (sel != -1 && count > 1 && build);
			this.GetCommandState("Create").Enable = (sel != -1 && build);
			this.GetCommandState("Duplicate").Enable = (sel != -1 && build);

			this.GetCommandState("Up").Enable = (sel != -1 && sel > 0 && build);
			this.GetCommandState("Down").Enable = (sel != -1 && sel < count-1 && build);

			if (this is Panels)
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

			if (this is Panels)
			{
				this.GetCommandState("FontBold").Enable = false;
				this.GetCommandState("FontItalic").Enable = false;
				this.GetCommandState("FontUnderlined").Enable = false;
				this.GetCommandState("Glyphs").Enable = false;
			}
			else
			{
				this.GetCommandState("FontBold").Enable = (sel != -1);
				this.GetCommandState("FontItalic").Enable = (sel != -1);
				this.GetCommandState("FontUnderlined").Enable = (sel != -1);
				this.GetCommandState("Glyphs").Enable = (sel != -1);
			}

			if (this is Panels)
			{
				int objSelected, objCount;
				Panels panels = this as Panels;
				panels.PanelEditor.GetSelectionInfo(out objSelected, out objCount);

				this.GetCommandState("PanelDelete").Enable = (objSelected != 0);
				this.GetCommandState("PanelDuplicate").Enable = (objSelected != 0);
				this.GetCommandState("PanelDeselectAll").Enable = (objSelected != 0);
				this.GetCommandState("PanelSelectAll").Enable = (objSelected < objCount);
				this.GetCommandState("PanelSelectInvert").Enable = (objCount > 0);

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

				this.GetCommandState("OrderUpAll").Enable = (objSelected != 0 && objCount >= 2);
				this.GetCommandState("OrderDownAll").Enable = (objSelected != 0 && objCount >= 2);
				this.GetCommandState("OrderUpOne").Enable = (objSelected != 0 && objCount >= 2);
				this.GetCommandState("OrderDownOne").Enable = (objSelected != 0 && objCount >= 2);

				this.GetCommandState("TabIndexClear").Enable = (objSelected != 0);
				this.GetCommandState("TabIndexRenum").Enable = (objCount != 0);
				this.GetCommandState("TabIndexLast").Enable = (objSelected != 0);
				this.GetCommandState("TabIndexPrev").Enable = (objSelected != 0);
				this.GetCommandState("TabIndexNext").Enable = (objSelected != 0);
				this.GetCommandState("TabIndexFirst").Enable = (objSelected != 0);
			}
			else
			{
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
			}

			this.module.MainWindow.UpdateInfoCurrentModule();
			this.module.MainWindow.UpdateInfoAccess();
			this.module.MainWindow.UpdateInfoViewer();
		}

		protected void UpdateCommandTool(string name)
		{
			this.GetCommandState(name).ActiveState = (this.context.Tool == name) ? ActiveState.Yes : ActiveState.No;
		}

		protected CommandState GetCommandState(string command)
		{
			return this.module.MainWindow.GetCommandState(command);
		}


		protected void SetTextField(AbstractTextField textField, int index, string cultureName, string fieldName)
		{
			if (fieldName == null)
			{
				textField.Enable = false;
				textField.Text = "";
			}
			else
			{
				ResourceAccess.Field field = this.access.GetField(index, cultureName, fieldName);

				textField.Enable = true;
				textField.Text = field.String;
			}
		}

		protected void SetTextField(MyWidgets.StringCollection collection, int index, string cultureName, string fieldName)
		{
			if (fieldName == null)
			{
				collection.Enable = false;
				collection.Collection = null;
			}
			else
			{
				ResourceAccess.Field field = this.access.GetField(index, cultureName, fieldName);

				List<string> list = new List<string>();
				foreach (string text in field.StringCollection)
				{
					list.Add(text);
				}

				collection.Enable = true;
				collection.Collection = list;
			}
		}

		protected void SetTextField(IconButton button, int index, string cultureName, string fieldName)
		{
			if (fieldName == null)
			{
				button.Enable = false;
				button.IconName = null;
			}
			else
			{
				ResourceAccess.Field field = this.access.GetField(index, cultureName, fieldName);

				button.Enable = true;
				button.IconName = field.String;
			}
		}

		protected void UpdateFieldName(AbstractTextField edit, int sel)
		{
			//	Change le 'Name' d'une ressource, en gérant les diverses impossibilités.
			string editedName = edit.Text;
			string initialName = this.access.GetField(sel, null, "Name").String;

			if (!Misc.IsValidLabel(ref editedName))
			{
				this.ignoreChange = true;
				edit.Text = initialName;
				edit.SelectAll();
				this.ignoreChange = false;

				this.module.MainWindow.DialogError(Res.Strings.Error.InvalidLabel);
				return;
			}

			this.ignoreChange = true;
			edit.Text = editedName;
			edit.SelectAll();
			this.ignoreChange = false;

			if (editedName == initialName)  // label inchangé ?
			{
				return;
			}

			if (this.access.IsExistingName(editedName))
			{
				this.ignoreChange = true;
				edit.Text = initialName;
				edit.SelectAll();
				this.ignoreChange = false;

				this.module.MainWindow.DialogError(Res.Strings.Error.NameAlreadyExist);
				return;
			}

			this.access.SetField(sel, null, "Name", new ResourceAccess.Field(editedName));
			this.UpdateArrayField(0, sel, null, "Name");
		}

		protected void UpdateArrayField(int column, int row, string culture, string fieldName)
		{
			//	Met à jour une cellule dans le tableau.
			if (fieldName == null)
			{
				this.array.SetLineString(column, row, "");
				this.array.SetLineState(column, row, MyWidgets.StringList.CellState.Disabled);
			}
			else
			{
				ResourceAccess.Field field = this.access.GetField(row, culture, fieldName);
				ResourceAccess.ModificationState state = this.access.GetModification(row, culture);

				if (column == 0)
				{
					state = ResourceAccess.ModificationState.Normal;
				}

				this.array.SetLineString(column, row, field.String);
				this.array.SetLineState(column, row, ResourceAccess.CellState(state));
			}
		}

		protected static Color GetBackgroundColor(ResourceAccess.ModificationState state, double intensity)
		{
			//	Donne une couleur pour un fond de panneau.
			switch (state)
			{
				case ResourceAccess.ModificationState.Empty:
					return Color.FromAlphaRgb(intensity, 0.91, 0.40, 0.40);  // rouge

				case ResourceAccess.ModificationState.Modified:
					return Color.FromAlphaRgb(intensity, 0.91, 0.81, 0.41);  // jaune

				default:
					IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
					Color cap = adorner.ColorCaption;
					return Color.FromAlphaRgb(intensity, 0.5+cap.R*0.5, 0.5+cap.G*0.5, 0.5+cap.B*0.5);
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
					AbstractCaptions.SearchCreateFilterGroup(parent, handler);
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


		protected void HandleEditKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appelé lorsqu'une ligne éditable voit son focus changer.
			bool focused = (bool) e.NewValue;

			if (focused)
			{
				this.currentTextField = sender as AbstractTextField;
			}
		}

		protected void HandleSecondaryCultureClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton pour changer de culture secondaire a été cliqué.
			IconButtonMark button = sender as IconButtonMark;
			this.secondaryCulture = button.Name;

			this.UpdateSelectedCulture();
			this.UpdateArray();
			this.UpdateEdit();
			this.UpdateCommands();
		}


		protected Module					module;
		protected PanelsContext				context;
		protected ResourceAccess			access;
		protected string					secondaryCulture;
		protected bool						ignoreChange = false;
		protected bool						lastActionIsReplace = false;
		protected IconButtonMark			primaryCulture;
		protected IconButtonMark[]			secondaryCultures;
		protected MyWidgets.StringArray		array;
		protected AbstractTextField			currentTextField;
		protected int						tabIndex = 0;
	}
}
