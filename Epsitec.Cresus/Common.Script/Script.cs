//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script
{
	/// <summary>
	/// Summary description for Script.
	/// </summary>
	public class Script : System.MarshalByRefObject, System.IDisposable, Glue.IScriptHost
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
		
		
		public bool Execute(string name)
		{
			return this.script.Execute (name);
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
			return false;
		}
		
		public void SetEnableState(string name, bool mode)
		{
		}
		
		public bool ReadData(string name, out object value)
		{
			value = null;
			return false;
		}
		
		public bool GetEnableState(string name)
		{
			return false;
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
	}
}
