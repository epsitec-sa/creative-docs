//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			if (manager.BasedOnPatchModule)
			{
				this.referenceAccessor = new StructuredTypeResourceAccessor ();
				this.referenceAccessor.Load (manager.GetManagerForReferenceModule ());
			}

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
				return new FieldBroker (this);
			}
			if (fieldId == Res.Fields.ResourceStructuredType.InterfaceIds.ToString ())
			{
				return new InterfaceIdBroker (this);
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
		/// Notifies the resource accessor that the specified item changed.
		/// </summary>
		/// <param name="item">The item which was modified.</param>
		/// <param name="container">The container which changed, if any.</param>
		/// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		public override void NotifyItemChanged(CultureMap item, StructuredData container, DependencyPropertyChangedEventArgs e)
		{
			base.NotifyItemChanged (item, container, e);

			if ((container != null) &&
				(e != null) &&
				(item.Source == CultureMapSource.DynamicMerge))
			{
				Druid id = Druid.Parse (e.PropertyName);

				//	If the expression of a given field was edited in a patch module,
				//	then we have to update the field's source to reflect the fact that
				//	it is the result of a dynamic merge.

				if ((id == Res.Fields.Field.Expression) &&
					((CultureMapSource) container.GetValue (Res.Fields.Field.CultureMapSource) == CultureMapSource.ReferenceModule))
				{
					container.SetValue (Res.Fields.Field.CultureMapSource, CultureMapSource.DynamicMerge);
				}
			}
		}


		/// <summary>
		/// Gets the type based on the data stored in the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <param name="namedTypeResolver">The named type resolver which knows how to resolve an id to a type.</param>
		/// <returns>The type or <c>null</c>.</returns>
		public StructuredType GetStructuredTypeViewOfData(CultureMap item, string twoLetterISOLanguageName, System.Func<Druid, INamedType> namedTypeResolver)
		{
			if (this.ContainsCaption (twoLetterISOLanguageName) == false)
			{
				twoLetterISOLanguageName = Resources.DefaultTwoLetterISOLanguageName;
			}

			StructuredData data = item.GetCultureData (twoLetterISOLanguageName);
			Caption caption = base.CreateCaptionFromData (item.Id, null, data, item.Name, twoLetterISOLanguageName);

			return this.CreateTypeFromData (data, caption, namedTypeResolver);
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
				
				this.RefreshFields (item);
			}
			else
			{
				base.ResetToOriginal (item, container, fieldId);
			}
		}

		/// <summary>
		/// Promotes the data in the specified data record to the "original"
		/// state. This is required in order to be able to reset the data back
		/// later on.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="data">The data record.</param>
		protected override void PromoteToOriginal(CultureMap item, StructuredData data)
		{
			base.PromoteToOriginal (item, data);

			ObservableList<StructuredData> fields = data.GetValue (Res.Fields.ResourceStructuredType.Fields) as ObservableList<StructuredData>;
			ObservableList<StructuredData> interfaceIds = data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as ObservableList<StructuredData>;

			using (fields.DisableNotifications ())
			{
				foreach (StructuredData field in fields)
				{
					field.PromoteToOriginal ();
				}
			}

			using (interfaceIds.DisableNotifications ())
			{
				foreach (StructuredData interfaceId in interfaceIds)
				{
					interfaceId.PromoteToOriginal ();
				}
			}

			Listener.FindListener<FieldListener> (fields).CreateSnapshot ();
			Listener.FindListener<InterfaceListener> (interfaceIds).CreateSnapshot ();
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
		/// <param name="captionId">The caption id.</param>
		/// <param name="sourceBundle">The source bundle.</param>
		/// <param name="data">The data record.</param>
		/// <param name="name">The name of the caption.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <returns>A <see cref="Caption"/> instance.</returns>
		protected override Caption CreateCaptionFromData(Druid captionId, ResourceBundle sourceBundle, Types.StructuredData data, string name, string twoLetterISOLanguageName)
		{
			Caption caption = base.CreateCaptionFromData (captionId, sourceBundle, data, name, twoLetterISOLanguageName);
			
			if (twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName)
			{
				StructuredType type = this.CreateTypeFromData (data, caption, null);

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
				object                 baseTypeValue   = data.GetValue (Res.Fields.ResourceStructuredType.BaseType);
				object                 classValue      = data.GetValue (Res.Fields.ResourceStructuredType.Class);
				IList<StructuredData>  fields          = data.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
				IList<StructuredData>  interfaceIds    = data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;
				string                 designerLayouts = data.GetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts) as string;
				DataLifetimeExpectancy defaultLifetime = data.GetValueOrDefault<DataLifetimeExpectancy> (Res.Fields.ResourceStructuredType.DefaultLifetimeExpectancy);
				StructuredTypeFlags    flags           = data.GetValueOrDefault<StructuredTypeFlags> (Res.Fields.ResourceStructuredType.Flags);

				if ((UndefinedValue.IsUndefinedValue (baseTypeValue)) &&
					/*(UndefinedValue.IsUndefinedValue (classValue)) && -- see ComputeDataDelta */
					((fields == null) || (fields.Count == 0)) &&
					((interfaceIds == null) || (interfaceIds.Count == 0)) &&
					(string.IsNullOrEmpty (designerLayouts)) &&
					(defaultLifetime == DataLifetimeExpectancy.Unknown) &&
					(flags == StructuredTypeFlags.None))
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
			
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceStructuredType.BaseType);

			string rawLayout = rawData.GetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts) as string;
			string refLayout = refData.GetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts) as string;

			if (rawLayout != refLayout)
			{
				AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceStructuredType.SerializedDesignerLayouts);
			}

			var rawLifetime = rawData.GetValueOrDefault<DataLifetimeExpectancy> (Res.Fields.ResourceStructuredType.DefaultLifetimeExpectancy);
			var refLifetime = refData.GetValueOrDefault<DataLifetimeExpectancy> (Res.Fields.ResourceStructuredType.DefaultLifetimeExpectancy);

			if (rawLifetime != refLifetime)
			{
				AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceStructuredType.DefaultLifetimeExpectancy);
			}

			var rawFlags = rawData.GetValueOrDefault<StructuredTypeFlags> (Res.Fields.ResourceStructuredType.Flags);
			var refFla	 = refData.GetValueOrDefault<StructuredTypeFlags> (Res.Fields.ResourceStructuredType.Flags);

			if (rawFlags != refFla)
			{
				AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceStructuredType.Flags);
			}

			//	The structured type class must be defined, or else we won't be able
			//	to generate the correct StructuredType instance for the caption
			//	serialization. Defining only the "Class" value won't make the record
			//	show up as non-empty in the IsEmptyCaption test.
			
			patchData.SetValue (Res.Fields.ResourceStructuredType.Class, refData.GetValue (Res.Fields.ResourceStructuredType.Class));
			
			IList<StructuredData> refFields = refData.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
			IList<StructuredData> rawFields = rawData.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
			
			IList<StructuredData> refInterfaceIds = refData.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;
			IList<StructuredData> rawInterfaceIds = rawData.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;
			
			if ((rawFields != null) &&
				(rawFields.Count > 0) &&
				(!Types.Collection.CompareEqual (rawFields, refFields)))
			{
				List<StructuredData> temp = new List<StructuredData> ();

				//	Include all fields which are either local to the patch module or
				//	defined both in the patch module and in the reference module. The
				//	deserialization will take care of properly merging the fields.

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

				//	Include all interfaces which are defined by the patch module.
				//	No merge is possible at this level.
				
				foreach (StructuredData interfaceId in rawInterfaceIds)
				{
					CultureMapSource interfaceIdSource = (CultureMapSource) interfaceId.GetValue (Res.Fields.InterfaceId.CultureMapSource);

					System.Diagnostics.Debug.Assert (interfaceIdSource != CultureMapSource.DynamicMerge);

					if (interfaceIdSource == CultureMapSource.PatchModule)
					{
						temp.Add (interfaceId);
					}
				}

				patchData.SetValue (Res.Fields.ResourceStructuredType.InterfaceIds, temp);
			}
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

		#region CollectionChanged Handler Methods

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
				CultureMap     item = sender as CultureMap;
				StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
				Druid   newBaseType = (Druid) e.NewValue;
				
				this.UpdateInheritedFields (item, data, newBaseType, null);
			}
		}

		#endregion

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
				
				this.UpdateInheritedFields (item, data, baseTypeId, null);
				
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
		private StructuredType CreateTypeFromData(StructuredData data, Caption caption, System.Func<Druid, INamedType> namedTypeResolver)
		{
			if (UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStructuredType.Class)))
			{
				return null;
			}

			StructuredTypeClass    typeClass = StructuredTypeResourceAccessor.ToStructuredTypeClass (data.GetValue (Res.Fields.ResourceStructuredType.Class));
			Druid                  baseType  = StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.ResourceStructuredType.BaseType));
			string                 layouts   = data.GetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts) as string;
			DataLifetimeExpectancy lifetime  = data.GetValueOrDefault<DataLifetimeExpectancy> (Res.Fields.ResourceStructuredType.DefaultLifetimeExpectancy);
			StructuredTypeFlags    flags     = data.GetValueOrDefault<StructuredTypeFlags> (Res.Fields.ResourceStructuredType.Flags);

			StructuredType type = new StructuredType (typeClass, baseType);

			if (caption != null)
			{
				type.DefineCaption (caption);
			}

			type.SerializedDesignerLayouts = layouts;
			type.DefaultLifetimeExpectancy = lifetime;
			type.Flags                     = flags;
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
					FieldMembership membership = StructuredTypeResourceAccessor.Simplify ((FieldMembership) fieldData.GetValue (Res.Fields.Field.Membership));
					FieldSource source = (FieldSource) fieldData.GetValue (Res.Fields.Field.Source);
					FieldOptions options = (FieldOptions) fieldData.GetValue (Res.Fields.Field.Options);
					string expression = fieldData.GetValue (Res.Fields.Field.Expression) as string;
					Druid fieldDefiningType = StructuredTypeResourceAccessor.ToDruid (fieldData.GetValue (Res.Fields.Field.DefiningTypeId));
					bool? interfaceDefinition = StructuredTypeResourceAccessor.ToBoolean (fieldData.GetValue (Res.Fields.Field.IsInterfaceDefinition));

					//	A field must be stored in the type only if it is defined locally
					//	and if it does not belong to a locally defined interface; or
					//	if it redefines the expression for a local interface field or
					//	overrides an inherited field.

					if (((membership == FieldMembership.Local) && (fieldDefiningType.IsEmpty)) ||
						((membership == FieldMembership.Local) && (interfaceDefinition.HasValue) && (interfaceDefinition.Value == false)) ||
						(namedTypeResolver != null))
					{
						StructuredTypeField field = new CustomField (fieldType, fieldCaption, rank++, relation, membership, source, options, expression, namedTypeResolver);
						type.Fields.Add (field);
					}
				}
			}

			return type;
		}

		private class CustomField : StructuredTypeField
		{
			public CustomField(Druid typeId, Druid captionId, int rank, FieldRelation relation, FieldMembership membership, FieldSource source, FieldOptions options, string expression, System.Func<Druid, INamedType> namedTypeResolver)
				: base (null, null, captionId, rank, relation, membership, source, options, expression)
			{
				this.namedTypeResolver = namedTypeResolver;
				this.DefineTypeId (typeId);
			}

			public override INamedType Type
			{
				get
				{
					if (this.type == null)
					{
						if (this.namedTypeResolver == null)
						{
							throw new System.InvalidOperationException ("Trying to read inexistant type information");
						}

						this.type = this.namedTypeResolver (this.typeId);
					}

					return this.type;
				}
			}

			private readonly System.Func<Druid, INamedType> namedTypeResolver;
		}

		/// <summary>
		/// Fills the data record based on a <see cref="StructuredType"/> instance.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="data">The data record which is to be filled.</param>
		/// <param name="type">The type instance.</param>
		/// <param name="mode">The data creation mode.</param>
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
			if (interfaceIds == null)
			{
				interfaceIds = new ObservableList<StructuredData> ();
				recordInterfaceIds = true;
			}

			if (type != null)
			{
				type.FreezeInheritance ();

				UpdateState state = UpdateState.None;

				//	Populate the field list.

				foreach (string fieldId in type.GetFieldIds ())
				{
					StructuredTypeField typeField = type.Fields[fieldId];
					StructuredData oldField;
					StructuredData newField = new StructuredData (Res.Types.Field);
					StructuredTypeResourceAccessor.FillDataFromField (item, newField, typeField, recordFields ? item.Source : CultureMapSource.PatchModule);

				again:

					if (Types.Collection.TryFind (fields,
						/**/					  delegate (StructuredData find)
												  {
													  return StructuredTypeResourceAccessor.ToDruid (find.GetValue (Res.Fields.Field.CaptionId)) == typeField.CaptionId;
												  },
						/**/					  out oldField))
					{
						//	This field was already defined previously, in the reference
						//	module. This will result in a merged expression definition
						//	without adding a new record to the field list :

						System.Diagnostics.Debug.Assert (oldField != null);
						System.Diagnostics.Debug.Assert (recordFields == false);
						System.Diagnostics.Debug.Assert (state != UpdateState.Adding);
						System.Diagnostics.Debug.Assert (this.referenceAccessor != null);

						if (state == UpdateState.None)
						{
							this.SnapshotReferenceFields (item, data, fields);
							state = UpdateState.Merging;

							//	The previously queried "old" field might no longer be in
							//	use now: creating a snapshot will call UpdateInheritedFields
							//	which can replace the field definitions when merging inherited
							//	data with local overrides.

							goto again;
						}

						oldField.SetValue (Res.Fields.Field.Expression, newField.GetValue (Res.Fields.Field.Expression));
						oldField.SetValue (Res.Fields.Field.CultureMapSource, CultureMapSource.DynamicMerge);
					}
					else
					{
						//	This is a new field definition; simply add it to the list
						//	and, if needed, attach the new record to the item event handler
						//	code :

						state = UpdateState.Adding;
						fields.Add (newField);

						if (mode == DataCreationMode.Public)
						{
							item.NotifyDataAdded (newField);
						}
					}
				}

				if ((state == UpdateState.None) &&
					(recordFields == false))
				{
					this.SnapshotReferenceFields (item, data, fields);
				}

				//	Populate the interface list.

				state = UpdateState.None;

				foreach (Druid interfaceId in type.InterfaceIds)
				{
					StructuredData oldInterfaceId;
					StructuredData newInterfaceId = new StructuredData (Res.Types.InterfaceId);

					if (Types.Collection.TryFind (interfaceIds,
						/**/					  delegate (StructuredData find)
												  {
													  return StructuredTypeResourceAccessor.ToDruid (find.GetValue (Res.Fields.InterfaceId.CaptionId)) == interfaceId;
												  },
						/**/					  out oldInterfaceId))
					{
						System.Diagnostics.Debug.Fail ("Duplicate interface definition");
					}
					else
					{
						//	This is a new interface id definition; add it to the list
						//	and attach the new record to the item event handler code :

						state = UpdateState.Adding;
						newInterfaceId.SetValue (Res.Fields.InterfaceId.CaptionId, interfaceId);
						newInterfaceId.SetValue (Res.Fields.InterfaceId.CultureMapSource, recordInterfaceIds ? item.Source : CultureMapSource.PatchModule);

						interfaceIds.Add (newInterfaceId);

						if (mode == DataCreationMode.Public)
						{
							item.NotifyDataAdded (newInterfaceId);
						}
					}
				}

				if ((state == UpdateState.None) &&
					(recordInterfaceIds == false))
				{
					this.SnapshotReferenceInterfaceIds (item, data, interfaceIds);
				}
				
				//	Fill the remaining entity definitions by overwriting the previous
				//	data :
				
				if ((type.BaseTypeId.IsValid) ||
					(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStructuredType.BaseType))))
				{
					data.SetValue (Res.Fields.ResourceStructuredType.BaseType, type.BaseTypeId);
				}

				if (UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStructuredType.Class)))
				{
					data.SetValue (Res.Fields.ResourceStructuredType.Class, type.Class);
				}

				if ((!string.IsNullOrEmpty (type.SerializedDesignerLayouts)) ||
					(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts))))
				{
					data.SetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts, type.SerializedDesignerLayouts);
				}

				if (type.DefaultLifetimeExpectancy != DataLifetimeExpectancy.Unknown)
				{
					data.SetValue (Res.Fields.ResourceStructuredType.DefaultLifetimeExpectancy, type.DefaultLifetimeExpectancy);
				}
				
				if (type.Flags != StructuredTypeFlags.None)
				{
					data.SetValue (Res.Fields.ResourceStructuredType.Flags, type.Flags);
				}
			}
			else
			{
				data.SetValue (Res.Fields.ResourceStructuredType.BaseType, Druid.Empty);
				data.SetValue (Res.Fields.ResourceStructuredType.Class, StructuredTypeClass.None);
				data.SetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts, "");
				data.SetValue (Res.Fields.ResourceStructuredType.DefaultLifetimeExpectancy, DataLifetimeExpectancy.Unknown);
				data.SetValue (Res.Fields.ResourceStructuredType.Flags, StructuredTypeFlags.None);
			}

			//	Record the fields and the interface ids collections into the
			//	entity record :

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
		}

		#region UpdateState Enumeration

		private enum UpdateState
		{
			None,
			Merging,
			Adding
		}

		#endregion

		/// <summary>
		/// Creates a snapshot of the fields, to be used when the <c>ResetToOriginal</c>
		/// method gets called.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="data">The data record.</param>
		/// <param name="fields">The fields collection.</param>
		private void SnapshotReferenceFields(CultureMap item, StructuredData data, ObservableList<StructuredData> fields)
		{
			//	The fields list is still intact. We have to artificially
			//	cause an "original" snapshot to be taken, but for this,
			//	all inherited fields have to be included first :

			Druid baseTypeId = StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.ResourceStructuredType.BaseType));

			this.UpdateInheritedFields (item, data, baseTypeId, this.referenceAccessor);

			//	Snapshot by faking a collection changing event :

			FieldListener listener = Listener.FindListener<FieldListener> (fields);
			System.Diagnostics.Debug.Assert (listener.HasSnapshotData);	// is this always true ?
			listener.CreateSnapshot ();
		}

		/// <summary>
		/// Creates a snapshot of the interface ids, to be used when the <c>ResetToOriginal</c>
		/// method gets called.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="data">The data record.</param>
		/// <param name="interfaceIds">The interface ids collection.</param>
		private void SnapshotReferenceInterfaceIds(CultureMap item, StructuredData data, ObservableList<StructuredData> interfaceIds)
		{
			//	The interface ids list is still intact. We have to artificially
			//	cause an "original" snapshot to be taken :
			
			InterfaceListener listener = Listener.FindListener<InterfaceListener> (interfaceIds);
			System.Diagnostics.Debug.Assert (listener.HasSnapshotData);	// is this always true ?
			listener.CreateSnapshot ();
		}

		/// <summary>
		/// Fills the data record based on a field definition.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="data">The data record which is to be filled.</param>
		/// <param name="field">The field definition.</param>
		/// <param name="source">The item source for this field.</param>
		private static void FillDataFromField(CultureMap item, StructuredData data, StructuredTypeField field, CultureMapSource source)
		{
			System.Diagnostics.Debug.Assert (field.DefiningTypeId.IsEmpty);
			System.Diagnostics.Debug.Assert (field.Membership != FieldMembership.LocalOverride);

			data.SetValue (Res.Fields.Field.TypeId, field.Type == null ? Druid.Empty : field.Type.CaptionId);
			data.SetValue (Res.Fields.Field.CaptionId, field.CaptionId);
			data.SetValue (Res.Fields.Field.Relation, field.Relation);
			data.SetValue (Res.Fields.Field.Membership, field.Membership);
			data.SetValue (Res.Fields.Field.CultureMapSource, source);
			data.SetValue (Res.Fields.Field.Source, field.Source);
			data.SetValue (Res.Fields.Field.Options, field.Options);
			data.SetValue (Res.Fields.Field.Expression, field.Expression ?? "");
			data.SetValue (Res.Fields.Field.DefiningTypeId, Druid.Empty);
			data.SetValue (Res.Fields.Field.DeepDefiningTypeId, Druid.Empty);
			data.LockValue (Res.Fields.Field.DefiningTypeId);
			data.LockValue (Res.Fields.Field.DeepDefiningTypeId);
		}

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
		/// <param name="item">The item.</param>
		/// <param name="data">The data describing the structured type.</param>
		/// <param name="baseTypeId">The Druid of the base type.</param>
		/// <param name="accessor">The specific accessor (or <c>null</c>).</param>
		private void UpdateInheritedFields(CultureMap item, StructuredData data, Druid baseTypeId, StructuredTypeResourceAccessor accessor)
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
					FieldUpdater updater = new FieldUpdater (accessor ?? this);

					updater.IncludeType (baseTypeId, FieldMembership.Inherited);

					foreach (StructuredData interfaceData in interfaceIds)
					{
						Druid interfaceId = StructuredTypeResourceAccessor.ToDruid (interfaceData.GetValue (Res.Fields.InterfaceId.CaptionId));
						updater.IncludeType (interfaceId, FieldMembership.Local);
						updater.IncludeInterfaceId (interfaceId);
					}

					StructuredTypeResourceAccessor.RemoveInheritedFields (item, fields, updater.InterfaceIds);

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
							item.NotifyDataAdded (field);
						}
						else
						{
							FieldMembership membership = (FieldMembership) field.GetValue (Res.Fields.Field.Membership);
							Druid definingTypeId = StructuredTypeResourceAccessor.ToDruid (field.GetValue (Res.Fields.Field.DefiningTypeId));

							switch (membership)
							{
								case FieldMembership.Local:
									//	The local field definition provides an implementation for the
									//	matching field in the interface.

									System.Diagnostics.Debug.Assert (updater.InterfaceIds.Contains (definingTypeId));
									break;

								case FieldMembership.Inherited:
									//	The local field definition overrides the parent field definition. The
									//	field will be identified with the special membership value.
									field.SetValue (Res.Fields.Field.Membership, FieldMembership.LocalOverride);
									break;

								default:
									throw new System.NotImplementedException (string.Format ("Unsupported field override for field {0}", field.GetValue (Res.Fields.Field.CaptionId)));
							}

							//	The field is defined both locally and in the interface or in a
							//	parent (this is a real class inheritance); keep just the
							//	expression from the local definition and for the other values,
							//	use the inherited or interface definitions :

							field.SilentlyCopyValueFrom (Res.Fields.Field.CultureMapSource, fields[pos]);
							field.SilentlyCopyValueFrom (Res.Fields.Field.Source, fields[pos]);
							field.SilentlyCopyValueFrom (Res.Fields.Field.Expression, fields[pos]);
							field.SetValue (Res.Fields.Field.IsInterfaceDefinition, false);
							field.PromoteToOriginalValue (Res.Fields.Field.IsInterfaceDefinition.ToString ());

							System.Diagnostics.Debug.Assert (i <= pos);

							item.NotifyDataRemoved (fields[pos]);

							fields.RemoveAt (pos);
							fields.Insert (i++, field);

							item.NotifyDataAdded (field);
						}

                        Druid DT = StructuredTypeResourceAccessor.ToDruid(field.GetValue(Res.Fields.Field.DefiningTypeId));
                        bool? IID = StructuredTypeResourceAccessor.ToBoolean(field.GetValue(Res.Fields.Field.IsInterfaceDefinition));
                        FieldMembership M = (FieldMembership)field.GetValue(Res.Fields.Field.Membership);
                        Druid FI = StructuredTypeResourceAccessor.ToDruid(field.GetValue(Res.Fields.Field.CaptionId));

                        System.Diagnostics.Debug.WriteLine (string.Format ("EntityName={0} EntityId={1} FieldId={2} DT={3} M={4} IID={5}", item.Name, item.Id, FI, DT, M, IID.HasValue ? IID.Value.ToString() : "<null>"));
					}
				}
				else
				{
					StructuredTypeResourceAccessor.RemoveInheritedFields (item, fields);
				}
			}
		}

		/// <summary>
		/// Finds the index of a compatible field in the given collection.
		/// </summary>
		/// <param name="fields">The fields collection.</param>
		/// <param name="field">The field to locate.</param>
		/// <returns>The index of the field in the collection or <c>-1</c>
		/// if no compatible field can be found.</returns>
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
		private static void RemoveInheritedFields(CultureMap item, IList<StructuredData> fields)
		{
			StructuredTypeResourceAccessor.RemoveInheritedFields (item, fields, EmptyList<Druid>.Instance);
		}

		/// <summary>
		/// Removes the inherited and included fields, but keeps locally redefined
		/// or overriden fields.
		/// </summary>
		/// <param name="fields">The field collection.</param>
		/// <param name="interfaceIds">The live interface ids.</param>
		private static void RemoveInheritedFields(CultureMap item, IList<StructuredData> fields, IList<Druid> interfaceIds)
		{
			for (int i = 0; i < fields.Count; i++)
			{
				StructuredData field = fields[i];
				FieldMembership membership = (FieldMembership) field.GetValue (Res.Fields.Field.Membership);
				Druid definingTypeId = StructuredTypeResourceAccessor.ToDruid (field.GetValue (Res.Fields.Field.DefiningTypeId));
				bool? interfaceDefinition = StructuredTypeResourceAccessor.ToBoolean (field.GetValue (Res.Fields.Field.IsInterfaceDefinition));
				
				if (membership == FieldMembership.Inherited)
				{
					//	If the field is inherited from a base entity or imported through
					//	an interface, then we remove it from the collection :
					
					fields.RemoveAt (i--);
					item.NotifyDataRemoved (field);
				}
				else if (membership == FieldMembership.LocalOverride)
				{
					//	If the field is inherited from a parent entity and is overridden
					//	locally, then we keep it.
				}
				else if (definingTypeId.IsValid)
				{
					if ((interfaceDefinition.HasValue) &&
						(interfaceDefinition.Value == false))
					{
						//	The field is defined locally and its content is not defined
						//	in an interface; if the interface still exists for this entity,
						//	then keep the field.

						if (interfaceIds.Contains (definingTypeId))
						{
							continue;
						}
					}

					//	The field is simply inherited by a parent, without any additional
					//	local information; discard it.
					
					fields.RemoveAt (i--);
					item.NotifyDataRemoved (field);
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

				Druid                 baseId       = StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.ResourceStructuredType.BaseType));
				IList<StructuredData> interfaceIds = data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;
				IList<StructuredData> fields       = data.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

				if (baseId.IsValid)
				{
					this.IncludeType (baseId, definingTypeId, FieldMembership.Inherited, depth+1);
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

		#region FieldListener Class

		protected class FieldListener : Listener
		{
			public FieldListener(StructuredTypeResourceAccessor accessor, CultureMap item, StructuredData data)
				: base (accessor, item, data)
			{
			}

			public bool HasSnapshotData
			{
				get
				{
					return this.originalFields != null;
				}
			}

			public void CreateSnapshot()
			{
				if (this.SaveField (Res.Fields.ResourceStructuredType.Fields))
				{
					this.originalFields = new List<StructuredData> ();

					IList<StructuredData> fields = this.Data.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

					foreach (StructuredData data in fields)
					{
						this.originalFields.Add (data.GetShallowCopy ());
					}
				}
			}

			public override void HandleCollectionChanging(object sender)
			{
				this.CreateSnapshot ();
			}

			public override void ResetToOriginalValue()
			{
				if (this.originalFields != null)
				{
					this.RestoreField (Res.Fields.ResourceStructuredType.Fields);

					ObservableList<StructuredData> fields = this.Data.GetValue (Res.Fields.ResourceStructuredType.Fields) as ObservableList<StructuredData>;
					List<Druid> fieldIds = new List<Druid> ();

					using (fields.DisableNotifications ())
					{
						int index = fields.Count - 1;
						
						while (index >= 0)
						{
							StructuredData data = fields[index];

							if (FieldListener.IsLocalField (data))
							{
								fieldIds.Add (StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.Field.CaptionId)));
							}
							
							fields.RemoveAt (index--);
							this.Item.NotifyDataRemoved (data);
						}

						System.Diagnostics.Debug.Assert (fields.Count == 0);

						foreach (StructuredData data in this.originalFields)
						{
							if (FieldListener.IsLocalField (data))
							{
								fieldIds.Remove (StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.Field.CaptionId)));
							}
							
							StructuredData copy = data.GetShallowCopy ();
							fields.Add (copy);
							this.Item.NotifyDataAdded (copy);
							copy.PromoteToOriginal ();
						}
					}

					StructuredTypeResourceAccessor accessor = this.Accessor as StructuredTypeResourceAccessor;
					IResourceAccessor fieldAccessor = accessor.FieldAccessor;
					
					if (fieldIds.Count > 0)
					{
						//	Some fields got orphaned while resetting to the original fields
						//	collection. We have to remove them from the field accessor too,
						//	or else we will accumulate dead fields over the time :

						foreach (Druid id in fieldIds)
						{
							CultureMap fieldItem = fieldAccessor.Collection[id];

							if (fieldItem != null)
							{
								fieldAccessor.Collection.Remove (fieldItem);
							}
						}
					}

					accessor.RefreshItem (this.Item);
				}
			}

			private static bool IsLocalField(StructuredData data)
			{
				Druid fieldDefiningType = StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.Field.DefiningTypeId));
				FieldMembership membership = StructuredTypeResourceAccessor.Simplify ((FieldMembership) data.GetValue (Res.Fields.Field.Membership));
				bool? interfaceDefinition = StructuredTypeResourceAccessor.ToBoolean (data.GetValue (Res.Fields.Field.IsInterfaceDefinition));

				if (((membership == FieldMembership.Local) && (fieldDefiningType.IsEmpty)) ||
					((membership == FieldMembership.Local) && (interfaceDefinition.HasValue) && (interfaceDefinition.Value == false)))
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			private List<StructuredData> originalFields;
		}

		#endregion

		#region InterfaceListener Class

		protected class InterfaceListener : Listener
		{
			public InterfaceListener(StructuredTypeResourceAccessor accessor, CultureMap item, StructuredData data)
				: base (accessor, item, data)
			{
			}

			public bool HasSnapshotData
			{
				get
				{
					return this.originalInterfaceIds != null;
				}
			}

			public void CreateSnapshot()
			{
				if (this.SaveField (Res.Fields.ResourceStructuredType.InterfaceIds))
				{
					this.originalInterfaceIds = new List<StructuredData> ();

					IList<StructuredData> interfaceIds = this.Data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;

					foreach (StructuredData data in interfaceIds)
					{
						this.originalInterfaceIds.Add (data.GetShallowCopy ());
					}
				}
			}

			public override void HandleCollectionChanging(object sender)
			{
				this.CreateSnapshot ();
			}

			public override void HandleCollectionChanged(object sender, CollectionChangedEventArgs e)
			{
				StructuredTypeResourceAccessor accessor = this.Accessor as StructuredTypeResourceAccessor;
				accessor.RefreshItem (this.Item);
				
				base.HandleCollectionChanged (sender, e);
			}

			public override void ResetToOriginalValue()
			{
				if (this.originalInterfaceIds != null)
				{
					this.RestoreField (Res.Fields.ResourceStructuredType.InterfaceIds);

					ObservableList<StructuredData> interfaceIds = this.Data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as ObservableList<StructuredData>;

					using (interfaceIds.DisableNotifications ())
					{
						int index = interfaceIds.Count - 1;

						while (index >= 0)
						{
							StructuredData data = interfaceIds[index];
							interfaceIds.RemoveAt (index--);
							this.Item.NotifyDataRemoved (data);
						}

						System.Diagnostics.Debug.Assert (interfaceIds.Count == 0);

						foreach (StructuredData data in this.originalInterfaceIds)
						{
							StructuredData copy = data.GetShallowCopy ();
							interfaceIds.Add (copy);
							this.Item.NotifyDataAdded (copy);
							copy.PromoteToOriginal ();
						}
					}

					StructuredTypeResourceAccessor accessor = this.Accessor as StructuredTypeResourceAccessor;
					accessor.RefreshItem (this.Item);
				}
			}

			private List<StructuredData> originalInterfaceIds;
		}

		#endregion

		#region FieldBroker Class

		private class FieldBroker : IDataBroker
		{
			public FieldBroker(StructuredTypeResourceAccessor accessor)
			{
				this.accessor = accessor;
			}

			#region IDataBroker Members

			public StructuredData CreateData(CultureMap container)
			{
				StructuredData data = new StructuredData (Res.Types.Field);
				CultureMapSource source = container.Source;

				if (this.accessor.BasedOnPatchModule)
				{
					source = CultureMapSource.PatchModule;
				}

				data.SetValue (Res.Fields.Field.TypeId, Druid.Empty);
				data.SetValue (Res.Fields.Field.CaptionId, Druid.Empty);
				data.SetValue (Res.Fields.Field.Relation, FieldRelation.None);
				data.SetValue (Res.Fields.Field.Membership, FieldMembership.Local);
				data.SetValue (Res.Fields.Field.CultureMapSource, source);
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

			private StructuredTypeResourceAccessor accessor;
		}

		#endregion

		#region InterfaceIdBroker Class

		private class InterfaceIdBroker : IDataBroker
		{
			public InterfaceIdBroker(StructuredTypeResourceAccessor accessor)
			{
				this.accessor = accessor;
			}

			#region IDataBroker Members

			public StructuredData CreateData(CultureMap container)
			{
				StructuredData data = new StructuredData (Res.Types.InterfaceId);
				CultureMapSource source = container.Source;

				if (this.accessor.BasedOnPatchModule)
				{
					source = CultureMapSource.PatchModule;
				}

				data.SetValue (Res.Fields.InterfaceId.CaptionId, Druid.Empty);
				data.SetValue (Res.Fields.InterfaceId.CultureMapSource, source);
				
				return data;
			}

			#endregion

			private StructuredTypeResourceAccessor accessor;
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

			private List<Weak<StructuredTypeResourceAccessor>> list;
		}

		#endregion

		public static bool? ToBoolean(object value)
		{
			return UndefinedValue.GetValue<bool?> (value, null);
		}

		internal static Druid ToDruid(object value)
		{
			return UndefinedValue.GetValue<Druid> (value, Druid.Empty);
		}

		private static StructuredTypeClass ToStructuredTypeClass(object value)
		{
			return UndefinedValue.GetValue<StructuredTypeClass> (value, StructuredTypeClass.None);
		}

		private static FieldMembership Simplify(FieldMembership membership)
		{
			switch (membership)
			{
				case FieldMembership.LocalOverride:
					return FieldMembership.Local;
				
				default:
					return membership;
			}
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
		private StructuredTypeResourceAccessor referenceAccessor;
		private bool postponeRefreshWhileLoading;
	}
}
