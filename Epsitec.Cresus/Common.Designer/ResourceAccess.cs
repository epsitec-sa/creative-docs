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
			Unknow,
			Strings,
			Captions,
			Commands,
			Types,
			Panels,
			Scripts,
		}

		public enum ModificationState
		{
			Normal,			//	défini normalement
			Empty,			//	vide (fond rouge)
			Modified,		//	modifié (fond jaune)
		}


		public ResourceAccess(Type type, ResourceManager resourceManager, ResourceModuleInfo moduleInfo)
		{
			//	Constructeur unique pour accéder aux ressources d'un type donné.
			//	Par la suite, l'instance créée accédera toujours aux ressources de ce type,
			//	sauf pour les ressources Captions, Commands et Types.
			this.type = type;
			this.resourceManager = resourceManager;
			this.moduleInfo = moduleInfo;

			this.druidsIndex = new List<Druid>();

			this.filterStrings = new string[10];
			this.filterModes = new Searcher.SearchingMode[10];
		}


		public Type ResourceType
		{
			//	Type des ressources accédées.
			get
			{
				return this.type;
			}

			set
			{
				System.Diagnostics.Debug.Assert(this.type == Type.Captions || this.type == Type.Commands || this.type == Type.Types);
				System.Diagnostics.Debug.Assert(value == Type.Captions || value == Type.Commands || value == Type.Types);
				if (this.type != value)
				{
					this.type = value;

					//	Remet le filtre correspondant au type.
					int i = (int) this.type;
					this.SetFilter(this.filterStrings[i], this.filterModes[i]);
				}
			}
		}

		public void Load()
		{
			//	Charge les ressources.
			if (this.IsBundlesType)
			{
				this.LoadBundles();
			}

			if (this.type == Type.Panels)
			{
				this.LoadPanels();
			}

			this.SetFilter("", Searcher.SearchingMode.None);

			this.IsDirty = false;
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

			this.IsDirty = false;
		}

		public bool IsDirty
		{
			//	Est-ce que les ressources ont été modifiées.
			get
			{
				return this.isDirty;
			}
			set
			{
				if (this.isDirty != value)
				{
					this.isDirty = value;
					this.OnDirtyChanged();
				}
			}
		}


		public void Duplicate(string newName, bool duplicateContent)
		{
			//	Duplique la ressource courante.
			Druid newDruid = Druid.Empty;

			if (this.IsBundlesType)
			{
				newName = this.AddFilter(newName);

				Druid actualDruid = this.druidsIndex[this.accessIndex];
				int aIndex = this.GetAbsoluteIndex(actualDruid);
				newDruid = this.CreateUniqueDruid();

				foreach (ResourceBundle bundle in this.bundles)
				{
					ResourceBundle.Field newField = bundle.CreateField(ResourceFieldType.Data);
					newField.SetDruid(newDruid);
					newField.SetName(newName);

					if (duplicateContent)
					{
						ResourceBundle.Field field = bundle[actualDruid];
						if (field.IsEmpty)
						{
							newField.SetStringValue("");
						}
						else
						{
							newField.SetStringValue(field.AsString);
							newField.SetAbout(field.About);
						}
					}
					else
					{
						newField.SetStringValue("");
					}

					if (bundle == this.primaryBundle)
					{
						newField.SetModificationId(1);
						bundle.Insert(aIndex+1, newField);
					}
					else
					{
						newField.SetModificationId(0);
						bundle.Add(newField);
					}
				}
			}

			if (this.type == Type.Panels)
			{
				newDruid = this.CreateUniqueDruid();
				string prefix = this.resourceManager.ActivePrefix;
				System.Globalization.CultureInfo culture = this.BaseCulture;
				ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, newDruid.ToBundleId(), ResourceLevel.Default, culture);

				bundle.DefineType(this.BundleName(false));
				bundle.DefineCaption(newName);

				//	TODO: gérer duplicateContent

				this.panelsList.Insert(this.accessIndex, bundle);
				this.panelsToCreate.Add(bundle);
			}

			this.druidsIndex.Insert(this.accessIndex+1, newDruid);
			this.accessIndex ++;
			this.CacheClear();

			this.IsDirty = true;
		}

		public void Delete()
		{
			//	Supprime la ressource courante dans toutes les cultures.
			if (this.IsBundlesType)
			{
				Druid druid = this.druidsIndex[this.accessIndex];

				foreach (ResourceBundle bundle in this.bundles)
				{
					int aIndex = bundle.IndexOf(druid);
					if (aIndex >= 0)
					{
						bundle.Remove(aIndex);
					}
				}
			}

			if (this.type == Type.Panels)
			{
				ResourceBundle bundle = this.PanelBundle(this.accessIndex);

				this.panelsList.Remove(bundle);

				//	S'il ne s'agit pas d'une nouvelle ressource, il faut l'ajouter dans la liste
				//	des ressources à détruire (lors du PanelsWrite).
				if (this.panelsToCreate.Contains(bundle))
				{
					this.panelsToCreate.Remove(bundle);
				}
				else
				{
					this.panelsToDelete.Add(bundle);
				}
			}

			this.druidsIndex.RemoveAt(this.accessIndex);

			if (this.accessIndex >= this.druidsIndex.Count)
			{
				this.accessIndex --;
			}

			this.CacheClear();

			this.IsDirty = true;
		}

		public void Move(int direction)
		{
			//	Déplace la ressource courante.
			Druid druid = this.druidsIndex[this.accessIndex];
			int aIndex = this.GetAbsoluteIndex(druid);
			System.Diagnostics.Debug.Assert(aIndex != -1);

			if (this.IsBundlesType)
			{
				ResourceBundle.Field field = this.primaryBundle[aIndex];
				this.primaryBundle.Remove(aIndex);
				this.primaryBundle.Insert(aIndex+direction, field);
			}

			if (this.type == Type.Panels)
			{
				ResourceBundle bundle = this.panelsList[aIndex];
				this.panelsList.RemoveAt(aIndex);
				this.panelsList.Insert(aIndex+direction, bundle);
			}

			this.druidsIndex[this.accessIndex] = this.druidsIndex[this.accessIndex+direction];
			this.druidsIndex[this.accessIndex+direction] = druid;

			this.accessIndex += direction;
			this.CacheClear();

			this.IsDirty = true;
		}


		public void SetFilter(string filter, Searcher.SearchingMode mode)
		{
			//	Construit l'index en fonction des ressources primaires.
			Druid druid = Druid.Empty;
			if (this.accessIndex >= 0 && this.accessIndex < this.druidsIndex.Count)
			{
				druid = this.druidsIndex[this.accessIndex];
			}

			//	Met à jour druidsIndex.
			if (this.IsBundlesType)
			{
				this.SetFilterBundles(filter, mode);
			}

			if (this.type == Type.Panels)
			{
				this.SetFilterPanels(filter, mode);
			}

			//	Mémorise le filtre utilisé.
			int i = (int) this.type;
			this.filterStrings[i] = filter;
			this.filterModes[i] = mode;

			//	Cherche l'index correspondant à la ressource d'avant le changement de filtre.
			int index = this.druidsIndex.IndexOf(druid);
			if (index == -1)
			{
				index = 0;
			}
			this.accessIndex = index;

			this.CacheClear();
		}

		public int TotalCount
		{
			//	Retourne le nombre de données accessibles.
			get
			{
				if (this.IsBundlesType)
				{
					return this.primaryBundle.FieldCount;
				}

				if (this.type == Type.Panels)
				{
					return this.panelsList.Count;
				}

				return 0;
			}
		}

		public int AccessCount
		{
			//	Retourne le nombre de données accessibles.
			get
			{
				return this.druidsIndex.Count;
			}
		}

		public int AccessIndex
		{
			//	Index de l'accès en cours.
			get
			{
				return this.accessIndex;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, this.druidsIndex.Count-1);
				this.accessIndex = value;
			}
		}

		public string[] GetAccessFieldNames
		{
			//	Donne les noms internes (fieldName) des champs accessibles.
			get
			{
				switch (this.type)
				{
					case Type.Strings:
						return ResourceAccess.NameStrings;

					case Type.Captions:
						return ResourceAccess.NameCaptions;
				}

				return null;
			}
		}

		public Field.Type[] GetAccessFieldTypes
		{
			//	Donne les types des champs accessibles.
			get
			{
				switch (this.type)
				{
					case Type.Strings:
						return ResourceAccess.TypeStrings;

					case Type.Captions:
						return ResourceAccess.TypeCaptions;
				}

				return null;
			}
		}

		public string GetFieldDescription(string fieldName)
		{
			//	Donne le texte descriptif pour un champ.
			if (this.type == Type.Strings)
			{
				if (fieldName == ResourceAccess.NameStrings[1])
				{
					return Res.Strings.Viewers.Strings.Edit;
				}

				if (fieldName == ResourceAccess.NameStrings[2])
				{
					return Res.Strings.Viewers.Strings.About;
				}
			}

			if (this.type == Type.Captions)
			{
				if (fieldName == ResourceAccess.NameCaptions[1])
				{
					return Res.Strings.Viewers.Captions.Labels;
				}

				if (fieldName == ResourceAccess.NameCaptions[2])
				{
					return Res.Strings.Viewers.Captions.Description;
				}

				if (fieldName == ResourceAccess.NameCaptions[3])
				{
					return Res.Strings.Viewers.Captions.Icon;
				}

				if (fieldName == ResourceAccess.NameCaptions[4])
				{
					return Res.Strings.Viewers.Captions.About;
				}
			}

			return null;
		}


		public bool IsExistingName(string name)
		{
			//	Vérifie si un futur "Name" existe déjà.
			if (this.IsBundlesType)
			{
				name = this.AddFilter(name);
				ResourceBundle.Field field = this.primaryBundle[name];
				return (field != null && field.Name != null);
			}

			if (this.type == Type.Panels)
			{
				foreach (ResourceBundle bundle in this.panelsList)
				{
					if (bundle.Caption == name)
					{
						return true;
					}
				}
			}

			return false;
		}

		public string GetDuplicateName(string baseName)
		{
			//	Retourne le nom à utiliser lorsqu'un nom existant est dupliqué.
			int numberLength = 0;
			while (baseName.Length > 0)
			{
				char last = baseName[baseName.Length-1-numberLength];
				if (last >= '0' && last <= '9')
				{
					numberLength++;
				}
				else
				{
					break;
				}
			}

			int nextNumber = 2;
			if (numberLength > 0)
			{
				nextNumber = int.Parse(baseName.Substring(baseName.Length-numberLength))+1;
				baseName = baseName.Substring(0, baseName.Length-numberLength);
			}

			string newName = baseName;
			for (int i=nextNumber; i<nextNumber+100; i++)
			{
				newName = string.Concat(baseName, i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				if (!this.IsExistingName(newName))
				{
					break;
				}
			}

			return newName;
		}


		public Druid GetBypassFilterDruid(int index)
		{
			//	Donne un druid, sans tenir compte du filtre.
			System.Diagnostics.Debug.Assert(this.type == Type.Strings);
			return this.druidsIndex[index];
		}

		public void GetBypassFilterStrings(Druid druid, ResourceBundle bundle, out string name, out string text, out bool isDefined)
		{
			//	Donne les chaînes 'Name' et 'String' d'une ressource de type Strings,
			//	sans tenir compte du filtre.
			System.Diagnostics.Debug.Assert(this.type == Type.Strings);
			ResourceBundle.Field field = bundle[druid];

			isDefined = true;

			if (field.IsEmpty)
			{
				field = this.primaryBundle[druid];
				isDefined = false;
			}

			name = this.SubFilter(field.Name);
			text = field.AsString;
		}

		public Druid CreateBypassFilter(ResourceBundle bundle, string name, string text)
		{
			//	Crée une nouvelle ressource 'Strings' à la fin.
			System.Diagnostics.Debug.Assert(this.type == Type.Strings);
			name = this.AddFilter(name);

			Druid newDruid = this.CreateUniqueDruid();

			foreach (ResourceBundle b in this.bundles)
			{
				ResourceBundle.Field newField = b.CreateField(ResourceFieldType.Data);
				newField.SetDruid(newDruid);
				newField.SetName(name);

				if (b == bundle)
				{
					newField.SetStringValue(text);
				}
				else
				{
					newField.SetStringValue("");
				}

				if (b == this.primaryBundle)
				{
					newField.SetModificationId(1);
					b.Add(newField);
				}
				else
				{
					newField.SetModificationId(0);
					b.Add(newField);
				}
			}

			this.druidsIndex.Add(newDruid);

			return newDruid;
		}


		public Field GetField(int index, string cultureName, string fieldName)
		{
			//	Retourne les données d'un champ.
			//	Si cultureName est nul, on accède à la culture de base.
			this.CacheResource(index, cultureName);

			if (this.IsBundlesType)
			{
				if (fieldName == ResourceAccess.NameStrings[0])
				{
					return new Field(this.SubFilter(this.accessField.Name));
				}
			}

			if (this.type == Type.Strings)
			{
				if (this.accessField == null)
				{
					return null;
				}

				if (fieldName == ResourceAccess.NameStrings[1])
				{
					return new Field(this.accessField.AsString);
				}

				if (fieldName == ResourceAccess.NameStrings[2])
				{
					return new Field(this.accessField.About);
				}
			}

			if (this.type == Type.Captions)
			{
				if (this.accessField == null || this.accessCaption == null)
				{
					return null;
				}

				if (fieldName == ResourceAccess.NameCaptions[1])
				{
					return new Field(this.accessCaption.Labels);
				}

				if (fieldName == ResourceAccess.NameCaptions[2])
				{
					return new Field(this.accessCaption.Description);
				}

				if (fieldName == ResourceAccess.NameCaptions[3])
				{
					return new Field(this.accessCaption.Icon);
				}

				if (fieldName == ResourceAccess.NameCaptions[4])
				{
					return new Field(this.accessField.About);
				}
			}

			if (this.type == Type.Panels)
			{
				ResourceBundle bundle = this.PanelBundle(index);
				if (bundle == null)
				{
					return null;
				}

				if (fieldName == ResourceAccess.NamePanels[0])
				{
					return new Field(bundle.Caption);
				}

				if (fieldName == ResourceAccess.NamePanels[1])
				{
					return new Field(bundle);
				}
			}

			return null;
		}

		public void SetField(int index, string cultureName, string fieldName, Field field)
		{
			//	Modifie les données d'un champ.
			//	Si cultureName est nul, on accède à la culture de base.
			this.CacheResource(index, cultureName);

			if (this.IsBundlesType)
			{
				this.CreateIfNecessary();

				if (fieldName == ResourceAccess.NameStrings[0])
				{
					this.accessField.SetName(this.AddFilter(field.String));
				}
			}

			if (this.type == Type.Strings)
			{
				if (this.accessField == null)
				{
					return;
				}

				if (fieldName == ResourceAccess.NameStrings[1])
				{
					this.accessField.SetStringValue(field.String);
				}

				if (fieldName == ResourceAccess.NameStrings[2])
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

				if (fieldName == ResourceAccess.NameCaptions[1])
				{
					ICollection<string> src = field.StringCollection;
					ICollection<string> dst = this.accessCaption.Labels;

					dst.Clear();
					foreach (string s in src)
					{
						dst.Add(s);
					}

					this.accessField.SetStringValue(this.accessCaption.SerializeToString());
				}

				if (fieldName == ResourceAccess.NameCaptions[2])
				{
					this.accessCaption.Description = field.String;
					this.accessField.SetStringValue(this.accessCaption.SerializeToString());
				}

				if (fieldName == ResourceAccess.NameCaptions[3])
				{
					this.accessCaption.Icon = field.String;
					this.accessField.SetStringValue(this.accessCaption.SerializeToString());
				}

				if (fieldName == ResourceAccess.NameCaptions[4])
				{
					this.accessField.SetAbout(field.String);
				}
			}

			if (this.type == Type.Panels)
			{
				ResourceBundle bundle = this.PanelBundle(index);
				if (bundle == null)
				{
					return;
				}

				if (fieldName == ResourceAccess.NamePanels[0])
				{
					bundle.DefineCaption(field.String);
				}

				if (fieldName == ResourceAccess.NamePanels[1])
				{
					//	TODO:
				}
			}

			this.IsDirty = true;
		}


		public static MyWidgets.StringList.CellState CellState(ModificationState state)
		{
			//	Conversion de l'état d'une ressource.
			MyWidgets.StringList.CellState cs = MyWidgets.StringList.CellState.Normal;

			switch (state)
			{
				case ResourceAccess.ModificationState.Empty:
					cs = MyWidgets.StringList.CellState.Warning;
					break;

				case ResourceAccess.ModificationState.Modified:
					cs = MyWidgets.StringList.CellState.Modified;
					break;
			}

			return cs;
		}

		public ModificationState GetModification(int index, string cultureName)
		{
			//	Donne l'état 'modifié'.
			if (index != -1)
			{
				this.CacheResource(index, cultureName);

				if (this.IsBundlesType)
				{
					if (this.accessField == null || string.IsNullOrEmpty(this.accessField.AsString))
					{
						return ModificationState.Empty;
					}

					if (this.accessBundle != this.primaryBundle)  // culture secondaire ?
					{
						Druid druid = this.druidsIndex[index];
						ResourceBundle.Field primaryField = this.primaryBundle[druid];

						if (primaryField.ModificationId > this.accessField.ModificationId)
						{
							return ModificationState.Modified;
						}
					}
				}
			}

			return ModificationState.Normal;
		}

		public void ModificationClear(int index, string cultureName)
		{
			//	Considère une ressource comme 'à jour' dans une culture.
			this.CacheResource(index, cultureName);

			if (this.IsBundlesType)
			{
				if (this.accessBundle != this.primaryBundle && !this.accessField.IsEmpty)
				{
					Druid druid = this.druidsIndex[index];
					this.accessField.SetModificationId(this.primaryBundle[druid].ModificationId);
				}
			}

			this.IsDirty = true;
		}

		public void ModificationSetAll(int index)
		{
			//	Considère une ressource comme 'modifiée' dans toutes les cultures.
			if (this.IsBundlesType)
			{
				Druid druid = this.druidsIndex[index];
				int id = this.primaryBundle[druid].ModificationId;
				this.primaryBundle[druid].SetModificationId(id+1);

				this.CacheClear();
			}

			this.IsDirty = true;
		}

		public bool IsModificationAll(int index)
		{
			//	Donne l'état de la commande ModificationAll.
			if (this.IsBundlesType)
			{
				Druid druid = this.druidsIndex[index];
				int id = this.primaryBundle[druid].ModificationId;
				int count = 0;
				foreach (ResourceBundle bundle in this.bundles)
				{
					if (bundle != this.primaryBundle)
					{
						if (bundle[druid].IsEmpty || bundle[druid].ModificationId < id)
						{
							count++;
						}
					}
				}
				return (count != this.bundles.Count-1);
			}

			return false;
		}


		public void SearcherIndexToAccess(int field, string secondaryCulture, out string cultureName, out string fieldName)
		{
			//	Conversion d'un index de champ (0..n) en l'information nécessaire pour Get/SetField.
			if (this.type == Type.Strings)
			{
				switch (field)
				{
					case 0:
						cultureName = null;
						fieldName = "Name";
						return;

					case 1:
						cultureName = null;
						fieldName = "String";
						return;

					case 3:
						cultureName = null;
						fieldName = "About";
						return;
				}

				if (secondaryCulture != null)
				{
					switch (field)
					{
						case 2:
							cultureName = secondaryCulture;
							fieldName = "String";
							return;

						case 4:
							cultureName = secondaryCulture;
							fieldName = "About";
							return;
					}
				}
			}

			if (this.type == Type.Captions)
			{
				switch (field)
				{
					case 0:
						cultureName = null;
						fieldName = "Name";
						return;

					case 1:
						cultureName = null;
						fieldName = "Labels";
						return;

					case 3:
						cultureName = null;
						fieldName = "Description";
						return;

					case 5:
						cultureName = null;
						fieldName = "About";
						return;
				}

				if (secondaryCulture != null)
				{
					switch (field)
					{
						case 2:
							cultureName = secondaryCulture;
							fieldName = "Labels";
							return;

						case 4:
							cultureName = secondaryCulture;
							fieldName = "Description";
							return;

						case 6:
							cultureName = secondaryCulture;
							fieldName = "About";
							return;
					}
				}
			}

			if (this.type == Type.Panels)
			{
				cultureName = null;

				if (field == 0)
				{
					fieldName = "Name";
					return;
				}
			}

			cultureName = null;
			fieldName = null;
		}


		protected void CacheResource(int index, string cultureName)
		{
			//	Cache une ressource.
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
						this.accessBundle = this.GetCultureBundle(this.accessCulture);
					}
				}

				if (this.accessBundle == null || index < 0 || index >= this.druidsIndex.Count)
				{
					this.accessCached = -1;
					return;
				}

				//	Met en cache le ResourceBundle.Field.
				if (this.accessCached != index || this.accessField == null)
				{
					Druid druid = this.druidsIndex[index];
					this.accessField = this.accessBundle[druid];
				}

				//	Met en cache le Caption.
				if (this.type == Type.Captions)
				{
					if (this.accessCached != index || this.accessCaption == null)
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

			this.accessCached = index;
		}

		protected void CacheClear()
		{
			//	Vide le cache.
			if (this.IsBundlesType)
			{
				this.accessCulture = "?";  // nom différent de null, d'une chaîne vide ou d'un nom existant
				this.accessField = null;
				this.accessCaption = null;
				this.accessCached = -1;
			}
		}

		protected void CreateIfNecessary()
		{
			//	Crée une ressource secondaire, si nécessaire.
			if (this.accessBundle != this.primaryBundle && this.accessField.IsEmpty)
			{
				Druid druid = this.druidsIndex[this.accessIndex];
				ResourceBundle.Field defaultField = this.primaryBundle[druid];
				this.accessField = this.accessBundle.CreateField(ResourceFieldType.Data);
				this.accessField.SetName(defaultField.Name);
				this.accessField.SetDruid(druid);
				this.accessField.SetModificationId(defaultField.ModificationId);

				this.accessBundle.Add(this.accessField);
			}
		}

		protected static string[] NameStrings = { "Name", "String", "About" };
		protected static string[] NameCaptions = { "Name", "Labels", "Description", "Icon", "About" };
		protected static string[] NamePanels = { "Name", "Panel" };

		protected static Field.Type[] TypeStrings = { Field.Type.String, Field.Type.String, Field.Type.String };
		protected static Field.Type[] TypeCaptions = { Field.Type.String, Field.Type.StringCollection, Field.Type.String, Field.Type.String, Field.Type.String };
		protected static Field.Type[] TypePanels = { Field.Type.String, Field.Type.Bundle };


		public int CultureCount
		{
			//	Retourne le nombre de cultures.
			get
			{
				return this.bundles.Count;
			}
		}

		public ResourceBundle GetCultureBundle(string cultureName)
		{
			//	Cherche le bundle d'une culture.
			System.Diagnostics.Debug.Assert(cultureName.Length == 2);
			for (int b=0; b<bundles.Count; b++)
			{
				ResourceBundle bundle = bundles[b];
				if (Misc.CultureBaseName(bundle.Culture) == cultureName)
				{
					return bundle;
				}
			}
			return null;
		}

		public string GetBaseCultureName()
		{
			//	Retourne le nom de la culture de base.
			if (this.IsBundlesType)
			{
				return this.primaryBundle.Culture.Name;
			}

			return null;
		}

		public List<string> GetSecondaryCultureNames()
		{
			//	Retourne la liste des cultures secondaires, triés par ordre alphabétique.
			List<string> list = new List<string>();

			if (this.IsBundlesType)
			{
				if (this.bundles.Count > 1)
				{
					for (int b=0; b<this.bundles.Count; b++)
					{
						ResourceBundle bundle = this.bundles[b];
						if (bundle != this.primaryBundle)
						{
							list.Add(bundle.Culture.Name);
						}
					}

					list.Sort();
				}
			}

			return list;
		}

		public bool IsExistingCulture(string cultureName)
		{
			//	Indique si une culture donnée existe.
			System.Diagnostics.Debug.Assert(cultureName.Length == 2);
			if (this.IsBundlesType)
			{
				for (int b=0; b<this.bundles.Count; b++)
				{
					ResourceBundle bundle = this.bundles[b];
					if (cultureName == bundle.Culture.Name)
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
			System.Diagnostics.Debug.Assert(cultureName.Length == 2);
			if (this.IsBundlesType)
			{
				string prefix = this.resourceManager.ActivePrefix;
				System.Globalization.CultureInfo culture = Resources.FindSpecificCultureInfo(cultureName);
				ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, this.bundles.Name, ResourceLevel.Localized, culture);

				bundle.DefineType(this.BundleName(false));
				this.resourceManager.SetBundle(bundle, ResourceSetMode.CreateOnly);

				this.LoadBundles();
				this.IsDirty = true;
			}
		}

		public void DeleteCulture(string cultureName)
		{
			//	Supprime une culture.
			System.Diagnostics.Debug.Assert(cultureName.Length == 2);
			if (this.IsBundlesType)
			{
				ResourceBundle bundle = this.GetCultureBundle(cultureName);
				if (bundle != null)
				{
					this.resourceManager.RemoveBundle(this.BundleName(true), ResourceLevel.Localized, bundle.Culture);
					this.LoadBundles();
					this.IsDirty = true;
				}
			}
		}


		protected void LoadBundles()
		{
			string[] ids = this.resourceManager.GetBundleIds("*", this.BundleName(false), ResourceLevel.Default);
			if (ids.Length == 0)
			{
				//	Crée un premier bundle vide.
				string prefix = this.resourceManager.ActivePrefix;
				System.Globalization.CultureInfo culture = this.BaseCulture;
				ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, this.BundleName(true), ResourceLevel.Default, culture);
				bundle.DefineType(this.BundleName(false));

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

				ids = this.resourceManager.GetBundleIds("*", this.BundleName(false), ResourceLevel.Default);
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

		protected void SetFilterPanels(string filter, Searcher.SearchingMode mode)
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

			foreach (ResourceBundle bundle in this.panelsList)
			{
				string name = this.SubFilter(bundle.Caption);

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

				this.druidsIndex.Add(bundle.Druid);
			}
		}

		protected int GetAbsoluteIndex(Druid druid)
		{
			//	Cherche l'index absolu d'une ressource d'après son druid.
			if (this.IsBundlesType)
			{
				ResourceBundle.Field field = this.primaryBundle[druid];
				return this.primaryBundle.IndexOf(field);
			}

			if (this.type == Type.Panels)
			{
				for (int i=0; i<this.panelsList.Count; i++)
				{
					if (this.panelsList[i].Druid == druid)
					{
						return i;
					}
				}
			}

			return -1;
		}

		protected Druid CreateUniqueDruid()
		{
			//	Crée un nouveau druid unique.
			if (this.IsBundlesType)
			{
				int moduleId = this.primaryBundle.Module.Id;
				int developerId = 0;  // [PA] provisoire
				int localId = 0;

				foreach (ResourceBundle.Field field in this.primaryBundle.Fields)
				{
					Druid druid = field.Druid;

					if (druid.IsValid && druid.Developer == developerId && druid.Local >= localId)
					{
						localId = druid.Local+1;
					}
				}

				return new Druid(moduleId, developerId, localId);
			}

			if (this.type == Type.Panels)
			{
				int moduleId = this.moduleInfo.Id;
				int developerId = 0;  // [PA] provisoire
				int localId = 0;

				foreach (ResourceBundle bundle in this.panelsList)
				{
					Druid druid = bundle.Druid;

					if (druid.IsValid && druid.Developer == developerId && druid.Local >= localId)
					{
						localId = druid.Local+1;
					}
				}

				return new Druid(moduleId, developerId, localId);
			}

			return Druid.Empty;
		}


		protected void LoadPanels()
		{
			if (this.panelsList != null)
			{
				return;
			}

			this.panelsList = new List<ResourceBundle>();
			this.panelsToCreate = new List<ResourceBundle>();
			this.panelsToDelete = new List<ResourceBundle>();

			string[] names = this.resourceManager.GetBundleIds("*", this.BundleName(false), ResourceLevel.Default);
			if (names.Length == 0)
			{
				//	S'il n'existe aucun panneau, crée un premier panneau vide.
				//	Ceci est nécessaire, car il n'existe pas de commande pour créer un panneau à partir
				//	de rien, mais seulement une commande pour dupliquer un panneau existant.
				Druid druid = this.CreateUniqueDruid();
				string prefix = this.resourceManager.ActivePrefix;
				System.Globalization.CultureInfo culture = this.BaseCulture;
				ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, druid.ToBundleId(), ResourceLevel.Default, culture);

				bundle.DefineType(this.BundleName(false));
				bundle.DefineCaption(Res.Strings.Viewers.Panels.New);
				bundle.DefineRank(0);

				this.panelsList.Add(bundle);
				this.panelsToCreate.Add(bundle);
			}
			else
			{
				foreach (string name in names)
				{
					ResourceBundle bundle = this.resourceManager.GetBundle(name, ResourceLevel.Default);
					this.panelsList.Add(bundle);

					ResourceBundle.Field field = bundle[this.BundleName(false)];

					if (field.IsValid)
					{
						UI.Panel panel = UserInterface.DeserializePanel(field.AsString, this.resourceManager);
						panel.DrawDesignerFrame = true;
						Viewers.Panels.SetPanel(bundle, panel);
					}
				}

				this.panelsList.Sort(new Comparers.BundleRank());  // trie selon les rangs
			}
		}

		protected void SavePanels()
		{
			if (this.panelsList == null)
			{
				return;
			}

			for (int i=0; i<this.panelsList.Count; i++)
			{
				ResourceBundle bundle = this.panelsList[i];
				bundle.DefineRank(i);
				UI.Panel panel = Viewers.Panels.GetPanel(bundle);

				if (panel != null)
				{
					if (!bundle.Contains(this.BundleName(false)))
					{
						ResourceBundle.Field field = bundle.CreateField(ResourceFieldType.Data);
						field.SetName(this.BundleName(false));
						bundle.Add(field);
					}

					bundle[this.BundleName(false)].SetXmlValue(UserInterface.SerializePanel(panel));
				}

				if (this.panelsToCreate.Contains(bundle))
				{
					this.resourceManager.SetBundle(bundle, ResourceSetMode.CreateOnly);
				}
				else
				{
					this.resourceManager.SetBundle(bundle, ResourceSetMode.UpdateOnly);
				}
			}
			this.panelsToCreate.Clear();

			//	Supprime tous les panneaux mis dans la liste 'à supprimer'.
			foreach (ResourceBundle bundle in this.panelsToDelete)
			{
				this.resourceManager.RemoveBundle(bundle.Druid.ToBundleId(), ResourceLevel.Default, bundle.Culture);
			}
			this.panelsToDelete.Clear();
		}

		public UI.Panel NewPanel(int index)
		{
			//	Crée le UI.Panel associé à une ressource.
			ResourceBundle bundle = this.PanelBundle(index);

			UI.Panel newPanel = Viewers.Panels.GetPanel(bundle);

			if (newPanel == null)
			{
				newPanel = this.CreateEmptyPanel();
				Viewers.Panels.SetPanel(bundle, newPanel);
			}

			return newPanel;
		}

		public UI.Panel CreateEmptyPanel()
		{
			UI.Panel panel = new UI.Panel();
			panel.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Anchored;
			//?panel.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Docked;
			//?panel.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			panel.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
			panel.PreferredSize = new Size(200, 200);
			panel.Anchor = AnchorStyles.BottomLeft;
			panel.Padding = new Margins(20, 20, 20, 20);
			panel.DrawDesignerFrame = true;

			return panel;
		}

		protected ResourceBundle PanelBundle(int index)
		{
			//	Donne le bundle d'un panneau en fonction de l'index du druid.
			Druid druid = this.druidsIndex[index];
			int i = this.GetAbsoluteIndex(druid);
			if (i == -1)
			{
				return null;
			}
			else
			{
				return this.panelsList[i];
			}
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

		protected string BundleName(bool many)
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

				case Type.Panels:
					return "Panel";
			}

			return null;
		}


		#region FixFilter
		protected string AddFilter(string name)
		{
			//	Ajoute le filtre fixe si nécessaire.
			if (!this.HasFixFilter(name))
			{
				string fix = this.FixFilter;
				if (fix == null)
				{
					return name;
				}
				else
				{
					return fix + name;
				}
			}

			return name;
		}

		protected string SubFilter(string name)
		{
			//	Supprime le filtre fixe si nécessaire.
			if (this.HasFixFilter(name))
			{
				string fix = this.FixFilter;
				if (fix == null)
				{
					return name;
				}
				else
				{
					return name.Substring(fix.Length);
				}
			}

			return name;
		}

		protected bool HasFixFilter(string name)
		{
			//	Indique si un nom commence par le filtre fixe.
			string fix = this.FixFilter;
			
			if (fix == null)
			{
				return true;
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

			public Field(string value)
			{
				this.type = Type.String;
				this.stringValue = value;
			}

			public Field(ICollection<string> value)
			{
				this.type = Type.StringCollection;
				this.stringCollection = value;
			}

			public Field(ResourceBundle value)
			{
				this.type = Type.Bundle;
				this.bundle = value;
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
			}

			public ICollection<string> StringCollection
			{
				get
				{
					System.Diagnostics.Debug.Assert(this.type == Type.StringCollection);
					return this.stringCollection;
				}
			}

			public ResourceBundle Bundle
			{
				get
				{
					System.Diagnostics.Debug.Assert(this.type == Type.Bundle);
					return this.bundle;
				}
			}

			protected Type						type;
			protected string					stringValue;
			protected ICollection<string>		stringCollection;
			protected ResourceBundle			bundle;
		}
		#endregion


		#region Events handler
		protected virtual void OnDirtyChanged()
		{
			if (this.DirtyChanged != null)  // qq'un écoute ?
			{
				this.DirtyChanged(this);
			}
		}

		public event Support.EventHandler DirtyChanged;
		#endregion


		protected Type							type;
		protected ResourceManager				resourceManager;
		protected ResourceModuleInfo			moduleInfo;
		protected bool							isDirty = false;

		protected ResourceBundleCollection		bundles;
		protected ResourceBundle				primaryBundle;
		protected List<Druid>					druidsIndex;
		protected string						accessCulture;
		protected ResourceBundle				accessBundle;
		protected int							accessIndex;
		protected int							accessCached;
		protected ResourceBundle.Field			accessField;
		protected Common.Types.Caption			accessCaption;
		protected List<ResourceBundle>			panelsList;
		protected List<ResourceBundle>			panelsToCreate;
		protected List<ResourceBundle>			panelsToDelete;
		protected string[]						filterStrings;
		protected Searcher.SearchingMode[]		filterModes;
	}
}
