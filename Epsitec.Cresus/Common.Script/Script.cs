//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script
{
	/// <summary>
	/// Summary description for Script.
	/// </summary>
	public class Script : System.IDisposable
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
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.domain != null)
				{
					System.AppDomain.Unload (this.domain);
					this.domain = null;
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
		
		internal void DefineScript(Glue.IScript script)
		{
			this.script = script;
		}
		
		
		private string[]					errors = new string[0];
		private System.AppDomain			domain;
		private Glue.IScript				script;
	}
}
