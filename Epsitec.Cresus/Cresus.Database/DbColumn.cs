//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Database.Collections;
using System.Collections.Generic;
using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbColumn</c> class describes a column in a database table.
	/// This is our version of the column metadata wrapper (compare with the
	/// ADO.NET <see cref="System.Data.DataColumn"/> class).
	/// </summary>
	public sealed class DbColumn : IName, ICaption, IXmlSerializable, System.IEquatable<DbColumn>, System.ICloneable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumn"/> class.
		/// </summary>
		public DbColumn()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumn"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		public DbColumn(string name, DbTypeDef type)
			: this ()
		{
			this.DefineDisplayName (name);
			this.DefineType (type);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumn"/> class.
		/// </summary>
		/// <param name="captionId">The caption DRUID.</param>
		/// <param name="type">The type.</param>
		public DbColumn(Druid captionId, DbTypeDef type)
			: this ()
		{
			this.DefineType (type);
			this.DefineCaptionId (captionId);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumn"/> class.
		/// </summary>
		/// <param name="captionId">The caption DRUID.</param>
		/// <param name="type">The type.</param>
		/// <param name="category">The category.</param>
		public DbColumn(Druid captionId, DbTypeDef type, DbElementCat category)
			: this (captionId, type)
		{
			this.DefineCategory (category);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumn"/> class.
		/// </summary>
		/// <param name="field">The structured type field definition.</param>
		public DbColumn(StructuredTypeField field)
			: this (field.CaptionId, new DbTypeDef (field.Type))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumn"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="columnClass">The column class.</param>
		public DbColumn(string name, DbTypeDef type, DbColumnClass columnClass)
			: this (name, type)
		{
			this.DefineColumnClass (columnClass);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumn"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="columnClass">The column class.</param>
		/// <param name="category">The category.</param>
		public DbColumn(string name, DbTypeDef type, DbColumnClass columnClass, DbElementCat category)
			: this (name, type, columnClass)
		{
			this.DefineCategory (category);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumn"></see> class.
		/// </summary>
		/// <param name="captionId">The caption id.</param>
		/// <param name="type">The type.</param>
		/// <param name="columnClass">The column class.</param>
		/// <param name="category">The category.</param>
		public DbColumn(Druid captionId, DbTypeDef type, DbColumnClass columnClass, DbElementCat category)
			: this (captionId, type)
		{
			this.DefineColumnClass (columnClass);
			this.DefineCategory (category);
		}

		
		#region IName Members


		/// <summary>
		/// Gets the name of the column.
		/// </summary>
		/// <value>The name of the column.</value>
		public string							Name
		{
			get
			{
				if (this.captionId.IsValid)
				{
					return DbColumn.GetColumnName (this.CaptionId);
				}
				else
				{
					return this.name;
				}
			}
		}

		#endregion

		/// <summary>
		/// Gets the display name of the column. If no name is defined, tries to use
		/// the caption name instead.
		/// </summary>
		/// <value>The display name of the column.</value>
		public string DisplayName
		{
			get
			{
				if (this.name == null)
				{
					Caption caption = this.Caption;
					return caption == null ? null : caption.Name;
				}
				else
				{
					return this.name;
				}
			}
		}

		#region ICaption Members

		/// <summary>
		/// Gets the caption id for the column.
		/// </summary>
		/// <value>The caption DRUID.</value>
		public Druid CaptionId
		{
			get
			{
				return this.captionId;
			}
		}

		#endregion

		/// <summary>
		/// Gets the caption for the column.
		/// </summary>
		/// <value>The caption or <c>null</c> if the <c>CaptionId</c> is not valid.</value>
		public Caption							Caption
		{
			get
			{
				if (this.caption == null)
				{
					this.caption = DbContext.Current.ResourceManager.GetCaption (this.captionId) ?? DbColumn.nullCaption;
				}

				if (this.caption == DbColumn.nullCaption)
				{
					return null;
				}
				else
				{
					return this.caption;
				}
			}
		}


		/// <summary>
		/// Gets the key for the column, used internally to identify the column
		/// metadata.
		/// </summary>
		/// <value>The key.</value>
		public DbKey							Key
		{
			get
			{
				return this.key;
			}
		}


		/// <summary>
		/// Gets the name of the target table, if the column is a reference to
		/// another table (foreign key).
		/// </summary>
		/// <value>The name of the target table.</value>
		public string							TargetTableName
		{
			get
			{
				return this.targetTableName;
			}
		}

		/// <summary>
		/// Gets the name of the target column, if the column is a reference to
		/// another table. The target column contains the key in the target table
		/// which is used to create the relation.
		/// </summary>
		/// <value>The name of the target column.</value>
		public string							TargetColumnName
		{
			get
			{
				if (this.columnClass == DbColumnClass.RefId)
				{
					return Tags.ColumnId;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the containing table.
		/// </summary>
		/// <value>The containing table.</value>
		public DbTable							Table
		{
			get
			{
				return this.table;
			}
		}

		/// <summary>
		/// Gets the index of the column in the containing column.
		/// </summary>
		/// <value>The index of the column or <c>-1</c> if the column does not
		/// belong to a table.</value>
		public int								TableColumnIndex
		{
			get
			{
				if (this.table != null)
				{
					return this.table.Columns.IndexOf (this);
				}
				else
				{
					return -1;
				}
			}
		}


		/// <summary>
		/// Gets the type of the column.
		/// </summary>
		/// <value>The type definition.</value>
		public DbTypeDef						Type
		{
			get
			{
				return this.type;
			}
		}


		/// <summary>
		/// Gets the category of the column (internal column or user data).
		/// </summary>
		/// <value>The category.</value>
		public DbElementCat						Category
		{
			get
			{
				return this.category;
			}
		}


		/// <summary>
		/// Gets a value indicating whether this column is a primary key.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this column is a primary key; otherwise, <c>false</c>.
		/// </value>
		public bool								IsPrimaryKey
		{
			get
			{
				return this.isPrimaryKey;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this column defines a foreign key.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this column defines a foreign key; otherwise, <c>false</c>.
		/// </value>
		public bool								IsForeignKey
		{
			get
			{
				return (string.IsNullOrEmpty (this.TargetTableName) == false)
					&& (string.IsNullOrEmpty (this.TargetColumnName) == false);
			}
		}

		/// <summary>
		/// Gets the column class (data, reference, key, etc.).
		/// </summary>
		/// <value>The column class.</value>
		public DbColumnClass					ColumnClass
		{
			get
			{
				return this.columnClass;
			}
		}

		/// <summary>
		/// Gets the cardinality for the column relation.
		/// </summary>
		/// <value>The cardinality.</value>
		public DbCardinality					Cardinality
		{
			get
			{
				return this.cardinality;
			}
		}


		public string Comment
		{
			get
			{
				return this.comment;
			}
			set
			{
				this.comment = value;
			}
		}


		public bool IsAutoIncremented
		{
			get
			{
				return this.isAutoIncremented;
			}
			set
			{
				this.isAutoIncremented = value;
			}
		}


		public long AutoIncrementStartValue
		{
			get
			{
				return this.autoIncrementStartValue;
			}
			set
			{
				value.ThrowIf (v => v < 0, "Value cannot be lower than zero.");

				this.autoIncrementStartValue = value;
			}
		}


		public bool IsNullable
		{
			get;
			set;
		}


		public bool IsAutoTimeStampOnInsert
		{
			get
			{
				return this.isAutoTimeStampOnInsert;
			}
			set
			{
				this.isAutoTimeStampOnInsert = value;
			}
		}


		public bool IsAutoTimeStampOnUpdate
		{
			get
			{
				return this.isAutoTimeStampOnUpdate;
			}
			set
			{
				this.isAutoTimeStampOnUpdate = value;
			}
		}


		/// <summary>
		/// Defines the column category. A column category may not be changed
		/// after it has been defined.
		/// </summary>
		/// <param name="category">The category.</param>
		internal void DefineCategory(DbElementCat category)
		{
			if (this.category == category)
			{
				return;
			}

			if (this.category == DbElementCat.Unknown)
			{
				this.category = category;
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Column '{0}' cannot define a new category", this.Name));
			}
		}

		/// <summary>
		/// Defines the containing table.
		/// </summary>
		/// <param name="table">The table.</param>
		internal void DefineTable(DbTable table)
		{
			this.table = table;
		}

		/// <summary>
		/// Defines the name of the target table. A target table may not be changed
		/// after it has been defined.
		/// </summary>
		/// <param name="targetTableName">Name of the target table.</param>
		internal void DefineTargetTableName(string targetTableName)
		{
			if (this.targetTableName == targetTableName)
			{
				return;
			}

			if (string.IsNullOrEmpty (this.targetTableName))
			{
				this.targetTableName = targetTableName;
			}
			else
			{
				string message = string.Format ("Column '{0}' cannot change its target table name from '{1}' to '{2}'.",
					/* */						this.Name, this.TargetTableName, targetTableName);
				throw new System.InvalidOperationException (message);
			}
		}

		/// <summary>
		/// Defines the column key. A column key may not be changed
		/// after it has been defined.
		/// </summary>
		/// <param name="key">The key.</param>
		internal void DefineKey(DbKey key)
		{
			if (this.key == key)
			{
				return;
			}

			if (this.key.IsEmpty)
			{
				this.key = key;
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Column '{0}' cannot change its key", this.Name));
			}
		}

		/// <summary>
		/// Defines the column name. The name can be specified explicitely or as
		/// a caption identifier (DRUID), in which case this method behaves like
		/// the method <see cref="DefineCaptionId"/>.
		/// </summary>
		/// <param name="name">The name or a caption DRUID (like <c>"[1234]"</c>).</param>
		internal void DefineDisplayName(string name)
		{
			if (Druid.IsValidResourceId (name))
			{
				this.DefineCaptionId (Druid.Parse (name));
			}
			else
			{
				this.name = name;
			}
		}

		/// <summary>
		/// Defines the caption id for the column. This clears the name, as it
		/// will be derived automatically from the caption.
		/// </summary>
		/// <param name="captionId">The caption DRUID.</param>
		internal void DefineCaptionId(Druid captionId)
		{
			this.captionId = captionId;
			this.caption   = null;
			this.name      = null;
		}



		/// <summary>
		/// Defines the column type. The column type may not be changed
		/// after it has been defined.
		/// </summary>
		/// <param name="value">The type definition.</param>
		internal void DefineType(DbTypeDef value)
		{
			if (this.type == value)
			{
				return;
			}

			if (this.type == null)
			{
				this.type = value;
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Column '{0}' cannot change its type", this.Name));
			}
		}

		/// <summary>
		/// Defines the column class (data, reference, key, etc.).
		/// </summary>
		/// <param name="value">The column class.</param>
		internal void DefineColumnClass(DbColumnClass value)
		{
			this.columnClass = value;
		}

		/// <summary>
		/// Defines the column cardinality.
		/// </summary>
		/// <param name="value">The cardinality.</param>
		public void DefineCardinality(DbCardinality value)
		{
			if (this.cardinality == value)
			{
				return;
			}
			if (this.cardinality == DbCardinality.None)
			{
				this.cardinality = value;
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Column '{0}' cannot change its cardinality", this.Name));
			}
		}

		/// <summary>
		/// Defines whether the column is used as part of the primary key.
		/// </summary>
		/// <param name="value">if set to <c>true</c>, the column is part of the primary key.</param>
		internal void DefinePrimaryKey(bool value)
		{
			this.isPrimaryKey = value;
		}

		/// <summary>
		/// Converts a simple value (using a unified type) to an ADO.NET compatible
		/// value. This will, for instance, convert a <see cref="Date"/> to a
		/// <see cref="System.DateTime"/> value.
		/// </summary>
		/// <param name="simpleValue">The simple value.</param>
		/// <returns>The ADO.NET compatible value.</returns>
		public object ConvertSimpleToAdo(object simpleValue)
		{
			if (simpleValue == System.DBNull.Value)
			{
				return simpleValue;
			}
			else
			{
				return TypeConverter.ConvertFromSimpleType (simpleValue, this.Type.SimpleType, this.Type.NumDef);
			}
		}

		/// <summary>
		/// Converts an ADO.NET value to a simple value (using a unified type).
		/// This will, for instance, convert a <see cref="System.DateTime"/> to
		/// a <see cref="Date"/> value.
		/// </summary>
		/// <param name="adoValue">The ADO.NET compatible value.</param>
		/// <returns>The simple value.</returns>
		public object ConvertAdoToSimple(object adoValue)
		{
			if (adoValue == System.DBNull.Value)
			{
				return adoValue;
			}
			else
			{
				return TypeConverter.ConvertToSimpleType (adoValue, this.Type.SimpleType, this.Type.NumDef);
			}
		}

		/// <summary>
		/// Converts a low level internal value, as stored in the database, to
		/// a higher level ADO.NET equivalent. This will, for instance, convert
		/// a <see cref="string"/> back to a <see cref="System.Guid"/> value.
		/// </summary>
		/// <param name="typeConverter">The type converter.</param>
		/// <param name="internalValue">The internal value.</param>
		/// <returns>The ADO.NET compatible value.</returns>
		public object ConvertInternalToAdo(ITypeConverter typeConverter, object internalValue)
		{
			if (internalValue == System.DBNull.Value)
			{
				return internalValue;
			}
			else
			{
				IRawTypeConverter rawConverter;
				DbRawType rawType = this.type.RawType;

				if (typeConverter.CheckNativeSupport (rawType))
				{
					return internalValue;
				}
				else if (typeConverter.GetRawTypeConverter (rawType, out rawConverter))
				{
					return rawConverter.ConvertFromInternalType (internalValue);
				}
				else
				{
					throw new System.InvalidOperationException (string.Format ("Data in column '{0}' cannot be converted", this.Name));
				}
			}
		}

		/// <summary>
		/// Converts a high level ADO.NET value to a lower level internal value,
		/// as stored in the database. This will, for instance, convert a
		/// <see cref="System.Guid"/> value to a <see cref="string"/> if the
		/// underlying database does not know how to handle GUIDs.
		/// </summary>
		/// <param name="typeConverter">The type converter.</param>
		/// <param name="adoValue">The ADO.NET compatible value.</param>
		/// <returns>The low level internal value.</returns>
		public object ConvertAdoToInternal(ITypeConverter typeConverter, object adoValue)
		{
			if (adoValue == System.DBNull.Value)
			{
				return adoValue;
			}
			else
			{
				IRawTypeConverter rawConverter;
				DbRawType rawType = this.type.RawType;

				if (typeConverter.CheckNativeSupport (rawType))
				{
					return adoValue;
				}
				else if (typeConverter.GetRawTypeConverter (rawType, out rawConverter))
				{
					return rawConverter.ConvertToInternalType (adoValue);
				}
				else
				{
					throw new System.InvalidOperationException (string.Format ("Data in column '{0}' cannot be converted", this.Name));
				}
			}
		}

		/// <summary>
		/// Creates the SQL column for this column definition.
		/// </summary>
		/// <param name="typeConverter">The type converter.</param>
		/// <returns>The SQL column.</returns>
		public SqlColumn CreateSqlColumn(ITypeConverter typeConverter)
		{
			DbRawType rawType = this.type.RawType;
			SqlColumn column;

			if (this.columnClass == DbColumnClass.KeyId)
			{
				if (this.Category != DbElementCat.Internal)
				{
					throw new System.InvalidOperationException (string.Format ("Column '{0}' category should be internal, but is {1}", this.Name, this.Category));
				}
			}

			IRawTypeConverter rawConverter;

			if (typeConverter.CheckNativeSupport (rawType))
			{
				column = new SqlColumn ();
				column.SetType (rawType, this.Type.Length, this.Type.IsFixedLength, DbCharacterEncoding.Unicode);
			}
			else if (typeConverter.GetRawTypeConverter (rawType, out rawConverter))
			{
				column = new SqlColumn ();
				column.SetType (rawConverter.InternalType, rawConverter.Length, rawConverter.IsFixedLength, rawConverter.Encoding);
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Column '{0}' cannot be translated to an SQL column", this.Name));
			}

			column.Name = this.GetSqlName ();
			column.Comment = this.Comment;
			column.IsNullable = this.IsNullable || this.Type.IsNullable;
			column.IsForeignKey = this.IsForeignKey;
	
			return column;
		}

		/// <summary>
		/// Creates the SQL name of the column. This name will most certainly
		/// be different from the high level <c>DbColumn.Name</c>.
		/// </summary>
		/// <returns>The SQL name.</returns>
		public string GetSqlName()
		{
			if (this.Category == DbElementCat.Internal)
			{
				//	Make sure the name of the internal column is properly prefixed
				//	with "CR_" or "CREF_" :

				return DbSqlStandard.MakeSimpleSqlName (this.Name, DbElementCat.Internal);
			}

			switch (this.columnClass)
			{
				case DbColumnClass.Data:		return DbSqlStandard.MakeSimpleSqlName (this.Name, this.Category);
				case DbColumnClass.KeyId:		return Tags.ColumnId;
				case DbColumnClass.RefId:		return DbSqlStandard.MakeSimpleSqlName (this.Name, "REF", "ID");
			}

			throw new System.NotSupportedException (string.Format ("Column '{0}' has an unsupported class", this.Name));
		}


		public static string GetColumnName(Druid fieldId)
		{
			return Druid.ToFullString (fieldId.ToLong ());
		}


		public static string GetColumnName(string field)
		{
			return field.Substring (1, field.Length - 2);
		}


		#region IEquatable<DbColumn> Members

		/// <summary>
		/// Compares the column with another column.
		/// </summary>
		/// <param name="other">Column to compare with.</param>
		/// <returns><c>true</c> if both columns have the same name.</returns>
		public bool Equals(DbColumn other)
		{
			if (object.ReferenceEquals (other, null))
			{
				return false;
			}
			else
			{
				return this.Name == other.Name;
				//	TODO: ...
			}
		}

		#endregion

		#region Equals and GetHashCode support
		
		public override bool Equals(object obj)
		{
			return this.Equals (obj as DbColumn);
		}

		public override int GetHashCode()
		{
			string name = this.Name;
			return string.IsNullOrEmpty (name) ? 0 : name.GetHashCode ();
		}

		#endregion

		
		public void EnsureIsDeserialized()
		{
			var columnCaption = this.Caption;
			var columnType = this.Type;

			if (columnType != null)
			{
				var columnTypeCaption = columnType.Caption;
			}
		}
        
        /// <summary>
		/// Deserializes a column from the specified XML reader.
		/// </summary>
		/// <param name="xmlReader">The XML reader.</param>
		/// <returns>The column.</returns>
		public static DbColumn Deserialize(System.Xml.XmlTextReader xmlReader)
		{
			if ((xmlReader.NodeType == System.Xml.XmlNodeType.Element) &&
				(xmlReader.LocalName == "column"))
			{
				DbColumn column = new DbColumn ();
				bool isEmptyElement = xmlReader.IsEmptyElement;

				column.captionId         = DbTools.ParseDruid (xmlReader.GetAttribute ("capt"));
				column.category          = DbTools.ParseElementCategory (xmlReader.GetAttribute ("cat"));
				column.columnClass       = DbTools.ParseColumnClass (xmlReader.GetAttribute ("class"));
				column.cardinality       = DbTools.ParseCardinality (xmlReader.GetAttribute ("card"));
				column.isPrimaryKey      = DbTools.ParseDefaultingToFalseBool (xmlReader.GetAttribute ("pk"));
				column.targetTableName   = DbTools.ParseString (xmlReader.GetAttribute ("ttab"));
				column.comment			 = DbTools.ParseString (xmlReader.GetAttribute ("com"));
				column.isAutoIncremented = DbTools.ParseDefaultingToFalseBool (xmlReader.GetAttribute ("inc"));
				column.IsNullable		 = DbTools.ParseDefaultingToFalseBool (xmlReader.GetAttribute ("null"));
				column.IsAutoTimeStampOnInsert	 = DbTools.ParseDefaultingToFalseBool (xmlReader.GetAttribute ("atsi"));
				column.IsAutoTimeStampOnUpdate	 = DbTools.ParseDefaultingToFalseBool (xmlReader.GetAttribute ("atsu"));

				if (!isEmptyElement)
				{
					xmlReader.ReadEndElement ();
				}

				return column;
			}
			else
			{
				throw new System.Xml.XmlException (string.Format ("Unexpected element {0}", xmlReader.LocalName), null, xmlReader.LineNumber, xmlReader.LinePosition);
			}
		}

		#region IXmlSerializable Members

		/// <summary>
		/// Serializes the column using the specified XML writer.
		/// </summary>
		/// <param name="xmlWriter">The XML writer.</param>
		public void Serialize(System.Xml.XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("column");

			DbTools.WriteAttribute (xmlWriter, "capt", DbTools.DruidToString (this.CaptionId));
			DbTools.WriteAttribute (xmlWriter, "cat", DbTools.ElementCategoryToString (this.Category));
			DbTools.WriteAttribute (xmlWriter, "class", DbTools.ColumnClassToString (this.ColumnClass));
			DbTools.WriteAttribute (xmlWriter, "card", DbTools.CardinalityToString (this.Cardinality));
			DbTools.WriteAttribute (xmlWriter, "pk", DbTools.BoolDefaultingToFalseToString (this.IsPrimaryKey));
			DbTools.WriteAttribute (xmlWriter, "ttab", DbTools.StringToString (this.TargetTableName));
			DbTools.WriteAttribute (xmlWriter, "com", DbTools.StringToString (this.comment));
			DbTools.WriteAttribute (xmlWriter, "inc", DbTools.BoolDefaultingToFalseToString (this.isAutoIncremented));
			DbTools.WriteAttribute (xmlWriter, "null", DbTools.BoolDefaultingToFalseToString (this.IsNullable));
			DbTools.WriteAttribute (xmlWriter, "atsi", DbTools.BoolDefaultingToFalseToString (this.IsAutoTimeStampOnInsert));
			DbTools.WriteAttribute (xmlWriter, "atsu", DbTools.BoolDefaultingToFalseToString (this.IsAutoTimeStampOnUpdate));
			
			xmlWriter.WriteEndElement ();
		}

		#endregion


		#region ICloneable Members


		public object Clone()
		{
			DbColumn clone = new DbColumn ();

			clone.type = this.type;
			clone.table = this.table;
			clone.name = this.name;
			clone.targetTableName = this.targetTableName;
			clone.captionId = this.captionId;
			clone.caption = this.caption;
			clone.key = this.key;
			clone.isPrimaryKey = this.isPrimaryKey;
			clone.isAutoIncremented = this.isAutoIncremented;
			clone.isAutoTimeStampOnInsert = this.isAutoTimeStampOnInsert;
			clone.isAutoTimeStampOnUpdate = this.isAutoTimeStampOnUpdate;
			clone.autoIncrementStartValue = this.autoIncrementStartValue;
			clone.comment = this.comment;
			clone.category = this.category;
			clone.columnClass = this.columnClass;
			clone.cardinality = this.cardinality;
			clone.IsNullable = this.IsNullable;

			return clone;
		}


		#endregion


		private static readonly Caption nullCaption = new Caption ();

		private DbTypeDef						type;
		private DbTable							table;

		private string							name;
		private string							targetTableName;
		private Druid							captionId;
		private Caption							caption;

		private DbKey							key;
		
		private bool							isPrimaryKey;
		private bool							isAutoIncremented;
		private bool							isAutoTimeStampOnInsert;
		private bool							isAutoTimeStampOnUpdate;
		private long							autoIncrementStartValue;
		private string							comment;
		private DbElementCat					category;
		private DbColumnClass					columnClass;
		private DbCardinality					cardinality;

	}
}
