//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using System.Collections.ObjectModel;

using System.Runtime.Serialization;


namespace Epsitec.Cresus.Requests
{
	
	
	/// <summary>
	/// The <c>InsertStaticDataRequest</c> class describes a static data insertion.
	/// The data provided is plain static data which needs not be recomputed but
	/// can be used as is.
	/// </summary>
	[System.Serializable]
	public class InsertStaticDataRequest : AbstractDataRequest
	{
		
		
		public InsertStaticDataRequest() : base()
		{
		}

		
		/// <summary>
		/// Initializes a new instance of the <see cref="InsertStaticDataRequest"/> class.
		/// </summary>
		/// <param name="row">The source data row.</param>
		public InsertStaticDataRequest(System.Data.DataRow row) : this ()
		{
			System.Diagnostics.Debug.Assert (row != null);
			System.Diagnostics.Debug.Assert (row.Table != null);

			System.Data.DataTable            table   = row.Table;
			System.Data.DataColumnCollection columns = table.Columns;

			int n = columns.Count;

			this.TableName = table.TableName;

			this.columnNames  = new string[n];
			this.columnValues = (object[]) row.ItemArray.Clone ();

			for (int i = 0; i < n; i++)
			{
				this.columnNames[i] = columns[i].ColumnName;
			}
		}


		/// <summary>
		/// Gets the column names.
		/// </summary>
		/// <value>The column names.</value>
		public override ReadOnlyCollection<string>	ColumnNames
		{
			get
			{
				return new ReadOnlyCollection<string> (this.columnNames);
			}
		}

		/// <summary>
		/// Gets the column values.
		/// </summary>
		/// <value>The column values.</value>
		public override ReadOnlyCollection<object>	ColumnValues
		{
			get
			{
				return new ReadOnlyCollection<object> (this.columnValues);
			}
		}


		/// <summary>
		/// Executes the request using the specified execution engine.
		/// </summary>
		/// <param name="engine">The execution engine.</param>
		public override void Execute(ExecutionEngine engine)
		{
			engine.GenerateInsertDataCommand (this.TableName, this.columnNames, this.columnValues);
		}
		
		
		#region ISerializable Members


		protected InsertStaticDataRequest(SerializationInfo info, StreamingContext context) : base (info, context)
		{
			this.columnNames = info.GetValue (SerializationKeys.ColumnNames, typeof (string[])) as string[];
			this.columnValues = info.GetValue (SerializationKeys.ColumnValues, typeof (object[])) as object[];
		}


		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData (info, context);

			info.AddValue (SerializationKeys.ColumnNames, this.columnNames);
			info.AddValue (SerializationKeys.ColumnValues, this.columnValues);
		}
		
		#endregion


		#region SerializationKeys Class


		private static class SerializationKeys
		{
			public const string ColumnNames = "C.Names";
			public const string ColumnValues = "C.Values";
		}


		#endregion


		private readonly string[]						columnNames;
		private readonly object[]						columnValues;
	
	
	}


}
