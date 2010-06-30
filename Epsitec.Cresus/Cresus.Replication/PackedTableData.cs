//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;
using System.Collections.Generic;

namespace Epsitec.Cresus.Replication
{
	/// <summary>
	/// The <c>PackedTableData</c> class stores a data table as a series of compact
	/// arrays, which is easily and efficiently serializable.
	/// </summary>
	
	[System.Serializable]
	public sealed class PackedTableData : System.Runtime.Serialization.ISerializable
	{
		private PackedTableData()
		{
			this.columnDataRows = new List<System.Array> ();
			this.columnNullFlags = new List<System.Array> ();
		}

		private PackedTableData(string tableName, DbId tableKey, int rowCount)
			: this ()
		{
			this.tableName = tableName;
			this.tableKey  = tableKey;
			this.rowCount  = rowCount;
		}


		/// <summary>
		/// Gets the table name.
		/// </summary>
		/// <value>The table name.</value>
		public string							Name
		{
			get
			{
				return this.tableName;
			}
		}

		/// <summary>
		/// Gets the table key.
		/// </summary>
		/// <value>The table key.</value>
		public DbId								Key
		{
			get
			{
				return this.tableKey;
			}
		}

		/// <summary>
		/// Gets the row count.
		/// </summary>
		/// <value>The row count.</value>
		public int								RowCount
		{
			get
			{
				return this.rowCount;
			}
		}

		/// <summary>
		/// Gets the column count.
		/// </summary>
		/// <value>The column count.</value>
		public int								ColumnCount
		{
			get
			{
				return this.columnDataRows.Count;
			}
		}


		/// <summary>
		/// Determines whether the specified column has at least one null value.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <returns>
		/// 	<c>true</c> if the specified column has null values; otherwise, <c>false</c>.
		/// </returns>
		public bool HasNullValues(int column)
		{
			this.UnpackNullFlags ();

			//	No data => everything is null
			//	There are null flags => there is at least one null value
			return (this.columnDataRows[column] == null) || (this.columnNullFlags[column] != null);
		}

		/// <summary>
		/// Determines whether the specified column has at least one non null value.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <returns>
		/// 	<c>true</c> if the specified column has non null values; otherwise, <c>false</c>.
		/// </returns>
		public bool HasNonNullValues(int column)
		{
			return (this.columnDataRows[column] != null);
		}


		/// <summary>
		/// Fills the table with the data stored in this instance. The schema of the
		/// table must match, but no explicit check is done here.
		/// </summary>
		/// <param name="table">The data table which will be filled.</param>
		public void FillTable(System.Data.DataTable table)
		{
			object[][] values = this.GetAllValues ();
			
			foreach (object[] row in values)
			{
				table.Rows.Add (row);
			}
		}


		/// <summary>
		/// Gets all values stored in this instance.
		/// </summary>
		/// <returns>An array of rows, where each row is an array of values.</returns>
		public object[][] GetAllValues()
		{
			this.UnpackNullFlags ();
			
			object[][] values = new object[this.RowCount][];
			
			int n = this.ColumnCount;
			
			for (int i = 0; i < n; i++)
			{
				System.Array data = this.columnDataRows[i];
				bool[]  nullFlags = this.columnNullFlags[i] as bool[];

				PackedTableData.UnpackColumnFromNativeArray (values, i, n, data, nullFlags);
			}
			
			return values;
		}

		/// <summary>
		/// Gets the values for the specified row.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <returns>The values.</returns>
		public object[]   GetRowValues(int row)
		{
			this.UnpackNullFlags ();
			
			int n = this.ColumnCount;
			
			object[] values = new object[n];
			
			for (int i = 0; i < n; i++)
			{
				System.Array data = this.columnDataRows[i];
				bool[]  nullFlags = this.columnNullFlags[i] as bool[];

				values[i] = PackedTableData.UnpackValueFromNativeArray (row, data, nullFlags);
			}
			
			return values;
		}


		/// <summary>
		/// Creates a <c>PackedTableData</c> from table the specified data table. The
		/// data will be stored column by column and <c>DBNull</c> values are optimized
		/// using a bitmap.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="dataTable">The data table.</param>
		/// <returns>The <c>PackedTableData</c>.</returns>
		public static PackedTableData CreateFromTable(DbTable table, System.Data.DataTable dataTable)
		{
			int rowCount = dataTable.Rows.Count;
			int colCount = dataTable.Columns.Count;

			PackedTableData data = new PackedTableData (table.Name, table.Key.Id, rowCount);
			
			for (int i = 0; i < colCount; i++)
			{
				bool[] nullFlags;
				int    nullCount;
				
				System.Array array = PackedTableData.PackColumnToNativeArray (dataTable, i, out nullFlags, out nullCount);
				
				if (nullCount == 0)
				{
					nullFlags = null;
				}
				else if (nullCount == rowCount)
				{
					array     = null;
					nullFlags = null;
				}
				else
				{
					System.Diagnostics.Debug.Assert (nullCount > 0);
					System.Diagnostics.Debug.Assert (nullCount < rowCount);
				}
				
				data.columnDataRows.Add (array);
				data.columnNullFlags.Add (nullFlags);
			}
			
			return data;
		}


		/// <summary>
		/// Packs the specified column data into a native array.
		/// </summary>
		/// <param name="table">The table data.</param>
		/// <param name="column">The column to pack.</param>
		/// <param name="nullValues">An array of flags; one flag for every null values.</param>
		/// <param name="nullCount">The number of null values.</param>
		/// <returns>A native array containing the column data.</returns>
		private static System.Array PackColumnToNativeArray(System.Data.DataTable table, int column, out bool[] nullValues, out int nullCount)
		{
			//	Handles following native types :
			//
			//	- bool, short, int, long, decimal
			//	- string, DateTime, byte[]

			System.Type type = table.Columns[column].DataType;

			if (type == typeof (bool))
			{
				return PackColumnToArray<bool> (table, column, out nullValues, out nullCount);
			}
			else if (type == typeof (short))
			{
				return PackColumnToArray<short> (table, column, out nullValues, out nullCount);
			}
			else if (type == typeof (int))
			{
				return PackColumnToArray<int> (table, column, out nullValues, out nullCount);
			}
			else if (type == typeof (long))
			{
				return PackColumnToArray<long> (table, column, out nullValues, out nullCount);
			}
			else if (type == typeof (decimal))
			{
				return PackColumnToArray<decimal> (table, column, out nullValues, out nullCount);
			}
			else if (type == typeof (string))
			{
				return PackColumnToArray<string> (table, column, out nullValues, out nullCount);
			}
			else if (type == typeof (System.DateTime))
			{
				return PackColumnToArray<System.DateTime> (table, column, out nullValues, out nullCount);
			}
			else if (type == typeof (byte[]))
			{
				return PackColumnToArray<byte[]> (table, column, out nullValues, out nullCount);
			}
			else
			{
				return PackColumnToArray<object> (table, column, out nullValues, out nullCount);
			}
		}

		/// <summary>
		/// Unpacks the specified column from the native array into an array of rows.
		/// </summary>
		/// <param name="values">The resulting values (array of rows; each row is an array of values or <c>null</c>).</param>
		/// <param name="column">The column to unpack.</param>
		/// <param name="columnCount">The total number of columns.</param>
		/// <param name="array">The native array.</param>
		/// <param name="nullValues">The null values.</param>
		private static void UnpackColumnFromNativeArray(object[][] values, int column, int columnCount, System.Array array, bool[] nullValues)
		{
			//	Access the values through values[row][column]

			//	A row might not yet exist in the values array; this is identified by the
			//	fact that values[row] == null.

			int n = values.Length;

			if (nullValues != null && nullValues.Length > 0)
			{
				//	Some values are null, but not all.

				System.Diagnostics.Debug.Assert (array.Length == n);

				for (int i = 0; i < n; i++)
				{
					if (values[i] == null)
					{
						values[i] = new object[columnCount];
					}

					if (nullValues[i])
					{
						values[i][column] = System.DBNull.Value;
					}
					else
					{
						values[i][column] = array.GetValue (i);
					}
				}
			}
			else if (array == null || array.Length == 0)
			{
				//	All values are null.

				for (int i = 0; i < n; i++)
				{
					if (values[i] == null)
					{
						values[i] = new object[columnCount];
					}

					values[i][column] = System.DBNull.Value;
				}
			}
			else
			{
				//	No values are null.

				System.Diagnostics.Debug.Assert (array.Length == n);

				for (int i = 0; i < n; i++)
				{
					if (values[i] == null)
					{
						values[i] = new object[columnCount];
					}

					values[i][column] = array.GetValue (i);
				}
			}
		}

		/// <summary>
		/// Unpacks a value from the native array of a given column.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <param name="array">The column array.</param>
		/// <param name="nullValues">The null values for the column.</param>
		/// <returns>The value.</returns>
		static object UnpackValueFromNativeArray(int row, System.Array array, bool[] nullValues)
		{
			if (nullValues != null && nullValues.Length > 0)
			{
				//	Some values might be null.

				if (nullValues[row])
				{
					return System.DBNull.Value;
				}
				else
				{
					return array.GetValue (row);
				}
			}
			else if (array == null || array.Length == 0)
			{
				//	All values are null.

				return System.DBNull.Value;
			}
			else
			{
				// No value is null.

				return array.GetValue (row);
			}
		}


		/// <summary>
		/// Packs the boolean array into a bitmap. The exact size of the array cannot be
		/// inferred from the bitmap (the caller will have to remember it).
		/// </summary>
		/// <param name="values">The boolean array.</param>
		static byte[] PackBooleanArray(bool[] values)
		{
			int n = values.Length;

			int nFullBytes = n / 8;
			int nPartBits  = n - (8 * nFullBytes);
			int nBytes     = (nPartBits == 0) ? (nFullBytes) : (nFullBytes + 1);

			byte[] packed = new byte[nBytes];

			for (int i = 0; i < nFullBytes; i++)
			{
				byte value = 0;

				for (int bit = 0; bit < 8; bit++)
				{
					if (values[i * 8 + bit])
					{
						value |= (byte) (1 << bit);
					}
				}

				packed[i] = value;
			}

			if (nPartBits > 0)
			{
				byte value = 0;

				for (int bit = 0; bit < nPartBits; bit++)
				{
					if (values[nFullBytes * 8 + bit])
					{
						value |= (byte) (1 << bit);
					}
				}

				packed[nFullBytes] = value;
			}

			return packed;
		}

		/// <summary>
		/// Unpacks the bitmap into an already allocated boolean array.
		/// </summary>
		/// <param name="packed">The packed bitmap.</param>
		/// <param name="values">The boolean array.</param>
		public static void UnpackBooleanArray(byte[] packed, bool[] values)
		{
			int n = values.Length;

			int nFullBytes = n / 8;
			int nPartBits  = n - (8 * nFullBytes);
			int nBytes     = (nPartBits == 0) ? (nFullBytes) : (nFullBytes + 1);

			System.Diagnostics.Debug.Assert (nBytes == packed.Length);

			for (int i = 0; i < nFullBytes; i++)
			{
				byte value = packed[i];

				for (int bit = 0; bit < 8; bit++)
				{
					values[i * 8 + bit] = ((value & (1 << bit)) != 0);
				}
			}

			if (nPartBits > 0)
			{
				byte value = packed[nFullBytes];

				for (int bit = 0; bit < nPartBits; bit++)
				{
					values[nFullBytes * 8 + bit] = ((value & (1 << bit)) != 0);
				}
			}
		}


		/// <summary>
		/// Packs the specified column into a native array.
		/// </summary>
		/// <typeparam name="T">The native type.</typeparam>
		/// <param name="table">The table containing the data to pack into the native array.</param>
		/// <param name="column">The column.</param>
		/// <param name="nullValues">The null values (or <c>null</c> if all values are non-null).</param>
		/// <param name="nullCount">The number of null values.</param>
		/// <returns>The native array.</returns>
		static System.Array PackColumnToArray<T>(System.Data.DataTable table, int column, out bool[] nullValues, out int nullCount)
		{
			int n = table.Rows.Count;

			nullValues = new bool[n];
			nullCount  = 0;

			T[] data = new T[n];
			var rows = table.Rows;

			for (int i = 0; i < n; i++)
			{
				object value = rows[i][column];

				if (value == System.DBNull.Value || value == null)
				{
					nullValues[i] = true;
					data[i]       = default (T);
					nullCount++;
				}
				else
				{
					nullValues[i] = false;
					data[i]       = (T) value;
				}
			}

			return data;
		}

		
		#region ISerializable Members
		
		private PackedTableData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: this ()
		{
			int colCount = info.GetInt32 (Strings.ColumnCount);
			int rowCount = info.GetInt32 (Strings.RowCount);
			
			for (int i = 0; i < colCount; i++)
			{
				string dataName = string.Format (System.Globalization.CultureInfo.InvariantCulture, Strings.DataName, i);
				string nullName = string.Format (System.Globalization.CultureInfo.InvariantCulture, Strings.NullName, i);
				
				this.columnDataRows.Add ((System.Array) info.GetValue (dataName, typeof (System.Array)));
				this.columnNullFlags.Add ((byte[]) info.GetValue (nullName, typeof (byte[])));
			}
			
			this.tableName = info.GetString (Strings.TableName);
			this.tableKey  = info.GetInt64 (Strings.TableKey);
			this.rowCount  = rowCount;
			
			this.isNullFlagsArrayPacked = true;
		}
		
		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			System.Diagnostics.Debug.Assert (this.columnDataRows.Count == this.columnNullFlags.Count);
			
			int colCount = this.ColumnCount;
			int rowCount = this.RowCount;
			
			info.AddValue (Strings.ColumnCount, colCount);
			info.AddValue (Strings.RowCount, rowCount);
			
			for (int i = 0; i < colCount; i++)
			{
				System.Array dataRows   = this.columnDataRows[i];
				System.Array nullFlags  = this.columnNullFlags[i];
				byte[]       nullPacked = null;
				
				//	S'il y a un tableau de bool pour représenter les valeurs DBNull dans la
				//	colonne, on le comprime au préalable (si ça n'a pas encore été fait) :
				
				if (nullFlags != null)
				{
					if (this.isNullFlagsArrayPacked)
					{
						nullPacked = (byte[]) nullFlags;
					}
					else
					{
						bool[] values = (bool[]) nullFlags;
						nullPacked = PackedTableData.PackBooleanArray (values);
					}
				}
				
				info.AddValue (string.Format (System.Globalization.CultureInfo.InvariantCulture, Strings.DataName, i), dataRows);
				info.AddValue (string.Format (System.Globalization.CultureInfo.InvariantCulture, Strings.NullName, i), nullPacked);
			}
			
			info.AddValue (Strings.TableName, this.tableName);
			info.AddValue (Strings.TableKey, this.tableKey.Value);
		}

		private static class Strings
		{
			public const string ColumnCount = "Col#";
			public const string RowCount = "Row#";
			public const string TableName = "Name";
			public const string TableKey = "Id";
			public const string DataName = "D.{0}";
			public const string NullName = "N.{0}";
		}
		
		#endregion

		/// <summary>
		/// Unpacks the null flags, if they are still stored as a packed bitmap.
		/// </summary>
		private void UnpackNullFlags()
		{
			if (this.isNullFlagsArrayPacked)
			{
				int n = this.columnNullFlags.Count;
				
				for (int i = 0; i < n; i++)
				{
					System.Array data = this.columnDataRows[i];
					
					byte[] packedColumn = this.columnNullFlags[i] as byte[];
					bool[] values;
					
					if (data == null)
					{
						//	No data at all, we don't need to provide null flags (all values are null).

						values = null;
					}
					else if (packedColumn == null)
					{
						//	No null values at all; we need not allocate the null flags.
						
						values = null;
					}
					else
					{
						//	Some values are null; the information is stored in the bits of the
						//	packed bitmap :
						
						int dataRowCount = data.GetLength (0);
						
						values = new bool[dataRowCount];

						PackedTableData.UnpackBooleanArray (packedColumn, values);
					}
					
					this.columnNullFlags[i] = values;
				}
				
				this.isNullFlagsArrayPacked = false;
			}
		}


		private readonly List<System.Array>				columnDataRows;
		private readonly List<System.Array>				columnNullFlags;
		private readonly string							tableName;
		private readonly DbId							tableKey;
		private readonly int							rowCount;
		private bool									isNullFlagsArrayPacked;
	}
}
