//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IDbAbstraction est utilisée pour accéder aux mécanismes ADO.NET
	/// dépendants d'un provider spécifique.
	/// </summary>
	public interface IDbAbstraction : System.IDisposable
	{
		IDbAbstractionFactory		Factory				{ get; }
		System.Data.IDbConnection	Connection			{ get; }
		ISqlBuilder					SqlBuilder			{ get; }
		ISqlEngine					SqlEngine			{ get; }
		
		bool						IsConnectionOpen	{ get; }
		bool						IsConnectionAlive	{ get; }
		
		string[]					UserTableNames		{ get; }
		
		System.Data.IDbCommand NewDbCommand();
		System.Data.IDataAdapter NewDataAdapter(System.Data.IDbCommand command);
		System.Data.IDbTransaction BeginTransaction();
		
		void ReleaseConnection();
	}
}
