//	Copyright © 2003-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using FirebirdSql.Data.FirebirdClient;

using System.Collections.Generic;

using System.Data;


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

		public void Execute(IDbCommand command, DbCommandType type, int commandCount, out int result)
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
						result += this.ExecuteQueryAndConvertFbExceptions (() => command.ExecuteNonQuery ());
					}
				}
			}
			else if (commandCount > 0)
			{
				result += this.ExecuteQueryAndConvertFbExceptions (() => command.ExecuteNonQuery ());
			}

		}

		public void Execute(IDbCommand command, DbCommandType type, int commandCount, out object simpleData)
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
					simpleData = this.ExecuteQueryAndConvertFbExceptions (() => command.ExecuteNonQuery ());
					break;

				case DbCommandType.ReturningData:
					simpleData = this.ExecuteQueryAndConvertFbExceptions (() => command.ExecuteScalar ());
					break;

				default:
					throw new Exceptions.GenericException (this.fb.DbAccess, "Illegal command type");
			}
		}

		public void Execute(IDbCommand command, DbCommandType type, int commandCount, out IList<object> outputValues)
		{
			outputValues = new List<object> ();

			if (commandCount > 1)
			{
				throw new Exceptions.GenericException (this.fb.DbAccess, "Multiple command not supported");
			}
			if (commandCount < 1)
			{
				return;
			}

			switch (type)
			{
				case DbCommandType.Silent:
				case DbCommandType.NonQuery:
					this.ExecuteQueryAndConvertFbExceptions (() => command.ExecuteNonQuery ());
					break;

				case DbCommandType.ReturningData:
					this.ExecuteQueryAndConvertFbExceptions (() => command.ExecuteScalar ());
					break;

				default:
					throw new Exceptions.GenericException (this.fb.DbAccess, "Illegal command type");
			}

			foreach (object item in command.Parameters)
			{
				IDataParameter parameter = item as IDataParameter;

				if (parameter != null)
				{
					switch (parameter.Direction)
					{
						case ParameterDirection.InputOutput:
						case ParameterDirection.Output:
						case ParameterDirection.ReturnValue:
							outputValues.Add (parameter.Value);
							break;
					}
				}
			}
		}

		public void Execute(IDbCommand command, DbCommandType type, int commandCount, out DataSet dataSet)
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
					this.ExecuteQueryAndConvertFbExceptions (() => this.Execute (command, type, commandCount, out result));
					dataSet = null;
					break;

				case DbCommandType.ReturningData:
					dataSet = this.ExecuteQueryAndConvertFbExceptions (() =>
					{
						IDataAdapter adapter = this.fb.NewDataAdapter (command);
						DataSet ds = new DataSet ();
						adapter.Fill (ds);
						return ds;
					});
					break;

				default:
					throw new Exceptions.GenericException (this.fb.DbAccess, "Illegal command type");
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion

		private void ExecuteQueryAndConvertFbExceptions(System.Action query)
		{
			this.ExecuteQueryAndConvertFbExceptions (() =>
			{
				query ();
				return 0;
			});
		}


		private T ExecuteQueryAndConvertFbExceptions<T>(System.Func<T> query)
		{
			try
			{
				return query ();
			}
			catch (FbException e)
			{
				DbAccess dbAccess = this.fb.DbAccess;
				string message = "Could not execute query because a problem occurred : " + e.Message;

				throw new Database.Exceptions.GenericException (dbAccess, message, e);
			}
		}

		private FirebirdAbstraction		fb;
	}
}
