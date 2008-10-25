//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IOperatorService g�re la cr�ation de bases de donn�es
	/// "client" et leur dur�e de vie.
	/// </summary>
	public interface IOperatorService : IRemoteService
	{
		void CreateRoamingClient(string clientName, out IOperation operation);
		
		void GetRoamingClientData(IOperation operation, out ClientIdentity client, out byte[] data);
	}
}
