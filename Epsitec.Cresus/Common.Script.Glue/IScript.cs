//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script.Glue
{
	/// <summary>
	/// Summary description for IScript.
	/// </summary>
	public interface IScript
	{
		bool Execute(string name/*, object[] in_args, object[] out_args*/);
	}
}
