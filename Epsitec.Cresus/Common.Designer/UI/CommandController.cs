//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.UI
{
	/// <summary>
	/// Summary description for CommandController.
	/// </summary>
	public class CommandController : Epsitec.Common.UI.Controllers.AbstractController
	{
		public CommandController()
		{
		}
		
		
		public new CommandAdapter				Adapter
		{
			get
			{
				return base.Adapter as CommandAdapter;
			}
		}
		
		
		public override void CreateUI(Widget panel)
		{
			this.caption_label = new StaticText (panel);
			this.combo_cmd    = new TextFieldCombo (panel);
			
			double h_line = this.combo_cmd.Height;
			double h_pane = System.Math.Max (panel.Height, h_line + 6);
			
			panel.Height = h_pane;
			
			double y = System.Math.Floor ((h_pane - h_line) / 2);
			
			this.caption_label.Size          = new Drawing.Size (80, h_line);
			this.caption_label.Anchor        = AnchorStyles.TopLeft;
			this.caption_label.AnchorMargins = new Drawing.Margins (0, 0, h_pane - y - h_line, 0);
			
			this.combo_cmd.Height         = h_line;
			this.combo_cmd.Anchor         = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.combo_cmd.AnchorMargins  = new Drawing.Margins (this.caption_label.Right, 0, h_pane - y - h_line, 0);
			this.combo_cmd.TextChanged   += new Support.EventHandler (this.HandleComboCommandTextChanged);
			this.combo_cmd.TabIndex       = 10;
			this.combo_cmd.Name           = "Value";
			
			this.OnCaptionChanged ();
			
			this.SyncFromCommands ();
			this.SyncFromAdapter (Common.UI.SyncReason.Initialisation);
		}
		
		public override void SyncFromAdapter(Common.UI.SyncReason reason)
		{
			CommandAdapter adapter = this.Adapter;
			
			if ((adapter != null) &&
				(this.combo_cmd != null))
			{
				this.combo_cmd.Text = adapter.Value;
				
				if ((reason == Common.UI.SyncReason.SourceChanged) ||
					(reason == Common.UI.SyncReason.Initialisation))
				{
					this.combo_cmd.SelectAll ();
				}
			}
		}
		
		public override void SyncFromUI()
		{
			CommandAdapter adapter = this.Adapter;
			
			if ((adapter != null) &&
				(this.combo_cmd != null))
			{
				adapter.Value = this.combo_cmd.Text;
			}
		}
		
		
		private void SyncFromCommands()
		{
			//	Refait la liste des commandes.
			
			this.combo_cmd.Items.Clear ();
			
			DialogDesigner designer = this.Adapter.Application.InterfaceEditController.ActiveDialogDesigner;
			
			if (designer == null)
			{
				return;
			}
			
			Support.CommandDispatcher commands = designer.DialogCommands;
			
			if (commands == null)
			{
				return;
			}
			
			this.combo_cmd.Items.AddRange (commands.FindCommandNames ());
		}
		
		
		private void HandleComboCommandTextChanged(object sender)
		{
			this.OnUIDataChanged ();
		}
		
		
		private TextFieldCombo					combo_cmd;
	}
}
