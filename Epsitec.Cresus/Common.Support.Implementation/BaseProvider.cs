//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Globalization;
using System.Text.RegularExpressions;

using Epsitec.Cresus.Database;

namespace Epsitec.Common.Support.Implementation
{
	/// <summary>
	/// La classe BaseProvider donne acc�s aux ressources stock�es dans une base
	/// de donn�es.
	/// </summary>
	public class BaseProvider : AbstractResourceProvider
	{
		public BaseProvider()
		{
		}
		
		
		public static void CreateResourceDatabase(string application_name)
		{
			//	TODO: cr�e la base de donn�es au moyen de DbInfrastructure.CreateDatabase,
			//	puis remplit celle-ci avec les types et tables initiales, puis ferme la
			//	connexion et retourne. Si on essaie de faire un CreateResourceDatabase
			//	alors que la base existe d�j�, une exception sera g�n�r�e.
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
			string base_name = application + ".resdb";
			return DbInfrastructure.CreateDbAccess (base_name);
		}
		
		
		public override string			Prefix
		{
			get { return "base"; }
		}
		
		
		public override void Setup(string application)
		{
			//	Le nom de l'application est utile pour d�terminer le nom de la
			//	base de donn�es � laquelle on va se connecter.
			
			//	TODO: il faut se connecter � la base de donn�es (qui doit exister).
#if false
			this.dbi = new DbInfrastructure ();
			this.dbi.AttachDatabase (BaseProvider.GetDbAccess (application));
#endif
		}
		
		public override void SelectLocale(System.Globalization.CultureInfo culture)
		{
			base.SelectLocale (culture);
		}
		
		
		
		public override bool ValidateId(string id)
		{
			//	Autorise en principe tous les caract�res, car c'est un nom qui ne
			//	sera utilis� que pour une recherche param�tris�e o� il n'y a pas
			//	besoin de faire attention aux caract�res sp�ciaux.
			
			return base.ValidateId (id);
		}
		
		public override bool Contains(string id)
		{
			if (this.ValidateId (id))
			{
				//	On valide toujours le nom avant, pour �viter des mauvaises surprises si
				//	l'appelant est malicieux.
				
				//	TODO: v�rifie si la ressource existe, en cherchant uniquement le niveau
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
			
			//	TODO: impl�menter GetData.
			
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
		
		protected DbInfrastructure		dbi;
		
		protected string				column_default;
		protected string				column_local;
		protected string				column_custom;
	}
}
