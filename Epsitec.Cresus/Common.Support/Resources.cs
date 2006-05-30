//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe Resources permet de gérer les ressources de l'application.
	/// </summary>
	public sealed class Resources
	{
		private Resources()
		{
		}
		
		static Resources()
		{
			string[] names = { "fr", "de", "it", "en" };
			
			Resources.InternalDefineCultures (names);
			Resources.InternalInitialise ();
		}
		
		
		public static int						ProviderCount
		{
			get
			{
				return Resources.manager.ProviderCount;
			}
		}
		
		public static string[]					ProviderPrefixes
		{
			get
			{
				return Resources.manager.ProviderPrefixes;
			}
		}
		
		public static string					ActivePrefix
		{
			get
			{
				return Resources.manager.ActivePrefix;
			}
			set
			{
				Resources.manager.ActivePrefix = value;
			}
		}
		
		public static CultureInfo				ActiveCulture
		{
			get
			{
				return Resources.manager.ActiveCulture;
			}
			set
			{
				Resources.manager.ActiveCulture = value;
			}
		}
		
		public static ResourceManager			DefaultManager
		{
			get
			{
				return Resources.manager;
			}
		}
		
		
		public static CultureInfo[]				Cultures
		{
			get
			{
				return Resources.cultures;
			}
		}
		
		
		public static string					DefaultSuffix
		{
			get
			{
				return Resources.manager.DefaultSuffix;
			}
		}
		
		public static string					LocalisedSuffix
		{
			get
			{
				return Resources.manager.LocalisedSuffix;
			}
		}
		
		public static string					CustomisedSuffix
		{
			get
			{
				return Resources.manager.CustomisedSuffix;
			}
		}
		
		
		public static void DefineDefaultModuleName(string application_name)
		{
			Resources.manager.DefineDefaultModuleName (application_name);
		}
		
		
		public static bool ValidateId(string id)
		{
			return Resources.manager.ValidateId (id);
		}
		
		public static bool ContainsId(string id)
		{
			return Resources.manager.ContainsId (id);
		}
		
		
		public static string ExtractPrefix(string full_id)
		{
			if (full_id != null)
			{
				int pos = full_id.IndexOf (":");
			
				if (pos > 0)
				{
					return full_id.Substring (0, pos);
				}
			}
			
			return null;
		}
		
		public static string ExtractSuffix(string full_id)
		{
			//	L'extraction d'un suffixe n'a de sens que si l'on utilise GetBundleIds
			//	avec level = ResourceLevel.All; sinon, en principe, on n'a jamais besoin
			//	de se soucier des suffixes.
			
			if (full_id != null)
			{
				int pos = full_id.LastIndexOf (".") + 1;
				int len = full_id.Length;
				
				if (pos > 0)
				{
					if ((pos + 2 == len) ||
						(pos + 3 == len))
					{
						return full_id.Substring (pos);
					}
				}
			}
			
			return null;
		}
		
		public static string StripSuffix(string full_id)
		{
			string suffix = Resources.ExtractSuffix (full_id);
			
			if (suffix != null)
			{
				return full_id.Substring (0, full_id.Length - suffix.Length - 1);
			}
			
			return full_id;
		}
		
		public static string ExtractName(string full_id)
		{
			if (full_id != null)
			{
				int pos = full_id.IndexOf (":");
			
				if (pos > 0)
				{
					return full_id.Substring (pos+1);
				}
				else
				{
					return full_id;
				}
			}
			
			return null;
		}
		
		public static string MakeFullName(string prefix, string name)
		{
			return Resources.manager.MakeFullName(prefix, name);
		}
		
		
		public static string MapToSuffix(ResourceLevel level, CultureInfo culture)
		{
			return Resources.manager.MapToSuffix (level, culture);
		}
		
		public static void MapFromSuffix(string suffix, out ResourceLevel level, out CultureInfo culture)
		{
			Resources.manager.MapFromSuffix (suffix, out level, out culture);
		}
		
		
		public static string GetLevelCaption(ResourceLevel level, CultureInfo culture)
		{
			return Resources.manager.GetLevelCaption (level, culture);
		}

		public static IEnumerable<string> GetModuleNames(string prefix)
		{
			return Resources.manager.GetModuleNames (prefix);
		}

		public static IEnumerable<ResourceModuleInfo> GetModuleInfos(string prefix)
		{
			return Resources.manager.GetModuleInfos (prefix);
		}

		public static void RefreshModuleInfos(string prefix)
		{
			Resources.manager.RefreshModuleInfos (prefix);
		}

		public static string[] GetBundleIds(string name_filter)
		{
			return Resources.manager.GetBundleIds (name_filter);
		}
		
		public static string[] GetBundleIds(string name_filter, string type_filter)
		{
			return Resources.manager.GetBundleIds (name_filter, type_filter);
		}
		
		public static string[] GetBundleIds(string name_filter, ResourceLevel level)
		{
			return Resources.manager.GetBundleIds (name_filter, level);
		}
		
		public static string[] GetBundleIds(string name_filter, string type_filter, ResourceLevel level)
		{
			return Resources.manager.GetBundleIds (name_filter, type_filter, level);
		}
		
		public static string[] GetBundleIds(string name_filter, ResourceLevel level, CultureInfo culture)
		{
			return Resources.manager.GetBundleIds (name_filter, level, culture);
		}
		
		public static string[] GetBundleIds(string name_filter, string type_filter, ResourceLevel level, CultureInfo culture)
		{
			return Resources.manager.GetBundleIds (name_filter, type_filter, level, culture);
		}
		
		
		public static ResourceBundle GetBundle(string id)
		{
			return Resources.manager.GetBundle (id);
		}
		
		public static ResourceBundle GetBundle(string id, ResourceLevel level)
		{
			return Resources.manager.GetBundle (id, level);
		}
		
		public static ResourceBundle GetBundle(string id, ResourceLevel level, int recursion)
		{
			return Resources.manager.GetBundle (id, level, recursion);
		}
		
		public static ResourceBundle GetBundle(string id, ResourceLevel level, CultureInfo culture)
		{
			return Resources.manager.GetBundle (id, level, culture);
		}
		
		public static ResourceBundle GetBundle(string id, ResourceLevel level, CultureInfo culture, int recursion)
		{
			return Resources.manager.GetBundle (id, level, culture, recursion);
		}
		
		
		public static byte[] GetBinaryData(string id)
		{
			return Resources.manager.GetBinaryData (id);
		}
		
		public static byte[] GetBinaryData(string id, ResourceLevel level)
		{
			return Resources.manager.GetBinaryData (id, level);
		}
		
		public static byte[] GetBinaryData(string id, ResourceLevel level, CultureInfo culture)
		{
			return Resources.manager.GetBinaryData (id, level, culture);
		}
		
		
		public static bool IsTextRef(string text)
		{
			if ((text != null) &&
				(text.StartsWith ("[res:")) &&
				(text.EndsWith ("]")))
			{
				return true;
			}
			
			return false;
		}
		
		public static string MakeTextRef(string id)
		{
			if ((id == null) ||
				(id.Length == 0))
			{
				return id;
			}
			
			return string.Concat ("[res:", id, "]");
		}
		
		public static string ExtractTextRefTarget(string text)
		{
			if (Resources.IsTextRef (text))
			{
				return text.Substring (5, text.Length-6);
			}
			
			return null;
		}
		
		public static string ResolveTextRef(string text)
		{
			return Resources.manager.ResolveTextRef (text);
		}
		
		
		public static string GetText(string id)
		{
			return Resources.manager.GetText (id);
		}
		
		public static string GetText(string id, ResourceLevel level)
		{
			return Resources.manager.GetText (id, level);
		}
		
		
		public static void SetBundle(ResourceBundle bundle, ResourceSetMode mode)
		{
			Resources.manager.SetBundle (bundle, mode);
		}
		
		public static bool SetBinaryData(string id, ResourceLevel level, CultureInfo culture, byte[] data, ResourceSetMode mode)
		{
			return Resources.manager.SetBinaryData (id, level, culture, data, mode);
		}
		
		
		public static bool RemoveBundle(string id, ResourceLevel level, CultureInfo culture)
		{
			return Resources.manager.RemoveBundle (id, level, culture);
		}
		
		
		public static void DebugDumpProviders()
		{
			Resources.manager.DebugDumpProviders ();
		}
		
		
		public static CultureInfo FindCultureInfo(string two_letter_code)
		{
			CultureInfo[] cultures = CultureInfo.GetCultures (System.Globalization.CultureTypes.NeutralCultures);
			
			for (int i = 0; i < cultures.Length; i++)
			{
				if (cultures[i].TwoLetterISOLanguageName == two_letter_code)
				{
					return cultures[i];
				}
			}
			
			return null;
		}
		
		public static CultureInfo FindSpecificCultureInfo(string two_letter_code)
		{
			//	FindSpecificCultureInfo retourne une culture propre à un pays, avec
			//	une préférence pour la Suisse ou les USA.
			
			CultureInfo[] cultures = CultureInfo.GetCultures (System.Globalization.CultureTypes.SpecificCultures);
			
			CultureInfo found = null;
			
			for (int i = 0; i < cultures.Length; i++)
			{
				CultureInfo item = cultures[i];
				string      name = item.Name;
				
				if ((item.TwoLetterISOLanguageName == two_letter_code) &&
					(name.Length == 5) &&
					(name[2] == '-'))
				{
					if (((name[3] == 'C') && (name[4] == 'H')) ||
						((name[3] == 'U') && (name[4] == 'S')))
					{
						return item;
					}
					
					if (found == null)
					{
						found = item;
					}
				}
			}
			
			return found;
		}

		
		public static bool EqualCultures(ResourceLevel level_a, CultureInfo culture_a, ResourceLevel level_b, CultureInfo culture_b)
		{
			if (level_a != level_b)
			{
				return false;
			}
			if (level_a == ResourceLevel.Default)
			{
				return true;
			}
			
			return Resources.EqualCultures (culture_a, culture_b);
		}
		
		public static bool EqualCultures(CultureInfo culture_a, CultureInfo culture_b)
		{
			if (culture_a == culture_b)
			{
				return true;
			}
			
			if ((culture_a == null) ||
				(culture_b == null))
			{
				return false;
			}
			
			return culture_a.TwoLetterISOLanguageName == culture_b.TwoLetterISOLanguageName;
		}
		
		
		private static void InternalInitialise()
		{
			Resources.manager = new ResourceManager (typeof (ResourceManager));
		}
		
		private static void InternalDefineCultures(string[] names)
		{
			int n = names.Length;
			
			Resources.cultures = new CultureInfo[n];
			
			for (int i = 0; i < n; i++)
			{
				Resources.cultures[i] = Resources.FindCultureInfo (names[i]);
			}
		}
		
		
		
		private static CultureInfo[]			cultures;
		private static ResourceManager			manager;
	}
}
