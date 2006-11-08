//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Database.Settings
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	
	/// <summary>
	/// The <c>AbstractBase</c> class is used to manipulate and store settings
	/// using a <see cref="DbDict"/> instance.
	/// </summary>
	public abstract class AbstractBase : INotifyPropertyChanged
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractBase"/> class.
		/// </summary>
		protected AbstractBase()
		{
		}

		/// <summary>
		/// Persists the settings to the database.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		public void PersistToBase(DbTransaction transaction)
		{
			StringDict saveDict = new StringDict (this.dict);
			
			int changesBefore = this.dict.ChangeCount;
			
			ObjectDictMapper.CopyToDict (this, this.dict);
			
			int changesAfter  = this.dict.ChangeCount;
			
			if (changesAfter != changesBefore)
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
					StringDict.Copy (saveDict, this.dict);
					throw;
				}
			}
		}

		/// <summary>
		/// Creates the table in the database.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="transaction">The transaction.</param>
		/// <param name="name">The table name.</param>
		/// <param name="category">The category.</param>
		/// <param name="revisionMode">The revision mode.</param>
		/// <param name="replicationMode">The replication mode.</param>
		public static void CreateTable(DbInfrastructure infrastructure, DbTransaction transaction, string name, DbElementCat category, DbRevisionMode revisionMode, DbReplicationMode replicationMode)
		{
			DbDict.CreateTable (infrastructure, transaction, AbstractBase.CreateTableName (name), category, revisionMode, replicationMode);
		}

		/// <summary>
		/// Creates the name of the database table.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The name of the database table.</returns>
		protected static string CreateTableName(string name)
		{
			return name;
		}

		/// <summary>
		/// Loads the settings from the database.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="transaction">The transaction.</param>
		/// <param name="name">The table name.</param>
		protected void AttachAndLoad(DbInfrastructure infrastructure, DbTransaction transaction, string name)
		{
			this.dict = new DbDict ();
			this.dict.Attach (infrastructure, infrastructure.ResolveDbTable (transaction, AbstractBase.CreateTableName (name)));
			this.dict.RestoreFromBase (transaction);
			
			Epsitec.Common.Support.ObjectDictMapper.CopyFromDict (this, this.dict);
		}

		/// <summary>
		/// Notifies that a property changed.
		/// </summary>
		/// <param name="name">The property name.</param>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected void NotifyPropertyChanged(string name, object oldValue, object newValue)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged (this, new Epsitec.Common.Types.DependencyPropertyChangedEventArgs (name, oldValue, newValue));
			}
		}

		#region INotifyPropertyChanged Members
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		#endregion
		
		private DbDict							dict;
	}
}
