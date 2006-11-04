//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Database
{
	using InvariantConverter = Epsitec.Common.Types.InvariantConverter;
	
	/// <summary>
	/// La classe DbTools fournit quelques fonctions utilitaires qui n'ont pas
	/// de rapport direct avec SQL, mais sont utiles aux classes DbXyz.
	/// </summary>
	public class DbTools
	{
		private DbTools()
		{
		}
		
		
		public static string BuildLocalisedName(string name, Common.Support.ResourceLevel level)
		{
			//	TODO: gérer les suffixes en fonction de la culture active, non en fonction de
			//	la culture par défaut du gestionnaire de ressources par défaut !
			
			System.Globalization.CultureInfo culture = Resources.DefaultManager.ActiveCulture;
			ResourceManager                  manager = Resources.DefaultManager;
			
			switch (level)
			{
				case ResourceLevel.Default:
					return name;
				
				case ResourceLevel.Customized:
				case ResourceLevel.Localized:
					return DbTools.BuildCompositeName (name, manager.MapToSuffix (level, culture));
			}
			
			throw new System.ArgumentException (string.Format ("Level {0} not valid here (name='{1}').", level, name), "level");
		}
		
		public static string BuildCompositeName(params string[] list)
		{
			int num = list.Length;
			
			//	Retire de la liste toutes les chaînes vides qui pourraient s'être
			//	accumulées à la fin; ça évite que la composition de "a" avec "" ne
			//	donne "a_" en fin de compte (le résultat sera "a", simplement).
			
			while ((num > 0) && (list[num-1].Length == 0))
			{
				num--;
			}
			
			return (num == 0) ? "" : string.Join ("_", list, 0, num);
		}
		
		
		public static DbElementCat   ParseElementCategory(string text)
		{
			if ((text == null) ||
				(text.Length == 0))
			{
				return DbElementCat.Unknown;
			}
			
			int cat;
			InvariantConverter.Convert (text, out cat);
			return (DbElementCat) cat;
		}
		
		public static DbRevisionMode ParseRevisionMode(string text)
		{
			if ((text == null) ||
				(text.Length == 0))
			{
				return DbRevisionMode.Unknown;
			}
			
			int mode;
			InvariantConverter.Convert (text, out mode);
			return (DbRevisionMode) mode;
		}
		
		public static DbReplicationMode ParseReplicationMode(string text)
		{
			if ((text == null) ||
				(text.Length == 0))
			{
				return DbReplicationMode.Unknown;
			}
			
			int mode;
			InvariantConverter.Convert (text, out mode);
			return (DbReplicationMode) mode;
		}
		
		
		public static string ElementCategoryToString(DbElementCat cat)
		{
			if (cat == DbElementCat.Unknown)
			{
				return null;
			}
			
			return InvariantConverter.ToString ((int) cat);
		}
		
		public static string RevisionModeToString(DbRevisionMode mode)
		{
			if (mode == DbRevisionMode.Unknown)
			{
				return null;
			}
			
			return InvariantConverter.ToString ((int) mode);
		}
		
		public static string ReplicationModeToString(DbReplicationMode mode)
		{
			if (mode == DbReplicationMode.Unknown)
			{
				return null;
			}
			
			return InvariantConverter.ToString ((int) mode);
		}

		public static string ColumnClassToString(DbColumnClass value)
		{
			if (value == DbColumnClass.Data)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString ((int) value);
			}
		}

		public static string ColumnLocalisationToString(DbColumnLocalisation value)
		{
			if (value == DbColumnLocalisation.None)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString ((int) value);
			}
		}

		public static string DruidToString(Druid value)
		{
			if (value.IsEmpty)
			{
				return null;
			}
			else
			{
				return value.ToString ();
			}
		}

		public static string TypeToString(INamedType value)
		{
			if (value == null)
			{
				return null;
			}
			else
			{
				return value.CaptionId.ToString ();
			}
		}
	}
}
