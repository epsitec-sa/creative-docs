//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script.Glue
{
	/// <summary>
	/// Summary description for ScriptFactory.
	/// </summary>
	public class ScriptFactory : System.MarshalByRefObject
	{
		public ScriptFactory()
		{
		}
		
		public IScript CreateScript(string name, IScriptHost host)
		{
			return null;
		}
	}
}
