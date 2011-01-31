using Epsitec.Common.Support.Extensions;

using System;

using System.Collections.Generic;

using System.Data;

using System.Linq;


namespace Epsitec.Cresus.Database.Logging
{


	// TODO Comment this class.
	// Marc
	
	
	public abstract class AbstractLog
	{


		internal AbstractLog()
		{
			this.Mode = LogMode.Basic;
		}


		public LogMode Mode
		{
			get;
			set;
		}


		public abstract void Clear();


		public abstract int GetNbEntries();


		public abstract Query GetEntry(int index);


		internal abstract void AddEntry(Query query);


		internal void AddEntry(IDbCommand command, DateTime startTime, TimeSpan duration)
		{
			command.ThrowIfNull ("command");

			Result result = null;
			Query query = AbstractLog.GetQuery (command, result, startTime, duration);

			this.AddEntry (query);
		}


		internal void AddEntry(IDbCommand command, DateTime startTime, TimeSpan duration, object data)
		{
			command.ThrowIfNull ("command");

			Result result = this.DiscardResult () ? null : AbstractLog.GetResult (data); 
			Query query = AbstractLog.GetQuery (command, result, startTime, duration);

			this.AddEntry (query);
		}


		internal void AddEntry(IDbCommand command, DateTime startTime, TimeSpan duration, IList<object> data)
		{
			command.ThrowIfNull ("command");

			Result result = this.DiscardResult () ? null : AbstractLog.GetResult (command, data);
			Query query = AbstractLog.GetQuery (command, result, startTime, duration);

			this.AddEntry (query);
		}


		internal void AddEntry(IDbCommand command, DateTime startTime, TimeSpan duration, DataSet data)
		{
			command.ThrowIfNull ("command");

			Result result = this.DiscardResult () ? null : AbstractLog.GetResult (data);
			Query query = AbstractLog.GetQuery (command, result, startTime, duration);

			this.AddEntry (query);
		}


		private bool DiscardResult()
		{
			return this.Mode == LogMode.Basic;
		}


		private static Query GetQuery(IDbCommand command, Result result, DateTime startTime, TimeSpan duration)
		{
			string sourceCode = AbstractLog.GetSourceCode (command);
			IEnumerable<Parameter> parameter = AbstractLog.GetParameters (command);

			return new Query (sourceCode, parameter, result, startTime, duration);
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
