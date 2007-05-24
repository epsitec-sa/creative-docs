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
			StructuredType type = new StructuredType ();
			type.DefineCaption (caption);

			return type;
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
				fields.Add (x);
				
				item.NotifyDataAdded (x);
			}

			data.SetValue (Res.Fields.ResourceStructuredType.Fields, fields);
			data.LockValue (Res.Fields.ResourceStructuredType.Fields);
			
			fields.CollectionChanged += new Listener (this, item).HandleCollectionChanged;

		}

		protected override bool FilterField(ResourceBundle.Field field)
		{
			return base.FilterField (field);
		}


		private class FieldBroker : IDataBroker
		{
			#region IDataBroker Members

			public StructuredData CreateData(CultureMap container)
			{
				return null;
			}

			#endregion
		}
}
}
