using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant d'éditer une expression régulière dans l'éditeur d'entités.
	/// </summary>
	public class EntityExpression : Abstract
	{
		public EntityExpression(DesignerApplication designerApplication) : base(designerApplication)
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
				this.WindowInit("EntityExpression", 400, 250, true);
				this.window.Text = "Expression";  // Res.Strings.Dialog.EntityExpression.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(400, 150);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				//	Crée l'en-tête et son contenu.
				this.header = new FrameBox(this.window.Root);
				this.header.PreferredHeight = 0;
				this.header.MinHeight = 0;
				this.header.Margins = new Margins(0, 0, 0, 3);
				this.header.Dock = DockStyle.Top;

				this.buttonRedefine = new CheckButton(this.header);
				this.buttonRedefine.Text = "Redéfinir localement";
				this.buttonRedefine.PreferredWidth = 200;
				this.buttonRedefine.AutoToggle = false;
				this.buttonRedefine.Dock = DockStyle.Top;
				this.buttonRedefine.Clicked += new MessageEventHandler(this.HandleButtonClicked);

				//	Crée le grand pavé de texte éditable.
				this.fieldExpression = new TextFieldMulti(this.window.Root);
				this.fieldExpression.Dock = DockStyle.Fill;
				this.fieldExpression.TabIndex = 1;

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonDeep = new CheckButton(footer);
				this.buttonDeep.Text = "Voir l'expression dans l'interface";
				this.buttonDeep.PreferredWidth = 200;
				this.buttonDeep.AutoToggle = false;
				this.buttonDeep.Dock = DockStyle.Left;
				this.buttonDeep.Clicked += new MessageEventHandler(this.HandleButtonClicked);

				this.buttonCancel = new Button(footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonCancel.TabIndex = 11;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonOk = new Button(footer);
				this.buttonOk.PreferredWidth = 75;
				this.buttonOk.Text = Res.Strings.Dialog.Button.OK;
				this.buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonOk.Dock = DockStyle.Right;
				this.buttonOk.Margins = new Margins(0, 6, 0, 0);
				this.buttonOk.Clicked += new MessageEventHandler(this.HandleButtonOkClicked);
				this.buttonOk.TabIndex = 10;
				this.buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.UpdateExpression();

			this.window.ShowDialog();
		}

		public void Initialise(bool isEditLocked, bool isInterface, string deepExpression, string expression)
		{
			this.isEditOk = false;
			this.isEditLocked = isEditLocked;
			this.isInterface = isInterface;
			this.deepExpression = deepExpression;
			this.expression = expression;
		}

		public bool IsEditOk
		{
			get
			{
				return this.isEditOk;
			}
		}

		public string Expression
		{
			get
			{
				if (this.isInterface && this.buttonRedefine.ActiveState == ActiveState.No)
				{
					return null;
				}
				else
				{
					return this.fieldExpression.Text;
				}
			}
		}


		protected void UpdateExpression()
		{
			this.buttonDeep.Visibility = this.isInterface;
			this.buttonDeep.ActiveState = ActiveState.No;

			this.buttonRedefine.Visibility = this.isInterface;
			this.buttonRedefine.ActiveState = (this.expression == null) ? ActiveState.No : ActiveState.Yes;
			this.header.Visibility = this.buttonRedefine.Visibility;

			this.fieldExpression.Text = this.expression;
			this.fieldExpression.Enable = (!this.isInterface || this.expression != null);
			this.fieldExpression.IsReadOnly = this.isEditLocked;

			this.buttonOk.Enable = true;
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

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			AbstractButton button = sender as AbstractButton;

			if (button == this.buttonDeep)
			{
				if (this.buttonDeep.ActiveState == ActiveState.No)
				{
					this.buttonDeep.ActiveState = ActiveState.Yes;
					this.buttonRedefine.Visibility = false;
					this.header.Visibility = this.buttonRedefine.Visibility;

					this.expression = this.fieldExpression.Text;
					this.fieldExpression.Text = this.deepExpression;
					this.fieldExpression.Enable = true;
					this.fieldExpression.IsReadOnly = true;

					this.buttonOk.Enable = false;
				}
				else
				{
					this.buttonDeep.ActiveState = ActiveState.No;
					this.buttonRedefine.Visibility = true;
					this.header.Visibility = this.buttonRedefine.Visibility;

					this.fieldExpression.Text = this.expression;
					this.fieldExpression.Enable = (this.buttonRedefine.ActiveState == ActiveState.Yes);
					this.fieldExpression.IsReadOnly = this.isEditLocked;

					this.buttonOk.Enable = true;
				}
			}

			if (button == this.buttonRedefine)
			{
				if (this.buttonRedefine.ActiveState == ActiveState.No)
				{
					this.buttonRedefine.ActiveState = ActiveState.Yes;

					this.fieldExpression.Text = this.deepExpression;
					this.fieldExpression.Enable = true;
					this.fieldExpression.SelectAll();
					this.fieldExpression.Focus();
				}
				else
				{
					this.buttonRedefine.ActiveState = ActiveState.No;

					this.fieldExpression.Text = "";
					this.fieldExpression.Enable = false;
				}
			}
		}


		protected bool							isEditLocked;
		protected bool							isEditOk;
		protected bool							isInterface;
		protected string						deepExpression;
		protected string						expression;
		protected FrameBox						header;
		protected CheckButton					buttonDeep;
		protected CheckButton					buttonRedefine;
		protected TextFieldMulti				fieldExpression;
		protected Button						buttonOk;
		protected Button						buttonCancel;
	}
}
