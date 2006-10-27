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
	public class Commands : AbstractCaptions
	{
		public Commands(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			MyWidgets.StackedPanel leftContainer, rightContainer;

			//	Aspect et Statefull.
			this.CreateBand(out leftContainer, Res.Strings.Viewers.Commands.Statefull.Title, 0.1);

			StaticText label = new StaticText(leftContainer.Container);
			label.CaptionDruid = Res.Captions.Aspect.ButtonAspect.Druid;
			label.PreferredWidth = 110;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Margins = new Margins(0, 5, 0, 0);
			label.Dock = DockStyle.Left;

			this.primaryAspectDialog = new IconButton(leftContainer.Container);
			this.primaryAspectDialog.MinSize = this.primaryAspectDialog.PreferredSize;  // attention, très important !
			this.primaryAspectDialog.CommandDruid = Res.Values.Widgets.ButtonAspect.DialogButton.Druid;
			this.primaryAspectDialog.Dock = DockStyle.Left;
			this.primaryAspectDialog.Clicked += new MessageEventHandler(this.HandlePrimaryAspectClicked);

			this.primaryAspectIcon = new IconButton(leftContainer.Container);
			this.primaryAspectIcon.MinSize = this.primaryAspectIcon.PreferredSize;  // attention, très important !
			this.primaryAspectIcon.CommandDruid = Res.Values.Widgets.ButtonAspect.IconButton.Druid;
			this.primaryAspectIcon.Dock = DockStyle.Left;
			this.primaryAspectIcon.Clicked += new MessageEventHandler(this.HandlePrimaryAspectClicked);

			this.primaryStatefull = new CheckButton(leftContainer.Container);
			this.primaryStatefull.Text = Res.Strings.Viewers.Commands.Statefull.CheckButton;
			this.primaryStatefull.PreferredWidth = 250;
			this.primaryStatefull.Margins = new Margins(50, 0, 0, 0);
			this.primaryStatefull.Dock = DockStyle.Left;
			this.primaryStatefull.Pressed += new MessageEventHandler(this.HandleStatefullPressed);
			this.primaryStatefull.TabIndex = this.tabIndex++;
			this.primaryStatefull.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Shortcuts.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Commands.Shortcut.Title, 0.3);

			this.primaryShortcut1 = new ShortcutEditor(leftContainer.Container);
			this.primaryShortcut1.Title = Res.Strings.Viewers.Commands.Shortcut.Main;
			this.primaryShortcut1.Margins = new Margins(0, 0, 0, 2);
			this.primaryShortcut1.Dock = DockStyle.StackBegin;
			this.primaryShortcut1.EditedShortcutChanged += new EventHandler(this.HandleShortcutEditedShortcutChanged);
			this.primaryShortcut1.TabIndex = this.tabIndex++;
			this.primaryShortcut1.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.primaryShortcut2 = new ShortcutEditor(leftContainer.Container);
			this.primaryShortcut2.Title = Res.Strings.Viewers.Commands.Shortcut.Suppl;
			this.primaryShortcut2.Dock = DockStyle.StackBegin;
			this.primaryShortcut2.EditedShortcutChanged += new EventHandler(this.HandleShortcutEditedShortcutChanged);
			this.primaryShortcut2.TabIndex = this.tabIndex++;
			this.primaryShortcut2.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.secondaryShortcut1 = new ShortcutEditor(rightContainer.Container);
			this.secondaryShortcut1.Title = Res.Strings.Viewers.Commands.Shortcut.Main;
			this.secondaryShortcut1.Margins = new Margins(0, 0, 0, 2);
			this.secondaryShortcut1.Dock = DockStyle.StackBegin;
			this.secondaryShortcut1.EditedShortcutChanged += new EventHandler(this.HandleShortcutEditedShortcutChanged);
			this.secondaryShortcut1.TabIndex = this.tabIndex++;
			this.secondaryShortcut1.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.secondaryShortcut2 = new ShortcutEditor(rightContainer.Container);
			this.secondaryShortcut2.Title = Res.Strings.Viewers.Commands.Shortcut.Suppl;
			this.secondaryShortcut2.Dock = DockStyle.StackBegin;
			this.secondaryShortcut2.EditedShortcutChanged += new EventHandler(this.HandleShortcutEditedShortcutChanged);
			this.secondaryShortcut2.TabIndex = this.tabIndex++;
			this.secondaryShortcut2.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			//	Group.
			this.CreateBand(out leftContainer, Res.Strings.Viewers.Commands.Group.Title, 0.5);

			this.primaryGroup = new TextFieldCombo(leftContainer.Container);
			this.primaryGroup.PreferredWidth = 200;
			this.primaryGroup.HorizontalAlignment = HorizontalAlignment.Left;
			this.primaryGroup.Dock = DockStyle.StackBegin;
			this.primaryGroup.TextChanged += new EventHandler(this.HandleGroupTextChanged);
			this.primaryGroup.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryGroup.ComboOpening += new EventHandler<CancelEventArgs>(this.HandleGroupComboOpening);
			this.primaryGroup.TabIndex = this.tabIndex++;
			this.primaryGroup.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.UpdateEdit();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.primaryAspectDialog.Clicked -= new MessageEventHandler(this.HandlePrimaryAspectClicked);
				this.primaryAspectIcon.Clicked -= new MessageEventHandler(this.HandlePrimaryAspectClicked);
				this.primaryStatefull.Pressed -= new MessageEventHandler(this.HandleStatefullPressed);

				this.primaryShortcut1.EditedShortcutChanged -= new EventHandler(this.HandleShortcutEditedShortcutChanged);
				this.primaryShortcut2.EditedShortcutChanged -= new EventHandler(this.HandleShortcutEditedShortcutChanged);
				this.secondaryShortcut1.EditedShortcutChanged -= new EventHandler(this.HandleShortcutEditedShortcutChanged);
				this.secondaryShortcut2.EditedShortcutChanged -= new EventHandler(this.HandleShortcutEditedShortcutChanged);

				this.primaryGroup.TextChanged -= new EventHandler(this.HandleGroupTextChanged);
				this.primaryGroup.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
				this.primaryGroup.ComboOpening -= new EventHandler<CancelEventArgs>(this.HandleGroupComboOpening);
			}

			base.Dispose(disposing);
		}


		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Commands;
			}
		}


		public override void Update()
		{
			//	Met à jour le contenu du Viewer.
			base.Update();
		}

		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			if (this.primaryStatefull == null)
			{
				return;
			}

			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			int sel = this.access.AccessIndex;

			if (sel >= this.access.AccessCount)
			{
				sel = -1;
			}

			if (sel == -1)
			{
				this.primaryAspectDialog.Enable = false;
				this.primaryAspectIcon.Enable = false;

				this.primaryStatefull.Enable = false;
				this.primaryStatefull.ActiveState = ActiveState.No;

				this.SetShortcut(this.primaryShortcut1, this.primaryShortcut2, 0, null, ResourceAccess.FieldType.None);
				this.SetShortcut(this.secondaryShortcut1, this.secondaryShortcut2, 0, null, ResourceAccess.FieldType.None);

				this.SetTextField(this.primaryGroup, 0, null, ResourceAccess.FieldType.None);
			}
			else
			{
				ResourceAccess.Field field;

				field = this.access.GetField(sel, null, ResourceAccess.FieldType.Controller);
				this.primaryAspectDialog.Enable = true;
				this.primaryAspectDialog.ActiveState = (field.String == "DialogButton" || string.IsNullOrEmpty(field.String)) ? ActiveState.Yes : ActiveState.No;
				this.primaryAspectIcon.Enable = true;
				this.primaryAspectIcon.ActiveState = (field.String == "IconButton") ? ActiveState.Yes : ActiveState.No;

				field = this.access.GetField(sel, null, ResourceAccess.FieldType.Statefull);
				bool statefull = field.Boolean;
				this.primaryStatefull.Enable = true;
				this.primaryStatefull.ActiveState = statefull ? ActiveState.Yes : ActiveState.No;

				this.SetShortcut(this.primaryShortcut1, this.primaryShortcut2, sel, null, ResourceAccess.FieldType.Shortcuts);
				this.SetShortcut(this.secondaryShortcut1, this.secondaryShortcut2, sel, this.secondaryCulture, ResourceAccess.FieldType.Shortcuts);

				this.SetTextField(this.primaryGroup, sel, null, ResourceAccess.FieldType.Group);
			}

			this.ignoreChange = iic;

			base.UpdateEdit();
		}

		protected void SetShortcut(ShortcutEditor editor1, ShortcutEditor editor2, int index, string cultureName, ResourceAccess.FieldType fieldType)
		{
			if (fieldType == ResourceAccess.FieldType.None)
			{
				editor1.Enable = false;
				editor2.Enable = false;

				editor1.Shortcut = new Shortcut(KeyCode.None);
				editor2.Shortcut = new Shortcut(KeyCode.None);
			}
			else
			{
				editor1.Enable = true;
				editor2.Enable = true;

				ResourceAccess.Field field = this.access.GetField(index, cultureName, fieldType);
				Widgets.Collections.ShortcutCollection collection = field.ShortcutCollection;

				if (collection.Count < 1)
				{
					editor1.Shortcut = new Shortcut(KeyCode.None);
				}
				else
				{
					editor1.Shortcut = collection[0];
				}

				if (collection.Count < 2)
				{
					editor2.Shortcut = new Shortcut(KeyCode.None);
				}
				else
				{
					editor2.Shortcut = collection[1];
				}
			}
		}

		protected void UpdateGroupCombo()
		{
			//	Met à jour le menu du combo des groupes.
			List<string> list = new List<string>();

			for (int i=0; i<this.access.TotalCount; i++)
			{
				Druid druid = this.access.DirectGetDruid(i);
				string group = this.access.DirectGetGroup(druid);
				if (group != null && !list.Contains(group))
				{
					list.Add(group);
				}
			}

			list.Sort();  // trie par ordre alphabétique

			this.primaryGroup.Items.Clear();
			foreach (string name in list)
			{
				this.primaryGroup.Items.Add(name);
			}
		}


		protected override void TextFieldToIndex(AbstractTextField textField, out int field, out int subfield)
		{
			//	Cherche les index correspondant à un texte éditable.
			base.TextFieldToIndex(textField, out field, out subfield);
		}

		protected override AbstractTextField IndexToTextField(int field, int subfield)
		{
			//	Cherche le TextField permettant d'éditer des index.
			return base.IndexToTextField(field, subfield);
		}


		private void HandlePrimaryAspectClicked(object sender, MessageEventArgs e)
		{
			//	Bouton 'aspect' pressé.
			string defaultParameter = null;

			if (sender == this.primaryAspectDialog)
			{
				defaultParameter = "DialogButton";
			}

			if (sender == this.primaryAspectIcon)
			{
				defaultParameter = "IconButton";
			}

			int sel = this.access.AccessIndex;
			this.access.SetField(sel, null, ResourceAccess.FieldType.Controller, new ResourceAccess.Field(defaultParameter));

			this.UpdateEdit();
			this.UpdateColor();
		}

		private void HandleStatefullPressed(object sender, MessageEventArgs e)
		{
			//	Bouton à cocher 'Statefull' pressé.
			if (this.ignoreChange)
			{
				return;
			}

			bool statefull = (this.primaryStatefull.ActiveState == ActiveState.No);
			int sel = this.access.AccessIndex;
			this.access.SetField(sel, null, ResourceAccess.FieldType.Statefull, new ResourceAccess.Field(statefull));

			this.UpdateColor();
		}

		private void HandleShortcutEditedShortcutChanged(object sender)
		{
			//	Un raccourci clavier a été changé.
			if (this.ignoreChange)
			{
				return;
			}

			ShortcutEditor editor = sender as ShortcutEditor;
			ShortcutEditor editor1 = null;
			ShortcutEditor editor2 = null;
			int sel = this.access.AccessIndex;
			string cultureName = null;
			int rank = -1;

			if (editor == this.primaryShortcut1)
			{
				cultureName = null;  // culture principale
				rank = 0;  // raccourci principal
				editor1 = this.primaryShortcut1;
				editor2 = this.primaryShortcut2;
			}

			if (editor == this.primaryShortcut2)
			{
				cultureName = null;  // culture principale
				rank = 1;  // raccourci supplémentaire
				editor1 = this.primaryShortcut1;
				editor2 = this.primaryShortcut2;
			}

			if (editor == this.secondaryShortcut1)
			{
				cultureName = this.secondaryCulture;  // culture secondaire
				rank = 0;  // raccourci principal
				editor1 = this.secondaryShortcut1;
				editor2 = this.secondaryShortcut2;
			}

			if (editor == this.secondaryShortcut2)
			{
				cultureName = this.secondaryCulture;  // culture secondaire
				rank = 1;  // raccourci supplémentaire
				editor1 = this.secondaryShortcut1;
				editor2 = this.secondaryShortcut2;
			}

			System.Diagnostics.Debug.Assert(rank != -1);

			ResourceAccess.Field field = this.access.GetField(sel, cultureName, ResourceAccess.FieldType.Shortcuts);
			Widgets.Collections.ShortcutCollection collection = field.ShortcutCollection;

			if (rank == 0)  // raccourci principal ?
			{
				if (collection.Count == 0)
				{
					collection.Add(editor.Shortcut);  // insère le raccourci principal
				}
				else
				{
					collection[rank] = editor.Shortcut;  // modifie le raccourci principal
				}
			}

			if (rank == 1)  // raccourci supplémentaire ?
			{
				if (collection.Count == 0)
				{
					collection.Add(new Shortcut(KeyCode.None));  // insère un raccourci principal inexistant
					collection.Add(editor.Shortcut);  // insère le raccourci supplémentaire
				}
				else if (collection.Count == 1)
				{
					collection.Add(editor.Shortcut);  // insère le raccourci supplémentaire
				}
				else
				{
					collection[rank] = editor.Shortcut;  // modifie le raccourci supplémentaire
				}
			}

			//	Supprime tous les raccourcis inexistant KeyCode.None de la collection.
			int i = 0;
			bool removed = false;
			while (i < collection.Count)
			{
				if (collection[i] == KeyCode.None)
				{
					collection.RemoveAt(i);
					removed = true;
				}
				else
				{
					i++;
				}
			}

			//	Si des raccourcis inexistants ont été supprimés, il faut réinitialiser les
			//	widgets, pour éviter d'avoir un trou (KeyCode.None puis KeyCode.AlphaA).
			if (removed)
			{
				this.ignoreChange = true;
				
				if (collection.Count < 1)
				{
					editor1.Shortcut = new Shortcut(KeyCode.None);
				}
				else
				{
					editor1.Shortcut = collection[0];
				}

				if (collection.Count < 2)
				{
					editor2.Shortcut = new Shortcut(KeyCode.None);
				}
				else
				{
					editor2.Shortcut = collection[1];
				}

				this.ignoreChange = false;
			}

			this.access.SetField(sel, cultureName, ResourceAccess.FieldType.Shortcuts, new ResourceAccess.Field(collection));
			this.UpdateColor();
		}

		private void HandleGroupComboOpening(object sender, CancelEventArgs e)
		{
			//	Le combo pour le groupe va être ouvert.
			this.UpdateGroupCombo();
		}

		private void HandleGroupTextChanged(object sender)
		{
			//	Le texte éditable pour le groupe a changé.
			if (this.ignoreChange)
			{
				return;
			}

			AbstractTextField edit = sender as AbstractTextField;
			string text = edit.Text;
			int sel = this.access.AccessIndex;

			if (edit == this.primaryGroup)
			{
				this.access.SetField(sel, null, ResourceAccess.FieldType.Group, new ResourceAccess.Field(text));
			}

			this.UpdateColor();
		}
		

		protected IconButton					primaryAspectDialog;
		protected IconButton					primaryAspectIcon;
		protected CheckButton					primaryStatefull;
		protected ShortcutEditor				primaryShortcut1;
		protected ShortcutEditor				primaryShortcut2;
		protected ShortcutEditor				secondaryShortcut1;
		protected ShortcutEditor				secondaryShortcut2;
		protected TextFieldCombo				primaryGroup;
	}
}
