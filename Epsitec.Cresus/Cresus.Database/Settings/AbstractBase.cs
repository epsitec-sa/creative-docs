//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Database.Settings
{
	/// <summary>
	/// La classe AbstractBase sert de base pour les diverses variables
	/// globales (réglages) qui utilisent DbDict.
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
				//	Il y a eu des changements au niveau des données stockées dans
				//	le dictionnaire; il faut donc considérer une mise à jour dans
				//	la base de données.
				
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
		
		
		protected void Setup(DbInfrastructure infrastructure, DbTransaction transaction, string name)
		{
			this.dict = new DbDict ();
			this.dict.Attach (infrastructure, infrastructure.ResolveDbTable (transaction, name));
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
