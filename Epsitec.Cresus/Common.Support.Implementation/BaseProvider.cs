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
			
			return "Data";
		}
		
		
		protected static DbAccess GetDbAccess(string application)
		{
			string base_name = application + "_resdb";
			return DbInfrastructure.CreateDbAccess (base_name);
		}
		
		
		public override string			Prefix
		{
			get { return "base"; }
		}
		
		
		public override void Setup(string application)
		{
			//	Le nom de l'application est utile pour déterminer le nom de la
			//	base de données à laquelle on va se connecter.
			
			this.dbi = new DbInfrastructure ();
			this.dbi.AttachDatabase (BaseProvider.GetDbAccess (application));
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
				
				//	TODO: vérifie si la ressource existe, en cherchant uniquement le niveau
				//	ResourceLevel.Default.
			}
			
			return false;
		}
		
		
		public override byte[] GetData(string id, Epsitec.Common.Support.ResourceLevel level, System.Globalization.CultureInfo culture)
		{
			if (this.culture != culture)
			{
				this.SelectLocale (culture);
			}
			
			string row_name = this.GetRowNameFromId (id, level);
			
			//	TODO: implémenter GetData.
			
			throw new System.NotImplementedException ("GetData not implemented.");
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
			// TODO:  Add FileProvider.Update implementation
			throw new ResourceException ("Not implemented");
		}
		
		public override bool Remove(string id, Epsitec.Common.Support.ResourceLevel level, System.Globalization.CultureInfo culture)
		{
			// TODO:  Add FileProvider.Remove implementation
			throw new ResourceException ("Not implemented");
		}
		
		
		protected static void SetupInitialBase(DbInfrastructure infrastructure, DbTransaction transaction)
		{
			DbTable db_table = infrastructure.CreateDbTable ("Data", DbElementCat.UserDataManaged);
			
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
		
		
		protected DbInfrastructure		dbi;
		
		protected string				column_default;
		protected string				column_local;
		protected string				column_custom;
	}
}
