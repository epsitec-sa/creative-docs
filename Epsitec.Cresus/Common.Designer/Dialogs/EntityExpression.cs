using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant d'�diter une expression r�guli�re dans l'�diteur d'entit�s.
	/// </summary>
	public class EntityExpression : Abstract
	{
		public EntityExpression(DesignerApplication designerApplication) : base(designerApplication)
		{
		}

		public override void Show()
		{
			//	Cr�e et montre la fen�tre du dialogue.
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

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				FrameBox main = new FrameBox(this.window.Root);
				main.Dock = DockStyle.Fill;

				FrameBox top = new FrameBox(main);
				top.Dock = DockStyle.Fill;
				top.Padding = new Margins(8, 8, 8, 8);

				this.bottom = new FrameBox(main);
				this.bottom.PreferredHeight = 100;
				this.bottom.Dock = DockStyle.Bottom;
				this.bottom.Padding = new Margins(8, 8, 8, 8);

				this.splitter = new HSplitter(main);
				this.splitter.Dock = DockStyle.Bottom;
				this.splitter.Margins = new Margins(0, 0, 0, 0);

				//	Cr�e l'en-t�te et son contenu.
				this.header = new FrameBox(top);
				this.header.PreferredHeight = 0;
				this.header.MinHeight = 0;
				this.header.Margins = new Margins(0, 0, 0, 3);
				this.header.Dock = DockStyle.Top;

				this.buttonRedefine = new CheckButton(this.header);
				this.buttonRedefine.Text = "Red�finir localement";
				this.buttonRedefine.PreferredWidth = 200;
				this.buttonRedefine.AutoToggle = false;
				this.buttonRedefine.Dock = DockStyle.Top;
				this.buttonRedefine.Clicked += new MessageEventHandler(this.HandleButtonClicked);

				//	Cr�e le grand pav� de texte �ditable.
				this.fieldExpression = new TextFieldMulti(top);
				this.fieldExpression.Dock = DockStyle.Fill;
				this.fieldExpression.TabIndex = 1;

				StaticText label = new StaticText(this.bottom);
				label.Text = "Expression de base :";
				label.Dock = DockStyle.Top;
				label.Margins = new Margins(0, 0, -2, 2);

				this.fieldDeepExpression = new TextFieldMulti(this.bottom);
				this.fieldDeepExpression.Dock = DockStyle.Fill;
				this.fieldDeepExpression.IsReadOnly = true;

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(8, 8, 0, 8);
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

			this.UpdateEnable();
			this.UpdateExpression();

			this.window.ShowDialog();
		}

		public void Initialise(bool isReadOnly, bool isOverridable, string inheritedExpression, string localExpression)
		{
			//	Expression locale:    isOverridable = false, inheritedExpression = null,        localExpression = null (valeur) ou calcul
			//	Selon h�ritage:       isOverridable = true,  inheritedExpression = null/calcul, localExpression = null
			//	Surcharg� localement: isOverridable = true,  inheritedExpression = null/calcul, localExpression = "" (valeur) ou calcul
			//
			//	Donc, si isOverridable = true, localExpression peut prendre trois valeurs:
			//	localExpression = null:   pas de red�finition locale, on utilise le calcul h�rit�
			//	localExpression = "":     red�finition d'une valeur, qui aura la priorit� sur le calcul h�rit�
			//	localExpression = calcul: red�finition d'un calcul, qui aura la priorit� sur le calcul h�rit�
			this.isEditOk = false;
			this.isEditLocked = isReadOnly;
			this.isInterface = isOverridable;
			this.deepExpression = inheritedExpression;
			this.expression = localExpression;
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

		protected void UpdateEnable()
		{
			if (this.isEditLocked)
			{
				this.fieldExpression.IsReadOnly = true;
				this.buttonRedefine.Enable = false;
				this.buttonOk.Visibility = false;
				this.buttonOk.Enable = false;
			}
			else
			{
				this.fieldExpression.IsReadOnly = false;
				this.buttonRedefine.Enable = true;
				this.buttonOk.Visibility = true;
				this.buttonOk.Enable = true;
			}
		}

		protected void UpdateExpression()
		{
			this.buttonRedefine.Visibility = this.isInterface;
			this.buttonRedefine.ActiveState = (this.expression == null) ? ActiveState.No : ActiveState.Yes;
			this.header.Visibility = this.buttonRedefine.Visibility;

			this.fieldExpression.Text = this.expression;
			this.fieldExpression.Enable = (!this.isInterface || this.expression != null);
			this.fieldExpression.SelectAll();
			this.fieldExpression.Focus();

			this.splitter.Visibility = this.isInterface;
			this.bottom.Visibility = this.isInterface;
			this.fieldDeepExpression.Text = this.deepExpression;

			this.buttonOk.Enable = !this.isEditLocked;
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
		protected CheckButton					buttonRedefine;
		protected TextFieldMulti				fieldExpression;
		protected HSplitter						splitter;
		protected FrameBox						bottom;
		protected TextFieldMulti				fieldDeepExpression;
		protected Button						buttonOk;
		protected Button						buttonCancel;
	}
}
