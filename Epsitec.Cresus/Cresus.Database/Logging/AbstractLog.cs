using Epsitec.Common.Support.Extensions;

using System;

using System.Collections.Generic;

using System.Collections.ObjectModel;

using System.Data;

using System.Diagnostics;

using System.Linq;

using System.Threading;


namespace Epsitec.Cresus.Database.Logging
{


	// TODO Comment this class.
	// Marc
	
	
	public abstract class AbstractLog
	{


		internal AbstractLog()
		{
			this.LogResult = false;
			this.LogStackTrace = false;
			this.LogThreadName = false;
		}


		public bool LogResult
		{
			get;
			set;
		}


		public bool LogStackTrace
		{
			get;
			set;
		}


		public bool LogThreadName
		{
			get;
			set;
		}


		public abstract void Clear();


		public abstract int GetNbEntries();


		public abstract Query GetEntry(int index);


		public abstract ReadOnlyCollection<Query> GetEntries(int index, int count);


		protected abstract void AddEntry(Query query);


		protected abstract int GetNextNumber();


		internal void AddEntry(IDbCommand command, DateTime startTime, TimeSpan duration)
		{
			command.ThrowIfNull ("command");

			Result result = null;
			Query query = this.GetQuery (command, result, startTime, duration);

			this.AddEntry (query);
		}


		internal void AddEntry(IDbCommand command, DateTime startTime, TimeSpan duration, object data)
		{
			command.ThrowIfNull ("command");

			Result result = this.LogResult ? AbstractLog.GetResult (data) : null;
			Query query = this.GetQuery (command, result, startTime, duration);

			this.AddEntry (query);
		}


		internal void AddEntry(IDbCommand command, DateTime startTime, TimeSpan duration, IList<object> data)
		{
			command.ThrowIfNull ("command");

			Result result = this.LogResult ? AbstractLog.GetResult (command, data) : null;
			Query query = this.GetQuery (command, result, startTime, duration);

			this.AddEntry (query);
		}


		internal void AddEntry(IDbCommand command, DateTime startTime, TimeSpan duration, DataSet data)
		{
			command.ThrowIfNull ("command");

			Result result = this.LogResult ? AbstractLog.GetResult (data) : null;
			Query query = this.GetQuery (command, result, startTime, duration);

			this.AddEntry (query);
		}


		private Query GetQuery(IDbCommand command, Result result, DateTime startTime, TimeSpan duration)
		{
			int number = this.GetNextNumber ();
			string sourceCode = AbstractLog.GetSourceCode (command);
			IEnumerable<Parameter> parameter = AbstractLog.GetParameters (command);
			string threadName = this.LogThreadName ? Thread.CurrentThread.Name : null;
			StackTrace stackTrace = this.LogStackTrace ? new StackTrace (2, true) : null;
			
			return new Query (number, startTime, duration, sourceCode, parameter, result, threadName, stackTrace);
		}


		private static String GetSourceCode(IDbCommand command)
		{
			return command.CommandText;
		}


		private static IEnumerable<Parameter> GetParameters(IDbCommand command)
		{
			return from IDataParameter parameter in command.Parameters
				   where parameter.Direction == ParameterDirection.Input
				   let name = parameter.ParameterName
				   let value = parameter.Value
				   select new Parameter (name, value);
		}


		private static Result GetResult(object data)
		{
			List<Column> columns = new List<Column> () { new Column ("result") };
			List<Row> rows = new List<Row> () { new Row (new List<object> { data }) };
			List<Table> tables = new List<Table> () { new Table ("result", columns, rows) };

			return new Result (tables);
		}


		private static Result GetResult(IDbCommand command, IList<object> data)
		{
			IEnumerable<Column> columns = from IDataParameter parameter in command.Parameters
										  where parameter.Direction == ParameterDirection.Output
										  let name = parameter.ParameterName
										  select new Column (name);

			IEnumerable<Row> rows = new List<Row> ()
			{
				new Row (data)
			};

			List<Table> tables = new List<Table> () { new Table ("result", columns, rows) };

			return new Result (tables);
		}


		private static Result GetResult(DataSet dataSet)
		{
			List<Table> tables = new List<Table> ();

			foreach (DataTable dataTable in dataSet.Tables)
			{
				string name = dataTable.TableName;

				IEnumerable<Column> columns = from DataColumn dataColumn in dataTable.Columns
											  select new Column (dataColumn.ColumnName);

				IEnumerable<Row> rows = from DataRow dataRow in dataTable.Rows
										select new Row (dataRow.ItemArray);

				tables.Add (new Table (name, columns, rows));
			}

			return new Result (tables);
		}


	}
	

}
