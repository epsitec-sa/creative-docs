//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IOperatorService gère la création de bases de données
	/// "client" et leur durée de vie.
	/// </summary>
	public interface IOperatorService
	{
		void CreateRoamingClient(out IOperation operation);
		bool ReadRoamingClientData(IOperation operation, out byte[] compressed_data);
	}
}
