//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	/// <summary>
	/// The <c>AnyTypeResourceAccessor</c> is used to access standard type
	/// resources, stored in the <c>Captions</c> resource bundle and which
	/// have a field name prefixed with <c>"Typ."</c>.
	/// For structured types, use <see cref="StructuredTypeResourceAccessor"/>
	/// instead.
	/// </summary>
	public class AnyTypeResourceAccessor : CaptionResourceAccessor
	{
		public AnyTypeResourceAccessor()
		{
		}

		public IResourceAccessor ValueAccessor
		{
			get
			{
				return this.valueAccessor;
			}
		}

		protected override string Prefix
		{
			get
			{
				return "Typ.";
			}
		}

		public override int PersistChanges()
		{
			int n = 0;

			n += this.ValueAccessor.PersistChanges ();
			n += base.PersistChanges ();

			return n;
		}

		public override int RevertChanges()
		{
			int n = 0;

			n += base.RevertChanges ();
			n += this.ValueAccessor.RevertChanges ();

			return n;
		}

		public override void Load(ResourceManager manager)
		{
			base.Load (manager);

			if (this.valueAccessor == null)
			{
				this.valueAccessor = new ValueResourceAccessor ();
			}

			this.valueAccessor.Load (manager);
		}

		public override IDataBroker GetDataBroker(StructuredData container, string fieldId)
		{
			if (fieldId == Res.Fields.ResourceEnumType.Values.ToString ())
			{
				return new EnumValueBroker ();
			}
			
			return base.GetDataBroker (container, fieldId);
		}

		public CultureMap CreateValueItem(CultureMap item)
		{
			StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);

			System.Diagnostics.Debug.Assert (data != null);
			System.Diagnostics.Debug.Assert (AnyTypeResourceAccessor.ToTypeCode (data.GetValue (Res.Fields.ResourceBaseType.TypeCode)) == TypeCode.Enum);
			
			CultureMap valueItem = this.valueAccessor.CreateValueItem (item.Name);

			valueItem.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			
			return valueItem;
		}

		public bool CreateMissingValueItems(CultureMap item)
		{
			StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			
			IList<StructuredData> values     = data.GetValue (Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;
			System.Type           systemType = data.GetValue (Res.Fields.ResourceEnumType.SystemType) as System.Type;

			if ((systemType == null) ||
				(systemType == typeof (NotAnEnum)))
			{
				//	There is no base .NET type

				return false;
			}
			else
			{
				EnumType template = new EnumType (systemType, new Caption ());
				string   typeName = item.Name;
				bool     creation = false;
				
				foreach (EnumValue value in template.EnumValues)
				{
					if (!value.IsHidden)
					{
						string name  = value.Name;
						bool   found = false;

						foreach (PrefixedCultureMap valueItem in this.valueAccessor.Collection)
						{
							if ((valueItem.Prefix == typeName) &&
								(valueItem.Name == name))
							{
								found = true;
								break;
							}
						}
						
						if (!found)
						{
							CultureMap newValue = this.CreateValueItem (item);
							StructuredData dataValue = this.GetDataBroker (data, Res.Fields.ResourceEnumType.Values.ToString ()).CreateData (item);

							newValue.Name = name;
							this.valueAccessor.Collection.Add (newValue);
							creation = true;

							dataValue.SetValue (Res.Fields.EnumValue.CaptionId, newValue.Id);
							
							values.Add (dataValue);
						}
					}
				}

				return creation;
			}
		}

		public override void NotifyItemChanged(CultureMap item)
		{
			base.NotifyItemChanged (item);

			StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			TypeCode code = AnyTypeResourceAccessor.ToTypeCode (data.GetValue (Res.Fields.ResourceBaseType.TypeCode));
			
			if (code == TypeCode.Enum)
			{
				object value = data.GetValue (Res.Fields.ResourceEnumType.Values);

				if (UndefinedValue.IsUndefinedValue (value))
				{
					ObservableList<StructuredData> values = new ObservableList<StructuredData> ();
					
					data.SetValue (Res.Fields.ResourceEnumType.Values, values);
					data.LockValue (Res.Fields.ResourceEnumType.Values);
					
					values.CollectionChanged += new Listener (this, item).HandleCollectionChanged;
				}
			}
		}

		protected override IStructuredType GetStructuredType()
		{
			return null;
		}

		protected override bool FilterField(ResourceBundle.Field field, string fieldName)
		{
			if (base.FilterField (field, fieldName))
			{
				//	Filter out the structured types, as they are handled elsewhere
				//	(see the StructuredTypeResourceAccessor class).

				if (fieldName.StartsWith ("Typ.StructuredType."))
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

		protected override Caption CreateCaptionFromData(ResourceBundle sourceBundle, Types.StructuredData data, string name, string twoLetterISOLanguageName)
		{
			Caption caption = base.CreateCaptionFromData (sourceBundle, data, name, twoLetterISOLanguageName);
			
			if (twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName)
			{
				this.FillCaptionWithData (caption, data);
			}
			
			return caption;
		}

		protected override void FillDataFromCaption(CultureMap item, Types.StructuredData data, Caption caption, DataCreationMode mode)
		{
			base.FillDataFromCaption (item, data, caption, mode);

			//	TODO: pass on the mode to the misc. FillDataFrom... methods

			AbstractType type = TypeRosetta.CreateTypeObject (caption, false);
			TypeCode code = type == null ? TypeCode.Invalid : type.TypeCode;

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
					this.FillDataFromEnumType (item, data, type as EnumType);
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
			if ((base.IsEmptyCaption (data)) &&
				(ResourceBundle.Field.IsNullString (data.GetValue (Res.Fields.ResourceBaseType.DefaultController) as string)) &&
				(ResourceBundle.Field.IsNullString (data.GetValue (Res.Fields.ResourceBaseType.DefaultControllerParameter) as string)))
			{
				TypeCode code = AnyTypeResourceAccessor.ToTypeCode (data.GetValue (Res.Fields.ResourceBaseType.TypeCode));

				switch (code)
				{
					case TypeCode.Invalid:
						return true;
					
					case TypeCode.Binary:
						if (ResourceBundle.Field.IsNullString (data.GetValue (Res.Fields.ResourceBinaryType.MimeType) as string))
						{
							return true;
						}
						break;

					case TypeCode.Boolean:
					case TypeCode.Decimal:
					case TypeCode.Double:
					case TypeCode.Integer:
					case TypeCode.LongInteger:
						if ((UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceNumericType.Range))) &&
							(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceNumericType.PreferredRange))) &&
							(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceNumericType.SmallStep))) &&
							(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceNumericType.LargeStep))))
						{
							return true;
						}

						break;
					
					case TypeCode.Collection:
						if ((UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceCollectionType.ItemType))) ||
							(((Druid) data.GetValue (Res.Fields.ResourceCollectionType.ItemType)).IsEmpty))
						{
							return true;
						}
						break;

					case TypeCode.Date:
					case TypeCode.DateTime:
					case TypeCode.Time:
						if ((UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceDateTimeType.Resolution))) &&
							(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceDateTimeType.MinimumDate))) &&
							(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceDateTimeType.MaximumDate))) &&
							(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceDateTimeType.MinimumTime))) &&
							(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceDateTimeType.MaximumTime))) &&
							(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceDateTimeType.DateStep))) &&
							(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceDateTimeType.TimeStep))))
						{
							return true;
						}
						break;

					case TypeCode.Enum:
						IList<StructuredData> values = data.GetValue (Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;
						System.Type   enumSystemType = data.GetValue (Res.Fields.ResourceEnumType.SystemType) as System.Type;

						if (((values == null) || (values.Count == 0)) &&
							((enumSystemType == null) || (enumSystemType == typeof (NotAnEnum))))
						{
							return true;
						}
						break;


					case TypeCode.Other:
						if ((data.GetValue (Res.Fields.ResourceOtherType.SystemType) as System.Type) == null)
						{
							return true;
						}
						break;

					case TypeCode.String:
						object useMultilingualStorage = data.GetValue (Res.Fields.ResourceStringType.UseMultilingualStorage);

						if ((UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStringType.MinimumLength))) &&
							(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStringType.MaximumLength))) &&
							((UndefinedValue.IsUndefinedValue (useMultilingualStorage)) || ((bool) useMultilingualStorage == false)))
						{
							return true;
						}
						break;

					default:
						throw new System.NotSupportedException (string.Format ("Type code '{0}' not supported", code));
				}
			}
			
			return false;
		}

		protected override void ComputeDataDelta(StructuredData rawData, StructuredData refData, StructuredData patchData)
		{
			base.ComputeDataDelta (rawData, refData, patchData);

			TypeCode refCode = AnyTypeResourceAccessor.ToTypeCode (refData.GetValue (Res.Fields.ResourceBaseType.TypeCode));
			TypeCode rawCode = AnyTypeResourceAccessor.ToTypeCode (rawData.GetValue (Res.Fields.ResourceBaseType.TypeCode));

			if (refCode != rawCode)
			{
				throw new System.InvalidOperationException (string.Format ("Mismatched type codes '{0}' and '{1}'", refCode, rawCode));
			}

			//	We must always set the type code in the resulting patch data
			//	record, since otherwise, it would be impossible to determine
			//	for what type the record stands for :
			
			patchData.SetValue (Res.Fields.ResourceBaseType.TypeCode, rawCode);


			string refDefaultController          = refData.GetValue (Res.Fields.ResourceBaseType.DefaultController) as string;
			string refDefaultControllerParameter = refData.GetValue (Res.Fields.ResourceBaseType.DefaultControllerParameter) as string;

			string rawDefaultController          = rawData.GetValue (Res.Fields.ResourceBaseType.DefaultController) as string;
			string rawDefaultControllerParameter = rawData.GetValue (Res.Fields.ResourceBaseType.DefaultControllerParameter) as string;

			if ((!ResourceBundle.Field.IsNullString (rawDefaultController)) &&
				(refDefaultController != rawDefaultController))
			{
				patchData.SetValue (Res.Fields.ResourceBaseType.DefaultController, rawDefaultController);
			}
			if ((!ResourceBundle.Field.IsNullString (rawDefaultControllerParameter)) &&
				(refDefaultControllerParameter != rawDefaultControllerParameter))
			{
				patchData.SetValue (Res.Fields.ResourceBaseType.DefaultControllerParameter, rawDefaultControllerParameter);
			}


			switch (rawCode)
			{
				case TypeCode.Invalid:
					break;

				case TypeCode.Binary:

					string refMimeType = refData.GetValue (Res.Fields.ResourceBinaryType.MimeType) as string;
					string rawMimeType = rawData.GetValue (Res.Fields.ResourceBinaryType.MimeType) as string;

					if ((!ResourceBundle.Field.IsNullString (rawMimeType)) &&
						(refMimeType != rawMimeType))
					{
						patchData.SetValue (Res.Fields.ResourceBinaryType.MimeType, rawMimeType);
					}
					break;

				case TypeCode.Boolean:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Integer:
				case TypeCode.LongInteger:

					object refRangeValue          = refData.GetValue (Res.Fields.ResourceNumericType.Range);
					object refPreferredRangeValue = refData.GetValue (Res.Fields.ResourceNumericType.PreferredRange);
					object refSmallStepValue      = refData.GetValue (Res.Fields.ResourceNumericType.SmallStep);
					object refLargeStepValue      = refData.GetValue (Res.Fields.ResourceNumericType.LargeStep);
					
					object rawRangeValue          = rawData.GetValue (Res.Fields.ResourceNumericType.Range);
					object rawPreferredRangeValue = rawData.GetValue (Res.Fields.ResourceNumericType.PreferredRange);
					object rawSmallStepValue      = rawData.GetValue (Res.Fields.ResourceNumericType.SmallStep);
					object rawLargeStepValue      = rawData.GetValue (Res.Fields.ResourceNumericType.LargeStep);

					if ((!UndefinedValue.IsUndefinedValue (rawRangeValue)) &&
						((UndefinedValue.IsUndefinedValue (refRangeValue)) || ((DecimalRange) refRangeValue != (DecimalRange) rawRangeValue)))
					{
						patchData.SetValue (Res.Fields.ResourceNumericType.Range, rawRangeValue);
					}
					if ((!UndefinedValue.IsUndefinedValue (rawPreferredRangeValue)) &&
						((UndefinedValue.IsUndefinedValue (refPreferredRangeValue)) || ((DecimalRange) refPreferredRangeValue != (DecimalRange) rawPreferredRangeValue)))
					{
						patchData.SetValue (Res.Fields.ResourceNumericType.PreferredRange, rawPreferredRangeValue);
					}

					if ((!UndefinedValue.IsUndefinedValue (rawSmallStepValue)) &&
						((UndefinedValue.IsUndefinedValue (refSmallStepValue)) || ((decimal) refSmallStepValue != (decimal) rawSmallStepValue)))
					{
						patchData.SetValue (Res.Fields.ResourceNumericType.SmallStep, rawSmallStepValue);
					}
					if ((!UndefinedValue.IsUndefinedValue (rawLargeStepValue)) &&
						((UndefinedValue.IsUndefinedValue (refLargeStepValue)) || ((decimal) refLargeStepValue != (decimal) rawLargeStepValue)))
					{
						patchData.SetValue (Res.Fields.ResourceNumericType.LargeStep, rawLargeStepValue);
					}

					break;

				case TypeCode.Collection:

					object refItemTypeValue = refData.GetValue (Res.Fields.ResourceCollectionType.ItemType);
					object rawItemTypeValue = rawData.GetValue (Res.Fields.ResourceCollectionType.ItemType);

					if ((!UndefinedValue.IsUndefinedValue (rawItemTypeValue)) &&
						(((Druid) rawItemTypeValue).IsValid) &&
						((UndefinedValue.IsUndefinedValue (refItemTypeValue)) || ((Druid) refItemTypeValue != (Druid) rawItemTypeValue)))
					{
						patchData.SetValue (Res.Fields.ResourceCollectionType.ItemType, rawItemTypeValue);
					}
					break;

				case TypeCode.Date:
				case TypeCode.DateTime:
				case TypeCode.Time:

					object refResolutionValue  = refData.GetValue (Res.Fields.ResourceDateTimeType.Resolution);
					object refMinimumDateValue = refData.GetValue (Res.Fields.ResourceDateTimeType.MinimumDate);
					object refMaximumDateValue = refData.GetValue (Res.Fields.ResourceDateTimeType.MaximumDate);
					object refMinimumTimeValue = refData.GetValue (Res.Fields.ResourceDateTimeType.MinimumTime);
					object refMaximumTimeValue = refData.GetValue (Res.Fields.ResourceDateTimeType.MaximumTime);
					object refDateStepValue    = refData.GetValue (Res.Fields.ResourceDateTimeType.DateStep);
					object refTimeStepValue    = refData.GetValue (Res.Fields.ResourceDateTimeType.TimeStep);

					object rawResolutionValue  = rawData.GetValue (Res.Fields.ResourceDateTimeType.Resolution);
					object rawMinimumDateValue = rawData.GetValue (Res.Fields.ResourceDateTimeType.MinimumDate);
					object rawMaximumDateValue = rawData.GetValue (Res.Fields.ResourceDateTimeType.MaximumDate);
					object rawMinimumTimeValue = rawData.GetValue (Res.Fields.ResourceDateTimeType.MinimumTime);
					object rawMaximumTimeValue = rawData.GetValue (Res.Fields.ResourceDateTimeType.MaximumTime);
					object rawDateStepValue    = rawData.GetValue (Res.Fields.ResourceDateTimeType.DateStep);
					object rawTimeStepValue    = rawData.GetValue (Res.Fields.ResourceDateTimeType.TimeStep);

					if ((!UndefinedValue.IsUndefinedValue (rawResolutionValue)) &&
						((UndefinedValue.IsUndefinedValue (refResolutionValue)) || ((TimeResolution) refResolutionValue != (TimeResolution) rawResolutionValue)))
					{
						patchData.SetValue (Res.Fields.ResourceDateTimeType.Resolution, rawResolutionValue);
					}
					if ((!UndefinedValue.IsUndefinedValue (rawMinimumDateValue)) &&
						((UndefinedValue.IsUndefinedValue (refMinimumDateValue)) || ((Date) refMinimumDateValue != (Date) rawMinimumDateValue)))
					{
						patchData.SetValue (Res.Fields.ResourceDateTimeType.MinimumDate, rawMinimumDateValue);
					}
					if ((!UndefinedValue.IsUndefinedValue (rawMaximumDateValue)) &&
						((UndefinedValue.IsUndefinedValue (refMaximumDateValue)) || ((Date) refMaximumDateValue != (Date) rawMaximumDateValue)))
					{
						patchData.SetValue (Res.Fields.ResourceDateTimeType.MaximumDate, rawMaximumDateValue);
					}
					if ((!UndefinedValue.IsUndefinedValue (rawMinimumTimeValue)) &&
						((UndefinedValue.IsUndefinedValue (refMinimumTimeValue)) || ((Time) refMinimumTimeValue != (Time) rawMinimumTimeValue)))
					{
						patchData.SetValue (Res.Fields.ResourceDateTimeType.MinimumTime, rawMinimumTimeValue);
					}
					if ((!UndefinedValue.IsUndefinedValue (rawMaximumTimeValue)) &&
						((UndefinedValue.IsUndefinedValue (refMaximumTimeValue)) || ((Time) refMaximumTimeValue != (Time) rawMaximumTimeValue)))
					{
						patchData.SetValue (Res.Fields.ResourceDateTimeType.MaximumTime, rawMaximumTimeValue);
					}
					if ((!UndefinedValue.IsUndefinedValue (rawDateStepValue)) &&
						((UndefinedValue.IsUndefinedValue (refDateStepValue)) || ((DateSpan) refDateStepValue != (DateSpan) rawDateStepValue)))
					{
						patchData.SetValue (Res.Fields.ResourceDateTimeType.DateStep, rawDateStepValue);
					}
					if ((!UndefinedValue.IsUndefinedValue (rawTimeStepValue)) &&
						((UndefinedValue.IsUndefinedValue (refTimeStepValue)) || ((System.TimeSpan) refTimeStepValue != (System.TimeSpan) rawTimeStepValue)))
					{
						patchData.SetValue (Res.Fields.ResourceDateTimeType.TimeStep, rawTimeStepValue);
					}
					
					break;

				case TypeCode.Enum:
					IList<StructuredData> refValues = refData.GetValue (Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;
					System.Type   refEnumSystemType = refData.GetValue (Res.Fields.ResourceEnumType.SystemType) as System.Type;

					IList<StructuredData> rawValues = rawData.GetValue (Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;
					System.Type   rawEnumSystemType = rawData.GetValue (Res.Fields.ResourceEnumType.SystemType) as System.Type;
					
					//	TODO: ...merge lists...

					if ((rawEnumSystemType != null) &&
						((refEnumSystemType == null) || (refEnumSystemType.FullName != rawEnumSystemType.FullName)))
					{
						patchData.SetValue (Res.Fields.ResourceEnumType.SystemType, rawEnumSystemType);
					}
					
					break;


				case TypeCode.Other:

					System.Type refOtherSystemType = refData.GetValue (Res.Fields.ResourceOtherType.SystemType) as System.Type;
					System.Type rawOtherSystemType = rawData.GetValue (Res.Fields.ResourceOtherType.SystemType) as System.Type;

					if ((rawOtherSystemType != null) &&
						((refOtherSystemType == null) || (refOtherSystemType.FullName != rawOtherSystemType.FullName)))
					{
						patchData.SetValue (Res.Fields.ResourceOtherType.SystemType, rawOtherSystemType);
					}
					break;

				case TypeCode.String:

					object refUseMultilingualStorageValue = refData.GetValue (Res.Fields.ResourceStringType.UseMultilingualStorage);
					object refMinimumLengthValue          = refData.GetValue (Res.Fields.ResourceStringType.MinimumLength);
					object refMaximumLengthValue          = refData.GetValue (Res.Fields.ResourceStringType.MaximumLength);
					
					object rawUseMultilingualStorageValue = rawData.GetValue (Res.Fields.ResourceStringType.UseMultilingualStorage);
					object rawMinimumLengthValue          = rawData.GetValue (Res.Fields.ResourceStringType.MinimumLength);
					object rawMaximumLengthValue          = rawData.GetValue (Res.Fields.ResourceStringType.MaximumLength);

					if ((!UndefinedValue.IsUndefinedValue (rawUseMultilingualStorageValue)) &&
						((UndefinedValue.IsUndefinedValue (refUseMultilingualStorageValue)) || ((bool) refUseMultilingualStorageValue != (bool) rawUseMultilingualStorageValue)))
					{
						patchData.SetValue (Res.Fields.ResourceStringType.UseMultilingualStorage, rawUseMultilingualStorageValue);
					}
					if ((!UndefinedValue.IsUndefinedValue (rawMinimumLengthValue)) &&
						((UndefinedValue.IsUndefinedValue (refMinimumLengthValue)) || ((int) refMinimumLengthValue != (int) rawMinimumLengthValue)))
					{
						patchData.SetValue (Res.Fields.ResourceStringType.MinimumLength, rawMinimumLengthValue);
					}
					if ((!UndefinedValue.IsUndefinedValue (rawMaximumLengthValue)) &&
						((UndefinedValue.IsUndefinedValue (refMaximumLengthValue)) || ((int) refMaximumLengthValue != (int) rawMaximumLengthValue)))
					{
						patchData.SetValue (Res.Fields.ResourceStringType.MaximumLength, rawMaximumLengthValue);
					}
					break;

				default:
					throw new System.NotSupportedException (string.Format ("Type code '{0}' not supported", rawCode));
			}
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
					this.CreateType (new CollectionType (caption), data);
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
					this.CreateEnumType (caption, data);
					break;
				
				case TypeCode.Integer:
					this.CreateType (new IntegerType (caption), data);
					break;

				case TypeCode.LongInteger:
					this.CreateType (new LongIntegerType (caption), data);
					break;

				case TypeCode.Other:
					this.CreateType (new OtherType (caption), data);
					break;
				
				case TypeCode.String:
					this.CreateType (new StringType (caption), data);
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

		private void CreateType(CollectionType type, StructuredData data)
		{
			this.SetupType (type, data);

			object value = data.GetValue (Res.Fields.ResourceCollectionType.ItemType);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				Druid id = (Druid) value;

				if (id.IsValid)
				{
					type.DefineItemType (new DummyNamedType (id));
				}
			}
		}

		private void CreateEnumType(Caption caption, StructuredData data)
		{
			System.Type sysType = data.GetValue (Res.Fields.ResourceEnumType.SystemType) as System.Type;
			EnumType type = new EnumType (sysType, caption);
			
			this.SetupType (type, data);

			IList<StructuredData> values = data.GetValue (Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;
			int rank = 0;
			
			type.MakeEditable ();

			foreach (StructuredData value in values)
			{
				Druid id = (Druid) value.GetValue (Res.Fields.EnumValue.CaptionId);

				type.EnumValues.Add (new EnumValue (rank++, id));
			}
		}

		private void CreateType(OtherType type, StructuredData data)
		{
			this.SetupType (type, data);

			System.Type sysType = data.GetValue (Res.Fields.ResourceOtherType.SystemType) as System.Type;

			if (sysType != null)
			{
				type.DefineSystemType (sysType);
			}
		}

		private void CreateType(StringType type, StructuredData data)
		{
			this.SetupType (type, data);

			object value;
			
			value = data.GetValue (Res.Fields.ResourceStringType.UseMultilingualStorage);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				type.DefineUseMultilingualStorage ((bool) value);
			}

			value = data.GetValue (Res.Fields.ResourceStringType.MinimumLength);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				int min = (int) value;

				if (min > 0)
				{
					type.DefineMinimumLength (min);
				}
			}

			value = data.GetValue (Res.Fields.ResourceStringType.MaximumLength);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				int max = (int) value;

				if (max > 0)
				{
					type.DefineMaximumLength (max);
				}
			}
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

		private void FillDataFromEnumType(CultureMap item, StructuredData data, EnumType type)
		{
			data.DefineStructuredType (Res.Types.ResourceEnumType);
			
			ObservableList<StructuredData> values = new ObservableList<StructuredData> ();

			foreach (EnumValue value in type.Values)
			{
				if (value.IsHidden)
				{
					continue;
				}

				StructuredData x = new StructuredData (Res.Types.EnumValue);

				x.SetValue (Res.Fields.EnumValue.CaptionId, value.CaptionId);
				values.Add (x);

				item.NotifyDataAdded (x);
			}

			data.SetValue (Res.Fields.ResourceEnumType.Values, values);
			data.LockValue (Res.Fields.ResourceEnumType.Values);

			values.CollectionChanged += new Listener (this, item).HandleCollectionChanged;

			data.SetValue (Res.Fields.ResourceEnumType.SystemType, type.SystemType);
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
			System.Diagnostics.Debug.Assert (item.IsCultureDefined (Resources.DefaultTwoLetterISOLanguageName));

			StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			TypeCode       code = AnyTypeResourceAccessor.ToTypeCode (data.GetValue (Res.Fields.ResourceBaseType.TypeCode));

			if (code == TypeCode.Invalid)
			{
				throw new System.ArgumentException ("Item has no valid TypeCode defined");
			}

			if (code == TypeCode.Enum)
			{
				item.PropertyChanged += this.HandleItemPropertyChanged;
			}
		}

		private void HandleCultureMapRemoved(CultureMap item)
		{
			System.Diagnostics.Debug.Assert (item.IsCultureDefined (Resources.DefaultTwoLetterISOLanguageName));
			
			StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			TypeCode code = AnyTypeResourceAccessor.ToTypeCode (data.GetValue (Res.Fields.ResourceBaseType.TypeCode));

			if (code == TypeCode.Invalid)
			{
				throw new System.ArgumentException ("Item has no valid TypeCode defined");
			}

			if (code == TypeCode.Enum)
			{
				item.PropertyChanged -= this.HandleItemPropertyChanged;
			}
		}

		private void HandleItemPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Name")
			{
				string oldName = e.OldValue as string;
				string newName = e.NewValue as string;

				this.ChangeFieldPrefix (oldName, newName);
			}
		}
		
		private void ChangeFieldPrefix(string oldName, string newName)
		{
			ResourceBundle bundle = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default);

			string oldPrefix = string.Concat ("Val.", oldName, ".");
			string newPrefix = string.Concat ("Val.", newName, ".");

			foreach (ResourceBundle.Field field in bundle.Fields)
			{
				if (field.Name.StartsWith (oldPrefix))
				{
					field.SetName (newPrefix + field.Name.Substring (oldPrefix.Length));
				}
			}

			foreach (PrefixedCultureMap item in this.valueAccessor.Collection)
			{
				if (item.Prefix == oldName)
				{
					item.Prefix = newName;
				}
			}
		}

		#region DummyNamedType Class

		/// <summary>
		/// The <c>DummyNamedType</c> class is needed to define the <c>ItemType</c>
		/// of a collection type description; without it, we really would have to
		/// get access to the type described by the DRUID. I prefer not to.
		/// </summary>
		private class DummyNamedType : INamedType
		{
			public DummyNamedType(Druid id)
			{
				this.id = id;
			}

			#region INamedType Members

			public string DefaultController
			{
				get
				{
					return null;
				}
			}

			public string DefaultControllerParameter
			{
				get
				{
					return null;
				}
			}

			#endregion

			#region ICaption Members

			public Druid CaptionId
			{
				get
				{
					return this.id;
				}
			}

			#endregion

			#region IName Members

			public string Name
			{
				get
				{
					return null;
				}
			}

			#endregion

			#region ISystemType Members

			public System.Type SystemType
			{
				get
				{
					return null;
				}
			}

			#endregion

			Druid id;
		}

		#endregion

		#region EnumValueBroker Class

		private class EnumValueBroker : IDataBroker
		{
			#region IDataBroker Members

			public StructuredData CreateData(CultureMap container)
			{
				StructuredData data = new StructuredData (Res.Types.EnumValue);

				data.SetValue (Res.Fields.EnumValue.CaptionId, Druid.Empty);

				return data;
			}

			#endregion
		}

		#endregion

		private static TypeCode ToTypeCode(object value)
		{
			return UndefinedValue.IsUndefinedValue (value) ? TypeCode.Invalid : (TypeCode) value;
		}

		private ValueResourceAccessor valueAccessor;
	}
}
