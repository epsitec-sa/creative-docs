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
	/// The <c>AnyTypeResourceAccessor</c> is used to access standard type
	/// resources, stored in the <c>Captions</c> resource bundle and which
	/// have a field name prefixed with <c>"Typ."</c>.
	/// </summary>
	public class AnyTypeResourceAccessor : CaptionResourceAccessor
	{
		public AnyTypeResourceAccessor()
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
				return "Typ.";
			}
		}

		protected override IStructuredType GetStructuredType()
		{
			return null;
		}

		protected override bool FilterField(ResourceBundle.Field field)
		{
			if (base.FilterField (field))
			{
				//	Filter out the structured types, as they are handled elsewhere
				//	(see the StructuredTypeResourceAccessor class).

				if (field.Name.StartsWith ("Typ.StructuredType."))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		protected override Caption GetCaptionFromData(ResourceBundle sourceBundle, Types.StructuredData data, string name, string twoLetterISOLanguageName)
		{
			Caption caption = base.GetCaptionFromData (sourceBundle, data, name, twoLetterISOLanguageName);
			
			if (twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName)
			{
				this.FillCaptionWithData (caption, data);
			}
			
			return caption;
		}

		private void FillCaptionWithData(Caption caption, StructuredData data)
		{
			if (data == null)
			{
				return;
			}

			TypeCode code = AnyTypeResourceAccessor.ToTypeCode (data.GetValue (Res.Fields.ResourceBaseType.TypeCode));

			switch (code)
			{
				case TypeCode.Binary:
					this.CreateType (new BinaryType (caption), data);
					break;

				case TypeCode.Boolean:
					this.CreateType (new BooleanType (caption), data);
					break;

				case TypeCode.Collection:
					break;
				
				case TypeCode.Date:
					this.CreateType (new DateType (caption), data);
					break;
				
				case TypeCode.DateTime:
					this.CreateType (new DateTimeType (caption), data);
					break;
				
				case TypeCode.Decimal:
					this.CreateType (new DecimalType (caption), data);
					break;

				case TypeCode.Double:
					this.CreateType (new DoubleType (caption), data);
					break;

				case TypeCode.Enum:
					break;
				
				case TypeCode.Integer:
					this.CreateType (new IntegerType (caption), data);
					break;

				case TypeCode.LongInteger:
					this.CreateType (new LongIntegerType (caption), data);
					break;

				case TypeCode.Other:
					break;
				
				case TypeCode.String:
					break;
				
				case TypeCode.Time:
					this.CreateType (new TimeType (caption), data);
					break;
			}
		}

		private void SetupType(AbstractType type, StructuredData data)
		{
			System.Diagnostics.Debug.Assert (type.TypeCode == (TypeCode) data.GetValue (Res.Fields.ResourceBaseType.TypeCode));

			object value;

			value = data.GetValue (Res.Fields.ResourceBaseType.Nullable);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				type.DefineIsNullable ((bool) value);
			}

			string controller = data.GetValue (Res.Fields.ResourceBaseType.DefaultController) as string;
			string controllerParameter = data.GetValue (Res.Fields.ResourceBaseType.DefaultControllerParameter) as string;

			if (controller != null)
			{
				type.DefineDefaultController (controller, controllerParameter);
			}
		}

		private void CreateType(BinaryType type, StructuredData data)
		{
			this.SetupType (type, data);
			
			string mimeType = data.GetValue (Res.Fields.ResourceBinaryType.MimeType) as string;

			if (mimeType != null)
			{
				type.DefineMimeType (mimeType);
			}
		}

		private void CreateType(AbstractDateTimeType type, StructuredData data)
		{
			this.SetupType (type, data);

			object value;

			value = data.GetValue (Res.Fields.ResourceDateTimeType.Resolution);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				type.DefineResolution ((TimeResolution) value);
			}

			value = data.GetValue (Res.Fields.ResourceDateTimeType.MinimumDate);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				Date date = (Date) value;

				if (!date.IsNull)
				{
					type.DefineMinimumDate (date);
				}
			}

			value = data.GetValue (Res.Fields.ResourceDateTimeType.MaximumDate);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				Date date = (Date) value;

				if (!date.IsNull)
				{
					type.DefineMaximumDate (date);
				}
			}

			value = data.GetValue (Res.Fields.ResourceDateTimeType.MinimumTime);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				Time time = (Time) value;

				if (!time.IsNull)
				{
					type.DefineMinimumTime (time);
				}
			}

			value = data.GetValue (Res.Fields.ResourceDateTimeType.MaximumTime);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				Time time = (Time) value;

				if (!time.IsNull)
				{
					type.DefineMaximumTime (time);
				}
			}

			value = data.GetValue (Res.Fields.ResourceDateTimeType.TimeStep);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				System.TimeSpan span = (System.TimeSpan) value;

				if (span != System.TimeSpan.Zero)
				{
					type.DefineTimeStep (span);
				}
			}

			value = data.GetValue (Res.Fields.ResourceDateTimeType.DateStep);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				DateSpan span = (DateSpan) value;

				if (span != DateSpan.Zero)
				{
					type.DefineDateStep (span);
				}
			}
		}

		private void CreateType(AbstractNumericType type, StructuredData data)
		{
			this.SetupType (type, data);

			object value;

			value = data.GetValue (Res.Fields.ResourceNumericType.Range);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				type.DefineRange ((DecimalRange) value);
			}

			value = data.GetValue (Res.Fields.ResourceNumericType.PreferredRange);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				type.DefinePreferredRange ((DecimalRange) value);
			}

			value = data.GetValue (Res.Fields.ResourceNumericType.SmallStep);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				type.DefineSmallStep ((decimal) value);
			}

			value = data.GetValue (Res.Fields.ResourceNumericType.LargeStep);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				type.DefineLargeStep ((decimal) value);
			}
		}

		protected static TypeCode ToTypeCode(object value)
		{
			return UndefinedValue.IsUndefinedValue (value) ? TypeCode.Invalid : (TypeCode) value;
		}

		protected static DecimalRange ToDecimalRange(object value)
		{
			return UndefinedValue.IsUndefinedValue (value) ? DecimalRange.Empty : (DecimalRange) value;
		}

		protected static System.TimeSpan ToTimeSpan(object value)
		{
			return UndefinedValue.IsUndefinedValue (value) ? System.TimeSpan.Zero : (System.TimeSpan) value;
		}

		protected static DateSpan ToDateSpan(object value)
		{
			return UndefinedValue.IsUndefinedValue (value) ? DateSpan.Zero : (DateSpan) value;
		}

		protected static Date ToDate(object value)
		{
			return UndefinedValue.IsUndefinedValue (value) ? Date.Null : (Date) value;
		}

		protected static Time ToTime(object value)
		{
			return UndefinedValue.IsUndefinedValue (value) ? Time.Null : (Time) value;
		}

		protected override void FillDataFromCaption(CultureMap item, Types.StructuredData data, Caption caption)
		{
			base.FillDataFromCaption (item, data, caption);

			AbstractType type = TypeRosetta.CreateTypeObject (caption, false);
			TypeCode     code = type == null ? TypeCode.Invalid : type.TypeCode;

			switch (code)
			{
				case TypeCode.Boolean:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Integer:
				case TypeCode.LongInteger:
					this.FillDataFromNumericType (data, type as AbstractNumericType);
					break;

				case TypeCode.Binary:
					this.FillDataFromBinaryType (data, type as BinaryType);
					break;

				case TypeCode.Collection:
					this.FillDataFromCollectionType (data, type as CollectionType);
					break;

				case TypeCode.Date:
				case TypeCode.DateTime:
				case TypeCode.Time:
					this.FillDataFromDateTimeType (data, type as AbstractDateTimeType);
					break;

				case TypeCode.Enum:
					this.FillDataFromEnumType (data, type as EnumType);
					break;

				case TypeCode.Other:
					this.FillDataFromOtherType (data, type as OtherType);
					break;

				case TypeCode.String:
					this.FillDataFromStringType (data, type as StringType);
					break;

				default:
					code = TypeCode.Invalid;
					break;
			}

			data.SetValue (Res.Fields.ResourceBaseType.TypeCode, code);
		}

		private void FillDataFromNumericType(StructuredData data, AbstractNumericType type)
		{
			data.DefineStructuredType (Res.Types.ResourceNumericType);

			data.SetValue (Res.Fields.ResourceNumericType.Range, type.Range);
			data.SetValue (Res.Fields.ResourceNumericType.PreferredRange, type.PreferredRange);
			data.SetValue (Res.Fields.ResourceNumericType.SmallStep, type.SmallStep);
			data.SetValue (Res.Fields.ResourceNumericType.LargeStep, type.LargeStep);
		}
		
		private void FillDataFromBinaryType(StructuredData data, BinaryType type)
		{
			data.DefineStructuredType (Res.Types.ResourceBinaryType);

			data.SetValue (Res.Fields.ResourceBinaryType.MimeType, type.MimeType);
		}

		private void FillDataFromCollectionType(StructuredData data, CollectionType type)
		{
			data.DefineStructuredType (Res.Types.ResourceCollectionType);

			data.SetValue (Res.Fields.ResourceCollectionType.ItemType, type.ItemType.CaptionId);
		}

		private void FillDataFromDateTimeType(StructuredData data, AbstractDateTimeType type)
		{
			data.DefineStructuredType (Res.Types.ResourceDateTimeType);

			data.SetValue (Res.Fields.ResourceDateTimeType.Resolution, type.Resolution);
			
			data.SetValue (Res.Fields.ResourceDateTimeType.MinimumDate, type.MinimumDate);
			data.SetValue (Res.Fields.ResourceDateTimeType.MaximumDate, type.MaximumDate);
			data.SetValue (Res.Fields.ResourceDateTimeType.MinimumTime, type.MinimumTime);
			data.SetValue (Res.Fields.ResourceDateTimeType.MaximumTime, type.MaximumTime);
			data.SetValue (Res.Fields.ResourceDateTimeType.DateStep, type.DateStep);
			data.SetValue (Res.Fields.ResourceDateTimeType.TimeStep, type.TimeStep);
		}

		private void FillDataFromEnumType(StructuredData data, EnumType type)
		{
			//	TODO: ...
		}

		private void FillDataFromOtherType(StructuredData data, OtherType type)
		{
			data.DefineStructuredType (Res.Types.ResourceOtherType);

			data.SetValue (Res.Fields.ResourceOtherType.SystemType, type.SystemType);
		}

		private void FillDataFromStringType(StructuredData data, StringType type)
		{
			data.DefineStructuredType (Res.Types.ResourceStringType);

			data.SetValue (Res.Fields.ResourceStringType.MinimumLength, type.MinimumLength);
			data.SetValue (Res.Fields.ResourceStringType.MaximumLength, type.MaximumLength);
			data.SetValue (Res.Fields.ResourceStringType.UseMultilingualStorage, type.UseMultilingualStorage);
		}

	}
}
