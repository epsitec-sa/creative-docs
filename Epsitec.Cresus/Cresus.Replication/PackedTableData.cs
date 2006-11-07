//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
				
				DataCruncher.UnpackColumnFromNativeArray (values, i, n, data, null_flags);
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
				
				DataCruncher.UnpackValueFromNativeArray (row, data, null_flags, out values[i]);
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
				
				DataCruncher.PackColumnToNativeArray (data_table, i, out array, out null_flags, out null_count);
				
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
						DataCruncher.PackBooleanArray (values, out null_packed);
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
						
						DataCruncher.UnpackBooleanArray (packed_column, values);
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
