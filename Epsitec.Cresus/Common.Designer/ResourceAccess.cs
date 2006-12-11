using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Acc�s g�n�ralis� aux ressources.
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
			Values,
			Panels,
			Scripts,
		}

		public enum FieldType
		{
			None,
			Name,
			String,
			Labels,
			Description,
			Icon,
			About,
			Statefull,
			Shortcuts,
			Group,
			Controller,
			AbstractType,
			Panel,
			Caption,
		}

		public enum TypeType
		{
			None,
			Void,
			Boolean,
			Integer,
			LongInteger,
			Double,
			Decimal,
			String,
			Enum,
			Structured,
			Collection,
			Date,
			Time,
			DateTime,
			Binary,
		}

		public enum ModificationState
		{
			Normal,			//	d�fini normalement
			Empty,			//	vide (fond rouge)
			Modified,		//	modifi� (fond jaune)
		}


		public ResourceAccess(Type type, ResourceManager resourceManager, ResourceModuleInfo moduleInfo, MainWindow mainWindow)
		{
			//	Constructeur unique pour acc�der aux ressources d'un type donn�.
			//	Par la suite, l'instance cr��e acc�dera toujours aux ressources de ce type,
			//	sauf pour les ressources Captions, Commands, Types et Values.
			this.type = type;
			this.resourceManager = resourceManager;
			this.moduleInfo = moduleInfo;
			this.mainWindow = mainWindow;

			this.druidsIndex = new List<Druid>();

			this.captionCounters = new Dictionary<Type, int>();
			this.captionCounters[Type.Captions] = 0;
			this.captionCounters[Type.Commands] = 0;
			this.captionCounters[Type.Types] = 0;
			this.captionCounters[Type.Values] = 0;

			this.filterIndexes = new Dictionary<Type, int>();
			this.filterStrings = new Dictionary<Type, string>();
			this.filterModes = new Dictionary<Type, Searcher.SearchingMode>();
		}


		public Type ResourceType
		{
			//	Type des ressources acc�d�es.
			get
			{
				return this.type;
			}

			set
			{
				if (this.type != value)
				{
					System.Diagnostics.Debug.Assert(this.IsCaptionsType);
					this.type = value;
					System.Diagnostics.Debug.Assert(this.IsCaptionsType);

					//	Remet le filtre correspondant au type.
					string filter = "";
					if (this.filterStrings.ContainsKey(this.type))
					{
						filter = this.filterStrings[this.type];
					}

					Searcher.SearchingMode mode = Searcher.SearchingMode.None;
					if (this.filterModes.ContainsKey(this.type))
					{
						mode = this.filterModes[this.type];
					}

					this.SetFilter(filter, mode);

					int index = 0;
					if (this.filterIndexes.ContainsKey(this.type))
					{
						index = this.filterIndexes[this.type];
					}

					this.AccessIndex = index;
				}
			}
		}


		public static string TypeDisplayName(Type type)
		{
			//	Retourne le nom au pluriel correspondant � un type.
			return ResourceAccess.TypeDisplayName(type, true);
		}

		public static string TypeDisplayName(Type type, bool many)
		{
			//	Retourne le nom au singulier ou au pluriel correspondant � un type.
			switch (type)
			{
				case Type.Strings:
					return many ? Res.Strings.BundleType.Strings : Res.Strings.BundleType.String;

				case Type.Captions:
					return many ? Res.Strings.BundleType.Captions : Res.Strings.BundleType.Caption;

				case Type.Commands:
					return many ? Res.Strings.BundleType.Commands : Res.Strings.BundleType.Command;

				case Type.Types:
					return many ? Res.Strings.BundleType.Types : Res.Strings.BundleType.Type;

				case Type.Values:
					return many ? Res.Strings.BundleType.Values : Res.Strings.BundleType.Value;

				case Type.Panels:
					return many ? Res.Strings.BundleType.Panels : Res.Strings.BundleType.Panel;

				case Type.Scripts:
					return many ? Res.Strings.BundleType.Scripts : Res.Strings.BundleType.Script;
			}

			return "?";
		}


		public static TypeType AbstractTypeToTypeType(AbstractType type)
		{
			if (type is VoidType       )  return TypeType.Void;
			if (type is BooleanType    )  return TypeType.Boolean;
			if (type is IntegerType    )  return TypeType.Integer;
			if (type is LongIntegerType)  return TypeType.LongInteger;
			if (type is DoubleType     )  return TypeType.Double;
			if (type is DecimalType    )  return TypeType.Decimal;
			if (type is StringType     )  return TypeType.String;
			if (type is EnumType       )  return TypeType.Enum;
			if (type is StructuredType )  return TypeType.Structured;
			if (type is CollectionType )  return TypeType.Collection;
			if (type is DateType       )  return TypeType.Date;
			if (type is TimeType       )  return TypeType.Time;
			if (type is DateTimeType   )  return TypeType.DateTime;
			if (type is BinaryType     )  return TypeType.Binary;

			return TypeType.None;
		}

		protected static string TypeTypeController(AbstractType type)
		{
			if (type is VoidType             )  return null;
			if (type is BooleanType          )  return "Boolean";
			if (type is IntegerType          )  return "Numeric";
			if (type is LongIntegerType      )  return "Numeric";
			if (type is DoubleType           )  return "Numeric";
			if (type is DecimalType          )  return "Numeric";
			if (type is StringType           )  return "String";
			if (type is EnumType             )  return "Enum";
			if (type is StructuredType       )  return null;
			if (type is CollectionType       )  return null;
			if (type is AbstractDateTimeType )  return "DateTime";
			if (type is BinaryType           )  return "Binary";

			return null;
		}

		protected static string TypeTypeControllerParameter(AbstractType type)
		{
			if (type is VoidType       )  return null;
			if (type is BooleanType    )  return null;
			if (type is IntegerType    )  return null;
			if (type is LongIntegerType)  return null;
			if (type is DoubleType     )  return null;
			if (type is DecimalType    )  return null;
			if (type is StringType     )  return null;
			if (type is EnumType       )  return "Icons";
			if (type is StructuredType )  return null;
			if (type is CollectionType )  return null;
			if (type is DateType       )  return null;
			if (type is TimeType       )  return null;
			if (type is DateTimeType   )  return null;
			if (type is BinaryType     )  return null;

			return null;
		}

		protected static AbstractType TypeTypeCreate(TypeType type)
		{
			switch (type)
			{
				case TypeType.Void:         return new VoidType();
				case TypeType.Boolean:      return new BooleanType();
				case TypeType.Integer:      return new IntegerType();
				case TypeType.LongInteger:  return new LongIntegerType();
				case TypeType.Double:       return new DoubleType();
				case TypeType.Decimal:      return new DecimalType();
				case TypeType.String:       return new StringType();
				case TypeType.Enum:         return new EnumType();
				case TypeType.Structured:   return new StructuredType();
				case TypeType.Collection:   return new CollectionType();
				case TypeType.Date:         return new DateType();
				case TypeType.Time:         return new TimeType();
				case TypeType.DateTime:     return new DateTimeType();
				case TypeType.Binary:       return new BinaryType();
			}

			return null;
		}

		public static string TypeTypeToDisplay(TypeType type)
		{
			switch (type)
			{
				case TypeType.Void:         return Res.Strings.Viewers.Types.Editor.Void;
				case TypeType.Boolean:      return Res.Strings.Viewers.Types.Editor.Boolean;
				case TypeType.Integer:      return Res.Strings.Viewers.Types.Editor.Integer;
				case TypeType.LongInteger:  return Res.Strings.Viewers.Types.Editor.LongInteger;
				case TypeType.Decimal:      return Res.Strings.Viewers.Types.Editor.Decimal;
				case TypeType.String:       return Res.Strings.Viewers.Types.Editor.String;
				case TypeType.Enum:         return Res.Strings.Viewers.Types.Editor.Enum;
				case TypeType.Structured:   return Res.Strings.Viewers.Types.Editor.Structured;
				case TypeType.Collection:   return Res.Strings.Viewers.Types.Editor.Collection;
				case TypeType.Date:         return Res.Strings.Viewers.Types.Editor.Date;
				case TypeType.Time:         return Res.Strings.Viewers.Types.Editor.Time;
				case TypeType.DateTime:     return Res.Strings.Viewers.Types.Editor.DateTime;
				case TypeType.Binary:       return Res.Strings.Viewers.Types.Editor.Binary;
			}

			return null;
		}

		public static string TypeTypeToName(TypeType type)
		{
			switch (type)
			{
				case TypeType.Void:         return "Void";
				case TypeType.Boolean:      return "Boolean";
				case TypeType.Integer:      return "Integer";
				case TypeType.LongInteger:  return "LongInteger";
				case TypeType.Double:       return "Double";
				case TypeType.Decimal:      return "Decimal";
				case TypeType.String:       return "String";
				case TypeType.Enum:         return "Enum";
				case TypeType.Structured:   return "Structured";
				case TypeType.Collection:   return "Collection";
				case TypeType.Date:         return "Date";
				case TypeType.Time:         return "Time";
				case TypeType.DateTime:     return "DateTime";
				case TypeType.Binary:       return "Binary";
			}

			return null;
		}

		public static TypeType NameToTypeType(string name)
		{
			switch (name)
			{
				case "Void":         return TypeType.Void;
				case "Boolean":      return TypeType.Boolean;
				case "Integer":      return TypeType.Integer;
				case "LongInteger":  return TypeType.LongInteger;
				case "Double":       return TypeType.Double;
				case "Decimal":      return TypeType.Decimal;
				case "String":       return TypeType.String;
				case "Enum":         return TypeType.Enum;
				case "Structured":   return TypeType.Structured;
				case "Collection":   return TypeType.Collection;
				case "Date":         return TypeType.Date;
				case "Time":         return TypeType.Time;
				case "DateTime":     return TypeType.DateTime;
				case "Binary":       return TypeType.Binary;
			}

			return TypeType.None;
		}


		public void Load()
		{
			//	Charge les ressources.
			if (this.IsBundlesType)
			{
				this.LoadBundles();
				this.AdjustBundlesAfterLoad();
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
				this.AdjustBundlesBeforeSave();
				this.SaveBundles();
			}

			if (this.type == Type.Panels)
			{
				this.SavePanels();
			}

			this.IsDirty = false;
		}


		public void AddShortcuts(List<ShortcutItem> list)
		{
			//	Ajoute tous les raccourcis d�finis dans la liste.
			if (this.IsCaptionsType)
			{
				foreach (ResourceBundle bundle in this.bundles)
				{
					this.AddShortcutsCaptions(bundle, list);
				}
			}
		}

		public static void CheckShortcuts(System.Text.StringBuilder builder, List<ShortcutItem> list)
		{
			//	V�rifie les raccourcis, en construisant un message d'avertissement
			//	pour tous les raccourcis utilis�s plus d'une fois.
			string culture = null;
			List<ShortcutItem> uses = new List<ShortcutItem>();

			for (int i=0; i<list.Count; i++)
			{
				if (ShortcutItem.Contains(uses, list[i]))  // raccourci d�j� trait� ?
				{
					continue;
				}

				List<int> indexes = ShortcutItem.IndexesOf(list, i);
				if (indexes == null)  // utilis� une seule fois ?
				{
					continue;
				}

				if (culture == null || culture != list[i].Culture)  // autre culture ?
				{
					if (builder.Length > 0)  // d�j� d'autres avertissements ?
					{
						builder.Append("<br/><br/>");
					}

					builder.Append("<font size=\"120%\">");
					builder.Append(string.Format(Res.Strings.Error.ShortcutMany, list[i].Culture));
					builder.Append("</font><br/><br/>");
					culture = list[i].Culture;
				}

				builder.Append("<list type=\"fix\" width=\"1.5\"/><b>");
				builder.Append(Message.GetKeyName(list[i].Shortcut.KeyCode));  // nom du raccourci
				builder.Append("</b>: ");

				for (int s=0; s<indexes.Count; s++)
				{
					builder.Append(list[indexes[s]].Name);  // nom de la commande utilisant le raccourci

					if (s < indexes.Count-1)
					{
						builder.Append(", ");
					}
				}
				builder.Append("<br/>");

				uses.Add(list[i]);  // ajoute � la liste des raccourcis trait�s
			}
		}


		public bool IsDirty
		{
			//	Est-ce que les ressources ont �t� modifi�es.
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
			EnumType et = null;
			StructuredType st = null;

			if (this.type == Type.Types && !duplicateContent)
			{
				TypeType tt = this.lastTypeTypeCreatated;
				this.mainWindow.DlgResourceTypeType(this, ref tt, out this.lastTypeTypeSystem);
				if (tt == TypeType.None)  // annuler ?
				{
					return;
				}
				this.lastTypeTypeCreatated = tt;

				if (this.lastTypeTypeCreatated == TypeType.Enum && this.lastTypeTypeSystem != null)
				{
					et = this.CreateEnumValues(this.lastTypeTypeSystem);
					newName = this.GetEnumBaseName(this.lastTypeTypeSystem);
				}
			}

			if (this.type == Type.Panels && !duplicateContent)
			{
				//	Choix d'une ressource type de type 'Types', mais uniquement parmi les TypeType.Structured.
				Module module = this.mainWindow.CurrentModule;
				Druid druid = this.mainWindow.DlgResourceSelector(module, ResourceAccess.Type.Types, ResourceAccess.TypeType.Structured, Druid.Empty, null);
				if (druid.IsEmpty)  // annuler ?
				{
					return;
				}
				st = module.AccessCaptions.DirectGetAbstractType(druid) as StructuredType;
				System.Diagnostics.Debug.Assert(st != null);
			}

			Druid newDruid = Druid.Empty;

			if (this.IsBundlesType)
			{
				newName = this.AddFilter(newName, false);

				Druid actualDruid = this.druidsIndex[this.accessIndex];
				int aIndex = this.GetAbsoluteIndex(actualDruid);
				newDruid = this.CreateUniqueDruid();

				foreach (ResourceBundle bundle in this.bundles)
				{
					ResourceBundle.Field newField = bundle.CreateField(ResourceFieldType.Data);
					newField.SetDruid(newDruid);
					
					if (bundle == this.primaryBundle)
					{
						newField.SetName(newName);
					}

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

					if (this.type == Type.Types)
					{
						if (string.IsNullOrEmpty(newField.AsString))
						{
							AbstractType type;

							if (et == null)
							{
								type = ResourceAccess.TypeTypeCreate(this.lastTypeTypeCreatated);
							}
							else
							{
								type = et;
							}

							type.DefineDefaultController(ResourceAccess.TypeTypeController(type), ResourceAccess.TypeTypeControllerParameter(type));
							newField.SetStringValue(type.Caption.SerializeToString());
						}
					}

					if (this.IsCaptionsType)
					{
						Caption caption = new Caption();

						string s = newField.AsString;
						if (!string.IsNullOrEmpty(s))
						{
							caption.DeserializeFromString(s, this.resourceManager);
						}

						this.AdjustCaptionName(bundle, newField, caption);
						newField.SetStringValue(caption.SerializeToString());
					}

					if (bundle == this.primaryBundle)
					{
						//?newField.SetModificationId(1);
						bundle.Insert(aIndex+1, newField);
					}
					else
					{
						//?newField.SetModificationId(0);
						bundle.Add(newField);
					}
				}

				if (this.IsCaptionsType)
				{
					this.CaptionsCountersModify(1);
				}
			}

			if (this.type == Type.Panels)
			{
				newDruid = this.CreateUniqueDruid();
				ResourceBundle newBundle = null;

				if (duplicateContent)
				{
					ResourceBundle actualBundle = this.panelsList[this.accessIndex];
					newBundle = actualBundle.Clone();
					newBundle.DefineName(newDruid.ToBundleId());
					newBundle.DefineCaption(newName);

					UI.Panel actualPanel = UI.Panel.GetPanel(actualBundle);
					UI.Panel newPanel = UserInterface.Duplicate(actualPanel, this.resourceManager) as UI.Panel;

					UI.Panel.SetPanel(newBundle, newPanel);
					newPanel.SetupSampleDataSource();
				}
				else
				{
					string prefix = this.resourceManager.ActivePrefix;
					System.Globalization.CultureInfo culture = this.BaseCulture;
					newBundle = ResourceBundle.Create(this.resourceManager, prefix, newDruid.ToBundleId(), ResourceLevel.Default, culture);

					newBundle.DefineType(this.BundleName(false));
					newBundle.DefineCaption(newName);
				}

				this.panelsList.Insert(this.accessIndex, newBundle);
				this.panelsToCreate.Add(newBundle);
				
				this.resourceManager.SetBundle(newBundle, ResourceSetMode.None);

				if (st != null)
				{
					UI.Panel newPanel = this.GetPanel(newBundle);
					ObjectModifier.SetStructuredType(newPanel, st);
				}
			}

#if false
			this.druidsIndex.Insert(this.accessIndex+1, newDruid);
			this.accessIndex ++;
			this.CacheClear();
#else
			this.druidsIndex.Add(newDruid);
			this.Sort();
			this.accessIndex = this.druidsIndex.IndexOf(newDruid);
			this.CacheClear();
#endif

			this.IsDirty = true;
		}

		protected EnumType CreateEnumValues(System.Type stype)
		{
			EnumType et = new EnumType(stype, new Caption());

			foreach (EnumValue value in et.EnumValues)
			{
				if (!value.IsHidden)
				{
					string name = this.GetEnumBaseName(stype);
					name = string.Concat(name, ".", value.Name);
					string newName = string.Concat(ResourceAccess.GetFixFilter(Type.Values), name);

					if (this.primaryBundle.IndexOf(newName) == -1)
					{
						Druid newDruid = this.CreateUniqueDruid();
						ResourceBundle.Field newField = this.primaryBundle.CreateField(ResourceFieldType.Data);
						newField.SetDruid(newDruid);
						newField.SetName(newName);

						Caption caption = value.Caption;
						caption.DefineDruid(newDruid);
						
						System.Diagnostics.Debug.Assert(caption.Name == value.Name);
						System.Diagnostics.Debug.Assert(value.IsNative);

						newField.SetStringValue(caption.SerializeToString());
						value.DefineCaption(caption);

						this.primaryBundle.Add(newField);
					}
				}
			}

			return et;
		}

		public string GetEnumBaseName(System.Type stype)
		{
			//	Retourne le nom de base � utiliser pour une �num�ration native C#.
			string name = stype.FullName.Replace('+', '.');

			//	Enl�ve le pr�fixe "Epsitec.Common." s'il existe.
			if (name.StartsWith(ResourceAccess.filterPrefix))
			{
				name = name.Substring(ResourceAccess.filterPrefix.Length);
			}

			//	Enl�ve son propre nom de module s'il existe.
			string module = string.Concat(ResourceAccess.LastName(this.moduleInfo.Name), ".");
			if (name.StartsWith(module))
			{
				name = name.Substring(module.Length);
			}

			return name;
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

				if (this.IsCaptionsType)
				{
					this.CaptionsCountersModify(-1);
				}
			}

			if (this.type == Type.Panels)
			{
				ResourceBundle bundle = this.PanelBundle(this.accessIndex);

				this.panelsList.Remove(bundle);

				//	S'il ne s'agit pas d'une nouvelle ressource, il faut l'ajouter dans la liste
				//	des ressources � d�truire (lors du PanelsWrite).
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

#if false
		public void Move(int direction)
		{
			//	D�place la ressource courante.
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
#endif


		public void SetFilter(string filter, Searcher.SearchingMode mode)
		{
			//	Construit l'index en fonction des ressources primaires.
			Druid druid = Druid.Empty;
			if (this.accessIndex >= 0 && this.accessIndex < this.druidsIndex.Count)
			{
				druid = this.druidsIndex[this.accessIndex];
			}

			//	Met � jour druidsIndex.
			if (this.IsBundlesType)
			{
				this.SetFilterBundles(filter, mode);
			}

			if (this.type == Type.Panels)
			{
				this.SetFilterPanels(filter, mode);
			}

			//	Trie druidsIndex.
			this.Sort();

			//	M�morise le filtre utilis�.
			if (this.filterStrings.ContainsKey(this.type))
			{
				this.filterStrings[this.type] = filter;
			}
			else
			{
				this.filterStrings.Add(this.type, filter);
			}

			if (this.filterModes.ContainsKey(this.type))
			{
				this.filterModes[this.type] = mode;
			}
			else
			{
				this.filterModes.Add(this.type, mode);
			}

			//	Cherche l'index correspondant � la ressource d'avant le changement de filtre.
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
			//	Retourne le nombre de donn�es accessibles.
			get
			{
				if (this.IsCaptionsType)
				{
					return this.captionCounters[this.type];
				}

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
			//	Retourne le nombre de donn�es accessibles.
			get
			{
				return this.druidsIndex.Count;
			}
		}

		public Druid AccessDruid(int index)
		{
			//	Retourne le druid d'un index donn�.
			return this.druidsIndex[index];
		}

		public int AccessIndex
		{
			//	Index de l'acc�s en cours.
			get
			{
				return this.accessIndex;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, this.druidsIndex.Count-1);
				this.accessIndex = value;

				if (this.filterIndexes.ContainsKey(this.type))
				{
					this.filterIndexes[this.type] = value;
				}
				else
				{
					this.filterIndexes.Add(this.type, value);
				}
			}
		}


		public bool IsCorrectNewName(ref string name, bool bypass)
		{
			//	Retourne true s'il est possible de cr�er cette nouvelle ressource.
			return (this.CheckNewName(ref name, bypass) == null);
		}

		public string CheckNewName(ref string name, bool bypass)
		{
			//	Retourne l'�ventuelle erreur si on tente de cr�er cette nouvelle ressource.
			//	Retourne null si tout est correct.
			System.Diagnostics.Debug.Assert(!bypass || this.bypassType != Type.Unknow);

			if (!Misc.IsValidLabel(ref name))
			{
				return Res.Strings.Error.Name.Invalid;
			}

			//	Refuse "Abc.Abc", "Toto.Abc.Abc", "Abc.Abc.Toto" ou "Toto.Abc.Abc.Titi"
			//	Accepte "Abc.Toto.Abc"
			string[] sub = name.Split('.');
			for (int i=0; i<sub.Length-1; i++)
			{
				if (sub[i] == sub[i+1])  // deux parties successives identiques ?
				{
					return Res.Strings.Error.Name.Twofold;
				}
			}

			//	Cherche si le nom existe d�j�.
			string err;
			if (bypass)
			{
				string n = this.AddFilter(name, true);

				foreach (Druid druid in this.bypassDruids)
				{
					ResourceBundle.Field field = this.primaryBundle[druid];
					if (field != null && field.Name != null)
					{
						err = ResourceAccess.CheckNames(field.Name, n);
						if (err != null)
						{
							return err;
						}
					}
				}

				if (this.bypassExclude != null)
				{
					foreach (Druid druid in this.bypassExclude)
					{
						ResourceBundle.Field field = this.primaryBundle[druid];
						if (field != null && field.Name != null)
						{
							err = ResourceAccess.CheckNames(field.Name, n);
							if (err != null)
							{
								return err;
							}
						}
					}
				}
			}
			else
			{
				if (this.IsBundlesType)
				{
					string n = this.AddFilter(name, false);
					foreach (ResourceBundle.Field field in this.primaryBundle.Fields)
					{
						if (field != null && field.Name != null)
						{
							err = ResourceAccess.CheckNames(field.Name, n);
							if (err != null)
							{
								return err;
							}
						}
					}
				}

				if (this.type == Type.Panels)
				{
					foreach (ResourceBundle bundle in this.panelsList)
					{
						err = ResourceAccess.CheckNames(bundle.Caption, name);
						if (err != null)
						{
							return err;
						}
					}
				}
			}

			return null;  // ok
		}

		protected static string CheckNames(string n1, string n2)
		{
			if (n1 == n2)
			{
				return Res.Strings.Error.Name.AlreadyExist;
			}

			if (!ResourceAccess.CheckNamesOnce(n1, n2))
			{
				return Res.Strings.Error.Name.SamePrefix;
			}

			if (!ResourceAccess.CheckNamesOnce(n2, n1))
			{
				return Res.Strings.Error.Name.SamePrefix;
			}

			return null;
		}

		protected static bool CheckNamesOnce(string n1, string n2)
		{
			if (n2.Length > n1.Length && n2.StartsWith(n1))
			{
				if (n2[n1.Length] == '.')
				{
					return false;
				}
			}

			return true;
		}

		public string GetDuplicateName(string baseName)
		{
			//	Retourne le nom � utiliser lorsqu'un nom existant est dupliqu�.
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
				if (this.IsCorrectNewName(ref newName, false))
				{
					break;
				}
			}

			return newName;
		}


		#region ByPassFilter
		public void BypassFilterOpenAccess(Type type, TypeType typeType, List<Druid> exclude)
		{
			//	Ouvre l'acc�s 'bypass'.
			//	Ce type d'acc�s n'est possible que sur une ressource 'Caption' (Captions,
			//	Commands, Types et Values) et 'Panel'.
			System.Diagnostics.Debug.Assert(this.bypassType == Type.Unknow);
			this.bypassType = type;
			System.Diagnostics.Debug.Assert(this.bypassType != Type.Unknow);

			this.bypassDruids = new List<Druid>();

			if (this.IsCaptionsType)
			{
				for (int i=0; i<this.primaryBundle.FieldCount; i++)
				{
					ResourceBundle.Field field = this.primaryBundle[i];
					if (this.HasFixFilter(field.Name, true))
					{
						Druid fullDruid = new Druid(field.Id, this.primaryBundle.Module.Id);

						if (exclude == null || !exclude.Contains(fullDruid))
						{
							if (type == Type.Types && typeType != TypeType.None)
							{
								AbstractType at = this.DirectGetAbstractType(fullDruid);
								TypeType tt = ResourceAccess.AbstractTypeToTypeType(at);
								if (tt != typeType)
								{
									continue;
								}
							}

							this.bypassDruids.Add(fullDruid);
						}
					}
				}
			}
			else
			{
				foreach (ResourceBundle bundle in this.panelsList)
				{
					Druid druid = bundle.Id;

					if (!druid.IsEmpty && (exclude == null || !exclude.Contains(druid)))
					{
						this.bypassDruids.Add(druid);
					}
				}
			}

			this.bypassExclude = exclude;
		}

		public int BypassFilterCount
		{
			//	Retourne le nombre de ressources disponibles � travers l'acc�s 'bypass'.
			get
			{
				System.Diagnostics.Debug.Assert(this.bypassType != Type.Unknow);
				return this.bypassDruids.Count;
			}
		}

		public Druid BypassFilterGetDruid(int index)
		{
			//	Retourne le Druid � un index donn�.
			System.Diagnostics.Debug.Assert(this.bypassType != Type.Unknow);
			return this.bypassDruids[index];
		}

		public int BypassFilterGetIndex(Druid druid)
		{
			//	Cherche l'index d'un Druid donn�.
			System.Diagnostics.Debug.Assert(this.bypassType != Type.Unknow);
			return this.bypassDruids.IndexOf(druid);
		}

		public void BypassFilterGetStrings(Druid druid, ResourceBundle bundle, double availableHeight, out string name, out string text, out bool isDefined)
		{
			//	Retourne une ressource de l'acc�s 'bypass'.
			System.Diagnostics.Debug.Assert(this.bypassType != Type.Unknow);
			if (this.bypassType == Type.Panels)
			{
				name = "";
				text = "";
				isDefined = false;

				int i = this.GetAbsoluteIndex(druid);
				if (i != -1)
				{
					bundle = this.panelsList[i];
					name = this.SubFilter(bundle.Caption, false);
					isDefined = true;
				}
			}
			else
			{
				if (bundle == null)
				{
					this.BypassFilterGetStrings(druid, this.primaryBundle, availableHeight, out name, out text);
					isDefined = false;
				}
				else
				{
					this.BypassFilterGetStrings(druid, bundle, availableHeight, out name, out text);

					if (string.IsNullOrEmpty(text) && bundle != this.primaryBundle)
					{
						this.BypassFilterGetStrings(druid, this.primaryBundle, availableHeight, out name, out text);
						isDefined = false;
					}
					else
					{
						isDefined = true;
					}
				}
			}
		}

		protected void BypassFilterGetStrings(Druid druid, ResourceBundle bundle, double availableHeight, out string name, out string text)
		{
			//	Retourne une ressource de l'acc�s 'bypass'.
			ResourceBundle.Field field = bundle[druid];

			if (field.IsEmpty)
			{
				name = null;
				text = null;
				return;
			}

			ResourceBundle.Field primaryField = this.primaryBundle[druid];
			System.Diagnostics.Debug.Assert(!primaryField.IsEmpty && !string.IsNullOrEmpty(primaryField.Name));
			name = this.SubFilter(primaryField.Name, true);

			if (this.type == Type.Strings)
			{
				text = field.AsString;
			}
			else
			{
				Caption caption = new Caption();

				string s = field.AsString;
				if (!string.IsNullOrEmpty(s))
				{
					caption.DeserializeFromString(s, this.resourceManager);
				}

				text = ResourceAccess.GetCaptionNiceDescription(caption, availableHeight);
			}
		}

		public Druid BypassFilterCreate(ResourceBundle bundle, string name, string text)
		{
			//	Cr�e une nouvelle ressource 'Strings' � la fin.
			System.Diagnostics.Debug.Assert(this.bypassType != Type.Unknow);
			name = this.AddFilter(name, true);

			Druid newDruid = this.CreateUniqueDruid();

			foreach (ResourceBundle b in this.bundles)
			{
				ResourceBundle.Field newField = b.CreateField(ResourceFieldType.Data);
				newField.SetDruid(newDruid);
				newField.SetName(name);

				if (this.type == Type.Strings)
				{
					if (b == bundle)
					{
						newField.SetStringValue(text);
					}
					else
					{
						newField.SetStringValue("");
					}
				}
				else
				{
					if (b == bundle)
					{
						Caption caption = new Caption();

						ICollection<string> dst = caption.Labels;
						dst.Add(text);

						this.AdjustCaptionName(bundle, newField, caption);
						newField.SetStringValue(caption.SerializeToString());
					}
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

			if (this.IsCaptionsType)
			{
				this.CaptionsCountersModify(1);
			}

			this.druidsIndex.Add(newDruid);

			return newDruid;
		}

		public void BypassFilterCloseAccess()
		{
			//	Ferme l'acc�s 'bypass'.
			System.Diagnostics.Debug.Assert(this.bypassType != Type.Unknow);
			this.bypassType = Type.Unknow;
			this.bypassDruids = null;
			this.bypassExclude = null;
		}
		#endregion


		public static string GetCaptionNiceDescription(Caption caption, double availableHeight)
		{
			//	Construit un texte d'apr�s les labels et la description.
			//	Les diff�rents labels sont s�par�s par des virgules.
			//	La description vient sur une deuxi�me ligne (si la hauteur
			//	disponible le permet), mais seulement si elle est diff�rente
			//	de tous les labels.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			string description = caption.Description;

			foreach (string label in caption.Labels)
			{
				if (builder.Length > 0)
				{
					builder.Append(", ");
				}
				builder.Append(label);

				if (description != null)
				{
					if (description == label)  // description identique � un label ?
					{
						description = null;  // pas n�cessaire de montrer la description
					}
				}
			}

			if (description != null)  // faut-il montrer la description ?
			{
				if (builder.Length > 0)
				{
					if (availableHeight >= 30)  // assez de place pour 2 lignes ?
					{
						builder.Append("<br/>");  // sur une deuxi�me ligne
					}
					else
					{
						builder.Append(". ");  // sur la m�me ligne
					}
				}
				builder.Append(description);
			}

			return builder.ToString();
		}

		public static string GetCaptionShortDescription(Caption caption)
		{
			//	Construit un texte tr�s court d'apr�s les labels et la description.
			foreach (string label in caption.Labels)
			{
				return label;
			}

			return caption.Description;
		}


		#region Direct
		public Type DirectGetType(Druid druid)
		{
			//	Retourne le type d'une ressource 'Caption'.
			if (this.type == Type.Panels)
			{
				return Type.Panels;
			}
			else
			{
				System.Diagnostics.Debug.Assert(this.IsCaptionsType);
				string name = this.DirectGetName(druid);
				if (string.IsNullOrEmpty(name))
				{
					return Type.Unknow;
				}
				else
				{
					return ResourceAccess.GetFilterType(name);
				}
			}
		}

		public string DirectGetDisplayName(Druid druid)
		{
			//	Retourne le nom d'une ressource � afficher, sans tenir compte du filtre.
			string name = this.DirectGetName(druid);
			return ResourceAccess.SubAllFilter(name);
		}

		public string DirectGetName(Druid druid)
		{
			//	Retourne le nom d'une ressource, sans tenir compte du filtre.
			//	La recherche s'effectue toujours dans la culture de base.
			if (this.type == Type.Panels)
			{
				foreach (ResourceBundle bundle in this.panelsList)
				{
					if (bundle.Id == druid)
					{
						return bundle.Caption;
					}
				}
				return "?";
			}
			else
			{
				System.Diagnostics.Debug.Assert(this.IsBundlesType);
				ResourceBundle.Field field = this.primaryBundle[druid];
				return field.Name;
			}
		}

		public string DirectGetIcon(Druid druid)
		{
			//	Retourne l'ic�ne d'une ressource, sans tenir compte du filtre.
			//	La recherche s'effectue toujours dans la culture de base.
			Caption caption = this.resourceManager.GetCaption(druid);
			if (caption == null)
			{
				return null;
			}

			return caption.Icon;
		}

		public string DirectDefaultParameter(Druid druid)
		{
			//	Retourne le param�tre par d�faut d'une commande, sans tenir compte du filtre.
			//	La recherche s'effectue toujours dans la culture de base.
			Caption caption = this.resourceManager.GetCaption(druid);
			if (caption == null)
			{
				return null;
			}

			return Command.GetDefaultParameter(caption);
		}

		public string DirectGetGroup(Druid druid)
		{
			//	Retourne le groupe d'une commande.
			Caption caption = this.resourceManager.GetCaption(druid);
			if (caption == null)
			{
				return null;
			}

			return Command.GetGroup(caption);
		}

		public AbstractType DirectGetAbstractType(Druid druid)
		{
			//	Retourne le type abstrait d'un caption de type StructuredType (ou autre).
			Caption caption = this.resourceManager.GetCaption(druid);
			if (caption == null)
			{
				return null;
			}

			return ResourceAccess.GetAbstractType(caption);
		}

		public Druid DirectGetDruid(int index)
		{
			//	Retourne le Druid correspondant � un index.
			System.Diagnostics.Debug.Assert(this.IsBundlesType);
			ResourceBundle.Field field = this.primaryBundle[index];
			return new Druid(field.Id, this.primaryBundle.Module.Id);
		}
		#endregion


		public Field GetField(int index, string cultureName, FieldType fieldType)
		{
			//	Retourne les donn�es d'un champ.
			//	Si cultureName est nul, on acc�de � la culture de base.
			//	[Note1] Lorsqu'on utilise FieldType.AbstractType ou FieldType.Panel,
			//	les donn�es peuvent �tre modifi�es directement, sans qu'il faille les
			//	redonner lors du SetField. Cela oblige � ne pas faire d'autres GetField
			//	avant le SetField !
			this.lastAccessField = fieldType;
			this.CacheResource(index, cultureName);

			if (this.IsBundlesType)
			{
				if (this.accessField == null)
				{
					return null;
				}

				if (fieldType == FieldType.Name)
				{
					return new Field(this.SubFilter(this.accessField.Name, false));
				}
			}

			if (this.type == Type.Strings)
			{
				if (fieldType == FieldType.String)
				{
					return new Field(this.accessField.AsString);
				}

				if (fieldType == FieldType.About)
				{
					return new Field(this.accessField.About);
				}
			}

			if (this.IsCaptionsType)
			{
				if (this.accessCaption == null)
				{
					return null;
				}

				if (fieldType == FieldType.Labels)
				{
					return new Field(this.accessCaption.Labels);
				}

				if (fieldType == FieldType.Description)
				{
					return new Field(this.accessCaption.Description);
				}

				if (fieldType == FieldType.Icon)
				{
					return new Field(this.accessCaption.Icon);
				}

				if (fieldType == FieldType.About)
				{
					return new Field(this.accessField.About);
				}

				if (fieldType == FieldType.Caption)
				{
					return new Field(this.accessCaption);
				}
			}

			if (this.type == Type.Commands)
			{
				if (fieldType == FieldType.Controller)
				{
					string dp = Command.GetDefaultParameter(this.accessCaption);
					return new Field(dp);
				}

				if (fieldType == FieldType.Statefull)
				{
					bool statefull = Command.GetStatefull(this.accessCaption);
					return new Field(statefull);
				}

				if (fieldType == FieldType.Shortcuts)
				{
					Widgets.Collections.ShortcutCollection collection = Shortcut.GetShortcuts(this.accessCaption);
					return new Field(collection);
				}

				if (fieldType == FieldType.Group)
				{
					string group = Command.GetGroup(this.accessCaption);
					return new Field(group);
				}
			}

			if (this.type == Type.Types)
			{
				if (fieldType == FieldType.AbstractType)
				{
					AbstractType type = this.CachedAbstractType;
					if (type == null)
					{
						return null;
					}

					return new Field(type);
				}

				if (fieldType == FieldType.Controller)
				{
					AbstractType type = this.CachedAbstractType;
					if (type == null)
					{
						return null;
					}

					string s;
					if (string.IsNullOrEmpty(type.DefaultControllerParameter))
					{
						s = "";
					}
					else
					{
						s = type.DefaultControllerParameter;
					}
					return new Field(s);
				}
			}

			if (this.type == Type.Panels)
			{
				ResourceBundle bundle = this.PanelBundle(index);
				if (bundle == null)
				{
					return null;
				}

				if (fieldType == FieldType.Name)
				{
					return new Field(bundle.Caption);
				}

				if (fieldType == FieldType.Panel)
				{
					return new Field(bundle);
				}
			}

			return null;
		}

		public void SetField(int index, string cultureName, FieldType fieldType, Field field)
		{
			//	Modifie les donn�es d'un champ.
			//	Si cultureName est nul, on acc�de � la culture de base.
			this.CacheResource(index, cultureName);

			if (this.IsBundlesType)
			{
				if (this.accessField == null)
				{
					return;
				}

				this.CreateIfNecessary();

				if (fieldType == FieldType.Name)
				{
					string name = this.AddFilter(field.String, false);
					this.accessField.SetName(name);

#if false
					Druid druid = this.druidsIndex[index];
					foreach (ResourceBundle bundle in this.bundles)
					{
						ResourceBundle.Field f = bundle[druid];
						if (!f.IsEmpty)
						{
							f.SetName(name);
						}
					}
#endif

					if (this.IsCaptionsType)
					{
						this.AdjustCaptionName(this.primaryBundle, this.accessField, this.accessCaption);
						this.accessField.SetStringValue(this.accessCaption.SerializeToString());
					}
				}
			}

			if (this.type == Type.Strings)
			{
				if (fieldType == FieldType.String)
				{
					this.accessField.SetStringValue(field.String);
				}

				if (fieldType == FieldType.About)
				{
					this.accessField.SetAbout(field.String);
				}
			}

			if (this.IsCaptionsType)
			{
				if (this.accessCaption == null)
				{
					return;
				}

				if (fieldType == FieldType.Labels)
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

				if (fieldType == FieldType.Description)
				{
					this.accessCaption.Description = field.String;
					this.accessField.SetStringValue(this.accessCaption.SerializeToString());
				}

				if (fieldType == FieldType.Icon)
				{
					if (field.String == null)
					{
						this.accessCaption.Icon = "";
					}
					else
					{
						this.accessCaption.Icon = field.String;
					}

					this.accessField.SetStringValue(this.accessCaption.SerializeToString());
				}

				if (fieldType == FieldType.About)
				{
					this.accessField.SetAbout(field.String);
				}

				if (fieldType == FieldType.Caption)
				{
					throw new System.InvalidOperationException("Operation not suported");
				}
			}

			if (this.type == Type.Commands)
			{
				if (fieldType == FieldType.Controller)
				{
					string dp = field.String;
					Command.SetDefaultParameter(this.accessCaption, dp);
					this.accessField.SetStringValue(this.accessCaption.SerializeToString());
				}

				if (fieldType == FieldType.Statefull)
				{
					bool statefull = field.Boolean;
					Command.SetStatefull(this.accessCaption, statefull);
					this.accessField.SetStringValue(this.accessCaption.SerializeToString());
				}

				if (fieldType == FieldType.Shortcuts)
				{
					Widgets.Collections.ShortcutCollection collection = field.ShortcutCollection;
					Shortcut.SetShortcuts(this.accessCaption, collection);
					this.accessField.SetStringValue(this.accessCaption.SerializeToString());
				}

				if (fieldType == FieldType.Group)
				{
					string group = field.String;
					Command.SetGroup(this.accessCaption, group);
					this.accessField.SetStringValue(this.accessCaption.SerializeToString());
				}
			}

			if (this.type == Type.Types)
			{
				if (fieldType == FieldType.AbstractType)
				{
					//	[Note1] Curieusement, on n'utilise pas le param�tre 'field' pass� en
					//	entr�e (premier Assert). En effet, le type AbstractType est d�j� dans
					//	la ressource en cache, dans this.accessCaption. Mais il faut �tre
					//	certain de ne pas faire d'autres GetField avant ce SetField (ce qui
					//	est v�rifi� par le deuxi�me Assert) !
					System.Diagnostics.Debug.Assert(field == null);
					System.Diagnostics.Debug.Assert(fieldType == this.lastAccessField);
					this.accessField.SetStringValue(this.accessCaption.SerializeToString());
				}

				if (fieldType == FieldType.Controller)
				{
					AbstractType type = this.CachedAbstractType;
					if (type == null)
					{
						return;
					}

					type.DefineDefaultController(ResourceAccess.TypeTypeController(type), field.String);
					this.accessField.SetStringValue(this.accessCaption.SerializeToString());
				}
			}

			if (this.type == Type.Panels)
			{
				ResourceBundle bundle = this.PanelBundle(index);
				if (bundle == null)
				{
					return;
				}

				if (fieldType == FieldType.Name)
				{
					bundle.DefineCaption(field.String);
				}

				if (fieldType == FieldType.Panel)
				{
					System.Diagnostics.Debug.Assert(field == null);
					System.Diagnostics.Debug.Assert(fieldType == this.lastAccessField);
				}
			}

			this.IsDirty = true;
		}

		protected AbstractType CachedAbstractType
		{
			//	Retourne le AbstractType correspondant au Caption dans le cache.
			get
			{
				return ResourceAccess.GetAbstractType(this.accessCaption);
			}
		}

		protected static AbstractType GetAbstractType(Caption caption)
		{
			//	Retourne le AbstractType correspondant � un caption.
			AbstractType type = AbstractType.GetCachedType(caption);

			if (type == null)
			{
				type = TypeRosetta.CreateTypeObject(caption);
				if (type == null)
				{
					return null;
				}

				AbstractType.SetCachedType(caption, type);
			}

			return type;
		}


		public static MyWidgets.StringList.CellState CellState(ModificationState state)
		{
			//	Conversion de l'�tat d'une ressource.
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
			//	Donne l'�tat 'modifi�'.
			if (index != -1)
			{
				this.CacheResource(index, cultureName);

				if (this.IsBundlesType)
				{
					if (this.accessField == null || string.IsNullOrEmpty(this.accessField.AsString))
					{
						return ModificationState.Empty;
					}

					if (this.IsCaptionsType)
					{
						Caption caption = new Caption();

						string s = this.accessField.AsString;
						if (!string.IsNullOrEmpty(s))
						{
							caption.DeserializeFromString(s, this.resourceManager);
						}

						if (ResourceAccess.IsEmptyCollection(caption.Labels) && string.IsNullOrEmpty(caption.Description))
						{
							return ModificationState.Empty;
						}
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

		protected static bool IsEmptyCollection(ICollection<string> collection)
		{
			//	Indique si une collection de strings doit �tre consid�r�e comme vide.
			if (collection == null || collection.Count == 0)
			{
				return true;
			}

			foreach (string s in collection)
			{
				if (!string.IsNullOrEmpty(s))
				{
					return false;
				}
			}

			return true;
		}

		public void ModificationClear(int index, string cultureName)
		{
			//	Consid�re une ressource comme '� jour' dans une culture.
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
			//	Consid�re une ressource comme 'modifi�e' dans toutes les cultures.
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
			//	Donne l'�tat de la commande ModificationAll.
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


		public void SearcherIndexToAccess(int field, string secondaryCulture, out string cultureName, out FieldType fieldType)
		{
			//	Conversion d'un index de champ (0..n) en l'information n�cessaire pour Get/SetField.
			if (this.type == Type.Strings)
			{
				switch (field)
				{
					case 0:
						cultureName = null;
						fieldType = FieldType.Name;
						return;

					case 1:
						cultureName = null;
						fieldType = FieldType.String;
						return;

					case 3:
						cultureName = null;
						fieldType = FieldType.About;
						return;
				}

				if (secondaryCulture != null)
				{
					switch (field)
					{
						case 2:
							cultureName = secondaryCulture;
							fieldType = FieldType.String;
							return;

						case 4:
							cultureName = secondaryCulture;
							fieldType = FieldType.About;
							return;
					}
				}
			}

			if (this.IsCaptionsType)
			{
				switch (field)
				{
					case 0:
						cultureName = null;
						fieldType = FieldType.Name;
						return;

					case 1:
						cultureName = null;
						fieldType = FieldType.Labels;
						return;

					case 3:
						cultureName = null;
						fieldType = FieldType.Description;
						return;

					case 5:
						cultureName = null;
						fieldType = FieldType.About;
						return;
				}

				if (secondaryCulture != null)
				{
					switch (field)
					{
						case 2:
							cultureName = secondaryCulture;
							fieldType = FieldType.Labels;
							return;

						case 4:
							cultureName = secondaryCulture;
							fieldType = FieldType.Description;
							return;

						case 6:
							cultureName = secondaryCulture;
							fieldType = FieldType.About;
							return;
					}
				}
			}

			if (this.type == Type.Panels)
			{
				cultureName = null;

				if (field == 0)
				{
					fieldType = FieldType.Name;
					return;
				}
			}

			cultureName = null;
			fieldType = FieldType.None;
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
				if (this.IsCaptionsType)
				{
					if (this.accessCached != index || this.accessCaption == null)
					{
						this.accessCaption = new Caption();

						if (this.accessField != null)
						{
							string s = this.accessField.AsString;
							if (!string.IsNullOrEmpty(s))
							{
								this.accessCaption.DeserializeFromString(s, this.resourceManager);
							}

							this.AdjustCaptionName(this.accessBundle, this.accessField, this.accessCaption);
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
				this.accessCulture = "?";  // nom diff�rent de null, d'une cha�ne vide ou d'un nom existant
				this.accessField = null;
				this.accessCaption = null;
				this.accessCached = -1;
			}
		}

		protected void CreateIfNecessary()
		{
			//	Cr�e une ressource secondaire, si n�cessaire.
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
			if (cultureName == null)
			{
				return this.primaryBundle;
			}

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
			//	Retourne la liste des cultures secondaires, tri�s par ordre alphab�tique.
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
			//	Indique si une culture donn�e existe.
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
			//	Cr�e un nouveau bundle pour une culture donn�e.
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
				//	Cr�e un premier bundle vide.
				string prefix = this.resourceManager.ActivePrefix;
				System.Globalization.CultureInfo culture = this.BaseCulture;
				ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, this.BundleName(true), ResourceLevel.Default, culture);
				bundle.DefineType(this.BundleName(false));

				//	Cr�e un premier champ vide avec un premier Druid.
				//	Ceci est n�cessaire, car il n'existe pas de commande pour cr�er un champ � partir
				//	de rien, mais seulement une commande pour dupliquer un champ existant.
				if (this.IsCaptionsType)
				{
					this.CreateFirstField(bundle, 0, ResourceAccess.GetFixFilter(Type.Captions)+Res.Strings.Viewers.Panels.New);
					this.CreateFirstField(bundle, 1, ResourceAccess.GetFixFilter(Type.Commands)+Res.Strings.Viewers.Panels.New);
					this.CreateFirstField(bundle, 2, ResourceAccess.GetFixFilter(Type.Types)+Res.Strings.Viewers.Panels.New);
					this.CreateFirstField(bundle, 3, ResourceAccess.GetFixFilter(Type.Values)+Res.Strings.Viewers.Panels.New);
				}
				else
				{
					this.CreateFirstField(bundle, 0, Res.Strings.Viewers.Panels.New);
				}

				//	S�rialise le bundle et son premier champ sur disque. Il serait pr�f�rable de
				//	faire ceci lors du Save, mais cette situation �tant exceptionelle, il est
				//	acceptable de faire ainsi !
				this.resourceManager.SetBundle(bundle, ResourceSetMode.CreateOnly);

				ids = this.resourceManager.GetBundleIds("*", this.BundleName(false), ResourceLevel.Default);
				System.Diagnostics.Debug.Assert(ids.Length != 0);
			}

			this.bundles = new ResourceBundleCollection(this.resourceManager);
			this.bundles.LoadBundles(this.resourceManager.ActivePrefix, this.resourceManager.GetBundleIds(ids[0], ResourceLevel.All));

			this.primaryBundle = this.bundles[ResourceLevel.Default];

			if (this.IsCaptionsType)
			{
				this.CaptionsCountersUpdate();

				if (this.captionCounters[Type.Values] == 0)
				{
					Druid newDruid = this.CreateUniqueDruid();
					this.CreateFirstField(this.primaryBundle, newDruid.Local, ResourceAccess.GetFixFilter(Type.Values)+Res.Strings.Viewers.Panels.New);
				}
			}
		}

		protected void CreateFirstField(ResourceBundle bundle, int localId, string name)
		{
			int moduleId = bundle.Module.Id;
			int developerId = 0;  // [PA] provisoire
			Druid newDruid = new Druid(moduleId, developerId, localId);

			ResourceBundle.Field newField = bundle.CreateField(ResourceFieldType.Data);
			newField.SetDruid(newDruid);
			newField.SetName(name);

			if (this.IsCaptionsType)
			{
				Caption caption = new Caption();

				this.AdjustCaptionName(bundle, newField, caption);
				newField.SetStringValue(caption.SerializeToString());
			}
			else
			{
				newField.SetStringValue("");
			}

			bundle.Add(newField);
		}

		protected void AdjustBundlesAfterLoad()
		{
			//	Ajuste les bundles apr�s une d�s�rialisation.
			if (this.type == Type.Strings)
			{
				foreach (ResourceBundle bundle in this.bundles)
				{
					for (int i=0; i<bundle.FieldCount; i++)
					{
						ResourceBundle.Field field = bundle[i];

						string s1 = field.AsString;
						string s2 = TextLayout.ConvertToXmlText(s1);

						if (s2 != s1)
						{
							field.SetStringValue(s2);
						}
					}
				}
			}
		}

		protected void AdjustBundlesBeforeSave()
		{
			//	Ajuste les bundles avant une s�rialisation.
			foreach (ResourceBundle bundle in this.bundles)
			{
				for (int i=0; i<bundle.FieldCount; i++)
				{
					ResourceBundle.Field field = bundle[i];

					if (this.IsCaptionsType)
					{
						Caption caption = new Caption();

						string s = field.AsString;
						if (!string.IsNullOrEmpty(s))
						{
							caption.DeserializeFromString(s, this.resourceManager);
						}

						string name = caption.Name;
						this.AdjustCaptionName(bundle, field, caption);
						System.Diagnostics.Debug.Assert(caption.Name == name);

						field.SetStringValue(caption.SerializeToString());
					}

					if (bundle != this.primaryBundle)
					{
						//	Supprime le 'name="Truc.Chose' dans tous les bundles,
						//	sauf dans le bundle par d�faut.
						field.SetName(null);

						//	Si une ressource est vide dans un bundle autre que le bundle
						//	par d�faut, il faut la supprimer.
						string s = field.AsString;
						if (string.IsNullOrEmpty(s))
						{
							bundle.Remove(i);
							i--;
						}
					}
				}
			}
		}

		protected void AdjustCaptionName(ResourceBundle bundle, ResourceBundle.Field field, Caption caption)
		{
			//	Met � jour le caption.Name en fonction de field.Name.
			//	Le Caption.Name doit contenir le nom de la commande, sans le pr�fixe,
			//	dans le bundle par d�faut. Dans les autres bundles, il ne faut rien.
			//	Pour une Value, caption.Name ne contient que le dernier nom.
			if (bundle == this.primaryBundle)
			{
				if (field.Name.StartsWith(ResourceAccess.GetFixFilter(Type.Values)))
				{
					caption.Name = ResourceAccess.LastName(field.Name);
				}
				else
				{
					caption.Name = ResourceAccess.SubAllFilter(field.Name);
				}
			}
			else
			{
				caption.Name = null;
			}
		}

		protected void SaveBundles()
		{
			foreach (ResourceBundle bundle in this.bundles)
			{
				this.resourceManager.SetBundle(bundle, ResourceSetMode.UpdateOnly);
			}
		}


		protected void AddShortcutsCaptions(ResourceBundle bundle, List<ShortcutItem> list)
		{
			//	Ajoute tous les raccourcis d�finis dans la liste.
			for (int i=0; i<bundle.FieldCount; i++)
			{
				ResourceBundle.Field field = bundle[i];

				Caption caption = new Caption();

				string s = field.AsString;
				if (!string.IsNullOrEmpty(s))
				{
					caption.DeserializeFromString(s, this.resourceManager);
				}

				Widgets.Collections.ShortcutCollection collection = Shortcut.GetShortcuts(caption);
				if (collection != null && collection.Count > 0)
				{
					foreach (Shortcut shortcut in collection)
					{
						ShortcutItem item = new ShortcutItem(shortcut, field.Name, Misc.CultureName(bundle.Culture));
						list.Add(item);
					}
				}
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
				if (!this.HasFixFilter(field.Name, false))
				{
					continue;
				}

				string name = this.SubFilter(field.Name, false);

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

				Druid fullDruid = new Druid(field.Id, this.primaryBundle.Module.Id);
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
				string name = this.SubFilter(bundle.Caption, false);

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

				this.druidsIndex.Add(bundle.Id);
			}
		}


		public int Sort(int index)
		{
			//	Trie toutes les ressources et retourne le nouvel index du Druid.
			Druid druid = this.druidsIndex[index];
			this.Sort();
			return this.druidsIndex.IndexOf(druid);
		}

		protected void Sort()
		{
			//	Trie druidsIndex.
			if (this.IsBundlesType)
			{
				this.druidsIndex.Sort(new FieldDruidSort(this));
			}

			if (this.type == Type.Panels)
			{
				this.druidsIndex.Sort(new PanelDruidSort(this));
			}
		}

		protected class FieldDruidSort : IComparer<Druid>
		{
			public FieldDruidSort(ResourceAccess resourceAccess)
			{
				this.resourceAccess = resourceAccess;
			}

			public int Compare(Druid obj1, Druid obj2)
			{
				ResourceBundle.Field field1 = this.resourceAccess.primaryBundle[obj1];
				ResourceBundle.Field field2 = this.resourceAccess.primaryBundle[obj2];

				if (field1 == null && field2 == null)
				{
					return 0;
				}

				if (field1 == null)
				{
					return 1;
				}

				if (field2 == null)
				{
					return -1;
				}

				return field1.Name.CompareTo(field2.Name);
			}

			protected ResourceAccess resourceAccess;
		}

		protected class PanelDruidSort : IComparer<Druid>
		{
			public PanelDruidSort(ResourceAccess resourceAccess)
			{
				this.resourceAccess = resourceAccess;
			}

			public int Compare(Druid obj1, Druid obj2)
			{
				ResourceBundle bundle1 = null;
				ResourceBundle bundle2 = null;

				int i = this.resourceAccess.GetAbsoluteIndex(obj1);
				if (i != -1)
				{
					bundle1 = this.resourceAccess.panelsList[i];
				}

				i = this.resourceAccess.GetAbsoluteIndex(obj2);
				if (i != -1)
				{
					bundle2 = this.resourceAccess.panelsList[i];
				}

				if (bundle1 == null && bundle2 == null)
				{
					return 0;
				}

				if (bundle1 == null)
				{
					return 1;
				}

				if (bundle2 == null)
				{
					return -1;
				}

				return bundle1.Caption.CompareTo(bundle2.Caption);
			}

			protected ResourceAccess resourceAccess;
		}


		protected int GetAbsoluteIndex(Druid druid)
		{
			//	Cherche l'index absolu d'une ressource d'apr�s son druid.
			if (this.IsBundlesType)
			{
				ResourceBundle.Field field = this.primaryBundle[druid];
				return this.primaryBundle.IndexOf(field);
			}

			if (this.type == Type.Panels)
			{
				for (int i=0; i<this.panelsList.Count; i++)
				{
					if (this.panelsList[i].Id == druid)
					{
						return i;
					}
				}
			}

			return -1;
		}

		protected void CaptionsCountersModify(int value)
		{
			//	Modifie le compte des ressources 'Captions'.
			System.Diagnostics.Debug.Assert(this.IsCaptionsType);
			this.captionCounters[this.type] += value;
		}

		protected void CaptionsCountersUpdate()
		{
			//	Met � jour les comptes des ressources 'Captions'.
			System.Diagnostics.Debug.Assert(this.IsCaptionsType);

			int countCap = 0;
			int countCmd = 0;
			int countTyp = 0;
			int countVal = 0;

			string filterCap = ResourceAccess.GetFixFilter(Type.Captions);
			string filterCmd = ResourceAccess.GetFixFilter(Type.Commands);
			string filterTyp = ResourceAccess.GetFixFilter(Type.Types);
			string filterVal = ResourceAccess.GetFixFilter(Type.Values);

			foreach (ResourceBundle.Field field in this.primaryBundle.Fields)
			{
				string name = field.Name;

				if (name.StartsWith(filterCap))
				{
					countCap++;
				}

				if (name.StartsWith(filterCmd))
				{
					countCmd++;
				}

				if (name.StartsWith(filterTyp))
				{
					countTyp++;
				}

				if (name.StartsWith(filterVal))
				{
					countVal++;
				}
			}

			this.captionCounters[Type.Captions] = countCap;
			this.captionCounters[Type.Commands] = countCmd;
			this.captionCounters[Type.Types] = countTyp;
			this.captionCounters[Type.Values] = countVal;
		}

		protected Druid CreateUniqueDruid()
		{
			//	Cr�e un nouveau druid unique.
			if (this.IsBundlesType)
			{
				int moduleId = this.primaryBundle.Module.Id;
				int developerId = 0;  // [PA] provisoire
				int localId = 0;

				foreach (ResourceBundle.Field field in this.primaryBundle.Fields)
				{
					Druid druid = field.Id;

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
					Druid druid = bundle.Id;

					if (druid.IsValid && druid.Developer == developerId && druid.Local >= localId)
					{
						localId = druid.Local+1;
					}
				}

				foreach (ResourceBundle bundle in this.panelsToDelete)
				{
					Druid druid = bundle.Id;

					if (druid.IsValid && druid.Developer == developerId && druid.Local >= localId)
					{
						localId = druid.Local+1;
					}
				}

				foreach (ResourceBundle bundle in this.panelsToCreate)
				{
					Druid druid = bundle.Id;

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
				//	S'il n'existe aucun panneau, cr�e un premier panneau vide.
				//	Ceci est n�cessaire, car il n'existe pas de commande pour cr�er un panneau � partir
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

					ResourceBundle.Field field = bundle[UI.Panel.PanelBundleField];

					if (field.IsValid)
					{
						UI.Panel panel = UserInterface.DeserializePanel(field.AsString, this.resourceManager);
						panel.DrawDesignerFrame = true;
						UI.Panel.SetPanel(bundle, panel);
						panel.SetupSampleDataSource();
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
				UI.Panel panel = UI.Panel.GetPanel(bundle);

				if (panel != null)
				{
					if (!bundle.Contains (UI.Panel.PanelBundleField))
					{
						ResourceBundle.Field field = bundle.CreateField (ResourceFieldType.Data);
						field.SetName (UI.Panel.PanelBundleField);
						bundle.Add (field);
					}
					if (!bundle.Contains (UI.Panel.DefaultSizeBundleField))
					{
						ResourceBundle.Field field = bundle.CreateField (ResourceFieldType.Data);
						field.SetName (UI.Panel.DefaultSizeBundleField);
						bundle.Add (field);
					}

					bundle[UI.Panel.PanelBundleField].SetXmlValue (UserInterface.SerializePanel (panel, this.resourceManager));
					bundle[UI.Panel.DefaultSizeBundleField].SetStringValue (panel.PreferredSize.ToString ());
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

			//	Supprime tous les panneaux mis dans la liste '� supprimer'.
			foreach (ResourceBundle bundle in this.panelsToDelete)
			{
				this.resourceManager.RemoveBundle(bundle.Id.ToBundleId(), ResourceLevel.Default, bundle.Culture);
			}
			this.panelsToDelete.Clear();
		}

		public UI.Panel GetPanel(int index)
		{
			//	Retourne le UI.Panel associ� � une ressource.
			//	Si n�cessaire, il est cr�� la premi�re fois.
			return this.GetPanel(this.PanelBundle(index));
		}

		public UI.Panel GetPanel(ResourceBundle bundle)
		{
			//	Retourne le UI.Panel associ� � un bundle.
			//	Si n�cessaire, il est cr�� la premi�re fois.
			UI.Panel newPanel = UI.Panel.GetPanel(bundle);

			if (newPanel == null)
			{
				newPanel = this.CreateEmptyPanel();
				UI.Panel.SetPanel(bundle, newPanel);
				newPanel.SetupSampleDataSource();
			}

			return newPanel;
		}

		public UI.Panel CreateEmptyPanel()
		{
			//	Cr�e un nouveau panneau vide.
			UI.Panel panel = new UI.Panel();
			this.InitializePanel(panel);
			return panel;
		}

		internal void InitializePanel(UI.Panel panel)
		{
			panel.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Stacked;
			panel.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
			panel.PreferredSize = new Size(200, 200);
			panel.Anchor = AnchorStyles.BottomLeft;
			panel.Padding = new Margins(20, 20, 20, 20);
			panel.DrawDesignerFrame = true;
			panel.ResourceManager = this.resourceManager;
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
			//	Retourne la culture de base, d�finie par les ressources "Strings" ou "Captions".
			get
			{
				if (this.bundles == null)
				{
					//	S'il n'existe aucun bundle, retourne la culture la plus importante.
					return new System.Globalization.CultureInfo(Misc.Cultures[0]);
				}
				else
				{
					//	Retourne la culture du bundle par d�faut.
					ResourceBundle res = this.bundles[ResourceLevel.Default];
					return res.Culture;
				}
			}
		}


		protected bool IsBundlesType
		{
			//	Retourne true si on acc�de � des ressources de type
			//	"un bundle par culture, plusieurs ressources par bundle".
			get
			{
				return (this.type == Type.Strings || this.IsCaptionsType);
			}
		}

		protected bool IsCaptionsType
		{
			//	Retourne true si on acc�de � des ressources de type Captions/Commands/Types/Values.
			get
			{
				return (this.type == Type.Captions || this.type == Type.Commands || this.type == Type.Types || this.type == Type.Values);
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
				case Type.Values:
					return many ? "Captions" : "Caption";

				case Type.Panels:
					return "Panel";
			}

			return null;
		}


		protected static string LastName(string name)
		{
			//	Retourne la derni�re partie d'un nom s�par� par des points.
			//	"Toto.Titi.Tutu" retourne "Tutu".
			int i = name.LastIndexOf('.');
			if (i != -1)
			{
				name = name.Substring(i+1);
			}
			return name;
		}


		#region FixFilter
		protected string AddFilter(string name, bool bypass)
		{
			//	Ajoute le filtre fixe si n�cessaire.
			if (!this.HasFixFilter(name, bypass))
			{
				string fix = this.FixFilter(bypass);
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

		protected string SubFilter(string name, bool bypass)
		{
			//	Supprime le filtre fixe si n�cessaire.
			if (this.HasFixFilter(name, bypass))
			{
				string fix = this.FixFilter(bypass);
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

		public static string SubAllFilter(string name)
		{
			//	Supprime tous les filtres fixes connus si n�cessaire.
			Type type = ResourceAccess.GetFilterType(name);

			if (type != Type.Unknow)
			{
				string filter = ResourceAccess.GetFixFilter(type);
				name = name.Substring(filter.Length);
			}

			return name;
		}

		protected static Type GetFilterType(string name)
		{
			//	Retourne le type en fonction du pr�fixe du nom.
			string filter;

			if (string.IsNullOrEmpty(name))
			{
				return Type.Unknow;
			}

			filter = ResourceAccess.GetFixFilter(Type.Captions);
			if (name.StartsWith(filter))
			{
				return Type.Captions;
			}

			filter = ResourceAccess.GetFixFilter(Type.Commands);
			if (name.StartsWith(filter))
			{
				return Type.Commands;
			}

			filter = ResourceAccess.GetFixFilter(Type.Types);
			if (name.StartsWith(filter))
			{
				return Type.Types;
			}

			filter = ResourceAccess.GetFixFilter(Type.Values);
			if (name.StartsWith(filter))
			{
				return Type.Values;
			}

			return Type.Unknow;
		}

		protected bool HasFixFilter(string name, bool bypass)
		{
			//	Indique si un nom commence par le filtre fixe.
			string fix = this.FixFilter(bypass);
			
			if (fix == null)
			{
				return true;
			}
			else
			{
				return name.StartsWith(fix);
			}
		}

		protected string FixFilter(bool bypass)
		{
			//	Retourne la cha�ne fixe du filtre.
			return ResourceAccess.GetFixFilter(bypass ? this.bypassType : this.type);
		}

		protected static string GetFixFilter(Type type)
		{
			//	Retourne la cha�ne fixe du filtre pour un type donn�.
			switch (type)
			{
				case Type.Captions:
					return "Cap.";

				case Type.Commands:
					return "Cmd.";

				case Type.Types:
					return "Typ.";

				case Type.Values:
					return "Val.";
			}

			return null;
		}
		#endregion


		#region Class ShortcutItem
		public class ShortcutItem
		{
			public ShortcutItem(Shortcut shortcut, string name, string culture)
			{
				this.shortcut = shortcut;
				this.name     = name;
				this.culture  = culture;
			}

			public Shortcut Shortcut
			{
				//	Raccourci.
				get
				{
					return this.shortcut;
				}
			}

			public string Name
			{
				//	Nom du Command associ� (normalement "Cap.*").
				get
				{
					return this.name;
				}
			}

			public string Culture
			{
				//	Nom standard de la culture associ�e.
				get
				{
					return this.culture;
				}
			}

			static public bool IsEqual(ShortcutItem item1, ShortcutItem item2)
			{
				//	Indique si deux raccourcis sont identiques (m�mes raccourcis pour la m�me culture).
				return (item1.Shortcut == item2.Shortcut && item1.Culture == item2.Culture);
			}

			static public bool Contains(List<ShortcutItem> list, ShortcutItem item)
			{
				//	Cherche si un raccourci existe dans une liste.
				foreach (ShortcutItem i in list)
				{
					if (ShortcutItem.IsEqual(i, item))
					{
						return true;
					}
				}
				return false;
			}

			static public List<int> IndexesOf(List<ShortcutItem> list, int index)
			{
				//	Retourne la liste des index du raccourci dont on sp�cifie l'index,
				//	seulement s'il y en a plus d'un.
				ShortcutItem item = list[index];

				int count = 0;
				for (int i=0; i<list.Count; i++)
				{
					if (ShortcutItem.IsEqual(list[i], item))
					{
						count ++;
					}
				}

				if (count > 1)
				{
					List<int> indexes = new List<int>(count);

					for (int i=0; i<list.Count; i++)
					{
						if (ShortcutItem.IsEqual(list[i], item))
						{
							indexes.Add(i);
						}
					}

					return indexes;
				}
				else
				{
					return null;
				}
			}

			protected Shortcut			shortcut;
			protected string			name;
			protected string			culture;
		}
		#endregion


		#region Class Field
		/// <summary>
		/// Permet d'acc�der � un champ de n'importe quel type.
		/// </summary>
		public class Field
		{
			public enum Type
			{
				String,
				StringCollection,
				Bundle,
				Boolean,
				Integer,
				Shortcuts,
				AbstractType,
				Caption,
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

			public Field(bool value)
			{
				this.type = Type.Boolean;
				this.booleanValue = value;
			}

			public Field(int value)
			{
				this.type = Type.Integer;
				this.integerValue = value;
			}

			public Field(Widgets.Collections.ShortcutCollection value)
			{
				this.type = Type.Shortcuts;
				this.shortcutCollection = value;
			}

			public Field(AbstractType value)
			{
				this.type = Type.AbstractType;
				this.abstractType = value;
			}

			public Field(Caption value)
			{
				this.type = Type.Caption;
				this.caption = value;
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

			public bool Boolean
			{
				get
				{
					System.Diagnostics.Debug.Assert(this.type == Type.Boolean);
					return this.booleanValue;
				}
			}

			public int Integer
			{
				get
				{
					System.Diagnostics.Debug.Assert(this.type == Type.Integer);
					return this.integerValue;
				}
			}

			public Widgets.Collections.ShortcutCollection ShortcutCollection
			{
				get
				{
					System.Diagnostics.Debug.Assert(this.type == Type.Shortcuts);
					return this.shortcutCollection;
				}
			}

			public AbstractType AbstractType
			{
				get
				{
					System.Diagnostics.Debug.Assert(this.type == Type.AbstractType);
					return this.abstractType;
				}
			}

			public Caption Caption
			{
				get
				{
					System.Diagnostics.Debug.Assert(this.type == Type.Caption);
					return this.caption;
				}
			}

			protected Type type;
			protected string									stringValue;
			protected ICollection<string>						stringCollection;
			protected ResourceBundle							bundle;
			protected bool										booleanValue;
			protected int										integerValue;
			protected Widgets.Collections.ShortcutCollection	shortcutCollection;
			protected AbstractType								abstractType;
			protected Caption									caption;
		}
		#endregion


		#region Events handler
		protected virtual void OnDirtyChanged()
		{
			if (this.DirtyChanged != null)  // qq'un �coute ?
			{
				this.DirtyChanged(this);
			}
		}

		public event Support.EventHandler DirtyChanged;
		#endregion


		protected static string								filterPrefix = "Epsitec.Common.";

		protected Type										type;
		protected ResourceManager							resourceManager;
		protected ResourceModuleInfo						moduleInfo;
		protected MainWindow								mainWindow;
		protected bool										isDirty = false;

		protected ResourceBundleCollection					bundles;
		protected ResourceBundle							primaryBundle;
		protected List<Druid>								druidsIndex;
		protected string									accessCulture;
		protected ResourceBundle							accessBundle;
		protected int										accessIndex;
		protected int										accessCached;
		protected ResourceBundle.Field						accessField;
		protected Caption									accessCaption;
		protected FieldType									lastAccessField = FieldType.None;
		protected List<ResourceBundle>						panelsList;
		protected List<ResourceBundle>						panelsToCreate;
		protected List<ResourceBundle>						panelsToDelete;
		protected Dictionary<Type, int>						captionCounters;
		protected Dictionary<Type, int>						filterIndexes;
		protected Dictionary<Type, string>					filterStrings;
		protected Dictionary<Type, Searcher.SearchingMode>	filterModes;
		protected Type										bypassType = Type.Unknow;
		protected List<Druid>								bypassDruids;
		protected List<Druid>								bypassExclude;
		protected ResourceAccess.TypeType					lastTypeTypeCreatated = TypeType.String;
		protected System.Type								lastTypeTypeSystem = null;
	}
}
