//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

using FirebirdSql.Data.Firebird;

namespace Epsitec.Cresus.Database.Implementation
{
	using Epsitec.Cresus.Database;
	
	/// <summary>
	/// Implémentation de ISqlEngine pour Firebird.
	/// </summary>
	public class FirebirdSqlEngine : ISqlEngine
	{
		public FirebirdSqlEngine(FirebirdAbstraction fb)
		{
			this.fb = fb;
		}
		
		
		#region ISqlEngine Members
		public void Execute(System.Data.IDbCommand command, DbCommandType type)
		{
			switch (type & DbCommandType.Mask)
			{
				case DbCommandType.Silent:
				case DbCommandType.NonQuery:
				case DbCommandType.ReturningData:
					break;
				
				default:
					throw new DbException (this.fb.DbAccess, "Illegal command type");
			}
			
			if ((type & DbCommandType.FlagMultiple) != 0)
			{
				using (System.Data.IDataReader reader = command.ExecuteReader ())
				{
					while (reader.NextResult ())
					{
						//	Il faut appeler NextResult plusieurs fois, jusqu'à ce qu'il retourne
						//	false avec Firebird, car c'est le seul moyen d'exécuter des commandes
						//	multiples.
					}
				}
			}
			else
			{
				command.ExecuteNonQuery ();
			}
			
		}
		
		public void Execute(System.Data.IDbCommand command, DbCommandType type, out object simple_data)
		{
			if ((type & DbCommandType.FlagMultiple) != 0)
			{
				throw new DbException (this.fb.DbAccess, "Multiple command not supported");
			}
			
			switch (type & DbCommandType.Mask)
			{
				case DbCommandType.Silent:
				case DbCommandType.NonQuery:
					simple_data = command.ExecuteNonQuery ();
					break;
				
				case DbCommandType.ReturningData:
					simple_data = command.ExecuteScalar ();
					break;
				
				default:
					throw new DbException (this.fb.DbAccess, "Illegal command type");
			}
		}
		
		public void Execute(System.Data.IDbCommand command, DbCommandType type, out System.Data.DataSet data_set)
		{
			switch (type & DbCommandType.Mask)
			{
				case DbCommandType.Silent:
				case DbCommandType.NonQuery:
					this.Execute (command, type);
					data_set = null;
					break;
				
				case DbCommandType.ReturningData:
					System.Data.IDataAdapter adapter = this.fb.NewDataAdapter (command);
					data_set = new System.Data.DataSet ();
					adapter.Fill (data_set);
					break;
				
				default:
					throw new DbException (this.fb.DbAccess, "Illegal command type");
			}
		}
		#endregion
		
		private FirebirdAbstraction		fb;
	}
}
