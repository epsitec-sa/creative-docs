//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Script.Developer
{
	/// <summary>
	/// Summary description for EditionController.
	/// </summary>
	public class EditionController
	{
		public EditionController()
		{
			this.dispatcher = new CommandDispatcher ("EditionController", true);
			this.dispatcher.RegisterController (this);
			
			this.save_command_state = CommandState.Find ("SaveSource", this.dispatcher);
		}
		
		
		public Source							Source
		{
			get
			{
				return this.source;
			}
			set
			{
				if (this.source != value)
				{
					this.source = value;
					this.UpdateFromSource ();
				}
			}
		}
		
		
		public void CreateWidgets(Widget parent)
		{
			this.panel    = new Panels.MethodEditionPanel ();
			this.tool_bar = new HToolBar (parent);
			
			this.method_combo = new TextFieldCombo ();
			this.method_combo.Width      = 120;
			this.method_combo.IsReadOnly = true;
			this.method_combo.SelectedIndexChanged += new EventHandler (this.HandleSelectedMethodChanged);
			
			this.tool_bar.Dock = DockStyle.Top;
			
			this.panel.Widget.Parent      = parent;
			this.panel.Widget.Dock        = DockStyle.Fill;
			this.panel.Widget.DockMargins = new Drawing.Margins (4, 4, 4, 4);
			
			this.panel.IsModifiedChanged += new EventHandler (this.HandlePanelIsModifiedChanged);
			
			this.UpdateToolBar ();
			this.UpdateFromSource ();
			this.UpdateCommandStates (true);
		}
		
		
		protected virtual void UpdateToolBar()
		{
			this.tool_bar.CommandDispatcher = this.dispatcher;
			
			this.tool_bar.Items.Clear ();
			this.tool_bar.Items.Add (IconButton.CreateSimple ("SaveSource", "manifest:Epsitec.Common.Script.Developer.Images.Save.icon"));
			this.tool_bar.Items.Add (new IconSeparator ());
			this.tool_bar.Items.Add (this.method_combo);
		}
		
		protected virtual void UpdateFromSource()
		{
			if (this.panel != null)
			{
				this.panel.Method = this.source.Methods[this.method_index];
				
				this.method_combo.Items.Clear ();
				
				foreach (Source.Method method in this.source.Methods)
				{
					this.method_combo.Items.Add (method.Name);
				}
				
				if ((this.method_combo.SelectedIndex == -1) &&
					(this.method_combo.Items.Count > 0))
				{
					this.method_combo.SelectedIndex = 0;
				}
			}
		}
		
		protected virtual void UpdateCommandStates(bool synchronise)
		{
			this.save_command_state.Enabled = this.panel.IsModified;
			
			if (synchronise)
			{
				this.save_command_state.Synchronise ();
			}
		}
		
		
		private void HandlePanelIsModifiedChanged(object sender)
		{
			this.UpdateCommandStates (false);
		}
		
		private void HandleSelectedMethodChanged(object sender)
		{
			if (this.method_index != this.method_combo.SelectedIndex)
			{
				this.method_index = this.method_combo.SelectedIndex;
				this.UpdateFromSource ();
			}
		}
		
		
		[Command ("SaveSource")] void CommandSaveSource()
		{
			this.panel.IsModified = false;
		}
		
		
		protected CommandDispatcher				dispatcher;
		protected Source						source;
		protected Panels.MethodEditionPanel		panel;
		protected HToolBar						tool_bar;
		protected TextFieldCombo				method_combo;
		protected int							method_index;
		
		protected CommandState					save_command_state;
	}
}
