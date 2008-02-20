//	Copyright � 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Identity;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.CodeCompilation;

namespace Epsitec.Common.Designer
{
	using AbstractResourceAccessor=Support.ResourceAccessors.AbstractResourceAccessor;
	using AbstractCaptionResourceAccessor=Support.ResourceAccessors.AbstractCaptionResourceAccessor;
	using StructuredTypeResourceAccessor=Support.ResourceAccessors.StructuredTypeResourceAccessor;
	using AnyTypeResourceAccessor=Support.ResourceAccessors.AnyTypeResourceAccessor;
	using AbstractFileResourceAccessor=Support.ResourceAccessors.AbstractFileResourceAccessor;

	/// <summary>
	/// Description d'un module de ressources ouvert par l'application Designer.
	/// </summary>
	public class Module
	{
		public Module(DesignerApplication designerApplication, DesignerMode mode, ResourceModuleId moduleId)
			: this (designerApplication, mode, designerApplication.ResourceManagerPool, moduleId)
		{
		}

		public Module(ResourceManagerPool pool, ResourceModuleId moduleId)
			: this (null, DesignerMode.Build, pool, moduleId)
		{
		}

		private Module(DesignerApplication designerApplication, DesignerMode mode, ResourceManagerPool pool, ResourceModuleId moduleId)
		{
			System.Diagnostics.Debug.Assert (pool != null);
			
			this.designerApplication = designerApplication;
			this.mode = mode;

			this.moduleId = moduleId;
			this.batchSaver = new ResourceBundleBatchSaver ();
			this.batchSaver.BundleSaved += this.HandleBatchSaverModuleSaved;

			this.resourceManager = new ResourceManager(pool, moduleId);
			this.resourceManager.DefineDefaultModuleName(this.moduleId.Name);

			this.moduleInfo = this.resourceManager.DefaultModuleInfo.Clone ();

			this.modifier = new Modifier(this);

			this.accessStrings  = new ResourceAccess(ResourceAccess.Type.Strings,  this, this.moduleId);
			this.accessCaptions = new ResourceAccess(ResourceAccess.Type.Captions, this, this.moduleId);
			this.accessCommands = new ResourceAccess(ResourceAccess.Type.Commands, this, this.moduleId);
			this.accessPanels   = new ResourceAccess(ResourceAccess.Type.Panels,   this, this.moduleId);
			this.accessEntities = new ResourceAccess(ResourceAccess.Type.Entities, this, this.moduleId);
			this.accessTypes    = new ResourceAccess(ResourceAccess.Type.Types,    this, this.moduleId);
			this.accessForms    = new ResourceAccess(ResourceAccess.Type.Forms,    this, this.moduleId);
			this.Load();

			//	Attention: il faut avoir fait le this.accessEntities.Load() avant de cr�er this.accessFields !
			this.accessFields   = new ResourceAccess(ResourceAccess.Type.Fields,   this, this.moduleId);
			this.accessFields.Load(this);

			//	Attention: il faut avoir fait le this.accessTypes.Load() avant de cr�er this.accessValues !
			this.accessValues   = new ResourceAccess(ResourceAccess.Type.Values,   this, this.moduleId);
			this.accessValues.Load(this);

			foreach (ResourceAccess access in this.Accesses)
			{
				access.DirtyChanged += new EventHandler(this.HandleAccessDirtyChanged);
			}
		}

		public void Dispose()
		{
			foreach (ResourceAccess access in this.Accesses)
			{
				access.DirtyChanged -= new EventHandler(this.HandleAccessDirtyChanged);
			}

			this.modifier.Dispose();
		}


		public DesignerApplication DesignerApplication
		{
			get
			{
				return this.designerApplication;
			}
		}

		public DesignerMode Mode
		{
			//	Retourne le mode de fonctionnement du logiciel.
			get
			{
				return this.mode;
			}
		}

		public Modifier Modifier
		{
			get
			{
				return this.modifier;
			}
		}

		public ResourceBundleBatchSaver BatchSaver
		{
			get
			{
				return this.batchSaver;
			}
		}


		public ResourceManager ResourceManager
		{
			get
			{
				return this.resourceManager;
			}
		}

		public ResourceModuleId ModuleId
		{
			get
			{
				return this.moduleId;
			}
		}

		public ResourceModuleInfo ModuleInfo
		{
			get
			{
				return this.moduleInfo;
			}
		}


		public ResourceAccess AccessStrings
		{
			get
			{
				return this.accessStrings;
			}
		}

		public ResourceAccess AccessCaptions
		{
			get
			{
				return this.accessCaptions;
			}
		}

		public ResourceAccess AccessCommands
		{
			get
			{
				return this.accessCommands;
			}
		}

		public ResourceAccess AccessPanels
		{
			get
			{
				return this.accessPanels;
			}
		}

		public ResourceAccess AccessEntities
		{
			get
			{
				return this.accessEntities;
			}
		}

		public ResourceAccess AccessFields
		{
			get
			{
				return this.accessFields;
			}
		}

		public ResourceAccess AccessValues
		{
			get
			{
				return this.accessValues;
			}
		}

		public ResourceAccess AccessTypes
		{
			get
			{
				return this.accessTypes;
			}
		}

		public ResourceAccess AccessForms
		{
			get
			{
				return this.accessForms;
			}
		}

		public ResourceAccess GetAccess(ResourceAccess.Type type)
		{
			//	Cherche un acc�s d'apr�s son type.
			switch (type)
			{
				case ResourceAccess.Type.Strings:
					return this.accessStrings;

				case ResourceAccess.Type.Captions:
					return this.accessCaptions;

				case ResourceAccess.Type.Commands:
					return this.accessCommands;

				case ResourceAccess.Type.Panels:
					return this.accessPanels;

				case ResourceAccess.Type.Entities:
					return this.accessEntities;

				case ResourceAccess.Type.Fields:
					return this.accessFields;

				case ResourceAccess.Type.Values:
					return this.accessValues;

				case ResourceAccess.Type.Types:
					return this.accessTypes;

				case ResourceAccess.Type.Forms:
					return this.accessForms;
			}

			return null;
		}


		public bool IsEditLocked
		{
			//	Indique si les modifications sont interdites dans ce module.
			get
			{
				return this.isEditLocked;
			}
			set
			{
				this.isEditLocked = value;
			}
		}

		public bool IsPatch
		{
			//	Indique si on est dans un module de patch.
			get
			{
				ResourceModuleInfo info = this.resourceManager.DefaultModuleInfo;
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


		public bool IsGlobalDirty
		{
			get
			{
				foreach (ResourceAccess access in this.Accesses)
				{
					if (access.IsGlobalDirty)
					{
						return true;
					}
				}

				return false;
			}
		}

		public bool IsLocalDirty
		{
			get
			{
				foreach (ResourceAccess access in this.Accesses)
				{
					if (access.IsLocalDirty)
					{
						return true;
					}
				}

				return false;
			}
		}


		public ResourceAccess.Type GetCaptionType(Druid id)
		{
			//	Cherche � quel type 'caption' appartient un Druid.
			//	Il ne peut pas s'agir d'un type ResourceAccess.Type.Strings, car les Druids 'string' et 'caption'
			//	peuvent �tre identiques !
			if (this.accessCaptions.Accessor.Collection[id] != null)
			{
				return ResourceAccess.Type.Captions;
			}

			if (this.accessCommands.Accessor.Collection[id] != null)
			{
				return ResourceAccess.Type.Commands;
			}

			if (this.accessTypes.Accessor.Collection[id] != null)
			{
				return ResourceAccess.Type.Types;
			}

			if (this.accessValues.Accessor.Collection[id] != null)
			{
				return ResourceAccess.Type.Values;
			}

			if (this.accessFields.Accessor.Collection[id] != null)
			{
				return ResourceAccess.Type.Fields;
			}

			if (this.accessEntities.Accessor.Collection[id] != null)
			{
				return ResourceAccess.Type.Entities;
			}

			if (this.accessForms.Accessor.Collection[id] != null)
			{
				return ResourceAccess.Type.Forms;
			}

			return ResourceAccess.Type.Unknown;
		}


		public void Load()
		{
			//	Charge toutes les ressources.
			foreach (ResourceAccess access in this.Accesses)
			{
				if (access != null)
				{
					access.Load(this);
				}
			}
		}

		public void Save()
		{
			//	Enregistre toutes les ressources et met � jour le fichier module.info.
			this.SaveResources();
			this.UpdateManifest();
		}

		public string CheckMessage()
		{
			//	Retourne l'�ventuel rapport.
			List<ResourceAccess.ShortcutItem> list = new List<ResourceAccess.ShortcutItem>();

			//	V�rifie l'�ventuel usage � double des raccourcis clavier.
			foreach (ResourceAccess access in this.Accesses)
			{
				access.AddShortcuts(list);
			}

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			ResourceAccess.CheckShortcuts(builder, list);

			//	V�rifie les Forms.
			this.accessForms.CheckForms(builder);

			string message = builder.ToString();
			if (message.EndsWith("<br/>"))
			{
				//	Pour compenser un bug dans TextLayout. Si le texte se termine par "<br/>", le calcul
				//	de la hauteur est incorrect (la derni�re ligne n'est pas compt�e). En revanche, si
				//	le texte se termine par "<br/> ", tout semble fonctionner !
				//	TODO: corriger TextLayout et supprimer cette magouille !
				message += " ";
			}

			return message;
		}

		public void Check()
		{
			//	V�rifie toutes les ressources et affiche un rapport.
			string message = this.CheckMessage();

			if (string.IsNullOrEmpty(message))  // aucune anomalie ?
			{
				this.designerApplication.DialogMessage(Res.Strings.Error.CheckOK);
			}
			else
			{
				this.designerApplication.DialogError(message);
			}
		}

		public void RunPanel(int index)
		{
			//	Montre une ressource 'Panel' dans une fen�tre.
			string name = this.accessPanels.GetField(index, null, ResourceAccess.FieldType.Name).String;
			UI.Panel panel = this.accessPanels.GetPanel(index);

			if (panel != null)
			{
				UserInterface.RunPanel(panel, this.resourceManager, this.designerApplication.Window, name);
			}
		}

		public void RunForm(int index)
		{
			//	Montre une ressource 'Form' dans une fen�tre.
			string name = this.accessForms.GetField(index, null, ResourceAccess.FieldType.Name).String;
			Druid druid = this.accessForms.AccessDruid(index);

			FormEngine.Engine engine = new FormEngine.Engine(this.FormResourceProvider);
			UI.Panel panel = engine.CreateForm(druid);

			if (panel != null)
			{
				UserInterface.RunForm(panel, this.designerApplication.Window, new Size(800, 600), name);
			}
		}


		internal FormEngine.IFormResourceProvider FormResourceProvider
		{
			get
			{
				return new InternalFormResourceProvider(this);
			}
		}


		#region InternalFormResourceProvider Class

		/// <summary>
		/// The <c>InternalFormResourceProvider</c> class implements the <see cref="FormEngine.IFormResourceProvider"/>
		/// used by the <see cref="FormEngine.Engine"/>.
		/// </summary>
		private class InternalFormResourceProvider : FormEngine.IFormResourceProvider
		{
			public InternalFormResourceProvider(Module module)
			{
				this.module = module;
				this.typeCache = new Dictionary<Druid, INamedType>();
				this.defaultProvider = new FormEngine.DefaultResourceProvider(this.module.resourceManager);
			}

			/// <summary>
			/// Gets the two letter ISO language name for the currently active
			/// culture.
			/// </summary>
			/// <value>The two letter ISO language name.</value>
			private string TwoLetterISOLanguageName
			{
				get
				{
					return this.module.ResourceManager.ActiveCulture.TwoLetterISOLanguageName;
				}
			}
			
			#region IFormResourceProvider Members
			/// <summary>
			/// Clears the cached information.
			/// </summary>
			public void ClearCache()
			{
				this.typeCache.Clear();
				this.defaultProvider.ClearCache();
			}

			/// <summary>
			/// Gets the XML source for the specified form.
			/// </summary>
			/// <param name="formId">The form id.</param>
			/// <returns>The XML source or <c>null</c>.</returns>
			public string GetFormXmlSource(Druid formId)
			{
				Module module = this.module.designerApplication.SearchModule(formId);

				if (module == null)
				{
					//	Pas trouv� de module charg� pour le masque sp�cifi�, alors on va
					//	passer par le gestionnaire de ressources standard :
					return this.defaultProvider.GetFormXmlSource(formId);
				}

				IResourceAccessor accessor = module.accessForms.Accessor;
				CultureMap item = accessor.Collection[formId];

				if (item != null)
				{
					StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
					
					if (accessor.BasedOnPatchModule)
					{
						//	Oups: c'est un module de patch. La vue "r�elle" des donn�es est
						//	synth�tis�e par Designer, � partir du Form du module de r�f�rence
						//	et du Form du module de patch.

						string value;

						switch (item.Source)
						{
							case CultureMapSource.DynamicMerge:
								//	On va forcer la re-g�n�ration du "merge", comme �a je suis
								//	s�r que les donn�es avec lesquelles on travaille sont bien
								//	� jour, m�me si l'utilisateur a �dit� une ressource provenant
								//	d'un autre module.
								module.accessForms.FormMerge(item);
								value = data.GetValue(Support.Res.Fields.ResourceForm.XmlSourceMerge) as string;
								break;

							case CultureMapSource.PatchModule:
								value = data.GetValue(Support.Res.Fields.ResourceForm.XmlSource) as string;
								break;

							case CultureMapSource.ReferenceModule:
								value = data.GetValue(Support.Res.Fields.ResourceForm.XmlSourceAux) as string;
								break;

							default:
								throw new System.InvalidOperationException();
						}

						System.Diagnostics.Debug.Assert(value != null);

						return value;
					}
					else
					{
						//	Si c'est un module normal, pas de probl�me: on peut simplement lire
						//	le XML source tel qu'il est stock� dans les ressources :
						return data.GetValue(Support.Res.Fields.ResourceForm.XmlSource) as string;
					}
				}
				else
				{
					return null;
				}
			}
			#endregion

			#region ICaptionResolver Members
			public Caption GetCaption(Druid captionId)
			{
				Module module = this.module.designerApplication.SearchModule(captionId);

				if (module == null)
				{
					//	Pas trouv� de module charg� pour le caption sp�cifi�, alors on va
					//	passer par le gestionnaire de ressources standard :
					return this.defaultProvider.GetCaption(captionId);
				}

				//	Cherche le caption dans tous les accesseurs possibles et imaginables
				//	qui repr�sentent les donn�es sous la forme de captions :

				CultureMap item = null;
				AbstractCaptionResourceAccessor accessor = null;
				
				foreach (ResourceAccess access in module.Accesses)
				{
					accessor = access.Accessor as AbstractCaptionResourceAccessor;

					if (accessor == null)
					{
						continue;
					}

					item = accessor.Collection[captionId];

					if (item != null)
					{
						break;
					}
				}

				if (item == null)
				{
					return null;
				}
				else
				{
					Caption captionDefaultCulture = accessor.GetCaptionViewOfData(item, Resources.DefaultTwoLetterISOLanguageName);
					Caption captionCurrentCulture = accessor.GetCaptionViewOfData(item, this.TwoLetterISOLanguageName);

					return Caption.Merge(captionDefaultCulture, captionCurrentCulture);
				}
			}
			#endregion

			#region IStructuredTypeResolver Members
			/// <summary>
			/// Gets the structured type for the specified id.
			/// </summary>
			/// <param name="id">The id for the structured type.</param>
			/// <returns>The structured type or <c>null</c>.</returns>
			public StructuredType GetStructuredType(Druid id)
			{
				INamedType type;

				if (this.typeCache.TryGetValue(id, out type))
				{
					return type as StructuredType;
				}

				Module module = this.module.designerApplication.SearchModule(id);

				if (module == null)
				{
					type = this.defaultProvider.GetStructuredType(id);
				}
				else
				{
					CultureMap item = module.accessEntities.Accessor.Collection[id];

					if (item == null)
					{
						type = null;
					}
					else
					{
						StructuredTypeResourceAccessor accessor = module.accessEntities.Accessor as StructuredTypeResourceAccessor;
						string culture = Resources.DefaultTwoLetterISOLanguageName;
						type = accessor.GetStructuredTypeViewOfData(item, culture, this.GetAnyType);
					}
				}

				//	Prend note du type dans un cache interne.
				this.typeCache[id] = type;

				return type as StructuredType;
			}
			#endregion

			/// <summary>
			/// Gets the named type for the specified type id.
			/// </summary>
			/// <param name="id">The type id.</param>
			/// <returns>The named type or <c>null</c>.</returns>
			private INamedType GetAnyType(Druid id)
			{
				INamedType type;

				if (this.typeCache.TryGetValue(id, out type))
				{
					return type;
				}

				Module module = this.module.designerApplication.SearchModule(id);

				if (module == null)
				{
					//	Pas trouv� de module charg� pour le type sp�cifi�, alors on va
					//	passer par le gestionnaire de ressources standard :
					Caption caption = this.module.resourceManager.GetCaption (id);

					if (caption != null)
					{
						type = TypeRosetta.CreateTypeObject(caption, false);
					}
				}
				else
				{
					CultureMap item;

					item = module.accessTypes.Accessor.Collection[id];

					if (item != null)
					{
						AnyTypeResourceAccessor accessor = module.accessTypes.Accessor as AnyTypeResourceAccessor;
						string culture = Resources.DefaultTwoLetterISOLanguageName;
						type = accessor.GetAnyTypeViewOfData(item, culture, this.GetAnyType);
					}
					else
					{
						item = module.accessEntities.Accessor.Collection[id];

						if (item != null)
						{
							type = this.GetStructuredType(id);
						}
					}
				}

				//	Prend note du type dans un cache interne.
				this.typeCache[id] = type;

				return type;
			}

			private readonly Module module;
			private readonly Dictionary<Druid, INamedType> typeCache;
			private readonly FormEngine.DefaultResourceProvider defaultProvider;
		}

		#endregion


		internal void SetMergeMode(bool mode, ResourceManager manager)
		{
			this.isMergingModules = mode;
			this.mergedManager = manager;
			foreach (ResourceAccess access in this.Accesses)
			{
				access.Accessor.ForceModuleMerge = mode;
			}
		}

		internal void Regenerate()
		{
			if (this.accessForms.Accessor.Collection.Count > 0)
			{
				this.accessForms.FormsMerge ();
			}

			foreach (ResourceAccess access in this.Accesses)
			{
				IResourceAccessor accessor = access.Accessor;

				if (accessor != null)
				{
					access.RegenerateAllFieldsInBundle();
					accessor.PersistChanges();
				}
			}
		}

		internal void SaveResources()
		{
			//	Enregistre toutes les ressources.
			foreach (ResourceAccess access in this.Accesses)
			{
				access.Save(this.batchSaver);
			}

			//	Passe en revue les bundles tels que les accesseurs les ont
			//	pr�par�s pour les optimiser :
			foreach (ResourceBundle bundle in this.batchSaver.GetLiveBundles())
			{
				this.OptimizeBundles(bundle);
			}

			this.batchSaver.Execute();
			
			if ((this.AccessForms.AccessCount > 0) && (this.IsPatch == false))
			{
				//	Il y a des masques de saisie d�finis pour ce module; il faut donc encore
				//	g�n�rer le code C# correspondant.

				List<ResourceBundle> bundles = new List<ResourceBundle>();
				
				foreach (CultureMap item in this.AccessForms.Accessor.Collection)
				{
					bundles.Add (this.ResourceManager.GetBundle(item.Id, ResourceLevel.Default));
				}

				this.RegenerateFormsSourceCode(bundles);
			}
		}

		private void UpdateManifest()
		{
			int devId = Settings.Default.DeveloperId;
			ResourceModuleVersion version = null;

			foreach (ResourceModuleVersion item in this.moduleInfo.Versions)
			{
				if (item.DeveloperId == devId)
				{
					version = item;
					break;
				}
			}

			if (version == null)
			{
				version = new ResourceModuleVersion(devId, 1, System.DateTime.Now.ToUniversalTime());
			}
			else
			{
				version = new ResourceModuleVersion(devId, version.BuildNumber+1, System.DateTime.Now.ToUniversalTime());
			}

			this.moduleInfo.UpdateVersion(version);

			ResourceModule.SaveManifest(this.moduleInfo);
		}

		private void OptimizeBundles(ResourceBundle bundle)
		{
			//	Optimise un bundle; cela va supprimer un certain nombre
			//	d'informations inutiles (par ex. des ressources secondaires
			//	vides).

			if (bundle.BasedOnPatchModule && !this.isMergingModules)
			{
				//	N'optimise pas les ressources d'un module de patch, car
				//	l'optimisation ne peut se faire qu'apr�s fusion.

				return;
			}

			System.Diagnostics.Debug.Assert(bundle.ResourceLevel != ResourceLevel.Merged);
			System.Diagnostics.Debug.Assert(bundle.ResourceLevel != ResourceLevel.None);

			if (bundle.Name == Resources.CaptionsBundleName ||
				bundle.Name == Resources.StringsBundleName)
			{
				for (int i=0; i<bundle.FieldCount; i++)
				{
					ResourceBundle.Field field = bundle[i];

					if (field.About == "" || ResourceBundle.Field.IsNullString(field.About))
					{
						//	Si un champ contient un commentaire vide et qu'il
						//	s'agit d'une ressource d'un module de r�f�rence,
						//	alors on peut supprimer compl�tement son contenu.

						field.SetAbout(null);
					}

					if (bundle.ResourceLevel != ResourceLevel.Default)
					{
						System.Diagnostics.Debug.Assert(field.Name == null);

						//	Si une ressource est vide dans un bundle autre que le bundle
						//	par d�faut, il faut la supprimer.
						if (ResourceBundle.Field.IsNullString(field.AsString) &&
							ResourceBundle.Field.IsNullString(field.About))
						{
							bundle.Remove(i);
							i--;
						}
					}
				}
			}
		}

		private void HandleBatchSaverModuleSaved(ResourceManager manager, ResourceBundle bundle, ResourceSetMode mode)
		{
			switch (mode)
			{
				case ResourceSetMode.CreateOnly:
				case ResourceSetMode.InMemory:
				case ResourceSetMode.UpdateOnly:
				case ResourceSetMode.Write:
					break;

				case ResourceSetMode.None:
				case ResourceSetMode.Remove:
					return;

				default:
					throw new System.NotSupportedException();
			}

			if (bundle.Type == Resources.CaptionTypeName &&
				!string.IsNullOrEmpty(this.moduleInfo.SourceNamespace))
			{
				foreach (ResourceBundle.Field field in bundle.Fields)
				{
					if (field.Name == null)
					{
						System.Diagnostics.Debug.WriteLine("Found suspect field (no name)");
					}

					if (field.Name != null &&
						field.Name.StartsWith("Typ.StructuredType."))
					{
						try
						{
							this.RegenerateEntitiesSourceCode(this.mergedManager ?? manager, bundle);
						}
						catch (System.Exception ex)
						{
							System.Diagnostics.Debug.WriteLine("Exception : " + ex.Message);
						}
						break;
					}
				}
			}
		}

		/// <summary>
		/// Regenerates the C# entities source code.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="bundle">The bundle.</param>
		private void RegenerateEntitiesSourceCode(ResourceManager manager, ResourceBundle bundle)
		{
			CodeGenerator generator = new CodeGenerator(manager);
			generator.Emit();

			string modulePath = this.moduleId.Path;
			string sourceCodeRoot = System.IO.Path.Combine(modulePath, "SourceCode");
			string sourceCodePath = System.IO.Path.Combine(sourceCodeRoot, "Entities.cs");

			if (!System.IO.Directory.Exists(sourceCodeRoot))
			{
				System.IO.Directory.CreateDirectory(sourceCodeRoot);
			}

			generator.Formatter.SaveCodeToTextFile(sourceCodePath, System.Text.Encoding.UTF8);

#if false
			using (BuildDriver driver = new BuildDriver ())
			{
				driver.CreateBuildDirectory ();

				generator.Formatter.SaveCodeToTextFile (System.IO.Path.Combine (driver.BuildDirectory, "Entities.cs"), System.Text.Encoding.UTF8);
				CodeProjectSettings settings = driver.CreateSettings ("Common.Support.Entities");

				settings.References.Add (new CodeProjectReference ("System.Core"));
				settings.References.Add (CodeProjectReference.FromAssembly (typeof (Common.Support.Res).Assembly));
				settings.References.Add (CodeProjectReference.FromAssembly (typeof (Common.Types.Res).Assembly));

				settings.Sources.Add (new CodeProjectSource ("Entities.cs"));

				List<string> messages;

				bool result = driver.Compile (new CodeProject (settings));

				messages = Types.Collection.ToList (driver.GetBuildMessages ());

				foreach (string message in messages)
				{
					System.Diagnostics.Debug.WriteLine (message);
				}

				if (messages.Count == 0)
				{
				}

				if ((driver.GetCompiledAssemblyPath () != null) &&
					(driver.GetCompiledAssemblyDebugInfoPath () != null))
				{
					//	...
				}
			}
#endif
		}

		/// <summary>
		/// Regenerates the C# forms source code.
		/// </summary>
		/// <param name="bundles">The bundles.</param>
		private void RegenerateFormsSourceCode(IEnumerable<ResourceBundle> bundles)
		{
			FormEngine.CodeGenerator generator = new FormEngine.CodeGenerator(this.ResourceManager);
			generator.Emit(bundles);

			string modulePath = this.moduleId.Path;
			string sourceCodeRoot = System.IO.Path.Combine(modulePath, "SourceCode");
			string sourceCodePath = System.IO.Path.Combine(sourceCodeRoot, "Forms.cs");

			if (!System.IO.Directory.Exists(sourceCodeRoot))
			{
				System.IO.Directory.CreateDirectory(sourceCodeRoot);
			}

			generator.Formatter.SaveCodeToTextFile(sourceCodePath, System.Text.Encoding.UTF8);
		}

		private void HandleAccessDirtyChanged(object sender)
		{
			//	Appel� lorsque l'�tat IsDirty d'un acc�s a chang�.
			if (this.designerApplication != null)
			{
				this.designerApplication.GetCommandState("Save").Enable = this.IsGlobalDirty;
				this.designerApplication.GetCommandState("EditOk").Enable = this.IsLocalDirty;
				this.designerApplication.GetCommandState("EditCancel").Enable = this.IsLocalDirty;
				this.designerApplication.UpdateBookModules();
			}
		}


		protected IEnumerable<ResourceAccess> Accesses
		{
			//	Enum�re tous les acc�s.
			get
			{
				yield return accessStrings;
				yield return accessCaptions;
				yield return accessCommands;
				yield return accessEntities;
				yield return accessFields;
				yield return accessValues;
				yield return accessTypes;
				yield return accessPanels;
				yield return accessForms;
			}
		}


		protected ResourceBundleBatchSaver	batchSaver;
		protected DesignerApplication		designerApplication;
		protected DesignerMode				mode;
		protected ResourceModuleId			moduleId;
		protected ResourceModuleInfo		moduleInfo;
		protected Modifier					modifier;
		protected ResourceManager			resourceManager;
		protected ResourceAccess			accessStrings;
		protected ResourceAccess			accessCaptions;
		protected ResourceAccess			accessCommands;
		protected ResourceAccess			accessPanels;
		protected ResourceAccess			accessEntities;
		protected ResourceAccess			accessFields;
		protected ResourceAccess			accessValues;
		protected ResourceAccess			accessTypes;
		protected ResourceAccess			accessForms;
		protected bool						isEditLocked = true;
		protected bool						isMergingModules;
		protected ResourceManager			mergedManager;
	}
}
