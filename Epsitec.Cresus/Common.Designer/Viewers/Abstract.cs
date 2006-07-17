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
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}


		protected abstract ResourceAccess.Type ResourceType
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
			if (type == ResourceAccess.Type.Panels)  return new Panels(module, context, access);
			if (type == ResourceAccess.Type.Scripts)  return new Scripts(module, context, access);
			return null;
		}


		public virtual ResourceAccess.Type BundleType
		{
			get
			{
				return ResourceAccess.Type.Unknow;
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

		public void DoSearch(string search, Searcher.SearchingMode mode)
		{
			//	Effectue une recherche.
#if false
			Searcher searcher = new Searcher(this.druidsIndex, this.primaryBundle, this.secondaryBundle, this.BundleType);
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
#endif
		}

		public void DoCount(string search, Searcher.SearchingMode mode)
		{
			//	Effectue un comptage.
#if false
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
#endif
		}

		public void DoReplace(string search, string replace, Searcher.SearchingMode mode)
		{
			//	Effectue un remplacement.
#if false
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

					if (this.access.IsExistingName(validReplace))
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

					this.UpdateArrayField(1, searcher.Row, this.primaryBundle[druid], this.secondaryBundle[druid]);
				}

				if (searcher.Field == 2 && this.secondaryBundle != null)
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

				if (searcher.Field == 4 && this.secondaryBundle != null)
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
#endif
		}

		public void DoReplaceAll(string search, string replace, Searcher.SearchingMode mode)
		{
			//	Effectue un 'remplacer tout'.
#if false
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
#endif
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
				this.UpdateModifiers();
				this.UpdateCommands();
			}
			else if (name == "ModificationClear")
			{
				if (sel == -1)  return;
				this.access.ModificationClear(sel, this.secondaryCulture);

				this.UpdateArray();
				this.UpdateModifiers();
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

			this.UpdateCultures(this);
			this.UpdateArray();
			this.UpdateModifiers();
			this.UpdateClientGeometry();
			this.UpdateCommands();
		}

		public void DoDeleteCulture()
		{
			//	Supprime la culture courante.
			string question = string.Format(Res.Strings.Dialog.DeleteCulture.Question, this.secondaryCulture);
			Common.Dialogs.DialogResult result = this.module.MainWindow.DialogQuestion(question);
			if ( result != Epsitec.Common.Dialogs.DialogResult.Yes )  return;

			this.access.DeleteCulture(this.secondaryCulture);

			this.UpdateCultures(this);
			if (this.secondaryCulture != null)
			{
				this.UpdateSelectedCulture();
			}
			this.UpdateArray();
			this.UpdateModifiers();
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
					builder.Append(field.String);
					builder.Append(": ");
					builder.Append((sel+1).ToString());
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


		protected virtual void UpdateArray()
		{
		}

		protected void UpdateModifiers()
		{
			if (this.secondaryCultures == null)
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

		protected virtual void SelectEdit(bool secondary)
		{
		}

		protected virtual void UpdateEdit()
		{
		}

		protected void UpdateCultures(Widget parent)
		{
			if (this.secondaryCultures != null)
			{
				foreach (IconButtonMark button in this.secondaryCultures)
				{
					button.Clicked -= new MessageEventHandler(this.HandleSecondaryCultureClicked);
					button.Dispose();
				}
				this.secondaryCultures = null;
			}

			ResourceBundle bundle = this.access.GetCulture(this.access.GetBaseCultureName());
			this.primaryCulture.Text = string.Format(Res.Strings.Viewers.Strings.Reference, Misc.CultureName(bundle.Culture));

			List<string> list = this.access.GetSecondaryCultureNames();
			if (list.Count > 0)
			{
				this.secondaryCultures = new IconButtonMark[list.Count];
				for (int i=0; i<list.Count; i++)
				{
					bundle = this.access.GetCulture(list[i]);

					this.secondaryCultures[i] = new IconButtonMark(this);
					this.secondaryCultures[i].ButtonStyle = ButtonStyle.ActivableIcon;
					this.secondaryCultures[i].SiteMark = SiteMark.OnBottom;
					this.secondaryCultures[i].MarkDimension = 5;
					this.secondaryCultures[i].Name = list[i];
					this.secondaryCultures[i].Text = Misc.CultureName(bundle.Culture);
					this.secondaryCultures[i].AutoFocus = false;
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

			this.access.SetFilter("", Searcher.SearchingMode.None);
		}

		public virtual void Update()
		{
			//	Met à jour le contenu du Viewer.
		}

		public virtual void UpdateCommands()
		{
			//	Met à jour les commandes en fonction de la ressource sélectionnée.
			int sel = this.access.AccessIndex;
			int count = this.access.AccessCount;
			bool build = (this.module.Mode == DesignerMode.Build);

			this.GetCommandState ("Save").Enable = this.access.IsDirty;
			this.GetCommandState ("SaveAs").Enable = true;
			
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
			this.UpdateArrayField(0, sel, null, editedName);
		}

		protected void UpdateArrayField(int column, int row, string culture, string name)
		{
			//	Met à jour une cellule dans le tableau.
			if (name == null)
			{
				this.array.SetLineString(column, row, "");
				this.array.SetLineState(column, row, MyWidgets.StringList.CellState.Disabled);
			}
			else
			{
				ResourceAccess.Field field = this.access.GetField(row, culture, name);
				ResourceAccess.ModificationState state = this.access.GetModification(row, culture);

				this.array.SetLineString(column, row, field.String);
				this.array.SetLineState(column, row, ResourceAccess.CellState(state));
			}
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


#if false
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
#endif


		void HandleSecondaryCultureClicked(object sender, MessageEventArgs e)
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
	}
}
