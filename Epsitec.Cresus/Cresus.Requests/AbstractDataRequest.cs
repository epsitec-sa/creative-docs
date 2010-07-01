//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using System.Collections.ObjectModel;

using System.Runtime.Serialization;


namespace Epsitec.Cresus.Requests
{
	
	
	/// <summary>
	/// The <c>AbstractDataRequest</c> class is the base class for all requests
	/// which manipulate data in data tables.
	/// </summary>	
	[System.Serializable]
	public abstract class AbstractDataRequest : AbstractRequest
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
		public string TableName
		{
			get;
			protected set;
		}

		
		/// <summary>
		/// Gets the column names.
		/// </summary>
		/// <value>The column names.</value>
		public abstract ReadOnlyCollection<string>	ColumnNames
		{
			get;
		}

		
		/// <summary>
		/// Gets the column values.
		/// </summary>
		/// <value>The column values.</value>
		public abstract ReadOnlyCollection<object> ColumnValues
		{
			get;
		}
		
		
		#region ISerializable Members


		protected AbstractDataRequest(SerializationInfo info, StreamingContext context) : base (info, context)
		{
			this.TableName = info.GetString (SerializationKeys.TableName);
		}


		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData (info, context);

			info.AddValue (SerializationKeys.TableName, this.TableName);
		}
		

		#endregion


		#region SerializationKeys Class


		private static class SerializationKeys
		{
			public const string TableName = "T.Name";
		}


		#endregion


	}


}
