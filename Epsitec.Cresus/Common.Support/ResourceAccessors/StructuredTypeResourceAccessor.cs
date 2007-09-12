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
			base.Load (manager);

			if (this.fieldAccessor == null)
			{
				this.fieldAccessor = new FieldResourceAccessor ();
			}

			this.fieldAccessor.Load (manager);

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
				StructuredType type = this.GetTypeFromData (data, caption);

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
			
			object                refBaseTypeValue   = refData.GetValue (Res.Fields.ResourceStructuredType.BaseType);
			object                refClassValue      = refData.GetValue (Res.Fields.ResourceStructuredType.Class);
			IList<StructuredData> refFields          = refData.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
			IList<StructuredData> refInterfaceIds    = refData.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;
			string                refDesignerLayouts = refData.GetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts) as string;

			object                rawBaseTypeValue   = rawData.GetValue (Res.Fields.ResourceStructuredType.BaseType);
			object                rawClassValue      = rawData.GetValue (Res.Fields.ResourceStructuredType.Class);
			IList<StructuredData> rawFields          = rawData.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
			IList<StructuredData> rawInterfaceIds    = rawData.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;
			string                rawDesignerLayouts = rawData.GetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts) as string;

			if ((!UndefinedValue.IsUndefinedValue (rawBaseTypeValue)) &&
				((UndefinedValue.IsUndefinedValue (refBaseTypeValue)) || ((Druid) refBaseTypeValue != (Druid) rawBaseTypeValue)))
			{
				patchData.SetValue (Res.Fields.ResourceStructuredType.BaseType, rawBaseTypeValue);
			}
			if (!UndefinedValue.IsUndefinedValue (rawClassValue))
			{
				System.Diagnostics.Debug.Assert ((StructuredTypeClass) refClassValue == (StructuredTypeClass) rawClassValue);
			}

			//	The structured type class must be defined, or else we won't be able
			//	to generate the correct StructuredType instance for the caption
			//	serialization.
			
			patchData.SetValue (Res.Fields.ResourceStructuredType.Class, refClassValue);

			if ((!string.IsNullOrEmpty (rawDesignerLayouts)) &&
				(refDesignerLayouts != rawDesignerLayouts))
			{
				patchData.SetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts, rawDesignerLayouts);
			}
			if ((rawFields != null) &&
				(rawFields.Count > 0) &&
				(!Types.Collection.CompareEqual (rawFields, refFields)))
			{
				patchData.SetValue (Res.Fields.ResourceStructuredType.Fields, rawFields);
			}
			if ((rawInterfaceIds != null) &&
				(rawInterfaceIds.Count > 0) &&
				(!Types.Collection.CompareEqual (rawInterfaceIds, refInterfaceIds)))
			{
				patchData.SetValue (Res.Fields.ResourceStructuredType.InterfaceIds, rawInterfaceIds);
			}
		}
		
		
		private StructuredType GetTypeFromData(StructuredData data, Caption caption)
		{
			if (UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStructuredType.Class)))
			{
				return null;
			}

			StructuredTypeClass typeClass = (StructuredTypeClass) data.GetValue (Res.Fields.ResourceStructuredType.Class);
			Druid               baseType  = StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.ResourceStructuredType.BaseType));
			string              layouts   = data.GetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts) as string;

			StructuredType type = new StructuredType (typeClass, baseType);
			type.DefineCaption (caption);
			type.SerializedDesignerLayouts = layouts;

			IList<StructuredData> interfaceIds = data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;

			if (interfaceIds != null)
			{
				foreach (StructuredData interfaceId in interfaceIds)
				{
					type.InterfaceIds.Add ((Druid) interfaceId.GetValue (Res.Fields.InterfaceId.CaptionId));
				}
			}
			
			IList<StructuredData> fieldsData = data.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
			
			int rank = 0;

			if (fieldsData != null)
			{
				type.SetValue (StructuredType.DebugDisableChecksProperty, true);

				foreach (StructuredData fieldData in fieldsData)
				{
					Druid fieldType = StructuredTypeResourceAccessor.ToDruid (fieldData.GetValue (Res.Fields.Field.TypeId));
					Druid fieldCaption = StructuredTypeResourceAccessor.ToDruid (fieldData.GetValue (Res.Fields.Field.CaptionId));
					FieldRelation relation = (FieldRelation) fieldData.GetValue (Res.Fields.Field.Relation);
					FieldMembership membership = (FieldMembership) fieldData.GetValue (Res.Fields.Field.Membership);
					FieldSource source = (FieldSource) fieldData.GetValue (Res.Fields.Field.Source);
					FieldOptions options = (FieldOptions) fieldData.GetValue (Res.Fields.Field.Options);
					string expression = fieldData.GetValue (Res.Fields.Field.Expression) as string;

					if (membership == FieldMembership.Local)
					{
						StructuredTypeField field = new StructuredTypeField (null, null, fieldCaption, rank++, relation, membership, source, options, expression);
						field.DefineTypeId (fieldType);
						type.Fields.Add (field);
					}
				}

				type.ClearValue (StructuredType.DebugDisableChecksProperty);
			}

			return type;
		}

		private void FillDataFromType(CultureMap item, StructuredData data, StructuredType type, DataCreationMode mode)
		{
			if (mode != DataCreationMode.Temporary)
			{
				System.Diagnostics.Debug.Assert ((type == null) || type.CaptionId.IsValid);
			}

			ObservableList<StructuredData> fields = data.GetValue (Res.Fields.ResourceStructuredType.Fields) as ObservableList<StructuredData>;
			ObservableList<StructuredData> interfaceIds = data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as ObservableList<StructuredData>;
			
			if (fields == null)
			{
				fields = new ObservableList<StructuredData> ();
			}
			if (interfaceIds == null)
			{
				interfaceIds = new ObservableList<StructuredData> ();
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
							Druid findId = (Druid) find.GetValue (Res.Fields.Field.CaptionId);
							return findId == field.CaptionId;
						}))
					{
						StructuredData x = new StructuredData (Res.Types.Field);

						StructuredTypeResourceAccessor.FillDataFromField (x, field);
						fields.Add (x);

						if (mode == DataCreationMode.Public)
						{
							item.NotifyDataAdded (x);
						}
					}
				}
			}

			if (UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStructuredType.Fields)))
			{
				data.SetValue (Res.Fields.ResourceStructuredType.Fields, fields);
				data.LockValue (Res.Fields.ResourceStructuredType.Fields);

				if (mode == DataCreationMode.Public)
				{
					fields.CollectionChanged += new Listener (this, item).HandleCollectionChanged;
				}
			}
			

			if (type != null)
			{
				foreach (Druid interfaceId in type.InterfaceIds)
				{
					if (!Types.Collection.Contains (interfaceIds,
						delegate (StructuredData find)
						{
							Druid findId = (Druid) find.GetValue (Res.Fields.InterfaceId.CaptionId);
							return findId == interfaceId;
						}))
					{
						StructuredData x = new StructuredData (Res.Types.InterfaceId);

						x.SetValue (Res.Fields.InterfaceId.CaptionId, interfaceId);
						interfaceIds.Add (x);

						if (mode == DataCreationMode.Public)
						{
							//	TODO: ...
//-							item.NotifyDataAdded (x);
						}
					}
				}
			}

			if (UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds)))
			{
				data.SetValue (Res.Fields.ResourceStructuredType.InterfaceIds, interfaceIds);
				data.LockValue (Res.Fields.ResourceStructuredType.InterfaceIds);

				if (mode == DataCreationMode.Public)
				{
					interfaceIds.CollectionChanged += new InterfaceListener (this, item).HandleCollectionChanged;
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

		private static void FillDataFromField(StructuredData data, StructuredTypeField field)
		{
			data.SetValue (Res.Fields.Field.TypeId, field.Type == null ? Druid.Empty : field.Type.CaptionId);
			data.SetValue (Res.Fields.Field.CaptionId, field.CaptionId);
			data.SetValue (Res.Fields.Field.Relation, field.Relation);
			data.SetValue (Res.Fields.Field.Membership, field.Membership);
			data.SetValue (Res.Fields.Field.Source, field.Source);
			data.SetValue (Res.Fields.Field.Options, field.Options);
			data.SetValue (Res.Fields.Field.Expression, field.Expression ?? "");
			data.SetValue (Res.Fields.Field.DefiningTypeId, field.DefiningTypeId);
			data.LockValue (Res.Fields.Field.DefiningTypeId);
		}

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

		protected override void RefreshItem(CultureMap item)
		{
			StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			object baseTypeIdValue = data.GetValue (Res.Fields.ResourceStructuredType.BaseType);
			Druid baseTypeId = UndefinedValue.IsUndefinedValue (baseTypeIdValue) ? Druid.Empty : (Druid) baseTypeIdValue;
			this.UpdateInheritedFields (data, baseTypeId);
			base.RefreshItem (item);
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
			IList<StructuredData>          interfaceIds = data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;

			//	We temporarily disable notifications, in order to avoid generating
			//	circular update events when just refreshing fields that are in fact
			//	read only for the user :
			
			using (fields.DisableNotifications ())
			{
				StructuredTypeResourceAccessor.RemoveInheritedFields (fields);

				if ((baseTypeId.IsValid) ||
					(interfaceIds.Count > 0))
				{
					//	Create an empty type which will be used to extract the inherited
					//	fields and those imported throught interfaces :
					
					StructuredType type = new StructuredType (StructuredTypeClass.Entity, baseTypeId);
					ResourceManager.SetResourceManager (type, this.ResourceManager);
					type.FreezeInheritance ();

					//	TODO: rewrite all this code to no longer rely on StructuredType to resolve the base type and interfaces

					foreach (StructuredData interfaceId in interfaceIds)
					{
						type.InterfaceIds.Add ((Druid) interfaceId.GetValue (Res.Fields.InterfaceId.CaptionId));
					}

					int i = 0;

					foreach (string fieldId in type.GetFieldIds ())
					{
						StructuredTypeField field = type.Fields[fieldId];

						if ((field.Membership == FieldMembership.Inherited) ||
							(field.DefiningTypeId.IsValid))
						{
							StructuredData x = new StructuredData (Res.Types.Field);
							StructuredTypeResourceAccessor.FillDataFromField (x, field);
							fields.Insert (i++, x);
						}
					}
				}
			}
		}

		/// <summary>
		/// Removes the inherited and included fields.
		/// </summary>
		/// <param name="fields">The field collection.</param>
		private static void RemoveInheritedFields(IList<StructuredData> fields)
		{
			for (int i = 0; i < fields.Count; i++)
			{
				StructuredData field = fields[i];
				FieldMembership membership = (FieldMembership) field.GetValue (Res.Fields.Field.Membership);
				Druid definingTypeId = (Druid) field.GetValue (Res.Fields.Field.DefiningTypeId);

				//	If the field is inherited from a base entity or imported through
				//	an interface, then we remove it from the collection :
				
				if ((membership == FieldMembership.Inherited) ||
					(definingTypeId.IsValid))
				{
					fields.RemoveAt (i--);
				}
			}
		}

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

		#region Listener Class

		protected class InterfaceListener
		{
			public InterfaceListener(StructuredTypeResourceAccessor accessor, CultureMap item)
			{
				this.accessor = accessor;
				this.item = item;
			}

			public void HandleCollectionChanged(object sender, CollectionChangedEventArgs e)
			{
				this.accessor.RefreshItem (this.item);
				this.accessor.NotifyItemChanged (this.item);
			}


			private StructuredTypeResourceAccessor accessor;
			private CultureMap item;
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
				data.SetValue (Res.Fields.Field.Source, FieldSource.Value);
				data.SetValue (Res.Fields.Field.Options, FieldOptions.None);
				data.SetValue (Res.Fields.Field.Expression, "");
				data.SetValue (Res.Fields.Field.DefiningTypeId, Druid.Empty);
				data.LockValue (Res.Fields.Field.DefiningTypeId);
				
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

				data.SetValue (Res.Fields.Field.CaptionId, Druid.Empty);
				
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

		private static Druid ToDruid(object value)
		{
			return UndefinedValue.IsUndefinedValue (value) ? Druid.Empty : (Druid) value;
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
	}
}
