//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;
using System.Collections.Generic;

namespace Epsitec.Cresus.Replication
{
	/// <summary>
	/// La classe PackedTableData représente les données d'une DataTable sous une forme
	/// facilement comprimable et sérialisable.
	/// </summary>
	
	[System.Serializable]
	public sealed class PackedTableData : System.Runtime.Serialization.ISerializable
	{
		public PackedTableData()
		{
			this.columnDataRows = new List<System.Array> ();
			this.columnNullFlags = new List<System.Array> ();
		}
		
		
		public string							Name
		{
			get
			{
				return this.tableName;
			}
		}
		
		public DbId								Key
		{
			get
			{
				return this.tableKey;
			}
		}
		
		public int								RowCount
		{
			get
			{
				return this.rowCount;
			}
		}
		
		public int								ColumnCount
		{
			get
			{
				return this.columnDataRows.Count;
			}
		}
		
		
		public bool HasNullValues(int column)
		{
			//	Return 'true' si la colonne contient au moins 1 valeur DBNull.
			
			this.UnpackNullFlags ();
			
			if (this.columnDataRows[column] == null)	//	pas de données => colonne DBNull
			{
				return true;
			}
			if (this.columnNullFlags[column] != null)	//	flags => colonne contient au moins un DBNull
			{
				return true;
			}
			
			return false;
		}
		
		public bool HasNonNullValues(int column)
		{
			//	Return 'true' si la colonne contient au moins 1 valeur qui ne soit
			//	pas DBNull.
			
			this.UnpackNullFlags ();
			
			if (this.columnDataRows[column] == null)	//	pas de données => colonne DBNull
			{
				return false;
			}
			
			return true;
		}
		
		
		public void FillTable(System.Data.DataTable table)
		{
			//	Ajoute les lignes stockées dans notre objet à la table passée en entrée.
			//	Le schéma de la table doit être correct, mais aucune vérification n'est
			//	faite ici.
			
			object[][] values = this.GetAllValues ();
			
			for (int i = 0; i < this.RowCount; i++)
			{
				table.Rows.Add (values[i]);
			}
		}
		
		
		public object[][] GetAllValues()
		{
			//	Retourne un tableau de valeurs correspondant à la table stockée dans
			//	notre objet; le tableau a n lignes de m colonnes.
			
			this.UnpackNullFlags ();
			
			object[][] values = new object[this.RowCount][];
			
			int n = this.ColumnCount;
			
			for (int i = 0; i < n; i++)
			{
				System.Array data = this.columnDataRows[i];
				bool[] null_flags = this.columnNullFlags[i] as bool[];

				PackedTableData.UnpackColumnFromNativeArray (values, i, n, data, null_flags);
			}
			
			return values;
		}
		
		public object[]   GetRowValues(int row)
		{
			//	Retourne un tableau de valeurs pour la ligne spécifiée : 1 ligne de m colonnes.
			
			this.UnpackNullFlags ();
			
			int n = this.ColumnCount;
			
			object[] values = new object[n];
			
			for (int i = 0; i < n; i++)
			{
				System.Array data = this.columnDataRows[i];
				bool[] null_flags = this.columnNullFlags[i] as bool[];

				PackedTableData.UnpackValueFromNativeArray (row, data, null_flags, out values[i]);
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
			PackedTableData data = new PackedTableData ();
			
			int rowCount = dataTable.Rows.Count;
			int colCount = dataTable.Columns.Count;
			
			data.rowCount  = rowCount;
			data.tableName = table.Name;
			data.tableKey  = table.Key.Id;
			
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
		/// Packs the column data into a native array.
		/// </summary>
		/// <param name="table">The table data.</param>
		/// <param name="column">The column to pack.</param>
		/// <param name="nullValues">An array of flags; one flag for every null values.</param>
		/// <param name="nullCount">The number of null values.</param>
		/// <returns>A native array containing the column data.</returns>
		public static System.Array PackColumnToNativeArray(System.Data.DataTable table, int column, out bool[] nullValues, out int nullCount)
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

		public static void UnpackColumnFromNativeArray(object[][] values, int column, int columnCount, System.Array array, bool[] nullValues)
		{
			//	A partir de la représentation sous forme de tableau d'une colonne, produit
			//	les valeurs correspondantes (ou DBNull) dans le tableau de valeurs.
			//
			//	Le tableau de valeurs (object[][] values) contient n lignes de m colonnes.
			//	Ainsi, pour accéder à une cellule, il faut utiliser values[row][column].
			//
			//	Si une ligne n'existe pas encore dans le tableau values, cette méthode
			//	allouera une ligne vide (remplie de "null" -- ainsi "null" signifie que
			//	la cellule n'a pas encore été remplie et "DBNull" que la cellule a été
			//	remplie avec une source nulle).

			int n = values.Length;

			if ((nullValues != null) &&
				(nullValues.Length > 0))
			{
				//	Il existe des indications pour savoir si une cellule contient un DBNull
				//	ou non; utilise les informations disponibles :

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
			else
			{
				if ((array == null) ||
					(array.Length == 0))
				{
					//	Il n'y a ni indications au sujet des valeurs, ni au sujet de leur null-ité.
					//	Cela signifie que toutes les lignes sont à initialiser à DBNull !

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
		}

		public static void UnpackValueFromNativeArray(int row, System.Array array, bool[] nullValues, out object value)
		{
			//	Extrait la valeur d'une cellule précise du tableau (l'appelant sélectionne
			//	la colonne adéquate en passant 'array' et 'null_values' correspondants; la
			//	ligne est passée explicitement).

			if ((nullValues != null) &&
				(nullValues.Length > 0))
			{
				//	Il existe des indications pour savoir si une cellule contient un DBNull
				//	ou non; utilise les informations disponibles :

				if (nullValues[row])
				{
					value = System.DBNull.Value;
				}
				else
				{
					value = array.GetValue (row);
				}
			}
			else
			{
				if ((array == null) ||
					(array.Length == 0))
				{
					//	Il n'y a ni indications au sujet des valeurs, ni au sujet de leur null-ité.
					//	Cela signifie que toutes les lignes contiennent DBNull !

					value = System.DBNull.Value;
				}
				else
				{
					value = array.GetValue (row);
				}
			}
		}


		public static void PackBooleanArray(bool[] values, out byte[] packed)
		{
			//	Stocke un tableau de bool sous la forme d'un bitmap compact. La taille exacte
			//	du tableau de bool n'est pas conservée (l'appelant devra s'en souvenir lui-
			//	même pour pouvoir appeler UnpackBooleanArray).

			int n = values.Length;

			int n_full_bytes = n / 8;
			int n_part_bits  = n - 8 * n_full_bytes;
			int n_bytes      = (n_part_bits == 0) ? (n_full_bytes) : (n_full_bytes + 1);

			packed = new byte[n_bytes];

			for (int i = 0; i < n_full_bytes; i++)
			{
				byte value = 0;

				for (int bit = 0; bit < 8; bit++)
				{
					if (values[i*8+bit])
					{
						value |= (byte) (1 << bit);
					}
				}

				packed[i] = value;
			}

			if (n_part_bits > 0)
			{
				byte value = 0;

				for (int bit = 0; bit < n_part_bits; bit++)
				{
					if (values[n_full_bytes*8 + bit])
					{
						value |= (byte) (1 << bit);
					}
				}

				packed[n_full_bytes] = value;
			}
		}

		public static void UnpackBooleanArray(byte[] packed, bool[] values)
		{
			//	Initialise un tableau de bool (déjà alloué) à partir de le représentation
			//	compacte sous forme de bitmap :

			int n = values.Length;

			int n_full_bytes = n / 8;
			int n_part_bits  = n - 8 * n_full_bytes;
			int n_bytes      = (n_part_bits == 0) ? (n_full_bytes) : (n_full_bytes + 1);

			System.Diagnostics.Debug.Assert (n_bytes == packed.Length);

			for (int i = 0; i < n_full_bytes; i++)
			{
				byte value = packed[i];

				for (int bit = 0; bit < 8; bit++)
				{
					values[i*8 + bit] = ((value & (1 << bit)) == 0) ? false : true;
				}
			}

			if (n_part_bits > 0)
			{
				byte value = packed[n_full_bytes];

				for (int bit = 0; bit < n_part_bits; bit++)
				{
					values[n_full_bytes*8 + bit] = ((value & (1 << bit)) == 0) ? false : true;
				}
			}
		}


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

				if ((value == System.DBNull.Value) ||
					(value == null))
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
		
		public PackedTableData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
			
			int col_count = this.ColumnCount;
			int row_count = this.RowCount;
			
			info.AddValue (Strings.ColumnCount, col_count);
			info.AddValue (Strings.RowCount, row_count);
			
			for (int i = 0; i < col_count; i++)
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
						PackedTableData.PackBooleanArray (values, out nullPacked);
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
		
		private void UnpackNullFlags()
		{
			//	A partir des bitmaps, génère les bool[] correspondant aux informations
			//	de null-ité des diverses lignes dans les colonnes de la table.
			
			if (this.isNullFlagsArrayPacked)
			{
				int n = this.columnNullFlags.Count;
				
				for (int i = 0; i < n; i++)
				{
					System.Array data = this.columnDataRows[i];
					
					byte[] packed_column = this.columnNullFlags[i] as byte[];
					bool[] values;
					
					if (data == null)
					{
						//	Cas particulier: il n'y a aucune donnée dans cette colonne, donc on part du principe
						//	que ça revient à dire que toute la colonne est remplie de DBNull. Donc il n'y a pas
						//	besoin d'allouer de tableau d'infos sur la nullabilité non plus...
						
						values = null;
					}
					else if (packed_column == null)
					{
						//	Autre cas particulier: il n'y a aucune information sur la nullabilité, mais il y a
						//	des données; ça veut dire qu'aucune ligne n'est nulle; on peut donc aussi se passer
						//	d'allouer le tableau d'infos sur la nullabilité...
						
						values = null;
					}
					else
					{
						//	Il y a des lignes et nous avons une représentation sous forme de bitset des divers
						//	fanions de nullabilité. On va décomprimer le bitset en un tableau de booléens :
						
						int data_row_count = data.GetLength (0);
						
						values = new bool[data_row_count];

						PackedTableData.UnpackBooleanArray (packed_column, values);
					}
					
					this.columnNullFlags[i] = values;
				}
				
				this.isNullFlagsArrayPacked = false;
			}
		}


		readonly List<System.Array>				columnDataRows;
		readonly List<System.Array>				columnNullFlags;
		private string							tableName;
		private DbId							tableKey;
		private int								rowCount;
		private bool							isNullFlagsArrayPacked;
	}
}
