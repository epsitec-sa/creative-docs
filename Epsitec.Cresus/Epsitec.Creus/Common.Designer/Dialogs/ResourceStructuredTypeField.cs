using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir un champ d'un type structuré.
	/// </summary>
	public class ResourceStructuredTypeField : Abstract
	{
		public ResourceStructuredTypeField(DesignerApplication designerApplication) : base(designerApplication)
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
				this.WindowInit("ResourceStructuredTypeField", 250, 300, true);
				this.window.Text = Res.Strings.Dialog.ResourceStructuredTypeField.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				//	Liste principale.
				this.list = new ScrollList(this.window.Root);
				this.list.TabIndex = 1;
				this.list.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.list.Dock = DockStyle.Fill;

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
				this.buttonOk.Text = Res.Strings.Dialog.ResourceStructuredTypeField.Button.OK;
				this.buttonOk.Dock = DockStyle.Right;
				this.buttonOk.Margins = new Margins(0, 6, 0, 0);
				this.buttonOk.Clicked += new MessageEventHandler(this.HandleButtonOkClicked);
				this.buttonOk.TabIndex = 10;
				this.buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.UpdateList();

			this.window.ShowDialog();
		}

		public void Initialise(StructuredType st, string field)
		{
			this.structuredType = st;
			this.initialField = field;
			this.selectedField = null;
		}

		public string SelectedField
		{
			get
			{
				return this.selectedField;
			}
		}


		protected void UpdateList()
		{
			this.list.Items.Clear();

			int sel = -1;
			int i = 0;
			foreach (string id in this.structuredType.GetFieldIds())
			{
				this.list.Items.Add(id);
				if (id == this.initialField)
				{
					sel = i;
				}

				i++;
			}

			this.list.SelectedIndex = sel;
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

			int sel = this.list.SelectedIndex;
			if (sel != -1)
			{
				this.selectedField = this.list.Items[sel];
			}
		}


		protected StructuredType				structuredType;
		protected string						initialField;
		protected string						selectedField;
		protected ScrollList					list;
		protected Button						buttonOk;
		protected Button						buttonCancel;
	}
}
