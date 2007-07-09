//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

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
			int n = 0;
			
			n += this.FieldAccessor.PersistChanges ();
			n += base.PersistChanges ();
			
			return n;
		}

		public override int RevertChanges()
		{
			int n = 0;

			n += base.RevertChanges ();
			n += this.FieldAccessor.RevertChanges ();

			return n;
		}


		/// <summary>
		/// Refreshes the automatically generated fields (inherited from a parent
		/// or included through interfaces).
		/// </summary>
		public void RefreshFields()
		{
			foreach (CultureMap item in this.Collection)
			{
				this.RefreshFields (item);
			}
		}

		public void RefreshFields(CultureMap item)
		{
			StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			object baseTypeIdValue = data.GetValue (Res.Fields.ResourceStructuredType.BaseType);

			if ((!UndefinedValue.IsUndefinedValue (baseTypeIdValue)) &&
				(((Druid) baseTypeIdValue).IsValid))
			{
				this.UpdateInheritedFields (data, (Druid) baseTypeIdValue);
			}
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
				AbstractType.SetComplexType (caption, type);
			}
			else
			{
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
				string expression = fieldData.GetValue (Res.Fields.Field.Expression) as string;

				if (membership == FieldMembership.Local)
				{
					StructuredTypeField field = new StructuredTypeField (null, null, fieldCaption, rank++, relation, membership, source, expression);
					field.DefineTypeId (fieldType);
					type.Fields.Add (field);
				}
			}

			return type;
		}

		private void FillDataFromType(CultureMap item, StructuredData data, StructuredType type)
		{
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

		private void HandleCultureMapAdded(CultureMap item)
		{
			item.PropertyChanged += this.HandleItemPropertyChanged;
			this.RefreshFields (item);
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
		/// <c>CultureMap</c> object is added.
		/// </summary>
		/// <param name="data">The data describing the structured type.</param>
		/// <param name="newBaseType">The Druid of the base type.</param>
		private void UpdateInheritedFields(StructuredData data, Druid baseTypeId)
		{
			IList<StructuredData> fields = data.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
			IList<Druid> interfaceIds = data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<Druid>;

			StructuredTypeResourceAccessor.RemoveInheritedFields (fields);

			if (baseTypeId.IsValid)
			{
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

		private static void RemoveInheritedFields(IList<StructuredData> fields)
		{
			for (int i = 0; i < fields.Count; i++)
			{
				StructuredData field = fields[i];
				FieldMembership membership = (FieldMembership) field.GetValue (Res.Fields.Field.Membership);
				Druid definingTypeId = (Druid) field.GetValue (Res.Fields.Field.DefiningTypeId);

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
				this.accessor.RefreshFields (this.item);
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
				data.SetValue (Res.Fields.Field.Expression, "");
				data.SetValue (Res.Fields.Field.DefiningTypeId, Druid.Empty);
				data.LockValue (Res.Fields.Field.DefiningTypeId);
				
				return data;
			}

			#endregion
		}

		#endregion

		private static Druid ToDruid(object value)
		{
			return UndefinedValue.IsUndefinedValue (value) ? Druid.Empty : (Druid) value;
		}

		private FieldResourceAccessor fieldAccessor;
	}
}
