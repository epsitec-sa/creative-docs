//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	using System.Globalization;
	using System.Collections;
	
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
			Resources.resource_providers     = new IResourceProvider[0];
			Resources.resource_provider_hash = new System.Collections.Hashtable ();
			Resources.bundle_providers       = new IBundleProvider[0];
			Resources.culture                = CultureInfo.CurrentCulture;
			
			string[] names = { "fr", "de", "it", "en" };
			
			Resources.InternalDefineCultures (names);
			Resources.InternalInitialise ();
		}
		
		
		public static CultureInfo				Culture
		{
			get
			{
				return Resources.culture;
			}
			set
			{
				if (Resources.culture != value)
				{
					Resources.SelectLocale (value);
				}
			}
		}
		
		public static int						ProviderCount
		{
			get { return Resources.resource_providers.Length; }
		}
		
		public static string[]					ProviderPrefixes
		{
			get
			{
				string[] prefixes = new string[Resources.resource_providers.Length];
				
				for (int i = 0; i < Resources.resource_providers.Length; i++)
				{
					prefixes[i] = Resources.resource_providers[i].Prefix;
				}
				
				return prefixes;
			}
		}
		
		public static string					DefaultPrefix
		{
			get
			{
				if (Resources.default_prefix == null)
				{
					return "";
				}
				
				return Resources.default_prefix;
			}
			set
			{
				if (value == "")
				{
					value = null;
				}
				
				Resources.default_prefix = value;
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
			get { return "00"; }
		}
		
		public static string					LocalisedSuffix
		{
			get { return Resources.culture.TwoLetterISOLanguageName; }
		}
		
		public static string					CustomisedSuffix
		{
			get { return string.Concat ("X", Resources.LocalisedSuffix); }
		}
		
		
		public static void SetupProviders(string application_name)
		{
			if (Resources.application_name == application_name)
			{
				return;
			}
			
			if (Resources.application_name != null)
			{
				throw new System.InvalidOperationException ("Resource Providers may not be setup more than once.");
			}
			
			Resources.application_name = application_name;
			
			for (int i = 0; i < Resources.resource_providers.Length; i++)
			{
				Resources.resource_providers[i].Setup (application_name);
			}
		}
		
		
		public static void Add(IBundleProvider bundle_provider)
		{
			ArrayList list = new ArrayList ();
			list.AddRange (Resources.bundle_providers);
			list.Add (bundle_provider);
			
			Resources.bundle_providers = new IBundleProvider[list.Count];
			list.CopyTo (Resources.bundle_providers);
		}
		
		public static void Remove(IBundleProvider bundle_provider)
		{
			ArrayList list = new ArrayList ();
			list.AddRange (Resources.bundle_providers);
			list.Remove (bundle_provider);
			
			Resources.bundle_providers = new IBundleProvider[list.Count];
			list.CopyTo (Resources.bundle_providers);
		}
		
		
		public static bool ValidateId(string id)
		{
			IResourceProvider provider = Resources.FindProvider (id, out id);
			
			if (provider != null)
			{
				return provider.ValidateId (id);
			}
			
			return false;
		}
		
		public static bool Contains(string id)
		{
			IResourceProvider provider = Resources.FindProvider (id, out id);
			
			if (provider != null)
			{
				return provider.Contains (id);
			}
			
			return false;
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
			if (name == null)
			{
				throw new ResourceException ("Cannot make full name if name is missing.");
			}
			if (prefix == null)
			{
				throw new ResourceException ("Cannot make full name if prefix is missing.");
			}
			
			return string.Concat (prefix, ":", name);
		}
		
		
		public static void MapToSuffix(ResourceLevel level, CultureInfo culture, out string suffix)
		{
			if (culture == null)
			{
				culture = Resources.Culture;
			}
			
			switch (level)
			{
				case ResourceLevel.Default:		suffix = Resources.DefaultSuffix;								return;
				case ResourceLevel.Localised:	suffix = culture.TwoLetterISOLanguageName;						return;
				case ResourceLevel.Customised:	suffix = string.Concat ("X", culture.TwoLetterISOLanguageName);	return;
			}
			
			throw new ResourceException (string.Format ("Invalid level {0} specified in MapToSuffix.", level));
		}
		
		public static void MapFromSuffix(string suffix, out ResourceLevel level, out CultureInfo culture)
		{
			int len = suffix.Length;
			
			if (len == 2)
			{
				if (suffix == "00")
				{
					level   = ResourceLevel.Default;
					culture = Resources.Culture;
					return;
				}
				
				culture = Resources.FindCultureInfo (suffix);
				
				if (culture != null)
				{
					level = ResourceLevel.Localised;
					return;
				}
			}
			
			if ((len == 3) &&
				(suffix[0] == 'X'))
			{
				culture = Resources.FindCultureInfo (suffix.Substring (1, 2));
				
				if (culture != null)
				{
					level = ResourceLevel.Customised;
					return;
				}
			}
			
			throw new ResourceException (string.Format ("Invalid suffix ({0}) specified in MapFromSuffix.", suffix));
		}
		
		
		public static string GetLevelCaption(ResourceLevel level, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = Resources.Culture;
			}
			
			switch (level)
			{
				case ResourceLevel.Default:
					return "*";
				case ResourceLevel.Localised:
					return culture.DisplayName;
				case ResourceLevel.Customised:
					return string.Concat ("Perso. ", culture.DisplayName);
			}
			
			throw new ResourceException (string.Format ("Invalid level {0} specified in GetLevelCaption.", level));
		}
		
		
		public static string[] GetBundleIds(string name_filter)
		{
			return Resources.GetBundleIds (name_filter, null, ResourceLevel.Default, Resources.Culture);
		}
		
		public static string[] GetBundleIds(string name_filter, string type_filter)
		{
			return Resources.GetBundleIds (name_filter, type_filter, ResourceLevel.Default, Resources.Culture);
		}
		
		public static string[] GetBundleIds(string name_filter, ResourceLevel level)
		{
			return Resources.GetBundleIds (name_filter, null, level, Resources.Culture);
		}
		
		public static string[] GetBundleIds(string name_filter, string type_filter, ResourceLevel level)
		{
			return Resources.GetBundleIds (name_filter, type_filter, level, Resources.Culture);
		}
		
		public static string[] GetBundleIds(string name_filter, ResourceLevel level, CultureInfo culture)
		{
			return Resources.GetBundleIds (name_filter, null, level, culture);
		}
		
		public static string[] GetBundleIds(string name_filter, string type_filter, ResourceLevel level, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = Resources.Culture;
			}
			
			switch (level)
			{
				case ResourceLevel.Default:
				case ResourceLevel.Customised:
				case ResourceLevel.Localised:
				case ResourceLevel.All:
					break;
				default:
					throw new ResourceException (string.Format ("Invalid level {0} specified in GetBundleIds.", level));
			}
			
			string name_filter_id;
			
			IResourceProvider provider = Resources.FindProvider (name_filter, out name_filter_id);
			
			if (provider != null)
			{
				return provider.GetIds (name_filter_id, type_filter, level, culture);
			}
			
			return null;
		}
		
		
		public static ResourceBundle GetBundle(string id)
		{
			return Resources.GetBundle (id, ResourceLevel.Merged, 0);
		}
		
		public static ResourceBundle GetBundle(string id, ResourceLevel level)
		{
			return Resources.GetBundle (id, level, 0);
		}
		
		public static ResourceBundle GetBundle(string id, ResourceLevel level, int recursion)
		{
			return Resources.GetBundle (id, level, Resources.Culture, recursion);
		}
		
		public static ResourceBundle GetBundle(string id, ResourceLevel level, CultureInfo culture)
		{
			return Resources.GetBundle (id, level, culture, 0);
		}
		
		public static ResourceBundle GetBundle(string id, ResourceLevel level, CultureInfo culture, int recursion)
		{
			if (culture == null)
			{
				culture = Resources.Culture;
			}
			
			//	TODO: il faudra rajouter un cache pour éviter de consulter chaque fois
			//	le provider, lorsqu'une ressource est demandée...
			
			string resource_id;
			
			IResourceProvider provider = Resources.FindProvider (id, out resource_id);
			ResourceBundle    bundle   = null;
			
			//	Passe en revue les divers providers de bundles pour voir si la ressource
			//	demandée n'est pas disponible chez eux. Si oui, c'est celle-ci qui sera
			//	utilisée :
			
			foreach (IBundleProvider bundle_provider in Resources.bundle_providers)
			{
				bundle = bundle_provider.GetBundle (provider, resource_id, level, culture, recursion);
				
				if (bundle != null)
				{
					return bundle;
				}
			}
			
			if (provider != null)
			{
				string prefix = provider.Prefix;
				
				switch (level)
				{
					case ResourceLevel.Merged:
						bundle = ResourceBundle.Create (prefix, resource_id, level, culture, recursion);
						bundle.Compile (provider.GetData (resource_id, ResourceLevel.Default, culture));
						bundle.Compile (provider.GetData (resource_id, ResourceLevel.Localised, culture));
						bundle.Compile (provider.GetData (resource_id, ResourceLevel.Customised, culture));
						break;
					
					case ResourceLevel.Default:
					case ResourceLevel.Localised:
					case ResourceLevel.Customised:
						bundle = ResourceBundle.Create (prefix, resource_id, level, culture, recursion);
						bundle.Compile (provider.GetData (resource_id, level, culture));
						break;
					
					default:
						throw new ResourceException (string.Format ("Invalid level {0} for resource '{1}'", level, id));
				}
			}
			
			if ((bundle != null) &&
				(bundle.IsEmpty))
			{
				bundle = null;
			}
			
			return bundle;
		}
		
		
		public static byte[] GetBinaryData(string id)
		{
			return Resources.GetBinaryData (id, ResourceLevel.Merged, null);
		}
		
		public static byte[] GetBinaryData(string id, ResourceLevel level)
		{
			return Resources.GetBinaryData (id, ResourceLevel.Merged, null);
		}
		
		public static byte[] GetBinaryData(string id, ResourceLevel level, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = Resources.Culture;
			}
			
			//	TODO: il faudrait peut-être rajouter un cache pour éviter de consulter
			//	chaque fois le provider, lorsqu'une ressource est demandée.
			
			string resource_id;
			byte[] data = null;
			
			IResourceProvider provider = Resources.FindProvider (id, out resource_id);
			
			if (provider != null)
			{
				switch (level)
				{
					case ResourceLevel.Merged:
						data = provider.GetData (resource_id, ResourceLevel.Default, culture);
						if (data != null) break;
						data = provider.GetData (resource_id, ResourceLevel.Localised, culture);
						if (data != null) break;
						data = provider.GetData (resource_id, ResourceLevel.Customised, culture);
						if (data != null) break;
						break;
					
					case ResourceLevel.Default:
					case ResourceLevel.Localised:
					case ResourceLevel.Customised:
						data = provider.GetData (resource_id, level, culture);
						break;
					
					default:
						throw new ResourceException (string.Format ("Invalid level {0} for resource '{1}'", level, id));
				}
			}
			
			return data;
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
			if (Resources.IsTextRef (text))
			{
				string res = Resources.GetText (text.Substring (5, text.Length-6));
				
				if (res == null)
				{
					res = string.Concat (@"<font color=""#ff0000"">", text, "</font>");
				}
				
				return res;
			}
			
			return text;
		}
		
		
		public static string GetText(string id)
		{
			return Resources.GetText (id, ResourceLevel.Merged);
		}
		
		public static string GetText(string id, ResourceLevel level)
		{
			string bundle_name;
			string field_name;
			
			if (ResourceBundle.SplitTarget (id, out bundle_name, out field_name))
			{
				ResourceBundle bundle = Resources.GetBundle (bundle_name, level);
			
				if (bundle != null)
				{
					ResourceBundle.Field field = bundle[field_name];
					
					if (field != null)
					{
						return field.AsString;
					}
				}
			}
			
			return null;
		}
		
		
		public static void SetBundle(ResourceBundle bundle, ResourceSetMode mode)
		{
			ResourceLevel level   = bundle.ResourceLevel;
			CultureInfo   culture = bundle.Culture;
			string        id      = bundle.PrefixedName;
			
			if (culture == null)
			{
				culture = Resources.Culture;
			}
			
			switch (level)
			{
				case ResourceLevel.Default:
				case ResourceLevel.Localised:
				case ResourceLevel.Customised:
					break;
				
				default:
					throw new ResourceException (string.Format ("Invalid level {0} for resource '{1}'", level, id));
			}
			
			string resource_id;
			
			IResourceProvider provider = Resources.FindProvider (id, out resource_id);
			byte[]            data     = bundle.CreateXmlAsData ();
			
			if (provider != null)
			{
				if (provider.SetData (resource_id, level, culture, data, mode) == false)
				{
					throw new ResourceException (string.Format ("Could not store bundle '{0}'.", id));
				}
			}
		}
		
		public static void DebugDumpProviders()
		{
			for (int i = 0; i < Resources.resource_providers.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Prefix '{0}' implemented by class {1}", Resources.resource_providers[i].Prefix, Resources.resource_providers[i].GetType ().Name));
			}
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
			System.Collections.ArrayList providers = new System.Collections.ArrayList ();
			System.Reflection.Assembly   assembly  = System.Reflection.Assembly.LoadWithPartialName ("Common.Support.Implementation");
			
			System.Type[] types_in_assembly = assembly.GetTypes ();
			
			foreach (System.Type type in types_in_assembly)
			{
				if ((type.IsClass) &&
					(!type.IsAbstract))
				{
					if (type.GetInterface ("IResourceProvider") != null)
					{
						IResourceProvider provider = System.Activator.CreateInstance (type) as IResourceProvider;
						
						if (provider != null)
						{
							System.Diagnostics.Debug.Assert (Resources.resource_provider_hash.Contains (provider.Prefix) == false);
							
							providers.Add (provider);
							Resources.resource_provider_hash[provider.Prefix] = provider;
							
							provider.SelectLocale (Resources.culture);
						}
					}
				}
			}
			
			Resources.resource_providers = new IResourceProvider[providers.Count];
			providers.CopyTo (Resources.resource_providers);
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
		
		
		private static void SelectLocale(CultureInfo culture)
		{
			Resources.culture = culture;
			
			for (int i = 0; i < Resources.resource_providers.Length; i++)
			{
				Resources.resource_providers[i].SelectLocale (culture);
			}
		}
		
		private static IResourceProvider FindProvider(string full_id, out string local_id)
		{
			if (full_id != null)
			{
				int pos = full_id.IndexOf (":");
			
				if (pos > 0)
				{
					string prefix;
					
					prefix   = full_id.Substring (0, pos);
					local_id = full_id.Substring (pos+1);
					
					IResourceProvider provider = Resources.resource_provider_hash[prefix] as IResourceProvider;
					
					return provider;
				}
				
				if (Resources.default_prefix != null)
				{
					string prefix;
					
					prefix   = Resources.DefaultPrefix;
					local_id = full_id;
					
					IResourceProvider provider = Resources.resource_provider_hash[prefix] as IResourceProvider;
					
					return provider;
				}
			}
			
			local_id = null;
			return null;
		}
		
		
		
		private static CultureInfo				culture;
		private static CultureInfo[]			cultures;
		private static IResourceProvider[]		resource_providers;
		private static Hashtable				resource_provider_hash;
		private static string					application_name;
		private static string					default_prefix = "file";
		private static IBundleProvider[]		bundle_providers;
	}
}
