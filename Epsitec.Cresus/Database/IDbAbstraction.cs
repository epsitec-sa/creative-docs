//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IDbAbstraction est utilisée pour accéder aux mécanismes ADO.NET
	/// dépendants d'un provider spécifique.
	/// </summary>
	public interface IDbAbstraction
	{
		IDbAbstractionFactory		Factory				{ get; }
		System.Data.IDbConnection	Connection			{ get; }
		ISqlBuilder					SqlBuilder			{ get; }
		
		bool						IsConnectionOpen	{ get; }
		bool						IsConnectionAlive	{ get; }
		
		string[]					UserTableNames		{ get; }
		
		System.Data.IDbCommand NewDbCommand();
		System.Data.IDataAdapter NewDataAdapter(System.Data.IDbCommand command);
		System.Data.IDbTransaction BeginTransaction();
	}
}
