//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Globalization;
using System.Collections;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe ResourceManager permet de g�rer les ressources de l'application.
	/// </summary>
	public sealed class ResourceManager
	{
		public ResourceManager()
		{
			this.resource_providers     = new IResourceProvider[0];
			this.resource_provider_hash = new System.Collections.Hashtable ();
			this.bundle_providers       = new IBundleProvider[0];
			this.culture                = CultureInfo.CurrentCulture;
			
			this.InternalInitialise ();
		}
		
		
		public int								ProviderCount
		{
			get
			{
				return this.resource_providers.Length;
			}
		}
		
		public string[]							ProviderPrefixes
		{
			get
			{
				string[] prefixes = new string[this.resource_providers.Length];
				
				for (int i = 0; i < this.resource_providers.Length; i++)
				{
					prefixes[i] = this.resource_providers[i].Prefix;
				}
				
				return prefixes;
			}
		}
		
		public string							DefaultPrefix
		{
			get
			{
				if (this.default_prefix == null)
				{
					return "";
				}
				
				return this.default_prefix;
			}
			set
			{
				if (value == "")
				{
					value = null;
				}
				
				this.default_prefix = value;
			}
		}
		
		public CultureInfo						DefaultCulture
		{
			get
			{
				return this.culture;
			}
			set
			{
				if (this.culture != value)
				{
					this.SelectLocale (value);
				}
			}
		}
		
		
		public string							DefaultSuffix
		{
			get
			{
				return this.MapToSuffix (ResourceLevel.Default, this.culture);
			}
		}
		
		public string							LocalisedSuffix
		{
			get
			{
				return this.MapToSuffix (ResourceLevel.Localised, this.culture);
			}
		}
		
		public string							CustomisedSuffix
		{
			get
			{
				return this.MapToSuffix (ResourceLevel.Localised, this.culture);
			}
		}
		
		
		public bool								IsReady
		{
			get
			{
				return this.application_name != null;
			}
		}
		
		
		public void SetupApplication(string application_name)
		{
			//	Initialise les fournisseurs de ressources pour le nom d'application
			//	sp�cifi�. Ceci ne peut �tre fait qu'une seule fois.
			
			if (this.application_name == application_name)
			{
				return;
			}
			
			if (this.application_name != null)
			{
				throw new System.InvalidOperationException ("Resource Providers may not be setup more than once.");
			}
			
			this.application_name = application_name;
			
			//	On reconstruit la table des fournisseurs disponibles en fonction du
			//	succ�s de leur initialisation :
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			this.resource_provider_hash.Clear ();
			
			for (int i = 0; i < this.resource_providers.Length; i++)
			{
				IResourceProvider provider = this.resource_providers[i];
				
				if (provider.SetupApplication (application_name))
				{
					//	Conserve le fournisseur qui a r�ussi son initialisation; un fournisseur qui
					//	�choue ici est simplement �cart�...
					
					list.Add (provider);
					this.resource_provider_hash[provider.Prefix] = provider;
				}
			}
			
			this.resource_providers = new IResourceProvider[list.Count];
			list.CopyTo (this.resource_providers);
		}
		
		
		public void AddBundleProvider(IBundleProvider bundle_provider)
		{
			ArrayList list = new ArrayList ();
			list.AddRange (this.bundle_providers);
			list.Add (bundle_provider);
			
			this.bundle_providers = new IBundleProvider[list.Count];
			list.CopyTo (this.bundle_providers);
		}
		
		public void RemoveBundleProvider(IBundleProvider bundle_provider)
		{
			ArrayList list = new ArrayList ();
			list.AddRange (this.bundle_providers);
			list.Remove (bundle_provider);
			
			this.bundle_providers = new IBundleProvider[list.Count];
			list.CopyTo (this.bundle_providers);
		}
		
		
		public bool ValidateId(string id)
		{
			IResourceProvider provider = this.FindProvider (id, out id);
			
			if (provider != null)
			{
				return provider.ValidateId (id);
			}
			
			return false;
		}
		
		public bool ContainsId(string id)
		{
			IResourceProvider provider = this.FindProvider (id, out id);
			
			if (provider != null)
			{
				return provider.Contains (id);
			}
			
			return false;
		}
		
		
		public string MakeFullName(string prefix, string name)
		{
			if (name == null)
			{
				throw new ResourceException ("Cannot make full name if name is missing.");
			}
			if (prefix == null)
			{
				prefix = this.DefaultPrefix;
				
				if (prefix == null)
				{
					throw new ResourceException (string.Format ("Cannot make full name if prefix is missing for resource {0}.", name));
				}
			}
			
			return string.Concat (prefix, ":", name);
		}
		
		
		public string MapToSuffix(ResourceLevel level, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = this.culture;
			}
			
			switch (level)
			{
				case ResourceLevel.Default:		return "00";
				case ResourceLevel.Localised:	return culture.TwoLetterISOLanguageName;
				case ResourceLevel.Customised:	return string.Concat ("X", culture.TwoLetterISOLanguageName);
			}
			
			throw new ResourceException (string.Format ("Invalid level {0} specified in MapToSuffix.", level));
		}
		
		public void MapFromSuffix(string suffix, out ResourceLevel level, out CultureInfo culture)
		{
			int len = suffix.Length;
			
			if (len == 2)
			{
				if (suffix == "00")
				{
					level   = ResourceLevel.Default;
					culture = this.culture;
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
		
		
		public string GetLevelCaption(ResourceLevel level, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = this.culture;
			}
			
			switch (level)
			{
				case ResourceLevel.Default:		return "*";
				case ResourceLevel.Localised:	return culture.DisplayName;
				case ResourceLevel.Customised:	return string.Concat ("Pers. ", culture.DisplayName);
			}
			
			throw new ResourceException (string.Format ("Invalid level {0} specified in GetLevelCaption.", level));
		}
		
		
		public string[] GetBundleIds(string name_filter)
		{
			return this.GetBundleIds (name_filter, null, ResourceLevel.Default, this.culture);
		}
		
		public string[] GetBundleIds(string name_filter, string type_filter)
		{
			return this.GetBundleIds (name_filter, type_filter, ResourceLevel.Default, this.culture);
		}
		
		public string[] GetBundleIds(string name_filter, ResourceLevel level)
		{
			return this.GetBundleIds (name_filter, null, level, this.culture);
		}
		
		public string[] GetBundleIds(string name_filter, string type_filter, ResourceLevel level)
		{
			return this.GetBundleIds (name_filter, type_filter, level, this.culture);
		}
		
		public string[] GetBundleIds(string name_filter, ResourceLevel level, CultureInfo culture)
		{
			return this.GetBundleIds (name_filter, null, level, culture);
		}
		
		public string[] GetBundleIds(string name_filter, string type_filter, ResourceLevel level, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = this.culture;
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
			
			IResourceProvider provider = this.FindProvider (name_filter, out name_filter_id);
			
			if (provider != null)
			{
				return provider.GetIds (name_filter_id, type_filter, level, culture);
			}
			
			return null;
		}
		
		
		public ResourceBundle GetBundle(string id)
		{
			return this.GetBundle (id, ResourceLevel.Merged, 0);
		}
		
		public ResourceBundle GetBundle(string id, ResourceLevel level)
		{
			return this.GetBundle (id, level, 0);
		}
		
		public ResourceBundle GetBundle(string id, ResourceLevel level, int recursion)
		{
			return this.GetBundle (id, level, this.culture, recursion);
		}
		
		public ResourceBundle GetBundle(string id, ResourceLevel level, CultureInfo culture)
		{
			return this.GetBundle (id, level, culture, 0);
		}
		
		public ResourceBundle GetBundle(string id, ResourceLevel level, CultureInfo culture, int recursion)
		{
			if (culture == null)
			{
				culture = this.culture;
			}
			
			//	TODO: il faudra rajouter un cache pour �viter de consulter chaque fois
			//	le provider, lorsqu'une ressource est demand�e...
			
			string resource_id;
			
			IResourceProvider provider = this.FindProvider (id, out resource_id);
			ResourceBundle    bundle   = null;
			
			//	Passe en revue les divers providers de bundles pour voir si la ressource
			//	demand�e n'est pas disponible chez eux. Si oui, c'est celle-ci qui sera
			//	utilis�e :
			
			foreach (IBundleProvider bundle_provider in this.bundle_providers)
			{
				bundle = bundle_provider.GetBundle (this, provider, resource_id, level, culture, recursion);
				
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
						bundle = ResourceBundle.Create (this, prefix, resource_id, level, culture, recursion);
						bundle.Compile (provider.GetData (resource_id, ResourceLevel.Default, culture));
						bundle.Compile (provider.GetData (resource_id, ResourceLevel.Localised, culture));
						bundle.Compile (provider.GetData (resource_id, ResourceLevel.Customised, culture));
						break;
					
					case ResourceLevel.Default:
					case ResourceLevel.Localised:
					case ResourceLevel.Customised:
						bundle = ResourceBundle.Create (this, prefix, resource_id, level, culture, recursion);
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
		
		
		public byte[] GetBinaryData(string id)
		{
			return this.GetBinaryData (id, ResourceLevel.Merged, null);
		}
		
		public byte[] GetBinaryData(string id, ResourceLevel level)
		{
			return this.GetBinaryData (id, ResourceLevel.Merged, null);
		}
		
		public byte[] GetBinaryData(string id, ResourceLevel level, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = this.culture;
			}
			
			//	TODO: il faudrait peut-�tre rajouter un cache pour �viter de consulter
			//	chaque fois le provider, lorsqu'une ressource est demand�e.
			
			string resource_id;
			byte[] data = null;
			
			IResourceProvider provider = this.FindProvider (id, out resource_id);
			
			if (provider != null)
			{
				switch (level)
				{
					case ResourceLevel.Merged:
						data = provider.GetData (resource_id, ResourceLevel.Customised, culture);	if (data != null) break;
						data = provider.GetData (resource_id, ResourceLevel.Localised, culture);	if (data != null) break;
						data = provider.GetData (resource_id, ResourceLevel.Default, culture);
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
		
		
		public string ResolveTextRef(string text)
		{
			if (Resources.IsTextRef (text))
			{
				string res = this.GetText (text.Substring (5, text.Length-6));
				
				if (res == null)
				{
					res = string.Concat (@"<font color=""#ff0000"">", text, "</font>");
				}
				
				return res;
			}
			
			return text;
		}
		
		
		public string GetText(string id)
		{
			return this.GetText (id, ResourceLevel.Merged);
		}
		
		public string GetText(string id, ResourceLevel level)
		{
			string bundle_name;
			string field_name;
			
			if (ResourceBundle.SplitTarget (id, out bundle_name, out field_name))
			{
				ResourceBundle bundle = this.GetBundle (bundle_name, level);
			
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
		
		
		public void SetBundle(ResourceBundle bundle, ResourceSetMode mode)
		{
			ResourceLevel level   = bundle.ResourceLevel;
			CultureInfo   culture = bundle.Culture;
			string        id      = bundle.PrefixedName;
			byte[]        data    = bundle.CreateXmlAsData ();
			
			if (this.SetBinaryData (id, level, culture, data, mode) == false)
			{
				throw new ResourceException (string.Format ("Could not store bundle '{0}'.", id));
			}
		}
		
		public bool SetBinaryData(string id, ResourceLevel level, CultureInfo culture, byte[] data, ResourceSetMode mode)
		{
			if (culture == null)
			{
				culture = this.culture;
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
			
			IResourceProvider provider = this.FindProvider (id, out resource_id);
			
			if (provider != null)
			{
				if (provider.SetData (resource_id, level, culture, data, mode))
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		public bool RemoveResource(string id, ResourceLevel level, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = this.culture;
			}
			
			switch (level)
			{
				case ResourceLevel.Default:
				case ResourceLevel.Localised:
				case ResourceLevel.Customised:
				case ResourceLevel.All:
					break;
				
				default:
					throw new ResourceException (string.Format ("Invalid level {0} for resource '{1}'", level, id));
			}
			
			string resource_id;
			
			IResourceProvider provider = this.FindProvider (id, out resource_id);
			
			if (provider != null)
			{
				if (provider.Remove (resource_id, level, culture))
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		public void DebugDumpProviders()
		{
			for (int i = 0; i < this.resource_providers.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Prefix '{0}' implemented by class {1}", this.resource_providers[i].Prefix, this.resource_providers[i].GetType ().Name));
			}
		}
		
		

		
		private void InternalInitialise()
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
							System.Diagnostics.Debug.Assert (this.resource_provider_hash.Contains (provider.Prefix) == false);
							
							provider.Setup (this);
							provider.SelectLocale (this.culture);
							
							providers.Add (provider);
							this.resource_provider_hash[provider.Prefix] = provider;
						}
					}
				}
			}
			
			this.resource_providers = new IResourceProvider[providers.Count];
			providers.CopyTo (this.resource_providers);
		}
		
		
		
		private void SelectLocale(CultureInfo culture)
		{
			this.culture = culture;
			
			for (int i = 0; i < this.resource_providers.Length; i++)
			{
				this.resource_providers[i].SelectLocale (culture);
			}
		}
		
		private IResourceProvider FindProvider(string full_id, out string local_id)
		{
			if (full_id != null)
			{
				int pos = full_id.IndexOf (":");
			
				if (pos > 0)
				{
					string prefix;
					
					prefix   = full_id.Substring (0, pos);
					local_id = full_id.Substring (pos+1);
					
					IResourceProvider provider = this.resource_provider_hash[prefix] as IResourceProvider;
					
					return provider;
				}
				
				if (this.default_prefix != null)
				{
					string prefix;
					
					prefix   = this.DefaultPrefix;
					local_id = full_id;
					
					IResourceProvider provider = this.resource_provider_hash[prefix] as IResourceProvider;
					
					return provider;
				}
			}
			
			local_id = null;
			return null;
		}
		
		
		
		private CultureInfo				culture;
		private IResourceProvider[]		resource_providers;
		private Hashtable				resource_provider_hash;
		private string					application_name;
		private string					default_prefix = "file";
		private IBundleProvider[]		bundle_providers;
	}
}
