using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Accès généralisé aux ressources.
	/// </summary>
	public class ResourceAccess
	{
		public enum Type
		{
			Strings,
			Captions,
			Commands,
			Types,
			Panels,
		}

		public enum ModificationState
		{
			Normal,			//	défini normalement
			Empty,			//	vide (fond rouge)
			Modified,		//	modifié (fond jaune)
		}


		public ResourceAccess(Type type, ResourceManager resourceManager)
		{
			//	Constructeur unique pour accéder aux ressources d'un type donné.
			//	Par la suite, l'instance créée accédera toujours aux ressources de ce type.
			this.type = type;
			this.resourceManager = resourceManager;

			if (this.IsBundlesType)
			{
				this.druidsIndex = new List<Druid>();
			}
		}


		public void Load()
		{
			//	Charge les ressources.
			if (this.IsBundlesType)
			{
				this.LoadBundles();
				this.SetFilterBundles("", Searcher.SearchingMode.None);
			}

			if (this.type == Type.Panels)
			{
				this.LoadPanels();
			}

			this.isDirty = false;
		}

		public void Save()
		{
			//	Enregistre les modifications des ressources.
			if (this.IsBundlesType)
			{
				this.SaveBundles();
			}

			if (this.type == Type.Panels)
			{
				this.SavePanels();
			}

			this.isDirty = false;
		}

		public bool IsDirty
		{
			//	Est-ce que les ressources ont été modifiées.
			get
			{
				return this.isDirty;
			}
		}


		public Druid Duplicate(string name, int index, bool duplicate)
		{
			//	Duplique une ressource.
			return Druid.Empty;
		}

		public void Delete(Druid druid)
		{
			//	Supprime une ressource.
		}

		public void Move(Druid druid, int newIndex)
		{
			//	Déplace une ressource.
		}


		public void ModificationSetOne(Druid druid, string cultureName)
		{
			//	Considère une ressource comme modifiée dans une culture.
		}

		public void ModificationClearAll(Druid druid)
		{
			//	Considère une ressource comme à jour dans toutes les cultures.
		}

		public bool IsModificationAll(Druid druid)
		{
			//	Donne l'état de la commande ModificationAll.
			return false;
		}


		public void SetFilter(string filter, Searcher.SearchingMode mode)
		{
			//	Construit l'index en fonction des ressources primaires.
			if (this.IsBundlesType)
			{
				this.SetFilterBundles(filter, mode);
			}
		}

		public int AccessCount
		{
			//	Retourne le nombre de données accessibles.
			get
			{
				if (this.IsBundlesType)
				{
					return this.druidsIndex.Count;
				}

				return 0;
			}
		}

		public Druid GetDruid(int index)
		{
			//	Retourne le Druid correspondant à un index donné.
			if (this.IsBundlesType)
			{
				return this.druidsIndex[index];
			}

			return Druid.Empty;
		}

		public string[] GetAccessFieldNames
		{
			//	Donne les noms internes (fieldName) des champs accessibles.
			get
			{
				switch (this.type)
				{
					case Type.Strings:
						return ResourceAccess.AccessStrings;

					case Type.Captions:
						return ResourceAccess.AccessCaptions;
				}

				return null;
			}
		}

		public string GetFieldDescription(string fieldName)
		{
			//	Donne le texte descriptif pour un champ.
			if (this.type == Type.Strings)
			{
				if (fieldName == ResourceAccess.AccessStrings[0])
				{
					return Res.Strings.Viewers.Strings.Edit;
				}

				if (fieldName == ResourceAccess.AccessStrings[1])
				{
					return Res.Strings.Viewers.Strings.About;
				}
			}

			if (this.type == Type.Captions)
			{
				if (fieldName == ResourceAccess.AccessCaptions[0])
				{
					return Res.Strings.Viewers.Captions.Labels;
				}

				if (fieldName == ResourceAccess.AccessCaptions[1])
				{
					return Res.Strings.Viewers.Captions.Description;
				}

				if (fieldName == ResourceAccess.AccessCaptions[2])
				{
					return Res.Strings.Viewers.Captions.Icon;
				}

				if (fieldName == ResourceAccess.AccessCaptions[3])
				{
					return Res.Strings.Viewers.Captions.About;
				}
			}

			return null;
		}

		public Field GetField(int index, string cultureName, string fieldName)
		{
			//	Retourne les données d'un champ.
			//	Si cultureName est nul, on accède à la culture de base.
			this.AccessCache(index, cultureName);

			if (this.IsBundlesType)
			{
				if (fieldName == ResourceAccess.AccessStrings[0])
				{
					Field field = new Field(Field.Type.String);
					field.String = this.accessField.Name;
					return field;
				}
			}

			if (this.type == Type.Strings)
			{
				if (this.accessField == null)
				{
					return null;
				}

				if (fieldName == ResourceAccess.AccessStrings[1])
				{
					Field field = new Field(Field.Type.String);
					field.String = this.accessField.AsString;
					return field;
				}

				if (fieldName == ResourceAccess.AccessStrings[2])
				{
					Field field = new Field(Field.Type.String);
					field.String = this.accessField.About;
					return field;
				}
			}

			if (this.type == Type.Captions)
			{
				if (this.accessField == null || this.accessCaption == null)
				{
					return null;
				}

				if (fieldName == ResourceAccess.AccessCaptions[1])
				{
					Field field = new Field(Field.Type.StringCollection);
					field.StringCollection = this.accessCaption.Labels;
					return field;
				}

				if (fieldName == ResourceAccess.AccessCaptions[2])
				{
					Field field = new Field(Field.Type.String);
					field.String = this.accessCaption.Description;
					return field;
				}

				if (fieldName == ResourceAccess.AccessCaptions[3])
				{
					Field field = new Field(Field.Type.String);
					field.String = this.accessCaption.Icon;
					return field;
				}

				if (fieldName == ResourceAccess.AccessCaptions[4])
				{
					Field field = new Field(Field.Type.String);
					field.String = this.accessField.About;
					return field;
				}
			}

			return null;
		}

		public void SetField(int index, string cultureName, string fieldName, Field field)
		{
			//	Modifie les données d'un champ.
			//	Si cultureName est nul, on accède à la culture de base.
			this.AccessCache(index, cultureName);

			if (this.IsBundlesType)
			{
				if (fieldName == ResourceAccess.AccessStrings[0])
				{
					this.accessField.SetName(field.String);
				}
			}

			if (this.type == Type.Strings)
			{
				if (this.accessField == null)
				{
					return;
				}

				if (fieldName == ResourceAccess.AccessStrings[1])
				{
					this.accessField.SetStringValue(field.String);
				}

				if (fieldName == ResourceAccess.AccessStrings[2])
				{
					this.accessField.SetAbout(field.String);
				}
			}

			if (this.type == Type.Captions)
			{
				if (this.accessField == null || this.accessCaption == null)
				{
					return;
				}

				if (fieldName == ResourceAccess.AccessCaptions[1])
				{
					ICollection<string> src = field.StringCollection;
					ICollection<string> dst = this.accessCaption.Labels;

					dst.Clear();
					foreach (string s in src)
					{
						dst.Add(s);
					}
				}

				if (fieldName == ResourceAccess.AccessCaptions[2])
				{
					this.accessCaption.Description = field.String;
				}

				if (fieldName == ResourceAccess.AccessCaptions[3])
				{
					this.accessCaption.Icon = field.String;
				}

				if (fieldName == ResourceAccess.AccessCaptions[4])
				{
					this.accessField.SetAbout(field.String);
				}
			}
		}

		public ModificationState GetModification(int index, string cultureName)
		{
			//	Donne l'état 'modifié'.
			this.AccessCache(index, cultureName);

			if (this.IsBundlesType)
			{
				if (this.accessField == null || string.IsNullOrEmpty(this.accessField.AsString))
				{
					return ModificationState.Empty;
				}

				if (this.accessBundle != this.primaryBundle)  // culture secondaire ?
				{
					Druid druid = this.druidsIndex[this.accessIndex];
					ResourceBundle.Field primaryField = this.primaryBundle[druid];

					if (primaryField.ModificationId > this.accessField.ModificationId)
					{
						return ModificationState.Modified;
					}
				}
			}

			return ModificationState.Normal;
		}

		public void SetModification(int index, string cultureName, ModificationState state)
		{
			//	Change l'état 'modifié'.
			this.AccessCache(index, cultureName);

			if (this.IsBundlesType)
			{
			}
		}

		protected void AccessCache(int index, string cultureName)
		{
			if (this.IsBundlesType)
			{
				if (this.accessCulture != cultureName)  // changement de culture ?
				{
					this.accessCulture = cultureName;
					this.accessField = null;
					this.accessCaption = null;

					if (string.IsNullOrEmpty(this.accessCulture))  // culture de base ?
					{
						this.accessBundle = this.bundles[ResourceLevel.Default];
					}
					else
					{
						this.accessBundle = this.GetCulture(this.accessCulture);
					}
				}

				if (this.accessBundle == null || index < 0 || index >= this.druidsIndex.Count)
				{
					return;
				}

				//	Met en cache le ResourceBundle.Field.
				if (this.accessIndex != index || this.accessField == null)
				{
					Druid druid = this.druidsIndex[index];
					this.accessField = this.accessBundle[druid];
				}

				//	Met en cache le Caption.
				if (this.type == Type.Captions)
				{
					if (this.accessIndex != index || this.accessCaption == null)
					{
						this.accessCaption = new Common.Types.Caption();

						if (this.accessField != null)
						{
							string s = this.accessField.AsString;
							if (!string.IsNullOrEmpty(s))
							{
								this.accessCaption.DeserializeFromString(s);
							}
						}
					}
				}
			}

			this.accessIndex = index;
		}

		protected static string[] AccessStrings = { "Name", "String", "About" };
		protected static string[] AccessCaptions = { "Name", "Labels", "Description", "Icon", "About" };


		public bool IsExistingCulture(string name)
		{
			//	Indique si une culture donnée existe.
			if (this.IsBundlesType)
			{
				for (int b=0; b<this.bundles.Count; b++)
				{
					ResourceBundle bundle = this.bundles[b];
					if (name == bundle.Culture.Name)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void CreateCulture(string cultureName)
		{
			//	Crée un nouveau bundle pour une culture donnée.
			if (this.IsBundlesType)
			{
				string prefix = this.resourceManager.ActivePrefix;
				System.Globalization.CultureInfo culture = Resources.FindSpecificCultureInfo(cultureName);
				ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, this.bundles.Name, ResourceLevel.Localized, culture);

				bundle.DefineType(this.BundlesName(false));
				this.resourceManager.SetBundle(bundle, ResourceSetMode.CreateOnly);

				this.LoadBundles();
			}
		}

		public void DeleteCulture(string cultureName)
		{
			//	Supprime une culture.
			if (this.IsBundlesType)
			{
				ResourceBundle bundle = this.GetCulture(cultureName);
				if (bundle != null)
				{
					this.resourceManager.RemoveBundle(this.BundlesName(true), ResourceLevel.Localized, bundle.Culture);
					this.LoadBundles();
				}
			}
		}



		protected ResourceBundle GetCulture(string cultureName)
		{
			//	Cherche le bundle d'une culture.
			for (int b=0; b<bundles.Count; b++)
			{
				ResourceBundle bundle = bundles[b];
				if (Misc.CultureName(bundle.Culture) == cultureName)
				{
					return bundle;
				}
			}
			return null;
		}


		protected void LoadBundles()
		{
			string[] ids = this.resourceManager.GetBundleIds("*", this.BundlesName(false), ResourceLevel.Default);
			if (ids.Length == 0)
			{
				//	Crée un premier bundle vide.
				string prefix = this.resourceManager.ActivePrefix;
				System.Globalization.CultureInfo culture = this.BaseCulture;
				ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, this.BundlesName(true), ResourceLevel.Default, culture);
				bundle.DefineType(this.BundlesName(false));

				//	Crée un premier champ vide avec un premier Druid.
				//	Ceci est nécessaire, car il n'existe pas de commande pour créer un champ à partir
				//	de rien, mais seulement une commande pour dupliquer un champ existant.
				int moduleId = bundle.Module.Id;
				int developerId = 0;  // [PA] provisoire
				int localId = 0;
				Druid newDruid = new Druid(moduleId, developerId, localId);

				ResourceBundle.Field newField = bundle.CreateField(ResourceFieldType.Data);
				newField.SetDruid(newDruid);
				newField.SetName(Res.Strings.Viewers.Panels.New);
				newField.SetStringValue("");
				bundle.Add(newField);

				//	Sérialise le bundle et son premier champ sur disque. Il serait préférable de
				//	faire ceci lors du Save, mais cette situation étant exceptionelle, il est
				//	acceptable de faire ainsi !
				this.resourceManager.SetBundle(bundle, ResourceSetMode.CreateOnly);

				ids = this.resourceManager.GetBundleIds("*", this.BundlesName(false), ResourceLevel.Default);
				System.Diagnostics.Debug.Assert(ids.Length != 0);
			}

			this.bundles = new ResourceBundleCollection(this.resourceManager);
			this.bundles.LoadBundles(this.resourceManager.ActivePrefix, this.resourceManager.GetBundleIds(ids[0], ResourceLevel.All));

			this.primaryBundle = this.bundles[ResourceLevel.Default];
		}

		protected void SaveBundles()
		{
			foreach (ResourceBundle bundle in this.bundles)
			{
				this.resourceManager.SetBundle(bundle, ResourceSetMode.UpdateOnly);
			}
		}

		protected void SetFilterBundles(string filter, Searcher.SearchingMode mode)
		{
			this.druidsIndex.Clear();

			if ((mode&Searcher.SearchingMode.CaseSensitive) == 0)
			{
				filter = Searcher.RemoveAccent(filter.ToLower());
			}

			Regex regex = null;
			if ((mode&Searcher.SearchingMode.Jocker) != 0)
			{
				regex = RegexFactory.FromSimpleJoker(filter, RegexFactory.Options.None);
			}

			foreach (ResourceBundle.Field field in this.primaryBundle.Fields)
			{
				if (!this.HasFixFilter(field.Name))
				{
					continue;
				}

				string name = this.SubFilter(field.Name);

				if (filter != "")
				{
					if ((mode&Searcher.SearchingMode.Jocker) != 0)
					{
						string text = name;
						if ((mode&Searcher.SearchingMode.CaseSensitive) == 0)
						{
							text = Searcher.RemoveAccent(text.ToLower());
						}
						if (!regex.IsMatch(text))
						{
							continue;
						}
					}
					else
					{
						int index = Searcher.IndexOf(name, filter, 0, mode);
						if (index == -1)
						{
							continue;
						}
						if ((mode&Searcher.SearchingMode.AtBeginning) != 0 && index != 0)
						{
							continue;
						}
					}
				}

				Druid fullDruid = new Druid(field.Druid, this.primaryBundle.Module.Id);
				this.druidsIndex.Add(fullDruid);
			}
		}


		protected void LoadPanels()
		{
		}

		protected void SavePanels()
		{
		}


		protected System.Globalization.CultureInfo BaseCulture
		{
			//	Retourne la culture de base, définie par les ressources "Strings" ou "Captions".
			get
			{
				if (this.bundles == null)
				{
					//	S'il n'existe aucun bundle, retourne la culture la plus importante.
					return new System.Globalization.CultureInfo(Misc.Cultures[0]);
				}
				else
				{
					//	Retourne la culture du bundle par défaut.
					ResourceBundle res = this.bundles[ResourceLevel.Default];
					return res.Culture;
				}
			}
		}


		protected bool IsBundlesType
		{
			//	Retourne true si on accède à des ressources de type
			//	"un bundle par culture, plusieurs ressources par bundle".
			get
			{
				return (this.type == Type.Strings || this.type == Type.Captions || this.type == Type.Commands || this.type == Type.Types);
			}
		}

		protected string BundlesName(bool many)
		{
			//	Retourne un nom interne (pour Common.Support & Cie) en fonction du type.
			switch (this.type)
			{
				case Type.Strings:
					return many ? "Strings" : "String";

				case Type.Captions:
				case Type.Commands:
				case Type.Types:
					return many ? "Captions" : "Caption";
			}

			return null;
		}


		#region FixFilter
		protected string AddFilter(string name)
		{
			//	Ajoute le filtre fixe si nécessaire.
			if (!this.HasFixFilter(name))
			{
				return this.FixFilter + name;
			}

			return name;
		}

		public string SubFilter(string name)
		{
			//	Supprime le filtre fixe si nécessaire.
			if (this.HasFixFilter(name))
			{
				return name.Substring(this.FixFilter.Length);
			}

			return name;
		}

		protected bool HasFixFilter(string name)
		{
			//	Indique si un nom commence par le filtre fixe.
			string fix = this.FixFilter;
			
			if (fix == null)
			{
				return false;
			}
			else
			{
				return name.StartsWith(fix);
			}
		}

		protected string FixFilter
		{
			//	Retourne la chaîne fixe du filtre.
			get
			{
				switch (this.type)
				{
					case Type.Captions:
						return "Cap.";

					case Type.Commands:
						return "Cmd.";

					case Type.Types:
						return "Typ.";
				}

				return null;
			}
		}
		#endregion


		#region Field
		/// <summary>
		/// Permet d'accéder à un champ de n'importe quel type.
		/// </summary>
		public class Field
		{
			public enum Type
			{
				String,
				StringCollection,
				Bundle,
			}

			public Field(Type type)
			{
				this.type = type;
			}

			public Type FieldType
			{
				get
				{
					return this.type;
				}
			}

			public string String
			{
				get
				{
					System.Diagnostics.Debug.Assert(this.type == Type.String);
					return this.stringValue;
				}
				set
				{
					System.Diagnostics.Debug.Assert(this.type == Type.String);
					this.stringValue = value;
				}
			}

			public ICollection<string> StringCollection
			{
				get
				{
					System.Diagnostics.Debug.Assert(this.type == Type.StringCollection);
					return this.stringCollection;
				}
				set
				{
					System.Diagnostics.Debug.Assert(this.type == Type.StringCollection);
					this.stringCollection = value;
				}
			}

			public ResourceBundle Bundle
			{
				get
				{
					System.Diagnostics.Debug.Assert(this.type == Type.Bundle);
					return this.bundle;
				}
				set
				{
					System.Diagnostics.Debug.Assert(this.type == Type.Bundle);
					this.bundle = value;
				}
			}

			protected Type						type;
			protected string					stringValue;
			protected ICollection<string>		stringCollection;
			protected ResourceBundle			bundle;
		}
		#endregion


		protected Type							type;
		protected ResourceManager				resourceManager;
		protected bool							isDirty = false;

		protected ResourceBundleCollection		bundles;
		protected ResourceBundle				primaryBundle;
		protected List<Druid>					druidsIndex;
		protected string						accessCulture;
		protected ResourceBundle				accessBundle;
		protected int							accessIndex;
		protected ResourceBundle.Field			accessField;
		protected Common.Types.Caption			accessCaption;
	}
}
