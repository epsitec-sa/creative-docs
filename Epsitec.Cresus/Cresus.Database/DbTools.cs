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
	public static class DbTools
	{
		public static void WriteAttribute(System.Xml.XmlTextWriter xmlWriter, string name, string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				//	Nothing to do; an empty attribute will never be written to
				//	the XML output.
			}
			else
			{
				xmlWriter.WriteAttributeString (name, value);
			}
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
			else
			{
				return InvariantConverter.ToString ((int) cat);
			}
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

		public static string RawTypeToString(DbRawType dbRawType)
		{
			if (dbRawType == DbRawType.Unsupported)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString ((int) dbRawType);
			}
		}
		
		public static string SimpleTypeToString(DbSimpleType dbSimpleType)
		{
			if (dbSimpleType == DbSimpleType.Unsupported)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString ((int) dbSimpleType);
			}
		}


		public static DbRawType ParseRawType(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return DbRawType.Unsupported;
			}

			int num;
			InvariantConverter.Convert (value, out num);
			return (DbRawType) num;
		}

		public static DbSimpleType ParseSimpleType(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return DbSimpleType.Unsupported;
			}

			int num;
			InvariantConverter.Convert (value, out num);
			return (DbSimpleType) num;
		}

		public static string ParseString(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}
			else
			{
				return value;
			}
		}

		public static int ParseInt(string value)
		{
			return InvariantConverter.ParseInt (value);
		}

		public static Druid ParseDruid(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return Druid.Empty;
			}
			else
			{
				return Druid.Parse (value);
			}
		}

		public static string IntToString(int value)
		{
			if (value == 0)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString (value);
			}
		}

		public static string BoolToString(bool value)
		{
			return value ? "Y" : null;
		}

		public static bool ParseBool(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return false;
			}
			else
			{
				System.Diagnostics.Debug.Assert (value == "Y");
				
				return true;
			}
		}

		public static string StringToString(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}
			else
			{
				return value;
			}
		}
	}
}
