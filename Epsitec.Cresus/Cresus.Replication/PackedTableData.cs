//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;

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
		}
		
		
		public string							Name
		{
			get
			{
				return this.table_name;
			}
		}
		
		public DbId								Key
		{
			get
			{
				return this.table_key;
			}
		}
		
		public int								RowCount
		{
			get
			{
				return this.row_count;
			}
		}
		
		public int								ColumnCount
		{
			get
			{
				return this.column_data_rows.Count;
			}
		}
		
		
		public bool HasNullValues(int column)
		{
			//	Return 'true' si la colonne contient au moins 1 valeur DBNull.
			
			this.UnpackNullFlags ();
			
			if (this.column_data_rows[column] == null)	//	pas de données => colonne DBNull
			{
				return true;
			}
			if (this.column_null_flags[column] != null)	//	flags => colonne contient au moins un DBNull
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
			
			if (this.column_data_rows[column] == null)	//	pas de données => colonne DBNull
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
				System.Array data = this.column_data_rows[i] as System.Array;
				bool[] null_flags = this.column_null_flags[i] as bool[];

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
				System.Array data = this.column_data_rows[i] as System.Array;
				bool[] null_flags = this.column_null_flags[i] as bool[];

				PackedTableData.UnpackValueFromNativeArray (row, data, null_flags, out values[i]);
			}
			
			return values;
		}
		
		
		public static PackedTableData CreateFromTable(DbTable table, System.Data.DataTable data_table)
		{
			//	Crée une représentation compacte de la table passée en entrée. Les
			//	données sont stockées colonne par colonne, ce qui simplifie la
			//	compression. Les valeurs DBNull sont stockées sous la forme d'un
			//	tableau de bool (ou bitmap).
			
			PackedTableData data = new PackedTableData ();
			
			int row_count = data_table.Rows.Count;
			int col_count = data_table.Columns.Count;
			
			data.row_count  = row_count;
			data.table_name = table.Name;
			data.table_key  = table.Key.Id;
			
			for (int i = 0; i < col_count; i++)
			{
				System.Array array;
				
				bool[] null_flags;
				int    null_count;
				
				//	Chaque colonne est stockée dans un tableau de son type respectif.
				//	Ainsi, si la colonne contient des 'int', les données seront stockées
				//	dans un 'int[]', ce qui est nettement plus compact qu'un tableau
				//	générique 'object[]' (pas de boxing) :

				PackedTableData.PackColumnToNativeArray (data_table, i, out array, out null_flags, out null_count);
				
				if (null_count == 0)
				{
					null_flags = null;
				}
				else if (null_count == row_count)
				{
					array      = null;
					null_flags = null;
				}
				else
				{
					System.Diagnostics.Debug.Assert (null_count > 0);
					System.Diagnostics.Debug.Assert (null_count < row_count);
				}
				
				data.column_data_rows.Add (array);
				data.column_null_flags.Add (null_flags);
			}
			
			return data;
		}

		public static void PackColumnToNativeArray(System.Data.DataTable table, int column, out System.Array array, out bool[] null_values, out int null_count)
		{
			//	Passe en revue une colonne de la table et remplit un tableau avec les
			//	valeurs trouvées; le tableau utilise le type natif sous-jacent s'il fait
			//	partie de la liste des types reconnus :
			//	
			//	- bool, short, int, long, decimal
			//	- string, DateTime, byte[]
			//
			//	En plus, les lignes contenant DBNull sont marquées comme telles dans le
			//	tableau null_values et comptées avec null_count.

			System.Type type = table.Columns[column].DataType;

			int n = table.Rows.Count;

			null_values = new bool[n];
			null_count  = 0;

			if (type == typeof (bool))
			{
				bool[] data = new bool[n];

				for (int i = 0; i < n; i++)
				{
					if (table.Rows[i][column] == System.DBNull.Value)
					{
						null_values[i] = true;
						data[i]        = false;
						null_count++;
					}
					else
					{
						null_values[i] = false;
						data[i]        = (bool) table.Rows[i][column];
					}
				}

				array = data;
			}
			else if (type == typeof (short))
			{
				short[] data = new short[n];

				for (int i = 0; i < n; i++)
				{
					if (table.Rows[i][column] == System.DBNull.Value)
					{
						null_values[i] = true;
						data[i]        = 0;
						null_count++;
					}
					else
					{
						null_values[i] = false;
						data[i]        = (short) table.Rows[i][column];
					}
				}

				array = data;
			}
			else if (type == typeof (int))
			{
				int[] data = new int[n];

				for (int i = 0; i < n; i++)
				{
					if (table.Rows[i][column] == System.DBNull.Value)
					{
						null_values[i] = true;
						data[i]        = 0;
						null_count++;
					}
					else
					{
						null_values[i] = false;
						data[i]        = (int) table.Rows[i][column];
					}
				}

				array = data;
			}
			else if (type == typeof (long))
			{
				long[] data = new long[n];

				for (int i = 0; i < n; i++)
				{
					if (table.Rows[i][column] == System.DBNull.Value)
					{
						null_values[i] = true;
						data[i]        = 0;
						null_count++;
					}
					else
					{
						null_values[i] = false;
						data[i]        = (long) table.Rows[i][column];
					}
				}

				array = data;
			}
			else if (type == typeof (decimal))
			{
				decimal[] data = new decimal[n];

				for (int i = 0; i < n; i++)
				{
					if (table.Rows[i][column] == System.DBNull.Value)
					{
						null_values[i] = true;
						data[i]        = 0;
						null_count++;
					}
					else
					{
						null_values[i] = false;
						data[i]        = (decimal) table.Rows[i][column];
					}
				}

				array = data;
			}
			else if (type == typeof (string))
			{
				string[] data = new string[n];

				for (int i = 0; i < n; i++)
				{
					if (table.Rows[i][column] == System.DBNull.Value)
					{
						null_values[i] = true;
						data[i]        = null;
						null_count++;
					}
					else
					{
						null_values[i] = false;
						data[i]        = (string) table.Rows[i][column];
					}
				}

				array = data;
			}
			else if (type == typeof (System.DateTime))
			{
				System.DateTime[] data = new System.DateTime[n];
				System.DateTime   zero = new System.DateTime (0);

				for (int i = 0; i < n; i++)
				{
					if (table.Rows[i][column] == System.DBNull.Value)
					{
						null_values[i] = true;
						data[i]        = zero;
						null_count++;
					}
					else
					{
						null_values[i] = false;
						data[i]        = (System.DateTime) table.Rows[i][column];
					}
				}

				array = data;
			}
			else if (type == typeof (byte[]))
			{
				byte[][] data = new byte[n][];

				for (int i = 0; i < n; i++)
				{
					if (table.Rows[i][column] == System.DBNull.Value)
					{
						null_values[i] = true;
						data[i]        = null;
						null_count++;
					}
					else
					{
						null_values[i] = false;
						data[i]        = (byte[]) table.Rows[i][column];
					}
				}

				array = data;
			}
			else
			{
				object[] data = new object[n];

				for (int i = 0; i < n; i++)
				{
					if (table.Rows[i][column] == System.DBNull.Value)
					{
						null_values[i] = true;
						data[i]        = null;
						null_count++;
					}
					else
					{
						null_values[i] = false;
						data[i]        = table.Rows[i][column];
					}
				}

				array = data;
			}
		}

		public static void UnpackColumnFromNativeArray(object[][] values, int column, int column_count, System.Array array, bool[] null_values)
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

			if ((null_values != null) &&
				(null_values.Length > 0))
			{
				//	Il existe des indications pour savoir si une cellule contient un DBNull
				//	ou non; utilise les informations disponibles :

				System.Diagnostics.Debug.Assert (array.Length == n);

				for (int i = 0; i < n; i++)
				{
					if (values[i] == null)
					{
						values[i] = new object[column_count];
					}

					if (null_values[i])
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
							values[i] = new object[column_count];
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
							values[i] = new object[column_count];
						}

						values[i][column] = array.GetValue (i);
					}
				}
			}
		}

		public static void UnpackValueFromNativeArray(int row, System.Array array, bool[] null_values, out object value)
		{
			//	Extrait la valeur d'une cellule précise du tableau (l'appelant sélectionne
			//	la colonne adéquate en passant 'array' et 'null_values' correspondants; la
			//	ligne est passée explicitement).

			if ((null_values != null) &&
				(null_values.Length > 0))
			{
				//	Il existe des indications pour savoir si une cellule contient un DBNull
				//	ou non; utilise les informations disponibles :

				if (null_values[row])
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
		
		
		
		#region ISerializable Members
		public PackedTableData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			int col_count = info.GetInt32 ("Col#");
			int row_count = info.GetInt32 ("Row#");
			
			for (int i = 0; i < col_count; i++)
			{
				this.column_data_rows.Add (info.GetValue (string.Format (System.Globalization.CultureInfo.InvariantCulture, "D.{0}", i), typeof (System.Array)));
				this.column_null_flags.Add (info.GetValue (string.Format (System.Globalization.CultureInfo.InvariantCulture, "N.{0}", i), typeof (byte[])));
			}
			
			this.table_name = info.GetString ("Name");
			this.table_key  = info.GetInt64 ("Id");
			this.row_count  = row_count;
			this.is_null_flags_array_packed = true;
		}
		
		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			System.Diagnostics.Debug.Assert (this.column_data_rows.Count == this.column_null_flags.Count);
			
			int col_count = this.ColumnCount;
			int row_count = this.RowCount;
			
			info.AddValue ("Col#", col_count);
			info.AddValue ("Row#", row_count);
			
			for (int i = 0; i < col_count; i++)
			{
				System.Array data_rows   = this.column_data_rows[i] as System.Array;
				System.Array null_flags  = this.column_null_flags[i] as System.Array;
				byte[]       null_packed = null;
				
				//	S'il y a un tableau de bool pour représenter les valeurs DBNull dans la
				//	colonne, on le comprime au préalable (si ça n'a pas encore été fait) :
				
				if (null_flags != null)
				{
					if (this.is_null_flags_array_packed)
					{
						null_packed = (byte[]) null_flags;
					}
					else
					{
						bool[] values = (bool[]) null_flags;
						PackedTableData.PackBooleanArray (values, out null_packed);
					}
				}
				
				info.AddValue (string.Format (System.Globalization.CultureInfo.InvariantCulture, "D.{0}", i), data_rows);
				info.AddValue (string.Format (System.Globalization.CultureInfo.InvariantCulture, "N.{0}", i), null_packed);
			}
			
			info.AddValue ("Name", this.table_name);
			info.AddValue ("Id", this.table_key.Value);
		}
		#endregion
		
		private void UnpackNullFlags()
		{
			//	A partir des bitmaps, génère les bool[] correspondant aux informations
			//	de null-ité des diverses lignes dans les colonnes de la table.
			
			if (this.is_null_flags_array_packed)
			{
				int n = this.column_null_flags.Count;
				
				for (int i = 0; i < n; i++)
				{
					System.Array data = this.column_data_rows[i] as System.Array;
					
					byte[] packed_column = this.column_null_flags[i] as byte[];
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
					
					this.column_null_flags[i] = values;
				}
			}
			
			this.is_null_flags_array_packed = false;
		}
		
		
		private string							table_name;
		private DbId							table_key;
		private int								row_count;
		private System.Collections.ArrayList	column_data_rows  = new System.Collections.ArrayList ();
		private System.Collections.ArrayList	column_null_flags = new System.Collections.ArrayList ();
		private bool							is_null_flags_array_packed = false;
	}
}
