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
	public class NewDialog : AbstractDialog
	{
		public NewDialog(DesignerApplication designerApplication) : base(designerApplication)
		{
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
				this.WindowInit("New", 500, 230, true);
				this.window.Text = Res.Strings.Dialog.New.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				FrameBox box = new FrameBox(this.window.Root);
				box.Margins = new Margins(0, 0, 0, 12);
				box.Dock = DockStyle.Top;

				this.radioTypeReference = new RadioButton(box);
				this.radioTypeReference.Text = Res.Strings.Dialog.New.Type.Reference;
				this.radioTypeReference.PreferredWidth = 150;
				this.radioTypeReference.Dock = DockStyle.Left;
				this.radioTypeReference.Clicked += this.HandleRadioTypeClicked;

				this.radioTypePatch = new RadioButton(box);
				this.radioTypePatch.Text = Res.Strings.Dialog.New.Type.Patch0;
				this.radioTypePatch.PreferredWidth = 300;
				this.radioTypePatch.Dock = DockStyle.Left;
				this.radioTypePatch.Clicked += this.HandleRadioTypeClicked;

				Separator sep = new Separator(this.window.Root);
				sep.PreferredHeight = 1;
				sep.Margins = new Margins(0, 0, 0, 18);
				sep.Dock = DockStyle.Top;

				this.fieldRootDirectoryPath = this.CreateTextField(1, Res.Strings.Dialog.New.Field.RootDirectoryPath, this.initialRootDirectoryPath);
				this.fieldModuleName        = this.CreateTextField(2, Res.Strings.Dialog.New.Field.ModuleName,        this.initialModuleName);
				this.fieldSourceNamespace   = this.CreateTextField(3, Res.Strings.Dialog.New.Field.SourceNamespace,   this.initialSourceNamespace);

				this.radioLayerBox = new FrameBox(this.window.Root);
				this.radioLayerBox.Margins = new Margins(125, 0, 5, 0);
				this.radioLayerBox.Dock = DockStyle.Top;

				this.radioLayerSystem = new RadioButton(this.radioLayerBox);
				this.radioLayerSystem.Text = Res.Strings.Dialog.New.Layer.System;
				this.radioLayerSystem.Dock = DockStyle.Top;
				this.radioLayerSystem.Clicked += this.HandleRadioLayerClicked;

				this.radioLayerApplication = new RadioButton(this.radioLayerBox);
				this.radioLayerApplication.Text = Res.Strings.Dialog.New.Layer.Application;
				this.radioLayerApplication.Dock = DockStyle.Top;
				this.radioLayerApplication.Clicked += this.HandleRadioLayerClicked;

				this.radioLayerUser = new RadioButton(this.radioLayerBox);
				this.radioLayerUser.Text = Res.Strings.Dialog.New.Layer.User;
				this.radioLayerUser.Dock = DockStyle.Top;
				this.radioLayerUser.Clicked += this.HandleRadioLayerClicked;

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

				this.buttonNew = new Button(footer);
				this.buttonNew.PreferredWidth = 75;
				this.buttonNew.Text = Res.Strings.Dialog.New.Button.New;
				this.buttonNew.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonNew.Dock = DockStyle.Right;
				this.buttonNew.Margins = new Margins(0, 6, 0, 0);
				this.buttonNew.Clicked += this.HandleButtonNewClicked;
				this.buttonNew.TabIndex = 10;
				this.buttonNew.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}
			else
			{
				this.fieldRootDirectoryPath.Text = this.initialRootDirectoryPath;
				this.fieldModuleName.Text        = this.initialModuleName;
				this.fieldSourceNamespace.Text   = this.initialSourceNamespace;
			}

			this.UpdateButtons();
			this.fieldRootDirectoryPath.SelectAll();
			this.fieldRootDirectoryPath.Focus();

			this.window.ShowDialog();
		}


		public void Initialize(string actualModuleName, string rootDirectoryPath, string moduleName, string sourceNamespace, ResourceModuleLayer resourceModuleLayer)
		{
			this.actualModuleName         = actualModuleName;

			this.initialRootDirectoryPath = rootDirectoryPath;
			this.initialModuleName        = moduleName;
			this.initialSourceNamespace   = sourceNamespace;

			this.finalRootDirectoryPath   = null;
			this.finalModuleName          = null;
			this.finalSourceNamespace     = null;

			this.resourceModuleLayer = resourceModuleLayer;
			this.isPatch = false;
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

		public ResourceModuleLayer ResourceModuleLayer
		{
			get
			{
				return this.resourceModuleLayer;
			}
		}

		public bool IsPatch
		{
			get
			{
				return this.isPatch;
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
			field.TextChanged += this.HandleFieldTextChanged;

			return field;
		}


		protected void UpdateButtons()
		{
			//	Met à jour tous les boutons.
			if (string.IsNullOrEmpty(this.actualModuleName))
			{
				this.radioTypePatch.Text = Res.Strings.Dialog.New.Type.Patch0;
				this.radioTypePatch.Enable = false;
			}
			else
			{
				this.radioTypePatch.Text = string.Format(Res.Strings.Dialog.New.Type.Patch1, this.actualModuleName);
				this.radioTypePatch.Enable = true;
			}

			this.radioTypeReference.ActiveState = this.isPatch ? ActiveState.No  : ActiveState.Yes;
			this.radioTypePatch.ActiveState     = this.isPatch ? ActiveState.Yes : ActiveState.No;

			this.radioLayerSystem.ActiveState      = (this.resourceModuleLayer == ResourceModuleLayer.System     ) ? ActiveState.Yes : ActiveState.No;
			this.radioLayerApplication.ActiveState = (this.resourceModuleLayer == ResourceModuleLayer.Application) ? ActiveState.Yes : ActiveState.No;
			this.radioLayerUser.ActiveState        = (this.resourceModuleLayer == ResourceModuleLayer.User       ) ? ActiveState.Yes : ActiveState.No;

			this.fieldModuleName.Enable      = !this.isPatch;
			this.fieldSourceNamespace.Enable = !this.isPatch;
			this.radioLayerBox.Enable        = !this.isPatch;

			bool defined = !string.IsNullOrEmpty(this.fieldRootDirectoryPath.Text) &&
						   (!string.IsNullOrEmpty(this.fieldModuleName.Text)      || this.isPatch) &&
						   (!string.IsNullOrEmpty(this.fieldSourceNamespace.Text) || this.isPatch);

			this.buttonNew.Enable = defined;
		}



		private void HandleRadioTypeClicked(object sender, MessageEventArgs e)
		{
			if (sender == this.radioTypeReference)
			{
				this.isPatch = false;
				this.fieldRootDirectoryPath.Text = "%custom%";
			}

			if (sender == this.radioTypePatch)
			{
				this.isPatch = true;
				this.fieldRootDirectoryPath.Text = "%patches%";
			}

			this.UpdateButtons();
		}

		private void HandleRadioLayerClicked(object sender, MessageEventArgs e)
		{
			if (sender == this.radioLayerSystem)
			{
				this.resourceModuleLayer = ResourceModuleLayer.System;
			}

			if (sender == this.radioLayerApplication)
			{
				this.resourceModuleLayer = ResourceModuleLayer.Application;
			}

			if (sender == this.radioLayerUser)
			{
				this.resourceModuleLayer = ResourceModuleLayer.User;
			}

			this.UpdateButtons();
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


		protected string						actualModuleName;
		protected string						initialRootDirectoryPath;
		protected string						initialModuleName;
		protected string						initialSourceNamespace;
		protected ResourceModuleLayer			resourceModuleLayer;
		protected string						finalRootDirectoryPath;
		protected string						finalModuleName;
		protected string						finalSourceNamespace;
		protected RadioButton					radioTypeReference;
		protected RadioButton					radioTypePatch;
		protected TextField						fieldRootDirectoryPath;
		protected TextField						fieldModuleName;
		protected TextField						fieldSourceNamespace;
		protected FrameBox						radioLayerBox;
		protected RadioButton					radioLayerSystem;
		protected RadioButton					radioLayerApplication;
		protected RadioButton					radioLayerUser;
		protected Button						buttonNew;
		protected Button						buttonCancel;
		protected bool							isPatch;
		protected bool							ignoreChange;
	}
}
