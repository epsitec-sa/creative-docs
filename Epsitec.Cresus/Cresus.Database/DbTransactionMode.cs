//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'énumération DbTransactionMode définit le type de transaction
	/// requise pour une opération donnée avec DbInfrastructure.
	/// </summary>
	public enum DbTransactionMode
	{
		Unsupported		= 0,
		Unknown			= Unsupported,
		
		ReadWrite		= 1,
		ReadOnly		= 2
	}
}
