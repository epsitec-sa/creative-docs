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
		public UpdateStaticData() : base (Type.UpdateStaticData)
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
				return this.col_values;
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
				this.col_names  = new string[n];
				this.col_values = row.ItemArray;
				
				for (int i = 0; i < n; i++)
				{
					this.col_names[i] = columns[i].ColumnName;
				}
			}
			else if (mode == UpdateMode.Changed)
			{
				System.Collections.ArrayList names  = new System.Collections.ArrayList ();
				System.Collections.ArrayList values = new System.Collections.ArrayList ();
				
				int unique_count = 0;
				
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
					
					names.Add (columns[i].ColumnName);
					values.Add (o_current);
				}
				
				n = names.Count;
				
				if (n > unique_count)
				{
					this.col_names  = new string[n];
					this.col_values = new object[n];
					
					names.CopyTo (this.col_names);
					values.CopyTo (this.col_values);
				}
				else
				{
					this.col_names = null;
					this.col_values = null;
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
			this.SetupType (Type.UpdateStaticData);
			
			this.col_names  = info.GetValue ("ColNames", typeof (string[])) as string[];
			this.col_values = info.GetValue ("ColValues", typeof (object[])) as object[];
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
			
			info.AddValue ("ColNames", this.col_names);
			info.AddValue ("ColValues", this.col_values);
		}
		#endregion
		
		private string[]						col_names;
		private object[]						col_values;
	}
}
