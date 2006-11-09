//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbSelectCondition représente une condition pour la clause
	/// WHERE d'un SELECT.
	/// </summary>
	public class DbSelectCondition
	{
		public DbSelectCondition(ITypeConverter type_converter)
			: this (type_converter, DbSelectRevision.All)
		{
		}

		public DbSelectCondition(ITypeConverter type_converter, DbSelectRevision revision)
		{
			this.type_converter = type_converter;
			this.sql_fields     = new Collections.SqlFields ();
			this.revision       = revision;
		}
		
		
		public DbSelectRevision					Revision
		{
			get
			{
				return this.revision;
			}
			set
			{
				this.revision = value;
			}
		}
		
		public DbCompareCombiner				Combiner
		{
			get
			{
				return this.combiner;
			}
			set
			{
				this.combiner = value;
			}
		}
		
		
		public void AddCondition(DbColumn a, DbCompare comparison, short value)
		{
			this.AddConditionWithRawValue (a, comparison, value, DbRawType.Int16);
		}
		
		public void AddCondition(DbColumn a, DbCompare comparison, int value)
		{
			this.AddConditionWithRawValue (a, comparison, value, DbRawType.Int32);
		}
		
		public void AddCondition(DbColumn a, DbCompare comparison, long value)
		{
			this.AddConditionWithRawValue (a, comparison, value, DbRawType.Int64);
		}
		
		public void AddCondition(DbColumn a, DbCompare comparison, decimal value, DbNumDef num_def)
		{
			DbRawType raw_type  = TypeConverter.GetRawType (DbSimpleType.Decimal, num_def);
			object    raw_value = TypeConverter.ConvertFromSimpleType (value, DbSimpleType.Decimal, num_def);
			
			this.AddConditionWithRawValue (a, comparison, raw_value, raw_type);
		}
		
		public void AddCondition(DbColumn a, DbCompare comparison, string value)
		{
			this.AddConditionWithRawValue (a, comparison, value, DbRawType.String);
		}
		
		
		public void AddCondition(DbColumn a, DbCompare comparison, DbColumn b)
		{
			SqlField    field_a  = SqlField.CreateName (a);
			SqlField    field_b  = SqlField.CreateName (b);
			SqlFunction function = new SqlFunction (this.MapDbCompareToSqlFunctionType (comparison), field_a, field_b);
			
			this.sql_fields.Add (SqlField.CreateFunction (function));
		}
		
		public void AddConditionIsNull(DbColumn a)
		{
			SqlField    field_a  = SqlField.CreateName (a);
			SqlFunction function = new SqlFunction (SqlFunctionType.CompareIsNull, field_a);
			
			this.sql_fields.Add (SqlField.CreateFunction (function));
		}
		
		public void AddConditionIsNotNull(DbColumn a)
		{
			SqlField    field_a  = SqlField.CreateName (a);
			SqlFunction function = new SqlFunction (SqlFunctionType.CompareIsNotNull, field_a);
			
			this.sql_fields.Add (SqlField.CreateFunction (function));
		}
		
		
		internal void CreateConditions(DbTable main_table, Collections.SqlFields fields)
		{
			switch (this.revision)
			{
				case DbSelectRevision.LiveAll:
					this.AddCondition (main_table.Columns[Tags.ColumnStatus], DbCompare.LessThan, DbKey.ConvertToIntStatus (DbRowStatus.Deleted));
					break;
				
				case DbSelectRevision.LiveActive:
					this.AddCondition (main_table.Columns[Tags.ColumnStatus], DbCompare.LessThan, DbKey.ConvertToIntStatus (DbRowStatus.ArchiveCopy));
					break;
				
				case DbSelectRevision.All:
					break;
				
				default:
					throw new System.NotSupportedException (string.Format ("DbSelectRevision.{0} not supported", this.revision));
			}
			
			switch (this.combiner)
			{
				case DbCompareCombiner.And:
					fields.AddRange (this.sql_fields);
					break;
				
				case DbCompareCombiner.Or:
					if (this.sql_fields.Count > 0)
					{
						fields.Add (this.sql_fields.Merge (SqlFunctionType.LogicOr));
					}
					break;
				
				default:
					throw new System.InvalidOperationException (string.Format ("Cannot create conditions with {0} combiner.", this.combiner));
			}
		}
		
		
		protected void AddConditionWithRawValue(DbColumn a, DbCompare comparison, object raw_value, DbRawType raw_type)
		{
			SqlField    field_a  = SqlField.CreateName (a);
			SqlField    field_b  = SqlField.CreateConstant (raw_value, raw_type);
			SqlFunction function = new SqlFunction (this.MapDbCompareToSqlFunctionType (comparison), field_a, field_b);
			
			this.sql_fields.Add (SqlField.CreateFunction (function));
		}
		
		
		protected SqlFunctionType MapDbCompareToSqlFunctionType(DbCompare comparison)
		{
			switch (comparison)
			{
				case DbCompare.Equal:				return SqlFunctionType.CompareEqual;
				case DbCompare.NotEqual:			return SqlFunctionType.CompareNotEqual;
				case DbCompare.LessThan:			return SqlFunctionType.CompareLessThan;
				case DbCompare.LessThanOrEqual:		return SqlFunctionType.CompareLessThanOrEqual;
				case DbCompare.GreaterThan:			return SqlFunctionType.CompareGreaterThan;
				case DbCompare.GreaterThanOrEqual:	return SqlFunctionType.CompareGreaterThanOrEqual;
				case DbCompare.Like:				return SqlFunctionType.CompareLike;
				case DbCompare.NotLike:				return SqlFunctionType.CompareNotLike;
			}
			
			throw new System.ArgumentException (string.Format ("Unsupported comparison {0}.", comparison), "comparison");
		}
		
		
		
		private ITypeConverter					type_converter;
		private Collections.SqlFields			sql_fields;
		private DbSelectRevision				revision;
		private DbCompareCombiner				combiner = DbCompareCombiner.And;
	}
}
