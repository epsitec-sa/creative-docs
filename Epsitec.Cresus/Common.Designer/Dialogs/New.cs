using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de créer un nouveau module.
	/// </summary>
	public class New : Abstract
	{
		public New(DesignerApplication designerApplication) : base(designerApplication)
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
				this.WindowInit("New", 400, 200, true);
				this.window.Text = "Nouveau"; // Res.Strings.Dialog.New.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				this.fieldRootDirectoryPath = this.CreateTextField(1, "Chemin de la racine", this.initialRootDirectoryPath);
				this.fieldModuleName        = this.CreateTextField(2, "Nom du module",       this.initialModuleName);
				this.fieldSourceNamespace   = this.CreateTextField(3, "Namespace source",    this.initialSourceNamespace);

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
				this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonCancel.TabIndex = 11;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonNew = new Button(footer);
				this.buttonNew.PreferredWidth = 75;
				this.buttonNew.Text = "Créer"; // Res.Strings.Dialog.Open.Button.New;
				this.buttonNew.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonNew.Dock = DockStyle.Right;
				this.buttonNew.Margins = new Margins(0, 6, 0, 0);
				this.buttonNew.Clicked += new MessageEventHandler(this.HandleButtonNewClicked);
				this.buttonNew.TabIndex = 10;
				this.buttonNew.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.UpdateButtons();
			this.fieldRootDirectoryPath.Focus();

			this.window.ShowDialog();
		}


		public void Initialize(string rootDirectoryPath, string moduleName, string sourceNamespace)
		{
			this.initialRootDirectoryPath = rootDirectoryPath;
			this.initialModuleName        = moduleName;
			this.initialSourceNamespace   = sourceNamespace;
			this.finalRootDirectoryPath   = null;
			this.finalModuleName          = null;
			this.finalSourceNamespace     = null;
		}

		public string RootDirectoryPath
		{
			get
			{
				return this.finalRootDirectoryPath;
			}
		}

		public string ModuleName
		{
			get
			{
				return this.finalModuleName;
			}
		}

		public string SourceNamespace
		{
			get
			{
				return this.finalSourceNamespace;
			}
		}


		protected TextField CreateTextField(int tabIndex, string labelName, string initialContent)
		{
			FrameBox box = new FrameBox(this.window.Root);
			box.Margins = new Margins(0, 0, 0, 5);
			box.Dock = DockStyle.Top;
			box.TabIndex = tabIndex;
			box.TabNavigationMode = TabNavigationMode.ForwardTabActive;

			StaticText label = new StaticText(box);
			label.Text = labelName;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.PreferredWidth = 120;
			label.Margins = new Margins(0, 5, 0, 0);
			label.Dock = DockStyle.Left;

			TextField field = new TextField(box);
			field.Text = initialContent;
			field.Dock = DockStyle.Fill;
			field.TabIndex = tabIndex;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			field.TextChanged += new EventHandler(this.HandleFieldTextChanged);

			return field;
		}


		protected void UpdateButtons()
		{
			//	Met à jour tous les boutons.
			bool defined = !string.IsNullOrEmpty(this.fieldRootDirectoryPath.Text) &&
						   !string.IsNullOrEmpty(this.fieldModuleName.Text) &&
						   !string.IsNullOrEmpty(this.fieldSourceNamespace.Text);

			this.buttonNew.Enable = defined;
		}



		private void HandleFieldTextChanged(object sender)
		{
			this.UpdateButtons();
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

		private void HandleButtonNewClicked(object sender, MessageEventArgs e)
		{
			this.finalRootDirectoryPath = this.fieldRootDirectoryPath.Text;
			this.finalModuleName        = this.fieldModuleName.Text;
			this.finalSourceNamespace   = this.fieldSourceNamespace.Text;

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}



		protected string						initialRootDirectoryPath;
		protected string						initialModuleName;
		protected string						initialSourceNamespace;
		protected string						finalRootDirectoryPath;
		protected string						finalModuleName;
		protected string						finalSourceNamespace;
		protected TextField						fieldRootDirectoryPath;
		protected TextField						fieldModuleName;
		protected TextField						fieldSourceNamespace;
		protected Button						buttonNew;
		protected Button						buttonCancel;
		protected bool							ignoreChange;
	}
}
