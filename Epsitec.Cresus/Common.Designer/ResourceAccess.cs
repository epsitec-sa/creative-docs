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

		public void SetFilter(string filter, Searcher.SearchingMode mode)
		{
			//	Construit l'index en fonction des ressources primaires.
			if (this.IsBundlesType)
			{
				this.SetFilterBundles(filter, mode);
			}
		}


		public int DataCount
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

		public string[] GetAccessFieldNames
		{
			//	Donne les noms des champs accessibles.
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



		public Field GetAccessField(int index, string cultureName, string fieldName)
		{
			//	Retourne une donnée.
			this.accessIndex = index;

			if (this.IsBundlesType)
			{
				if (this.accessCulture != cultureName)
				{
					this.accessCulture = cultureName;

					if (this.accessCulture == "")  // culture par défaut ?
					{
						this.accessBundle = this.bundles[ResourceLevel.Default];
					}
					else
					{
						this.accessBundle = this.GetCulture(this.accessCulture);
					}
				}
			}

			if (this.type == Type.Strings)
			{
				if (this.accessBundle == null || this.accessIndex < 0 || this.accessIndex >= this.druidsIndex.Count)
				{
					return null;
				}
				Druid druid = this.druidsIndex[this.accessIndex];
				ResourceBundle.Field rbf = this.accessBundle[druid];

				if (fieldName == ResourceAccess.AccessStrings[0])
				{
					Field field = new Field(Field.Type.String);
					field.String = rbf.AsString;
					return field;
				}

				if (fieldName == ResourceAccess.AccessStrings[1])
				{
				}
			}

			if (this.type == Type.Captions)
			{
				if (fieldName == ResourceAccess.AccessCaptions[0])
				{
				}

				if (fieldName == ResourceAccess.AccessCaptions[1])
				{
				}

				if (fieldName == ResourceAccess.AccessCaptions[2])
				{
				}

				if (fieldName == ResourceAccess.AccessCaptions[3])
				{
				}
			}

			return null;
		}

		protected static string[] AccessStrings = { "String", "About" };
		protected static string[] AccessCaptions = { "Labels", "Description", "Icon", "About" };


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
		public class Field
		{
			public enum Type
			{
				String,
				StringsCollection,
				Bundle,
			}

			public Field(Type type)
			{
				this.type = type;
			}

			public Type GetType
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
					return this.stringValue;
				}
				set
				{
					this.stringValue = value;
				}
			}

			public ICollection<string> StringCollection
			{
				get
				{
					return this.stringCollection;
				}
				set
				{
					this.stringCollection = value;
				}
			}

			public ResourceBundle Bundle
			{
				get
				{
					return this.bundle;
				}
				set
				{
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
	}
}
