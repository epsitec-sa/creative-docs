//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Support
{
	using System.Globalization;
	using System.Collections;
	
	/// <summary>
	/// La classe Resources permet de gérer les ressources de l'application.
	/// </summary>
	public class Resources
	{
		private Resources()
		{
		}
		
		static Resources()
		{
			Resources.providers     = new IResourceProvider[0];
			Resources.provider_hash = new System.Collections.Hashtable ();
			Resources.culture       = CultureInfo.CurrentCulture;
			
			Resources.Initialise ();
		}
		
		
		protected static void Initialise()
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
							System.Diagnostics.Debug.Assert (Resources.provider_hash.Contains (provider.Prefix) == false);
							
							providers.Add (provider);
							Resources.provider_hash[provider.Prefix] = provider;
							
							provider.SelectLocale (Resources.culture);
						}
					}
				}
			}
			
			Resources.providers = new IResourceProvider[providers.Count];
			providers.CopyTo (Resources.providers);
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
			
			for (int i = 0; i < Resources.providers.Length; i++)
			{
				Resources.providers[i].Setup (application_name);
			}
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
			get { return Resources.providers.Length; }
		}
		
		public static string[]					ProviderPrefixes
		{
			get
			{
				string[] prefixes = new string[Resources.providers.Length];
				
				for (int i = 0; i < Resources.providers.Length; i++)
				{
					prefixes[i] = Resources.providers[i].Prefix;
				}
				
				return prefixes;
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
			get { return "99"; }
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
		
		
		public static string[] GetBundleIds(string filter)
		{
			return Resources.GetBundleIds (filter, ResourceLevel.Default, Resources.Culture);
		}
		
		public static string[] GetBundleIds(string filter, ResourceLevel level)
		{
			return Resources.GetBundleIds (filter, level, Resources.Culture);
		}
		
		public static string[] GetBundleIds(string filter, ResourceLevel level, CultureInfo culture)
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
					break;
				default:
					throw new ResourceException (string.Format ("Invalid level {0} specified in GetBundleIds.", level));
			}
			
			string resource_id;
			
			IResourceProvider provider = Resources.FindProvider (filter, out resource_id);
			
			if (provider != null)
			{
				return provider.GetIds (resource_id, level, culture);
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
			return Resources.GetBundle (id, level, recursion, Resources.Culture);
		}
		
		public static ResourceBundle GetBundle(string id, ResourceLevel level, CultureInfo culture)
		{
			return Resources.GetBundle (id, level, 0, culture);
		}
		
		public static ResourceBundle GetBundle(string id, ResourceLevel level, int recursion, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = Resources.Culture;
			}
			
			//	TODO: il faudrait peut-être rajouter un cache pour éviter de consulter
			//	chaque fois le provider, lorsqu'une ressource est demandée.
			
			string resource_id;
			
			IResourceProvider provider = Resources.FindProvider (id, out resource_id);
			ResourceBundle    bundle   = null;
			
			if (provider != null)
			{
				string prefix = provider.Prefix;
				
				switch (level)
				{
					case ResourceLevel.Merged:
						bundle = ResourceBundle.Create (resource_id, prefix, level, culture, recursion);
						bundle.Compile (provider.GetData (resource_id, ResourceLevel.Default, culture));
						bundle.Compile (provider.GetData (resource_id, ResourceLevel.Localised, culture));
						bundle.Compile (provider.GetData (resource_id, ResourceLevel.Customised, culture));
						break;
					
					case ResourceLevel.Default:
					case ResourceLevel.Localised:
					case ResourceLevel.Customised:
						bundle = ResourceBundle.Create (resource_id, prefix, level, culture, recursion);
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
			string        id      = bundle.Name;
			
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
				provider.SetData (resource_id, level, culture, data, mode);
			}
		}
		
		public static void DebugDumpProviders()
		{
			for (int i = 0; i < Resources.providers.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Prefix '{0}' implemented by class {1}", Resources.providers[i].Prefix, Resources.providers[i].GetType ().Name));
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

		
		
		protected static void SelectLocale(CultureInfo culture)
		{
			Resources.culture = culture;
			
			for (int i = 0; i < Resources.providers.Length; i++)
			{
				Resources.providers[i].SelectLocale (culture);
			}
		}
		
		protected static IResourceProvider FindProvider(string full_id, out string local_id)
		{
			if (full_id != null)
			{
				int pos = full_id.IndexOf (":");
			
				if (pos > 0)
				{
					string prefix;
					
					prefix   = full_id.Substring (0, pos);
					local_id = full_id.Substring (pos+1);
					
					IResourceProvider provider = Resources.provider_hash[prefix] as IResourceProvider;
					
					return provider;
				}
			}
			
			local_id = null;
			return null;
		}
		
		
		
		protected static CultureInfo			culture;
		protected static IResourceProvider[]	providers;
		protected static Hashtable				provider_hash;
		protected static string					application_name;
	}
}
