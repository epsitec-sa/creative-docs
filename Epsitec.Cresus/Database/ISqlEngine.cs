//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface ISqlEngine permet d'ex�cuter des commandes SQL pr�par�es par
	/// ISqlBuilder.
	/// </summary>
	public interface ISqlEngine
	{
		void Execute(System.Data.IDbCommand command, DbCommandType type);
		void Execute(System.Data.IDbCommand command, DbCommandType type, out object simple_data);
		void Execute(System.Data.IDbCommand command, DbCommandType type, out System.Data.DataSet data_set);
	}
}
