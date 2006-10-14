using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir le type d'un Caption.Type.
	/// </summary>
	public class ResourceTypeType : Abstract
	{
		public ResourceTypeType(MainWindow mainWindow) : base(mainWindow)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("TypeType", 500, 250, true);
				this.window.Text = Res.Strings.Dialog.TypeType.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(220, 190);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				Widget main = new Widget(this.window.Root);
				main.Dock = DockStyle.Fill;

				this.leftPanel = new Widget(main);
				this.leftPanel.PreferredWidth = 140;
				this.leftPanel.Dock = DockStyle.Left;

				this.rightPanel = new Widget(main);
				this.rightPanel.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				this.index = 0;
				this.radioButtons = new List<RadioButton>();
				this.CreateRadio(Res.Captions.Types.Type.Void);
				this.CreateRadio(Res.Captions.Types.Type.Boolean);
				this.CreateRadio(Res.Captions.Types.Type.Integer);
				this.CreateRadio(Res.Captions.Types.Type.LongInteger);
				this.CreateRadio(Res.Captions.Types.Type.Decimal);
				this.CreateRadio(Res.Captions.Types.Type.String);
				this.CreateRadio(Res.Captions.Types.Type.Enum);
				this.CreateRadio(Res.Captions.Types.Type.Structured);
				this.tabIndex++;

				this.checkCSharp = new CheckButton(this.rightPanel);
				this.checkCSharp.Text = "Enumération C#";
				this.checkCSharp.Margins = new Margins(0, 0, 0, 8);
				this.checkCSharp.Dock = DockStyle.Top;
				this.checkCSharp.ActiveStateChanged += new EventHandler(this.HandleCheckCSharpActiveStateChanged);

				this.fieldFilter = new TextFieldCombo(this.rightPanel);
				this.fieldFilter.Text = Res.Strings.Dialog.Icon.Filter.All;
				this.fieldFilter.IsReadOnly = true;
				this.fieldFilter.Margins = new Margins(0, 0, 0, 8);
				this.fieldFilter.Dock = DockStyle.Top;
				this.fieldFilter.ComboClosed += new EventHandler(this.HandleFieldFilterComboClosed);
				this.UpdateFilter();

				this.enumList = new ScrollList(this.rightPanel);
				this.enumList.Dock = DockStyle.Fill;

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				Button buttonOk = new Button(footer);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.Button.OK;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Dock = DockStyle.Left;
				buttonOk.Margins = new Margins(0, 6, 0, 0);
				buttonOk.Clicked += new MessageEventHandler(this.HandleButtonOKClicked);
				buttonOk.TabIndex = this.tabIndex++;
				buttonOk.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonClose = new Button(footer);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Cancel;
				buttonClose.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonClose.Dock = DockStyle.Left;
				buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				buttonClose.TabIndex = this.tabIndex++;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.UpdateRadios();
			this.UpdateEnumList();

			this.window.ShowDialog();
		}

		public void SetResourceManager(ResourceManager manager)
		{
			this.manager = manager;
		}

		public ResourceAccess.TypeType ContentType
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}


		protected void CreateRadio(Caption caption)
		{
			//	Crée un bouton radio.
			RadioButton button = new RadioButton(this.leftPanel);
			button.CaptionDruid = caption.Druid;
			System.Diagnostics.Debug.Assert(button.Name.StartsWith("Types.Type."));
			button.Dock = DockStyle.Top;
			button.Margins = new Margins(0, 0, 2, 2);
			button.Index = this.index++;
			button.TabIndex = this.tabIndex;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			button.ActiveStateChanged += new EventHandler(this.HandleRadioButtonActiveStateChanged);

			this.radioButtons.Add(button);
		}

		protected void UpdateRadios()
		{
			//	Met à jour le bouton radio enfoncé en fonction du type.
			string actual = ResourceAccess.ConvTypeType(this.type);

			this.ignoreChanged = true;
			foreach (RadioButton button in this.radioButtons)
			{
				string name = button.Name;
				name = name.Substring(name.LastIndexOf('.')+1);  // enlève "Res.Captions.Types.Type."

				button.ActiveState = (name == actual) ? ActiveState.Yes : ActiveState.No;

				if (name == actual)
				{
					button.Focus();
				}
			}
			this.ignoreChanged = false;

			this.checkCSharp.Enable = (this.type == ResourceAccess.TypeType.Enum);
			this.fieldFilter.Enable = (this.type == ResourceAccess.TypeType.Enum && this.checkCSharp.ActiveState == ActiveState.Yes);
			this.enumList.Enable    = (this.type == ResourceAccess.TypeType.Enum && this.checkCSharp.ActiveState == ActiveState.Yes);
		}

		protected void UpdateFilter()
		{
			this.filters = new List<string>();

			System.Type[] types = Collection.ToArray(EnumLister.GetPublicEnums());
			foreach (System.Type type in types)
			{
				string name = type.FullName;
				if (name.StartsWith(ResourceTypeType.fix))
				{
					name = name.Substring(ResourceTypeType.fix.Length);
					int i = name.IndexOf('.');
					if (i != -1)
					{
						string prefix = name.Substring(0, i);
						if (!this.filters.Contains(prefix))
						{
							this.filters.Add(prefix);
						}
					}
				}
			}

			this.fieldFilter.Items.Clear();
			this.fieldFilter.Items.Add("Tout montrer");
			foreach (string filter in this.filters)
			{
				this.fieldFilter.Items.Add(string.Format("Seulement {0}", filter));
			}
		}

		protected void UpdateEnumList()
		{
			string filter = null;

			if (this.fieldFilter.SelectedIndex > 0)
			{
				filter = this.filters[this.fieldFilter.SelectedIndex-1];
			}

			this.enumList.Items.Clear();
			System.Type[] types = Collection.ToArray(EnumLister.GetPublicEnums());
			foreach (System.Type type in types)
			{
				string name = type.FullName;
				if (name.StartsWith(ResourceTypeType.fix))
				{
					name = name.Substring(ResourceTypeType.fix.Length);
					if (filter == null || name.StartsWith(filter))
					{
						this.enumList.Items.Add(TextLayout.ConvertToTaggedText(name));
					}
				}
			}
		}


		private void HandleRadioButtonActiveStateChanged(object sender)
		{
			if (this.ignoreChanged)
			{
				return;
			}

			RadioButton button = sender as RadioButton;
			string name = button.Name;
			name = name.Substring(name.LastIndexOf('.')+1);  // enlève "Res.Captions.Types.Type."
			this.type = ResourceAccess.ConvTypeType(name);
			this.UpdateRadios();
		}

		private void HandleCheckCSharpActiveStateChanged(object sender)
		{
			this.UpdateRadios();
		}

		private void HandleFieldFilterComboClosed(object sender)
		{
			//	Menu pour choisir le filtre fermé.
			this.UpdateEnumList();
		}

		private void HandleWindowCloseClicked(object sender)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.type = ResourceAccess.TypeType.None;

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonOKClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}


		protected static string					fix = "Epsitec.Common.";

		protected ResourceManager				manager;
		protected ResourceAccess.TypeType		type;
		protected List<string>					filters;

		protected Widget						leftPanel;
		protected List<RadioButton>				radioButtons;

		protected Widget						rightPanel;
		protected CheckButton					checkCSharp;
		protected TextFieldCombo				fieldFilter;
		protected ScrollList					enumList;
		
		protected int							index;
		protected int							tabIndex;
	}
}
