//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;

namespace Epsitec.Common.Script
{
	/// <summary>
	/// Summary description for Script.
	/// </summary>
	public class Script : System.MarshalByRefObject, System.IDisposable, Glue.IScriptHost, ICommandDispatcher
	{
		internal Script()
		{
		}
		
		
		public string[]						Errors
		{
			get
			{
				return this.errors;
			}
		}
		
		public bool							HasErrors
		{
			get
			{
				return (this.errors.Length > 0);
			}
		}
		
		public System.AppDomain				AppDomain
		{
			get
			{
				return this.domain;
			}
		}
		
		
		public void Attach(Types.IDataGraph data)
		{
			this.data = data;
		}
		
		
		public bool Execute(string name)
		{
			object[] out_args;
			return this.Execute (name, null, out out_args);
		}
		
		public bool Execute(string name, object[] in_args)
		{
			object[] out_args;
			return this.Execute (name, in_args, out out_args);
		}
		
		public bool Execute(string name, object[] in_args, out object[] out_args)
		{
			return this.script.Execute (name, in_args, out out_args);
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		#region IScriptHost Members
		public string						Name
		{
			get
			{
				return this.domain.FriendlyName;
			}
		}
		
		
		public bool ExecuteCommand(string command)
		{
			return false;
		}
		
		public bool WriteData(string name, object value)
		{
			System.Diagnostics.Debug.WriteLine ("Asked to write data " + name + " of type " + value.GetType ().Name);
			
			if (this.data != null)
			{
				Types.IDataItem  item = this.data.Navigate (name);
				Types.IDataValue data = item as Types.IDataValue;
				
				if (data != null)
				{
					data.WriteValue (value);
					return true;
				}
			}
			
			return false;
		}
		
		public void SetEnableState(string name, bool mode)
		{
		}
		
		public bool ReadData(string name, System.Type type, out object value)
		{
			System.Diagnostics.Debug.WriteLine ("Asked to read data " + name + " of type " + type.Name);
			
			if (this.data != null)
			{
				Types.IDataItem  item = this.data.Navigate (name);
				Types.IDataValue data = item as Types.IDataValue;
				
				if (data != null)
				{
					if (Types.InvariantConverter.Convert (data.ReadValue (), type, out value))
					{
						return true;
					}
				}
			}
			
			value = null;
			return false;
		}
		
		public bool GetEnableState(string name)
		{
			return false;
		}
		#endregion
		
		#region ICommandDispatcher Members
		bool ICommandDispatcher.DispatchCommand(CommandDispatcher sender, CommandEventArgs e)
		{
			return this.Execute (e.Command.Name, new string[0]);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.domain != null)
				{
					System.AppDomain.Unload (this.domain);
					this.domain = null;
				}
				
				if (this.dll_file_name != null)
				{
					System.IO.File.Delete (this.dll_file_name);
				}
				
				if (this.pdb_file_name != null)
				{
					System.IO.File.Delete (this.pdb_file_name);
				}
			}
		}
		
		
		internal void DefineErrors(string[] errors)
		{
			this.errors = errors;
		}
		
		internal void DefineAppDomain(System.AppDomain domain)
		{
			this.domain = domain;
		}
		
		internal void DefineDllFileName(string name)
		{
			this.dll_file_name = name;
		}
		
		internal void DefinePdbFileName(string name)
		{
			this.pdb_file_name = name;
		}
		
		internal void DefineScript(Glue.IScript script)
		{
			this.script = script;
			this.script.SetScriptHost (this);
		}
		
		
		private string[]					errors = new string[0];
		private System.AppDomain			domain;
		private Glue.IScript				script;
		private string						dll_file_name;
		private string						pdb_file_name;
		private Types.IDataGraph			data;
	}
}
