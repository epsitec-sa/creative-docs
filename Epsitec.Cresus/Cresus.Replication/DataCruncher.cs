//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;

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
		
		
		public void PackColumnToArray(System.Data.DataTable table, int column, out System.Array array, out bool[] null_values, out int null_count)
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
		
		public void UnpackColumnFromArray(object[][] table, int column, int column_count, System.Array array, bool[] null_values)
		{
			System.Diagnostics.Debug.Assert (table.Length == array.Length);
			
			int n = table.Length;
			
			if ((null_values != null) &&
				(null_values.Length > 0))
			{
				for (int i = 0; i < n; i++)
				{
					if (table[i] == null)
					{
						table[i] = new object[column_count];
					}
					
					if (null_values[i])
					{
						table[i][column] = System.DBNull.Value;
					}
					else
					{
						table[i][column] = array.GetValue (i);
					}
				}
			}
			else
			{
				for (int i = 0; i < n; i++)
				{
					if (table[i] == null)
					{
						table[i] = new object[column_count];
					}
					
					table[i][column] = array.GetValue (i);
				}
			}
		}
		
		
		
		private DbInfrastructure				infrastructure;
		private DbTransaction					transaction;
	}
}
