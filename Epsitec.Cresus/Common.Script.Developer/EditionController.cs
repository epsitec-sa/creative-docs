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
			this.compile_command_state = CommandState.Find ("CompileSourceCode", this.dispatcher);
			this.find_error_command_state = CommandState.Find ("FindNextError", this.dispatcher);
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
			this.tool_tip = new ToolTip ();
			
			this.tool_tip.Behaviour = ToolTipBehaviour.Manual;
			
			this.method_combo = new TextFieldCombo ();
			this.method_combo.Width      = 120;
			this.method_combo.IsReadOnly = true;
			this.method_combo.SelectedIndexChanged += new EventHandler (this.HandleSelectedMethodChanged);
			this.method_combo.OpeningCombo         += new CancelEventHandler (this.HandleMethodOpeningCombo);
			
			this.compile_button = new Button ();
			this.compile_button.Text = "Compile";
			this.compile_button.Command = "CompileSourceCode";
			
			this.find_next_error_button = new Button ();
			this.find_next_error_button.Width = 20;
			this.find_next_error_button.Text = "&gt;";
			this.find_next_error_button.Command = "FindNextError";
			
			this.tool_bar.Dock = DockStyle.Top;
			
			this.panel.Widget.Parent      = parent;
			this.panel.Widget.Dock        = DockStyle.Fill;
			this.panel.Widget.DockMargins = new Drawing.Margins (4, 4, 4, 4);
			
			this.panel.IsModifiedChanged += new EventHandler (this.HandlePanelIsModifiedChanged);
			this.panel.SourceWidget.CursorChanged += new EventHandler (this.HandleSourceCursorChanged);
			
			this.UpdateToolBar ();
			this.UpdateFromSource ();
			this.UpdateCommandStates (true);
			
			new UniqueValueValidator (this, this.panel.MethodProtoPanel.MethodNameWidget);
			
			parent.Window.FocusedWidgetChanged += new EventHandler (this.HandleWindowFocusedWidgetChanged);
		}
		
		
		protected virtual void UpdateToolBar()
		{
			this.tool_bar.CommandDispatcher = this.dispatcher;
			
			this.tool_bar.Items.Clear ();
			this.tool_bar.Items.Add (IconButton.CreateSimple ("SaveSource", "manifest:Epsitec.Common.Script.Developer.Images.Save.icon"));
			this.tool_bar.Items.Add (new IconSeparator ());
			this.tool_bar.Items.Add (this.method_combo);
			this.tool_bar.Items.Add (new IconSeparator ());
			this.tool_bar.Items.Add (this.compile_button);
			this.tool_bar.Items.Add (this.find_next_error_button);
		}
		
		protected virtual void UpdateFromSource()
		{
			if (this.panel != null)
			{
				this.panel.Method = this.source.Methods[this.method_index];
				
				this.UpdateMethodCombo ();
			}
		}
		
		protected virtual void UpdateMethodCombo()
		{
			this.method_combo.Items.Clear ();
			
			foreach (Source.Method method in this.source.Methods)
			{
				this.method_combo.Items.Add (method.Signature, method.Name);
			}
			
			if ((this.method_combo.SelectedIndex == -1) &&
				(this.method_combo.Items.Count > 0))
			{
				this.method_combo.SelectedIndex = 0;
			}
		}
		
		protected virtual void UpdateCommandStates(bool synchronise)
		{
			this.save_command_state.Enabled       = this.panel.IsModified;
			this.compile_command_state.Enabled    = this.source.Methods.Length > 0;
			this.find_error_command_state.Enabled = this.errors.Count > 0;
			
			if (synchronise)
			{
				this.save_command_state.Synchronise ();
				this.compile_command_state.Synchronise ();
				this.find_error_command_state.Synchronise ();
			}
		}
		
		
		protected virtual void SyncFromUI ()
		{
			if (this.method_index != -1)
			{
				this.tool_tip.HideToolTipForWidget (this.panel.SourceWidget);
				
				this.source.Methods[this.method_index] = this.panel.Method;
				
				this.UpdateMethodCombo ();
			}
		}
		
		
		private void HandlePanelIsModifiedChanged(object sender)
		{
			this.UpdateCommandStates (false);
		}
		
		private void HandleSourceCursorChanged(object sender)
		{
			TextFieldMulti text = sender as TextFieldMulti;
			
			int tag;
			
			if ((text.TextLayout.IsSelectionWaved (text.CursorFrom, true)) &&
				(Error.FindTagInRichText (text.Text, text.Cursor, out tag)) &&
				(tag >= 0) &&
				(tag < this.errors.Count))
			{
				Error error = this.errors[tag] as Error;
				
				Drawing.Point p1, p2;
				
				if ((error == null) ||
					(text.GetCursorPosition (out p1, out p2) == false))
				{
					this.tool_tip.HideToolTipForWidget (text);
				}
				else
				{
					this.tool_tip.InitialLocation = text.MapClientToScreen (p1);
					this.tool_tip.SetToolTip (text, error.Description);
					this.tool_tip.ShowToolTipForWidget (text);
				}
			}
			else
			{
				this.tool_tip.HideToolTipForWidget (text);
			}
		}
		
		private void HandleSelectedMethodChanged(object sender)
		{
			if (this.method_index != this.method_combo.SelectedIndex)
			{
				this.method_index = this.method_combo.SelectedIndex;
				this.UpdateFromSource ();
			}
		}
		
		private void HandleWindowFocusedWidgetChanged(object sender)
		{
			EditArray edit  = this.panel.ParameterInfoPanel.EditArray;
			bool      value = edit.ContainsFocus;
			
			if (this.edit_array_focused != value)
			{
				this.edit_array_focused = value;
				
				if (this.edit_array_focused == false)
				{
					if (edit.InteractionMode == ScrollInteractionMode.Edition)
					{
						edit.ValidateEdition (false);
						
						System.Diagnostics.Debug.WriteLine ("EditArray: validated edition.");
					}
				}
			}
		}
		
		private void HandleMethodOpeningCombo(object sender, CancelEventArgs e)
		{
			this.SyncFromUI ();
		}
		
		
		[Command ("SaveSource")]		void CommandSaveSource()
		{
			this.SyncFromUI ();
			this.panel.IsModified = false;
		}
		
		[Command ("CompileSourceCode")]	void CommandCompileSourceCode()
		{
			this.SyncFromUI ();
			
			string source = this.source.GenerateAssemblySource ();
			Engine engine = new Engine ();
			Script script = engine.Compile (source);
			
			foreach (Source.Method method in this.source.Methods)
			{
				foreach (Source.CodeSection section in method.CodeSections)
				{
					section.HiliteError (-1, -1, -1);
				}
			}
			
			this.errors.Clear ();
			this.next_error = 0;
			
			if (script.HasErrors)
			{
				string[] lines = source.Split ('\n');
				
				foreach (string error in script.Errors)
				{
					string arg_line = error.Substring (0, error.IndexOf ('.'));
					string arg_col  = error.Substring (arg_line.Length+1, error.IndexOf (':')-arg_line.Length-1);
					
					int line;
					int col;
					
					Types.Converter.Convert (arg_line, out line);	line -= 1;
					Types.Converter.Convert (arg_col, out col);		col  -= 1;
					
					System.Diagnostics.Debug.WriteLine (error);
					System.Diagnostics.Debug.WriteLine (lines[line]);
					
					string method_signature;
					int    section_id;
					int    line_id;
					
					if (Source.Find (lines, line, col, out method_signature, out section_id, out line_id))
					{
						Source.Method method = this.source.FindMethod (method_signature);
						
						if ((method != null) &&
							(section_id < method.CodeSections.Length))
						{
							Source.CodeSection code = method.CodeSections[section_id];
							code.HiliteError (line_id, col, this.errors.Count);
							
							this.errors.Add (new Error (method, section_id, line_id, col, error));
						}
					}
				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("OK...");
			}
			
			this.UpdateFromSource ();
			this.UpdateCommandStates (false);
		}
		
		[Command ("FindNextError")]		void CommandFindNextError()
		{
			this.SyncFromUI ();
			
			if (this.errors.Count > 0)
			{
				int next = this.next_error;
				
				this.next_error = (next+1) < this.errors.Count ? next+1 : 0;
				
				Error error = this.errors[next] as Error;
				
				this.method_combo.SelectedName = error.Method.Signature;
				this.panel.SourceWidget.Cursor = Error.FindPosInRichText (this.panel.SourceWidget.Text, error.Line, error.Column);
			}
		}
		
		
		#region UniqueValueValidator Class
		public class UniqueValueValidator : Common.Widgets.Validators.AbstractTextValidator
		{
			public UniqueValueValidator() : base (null)
			{
			}
			
			public UniqueValueValidator(EditionController controller, Widget widget) : base (widget)
			{
				this.controller = controller;
			}
			
			
			protected override void ValidateText(string text)
			{
				this.state = Support.ValidationState.Ok;
				
				int max = this.controller.Source.Methods.Length;
				
				for (int i = 0; i < max; i++)
				{
					if (i != this.controller.method_index)
					{
						if (this.controller.Source.Methods[i].Name == text)
						{
							this.state = Support.ValidationState.Error;
							return;
						}
					}
				}
			}
			
			protected EditionController			controller;
		}
		#endregion
		
		public class Error
		{
			public Error(Source.Method method, int section_id, int line, int column, string description)
			{
				this.method      = method;
				this.section_id  = section_id;
				this.line        = line;
				this.column      = column;
				this.description = description;
			}
			
			
			public string						Description
			{
				get
				{
					return this.description;
				}
			}
			
			public int							Line
			{
				get
				{
					return this.line;
				}
			}
			
			public int							Column
			{
				get
				{
					return this.column;
				}
			}
			
			public Source.Method				Method
			{
				get
				{
					return this.method;
				}
			}
			
			public int							Section
			{
				get
				{
					return this.section_id;
				}
			}
			
			
			public static Error GetError(System.Collections.ArrayList errors, Source.Method method, int section_id, int line, int column)
			{
				Error best = null;
				
				foreach (Error error in errors)
				{
					if ((error.method == method) &&
						(error.section_id == section_id) &&
						(error.line == line) &&
						(error.column <= column))
					{
						best = error;
					}
				}
				
				return best;
			}
			
			public static bool FindPosInRichText(string rich_text, int cursor_index, out int line, out int column)
			{
				string   text  = System.Utilities.XmlBreakToText (rich_text);
				string[] lines = text.Split ('\n');
				
				line   = 0;
				column = 0;
				
				while (cursor_index > lines[line].Length)
				{
					cursor_index -= lines[line].Length + 1;
					
					if (++line >= lines.Length)
					{
						line   = -1;
						column = -1;
						
						return false;
					}
				}
				
				column = cursor_index;
				
				return true;
			}
			
			public static bool FindTagInRichText(string rich_text, int cursor_index, out int tag)
			{
				string   text  = rich_text.Replace ("<br/>", "\n");
				string[] lines = text.Split ('\n');
				
				int line   = 0;
				int column = 0;
				
				int line_length = System.Utilities.CountXmlChars (lines[line]);
				
				while (cursor_index > line_length)
				{
					cursor_index -= line_length + 1;
					
					if (++line >= lines.Length)
					{
						tag = -1;
						return false;
					}
					
					line_length = System.Utilities.CountXmlChars (lines[line]);
				}
				
				column = cursor_index;
				
				text = lines[line];
				
				int offset = System.Utilities.SkipXmlChars (text, cursor_index);
				int wave   = -1;
				
				for (;;)
				{
					int pos = text.IndexOf ("<w id=\"", wave+1);
					
					if ((pos > wave) &&
						(pos <= offset))
					{
						wave = pos;
					}
					else
					{
						break;
					}
				}
				
				if (wave >= 0)
				{
					int pos = text.IndexOf ("\">", wave+7);
					
					System.Diagnostics.Debug.Assert (pos > wave+7);
					
					string arg = text.Substring (wave+7, pos-wave-7);
					
					if (Types.Converter.Convert (arg, out tag))
					{
						return true;
					}
				}
				
				tag = -1;
				return false;
			}
			
			public static int  FindPosInRichText(string rich_text, int line, int column)
			{
				string   text  = System.Utilities.XmlBreakToText (rich_text);
				string[] lines = text.Split ('\n');
				
				int cursor_index = 0;
				
				for (int i = 0; i < line; i++)
				{
					cursor_index += System.Utilities.CountXmlChars (lines[i]) + 1;
				}
				
				return cursor_index + column;
			}
			
			
			private Source.Method				method;
			private int							section_id;
			private int							line;
			private int							column;
			private string						description;
		}
		
		protected CommandDispatcher				dispatcher;
		protected Source						source;
		protected Panels.MethodEditionPanel		panel;
		protected ToolTip						tool_tip;
		protected HToolBar						tool_bar;
		protected TextFieldCombo				method_combo;
		protected Button						compile_button;
		protected Button						find_next_error_button;
		protected int							method_index;
		protected bool							edit_array_focused;
		protected System.Collections.ArrayList	errors = new System.Collections.ArrayList ();
		protected int							next_error;
		
		protected CommandState					save_command_state;
		protected CommandState					compile_command_state;
		protected CommandState					find_error_command_state;
	}
}
