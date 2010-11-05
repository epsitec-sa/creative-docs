using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Schema
{


	/// <summary>
	/// The <c>DataConverter</c> class is used to convert data and types between their representation
	/// in Cresus and their representation in the database. The conversion process uses an intermediate
	/// .NET representation.
	/// </summary>
	internal sealed class DataConverter
	{


		// TODO All this conversion stuff is kind of low level and it might make sense to move it (or
		// part of it) somewhere else, like in the Database name space. I suspect that we need this
		// because when we execute a query, the data is returned unchanged, but if we could somehow
		// intercept the data sets and convert them to something else like some kind of improved
		// "List<List<object>>", then we could do all this nasty conversion stuff at lower level and
		// would not be bothered by that here or anywhere else. That would be kind of cool.
		// Marc


		/// <summary>
		/// Creates a new <c>DataConverter</c>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> to be used by this instance.</param>
		public DataConverter(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");

			this.DataContext = dataContext;
		}


		/// <summary>
		/// The <see cref="DataContext"/> used by this instance.
		/// </summary>
		private DataContext DataContext
		{
			get;
			set;
		}


		/// <summary>
		/// The <see cref="DbInfrastructure"/> used by this instance.
		/// </summary>
		private DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.DataContext.DataInfrastructure.DbInfrastructure;
			}
		}


		/// <summary>
		/// Converts a value in the database representation to its Cresus representation.
		/// </summary>
		/// <param name="type">The expected type of the value in its Cresus representation.</param>
		/// <param name="rawType">The <see cref="DbRawType"/> of the value in its intermediate .NET representation.</param>
		/// <param name="simpleType">The <see cref="DbSimpleType"/> of the value in its intermediate .NET representation.</param>
		/// <param name="numDef">The <see cref="DbNumDef"/> of the value in its intermediate .NET representation.</param>
		/// <param name="value">The value in its database representation.</param>
		/// <returns>The value in its Cresus representation.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="type"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="rawType"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="rawType"/> is <c>unknown</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="simpleType"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="simpleType"/> is <c>unknown</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="numDef"/> is not <c>null</c> and invalid.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
		public object FromDatabaseToCresusValue(INamedType type, DbRawType rawType, DbSimpleType simpleType, DbNumDef numDef, object value)
		{
			type.ThrowIfNull ("type");
			rawType.ThrowIf (rt => rt==DbRawType.Null, "rawType cannot be null");
			rawType.ThrowIf (rt => rt==DbRawType.Unknown, "rawType cannot be unknown");
			simpleType.ThrowIf (st => st==DbSimpleType.Null, "simpleType cannot be null");
			simpleType.ThrowIf (st => st==DbSimpleType.Unknown, "simpleType cannot be unknown");
			numDef.ThrowIf (nd => nd!=null&&!nd.IsValid, "numDef must be null or valid");
			value.ThrowIfNull ("value");

			object newValue = value;

			if (newValue != System.DBNull.Value)
			{
				newValue = this.FromDatabaseToDotNetValue (rawType, newValue);
				newValue = this.FromDotNetToCresusValue (type, simpleType, numDef, newValue);
			}

			return newValue;
		}


		/// <summary>
		/// Converts a value in the database representation to the corresponding value in the .NET
		/// representation.
		/// </summary>
		/// <param name="rawType">The <see cref="DbRawType"/> of the value in its intermediate .NET representation.</param>
		/// <param name="value">The value in its database representation.</param>
		/// <returns>The value in its intermediate .NET representation.</returns>
		private object FromDatabaseToDotNetValue(DbRawType rawType, object value)
		{
			object newValue = value;

			ITypeConverter typeConverter = this.DbInfrastructure.Converter;

			if (!typeConverter.CheckNativeSupport (rawType))
			{
				IRawTypeConverter rawTypeConverter;
				bool success = typeConverter.GetRawTypeConverter (rawType, out rawTypeConverter);

				if (!success)
				{
					throw new System.NotSupportedException ("Unable to convert unsupported raw type: " + rawType);
				}

				newValue = rawTypeConverter.ConvertFromInternalType (newValue);
			}
			
			return newValue;
		}


		/// <summary>
		/// Converts a value in the intermediate .NET representation to the corresponding value in the Cresus
		/// representation.
		/// </summary>
		/// <param name="type">The expected type of the value in its Cresus representation.</param>
		/// <param name="simpleType">The <see cref="DbSimpleType"/> of the value in its intermediate .NET representation.</param>
		/// <param name="numDef">The <see cref="DbNumDef"/> of the value in its intermediate .NET representation.</param>
		/// <param name="value">The value in its intermediate .NET representation.</param>
		/// <returns>The value in its Cresus representation.</returns>
		private object FromDotNetToCresusValue(INamedType type, DbSimpleType simpleType, DbNumDef numDef, object value)
		{
			object newValue = value;

			newValue = TypeConverter.ConvertToSimpleType (newValue, simpleType, numDef);

			if (simpleType == DbSimpleType.String)
			{
				newValue = this.ProcessDotNetToCresusString (type, newValue);
			}
			else if (simpleType == DbSimpleType.Decimal)
			{
				newValue = this.ProcessDotNetToCresusNumber (type, newValue);
			}

			return newValue;
		}
		

		/// <summary>
		/// Applies a special treatment required for the string values.
		/// </summary>
		/// <param name="type">The expected type of the value in its Cresus representation.</param>
		/// <param name="value">The value in its Cresus representation.</param>
		/// <returns>The processed value in its Cresus representation</returns>
		private object ProcessDotNetToCresusString(INamedType type, object value)
		{
			object newValue = value;
			
			IStringType stringType = type as IStringType;

			if (stringType != null && stringType.UseFormattedText)
			{
				newValue = FormattedText.CastToFormattedText (newValue);
			}

			return newValue;
		}


		/// <summary>
		/// Applies a special treatment required for the number values.
		/// </summary>
		/// <param name="type">The expected type of the value in its Cresus representation.</param>
		/// <param name="value">The value in its Cresus representation.</param>
		/// <returns>The processed value in its Cresus representation</returns>
		private object ProcessDotNetToCresusNumber(INamedType type, object value)
		{
			object newValue = value;

			bool success = InvariantConverter.Convert (newValue, type.SystemType, out newValue);

			if (!success)
			{
				throw new System.NotSupportedException ("Unable to convert unsupported type: " + type);
			}

			return newValue;
		}


		/// <summary>
		/// Converts a value in the Cresus representation to its database representation.
		/// </summary>
		/// <param name="rawType">The <see cref="DbRawType"/> of the value in its intermediate .NET representation.</param>
		/// <param name="simpleType">The <see cref="DbSimpleType"/> of the value in its intermediate .NET representation.</param>
		/// <param name="numDef">The <see cref="DbNumDef"/> of the value in its intermediate .NET representation.</param>
		/// <param name="value">The value in its Cresus representation.</param>
		/// <returns>The value in its database representation.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="rawType"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="rawType"/> is <c>unknown</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="simpleType"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="simpleType"/> is <c>unknown</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="numDef"/> is not <c>null</c> and invalid.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
		public object FromCresusToDatabaseValue(DbRawType rawType, DbSimpleType simpleType, DbNumDef numDef, object value)
		{
			rawType.ThrowIf (rt => rt==DbRawType.Null, "rawType cannot be null");
			rawType.ThrowIf (rt => rt==DbRawType.Unknown, "rawType cannot be unknown");
			simpleType.ThrowIf (st => st==DbSimpleType.Null, "simpleType cannot be null");
			simpleType.ThrowIf (st => st==DbSimpleType.Unknown, "simpleType cannot be unknown");
			numDef.ThrowIf (nd => nd!=null&&!nd.IsValid, "numDef must be null or valid");
			value.ThrowIfNull ("value");
			
			object newValue = value;

			if (newValue != System.DBNull.Value)
			{
				newValue = this.FromCresusToDotNetValue (newValue, simpleType, numDef);
				newValue = this.FromDotNetToDatabaseValue (rawType, newValue);
			}

			return newValue;
		}


		/// <summary>
		/// Converts a value in the Cresus representation to the corresponding value in the .NET
		/// representation.
		/// </summary>
		/// <param name="simpleType">The <see cref="DbSimpleType"/> of the value in its intermediate .NET representation.</param>
		/// <param name="numDef">The <see cref="DbNumDef"/> of the value in its intermediate .NET representation.</param>
		/// <param name="value">The value in its Cresus representation.</param>
		/// <returns>The value in its intermediate .NET representation.</returns>
		private object FromCresusToDotNetValue(object value, DbSimpleType simpleType, DbNumDef numDef)
		{
			object newValue = value;

			if (simpleType == DbSimpleType.String)
			{
				newValue = this.ProcessCresusToDotNetString (newValue);
			}
			else if (simpleType == DbSimpleType.Decimal)
			{
				newValue = this.ProcessCresusToDotNetNumber (value);
			}
			
			newValue = TypeConverter.ConvertFromSimpleType (newValue, simpleType, numDef);

			return newValue;
		}

		
		/// <summary>
		/// Applies a special treatment required for the string values.
		/// </summary>
		/// <param name="value">The value in its Cresus representation.</param>
		/// <returns>The processed value in its Cresus representation</returns>
		private object ProcessCresusToDotNetString(object value)
		{
			object newValue = value;

			if (newValue is FormattedText)
			{
				newValue = FormattedText.CastToString (newValue);
			}

			return newValue;
		}


		/// <summary>
		/// Applies a special treatment required for the string number.
		/// </summary>
		/// <param name="value">The value in its Cresus representation.</param>
		/// <returns>The processed value in its Cresus representation</returns>
		private object ProcessCresusToDotNetNumber(object value)
		{
			decimal newValue;

			bool success = InvariantConverter.Convert (value, out newValue);

			if (!success)
			{
				throw new System.ArgumentException ("Invalid value: not compatible with a numeric type");
			}

			return newValue;
		}


		/// <summary>
		/// Converts a value in the intermediate .NET representation to the corresponding value in the database
		/// representation.
		/// </summary>
		/// <param name="rawType">The <see cref="DbRawType"/> of the value in its intermediate .NET representation.</param>
		/// <param name="value">The value in its intermediate .NET representation.</param>
		/// <returns>The value in its database representation.</returns>
		private object FromDotNetToDatabaseValue(DbRawType rawType, object value)
		{
			object newValue = value;

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


		/// <summary>
		/// Converts a <see cref="DbRawType"/> in the intermediate .NET representation to the corresponding
		/// <see cref="DbRawType"/> in the database representation.
		/// </summary>
		/// <param name="rawType">The <see cref="DbRawType"/> in its intermediate .NET representation.</param>
		/// <returns>The <see cref="DbRawType"/> in its database representation.</returns>
		public DbRawType FromDotNetToDatabaseType(DbRawType rawType)
		{
			DbRawType newRawType = rawType;

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
