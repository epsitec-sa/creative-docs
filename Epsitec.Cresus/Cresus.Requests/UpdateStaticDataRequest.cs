//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// The <c>UpdateStaticDataRequest</c> class describes a static data update.
	/// The data provided is plain static data which needs not be recomputed but
	/// can be used as is..
	/// </summary>
	
	[System.Serializable]
	
	public class UpdateStaticDataRequest : AbstractDataRequest, System.Runtime.Serialization.ISerializable
	{
		public UpdateStaticDataRequest()
		{
		}

		public UpdateStaticDataRequest(System.Data.DataRow row, UpdateMode mode)
			: this ()
		{
			//	Selon le mode de mise à jour spécifié, on ne va enregistrer que les
			//	colonnes modifiées (et les colonnes servant d'index) ou alors tout
			//	enregistrer.

			System.Data.DataTable            table   = row.Table;
			System.Data.DataColumnCollection columns = table.Columns;

			int n = columns.Count;

			this.DefineTableName (table.TableName);

			switch (mode)
			{
				case UpdateMode.Full:
					
					//	Update data in every column of the specified row :

					if (n > 0)
					{
						this.columnNames          = new string[n];
						this.columnCurrentValues  = new object[n];
						this.columnOriginalValues = new object[n];

						for (int i = 0; i < n; i++)
						{
							this.columnNames[i]          = columns[i].ColumnName;
							this.columnCurrentValues[i]  = row[i, System.Data.DataRowVersion.Current];
							this.columnOriginalValues[i] = row[i, System.Data.DataRowVersion.Original];
						}
					}
					break;

				case UpdateMode.Changed:

					//	Only record columns which contain changed data or which belong to
					//	the index and/or primary key :

					if (n > 0)
					{
						int[] indexes     = new int[n];
						int   uniqueCount = 0;
						int   indexCount  = 0;

						//	Find out which columns will have to be recorded :

						for (int i = 0; i < n; i++)
						{
							object originalData = row[i, System.Data.DataRowVersion.Original];
							object currentData  = row[i, System.Data.DataRowVersion.Current];

							if (columns[i].Unique)
							{
								uniqueCount++;
							}
							else if (Epsitec.Common.Types.Comparer.Equal (currentData, originalData))
							{
								continue;
							}

							indexes[indexCount] = i;
							indexCount++;
						}

						//	There might be zero modified columns, even though there are some
						//	columns marked as "need recording"...

						if (indexCount > uniqueCount)
						{
							this.columnNames          = new string[indexCount];
							this.columnCurrentValues  = new object[indexCount];
							this.columnOriginalValues = new object[indexCount];

							for (int i = 0; i < indexCount; i++)
							{
								int sourceIndex = indexes[i];

								this.columnNames[i]          = columns[sourceIndex].ColumnName;
								this.columnCurrentValues[i]  = row[sourceIndex, System.Data.DataRowVersion.Current];
								this.columnOriginalValues[i] = row[sourceIndex, System.Data.DataRowVersion.Original];
							}
						}
					}
					break;

				default:
					throw new System.ArgumentException (string.Format ("Invalid mode {0} specified.", mode.ToString ()), "mode");
			}
		}


		/// <summary>
		/// Gets the column names.
		/// </summary>
		/// <value>The column names.</value>
		public override ReadOnlyList<string>	ColumnNames
		{
			get
			{
				return new ReadOnlyList<string> (this.columnNames);
			}
		}

		/// <summary>
		/// Gets the column values.
		/// </summary>
		/// <value>The column values.</value>
		public override ReadOnlyList<object>	ColumnValues
		{
			get
			{
				return new ReadOnlyList<object> (this.columnCurrentValues);
			}
		}

		/// <summary>
		/// Gets the column original values.
		/// </summary>
		/// <value>The column original values.</value>
		public ReadOnlyList<object>				ColumnOriginalValues
		{
			get
			{
				return new ReadOnlyList<object> (this.columnOriginalValues);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this request contains any data.
		/// </summary>
		/// <value><c>true</c> if this request contains any data; otherwise, <c>false</c>.</value>
		public bool								ContainsData
		{
			get
			{
				return (this.columnNames != null) && (this.columnNames.Length > 0);
			}
		}


		/// <summary>
		/// Executes the request using the specified execution engine.
		/// </summary>
		/// <param name="engine">The execution engine.</param>
		public override void Execute(ExecutionEngine engine)
		{
			if (this.columnNames != null)
			{
				engine.GenerateUpdateDataCommand (this.TableName, this.columnNames, this.columnOriginalValues, this.columnNames, this.columnCurrentValues);
			}
		}
		
		
		#region ISerializable Members
		
		protected UpdateStaticDataRequest(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
			System.Diagnostics.Debug.Assert (Epsitec.Common.Support.Serialization.Helper.FindElement (info, "ColNames"));
			
			this.columnNames  = info.GetValue (Strings.ColumnNames, typeof (string[])) as string[];
			this.columnCurrentValues = info.GetValue (Strings.ColumnValues, typeof (object[])) as object[];
			this.columnOriginalValues = info.GetValue (Strings.ColumnOriginals, typeof (object[])) as object[];
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
			
			info.AddValue (Strings.ColumnNames, this.columnNames);
			info.AddValue (Strings.ColumnValues, this.columnCurrentValues);
			info.AddValue (Strings.ColumnOriginals, this.columnOriginalValues);
		}
		
		#endregion
		
		private string[]						columnNames;
		private object[]						columnCurrentValues;
		private object[]						columnOriginalValues;
	}
}
