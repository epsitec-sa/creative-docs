using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	/// <summary>
	/// The <c>UidManager</c> class provides the low levels tools used to generate unique ids in
	/// the database. These counters are addressed by a name and a slot number. In addition, each
	/// contains a minimum value, a maximum value and a next value.
	/// </summary>
	internal sealed class UidManager
	{
		
		
		// TODO Comment this class.
		// Marc


		/// <summary>
		/// Builds a new instance of <see cref="UidManager"/>.
		/// </summary>
		public UidManager(DbInfrastructure dbInfrastructure, ServiceSchemaEngine schemaEngine)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			schemaEngine.ThrowIfNull ("schemaEngine");

			var table = schemaEngine.GetServiceTable (UidManager.TableFactory.TableName);
			var tableQueryHelper = new TableQueryHelper (dbInfrastructure, table);

			this.table = table;
			this.dbInfrastructure = dbInfrastructure;
			this.tableQueryHelper = tableQueryHelper;
		}


		public UidGenerator CreateUidGenerator(string name, IList<UidSlot> slots)
		{
			name.ThrowIfNullOrEmpty ("name");
			slots.ThrowIfNull ("slots");
			slots.ThrowIf(s=>s.IsEmpty(), "No slots defined.");

			for (int i = 0; i < slots.Count - 1; i++)
			{
				if (slots[i].MaxValue >= slots[i + 1].MinValue)
				{
					throw new System.ArgumentException ("Slots cannot overlap each others and must be ordered.");
				}
			}

			using (DbTransaction transaction = this.tableQueryHelper.CreateLockTransaction ())
			{
				if (this.DoesRowExist (name))
				{
					throw new System.InvalidOperationException ("UidGenerator " + name + " already exists.");
				}

				for (int i = 0; i < slots.Count; i++)
				{
					long minValue = slots[i].MinValue;
					long maxValue = slots[i].MaxValue;
					
					this.InsertRow (name, minValue, maxValue);
				}

				transaction.Commit ();
			}

			return new UidGenerator (this, name, slots);
		}


		public void DeleteUidGenerator(string name)
		{
			name.ThrowIfNullOrEmpty ("name");

			using (DbTransaction transaction = this.tableQueryHelper.CreateLockTransaction ())
			{
				int nbRowsAffected = this.DeleteRow (name);

				transaction.Commit ();

				if (nbRowsAffected == 0)
				{
					throw new System.InvalidOperationException ("UidGenerator " + name + " does not exists.");
				}
			}
		}


		public bool DoesUidGeneratorExist(string name)
		{
			name.ThrowIfNullOrEmpty ("name");

			return this.DoesRowExist (name);
		}


		public UidGenerator GetUidGenerator(string name)
		{
			name.ThrowIfNullOrEmpty ("name");

			var slots = this.GetRows (name)
					.Select (r => new UidSlot ((long) r[2], (long) r[3]))
					.OrderBy (s => s.MinValue)
					.ToList ();

			if (slots.IsEmpty ())
			{
				throw new System.InvalidOperationException ("UidGenerator " + name + " does not exists.");
			}

			return new UidGenerator (this, name, slots);
		}


		public long? GetNextUid(string name)
		{
			name.ThrowIfNullOrEmpty ("name");

			using (DbTransaction transaction = this.tableQueryHelper.CreateLockTransaction ())
			{
				if (!this.DoesRowExist (name))
				{
					throw new System.InvalidOperationException ("UidGenerator " + name + " does not exists.");
				}

				var slot = this.GetRows (name)
					.Select (r => new { MinValue = (long) r[2], MaxValue = (long) r[3], NextValue = (long) r[4] })
					.Where(s => s.NextValue <= s.MaxValue)
					.OrderBy (s => s.MinValue)
					.FirstOrDefault ();

				long? result = null;

				if (slot != null)
				{
					this.IncrementRow (name, slot.MinValue, slot.NextValue);

					result = slot.NextValue;
				}
				
				transaction.Commit ();
				
				return result;
			}
		}


		private void InsertRow(string name, long minValue, long maxValue)
		{
			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
            {
                {UidManager.TableFactory.ColumnNameName, name},
                {UidManager.TableFactory.ColumnMinName, minValue},
                {UidManager.TableFactory.ColumnMaxName, maxValue},
                {UidManager.TableFactory.ColumnNextName, minValue},
            };

			this.tableQueryHelper.AddRow (columnNamesToValues);
		}


		private bool DoesRowExist(string name)
		{
			SqlFunction[] conditions = new SqlFunction[]
            {
            	this.CreateConditionForName (name),
            };

			return this.tableQueryHelper.DoesRowExist (conditions);
		}
		

		private IList<IList<object>> GetRows(string name)
		{
			SqlFunction condition = this.CreateConditionForName (name);

			return this.tableQueryHelper.GetRows (condition);
		}


		private int DeleteRow(string name)
		{
			SqlFunction[] conditions = new SqlFunction[]
            {
            	this.CreateConditionForName (name),
            };

			return this.tableQueryHelper.RemoveRows (conditions);
		}


		private void IncrementRow(string name, long minValue, long nextValue)
		{
			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
		    {
		        {UidManager.TableFactory.ColumnNextName, nextValue + 1},
		    };

			SqlFunction[] conditions = new SqlFunction[]
		    {
		        this.CreateConditionForName (name),
				this.CreateConditionForMinValue (minValue),
		        this.CreateConditionForNextValue (nextValue),
		    };

			this.tableQueryHelper.SetRow (columnNamesToValues, conditions);
		}


		/// <summary>
		/// Creates the <see cref="SqlFunction"/> to use as a condition for the name of a counter for
		/// uids.
		/// </summary>
		/// <param name="name">The name of the counter to match.</param>
		/// <returns>The <see cref="SqlFunction"/> to use as a condition.</returns>
		private SqlFunction CreateConditionForName(string name)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.table.Columns[UidManager.TableFactory.ColumnNameName].GetSqlName ()),
				SqlField.CreateConstant (name, DbRawType.String)
			);
		}


		private SqlFunction CreateConditionForNextValue(long nextValue)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.table.Columns[UidManager.TableFactory.ColumnNextName].GetSqlName ()),
				SqlField.CreateConstant (nextValue, DbRawType.Int64)
			);
		}


		private SqlFunction CreateConditionForMinValue(long minValue)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.table.Columns[UidManager.TableFactory.ColumnMinName].GetSqlName ()),
				SqlField.CreateConstant (minValue, DbRawType.Int64)
			);
		}


		private readonly DbTable table;


		private readonly DbInfrastructure dbInfrastructure;


		private readonly TableQueryHelper tableQueryHelper;


		public static TableBuilder TableFactory
		{
			get
			{
				return UidManager.tableFactory;
			}
		}


		private static readonly TableBuilder tableFactory = new TableBuilder ();


		public class TableBuilder : ITableFactory
		{


			#region ITableHelper Members


			public string TableName
			{
				get
				{
					return "CR_UID";
				}
			}


			public DbTable BuildTable()
			{
				DbTypeDef typeKeyId = new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Num.KeyId);
				DbTypeDef typeName = new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Str.Name);
				DbTypeDef typeLongInteger = new DbTypeDef (LongIntegerType.Default);

				DbTable table = new DbTable (this.TableName);

				DbColumn columnId =    new DbColumn (this.ColumnIdName, typeKeyId, DbColumnClass.KeyId, DbElementCat.Internal)
				{
					IsAutoIncremented = true
				};

				DbColumn columnName = new DbColumn (this.ColumnNameName, typeName, DbColumnClass.Data, DbElementCat.Internal);
				DbColumn columnUidMin = new DbColumn (this.ColumnMinName, typeLongInteger, DbColumnClass.Data, DbElementCat.Internal);
				DbColumn columnUidMax = new DbColumn (this.ColumnMaxName, typeLongInteger, DbColumnClass.Data, DbElementCat.Internal);
				DbColumn columnUidNext = new DbColumn (this.ColumnNextName, typeLongInteger, DbColumnClass.Data, DbElementCat.Internal);

				table.Columns.Add (columnId);
				table.Columns.Add (columnName);
				table.Columns.Add (columnUidMin);
				table.Columns.Add (columnUidMax);
				table.Columns.Add (columnUidNext);

				table.DefineCategory (DbElementCat.Internal);

				table.DefinePrimaryKey (columnId);
				table.UpdatePrimaryKeyInfo ();

				table.AddIndex ("IDX_UID_NAME", SqlSortOrder.Ascending, columnName);

				return table;
			}


			#endregion


			public string ColumnIdName
			{
				get
				{
					return "CR_ID";
				}
			}


			public string ColumnNameName
			{
				get
				{
					return "CR_NAME";
				}
			}


			public string ColumnMinName
			{
				get
				{
					return "CR_MIN";
				}
			}


			public string ColumnMaxName
			{
				get
				{
					return "CR_MAX";
				}
			}


			public string ColumnNextName
			{
				get
				{
					return "CR_NEXT";
				}
			}

		
		}

	}


}
