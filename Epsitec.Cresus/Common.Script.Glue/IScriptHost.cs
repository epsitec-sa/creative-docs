//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script.Glue
{
	/// <summary>
	/// L'interface IScriptHost doit être implémentée par la partie "hôte" qui crée
	/// le AppDomain et télécommande le script proprement dit; cette interface permet
	/// au script d'accéder à l'environnement de son hôte.
	/// </summary>
	public interface IScriptHost
	{
		string	Name	{ get; }
		
		void SetEnableState(string name, bool mode);
		bool GetEnableState(string name);
		
		bool ReadData(string name, System.Type type, out object value);
		bool WriteData(string name, object value);
		
		bool ExecuteCommand(string command);
	}
}
