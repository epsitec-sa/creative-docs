//	Copyright © 2004-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Globalization;
using System.Collections.Generic;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

[assembly: DependencyClass (typeof (ResourceManager))]

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
	public sealed class ResourceManager : DependencyObject, System.IComparable<ResourceManager>, System.IEquatable<ResourceManager>, IStructuredTypeResolver, ICaptionResolver
	{
		public ResourceManager()
			: this (null, Support.Globals.Directories.ExecutableRoot, null)
		{
		}

		public ResourceManager(System.Type type)
			: this (null, System.IO.Path.GetDirectoryName (type.Assembly.Location), null)
		{
		}

		public ResourceManager(string path)
			: this (null, path, null)
		{
		}

		public ResourceManager(ResourceManagerPool pool)
			: this (pool, Support.Globals.Directories.ExecutableRoot, null)
		{
		}

		public ResourceManager(ResourceManagerPool pool, ResourceModuleInfo info)
			: this (pool, Support.Globals.Directories.ExecutableRoot, info == null ? null : info.FullId.Path)
		{
			if (info != null)
			{
				this.defaultModuleId = info.FullId.Id;
				this.defaultModuleName = info.FullId.Name;
			}
		}

		public ResourceManager(ResourceManagerPool pool, ResourceModuleId module)
			: this (pool, Support.Globals.Directories.ExecutableRoot, module.Path)
		{
			this.defaultModuleId   = module.Id;
			this.defaultModuleName = module.Name;
		}

		public ResourceManager(ResourceManagerPool pool, System.Type type)
			: this (pool, System.IO.Path.GetDirectoryName (type.Assembly.Location), null)
		{
		}

		public ResourceManager(ResourceManagerPool pool, string executablePath, string modulePath)
		{
			this.pool = pool ?? ResourceManagerPool.Default ?? new ResourceManagerPool ();
			this.bundleRelatedCache = new Dictionary<string, BundleRelatedCache> ();
			this.captionCache = new Dictionary<string, Weak<Caption>> ();
			this.defaultModulePath = modulePath;

			this.serialId = System.Threading.Interlocked.Increment (ref ResourceManager.nextSerialId);
			this.providers = new Dictionary<string, ProviderRecord> ();
			this.culture = CultureInfo.CurrentCulture;
			this.defaultPath = string.IsNullOrEmpty (executablePath) ? null : executablePath;

			foreach (Allocator<IResourceProvider, ResourceManager> allocator in Resources.Factory.Allocators)
			{
				ProviderRecord record = new ProviderRecord (this, allocator);
				this.providers.Add (record.Prefix, record);
			}
			
			this.pool.Register (this);
		}
		

		internal long							SerialId
		{
			get
			{
				return this.serialId;
			}
		}

		/// <summary>
		/// Gets the probing paths where the Resource directory could be found.
		/// </summary>
		/// <value>The resource probing paths.</value>
		public IEnumerable<string>				ResourceProbingPaths
		{
			get
			{
				if (this.defaultPath != null)
				{
					yield return this.defaultPath;
				}
				
				if (this.pool != null)
				{
					foreach (var path in this.pool.ResourceProbingPaths)
					{
						yield return path;
					}
				}
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
		
		public string							LocalizedSuffix
		{
			get
			{
				return this.MapToSuffix (ResourceLevel.Localized, this.culture);
			}
		}
		
		public string							CustomizedSuffix
		{
			get
			{
				return this.MapToSuffix (ResourceLevel.Localized, this.culture);
			}
		}
		
		public string							DefaultPath
		{
			get
			{
				return this.defaultPath;
			}
		}

		public int								DefaultModuleId
		{
			get
			{
				return this.defaultModuleId;
			}
		}

		public string							DefaultModulePath
		{
			get
			{
				return this.defaultModulePath;
			}
		}

		public string							DefaultModuleName
		{
			get
			{
				return this.defaultModuleName;
			}
		}

		public ResourceModuleInfo				DefaultModuleInfo
		{
			get
			{
				System.Diagnostics.Debug.Assert (this.pool != null);

				if (string.IsNullOrEmpty (this.defaultModulePath))
				{
					return null;
				}
				else
				{
					return this.pool.GetModuleInfo (this.defaultModulePath);
				}
			}
		}
		
		public bool								IsReady
		{
			get
			{
				return (this.defaultModuleName != null) && (this.defaultModuleId >= 0);
			}
		}

		public System.Func<int, ResourceManager> MissingModuleResolver
		{
			get;
			set;
		}

		public ResourceManagerPool				Pool
		{
			get
			{
				return this.pool;
			}
		}

		public bool								BasedOnPatchModule
		{
			get
			{
				ResourceModuleInfo info = this.DefaultModuleInfo;

				if (info == null)
				{
					return false;
				}
				else
				{
					return info.IsPatchModule;
				}
			}
		}

		public int								PatchDepth
		{
			get
			{
				if (this.BasedOnPatchModule)
				{
					ResourceManager parent = this.GetManagerForReferenceModule ();
					return parent.PatchDepth + 1;
				}
				else
				{
					ResourceModuleInfo info = this.DefaultModuleInfo;

					if (info == null)
					{
						return 0;
					}
					else
					{
						return info.PatchDepth;
					}
				}
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
					if (this.defaultModulePath == null)
					{
						this.defaultModulePath = module.ModuleInfo.Path;
					}
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

		public ResourceManager GetManagerForReferenceModule()
		{
			if (this.ContainsValue (ResourceManager.ReferenceManagerProperty))
			{
				return this.GetValue (ResourceManager.ReferenceManagerProperty) as ResourceManager;
			}

			ResourceModuleInfo info = this.DefaultModuleInfo;
			ResourceManager manager = null;

			if ((info != null) &&
				(info.IsPatchModule))
			{
				string modulePath = this.pool.GetRootRelativePath (info, info.ReferenceModulePath);
				info = this.pool.GetModuleInfo (modulePath);

				if (info != null)
				{
					manager = new ResourceManager (this.pool, info);
				}
			}

			this.SetValue (ResourceManager.ReferenceManagerProperty, manager);

			return manager;
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

		public ResourceModuleId GetModuleFromFullId(string id)
		{
			string prefix;
			string localId;
			ResourceModuleId module;

			this.AnalyzeFullId (id, out prefix, out localId, out module);

			return module;
		}

		public void AnalyzeFullId(string id, out string prefix, out string localId, out ResourceModuleId module)
		{
			if (!string.IsNullOrEmpty (id))
			{
				string moduleName;

				Resources.SplitFullId (id, out prefix, out localId);
				Resources.ResolveStringsIdReference (ref prefix, ref localId);
				Resources.ResolveBundleIdReference (ref prefix, ref localId);
				Resources.SplitFullPrefix (prefix, out prefix, out moduleName);

				if (string.IsNullOrEmpty (prefix))
				{
					prefix = this.ActivePrefix;
				}
				if (string.IsNullOrEmpty (moduleName))
				{
					module = new ResourceModuleId (this.defaultModuleName, this.defaultModulePath, this.defaultModuleId);
				}
				else
				{
					module = ResourceModuleId.Parse (moduleName);
					
					if (module.Id == this.defaultModuleId)
					{
						module = new ResourceModuleId (this.defaultModuleName, this.defaultModulePath, this.defaultModuleId);
					}
				}
			}
			else
			{
				prefix  = null;
				localId = null;
				module  = new ResourceModuleId ();
			}
		}

		public string NormalizeFullId(string id)
		{
			string prefix;
			string localId;

			Resources.SplitFullId (id, out prefix, out localId);
			Resources.ResolveStringsIdReference (ref prefix, ref localId);
			Resources.ResolveBundleIdReference (ref prefix, ref localId);

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

			ResourceModuleId module = ResourceModuleId.Parse (moduleName);

			this.NormalizeModule (prefix, ref module);
			prefix = Resources.JoinFullPrefix (prefix, module.ToString ());

			return Resources.JoinFullId (prefix, localId);
		}

		public void NormalizeModule(string prefix, ref ResourceModuleId module)
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
				case ResourceLevel.Default:		return Resources.DefaultTwoLetterISOLanguageName;
				case ResourceLevel.Localized:	return Resources.GetTwoLetterISOLanguageName (culture);
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
			foreach (ResourceModuleId info in this.GetModuleInfos (prefix))
			{
				yield return info.Name;
			}
		}

		public IEnumerable<ResourceModuleId> GetModuleInfos(string prefix)
		{
			ProviderRecord provider;

			if (this.providers.TryGetValue (prefix, out provider))
			{
				return provider.Modules;
			}
			else
			{
				return new ResourceModuleId[0];
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

		public string[] GetBundleIds(string nameFilter)
		{
			return this.GetBundleIds (nameFilter, null, ResourceLevel.Default, this.culture);
		}
		
		public string[] GetBundleIds(string nameFilter, string typeFilter)
		{
			return this.GetBundleIds (nameFilter, typeFilter, ResourceLevel.Default, this.culture);
		}
		
		public string[] GetBundleIds(string nameFilter, ResourceLevel level)
		{
			return this.GetBundleIds (nameFilter, null, level, this.culture);
		}
		
		public string[] GetBundleIds(string nameFilter, string typeFilter, ResourceLevel level)
		{
			return this.GetBundleIds (nameFilter, typeFilter, level, this.culture);
		}
		
		public string[] GetBundleIds(string nameFilter, ResourceLevel level, CultureInfo culture)
		{
			return this.GetBundleIds (nameFilter, null, level, culture);
		}
		
		public string[] GetBundleIds(string nameFilter, string typeFilter, ResourceLevel level, CultureInfo culture)
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
			
			string nameFilterId;
			
			IResourceProvider provider = this.FindProvider (nameFilter, out nameFilterId);
			
			if (provider != null)
			{
				return provider.GetIds (nameFilterId, typeFilter, level, culture);
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
			ResourceModuleId module;
			
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
				string key = Resources.CreateBundleKey (prefix, module, resourceId, level, culture, this.GetKeySuffix ());
				CultureInfo defaultCulture = Resources.FindCultureInfo (Resources.GetDefaultTwoLetterISOLanguageName ());

				bundle = this.pool.FindBundle (key);

				if (bundle != null)
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

							if (defaultCulture != null)
							{
								this.CompileBundle (bundle, provider, resourceId, ResourceLevel.Localized, defaultCulture);
							}

							if (defaultCulture != culture)
							{
								this.CompileBundle (bundle, provider, resourceId, ResourceLevel.Localized, culture);
							}

							this.CompileBundle (bundle, provider, resourceId, ResourceLevel.Customized, culture);
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

					this.pool.AddBundle (key, bundle);
				}
			}
			else if (recursion == 0)
			{
				foreach (ResourceManager manager in this.pool.Managers)
				{
					if (manager.DefaultModuleId == module.Id)
					{
						return manager.GetBundle (id, level, culture, recursion+1);
					}
				}

				if (this.MissingModuleResolver != null)
				{
					var manager = this.MissingModuleResolver (module.Id);

					if (manager != null)
					{
						return manager.GetBundle (id, level, culture, recursion+1);
					}
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
			byte[] data;
			string key = Resources.CreateBundleKey (provider.Prefix, bundle.Module, resourceId, level, culture, this.GetKeySuffix ());
			ResourceBundle source = this.pool.FindBundle (key);

			if (source != null)
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
		public ResourceBundle.Field GetStringsBundleField(Druid druid)
		{
			return this.GetStringsBundleField (druid, ResourceLevel.Merged, this.culture);
		}
		
		public ResourceBundle.Field GetStringsBundleField(Druid druid, ResourceLevel level)
		{
			return this.GetStringsBundleField (druid, level, this.culture);
		}
		
		public ResourceBundle.Field GetStringsBundleField(Druid druid, ResourceLevel level, CultureInfo culture)
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
			
			string resourceId;
			byte[] data = null;
			
			IResourceProvider provider = this.FindProvider (id, out resourceId);
			CultureInfo defaultCulture = Resources.FindCultureInfo (Resources.GetDefaultTwoLetterISOLanguageName ());
			
			if (provider != null)
			{
				switch (level)
				{
					case ResourceLevel.Merged:
						data = provider.GetData (resourceId, ResourceLevel.Customized, culture);
						
						if (data != null)
						{
							break;
						}

						data = provider.GetData (resourceId, ResourceLevel.Localized, culture);
						
						if (data != null)
						{
							break;
						}

						if (defaultCulture != null)
						{
							data = provider.GetData (resourceId, ResourceLevel.Localized, defaultCulture);
							
							if (data != null)
							{
								break;
							}
						}

						data = provider.GetData (resourceId, ResourceLevel.Default, culture);
						break;
					
					case ResourceLevel.Default:
					case ResourceLevel.Localized:
					case ResourceLevel.Customized:
						data = provider.GetData (resourceId, level, culture);
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
			return this.GetCaption (druid, ResourceLevel.Merged, this.culture, cache: true);
		}

		public Caption GetCaption(Druid druid, ResourceLevel level)
		{
			return this.GetCaption (druid, level, this.culture, cache: true);
		}
		
		public Caption GetCaption(Druid druid, ResourceLevel level, CultureInfo culture)
		{
			return this.GetCaption (druid, level, culture, cache: true);
		}

		public Caption GetCaption(Caption caption, CultureInfo culture)
		{
			if (caption == null)
			{
				return null;
			}
			else
			{
				return this.GetCaption (caption.Id, ResourceLevel.Merged, culture);
			}
		}

		#region IStructuredTypeResolver Members

		public StructuredType GetStructuredType(Druid id)
		{
			Caption caption = this.GetCaption (id);
			
			if (caption == null)
			{
				return null;
			}
			else
			{
				return TypeRosetta.GetTypeObject (caption) as StructuredType;
			}
		}

		#endregion

		/// <summary>
		/// Clears the caption cache for the specified caption.
		/// </summary>
		/// <param name="druid">The DRUID of the caption.</param>
		/// <param name="level">The resource level.</param>
		/// <param name="culture">The culture.</param>
		/// <returns><c>true</c> if a value was removed from the cache; otherwise, <c>false</c>.</returns>
		public bool ClearCaptionCache(Druid druid, ResourceLevel level, CultureInfo culture)
		{
			culture = culture ?? this.culture;

			string fieldName  = druid.ToFieldName ();
			string bundleName = Resources.ResolveCaptionsIdReference (this.ActivePrefix, druid);
			
			bool removed = false;

			switch (level)
			{
				case ResourceLevel.Localized:
				case ResourceLevel.Customized:
				case ResourceLevel.Default:
					removed |= this.captionCache.Remove (Resources.CreateCaptionKey (druid, ResourceLevel.Merged, culture));
					removed |= this.captionCache.Remove (Resources.CreateCaptionKey (druid, level, culture));
					break;

				case ResourceLevel.All:
					removed |= this.captionCache.Remove (Resources.CreateCaptionKey (druid, ResourceLevel.Default, culture));
					removed |= this.captionCache.Remove (Resources.CreateCaptionKey (druid, ResourceLevel.Localized, culture));
					removed |= this.captionCache.Remove (Resources.CreateCaptionKey (druid, ResourceLevel.Customized, culture));
					removed |= this.captionCache.Remove (Resources.CreateCaptionKey (druid, ResourceLevel.Merged, culture));
					break;

				default:
					removed |= this.captionCache.Remove (Resources.CreateCaptionKey (druid, level, culture));
					break;
			}

			return removed;
		}
		
		
		private Caption GetCaption(Druid druid, ResourceLevel level, CultureInfo culture, bool cache)
		{
			if (druid.IsEmpty)
			{
				return null;
			}
			if (druid.IsTemporary)
			{
				throw new System.ArgumentException ("Temporary DRUIDs not allowed in GetCaption");
			}

			culture = culture ?? this.culture;

			string prefix = this.ActivePrefix;

			if (string.IsNullOrEmpty (prefix))
			{
				throw new ResourceException ("Cannot normalize resource name; no prefix available");
			}

			string fieldName  = druid.ToFieldName ();
			string bundleName = Resources.ResolveCaptionsIdReference (prefix, druid);

			Caption caption = null;
			
			string key = Resources.CreateCaptionKey (druid, level, culture);
			Weak<Caption> weakCaption;

			if ((cache) &&
				(this.captionCache.TryGetValue (key, out weakCaption)))
			{
				caption = weakCaption.Target;
			}
			
			if (caption == null)
			{
				CultureInfo defaultCulture = Resources.FindCultureInfo (Resources.GetDefaultTwoLetterISOLanguageName ());

				switch (level)
				{
					case ResourceLevel.Merged:
						this.MergeWithCaption (this.GetBundle (bundleName, ResourceLevel.Default, culture), druid, ref caption);

						if (defaultCulture != null)
						{
							this.MergeWithCaption (this.GetBundle (bundleName, ResourceLevel.Localized, defaultCulture), druid, ref caption);
						}

						if (defaultCulture != culture)
						{
							this.MergeWithCaption (this.GetBundle (bundleName, ResourceLevel.Localized, culture), druid, ref caption);
						}
						
						this.MergeWithCaption (this.GetBundle (bundleName, ResourceLevel.Customized, culture), druid, ref caption);

						if (caption != null)
						{
							this.ResolveDruidReferencesInCaption (caption, ResourceLevel.Merged, culture);
						}
						break;

					case ResourceLevel.Default:
					case ResourceLevel.Localized:
					case ResourceLevel.Customized:
						this.MergeWithCaption (this.GetBundle (bundleName, level, culture), druid, ref caption);
						break;

					default:
						throw new ResourceException ("Invalid resource level");
				}
				
				if (caption != null)
				{
					caption.DefineId (druid);

					if (cache)
					{
						this.captionCache[key] = new Weak<Caption> (caption);
						this.GetBundleRelatedCache (bundleName, level, culture).AddCaption (caption);
					}
				}
			}

			return caption;
		}

		private void ResolveDruidReferencesInCaption(Caption caption, ResourceLevel level, CultureInfo culture)
		{
			TransformCallback<string> transform = delegate (string value) { return this.ResolveDruidReferenceInText (value, level, culture); };

			caption.TransformTexts (transform);
		}

		private string ResolveDruidReferenceInText(string value, ResourceLevel level, CultureInfo culture)
		{
			Druid druid;
			
			if (Druid.TryParse (value, out druid))
			{
				value = this.GetText (druid, level, culture);
			}
			else
			{
				value = Druid.Unescape (value);
			}
			
			return value;
		}

		private void MergeWithCaption(ResourceBundle bundle, Druid druid, ref Caption caption)
		{
			if (bundle != null)
			{
				ResourceBundle.Field field = bundle[druid];
				
				string source = field.AsString;
				
				if (string.IsNullOrEmpty (source))
				{
					return;
				}

				Caption temp = new Caption ();
				temp.DefineId (druid);
				temp.DeserializeFromString (source, this);
				
				caption = (caption == null) ? temp : Caption.Merge (caption, temp, true);

				System.Diagnostics.Debug.Assert (caption.Id.IsValid);
				System.Diagnostics.Debug.Assert (AbstractType.CheckComplexTypeBindingToCaption (caption));
				
				if (caption.ContainsLocalValue (ResourceManager.SourceBundleProperty) == false)
				{
					ResourceManager.SetSourceBundle (caption, bundle);
				}
			}
		}

		/// <summary>
		/// Gets the text based on the DRUID. The bundle used for this lookup
		/// is always the "Strings" bundle and the DRUID is used to find the
		/// matching field inside that bundle.
		/// </summary>
		/// <param name="druid">The druid of the field.</param>
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
			if (druid.IsEmpty)
			{
				return null;
			}

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
			string bundleName;
			string fieldName;

			if (Resources.SplitFieldId (id, out bundleName, out fieldName))
			{
				ResourceBundle bundle = this.GetBundle (bundleName, level, culture);
			
				if (bundle != null)
				{
					ResourceBundle.Field field = bundle[fieldName];
					
					if (field != null)
					{
						string text = field.AsString;

						if (text == ResourceBundle.Field.Null)
						{
							return null;
						}
						else
						{
							return text;
						}
					}
				}
			}
			
			return null;
		}
		
		public object GetData(string id, ResourceLevel level, CultureInfo culture)
		{
			if (string.IsNullOrEmpty (id))
			{
				return null;
			}

			culture = culture ?? this.culture;
			
			string bundleName;
			string fieldName;

			if (Resources.SplitFieldId (id, out bundleName, out fieldName))
			{
				ResourceBundle bundle = this.GetBundle (bundleName, level, culture);
			
				if (bundle != null)
				{
					ResourceBundle.Field field = bundle[fieldName];
					
					if (field != null)
					{
						return field.Data;
					}
				}
			}
			
			return null;
		}

		public void ReplaceSetBundle(SetBundleCallback callback)
		{
			this.setBundleCallback = callback;
		}

		public void SetBundle(ResourceBundle bundle, ResourceSetMode mode)
		{
			if ((this.setBundleCallback == null) ||
				(this.setBundleCallback (bundle, mode) == SetBundleOperation.Execute))
			{
				ResourceLevel level   = bundle.ResourceLevel;
				CultureInfo culture = bundle.Culture;
				string id      = bundle.PrefixedName;
				byte[] data    = bundle.CreateXmlAsData ();

				if (mode == ResourceSetMode.Remove)
				{
					this.RemoveBundle (id, level, culture, bundle.Module);
					return;
				}

				if (this.SetBinaryData (id, level, culture, data, mode, bundle.Module) == false)
				{
					throw new ResourceException (string.Format ("Could not store bundle '{0}'.", id));
				}

				ResourceModuleId module;
				IResourceProvider provider = this.FindProvider (id, out id, out module);

				string keySuffix = this.GetKeySuffix ();
				string prefix = provider.Prefix;
				string key = Resources.CreateBundleKey (prefix, module, id, ResourceLevel.Merged, culture, keySuffix);

				this.pool.RemoveBundle (key);

				key = Resources.CreateBundleKey (prefix, module, id, level, culture, keySuffix);
				this.pool.RefreshBundle (key, bundle);
			}
		}

		private string GetKeySuffix()
		{
			if (this.BasedOnPatchModule)
			{
				return this.serialId.ToString (System.Globalization.CultureInfo.InvariantCulture);
			}
			else
			{
				return null;
			}
		}
		
		public bool SetBinaryData(string id, ResourceLevel level, CultureInfo culture, byte[] data, ResourceSetMode mode, ResourceModuleId moduleId)
		{
			if ((mode == ResourceSetMode.None) ||
				(mode == ResourceSetMode.InMemory))
			{
				return true;
			}

			if (mode == ResourceSetMode.Remove)
			{
				return this.RemoveBundle (id, level, culture, moduleId);
			}
			
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
			
			string resourceId;
			
			IResourceProvider provider = this.FindProvider (id, out resourceId);

			if (provider != null)
			{
				if (!moduleId.IsEmpty)
				{
					provider.SelectModule (ref moduleId);
				}
				
				if (provider.SetData (resourceId, level, culture, data, mode))
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		private bool RemoveBundle(string id, ResourceLevel level, CultureInfo culture, ResourceModuleId moduleId)
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
			
			string resourceId;
			ResourceModuleId module;
			
			IResourceProvider provider = this.FindProvider (id, out resourceId, out module);
			
			if (provider != null)
			{
				string prefix = provider.Prefix;
				string key = Resources.CreateBundleKey (prefix, module, resourceId, level, culture, this.GetKeySuffix ());

				this.pool.RemoveBundle (key);
				
				if (!moduleId.IsEmpty)
				{
					provider.SelectModule (ref moduleId);
				}
				
				if (provider.Remove (resourceId, level, culture))
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
			this.pool.Clear ();
		}

		public void ClearMergedBundlesFromBundleCache()
		{
			this.pool.ClearMergedBundles ();
		}

		public void TrimCache()
		{
			foreach (BundleRelatedCache proxy in this.bundleRelatedCache.Values)
			{
				proxy.TrimCache ();
			}
		}

		#region IComparable<ResourceManager> Members

		int System.IComparable<ResourceManager>.CompareTo(ResourceManager other)
		{
			if (this.serialId < other.serialId)
			{
				return -1;
			}
			else if (this.serialId > other.serialId)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		#endregion

		#region IEquatable<ResourceManager> Members

		bool System.IEquatable<ResourceManager>.Equals(ResourceManager other)
		{
			return this.serialId == other.serialId;
		}

		#endregion

		public override int GetHashCode()
		{
			return (int) this.serialId;
		}

		public override bool Equals(object obj)
		{
			ResourceManager other = obj as ResourceManager;

			if (object.ReferenceEquals (other, null))
			{
				return false;
			}
			else
			{
				return this.serialId == other.serialId;
			}
		}

		public override string ToString()
		{
			return string.Format ("[ResMan lang={1}/mod={2} ({3})/serial={0}]", this.serialId, this.culture == null ? "--" : this.culture.TwoLetterISOLanguageName, this.defaultModuleName ?? "*", this.defaultModuleId);
		}

		#region Internal and Private Methods

		internal void SetPool(ResourceManagerPool pool)
		{
			this.pool = pool;
		}

		internal bool SetResourceBinding(ResourceBinding binding)
		{
			string resourceId = this.NormalizeFullId (binding.ResourceId);

			string bundleName;
			string fieldName;

			if ((Resources.SplitFieldId (resourceId, out bundleName, out fieldName)) &&
				(bundleName.Length > 0) &&
				(fieldName.Length > 0))
			{
				BundleRelatedCache cache = this.GetBundleRelatedCache (bundleName, ResourceLevel.Merged, this.culture);

				if (cache != null)
				{
					binding.Source = cache;
					binding.Path = fieldName;

					cache.AddBinding (binding);

					return true;
				}
			}

			return false;
		}

		public int DebugCountLiveBindings()
		{
			int count = 0;

			foreach (BundleRelatedCache cache in this.bundleRelatedCache.Values)
			{
				count += cache.CountLiveBindings ();
			}

			return count;
		}

		public int DebugCountLiveCaptions()
		{
			int count = 0;

			foreach (BundleRelatedCache cache in this.bundleRelatedCache.Values)
			{
				count += cache.CountLiveCaptions ();
			}

			return count;
		}

		internal void SyncBundleRelatedCache()
		{
			//	Met à jour tous les proxies en les synchronisant avec la culture
			//	active.

			Dictionary<string, BundleRelatedCache> update = new Dictionary<string, BundleRelatedCache> ();
			BundleRelatedCache[] cacheArray = new BundleRelatedCache[this.bundleRelatedCache.Count];

			this.bundleRelatedCache.Values.CopyTo (cacheArray, 0);

			foreach (BundleRelatedCache cache in cacheArray)
			{
				string name = cache.Bundle.PrefixedName;

				ResourceLevel level = cache.Bundle.ResourceLevel;
				CultureInfo culture = cache.Bundle.Culture;

				System.Diagnostics.Debug.Assert (level != ResourceLevel.None);
				System.Diagnostics.Debug.Assert (culture != null);

				if (level == ResourceLevel.Merged)
				{
					culture = this.culture;
				}
				
				string prefix;
				string localId;
				ResourceModuleId module;

				this.AnalyzeFullId (name, out prefix, out localId, out module);

				string key = Resources.CreateBundleKey (prefix, module, localId, level, culture, this.GetKeySuffix ());

				ResourceBundle oldBundle = cache.Bundle;
				ResourceBundle newBundle = this.GetBundle (name, level, culture);

				cache.SwitchToBundle (newBundle, this);

				update[key] = cache;
			}

			this.bundleRelatedCache = update;
		}

		private BundleRelatedCache GetBundleRelatedCache(string bundleName, ResourceLevel level, CultureInfo culture)
		{
			//	Trouve le cache qui liste les bindings et les captions pour le
			//	bundle spécifié.

			//	Ces listes sont gérées par le gestionnaire de ressources plutôt
			//	que par le bundle lui-même, car il faut pouvoir passer d'une
			//	culture active à une autre sans perdre les informations de
			//	binding, ni les informations de captions.

			BundleRelatedCache cache;

			string prefix;
			string localId;
			ResourceModuleId module;

			this.AnalyzeFullId (bundleName, out prefix, out localId, out module);

			string key = Resources.CreateBundleKey (prefix, module, localId, level, culture, this.GetKeySuffix ());

			if (this.bundleRelatedCache.TryGetValue (key, out cache) == false)
			{
				//	Le cache pour le bundle en question n'existe pas. Il faut
				//	donc le créer :

				ResourceBundle bundle = this.GetBundle (bundleName, level, culture, 0);

				if (bundle == null)
				{
					return null;
				}

				cache = new BundleRelatedCache (bundle);

				System.Diagnostics.Debug.Assert (cache.Bundle.PrefixedName == bundleName);
				System.Diagnostics.Debug.Assert (cache.Bundle.ResourceLevel == level);
				System.Diagnostics.Debug.Assert (level == ResourceLevel.Default || Resources.EqualCultures (cache.Bundle.Culture, culture));

				this.bundleRelatedCache[key] = cache;
			}

			return cache;
		}

		private void SelectLocale(CultureInfo culture)
		{
			if (!Resources.EqualCultures (this.culture, culture))
			{
				this.culture = culture;
				this.SyncBundleRelatedCache ();
			}
		}

		private IResourceProvider FindProvider(string fullId, out string localId)
		{
			ResourceModuleId module;
			return this.FindProvider (fullId, out localId, out module);
		}
		
		private IResourceProvider FindProvider(string fullId, out string localId, out ResourceModuleId module)
		{
			string prefix;

			this.AnalyzeFullId (fullId, out prefix, out localId, out module);

			if (string.IsNullOrEmpty (localId))
			{
				return null;
			}
			
			return this.FindProviderFromPrefix (prefix, ref module);
		}

		private IResourceProvider FindProviderFromPrefix(string prefix, ref ResourceModuleId module)
		{
			if (!string.IsNullOrEmpty (prefix))
			{
				ProviderRecord provider;

				if (this.providers.TryGetValue (prefix, out provider))
				{
					if ((string.IsNullOrEmpty (module.Name)) &&
						(module.Id < 0))
					{
						module = new ResourceModuleId (this.defaultModuleName, this.defaultModulePath, this.defaultModuleId);
						return provider.ActiveProvider;
					}

					ModuleRecord record = null;
					
					if ((module.Id < 0) &&
						(string.IsNullOrEmpty (module.Path)))
					{
						record = provider.GetModuleRecord (module.Name);
					}
					else
					{
						record = provider.GetModuleRecord (module);
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

		#region Private BundleRelatedCache Class

		private class BundleRelatedCache : Types.IResourceBoundSource
		{
			public BundleRelatedCache(ResourceBundle bundle)
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
			
			public void SwitchToBundle(ResourceBundle bundle, ResourceManager manager)
			{
				if (this.bundle != bundle)
				{
					this.bundle = bundle;
					this.SyncBindings (manager);
					this.SyncCaptions (manager);
				}
			}

			public void AddBinding(Types.Binding binding)
			{
				this.bindings.Add (new Weak<Types.Binding> (binding));
			}

			public void AddCaption(Types.Caption caption)
			{
				this.captions.Add (new Weak<Types.Caption> (caption));
			}

			public void TrimCache()
			{
				this.TrimBindingCache ();
				this.TrimCaptionCache ();
			}

			internal int CountLiveBindings()
			{
				return this.TrimCaptionCache ();
			}

			internal int CountLiveCaptions()
			{
				return this.TrimCaptionCache ();
			}

			private void SyncBindings(ResourceManager manager)
			{
				this.bindings.RemoveAll
				(
					delegate (Weak<Types.Binding> item)
					{
						Types.Binding binding = item.Target;

						if (binding == null)
						{
							return true;
						}
						else
						{
							binding.UpdateTargets (Types.BindingUpdateMode.Reset);
							return false;
						}
					}
				);
			}

			private void SyncCaptions(ResourceManager manager)
			{
				Weak<Types.Caption>[] captions = this.captions.ToArray ();

				ResourceLevel level = this.bundle.ResourceLevel;
				CultureInfo culture = this.bundle.Culture;

				for (int i = 0; i < captions.Length; i++)
				{
					Types.Caption caption = captions[i].Target;

					if (caption == null)
					{
						this.captions.Remove (captions[i]);
					}
					else
					{
						Types.Caption update = manager.GetCaption (caption.Id, level, culture, cache: false);

						if (update != null)
						{
							DependencyObject.CopyDefinedProperties (update, caption);
						}
					}
				}
			}

			private int TrimBindingCache()
			{
				List<Weak<Types.Binding>> clean = new List<Weak<Types.Binding>> ();

				foreach (Weak<Types.Binding> binding in this.bindings)
				{
					if (binding.IsAlive)
					{
						clean.Add (binding);
					}
				}

				this.bindings = clean;
				
				return this.bindings.Count;
			}

			private int TrimCaptionCache()
			{
				List<Weak<Types.Caption>> clean = new List<Weak<Types.Caption>> ();

				foreach (Weak<Types.Caption> caption in this.captions)
				{
					if (caption.IsAlive)
					{
						clean.Add (caption);
					}
				}

				this.captions = clean;
				
				return this.captions.Count;
			}

			#region IResourceBoundSource Members

			object IResourceBoundSource.GetValue(string id)
			{
				System.Diagnostics.Debug.Assert (this.bundle != null);
				System.Diagnostics.Debug.Assert (this.bundle.Contains (id));
				
				return this.bundle[id].Data;
			}

			#endregion

			ResourceBundle						bundle;
			List<Weak<Types.Binding>>			bindings = new List<Weak<Types.Binding>> ();
			List<Weak<Types.Caption>>			captions = new List<Weak<Types.Caption>> ();
		}

		#endregion

		#region ProviderRecord Class

		private class ProviderRecord
		{
			public ProviderRecord(ResourceManager manager, Allocator<IResourceProvider, ResourceManager> allocator)
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

			public IEnumerable<ResourceModuleId> Modules
			{
				get
				{
					if (this.moduleIds == null)
					{
						this.moduleIds = this.defaultProvider.GetModules ();
					}
					
					return this.moduleIds;
				}
			}

			
			public void ClearModuleCache()
			{
				this.moduleIds = null;
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

				ResourceModuleId moduleInfo = new ResourceModuleId (moduleName);

				if (this.defaultProvider.SelectModule (ref moduleInfo))
				{
					ModuleRecord record = new ModuleRecord (this.defaultProvider, moduleInfo);
					this.modules.Add (record);
					this.defaultProvider = this.allocator (this.manager);
					return record;
				}
				
				return null;
			}

			public ModuleRecord GetModuleRecord(ResourceModuleId moduleId)
			{
				foreach (ModuleRecord item in this.modules)
				{
					if (item.ModuleInfo.Id == moduleId.Id)
					{
						return item;
					}
				}

				if (this.defaultProvider.SelectModule (ref moduleId))
				{
					ModuleRecord record = new ModuleRecord (this.defaultProvider, moduleId);
					this.modules.Add (record);
					this.defaultProvider = this.allocator (this.manager);
					return record;
				}

				return null;
			}

			private ResourceManager manager;
			private Allocator<IResourceProvider, ResourceManager> allocator;
			private IResourceProvider defaultProvider;
			private IResourceProvider activeProvider;
			private string prefix;
			private List<ModuleRecord> modules;
			private ResourceModuleId[] moduleIds;
		}

		#endregion

		#region ModuleRecord Class

		private class ModuleRecord
		{
			public ModuleRecord(IResourceProvider provider, ResourceModuleId moduleInfo)
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

			public ResourceModuleId ModuleInfo
			{
				get
				{
					return this.moduleInfo;
				}
			}

			private IResourceProvider provider;
			private ResourceModuleId moduleInfo;
		}

		#endregion

		#region DependencyProperty Accessors

		public static void SetResourceManager(DependencyObject obj, ResourceManager value)
		{
			if (value == null)
			{
				obj.ClearValue (ResourceManager.ResourceManagerProperty);
			}
			else
			{
				obj.SetValue (ResourceManager.ResourceManagerProperty, value);
			}
		}

		public static ResourceManager GetResourceManager(DependencyObject obj)
		{
			return (ResourceManager) obj.GetValue (ResourceManager.ResourceManagerProperty);
		}

		public static void SetSourceBundle(DependencyObject obj, ResourceBundle value)
		{
			if (value == null)
			{
				obj.ClearValue (ResourceManager.SourceBundleProperty);
			}
			else
			{
				obj.SetValue (ResourceManager.SourceBundleProperty, value);
			}
		}

		/// <summary>
		/// Gets the source bundle associated with an object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>The source bundle or <c>null</c>.</returns>
		public static ResourceBundle GetSourceBundle(DependencyObject obj)
		{
			return (ResourceBundle) obj.GetValue (ResourceManager.SourceBundleProperty);
		}

		/// <summary>
		/// Gets the source module associated with an object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>The source module or <c>ResourceModuleId.Empty</c>.</returns>
		public static ResourceModuleId GetSourceModule(DependencyObject obj)
		{
			ResourceBundle bundle = ResourceManager.GetSourceBundle (obj);
			
			if (bundle == null)
			{
				return ResourceModuleId.Empty;
			}
			else
			{
				return bundle.Module;
			}
		}

		#endregion

		public delegate SetBundleOperation SetBundleCallback(ResourceBundle bundle, ResourceSetMode mode);
		
		public enum SetBundleOperation
		{
			Execute,
			Skip,
		}

		public static readonly DependencyProperty ResourceManagerProperty = DependencyProperty.RegisterAttached ("ResourceManager", typeof (ResourceManager), typeof (ResourceManager), new DependencyPropertyMetadata ().MakeNotSerializable ());
		public static readonly DependencyProperty SourceBundleProperty    = DependencyProperty.RegisterAttached ("SourceBundle", typeof (ResourceBundle), typeof (ResourceManager), new DependencyPropertyMetadata ().MakeNotSerializable ());
		public static readonly DependencyProperty ReferenceManagerProperty = DependencyProperty.Register ("ReferenceManager", typeof (ResourceManager), typeof (ResourceManager), new DependencyPropertyMetadata ());
		
		private static long						nextSerialId = 1;
		
		private long							serialId;
		private CultureInfo						culture;
		private string							defaultModuleName;
		private int								defaultModuleId = -1;
		private string							defaultModulePath;
		private string							defaultPrefix = "file";
		private string							defaultPath;
		
		private ResourceManagerPool				pool;
		private SetBundleCallback				setBundleCallback;
		
		Dictionary<string, ProviderRecord>		providers;
		Dictionary<string, BundleRelatedCache>	bundleRelatedCache;
		Dictionary<string, Weak<Caption>>		captionCache;
	}
}
