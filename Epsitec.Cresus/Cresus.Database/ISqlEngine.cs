//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 27/11/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface ISqlEngine permet d'ex�cuter des commandes SQL pr�par�es par
	/// ISqlBuilder.
	/// </summary>
	public interface ISqlEngine
	{
		void Execute(System.Data.IDbCommand command, DbCommandType type, int command_count);
		void Execute(System.Data.IDbCommand command, DbCommandType type, int command_count, out object simple_data);
		void Execute(System.Data.IDbCommand command, DbCommandType type, int command_count, out System.Data.DataSet data_set);
		void Execute(DbRichCommand command);
	}
}
