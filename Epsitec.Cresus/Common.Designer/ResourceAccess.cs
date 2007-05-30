using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

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
			Strings2,
			Captions,
			Captions2,
			Fields,
			Commands,
			Commands2,
			Types,
			Values,
			Panels,
			Scripts,
			Entities,
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
			Normal,			//	défini normalement
			Empty,			//	vide ou indéfini (fond rouge)
			Modified,		//	modifié (fond jaune)
		}


		public ResourceAccess(Type type, ResourceManager resourceManager, ResourceModuleInfo moduleInfo, MainWindow mainWindow)
		{
			//	Constructeur unique pour accéder aux ressources d'un type donné.
			//	Par la suite, l'instance créée accédera toujours aux ressources de ce type,
			//	sauf pour les ressources Captions, Commands, Types et Values.
			this.type = type;
			this.resourceManager = resourceManager;
			this.moduleInfo = moduleInfo;
			this.mainWindow = mainWindow;

			if (this.IsAbstract2)
			{
				if (this.type == Type.Strings2)
				{
					this.accessor = new Support.ResourceAccessors.StringResourceAccessor();
				}
				if (this.type == Type.Captions2)
				{
					this.accessor = new Support.ResourceAccessors.CaptionResourceAccessor();
				}
				if (this.type == Type.Commands2)
				{
					this.accessor = new Support.ResourceAccessors.CommandResourceAccessor();
				}
				if (this.type == Type.Entities)
				{
					this.accessor = new Support.ResourceAccessors.StructuredTypeResourceAccessor();
				}

				this.collectionView = new CollectionView(this.accessor.Collection);
				this.collectionView.Filter = this.CollectionViewFilter;
			}
			else
			{
				this.druidsIndex = new List<Druid>();

				this.captionCounters = new Dictionary<Type, int>();
				this.captionCounters[Type.Captions] = 0;
				this.captionCounters[Type.Fields] = 0;
				this.captionCounters[Type.Commands] = 0;
				this.captionCounters[Type.Types] = 0;
				this.captionCounters[Type.Values] = 0;

				this.filterIndexes = new Dictionary<Type, int>();
				this.filterStrings = new Dictionary<Type, string>();
				this.filterModes = new Dictionary<Type, Searcher.SearchingMode>();
			}
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

		public ResourceBundleCollection ResourceBundles
		{
			get
			{
				return this.bundles;
			}
		}


		public ResourceManager ResourceManager
		{
			get
			{
				return this.resourceManager;
			}
		}

		public Support.ResourceAccessors.AbstractResourceAccessor Accessor
		{
			get
			{
				return this.accessor;
			}
		}

		public CollectionView CollectionView
		{
			get
			{
				return this.collectionView;
			}
		}

		public static string TypeDisplayName(Type type)
		{
			//	Retourne le nom au pluriel correspondant à un type.
			return ResourceAccess.TypeDisplayName(type, true);
		}

		public static string TypeDisplayName(Type type, bool many)
		{
			//	Retourne le nom au singulier ou au pluriel correspondant à un type.
			switch (type)
			{
				case Type.Strings:
				case Type.Strings2:
					return many ? Res.Strings.BundleType.Strings : Res.Strings.BundleType.String;

				case Type.Captions:
				case Type.Captions2:
					return many ? Res.Strings.BundleType.Captions : Res.Strings.BundleType.Caption;

				case Type.Fields:
					return many ? Res.Strings.BundleType.Fields : Res.Strings.BundleType.Field;

				case Type.Commands:
				case Type.Commands2:
					return many ? Res.Strings.BundleType.Commands : Res.Strings.BundleType.Command;

				case Type.Types:
					return many ? Res.Strings.BundleType.Types : Res.Strings.BundleType.Type;

				case Type.Values:
					return many ? Res.Strings.BundleType.Values : Res.Strings.BundleType.Value;

				case Type.Panels:
					return many ? Res.Strings.BundleType.Panels : Res.Strings.BundleType.Panel;

				case Type.Scripts:
					return many ? Res.Strings.BundleType.Scripts : Res.Strings.BundleType.Script;

				case Type.Entities:
					return many ? "Entités" : "Entité";
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

		protected static AbstractType TypeTypeCreate(TypeType type, ResourceBundle bundle)
		{
			AbstractType abstractType = null;

			switch (type)
			{
				case TypeType.Void:         abstractType = new VoidType(); break;
				case TypeType.Boolean:      abstractType = new BooleanType(); break;
				case TypeType.Integer:      abstractType = new IntegerType(); break;
				case TypeType.LongInteger:  abstractType = new LongIntegerType(); break;
				case TypeType.Double:       abstractType = new DoubleType(); break;
				case TypeType.Decimal:      abstractType = new DecimalType(); break;
				case TypeType.String:       abstractType = new StringType(); break;
				case TypeType.Enum:         abstractType = new EnumType(); break;
				case TypeType.Structured:	abstractType = new StructuredType(StructuredTypeClass.Entity, Druid.Empty); break;
				case TypeType.Collection:   abstractType = new CollectionType(); break;
				case TypeType.Date:         abstractType = new DateType(); break;
				case TypeType.Time:         abstractType = new TimeType(); break;
				case TypeType.DateTime:     abstractType = new DateTimeType(); break;
				case TypeType.Binary:       abstractType = new BinaryType(); break;
			}

			if (abstractType != null)
			{
				//	Associe le type avec le bundle dans lequel il sera stocké.
				//	Cela est nécessaire pour certains types qui doivent savoir
				//	de quel module ils proviennent.
				Caption caption = abstractType.Caption;
				ResourceManager.SetSourceBundle(caption, bundle);
			}

			return abstractType;
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


		public bool IsJustLoaded
		{
			get
			{
				return this.isJustLoaded;
			}
			set
			{
				this.isJustLoaded = value;
			}
		}

		public void Load()
		{
			//	Charge les ressources.
			if (this.IsAbstract2)
			{
				this.accessor.Load(this.resourceManager);
				this.collectionView.MoveCurrentToFirst();
			}

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
			this.isJustLoaded = true;
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
			//	Ajoute tous les raccourcis définis dans la liste.
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
			//	Vérifie les raccourcis, en construisant un message d'avertissement
			//	pour tous les raccourcis utilisés plus d'une fois.
			string culture = null;
			List<ShortcutItem> uses = new List<ShortcutItem>();

			for (int i=0; i<list.Count; i++)
			{
				if (ShortcutItem.Contains(uses, list[i]))  // raccourci déjà traité ?
				{
					continue;
				}

				List<int> indexes = ShortcutItem.IndexesOf(list, i);
				if (indexes == null)  // utilisé une seule fois ?
				{
					continue;
				}

				if (culture == null || culture != list[i].Culture)  // autre culture ?
				{
					if (builder.Length > 0)  // déjà d'autres avertissements ?
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

				uses.Add(list[i]);  // ajoute à la liste des raccourcis traités
			}
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
			EnumType et = null;
			StructuredType st = null;

			if (this.IsAbstract2)
			{
				CultureMap item = this.collectionView.CurrentItem as CultureMap;

				CultureMap newItem = this.accessor.CreateItem();
				newItem.Name = newName;
				this.accessor.Collection.Add(newItem);
				this.collectionView.MoveCurrentTo(newItem);

				if (duplicateContent)
				{
					//	Construit la liste des cultures à copier
					List<string> cultures = this.GetSecondaryCultureNames ();
					cultures.Insert(0, Resources.DefaultTwoLetterISOLanguageName);
					
					foreach (string culture in cultures)
					{
						StructuredData data = item.GetCultureData(culture);
						StructuredData newData = newItem.GetCultureData(culture);
						ResourceAccess.CopyData(this.accessor, newItem, data, newData);
					}
				}

				this.accessor.PersistChanges();

				this.IsDirty = true;
				return;
			}

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
				Druid druid = this.mainWindow.DlgResourceSelector(module, ResourceAccess.Type.Types, ResourceAccess.TypeType.Structured, Druid.Empty, null, null);
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
								type = ResourceAccess.TypeTypeCreate(this.lastTypeTypeCreatated, bundle);
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
							ResourceManager.SetSourceBundle(caption, bundle);
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

		private static void CopyData(IResourceAccessor accessor, CultureMap dstItem, StructuredData src, StructuredData dst)
		{
			//	Copie les données d'un StructuredData vers un autre, en tenant
			//	compte des collections de données qui ne peuvent pas être copiées
			//	sans autre.
			Types.IStructuredType type = src.StructuredType;
			foreach (string fieldId in type.GetFieldIds())
			{
				object value = src.GetValue(fieldId);

				if (!UndefinedValue.IsUndefinedValue(value))
				{
					ResourceAccess.SetStructuredDataValue (accessor, dstItem, dst, fieldId, value);
				}
			}
		}

		public static void SetStructuredDataValue(IResourceAccessor accessor, CultureMap map, StructuredData data, string id, object value)
		{
			//	Réalise un StructuredData.SetValue qui tienne compte des cas
			//	particuliers où les données à copier sont dans une collection.
			if (data.IsValueLocked(id))
			{
				//	La donnée que l'on cherche à modifier est verrouillée; c'est
				//	sans doute parce que c'est une collection et que l'on n'a pas
				//	le droit de la remplacer...
				ResourceAccess.AttemptCollectionCopy<string> (accessor, map, data, id, value, null);
				ResourceAccess.AttemptCollectionCopy<StructuredData> (accessor, map, data, id, value, ResourceAccess.CopyStructuredData);
			}
			else
			{
				data.SetValue(id, value);
			}
		}

		private delegate T CopyCallback<T>(IResourceAccessor accessor, CultureMap map, StructuredData container, string fieldId, T obj);

		private static StructuredData CopyStructuredData(IResourceAccessor accessor, CultureMap map, StructuredData container, string fieldId, StructuredData source)
		{
			//	Copie (récursivement) les données au niveau actuel en demandant au
			//	broker de s'occuper de l'allocation du StructuredData.
			IDataBroker broker = accessor.GetDataBroker(container, fieldId);
			StructuredData copy = broker.CreateData(map);
			ResourceAccess.CopyData(accessor, map, source, copy);
			return copy;
		}

		private static void AttemptCollectionCopy<T>(IResourceAccessor accessor, CultureMap map, StructuredData data, string id, object value, CopyCallback<T> copyMethod)
		{
			IEnumerable<T> source = value as IEnumerable<T>;
			IList<T>  destination = data.GetValue(id) as IList<T>;
			
			if (destination != null)
			{
				destination.Clear();
			}

			if (source != null && destination != null)
			{
				foreach (T item in source)
				{
					if (copyMethod == null)
					{
						destination.Add(item);
					}
					else
					{
						destination.Add(copyMethod (accessor, map, data, id, item));
					}
				}
			}
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
						caption.DefineId(newDruid);
						
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
			//	Retourne le nom de base à utiliser pour une énumération native C#.
			string name = stype.FullName.Replace('+', '.');

			//	Enlève le préfixe "Epsitec.Common." s'il existe.
			if (name.StartsWith(ResourceAccess.filterPrefix))
			{
				name = name.Substring(ResourceAccess.filterPrefix.Length);
			}

			//	Enlève son propre nom de module s'il existe.
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
			if (this.IsAbstract2)
			{
				CultureMap item = this.collectionView.CurrentItem as CultureMap;
				this.accessor.Collection.Remove(item);
				this.accessor.PersistChanges();

				this.IsDirty = true;
				return;
			}

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

#if false
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
#endif


		public void SetFilter(string filter, Searcher.SearchingMode mode)
		{
			//	Construit l'index en fonction des ressources primaires.
			if (this.IsAbstract2)
			{
				if (this.collectionViewFilter != filter || this.collectionViewMode != mode)
				{
					this.collectionViewMode = mode;

					if ((this.collectionViewMode&Searcher.SearchingMode.CaseSensitive) == 0)
					{
						this.collectionViewFilter = Searcher.RemoveAccent(filter.ToLower());
					}
					else
					{
						this.collectionViewFilter = filter;
					}

					this.collectionViewRegex = null;
					if ((this.collectionViewMode&Searcher.SearchingMode.Jocker) != 0)
					{
						this.collectionViewRegex = RegexFactory.FromSimpleJoker(this.collectionViewFilter, RegexFactory.Options.None);
					}

					this.collectionView.Refresh();
				}
			}
			else
			{
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

				//	Trie druidsIndex.
				this.Sort();

				//	Mémorise le filtre utilisé.
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

				//	Cherche l'index correspondant à la ressource d'avant le changement de filtre.
				int index = this.druidsIndex.IndexOf(druid);
				if (index == -1)
				{
					index = 0;
				}
				this.accessIndex = index;

				this.CacheClear();
			}
		}

		protected bool CollectionViewFilter(object obj)
		{
			CultureMap item = obj as CultureMap;
			
			if (!string.IsNullOrEmpty(this.collectionViewFilter))
			{
				if ((this.collectionViewMode&Searcher.SearchingMode.Jocker) != 0)
				{
					string text = item.Name;
					if ((this.collectionViewMode&Searcher.SearchingMode.CaseSensitive) == 0)
					{
						text = Searcher.RemoveAccent(text.ToLower());
					}
					if (!this.collectionViewRegex.IsMatch(text))
					{
						return false;
					}
				}
				else
				{
					int index = Searcher.IndexOf(item.Name, this.collectionViewFilter, 0, this.collectionViewMode);
					if (index == -1)
					{
						return false;
					}
					if ((this.collectionViewMode&Searcher.SearchingMode.AtBeginning) != 0 && index != 0)
					{
						return false;
					}
				}
			}

			return true;
		}

		public int TotalCount
		{
			//	Retourne le nombre de données accessibles.
			get
			{
				if (this.IsAbstract2)
				{
				}
				else
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
				}

				return 0;
			}
		}

		public int AccessCount
		{
			//	Retourne le nombre de données accessibles.
			get
			{
				if (this.IsAbstract2)
				{
					return this.collectionView.Count;
				}
				else
				{
					return this.druidsIndex.Count;
				}
			}
		}

		public Druid AccessDruid(int index)
		{
			//	Retourne le druid d'un index donné.
			if (this.IsAbstract2)
			{
				CultureMap item = this.collectionView.Items[index] as CultureMap;
				return item.Id;
			}
			else
			{
				if (index >= 0 && index < this.druidsIndex.Count)
				{
					return this.druidsIndex[index];
				}
				else
				{
					return Druid.Empty;
				}
			}
		}

		public int AccessIndexOfDruid(Druid druid)
		{
			//	Retourne l'index d'un Druid.
			if (this.IsAbstract2)
			{
				for (int i=0; i<this.collectionView.Items.Count; i++)
				{
					CultureMap item = this.collectionView.Items[i] as CultureMap;
					if (item.Id == druid)
					{
						return i;
					}
				}
				return -1;
			}
			else
			{
				return this.druidsIndex.IndexOf(druid);
			}
		}

		public int AccessIndex
		{
			//	Index de l'accès en cours.
			get
			{
				if (this.IsAbstract2)
				{
					return this.collectionView.CurrentPosition;
				}
				else
				{
					return this.accessIndex;
				}
			}

			set
			{
				if (this.IsAbstract2)
				{
					value = System.Math.Max(value, 0);
					value = System.Math.Min(value, this.collectionView.Count-1);

					if (this.collectionView.CurrentPosition != value)
					{
						this.collectionView.MoveCurrentToPosition(value);
						this.collectionView.Refresh();
					}
				}
				else
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
		}


		public bool IsCorrectNewName(ref string name, bool bypass)
		{
			//	Retourne true s'il est possible de créer cette nouvelle ressource.
			return (this.CheckNewName(ref name, bypass) == null);
		}

		public string CheckNewName(ref string name, bool bypass)
		{
			//	Retourne l'éventuelle erreur si on tente de créer cette nouvelle ressource.
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

			//	Cherche si le nom existe déjà.
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
				if (this.IsAbstract2)
				{
					CollectionView cv = new CollectionView(this.accessor.Collection);
					foreach (CultureMap item in cv.Items)
					{
						err = ResourceAccess.CheckNames(item.Name, name);
						if (err != null)
						{
							return err;
						}
					}
				}
				else if (this.IsBundlesType)
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
				else if (this.type == Type.Panels)
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
				if (this.IsCorrectNewName(ref newName, false))
				{
					break;
				}
			}

			return newName;
		}


		#region ByPassFilter
		public void BypassFilterOpenAccess(Type type, TypeType typeType, List<Druid> exclude, string includePrefix)
		{
			//	Ouvre l'accès 'bypass'.
			//	Ce type d'accès n'est possible que sur une ressource 'Caption' (Captions,
			//	Fields, Commands, Types et Values) et 'Panel'.
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
						if (includePrefix != null)
						{
							string name = this.SubFilter(field.Name, true);
							if (!name.StartsWith(includePrefix))
							{
								continue;
							}
						}

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
			//	Retourne le nombre de ressources disponibles à travers l'accès 'bypass'.
			get
			{
				System.Diagnostics.Debug.Assert(this.bypassType != Type.Unknow);
				return this.bypassDruids.Count;
			}
		}

		public Druid BypassFilterGetDruid(int index)
		{
			//	Retourne le Druid à un index donné.
			System.Diagnostics.Debug.Assert(this.bypassType != Type.Unknow);
			return this.bypassDruids[index];
		}

		public int BypassFilterGetIndex(Druid druid)
		{
			//	Cherche l'index d'un Druid donné.
			System.Diagnostics.Debug.Assert(this.bypassType != Type.Unknow);
			return this.bypassDruids.IndexOf(druid);
		}

		public void BypassFilterRenameStructuredField(Druid druid, ResourceBundle bundle, string initialName, string newName)
		{
			//	Renomme le ResourceBundle.Field.Name d'un champ, lorsque le nom du StructuredType a changé.
			//	"Fld.InitialName.Champ1" devient "Fld.NewName.Champ1".
			string name = this.SubFilter(bundle[druid].Name, true);
			if (name.StartsWith(string.Concat(initialName, ".")))
			{
				name = name.Substring(initialName.Length, name.Length-initialName.Length);
				name = this.AddFilter(string.Concat(newName, name), true);
				bundle[druid].SetName(name);

				//	TODO: adapter aussi Caption.Name !
				int i = name.LastIndexOf(".");
				if (i != -1)  // commence par le nom de l'entité ?
				{
					name = name.Substring(i+1);
				}

				//?Caption caption = bundle.Caption;
				//?caption.Name = name;
			}
		}

		public void BypassFilterGetStrings(Druid druid, ResourceBundle bundle, double availableHeight, out string name, out string text, out bool isDefined)
		{
			//	Retourne une ressource de l'accès 'bypass'.
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
			//	Retourne une ressource de l'accès 'bypass'.
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
					ResourceManager.SetSourceBundle(caption, bundle);
				}

				text = ResourceAccess.GetCaptionNiceDescription(caption, availableHeight);
			}
		}

		public Druid BypassFilterCreate(ResourceBundle bundle, string name, string text)
		{
			//	Crée une nouvelle ressource 'Strings' à la fin.
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
						ResourceManager.SetSourceBundle(caption, bundle);
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

			if (this.bypassType == this.type)
			{
				this.druidsIndex.Add(newDruid);
			}

			return newDruid;
		}

		public void BypassFilterCloseAccess()
		{
			//	Ferme l'accès 'bypass'.
			System.Diagnostics.Debug.Assert(this.bypassType != Type.Unknow);
			this.bypassType = Type.Unknow;
			this.bypassDruids = null;
			this.bypassExclude = null;
		}
		#endregion


		public static string GetCaptionNiceDescription(Caption caption, double availableHeight)
		{
			//	Construit un texte d'après les labels et la description.
			//	Les différents labels sont séparés par des virgules.
			//	La description vient sur une deuxième ligne (si la hauteur
			//	disponible le permet), mais seulement si elle est différente
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
					if (description == label)  // description identique à un label ?
					{
						description = null;  // pas nécessaire de montrer la description
					}
				}
			}

			if (description != null)  // faut-il montrer la description ?
			{
				if (builder.Length > 0)
				{
					if (availableHeight >= 30)  // assez de place pour 2 lignes ?
					{
						builder.Append("<br/>");  // sur une deuxième ligne
					}
					else
					{
						builder.Append(". ");  // sur la même ligne
					}
				}
				builder.Append(description);
			}

			return builder.ToString();
		}

		public static string GetCaptionShortDescription(Caption caption)
		{
			//	Construit un texte très court d'après les labels et la description.
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
			//	Retourne le nom d'une ressource à afficher, sans tenir compte du filtre.
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
			//	Retourne l'icône d'une ressource, sans tenir compte du filtre.
			//	La recherche s'effectue toujours dans la culture de base.
			Caption caption = this.resourceManager.GetCaption(druid);
			if (caption == null)
			{
				return null;
			}

			return caption.Icon;
		}

		public Caption DirectGetCaption(Druid druid)
		{
			Caption caption = this.resourceManager.GetCaption(druid);
			return caption;
		}

		public string DirectDefaultParameter(Druid druid)
		{
			//	Retourne le paramètre par défaut d'une commande, sans tenir compte du filtre.
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
			//	Retourne le Druid correspondant à un index.
			System.Diagnostics.Debug.Assert(this.IsBundlesType);
			ResourceBundle.Field field = this.primaryBundle[index];
			return new Druid(field.Id, this.primaryBundle.Module.Id);
		}
		#endregion


		public Field GetField(int index, string cultureName, FieldType fieldType)
		{
			//	Retourne les données d'un champ.
			//	Si cultureName est nul, on accède à la culture de base.
			//	[Note1] Lorsqu'on utilise FieldType.AbstractType ou FieldType.Panel,
			//	les données peuvent être modifiées directement, sans qu'il faille les
			//	redonner lors du SetField. Cela oblige à ne pas faire d'autres GetField
			//	avant le SetField !
			this.lastAccessField = fieldType;
			this.CacheResource(index, cultureName);

			if (this.IsAbstract2)
			{
				CultureMap item = this.collectionView.Items[index] as CultureMap;

				if (cultureName == null)
				{
					cultureName = Resources.DefaultTwoLetterISOLanguageName;
				}
				StructuredData data = item.GetCultureData(cultureName);

				if (fieldType == FieldType.Name)
				{
					return new Field(item.Name);
				}

				if (fieldType == FieldType.String)
				{
					string text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;
					return new Field(text);
				}

				if (fieldType == FieldType.Description)
				{
					string text = data.GetValue(Support.Res.Fields.ResourceCaption.Description) as string;
					return new Field(text);
				}

				if (fieldType == FieldType.Labels)
				{
					IList<string> list = data.GetValue(Support.Res.Fields.ResourceCaption.Labels) as IList<string>;
					return new Field(list);
				}

				if (fieldType == FieldType.About)
				{
					string text = data.GetValue(Support.Res.Fields.ResourceBase.Comment) as string;
					return new Field(text);
				}
			}

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
			//	Modifie les données d'un champ.
			//	Si cultureName est nul, on accède à la culture de base.
			this.CacheResource(index, cultureName);

			if (this.IsAbstract2)
			{
				CultureMap item = this.collectionView.Items[index] as CultureMap;

				if (cultureName == null)
				{
					cultureName = Resources.DefaultTwoLetterISOLanguageName;
				}
				StructuredData data = item.GetCultureData(cultureName);

				if (fieldType == FieldType.Name)
				{
					item.Name = field.String;
					this.accessor.PersistChanges();
					this.collectionView.Refresh();
				}

				if (fieldType == FieldType.String)
				{
					data.SetValue(Support.Res.Fields.ResourceString.Text, field.String);
					this.accessor.PersistChanges();
					this.collectionView.Refresh();
				}

				if (fieldType == FieldType.Description)
				{
					data.SetValue(Support.Res.Fields.ResourceCaption.Description, field.String);
					this.accessor.PersistChanges();
					this.collectionView.Refresh();
				}

				if (fieldType == FieldType.Labels)
				{
					IList<string> list = data.GetValue(Support.Res.Fields.ResourceCaption.Labels) as IList<string>;
					list.Clear();
					foreach (string text in field.StringCollection)
					{
						list.Add(text);
					}
					this.accessor.PersistChanges();
					this.collectionView.Refresh();
				}

				if (fieldType == FieldType.About)
				{
					data.SetValue(Support.Res.Fields.ResourceBase.Comment, field.String);
					this.accessor.PersistChanges();
					this.collectionView.Refresh();
				}

				this.IsDirty = true;
				return;
			}

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
					//	[Note1] Curieusement, on n'utilise pas le paramètre 'field' passé en
					//	entrée (premier Assert). En effet, le type AbstractType est déjà dans
					//	la ressource en cache, dans this.accessCaption. Mais il faut être
					//	certain de ne pas faire d'autres GetField avant ce SetField (ce qui
					//	est vérifié par le deuxième Assert) !
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
			//	Retourne le AbstractType correspondant à un caption.
			if (caption == null)
			{
				return null;
			}

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

				if (this.IsAbstract2)
				{
					CultureMap item = this.collectionView.Items[index] as CultureMap;
					return this.GetModification(item, cultureName);
				}
				else if (this.IsBundlesType)
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
							ResourceManager.SetSourceBundle(caption, this.accessBundle);
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

		public ModificationState GetModification(CultureMap item, string cultureName)
		{
			//	Donne l'état 'modifié'.
			if (this.IsAbstract2)
			{
				if (cultureName == null)
				{
					cultureName = Resources.DefaultTwoLetterISOLanguageName;
				}

				if (item == null)
				{
					return ModificationState.Empty;
				}

				StructuredData data = item.GetCultureData(cultureName);

				if (data.IsEmpty)
				{
					return ModificationState.Empty;
				}

				if (this.type == Type.Strings2)
				{
					string text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;
					if (string.IsNullOrEmpty(text))
					{
						return ModificationState.Empty;
					}
				}

				if (this.type == Type.Captions2 || this.type == Type.Commands2)
				{
					IList<string> labels = data.GetValue(Support.Res.Fields.ResourceCaption.Labels) as IList<string>;
					string text = data.GetValue(Support.Res.Fields.ResourceCaption.Description) as string;
					if (labels.Count == 0 && string.IsNullOrEmpty(text))
					{
						return ModificationState.Empty;
					}
				}

				if (cultureName != Resources.DefaultTwoLetterISOLanguageName)  // culture secondaire ?
				{
					StructuredData primaryData = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
					int pmod = this.GetModificationId(primaryData);
					int cmod = this.GetModificationId(data);
					if (pmod > cmod)
					{
						return ModificationState.Modified;
					}
				}

				return ModificationState.Normal;
			}

			return ModificationState.Normal;
		}

		protected static bool IsEmptyCollection(ICollection<string> collection)
		{
			//	Indique si une collection de strings doit être considérée comme vide.
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
			//	Considère une ressource comme 'à jour' dans une culture.
			this.CacheResource(index, cultureName);

			if (this.IsAbstract2)
			{
				CultureMap item = this.collectionView.Items[index] as CultureMap;
				StructuredData data = item.GetCultureData(cultureName);
				StructuredData primaryData = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

				int primaryValue = this.GetModificationId(primaryData);
				data.SetValue(Support.Res.Fields.ResourceBase.ModificationId, primaryValue);
			}
			else if (this.IsBundlesType)
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
			if (this.IsAbstract2)
			{
				CultureMap item = this.collectionView.Items[index] as CultureMap;
				StructuredData primaryData = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

				int value = this.GetModificationId(primaryData);
				primaryData.SetValue(Support.Res.Fields.ResourceBase.ModificationId, value+1);
			}
			else if (this.IsBundlesType)
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
			if (this.IsAbstract2)
			{
				CultureMap item = this.collectionView.Items[index] as CultureMap;
				StructuredData primaryData = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				int primaryValue = this.GetModificationId(primaryData);

				List<string> cultures = this.GetSecondaryCultureNames();
				int count = 0;
				foreach (string culture in cultures)
				{
					StructuredData data = item.GetCultureData(culture);
					int value = this.GetModificationId(data);

					if (value < primaryValue)
					{
						count++;
					}
				}
				return (count != cultures.Count);
			}
			else if (this.IsBundlesType)
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

		protected int GetModificationId(StructuredData data)
		{
			if (data.IsEmpty)
			{
				return 0;
			}
			else
			{
				object value = data.GetValue(Support.Res.Fields.ResourceBase.ModificationId);
				
				if (UndefinedValue.IsUndefinedValue(value))
				{
					return 0;
				}
				else
				{
					return (int) value;
				}
			}
		}


		public void SearcherIndexToAccess(int field, string secondaryCulture, out string cultureName, out FieldType fieldType)
		{
			//	Conversion d'un index de champ (0..n) en l'information nécessaire pour Get/SetField.
			if (this.type == Type.Strings || this.type == Type.Strings2)
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

			if (this.IsCaptionsType || this.type == Type.Captions2 || this.type == Type.Commands2)
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
			if (this.IsAbstract2)
			{
			}
			else if (this.IsBundlesType)
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
								ResourceManager.SetSourceBundle(this.accessCaption, this.accessBundle);
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


		public int CultureCount
		{
			//	Retourne le nombre de cultures.
			get
			{
				if (this.bundles == null)
				{
					return 0;
				}
				else
				{
					return this.bundles.Count;
				}
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
				System.Globalization.CultureInfo culture = Resources.FindCultureInfo(cultureName);
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
				if (this.IsCaptionsType)
				{
					this.CreateFirstField(bundle, 0, ResourceAccess.GetFixFilter(Type.Captions)+Res.Strings.Viewers.Panels.New);
					this.CreateFirstField(bundle, 1, ResourceAccess.GetFixFilter(Type.Commands)+Res.Strings.Viewers.Panels.New);
					this.CreateFirstField(bundle, 2, ResourceAccess.GetFixFilter(Type.Types)+Res.Strings.Viewers.Panels.New);
					this.CreateFirstField(bundle, 3, ResourceAccess.GetFixFilter(Type.Values)+Res.Strings.Viewers.Panels.New);
					this.CreateFirstField(bundle, 4, ResourceAccess.GetFixFilter(Type.Fields)+Res.Strings.Viewers.Panels.New);
				}
				else
				{
					this.CreateFirstField(bundle, 0, Res.Strings.Viewers.Panels.New);
				}

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

			if (this.IsCaptionsType)
			{
				this.CaptionsCountersUpdate();

				if (this.captionCounters[Type.Values] == 0)
				{
					Druid newDruid = this.CreateUniqueDruid();
					this.CreateFirstField(this.primaryBundle, newDruid.Local, ResourceAccess.GetFixFilter(Type.Values)+Res.Strings.Viewers.Panels.New);
				}

				if (this.captionCounters[Type.Fields] == 0)
				{
					Druid newDruid = this.CreateUniqueDruid();
					this.CreateFirstField(this.primaryBundle, newDruid.Local, ResourceAccess.GetFixFilter(Type.Fields)+Res.Strings.Viewers.Panels.New);
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
			//	Ajuste les bundles après une désérialisation.
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
			//	Ajuste les bundles avant une sérialisation.
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
							ResourceManager.SetSourceBundle(caption, bundle);
						}

						string name = caption.Name;
						this.AdjustCaptionName(bundle, field, caption);
						System.Diagnostics.Debug.Assert(caption.Name == name);

						field.SetStringValue(caption.SerializeToString());
					}

					if (bundle != this.primaryBundle)
					{
						//	Supprime le 'name="Truc.Chose' dans tous les bundles,
						//	sauf dans le bundle par défaut.
						field.SetName(null);

						//	Si une ressource est vide dans un bundle autre que le bundle
						//	par défaut, il faut la supprimer.
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
			//	Met à jour le caption.Name en fonction de field.Name.
			//	Le Caption.Name doit contenir le nom de la commande, sans le préfixe,
			//	dans le bundle par défaut. Dans les autres bundles, il ne faut rien.
			//	Pour une Value, caption.Name ne contient que le dernier nom.
			if (bundle == this.primaryBundle)
			{
				string name;

				if (field.Name.StartsWith(ResourceAccess.GetFixFilter(Type.Values)))
				{
					name = ResourceAccess.LastName(field.Name);
				}
				else
				{
					name = ResourceAccess.SubAllFilter(field.Name);
				}

				if ((this.bypassType == Type.Unknow && this.type == Type.Fields) || this.bypassType == Type.Fields)
				{
					int i = name.LastIndexOf(".");
					if (i != -1)  // commence par le nom de l'entité ?
					{
						name = name.Substring(i+1);
					}
				}

				caption.Name = name;

#if true
				StructuredType st = this.CachedAbstractType as StructuredType;
				if (st != null)
				{
					if (ResourceManager.GetSourceBundle(st.Caption) == null)
					{
						//	Ne devrait jamais être nécessaire !
						ResourceManager.SetSourceBundle(st.Caption, this.accessBundle);
					}
				}
#endif
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
			//	Ajoute tous les raccourcis définis dans la liste.
			for (int i=0; i<bundle.FieldCount; i++)
			{
				ResourceBundle.Field field = bundle[i];

				Caption caption = new Caption();

				string s = field.AsString;
				if (!string.IsNullOrEmpty(s))
				{
					caption.DeserializeFromString(s, this.resourceManager);
					ResourceManager.SetSourceBundle(caption, bundle);
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


		public int SortDefer(int index)
		{
			//	Empêche tous les tris jusqu'au Undefer.
			//	Retourne l'index pour accéder à une ressource après la suppression du tri. En effet,
			//	comme il n'y aura plus de tri, l'index change !
			if (this.collectionView != null)
			{
				CultureMap item = this.collectionView.Items[index] as CultureMap;
				this.SortDefer();
				index = this.collectionView.Items.IndexOf(item);
			}

			return index;
		}

		public int SortUndefer(int index)
		{
			//	Permet de nouveau les tris.
			//	Retourne l'index pour accéder à une ressource après la remise du tri. En effet,
			//	avec le nouveau tri, l'index change !
			if (this.collectionView != null)
			{
				CultureMap item = this.collectionView.Items[index] as CultureMap;
				this.SortUndefer();
				index = this.collectionView.Items.IndexOf(item);
			}

			return index;
		}

		public void SortDefer()
		{
			//	Empêche tous les tris jusqu'au Undefer.
			if (this.collectionView != null)
			{
				System.Diagnostics.Debug.Assert(this.collectionViewInitialSorts == null);
				this.collectionViewInitialSorts = this.collectionView.SortDescriptions.ToArray();
				this.collectionView.SortDescriptions.Clear();
			}
		}

		public void SortUndefer()
		{
			//	Permet de nouveau les tris.
			if (this.collectionView != null)
			{
				System.Diagnostics.Debug.Assert(this.collectionViewInitialSorts != null);
				this.collectionView.SortDescriptions.AddRange(this.collectionViewInitialSorts);
				this.collectionViewInitialSorts = null;
			}
		}

		public int Sort(int index, bool resortAll)
		{
			//	A partir d'une liste déjà triée, déplace un seul élément modifié pour qu'il
			//	soit de nouveau trié. Si resortAll = true, trie toutes les ressources et retourne
			//	le nouvel index du Druid.
			if (this.IsAbstract2)
			{
				return index;
			}

			this.CacheClear();

			if (resortAll)
			{
				Druid druid = this.druidsIndex[index];
				this.Sort();
				return this.druidsIndex.IndexOf(druid);
			}
			else
			{
				Druid druid = this.druidsIndex[index];
				this.druidsIndex.RemoveAt(index);

				int i=0;
				if (this.IsBundlesType)
				{
					i = this.druidsIndex.BinarySearch(druid, new FieldDruidSort(this));
				}
				if (this.type == Type.Panels)
				{
					i = this.druidsIndex.BinarySearch(druid, new PanelDruidSort(this));
				}

				if (i < 0)
				{
					i = -1-i;  // 0..n
				}

				this.druidsIndex.Insert(i, druid);
				return i;
			}
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
			//	Met à jour les comptes des ressources 'Captions'.
			System.Diagnostics.Debug.Assert(this.IsCaptionsType);

			int countCap = 0;
			int countFld = 0;
			int countCmd = 0;
			int countTyp = 0;
			int countVal = 0;

			string filterCap = ResourceAccess.GetFixFilter(Type.Captions);
			string filterFld = ResourceAccess.GetFixFilter(Type.Fields);
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

				if (name.StartsWith(filterFld))
				{
					countFld++;
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
			this.captionCounters[Type.Fields] = countFld;
			this.captionCounters[Type.Commands] = countCmd;
			this.captionCounters[Type.Types] = countTyp;
			this.captionCounters[Type.Values] = countVal;
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

			//	Supprime tous les panneaux mis dans la liste 'à supprimer'.
			foreach (ResourceBundle bundle in this.panelsToDelete)
			{
				this.resourceManager.RemoveBundle(bundle.Id.ToBundleId(), ResourceLevel.Default, bundle.Culture);
			}
			this.panelsToDelete.Clear();
		}

		public UI.Panel GetPanel(int index)
		{
			//	Retourne le UI.Panel associé à une ressource.
			//	Si nécessaire, il est créé la première fois.
			return this.GetPanel(this.PanelBundle(index));
		}

		public UI.Panel GetPanel(ResourceBundle bundle)
		{
			//	Retourne le UI.Panel associé à un bundle.
			//	Si nécessaire, il est créé la première fois.
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
			//	Crée un nouveau panneau vide.
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
				return (this.type == Type.Strings || this.type == Type.Strings2 || this.type == Type.Captions2 || this.type == Type.Commands2 || this.type == Type.Entities || this.IsCaptionsType);
			}
		}

		protected bool IsCaptionsType
		{
			//	Retourne true si on accède à des ressources de type Captions/Commands/Types/Values.
			get
			{
				return (this.type == Type.Captions || this.type == Type.Fields || this.type == Type.Commands || this.type == Type.Types || this.type == Type.Values);
			}
		}

		protected bool IsAbstract2
		{
			//	Retourne true si on accède à des ressources de type nouveau.
			get
			{
				return (this.type == Type.Strings2 || this.type == Type.Captions2 || this.type == Type.Commands2 || this.type == Type.Entities);
			}
		}

		protected string BundleName(bool many)
		{
			//	Retourne un nom interne (pour Common.Support & Cie) en fonction du type.
			switch (this.type)
			{
				case Type.Strings:
				case Type.Strings2:
					return many ? "Strings" : "String";

				case Type.Captions:
				case Type.Fields:
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
			//	Retourne la dernière partie d'un nom séparé par des points.
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
			//	Ajoute le filtre fixe si nécessaire.
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
			//	Supprime le filtre fixe si nécessaire.
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
			//	Supprime tous les filtres fixes connus si nécessaire.
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
			//	Retourne le type en fonction du préfixe du nom.
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

			filter = ResourceAccess.GetFixFilter(Type.Fields);
			if (name.StartsWith(filter))
			{
				return Type.Fields;
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
			//	Retourne la chaîne fixe du filtre.
			return ResourceAccess.GetFixFilter(bypass ? this.bypassType : this.type);
		}

		protected static string GetFixFilter(Type type)
		{
			//	Retourne la chaîne fixe du filtre pour un type donné.
			switch (type)
			{
				case Type.Captions:
					return "Cap.";

				case Type.Fields:
					return "Fld.";

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
				//	Nom du Command associé (normalement "Cap.*").
				get
				{
					return this.name;
				}
			}

			public string Culture
			{
				//	Nom standard de la culture associée.
				get
				{
					return this.culture;
				}
			}

			static public bool IsEqual(ShortcutItem item1, ShortcutItem item2)
			{
				//	Indique si deux raccourcis sont identiques (mêmes raccourcis pour la même culture).
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
				//	Retourne la liste des index du raccourci dont on spécifie l'index,
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
		/// Permet d'accéder à un champ de n'importe quel type.
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
			if (this.DirtyChanged != null)  // qq'un écoute ?
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
		protected bool										isJustLoaded = false;

		protected Support.ResourceAccessors.AbstractResourceAccessor accessor;
		protected CollectionView							collectionView;
		protected Searcher.SearchingMode					collectionViewMode;
		protected string									collectionViewFilter;
		protected Regex										collectionViewRegex;
		protected Types.SortDescription[]					collectionViewInitialSorts;

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
