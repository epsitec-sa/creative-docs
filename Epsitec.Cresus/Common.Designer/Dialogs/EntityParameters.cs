using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant d'�diter les param�tres d'une entit�.
	/// </summary>
	public class EntityParameters : Abstract
	{
		public EntityParameters(DesignerApplication designerApplication)
			: base (designerApplication)
		{
		}

		public override void Show()
		{
			//	Cr�e et montre la fen�tre du dialogue.
			this.isEditOk = false;

			if (this.window == null)
			{
				this.window = new Window();
				this.window.Icon = this.designerApplication.Icon;
				this.window.MakeSecondaryWindow ();
				this.window.PreventAutoClose = true;
				this.WindowInit ("EntityParameters", 400, 150, true);
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
			var mainPane = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			var leftPane = new GroupBox
			{
				Parent = mainPane,
				Text = Common.Types.Res.Types.StructuredTypeFlags.Caption.DefaultLabel,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 4, 0, 0),
				Padding = new Margins (8),
			};

			var rightPane = new GroupBox
			{
				Parent = mainPane,
				Text = Common.Types.Res.Types.DataLifetimeExpectancy.Caption.DefaultLabel,
				Dock = DockStyle.Fill,
				Margins = new Margins (4, 0, 0, 0),
				Padding = new Margins (8),
			};

			//	Rempli la colonne de gauche.
			this.abstractClassButton = new CheckButton
			{
				Parent = leftPane,
				Text = Common.Types.Res.Values.StructuredTypeFlags.AbstractClass.DefaultLabel,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			this.abstractClassButton.Clicked += delegate
			{
				this.StructuredTypeFlags ^= Types.StructuredTypeFlags.AbstractClass;
				this.UpdateWidgets ();
			};

			this.generateSchemaButton = new CheckButton
			{
				Parent = leftPane,
				Text = Common.Types.Res.Values.StructuredTypeFlags.GenerateSchema.DefaultLabel,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			this.generateSchemaButton.Clicked += delegate
			{
				this.StructuredTypeFlags ^= Types.StructuredTypeFlags.GenerateSchema;
				this.UpdateWidgets ();
			};

			this.generateRepositoryButton = new CheckButton
			{
				Parent = leftPane,
				Text = Common.Types.Res.Values.StructuredTypeFlags.GenerateRepository.DefaultLabel,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			this.generateRepositoryButton.Clicked += delegate
			{
				this.StructuredTypeFlags ^= Types.StructuredTypeFlags.GenerateRepository;
				this.UpdateWidgets ();
			};

			//	Rempli la colonne de droite.
			this.unknownButton = new RadioButton
			{
				Parent = rightPane,
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
				Parent = rightPane,
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
				Parent = rightPane,
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
				Parent = rightPane,
				Text = Common.Types.Res.Values.DataLifetimeExpectancy.Immutable.DefaultLabel,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			this.immutableButton.Clicked += delegate
			{
				this.DataLifetimeExpectancy = Types.DataLifetimeExpectancy.Immutable;
				this.UpdateWidgets ();
			};
		}

		public void Update()
		{
			this.UpdateWidgets ();
		}

		public bool IsEditOk
		{
			get
			{
				return this.isEditOk;
			}
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

			this.unknownButton.ActiveState   = (this.DataLifetimeExpectancy == Types.DataLifetimeExpectancy.Unknown  ) ? ActiveState.Yes : ActiveState.No;
			this.volatileButton.ActiveState  = (this.DataLifetimeExpectancy == Types.DataLifetimeExpectancy.Volatile ) ? ActiveState.Yes : ActiveState.No;
			this.stableButton.ActiveState    = (this.DataLifetimeExpectancy == Types.DataLifetimeExpectancy.Stable   ) ? ActiveState.Yes : ActiveState.No;
			this.immutableButton.ActiveState = (this.DataLifetimeExpectancy == Types.DataLifetimeExpectancy.Immutable) ? ActiveState.Yes : ActiveState.No;
		}


		private void HandleWindowCloseClicked(object sender)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonOkClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

			this.isEditOk = true;
		}


		private bool						isEditOk;

		private CheckButton					abstractClassButton;
		private CheckButton					generateSchemaButton;
		private CheckButton					generateRepositoryButton;

		private RadioButton					unknownButton;
		private RadioButton					volatileButton;
		private RadioButton					stableButton;
		private RadioButton					immutableButton;

		private Button						buttonOk;
		private Button						buttonCancel;
	}
}
