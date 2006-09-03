using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using Epsitec.Common.IO;
using System.IO;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	/// <summary>
	/// Dialogue pour ouvrir un document existant.
	/// </summary>
	public class FileOpen : AbstractFile
	{
		public FileOpen(DocumentEditor editor) : base(editor)
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
				this.WindowInit("FileOpen", 400, 300, true);
				this.window.Text = Res.Strings.Dialog.New.Title;
				this.window.Owner = this.editor.Window;
				this.window.Icon = Bitmap.FromManifestResource("Epsitec.App.DocumentEditor.Images.Application.icon", this.GetType().Assembly);
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(300, 200);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				int tabIndex = 0;

				this.table = new CellTable(this.window.Root);
				this.table.DefHeight = 50;
				this.table.HeaderHeight = 20;
				this.table.StyleH = CellArrayStyles.Stretch | CellArrayStyles.Separator | CellArrayStyles.Header | CellArrayStyles.Mobile;
				this.table.StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine;
				this.table.AlphaSeparator = 0.3;
				this.table.Margins = new Margins(0, 0, 0, 0);
				this.table.Dock = DockStyle.Fill;

				//	Pied.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				Button buttonOpen = new Button(footer);
				buttonOpen.PreferredWidth = 75;
				buttonOpen.Text = Res.Strings.Dialog.New.Button.Open;
				buttonOpen.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOpen.Dock = DockStyle.Left;
				buttonOpen.Margins = new Margins(0, 6, 0, 0);
				buttonOpen.Clicked += new MessageEventHandler(this.HandleButtonOpenClicked);
				buttonOpen.TabIndex = tabIndex++;
				buttonOpen.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonCancel = new Button(footer);
				buttonCancel.PreferredWidth = 75;
				buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				buttonCancel.Dock = DockStyle.Left;
				buttonCancel.Margins = new Margins(0, 6, 0, 0);
				buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCancelClicked);
				buttonCancel.TabIndex = tabIndex++;
				buttonCancel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.slider = new HSlider(footer);
				this.slider.PreferredWidth = 80;
				this.slider.Dock = DockStyle.Right;
				this.slider.Margins = new Margins(0, 0, 4, 4);
				this.slider.TabIndex = tabIndex++;
				this.slider.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.slider.MinValue = 20.0M;
				this.slider.MaxValue = 100.0M;
				this.slider.SmallChange = 1.0M;
				this.slider.LargeChange = 10.0M;
				this.slider.Resolution = 1.0M;
				this.slider.Value = (decimal) this.table.DefHeight;
				this.slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
				ToolTip.Default.SetToolTip(this.slider, Res.Strings.Dialog.New.Tooltip.PreviewSize);

			}

			this.selectedFilename = null;
			this.UpdateTable(0);

			this.window.ShowDialog();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("FileOpen");
		}
	}
}
