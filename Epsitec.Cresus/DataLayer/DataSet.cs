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
		
		public override bool					IsSet
		{
			get { return true; }
		}
		
		
		public override DataRecord FindRecord(string path)
		{
			string remaining;
			string local_path = this.SplitPath (path, out remaining);
			
			if (this.data.Contains (local_path))
			{
				DataRecord record = this.data[local_path] as DataRecord;
				
				if (remaining != null)
				{
					record = record.FindRecord (remaining);
				}
				
				return record;
			}
			
			return null;
		}
		
		
		protected System.Collections.Hashtable	data = new System.Collections.Hashtable ();
	}
}
