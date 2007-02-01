//	Copyright � 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script.Glue
{
	/// <summary>
	/// L'interface IScriptHost doit �tre impl�ment�e par la partie "h�te" qui cr�e
	/// le AppDomain et t�l�commande le script proprement dit; cette interface permet
	/// au script d'acc�der � l'environnement de son h�te.
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
