//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script.Glue
{
	/// <summary>
	/// L'interface IScript est réduite au minimum : c'est au travers d'elle
	/// que la communication entre une application et un script se fait, en
	/// franchissant les frontières des AppDomains.
	/// </summary>
	public interface IScript
	{
		void SetScriptHost(IScriptHost host);
		
		bool Execute(string name, object[] in_args, out object[] out_args);
	}
}
