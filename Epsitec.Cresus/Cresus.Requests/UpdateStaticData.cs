//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe UpdateStaticData définit une mise à jour statique (pré-
	/// calculée) d'une ligne dans une table.
	/// </summary>
	
	[System.Serializable]
	
	public class UpdateStaticData : AbstractData, System.Runtime.Serialization.ISerializable
	{
		public UpdateStaticData() : base (RequestType.UpdateStaticData)
		{
		}
		
		public UpdateStaticData(System.Data.DataRow row, UpdateMode mode) : this ()
		{
			this.DefineRow (row, mode);
		}
		
		
		public override string[]				ColumnNames
		{
			get
			{
				return this.col_names;
			}
		}
		
		public override object[]				ColumnValues
		{
			get
			{
				return this.col_cur_values;
			}
		}
		
		public object[]							ColumnOriginalValues
		{
			get
			{
				return this.col_org_values;
			}
		}
		
		public bool								ContainsData
		{
			get
			{
				return (this.col_names != null) && (this.col_names.Length > 0);
			}
		}
		
		
		public void DefineRow(System.Data.DataRow row, UpdateMode mode)
		{
			//	Selon le mode de mise à jour spécifié, on ne va enregistrer que les
			//	colonnes modifiées (et les colonnes servant d'index) ou alors tout
			//	enregistrer.
			
			System.Data.DataTable            table   = row.Table;
			System.Data.DataColumnCollection columns = table.Columns;
			
			int n = columns.Count;
			
			this.DefineTableName (table.TableName);
			
			if (mode == UpdateMode.Full)
			{
				//	Réalise une mise à jour de toutes les colonnes :
				
				this.col_names      = new string[n];
				this.col_cur_values = new object[n];
				this.col_org_values = new object[n];
				
				for (int i = 0; i < n; i++)
				{
					this.col_names[i]      = columns[i].ColumnName;
					this.col_cur_values[i] = row[i, System.Data.DataRowVersion.Current];
					this.col_org_values[i] = row[i, System.Data.DataRowVersion.Original];
				}
			}
			else if (mode == UpdateMode.Changed)
			{
				//	Réalise uniquement une mise à jour des colonnes modifiées et enregistre
				//	aussi les colonnes servant d'index :
				
				int[] indexes      = new int[n];
				int   unique_count = 0;
				int   index_count  = 0;
				
				//	Cherche quelles colonnes devront être conservées :
				
				for (int i = 0; i < n; i++)
				{
					object o_original = row[i, System.Data.DataRowVersion.Original];
					object o_current  = row[i, System.Data.DataRowVersion.Current];
					
					if (columns[i].Unique)
					{
						unique_count++;
					}
					else if (Epsitec.Common.Types.Comparer.Equal (o_current, o_original))
					{
						continue;
					}
					
					indexes[index_count] = i;
					index_count++;
				}
				
				//	Il se peut qu'aucune colonne n'ait été modifiée...
				
				if (index_count > unique_count)
				{
					this.col_names      = new string[index_count];
					this.col_cur_values = new object[index_count];
					this.col_org_values = new object[index_count];
					
					for (int i = 0; i < index_count; i++)
					{
						this.col_names[i]      = columns[indexes[i]].ColumnName;
						this.col_cur_values[i] = row[indexes[i], System.Data.DataRowVersion.Current];
						this.col_org_values[i] = row[indexes[i], System.Data.DataRowVersion.Original];
					}
				}
				else
				{
					this.col_names      = null;
					this.col_cur_values = null;
					this.col_org_values = null;
				}
			}
			else
			{
				throw new System.ArgumentException (string.Format ("Invalid mode {0} specified.", mode.ToString ()), "mode");
			}
		}
		
		
		#region ISerializable Members
		protected UpdateStaticData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
			this.SetupRequestType (RequestType.UpdateStaticData);
			
			System.Diagnostics.Debug.Assert (Epsitec.Common.Support.Serialization.Helper.FindElement (info, "ColNames"));
			
			this.col_names  = info.GetValue ("ColNames", typeof (string[])) as string[];
			this.col_cur_values = info.GetValue ("ColCurValues", typeof (object[])) as object[];
			this.col_org_values = info.GetValue ("ColOrgValues", typeof (object[])) as object[];
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
			
			info.AddValue ("ColNames", this.col_names);
			info.AddValue ("ColCurValues", this.col_cur_values);
			info.AddValue ("ColOrgValues", this.col_org_values);
		}
		#endregion
		
		private string[]						col_names;
		private object[]						col_cur_values;
		private object[]						col_org_values;
	}
}
