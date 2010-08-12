using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Schema
{


	public class DataConverter
	{

		// TODO Comment this class.
		// Marc


		// TODO All this conversion stuff is kind of low level and it might make sense to move it (or
		// part of it) somewhere else, like in the Database namespace.
		// Marc


		public DataConverter(DataContext dataContext)
		{
			this.DataContext = dataContext;
		}


		private DataContext DataContext
		{
			get;
			set;
		}


		private EntityContext EntityContext
		{
			get
			{
				return this.DataContext.EntityContext;
			}
		}


		private DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.DataContext.DbInfrastructure;
			}
		}


		private SchemaEngine SchemaEngine
		{
			get
			{
				return this.DataContext.SchemaEngine;
			}
		}


		public object ToCresusValue(Druid leafEntityId, StructuredTypeField field, object value)
		{
			object newValue = value;

			if (newValue != System.DBNull.Value)
			{
				IStringType stringType = field.Type as IStringType;

				if (stringType != null)
				{
					if (stringType.UseFormattedText)
					{
						newValue = FormattedText.CastToFormattedText (newValue);
					}
				}
				else
				{
					Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, field.CaptionId);
					string columnName = this.SchemaEngine.GetEntityColumnName (field.CaptionId);

					DbTable dbTable = this.SchemaEngine.GetEntityTableDefinition (localEntityId);
					DbColumn dbColumn = dbTable.Columns[columnName];

					//	The conversion is a two step process:
					//	1. Convert from an ADO.NET type to a simple type (i.e. almost all numbers map to decimal)
					//	2. Convert from the simple type to the expected field type

					newValue = this.ToCresusValue (dbColumn, newValue);
					InvariantConverter.Convert (newValue, field, out newValue);
				}
			}

			return newValue;
		}


		private object ToCresusValue(DbColumn dbColumn, object value)
		{
			object newValue = value;

			DbRawType rawType = dbColumn.Type.RawType;

			ITypeConverter typeConverter = this.DbInfrastructure.Converter;

			if (!typeConverter.CheckNativeSupport (rawType))
			{
				IRawTypeConverter rawTypeConverter;
				bool sucess = typeConverter.GetRawTypeConverter (rawType, out rawTypeConverter);

				if (!sucess)
				{
					throw new System.NotSupportedException ("Unable to convert unsupported raw type: " + rawType);
				}

				newValue = rawTypeConverter.ConvertFromInternalType (newValue);
			}

			if (value != System.DBNull.Value)
			{
				DbSimpleType typeDef = dbColumn.Type.SimpleType;
				DbNumDef numDef = dbColumn.Type.NumDef;

				return TypeConverter.ConvertToSimpleType (value, typeDef, numDef);
			}

			return newValue;
		}


		public object ToDatabaseValue(DbTypeDef dbType, object value)
		{
			object newValue = value;

			if (value != System.DBNull.Value)
			{
				if (dbType.SimpleType == DbSimpleType.Decimal)
				{
					decimal decimalValue;

					bool success = InvariantConverter.Convert (value, out decimalValue);

					if (!success)
					{
						throw new System.ArgumentException ("Invalid value: not compatible with a numeric type");
					}

					newValue = decimalValue;
				}

				newValue = TypeConverter.ConvertFromSimpleType (newValue, dbType.SimpleType, dbType.NumDef);
			}

			DbRawType rawType = dbType.RawType;

			ITypeConverter typeConverter = this.DbInfrastructure.Converter;

			if (!typeConverter.CheckNativeSupport (rawType))
			{
				IRawTypeConverter rawTypeConverter;
				bool sucess = typeConverter.GetRawTypeConverter (rawType, out rawTypeConverter);

				if (!sucess)
				{
					throw new System.NotSupportedException ("Unable to convert unsupported raw type: " + rawType);
				}

				newValue = rawTypeConverter.ConvertToInternalType (newValue);
			}

			return newValue;
		}


		public DbRawType ToDatabaseType(DbTypeDef dbType)
		{
			DbRawType newRawType = dbType.RawType;

			ITypeConverter typeConverter = this.DbInfrastructure.Converter;

			if (!typeConverter.CheckNativeSupport (newRawType))
			{
				IRawTypeConverter rawTypeConverter;
				bool sucess = typeConverter.GetRawTypeConverter (newRawType, out rawTypeConverter);

				if (!sucess)
				{
					throw new System.NotSupportedException ("Unable to convert unsupported raw type: " + newRawType);
				}

				newRawType = rawTypeConverter.InternalType;
			}

			return newRawType;
		}


	}


}
