//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using FirebirdSql.Data.FirebirdClient;

namespace Epsitec.Cresus.Database.Implementation
{
	using Epsitec.Cresus.Database;
	
	/// <summary>
	/// Implémentation de ISqlEngine pour Firebird.
	/// </summary>
	internal class FirebirdSqlEngine : ISqlEngine, System.IDisposable
	{
		public FirebirdSqlEngine(FirebirdAbstraction fb)
		{
			this.fb = fb;
		}
		
		
		#region ISqlEngine Members
		public void Execute(System.Data.IDbCommand command, DbCommandType type, int command_count, out int result)
		{
			result = 0;
			
			switch (type)
			{
				case DbCommandType.Silent:
				case DbCommandType.NonQuery:
				case DbCommandType.ReturningData:
					break;
				
				default:
					throw new Exceptions.GenericException (this.fb.DbAccess, "Illegal command type");
			}
			
			if (command_count > 1)
			{
				string[] commands = command.CommandText.Split ('\n');
				
				for (int i = 0; i < commands.Length; i++)
				{
					if (commands[i].Length > 0)
					{
						command.CommandText = commands[i];
						result += command.ExecuteNonQuery ();
					}
				}
			}
			else if (command_count > 0)
			{
				result += command.ExecuteNonQuery ();
			}
			
		}
		
		public void Execute(System.Data.IDbCommand command, DbCommandType type, int command_count, out object simple_data)
		{
			if (command_count > 1)
			{
				throw new Exceptions.GenericException (this.fb.DbAccess, "Multiple command not supported");
			}
			if (command_count < 1)
			{
				simple_data = null;
				return;
			}
			
			switch (type)
			{
				case DbCommandType.Silent:
				case DbCommandType.NonQuery:
					simple_data = command.ExecuteNonQuery ();
					break;
				
				case DbCommandType.ReturningData:
					simple_data = command.ExecuteScalar ();
					break;
				
				default:
					throw new Exceptions.GenericException (this.fb.DbAccess, "Illegal command type");
			}
		}
		
		public void Execute(System.Data.IDbCommand command, DbCommandType type, int command_count, out System.Data.DataSet data_set)
		{
			if (command_count < 1)
			{
				data_set = null;
				return;
			}
			
			int result;
			
			switch (type)
			{
				case DbCommandType.Silent:
				case DbCommandType.NonQuery:
					this.Execute (command, type, command_count, out result);
					data_set = null;
					break;
				
				case DbCommandType.ReturningData:
					System.Data.IDataAdapter adapter = this.fb.NewDataAdapter (command);
					data_set = new System.Data.DataSet ();
					adapter.Fill (data_set);
					break;
				
				default:
					throw new Exceptions.GenericException (this.fb.DbAccess, "Illegal command type");
			}
		}
		
		public void Execute(DbInfrastructure infrastructure, System.Data.IDbTransaction transaction, DbRichCommand rich_command)
		{
			int num_commands = rich_command.Commands.Count;
			
			System.Data.IDbDataAdapter[] adapters = new System.Data.IDbDataAdapter[num_commands];
			
			for (int i = 0; i < num_commands; i++)
			{
				System.Data.IDbCommand command = rich_command.Commands[i];
				
				FbDataAdapter    adapter = this.fb.NewDataAdapter (command) as FbDataAdapter;
				FbCommandBuilder builder = new FbCommandBuilder (adapter);
				
				adapters[i] = adapter;
			}
				
			rich_command.InternalFillDataSet (this.fb.DbAccess, transaction, adapters);
		}
		#endregion
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}
		
		
		private FirebirdAbstraction		fb;
	}
}
