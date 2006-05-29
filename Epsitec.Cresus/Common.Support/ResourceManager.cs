//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe ResourceManager permet de gérer les ressources de l'application.
	/// </summary>
	public sealed class ResourceManager
	{
		public ResourceManager() : this (Support.Globals.Directories.Executable)
		{
		}
		
		public ResourceManager(System.Type type) : this (System.IO.Path.GetDirectoryName (type.Assembly.Location))
		{
		}
		
		public ResourceManager(string path)
		{
			this.resource_providers     = new IResourceProvider[0];
			this.resource_provider_hash = new Dictionary<string, IResourceProvider> ();
			this.culture                = CultureInfo.CurrentCulture;
			this.moduleInfos = new Dictionary<string, ResourceModuleInfo[]> ();
			
			if ((path != null) &&
				(path.Length > 0))
			{
				this.default_path = path;
			}
			else
			{
				this.default_path = System.IO.Directory.GetCurrentDirectory ();
			}
			
			this.InternalInitialize ();
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
		
		public string							ActivePrefix
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
		
		public CultureInfo						ActiveCulture
		{
			get
			{
				return this.culture;
			}
			set
			{
				if (Resources.EqualCultures (this.culture, value) == false)
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
				return this.MapToSuffix (ResourceLevel.Localized, this.culture);
			}
		}
		
		public string							CustomisedSuffix
		{
			get
			{
				return this.MapToSuffix (ResourceLevel.Localized, this.culture);
			}
		}
		
		public IImageProvider					ImageProvider
		{
			get
			{
				return Support.ImageProvider.Default;
			}
		}

		public string							DefaultPath
		{
			get
			{
				return this.default_path;
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
			//	spécifié. Ceci ne peut être fait qu'une seule fois.
			
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
			//	succès de leur initialisation :

			List<IResourceProvider> list = new List<IResourceProvider> ();
			
			this.resource_provider_hash.Clear ();
			
			for (int i = 0; i < this.resource_providers.Length; i++)
			{
				IResourceProvider provider = this.resource_providers[i];
				
				if (provider.SetupApplication (application_name))
				{
					//	Conserve le fournisseur qui a réussi son initialisation; un fournisseur qui
					//	échoue ici est simplement écarté...
					
					list.Add (provider);
					this.resource_provider_hash[provider.Prefix] = provider;
				}
			}
			
			this.resource_providers = list.ToArray ();
		}
		
		
		public void AddBundleProvider(IBundleProvider bundle_provider)
		{
			this.bundleProviders.Add (bundle_provider);
		}
		
		public void RemoveBundleProvider(IBundleProvider bundle_provider)
		{
			this.bundleProviders.Remove (bundle_provider);
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
				prefix = this.ActivePrefix;
				
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
				case ResourceLevel.Localized:	return culture.TwoLetterISOLanguageName;
				case ResourceLevel.Customized:	return string.Concat ("X", culture.TwoLetterISOLanguageName);
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
					level = ResourceLevel.Localized;
					return;
				}
			}
			
			if ((len == 3) &&
				(suffix[0] == 'X'))
			{
				culture = Resources.FindCultureInfo (suffix.Substring (1, 2));
				
				if (culture != null)
				{
					level = ResourceLevel.Customized;
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
				case ResourceLevel.Localized:	return culture.DisplayName;
				case ResourceLevel.Customized:	return string.Concat ("Pers. ", culture.DisplayName);
			}
			
			throw new ResourceException (string.Format ("Invalid level {0} specified in GetLevelCaption.", level));
		}

		public string[] GetModuleNames(string prefix)
		{
			ResourceModuleInfo[] infos = this.GetModuleInfos (prefix);
			
			if (infos != null)
			{
				string[] names = new string[infos.Length];
				
				for (int i = 0; i < infos.Length; i++)
				{
					names[i] = infos[i].Name;
				}
				
				return names;
			}

			return null;
		}

		public ResourceModuleInfo[] GetModuleInfos(string prefix)
		{
			ResourceModuleInfo[] infos;
			
			if (! this.moduleInfos.TryGetValue (prefix, out infos))
			{
				this.RefreshModuleInfos (prefix);
				this.moduleInfos.TryGetValue (prefix, out infos);
			}

			return infos;
		}
		
		public void RefreshModuleInfos(string prefix)
		{
			IResourceProvider provider;

			if (this.resource_provider_hash.TryGetValue (prefix, out provider))
			{
				this.moduleInfos[prefix] = provider.GetModules ();
			}
			else
			{
				this.moduleInfos.Remove (prefix);
			}
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
				case ResourceLevel.Customized:
				case ResourceLevel.Localized:
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
			return this.GetBundle (id, ResourceLevel.Merged, this.culture, 0);
		}
		
		public ResourceBundle GetBundle(string id, ResourceLevel level)
		{
			return this.GetBundle (id, level, this.culture, 0);
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
			
			//	TODO: il faudra rajouter un cache pour éviter de consulter chaque fois
			//	le provider, lorsqu'une ressource est demandée...
			
			string resource_id;
			
			IResourceProvider provider = this.FindProvider (id, out resource_id);
			ResourceBundle    bundle   = null;
			
			//	Passe en revue les divers providers de bundles pour voir si la ressource
			//	demandée n'est pas disponible chez eux. Si oui, c'est celle-ci qui sera
			//	utilisée :
			
			foreach (IBundleProvider bundle_provider in this.bundleProviders)
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
				string key = ResourceManager.CreateBundleKey (prefix, resource_id, level, culture);

				if (this.bundleCache.TryGetValue (key, out bundle))
				{
					//	OK, on a trouvé un bundle dans le cache !
				}
				else
				{
					switch (level)
					{
						case ResourceLevel.Merged:
							bundle = ResourceBundle.Create (this, prefix, resource_id, level, culture, recursion);
							bundle.Compile (provider.GetData (resource_id, ResourceLevel.Default, culture));
							bundle.Compile (provider.GetData (resource_id, ResourceLevel.Localized, culture));
							bundle.Compile (provider.GetData (resource_id, ResourceLevel.Customized, culture));
							break;

						case ResourceLevel.Default:
						case ResourceLevel.Localized:
						case ResourceLevel.Customized:
							bundle = ResourceBundle.Create (this, prefix, resource_id, level, culture, recursion);
							bundle.Compile (provider.GetData (resource_id, level, culture));
							break;

						default:
							throw new ResourceException (string.Format ("Invalid level {0} for resource '{1}'", level, id));
					}

					System.Diagnostics.Debug.Assert (bundle != null);

					this.bundleCache[key] = bundle;
				}
			}
			
			if ((bundle != null) &&
				(bundle.IsEmpty) &&
				(bundle.Type == ""))
			{
				bundle = null;
			}
			
			return bundle;
		}


		public void Bind(Types.DependencyObject targetObject, Types.DependencyProperty targetProperty, string resourceId)
		{
			//	Attache la cible (object/propriété) avec la ressource décrite
			//	par une ID complète (prefix + bundle + field).
			
			if ((targetObject == null) ||
				(targetProperty == null) ||
				(resourceId == null))
			{
				throw new System.ArgumentNullException ();
			}
			
			string bundleName;
			string fieldName;

			if ((ResourceBundle.SplitTarget (resourceId, out bundleName, out fieldName)) &&
				(bundleName.Length > 0) &&
				(fieldName.Length > 0))
			{
				BundleBindingProxy proxy = this.GetBundleBindingProxy (bundleName);

				if (proxy != null)
				{
					Types.Binding binding = new Types.Binding (Types.BindingMode.OneTime, proxy, fieldName);

					proxy.AddBinding (binding);
					
					targetObject.SetBinding (targetProperty, binding);

					return;
				}
			}

			throw new ResourceException (string.Format ("Cannot bind to ressource '{0}'", resourceId));
		}

		private BundleBindingProxy GetBundleBindingProxy(string bundleName)
		{
			//	Trouve le proxy qui liste les bindings pour le bundle spécifié.
			
			//	La liste des proxies est gérée par le gestionnaire de ressources
			//	plutôt que le bundle lui-même, car il faut pouvoir passer d'une
			//	culture active à une autre sans perdre les informations de
			//	binding.
			
			BundleBindingProxy proxy;

			if (this.bindingProxies.TryGetValue (bundleName, out proxy) == false)
			{
				//	Le proxy pour le bundle en question n'existe pas. Il faut
				//	donc le créer :
				
				ResourceBundle bundle = this.GetBundle (bundleName, ResourceLevel.Merged, this.culture, 0);

				if (bundle == null)
				{
					return null;
				}
				
				proxy = new BundleBindingProxy (bundle);

				System.Diagnostics.Debug.Assert (proxy.Bundle.PrefixedName == bundleName);
				System.Diagnostics.Debug.Assert (Resources.EqualCultures (proxy.Bundle.Culture, this.culture));
				
				this.bindingProxies[bundleName] = proxy;
			}

			return proxy;
		}

		private void SyncBundleBindingProxies()
		{
			//	Met à jour tous les proxies en les synchronisant avec la culture
			//	active.
			
			foreach (KeyValuePair<string, BundleBindingProxy> pair in this.bindingProxies)
			{
				string name = pair.Value.Bundle.PrefixedName;
				
				ResourceBundle oldBundle = pair.Value.Bundle;
				ResourceBundle newBundle = this.GetBundle (name, ResourceLevel.Merged, this.culture);
				
				pair.Value.SwitchToBundle (newBundle);
			}
		}

		
		public Drawing.Image GetImage(string name)
		{
			return this.ImageProvider.GetImage (name, this);
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
			
			//	TODO: il faudrait peut-être rajouter un cache pour éviter de consulter
			//	chaque fois le provider, lorsqu'une ressource est demandée.
			
			string resource_id;
			byte[] data = null;
			
			IResourceProvider provider = this.FindProvider (id, out resource_id);
			
			if (provider != null)
			{
				switch (level)
				{
					case ResourceLevel.Merged:
						data = provider.GetData (resource_id, ResourceLevel.Customized, culture);	if (data != null) break;
						data = provider.GetData (resource_id, ResourceLevel.Localized, culture);	if (data != null) break;
						data = provider.GetData (resource_id, ResourceLevel.Default, culture);
						break;
					
					case ResourceLevel.Default:
					case ResourceLevel.Localized:
					case ResourceLevel.Customized:
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
		
		public object GetData(string id, ResourceLevel level, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = this.culture;
			}
			
			string bundle_name;
			string field_name;
			
			if (ResourceBundle.SplitTarget (id, out bundle_name, out field_name))
			{
				ResourceBundle bundle = this.GetBundle (bundle_name, level, culture);
			
				if (bundle != null)
				{
					ResourceBundle.Field field = bundle[field_name];
					
					if (field != null)
					{
						return field.Data;
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
				case ResourceLevel.Localized:
				case ResourceLevel.Customized:
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
		
		
		public bool RemoveBundle(string id, ResourceLevel level, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = this.culture;
			}
			
			switch (level)
			{
				case ResourceLevel.Default:
				case ResourceLevel.Localized:
				case ResourceLevel.Customized:
				case ResourceLevel.All:
					break;
				
				default:
					throw new ResourceException (string.Format ("Invalid level {0} for resource '{1}'", level, id));
			}
			
			string resource_id;
			
			IResourceProvider provider = this.FindProvider (id, out resource_id);
			
			if (provider != null)
			{
				string prefix = provider.Prefix;
				string key = ResourceManager.CreateBundleKey (prefix, resource_id, level, culture);
				
				this.bundleCache.Remove (key);
				
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

		public void ClearBundleCache()
		{
			this.bundleCache.Clear ();
		}

		public void TrimBindingCache()
		{
			foreach (BundleBindingProxy proxy in this.bindingProxies.Values)
			{
				proxy.TrimBindingCache ();
			}
		}


		private void InternalInitialize()
		{
			System.Collections.ArrayList providers = new System.Collections.ArrayList ();
			System.Reflection.Assembly   assembly  = AssemblyLoader.Load ("Common.Support.Implementation");
			
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
							System.Diagnostics.Debug.Assert (this.resource_provider_hash.ContainsKey (provider.Prefix) == false);
							
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

		private static string CreateBundleKey(string prefix, string resource_id, ResourceLevel level, CultureInfo culture)
		{
			System.Diagnostics.Debug.Assert (prefix != null);
			System.Diagnostics.Debug.Assert (prefix.Length > 0);
			System.Diagnostics.Debug.Assert (prefix.Contains (":") == false);
			System.Diagnostics.Debug.Assert (resource_id.Length > 0);
			System.Diagnostics.Debug.Assert (resource_id.Contains ("#") == false);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			buffer.Append (prefix);
			buffer.Append (":");
			buffer.Append (resource_id);
			buffer.Append ("~");
			buffer.Append ((int) level);
			buffer.Append ("~");
			buffer.Append (culture.TwoLetterISOLanguageName);

			return buffer.ToString ();
		}
		
		private void SelectLocale(CultureInfo culture)
		{
			this.culture = culture;
			
			for (int i = 0; i < this.resource_providers.Length; i++)
			{
				this.resource_providers[i].SelectLocale (culture);
			}

			this.SyncBundleBindingProxies ();
		}
		
		private IResourceProvider FindProvider(string full_id, out string local_id)
		{
			if (full_id != null)
			{
				int pos = full_id.IndexOf (':');
			
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
					
					prefix   = this.ActivePrefix;
					local_id = full_id;
					
					IResourceProvider provider = this.resource_provider_hash[prefix] as IResourceProvider;
					
					return provider;
				}
			}
			
			local_id = null;
			return null;
		}

		#region Private BundleBindingProxy Class

		private class BundleBindingProxy : Types.IResourceBoundSource
		{
			public BundleBindingProxy(ResourceBundle bundle)
			{
				this.bundle = bundle;
			}

			public ResourceBundle				Bundle
			{
				get
				{
					return this.bundle;
				}
			}
			
			public void SwitchToBundle(ResourceBundle bundle)
			{
				if (this.bundle != bundle)
				{
					this.bundle = bundle;
					this.SyncBindings ();
				}
			}

			public void SyncBindings()
			{
				WeakBinding[] bindings = this.bindings.ToArray ();

				for (int i = 0; i < bindings.Length; i++)
				{
					Types.Binding binding = bindings[i].Binding;

					if (binding == null)
					{
						this.bindings.Remove (bindings[i]);
					}
					else
					{
						binding.UpdateTargets (Types.BindingUpdateMode.Reset);
					}
				}
			}

			public void AddBinding(Types.Binding binding)
			{
				this.bindings.Add (new WeakBinding (binding));
			}

			public void TrimBindingCache()
			{
				List<WeakBinding> clean = new List<WeakBinding> ();
				
				foreach (WeakBinding binding in this.bindings)
				{
					if (binding.IsAlive)
					{
						clean.Add (binding);
					}
				}

				this.bindings = clean;
			}

			#region IResourceBoundSource Members

			object Epsitec.Common.Types.IResourceBoundSource.GetValue(string id)
			{
				System.Diagnostics.Debug.Assert (this.bundle != null);
				System.Diagnostics.Debug.Assert (this.bundle.Contains (id));
				
				return this.bundle[id].Data;
			}

			#endregion

			ResourceBundle						bundle;
			List<WeakBinding>					bindings = new List<WeakBinding> ();
		}

		#endregion

		#region Private WeakBinding Class

		private class WeakBinding : System.WeakReference
		{
			public WeakBinding(Types.Binding binding) : base (binding)
			{
			}

			public Types.Binding Binding
			{
				get
				{
					return this.Target as Types.Binding;
				}
			}
		}

		#endregion

		private Dictionary<string, ResourceModuleInfo[]> moduleInfos;
		private CultureInfo						culture;
		private IResourceProvider[]				resource_providers;
		private Dictionary<string, IResourceProvider> resource_provider_hash;
		private string							application_name;
		private string							default_prefix = "file";
		private string							default_path;
		
		List<IBundleProvider>					bundleProviders = new List<IBundleProvider> ();
		Dictionary<string, ResourceBundle>		bundleCache = new Dictionary<string, ResourceBundle> ();
		Dictionary<string, BundleBindingProxy>	bindingProxies = new Dictionary<string, BundleBindingProxy> ();
		
	}
}
