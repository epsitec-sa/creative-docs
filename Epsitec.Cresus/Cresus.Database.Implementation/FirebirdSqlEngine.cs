//	Copyright � 2003-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
		
		public void Execute(DbRichCommand richCommand, DbInfrastructure infrastructure, DbTransaction transaction)
		{
			int numCommands = richCommand.Commands.Count;
			
			System.Data.IDbDataAdapter[] adapters = new System.Data.IDbDataAdapter[numCommands];
			
			for (int i = 0; i < numCommands; i++)
			{
				System.Data.IDbCommand command = richCommand.Commands[i];
				
				FbDataAdapter    adapter = this.fb.NewDataAdapter (command) as FbDataAdapter;
				FbCommandBuilder builder = new FbCommandBuilder (adapter);

				adapters[i] = adapter;
				
#if true
				//	HACK: find another way of doing this
				//	TODO: set transaction on the update command too in DbRichCommand.SetCommandTransaction

				//	FbCommandBuilder.RowUpdatingHandler pourrait b�n�ficier du rajout
				//	suivant: e.Command.Transaction = this.DataAdapter.SelectCommand.Transaction
				//	ce qui permettrait de garantir que l'on utilise le bon objet de transaction;
				//	en attendant, on d�tecte la mise � jour via la commande et on s'assure nous-
				//	m�me que la transaction est bien correcte :

				adapter.RowUpdating +=
					delegate (object sender, FbRowUpdatingEventArgs e)
					{
						var activeTransaction = richCommand.GetActiveTransaction ().Transaction as FbTransaction;

						if ((e.Command != null) &&
							(e.Command.Transaction != activeTransaction))
						{
							System.Diagnostics.Debug.WriteLine (string.Format ("Fixed missing transaction for command:\n   {0}", e.Command.CommandText));
							e.Command.Transaction = activeTransaction;
						}
					};
#endif
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
