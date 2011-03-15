using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant d'éditer les paramètres d'une entité.
	/// </summary>
	public class EntityParametersDialog : AbstractDialog
	{
		public EntityParametersDialog(DesignerApplication designerApplication)
			: base (designerApplication)
		{
			this.titleEntity = "Exemple";
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if (this.window == null)
			{
				this.window = new Window();
				this.window.Icon = this.designerApplication.Icon;
				this.window.MakeSecondaryWindow ();
				this.window.PreventAutoClose = true;
				this.WindowInit ("EntityParameters", 400, 300, true);
				this.window.Text = Res.Strings.Dialog.EntityParameters.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.MinSize = new Size(200, 150);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				this.CreateUI (this.window.Root);

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonCancel = new Button(footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += this.HandleButtonCloseClicked;
				this.buttonCancel.TabIndex = 11;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonOk = new Button(footer);
				this.buttonOk.PreferredWidth = 75;
				this.buttonOk.Text = Res.Strings.Dialog.Button.OK;
				this.buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonOk.Dock = DockStyle.Right;
				this.buttonOk.Margins = new Margins(0, 6, 0, 0);
				this.buttonOk.Clicked += this.HandleButtonOkClicked;
				this.buttonOk.TabIndex = 10;
				this.buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.Update ();

			this.window.ShowDialog();
		}

		public void CreateUI(Widget parent)
		{
			var container = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			var mainPane = new FrameBox
			{
				Parent = container,
				PreferredWidth = 200,
				Dock = DockStyle.Left,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Margins = new Margins (0, 0, 2, 10),
			};

			var topPane = new GroupBox
			{
				Parent = mainPane,
				Text = Common.Types.Res.Types.StructuredTypeFlags.Caption.DefaultLabel,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 4),
				Padding = new Margins (8),
			};

			var bottomPane = new GroupBox
			{
				Parent = mainPane,
				Text = Common.Types.Res.Types.DataLifetimeExpectancy.Caption.DefaultLabel,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 4, 0),
				Padding = new Margins (8),
			};

			//	Rempli la colonne de gauche.
			this.abstractClassButton      = this.CreateCheckButton (topPane, Common.Types.Res.Values.StructuredTypeFlags.AbstractClass.DefaultLabel,      Types.StructuredTypeFlags.AbstractClass);
			this.generateSchemaButton     = this.CreateCheckButton (topPane, Common.Types.Res.Values.StructuredTypeFlags.GenerateSchema.DefaultLabel,     Types.StructuredTypeFlags.GenerateSchema);
			this.generateRepositoryButton = this.CreateCheckButton (topPane, Common.Types.Res.Values.StructuredTypeFlags.GenerateRepository.DefaultLabel, Types.StructuredTypeFlags.GenerateRepository);
			this.standaloneDisplayButton  = this.CreateCheckButton (topPane, Common.Types.Res.Values.StructuredTypeFlags.StandaloneDisplay.DefaultLabel,  Types.StructuredTypeFlags.StandaloneDisplay);
			this.standaloneCreationButton = this.CreateCheckButton (topPane, Common.Types.Res.Values.StructuredTypeFlags.StandaloneCreate.DefaultLabel,   Types.StructuredTypeFlags.StandaloneCreation);

			//	Rempli la colonne de droite.
			this.unknownButton = new RadioButton
			{
				Parent = bottomPane,
				Text = Common.Types.Res.Values.DataLifetimeExpectancy.Unknown.DefaultLabel,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			this.unknownButton.Clicked += delegate
			{
				this.DataLifetimeExpectancy = Types.DataLifetimeExpectancy.Unknown;
				this.UpdateWidgets ();
			};

			this.volatileButton = new RadioButton
			{
				Parent = bottomPane,
				Text = Common.Types.Res.Values.DataLifetimeExpectancy.Volatile.DefaultLabel,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			this.volatileButton.Clicked += delegate
			{
				this.DataLifetimeExpectancy = Types.DataLifetimeExpectancy.Volatile;
				this.UpdateWidgets ();
			};

			this.stableButton = new RadioButton
			{
				Parent = bottomPane,
				Text = Common.Types.Res.Values.DataLifetimeExpectancy.Stable.DefaultLabel,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			this.stableButton.Clicked += delegate
			{
				this.DataLifetimeExpectancy = Types.DataLifetimeExpectancy.Stable;
				this.UpdateWidgets ();
			};

			this.immutableButton = new RadioButton
			{
				Parent = bottomPane,
				Text = Common.Types.Res.Values.DataLifetimeExpectancy.Immutable.DefaultLabel,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			this.immutableButton.Clicked += delegate
			{
				this.DataLifetimeExpectancy = Types.DataLifetimeExpectancy.Immutable;
				this.UpdateWidgets ();
			};

			//	Crée l'exemple.
			this.entitySample = new MyWidgets.EntitySample
			{
				Parent = container,
				Dock = DockStyle.Fill,
			};
		}

		private CheckButton CreateCheckButton(Widget parent, string text, StructuredTypeFlags flag)
		{
			var button = new CheckButton
			{
				Parent = parent,
				Text = text,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			button.Clicked += delegate
			{
				//	Met ou enlève le flag correspondant au bouton.
				this.StructuredTypeFlags ^= flag;

				//	Gère les contraintes.
				if (flag == Types.StructuredTypeFlags.StandaloneDisplay)
				{
					if ((this.StructuredTypeFlags & Types.StructuredTypeFlags.StandaloneDisplay) == 0)
					{
						this.StructuredTypeFlags &= ~Types.StructuredTypeFlags.StandaloneCreation;
					}
				}

				if (flag == Types.StructuredTypeFlags.StandaloneCreation)
				{
					if ((this.StructuredTypeFlags & Types.StructuredTypeFlags.StandaloneCreation) != 0)
					{
						this.StructuredTypeFlags |= Types.StructuredTypeFlags.StandaloneDisplay;
					}
				}

				this.UpdateWidgets ();
			};

			return button;
		}

		public void Update()
		{
			this.isEditOk = false;
			this.closed = false;

			this.UpdateWidgets ();
		}

		public string TitleEntity
		{
			get
			{
				return this.titleEntity;
			}
			set
			{
				this.titleEntity = value;
				this.UpdateSample ();
			}
		}

		public bool IsEditOk
		{
			get
			{
				return this.isEditOk;
			}
		}

		public EntitiesEditor.ObjectBox ObjectBox
		{
			get;
			set;
		}

		public DataLifetimeExpectancy DataLifetimeExpectancy
		{
			get;
			set;
		}

		public StructuredTypeFlags StructuredTypeFlags
		{
			get;
			set;
		}


		private void UpdateWidgets()
		{
			this.abstractClassButton.ActiveState      = ((this.StructuredTypeFlags & Types.StructuredTypeFlags.AbstractClass     ) != 0) ? ActiveState.Yes : ActiveState.No;
			this.generateSchemaButton.ActiveState     = ((this.StructuredTypeFlags & Types.StructuredTypeFlags.GenerateSchema    ) != 0) ? ActiveState.Yes : ActiveState.No;
			this.generateRepositoryButton.ActiveState = ((this.StructuredTypeFlags & Types.StructuredTypeFlags.GenerateRepository) != 0) ? ActiveState.Yes : ActiveState.No;
			this.standaloneDisplayButton.ActiveState  = ((this.StructuredTypeFlags & Types.StructuredTypeFlags.StandaloneDisplay ) != 0) ? ActiveState.Yes : ActiveState.No;
			this.standaloneCreationButton.ActiveState = ((this.StructuredTypeFlags & Types.StructuredTypeFlags.StandaloneCreation) != 0) ? ActiveState.Yes : ActiveState.No;

			this.unknownButton.ActiveState   = (this.DataLifetimeExpectancy == Types.DataLifetimeExpectancy.Unknown  ) ? ActiveState.Yes : ActiveState.No;
			this.volatileButton.ActiveState  = (this.DataLifetimeExpectancy == Types.DataLifetimeExpectancy.Volatile ) ? ActiveState.Yes : ActiveState.No;
			this.stableButton.ActiveState    = (this.DataLifetimeExpectancy == Types.DataLifetimeExpectancy.Stable   ) ? ActiveState.Yes : ActiveState.No;
			this.immutableButton.ActiveState = (this.DataLifetimeExpectancy == Types.DataLifetimeExpectancy.Immutable) ? ActiveState.Yes : ActiveState.No;

			this.UpdateSample ();
		}

		private void UpdateSample()
		{
			if (this.ObjectBox == null)
			{
				this.entitySample.Title = this.titleEntity;
			}
			else
			{
				this.entitySample.Title     = this.ObjectBox.Title;
				this.entitySample.Subtitle  = this.ObjectBox.Subtitle;
				this.entitySample.MainColor = this.ObjectBox.BackgroundMainColor;
			}

			this.entitySample.DataLifetimeExpectancy = this.DataLifetimeExpectancy;
			this.entitySample.StructuredTypeFlags    = this.StructuredTypeFlags;
		}


		public void Close()
		{
			if (this.closed)
			{
				return;
			}

			if (this.buttonOk != null)  // mode "dialogue" (par opposition au mode "volet") ?
			{
				this.parentWindow.MakeActive ();
				this.window.Hide ();
				this.OnClosed ();
			}

			this.closed = true;
		}


		private void HandleWindowCloseClicked(object sender)
		{
			this.Close();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.Close ();
		}

		private void HandleButtonOkClicked(object sender, MessageEventArgs e)
		{
			this.Close ();
			this.isEditOk = true;
		}


		private bool							isEditOk;
		private bool							closed;
		private string							titleEntity;

		private CheckButton						abstractClassButton;
		private CheckButton						generateSchemaButton;
		private CheckButton						generateRepositoryButton;
		private CheckButton						standaloneDisplayButton;
		private CheckButton						standaloneCreationButton;

		private RadioButton						unknownButton;
		private RadioButton						volatileButton;
		private RadioButton						stableButton;
		private RadioButton						immutableButton;

		private MyWidgets.EntitySample			entitySample;

		private Button							buttonOk;
		private Button							buttonCancel;
	}
}
