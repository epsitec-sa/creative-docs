//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Support.ResourceAccessors.StructuredTypeResourceAccessor))]

namespace Epsitec.Common.Support.ResourceAccessors
{
	/// <summary>
	/// The <c>StructuredTypeResourceAccessor</c> is used to access entity
	/// resources, stored in the <c>Captions</c> resource bundle and which
	/// have a field name prefixed with <c>"Typ.StructuredType."</c>.
	/// </summary>
	public class StructuredTypeResourceAccessor : CaptionResourceAccessor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredTypeResourceAccessor"/> class.
		/// </summary>
		public StructuredTypeResourceAccessor()
		{
		}

		/// <summary>
		/// Gets the field accessor associated with the structured type accessor.
		/// </summary>
		/// <value>The <see cref="FieldResourceAccessor"/>.</value>
		public IResourceAccessor FieldAccessor
		{
			get
			{
				return this.fieldAccessor;
			}
		}

		/// <summary>
		/// Gets the caption prefix for this accessor.
		/// Note: several resource types are stored as captions; the prefix of
		/// the field name is used to differentiate them.
		/// </summary>
		/// <value>The caption <c>"Type.StructuredType."</c> prefix.</value>
		protected override string Prefix
		{
			get
			{
				return "Typ.StructuredType.";
			}
		}

		/// <summary>
		/// Checks if the data stored in the field matches this accessor. This
		/// can be used to filter out specific fields.
		/// </summary>
		/// <param name="field">The field to check.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns>
		/// 	<c>true</c> if data should be loaded from the field; otherwise, <c>false</c>.
		/// </returns>
		protected override bool FilterField(ResourceBundle.Field field, string fieldName)
		{
			return base.FilterField (field, fieldName);
		}

		/// <summary>
		/// Loads resources from the specified resource manager. The resource
		/// manager will be used for all upcoming accesses.
		/// </summary>
		/// <param name="manager">The resource manager.</param>
		public override void Load(ResourceManager manager)
		{
			//	We maintain a list of structured type resource accessors associated
			//	with the resource manager pool. This is required since we must mark
			//	all entities as dirty if any entity is modified in the pool...

			AccessorsCollection accessors = StructuredTypeResourceAccessor.GetAccessors (manager.Pool);

			if (accessors == null)
			{
				accessors = new AccessorsCollection ();
				StructuredTypeResourceAccessor.SetAccessors (manager.Pool, accessors);
			}
			else
			{
				accessors.Remove (this);
			}

			accessors.Add (this);

			//	Temporarily disable the item refreshing while we are loading the
			//	module data.

			this.postponeRefreshWhileLoading = true;
			base.Load (manager);
			this.postponeRefreshWhileLoading = false;

			if (this.fieldAccessor == null)
			{
				this.fieldAccessor = new FieldResourceAccessor ();
			}

			this.fieldAccessor.Load (manager);
		}

		/// <summary>
		/// Gets the data broker associated with the specified field. Usually,
		/// this is only meaningful if the field defines a collection of
		/// <see cref="StructuredData"/> items.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="fieldId">The id for the field in the specified container.</param>
		/// <returns>The data broker or <c>null</c>.</returns>
		public override IDataBroker GetDataBroker(StructuredData container, string fieldId)
		{
			//	When the user requests a broker for the "Fields" value, we provide
			//	the appropriate FieldBroker :

			if (fieldId == Res.Fields.ResourceStructuredType.Fields.ToString ())
			{
				return new FieldBroker ();
			}
			if (fieldId == Res.Fields.ResourceStructuredType.InterfaceIds.ToString ())
			{
				return new InterfaceIdBroker ();
			}

			return base.GetDataBroker (container, fieldId);
		}

		/// <summary>
		/// Creates a field item for the specified structured type item.
		/// </summary>
		/// <param name="item">The structured type item.</param>
		/// <returns>The field item which can then be added in the
		/// <see cref="FieldResourceAccessor"/> collection.</returns>
		public CultureMap CreateFieldItem(CultureMap item)
		{
			CultureMap fieldItem = this.fieldAccessor.CreateFieldItem (item.Name);

			fieldItem.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);

			return fieldItem;
		}

		/// <summary>
		/// Refreshes the fields for the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void RefreshFields(CultureMap item)
		{
			this.RefreshItem (item);
		}

		/// <summary>
		/// Persists the changes to the underlying data store.
		/// </summary>
		/// <returns>
		/// The number of items which have been persisted.
		/// </returns>
		public override int PersistChanges()
		{
			int n = this.FieldAccessor.PersistChanges ();
			int m = base.PersistChanges ();

			if (m > 0)
			{
				this.MarkAllItemsAsDirty ();
			}
			
			return n+m;
		}

		/// <summary>
		/// Reverts the changes applied to the accessor.
		/// </summary>
		/// <returns>
		/// The number of items which have been reverted.
		/// </returns>
		public override int RevertChanges()
		{
			int n = 0;

			n += base.RevertChanges ();
			n += this.FieldAccessor.RevertChanges ();

			return n;
		}

		/// <summary>
		/// Resets the specified field to its original value. This is the
		/// internal implementation which can be overridden.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="container">The data record.</param>
		/// <param name="fieldId">The field id.</param>
		protected override void ResetToOriginal(CultureMap item, StructuredData container, Druid fieldId)
		{
			Listener listener;

			if (fieldId == Res.Fields.ResourceStructuredType.Fields)
			{
				listener = Listener.FindListener<FieldListener> (container, fieldId);
			}
			else if (fieldId == Res.Fields.ResourceStructuredType.InterfaceIds)
			{
				listener = Listener.FindListener<InterfaceListener> (container, fieldId);
			}
			else
			{
				listener = null;
			}

			if (listener != null)
			{
				System.Diagnostics.Debug.Assert (listener != null);
				System.Diagnostics.Debug.Assert (listener.Item == item);
				System.Diagnostics.Debug.Assert (listener.Data == container);

				listener.ResetToOriginalValue ();
			}
			else
			{
				base.ResetToOriginal (item, container, fieldId);
			}
		}
		
		/// <summary>
		/// Gets the structured type which describes the caption data.
		/// </summary>
		/// <returns>
		/// The <see cref="StructuredType"/> instance.
		/// </returns>
		protected override IStructuredType GetStructuredType()
		{
			return Res.Types.ResourceStructuredType;
		}

		/// <summary>
		/// Creates a caption based on the definitions stored in a data record.
		/// </summary>
		/// <param name="sourceBundle">The source bundle.</param>
		/// <param name="data">The data record.</param>
		/// <param name="name">The name of the caption.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <returns>A <see cref="Caption"/> instance.</returns>
		protected override Caption CreateCaptionFromData(ResourceBundle sourceBundle, Types.StructuredData data, string name, string twoLetterISOLanguageName)
		{
			Caption caption = base.CreateCaptionFromData (sourceBundle, data, name, twoLetterISOLanguageName);
			
			if (twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName)
			{
				StructuredType type = this.CreateTypeFromData (data, caption);

				System.Diagnostics.Debug.Assert (AbstractType.GetComplexType (caption) == type);
				AbstractType.SetComplexType (caption, type);
			}
			else
			{
				System.Diagnostics.Debug.Assert (AbstractType.GetComplexType (caption) == null);
				AbstractType.SetComplexType (caption, null);
			}
			
			return caption;
		}

		/// <summary>
		/// Fills the data record from a given caption.
		/// </summary>
		/// <param name="item">The item associated with the data record.</param>
		/// <param name="data">The data record.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="mode">The creation mode for the data record.</param>
		protected override void FillDataFromCaption(CultureMap item, Types.StructuredData data, Caption caption, DataCreationMode mode)
		{
			base.FillDataFromCaption (item, data, caption, mode);

			StructuredType type = AbstractType.GetComplexType (caption) as StructuredType;
			this.FillDataFromType (item, data, type, mode);
		}

		/// <summary>
		/// Determines whether the specified data record describes an empty
		/// caption.
		/// </summary>
		/// <param name="data">The data record.</param>
		/// <returns>
		/// 	<c>true</c> if this is an empty caption; otherwise, <c>false</c>.
		/// </returns>
		protected override bool IsEmptyCaption(StructuredData data)
		{
			if (base.IsEmptyCaption (data))
			{
				object                baseTypeValue   = data.GetValue (Res.Fields.ResourceStructuredType.BaseType);
				object                classValue      = data.GetValue (Res.Fields.ResourceStructuredType.Class);
				IList<StructuredData> fields          = data.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
				IList<StructuredData> interfaceIds    = data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;
				string                designerLayouts = data.GetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts) as string;

				if ((UndefinedValue.IsUndefinedValue (baseTypeValue)) &&
					/*(UndefinedValue.IsUndefinedValue (classValue)) &&*/
					((fields == null) || (fields.Count == 0)) &&
					((interfaceIds == null) || (interfaceIds.Count == 0)) &&
					(string.IsNullOrEmpty (designerLayouts)))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Computes the difference between a raw data record and a reference
		/// data record and fills the patch data record with the resulting
		/// delta.
		/// </summary>
		/// <param name="rawData">The raw data record.</param>
		/// <param name="refData">The reference data record.</param>
		/// <param name="patchData">The patch data, which will be filled with the delta.</param>
		protected override void ComputeDataDelta(StructuredData rawData, StructuredData refData, StructuredData patchData)
		{
			base.ComputeDataDelta (rawData, refData, patchData);
			
#if true
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceStructuredType.BaseType);
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceStructuredType.Class);
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceStructuredType.Fields);
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceStructuredType.InterfaceIds);
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceStructuredType.SerializedDesignerLayouts);

			//	TODO: ...process intelligently field and interface lists to keep just the delta...
#else
			object                refBaseTypeValue   = refData.GetValue (Res.Fields.ResourceStructuredType.BaseType);
			StructuredTypeClass   refClass           = StructuredTypeResourceAccessor.ToStructuredTypeClass (refData.GetValue (Res.Fields.ResourceStructuredType.Class));
			IList<StructuredData> refFields          = refData.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
			IList<StructuredData> refInterfaceIds    = refData.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;
			string                refDesignerLayouts = refData.GetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts) as string;

			object                rawBaseTypeValue   = rawData.GetValue (Res.Fields.ResourceStructuredType.BaseType);
			StructuredTypeClass   rawClass           = StructuredTypeResourceAccessor.ToStructuredTypeClass (rawData.GetValue (Res.Fields.ResourceStructuredType.Class));
			IList<StructuredData> rawFields          = rawData.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
			IList<StructuredData> rawInterfaceIds    = rawData.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;
			string                rawDesignerLayouts = rawData.GetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts) as string;

			if ((!UndefinedValue.IsUndefinedValue (rawBaseTypeValue)) &&
				((UndefinedValue.IsUndefinedValue (refBaseTypeValue)) || ((Druid) refBaseTypeValue != (Druid) rawBaseTypeValue)))
			{
				patchData.SetValue (Res.Fields.ResourceStructuredType.BaseType, rawBaseTypeValue);
			}
			
			System.Diagnostics.Debug.Assert (refClass == rawClass);
			
			//	The structured type class must be defined, or else we won't be able
			//	to generate the correct StructuredType instance for the caption
			//	serialization.
			
			patchData.SetValue (Res.Fields.ResourceStructuredType.Class, refClass);

			if ((!string.IsNullOrEmpty (rawDesignerLayouts)) &&
				(refDesignerLayouts != rawDesignerLayouts))
			{
				patchData.SetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts, rawDesignerLayouts);
			}
			
			if ((rawFields != null) &&
				(rawFields.Count > 0) &&
				(!Types.Collection.CompareEqual (rawFields, refFields)))
			{
				List<StructuredData> temp = new List<StructuredData> ();
				
				foreach (StructuredData field in rawFields)
				{
					CultureMapSource fieldSource = (CultureMapSource) field.GetValue (Res.Fields.Field.CultureMapSource);
					
					if ((fieldSource == CultureMapSource.DynamicMerge) ||
						(fieldSource == CultureMapSource.PatchModule))
					{
						temp.Add (field);
					}
				}

				patchData.SetValue (Res.Fields.ResourceStructuredType.Fields, temp);
			}
			
			if ((rawInterfaceIds != null) &&
				(rawInterfaceIds.Count > 0) &&
				(!Types.Collection.CompareEqual (rawInterfaceIds, refInterfaceIds)))
			{
				List<StructuredData> temp = new List<StructuredData> ();

				foreach (StructuredData interfaceId in rawInterfaceIds)
				{
					CultureMapSource interfaceIdSource = (CultureMapSource) interfaceId.GetValue (Res.Fields.InterfaceId.CultureMapSource);

					if ((interfaceIdSource == CultureMapSource.DynamicMerge) ||
						(interfaceIdSource == CultureMapSource.PatchModule))
					{
						temp.Add (interfaceId);
					}
				}

				patchData.SetValue (Res.Fields.ResourceStructuredType.InterfaceIds, temp);
			}
#endif
		}

		/// <summary>
		/// Handles changes to the item collection.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="Epsitec.Common.Types.CollectionChangedEventArgs"/> instance containing the event data.</param>
		protected override void HandleItemsCollectionChanged(object sender, CollectionChangedEventArgs e)
		{
			base.HandleItemsCollectionChanged (sender, e);

			//	Always handle the changes, even if the base class suspended the
			//	notifications, as we need to keep our event handlers up to date:

			switch (e.Action)
			{
				case CollectionChangedAction.Add:
					foreach (CultureMap item in e.NewItems)
					{
						this.HandleCultureMapAdded (item);
					}
					break;

				case CollectionChangedAction.Remove:
					foreach (CultureMap item in e.OldItems)
					{
						this.HandleCultureMapRemoved (item);
					}
					break;

				case CollectionChangedAction.Replace:
					foreach (CultureMap item in e.OldItems)
					{
						this.HandleCultureMapRemoved (item);
					}
					foreach (CultureMap item in e.NewItems)
					{
						this.HandleCultureMapAdded (item);
					}
					break;
			}
		}

		/// <summary>
		/// Refreshes the item, by regenerating the inherited fields.
		/// </summary>
		/// <param name="item">The item.</param>
		protected override void RefreshItem(CultureMap item)
		{
			if (this.postponeRefreshWhileLoading)
			{
				//	Don't refresh right now, since to create the list of inherited
				//	fields; all modules should be loaded first.

				item.IsRefreshNeeded = true;
			}
			else
			{
				StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
				Druid    baseTypeId = StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.ResourceStructuredType.BaseType));
				
				this.UpdateInheritedFields (data, baseTypeId);
				
				base.RefreshItem (item);
			}
		}


		/// <summary>
		/// Marks all structured type resource items as dirty.
		/// </summary>
		private void MarkAllItemsAsDirty()
		{
			AccessorsCollection accessors = StructuredTypeResourceAccessor.GetAccessors (this.ResourceManager.Pool);

			//	Mark all items describing entities in the same pool as dirty,
			//	since the changes which have just been persisted may induce
			//	modifications to inherited fields or interfaces.

			foreach (StructuredTypeResourceAccessor accessor in accessors.Collection)
			{
				foreach (CultureMap item in accessor.Collection)
				{
					item.IsRefreshNeeded = true;
				}
			}
		}

		/// <summary>
		/// Builds a structured type from a definition stored in a data record,
		/// using the given caption as its vehicle.
		/// </summary>
		/// <param name="data">The data record.</param>
		/// <param name="caption">The caption.</param>
		/// <returns>The structured type.</returns>
		private StructuredType CreateTypeFromData(StructuredData data, Caption caption)
		{
			if (UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStructuredType.Class)))
			{
				return null;
			}

			StructuredTypeClass typeClass = StructuredTypeResourceAccessor.ToStructuredTypeClass (data.GetValue (Res.Fields.ResourceStructuredType.Class));
			Druid               baseType  = StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.ResourceStructuredType.BaseType));
			string              layouts   = data.GetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts) as string;

			StructuredType type = new StructuredType (typeClass, baseType);
			type.DefineCaption (caption);
			type.SerializedDesignerLayouts = layouts;
			type.FreezeInheritance ();

			IList<StructuredData> interfaceIds = data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;

			if (interfaceIds != null)
			{
				foreach (StructuredData interfaceId in interfaceIds)
				{
					type.InterfaceIds.Add (StructuredTypeResourceAccessor.ToDruid (interfaceId.GetValue (Res.Fields.InterfaceId.CaptionId)));
				}
			}
			
			IList<StructuredData> fieldsData = data.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
			
			int rank = 0;

			if (fieldsData != null)
			{
				foreach (StructuredData fieldData in fieldsData)
				{
					Druid fieldType = StructuredTypeResourceAccessor.ToDruid (fieldData.GetValue (Res.Fields.Field.TypeId));
					Druid fieldCaption = StructuredTypeResourceAccessor.ToDruid (fieldData.GetValue (Res.Fields.Field.CaptionId));
					FieldRelation relation = (FieldRelation) fieldData.GetValue (Res.Fields.Field.Relation);
					FieldMembership membership = (FieldMembership) fieldData.GetValue (Res.Fields.Field.Membership);
					FieldSource source = (FieldSource) fieldData.GetValue (Res.Fields.Field.Source);
					FieldOptions options = (FieldOptions) fieldData.GetValue (Res.Fields.Field.Options);
					string expression = fieldData.GetValue (Res.Fields.Field.Expression) as string;
					Druid fieldDefiningType = StructuredTypeResourceAccessor.ToDruid (fieldData.GetValue (Res.Fields.Field.DefiningTypeId));
					bool? interfaceDefinition = StructuredTypeResourceAccessor.ToBoolean (fieldData.GetValue (Res.Fields.Field.IsInterfaceDefinition));

					//	A field must be stored in the type only if it is defined locally
					//	and if it does not belong to a locally defined interface; or
					//	if it redefines the expression for a local interface field.

					if (((membership == FieldMembership.Local) && (fieldDefiningType.IsEmpty)) ||
						((membership == FieldMembership.Local) && (interfaceDefinition.HasValue) && (interfaceDefinition.Value == false)))
					{
						StructuredTypeField field = new StructuredTypeField (null, null, fieldCaption, rank++, relation, membership, source, options, expression);
						field.DefineTypeId (fieldType);
						type.Fields.Add (field);
					}
				}
			}

			return type;
		}

		private void FillDataFromType(CultureMap item, StructuredData data, StructuredType type, DataCreationMode mode)
		{
			if (mode != DataCreationMode.Temporary)
			{
				System.Diagnostics.Debug.Assert ((type == null) || type.CaptionId.IsValid);
			}

			bool recordFields = false;
			bool recordInterfaceIds = false;

			ObservableList<StructuredData> fields = data.GetValue (Res.Fields.ResourceStructuredType.Fields) as ObservableList<StructuredData>;
			ObservableList<StructuredData> interfaceIds = data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as ObservableList<StructuredData>;

			if (fields == null)
			{
				fields = new ObservableList<StructuredData> ();
				recordFields = true;
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("Re-using existing fields list");
			}
			if (interfaceIds == null)
			{
				interfaceIds = new ObservableList<StructuredData> ();
				recordInterfaceIds = true;
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("Re-using existing interface list");
			}

			if (type != null)
			{
				type.FreezeInheritance ();

				foreach (string fieldId in type.GetFieldIds ())
				{
					StructuredTypeField field = type.Fields[fieldId];

					if (!Types.Collection.Contains (fields,
						delegate (StructuredData find)
						{
							Druid findId = StructuredTypeResourceAccessor.ToDruid (find.GetValue (Res.Fields.Field.CaptionId));
							return findId == field.CaptionId;
						}))
					{
						StructuredData x = new StructuredData (Res.Types.Field);

						StructuredTypeResourceAccessor.FillDataFromField (item, x, field);
						fields.Add (x);

						if (mode == DataCreationMode.Public)
						{
							item.NotifyDataAdded (x);
						}
					}
				}
			}

			if (recordFields)
			{
				data.SetValue (Res.Fields.ResourceStructuredType.Fields, fields);
				data.LockValue (Res.Fields.ResourceStructuredType.Fields);

				if (mode == DataCreationMode.Public)
				{
					FieldListener listener = new FieldListener (this, item, data);

					fields.CollectionChanging += listener.HandleCollectionChanging;
					fields.CollectionChanged  += listener.HandleCollectionChanged;
				}
			}
			

			if (type != null)
			{
				foreach (Druid interfaceId in type.InterfaceIds)
				{
					if (!Types.Collection.Contains (interfaceIds,
						delegate (StructuredData find)
						{
							Druid findId = StructuredTypeResourceAccessor.ToDruid (find.GetValue (Res.Fields.InterfaceId.CaptionId));
							return findId == interfaceId;
						}))
					{
						StructuredData x = new StructuredData (Res.Types.InterfaceId);

						x.SetValue (Res.Fields.InterfaceId.CaptionId, interfaceId);
						x.SetValue (Res.Fields.InterfaceId.CultureMapSource, item.Source);

						interfaceIds.Add (x);

						if (mode == DataCreationMode.Public)
						{
							item.NotifyDataAdded (x);
						}
					}
				}
			}

			if (recordInterfaceIds)
			{
				data.SetValue (Res.Fields.ResourceStructuredType.InterfaceIds, interfaceIds);
				data.LockValue (Res.Fields.ResourceStructuredType.InterfaceIds);

				if (mode == DataCreationMode.Public)
				{
					InterfaceListener listener = new InterfaceListener (this, item, data);

					interfaceIds.CollectionChanging += listener.HandleCollectionChanging;
					interfaceIds.CollectionChanged  += listener.HandleCollectionChanged;
				}
			}

			if (type == null)
			{
				data.SetValue (Res.Fields.ResourceStructuredType.BaseType, Druid.Empty);
				data.SetValue (Res.Fields.ResourceStructuredType.Class, StructuredTypeClass.None);
				data.SetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts, "");
			}
			else
			{
				if ((type.BaseTypeId.IsValid) ||
					(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStructuredType.BaseType))))
				{
					data.SetValue (Res.Fields.ResourceStructuredType.BaseType, type.BaseTypeId);
				}
				
				data.SetValue (Res.Fields.ResourceStructuredType.Class, type.Class);

				if ((!string.IsNullOrEmpty (type.SerializedDesignerLayouts)) ||
					(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts))))
				{
					data.SetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts, type.SerializedDesignerLayouts);
				}
			}
		}

		private static void FillDataFromField(CultureMap item, StructuredData data, StructuredTypeField field)
		{
			System.Diagnostics.Debug.Assert (field.DefiningTypeId.IsEmpty);

			data.SetValue (Res.Fields.Field.TypeId, field.Type == null ? Druid.Empty : field.Type.CaptionId);
			data.SetValue (Res.Fields.Field.CaptionId, field.CaptionId);
			data.SetValue (Res.Fields.Field.Relation, field.Relation);
			data.SetValue (Res.Fields.Field.Membership, field.Membership);
			data.SetValue (Res.Fields.Field.CultureMapSource, item.Source);
			data.SetValue (Res.Fields.Field.Source, field.Source);
			data.SetValue (Res.Fields.Field.Options, field.Options);
			data.SetValue (Res.Fields.Field.Expression, field.Expression ?? "");
			data.SetValue (Res.Fields.Field.DefiningTypeId, Druid.Empty);
			data.SetValue (Res.Fields.Field.DeepDefiningTypeId, Druid.Empty);
			data.LockValue (Res.Fields.Field.DefiningTypeId);
			data.LockValue (Res.Fields.Field.DeepDefiningTypeId);
		}

		private void HandleCultureMapAdded(CultureMap item)
		{
			item.PropertyChanged += this.HandleItemPropertyChanged;
			this.RefreshItem (item);
		}

		private void HandleCultureMapRemoved(CultureMap item)
		{
			item.PropertyChanged -= this.HandleItemPropertyChanged;
		}

		private void HandleItemPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Name")
			{
				string oldName = e.OldValue as string;
				string newName = e.NewValue as string;

				this.ChangeFieldPrefix (oldName, newName);
			}
			if (e.PropertyName == Res.Fields.ResourceStructuredType.BaseType.ToString ())
			{
				StructuredData data = sender as StructuredData;
				Druid   newBaseType = (Druid) e.NewValue;
				
				this.UpdateInheritedFields (data, newBaseType);
			}
		}


		#region Private FieldUpdater Class
		
		private sealed class FieldUpdater
		{
			public FieldUpdater(StructuredTypeResourceAccessor host)
			{
				this.host         = host;
				this.ids          = new List<Druid> ();
				this.interfaceIds = new List<Druid> ();
				this.fields       = new List<StructuredData> ();
			}

			public IList<StructuredData> Fields
			{
				get
				{
					return this.fields;
				}
			}

			public IList<Druid> InterfaceIds
			{
				get
				{
					return this.interfaceIds;
				}
			}

			/// <summary>
			/// Includes the interface id in the scan.
			/// </summary>
			/// <param name="interfaceId">The interface id.</param>
			public void IncludeInterfaceId(Druid interfaceId)
			{
				if (this.interfaceIds.Contains (interfaceId))
				{
					//	Nothing to do, interface is already known
				}
				else
				{
					this.interfaceIds.Add (interfaceId);
				}
			}
			
			/// <summary>
			/// Includes all fields defined by the specified type, setting their
			/// membership accordingly.
			/// </summary>
			/// <param name="typeId">The type id.</param>
			/// <param name="membership">The top level field membership.</param>
			public void IncludeType(Druid typeId, FieldMembership membership)
			{
				this.IncludeType (typeId, typeId, membership, 0);
			}

			/// <summary>
			/// Includes all fields defined by the specified type, setting their
			/// membership accordingly.
			/// </summary>
			/// <param name="typeId">The type id.</param>
			/// <param name="definingTypeId">The type id of the defining type.</param>
			/// <param name="membership">The top level field membership.</param>
			/// <param name="depth">The recursion depth.</param>
			private void IncludeType(Druid typeId, Druid definingTypeId, FieldMembership membership, int depth)
			{
				StructuredData data = this.host.FindStructuredData (typeId);

				if (data == null)
				{
					return;
				}

				Druid                 baseDruid    = StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.ResourceStructuredType.BaseType));
				IList<StructuredData> interfaceIds = data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;
				IList<StructuredData> fields       = data.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

				if (baseDruid.IsValid)
				{
					this.IncludeType (baseDruid, definingTypeId, FieldMembership.Inherited, depth+1);
				}

				if (interfaceIds != null)
				{
					foreach (StructuredData interfaceId in interfaceIds)
					{
						Druid id = StructuredTypeResourceAccessor.ToDruid (interfaceId.GetValue (Res.Fields.InterfaceId.CaptionId));
						this.IncludeType (id, definingTypeId, membership, depth+1);
						this.IncludeInterfaceId (id);
					}
				}

				if (fields != null)
				{
					foreach (StructuredData field in fields)
					{
						Druid fieldId = StructuredTypeResourceAccessor.ToDruid (field.GetValue (Res.Fields.Field.CaptionId));

						if (this.ids.Contains (fieldId))
						{
							continue;
						}

						StructuredData copy = new StructuredData (Res.Types.Field);

						copy.SetValue (Res.Fields.Field.TypeId,             field.GetValue (Res.Fields.Field.TypeId));
						copy.SetValue (Res.Fields.Field.CaptionId,          field.GetValue (Res.Fields.Field.CaptionId));
						copy.SetValue (Res.Fields.Field.Relation,           field.GetValue (Res.Fields.Field.Relation));
						copy.SetValue (Res.Fields.Field.Membership,         membership);
						copy.SetValue (Res.Fields.Field.CultureMapSource,   field.GetValue (Res.Fields.Field.CultureMapSource));
						copy.SetValue (Res.Fields.Field.Source,             field.GetValue (Res.Fields.Field.Source));
						copy.SetValue (Res.Fields.Field.Options,            field.GetValue (Res.Fields.Field.Options));
						copy.SetValue (Res.Fields.Field.Expression,         field.GetValue (Res.Fields.Field.Expression));
						copy.SetValue (Res.Fields.Field.DefiningTypeId,     definingTypeId);
						copy.SetValue (Res.Fields.Field.DeepDefiningTypeId, typeId);
						
						copy.LockValue (Res.Fields.Field.DefiningTypeId);
						copy.LockValue (Res.Fields.Field.DeepDefiningTypeId);

						this.ids.Add (fieldId);
						this.fields.Add (copy);
					}
				}
			}

			private StructuredTypeResourceAccessor host;
			private List<Druid> ids;
			private List<Druid> interfaceIds;
			private List<StructuredData> fields;
		}

		#endregion

		/// <summary>
		/// Finds the structured type resource accessor for a given module.
		/// </summary>
		/// <param name="id">The full resource id.</param>
		/// <returns>The accessor or <c>null</c>.</returns>
		private StructuredTypeResourceAccessor FindAccessor(Druid id)
		{
			return this.FindAccessor (id.Module);
		}

		/// <summary>
		/// Finds the structured type resource accessor for a given module.
		/// </summary>
		/// <param name="moduleId">The module id.</param>
		/// <returns>The accessor or <c>null</c>.</returns>
		private StructuredTypeResourceAccessor FindAccessor(int moduleId)
		{
			AccessorsCollection            accessors = StructuredTypeResourceAccessor.GetAccessors (this.ResourceManager.Pool);
			StructuredTypeResourceAccessor candidate = null;

			if (this.ResourceManager.DefaultModuleId == moduleId)
			{
				candidate = this;
			}
			
			foreach (StructuredTypeResourceAccessor accessor in accessors.Collection)
			{
				if (accessor.ResourceManager.DefaultModuleId == moduleId)
				{
					if (accessor.ResourceManager.BasedOnPatchModule)
					{
						return accessor;
					}

					if (candidate == null)
					{
						candidate = accessor;
					}
				}
			}

			return candidate;
		}

		/// <summary>
		/// Finds the data record for a given resource id. This will use one of the
		/// available accessors to fetch the data.
		/// </summary>
		/// <param name="id">The resource id.</param>
		/// <returns>The <see cref="StructuredData"/> record or <c>null</c>.</returns>
		private StructuredData FindStructuredData(Druid id)
		{
			StructuredTypeResourceAccessor accessor = this.FindAccessor (id);

			if (accessor == null)
			{
				return null;
			}

			CultureMap map = accessor.Collection[id];

			if (map == null)
			{
				return null;
			}

			return map.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
		}

		/// <summary>
		/// Updates the inherited fields found in a structured type. This method
		/// can be used when the <c>BaseType</c> property changes or when a new
		/// <c>CultureMap</c> object is added, or when an interface is added or
		/// removed.
		/// </summary>
		/// <param name="data">The data describing the structured type.</param>
		/// <param name="newBaseType">The Druid of the base type.</param>
		private void UpdateInheritedFields(StructuredData data, Druid baseTypeId)
		{
			ObservableList<StructuredData> fields = data.GetValue (Res.Fields.ResourceStructuredType.Fields) as ObservableList<StructuredData>;
			IList<StructuredData> interfaceIds = data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;

			//	We temporarily disable notifications, in order to avoid generating
			//	circular update events when just refreshing fields that are in fact
			//	read only for the user :

			using (fields.DisableNotifications ())
			{
				if ((baseTypeId.IsValid) ||
					(interfaceIds.Count > 0))
				{
					FieldUpdater updater = new FieldUpdater (this);

					updater.IncludeType (baseTypeId, FieldMembership.Inherited);

					foreach (StructuredData interfaceData in interfaceIds)
					{
						Druid interfaceId = StructuredTypeResourceAccessor.ToDruid (interfaceData.GetValue (Res.Fields.InterfaceId.CaptionId));
						updater.IncludeType (interfaceId, FieldMembership.Local);
						updater.IncludeInterfaceId (interfaceId);
					}

					StructuredTypeResourceAccessor.RemoveInheritedFields (fields, updater.InterfaceIds);

					int i = 0;

					foreach (StructuredData field in updater.Fields)
					{
						System.Diagnostics.Debug.Assert (((FieldMembership) field.GetValue (Res.Fields.Field.Membership) == FieldMembership.Inherited)
							/**/					  || (StructuredTypeResourceAccessor.ToDruid (field.GetValue (Res.Fields.Field.CaptionId)).IsValid));


						int pos = StructuredTypeResourceAccessor.IndexOfCompatibleField (fields, field);

						if (pos < 0)
						{
							if ((FieldMembership) field.GetValue (Res.Fields.Field.Membership) == FieldMembership.Local)
							{
								//	The field is defined in an interface and its definition
								//	has not been tampered with (yet) locally :

								field.SetValue (Res.Fields.Field.IsInterfaceDefinition, true);
							}

							fields.Insert (i++, field);
						}
						else
						{
							System.Diagnostics.Debug.Assert ((FieldMembership) field.GetValue (Res.Fields.Field.Membership) == FieldMembership.Local);
							System.Diagnostics.Debug.Assert (updater.InterfaceIds.Contains (StructuredTypeResourceAccessor.ToDruid (field.GetValue (Res.Fields.Field.DefiningTypeId))));

							//	The field is defined both locally and in the interface; keep
							//	only the expression from the local definition and otherwise,
							//	use the interface definition of the field :

							field.SetValue (Res.Fields.Field.Source, fields[pos].GetValue (Res.Fields.Field.Source));
							field.SetValue (Res.Fields.Field.Expression, fields[pos].GetValue (Res.Fields.Field.Expression));
							field.SetValue (Res.Fields.Field.IsInterfaceDefinition, false);

							System.Diagnostics.Debug.Assert (i <= pos);

							fields.RemoveAt (pos);
							fields.Insert (i++, field);
						}
					}
				}
				else
				{
					StructuredTypeResourceAccessor.RemoveInheritedFields (fields);
				}
			}
		}

		private static int IndexOfCompatibleField(IList<StructuredData> fields, StructuredData field)
		{
			Druid id = StructuredTypeResourceAccessor.ToDruid (field.GetValue (Res.Fields.Field.CaptionId));
			
			for (int i = 0; i < fields.Count; i++)
			{
				if (StructuredTypeResourceAccessor.ToDruid (fields[i].GetValue (Res.Fields.Field.CaptionId)) == id)
				{
					return i;
				}
			}
			
			return -1;
		}


		/// <summary>
		/// Removes the inherited and included fields.
		/// </summary>
		/// <param name="fields">The field collection.</param>
		private static void RemoveInheritedFields(IList<StructuredData> fields)
		{
			StructuredTypeResourceAccessor.RemoveInheritedFields (fields, EmptyList<Druid>.Instance);
		}

		/// <summary>
		/// Removes the inherited and included fields.
		/// </summary>
		/// <param name="fields">The field collection.</param>
		/// <param name="interfaceIds">The live interface ids.</param>
		private static void RemoveInheritedFields(IList<StructuredData> fields, IList<Druid> interfaceIds)
		{
			for (int i = 0; i < fields.Count; i++)
			{
				StructuredData field = fields[i];
				FieldMembership membership = (FieldMembership) field.GetValue (Res.Fields.Field.Membership);
				Druid definingTypeId = StructuredTypeResourceAccessor.ToDruid (field.GetValue (Res.Fields.Field.DefiningTypeId));
				bool? interfaceDefinition = StructuredTypeResourceAccessor.ToBoolean (field.GetValue (Res.Fields.Field.IsInterfaceDefinition));

				//	If the field is inherited from a base entity or imported through
				//	an interface, then we remove it from the collection :
				
				if (membership == FieldMembership.Inherited)
				{
					fields.RemoveAt (i--);
				}
				else if (definingTypeId.IsValid)
				{
					if ((interfaceDefinition.HasValue) &&
						(interfaceDefinition.Value == false))
					{
						if (interfaceIds.Contains (definingTypeId))
						{
							continue;
						}
					}

					fields.RemoveAt (i--);
				}
			}
		}

		/// <summary>
		/// Changes the field prefix for all associated field definition
		/// resources. This is called when a structured type gets renamed.
		/// </summary>
		/// <param name="oldName">The old name.</param>
		/// <param name="newName">The new name.</param>
		private void ChangeFieldPrefix(string oldName, string newName)
		{
			ResourceBundle bundle = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default);

			string oldPrefix = string.Concat ("Fld.", oldName, ".");
			string newPrefix = string.Concat ("Fld.", newName, ".");

			foreach (ResourceBundle.Field field in bundle.Fields)
			{
				if (field.Name.StartsWith (oldPrefix))
				{
					field.SetName (newPrefix + field.Name.Substring (oldPrefix.Length));
				}
			}

			foreach (PrefixedCultureMap item in this.fieldAccessor.Collection)
			{
				if (item.Prefix == oldName)
				{
					item.Prefix = newName;
				}
			}
		}

		protected class FieldListener : Listener
		{
			public FieldListener(StructuredTypeResourceAccessor accessor, CultureMap item, StructuredData data)
				: base (accessor, item, data)
			{
			}

			public override void HandleCollectionChanging(object sender)
			{
				if (this.UsesOriginalValue (Res.Fields.ResourceStructuredType.Fields))
				{
					this.Data.UnlockValue (Res.Fields.ResourceStructuredType.Fields);
					this.Data.CopyOriginalToCurrentValue (Res.Fields.ResourceStructuredType.Fields);
					this.Data.LockValue (Res.Fields.ResourceStructuredType.Fields);

					ObservableList<StructuredData> fields = this.Data.GetValue (Res.Fields.ResourceStructuredType.Fields) as ObservableList<StructuredData>;

					this.originalFields = new List<StructuredData> ();

					foreach (StructuredData data in fields)
					{
						this.originalFields.Add (data.GetShallowCopy ());
					}
				}
			}

			public override void ResetToOriginalValue()
			{
				if (this.originalFields != null)
				{
					this.Data.UnlockValue (Res.Fields.ResourceStructuredType.Fields);
					this.Data.ResetToOriginalValue (Res.Fields.ResourceStructuredType.Fields);
					this.Data.LockValue (Res.Fields.ResourceStructuredType.Fields);

					ObservableList<StructuredData> fields = this.Data.GetValue (Res.Fields.ResourceStructuredType.Fields) as ObservableList<StructuredData>;

					using (fields.DisableNotifications ())
					{
						int index = fields.Count - 1;
						
						while (index >= 0)
						{
							StructuredData data = fields[index];
							fields.RemoveAt (index--);
							this.Item.NotifyDataRemoved (data);
						}

						System.Diagnostics.Debug.Assert (fields.Count == 0);

						foreach (StructuredData data in this.originalFields)
						{
							StructuredData copy = data.GetShallowCopy ();
							copy.PromoteToOriginal ();
							fields.Add (copy);
							this.Item.NotifyDataAdded (copy);
						}
					}
				}
			}

			List<StructuredData> originalFields;
		}

		#region InterfaceListener Class

		protected class InterfaceListener : Listener
		{
			public InterfaceListener(StructuredTypeResourceAccessor accessor, CultureMap item, StructuredData data)
				: base (accessor, item, data)
			{
			}

			public override void HandleCollectionChanging(object sender)
			{
				if (this.UsesOriginalValue (Res.Fields.ResourceStructuredType.InterfaceIds))
				{
					this.Data.UnlockValue (Res.Fields.ResourceStructuredType.InterfaceIds);
					this.Data.CopyOriginalToCurrentValue (Res.Fields.ResourceStructuredType.InterfaceIds);
					this.Data.LockValue (Res.Fields.ResourceStructuredType.InterfaceIds);

					ObservableList<StructuredData> interfaceIds = this.Data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as ObservableList<StructuredData>;

					this.originalInterfaceIds = new List<StructuredData> ();

					foreach (StructuredData data in interfaceIds)
					{
						this.originalInterfaceIds.Add (data.GetShallowCopy ());
					}
				}
			}

			public override void HandleCollectionChanged(object sender, CollectionChangedEventArgs e)
			{
				StructuredTypeResourceAccessor accessor = this.Accessor as StructuredTypeResourceAccessor;

				accessor.RefreshItem (this.Item);
				accessor.NotifyItemChanged (this.Item);
			}

			public override void ResetToOriginalValue()
			{
				if (this.originalInterfaceIds != null)
				{
					this.Data.UnlockValue (Res.Fields.ResourceStructuredType.InterfaceIds);
					this.Data.ResetToOriginalValue (Res.Fields.ResourceStructuredType.InterfaceIds);
					this.Data.LockValue (Res.Fields.ResourceStructuredType.InterfaceIds);

					ObservableList<StructuredData> interfaceIds = this.Data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as ObservableList<StructuredData>;

					using (interfaceIds.DisableNotifications ())
					{
						int index = interfaceIds.Count - 1;

						while (index >= 0)
						{
							StructuredData data = interfaceIds[index];
							interfaceIds.RemoveAt (index--);
						}

						System.Diagnostics.Debug.Assert (interfaceIds.Count == 0);

						foreach (StructuredData data in this.originalInterfaceIds)
						{
							StructuredData copy = data.GetShallowCopy ();
							copy.PromoteToOriginal ();
							interfaceIds.Add (copy);
						}
					}
				}
			}
			
			List<StructuredData> originalInterfaceIds;
		}

		#endregion

		#region FieldBroker Class

		private class FieldBroker : IDataBroker
		{
			#region IDataBroker Members

			public StructuredData CreateData(CultureMap container)
			{
				StructuredData data = new StructuredData (Res.Types.Field);

				data.SetValue (Res.Fields.Field.TypeId, Druid.Empty);
				data.SetValue (Res.Fields.Field.CaptionId, Druid.Empty);
				data.SetValue (Res.Fields.Field.Relation, FieldRelation.None);
				data.SetValue (Res.Fields.Field.Membership, FieldMembership.Local);
				data.SetValue (Res.Fields.Field.CultureMapSource, container.Source);
				data.SetValue (Res.Fields.Field.Source, FieldSource.Value);
				data.SetValue (Res.Fields.Field.Options, FieldOptions.None);
				data.SetValue (Res.Fields.Field.Expression, "");
				data.SetValue (Res.Fields.Field.DefiningTypeId, Druid.Empty);
				data.SetValue (Res.Fields.Field.DeepDefiningTypeId, Druid.Empty);
				data.LockValue (Res.Fields.Field.DefiningTypeId);
				data.LockValue (Res.Fields.Field.DeepDefiningTypeId);
				
				return data;
			}

			#endregion
		}

		#endregion

		#region InterfaceIdBroker Class

		private class InterfaceIdBroker : IDataBroker
		{
			#region IDataBroker Members

			public StructuredData CreateData(CultureMap container)
			{
				StructuredData data = new StructuredData (Res.Types.InterfaceId);

				data.SetValue (Res.Fields.InterfaceId.CaptionId, Druid.Empty);
				data.SetValue (Res.Fields.InterfaceId.CultureMapSource, container.Source);
				
				return data;
			}

			#endregion
		}

		#endregion

		#region AccessorsCollection Class

		/// <summary>
		/// The <c>AccessorsCollection</c> maintains a collection of <see cref="StructuredTypeReourceAccessor"/>
		/// instances.
		/// </summary>
		private class AccessorsCollection
		{
			public AccessorsCollection()
			{
				this.list = new List<Weak<StructuredTypeResourceAccessor>> ();
			}

			public void Add(StructuredTypeResourceAccessor item)
			{
				this.list.Add (new Weak<StructuredTypeResourceAccessor> (item));
			}

			public void Remove(StructuredTypeResourceAccessor item)
			{
				this.list.RemoveAll (
					delegate (Weak<StructuredTypeResourceAccessor> probe)
					{
						if (probe.IsAlive)
						{
							return probe.Target == item;
						}
						else
						{
							return true;
						}
					});
			}

			public IEnumerable<StructuredTypeResourceAccessor> Collection
			{
				get
				{
					foreach (Weak<StructuredTypeResourceAccessor> item in this.list)
					{
						StructuredTypeResourceAccessor accessor = item.Target;

						if (accessor != null)
						{
							yield return accessor;
						}
					}
				}
			}

			List<Weak<StructuredTypeResourceAccessor>> list;
		}

		#endregion

		private static bool? ToBoolean(object value)
		{
			return UndefinedValue.GetValue<bool?> (value, null);
		}

		private static Druid ToDruid(object value)
		{
			return UndefinedValue.GetValue<Druid> (value, Druid.Empty);
		}

		private static StructuredTypeClass ToStructuredTypeClass(object value)
		{
			return UndefinedValue.GetValue<StructuredTypeClass> (value, StructuredTypeClass.None);
		}


		private static void SetAccessors(DependencyObject obj, AccessorsCollection collection)
		{
			if (collection == null)
			{
				obj.ClearValue (StructuredTypeResourceAccessor.AccessorsProperty);
			}
			else
			{
				obj.SetValue (StructuredTypeResourceAccessor.AccessorsProperty, collection);
			}
		}

		private static AccessorsCollection GetAccessors(DependencyObject obj)
		{
			return obj.GetValue (StructuredTypeResourceAccessor.AccessorsProperty) as AccessorsCollection;
		}

		private static DependencyProperty AccessorsProperty = DependencyProperty.RegisterAttached ("Accessors", typeof (AccessorsCollection), typeof (StructuredTypeResourceAccessor), new DependencyPropertyMetadata ().MakeNotSerializable ());

		private FieldResourceAccessor fieldAccessor;
		private bool postponeRefreshWhileLoading;
	}
}
