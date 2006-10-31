//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using FirebirdSql.Data.FirebirdClient;

namespace Epsitec.Cresus.Database.Implementation
{
	/// <summary>
	/// The <c>FirebirdSqlEngine</c> class is the implementation of the <c>ISqlEngine</c>
	/// interface for the Firebird engine.
	/// </summary>
	internal class FirebirdSqlEngine : ISqlEngine, System.IDisposable
	{
		public FirebirdSqlEngine(FirebirdAbstraction fb)
		{
			this.fb = fb;
		}
		
		#region ISqlEngine Members
		
		public void Execute(System.Data.IDbCommand command, DbCommandType type, int commandCount, out int result)
		{
			result = 0;
			
			switch (type)
			{
				case DbCommandType.Silent:
				case DbCommandType.NonQuery:
				case DbCommandType.ReturningData:
					break;
				
				default:
					throw new Exceptions.GenericException (this.fb.DbAccess, string.Format ("Illegal command type {0}", type));
			}
			
			if (commandCount > 1)
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
			else if (commandCount > 0)
			{
				result += command.ExecuteNonQuery ();
			}
		}
		
		public void Execute(System.Data.IDbCommand command, DbCommandType type, int commandCount, out object simpleData)
		{
			if (commandCount > 1)
			{
				throw new Exceptions.GenericException (this.fb.DbAccess, "Multiple command not supported");
			}
			if (commandCount < 1)
			{
				simpleData = null;
				return;
			}
			
			switch (type)
			{
				case DbCommandType.Silent:
				case DbCommandType.NonQuery:
					simpleData = command.ExecuteNonQuery ();
					break;
				
				case DbCommandType.ReturningData:
					simpleData = command.ExecuteScalar ();
					break;
				
				default:
					throw new Exceptions.GenericException (this.fb.DbAccess, "Illegal command type");
			}
		}
		
		public void Execute(System.Data.IDbCommand command, DbCommandType type, int commandCount, out System.Data.DataSet dataSet)
		{
			if (commandCount < 1)
			{
				dataSet = null;
				return;
			}
			
			int result;
			
			switch (type)
			{
				case DbCommandType.Silent:
				case DbCommandType.NonQuery:
					this.Execute (command, type, commandCount, out result);
					dataSet = null;
					break;
				
				case DbCommandType.ReturningData:
					System.Data.IDataAdapter adapter = this.fb.NewDataAdapter (command);
					dataSet = new System.Data.DataSet ();
					adapter.Fill (dataSet);
					break;
				
				default:
					throw new Exceptions.GenericException (this.fb.DbAccess, "Illegal command type");
			}
		}
		
		public void Execute(DbInfrastructure infrastructure, System.Data.IDbTransaction transaction, DbRichCommand richCommand)
		{
			int numCommands = richCommand.Commands.Count;
			
			System.Data.IDbDataAdapter[] adapters = new System.Data.IDbDataAdapter[numCommands];
			
			for (int i = 0; i < numCommands; i++)
			{
				System.Data.IDbCommand command = richCommand.Commands[i];
				
				FbDataAdapter    adapter = this.fb.NewDataAdapter (command) as FbDataAdapter;
				FbCommandBuilder builder = new FbCommandBuilder (adapter);
				
				adapters[i] = adapter;
			}
				
			richCommand.InternalFillDataSet (this.fb.DbAccess, transaction, adapters);
		}
		
		#endregion
		
		#region IDisposable Members
		
		public void Dispose()
		{
		}
		
		#endregion
		
		private FirebirdAbstraction		fb;
	}
}
