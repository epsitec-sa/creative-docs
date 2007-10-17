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
		public enum Type
		{
			Normal,
			Interface,
			InterfaceRedefine,
		}

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
				this.WindowInit("EntityExpression", 350, 250, true);
				this.window.Text = "Expression";  // Res.Strings.Dialog.EntityExpression.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(200, 150);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				//	Crée l'en-tête et son contenu.
				this.header = new FrameBox(this.window.Root);
				this.header.Margins = new Margins(0, 0, 0, 3);
				this.header.Dock = DockStyle.Top;

				this.buttonExpression = new CheckButton(this.header);
				this.buttonExpression.Text = "Expression";
				this.buttonExpression.PreferredWidth = 200;
				this.buttonExpression.AutoToggle = false;
				this.buttonExpression.Dock = DockStyle.Top;
				this.buttonExpression.Clicked += new MessageEventHandler(this.HandleButtonClicked);

				this.buttonLocal = new CheckButton(this.header);
				this.buttonLocal.Text = "Redéfinition dans l'interface du patch";
				this.buttonLocal.PreferredWidth = 200;
				this.buttonLocal.AutoToggle = false;
				this.buttonLocal.Dock = DockStyle.Top;
				this.buttonLocal.Clicked += new MessageEventHandler(this.HandleButtonClicked);

				//	Crée le grand pavé de texte éditable.
				this.fieldExpression = new TextFieldMulti(this.window.Root);
				this.fieldExpression.Dock = DockStyle.Fill;
				this.fieldExpression.TabIndex = 1;

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

		public void Initialise(bool isEditLocked, Type type, string expression, string localExpression)
		{
			this.isEditOk = false;
			this.isEditLocked = isEditLocked;
			this.type = type;
			this.expression = expression;
			this.localExpression = localExpression;
		}

		public bool IsEditOk
		{
			get
			{
				return this.isEditOk;
			}
		}

		public Type InternalType
		{
			get
			{
				return this.type;
			}
		}

		public string Expression
		{
			get
			{
				if (this.buttonExpression.ActiveState == ActiveState.No)
				{
					return null;
				}
				else
				{
					if (this.type == Type.InterfaceRedefine)
					{
						return this.expression;
					}
					else
					{
						return this.fieldExpression.Text;
					}
				}
			}
		}

		public string LocalExpression
		{
			get
			{
				if (this.buttonExpression.ActiveState == ActiveState.No)
				{
					return null;
				}
				else
				{
					if (this.type == Type.InterfaceRedefine)
					{
						return this.fieldExpression.Text;
					}
					else
					{
						return this.localExpression;
					}
				}
			}
		}


		protected void UpdateExpression()
		{
			string expression = (this.type == Type.InterfaceRedefine) ? this.localExpression : this.expression;

			if (string.IsNullOrEmpty(expression))
			{
				this.fieldExpression.Text = "";
				this.fieldExpression.Enable = false;
				this.buttonExpression.ActiveState = ActiveState.No;
			}
			else
			{
				this.fieldExpression.Text = expression;
				this.fieldExpression.Enable = true;
				this.buttonExpression.ActiveState = ActiveState.Yes;
			}

			if (this.type == Type.InterfaceRedefine)
			{
				this.buttonLocal.ActiveState = ActiveState.Yes;
			}
			else
			{
				this.buttonLocal.ActiveState = ActiveState.No;
			}

			this.buttonLocal.Visibility = (this.type != Type.Normal);

			this.fieldExpression.IsReadOnly = this.isEditLocked;
			this.buttonExpression.Enable = !this.isEditLocked;
			this.buttonLocal.Enable = !this.isEditLocked;

			if (!this.isEditLocked)
			{
				this.fieldExpression.SelectAll();
				this.fieldExpression.Focus();
			}
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

			if (button == this.buttonExpression)
			{
				if (this.buttonExpression.ActiveState == ActiveState.No)
				{
					this.buttonExpression.ActiveState = ActiveState.Yes;
					this.fieldExpression.Enable = true;
					this.fieldExpression.SelectAll();
					this.fieldExpression.Focus();
				}
				else
				{
					this.buttonExpression.ActiveState = ActiveState.No;
					this.fieldExpression.Enable = false;
				}
			}

			if (button == this.buttonLocal)
			{
				if (this.buttonLocal.ActiveState == ActiveState.No)
				{
					this.buttonLocal.ActiveState = ActiveState.Yes;
					this.type = Type.InterfaceRedefine;

					this.expression = this.fieldExpression.Text;
					this.fieldExpression.Text = this.localExpression;
				}
				else
				{
					this.buttonLocal.ActiveState = ActiveState.No;
					this.type = Type.Interface;

					this.localExpression = this.fieldExpression.Text;
					this.fieldExpression.Text = this.expression;
				}
			}
		}


		protected bool							isEditLocked;
		protected bool							isEditOk;
		protected Type							type;
		protected string						expression;
		protected string						localExpression;
		protected FrameBox						header;
		protected CheckButton					buttonExpression;
		protected CheckButton					buttonLocal;
		protected TextFieldMulti				fieldExpression;
		protected Button						buttonOk;
		protected Button						buttonCancel;
	}
}
