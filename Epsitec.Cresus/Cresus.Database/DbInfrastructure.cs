//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbInfrastructure offre le support pour l'infrastructure
	/// nécessaire à toute base de données "Crésus" (tables internes, méta-
	/// données, etc.)
	/// </summary>
	public class DbInfrastructure : System.IDisposable
	{
		public DbInfrastructure()
		{
		}
		
		
		public void CreateDatabase(DbAccess db_access)
		{
			if (this.db_access.IsValid)
			{
				throw new DbException (this.db_access, "Database already exists");
			}
			
			this.db_access = db_access;
			this.db_access.Create = true;
			
			this.SetupDatabaseAbstraction ();
			
			//	La base de données vient d'être créée. Elle est donc toute vide (aucune
			//	table n'est encore définie).
			
			System.Diagnostics.Debug.Assert (this.db_abstraction.UserTableNames.Length == 0);
			
			//	Il faut créer les tables internes utilisées pour la gestion des méta-données.
			
			this.CreateTableTableDef ();
			this.CreateTableColumnDef ();
			this.CreateTableTypeDef ();
			this.CreateTableEnumValDef ();
			this.CreateTableRefDef ();
		}
		
		public void AttachDatabase(DbAccess db_access)
		{
			if (this.db_access.IsValid)
			{
				throw new DbException (this.db_access, "Database already attached");
			}
			
			this.db_access = db_access;
			this.db_access.Create = false;
			
			this.SetupDatabaseAbstraction ();
			
			System.Diagnostics.Debug.Assert (this.db_abstraction.UserTableNames.Length > 0);
		}
		
		
		public void ExecuteSilent()
		{
			using (System.Data.IDbTransaction transaction = this.db_abstraction.BeginTransaction ())
			{
				using (System.Data.IDbCommand command = this.sql_builder.Command)
				{
					command.Transaction = transaction;
					
					try
					{
						this.sql_engine.Execute (command, DbCommandType.Silent);
					}
					catch
					{
						System.Diagnostics.Debug.WriteLine ("DbInfrastructure.ExecuteSilent: Roll back transaction.");
						
						transaction.Rollback ();
						throw;
					}
				}
				
				transaction.Commit ();
			}
		}
		
		
		#region Internal Management Table Creation
		protected virtual void CreateTableTableDef()
		{
			SqlTable    table   = new SqlTable (DbTable.TagTableDef);
			SqlColumn[] columns = new SqlColumn[7];
			
			columns[0] = new SqlColumn (DbColumn.TagId,				DbRawType.Int32);
			columns[1] = new SqlColumn (DbColumn.TagRevision,		DbRawType.Int32);
			columns[2] = new SqlColumn (DbColumn.TagStatus,			DbRawType.Int32);
			columns[3] = new SqlColumn (DbColumn.TagName,			DbRawType.String, DbColumn.MaxNameLength,			false, Nullable.No);
			columns[4] = new SqlColumn (DbColumn.TagCaption,		DbRawType.String, DbColumn.MaxCaptionLength,		false, Nullable.Yes);
			columns[5] = new SqlColumn (DbColumn.TagDescription,	DbRawType.String, DbColumn.MaxDescriptionLength,	false, Nullable.Yes);
			columns[6] = new SqlColumn (DbColumn.TagInformation,	DbRawType.String, DbColumn.MaxInformationLength,	false, Nullable.No);
			
			table.Columns.AddRange (columns);
			table.PrimaryKey = new SqlColumn[] { columns[0], columns[1] };
			
			this.sql_builder.InsertTable (table);
			this.ExecuteSilent ();
		}
		
		protected virtual void CreateTableColumnDef()
		{
		}
		
		protected virtual void CreateTableTypeDef()
		{
		}
		
		protected virtual void CreateTableEnumValDef()
		{
		}
		
		protected virtual void CreateTableRefDef()
		{
		}
		#endregion
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.db_abstraction != null)
				{
					this.db_abstraction.Dispose ();
					this.db_abstraction = null;
					this.sql_builder = null;
					this.sql_engine = null;
				}
				
				System.Diagnostics.Debug.Assert (this.sql_builder == null);
				System.Diagnostics.Debug.Assert (this.sql_engine == null);
			}
		}
		
		protected virtual void SetupDatabaseAbstraction()
		{
			this.db_abstraction = DbFactory.FindDbAbstraction (this.db_access);
			
			this.sql_builder = this.db_abstraction.SqlBuilder;
			this.sql_engine  = this.db_abstraction.SqlEngine;
			
			System.Diagnostics.Debug.Assert (this.sql_builder != null);
			System.Diagnostics.Debug.Assert (this.sql_engine != null);
			
			this.sql_builder.AutoClear = true;
		}
		
		
		protected DbAccess				db_access;
		protected IDbAbstraction		db_abstraction;
		
		protected ISqlBuilder			sql_builder;
		protected ISqlEngine			sql_engine;
	}
}
