//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script.Glue
{
	/// <summary>
	/// Summary description for IScriptHost.
	/// </summary>
	public interface IScriptHost
	{
		string	Name	{ get; }
		
		void SetEnableState(string name, bool mode);
		bool GetEnableState(string name);
		
		bool ReadData(string name, out object value);
		bool WriteData(string name, object value);
		
		bool ExecuteCommand(string command);
	}
}
