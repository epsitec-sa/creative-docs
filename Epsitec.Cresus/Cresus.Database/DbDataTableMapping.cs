using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace Epsitec.Cresus.Database
{


	/// <summary>
	/// The <c>DbDataTableMapping</c> class maintains a mapping between the <see cref="DataRow"/>s of a
	/// <see cref="DataTable"/> and the values of a column. For instance, it allows other objects to quickly get
	/// all the rows of a table whose cr_source_id value is 1.
	/// </summary>
	class DbDataTableMapping
	{


		/// <summary>
		/// Gets the <see cref="DataTable"/> mapped by this instance.
		/// </summary>
		/// <value>The <see cref="DataTable"/>.</value>
		public DataTable DataTable
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the name of the column used for the mapping by this instance.
		/// </summary>
		/// <value>The name of the column.</value>
		public string ColumnName
		{
			get;
			private set;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="DbDataTableMapping"/> class.
		/// </summary>
		/// <param name="dataTable">The <see cref="DataTable"/> that will be mapped.</param>
		/// <param name="columnName">The name of the column used for the mapping.</param>
		public DbDataTableMapping(DataTable dataTable, string columnName)
		{
			this.DataTable = dataTable;
			this.ColumnName = columnName;

			this.ColumnIndex = dataTable.Columns.IndexOf (columnName);
			this.valueToRows = new Dictionary<long, List<DataRow>> ();
			this.rowToValue = new Dictionary<DataRow, long> ();

			this.PopulateCache ();
			this.SetupEventHandlers ();
		}


		private void PopulateCache()
		{
			foreach (DataRow dataRow in this.DataTable.Rows)
			{
				if (this.IsValid (dataRow))
				{
					this.AddDataRow (this.GetValue (dataRow), dataRow);
				}
			}
		}

		private bool IsValid(DataRow dataRow)
		{
			return dataRow.RowState != DataRowState.Detached;
		}

		private long GetValue(DataRow dataRow)
		{
			long value;

			switch (dataRow.RowState)
			{
				case DataRowState.Deleted:
					value = (long) dataRow[this.ColumnIndex, DataRowVersion.Original];
					break;

				case DataRowState.Detached:
					throw new System.InvalidOperationException();
					break;

				default:
					value = (long) dataRow[this.ColumnIndex];
					break;
			}

			return value;
		}


		private void AddDataRow(long value, DataRow dataRow)
		{
			if (!this.valueToRows.ContainsKey (value))
			{
				this.valueToRows[value] = new List<DataRow> ();
			}

			this.valueToRows[value].Add (dataRow);
			this.rowToValue[dataRow] = value;
		}


		private void RemoveDataRow(DataRow dataRow)
		{
			long value = this.rowToValue[dataRow];
			List<DataRow> dataRows = this.valueToRows[value];

			dataRows.Remove (dataRow);

			if (dataRows.Count == 0)
			{
				this.valueToRows.Remove (value);
			}

			this.rowToValue.Remove (dataRow);
		}


		private void SetupEventHandlers()
		{
			this.DataTable.RowDeleted += (sender, e) =>
			{
				this.RemoveDataRow (e.Row);
			};

			this.DataTable.RowChanged += (sender, e) =>
			{
				if (e.Action == DataRowAction.Change && this.IsValid(e.Row))
				{
					if (this.rowToValue.ContainsKey (e.Row))
					{
						this.RemoveDataRow (e.Row);
					}

					this.AddDataRow (this.GetValue (e.Row), e.Row);
				}
			};
		}


		/// <summary>
		/// Determines whether this <see cref="DataTable"/> mapped by this instance contains a
		/// <see cref="DataRow"/> whose column value is equal to <paramref name="value"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// <c>true</c> if there is such a row and <c>false</c> otherwise.
		/// </returns>
		public bool Contains(long value)
		{
			return this.valueToRows.ContainsKey (value);
		}


		/// <summary>
		/// Gets the first <see cref="DataRow"/> whose column value is equal to
		/// <paramref name="value"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The <see cref="DataRow"/></returns>
		public DataRow GetRow(long value)
		{
			if (!this.Contains (value))
			{
				throw new System.InvalidOperationException ();
			}

			return this.valueToRows[value].First();
		}


		/// <summary>
		/// Gets the <see cref="List"/> of <see cref="DataRow"/> whose column value are equal to
		/// <paramref name="value"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The <see cref="List"/> of <see cref="DataRow"/>.</returns>
		public List<DataRow> GetRows(long value)
		{
			if (!this.Contains (value))
			{
				throw new System.InvalidOperationException ();
			}

			return this.valueToRows[value];
		}


		private int ColumnIndex;


		private Dictionary<DataRow, long> rowToValue;


		private Dictionary<long, List<DataRow>> valueToRows;

	}


}