//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Summary description for DialogDesigner.
	/// </summary>
	public class DialogDesigner : Epsitec.Common.Dialogs.IDialogDesigner
	{
		public DialogDesigner(Application application)
		{
			this.application = application;
		}
		
		
		public Application						Application
		{
			get
			{
				return this.application;
			}
		}
		
		
		#region IDialogDesigner Members
		public Epsitec.Common.Types.IDataGraph	DialogData
		{
			get
			{
				return this.dialog_data;
			}
			set
			{
				if (this.dialog_data != value)
				{
					this.dialog_data = value;
					this.OnDialogDataChanged ();
				}
			}
		}
		
		public Support.CommandDispatcher		DialogCommands
		{
			get
			{
				return this.dialog_commands;
			}
			set
			{
				if (this.dialog_commands != value)
				{
					this.dialog_commands = value;
					this.OnDialogCommandsChanged ();
				}
			}
		}
		
		public Script.IScriptSource				DialogScript
		{
			get
			{
				return this.dialog_script;
			}
			set
			{
				if (this.dialog_script != value)
				{
					this.DetachScriptDeveloper ();
					this.dialog_script = value;
					this.AttachScriptDeveloper ();
					this.OnDialogScriptChanged ();
				}
			}
		}
		
		public Window							DialogWindow
		{
			get
			{
				return this.dialog_window;
			}
			set
			{
				if (this.dialog_window != value)
				{
					this.DetachWindow ();
					this.dialog_window = value;
					this.AttachWindow ();
				}
			}
		}
		
		public string							ResourceName
		{
			get
			{
				return this.resource_name;
			}
			set
			{
				this.resource_name = value;
			}
		}
		
		public void StartDesign()
		{
			if (this.dialog_window != null)
			{
				this.application.Initialise ();
				this.application.MainWindow.Show ();
				this.application.InterfaceEditController.CreateEditorForWindow (this.dialog_window, this.resource_name);
			}
		}
		#endregion
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		
		public static DialogDesigner FromWindow(Window window)
		{
			return window == null ? null : window.GetProperty (DialogDesigner.prop_dialog_designer) as DialogDesigner;
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.OnDisposed ();
			}
		}
		
		
		protected virtual void AttachWindow()
		{
			if (this.dialog_window != null)
			{
				this.dialog_window.SetProperty (DialogDesigner.prop_dialog_designer, this);
			}
		}
		
		protected virtual void DetachWindow()
		{
			if (this.dialog_window != null)
			{
				this.dialog_window.ClearProperty (DialogDesigner.prop_dialog_designer);
			}
		}
		
		protected virtual void AttachScriptDeveloper()
		{
		}
		
		protected virtual void DetachScriptDeveloper()
		{
		}
		
		
		protected virtual void OnDialogDataChanged()
		{
			if (this.DialogDataChanged != null)
			{
				this.DialogDataChanged (this);
			}
		}
		
		protected virtual void OnDialogCommandsChanged()
		{
			if (this.DialogCommandsChanged != null)
			{
				this.DialogCommandsChanged (this);
			}
		}
		
		protected virtual void OnDialogScriptChanged()
		{
			if (this.DialogScriptChanged != null)
			{
				this.DialogScriptChanged (this);
			}
		}
		
		protected virtual void OnDisposed()
		{
			if (this.Disposed != null)
			{
				this.Disposed (this);
			}
		}
		
		
		
		public event Support.EventHandler		DialogDataChanged;
		public event Support.EventHandler		DialogCommandsChanged;
		public event Support.EventHandler		DialogScriptChanged;
		public event Support.EventHandler		Disposed;
		
		private Application						application;
		private Types.IDataGraph				dialog_data;
		private Window							dialog_window;
		private Support.CommandDispatcher		dialog_commands;
		private Script.IScriptSource			dialog_script;
		private string							resource_name;
		
		private const string					prop_dialog_designer = "$designer$dialog designer$";
	}
}
