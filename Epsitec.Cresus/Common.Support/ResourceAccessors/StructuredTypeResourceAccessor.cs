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
		public StructuredTypeResourceAccessor()
		{
		}

		public IResourceAccessor FieldAccessor
		{
			get
			{
				return this.fieldAccessor;
			}
		}

		protected override string Prefix
		{
			get
			{
				return "Typ.StructuredType.";
			}
		}

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
		
		public override IDataBroker GetDataBroker(StructuredData container, string fieldId)
		{
			if (fieldId == Res.Fields.ResourceStructuredType.Fields.ToString ())
			{
				return new FieldBroker ();
			}

			return base.GetDataBroker (container, fieldId);
		}

		public CultureMap CreateFieldItem(CultureMap item)
		{
			return this.fieldAccessor.CreateFieldItem (item.Name);
		}

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

		public override int RevertChanges()
		{
			int n = 0;

			n += base.RevertChanges ();
			n += this.FieldAccessor.RevertChanges ();

			return n;
		}

		protected override IStructuredType GetStructuredType()
		{
			return Res.Types.ResourceStructuredType;
		}

		protected override Caption GetCaptionFromData(ResourceBundle sourceBundle, Types.StructuredData data, string name, string twoLetterISOLanguageName)
		{
			Caption caption = base.GetCaptionFromData (sourceBundle, data, name, twoLetterISOLanguageName);
			
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

		protected override void FillDataFromCaption(CultureMap item, Types.StructuredData data, Caption caption)
		{
			base.FillDataFromCaption (item, data, caption);

			StructuredType type = AbstractType.GetComplexType (caption) as StructuredType;
			this.FillDataFromType (item, data, type);
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

			IList<Druid> interfaceIds = data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<Druid>;

			foreach (Druid interfaceId in interfaceIds)
			{
				type.InterfaceIds.Add (interfaceId);
			}
			
			IList<StructuredData> fieldsData = data.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
			
			int rank = 0;

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

			return type;
		}

		private void FillDataFromType(CultureMap item, StructuredData data, StructuredType type)
		{
			System.Diagnostics.Debug.Assert ((type == null) || type.CaptionId.IsValid);

			ObservableList<StructuredData> fields = new ObservableList<StructuredData> ();

			if (type != null)
			{
				foreach (string fieldId in type.GetFieldIds ())
				{
					StructuredTypeField field = type.Fields[fieldId];
					StructuredData x = new StructuredData (Res.Types.Field);

					StructuredTypeResourceAccessor.FillDataFromField (x, field);
					fields.Add (x);

					item.NotifyDataAdded (x);
				}
			}

			data.SetValue (Res.Fields.ResourceStructuredType.Fields, fields);
			data.LockValue (Res.Fields.ResourceStructuredType.Fields);
			
			ObservableList<Druid> interfaceIds = new ObservableList<Druid> ();

			if (type != null)
			{
				interfaceIds.AddRange (type.InterfaceIds);
			}

			data.SetValue (Res.Fields.ResourceStructuredType.InterfaceIds, interfaceIds);
			data.LockValue (Res.Fields.ResourceStructuredType.InterfaceIds);

			data.SetValue (Res.Fields.ResourceStructuredType.BaseType, type == null ? Druid.Empty : type.BaseTypeId);
			data.SetValue (Res.Fields.ResourceStructuredType.Class, type == null ? StructuredTypeClass.None : type.Class);
			data.SetValue (Res.Fields.ResourceStructuredType.SerializedDesignerLayouts, type == null ? "" : type.SerializedDesignerLayouts);

			interfaceIds.CollectionChanged += new InterfaceListener (this, item).HandleCollectionChanged;
			fields.CollectionChanged += new Listener (this, item).HandleCollectionChanged;
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
			IList<Druid>             interfaceIds = data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<Druid>;

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

					foreach (Druid interfaceId in interfaceIds)
					{
						type.InterfaceIds.Add (interfaceId);
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
