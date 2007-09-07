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
		public Commands2(Module module, PanelsContext context, ResourceAccess access, DesignerApplication designerApplication) : base(module, context, access, designerApplication)
		{
			MyWidgets.StackedPanel leftContainer, rightContainer;

			//	Séparateur.
			this.CreateBand(out leftContainer, "", BandMode.Separator, GlyphShape.None, false, 0.0);

			//	Aspect (pour DefaultParameter) et Statefull.
			this.buttonSuiteCompact = this.CreateBand(out leftContainer, Res.Strings.Viewers.Commands.Statefull.Title, BandMode.SuiteView, GlyphShape.ArrowUp, true, 0.1);
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
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Commands.Shortcut.Title, BandMode.SuiteView, GlyphShape.None, false, 0.1);

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
			this.CreateBand(out leftContainer, Res.Strings.Viewers.Commands.Group.Title, BandMode.SuiteView, GlyphShape.None, false, 0.1);

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
			this.buttonSuiteExtend = this.CreateBand(out leftContainer, out rightContainer, "Résumé", BandMode.SuiteSummary, GlyphShape.ArrowDown, true, 0.1);
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
				return ResourceAccess.Type.Commands;
			}
		}


		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			base.UpdateEdit();

			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			this.primaryAspectIcon.Enable = !this.designerApplication.IsReadonly;
			this.primaryAspectDialog.Enable = !this.designerApplication.IsReadonly;
			this.primaryStatefull.Enable = !this.designerApplication.IsReadonly;
			this.primaryGroup.Enable = !this.designerApplication.IsReadonly;

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			StructuredData data = null;
			IList<StructuredData> shortcuts;

			if (item != null)
			{
				data = item.GetCultureData(this.GetTwoLetters(0));
			}

			if (data == null || this.designerApplication.IsReadonly)
			{
				this.primaryAspectIcon.ActiveState = ActiveState.No;
				this.primaryAspectDialog.ActiveState = ActiveState.No;
				this.primaryStatefull.ActiveState = ActiveState.No;
				this.SetShortcut(this.primaryShortcut1, this.primaryShortcut2, null);
				this.primaryGroup.Text = "";
			}
			else
			{
				string dp = data.GetValue(Support.Res.Fields.ResourceCommand.DefaultParameter) as string;
				this.primaryAspectIcon.ActiveState = (dp == "IconButton" || string.IsNullOrEmpty(dp)) ? ActiveState.Yes : ActiveState.No;
				this.primaryAspectDialog.ActiveState = (dp == "DialogButton") ? ActiveState.Yes : ActiveState.No;

				bool statefull = false;
				object value = data.GetValue(Support.Res.Fields.ResourceCommand.Statefull);
				if (!UndefinedValue.IsUndefinedValue(value))
				{
					statefull = (bool) value;
				}
				this.primaryStatefull.ActiveState = statefull ? ActiveState.Yes : ActiveState.No;

				shortcuts = data.GetValue(Support.Res.Fields.ResourceCommand.Shortcuts) as IList<StructuredData>;
				this.SetShortcut(this.primaryShortcut1, this.primaryShortcut2, shortcuts);

				string group = data.GetValue(Support.Res.Fields.ResourceCommand.Group) as string;
				this.primaryGroup.Text = group;
			}

			if (item == null || this.GetTwoLetters(1) == null || this.designerApplication.IsReadonly)
			{
				this.SetShortcut(this.secondaryShortcut1, this.secondaryShortcut2, null);
			}
			else
			{
				data = item.GetCultureData(this.GetTwoLetters(1));
				shortcuts = data.GetValue(Support.Res.Fields.ResourceCommand.Shortcuts) as IList<StructuredData>;
				this.SetShortcut(this.secondaryShortcut1, this.secondaryShortcut2, shortcuts);
			}

			//	Met à jour le résumé en fonction des widgets éditables.
			this.primarySuiteSummary.Text = this.GetSuiteSummary(this.primaryShortcut1, this.primaryShortcut2, this.primaryGroup);
			this.secondarySuiteSummary.Text = this.GetSuiteSummary(this.secondaryShortcut1, this.secondaryShortcut2, this.primaryGroup);

			string icon = null;
			if (this.primaryAspectIcon.ActiveState == ActiveState.Yes)
			{
				icon = Misc.Icon("ButtonAspectIcon");
			}
			if (this.primaryAspectDialog.ActiveState == ActiveState.Yes)
			{
				icon = Misc.Icon("ButtonAspectDialog");
			}
			this.primarySuiteSummaryIcon.IconName = icon;
			this.secondarySuiteSummaryIcon.IconName = icon;

			this.ignoreChange = iic;
		}

		protected string GetSuiteSummary(ShortcutEditor editor1, ShortcutEditor editor2, TextFieldCombo group)
		{
			//	Retourne le texte de la 2ème partie du résumé en fonction des widgets éditables.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			string s;

			s = editor1.Shortcut.ToString();
			if (!string.IsNullOrEmpty(s))
			{
				builder.Append(s);
			}
			
			s = editor2.Shortcut.ToString();
			if (!string.IsNullOrEmpty(s))
			{
				if (builder.Length > 0)
				{
					builder.Append(", ");
				}
				builder.Append(s);
			}

			s = group.Text;
			if (!string.IsNullOrEmpty(s))
			{
				if (builder.Length > 0)
				{
					builder.Append("<br/>");
				}
				builder.Append(s);
			}

			return builder.ToString();
		}

		protected void SetShortcut(ShortcutEditor editor1, ShortcutEditor editor2, IList<StructuredData> shortcuts)
		{
			if (shortcuts == null)
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

				if (shortcuts.Count < 1)
				{
					editor1.Shortcut = new Shortcut(KeyCode.None);
				}
				else
				{
					string keyCode = shortcuts[0].GetValue(Support.Res.Fields.Shortcut.KeyCode) as string;
					editor1.Shortcut = (KeyCode) System.Enum.Parse(typeof(KeyCode), keyCode);
				}

				if (shortcuts.Count < 2)
				{
					editor2.Shortcut = new Shortcut(KeyCode.None);
				}
				else
				{
					string keyCode = shortcuts[1].GetValue(Support.Res.Fields.Shortcut.KeyCode) as string;
					editor2.Shortcut = (KeyCode) System.Enum.Parse(typeof(KeyCode), keyCode);
				}
			}
		}

		protected void UpdateGroupCombo()
		{
			//	Met à jour le menu du combo des groupes.
			List<string> list = new List<string>();

			foreach (CultureMap item in this.access.Accessor.Collection)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(0));
				string group = data.GetValue(Support.Res.Fields.ResourceCommand.Group) as string;

				if (!string.IsNullOrEmpty(group) && !list.Contains(group))
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

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			if (item == null)
			{
				return;
			}

			StructuredData data = item.GetCultureData(this.GetTwoLetters(0));
			data.SetValue(Support.Res.Fields.ResourceCommand.DefaultParameter, defaultParameter);

			this.access.SetLocalDirty();
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

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			if (item == null)
			{
				return;
			}

			StructuredData data = item.GetCultureData(this.GetTwoLetters(0));
			data.SetValue(Support.Res.Fields.ResourceCommand.Statefull, statefull);

			this.access.SetLocalDirty();
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
			int row = -1;
			int rank = -1;

			if (editor == this.primaryShortcut1)
			{
				row = 0;  // culture principale
				rank = 0;  // raccourci principal
				editor1 = this.primaryShortcut1;
				editor2 = this.primaryShortcut2;
			}

			if (editor == this.primaryShortcut2)
			{
				row = 0;  // culture principale
				rank = 1;  // raccourci supplémentaire
				editor1 = this.primaryShortcut1;
				editor2 = this.primaryShortcut2;
			}

			if (editor == this.secondaryShortcut1)
			{
				row = 1;  // culture secondaire
				rank = 0;  // raccourci principal
				editor1 = this.secondaryShortcut1;
				editor2 = this.secondaryShortcut2;
			}

			if (editor == this.secondaryShortcut2)
			{
				row = 1;  // culture secondaire
				rank = 1;  // raccourci supplémentaire
				editor1 = this.secondaryShortcut1;
				editor2 = this.secondaryShortcut2;
			}

			System.Diagnostics.Debug.Assert(row != -1 && rank != -1);

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			if (item == null)
			{
				return;
			}

			StructuredData data = item.GetCultureData(this.GetTwoLetters(row));
			IList<StructuredData> shortcuts = data.GetValue(Support.Res.Fields.ResourceCommand.Shortcuts) as IList<StructuredData>;

			IDataBroker shortcutBroker = this.access.Accessor.GetDataBroker(data, Support.Res.Fields.ResourceCommand.Shortcuts.ToString());
			
			string sc = editor.Shortcut.KeyCode.ToString();
			StructuredData scData = shortcutBroker.CreateData(item);
			scData.SetValue(Support.Res.Fields.Shortcut.KeyCode, sc);

			if (rank == 0)  // raccourci principal ?
			{
				if (shortcuts.Count == 0)
				{
					shortcuts.Add(scData);  // insère le raccourci principal
				}
				else
				{
					shortcuts[rank] = scData;
				}
			}

			if (rank == 1)  // raccourci supplémentaire ?
			{
				if (shortcuts.Count == 0)
				{
					StructuredData scNone = shortcutBroker.CreateData(item);
					scNone.SetValue(Support.Res.Fields.Shortcut.KeyCode, "None");
					shortcuts.Add(scNone);  // insère un raccourci principal inexistant

					shortcuts.Add(scData);  // insère le raccourci supplémentaire
				}
				else if (shortcuts.Count == 1)
				{
					shortcuts.Add(scData);  // insère le raccourci supplémentaire
				}
				else
				{
					shortcuts[rank] = scData;
				}
			}

			//	Supprime tous les raccourcis inexistant KeyCode.None de la collection.
			int i = 0;
			bool removed = false;
			while (i < shortcuts.Count)
			{
				string keyCode = shortcuts[i].GetValue(Support.Res.Fields.Shortcut.KeyCode) as string;
				KeyCode kc = (KeyCode) System.Enum.Parse(typeof(KeyCode), keyCode);

				if (kc == KeyCode.None)
				{
					shortcuts.RemoveAt(i);
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
				this.SetShortcut(editor1, editor2, shortcuts);
				this.ignoreChange = false;
			}

			this.access.SetLocalDirty();
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

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			if (item == null)
			{
				return;
			}

			if (edit == this.primaryGroup)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(0));
				this.SetValue(item, data, Support.Res.Fields.ResourceCommand.Group, text, true);
			}

			this.access.SetLocalDirty();
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
