//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// The <c>AbstractDataRequest</c> class is the base class for all requests
	/// which manipulate data in data tables.
	/// </summary>
	
	[System.Serializable]
	
	public abstract class AbstractDataRequest : AbstractRequest, System.Runtime.Serialization.ISerializable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractDataRequest"/> class.
		/// </summary>
		protected AbstractDataRequest()
		{
		}


		/// <summary>
		/// Gets the name of the table.
		/// </summary>
		/// <value>The name of the table.</value>
		public string							TableName
		{
			get
			{
				return this.tableName;
			}
		}

		/// <summary>
		/// Gets the column names.
		/// </summary>
		/// <value>The column names.</value>
		public abstract ReadOnlyList<string>	ColumnNames
		{
			get;
		}

		/// <summary>
		/// Gets the column values.
		/// </summary>
		/// <value>The column values.</value>
		public abstract ReadOnlyList<object>	ColumnValues
		{
			get;
		}


		/// <summary>
		/// Defines the name of the table.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		public void DefineTableName(string tableName)
		{
			this.tableName = tableName;
		}
		
		
		#region ISerializable Members
		
		protected AbstractDataRequest(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
			this.tableName = info.GetValue (Strings.TableName, typeof (string)) as string;
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);

			info.AddValue (Strings.TableName, this.tableName);
		}
		
		#endregion

		#region Strings Class

		protected class Strings
		{
			public const string TableName		= "T.Name";
			public const string ColumnNames		= "C.Names";
			public const string ColumnValues	= "C.Values";
			public const string ColumnOriginals	= "C.Origs";
		}

		#endregion


		private string							tableName;
	}
}
