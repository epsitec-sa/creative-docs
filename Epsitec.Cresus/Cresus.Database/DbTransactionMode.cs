//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'�num�ration DbTransactionMode d�finit le type de transaction
	/// requise pour une op�ration donn�e avec DbInfrastructure.
	/// </summary>
	public enum DbTransactionMode
	{
		Unsupported		= 0,
		Unknown			= Unsupported,
		
		ReadWrite		= 1,
		ReadOnly		= 2
	}
}
