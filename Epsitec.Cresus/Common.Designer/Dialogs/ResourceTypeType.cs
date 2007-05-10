using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir le type d'un Caption.Type.
	/// Pour les énumérations natives, il est possible d'étendre le dialogue
	/// et de choisir une énumération C# dans une liste.
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
				this.WindowInit("TypeType", ResourceTypeType.windowWidthExtended, ResourceTypeType.windowHeight, true);
				this.window.Text = Res.Strings.Dialog.TypeType.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(ResourceTypeType.windowWidthCompacted, ResourceTypeType.windowHeight);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				//	Partie principale.
				Widget main = new Widget(this.window.Root);
				main.Dock = DockStyle.Fill;

				this.leftPanel = new Widget(main);
				this.leftPanel.PreferredWidth = 140;
				this.leftPanel.Dock = DockStyle.Left;

				this.rightPanel = new Widget(main);
				this.rightPanel.Dock = DockStyle.Fill;

				//	Partie gauche.
				this.tabIndex = 0;
				this.index = 0;
				this.radioButtons = new List<RadioButton>();
				this.CreateRadio(Res.Captions.Types.Type.Boolean);
				this.CreateRadio(Res.Captions.Types.Type.Integer);
				this.CreateRadio(Res.Captions.Types.Type.LongInteger);
				this.CreateRadio(Res.Captions.Types.Type.Decimal);
				this.CreateRadio(Res.Captions.Types.Type.String);
				this.CreateRadio(Res.Captions.Types.Type.Date);
				this.CreateRadio(Res.Captions.Types.Type.Time);
				this.CreateRadio(Res.Captions.Types.Type.DateTime);
				this.CreateRadio(Res.Captions.Types.Type.Enum);
				this.CreateRadio(Res.Captions.Types.Type.Structured);
				this.CreateRadio(Res.Captions.Types.Type.Collection);
				this.CreateRadio(Res.Captions.Types.Type.Binary);
				this.tabIndex++;

				//	Partie droite.
				this.checkNative = new CheckButton(this.rightPanel);
				this.checkNative.Text = Res.Strings.Dialog.TypeType.EnumNative;
				this.checkNative.Margins = new Margins(0, 0, 0, 8);
				this.checkNative.Dock = DockStyle.Top;
				this.checkNative.ActiveStateChanged += new EventHandler(this.HandleCheckNativeActiveStateChanged);

				this.fieldFilter = new TextFieldCombo(this.rightPanel);
				this.fieldFilter.Text = Res.Strings.Dialog.Icon.Filter.All;
				this.fieldFilter.IsReadOnly = true;
				this.fieldFilter.Margins = new Margins(0, 0, 0, 8);
				this.fieldFilter.Dock = DockStyle.Top;
				this.fieldFilter.ComboClosed += new EventHandler(this.HandleFieldFilterComboClosed);
				this.UpdateFilter();

				this.enumList = new ScrollList(this.rightPanel);
				this.enumList.Dock = DockStyle.Fill;
				this.enumList.SelectionActivated += new EventHandler(this.HandleEnumListSelectionActivated);

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonOk = new Button(footer);
				this.buttonOk.PreferredWidth = 75;
				this.buttonOk.Text = Res.Strings.Dialog.Button.OK;
				this.buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonOk.Dock = DockStyle.Left;
				this.buttonOk.Margins = new Margins(0, 6, 0, 0);
				this.buttonOk.Clicked += new MessageEventHandler(this.HandleButtonOKClicked);
				this.buttonOk.TabIndex = this.tabIndex++;
				this.buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonCancel = new Button(footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Left;
				this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCancelClicked);
				this.buttonCancel.TabIndex = this.tabIndex++;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonExtend = new GlyphButton(footer);
				this.buttonExtend.PreferredWidth = 16;
				this.buttonExtend.ButtonStyle = ButtonStyle.Slider;
				this.buttonExtend.AutoFocus = false;
				this.buttonExtend.TabNavigationMode = TabNavigationMode.None;
				this.buttonExtend.Dock = DockStyle.Left;
				this.buttonExtend.Margins = new Margins(6, 0, 3, 3);
				this.buttonExtend.Clicked += new MessageEventHandler(this.HandleButtonExtendClicked);
				ToolTip.Default.SetToolTip(this.buttonExtend, Res.Strings.Dialog.TypeType.Tooltip.Options);

				this.UpdateExtended();
			}

			this.UpdateRadios();
			this.UpdateEnumList();
			this.UpdateButtons();

			this.window.ShowDialog();
		}

		public ResourceAccess ResourceAccess
		{
			set
			{
				this.resourceAccess = value;
			}
			get
			{
				return this.resourceAccess;
			}
		}

		public ResourceAccess.TypeType ContentType
		{
			//	Set: spécifie le type initial.
			//	Get: retourne le type choisi (None si le bouton Annuler est utilisé)
			set
			{
				this.typeEdited = value;
				this.typeAccepted = ResourceAccess.TypeType.None;
			}
			get
			{
				return this.typeAccepted;
			}
		}

		public System.Type SystemType
		{
			//	Retourne le Sytem.Type à utiliser, lors d'une énumération C# native.
			get
			{
				if (this.typeAccepted == ResourceAccess.TypeType.Enum && this.checkNative.ActiveState == ActiveState.Yes && this.enumList.SelectedIndex != -1)
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
			button.CaptionId = caption.Id;
			System.Diagnostics.Debug.Assert(button.Name.StartsWith("Types.Type."));
			button.Dock = DockStyle.Top;
			button.Margins = new Margins(0, 0, 2, 2);
			button.Index = this.index++;
			button.TabIndex = this.tabIndex;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			button.ActiveStateChanged += new EventHandler(this.HandleRadioButtonActiveStateChanged);

			this.radioButtons.Add(button);
		}

		protected void UpdateRadios()
		{
			//	Met à jour le bouton radio enfoncé en fonction du type.
			string actual = ResourceAccess.TypeTypeToName(this.typeEdited);

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

			this.checkNative.Enable = (this.typeEdited == ResourceAccess.TypeType.Enum);
			this.fieldFilter.Enable = (this.typeEdited == ResourceAccess.TypeType.Enum && this.checkNative.ActiveState == ActiveState.Yes);
			this.enumList.Enable    = (this.typeEdited == ResourceAccess.TypeType.Enum && this.checkNative.ActiveState == ActiveState.Yes);
		}

		protected void UpdateExtended()
		{
			//	Met à jour le bouton pour montrer/cacher les options.
			this.buttonExtend.GlyphShape = this.isExtentended ? GlyphShape.ArrowLeft : GlyphShape.ArrowRight;
			this.rightPanel.Visibility = this.isExtentended;

			Size size = this.window.ClientSize;
			size.Width = this.isExtentended ? ResourceTypeType.windowWidthExtended : ResourceTypeType.windowWidthCompacted;
			this.window.ClientSize = size;
		}

		protected void UpdateButtons()
		{
			//	Met à jour le bouton D'accord.
			bool enable = true;

			if (this.typeEdited == ResourceAccess.TypeType.Enum && this.checkNative.ActiveState == ActiveState.Yes && this.enumList.SelectedIndex == -1)
			{
				enable = false;
			}

			this.buttonOk.Enable = enable;
		}

		protected void UpdateFilter()
		{
			//	Met à jour la liste des filtres.
			this.filters = new List<string>();

			foreach (System.Type stype in EnumLister.GetDesignerVisibleEnums())
			{
				string name = this.resourceAccess.GetEnumBaseName(stype);
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

			this.fieldFilter.Items.Clear();
			this.fieldFilter.Items.Add(Res.Strings.Dialog.TypeType.EnumAll);
			foreach (string filter in this.filters)
			{
				this.fieldFilter.Items.Add(string.Format(Res.Strings.Dialog.TypeType.EnumOne, filter));
			}
		}

		protected void UpdateEnumList()
		{
			//	Met à jour la liste des énumérations C# natives en fonction du filtre.
			string filter = null;
			if (this.fieldFilter.SelectedIndex > 0)  // pas "tout montrer" ?
			{
				filter = this.filters[this.fieldFilter.SelectedIndex-1];
			}

			this.systemTypes = new List<System.Type>();
			this.enumList.Items.Clear();
			foreach (System.Type stype in EnumLister.GetDesignerVisibleEnums())
			{
				string name = this.resourceAccess.GetEnumBaseName(stype);
				if (filter == null || name.StartsWith(filter))
				{
					if (this.resourceAccess.IsCorrectNewName(ref name, false))
					{
						this.systemTypes.Add(stype);
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
			if (button.ActiveState == ActiveState.Yes)
			{
				string name = button.Name;
				name = name.Substring(name.LastIndexOf('.')+1);  // enlève "Res.Captions.Types.Type."
				this.typeEdited = ResourceAccess.NameToTypeType(name);
				this.UpdateRadios();
				this.UpdateButtons();
			}
		}

		private void HandleCheckNativeActiveStateChanged(object sender)
		{
			//	Bouton C# native cliqué.
			this.UpdateRadios();
			this.UpdateButtons();
		}

		private void HandleFieldFilterComboClosed(object sender)
		{
			//	Menu pour choisir le filtre fermé.
			this.UpdateEnumList();
			this.UpdateButtons();
		}

		private void HandleEnumListSelectionActivated(object sender)
		{
			//	Sélection changée dans la liste des énumérations C# natives.
			this.UpdateButtons();
		}

		private void HandleWindowCloseClicked(object sender)
		{
			//	Fenêtre fermée.
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonCancelClicked(object sender, MessageEventArgs e)
		{
			//	Bouton Annuler cliqué.
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonOKClicked(object sender, MessageEventArgs e)
		{
			//	Bouton D'accord cliqué.
			this.typeAccepted = this.typeEdited;

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonExtendClicked(object sender, MessageEventArgs e)
		{
			//	Bouton ">" ou "<" pour montrer/cacher les options cliqué.
			this.isExtentended = !this.isExtentended;
			this.UpdateExtended();
			this.UpdateButtons();
		}


		protected static readonly double		windowWidthCompacted = 195;
		protected static readonly double		windowWidthExtended = 500;
		protected static readonly double		windowHeight = 260;

		protected ResourceAccess				resourceAccess;
		protected ResourceAccess.TypeType		typeEdited;
		protected ResourceAccess.TypeType		typeAccepted;
		protected List<string>					filters;
		protected List<System.Type>				systemTypes;
		protected bool							isExtentended;
		protected int							index;
		protected int							tabIndex;

		protected Widget						leftPanel;
		protected List<RadioButton>				radioButtons;

		protected Widget						rightPanel;
		protected CheckButton					checkNative;
		protected TextFieldCombo				fieldFilter;
		protected ScrollList					enumList;

		protected Button						buttonOk;
		protected Button						buttonCancel;
		protected GlyphButton					buttonExtend;
	}
}
