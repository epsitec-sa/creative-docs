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
		
		
		public override bool					IsField
		{
			get { return true; }
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
			switch (this.data_state)
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
			
			this.MarkAsUnchanged ();
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
					if (this.data_state != DataState.Added)
					{
						data = this.org_data;
						data = DataCopier.Copy (data);
					}
					break;
				
				case DataVersion.Active:
					if (this.data_state != DataState.Removed)
					{
						data = (this.data_state == DataState.Unchanged) ? this.org_data : this.new_data;
						data = DataCopier.Copy (data);
					}
					break;
				
				case DataVersion.ActiveOrDead:
					data = (this.data_state == DataState.Unchanged) ? this.org_data : this.new_data;
					data = DataCopier.Copy (data);
					break;
			}
			
			return data;
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
					
					switch (this.data_state)
					{
						case DataState.Unchanged:
							this.data_state = DataState.Modified;
							this.new_data   = DataCopier.Copy (data);
							return;
						
						case DataState.Added:
						case DataState.Modified:
							this.new_data   = DataCopier.Copy (data);
							return;
						
						case DataState.Removed:
							this.data_state = DataState.Modified;
							this.new_data   = DataCopier.Copy (data);
							return;
						
						case DataState.Invalid:
							this.data_state = DataState.Added;
							this.new_data   = DataCopier.Copy (data);
							return;
					}
					break;
			}
			
			throw new DataException ("SetData failed");
		}
		
		public void ResetData()
		{
			switch (this.data_state)
			{
				case DataState.Unchanged:
				case DataState.Invalid:
					break;
				
				case DataState.Modified:
				case DataState.Removed:
					this.new_data   = null;
					this.data_state = DataState.Unchanged;
					break;
				case DataState.Added:
					this.new_data   = null;
					this.data_state = DataState.Invalid;
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
