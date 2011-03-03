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
			Unknown,
			Strings,
			Captions,
			Fields,
			Commands,
			Types,
			Values,
			Entities,
			Panels,
			Forms,
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
			Normal,			//	défini normalement
			Empty,			//	vide ou indéfini (fond rouge)
			Modified,		//	modifié (fond jaune)
		}


		public ResourceAccess(Type type, Module module, ResourceModuleId moduleInfo)
		{
			//	Constructeur unique pour accéder aux ressources d'un type donné.
			//	Par la suite, l'instance créée accédera toujours aux ressources de ce type,
			//	sauf pour les ressources Captions, Commands, Types et Values.
			this.type = type;
			this.resourceManager = module.ResourceManager;
			this.batchSaver = module.BatchSaver;
			this.moduleInfo = moduleInfo;
			this.designerApplication = module.DesignerApplication;

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
			if (this.type == Type.Panels)
			{
				this.accessor = new Support.ResourceAccessors.PanelResourceAccessor();
			}
			if (this.type == Type.Forms)
			{
				this.accessor = new Support.ResourceAccessors.FormResourceAccessor();
			}

			this.collectionView = new CollectionView(this.accessor.Collection);
			this.collectionView.Filter = this.CollectionViewFilter;

			this.lastLifetime = DataLifetimeExpectancy.Stable;
			this.lastFlags = StructuredTypeFlags.None;
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
					this.type = value;

					this.SetFilter("", Searcher.SearchingMode.None);
					this.AccessIndex = 0;
				}
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
			//	Retourne le nom au pluriel correspondant à un type.
			return ResourceAccess.TypeDisplayName(type, true);
		}

		public static string TypeDisplayName(Type type, bool many)
		{
			//	Retourne le nom au singulier ou au pluriel correspondant à un type.
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
					return many ? Res.Strings.BundleType.Entities : Res.Strings.BundleType.Entity;

				case Type.Forms:
					return many ? Res.Strings.BundleType.Forms : Res.Strings.BundleType.Form;
			}

			return "?";
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
				case TypeCode.Other:        return "Native";
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
				case "Native":       return TypeCode.Other;
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

		public void Load(Module ownerModule)
		{
			this.ownerModule = ownerModule;
			
			this.CreateEmptyBundle();
			this.accessor.Load(this.resourceManager);
			this.collectionView.MoveCurrentToFirst();
			this.LoadBundles();

			this.SetFilter("", Searcher.SearchingMode.None);

			this.ClearGlobalDirty();
			this.isJustLoaded = true;
		}

		public void Save(ResourceBundleBatchSaver saver)
		{
			//	Enregistre les modifications des ressources.
			this.SaveBundles(saver);
			this.ClearGlobalDirty();
		}

		public void RegenerateAllFieldsInBundle()
		{
			foreach (CultureMap item in this.accessor.Collection)
			{
				this.accessor.NotifyItemChanged(item, null, null);
			}
		}

		public void AddShortcuts(List<ShortcutItem> list)
		{
			//	Ajoute tous les raccourcis définis dans la liste.
			if (this.type == Type.Commands)
			{
				List<string> cultures = this.GetSecondaryCultureNames();
				cultures.Insert(0, Resources.DefaultTwoLetterISOLanguageName);

				foreach (string culture in cultures)
				{
					foreach (CultureMap item in this.accessor.Collection)
					{
						StructuredData data = item.GetCultureData(culture);
						IList<StructuredData> shortcuts = data.GetValue(Support.Res.Fields.ResourceCommand.Shortcuts) as IList<StructuredData>;

						for (int i=0; i<shortcuts.Count; i++)
						{
							string keyCode = shortcuts[i].GetValue(Support.Res.Fields.Shortcut.KeyCode) as string;
							KeyCode kc = (KeyCode) System.Enum.Parse(typeof(KeyCode), keyCode);
							if (kc != KeyCode.None)
							{
								Shortcut sc = new Shortcut(kc);
								ShortcutItem si = new ShortcutItem(sc, item.Name, this.GetCultureName(culture));
								list.Add(si);
							}
						}
					}
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
						builder.Append("<br/>");
					}

					builder.Append("<font size=\"120%\">");
					builder.Append(string.Format(Res.Strings.Error.ShortcutMany, list[i].Culture));
					builder.Append("</font><br/>");
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


		public void CheckForms(System.Text.StringBuilder builder)
		{
			//	Vérifie les Forms, en construisant un message d'avertissement.
			System.Diagnostics.Debug.Assert(this.type == Type.Forms);
			FormEngine.Engine engine = new FormEngine.Engine(this.ownerModule.FormResourceProvider);
			bool first;

			//	Vérifie la structure des Forms.
			first = true;
			foreach (CultureMap item in this.accessor.Collection)
			{
				FormEngine.FormDescription form = this.GetForm(item);

				string error = engine.Arrange.Check(form.Fields);

				if (!string.IsNullOrEmpty(error))  // une erreur ?
				{
					if (first)  // premier avertissement de Form ?
					{
						first = false;

						if (builder.Length > 0)  // déjà d'autres avertissements ?
						{
							builder.Append("<br/>");
						}

						builder.Append("<font size=\"120%\">");
						builder.Append("Ces masques contiennent des erreurs :");
						builder.Append("</font><br/>");
					}

					//	Génère une erreur explicite.
					builder.Append("<list type=\"fix\" width=\"1.5\"/>");
					builder.Append(string.Format("<b>{0}</b>: {1}", item.FullName, error));
					builder.Append("<br/>");
				}
			}

			//	Vérifie les liens DeltaAttachGuid.
			first = true;
			foreach (CultureMap item in this.accessor.Collection)
			{
				FormEngine.FormDescription form = this.GetForm(item);

				if (form.DeltaBaseFormId.IsEmpty)  // pas un Form delta ?
				{
					continue;  // toujours ok
				}

				//	Fusionne le masque de base selon les indications du masque delta, pour
				//	déterminer les éventuelles erreurs, qui sont directement insérées dans
				//	les éléments FieldDescription de form. Donc, les deux listes générées
				//	baseFields et finalFields ne sont pas utiles !
				List<FormEngine.FieldDescription> baseFields, finalFields;
				Druid entityId;
				engine.Arrange.Build(form, null, out baseFields, out finalFields, out entityId);

				//	Compte le nombre de liens cassés.
				List<string> errors = new List<string>();
				foreach (FormEngine.FieldDescription field in form.Fields)
				{
					if (field.DeltaBrokenAttach)
					{
						string name = this.GetFieldNames(field.GetPath(null));

						if (string.IsNullOrEmpty(name))
						{
							name = field.Description;
						}

						errors.Add(name);
					}
				}

				if (errors.Count > 0)  // au moins un lien cassé ?
				{
					if (first)  // premier avertissement de Form ?
					{
						first = false;

						if (builder.Length > 0)  // déjà d'autres avertissements ?
						{
							builder.Append("<br/>");
						}

						builder.Append("<font size=\"120%\">");
						builder.Append("Ces masques contiennent des références indéfinies :");
						builder.Append("</font><br/>");
					}

					//	Génère une erreur explicite.
					builder.Append("<list type=\"fix\" width=\"1.5\"/>");
					builder.Append(string.Format("<b>{0}</b>: ", item.FullName));

					for (int i=0; i<errors.Count; i++)
					{
						if (i > 0)
						{
							if (i == errors.Count-1)
							{
								builder.Append(" et ");
							}
							else
							{
								builder.Append(", ");
							}
						}

						builder.Append(errors[i]);
					}

					builder.Append("<br/>");
				}
			}
		}


		public void ClearGlobalDirty()
		{
			//	Met les ressources dans l'état "propre", c'est-à-dire "non modifiées".
			if (this.isGlobalDirty || this.isLocalDirty)
			{
				this.isGlobalDirty = false;
				this.isLocalDirty = false;
				this.OnDirtyChanged();
			}
		}

		public void SetGlobalDirty()
		{
			//	Met les ressources dans l'état "sale", c'est-à-dire "modifiées".
			if (!this.isGlobalDirty)
			{
				this.isGlobalDirty = true;
				this.OnDirtyChanged();
			}
		}

		public bool IsGlobalDirty
		{
			//	Est-ce que les ressources ont été modifiées ?
			get
			{
				return this.isGlobalDirty;
			}
		}

		public void ClearLocalDirty()
		{
			//	Met les ressources dans l'état "propre", c'est-à-dire "non modifiées".
			if (this.isLocalDirty)
			{
				this.isLocalDirty = false;
				this.OnDirtyChanged();
			}
		}

		public void SetLocalDirty()
		{
			//	Met les ressources dans l'état "sale", c'est-à-dire "modifiées".
			if (!this.isLocalDirty || !this.isGlobalDirty)
			{
				this.isLocalDirty = true;
				this.isGlobalDirty = true;
				this.OnDirtyChanged();
			}
		}

		public bool IsLocalDirty
		{
			//	Est-ce que les ressources ont été modifiées ?
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


		public bool Duplicate(string newName, bool duplicateContent)
		{
			//	Duplique la ressource courante. Retourne false si rien n'a été créé.
			CultureMap newItem = null;
			bool generateMissingValues = false;

			if (this.type == Type.Types && !duplicateContent)
			{
				TypeCode code = this.lastTypeCodeCreatated;
				this.designerApplication.DlgResourceTypeCode(this, ref code, out this.lastTypeCodeSystem);
				if (code == TypeCode.Invalid)  // annuler ?
				{
					return false;
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
#if true
				var typeClass = StructuredTypeClass.Entity;
				var druid     = Druid.Empty;
				var lifetime  = this.lastLifetime;
				var flags     = this.lastFlags;

				var result = this.designerApplication.DlgEntityCreation (this.ownerModule, ref newName, ref typeClass, ref druid, ref lifetime, ref flags);

				if (result != Common.Dialogs.DialogResult.Accept)
				{
					return false;
				}

				if (!Misc.IsValidLabel (ref newName, true))
				{
					this.designerApplication.DialogError (Res.Strings.Error.Name.Invalid);
					return false;
				}

				newItem = this.accessor.CreateItem ();
				newItem.Name = newName;

				StructuredData data = newItem.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);

				data.SetValue (Support.Res.Fields.ResourceStructuredType.Class, typeClass);
				data.SetValue (Support.Res.Fields.ResourceStructuredType.BaseType, druid);

				//	StructuredTypeFlags et DataLifetimeExpectancy n'ont pas de sens pour une interface.
				if (typeClass != StructuredTypeClass.Interface)
				{
					data.SetValue (Support.Res.Fields.ResourceStructuredType.DefaultLifetimeExpectancy, lifetime);
					data.SetValue (Support.Res.Fields.ResourceStructuredType.Flags, flags);
				}

				this.lastLifetime = lifetime;
				this.lastFlags    = flags;
#else
				//	Demande le nom.
				newName = this.designerApplication.DlgResourceName(Dialogs.ResourceName.Operation.Create, Dialogs.ResourceName.Type.Entity, newName);
				if (string.IsNullOrEmpty(newName))
				{
					return false;
				}

				if (!Misc.IsValidLabel(ref newName, true))
				{
					this.designerApplication.DialogError(Res.Strings.Error.Name.Invalid);
					return false;
				}

				Druid druid = Druid.Empty;
				bool isNullable = false;
				StructuredTypeClass typeClass = StructuredTypeClass.Entity;

				//	Demande le type (interface, héritage, etc.).
				var result = this.designerApplication.DlgResourceSelector(Dialogs.ResourceSelector.Operation.InheritEntities, this.ownerModule, Type.Entities, ref typeClass, ref druid, ref isNullable, null, Druid.Empty);
				if (result != Common.Dialogs.DialogResult.Yes)
				{
					return false;
				}

				newItem = this.accessor.CreateItem();
				newItem.Name = newName;

				StructuredData data = newItem.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				data.SetValue(Support.Res.Fields.ResourceStructuredType.BaseType, druid);
				data.SetValue(Support.Res.Fields.ResourceStructuredType.Class, typeClass);

				//	Demande les paramètres (fanions et espérance de vie).
				var lifetime = DataLifetimeExpectancy.Stable;
				var flags    = StructuredTypeFlags.None;
				result = this.designerApplication.DlgEntityParameters (null, ref lifetime, ref flags);

				if (result != Common.Dialogs.DialogResult.Accept)
				{
					return false;
				}

				data.SetValue (Support.Res.Fields.ResourceStructuredType.DefaultLifetimeExpectancy, lifetime);
				data.SetValue (Support.Res.Fields.ResourceStructuredType.Flags,                     flags);
#endif
			}
			else if (this.type == Type.Forms && !duplicateContent)
			{
				string header = Res.Strings.Forms.Question.Create.Base;

				List<string> questions = new List<string>();
				questions.Add(ConfirmationButton.FormatContent(Res.Strings.Forms.Question.Create.Quick.Normal, Res.Strings.Forms.Question.Create.Long.Normal));
				questions.Add(ConfirmationButton.FormatContent(Res.Strings.Forms.Question.Create.Quick.Delta, Res.Strings.Forms.Question.Create.Long.Delta));

				var result = this.designerApplication.DialogConfirmation (header, questions, true);
				if (result == Epsitec.Common.Dialogs.DialogResult.Cancel)
				{
					return false;
				}

				if (result == Epsitec.Common.Dialogs.DialogResult.Answer1)  // normal ?
				{
					//	On demande l'entité sur laquelle sera basée le masque.
					Druid entityId = Druid.Empty;
					bool isNullable = false;
					StructuredTypeClass typeClass = StructuredTypeClass.Entity;

					var subResult = this.designerApplication.DlgResourceSelector(Dialogs.ResourceSelector.Operation.Entities, this.ownerModule, Type.Entities, ref typeClass, ref entityId, ref isNullable, null, Druid.Empty);
					if (subResult != Common.Dialogs.DialogResult.Yes)
					{
						return false;
					}

					FormEngine.FormDescription form = new FormEngine.FormDescription(entityId, Druid.Empty);
					this.FormInitialize(form, ref newName);

					string xml = FormEngine.Serialization.SerializeForm(form);

					newItem = this.accessor.CreateItem();
					newItem.Name = newName;

					StructuredData data = newItem.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
					data.SetValue(Support.Res.Fields.ResourceForm.XmlSource, xml);
				}

				if (result == Epsitec.Common.Dialogs.DialogResult.Answer2)  // delta (correctif) ?
				{
					//	On demande le masque sur lequel sera basé le masque delta.
					Druid deltaBaseformId = Druid.Empty;
					bool isNullable = false;
					StructuredTypeClass typeClass = StructuredTypeClass.None;

					var subResult = this.designerApplication.DlgResourceSelector(Dialogs.ResourceSelector.Operation.Form, this.ownerModule, Type.Forms, ref typeClass, ref deltaBaseformId, ref isNullable, null, Druid.Empty);
					if (subResult != Common.Dialogs.DialogResult.Yes)
					{
						return false;
					}

					//	On demande ensuite l'entité sur laquelle sera basée le masque. Le dialogue ne montrera
					//	que les entités qui héritent de l'entité de base du masque choisi précédemment.
					Druid entityId = Druid.Empty;
					isNullable = false;
					typeClass = StructuredTypeClass.Entity;

					subResult = this.designerApplication.DlgResourceSelector(Dialogs.ResourceSelector.Operation.Entities, this.ownerModule, Type.Entities, ref typeClass, ref entityId, ref isNullable, null, deltaBaseformId);
					if (subResult != Common.Dialogs.DialogResult.Yes)
					{
						return false;
					}

					FormEngine.FormDescription form = new FormEngine.FormDescription(entityId, deltaBaseformId);
					this.FormInitialize(form, ref newName);

					string xml = FormEngine.Serialization.SerializeForm(form);

					newItem = this.accessor.CreateItem();
					newItem.Name = newName;

					StructuredData data = newItem.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
					data.SetValue(Support.Res.Fields.ResourceForm.XmlSource, xml);
				}
			}
			else
			{
				newItem = this.accessor.CreateItem();
				newItem.Name = newName;
			}

			if (duplicateContent)
			{
				//	Construit la liste des cultures à copier
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

			//	Ici, si c'est un type, on a forcément TypeCode qui a été initialisé soit
			//	explicitement avec un SetValue, soit par recopie de l'original via CopyData;
			//	c'est indispensable que TypeCode soit défini avant de faire le Add :
			this.accessor.Collection.Add(newItem);
			this.collectionView.MoveCurrentTo(newItem);

			if (generateMissingValues)
			{
				Support.ResourceAccessors.AnyTypeResourceAccessor accessor = this.accessor as Support.ResourceAccessors.AnyTypeResourceAccessor;
				accessor.CreateMissingValueItems(newItem);
			}

			this.SetLocalDirty();
			return true;
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
					ResourceAccess.SetStructuredDataValue(accessor, dstItem, dst, fieldId, value);
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
				ResourceAccess.AttemptCollectionCopy<string>(accessor, map, data, id, value, null);
				ResourceAccess.AttemptCollectionCopy<StructuredData>(accessor, map, data, id, value, ResourceAccess.CopyStructuredData);
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
						destination.Add(copyMethod(accessor, map, data, id, item));
					}
				}
			}
		}


		private void FormInitialize(FormEngine.FormDescription form, ref string newName)
		{
			//	Initialise un masque de saisie avec tous les champs de l'entité de base associée.
			//	S'il s'agit d'un masque delta, on laisse vide la liste des champs.
			if (form.IsDelta)
			{
				newName = this.GetDuplicateName(this.GetFormName(form.DeltaBaseFormId));
			}
			else
			{
				IList<StructuredData> list = this.GetEntityDruidsPath(form.EntityId);

				foreach (StructuredData dataField in list)
				{
					FieldRelation rel = (FieldRelation) dataField.GetValue(Support.Res.Fields.Field.Relation);
					if (rel == FieldRelation.None)
					{
						Druid fieldCaptionId = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);

						FormEngine.FieldDescription field = new FormEngine.FieldDescription(FormEngine.FieldDescription.FieldType.Field);
						field.SetField(fieldCaptionId);

						form.Fields.Add(field);
					}
				}

				//	Utilise comme nom du masque le nom de l'entité, éventuellement complété d'un numéro.
				newName = this.GetDuplicateName(this.GetEntityName(form.EntityId));
			}
		}

		public Druid FormSearch(Druid typeId)
		{
			//	Cherche un Form défini pour un certain type.
			foreach (CultureMap item in this.accessor.Collection)
			{
				if (this.FormSearch(item, typeId))
				{
					return item.Id;
				}
			}

			return Druid.Empty;
		}

		public bool FormSearch(CultureMap item, Druid typeId)
		{
			//	Indique si un Form est défini pour un certain type.
			FormEngine.FormDescription form = this.GetForm(item);
			return form.EntityId == typeId;
		}

		public bool FormSearch(CultureMap item, List<Druid> typeIds)
		{
			//	Indique si un Form est défini pour un certain type.
			FormEngine.FormDescription form = this.GetForm(item);
			return typeIds.Contains(form.EntityId);
		}

		public Druid FormRelationEntity(Druid entityId, string druidsPath)
		{
			//	Retourne le Druid de l'entité utilisée par un champ de type relation.
			//	Par exemple, si entityId correspond à l'entité Affaire et que druidsPath correspond
			//	aux champs Facture.AdresseFacturation, on retourne le Druid de l'entité Adresse.
			//	Pour cela, on parcourt tous les champs de l'entité Affaire à la recherche du champ
			//	Facture. Puis, dans l'entité Facture, on parcourt tous les champs à la recherche du
			//	champ AdresseFacturation. Lorsqu'il est trouvé, son TypeId est le Druid de l'entité
			//	Adresse. Ouf !
			string[] druids = druidsPath.Split('.');
			return this.FormRelationEntity(entityId, druids, 0);
		}

		private Druid FormRelationEntity(Druid entityId, string[] druids, int index)
		{
			IList<StructuredData> list = this.GetEntityDruidsPath(entityId);
			Druid druid = Druid.Parse(druids[index]);

			foreach (StructuredData dataField in list)
			{
				Druid fieldCaptionId = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);
				if (fieldCaptionId == druid)
				{
					Druid fieldTypeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);

					if (index == druids.Length-1)
					{
						return fieldTypeId;
					}
					else
					{
						return this.FormRelationEntity(fieldTypeId, druids, index+1);
					}
				}
			}

			return Druid.Empty;
		}

		public string GetFormName(Druid formId)
		{
			//	Retourne le nom d'un masque.
			CultureMap item = this.GetFormItem(formId);
			if (item == null)
			{
				return null;
			}
			else
			{
				return item.Name;
			}
		}

		private CultureMap GetFormItem(Druid formId)
		{
			//	Retourne le CultureMap d'un masque.
			Module module = this.designerApplication.SearchModule(formId);
			if (module == null)
			{
				return null;
			}

			return module.AccessForms.accessor.Collection[formId];
		}

		public string GetFieldNames(string druidsPath)
		{
			//	Retourne le nom complet d'un champ. Par exemple:
			//	druidsPath = [63063].[630A]
			//	retour = Monnaie.Désignation
			if (druidsPath == null)
			{
				return null;
			}

			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			string[] druids = druidsPath.Split('.');
			foreach (string d in druids)
			{
				Druid druid = Druid.Parse(d);

				Module module = this.designerApplication.SearchModule(druid);
				if (module == null)
				{
					continue;
				}

				CultureMap item =  module.AccessFields.accessor.Collection[druid];
				if (item == null)
				{
					item =  module.AccessEntities.accessor.Collection[druid];
					if (item == null)
					{
						continue;
					}
				}

				if (builder.Length != 0)
				{
					builder.Append(".");
				}

				builder.Append(item.Name);
			}

			return builder.ToString();
		}

		public FieldRelation GetFieldRelation(string druidsPath)
		{
			//	Retourne le type de la relation d'un champ.
			if (druidsPath == null)
			{
				return FieldRelation.None;
			}

			string[] druids = druidsPath.Split('.');
			Druid druid = Druid.Parse(druids[druids.Length-1]);

			Module module = this.designerApplication.SearchModule(druid);
			if (module == null)
			{
				return FieldRelation.None;
			}

			CultureMap item =  module.AccessFields.accessor.Collection[druid];
			if (item == null)
			{
				item =  module.AccessEntities.accessor.Collection[druid];
				if (item == null)
				{
					return FieldRelation.None;
				}
			}

			StructuredData dataField = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			if (dataField == null)
			{
				return FieldRelation.None;
			}

			return (FieldRelation) dataField.GetValue(Support.Res.Fields.Field.Relation);
		}

		public List<Druid> GetInheritedEntitiesBack(Druid entityId)
		{
			//	Retourne la liste de toutes les entités qui héritent d'une entité.
			//	Par exemple:
			//		AdressePlus hérite de Adresse
			//		Adresse n'hérite de personne
			//	Si entityId = Adresse, retourne les Druids de AdressePlus et Adresse
			System.Diagnostics.Debug.Assert(this.type == Type.Entities);
			List<Druid> list = new List<Druid>();

			foreach (CultureMap item in this.accessor.Collection)
			{
				List<Druid> sub = this.GetInheriedEntities(item.Id);
				if (sub.Contains(entityId))
				{
					foreach (Druid druid in sub)
					{
						if (!list.Contains(druid))
						{
							list.Add(druid);
						}
					}
				}
			}

			return list;
		}

		public List<Druid> GetInheriedEntities(Druid entityId)
		{
			//	Retourne la liste de toutes les entités dont hérite une entité, à commencer par elle-même.
			//	Par exemple:
			//		AdressePlus hérite de Adresse
			//		Adresse n'hérite de personne
			//	Si entityId = AdressePlus, retourne les Druids de AdressePlus et Adresse
			System.Diagnostics.Debug.Assert(this.type == Type.Entities);
			List<Druid> list = new List<Druid>();

			while (!entityId.IsEmpty)
			{
				list.Add(entityId);

				CultureMap cultureMap = this.accessor.Collection[entityId];
				if (cultureMap == null)
				{
					break;
				}

				StructuredData data = cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				if (data == null)
				{
					break;
				}

				entityId = (Druid) data.GetValue(Support.Res.Fields.ResourceStructuredType.BaseType);
			}

			return list;
		}

		public string GetEntityName(Druid entityId)
		{
			//	Retourne le nom d'une entité.
			CultureMap item = this.GetEntityItem(entityId);
			if (item == null)
			{
				return null;
			}
			else
			{
				return item.Name;
			}
		}

		public IList<StructuredData> GetEntityDruidsPath(Druid entityId)
		{
			//	Retourne la liste des chemins de Druids des champs d'une entité.
			CultureMap item = this.GetEntityItem(entityId);
			if (item == null)
			{
				return null;
			}

			StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			if (data == null)
			{
				return null;
			}

			return data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
		}

		private CultureMap GetEntityItem(Druid entityId)
		{
			//	Retourne le CultureMap d'une entité.
			Module module = this.designerApplication.SearchModule(entityId);
			if (module == null)
			{
				return null;
			}

			return module.AccessEntities.accessor.Collection[entityId];
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
			CultureMap item = this.collectionView.CurrentItem as CultureMap;
			this.accessor.Collection.Remove(item);
			this.PersistChanges();
			this.SetGlobalDirty();
		}


		public void SetFilter(string filter, Searcher.SearchingMode mode)
		{
			//	Construit l'index en fonction des ressources primaires.
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
				if ((this.collectionViewMode&Searcher.SearchingMode.Joker) != 0)
				{
					this.collectionViewRegex = RegexFactory.FromSimpleJoker(this.collectionViewFilter, RegexFactory.Options.None);
				}

				this.collectionView.Refresh();
			}
		}

		private bool CollectionViewFilter(object obj)
		{
			//	Méthode passé comme paramètre System.Predicate<object> à CollectionView.Filter.
			//	Retourne false si la ressource doit être exclue.
			CultureMap item = obj as CultureMap;
			
			if (!string.IsNullOrEmpty(this.collectionViewFilter))
			{
				if ((this.collectionViewMode&Searcher.SearchingMode.Joker) != 0)
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
			//	Retourne le nombre total de données (donc sans tenir compte du filtre).
			get
			{
				if (string.IsNullOrEmpty(this.collectionViewFilter))
				{
					return this.collectionView.Count;
				}
				else
				{
					this.collectionView.Filter = null;  // supprime le filtre
					int count = this.collectionView.Count;
					this.collectionView.Filter = this.CollectionViewFilter;  // remet le filtre
					return count;
				}
			}
		}

		public int AccessCount
		{
			//	Retourne le nombre de données accessibles, à travers le filtre.
			get
			{
				return this.collectionView.Count;
			}
		}

		public Druid AccessDruid(int index)
		{
			//	Retourne le druid d'un index donné.
			CultureMap item = this.collectionView.Items[index] as CultureMap;
			return item.Id;
		}

		public int AccessIndexOfDruid(Druid druid)
		{
			//	Retourne l'index d'un Druid.
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

		public int AccessIndex
		{
			//	Index de l'accès en cours.
			get
			{
				return this.collectionView.CurrentPosition;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, this.collectionView.Count-1);

				if (this.collectionView.CurrentPosition != value)
				{
					this.collectionView.MoveCurrentToPosition(value);
					this.collectionView.Refresh();
				}
			}
		}


		public bool IsCorrectNewName(ref string name)
		{
			//	Retourne true s'il est possible de créer cette nouvelle ressource.
			return (this.CheckNewName(null, ref name) == null);
		}

		public string CheckNewName(string prefix, ref string name)
		{
			//	Retourne l'éventuelle erreur si on tente de créer cette nouvelle ressource.
			//	Retourne null si tout est correct.
			//	Le préfixe permet de distinguer les champs des ressources de type 'Field'.
			//	Par exemple, le champ Client de l'entité Facture (donc 'Facture.Client') ne doit
			//	pas être confondu avec les champs d'une autre entité comme 'Adresse.Client'.
			if (string.IsNullOrEmpty(name))
			{
				return "Nom inexistant !";
			}

			if (!Misc.IsValidLabel(ref name, this.type == Type.Entities || this.type == Type.Forms || this.type == Type.Fields))
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
			CollectionView cv = new CollectionView(this.accessor.Collection);
			foreach (CultureMap item in cv.Items)
			{
				string fullName = name;
				if (!string.IsNullOrEmpty(prefix))
				{
					fullName = string.Concat(prefix, ".", name);
				}

				//	item.FullName inclu le préfixe s'il exite !
				string err = ResourceAccess.CheckNames(item.FullName, fullName);
				if (err != null)
				{
					return err;
				}
			}

			return null;  // ok
		}

		private static string CheckNames(string n1, string n2)
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

		private static bool CheckNamesOnce(string n1, string n2)
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

			int nextNumber = 1;
			if (numberLength > 0)
			{
				nextNumber = int.Parse(baseName.Substring(baseName.Length-numberLength))+1;
				baseName = baseName.Substring(0, baseName.Length-numberLength);
			}

			string newName = baseName;
			if (this.IsCorrectNewName(ref newName))
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
			//	Construit un texte d'après les labels et la description.
			//	Les différents labels sont séparés par des virgules.
			//	La description vient sur une deuxième ligne (si la hauteur
			//	disponible le permet), mais seulement si elle est différente
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


		public string DirectGetName(Druid druid)
		{
			//	Retourne le nom d'une ressource, sans tenir compte du filtre.
			//	La recherche s'effectue toujours dans la culture de base.
			CultureMap item = this.accessor.Collection[druid];
			
			if (item == null)
			{
				return null;
			}
			else
			{
				return item.Name;
			}
		}


		public Field GetField(int index, string cultureName, FieldType fieldType)
		{
			//	Retourne les données d'un champ.
			//	Si cultureName est nul, on accède à la culture de base.
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

			return null;
		}

		public void SetField(int index, string cultureName, FieldType fieldType, Field field)
		{
			//	Modifie les données d'un champ.
			//	Si cultureName est nul, on accède à la culture de base.
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
		}


		public CultureMapSource GetCultureMapSource(int index)
		{
			//	Retourne le type de la ressource.
			if (index != -1)
			{
				CultureMap item = this.collectionView.Items[index] as CultureMap;
				return this.GetCultureMapSource(item);
			}

			return CultureMapSource.Invalid;
		}

		public CultureMapSource GetCultureMapSource(CultureMap item)
		{
			//	Retourne le type de la ressource.
			if (item == null)
			{
				return CultureMapSource.Invalid;
			}
			else
			{
				return item.Source;
			}
		}

		public bool IsNameReadOnly(int index)
		{
			//	Indique si le nom est en lecture seule.
			if (index != -1)
			{
				CultureMap item = this.collectionView.Items[index] as CultureMap;
				return item.IsNameReadOnly;
			}

			return true;
		}

		public ModificationState GetModification(int index, string cultureName)
		{
			//	Donne l'état 'modifié'.
			if (index != -1)
			{
				CultureMap item = this.collectionView.Items[index] as CultureMap;
				return this.GetModification(item, cultureName);
			}

			return ModificationState.Normal;
		}

		public ModificationState GetModification(CultureMap item, string cultureName)
		{
			//	Donne l'état 'modifié'.
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

		public void ModificationClear(int index, string cultureName)
		{
			//	Considère une ressource comme 'à jour' dans une culture.
			CultureMap item = this.collectionView.Items[index] as CultureMap;
			StructuredData data = item.GetCultureData(cultureName);
			StructuredData primaryData = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

			int primaryValue = this.GetModificationId(primaryData);
			data.SetValue(Support.Res.Fields.ResourceBase.ModificationId, primaryValue);

			this.SetGlobalDirty();
		}

		public void ModificationSetAll(int index)
		{
			//	Considère une ressource comme 'modifiée' dans toutes les cultures.
			CultureMap item = this.collectionView.Items[index] as CultureMap;
			StructuredData primaryData = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

			int value = this.GetModificationId(primaryData);
			primaryData.SetValue(Support.Res.Fields.ResourceBase.ModificationId, value+1);

			this.SetGlobalDirty();
		}

		public bool IsModificationAll(int index)
		{
			//	Donne l'état de la commande ModificationAll.
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

		private int GetModificationId(StructuredData data)
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

			if (this.type == Type.Panels || this.type == Type.Forms)
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
				return this.cultures == null ? 0 : this.cultures.Count;
			}
		}

		public System.Globalization.CultureInfo GetCulture(string cultureName)
		{
			if (string.IsNullOrEmpty(cultureName))
			{
				return this.primaryCulture;
			}
			else
			{
				return Resources.FindCultureInfo(cultureName);
			}
		}

		public string GetCultureName(string twoLettersCode)
		{
			//	Retourne le nom standard (Français, Deutsch, English, etc.) d'une culture.
			//	null -> "(indéfini)"
			//	"00" -> "Français"
			//	"de" -> "Deutsch"
			if (twoLettersCode == Resources.DefaultTwoLetterISOLanguageName)
			{
				return Misc.CultureName(this.GetPrimaryCultureName());
			}
			else
			{
				return Misc.CultureName(twoLettersCode);
			}
		}

		public string GetPrimaryCultureName()
		{
			//	Retourne le nom de la culture de base (twoLettersCode).
			return this.primaryCulture.Name;
		}

		public List<string> GetSecondaryCultureNames()
		{
			//	Retourne la liste des cultures secondaires, triés par ordre alphabétique (twoLettersCode).
			List<string> list = new List<string>();
			
			foreach (string name in this.cultures)
			{
				System.Globalization.CultureInfo culture = Resources.FindCultureInfo(name);
				if (culture != null)
				{
					list.Add(culture.Name);
				}
			}

			System.Diagnostics.Debug.Assert(!list.Contains(this.GetPrimaryCultureName()));
			list.Sort();
			
			return list;
		}

		public void CreateCulture(string cultureName)
		{
			//	Crée un nouveau bundle pour une culture donnée.
			System.Diagnostics.Debug.Assert(cultureName.Length == 2);

			if (!this.cultures.Contains(cultureName))
			{
				System.Globalization.CultureInfo culture = Resources.FindCultureInfo(cultureName);
				
				if (!this.BundleExists (ResourceLevel.Localized, culture))
				{
					this.CreateEmptyBundle (ResourceLevel.Localized, culture);
				}

				this.cultures.Add(cultureName);
				this.cultures.Sort();
				this.SetGlobalDirty();
			}
		}

		public void DeleteCulture(string cultureName)
		{
			//	Supprime une culture.
			System.Diagnostics.Debug.Assert(cultureName.Length == 2);
			System.Globalization.CultureInfo culture = this.GetCulture(cultureName);
			if (this.cultures.Contains(cultureName))
			{
				//	ATTENTION: Il faudra rajouter ici un garde-fou pour éviter de
				//	détruire un bundle partagé entre plusieurs accesseurs (par ex.
				//	suppression dans Commandes --> Caption sera affecté)

				foreach (CultureMap item in this.accessor.Collection)
				{
					item.ClearCultureData(cultureName);
				}
				this.DeleteBundle(culture, this.GetBundleName());
				this.cultures.Remove(cultureName);
				this.SetGlobalDirty();
			}
		}

		#region Méthodes de manipulation bas niveau de ResourceBundle

		private void DeleteBundle(System.Globalization.CultureInfo culture, string bundleName)
		{
			ResourceBundle bundle = this.resourceManager.GetBundle(bundleName, ResourceLevel.Localized, culture);
			this.batchSaver.DelaySave(this.resourceManager, bundle, ResourceSetMode.Remove);
		}

		private bool BundleExists(ResourceLevel level, System.Globalization.CultureInfo culture)
		{
			string bundleName = this.GetBundleName ();
			string bundleType = this.GetBundleType ();
			ResourceBundle bundle = this.resourceManager.GetBundle (bundleName, level, culture);

			if (bundle == null)
			{
				return false;
			}
			else
			{
				System.Diagnostics.Debug.Assert (bundle.Type == bundleType);

				return true;
			}
		}

		private ResourceBundle CreateEmptyBundle(ResourceLevel level, System.Globalization.CultureInfo culture)
		{
			string prefix = this.resourceManager.ActivePrefix;
			string bundleName = this.GetBundleName();
			string bundleType = this.GetBundleType();
			ResourceBundle bundle = ResourceBundle.Create(this.resourceManager, prefix, bundleName, level, culture);

			bundle.DefineType(bundleType);
			bundle.DefineModule(this.resourceManager.DefaultModuleInfo.FullId);

			this.resourceManager.SetBundle(bundle, ResourceSetMode.InMemory);
			this.batchSaver.DelaySave(this.resourceManager, bundle, ResourceSetMode.CreateOnly);

			return bundle;
		}

		private void CreateEmptyBundle()
		{
			if (this.type != Type.Panels && this.type != Type.Forms)
			{
				System.Globalization.CultureInfo culture = Resources.FindCultureInfo(Misc.Cultures[0]);
				
				if (!this.BundleExists (ResourceLevel.Default, culture))
				{
					this.CreateEmptyBundle(ResourceLevel.Default, culture);
				}
			}
		}

		private void LoadBundles()
		{
			System.Globalization.CultureInfo culture = Resources.FindCultureInfo(Misc.Cultures[0]);

			if (this.type == Type.Panels || this.type == Type.Forms)
			{
				this.primaryCulture = culture;
			}
			else
			{
				ResourceBundle bundle = this.resourceManager.GetBundle(this.GetBundleName(), ResourceLevel.Default);
				this.primaryCulture = bundle.Culture;
			}

			if (this.type == Type.Forms)
			{
				this.FormsMerge();
			}

			this.cultures = new List<string>(this.accessor.GetAvailableCultures());
		}

		private void SaveBundles(ResourceBundleBatchSaver saver)
		{
			this.accessor.Save(saver.DelaySave);
		}

		#endregion


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


		public string TypesCreateMissingValueItems(string moduleName)
		{
			//	Passe en revue toutes les énumérations à la recherche des énumérations système à compléter.
			//	Ceci peut se produire lorsqu'une énumération C# reflètée par une ressource a été complétée.
			//	Retourne un éventuel message donnant les noms des énumérations complétées.
			List<CultureMap> missingList = new List<CultureMap>();

			foreach (CultureMap item in this.accessor.Collection)
			{
				StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				object value = data.GetValue(Support.Res.Fields.ResourceEnumType.Values);
				if (!UndefinedValue.IsUndefinedValue(value))  // est-ce une énumération ?
				{
					IList<StructuredData> list = value as IList<StructuredData>;
					if (list != null)
					{
						bool missing = false;
						foreach (StructuredData valueData in list)
						{
							Druid druid = (Druid) valueData.GetValue(Support.Res.Fields.EnumValue.CaptionId);
							if (druid.IsEmpty)  // si on ne trouve pas le Druid, c'est que la valeur n'existe pas
							{
								missing = true;
								break;
							}
						}

						if (missing)  // manque une ou plusieurs valeurs ?
						{
							Support.ResourceAccessors.AnyTypeResourceAccessor accessor = this.accessor as Support.ResourceAccessors.AnyTypeResourceAccessor;
							accessor.CreateMissingValueItems(item);

							missingList.Add(item);
						}
					}
				}
			}

			if (missingList.Count == 0)  // toutes les énumérations sont complètes ?
			{
				return null;
			}
			else  // une ou plusieurs énumérations incomplètes ?
			{
				this.PersistChanges();

				//	Construit un joli message clair.
				System.Text.StringBuilder builder = new System.Text.StringBuilder();
				builder.Append("<br/>");

				foreach (CultureMap item in missingList)
				{
					builder.Append(string.Format("<list type=\"fix\" width=\"1.5\"/>{0}/Types: <a href=\"{2}\">{1}</a><br/>", moduleName, item.FullName, item.Id.ToString()));
				}

				return builder.ToString();
			}
		}


		#region Panel
		public void SetPanel(Druid druid, UI.Panel panel)
		{
			//	Sérialise le UI.Panel dans les ressources.
			if (druid.IsValid)
			{
				string xml = UI.Panel.SerializePanel(panel);
				string size = panel.PreferredSize.ToString();

				CultureMap item = this.accessor.Collection[druid];
				System.Diagnostics.Debug.Assert(item != null);
				StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				
				string oldXml = data.GetValue(Support.Res.Fields.ResourcePanel.XmlSource) as string;
				string oldSize = data.GetValue(Support.Res.Fields.ResourcePanel.DefaultSize) as string;

				if (xml != oldXml)
				{
					data.SetValue(Support.Res.Fields.ResourcePanel.XmlSource, xml);
				}
				if (size != oldSize)
				{
					data.SetValue(Support.Res.Fields.ResourcePanel.DefaultSize, size);
				}
			}
		}

		public UI.Panel GetPanel(Druid druid)
		{
			//	Retourne le UI.Panel associé à une ressource.
			//	Si nécessaire, il est créé la première fois.
			return this.GetPanel(this.accessor.Collection[druid]);
		}

		public UI.Panel GetPanel(int index)
		{
			//?CultureMap item = this.accessor.Collection[index];  // "RunPanel" affiche parfois la mauvaise resource avec cela !
			CultureMap item = this.collectionView.Items[index] as CultureMap;
			return this.GetPanel(item);
		}
		
		private UI.Panel GetPanel(CultureMap item)
		{
			if (item == null)
			{
				return null;
			}

			StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			string xml = data.GetValue(Support.Res.Fields.ResourcePanel.XmlSource) as string;

			if (string.IsNullOrEmpty(xml))
			{
				return this.CreateEmptyPanel();
			}
			else
			{
				return UI.Panel.DeserializePanel(xml, null, this.resourceManager);
			}
		}

		public UI.Panel CreateEmptyPanel()
		{
			//	Crée un nouveau panneau vide.
			UI.Panel panel = UI.Panel.CreateEmptyPanel(null, this.resourceManager);
			this.InitializePanel(panel);
			return panel;
		}

		private void InitializePanel(UI.Panel panel)
		{
			panel.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Stacked;
			panel.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
			panel.PreferredSize = new Size(200, 200);
			panel.Anchor = AnchorStyles.BottomLeft;
			panel.Padding = new Margins(20, 20, 20, 20);
			panel.DrawDesignerFrame = true;
		}
		#endregion


		#region Form
		public bool SetForm(Druid druid, FormEngine.FormDescription form)
		{
			//	Sérialise le masque de saisie dans les ressources.
			//	Retourne false si le Druid ne correspond plus à une ressource existante.
			if (!druid.IsValid)
			{
				this.ClearLocalDirty();
				return false;
			}

			CultureMap item = this.accessor.Collection[druid];
			if (item == null)
			{
				this.ClearLocalDirty();
				return false;
			}

			StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

			string xml = FormEngine.Serialization.SerializeForm(form);
			string oldXml = data.GetValue(Support.Res.Fields.ResourceForm.XmlSource) as string;

			if (xml != oldXml || this.accessor.ForceModuleMerge)
			{
				data.SetValue(Support.Res.Fields.ResourceForm.XmlSource, xml);

				if (this.accessor.BasedOnPatchModule && item.Source != CultureMapSource.PatchModule)
				{
					if (this.accessor.ForceModuleMerge)
					{
						this.FormMerge(item);
					}
					else
					{
						this.ClearFormMerge(item);
					}
				}
			}

			return true;
		}

		public void GetForm(Druid druid, FormEngine.FormDescription inputForm, out FormEngine.FormDescription workingForm, out List<FormEngine.FieldDescription> baseFields, out List<FormEngine.FieldDescription> finalFields, out Druid entityId)
		{
			//	Désérialise le masque de saisie dans les ressources.
			//	Si inputForm != null, il s'agit du Form que l'on désire retrouver après un undo/redo.
			CultureMap item = this.accessor.Collection[druid];

			if (item == null)
			{
				workingForm = null;
				baseFields = null;
				finalFields = null;
				entityId = Druid.Empty;
			}
			else
			{
				StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				FormEngine.Engine engine = new FormEngine.Engine(this.ownerModule.FormResourceProvider);
				string baseXml, deltaXml;
				FormEngine.FormDescription baseForm, deltaForm;

				if (this.accessor.BasedOnPatchModule)
				{
					switch (item.Source)
					{
						case CultureMapSource.ReferenceModule:
							baseXml = data.GetValue(Support.Res.Fields.ResourceForm.XmlSourceAux) as string;

							if (string.IsNullOrEmpty(baseXml))
							{
								baseForm = null;
							}
							else
							{
								baseForm = FormEngine.Serialization.DeserializeForm(baseXml);
							}

							if (inputForm == null)
							{
								deltaForm = new FormEngine.FormDescription(baseForm);
								deltaForm.Fields.Clear();
							}
							else
							{
								deltaForm = inputForm;
							}
							break;

						case CultureMapSource.PatchModule:
							if (inputForm == null)
							{
								baseXml = data.GetValue(Support.Res.Fields.ResourceForm.XmlSource) as string;

								if (string.IsNullOrEmpty(baseXml))
								{
									baseForm = null;
								}
								else
								{
									baseForm = FormEngine.Serialization.DeserializeForm(baseXml);
								}
							}
							else
							{
								baseForm = inputForm;
							}

							deltaForm = null;
							break;

						case CultureMapSource.DynamicMerge:
							this.FormMerge(item);  // utile si un autre module a changé

							baseXml = data.GetValue(Support.Res.Fields.ResourceForm.XmlSourceAux) as string;

							if (string.IsNullOrEmpty(baseXml))
							{
								baseForm = null;
							}
							else
							{
								baseForm = FormEngine.Serialization.DeserializeForm(baseXml);
							}

							if (inputForm == null)
							{
								deltaXml = data.GetValue(Support.Res.Fields.ResourceForm.XmlSource) as string;

								if (deltaXml == null)
								{
									deltaForm = null;
								}
								else
								{
									deltaForm = FormEngine.Serialization.DeserializeForm(deltaXml);
								}
							}
							else
							{
								deltaForm = inputForm;
							}
							break;

						default:
							throw new System.InvalidOperationException();
					}
				}
				else
				{
					if (inputForm == null)
					{
						baseXml = data.GetValue(Support.Res.Fields.ResourceForm.XmlSource) as string;

						if (string.IsNullOrEmpty(baseXml))
						{
							baseForm = null;
						}
						else
						{
							baseForm = FormEngine.Serialization.DeserializeForm(baseXml);
						}
					}
					else
					{
						baseForm = inputForm;
					}

					deltaForm = null;
				}

				engine.Arrange.Build(baseForm, deltaForm, out baseFields, out finalFields, out entityId);

				if (deltaForm == null)
				{
					workingForm = baseForm;
				}
				else
				{
					workingForm = deltaForm;
				}

				if (this.accessor.BasedOnPatchModule && item.Source != CultureMapSource.PatchModule)
				{
					workingForm.IsForceDelta = true;
				}
			}
		}

		public FormEngine.FormDescription GetForm(Druid druid)
		{
			//	Retourne le masque de saisie associé à une ressource.
			return this.GetForm(this.accessor.Collection[druid]);
		}

		private FormEngine.FormDescription GetForm(CultureMap item)
		{
			//	Désérialise un masque.
			if (item != null)
			{
				StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				string xml;

				if (this.accessor.BasedOnPatchModule)
				{
					switch (item.Source)
					{
						case CultureMapSource.DynamicMerge:
							this.FormMerge(item);  // utile si un autre module a changé
							xml = data.GetValue(Support.Res.Fields.ResourceForm.XmlSourceMerge) as string;
							break;

						case CultureMapSource.PatchModule:
							xml = data.GetValue(Support.Res.Fields.ResourceForm.XmlSource) as string;
							break;

						case CultureMapSource.ReferenceModule:
							xml = data.GetValue(Support.Res.Fields.ResourceForm.XmlSourceAux) as string;
							break;

						default:
							throw new System.InvalidOperationException();
					}
				}
				else
				{
					xml = data.GetValue(Support.Res.Fields.ResourceForm.XmlSource) as string;
				}

				if (!string.IsNullOrEmpty(xml))
				{
					return FormEngine.Serialization.DeserializeForm(xml);
				}
			}

			return null;
		}

		internal void FormsMerge()
		{
			//	Génère la ressource XmlSourceMerge de tous les masques si nécessaire.
			if (this.accessor.BasedOnPatchModule)
			{
				foreach (CultureMap item in this.accessor.Collection)
				{
					if (item.Source == CultureMapSource.DynamicMerge)
					{
						this.FormMerge(item);
					}
					else if (item.Source == CultureMapSource.ReferenceModule && this.accessor.ForceModuleMerge)
					{
						this.FormMerge(item);
					}
				}
			}
		}

		private void ClearFormMerge(CultureMap item)
		{
			System.Diagnostics.Debug.Assert(this.accessor.BasedOnPatchModule);
			System.Diagnostics.Debug.Assert(item.Source == CultureMapSource.DynamicMerge);

			StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

			data.SetValue(Support.Res.Fields.ResourceForm.XmlSourceMerge, null);
		}

		internal void FormMerge(CultureMap item)
		{
			//	Génère la ressource XmlSourceMerge d'un masque si nécessaire.
			if (this.accessor.BasedOnPatchModule && item.Source != CultureMapSource.PatchModule)
			{
				StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

				string xmlBase = data.GetValue(Support.Res.Fields.ResourceForm.XmlSourceAux) as string;
				string xmlDelta = data.GetValue(Support.Res.Fields.ResourceForm.XmlSource) as string;

				FormEngine.FormDescription formBase = string.IsNullOrEmpty(xmlBase) ? null : FormEngine.Serialization.DeserializeForm(xmlBase);
				FormEngine.FormDescription formDelta = string.IsNullOrEmpty(xmlDelta) ? null : FormEngine.Serialization.DeserializeForm(xmlDelta);

				FormEngine.Engine engine = new FormEngine.Engine(this.ownerModule.FormResourceProvider);
				List<FormEngine.FieldDescription> fields = engine.Arrange.Merge(formBase == null ? null : formBase.Fields, formDelta == null ? null : formDelta.Fields);

				FormEngine.FormDescription copy = new FormEngine.FormDescription(formBase ?? formDelta);
				copy.Fields.Clear();
				foreach (FormEngine.FieldDescription field in fields)
				{
					copy.Fields.Add(field);
				}

				string xmlFinal = FormEngine.Serialization.SerializeForm(copy);
				data.SetValue(Support.Res.Fields.ResourceForm.XmlSourceMerge, xmlFinal);
			}
		}
		#endregion


		private string GetBundleName()
		{
			//	Retourne le nom du bundle (pour Common.Support & Cie) en fonction du type.
			switch (this.type)
			{
				case Type.Strings:
					return Resources.StringsBundleName;

				case Type.Captions:
				case Type.Commands:
				case Type.Entities:
				case Type.Fields:
				case Type.Types:
				case Type.Values:
					return Resources.CaptionsBundleName;

				case Type.Panels:
				case Type.Forms:
					return null;

				default:
					throw new System.NotImplementedException();
			}
		}

		private string GetBundleType()
		{
			//	Retourne le type du bundle (pour Common.Support & Cie) en fonction du type.
			switch (this.type)
			{
				case Type.Strings:
					return Resources.StringTypeName;

				case Type.Captions:
				case Type.Commands:
				case Type.Entities:
				case Type.Fields:
				case Type.Types:
				case Type.Values:
					return Resources.CaptionTypeName;

				case Type.Panels:
					return Resources.PanelTypeName;
				
				case Type.Forms:
					return Resources.FormTypeName;
				
				default:
					throw new System.NotImplementedException();
			}
		}


		private static string LastName(string name)
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

			private Shortcut			shortcut;
			private string			name;
			private string			culture;
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

			private Type type;
			private string									stringValue;
			private ICollection<string>						stringCollection;
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


		private static string								filterPrefix = "Epsitec.Common.";

		private Module										ownerModule;
		private Type										type;
		private ResourceManager								resourceManager;
		private ResourceBundleBatchSaver					batchSaver;
		private ResourceModuleId							moduleInfo;
		private DesignerApplication							designerApplication;
		private bool										isGlobalDirty = false;
		private bool										isLocalDirty = false;
		private bool										isJustLoaded = false;

		private IResourceAccessor							accessor;
		private CollectionView								collectionView;
		private Searcher.SearchingMode						collectionViewMode;
		private string										collectionViewFilter;
		private Regex										collectionViewRegex;
		private Types.SortDescription[]						collectionViewInitialSorts;

		private List<string>								cultures;
		private System.Globalization.CultureInfo			primaryCulture;
		private TypeCode									lastTypeCodeCreatated = TypeCode.String;
		private System.Type									lastTypeCodeSystem = null;

		private DataLifetimeExpectancy						lastLifetime;
		private StructuredTypeFlags							lastFlags;
	}
}
