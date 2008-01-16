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
			Normal,			//	d�fini normalement
			Empty,			//	vide ou ind�fini (fond rouge)
			Modified,		//	modifi� (fond jaune)
		}


		public ResourceAccess(Type type, Module module, ResourceModuleId moduleInfo)
		{
			//	Constructeur unique pour acc�der aux ressources d'un type donn�.
			//	Par la suite, l'instance cr��e acc�dera toujours aux ressources de ce type,
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

		public void Load()
		{
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
				this.accessor.NotifyItemChanged (item, null, null);
			}
		}

		public void AddShortcuts(List<ShortcutItem> list)
		{
			//	Ajoute tous les raccourcis d�finis dans la liste.
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
			//	V�rifie les raccourcis, en construisant un message d'avertissement
			//	pour tous les raccourcis utilis�s plus d'une fois.
			string culture = null;
			List<ShortcutItem> uses = new List<ShortcutItem>();
			bool foo = false;

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
				foo = true;

				uses.Add(list[i]);  // ajoute � la liste des raccourcis trait�s
			}

			if (foo)
			{
				//	Pour compenser un bug dans TextLayout. Si le texte se termine par "<br/>", le calcul
				//	de la hauteur est incorrect (la derni�re ligne n'est pas compt�e). En revanche, si
				//	le texte se termine par "<br/> ", tout semble fonctionner !
				//	TODO: corriger TextLayout et supprimer cette magouille !
				builder.Append(" ");
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
			CultureMap newItem = null;
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
				bool isNullable = false;
				StructuredTypeClass typeClass = StructuredTypeClass.Entity;

				Common.Dialogs.DialogResult result = this.designerApplication.DlgResourceSelector(Dialogs.ResourceSelector.Operation.InheritEntities, this.designerApplication.CurrentModule, Type.Entities, ref typeClass, ref druid, ref isNullable, null, Druid.Empty);
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
			else if (this.type == Type.Forms && !duplicateContent)
			{
				string header = Res.Strings.Forms.Question.Create.Base;

				List<string> questions = new List<string>();
				questions.Add(ConfirmationButton.FormatContent(Res.Strings.Forms.Question.Create.Quick.Normal, Res.Strings.Forms.Question.Create.Long.Normal));
				questions.Add(ConfirmationButton.FormatContent(Res.Strings.Forms.Question.Create.Quick.Patch, Res.Strings.Forms.Question.Create.Long.Patch));
				
				Common.Dialogs.DialogResult result = this.designerApplication.DialogConfirmation(header, questions, true);
				if (result == Epsitec.Common.Dialogs.DialogResult.Cancel)
				{
					return;
				}

				if (result == Epsitec.Common.Dialogs.DialogResult.Answer1)  // normal ?
				{
					Druid entityId = Druid.Empty;
					bool isNullable = false;
					StructuredTypeClass typeClass = StructuredTypeClass.Entity;

					Common.Dialogs.DialogResult subResult = this.designerApplication.DlgResourceSelector(Dialogs.ResourceSelector.Operation.Entities, this.designerApplication.CurrentModule, Type.Entities, ref typeClass, ref entityId, ref isNullable, null, Druid.Empty);
					if (subResult != Common.Dialogs.DialogResult.Yes)
					{
						return;
					}

					FormEngine.FormDescription form = new FormEngine.FormDescription(entityId, Druid.Empty);
					this.FormInitialize(form, ref newName);

					string xml = FormEngine.Serialization.SerializeForm(form);

					newItem = this.accessor.CreateItem();
					newItem.Name = newName;

					StructuredData data = newItem.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
					data.SetValue(Support.Res.Fields.ResourceForm.XmlSource, xml);
				}

				if (result == Epsitec.Common.Dialogs.DialogResult.Answer2)  // patch ?
				{
					Druid formIdToPatch = Druid.Empty;
					bool isNullable = false;
					StructuredTypeClass typeClass = StructuredTypeClass.None;

					Common.Dialogs.DialogResult subResult = this.designerApplication.DlgResourceSelector(Dialogs.ResourceSelector.Operation.Form, this.designerApplication.CurrentModule, Type.Forms, ref typeClass, ref formIdToPatch, ref isNullable, null, Druid.Empty);
					if (subResult != Common.Dialogs.DialogResult.Yes)
					{
						return;
					}

					// TODO: emp�cher de choisir une entit� pas en accord avec le Form choisi pr�c�demment !
					Druid entityId = Druid.Empty;
					isNullable = false;
					typeClass = StructuredTypeClass.Entity;

					subResult = this.designerApplication.DlgResourceSelector(Dialogs.ResourceSelector.Operation.Entities, this.designerApplication.CurrentModule, Type.Entities, ref typeClass, ref entityId, ref isNullable, null, Druid.Empty);
					if (subResult != Common.Dialogs.DialogResult.Yes)
					{
						return;
					}

					FormEngine.FormDescription form = new FormEngine.FormDescription(entityId, formIdToPatch);
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
					ResourceAccess.SetStructuredDataValue(accessor, dstItem, dst, fieldId, value);
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
						destination.Add(copyMethod(accessor, map, data, id, item));
					}
				}
			}
		}


		private void FormInitialize(FormEngine.FormDescription form, ref string newName)
		{
			//	Initialise un masque de saisie avec tous les champs de l'entit� de base associ�e.
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

			//	Utilise comme nom du masque le nom de l'entit�, �ventuellement compl�t� d'un num�ro.
			newName = this.GetDuplicateName(this.GetEntityName(form.EntityId));
		}

		public Druid FormSearch(Druid typeId)
		{
			//	Cherche un Form d�fini pour un certain type.
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
			//	Indique si un Form est d�fini pour un certain type.
			FormEngine.FormDescription form = this.GetForm(item);
			return form.EntityId == typeId;
		}

		public bool FormSearch(CultureMap item, List<Druid> typeIds)
		{
			//	Indique si un Form est d�fini pour un certain type.
			FormEngine.FormDescription form = this.GetForm(item);
			return typeIds.Contains(form.EntityId);
		}

		public Druid FormRelationEntity(Druid entityId, string druidsPath)
		{
			//	Retourne le Druid de l'entit� utilis�e par un champ de type relation.
			//	Par exemple, si entityId correspond � l'entit� Affaire et que druidsPath correspond
			//	aux champs Facture.AdresseFacturation, on retourne le Druid de l'entit� Adresse.
			//	Pour cela, on parcourt tous les champs de l'entit� Affaire � la recherche du champ
			//	Facture. Puis, dans l'entit� Facture, on parcourt tous les champs � la recherche du
			//	champ AdresseFacturation. Lorsqu'il est trouv�, son TypeId est le Druid de l'entit�
			//	Adresse. Ouf !
			string[] druids = druidsPath.Split('.');
			return this.FormRelationEntity(entityId, druids, 0);
		}

		protected Druid FormRelationEntity(Druid entityId, string[] druids, int index)
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

		protected CultureMap GetFormItem(Druid formId)
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
			//	retour = Monnaie.D�signation
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

		public string GetEntityName(Druid entityId)
		{
			//	Retourne le nom d'une entit�.
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
			//	Retourne la liste des chemins de Druids des champs d'une entit�.
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

#if false
		public List<string> GetEntityDruidsPath(Druid entityId)
		{
			//	Retourne la liste des chemins de Druids des champs d'une entit�.
			//	Les relations sont explor�es r�cursivement.
			List<string> list = new List<string>();

			CultureMap item = this.GetEntityItem(entityId);
			if (item == null)
			{
				return list;
			}

			StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			if (data == null)
			{
				return list;
			}

			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
			Stack<Druid> alreadyVisited = new Stack<Druid>();
			alreadyVisited.Push(entityId);
			foreach (StructuredData dataField in dataFields)
			{
				this.GetEntityDruidsPath(list, alreadyVisited, dataField, null);
			}

			return list;
		}

		protected void GetEntityDruidsPath(List<string> list, Stack<Druid> alreadyVisited, StructuredData dataField, string prefix)
		{
			FieldRelation rel = (FieldRelation) dataField.GetValue(Support.Res.Fields.Field.Relation);
			if (rel == FieldRelation.None)
			{
				FormEngine.FieldDescription field = new FormEngine.FieldDescription(FormEngine.FieldDescription.FieldType.Field);

				Druid fieldCaptionId = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);

				string path = (prefix == null) ? fieldCaptionId.ToString() : string.Concat(prefix, fieldCaptionId.ToString());
				if (!list.Contains(path))
				{
					list.Add(path);
				}
			}
			else if (rel == FieldRelation.Reference)
			{
				Druid typeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);
				Druid fieldCaptionId = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);

				if (!alreadyVisited.Contains(typeId))
				{
					alreadyVisited.Push(typeId);

					Module typeModule = this.designerApplication.SearchModule(typeId);
					if (typeModule != null)
					{
						CultureMap typeCultureMap = typeModule.AccessEntities.Accessor.Collection[typeId];
						if (typeCultureMap != null)
						{
							StructuredData typeData = typeCultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
							IList<StructuredData> typeDataFields = typeData.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
							foreach (StructuredData typeDataField in typeDataFields)
							{
								this.GetEntityDruidsPath(list, alreadyVisited, typeDataField, string.Concat(prefix, fieldCaptionId.ToString(), "."));
							}
						}
					}

					alreadyVisited.Pop();
				}
			}
		}
#endif

		protected CultureMap GetEntityItem(Druid entityId)
		{
			//	Retourne le CultureMap d'une entit�.
			Module module = this.designerApplication.SearchModule(entityId);
			if (module == null)
			{
				return null;
			}

			return module.AccessEntities.accessor.Collection[entityId];
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
			CultureMap item = this.collectionView.CurrentItem as CultureMap;
			this.accessor.Collection.Remove(item);
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

		protected bool CollectionViewFilter(object obj)
		{
			//	M�thode pass� comme param�tre System.Predicate<object> � CollectionView.Filter.
			//	Retourne false si la ressource doit �tre exclue.
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
			//	Retourne le nombre total de donn�es (donc sans tenir compte du filtre).
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
			//	Retourne le nombre de donn�es accessibles, � travers le filtre.
			get
			{
				return this.collectionView.Count;
			}
		}

		public Druid AccessDruid(int index)
		{
			//	Retourne le druid d'un index donn�.
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
			//	Index de l'acc�s en cours.
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
			CollectionView cv = new CollectionView(this.accessor.Collection);
			foreach (CultureMap item in cv.Items)
			{
				string err = ResourceAccess.CheckNames(item.Name, name);
				if (err != null)
				{
					return err;
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
			//	Retourne les donn�es d'un champ.
			//	Si cultureName est nul, on acc�de � la culture de base.
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
			//	Modifie les donn�es d'un champ.
			//	Si cultureName est nul, on acc�de � la culture de base.
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
			//	Donne l'�tat 'modifi�'.
			if (index != -1)
			{
				CultureMap item = this.collectionView.Items[index] as CultureMap;
				return this.GetModification(item, cultureName);
			}

			return ModificationState.Normal;
		}

		public ModificationState GetModification(CultureMap item, string cultureName)
		{
			//	Donne l'�tat 'modifi�'.
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
			//	Consid�re une ressource comme '� jour' dans une culture.
			CultureMap item = this.collectionView.Items[index] as CultureMap;
			StructuredData data = item.GetCultureData(cultureName);
			StructuredData primaryData = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

			int primaryValue = this.GetModificationId(primaryData);
			data.SetValue(Support.Res.Fields.ResourceBase.ModificationId, primaryValue);

			this.SetGlobalDirty();
		}

		public void ModificationSetAll(int index)
		{
			//	Consid�re une ressource comme 'modifi�e' dans toutes les cultures.
			CultureMap item = this.collectionView.Items[index] as CultureMap;
			StructuredData primaryData = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

			int value = this.GetModificationId(primaryData);
			primaryData.SetValue(Support.Res.Fields.ResourceBase.ModificationId, value+1);

			this.SetGlobalDirty();
		}

		public bool IsModificationAll(int index)
		{
			//	Donne l'�tat de la commande ModificationAll.
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
			//	Retourne le nom standard (Fran�ais, Deutsch, English, etc.) d'une culture.
			//	null -> "(ind�fini)"
			//	"00" -> "Fran�ais"
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
			//	Retourne la liste des cultures secondaires, tri�s par ordre alphab�tique (twoLettersCode).
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
			//	Cr�e un nouveau bundle pour une culture donn�e.
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
				//	ATTENTION: Il faudra rajouter ici un garde-fou pour �viter de
				//	d�truire un bundle partag� entre plusieurs accesseurs (par ex.
				//	suppression dans Commandes --> Caption sera affect�)

				foreach (CultureMap item in this.accessor.Collection)
				{
					item.ClearCultureData(cultureName);
				}
				this.DeleteBundle(culture, this.GetBundleName());
				this.cultures.Remove(cultureName);
				this.SetGlobalDirty();
			}
		}

		#region M�thodes de manipulation bas niveau de ResourceBundle

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
			
			this.cultures = new List<string>(this.accessor.GetAvailableCultures());
		}

		private void SaveBundles(ResourceBundleBatchSaver saver)
		{
			this.accessor.Save(saver.DelaySave);
		}

		#endregion


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


		#region Panel
		public void SetPanel(Druid druid, UI.Panel panel)
		{
			//	S�rialise le UI.Panel dans les ressources.
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
			//	Retourne le UI.Panel associ� � une ressource.
			//	Si n�cessaire, il est cr�� la premi�re fois.
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
			//	Cr�e un nouveau panneau vide.
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
		public void SetForm(Druid druid, FormEngine.FormDescription form)
		{
			//	S�rialise le masque de saisie dans les ressources.
			if (druid.IsValid)
			{
				string xml = FormEngine.Serialization.SerializeForm(form);

				CultureMap item = this.accessor.Collection[druid];
				System.Diagnostics.Debug.Assert(item != null);
				StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

				string oldXml = data.GetValue(Support.Res.Fields.ResourceForm.XmlSource) as string;
				
				if (xml != oldXml)
				{
					data.SetValue(Support.Res.Fields.ResourceForm.XmlSource, xml);
				}
			}
		}

		public FormEngine.FormDescription GetForm(Druid druid)
		{
			//	Retourne le masque de saisie associ� � une ressource.
			return this.GetForm(this.accessor.Collection[druid]);
		}

		public FormEngine.FormDescription GetForm(int index)
		{
			//?CultureMap item = this.accessor.Collection[index];  // "RunPanel" affiche parfois la mauvaise resource avec cela !
			CultureMap item = this.collectionView.Items[index] as CultureMap;
			return this.GetForm(item);
		}
		
		private FormEngine.FormDescription GetForm(CultureMap item)
		{
			if (item != null)
			{
				StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				string xml = data.GetValue(Support.Res.Fields.ResourceForm.XmlSource) as string;

				if (!string.IsNullOrEmpty(xml))
				{
					return FormEngine.Serialization.DeserializeForm(xml, this.resourceManager);
				}
			}

			return null;
		}
		#endregion


		protected string GetBundleName()
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

		protected string GetBundleType()
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

			protected Type type;
			protected string									stringValue;
			protected ICollection<string>						stringCollection;
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
		protected ResourceBundleBatchSaver					batchSaver;
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

		protected List<string>								cultures;
		protected System.Globalization.CultureInfo			primaryCulture;
		protected TypeCode									lastTypeCodeCreatated = TypeCode.String;
		protected System.Type								lastTypeCodeSystem = null;
	}
}
