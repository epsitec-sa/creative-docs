//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IOperatorService g�re la cr�ation de bases de donn�es
	/// "client" et leur dur�e de vie.
	/// </summary>
	public interface IOperatorService
	{
		void CreateRoamingClient(string client_name, out IOperation operation);
		
		void GetRoamingClientData(IOperation operation, out ClientIdentity client, out byte[] compressed_data);
	}
}
