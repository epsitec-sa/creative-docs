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
	public class ResourceTypeCode : Abstract
	{
		public ResourceTypeCode(DesignerApplication designerApplication) : base(designerApplication)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window ();
				this.window.Icon = this.designerApplication.Icon;
				this.window.MakeSecondaryWindow ();
				this.window.PreventAutoClose = true;
				this.WindowInit ("TypeCode", ResourceTypeCode.windowWidthExtended, ResourceTypeCode.windowHeight, true);
				this.window.Text = Res.Strings.Dialog.TypeCode.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.MinSize = new Size (ResourceTypeCode.windowWidthCompacted, ResourceTypeCode.windowHeight);
				this.window.Root.Padding = new Margins (8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob (this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins (0, -8, 0, -8);
				ToolTip.Default.SetToolTip (resize, Res.Strings.Dialog.Tooltip.Resize);

				//	Partie principale.
				Widget main = new Widget (this.window.Root);
				main.Dock = DockStyle.Fill;

				this.leftPanel = new Widget (main);
				this.leftPanel.PreferredWidth = 140;
				this.leftPanel.Dock = DockStyle.Left;

				this.rightPanel = new Widget (main);
				this.rightPanel.Dock = DockStyle.Fill;

				//	Partie gauche.
				this.tabIndex = 1;
				this.index = 0;
				this.radioButtons = new List<RadioButton> ();
				this.CreateRadio (Res.Captions.Types.Type.Boolean);
				this.CreateRadio (Res.Captions.Types.Type.Integer);
				this.CreateRadio (Res.Captions.Types.Type.LongInteger);
				this.CreateRadio (Res.Captions.Types.Type.Decimal);
				this.CreateRadio (Res.Captions.Types.Type.String);
				this.CreateRadio (Res.Captions.Types.Type.Date);
				this.CreateRadio (Res.Captions.Types.Type.Time);
				this.CreateRadio (Res.Captions.Types.Type.DateTime);
				this.CreateRadio (Res.Captions.Types.Type.Enum);
				this.CreateRadio (Res.Captions.Types.Type.Collection);
				this.CreateRadio (Res.Captions.Types.Type.Native);
				this.CreateRadio (Res.Captions.Types.Type.Binary);
				this.tabIndex++;

				//	Partie droite.
				this.checkNative = new CheckButton (this.rightPanel);
				this.checkNative.Text = Res.Strings.Dialog.TypeCode.EnumNative;
				this.checkNative.Margins = new Margins (0, 0, 0, 8);
				this.checkNative.Dock = DockStyle.Top;
				this.checkNative.ActiveStateChanged += this.HandleCheckNativeActiveStateChanged;

				this.comboFilterField = new TextFieldCombo (this.rightPanel);
				this.comboFilterField.Text = Res.Strings.Dialog.Icon.Filter.All;
				this.comboFilterField.IsReadOnly = true;
				this.comboFilterField.Margins = new Margins (0, 0, 0, 8);
				this.comboFilterField.Dock = DockStyle.Top;
				this.comboFilterField.ComboClosed += this.HandleComboFilterFieldClosed;
				this.UpdateFilter ();

				//	Bande horizontale pour la recherche.
				{
					this.filterController = new Controllers.FilterController ();

					this.filterFrame = this.filterController.CreateUI (this.rightPanel);
					this.filterFrame.Margins = new Margins (0, 0, 0, 8);
					this.filterFrame.TabIndex = tabIndex++;
					this.filterFrame.TabIndex = tabIndex++;
					
					this.filterController.FilterChanged += new EventHandler (this.HandleFilterControllerChanged);
				}

				this.enumList = new ScrollList (this.rightPanel);
				this.enumList.Dock = DockStyle.Fill;
				this.enumList.SelectionActivated += this.HandleEnumListSelectionActivated;

				//	Boutons de fermeture.
				Widget footer = new Widget (this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins (0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonCancel = new Button (footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += this.HandleButtonCancelClicked;
				this.buttonCancel.TabIndex = 101;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonOk = new Button (footer);
				this.buttonOk.PreferredWidth = 75;
				this.buttonOk.Text = Res.Strings.Dialog.Button.OK;
				this.buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonOk.Dock = DockStyle.Right;
				this.buttonOk.Margins = new Margins (0, 6, 0, 0);
				this.buttonOk.Clicked += this.HandleButtonOKClicked;
				this.buttonOk.TabIndex = 100;
				this.buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.UpdateRadios ();
			this.UpdateEnumList ();
			this.UpdateExtended ();
			this.UpdateButtons ();

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

		public TypeCode ContentType
		{
			//	Set: spécifie le type initial.
			//	Get: retourne le type choisi (None si le bouton Annuler est utilisé)
			set
			{
				this.typeEdited = value;
				this.typeAccepted = TypeCode.Invalid;
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
				if (this.typeAccepted == TypeCode.Enum && this.checkNative.ActiveState == ActiveState.Yes && this.enumList.SelectedItemIndex != -1)
				{
					return this.systemTypes[this.enumList.SelectedItemIndex];
				}
				else
				{
					return null;
				}
			}
		}


		private void CreateRadio(Caption caption)
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
			button.Clicked += new EventHandler<MessageEventArgs> (this.HandleRadioButtonClicked);

			this.radioButtons.Add(button);
		}

		private void UpdateRadios()
		{
			//	Met à jour le bouton radio enfoncé en fonction du type.
			string actual = ResourceAccess.TypeCodeToName(this.typeEdited);

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

			this.checkNative.Enable      = (this.typeEdited == TypeCode.Enum);
			this.filterFrame.Enable      = (this.typeEdited == TypeCode.Enum && this.checkNative.ActiveState == ActiveState.Yes);
			this.comboFilterField.Enable = (this.typeEdited == TypeCode.Enum && this.checkNative.ActiveState == ActiveState.Yes);
			this.enumList.Enable         = (this.typeEdited == TypeCode.Enum && this.checkNative.ActiveState == ActiveState.Yes);
		}

		private void UpdateExtended()
		{
			//	Met à jour le bouton pour montrer/cacher les options.
			bool isExtended = (this.typeEdited == TypeCode.Enum);

			this.rightPanel.Visibility = isExtended;

			Size size = this.window.ClientSize;
			size.Width = isExtended ? ResourceTypeCode.windowWidthExtended : ResourceTypeCode.windowWidthCompacted;
			this.window.ClientSize = size;
		}

		private void UpdateButtons()
		{
			//	Met à jour le bouton D'accord.
			bool enable = true;

			if (this.typeEdited == TypeCode.Enum && this.checkNative.ActiveState == ActiveState.Yes && this.enumList.SelectedItemIndex == -1)
			{
				enable = false;
			}

			this.buttonOk.Enable = enable;
		}

		private void UpdateFilter()
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

			this.comboFilterField.Items.Clear();
			this.comboFilterField.Items.Add(Res.Strings.Dialog.TypeCode.EnumAll);
			foreach (string filter in this.filters)
			{
				this.comboFilterField.Items.Add(string.Format(Res.Strings.Dialog.TypeCode.EnumOne, filter));
			}
		}

		private void UpdateEnumList()
		{
			//	Met à jour la liste des énumérations C# natives en fonction du filtre.
			string filter = null;
			if (this.comboFilterField.SelectedItemIndex > 0)  // pas "tout montrer" ?
			{
				filter = this.filters[this.comboFilterField.SelectedItemIndex-1];
			}

			this.systemTypes = new List<System.Type>();
			this.enumList.Items.Clear();

			foreach (System.Type stype in EnumLister.GetDesignerVisibleEnums())
			{
				string name = this.resourceAccess.GetEnumBaseName(stype);

				if (this.IsFiltered (name))
				{
					continue;
				}

				if (filter == null || name.StartsWith(filter))
				{
					if (this.resourceAccess.IsCorrectNewName(ref name))
					{
						this.systemTypes.Add(stype);
						this.enumList.Items.Add(TextLayout.ConvertToTaggedText(name));
					}
				}
			}
		}

		private bool IsFiltered(string name)
		{
			return this.filterController.IsFiltered (name);
		}


		private void HandleRadioButtonClicked(object sender, MessageEventArgs e)
		{
			//	Bouton radio cliqué.
			RadioButton button = sender as RadioButton;

			string name = button.Name;
			name = name.Substring (name.LastIndexOf ('.')+1);  // enlève "Res.Captions.Types.Type."
			this.typeEdited = ResourceAccess.NameToTypeCode (name);

			this.UpdateRadios ();
			this.UpdateExtended ();
			this.UpdateButtons ();
		}

		private void HandleCheckNativeActiveStateChanged(object sender)
		{
			//	Bouton C# native cliqué.
			this.UpdateRadios();
			this.UpdateButtons();
		}

		private void HandleFilterControllerChanged(object sender)
		{
			this.UpdateEnumList ();
			this.UpdateButtons ();
		}

		private void HandleComboFilterFieldClosed(object sender)
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


		private static readonly double			windowWidthCompacted = 8+75+6+75+8;
		private static readonly double			windowWidthExtended = 500;
		private static readonly double			windowHeight = 284;

		private ResourceAccess					resourceAccess;
		private TypeCode						typeEdited;
		private TypeCode						typeAccepted;
		private List<string>					filters;
		private List<System.Type>				systemTypes;
		private int								index;
		private int								tabIndex;

		private Widget							leftPanel;
		private List<RadioButton>				radioButtons;

		private Widget							rightPanel;
		private CheckButton						checkNative;
		private Controllers.FilterController	filterController;
		private FrameBox						filterFrame;
		private TextFieldCombo					comboFilterField;
		private ScrollList						enumList;

		private Button							buttonOk;
		private Button							buttonCancel;
	}
}
