//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 03/11/2003

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataSet stocke une collection de DataFields.
	/// </summary>
	public class DataSet : DataRecord
	{
		public DataSet(string name)
		{
			this.name  = name;
			this.type  = new DataType (null);
			this.state = DataState.Added;
		}
		
		
		public DataField AddData(string name, object data, DataType data_type)
		{
			DataField record;
			
			if (this.data.Contains (name))
			{
				record = this.data[name] as DataField;
				
				if (record == null)
				{
					throw new DataException ("Incompatible change");
				}
				if (!record.DataType.Equals (data_type))
				{
					throw new DataException ("Incompatible type");
				}
				
				record.SetData (DataVersion.Active, data);
			}
			else
			{
				record = new DataField ();
				
				record.SetParent (this);
				record.SetData (DataVersion.Active, data);
				record.SetDataType (data_type);
				
				this.data.Add (name, record);
			}
			
			this.MarkAsModified ();
			this.NotifyDataChanged (name);
			
			return record;
		}
		
		public DataField UpdateData(string name, object data)
		{
			DataField record = this.FindRecord (name) as DataField;
			
			if (record == null)
			{
				throw new DataException (string.Format ("Invalid update for '{0}' in data set", name));
			}
			
			record.SetData (data);
			
			this.MarkAsModified ();
			this.NotifyDataChanged (name);
			
			return record;
		}
		
		public DataField ResetData(string name)
		{
			DataField record = this.FindRecord (name, DataVersion.ActiveOrDead) as DataField;
			
			if (record == null)
			{
				throw new DataException (string.Format ("Invalid reset for '{0}' in data set", name));
			}
			
			record.ResetData ();
			
			this.NotifyDataChanged (name);
			
			return record;
		}
		
		public void RemoveData(string name)
		{
			DataField record = this.FindRecord (name, DataVersion.ActiveOrDead) as DataField;
			
			if (record == null)
			{
				throw new DataException (string.Format ("Invalid remove for '{0}' in data set", name));
			}
			
			this.Remove (name);
			this.NotifyDataChanged (name);
		}
		
		
		public object GetData(string name)
		{
			DataField record = this.GetDataField (name);
			return (record == null) ? null : record.GetData ();
		}
		
		public DataField GetDataField(string name)
		{
			return this.FindRecord (name) as DataField;
		}
		
		
		
		protected void Remove(string name)
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
				
				this.MarkAsModified ();
				return;
			}
			
			throw new DataException (string.Format ("Cannot find '{0}' in data set", name));
		}
		
		
		public override bool					IsSet
		{
			get { return true; }
		}
		
		public override string					Name
		{
			get { return this.name; }
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
			string local_path = DataRecord.SplitPath (path, out remaining);
			
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
						
						if ((record.DataState != DataState.Removed) &&
							(record.DataState != DataState.Invalid))
						{
							return record.FindRecord (remaining, version);
						}
						break;
					
					case DataVersion.ActiveOrDead:
						return record.FindRecord (remaining, version);
				}
			}
			
			return null;
		}
		
		public override void ValidateChanges()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			foreach (System.Collections.DictionaryEntry entry in this.data)
			{
				string     name   = entry.Key as string;
				DataRecord record = entry.Value as DataRecord;
				
				switch (record.DataState)
				{
					case DataState.Removed:
					case DataState.Invalid:
						
						//	Les records qui ont été supprimés sont retirés définitivement de
						//	la table, ceux qui n'ont jamais existé aussi.
						
						list.Add (name);
						break;
					
					default:
						record.ValidateChanges ();
						break;
				}
			}
			
			if (list.Count > 0)
			{
				foreach (string name in list)
				{
					this.data.Remove (name);
				}
			}
		}
		
		
		protected string						name;
		protected System.Collections.Hashtable	data = new System.Collections.Hashtable ();
	}
}
