//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script.Glue
{
	/// <summary>
	/// Summary description for AbstractScriptBase.
	/// </summary>
	public abstract class AbstractScriptBase : System.MarshalByRefObject, Epsitec.Common.Script.Glue.IScript
	{
		public AbstractScriptBase()
		{
		}

		
		public IScriptHost					Host
		{
			get
			{
				return this.host;
			}
		}
		
		
		protected object ReadData(string name, System.Type type)
		{
			System.Diagnostics.Debug.WriteLine ("Read data '" + name + "' of type " + type.FullName);
			object value;
			
			if (this.host.ReadData (name, out value))
			{
				return value;
			}
			
			throw new System.ArgumentException (string.Format ("Data '{0}' not found in Script Host.", name));
		}
		
		public abstract bool Execute(string name, object[] in_args);
		
		#region Remaining IScript Members
		void Epsitec.Common.Script.Glue.IScript.SetScriptHost(IScriptHost host)
		{
			this.host = host;
		}
		#endregion
		
		private IScriptHost					host;
	}
}
