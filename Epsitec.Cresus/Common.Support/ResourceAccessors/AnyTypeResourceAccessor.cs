	//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		/// <summary>
		/// Initializes a new instance of the <see cref="AnyTypeResourceAccessor"/> class.
		/// </summary>
		public AnyTypeResourceAccessor()
		{
		}

		/// <summary>
		/// Gets the value accessor associated with this accessor.
		/// </summary>
		/// <value>The <see cref="ValueResourceAccessor"/>.</value>
		public IResourceAccessor ValueAccessor
		{
			get
			{
				return this.valueAccessor;
			}
		}

		/// <summary>
		/// Gets the caption prefix for this accessor.
		/// Note: several resource types are stored as captions; the prefix of
		/// the field name is used to differentiate them.
		/// </summary>
		/// <value>The caption <c>"Typ."</c> prefix.</value>
		protected override string Prefix
		{
			get
			{
				return "Typ.";
			}
		}

		/// <summary>
		/// Persists the changes to the underlying data store.
		/// </summary>
		/// <returns>
		/// The number of items which have been persisted.
		/// </returns>
		public override int PersistChanges()
		{
			int n = 0;

			n += this.valueAccessor.PersistChanges ();
			n += base.PersistChanges ();

			return n;
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
			n += this.valueAccessor.RevertChanges ();

			return n;
		}

		/// <summary>
		/// Loads resources from the specified resource manager. The resource
		/// manager will be used for all upcoming accesses.
		/// </summary>
		/// <param name="manager">The resource manager.</param>
		public override void Load(ResourceManager manager)
		{
			base.Load (manager);

			if (this.valueAccessor == null)
			{
				this.valueAccessor = new ValueResourceAccessor ();
			}

			this.valueAccessor.Load (manager);
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
			//	Return the broker for the enum value collection, if this is the
			//	field passed in by the caller :

			if (fieldId == Res.Fields.ResourceEnumType.Values.ToString ())
			{
				return new EnumValueBroker (this);
			}
			
			return base.GetDataBroker (container, fieldId);
		}

		/// <summary>
		/// Creates an item which can be stored in the <see cref="ValueResourceAccessor"/>'s
		/// collection.
		/// </summary>
		/// <param name="item">The item (describing an enum type).</param>
		/// <returns>An empty item for the <see cref="ValueResourceAccessor"/>.</returns>
		public CultureMap CreateValueItem(CultureMap item)
		{
			StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);

			System.Diagnostics.Debug.Assert (data != null);
			System.Diagnostics.Debug.Assert (AnyTypeResourceAccessor.ToTypeCode (data.GetValue (Res.Fields.ResourceBaseType.TypeCode)) == TypeCode.Enum);
			
			CultureMap valueItem = this.valueAccessor.CreateValueItem (item.Name);

			valueItem.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			
			return valueItem;
		}

		/// <summary>
		/// Creates the missing value items for a given enum type. This will
		/// create items in the <see cref="ValueResourceAccessor"/> if the
		/// enum type item describes a native C# (or .NET) enumeration.
		/// </summary>
		/// <param name="item">The enum type item.</param>
		/// <returns><c>true</c> if value items were created; otherwise, <c>false</c>.</returns>
		public bool CreateMissingValueItems(CultureMap item)
		{
			StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);

			TypeCode code = AnyTypeResourceAccessor.ToTypeCode (data.GetValue (Res.Fields.ResourceBaseType.TypeCode));

			System.Diagnostics.Debug.Assert (code == TypeCode.Enum);
			
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
				//	Create a temporary instance of the EnumType and then walk
				//	through all the defined values, checking that they exist
				//	in the value accessor's collection...

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
							CultureMap     newValue  = this.CreateValueItem (item);
							StructuredData dataValue = null;
							bool           insert    = false;

							//	Try to reuse an empty value (one which has no DRUID associated
							//	to it yet) in the list; there should always be one, because of
							//	how the EnumType class is initialized by the Caption deseriali-
							//	zation.

							foreach (StructuredData v in values)
							{
								if (StructuredTypeResourceAccessor.ToDruid (v.GetValue (Res.Fields.EnumValue.CaptionId)).IsEmpty)
								{
									dataValue = v;
									break;
								}
							}

//-							System.Diagnostics.Debug.Assert (dataValue != null);
							
							if (dataValue == null)
							{
								//	This should never happen... but I decided to keep the code
								//	nevertheless, just to make sure that if the assertion fails,
								//	we can safely continue.

								dataValue = this.GetDataBroker (data, Res.Fields.ResourceEnumType.Values.ToString ()).CreateData (item);
								insert    = true;
							}

							newValue.Name = name;
							this.valueAccessor.Collection.Add (newValue);
							creation = true;

							dataValue.SetValue (Res.Fields.EnumValue.CaptionId, newValue.Id);
							dataValue.SetValue (Res.Fields.EnumValue.CultureMapSource, item.Source);

							if (insert)
							{
								values.Add (dataValue);
							}
						}
					}
				}

				return creation;
			}
		}

		/// <summary>
		/// Notifies the resource accessor that the specified item changed.
		/// </summary>
		/// <param name="item">The item which was modified.</param>
		public override void NotifyItemChanged(CultureMap item, StructuredData container, DependencyPropertyChangedEventArgs e)
		{
			base.NotifyItemChanged (item, container, e);

			StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			TypeCode code = AnyTypeResourceAccessor.ToTypeCode (data.GetValue (Res.Fields.ResourceBaseType.TypeCode));
			
			if (code == TypeCode.Enum)
			{
				//	The user somehow edited a type enum item. If this item has
				//	no associated list of values, we use the opportunity to create
				//	it here :

				object value = data.GetValue (Res.Fields.ResourceEnumType.Values);

				if (UndefinedValue.IsUndefinedValue (value))
				{
					ObservableList<StructuredData> values = new ObservableList<StructuredData> ();
					
					data.SetValue (Res.Fields.ResourceEnumType.Values, values);
					data.LockValue (Res.Fields.ResourceEnumType.Values);

					EnumValueListener listener = new EnumValueListener (this, item, data);

					values.CollectionChanging += listener.HandleCollectionChanging;
					values.CollectionChanged  += listener.HandleCollectionChanged;
				}
			}
		}

		/// <summary>
		/// Gets the structured type which describes the caption data.
		/// </summary>
		/// <returns>
		/// Always <c>null</c>. The <c>StructuredType</c> will be filled in later
		/// by method <c>FillDataFromCaption</c>.
		/// </returns>
		protected override IStructuredType GetStructuredType()
		{
			return null;
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

		/// <summary>
		/// Resets the specified field to its original value. This is the
		/// internal implementation which can be overridden.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="container">The data record.</param>
		/// <param name="fieldId">The field id.</param>
		protected override void ResetToOriginal(CultureMap item, StructuredData container, Druid fieldId)
		{
			if (fieldId == Res.Fields.ResourceEnumType.Values)
			{
				EnumValueListener listener = Listener.FindListener<EnumValueListener> (container, fieldId);

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

		protected override void PromoteToOriginal(CultureMap item, StructuredData data)
		{
			base.PromoteToOriginal (item, data);

			ObservableList<StructuredData> enumValues = data.GetValue (Res.Fields.ResourceEnumType.Values) as ObservableList<StructuredData>;

			if (enumValues != null)
			{
				using (enumValues.DisableNotifications ())
				{
					foreach (StructuredData enumValue in enumValues)
					{
						enumValue.PromoteToOriginal ();
					}
				}

				Listener.FindListener<EnumValueListener> (enumValues).HandleCollectionChanging (enumValues);
			}
		}


		/// <summary>
		/// Gets the type based on the data stored in the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <param name="namedTypeResolver">The named type resolver which knows how to resolve an id to a type.</param>
		/// <returns>The type or <c>null</c>.</returns>
		public INamedType GetAnyTypeViewOfData(CultureMap item, string twoLetterISOLanguageName, System.Func<Druid, INamedType> namedTypeResolver)
		{
			if (this.ContainsCaption (twoLetterISOLanguageName) == false)
			{
				twoLetterISOLanguageName = Resources.DefaultTwoLetterISOLanguageName;
			}

			StructuredData data = item.GetCultureData (twoLetterISOLanguageName);
			Caption caption = base.CreateCaptionFromData (item.Id, null, data, item.Name, twoLetterISOLanguageName);
			this.FillCaptionWithData (caption, data);
			return TypeRosetta.CreateTypeObject (caption, false);
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
				this.FillCaptionWithData (caption, data);
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

			AbstractType type = TypeRosetta.CreateTypeObject (caption, false, this.ResourceManager);
			TypeCode code = type == null ? TypeCode.Invalid : type.TypeCode;

			string controller          = type == null ? null : type.DefaultController;
			string controllerParameter = type == null ? null : type.DefaultControllerParameters;
			
			object defaultValue = type == null ? null : type.DefaultValue;

			if (!string.IsNullOrEmpty (controller))
			{
				data.SetValue (Res.Fields.ResourceBaseType.DefaultController, controller);
			}
			if (!string.IsNullOrEmpty (controllerParameter))
			{
				data.SetValue (Res.Fields.ResourceBaseType.DefaultControllerParameters, controllerParameter);
			}
			if (type != null)
			{
				data.SetValue (Res.Fields.ResourceBaseType.Nullable, type.IsNullable);
			}

			if (defaultValue != null)
			{
				data.SetValue (Res.Fields.ResourceBaseType.DefaultValue, defaultValue);
			}

			switch (code)
			{
				case TypeCode.Boolean:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Integer:
				case TypeCode.LongInteger:
					this.FillDataFromNumericType (data, caption, type as AbstractNumericType);
					break;

				case TypeCode.Binary:
					this.FillDataFromBinaryType (data, caption, type as BinaryType);
					break;

				case TypeCode.Collection:
					this.FillDataFromCollectionType (data, caption, type as CollectionType);
					break;

				case TypeCode.Date:
				case TypeCode.DateTime:
				case TypeCode.Time:
					this.FillDataFromDateTimeType (data, caption, type as AbstractDateTimeType);
					break;

				case TypeCode.Enum:
					this.FillDataFromEnumType (item, data, caption, type as EnumType, mode);
					break;

				case TypeCode.Other:
					this.FillDataFromOtherType (data, caption, type as OtherType);
					break;

				case TypeCode.String:
					this.FillDataFromStringType (data, caption, type as StringType);
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
				(ResourceBundle.Field.IsNullString (data.GetValue (Res.Fields.ResourceBaseType.DefaultControllerParameters) as string)) &&
				(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceBaseType.DefaultValue))))
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
						object useFormattedText       = data.GetValue (Res.Fields.ResourceStringType.UseFormattedText);

						if ((UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStringType.MinimumLength))) &&
							(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStringType.MaximumLength))) &&
							((UndefinedValue.IsUndefinedValue (useMultilingualStorage)) || ((bool) useMultilingualStorage == false)) &&
							((UndefinedValue.IsUndefinedValue (useFormattedText)) || ((bool) useFormattedText == false)) &&
							(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStringType.DefaultSearchBehavior))) &&
							(UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceStringType.DefaultComparisonBehavior))))
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

			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceBaseType.DefaultController);
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceBaseType.DefaultControllerParameters);
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceBaseType.Nullable);
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceBaseType.DefaultValue);

			switch (rawCode)
			{
				case TypeCode.Invalid:
					break;

				case TypeCode.Binary:
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceBinaryType.MimeType);
					break;

				case TypeCode.Boolean:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Integer:
				case TypeCode.LongInteger:
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceNumericType.Range);
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceNumericType.PreferredRange);
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceNumericType.SmallStep);
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceNumericType.LargeStep);
					break;

				case TypeCode.Collection:
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceCollectionType.ItemType);
					break;

				case TypeCode.Date:
				case TypeCode.DateTime:
				case TypeCode.Time:
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceDateTimeType.Resolution);
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceDateTimeType.MinimumDate);
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceDateTimeType.MaximumDate);
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceDateTimeType.MinimumTime);
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceDateTimeType.MaximumTime);
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceDateTimeType.DateStep);
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceDateTimeType.TimeStep);
					break;

				case TypeCode.Enum:
					IList<StructuredData> refValues = refData.GetValue (Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;
					IList<StructuredData> rawValues = rawData.GetValue (Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;
					
					if ((rawValues != null) &&
						(rawValues.Count > 0) &&
						(!Types.Collection.CompareEqual (rawValues, refValues)))
					{
						patchData.SetValue (Res.Fields.ResourceEnumType.Values, rawValues);
					}

					//AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceEnumType.Values);
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceEnumType.SystemType);
					break;

				case TypeCode.Other:
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceOtherType.SystemType);
					break;

				case TypeCode.String:
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceStringType.UseMultilingualStorage);
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceStringType.UseFormattedText);
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceStringType.MinimumLength);
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceStringType.MaximumLength);
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceStringType.DefaultSearchBehavior);
					AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceStringType.DefaultComparisonBehavior);
					break;

				default:
					throw new System.NotSupportedException (string.Format ("Type code '{0}' not supported", rawCode));
			}
		}

		/// <summary>
		/// Merges two lists of enum values.
		/// </summary>
		/// <param name="sourceA">The first enum values list.</param>
		/// <param name="sourceB">The second enum values list.</param>
		/// <returns>The resulting enum values list.</returns>
		private static IList<StructuredData> MergeEnumValues(IList<StructuredData> sourceA, IList<StructuredData> sourceB)
		{
			List<StructuredData> list = new List<StructuredData> ();
			List<StructuredData> temp = sourceA == null ? new List<StructuredData> () : new List<StructuredData> (sourceA);

			//	If the first list starts with elements not found in the second
			//	list, copy them into the resulting list :
			
			AnyTypeResourceAccessor.FillEnumValuesListFromFirstSourceNotInSecondSource (list, temp, sourceB);
			
			//	For every element found in the second list, copy it into the
			//	resulting list. Every time, check to see if the next element
			//	found in the first list is absent from the second list, and
			//	handle it with a higher priority :
			
			foreach (StructuredData data in sourceB)
			{
				Druid bId = (Druid) data.GetValue (Res.Fields.EnumValue.CaptionId);
				
				int indexBinA = Types.Collection.FindIndex (temp,
					delegate (StructuredData findData)
					{
						return ((Druid) findData.GetValue (Res.Fields.EnumValue.CaptionId)) == bId;
					});

				list.Add (data);

				if (indexBinA >= 0)
				{
					temp.RemoveAt (indexBinA);
					AnyTypeResourceAccessor.FillEnumValuesListFromFirstSourceNotInSecondSource (list, temp, sourceB);
				}
			}

			return list;
		}

		/// <summary>
		/// Fills the enum values list, taking elements from the start of the
		/// first source list, as long as they are not in the second source
		/// list. Stop as soon as the element is found in the second list.
		/// </summary>
		/// <param name="list">The list to fill.</param>
		/// <param name="sourceA">The first source list.</param>
		/// <param name="sourceB">The second source list.</param>
		private static void FillEnumValuesListFromFirstSourceNotInSecondSource(List<StructuredData> list, List<StructuredData> sourceA, IList<StructuredData> sourceB)
		{
			while (sourceA.Count > 0)
			{
				Druid aId = (Druid) sourceA[0].GetValue (Res.Fields.EnumValue.CaptionId);

				int indexAinB = Types.Collection.FindIndex (sourceB,
					delegate (StructuredData findData)
					{
						return ((Druid) findData.GetValue (Res.Fields.EnumValue.CaptionId)) == aId;
					});

				if (indexAinB >= 0)
				{
					break;
				}
				
				list.Add (sourceA[0]);
				sourceA.RemoveAt (0);
			}
		}


		/// <summary>
		/// Fills the caption with the values stored in the data record.
		/// </summary>
		/// <param name="caption">The caption to fill.</param>
		/// <param name="data">The data record.</param>
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

		#region FillCaptionWithData Support Methods

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
			string controllerParameter = data.GetValue (Res.Fields.ResourceBaseType.DefaultControllerParameters) as string;
			object defaultValue = data.GetValue (Res.Fields.ResourceBaseType.DefaultValue);

			if (controller != null)
			{
				type.DefineDefaultController (controller, controllerParameter);
			}

			if (!UndefinedValue.IsUndefinedValue (defaultValue))
			{
				type.DefineDefaultValue (defaultValue);
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

			value = data.GetValue (Res.Fields.ResourceStringType.UseFormattedText);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				type.DefineUseFormattedText ((bool) value);
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

			value = data.GetValue (Res.Fields.ResourceStringType.DefaultSearchBehavior);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				type.DefineDefaultSearchBehavior ((StringSearchBehavior) value);
			}

			value = data.GetValue (Res.Fields.ResourceStringType.DefaultComparisonBehavior);

			if (!UndefinedValue.IsUndefinedValue (value))
			{
				type.DefineDefaultComparisonBehavior ((StringComparisonBehavior) value);
			}
		}


		private void FillDataFromNumericType(StructuredData data, Caption caption, AbstractNumericType type)
		{
			data.DefineStructuredType (Res.Types.ResourceNumericType);

			if (caption.ContainsLocalValue (AbstractNumericType.RangeProperty))
			{
				data.SetValue (Res.Fields.ResourceNumericType.Range, type.Range);
			}
			if (caption.ContainsLocalValue (AbstractNumericType.PreferredRangeProperty))
			{
				data.SetValue (Res.Fields.ResourceNumericType.PreferredRange, type.PreferredRange);
			}
			if (caption.ContainsLocalValue (AbstractNumericType.SmallStepProperty))
			{
				data.SetValue (Res.Fields.ResourceNumericType.SmallStep, type.SmallStep);
			}
			if (caption.ContainsLocalValue (AbstractNumericType.LargeStepProperty))
			{
				data.SetValue (Res.Fields.ResourceNumericType.LargeStep, type.LargeStep);
			}
		}
		
		private void FillDataFromBinaryType(StructuredData data, Caption caption, BinaryType type)
		{
			data.DefineStructuredType (Res.Types.ResourceBinaryType);

			if (caption.ContainsLocalValue (BinaryType.MimeTypeProperty))
			{
				data.SetValue (Res.Fields.ResourceBinaryType.MimeType, type.MimeType);
			}
		}

		private void FillDataFromCollectionType(StructuredData data, Caption caption, CollectionType type)
		{
			data.DefineStructuredType (Res.Types.ResourceCollectionType);

			if (caption.ContainsLocalValue (CollectionType.ItemTypeProperty))
			{
				data.SetValue (Res.Fields.ResourceCollectionType.ItemType, type.ItemType.CaptionId);
			}
		}

		private void FillDataFromDateTimeType(StructuredData data, Caption caption, AbstractDateTimeType type)
		{
			data.DefineStructuredType (Res.Types.ResourceDateTimeType);

			if (caption.ContainsLocalValue (AbstractDateTimeType.ResolutionProperty))
			{
				data.SetValue (Res.Fields.ResourceDateTimeType.Resolution, type.Resolution);
			}

			if (caption.ContainsLocalValue (AbstractDateTimeType.MinimumDateProperty))
			{
				data.SetValue (Res.Fields.ResourceDateTimeType.MinimumDate, type.MinimumDate);
			}
			if (caption.ContainsLocalValue (AbstractDateTimeType.MaximumDateProperty))
			{
				data.SetValue (Res.Fields.ResourceDateTimeType.MaximumDate, type.MaximumDate);
			}
			if (caption.ContainsLocalValue (AbstractDateTimeType.MinimumTimeProperty))
			{
				data.SetValue (Res.Fields.ResourceDateTimeType.MinimumTime, type.MinimumTime);
			}
			if (caption.ContainsLocalValue (AbstractDateTimeType.MaximumTimeProperty))
			{
				data.SetValue (Res.Fields.ResourceDateTimeType.MaximumTime, type.MaximumTime);
			}
			if (caption.ContainsLocalValue (AbstractDateTimeType.DateStepProperty))
			{
				data.SetValue (Res.Fields.ResourceDateTimeType.DateStep, type.DateStep);
			}
			if (caption.ContainsLocalValue (AbstractDateTimeType.TimeStepProperty))
			{
				data.SetValue (Res.Fields.ResourceDateTimeType.TimeStep, type.TimeStep);
			}
		}

		private void FillDataFromEnumType(CultureMap item, StructuredData data, Caption caption, EnumType type, DataCreationMode mode)
		{
			data.DefineStructuredType (Res.Types.ResourceEnumType);

			ObservableList<StructuredData> values = data.GetValue (Res.Fields.ResourceEnumType.Values) as ObservableList<StructuredData>;
			IList<StructuredData> source = new List<StructuredData> ();

			foreach (EnumValue value in type.Values)
			{
				if (value.IsHidden)
				{
					continue;
				}

				StructuredData x = new StructuredData (Res.Types.EnumValue);

				x.SetValue (Res.Fields.EnumValue.CaptionId, value.CaptionId);
				x.SetValue (Res.Fields.EnumValue.CultureMapSource, item.Source);

				source.Add (x);
			}

			if (values == null)
			{
				values = new ObservableList<StructuredData> ();
			}
			else if (source.Count > 0)
			{
				System.Diagnostics.Debug.Assert (item.Source != CultureMapSource.ReferenceModule);

				foreach (StructuredData value in values)
				{
					System.Diagnostics.Debug.Assert (!UndefinedValue.IsUndefinedValue (value.GetValue (Res.Fields.EnumValue.CaptionId)));
					System.Diagnostics.Debug.Assert (!UnknownValue.IsUnknownValue (value.GetValue (Res.Fields.EnumValue.CaptionId)));
					System.Diagnostics.Debug.Assert (!value.IsValueLocked (Res.Fields.EnumValue.CaptionId));

//-					value.LockValue (Res.Fields.EnumValue.CaptionId);
				}

				source = AnyTypeResourceAccessor.MergeEnumValues (values, source);

				foreach (StructuredData value in values)
				{
					StructuredData find;
					Druid valueId = StructuredTypeResourceAccessor.ToDruid (value.GetValue (Res.Fields.EnumValue.CaptionId));

					if (Types.Collection.TryFind (source,
						/**/					  delegate (StructuredData candidate)
												  {
													  return StructuredTypeResourceAccessor.ToDruid (candidate.GetValue (Res.Fields.EnumValue.CaptionId)) == valueId;
												  },
						/**/					  out find))
					{
						find.SetValue (Res.Fields.EnumValue.CultureMapSource, CultureMapSource.ReferenceModule);
					}
				}

				values.Clear ();
			}

			if (source.Count > 0)
			{
				foreach (StructuredData value in source)
				{
					StructuredData x = new StructuredData (Res.Types.EnumValue);

					x.SetValue (Res.Fields.EnumValue.CaptionId, value.GetValue (Res.Fields.EnumValue.CaptionId));
					x.SetValue (Res.Fields.EnumValue.CultureMapSource, value.GetValue (Res.Fields.EnumValue.CultureMapSource));

					values.Add (x);

					if (mode == DataCreationMode.Public)
					{
						item.NotifyDataAdded (x);
					}
				}
			}

			if (UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceEnumType.Values)))
			{
				data.SetValue (Res.Fields.ResourceEnumType.Values, values);
				data.LockValue (Res.Fields.ResourceEnumType.Values);

				if (mode == DataCreationMode.Public)
				{
					EnumValueListener listener = new EnumValueListener (this, item, data);

					values.CollectionChanging += listener.HandleCollectionChanging;
					values.CollectionChanged  += listener.HandleCollectionChanged;
				}
			}

			if ((UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceEnumType.SystemType))) ||
				(type.SystemType != null))
			{
				data.SetValue (Res.Fields.ResourceEnumType.SystemType, type.SystemType);
			}
		}

		private void FillDataFromOtherType(StructuredData data, Caption caption, OtherType type)
		{
			data.DefineStructuredType (Res.Types.ResourceOtherType);

			if (caption.ContainsLocalValue (AbstractType.SytemTypeProperty))
			{
				data.SetValue (Res.Fields.ResourceOtherType.SystemType, type.SystemType);
			}
		}

		private void FillDataFromStringType(StructuredData data, Caption caption, StringType type)
		{
			data.DefineStructuredType (Res.Types.ResourceStringType);

			if (caption.ContainsLocalValue (StringType.MinimumLengthProperty))
			{
				data.SetValue (Res.Fields.ResourceStringType.MinimumLength, type.MinimumLength);
			}
			if (caption.ContainsLocalValue (StringType.MaximumLengthProperty))
			{
				data.SetValue (Res.Fields.ResourceStringType.MaximumLength, type.MaximumLength);
			}
			if (caption.ContainsLocalValue (StringType.UseMultilingualStorageProperty))
			{
				data.SetValue (Res.Fields.ResourceStringType.UseMultilingualStorage, type.UseMultilingualStorage);
			}
			if (caption.ContainsLocalValue (StringType.UseFormattedTextProperty))
			{
				data.SetValue (Res.Fields.ResourceStringType.UseFormattedText, type.UseFormattedText);
			}
			if (caption.ContainsLocalValue (StringType.DefaultSearchBehaviorProperty))
			{
				data.SetValue (Res.Fields.ResourceStringType.DefaultSearchBehavior, type.DefaultSearchBehavior);
			}
			if (caption.ContainsLocalValue (StringType.DefaultComparisonBehaviorProperty))
			{
				data.SetValue (Res.Fields.ResourceStringType.DefaultComparisonBehavior, type.DefaultComparisonBehavior);
			}
		}

		#endregion

		protected class EnumValueListener : Listener
		{
			public EnumValueListener(AnyTypeResourceAccessor accessor, CultureMap item, StructuredData data)
				: base (accessor, item, data)
			{
			}

			public override void HandleCollectionChanging(object sender)
			{
				if (this.SaveField (Res.Fields.ResourceEnumType.Values))
				{
					this.originalValues = new List<StructuredData> ();

					IList<StructuredData> values = this.Data.GetValue (Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;

					foreach (StructuredData data in values)
					{
						this.originalValues.Add (data.GetShallowCopy ());
					}
				}
			}

			public override void ResetToOriginalValue()
			{
				if (this.originalValues != null)
				{
					this.RestoreField (Res.Fields.ResourceEnumType.Values);

					ObservableList<StructuredData> values = this.Data.GetValue (Res.Fields.ResourceEnumType.Values) as ObservableList<StructuredData>;

					using (values.DisableNotifications ())
					{
						int index = values.Count - 1;

						while (index >= 0)
						{
							StructuredData data = values[index];
							values.RemoveAt (index--);
							this.Item.NotifyDataRemoved (data);
						}

						System.Diagnostics.Debug.Assert (values.Count == 0);

						foreach (StructuredData data in this.originalValues)
						{
							StructuredData copy = data.GetShallowCopy ();
							copy.PromoteToOriginal ();
							values.Add (copy);
							this.Item.NotifyDataAdded (copy);
						}
					}
				}
			}

			private List<StructuredData> originalValues;
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
			if (item.IsCultureDefined (Resources.DefaultTwoLetterISOLanguageName))
			{
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
			else
			{
				//	Nothing to do, the resource has no associated data.
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

			public string DefaultControllerParameters
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

			private Druid id;
		}

		#endregion

		#region EnumValueBroker Class

		private class EnumValueBroker : IDataBroker
		{
			public EnumValueBroker(AnyTypeResourceAccessor accessor)
			{
				this.accessor = accessor;
			}

			#region IDataBroker Members

			public StructuredData CreateData(CultureMap container)
			{
				StructuredData data = new StructuredData (Res.Types.EnumValue);
				CultureMapSource source = container.Source;

				if (this.accessor.BasedOnPatchModule)
				{
					source = CultureMapSource.PatchModule;
				}

				data.SetValue (Res.Fields.EnumValue.CaptionId, Druid.Empty);
				data.SetValue (Res.Fields.EnumValue.CultureMapSource, source);

				return data;
			}

			#endregion

			private AnyTypeResourceAccessor accessor;
		}

		#endregion

		/// <summary>
		/// Converts the value to a type code. An invalid value will be mapped
		/// to the <c>TypeCode.Invalid</c> type code.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The <see cref="TypeCode"/> or <c>TypeCode.Invalid</c>.</returns>
		private static TypeCode ToTypeCode(object value)
		{
			return UndefinedValue.IsUndefinedValue (value) ? TypeCode.Invalid : (TypeCode) value;
		}

		private ValueResourceAccessor valueAccessor;
	}
}
