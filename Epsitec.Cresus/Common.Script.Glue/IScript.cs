//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script.Glue
{
	/// <summary>
	/// Summary description for IScript.
	/// </summary>
	public interface IScript
	{
		void SetScriptHost(IScriptHost host);
		
		bool Execute(string name, object[] in_args, out object[] out_args);
	}
}
