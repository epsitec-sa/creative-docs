//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	using CultureInfo=System.Globalization.CultureInfo;
	
	/// <summary>
	/// The <c>EntityResourceAccessor</c> is used to access entity resources,
	/// stored in the <c>Captions</c> resource bundle and which have a field
	/// name prefixed with <c>"Typ."</c>.
	/// </summary>
	public class StructuredTypeResourceAccessor : CaptionResourceAccessor
	{
		public StructuredTypeResourceAccessor()
		{
		}

		public override IDataBroker GetDataBroker(StructuredData container, string fieldId)
		{
			if (fieldId == Res.Fields.ResourceStructuredType.Fields.ToString ())
			{
				return new FieldBroker ();
			}

			return base.GetDataBroker (container, fieldId);
		}

		protected override string Prefix
		{
			get
			{
				return "Typ.StructuredType.";
			}
		}

		protected override IStructuredType GetStructuredType()
		{
			return Res.Types.ResourceStructuredType;
		}

		protected override Caption GetCaptionFromData(Types.StructuredData data, string name)
		{
			Caption     caption = base.GetCaptionFromData (data, name);
			StructuredType type = this.GetTypeFromData (data, caption);

			AbstractType.SetComplexType (caption, type);
			
			return caption;
		}

		private StructuredType GetTypeFromData(StructuredData data, Caption caption)
		{
			StructuredTypeClass typeClass = (StructuredTypeClass) data.GetValue (Res.Fields.ResourceStructuredType.Class);
			Druid               baseType  = StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.ResourceStructuredType.BaseType));
			
			StructuredType type = new StructuredType (typeClass, baseType);
			type.DefineCaption (caption);

			List<StructuredData> fieldsData = data.GetValue (Res.Fields.ResourceStructuredType.Fields) as List<StructuredData>;

			int rank = 0;

			foreach (StructuredData fieldData in fieldsData)
			{
				Druid fieldType = StructuredTypeResourceAccessor.ToDruid (fieldData.GetValue (Res.Fields.Field.Type));
				Druid fieldCaption = StructuredTypeResourceAccessor.ToDruid (fieldData.GetValue (Res.Fields.Field.Caption));
				Druid sourceFieldId = StructuredTypeResourceAccessor.ToDruid (fieldData.GetValue (Res.Fields.Field.SourceField));
				FieldRelation relation = (FieldRelation) fieldData.GetValue (Res.Fields.Field.Relation);
				FieldMembership membership = (FieldMembership) fieldData.GetValue (Res.Fields.Field.Membership);

				if (membership == FieldMembership.Local)
				{
					StructuredTypeField field = new StructuredTypeField (null, null, fieldCaption, rank++, relation, sourceFieldId.ToString (), membership);
					field.DefineTypeId (fieldType);
					type.Fields.Add (field);
				}
			}

			return type;
		}

		private static Druid ToDruid(object value)
		{
			return UndefinedValue.IsUndefinedValue (value) ? Druid.Empty : (Druid) value;
		}

		protected override void FillDataFromCaption(CultureMap item, Types.StructuredData data, Caption caption)
		{
			base.FillDataFromCaption (item, data, caption);

			StructuredType type = AbstractType.GetComplexType (caption) as StructuredType;

			if (type != null)
			{
				this.FillDataFromType (item, data, type);
			}
		}

		private void FillDataFromType(CultureMap item, StructuredData data, StructuredType type)
		{
			ObservableList<StructuredData> fields = new ObservableList<StructuredData> ();

			foreach (string fieldId in type.GetFieldIds ())
			{
				StructuredTypeField field = type.Fields[fieldId];
				StructuredData x = new StructuredData (Res.Types.Field);
				
				x.SetValue (Res.Fields.Field.Type, field.Type.CaptionId);
				x.SetValue (Res.Fields.Field.Caption, field.CaptionId);
				x.SetValue (Res.Fields.Field.Relation, field.Relation);
				x.SetValue (Res.Fields.Field.Membership, field.Membership);
				x.SetValue (Res.Fields.Field.SourceField, string.IsNullOrEmpty (field.SourceFieldId) ? Druid.Empty : Druid.Parse (field.SourceFieldId));
				fields.Add (x);
				
				item.NotifyDataAdded (x);
			}

			data.SetValue (Res.Fields.ResourceStructuredType.Fields, fields);
			data.LockValue (Res.Fields.ResourceStructuredType.Fields);

			data.SetValue (Res.Fields.ResourceStructuredType.BaseType, type.BaseTypeId);
			data.SetValue (Res.Fields.ResourceStructuredType.Class, type.Class);
			
			fields.CollectionChanged += new Listener (this, item).HandleCollectionChanged;

		}

		
		private class FieldBroker : IDataBroker
		{
			#region IDataBroker Members

			public StructuredData CreateData(CultureMap container)
			{
				return new StructuredData (Res.Types.Field);
			}

			#endregion
		}
}
}
