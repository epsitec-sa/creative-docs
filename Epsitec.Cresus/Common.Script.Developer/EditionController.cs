//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			this.dispatcher = new CommandDispatcher ("EditionController", CommandDispatcherLevel.Primary);
			this.dispatcher.RegisterController (this);
			
			this.save_command_state = CommandState.Find ("SaveSource");
			this.compile_command_state = CommandState.Find ("CompileSourceCode");
			this.next_error_command_state = CommandState.Find ("FindNextError");
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
		
		public string							Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}
		
		
		public void CreateWidgets(Widget parent)
		{
			this.panel    = new Panels.MethodEditionPanel ();
			this.tool_bar = new HToolBar (parent);
			this.tool_tip = new ToolTip ();
			
			this.tool_bar.Dock = DockStyle.Top;
			
			TabPage page = new TabPage ();
			
			page.TabTitle = "Source";
			
			this.method_book = new TabBook (parent);
			this.method_book.HasCloseButton = true;
			this.method_book.Dock    = DockStyle.Fill;
			this.method_book.Margins = new Drawing.Margins (4, 4, 4, 4);
			this.method_book.Items.Add (page);
			
			this.method_book.ActivePageChanged += new EventHandler (this.HandleBookActivePageChanged);
			this.method_book.CloseClicked      += new EventHandler (this.HandleBookCloseClicked);
			
			this.tool_tip.Behaviour = ToolTipBehaviour.Manual;
			
			this.compile_button = new Button ();
			this.compile_button.Text = "Compile";
			this.compile_button.Command = "CompileSourceCode";
			this.compile_button.Shortcuts.Add (new Shortcut (KeyCode.FuncF7));
			
			this.find_prev_error_button = new Button ();
			this.find_prev_error_button.PreferredWidth = 20;
			this.find_prev_error_button.Text = "&lt;";
			this.find_prev_error_button.Command = "FindNextError(-1)";
			this.find_prev_error_button.Shortcuts.Add (new Shortcut (KeyCode.FuncF8 | KeyCode.ModifierShift));
			
			this.find_next_error_button = new Button ();
			this.find_next_error_button.PreferredWidth = 20;
			this.find_next_error_button.Text = "&gt;";
			this.find_next_error_button.Command = "FindNextError(1)";
			this.find_next_error_button.Shortcuts.Add (new Shortcut (KeyCode.FuncF8));
			
			this.panel.Widget.SetParent (page);
			this.panel.Widget.Dock    = DockStyle.Fill;
			this.panel.Widget.Margins = new Drawing.Margins (4, 4, 4, 4);
			
			this.panel.IsModifiedChanged += new EventHandler (this.HandlePanelIsModifiedChanged);
			this.panel.SourceWidget.CursorChanged += new EventHandler (this.HandleSourceCursorChanged);
			
			this.UpdateToolBar ();
			this.UpdateFromSource ();
			this.UpdateCommandStates ();
			
			new UniqueValueValidator (this, this.panel.MethodProtoPanel.MethodNameWidget);
			
			this.panel.MethodProtoPanel.MethodNameWidget.TextChanged += new EventHandler (this.HandleMethodNameWidgetTextChanged);
			
			parent.Window.FocusedWidgetChanged += new EventHandler (this.HandleWindowFocusedWidgetChanged);
		}
		
		public void ShowMethod(string signature)
		{
			this.UpdateMethodBook ();
			
			foreach (TabPage page in this.method_book.Items)
			{
				if (page.Name == signature)
				{
					this.method_book.ActivePage = page;
					return;
				}
				
				if (this.Source.Methods[page.Index].Name == signature)
				{
					this.method_book.ActivePage = page;
					return;
				}
			}
		}
		
		
		protected virtual void UpdateToolBar()
		{
			CommandDispatcher.SetDispatcher (this.tool_bar, this.dispatcher);
			
			this.tool_bar.Items.Clear ();
			this.tool_bar.Items.Add (IconButton.CreateSimple ("NewMethod", "manifest:Epsitec.Common.Script.Developer.Images.NewMethod.icon"));
			this.tool_bar.Items.Add (IconButton.CreateSimple ("SaveSource", "manifest:Epsitec.Common.Script.Developer.Images.Save.icon"));
			this.tool_bar.Items.Add (new IconSeparator ());
			this.tool_bar.Items.Add (this.compile_button);
			this.tool_bar.Items.Add (this.find_prev_error_button);
			this.tool_bar.Items.Add (this.find_next_error_button);
		}
		
		protected virtual void UpdateFromSource()
		{
			if (this.panel != null)
			{
				this.panel.Method = this.source.Methods[this.method_index];
				
				this.UpdateMethodBook ();
				
				object c_from = this.source_cursor_from[this.method_index];
				object c_to   = this.source_cursor_to[this.method_index];
				
				if ((c_from != null) &&
					(c_to   != null))
				{
					this.panel.SourceWidget.CursorFrom = (int) c_from;
					this.panel.SourceWidget.CursorTo   = (int) c_to;
				}
			}
		}
		
		protected virtual void UpdateMethodBook()
		{
			if (this.source.Methods.Length != this.method_book.Items.Count)
			{
				this.panel.Widget.SetParent (null);
				
				this.method_book.Items.Clear ();
				
				foreach (Source.Method method in this.source.Methods)
				{
					TabPage page = new TabPage ();
					
					page.Name     = method.Signature;
					page.TabTitle = method.Name;
					page.TabButton.AutoFocus = false;
					
					this.method_book.Items.Add (page);
				}
			}
			else
			{
				for (int i = 0; i < this.source.Methods.Length; i++)
				{
					TabPage       page   = this.method_book.Items[i];
					Source.Method method = this.source.Methods[i];
					
					page.Name     = method.Signature;
					page.TabTitle = method.Name;
					page.TabButton.AutoFocus = false;
				}
			}
			
			this.UpdateVisiblePage ();
		}
		
		protected virtual void UpdateCommandStates()
		{
			this.save_command_state.Enable       = this.panel.IsModified;
			this.compile_command_state.Enable    = this.source.Methods.Length > 0;
			this.next_error_command_state.Enable = this.errors.Count > 0;
		}
		
		protected virtual void UpdateVisiblePage()
		{
			this.method_book.ActivePageIndex = this.method_index;
			this.panel.Widget.SetParent (this.method_book.ActivePage);
		}
		
		protected virtual void UpdateErrorMessage()
		{
			TextFieldMulti text = this.panel.SourceWidget;
			
			int tag;
			
			if ((text.TextLayout.IsSelectionWaved (text.CursorFrom, true)) &&
				(Error.FindTagInRichText (text.Text, text.Cursor, out tag)) &&
				(tag >= 0) &&
				(tag < this.errors.Count))
			{
				Error error = this.errors[tag] as Error;
				
				this.UpdateErrorMessage (error);
			}
			else
			{
				this.UpdateErrorMessage (null);
			}
		}
				
		protected virtual void UpdateErrorMessage(Error error)
		{
			TextFieldMulti text = this.panel.SourceWidget;
			
			Drawing.Point p1, p2;
			
			if ((error != null) &&
				(text.GetCursorPosition (out p1, out p2)))
			{
				this.tool_tip.InitialLocation = text.MapClientToScreen (p1);
				this.tool_tip.SetToolTip (text, error.Description);
				this.tool_tip.ShowToolTipForWidget (text);
			}
			else
			{
				this.tool_tip.HideToolTipForWidget (text);
			}
		}
		
		
		protected void SyncFromUI()
		{
			if (this.method_index != -1)
			{
				this.source_cursor_from[this.method_index] = this.panel.SourceWidget.CursorFrom;
				this.source_cursor_to[this.method_index]   = this.panel.SourceWidget.CursorTo;
				
				this.tool_tip.HideToolTipForWidget (this.panel.SourceWidget);
				
				this.source.Methods[this.method_index] = this.panel.Method;
			}
		}
		
		protected void FocusSource()
		{
			this.panel.SourceWidget.Focus ();
		}
		
		protected void SaveSource()
		{
			this.source.NotifyChanged ();
		}
		
		protected void CompileSource()
		{
			string source = this.source.GenerateAssemblySource ();
			Engine engine = new Engine ();
			Script script = engine.Compile (this.name, source);
			
			foreach (Source.Method method in this.source.Methods)
			{
				foreach (Source.CodeSection section in method.CodeSections)
				{
					section.HiliteError (-1, -1, -1);
				}
			}
			
			this.errors.Clear ();
			this.next_error = -1;
			
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
					
					if (Source.Find (lines, line, ref col, out method_signature, out section_id, out line_id))
					{
						Source.Method method = this.source.FindMethod (method_signature);
						
						if ((method != null) &&
							(section_id < method.CodeSections.Length))
						{
							Source.CodeSection code = method.CodeSections[section_id];
							code.HiliteError (line_id, col, this.errors.Count);
							
							this.errors.Add (new Error (method, section_id, line_id, col, error.Substring (error.IndexOf (":") + 2)));
						}
					}
				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("OK...");
			}
			
			this.UpdateFromSource ();
			this.UpdateCommandStates ();
		}
		
		protected void FindNextError(int dir)
		{
			if (this.errors.Count > 0)
			{
				int next = this.next_error;
				
				if (dir > 0)
				{
					next++;
				}
				else
				{
					next--;
				}
				
				if (next < 0)
				{
					next = this.errors.Count-1;
				}
				else if (next >= this.errors.Count)
				{
					next = 0;
				}
				
				this.next_error = next;
				
				Error error = this.errors[next] as Error;
				
				this.ShowMethod (error.Method.Signature);
				this.panel.SourceWidget.Cursor = Error.FindPosInRichText (this.panel.SourceWidget.Text, error.Line, error.Column);
				this.UpdateErrorMessage (error);
			}
		}
		
		protected void AddNewMethod(string name)
		{
			//	TODO: déterminer le nom de la nouvelle méthode
			//	TODO: ajouter une méthode
		}
		
		
		private void HandlePanelIsModifiedChanged(object sender)
		{
			this.UpdateCommandStates ();
		}
		
		private void HandleSourceCursorChanged(object sender)
		{
			this.UpdateErrorMessage ();
		}
		
		private void HandleBookActivePageChanged(object sender)
		{
			if (this.is_changing_page)
			{
				return;
			}
			
			try
			{
				this.is_changing_page = true;
				
				if (this.method_index != this.method_book.ActivePageIndex)
				{
					//	En retirant le panel de son parent, on force automatiquement la mise à jour
					//	des éventuels champs qui avaient encore le focus (en particulier, le nom de
					//	la méthode) :
					
					this.panel.Widget.SetParent (null);
					this.SyncFromUI ();
					
					this.tool_tip.HideToolTipForWidget (this.panel.SourceWidget);
					
					this.method_index = this.method_book.ActivePageIndex;
					
					this.UpdateVisiblePage ();
					this.UpdateFromSource ();
					this.FocusSource ();
				}
			}
			finally
			{
				this.is_changing_page = false;
			}
		}
		
		private void HandleBookCloseClicked(object sender)
		{
			//	TODO: supprime la méthode active, après avoir posé la question...
		}
		
		private void HandleWindowFocusedWidgetChanged(object sender)
		{
#if false
			EditArray edit  = this.panel.ParameterInfoPanel.EditArray;
			bool      value = edit.ContainsKeyboardFocus;
			
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
#endif
		}
		
		private void HandleMethodNameWidgetTextChanged(object sender)
		{
			this.method_book.ActivePage.TabTitle = this.panel.MethodProtoPanel.MethodNameWidget.Text;
		}
		
		
		[Command ("SaveSource")]		void CommandSaveSource()
		{
			this.FocusSource ();
			this.SyncFromUI ();
			this.panel.IsModified = false;
			this.SaveSource ();
		}
		
		[Command ("NewMethod")]			void CommandNewMethod()
		{
			this.FocusSource ();
			this.AddNewMethod (this.source.Methods.Length == 0 ? "Script" : "Routine");
			this.SyncFromUI ();
			this.panel.IsModified = true;
		}
		
		[Command ("CompileSourceCode")]	void CommandCompileSourceCode()
		{
			this.FocusSource ();
			this.SyncFromUI ();
			this.CompileSource ();
			this.FindNextError (1);
		}
		
		[Command ("FindNextError")]		void CommandFindNextError(CommandDispatcher d, CommandEventArgs e)
		{
			this.FocusSource ();
			this.SyncFromUI ();
			
			int dir = 1;
			
			if (e.CommandArgs.Length >= 1)
			{
				Types.Converter.Convert (e.CommandArgs[0], out dir);
			}
			
			this.FindNextError (dir);
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
				this.state = ValidationState.Ok;
				
				int max = this.controller.Source.Methods.Length;
				
				for (int i = 0; i < max; i++)
				{
					if (i != this.controller.method_index)
					{
						if (this.controller.Source.Methods[i].Name == text)
						{
							this.state = ValidationState.Error;
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
				
				line = System.Math.Min (line, lines.Length);
				
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
		protected string						name;
		protected Source						source;
		protected System.Collections.Hashtable	source_cursor_from = new System.Collections.Hashtable ();
		protected System.Collections.Hashtable	source_cursor_to   = new System.Collections.Hashtable ();
		protected Panels.MethodEditionPanel		panel;
		protected ToolTip						tool_tip;
		protected HToolBar						tool_bar;
		protected TabBook						method_book;
		protected Button						compile_button;
		protected Button						find_prev_error_button;
		protected Button						find_next_error_button;
		protected int							method_index;
		protected bool							edit_array_focused;
		protected System.Collections.ArrayList	errors = new System.Collections.ArrayList ();
		protected int							next_error;
		protected bool							is_changing_page;
		
		protected CommandState					save_command_state;
		protected CommandState					compile_command_state;
		protected CommandState					next_error_command_state;
	}
}
