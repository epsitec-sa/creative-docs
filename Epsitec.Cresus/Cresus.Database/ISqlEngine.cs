//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface ISqlEngine permet d'exécuter des commandes SQL préparées par
	/// ISqlBuilder.
	/// </summary>
	public interface ISqlEngine
	{
		void Execute(System.Data.IDbCommand command, DbCommandType type, int command_count, out int result);
		void Execute(System.Data.IDbCommand command, DbCommandType type, int command_count, out object simple_data);
		void Execute(System.Data.IDbCommand command, DbCommandType type, int command_count, out System.Data.DataSet data_set);
		void Execute(DbInfrastructure infrastructure, System.Data.IDbTransaction transaction, DbRichCommand command);
	}
}
