//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

#if false // cf. ResourcesTest.cs, ligne 12

using System.Globalization;
using System.Text.RegularExpressions;

using Epsitec.Cresus.Database;

namespace Epsitec.Common.Support.Implementation
{
	/// <summary>
	/// La classe BaseProvider donne accès aux ressources stockées dans une base
	/// de données.
	/// </summary>
	public class BaseProvider : AbstractResourceProvider
	{
		public BaseProvider()
		{
		}
		
		
		public override string				Prefix
		{
			get
			{
				return "base";
			}
		}
		
		
		public static void CreateResourceDatabase(string applicationName)
		{
			//	Crée une base de données pour stocker les ressources de l'application spécifiée.
			//	Cette opération échoue si la base existe déjà et génère une exception.
			
			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				infrastructure.CreateDatabase (BaseProvider.GetDbAccess (applicationName));
				
				using (DbTransaction transaction = infrastructure.BeginTransaction ())
				{
					BaseProvider.SetupInitialBase (infrastructure, transaction);
					transaction.Commit ();
					return;
				}
			}
		}
		
		
		protected string GetRowNameFromId(string id, ResourceLevel level)
		{
			if (this.ValidateId (id))
			{
				return id;
			}
			
			throw new ResourceException (string.Format ("Invalid resource id '{0}'.", id));
		}
		
		protected string GetRowLevelFromId(string id, ResourceLevel level)
		{
			switch (level)
			{
				case ResourceLevel.Default:		break;
				case ResourceLevel.Localized:	break;
				case ResourceLevel.Customized:	break;
				
				default:
					throw new ResourceException (string.Format ("Invalid resource level {0} for resource '{1}'.", level, id));
			}
			
			return Resources.MapToSuffix (level, this.culture);
		}
		
		protected string GetTableNameFromId(string id, ResourceLevel level)
		{
			switch (level)
			{
				case ResourceLevel.Default:		break;
				case ResourceLevel.Localized:	break;
				case ResourceLevel.Customized:	break;
				
				default:
					throw new ResourceException (string.Format ("Invalid resource level {0} for resource '{1}'.", level, id));
			}
			
			return BaseProvider.DataTableName;
		}
		
		
		protected static DbAccess GetDbAccess(string application)
		{
			string baseName = application + "_resdb";
			DbAccess  access = DbInfrastructure.CreateDbAccess (baseName);
			
			access.Provider = "Firebird";
			
			return access;
		}
		
		
		public override bool SetupApplication(string application)
		{
			//	Le nom de l'application est utile pour déterminer le nom de la
			//	base de données à laquelle on va se connecter.
			
			try
			{
				this.dbi = new DbInfrastructure ();
				this.dbi.AttachDatabase (BaseProvider.GetDbAccess (application));
			}
			catch
			{
				this.dbi.Dispose ();
				this.dbi = null;
				return false;
			}
			
			this.dataTable = this.dbi.ResolveDbTable (BaseProvider.DataTableName);
			
			if (this.dataTable == null)
			{
				this.dbi.Dispose ();
				this.dbi = null;
				return false;
			}
			
			this.dataColumn_index = this.data_table.Columns["Data"].TableColumnIndex;
			
			return true;
		}
		
		public override void SelectLocale(System.Globalization.CultureInfo culture)
		{
			base.SelectLocale (culture);
		}
		
		
		
		public override bool ValidateId(string id)
		{
			//	Autorise en principe tous les caractères, car c'est un nom qui ne
			//	sera utilisé que pour une recherche paramétrisée où il n'y a pas
			//	besoin de faire attention aux caractères spéciaux.
			
			return base.ValidateId (id);
		}
		
		public override bool Contains(string id)
		{
			if (this.ValidateId (id))
			{
				//	On valide toujours le nom avant, pour éviter des mauvaises surprises si
				//	l'appelant est malicieux.
				
				string rowName   = this.GetRowNameFromId (id, ResourceLevel.Default);
				string levelName = this.GetRowLevelFromId (id, ResourceLevel.Default);
				
				DbSelectCondition condition = this.CreateSelectCondition (rowName, levelName);
				DbRichCommand     command   = DbRichCommand.CreateFromTable (this.dbi, null, this.dataTable, condition);
				
				if (command.DataSet.Tables[0].Rows.Count > 0)
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		public override byte[] GetData(string id, Epsitec.Common.Support.ResourceLevel level, System.Globalization.CultureInfo culture)
		{
			if (this.culture != culture)
			{
				this.SelectLocale (culture);
			}
			
			string rowName   = this.GetRowNameFromId (id, level);
			string levelName = this.GetRowLevelFromId (id, level);
			
			DbSelectCondition condition = this.CreateSelectCondition (rowName, levelName);
			DbRichCommand     command   = DbRichCommand.CreateFromTable (this.dbi, null, this.dataTable, condition);
			
			this.dbi.ReleaseConnection ();

			if (command.DataSet.Tables[0].Rows.Count > 0)
			{
				byte[] data = (byte[]) command.DataSet.Tables[0].Rows[0][this.dataColumn_index];
				return data;
			}
			
			return null;
		}
		
		
		public override string[] GetIds(string nameFilter, string type_filter, ResourceLevel level, System.Globalization.CultureInfo culture)
		{
			if (this.culture != culture)
			{
				this.SelectLocale (culture);
			}
			
			//	TODO: retourne la liste de tous les <id> de ressources connus.
			
			return null;
		}

		
		public override bool SetData(string id, Epsitec.Common.Support.ResourceLevel level, System.Globalization.CultureInfo culture, byte[] data, ResourceSetMode mode)
		{
			if (this.culture != culture)
			{
				this.SelectLocale (culture);
			}
			
			string rowName   = this.GetRowNameFromId (id, level);
			string levelName = this.GetRowLevelFromId (id, level);
			
			DbSelectCondition condition = this.CreateSelectCondition (rowName, levelName);
			
			using (DbTransaction transaction = this.dbi.BeginTransaction ())
			{
				DbRichCommand command = DbRichCommand.CreateFromTable (this.dbi, transaction, this.dataTable, condition);
				
				switch (mode)
				{
					case ResourceSetMode.CreateOnly:
						if (command.DataSet.Tables[0].Rows.Count > 0)
						{
							throw new ResourceException (string.Format ("Resource {0} (level {1}) already exists.", id, level));
						}
						this.AddNewDataRow (command, transaction, rowName, levelName, data);
						break;
					
					case ResourceSetMode.UpdateOnly:
						if (command.DataSet.Tables[0].Rows.Count != 1)
						{
							throw new ResourceException (string.Format ("Resource {0} (level {1}) does not exist.", id, level));
						}
						this.UpdateDataRow (command, transaction, rowName, levelName, data);
						break;
					
					case ResourceSetMode.Write:
						if (command.DataSet.Tables[0].Rows.Count == 0)
						{
							this.AddNewDataRow (command, transaction, rowName, levelName, data);
						}
						else
						{
							this.UpdateDataRow (command, transaction, rowName, levelName, data);
						}
						break;
					
					default:
						throw new System.ArgumentException (string.Format ("Mode {0} not supported.", mode), "mode");
				}
				
				command.UpdateLogIds ();
				command.UpdateRealIds (transaction);
				command.UpdateTables (transaction);
				transaction.Commit ();
				
				this.dbi.ReleaseConnection ();
			}
			
			return true;
		}
		
		public override bool Remove(string id, Epsitec.Common.Support.ResourceLevel level, System.Globalization.CultureInfo culture)
		{
			DbSelectCondition condition;
			
			if (level == ResourceLevel.All)
			{
				condition = this.CreateSelectAllCondition (id);
			}
			else
			{
				if (this.culture != culture)
				{
					this.SelectLocale (culture);
				}
				
				string rowName   = this.GetRowNameFromId (id, level);
				string levelName = this.GetRowLevelFromId (id, level);
				
				condition = this.CreateSelectCondition (rowName, levelName);
			}
			
			using (DbTransaction transaction = this.dbi.BeginTransaction ())
			{
				DbRichCommand command = DbRichCommand.CreateFromTable (this.dbi, transaction, this.dataTable, condition);
				int           changes = 0;
				
				foreach (System.Data.DataRow row in command.DataSet.Tables[0].Rows)
				{
					command.DeleteExistingRow (row);
					changes++;
				}
				
				if (changes > 0)
				{
					command.UpdateLogIds ();
					command.UpdateTables (transaction);
					transaction.Commit ();
				}
				else
				{
					transaction.Rollback ();
				}
				
				this.dbi.ReleaseConnection ();
			}
			
			return true;
		}
		
		
		protected DbSelectCondition CreateSelectCondition(string name, string level)
		{
			DbSelectCondition condition = new DbSelectCondition (this.dbi.TypeConverter);
				
			//	Nous ne sommes intéressés qu'aux lignes de la table qui sont "live" et
			//	dont la révision est 0 (= version actuelle).
			
			condition.Revision = DbSelectRevision.LiveActive;
			
			condition.AddCondition (this.dataTable.Columns["Name"], DbCompare.Equal, name);
			condition.AddCondition (this.dataTable.Columns["Level"], DbCompare.Equal, level);
			
			return condition;
		}
		
		protected DbSelectCondition CreateSelectAllCondition(string name)
		{
			DbSelectCondition condition = new DbSelectCondition (this.dbi.TypeConverter);
				
			//	Nous ne sommes intéressés qu'aux lignes de la table qui sont "live" et
			//	dont la révision est 0 (= version actuelle).
			
			condition.Revision = DbSelectRevision.LiveActive;
			
			condition.AddCondition (this.dataTable.Columns["Name"], DbCompare.Equal, name);
			
			return condition;
		}
		
		
		protected void AddNewDataRow(DbRichCommand command, DbTransaction transaction, string name, string level, byte[] data)
		{
			System.Data.DataRow row;

			command.CreateNewRow (BaseProvider.DataTableName, out row);
			
			row.BeginEdit ();
			row["Name"]  = name;
			row["Level"] = level;
			row["Data"]  = data;
			row.EndEdit ();
		}
		
		protected void UpdateDataRow(DbRichCommand command, DbTransaction transaction, string name, string level, byte[] data)
		{
			System.Data.DataRow row = command.DataSet.Tables[0].Rows[0];
			
			System.Diagnostics.Debug.Assert ((string)row["Name"] == name);
			System.Diagnostics.Debug.Assert ((string)row["Level"] == level);
			
			row.BeginEdit ();
			row["Data"]  = data;
			row.EndEdit ();
		}
		
		
		protected static void SetupInitialBase(DbInfrastructure infrastructure, DbTransaction transaction)
		{
			//	La base de données a été créée et elle est parfaitement vide. Il faut maintenant
			//	définir les types et les tables de base :
			
			DbTable dbTable = infrastructure.CreateDbTable (BaseProvider.DataTableName, DbElementCat.UserDataManaged, BaseProvider.UseRevisions);
			
			DbType dbTypeName  = infrastructure.CreateDbType ("Name", 80, false) as DbTypeString;
			DbType dbTypeLevel = infrastructure.CreateDbType ("Level", 4, false) as DbTypeString;
			DbType dbTypeType  = infrastructure.CreateDbType ("Type", 25, false) as DbTypeString;
			DbType dbTypeData  = infrastructure.CreateDbTypeByteArray ("Data");
			
			infrastructure.RegisterNewDbType (transaction, dbTypeName);
			infrastructure.RegisterNewDbType (transaction, dbTypeLevel);
			infrastructure.RegisterNewDbType (transaction, dbTypeType);
			infrastructure.RegisterNewDbType (transaction, dbTypeData);
			
			DbColumn col1 = DbColumn.CreateUserDataColumn ("Name",  dbTypeName,  Nullable.No);
			DbColumn col2 = DbColumn.CreateUserDataColumn ("Level", dbTypeLevel, Nullable.No);
			DbColumn col3 = DbColumn.CreateUserDataColumn ("Type",  dbTypeType,  Nullable.Yes);
			DbColumn col4 = DbColumn.CreateUserDataColumn ("Data",  dbTypeData,  Nullable.Yes);
			
			dbTable.Columns.AddRange (new DbColumn[] { col1, col2, col3, col4 });
			
			infrastructure.RegisterNewDbTable (transaction, dbTable);
		}
		
		private const string				DataTableName = "Data";
		private const DbRevisionMode		UseRevisions  = DbRevisionMode.Disabled;
		
		protected DbInfrastructure			dbi;
		protected DbTable					dataTable;
		protected int						dataColumnIndex;
		
		protected string					columnDefault;
		protected string					columnLocal;
		protected string					columnCustom;
	}
}
#endif
