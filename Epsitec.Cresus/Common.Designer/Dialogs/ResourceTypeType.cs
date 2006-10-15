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
				this.WindowInit("TypeType", 500, 200, true);
				this.window.Text = Res.Strings.Dialog.TypeType.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(195, 200);
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
				this.checkCSharp.Text = Res.Strings.Dialog.TypeType.EnumCSharp;
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

				this.optionsExtend = new GlyphButton(footer);
				this.optionsExtend.PreferredWidth = 16;
				this.optionsExtend.ButtonStyle = ButtonStyle.Slider;
				this.optionsExtend.AutoFocus = false;
				this.optionsExtend.TabNavigation = Widget.TabNavigationMode.Passive;
				this.optionsExtend.Dock = DockStyle.Left;
				this.optionsExtend.Margins = new Margins(6, 0, 3, 3);
				this.optionsExtend.Clicked += new MessageEventHandler(this.HandleOptionsExtendClicked);
				ToolTip.Default.SetToolTip(this.optionsExtend, Res.Strings.Dialog.TypeType.Tooltip.Options);

				this.UpdateExtended();
			}

			this.UpdateRadios();
			this.UpdateEnumList();

			this.window.ShowDialog();
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

		public System.Type SystemType
		{
			get
			{
				if (this.type == ResourceAccess.TypeType.Enum && this.checkCSharp.ActiveState == ActiveState.Yes && this.enumList.SelectedIndex != -1)
				{
					return this.systemTypes[this.enumList.SelectedIndex];
				}
				else
				{
					return null;
				}
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

		protected void UpdateExtended()
		{
			this.optionsExtend.GlyphShape = this.isExtentended ? GlyphShape.ArrowLeft : GlyphShape.ArrowRight;
			this.rightPanel.Visibility = this.isExtentended;

			Size size = this.window.ClientSize;
			size.Width = this.isExtentended ? 500 : 195;
			this.window.ClientSize = size;
		}

		protected void UpdateFilter()
		{
			//	Met à jour la liste des filtres.
			this.filters = new List<string>();

			System.Type[] types = Collection.ToArray(EnumLister.GetPublicEnums());
			foreach (System.Type type in types)
			{
				string name = type.FullName;
				if (name.StartsWith(ResourceTypeType.filterPrefix))
				{
					name = name.Substring(ResourceTypeType.filterPrefix.Length);
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
			this.fieldFilter.Items.Add(Res.Strings.Dialog.TypeType.EnumAll);
			foreach (string filter in this.filters)
			{
				this.fieldFilter.Items.Add(string.Format(Res.Strings.Dialog.TypeType.EnumOne, filter));
			}
		}

		protected void UpdateEnumList()
		{
			//	Met à jour la liste des énumérations C# en fonction du filtre.
			string filter = null;
			if (this.fieldFilter.SelectedIndex > 0)  // pas "tout montrer" ?
			{
				filter = this.filters[this.fieldFilter.SelectedIndex-1];
			}

			this.systemTypes = new List<System.Type>();
			this.enumList.Items.Clear();
			System.Type[] types = Collection.ToArray(EnumLister.GetPublicEnums());
			foreach (System.Type type in types)
			{
				string name = type.FullName;
				if (name.StartsWith(ResourceTypeType.filterPrefix))
				{
					name = name.Substring(ResourceTypeType.filterPrefix.Length);
					if (filter == null || name.StartsWith(filter))
					{
						this.systemTypes.Add(type);
						this.enumList.Items.Add(TextLayout.ConvertToTaggedText(name));
					}
				}
			}
		}


		private void HandleRadioButtonActiveStateChanged(object sender)
		{
			//	Bouton radio cliqué.
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
			//	Bouton C# cliqué.
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

		private void HandleOptionsExtendClicked(object sender, MessageEventArgs e)
		{
			this.isExtentended = !this.isExtentended;
			this.UpdateExtended();
		}


		protected static string					filterPrefix = "Epsitec.Common.";

		protected ResourceAccess.TypeType		type;
		protected List<string>					filters;
		protected List<System.Type>				systemTypes;

		protected Widget						leftPanel;
		protected List<RadioButton>				radioButtons;
		protected GlyphButton					optionsExtend;

		protected Widget						rightPanel;
		protected CheckButton					checkCSharp;
		protected TextFieldCombo				fieldFilter;
		protected ScrollList					enumList;
		protected bool							isExtentended;
		
		protected int							index;
		protected int							tabIndex;
	}
}
