//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataSet stocke une collection de DataFields.
	/// </summary>
	public class DataSet : DataRecord
	{
		public DataSet()
		{
			this.data_type = new DataType ();
			this.data_type.Initialise (DataClass.Complex, null);
		}
		
		
		public void AddData(string name, object data)
		{
			if (this.data.Contains (name))
			{
				DataField record = this.data[name] as DataField;
				
				if (record == null)
				{
					throw new DataException ("Incompatible change");
				}
				
				record.SetData (DataVersion.Active, data);
			}
			else
			{
				DataField record = new DataField ();
				
				record.SetParent (this);
				record.SetData (DataVersion.Active, data);
				
				this.data.Add (name, record);
			}
		}
		
		public void Remove(string name)
		{
			if (this.data.Contains (name))
			{
				DataRecord record = this.data[name] as DataRecord;
				
				switch (record.DataState)
				{
					case DataState.Invalid:
						break;
					case DataState.Added:
						record.SetDataState (DataState.Invalid);
						break;
					case DataState.Removed:
						break;
					case DataState.Unchanged:
					case DataState.Modified:
						record.SetDataState (DataState.Removed);
						break;
				}
				
				return;
			}
			
			throw new DataException (string.Format ("Cannot find '{0}' in data set", name));
		}
		
		
		
		public override bool					IsSet
		{
			get { return true; }
		}
		
		
		public override DataRecord FindRecord(string path, DataVersion version)
		{
			if ((path == null) || (path.Length == 0))
			{
				return this;
			}
			
			//	Si aucune donnée n'est stockée dans ce DataSet, alors on ne peut
			//	de toute manière rien y trouver...
			
			if (this.data == null)
			{
				return null;
			}
			
			string remaining;
			string local_path = this.SplitPath (path, out remaining);
			
			if (this.data.Contains (local_path))
			{
				DataRecord record = this.data[local_path] as DataRecord;
				
				switch (version)
				{
					case DataVersion.Original:
						
						//	On demande une version originale: il faut donc éviter de
						//	continuer la recherche si on se rend compte que le record
						//	a été rajouté (il ne fait pas partie des données d'origine).
						
						if (record.DataState != DataState.Added)
						{
							return record.FindRecord (remaining, version);
						}
						break;
					
					case DataVersion.Active:
						
						//	On demande une version active: il faut donc éviter de
						//	continuer la recherche si on se rend compte que le record
						//	a été supprimé. Dans tous les autres cas, le record est à
						//	prendre en considération.
						
						if (record.DataState != DataState.Removed)
						{
							return record.FindRecord (remaining, version);
						}
						break;
				}
			}
			
			return null;
		}
		
		
		protected System.Collections.Hashtable	data = new System.Collections.Hashtable ();
	}
}
