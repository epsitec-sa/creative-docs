//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 03/11/2003

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataField stocke une donnée unique.
	/// </summary>
	public class DataField : DataRecord
	{
		public DataField()
		{
		}
		
		
		internal void Initialise(DataState state, object org_data, object new_data)
		{
			if (this.State != DataState.Invalid)
			{
				throw new DataException ("Illegal field reinitialisation");
			}
			
			this.SetState (state);
			
			this.org_data = org_data;
			this.new_data = new_data;
		}
		
		
		public override DataRecord FindRecord(string path, DataVersion version)
		{
			if ((path == null) || (path.Length == 0))
			{
				return this;
			}
			
			return null;
		}
		
		public override void ValidateChanges()
		{
			switch (this.State)
			{
				case DataState.Unchanged:
					break;
				
				case DataState.Added:
				case DataState.Modified:
					this.org_data = this.new_data;
					this.new_data = null;
					break;
				
				default:
					this.org_data = null;
					this.new_data = null;
					break;
			}
			
			base.ValidateChanges ();
		}
		
		
		public object GetData()
		{
			return this.GetData (DataVersion.Active);
		}
		
		public object GetData(DataVersion version)
		{
			object data = null;
			
			switch (version)
			{
				case DataVersion.Original:
					if (this.State != DataState.Added)
					{
						data = this.org_data;
						data = DataCopier.Copy (data);
					}
					break;
				
				case DataVersion.Active:
					if (this.State != DataState.Removed)
					{
						data = (this.State == DataState.Unchanged) ? this.org_data : this.new_data;
						data = DataCopier.Copy (data);
					}
					break;
				
				case DataVersion.ActiveOrDead:
					data = (this.State == DataState.Unchanged) ? this.org_data : this.new_data;
					data = DataCopier.Copy (data);
					break;
			}
			
			return data;
		}
		
		
		public void SetDataType(DataType type)
		{
			if (this.type != null)
			{
				if (this.type.Equals (type))
				{
					//	On redéfinit le champ comme ayant le même type qu'avant, ce
					//	qui est toléré.
					
					return;
				}
				
				throw new DataException ("Cannot set data type more than once");
			}
			
			this.type = type;
		}
		
		public void SetData(object data)
		{
			this.SetData (DataVersion.Active, data);
		}
		
		public void SetData(DataVersion version, object data)
		{
			switch (version)
			{
				case DataVersion.Original:
					this.org_data = DataCopier.Copy (data);
					return;
				
				case DataVersion.Active:
				case DataVersion.ActiveOrDead:
					
					//	Modifie la version active...
					
					switch (this.State)
					{
						case DataState.Unchanged:
							this.SetState (DataState.Modified);
							this.new_data = DataCopier.Copy (data);
							return;
						
						case DataState.Added:
						case DataState.Modified:
							this.new_data = DataCopier.Copy (data);
							return;
						
						case DataState.Removed:
							this.SetState (DataState.Modified);
							this.new_data = DataCopier.Copy (data);
							return;
						
						case DataState.Invalid:
							this.SetState (DataState.Added);
							this.new_data = DataCopier.Copy (data);
							return;
					}
					break;
			}
			
			throw new DataException ("SetData failed");
		}
		
		
		public void ResetData()
		{
			switch (this.State)
			{
				case DataState.Unchanged:
				case DataState.Invalid:
					break;
				
				case DataState.Modified:
				case DataState.Removed:
					this.new_data = null;
					this.SetState (DataState.Unchanged);
					break;
				case DataState.Added:
					this.new_data = null;
					this.SetState (DataState.Invalid);
					break;
			}
		}
		
		
		internal void ReplaceData(DataVersion version, object data)
		{
			switch (version)
			{
				case DataVersion.Original:
					this.org_data = data;
					break;
				case DataVersion.Active:
				case DataVersion.ActiveOrDead:
					this.new_data = data;
					break;
			}
		}
		
		internal object AccessData(DataVersion version, object data)
		{
			switch (version)
			{
				case DataVersion.Original:
					return this.org_data;
				case DataVersion.Active:
				case DataVersion.ActiveOrDead:
					return this.new_data;
			}
			
			return null;
		}
		
		
		protected object						org_data;
		protected object						new_data;
	}
}
