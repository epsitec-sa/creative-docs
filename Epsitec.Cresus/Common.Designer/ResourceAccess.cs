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
			Fields,
			Commands,
			Types,
			Values,
			Entities,
			Panels,
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

		public enum ModificationState
		{
			Normal,			//	d�fini normalement
			Empty,			//	vide ou ind�fini (fond rouge)
			Modified,		//	modifi� (fond jaune)
		}


		public ResourceAccess(Type type, Module module, ResourceModuleId moduleInfo, DesignerApplication designerApplication)
		{
			//	Constructeur unique pour acc�der aux ressources d'un type donn�.
			//	Par la suite, l'instance cr��e acc�dera toujours aux ressources de ce type,
			//	sauf pour les ressources Captions, Commands, Types et Values.
			this.type = type;
			this.resourceManager = module.ResourceManager;
			this.moduleInfo = moduleInfo;
			this.designerApplication = designerApplication;

			if (this.IsBundlesType)
			{
				if (this.type == Type.Strings)
				{
					this.accessor = new Support.ResourceAccessors.StringResourceAccessor();
				}
				if (this.type == Type.Captions)
				{
					this.accessor = new Support.ResourceAccessors.CaptionResourceAccessor();
				}
				if (this.type == Type.Commands)
				{
					this.accessor = new Support.ResourceAccessors.CommandResourceAccessor();
				}
				if (this.type == Type.Entities)
				{
					this.accessor = new Support.ResourceAccessors.StructuredTypeResourceAccessor();
				}
				if (this.type == Type.Types)
				{
					this.accessor = new Support.ResourceAccessors.AnyTypeResourceAccessor();
				}
				if (this.type == Type.Fields)
				{
					Support.ResourceAccessors.StructuredTypeResourceAccessor typeAccessor = module.AccessEntities.accessor as Support.ResourceAccessors.StructuredTypeResourceAccessor;
					this.accessor = typeAccessor.FieldAccessor;
				}
				if (this.type == Type.Values)
				{
					Support.ResourceAccessors.AnyTypeResourceAccessor typeAccessor = module.AccessTypes.accessor as Support.ResourceAccessors.AnyTypeResourceAccessor;
					this.accessor = typeAccessor.ValueAccessor;
				}

				this.collectionView = new CollectionView(this.accessor.Collection);
				this.collectionView.Filter = this.CollectionViewFilter;
			}
			else
			{
				this.druidsIndex = new List<Druid>();
				this.filterIndexes = new Dictionary<Type, int>();
				this.filterStrings = new Dictionary<Type, string>();
				this.filterModes = new Dictionary<Type, Searcher.SearchingMode>();
			}
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
					this.type = value;

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

		public IResourceAccessor Accessor
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

				case Type.Fields:
					return many ? Res.Strings.BundleType.Fields : Res.Strings.BundleType.Field;

				case Type.Commands:
					return many ? Res.Strings.BundleType.Commands : Res.Strings.BundleType.Command;

				case Type.Types:
					return many ? Res.Strings.BundleType.Types : Res.Strings.BundleType.Type;

				case Type.Values:
					return many ? Res.Strings.BundleType.Values : Res.Strings.BundleType.Value;

				case Type.Panels:
					return many ? Res.Strings.BundleType.Panels : Res.Strings.BundleType.Panel;

				case Type.Entities:
					return many ? "Entit�s" : "Entit�";
			}

			return "?";
		}


		public static TypeCode AbstractTypeToTypeCode(AbstractType type)
		{
			if (type is BooleanType    )  return TypeCode.Boolean;
			if (type is IntegerType    )  return TypeCode.Integer;
			if (type is LongIntegerType)  return TypeCode.LongInteger;
			if (type is DoubleType     )  return TypeCode.Double;
			if (type is DecimalType    )  return TypeCode.Decimal;
			if (type is StringType     )  return TypeCode.String;
			if (type is EnumType       )  return TypeCode.Enum;
			if (type is StructuredType )  return TypeCode.Structured;
			if (type is CollectionType )  return TypeCode.Collection;
			if (type is DateType       )  return TypeCode.Date;
			if (type is TimeType       )  return TypeCode.Time;
			if (type is DateTimeType   )  return TypeCode.DateTime;
			if (type is BinaryType     )  return TypeCode.Binary;

			return TypeCode.Invalid;
		}

		protected static string TypeCodeController(AbstractType type)
		{
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

		protected static string TypeCodeControllerParameter(AbstractType type)
		{
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

		protected static AbstractType TypeCodeCreate(TypeCode type, ResourceBundle bundle)
		{
			AbstractType abstractType = null;

			switch (type)
			{
				case TypeCode.Boolean:      abstractType = new BooleanType(); break;
				case TypeCode.Integer:      abstractType = new IntegerType(); break;
				case TypeCode.LongInteger:  abstractType = new LongIntegerType(); break;
				case TypeCode.Double:       abstractType = new DoubleType(); break;
				case TypeCode.Decimal:      abstractType = new DecimalType(); break;
				case TypeCode.String:       abstractType = new StringType(); break;
				case TypeCode.Enum:         abstractType = new EnumType(); break;
				case TypeCode.Structured:	abstractType = new StructuredType(StructuredTypeClass.Entity, Druid.Empty); break;
				case TypeCode.Collection:   abstractType = new CollectionType(); break;
				case TypeCode.Date:         abstractType = new DateType(); break;
				case TypeCode.Time:         abstractType = new TimeType(); break;
				case TypeCode.DateTime:     abstractType = new DateTimeType(); break;
				case TypeCode.Binary:       abstractType = new BinaryType(); break;
			}

			if (abstractType != null)
			{
				//	Associe le type avec le bundle dans lequel il sera stock�.
				//	Cela est n�cessaire pour certains types qui doivent savoir
				//	de quel module ils proviennent.
				Caption caption = abstractType.Caption;
				ResourceManager.SetSourceBundle(caption, bundle);
			}

			return abstractType;
		}

		public static string TypeCodeToDisplay(TypeCode type)
		{
			switch (type)
			{
				case TypeCode.Boolean:      return Res.Strings.Viewers.Types.Editor.Boolean;
				case TypeCode.Integer:      return Res.Strings.Viewers.Types.Editor.Integer;
				case TypeCode.LongInteger:  return Res.Strings.Viewers.Types.Editor.LongInteger;
				case TypeCode.Decimal:      return Res.Strings.Viewers.Types.Editor.Decimal;
				case TypeCode.String:       return Res.Strings.Viewers.Types.Editor.String;
				case TypeCode.Enum:         return Res.Strings.Viewers.Types.Editor.Enum;
				case TypeCode.Structured:   return Res.Strings.Viewers.Types.Editor.Structured;
				case TypeCode.Collection:   return Res.Strings.Viewers.Types.Editor.Collection;
				case TypeCode.Date:         return Res.Strings.Viewers.Types.Editor.Date;
				case TypeCode.Time:         return Res.Strings.Viewers.Types.Editor.Time;
				case TypeCode.DateTime:     return Res.Strings.Viewers.Types.Editor.DateTime;
				case TypeCode.Binary:       return Res.Strings.Viewers.Types.Editor.Binary;
			}

			return null;
		}

		public static string TypeCodeToName(TypeCode type)
		{
			switch (type)
			{
				case TypeCode.Boolean:      return "Boolean";
				case TypeCode.Integer:      return "Integer";
				case TypeCode.LongInteger:  return "LongInteger";
				case TypeCode.Double:       return "Double";
				case TypeCode.Decimal:      return "Decimal";
				case TypeCode.String:       return "String";
				case TypeCode.Enum:         return "Enum";
				case TypeCode.Structured:   return "Structured";
				case TypeCode.Collection:   return "Collection";
				case TypeCode.Date:         return "Date";
				case TypeCode.Time:         return "Time";
				case TypeCode.DateTime:     return "DateTime";
				case TypeCode.Binary:       return "Binary";
			}

			return null;
		}

		public static TypeCode NameToTypeCode(string name)
		{
			switch (name)
			{
				case "Boolean":      return TypeCode.Boolean;
				case "Integer":      return TypeCode.Integer;
				case "LongInteger":  return TypeCode.LongInteger;
				case "Double":       return TypeCode.Double;
				case "Decimal":      return TypeCode.Decimal;
				case "String":       return TypeCode.String;
				case "Enum":         return TypeCode.Enum;
				case "Structured":   return TypeCode.Structured;
				case "Collection":   return TypeCode.Collection;
				case "Date":         return TypeCode.Date;
				case "Time":         return TypeCode.Time;
				case "DateTime":     return TypeCode.DateTime;
				case "Binary":       return TypeCode.Binary;
			}

			return TypeCode.Invalid;
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
			if (this.IsBundlesType)
			{
				//?this.accessor.Load(this.resourceManager);
				Support.ResourceAccessors.AbstractResourceAccessor a = this.accessor as Support.ResourceAccessors.AbstractResourceAccessor;
				a.Load(this.resourceManager);
				this.collectionView.MoveCurrentToFirst();
				this.LoadBundles();
			}

			if (this.type == Type.Panels)
			{
				this.LoadPanels();
			}

			this.SetFilter("", Searcher.SearchingMode.None);

			this.ClearGlobalDirty();
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

			this.ClearGlobalDirty();
		}


		public void AddShortcuts(List<ShortcutItem> list)
		{
			//	Ajoute tous les raccourcis d�finis dans la liste.
			//	TODO:
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


		public void ClearGlobalDirty()
		{
			//	Met les ressources dans l'�tat "propre", c'est-�-dire "non modifi�es".
			if (this.isGlobalDirty || this.isLocalDirty)
			{
				this.isGlobalDirty = false;
				this.isLocalDirty = false;
				this.OnDirtyChanged();
			}
		}

		public void SetGlobalDirty()
		{
			//	Met les ressources dans l'�tat "sale", c'est-�-dire "modifi�es".
			if (!this.isGlobalDirty)
			{
				this.isGlobalDirty = true;
				this.OnDirtyChanged();
			}
		}

		public bool IsGlobalDirty
		{
			//	Est-ce que les ressources ont �t� modifi�es ?
			get
			{
				return this.isGlobalDirty;
			}
		}

		public void ClearLocalDirty()
		{
			//	Met les ressources dans l'�tat "propre", c'est-�-dire "non modifi�es".
			if (this.isLocalDirty)
			{
				this.isLocalDirty = false;
				this.OnDirtyChanged();
			}
		}

		public void SetLocalDirty()
		{
			//	Met les ressources dans l'�tat "sale", c'est-�-dire "modifi�es".
			if (!this.isLocalDirty || !this.isGlobalDirty)
			{
				this.isLocalDirty = true;
				this.isGlobalDirty = true;
				this.OnDirtyChanged();
			}
		}

		public bool IsLocalDirty
		{
			//	Est-ce que les ressources ont �t� modifi�es ?
			get
			{
				return this.isLocalDirty;
			}
		}

		public void PersistChanges()
		{
			this.accessor.PersistChanges();
		}

		public void RevertChanges()
		{
			this.accessor.RevertChanges();
		}


		public void Duplicate(string newName, bool duplicateContent)
		{
			//	Duplique la ressource courante.
			StructuredType st = null;

			if (this.IsBundlesType)
			{
				CultureMap newItem;
				bool generateMissingValues = false;

				if (this.type == Type.Types && !duplicateContent)
				{
					TypeCode code = this.lastTypeCodeCreatated;
					this.designerApplication.DlgResourceTypeCode(this, ref code, out this.lastTypeCodeSystem);
					if (code == TypeCode.Invalid)  // annuler ?
					{
						return;
					}
					
					this.lastTypeCodeCreatated = code;

					newItem = this.accessor.CreateItem();
					newItem.Name = newName;

					StructuredData data = newItem.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
					data.SetValue(Support.Res.Fields.ResourceBaseType.TypeCode, code);

					if (this.lastTypeCodeSystem != null)
					{
						data.SetValue(Support.Res.Fields.ResourceEnumType.SystemType, this.lastTypeCodeSystem);
						generateMissingValues = true;
					}
				}
				else if (this.type == Type.Entities && !duplicateContent)
				{
					newName = this.designerApplication.DlgResourceName(Dialogs.ResourceName.Operation.Create, Dialogs.ResourceName.Type.Entity, newName);
					if (string.IsNullOrEmpty(newName))
					{
						return;
					}

					if (!Misc.IsValidName(ref newName))
					{
						this.designerApplication.DialogError(Res.Strings.Error.Name.Invalid);
						return;
					}

					Druid druid = Druid.Empty;
					StructuredTypeClass typeClass = StructuredTypeClass.Entity;

					//	TODO: il faudra que dans le dialogue on puisse choisir si on
					//	veut cr�er une entit� ou une interface, donc retourner soit
					//	StructuredTypeClass.Entity, soit StructuredTypeClass.Interface
					Common.Dialogs.DialogResult result = this.designerApplication.DlgResourceSelector(Dialogs.ResourceSelector.Operation.InheritEntity, this.designerApplication.CurrentModule, Type.Entities, ref druid, null);
					if (result != Common.Dialogs.DialogResult.Yes)
					{
						return;
					}

					newItem = this.accessor.CreateItem();
					newItem.Name = newName;

					StructuredData data = newItem.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
					data.SetValue(Support.Res.Fields.ResourceStructuredType.BaseType, druid);
					data.SetValue(Support.Res.Fields.ResourceStructuredType.Class, typeClass);
				}
				else
				{
					newItem = this.accessor.CreateItem();
					newItem.Name = newName;
				}

				if (duplicateContent)
				{
					//	Construit la liste des cultures � copier
					List<string> cultures = this.GetSecondaryCultureNames();
					cultures.Insert(0, Resources.DefaultTwoLetterISOLanguageName);
					
					CultureMap item = this.collectionView.CurrentItem as CultureMap;

					foreach (string culture in cultures)
					{
						StructuredData data = item.GetCultureData(culture);
						StructuredData newData = newItem.GetCultureData(culture);
						ResourceAccess.CopyData(this.accessor, newItem, data, newData);
					}
				}

				//	Ici, si c'est un type, on a forc�ment TypeCode qui a �t� initialis� soit
				//	explicitement avec un SetValue, soit par recopie de l'original via CopyData;
				//	c'est indispensable que TypeCode soit d�fini avant de faire le Add :
				this.accessor.Collection.Add(newItem);
				this.collectionView.MoveCurrentTo(newItem);

				if (generateMissingValues)
				{
					Support.ResourceAccessors.AnyTypeResourceAccessor accessor = this.accessor as Support.ResourceAccessors.AnyTypeResourceAccessor;
					accessor.CreateMissingValueItems(newItem);
				}

				this.SetLocalDirty();
				return;
			}

			if (this.type == Type.Panels && !duplicateContent)
			{
				//	Choix d'une ressource type de type 'Types', mais uniquement parmi les TypeCode.Structured.
				Module module = this.designerApplication.CurrentModule;
				Druid druid = Druid.Empty;
				Common.Dialogs.DialogResult result = this.designerApplication.DlgResourceSelector(Dialogs.ResourceSelector.Operation.Selection, module, ResourceAccess.Type.Types, ref druid, null);
				if (result != Common.Dialogs.DialogResult.Yes)  // annuler ?
				{
					return;
				}
			}

			Druid newDruid = Druid.Empty;

			if (this.IsBundlesType)
			{
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

			this.druidsIndex.Add(newDruid);
			this.Sort();
			this.accessIndex = this.druidsIndex.IndexOf(newDruid);

			this.SetGlobalDirty();
		}

		private static void CopyData(IResourceAccessor accessor, CultureMap dstItem, StructuredData src, StructuredData dst)
		{
			//	Copie les donn�es d'un StructuredData vers un autre, en tenant
			//	compte des collections de donn�es qui ne peuvent pas �tre copi�es
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
			//	R�alise un StructuredData.SetValue qui tienne compte des cas
			//	particuliers o� les donn�es � copier sont dans une collection.
			if (data.IsValueLocked(id))
			{
				//	La donn�e que l'on cherche � modifier est verrouill�e; c'est
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
			//	Copie (r�cursivement) les donn�es au niveau actuel en demandant au
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
				CultureMap item = this.collectionView.CurrentItem as CultureMap;
				this.accessor.Collection.Remove(item);
				this.SetLocalDirty();
				return;
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

			this.SetGlobalDirty();
		}


		public void SetFilter(string filter, Searcher.SearchingMode mode)
		{
			//	Construit l'index en fonction des ressources primaires.
			if (this.IsBundlesType)
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
			//	Retourne le nombre de donn�es accessibles.
			get
			{
				if (this.IsBundlesType)
				{
					return this.collectionView.Count;
				}
				else if (this.type == Type.Panels)
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
				if (this.IsBundlesType)
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
			//	Retourne le druid d'un index donn�.
			if (this.IsBundlesType)
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
			if (this.IsBundlesType)
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
			//	Index de l'acc�s en cours.
			get
			{
				if (this.IsBundlesType)
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
				if (this.IsBundlesType)
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


		public bool IsCorrectNewName(ref string name)
		{
			//	Retourne true s'il est possible de cr�er cette nouvelle ressource.
			return (this.CheckNewName(ref name) == null);
		}

		public string CheckNewName(ref string name)
		{
			//	Retourne l'�ventuelle erreur si on tente de cr�er cette nouvelle ressource.
			//	Retourne null si tout est correct.
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
			if (this.IsBundlesType)
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

			int nextNumber = 1;
			if (numberLength > 0)
			{
				nextNumber = int.Parse(baseName.Substring(baseName.Length-numberLength))+1;
				baseName = baseName.Substring(0, baseName.Length-numberLength);
			}

			string newName = baseName;
			if (this.IsCorrectNewName (ref newName))
			{
				return newName;
			}

			for (int i=nextNumber; i<nextNumber+100; i++)
			{
				newName = string.Concat(baseName, i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				if (this.IsCorrectNewName(ref newName))
				{
					break;
				}
			}

			return newName;
		}


		public static string GetCaptionNiceDescription(StructuredData data, double availableHeight)
		{
			//	Construit un texte d'apr�s les labels et la description.
			//	Les diff�rents labels sont s�par�s par des virgules.
			//	La description vient sur une deuxi�me ligne (si la hauteur
			//	disponible le permet), mais seulement si elle est diff�rente
			//	de tous les labels.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			string description = data.GetValue(Support.Res.Fields.ResourceCaption.Description) as string;

			IList<string> list = data.GetValue(Support.Res.Fields.ResourceCaption.Labels) as IList<string>;
			foreach (string label in list)
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


		public Field GetField(int index, string cultureName, FieldType fieldType)
		{
			//	Retourne les donn�es d'un champ.
			//	Si cultureName est nul, on acc�de � la culture de base.
			//	[Note1] Lorsqu'on utilise FieldType.AbstractType ou FieldType.Panel,
			//	les donn�es peuvent �tre modifi�es directement, sans qu'il faille les
			//	redonner lors du SetField. Cela oblige � ne pas faire d'autres GetField
			//	avant le SetField !
			this.lastAccessField = fieldType;

			if (this.IsBundlesType)
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
			if (this.IsBundlesType)
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
					this.SetLocalDirty();
					this.collectionView.Refresh();
				}

				if (fieldType == FieldType.String)
				{
					data.SetValue(Support.Res.Fields.ResourceString.Text, field.String);
					this.SetLocalDirty();
					this.collectionView.Refresh();
				}

				if (fieldType == FieldType.Description)
				{
					data.SetValue(Support.Res.Fields.ResourceCaption.Description, field.String);
					this.SetLocalDirty();
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
					this.SetLocalDirty();
					this.collectionView.Refresh();
				}

				if (fieldType == FieldType.About)
				{
					data.SetValue(Support.Res.Fields.ResourceBase.Comment, field.String);
					this.SetLocalDirty();
					this.collectionView.Refresh();
				}

				this.SetGlobalDirty();
				return;
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

			this.SetGlobalDirty();
		}


		public ModificationState GetModification(int index, string cultureName)
		{
			//	Donne l'�tat 'modifi�'.
			if (index != -1)
			{
				if (this.IsBundlesType)
				{
					CultureMap item = this.collectionView.Items[index] as CultureMap;
					return this.GetModification(item, cultureName);
				}
			}

			return ModificationState.Normal;
		}

		public ModificationState GetModification(CultureMap item, string cultureName)
		{
			//	Donne l'�tat 'modifi�'.
			if (this.IsBundlesType)
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

				if (this.type == Type.Strings)
				{
					string text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;
					if (string.IsNullOrEmpty(text))
					{
						return ModificationState.Empty;
					}
				}

				if (this.type == Type.Captions || this.type == Type.Commands || this.type == Type.Types || this.type == Type.Fields || this.type == Type.Values)
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

		public void ModificationClear(int index, string cultureName)
		{
			//	Consid�re une ressource comme '� jour' dans une culture.
			if (this.IsBundlesType)
			{
				CultureMap item = this.collectionView.Items[index] as CultureMap;
				StructuredData data = item.GetCultureData(cultureName);
				StructuredData primaryData = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

				int primaryValue = this.GetModificationId(primaryData);
				data.SetValue(Support.Res.Fields.ResourceBase.ModificationId, primaryValue);
			}

			this.SetGlobalDirty();
		}

		public void ModificationSetAll(int index)
		{
			//	Consid�re une ressource comme 'modifi�e' dans toutes les cultures.
			if (this.IsBundlesType)
			{
				CultureMap item = this.collectionView.Items[index] as CultureMap;
				StructuredData primaryData = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

				int value = this.GetModificationId(primaryData);
				primaryData.SetValue(Support.Res.Fields.ResourceBase.ModificationId, value+1);
			}

			this.SetGlobalDirty();
		}

		public bool IsModificationAll(int index)
		{
			//	Donne l'�tat de la commande ModificationAll.
			if (this.IsBundlesType)
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

			if (this.type == Type.Captions || this.type == Type.Commands || this.type == Type.Types || this.type == Type.Fields || this.type == Type.Values)
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

		public void CreateCulture(string cultureName)
		{
			//	Cr�e un nouveau bundle pour une culture donn�e.
			System.Diagnostics.Debug.Assert(cultureName.Length == 2);
			if (this.IsBundlesType)
			{
				string prefix = this.resourceManager.ActivePrefix;
				System.Globalization.CultureInfo culture = Resources.FindCultureInfo(cultureName);
				ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, this.bundles.Name, ResourceLevel.Localized, culture);

				bundle.DefineType(this.BundleName(false));
				this.resourceManager.SetBundle(bundle, ResourceSetMode.CreateOnly);

				this.LoadBundles();
				this.SetGlobalDirty();
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
					this.SetGlobalDirty();
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
				this.CreateFirstField(bundle, 0, Res.Strings.Viewers.Panels.New);

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
		}

		protected void CreateFirstField(ResourceBundle bundle, int localId, string name)
		{
			int moduleId = bundle.Module.Id;
			int developerId = 0;  // [PA] provisoire
			Druid newDruid = new Druid(moduleId, developerId, localId);

			ResourceBundle.Field newField = bundle.CreateField(ResourceFieldType.Data);
			newField.SetDruid(newDruid);
			newField.SetName(name);
			newField.SetStringValue("");

			bundle.Add(newField);
		}

		protected void AdjustBundlesBeforeSave()
		{
			//	Ajuste les bundles avant une s�rialisation.
			foreach (ResourceBundle bundle in this.bundles)
			{
				bool patchModule = bundle.BasedOnPatchModule;

				for (int i=0; i<bundle.FieldCount; i++)
				{
					ResourceBundle.Field field = bundle[i];

					if (!patchModule)
					{
						if (field.About == "" || ResourceBundle.Field.IsNullString (field.About))
						{
							//	Si un champ contient un commentaire vide et qu'il
							//	s'agit d'une ressource d'un module de r�f�rence,
							//	alors on peut supprimer compl�tement son contenu.

							field.SetAbout(null);
						}
						
						if (bundle != this.primaryBundle)
						{
							System.Diagnostics.Debug.Assert (field.Name == null);

							//	Si une ressource est vide dans un bundle autre que le bundle
							//	par d�faut, il faut la supprimer.
							if ((ResourceBundle.Field.IsNullString (field.AsString)) &&
							(ResourceBundle.Field.IsNullString (field.About)))
							{
								bundle.Remove (i);
								i--;
							}
						}
					}
				}
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
				string name = field.Name;

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
				string name = bundle.Caption;

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
			//	Emp�che tous les tris jusqu'au Undefer.
			//	Retourne l'index pour acc�der � une ressource apr�s la suppression du tri. En effet,
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
			//	Retourne l'index pour acc�der � une ressource apr�s la remise du tri. En effet,
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
			//	Emp�che tous les tris jusqu'au Undefer.
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
			//	A partir d'une liste d�j� tri�e, d�place un seul �l�ment modifi� pour qu'il
			//	soit de nouveau tri�. Si resortAll = true, trie toutes les ressources et retourne
			//	le nouvel index du Druid.
			if (this.IsBundlesType)
			{
				return index;
			}

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

				string name1 = field1.Name ?? "";
				string name2 = field2.Name ?? "";

				return name1.CompareTo(name2);
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
				return (this.type == Type.Strings || this.type == Type.Captions || this.type == Type.Commands || this.type == Type.Entities || this.type == Type.Types || this.type == Type.Fields || this.type == Type.Values);
			}
		}

		protected string BundleName(bool many)
		{
			//	Retourne un nom interne (pour Common.Support & Cie) en fonction du type.
			switch (this.type)
			{
				case Type.Strings:
					return many ? "Strings" : "String";

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
				AbstractType,
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

			public Field(AbstractType value)
			{
				this.type = Type.AbstractType;
				this.abstractType = value;
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

			public AbstractType AbstractType
			{
				get
				{
					System.Diagnostics.Debug.Assert(this.type == Type.AbstractType);
					return this.abstractType;
				}
			}

			protected Type type;
			protected string									stringValue;
			protected ICollection<string>						stringCollection;
			protected ResourceBundle							bundle;
			protected AbstractType								abstractType;
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
		protected ResourceModuleId							moduleInfo;
		protected DesignerApplication						designerApplication;
		protected bool										isGlobalDirty = false;
		protected bool										isLocalDirty = false;
		protected bool										isJustLoaded = false;

		protected IResourceAccessor							accessor;
		protected CollectionView							collectionView;
		protected Searcher.SearchingMode					collectionViewMode;
		protected string									collectionViewFilter;
		protected Regex										collectionViewRegex;
		protected Types.SortDescription[]					collectionViewInitialSorts;

		protected ResourceBundleCollection					bundles;
		protected ResourceBundle							primaryBundle;
		protected List<Druid>								druidsIndex;
		protected int										accessIndex;
		protected FieldType									lastAccessField = FieldType.None;
		protected List<ResourceBundle>						panelsList;
		protected List<ResourceBundle>						panelsToCreate;
		protected List<ResourceBundle>						panelsToDelete;
		protected Dictionary<Type, int>						filterIndexes;
		protected Dictionary<Type, string>					filterStrings;
		protected Dictionary<Type, Searcher.SearchingMode>	filterModes;
		protected TypeCode									lastTypeCodeCreatated = TypeCode.String;
		protected System.Type								lastTypeCodeSystem = null;
	}
}
