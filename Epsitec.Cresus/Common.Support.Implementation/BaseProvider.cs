//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		
		
		public static void CreateResourceDatabase(string application_name)
		{
			//	Crée une base de données pour stocker les ressources de l'application spécifiée.
			//	Cette opération échoue si la base existe déjà et génère une exception.
			
			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				infrastructure.CreateDatabase (BaseProvider.GetDbAccess (application_name));
				
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
				case ResourceLevel.Localised:	break;
				case ResourceLevel.Customised:	break;
				
				default:
					throw new ResourceException (string.Format ("Invalid resource level {0} for resource '{1}'.", level, id));
			}
			
			string suffix;
			Resources.MapToSuffix (level, this.culture, out suffix);
			
			return suffix;
		}
		
		protected string GetTableNameFromId(string id, ResourceLevel level)
		{
			switch (level)
			{
				case ResourceLevel.Default:		break;
				case ResourceLevel.Localised:	break;
				case ResourceLevel.Customised:	break;
				
				default:
					throw new ResourceException (string.Format ("Invalid resource level {0} for resource '{1}'.", level, id));
			}
			
			return BaseProvider.DataTableName;
		}
		
		
		protected static DbAccess GetDbAccess(string application)
		{
			string base_name = application + "_resdb";
			return DbInfrastructure.CreateDbAccess (base_name);
		}
		
		
		public override void Setup(string application)
		{
			//	Le nom de l'application est utile pour déterminer le nom de la
			//	base de données à laquelle on va se connecter.
			
			this.dbi = new DbInfrastructure ();
			this.dbi.AttachDatabase (BaseProvider.GetDbAccess (application));
			this.data_table = this.dbi.ResolveDbTable (null, BaseProvider.DataTableName);
			
			if (this.data_table == null)
			{
				throw new ResourceException ("Cannot find data table.");
			}
			
			this.data_column_index = this.data_table.Columns["Data"].TableColumnIndex;
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
				
				string row_name   = this.GetRowNameFromId (id, ResourceLevel.Default);
				string level_name = this.GetRowLevelFromId (id, ResourceLevel.Default);
				
				DbSelectCondition condition = new DbSelectCondition (this.dbi.TypeConverter);
				
				//	TODO: ajouter une condition sur CR_REV/CR_STAT
				
				condition.AddCondition (this.data_table.Columns["Name"], DbCompare.Equal, row_name);
				condition.AddCondition (this.data_table.Columns["Level"], DbCompare.Equal, level_name);
				
				DbRichCommand command = DbRichCommand.CreateFromTable (this.dbi, null, this.data_table, condition);
				
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
			
			string row_name   = this.GetRowNameFromId (id, level);
			string level_name = this.GetRowLevelFromId (id, level);
			
			DbSelectCondition condition = new DbSelectCondition (this.dbi.TypeConverter);
			
			//	TODO: ajouter une condition sur CR_REV/CR_STAT
			
			condition.AddCondition (this.data_table.Columns["Name"], DbCompare.Equal, row_name);
			condition.AddCondition (this.data_table.Columns["Level"], DbCompare.Equal, level_name);
			
			DbRichCommand command = DbRichCommand.CreateFromTable (this.dbi, null, this.data_table, condition);
			
			if (command.DataSet.Tables[0].Rows.Count > 0)
			{
				byte[] data = (byte[]) command.DataSet.Tables[0].Rows[0][this.data_column_index];
				return data;
			}
			
			return null;
		}
		
		
		public override string[] GetIds(string name_filter, string type_filter, ResourceLevel level, System.Globalization.CultureInfo culture)
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
			
			string row_name   = this.GetRowNameFromId (id, level);
			string level_name = this.GetRowLevelFromId (id, level);
			
			DbSelectCondition condition = new DbSelectCondition (this.dbi.TypeConverter);
			
			//	TODO: ajouter une condition sur CR_REV/CR_STAT
			
			condition.AddCondition (this.data_table.Columns["Name"], DbCompare.Equal, row_name);
			condition.AddCondition (this.data_table.Columns["Level"], DbCompare.Equal, level_name);
			
			using (DbTransaction transaction = this.dbi.BeginTransaction ())
			{
				DbRichCommand command = DbRichCommand.CreateFromTable (this.dbi, transaction, this.data_table, condition);
				
				switch (mode)
				{
					case ResourceSetMode.CreateOnly:
						if (command.DataSet.Tables[0].Rows.Count > 0)
						{
							throw new ResourceException (string.Format ("Resource {0} (level {1}) already exists.", id, level));
						}
						this.AddNewDataRow (command, transaction, row_name, level_name, data);
						break;
					
					case ResourceSetMode.UpdateOnly:
						if (command.DataSet.Tables[0].Rows.Count != 1)
						{
							throw new ResourceException (string.Format ("Resource {0} (level {1}) does not exist.", id, level));
						}
						this.UpdateDataRow (command, transaction, row_name, level_name, data);
						break;
					
					case ResourceSetMode.Write:
						if (command.DataSet.Tables[0].Rows.Count == 0)
						{
							this.AddNewDataRow (command, transaction, row_name, level_name, data);
						}
						else
						{
							this.UpdateDataRow (command, transaction, row_name, level_name, data);
						}
						break;
					
					default:
						throw new System.ArgumentException (string.Format ("Mode {0} not supported.", mode), "mode");
				}
				
				command.UpdateRealIds (transaction);
				command.UpdateTables (transaction);
				transaction.Commit ();
			}
			
			return true;
		}
		
		public override bool Remove(string id, Epsitec.Common.Support.ResourceLevel level, System.Globalization.CultureInfo culture)
		{
			// TODO:  Add FileProvider.Remove implementation
			throw new ResourceException ("Not implemented");
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
			
			DbTable db_table = infrastructure.CreateDbTable (BaseProvider.DataTableName, DbElementCat.UserDataManaged);
			
			DbType db_type_name  = infrastructure.CreateDbType ("Name", 80, false) as DbTypeString;
			DbType db_type_level = infrastructure.CreateDbType ("Level", 4, false) as DbTypeString;
			DbType db_type_type  = infrastructure.CreateDbType ("Type", 25, false) as DbTypeString;
			DbType db_type_data  = infrastructure.CreateDbTypeByteArray ("Data");
			
			infrastructure.RegisterNewDbType (transaction, db_type_name);
			infrastructure.RegisterNewDbType (transaction, db_type_level);
			infrastructure.RegisterNewDbType (transaction, db_type_type);
			infrastructure.RegisterNewDbType (transaction, db_type_data);
			
			DbColumn col1 = DbColumn.CreateUserDataColumn ("Name",  db_type_name,  Nullable.No);
			DbColumn col2 = DbColumn.CreateUserDataColumn ("Level", db_type_level, Nullable.No);
			DbColumn col3 = DbColumn.CreateUserDataColumn ("Type",  db_type_type,  Nullable.Yes);
			DbColumn col4 = DbColumn.CreateUserDataColumn ("Data",  db_type_data,  Nullable.Yes);
			
			db_table.Columns.AddRange (new DbColumn[] { col1, col2, col3, col4 });
			
			infrastructure.RegisterNewDbTable (transaction, db_table);
		}
		
		private const string				DataTableName = "Data";
		
		protected DbInfrastructure			dbi;
		protected DbTable					data_table;
		protected int						data_column_index;
		
		protected string					column_default;
		protected string					column_local;
		protected string					column_custom;
	}
}
