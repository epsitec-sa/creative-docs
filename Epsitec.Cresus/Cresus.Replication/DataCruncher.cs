//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;
using System.Runtime.Serialization.Formatters.Binary;

namespace Epsitec.Cresus.Replication
{
	/// <summary>
	/// Summary description for DataCruncher.
	/// </summary>
	public sealed class DataCruncher
	{
		public DataCruncher(DbInfrastructure infrastructure, DbTransaction transaction)
		{
			this.infrastructure = infrastructure;
			this.transaction    = transaction;
		}
		
		
		public System.Data.DataTable ExtractData(DbTable table, DbId sync_id)
		{
			//	Extrait les données de la table spécifiée en ne considérant que ce qui a changé
			//	depuis la synchronisation définie par 'sync_id'.
			
			long sync_id_min = sync_id.Value;
			long sync_id_max = DbId.LocalRange * (sync_id.ClientId + 1) - 1;
			
			System.Diagnostics.Debug.Assert (DbId.AnalyzeClass (sync_id_min) == DbIdClass.Standard);
			System.Diagnostics.Debug.Assert (DbId.AnalyzeClass (sync_id_max) == DbIdClass.Standard);
			
			DbSelectCondition condition = new DbSelectCondition (this.infrastructure.TypeConverter);
			
			condition.AddCondition (table.Columns[Tags.ColumnRefLog], DbCompare.GreaterThan, sync_id_min);
			condition.AddCondition (table.Columns[Tags.ColumnRefLog], DbCompare.LessThan, sync_id_max);
			
			using (DbRichCommand command = DbRichCommand.CreateFromTable (this.infrastructure, this.transaction, table, condition))
			{
				return command.DataSet.Tables[0];
			}
		}
		
		
		public static void PackColumnToArray(System.Data.DataTable table, int column, out System.Array array, out bool[] null_values, out int null_count)
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
		
		public static void UnpackColumnFromArray(object[][] values, int column, int column_count, System.Array array, bool[] null_values)
		{
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
		
		
		public static void PackBooleanArray(bool[] values, out byte[] packed)
		{
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
						value |= (byte)(1 << bit);
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
						value |= (byte)(1 << bit);
					}
				}
				
				packed[n_full_bytes] = value;
			}
		}
		
		public static void UnpackBooleanArray(byte[] packed, bool[] values)
		{
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
		
		
		public static byte[] SerializeAndCompressToMemory(object o)
		{
			BinaryFormatter        formatter  = new BinaryFormatter ();
			System.IO.MemoryStream memory     = new System.IO.MemoryStream ();
			System.IO.Stream       compressed = Common.IO.Compression.CreateDeflateStream (memory, 9);
			
			formatter.Serialize (compressed, o);
			
			compressed.Close ();
			memory.Close ();
			
			return memory.ToArray ();
		}
		
		public static object DeserializeAndDecompressFromMemory(byte[] buffer)
		{
			BinaryFormatter        formatter    = new BinaryFormatter ();
			System.IO.MemoryStream memory       = new System.IO.MemoryStream(buffer, 0, buffer.Length, false, false);
			System.IO.Stream       decompressed = Common.IO.Decompression.CreateStream (memory);
			
			object o = formatter.Deserialize (decompressed);
			
			decompressed.Close ();
			memory.Close ();
			
			return o;
		}
		
		
		private DbInfrastructure				infrastructure;
		private DbTransaction					transaction;
	}
}
