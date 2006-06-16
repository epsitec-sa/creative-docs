//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Globalization;
using System.Collections;
using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ResourceManager</c> class provides access to the resources; the
	/// resources are organized in modules which contain 0...n resource bundles.
	/// Every resource bundle contains 0...m data fields.
	/// A data field (some systems call the field itself a "resource") can be
	/// accessed through its full resource id ("provider/module:bundle#field")
	/// or by specifying a DRUID.
	/// </summary>
	public sealed class ResourceManager : DependencyObject
	{
		public ResourceManager() : this (Support.Globals.Directories.Executable)
		{
		}
		
		public ResourceManager(System.Type type) : this (System.IO.Path.GetDirectoryName (type.Assembly.Location))
		{
		}
		
		public ResourceManager(string path)
		{
			this.providers = new Dictionary<string, ProviderRecord> ();
			this.culture = CultureInfo.CurrentCulture;
			this.defaultPath = string.IsNullOrEmpty (path) ? null : path;

			foreach (ResourceProviderFactory.Allocator allocator in Resources.Factory.Allocators)
			{
				ProviderRecord record = new ProviderRecord (this, allocator);
				this.providers.Add (record.Prefix, record);
			}
		}
		
		
		public int								ProviderCount
		{
			get
			{
				return this.providers.Count;
			}
		}
		
		public string[]							ProviderPrefixes
		{
			get
			{
				string[] prefixes = new string[this.providers.Count];
				this.providers.Keys.CopyTo (prefixes, 0);
				System.Array.Sort (prefixes);
				return prefixes;
			}
		}
		
		public string							ActivePrefix
		{
			get
			{
				return this.defaultPrefix ?? "";
			}
			set
			{
				if (value == "")
				{
					value = null;
				}
				
				this.defaultPrefix = value;
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
				return this.defaultPath;
			}
		}
		
		public bool								IsReady
		{
			get
			{
				return (this.defaultModuleName != null) && (this.defaultModuleId >= 0);
			}
		}
		
		
		public void DefineDefaultModuleName(string defaultModuleName)
		{
			//	Initialise les fournisseurs de ressources pour le nom d'application
			//	spécifié. Ceci ne peut être fait qu'une seule fois.
			
			if (this.defaultModuleName == defaultModuleName)
			{
				return;
			}
			
			if (this.defaultModuleName != null)
			{
				throw new System.InvalidOperationException ("Resource Providers may not be setup more than once.");
			}
			
			this.defaultModuleName = defaultModuleName;
			this.defaultModuleId   = -1;

			foreach (ProviderRecord provider in this.providers.Values)
			{
				ModuleRecord module = provider.GetModuleRecord (this.defaultModuleName);

				if (module != null)
				{
					if (this.defaultModuleId == -1)
					{
						this.defaultModuleId = module.ModuleInfo.Id;
					}
					else if (this.defaultModuleId != module.ModuleInfo.Id)
					{
						throw new ResourceException (string.Format ("Inconsistent module id for module '{0}'", this.defaultModuleName));
					}
				}

				provider.ActiveProvider = (module == null) ? null : module.Provider;
			}
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


		public void AnalyseFullId(string id, out string prefix, out string localId, out ResourceModuleInfo module)
		{
			if (!string.IsNullOrEmpty (id))
			{
				string moduleName;

				Resources.SplitFullId (id, out prefix, out localId);
				Resources.ResolveDruidReference (ref prefix, ref localId);
				Resources.SplitFullPrefix (prefix, out prefix, out moduleName);

				if (string.IsNullOrEmpty (prefix))
				{
					prefix = this.ActivePrefix;
				}
				if (string.IsNullOrEmpty (moduleName))
				{
					module = new ResourceModuleInfo (this.defaultModuleName, this.defaultModuleId);
				}
				else
				{
					module = ResourceModuleInfo.Parse (moduleName);
				}
			}
			else
			{
				prefix  = null;
				localId = null;
				module  = new ResourceModuleInfo ();
			}
		}

		public string NormalizeFullId(string id)
		{
			string prefix;
			string localId;

			Resources.SplitFullId (id, out prefix, out localId);
			Resources.ResolveDruidReference (ref prefix, ref localId);

			return this.NormalizeFullId (prefix, localId);
		}
		public string NormalizeFullId(string prefix, string localId)
		{
			if (string.IsNullOrEmpty (localId))
			{
				throw new ResourceException ("Cannot normalize resource name; local id is empty");
			}

			string moduleName;

			Resources.SplitFullPrefix (prefix, out prefix, out moduleName);
			
			if (string.IsNullOrEmpty (prefix))
			{
				prefix = this.ActivePrefix;
				
				if (string.IsNullOrEmpty (prefix))
				{
					throw new ResourceException (string.Format ("Cannot normalize resource name; prefix is empty for resource '{0}'", localId));
				}
			}

			ResourceModuleInfo module = ResourceModuleInfo.Parse (moduleName);

			this.NormalizeModule (prefix, ref module);
			prefix = Resources.JoinFullPrefix (prefix, module.ToString ());

			return Resources.JoinFullId (prefix, localId);
		}

		public void NormalizeModule(string prefix, ref ResourceModuleInfo module)
		{
			if (string.IsNullOrEmpty (prefix))
			{
				throw new ResourceException ("Cannot normalize bundle id; prefix is empty");
			}

			this.FindProviderFromPrefix (prefix, ref module);
		}

		public string MapToSuffix(ResourceLevel level, CultureInfo culture)
		{
			culture = culture ?? this.culture;
			
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
			culture = culture ?? this.culture;

			switch (level)
			{
				case ResourceLevel.Default:		return "*";
				case ResourceLevel.Localized:	return culture.DisplayName;
				case ResourceLevel.Customized:	return string.Concat ("Pers. ", culture.DisplayName);
			}
			
			throw new ResourceException (string.Format ("Invalid level {0} specified in GetLevelCaption.", level));
		}

		public IEnumerable<string> GetModuleNames(string prefix)
		{
			foreach (ResourceModuleInfo info in this.GetModuleInfos (prefix))
			{
				yield return info.Name;
			}
		}

		public IEnumerable<ResourceModuleInfo> GetModuleInfos(string prefix)
		{
			ProviderRecord provider;

			if (this.providers.TryGetValue (prefix, out provider))
			{
				return provider.Modules;
			}
			else
			{
				return new ResourceModuleInfo[0];
			}
		}
		
		public void RefreshModuleInfos(string prefix)
		{
			ProviderRecord provider;

			if (this.providers.TryGetValue (prefix, out provider))
			{
				provider.ClearModuleCache ();
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
			culture = culture ?? this.culture;

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


		/// <summary>
		/// Gets the bundle based on a DRUID. This will return the bundle which
		/// is named "_MMDLL".
		/// </summary>
		/// <param name="druid">The DRUID of the bundle.</param>
		/// <returns>The resource bundle; otherwise, <c>null</c>.</returns>
		public ResourceBundle GetBundle(Druid druid)
		{
			return this.GetBundle (druid.ToBundleId ());
		}
		
		public ResourceBundle GetBundle(Druid druid, ResourceLevel level)
		{
			return this.GetBundle (druid.ToBundleId (), level);
		}
		
		public ResourceBundle GetBundle(Druid druid, ResourceLevel level, CultureInfo culture)
		{
			return this.GetBundle (druid.ToBundleId (), level, culture);
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
			culture = culture ?? this.culture;
			
			string resourceId;
			ResourceModuleInfo module;
			
			IResourceProvider provider = this.FindProvider (id, out resourceId, out module);
			ResourceBundle    bundle   = null;

			if (Resources.IsFieldId (resourceId))
			{
				throw new ResourceException (string.Format ("'{0}' is not a bundle id (resolves to '{1}')", id, resourceId));
			}
			
			//	Passe en revue les divers providers de bundles pour voir si la ressource
			//	demandée n'est pas disponible chez eux. Si oui, c'est celle-ci qui sera
			//	utilisée :
			
			if (provider != null)
			{
				string prefix = provider.Prefix;
				string key = Resources.CreateBundleKey (prefix, module.Id, resourceId, level, culture);

				if (this.bundleCache.TryGetValue (key, out bundle))
				{
					//	OK, on a trouvé un bundle dans le cache !
					
					bundle.DefineRecursion (recursion);
				}
				else
				{
					switch (level)
					{
						case ResourceLevel.Merged:
							bundle = ResourceBundle.Create (this, prefix, module, resourceId, level, culture, recursion);
							this.CompileBundle (bundle, provider, resourceId, ResourceLevel.Default, culture);
							this.CompileBundle (bundle, provider, resourceId, ResourceLevel.Localized, culture);
							this.CompileBundle (bundle, provider, resourceId, ResourceLevel.Customized, culture);
							this.mergedBundlesInBundleCache++;
							break;

						case ResourceLevel.Default:
						case ResourceLevel.Localized:
						case ResourceLevel.Customized:
							bundle = ResourceBundle.Create (this, prefix, module, resourceId, level, culture, recursion);
							bundle.Compile (provider.GetData (resourceId, level, culture));
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

		private void CompileBundle(ResourceBundle bundle, IResourceProvider provider, string resourceId, ResourceLevel level, CultureInfo culture)
		{
			ResourceBundle source;
			byte[] data;
			string key = Resources.CreateBundleKey (provider.Prefix, bundle.Module.Id, resourceId, level, culture);

			if (this.bundleCache.TryGetValue (key, out source))
			{
				//	On a trouvé un bundle dans le cache. Il faut fusionner avec
				//	celui-ci plutôt que de charger un nouveau bundle depuis le
				//	disque !

				data = source.CreateXmlAsData ();
			}
			else
			{
				data = provider.GetData (resourceId, level, culture);
			}
			
			bundle.Compile (data, level);
		}

		/// <summary>
		/// Gets the bundle field based on a DRUID. The bundle used for this lookup
		/// is always the "Strings" bundle and the DRUID is used to find the matching
		/// field inside that bundle.
		/// </summary>
		/// <param name="druid">The DRUID of the field.</param>
		/// <returns>The resource bundle field; otherwise, <c>ResourceBundle.Field.Empty</c>.</returns>
		public ResourceBundle.Field GetBundleField(Druid druid)
		{
			return this.GetBundleField (druid, ResourceLevel.Merged, this.culture);
		}
		
		public ResourceBundle.Field GetBundleField(Druid druid, ResourceLevel level)
		{
			return this.GetBundleField (druid, level, this.culture);
		}
		
		public ResourceBundle.Field GetBundleField(Druid druid, ResourceLevel level, CultureInfo culture)
		{
			string id = this.NormalizeFullId (druid.ToResourceId ());
			
			string bundleName;
			string fieldName;

			if (Resources.SplitFieldId (id, out bundleName, out fieldName))
			{
				ResourceBundle bundle = this.GetBundle (bundleName, level, culture);

				if (bundle != null)
				{
					return bundle[fieldName];
				}
			}

			return null;
		}

		
		public void Bind(Types.DependencyObject targetObject, Types.DependencyProperty targetProperty, Druid druid)
		{
			this.Bind (targetObject, targetProperty, druid.ToResourceId ());
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

			ResourceBinding binding = new ResourceBinding (resourceId);

			if (this.SetResourceBinding (binding) == false)
			{
				throw new ResourceException (string.Format ("Cannot bind to ressource '{0}'", resourceId));
			}

			targetObject.SetBinding (targetProperty, binding);
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
			culture = culture ?? this.culture;
			
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


		/// <summary>
		/// Gets the caption based on the DRUID.
		/// </summary>
		/// <param name="druid">The DRUID of the caption.</param>
		/// <returns>The caption or <c>null</c> if it could not be found.</returns>
		public Caption GetCaption(Druid druid)
		{
			return this.GetCaption (druid, ResourceLevel.Merged, this.culture);
		}

		public Caption GetCaption(Druid druid, ResourceLevel level)
		{
			return this.GetCaption (druid, level, this.culture);
		}
		
		public Caption GetCaption(Druid druid, ResourceLevel level, CultureInfo culture)
		{
			culture = culture ?? this.culture;

			string resource = druid.ToResourceId ();
			string bundleName;
			string fieldName;

			Caption caption = null;
			
			if (Resources.SplitFieldId (resource, out bundleName, out fieldName))
			{
				switch (level)
				{
					case ResourceLevel.Merged:
						this.MergeWithCaption (this.GetBundle (bundleName, ResourceLevel.Default, culture), druid, ref caption);
						this.MergeWithCaption (this.GetBundle (bundleName, ResourceLevel.Localized, culture), druid, ref caption);
						this.MergeWithCaption (this.GetBundle (bundleName, ResourceLevel.Customized, culture), druid, ref caption);
						break;
					
					case ResourceLevel.Default:
					case ResourceLevel.Localized:
					case ResourceLevel.Customized:
						this.MergeWithCaption (this.GetBundle (bundleName, level, culture), druid, ref caption);
						break;
					
					default:
						throw new ResourceException ("Invalid resource level");
				}
			}

			return caption;
		}

		private void MergeWithCaption(ResourceBundle bundle, Druid druid, ref Caption caption)
		{
			if (bundle != null)
			{
				string source = bundle[druid].AsString;
				
				if (string.IsNullOrEmpty (source))
				{
					return;
				}

				Caption temp = new Caption ();
				temp.DeserializeFromString (source);
				
				caption = (caption == null) ? temp : Caption.Merge (caption, temp);
			}
		}

		/// <summary>
		/// Gets the text based on the DRUID. The bundle used for this lookup
		/// is always the "Strings" bundle and the DRUID is used to find the
		/// matching field inside that bundle.
		/// </summary>
		/// <param name="id">The druid of the field.</param>
		/// <returns>The text found in that field; otherwise, <c>null</c>.</returns>
		public string GetText(Druid druid)
		{
			return this.GetText (druid.ToResourceId (), ResourceLevel.Merged, this.culture);
		}
		
		public string GetText(Druid druid, ResourceLevel level)
		{
			return this.GetText (druid.ToResourceId (), level, this.culture);
		}
		
		public string GetText(Druid druid, ResourceLevel level, CultureInfo culture)
		{
			return this.GetText (druid.ToResourceId (), level, culture);
		}
		
		public string GetText(string id)
		{
			return this.GetText (id, ResourceLevel.Merged);
		}
		
		public string GetText(string id, ResourceLevel level)
		{
			return this.GetText (id, level, this.culture);
		}
		
		public string GetText(string id, ResourceLevel level, CultureInfo culture)
		{
			string bundle_name;
			string field_name;

			if (Resources.SplitFieldId (id, out bundle_name, out field_name))
			{
				ResourceBundle bundle = this.GetBundle (bundle_name, level, culture);
			
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
			culture = culture ?? this.culture;
			
			string bundle_name;
			string field_name;

			if (Resources.SplitFieldId (id, out bundle_name, out field_name))
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
			
			ResourceModuleInfo module;
			IResourceProvider provider = this.FindProvider (id, out id, out module);

			string prefix = provider.Prefix;
			string key = Resources.CreateBundleKey (prefix, module.Id, id, ResourceLevel.Merged, culture);

			this.bundleCache.Remove (key);
			this.SyncBundleBindingProxies ();
		}
		
		public bool SetBinaryData(string id, ResourceLevel level, CultureInfo culture, byte[] data, ResourceSetMode mode)
		{
			culture = culture ?? this.culture;
			
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
			culture = culture ?? this.culture;

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
			ResourceModuleInfo module;
			
			IResourceProvider provider = this.FindProvider (id, out resource_id, out module);
			
			if (provider != null)
			{
				string prefix = provider.Prefix;
				string key = Resources.CreateBundleKey (prefix, module.Id, resource_id, level, culture);
				
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
			foreach (ProviderRecord provider in this.providers.Values)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Prefix '{0}' implemented by class {1}", provider.Prefix, provider.DefaultProvider.GetType ().FullName));
			}
		}

		public void ClearBundleCache()
		{
			this.bundleCache.Clear ();
			this.mergedBundlesInBundleCache = 0;
		}

		public void ClearMergedBundlesFromBundleCache()
		{
			if (this.mergedBundlesInBundleCache > 0)
			{
				List<string> clear = new List<string> ();

				foreach (KeyValuePair<string, ResourceBundle> pair in this.bundleCache)
				{
					if (pair.Value.ResourceLevel == ResourceLevel.Merged)
					{
						clear.Add (pair.Key);
					}
				}

				foreach (string key in clear)
				{
					this.bundleCache.Remove (key);
				}
				
				this.mergedBundlesInBundleCache = 0;
			}
		}

		public void TrimBindingCache()
		{
			foreach (BundleBindingProxy proxy in this.bindingProxies.Values)
			{
				proxy.TrimBindingCache ();
			}
		}

		#region Internal and Private Methods

		internal bool SetResourceBinding(ResourceBinding binding)
		{
			string resourceId = this.NormalizeFullId (binding.ResourceId);

			string bundleName;
			string fieldName;

			if ((Resources.SplitFieldId (resourceId, out bundleName, out fieldName)) &&
				(bundleName.Length > 0) &&
				(fieldName.Length > 0))
			{
				BundleBindingProxy proxy = this.GetBundleBindingProxy (bundleName);

				if (proxy != null)
				{
					binding.Source = proxy;
					binding.Path = fieldName;

					proxy.AddBinding (binding);

					return true;
				}
			}

			return false;
		}

		internal void SyncBundleBindingProxies()
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

		private BundleBindingProxy GetBundleBindingProxy(string bundleName)
		{
			//	Trouve le proxy qui liste les bindings pour le bundle spécifié.

			//	La liste des proxies est gérée par le gestionnaire de ressources
			//	plutôt que le bundle lui-même, car il faut pouvoir passer d'une
			//	culture active à une autre sans perdre les informations de
			//	binding.

			BundleBindingProxy proxy;

			string prefix;
			string localId;
			ResourceModuleInfo module;

			this.AnalyseFullId (bundleName, out prefix, out localId, out module);

			string key = Resources.CreateBundleKey (prefix, module.Id, localId, ResourceLevel.Merged, this.culture);

			if (this.bindingProxies.TryGetValue (key, out proxy) == false)
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

				this.bindingProxies[key] = proxy;
			}

			return proxy;
		}

		private void SelectLocale(CultureInfo culture)
		{
			if (!Resources.EqualCultures (this.culture, culture))
			{
				this.culture = culture;
				this.SyncBundleBindingProxies ();
			}
		}

		private IResourceProvider FindProvider(string fullId, out string localId)
		{
			ResourceModuleInfo module;
			return this.FindProvider (fullId, out localId, out module);
		}
		
		private IResourceProvider FindProvider(string fullId, out string localId, out ResourceModuleInfo module)
		{
			string prefix;

			this.AnalyseFullId (fullId, out prefix, out localId, out module);

			if (string.IsNullOrEmpty (localId))
			{
				return null;
			}
			
			return this.FindProviderFromPrefix (prefix, ref module);
		}

		private IResourceProvider FindProviderFromPrefix(string prefix, ref ResourceModuleInfo module)
		{
			System.Diagnostics.Debug.Assert (this.defaultModuleId != -1);
			System.Diagnostics.Debug.Assert (this.defaultModuleName != null);
			
			if (!string.IsNullOrEmpty (prefix))
			{
				ProviderRecord provider;

				if (this.providers.TryGetValue (prefix, out provider))
				{
					if ((string.IsNullOrEmpty (module.Name)) &&
						(module.Id < 0))
					{
						module = new ResourceModuleInfo (this.defaultModuleName, this.defaultModuleId);
						return provider.ActiveProvider;
					}

					ModuleRecord record = null;
					int moduleId = module.Id;

					if (moduleId < 0)
					{
						record = provider.GetModuleRecord (module.Name);
					}
					else
					{
						record = provider.GetModuleRecord (moduleId);
					}

					if (record != null)
					{
						module = record.ModuleInfo;
						return record.Provider;
					}
				}
			}

			return null;
		}

		#endregion

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

		#region ProviderRecord Class

		private class ProviderRecord
		{
			public ProviderRecord(ResourceManager manager, ResourceProviderFactory.Allocator allocator)
			{
				this.manager = manager;
				this.allocator = allocator;
				this.defaultProvider = this.allocator (this.manager);
				this.activeProvider = null;
				this.prefix = this.defaultProvider.Prefix;
				this.modules = new List<ModuleRecord> ();
			}

			public string Prefix
			{
				get
				{
					return this.prefix;
				}
			}

			public IResourceProvider DefaultProvider
			{
				get
				{
					return this.defaultProvider;
				}
			}

			public IResourceProvider ActiveProvider
			{
				get
				{
					return this.activeProvider;
				}
				set
				{
					this.activeProvider = value;
				}
			}

			public IEnumerable<ResourceModuleInfo> Modules
			{
				get
				{
					if (this.infos == null)
					{
						this.infos = this.defaultProvider.GetModules ();
					}
					
					return this.infos;
				}
			}

			
			public void ClearModuleCache()
			{
				this.infos = null;
			}

			public ModuleRecord GetModuleRecord(string moduleName)
			{
				foreach (ModuleRecord item in this.modules)
				{
					if (item.ModuleInfo.Name == moduleName)
					{
						return item;
					}
				}

				ResourceModuleInfo moduleInfo = new ResourceModuleInfo (moduleName);

				if (this.defaultProvider.SelectModule (ref moduleInfo))
				{
					ModuleRecord record = new ModuleRecord (this.defaultProvider, moduleInfo);
					this.modules.Add (record);
					this.defaultProvider = this.allocator (this.manager);
					return record;
				}
				
				return null;
			}

			public ModuleRecord GetModuleRecord(int moduleId)
			{
				foreach (ModuleRecord item in this.modules)
				{
					if (item.ModuleInfo.Id == moduleId)
					{
						return item;
					}
				}

				ResourceModuleInfo moduleInfo = new ResourceModuleInfo (moduleId);

				if (this.defaultProvider.SelectModule (ref moduleInfo))
				{
					ModuleRecord record = new ModuleRecord (this.defaultProvider, moduleInfo);
					this.modules.Add (record);
					this.defaultProvider = this.allocator (this.manager);
					return record;
				}

				return null;
			}

			private ResourceManager manager;
			private ResourceProviderFactory.Allocator allocator;
			private IResourceProvider defaultProvider;
			private IResourceProvider activeProvider;
			private string prefix;
			private List<ModuleRecord> modules;
			private ResourceModuleInfo[] infos;
		}

		#endregion

		#region ModuleRecord Class

		private class ModuleRecord
		{
			public ModuleRecord(IResourceProvider provider, ResourceModuleInfo moduleInfo)
			{
				this.provider = provider;
				this.moduleInfo = moduleInfo;
			}

			public IResourceProvider Provider
			{
				get
				{
					return this.provider;
				}
			}

			public ResourceModuleInfo ModuleInfo
			{
				get
				{
					return this.moduleInfo;
				}
			}

			private IResourceProvider provider;
			private ResourceModuleInfo moduleInfo;
		}

		#endregion

		private Dictionary<string, ProviderRecord> providers;
		private CultureInfo						culture;
		private string							defaultModuleName;
		private int								defaultModuleId;
		private string							defaultPrefix = "file";
		private string							defaultPath;
		
		Dictionary<string, ResourceBundle>		bundleCache = new Dictionary<string, ResourceBundle> ();
		Dictionary<string, BundleBindingProxy>	bindingProxies = new Dictionary<string, BundleBindingProxy> ();
		
		private int								mergedBundlesInBundleCache;
	}
}
