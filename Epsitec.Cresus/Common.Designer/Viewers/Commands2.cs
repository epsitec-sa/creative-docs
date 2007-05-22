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
	public class Commands2 : AbstractCaptions2
	{
		public Commands2(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			MyWidgets.StackedPanel leftContainer, rightContainer;

			//	Séparateur.
			this.CreateBand(out leftContainer, "", BandMode.Separator, GlyphShape.None, false, 0.0);

			//	Aspect (pour DefaultParameter) et Statefull.
			this.buttonSuiteCompact = this.CreateBand(out leftContainer, Res.Strings.Viewers.Commands.Statefull.Title, BandMode.SuiteView, GlyphShape.ArrowUp, true, 0.6);
			this.buttonSuiteCompact.Clicked += new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);

			StaticText label = new StaticText(leftContainer.Container);
			label.CaptionId = Res.Captions.Command.ButtonAspect.Id;
			label.PreferredWidth = 150;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Margins = new Margins(0, 5, 0, 0);
			label.Dock = DockStyle.Left;

			this.primaryAspectIcon = new IconButton(leftContainer.Container);
			this.primaryAspectIcon.MinSize = this.primaryAspectIcon.PreferredSize;  // attention, très important !
			this.primaryAspectIcon.CommandDruid = Res.Values.Widgets.ButtonAspect.IconButton.Id;
			this.primaryAspectIcon.ButtonStyle = ButtonStyle.ActivableIcon;  // comme Statefull
			this.primaryAspectIcon.Dock = DockStyle.Left;
			this.primaryAspectIcon.Clicked += new MessageEventHandler(this.HandlePrimaryAspectClicked);

			this.primaryAspectDialog = new IconButton(leftContainer.Container);
			this.primaryAspectDialog.MinSize = this.primaryAspectDialog.PreferredSize;  // attention, très important !
			this.primaryAspectDialog.CommandDruid = Res.Values.Widgets.ButtonAspect.DialogButton.Id;
			this.primaryAspectDialog.ButtonStyle = ButtonStyle.ActivableIcon;  // comme Statefull
			this.primaryAspectDialog.Dock = DockStyle.Left;
			this.primaryAspectDialog.Clicked += new MessageEventHandler(this.HandlePrimaryAspectClicked);

			this.primaryStatefull = new CheckButton(leftContainer.Container);
			this.primaryStatefull.Text = Res.Strings.Viewers.Commands.Statefull.CheckButton;
			this.primaryStatefull.PreferredWidth = 250;
			this.primaryStatefull.Margins = new Margins(40, 0, 0, 0);
			this.primaryStatefull.Dock = DockStyle.Left;
			this.primaryStatefull.Pressed += new MessageEventHandler(this.HandleStatefullPressed);
			this.primaryStatefull.TabIndex = this.tabIndex++;
			this.primaryStatefull.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Shortcuts.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Commands.Shortcut.Title, BandMode.SuiteView, GlyphShape.None, false, 0.6);

			this.primaryShortcut1 = new ShortcutEditor(leftContainer.Container);
			this.primaryShortcut1.Title = Res.Strings.Viewers.Commands.Shortcut.Main;
			this.primaryShortcut1.Margins = new Margins(0, 0, 0, 2);
			this.primaryShortcut1.Dock = DockStyle.StackBegin;
			this.primaryShortcut1.EditedShortcutChanged += new EventHandler(this.HandleShortcutEditedShortcutChanged);
			this.primaryShortcut1.TabIndex = this.tabIndex++;
			this.primaryShortcut1.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.primaryShortcut2 = new ShortcutEditor(leftContainer.Container);
			this.primaryShortcut2.Title = Res.Strings.Viewers.Commands.Shortcut.Suppl;
			this.primaryShortcut2.Dock = DockStyle.StackBegin;
			this.primaryShortcut2.EditedShortcutChanged += new EventHandler(this.HandleShortcutEditedShortcutChanged);
			this.primaryShortcut2.TabIndex = this.tabIndex++;
			this.primaryShortcut2.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.secondaryShortcut1 = new ShortcutEditor(rightContainer.Container);
			this.secondaryShortcut1.Title = Res.Strings.Viewers.Commands.Shortcut.Main;
			this.secondaryShortcut1.Margins = new Margins(0, 0, 0, 2);
			this.secondaryShortcut1.Dock = DockStyle.StackBegin;
			this.secondaryShortcut1.EditedShortcutChanged += new EventHandler(this.HandleShortcutEditedShortcutChanged);
			this.secondaryShortcut1.TabIndex = this.tabIndex++;
			this.secondaryShortcut1.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.secondaryShortcut2 = new ShortcutEditor(rightContainer.Container);
			this.secondaryShortcut2.Title = Res.Strings.Viewers.Commands.Shortcut.Suppl;
			this.secondaryShortcut2.Dock = DockStyle.StackBegin;
			this.secondaryShortcut2.EditedShortcutChanged += new EventHandler(this.HandleShortcutEditedShortcutChanged);
			this.secondaryShortcut2.TabIndex = this.tabIndex++;
			this.secondaryShortcut2.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			//	Group.
			this.CreateBand(out leftContainer, Res.Strings.Viewers.Commands.Group.Title, BandMode.SuiteView, GlyphShape.None, false, 0.6);

			label = new StaticText(leftContainer.Container);
			label.Text = Res.Strings.Viewers.Commands.Group.Title;
			label.MinHeight = 20;  // attention, très important !
			label.PreferredHeight = 20;
			label.PreferredWidth = 89;  // calqué sur ShortcutEditor
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Margins = new Margins(0, 5, 0, 0);
			label.Dock = DockStyle.Left;

			this.primaryGroup = new TextFieldCombo(leftContainer.Container);
			this.primaryGroup.MinHeight = 20;  // attention, très important !
			this.primaryGroup.PreferredWidth = 216;  // calqué sur ShortcutEditor
			this.primaryGroup.HorizontalAlignment = HorizontalAlignment.Left;
			this.primaryGroup.Dock = DockStyle.Left;
			this.primaryGroup.TextChanged += new EventHandler(this.HandleGroupTextChanged);
			this.primaryGroup.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryGroup.ComboOpening += new EventHandler<CancelEventArgs>(this.HandleGroupComboOpening);
			this.primaryGroup.TabIndex = this.tabIndex++;
			this.primaryGroup.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Résumé des paramètres.
			this.buttonSuiteExtend = this.CreateBand(out leftContainer, out rightContainer, "Résumé", BandMode.SuiteSummary, GlyphShape.ArrowDown, true, 0.6);
			this.buttonSuiteExtend.Clicked += new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);

			this.primarySuiteSummary = new StaticText(leftContainer.Container);
			this.primarySuiteSummary.MinHeight = 30;
			this.primarySuiteSummary.Dock = DockStyle.Fill;

			this.primarySuiteSummaryIcon = new IconButton(leftContainer.Container);
			this.primarySuiteSummaryIcon.MinSize = new Size(30, 30);
			this.primarySuiteSummaryIcon.Dock = DockStyle.Right;

			this.secondarySuiteSummary = new StaticText(rightContainer.Container);
			this.secondarySuiteSummary.MinHeight = 30;
			this.secondarySuiteSummary.Dock = DockStyle.Fill;

			this.secondarySuiteSummaryIcon = new IconButton(rightContainer.Container);
			this.secondarySuiteSummaryIcon.MinSize = new Size(30, 30);
			this.secondarySuiteSummaryIcon.Dock = DockStyle.Right;

			this.UpdateAll();
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
				return ResourceAccess.Type.Commands2;
			}
		}


		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			base.UpdateEdit();

			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			StructuredData data;

			data = item.GetCultureData(this.GetTwoLetters(0));
			string shortcut = data.GetValue(Support.Res.Fields.ResourceCommand.Shortcuts) as string;

			

			this.ignoreChange = iic;
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

		
		protected IconButton					primaryAspectIcon;
		protected IconButton					primaryAspectDialog;
		protected CheckButton					primaryStatefull;
		protected ShortcutEditor				primaryShortcut1;
		protected ShortcutEditor				primaryShortcut2;
		protected ShortcutEditor				secondaryShortcut1;
		protected ShortcutEditor				secondaryShortcut2;
		protected TextFieldCombo				primaryGroup;
		protected StaticText					primarySuiteSummary;
		protected IconButton					primarySuiteSummaryIcon;
		protected StaticText					secondarySuiteSummary;
		protected IconButton					secondarySuiteSummaryIcon;
	}
}
