//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Database.Settings
{
	/// <summary>
	/// La classe AbstractBase sert de base pour les diverses variables
	/// globales (r�glages) qui utilisent DbDict.
	/// </summary>
	public abstract class AbstractBase : IPropertyChange
	{
		protected AbstractBase()
		{
		}
		
		
		public void PersistToBase(DbTransaction transaction)
		{
			StringDict save_dict = new StringDict (this.dict);
			
			int changes_before = this.dict.ChangeCount;
			
			ObjectDictMapper.CopyToDict (this, this.dict);
			
			int changes_after  = this.dict.ChangeCount;
			
			if (changes_after != changes_before)
			{
				//	Il y a eu des changements au niveau des donn�es stock�es dans
				//	le dictionnaire; il faut donc consid�rer une mise � jour dans
				//	la base de donn�es.
				
				try
				{
					this.dict.SerializeToBase (transaction);
				}
				catch
				{
					StringDict.Copy (save_dict, this.dict);
					throw;
				}
			}
		}
		
		
		public static string CreateTableName(string name)
		{
			return name;
		}
		
		public static void CreateTable(DbInfrastructure infrastructure, DbTransaction transaction, string name, DbElementCat category, DbRevisionMode revision_mode, DbReplicationMode replication_mode)
		{
			string table_name = AbstractBase.CreateTableName (name);
			
			DbDict.CreateTable (infrastructure, transaction, table_name, category, revision_mode, replication_mode);
		}
		
		
		protected void Setup(DbInfrastructure infrastructure, DbTransaction transaction, string name)
		{
			string table_name = AbstractBase.CreateTableName (name);
			
			this.dict = new DbDict ();
			this.dict.Attach (infrastructure, infrastructure.ResolveDbTable (transaction, table_name));
			this.dict.RestoreFromBase (transaction);
			
			Epsitec.Common.Support.ObjectDictMapper.CopyFromDict (this, this.dict);
		}
		
		protected void NotifyPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged (this, new PropertyChangedEventArgs (name));
			}
		}
		
		
		#region IPropertyChange Members
		public event Epsitec.Common.Support.PropertyChangedEventHandler PropertyChanged;
		#endregion
		
		private DbDict							dict;
	}
}
