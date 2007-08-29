//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	using CultureInfo=System.Globalization.CultureInfo;
	
	/// <summary>
	/// The <c>StringResourceAccessor</c> is used to access text resources,
	/// stored in the <c>Strings</c> resource bundle.
	/// </summary>
	public class StringResourceAccessor : AbstractResourceAccessor
	{
		public StringResourceAccessor()
		{
		}

		public override void Load(ResourceManager manager)
		{
			this.Initialize (manager);

			if (this.ResourceManager.BasedOnPatchModule)
			{
				ResourceManager patchModuleManager = this.ResourceManager;
				ResourceManager refModuleManager   = this.ResourceManager.GetManagerForReferenceModule ();

				System.Diagnostics.Debug.Assert (refModuleManager != null);
				System.Diagnostics.Debug.Assert (refModuleManager.BasedOnPatchModule == false);

				//	Load the data from the reference module first, then from the
				//	patch module; this will effectively merge both information :
				
				this.LoadFromBundle (refModuleManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Default), Resources.DefaultTwoLetterISOLanguageName);
				this.LoadFromBundle (patchModuleManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Default), Resources.DefaultTwoLetterISOLanguageName);
			}
			else
			{
				this.LoadFromBundle (this.ResourceManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Default), Resources.DefaultTwoLetterISOLanguageName);
			}
		}

		public override Types.StructuredData LoadCultureData(CultureMap item, string twoLetterISOLanguageName)
		{
			CultureInfo culture;
			ResourceLevel level;

			if (twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName)
			{
				culture = null;
				level = ResourceLevel.Default;
			}
			else
			{
				culture = Resources.FindCultureInfo (twoLetterISOLanguageName);
				level   = ResourceLevel.Localized;
			}
			
			ResourceBundle refBundle;
			ResourceBundle patchBundle;
			
			if (this.ResourceManager.BasedOnPatchModule)
			{
				refBundle   = this.ResourceManager.GetManagerForReferenceModule ().GetBundle (Resources.StringsBundleName, level, culture);
				patchBundle = this.ResourceManager.GetBundle (Resources.StringsBundleName, level, culture);
			}
			else
			{
				refBundle   = this.ResourceManager.GetBundle (Resources.StringsBundleName, level, culture);
				patchBundle = null;
			}

			ResourceBundle.Field refField   = refBundle   == null ? ResourceBundle.Field.Empty : refBundle[item.Id];
			ResourceBundle.Field patchField = patchBundle == null ? ResourceBundle.Field.Empty : patchBundle[item.Id];
			
			Types.StructuredData data  = null;

			if ((refField.IsEmpty) &&
				(patchField.IsEmpty))
			{
				data = new Types.StructuredData (Res.Types.ResourceString);
				item.RecordCultureData (twoLetterISOLanguageName, data);
			}
			else
			{
				Types.StructuredData data1 = (refField.IsEmpty)   ? null : this.LoadFromField (refField, refBundle.Module.Id, twoLetterISOLanguageName);
				Types.StructuredData data2 = (patchField.IsEmpty) ? null : this.LoadFromField (patchField, patchBundle.Module.Id, twoLetterISOLanguageName);

				data = data1 ?? data2;
			}

			return data;
		}

		public override IDataBroker GetDataBroker(StructuredData container, string fieldId)
		{
			return base.GetDataBroker (container, fieldId);
		}

		protected override Druid CreateId()
		{
			ResourceBundle bundle = this.ResourceManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Default);
			return AbstractResourceAccessor.CreateId (bundle, this.Collection);
		}

		protected override void DeleteItem(CultureMap item)
		{
			foreach (string twoLetterISOLanguageName in item.GetDefinedCultures ())
			{
				ResourceBundle bundle;
				CultureInfo culture;

				if (twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName)
				{
					bundle = this.ResourceManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Default);
				}
				else
				{
					culture = Resources.FindCultureInfo (twoLetterISOLanguageName);
					bundle  = this.ResourceManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Localized, culture);
				}

				if (bundle != null)
				{
					int pos = bundle.IndexOf (item.Id);
					
					if (pos >= 0)
					{
						bundle.Remove (pos);
					}
				}
			}
			
		}
		
		protected override void PersistItem(CultureMap item)
		{
			if (string.IsNullOrEmpty (item.Name))
			{
				throw new System.ArgumentException (string.Format ("No name for item {0}", item.Id));
			}

			ResourceManager refModuleManager = this.ResourceManager.GetManagerForReferenceModule ();
			bool            usePatchModule   = refModuleManager != null;

			int nonEmptyFieldCount = 0;
			
			foreach (string twoLetterISOLanguageName in item.GetDefinedCultures ())
			{
				StructuredData data    = item.GetCultureData (twoLetterISOLanguageName);
				CultureInfo    culture = Resources.FindCultureInfo (twoLetterISOLanguageName);
				ResourceLevel  level   = twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName ? ResourceLevel.Default : ResourceLevel.Localized;
				ResourceBundle bundle  = this.ResourceManager.GetBundle (Resources.StringsBundleName, level, culture);
				
				bool deleteField = false;

				if (usePatchModule)
				{
					//	The resource should be stored as a delta (patch) relative
					//	to the reference resource. Compute what that is...

					deleteField = this.ComputeDelta (refModuleManager, ref data, culture, level, bundle, item.Id);
				}

				if ((deleteField) ||
					(StringResourceAccessor.IsEmpty (data)))
				{
					//	There is no delta found for this resource, so we can safely
					//	remove it if it is a secondary resource or, if it is the
					//	primary resource, only remove it if we are sure that there
					//	are no secondary resources left.

					if ((twoLetterISOLanguageName != Resources.DefaultTwoLetterISOLanguageName) ||
						(nonEmptyFieldCount == 0))
					{
						if (bundle != null)
						{
							int index = bundle.IndexOf (item.Id);

							if (index >= 0)
							{
								bundle.Remove (index);
							}
						}
					}
				}
				else
				{
					//	The resource contains valid data. We will have to create the
					//	bundle and the field if they are currently missing :

					if (bundle == null)
					{
						ResourceModuleId moduleId = this.ResourceManager.GetModuleFromFullId (item.Id.ToString ());
						bundle = ResourceBundle.Create (this.ResourceManager, this.ResourceManager.ActivePrefix, moduleId, Resources.StringsBundleName, ResourceLevel.Localized, culture, 0);
						bundle.DefineType ("String");
						this.ResourceManager.SetBundle (bundle, ResourceSetMode.InMemory);
					}

					ResourceBundle.Field field = bundle[item.Id];

					if (field.IsEmpty)
					{
						field = bundle.CreateField (ResourceFieldType.Data);
						field.SetDruid (item.Id);
						bundle.Add (field);
					}

					string text  = data.GetValue (Res.Fields.ResourceString.Text) as string;
					string about = data.GetValue (Res.Fields.ResourceBase.Comment) as string;
					object modId = data.GetValue (Res.Fields.ResourceBase.ModificationId);

					if (ResourceBundle.Field.IsNullString (text))
					{
						text = null;
					}
					if (ResourceBundle.Field.IsNullString (about))
					{
						about = null;
					}

					if (twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName)
					{
						field.SetName (item.Name);
					}
					else
					{
						field.SetName (null);
					}
					
					field.SetStringValue (text);
					field.SetAbout (about);

					StringResourceAccessor.SetModificationId (field, modId);
					
					nonEmptyFieldCount++;
				}
			}
		}

		private static bool IsEmpty(StructuredData data)
		{
			return (UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceString.Text)))
				&& (UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceBase.Comment)))
				&& (UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceBase.ModificationId)));
		}

		private bool ComputeDelta(ResourceManager refModuleManager, ref StructuredData data, CultureInfo culture, ResourceLevel level, ResourceBundle patchBundle, Druid druid)
		{
			ResourceBundle refBundle = refModuleManager.GetBundle (Resources.StringsBundleName, level, culture);

			if (refBundle == null)
			{
				return false;
			}

			ResourceBundle.Field refField = refBundle[druid];
			
			if (refField.IsEmpty)
			{
				return false;
			}

			//	Get the reference data from the reference module resource :

			int    refModifId = refField.ModificationId;
			string refText    = refField.AsString == ResourceBundle.Field.Null ? null : refField.AsString;
			string refComment = refField.About;

			//	Get the resulting data the user would like to get when applying
			//	the patch to the reference data :

			object dataModifIdValue = data.GetValue (Res.Fields.ResourceBase.ModificationId);
			
			int    dataModifId = UndefinedValue.IsUndefinedValue (dataModifIdValue) ? -1 : (int) dataModifIdValue;
			string dataText    = data.GetValue (Res.Fields.ResourceString.Text) as string;
			string dataComment = data.GetValue (Res.Fields.ResourceBase.Comment) as string;

			//	If some of the resulting data are undefined, assume that this
			//	means that the user wants to get the reference data instead;
			//	so just merge the reference with the provided data :

			int    mergeModifId = dataModifId == -1   ? refModifId : dataModifId;
			string mergeText    = dataText    == null ? refText    : dataText;
			string mergeComment = dataComment == null ? refComment : dataComment;

			//	If the merged data is exactly the same as the reference data,
			//	then there is nothing left to patch; tell the caller that the
			//	patch resource can be safely discarded :

			if ((mergeModifId == refModifId) &&
				(mergeText    == refText) &&
				(mergeComment == refComment))
			{
				return true;
			}

			//	Wherever the patch data is the same as the reference data, use
			//	the "undefined" value instead :

			bool replace = false;

			if ((mergeModifId == refModifId) &&
				(dataModifId != -1))
			{
				dataModifId = -1;
				replace     = true;
			}
			
			if ((mergeText == refText) &&
				(dataText != null))
			{
				dataText = null;
				replace  = true;
			}
			
			if ((mergeComment == refComment) &&
				(dataComment != null))
			{
				dataComment = null;
				replace     = true;
			}

			if (replace)
			{
				data = new StructuredData (Res.Types.ResourceString);

				data.SetValue (Res.Fields.ResourceString.Text, dataText);
				data.SetValue (Res.Fields.ResourceBase.Comment, dataComment);
				data.SetValue (Res.Fields.ResourceBase.ModificationId, dataModifId);
			}

			return false;
		}

		internal static void SetModificationId(ResourceBundle.Field field, object modId)
		{
			if (!UndefinedValue.IsUndefinedValue (modId))
			{
				field.SetModificationId ((int) modId);
			}
			else
			{
				field.SetModificationId (-1);
			}
		}

		protected override Types.StructuredData LoadFromField(ResourceBundle.Field field, int module, string twoLetterISOLanguageName)
		{
			Druid id = new Druid (field.Id, module);
			bool insert;
			bool record;

			CultureMap item = this.Collection[id];
			CultureMapSource fieldSource = this.GetCultureMapSource (field);
			StructuredData data = new StructuredData (Res.Types.ResourceString);
			
			StringResourceAccessor.SetDataFromField (field, data);

			if (item == null)
			{
				//	Fresh item, not yet known :

				item = new CultureMap (this, id, fieldSource);
				
				insert = true;
				record = true;
			}
			else if (item.Source == fieldSource)
			{
				//	We already have an item for this id, but since we are fetching
				//	data from the same source as before, we can safely assume that
				//	this will produce new data for a not yet known culture :

				insert = false;
				record = true;
			}
			else
			{
				//	The source which was used to fill this item is different from
				//	the current source...

				if (item.IsCultureDefined (twoLetterISOLanguageName))
				{
					//	...and we know that there is already some data available
					//	for the culture. Merge the data :

					StructuredData newData = data;
					StructuredData oldData = item.GetCultureData (twoLetterISOLanguageName);

					if ((field.AsString != null) &&
						(field.AsString != ResourceBundle.Field.Null))
					{
						oldData.SetValue (Res.Fields.ResourceString.Text, newData.GetValue (Res.Fields.ResourceString.Text));
					}
					if (field.About != null)
					{
						oldData.SetValue (Res.Fields.ResourceBase.Comment, newData.GetValue (Res.Fields.ResourceBase.Comment));
					}
					if (field.ModificationId > 0)
					{
						oldData.SetValue (Res.Fields.ResourceBase.ModificationId, newData.GetValue (Res.Fields.ResourceBase.ModificationId));
					}
					

					data = oldData;

					insert = false;
					record = false;
				}
				else
				{
					//	...but we are filling in data for an unknown culture.
					//	Simply add the data to the item :

					insert = false;
					record = true;
				}

				//	Make sure we remember that the item contains merged data.
				
				item.Source = CultureMapSource.DynamicMerge;
			}

			item.Name = field.Name ?? item.Name;

			if (record)
			{
				item.RecordCultureData (twoLetterISOLanguageName, data);
			}
			if (insert)
			{
				this.Collection.Add (item);
			}

			return data;
		}

		private static void SetDataFromField(ResourceBundle.Field field, Types.StructuredData data)
		{
			data.SetValue (Res.Fields.ResourceString.Text, ResourceBundle.Field.IsNullString (field.AsString) ? null : field.AsString);
			data.SetValue (Res.Fields.ResourceBase.Comment, ResourceBundle.Field.IsNullString (field.About) ? null : field.About);
			data.SetValue (Res.Fields.ResourceBase.ModificationId, field.ModificationId);
		}

		protected override bool FilterField(ResourceBundle.Field field)
		{
			return true;
		}
	}
}
