//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		/// <summary>
		/// Initializes a new instance of the <see cref="StringResourceAccessor"/> class.
		/// </summary>
		public StringResourceAccessor()
		{
		}

		/// <summary>
		/// Loads resources from the specified resource manager. The resource
		/// manager will be used for all upcoming accesses.
		/// </summary>
		/// <param name="manager">The resource manager.</param>
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

				this.LoadFromBundle (refModuleManager.GetBundle (this.GetBundleName (), ResourceLevel.Default), Resources.DefaultTwoLetterISOLanguageName);
				this.LoadFromBundle (patchModuleManager.GetBundle (this.GetBundleName (), ResourceLevel.Default), Resources.DefaultTwoLetterISOLanguageName);
			}
			else
			{
				this.LoadFromBundle (this.ResourceManager.GetBundle (this.GetBundleName (), ResourceLevel.Default), Resources.DefaultTwoLetterISOLanguageName);
			}

			this.PostLoadCleanup ();
		}


		/// <summary>
		/// Loads the data for the specified culture into an existing item.
		/// </summary>
		/// <param name="item">The item to update.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <returns>
		/// The data loaded from the resources which was stored in the specified item.
		/// </returns>
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
			
			//	If the module is a patch module, then handle the merge between the
			//	data coming from the reference module and the patch module :
			
			ResourceBundle refBundle;
			ResourceBundle patchBundle;
			
			if (this.ResourceManager.BasedOnPatchModule)
			{
				refBundle   = this.ResourceManager.GetManagerForReferenceModule ().GetBundle (this.GetBundleName (), level, culture);
				patchBundle = this.ResourceManager.GetBundle (this.GetBundleName (), level, culture);
			}
			else
			{
				refBundle   = this.ResourceManager.GetBundle (this.GetBundleName (), level, culture);
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

		/// <summary>
		/// Gets the bundle name used by this accessor.
		/// </summary>
		/// <returns>The name of the bundle.</returns>
		protected override string GetBundleName()
		{
			return Resources.StringsBundleName;
		}

		/// <summary>
		/// Creates a new unique id.
		/// </summary>
		/// <returns>The new unique id.</returns>
		protected override Druid CreateId()
		{
			ResourceBundle bundle1 = this.ResourceManager.GetBundle (this.GetBundleName (), ResourceLevel.Default);
			ResourceBundle bundle2 = null;

			if (this.ResourceManager.BasedOnPatchModule)
			{
				bundle2 = this.ResourceManager.GetManagerForReferenceModule ().GetBundle (this.GetBundleName (), ResourceLevel.Default);
			}
			
			return AbstractResourceAccessor.CreateId (this.Collection, this.ResourceManager, bundle1, bundle2);
		}

		/// <summary>
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item to delete.</param>
		protected override void DeleteItem(CultureMap item)
		{
			foreach (string twoLetterISOLanguageName in item.GetDefinedCultures ())
			{
				ResourceBundle bundle;
				CultureInfo culture;

				if (twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName)
				{
					bundle = this.ResourceManager.GetBundle (this.GetBundleName (), ResourceLevel.Default);
				}
				else
				{
					culture = Resources.FindCultureInfo (twoLetterISOLanguageName);
					bundle  = this.ResourceManager.GetBundle (this.GetBundleName (), ResourceLevel.Localized, culture);
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

		/// <summary>
		/// Persists the specified item.
		/// </summary>
		/// <param name="item">The item to store as a resource.</param>
		protected override void PersistItem(CultureMap item)
		{
			if (string.IsNullOrEmpty (item.Name))
			{
				throw new System.ArgumentException (string.Format ("No name for item {0}", item.Id));
			}

			ResourceManager refModuleManager = this.ResourceManager.GetManagerForReferenceModule ();
			bool            usePatchModule   = (!this.ForceModuleMerge) && (refModuleManager != null);

			int nonEmptyFieldCount = 0;
			
			foreach (string twoLetterISOLanguageName in item.GetDefinedCultures ())
			{
				StructuredData data    = item.GetCultureData (twoLetterISOLanguageName);
				CultureInfo    culture = Resources.FindCultureInfo (twoLetterISOLanguageName);
				ResourceLevel  level   = twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName ? ResourceLevel.Default : ResourceLevel.Localized;
				ResourceBundle bundle  = this.ResourceManager.GetBundle (this.GetBundleName (), level, culture);
				
				bool deleteField = false;

				if (usePatchModule)
				{
					//	The resource should be stored as a delta (patch) relative
					//	to the reference resource. Compute what that is...

					if ((StringResourceAccessor.ComputeDelta (ref data, refModuleManager.GetBundle (this.GetBundleName (), level, culture), item.Id)) ||
						(StringResourceAccessor.IsEmpty (data)))
					{
						//	The resource is empty... but we may not remove it if
						//	this is the primary patch resource and we found some
						//	non-empty secondary resources.
						
						if ((twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName) &&
							(nonEmptyFieldCount > 0))
						{
							//	Don't delete the field for this resource...
						}
						else
						{
							deleteField = true;
						}
					}

					if (item.Source == CultureMapSource.ReferenceModule)
					{
						item.Source = CultureMapSource.DynamicMerge;
					}
				}
				else
				{
					//	If this an empty secondary resource, delete the corresponding
					//	field to avoid cluttering the resource bundle.

					if ((twoLetterISOLanguageName != Resources.DefaultTwoLetterISOLanguageName) &&
						(StringResourceAccessor.IsEmpty (data)))
					{
						deleteField = true;
					}
				}

				if (deleteField)
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
				else
				{
					//	The resource contains valid data. We will have to create the
					//	bundle and the field if they are currently missing :

					if (bundle == null)
					{
						ResourceModuleId moduleId = this.ResourceManager.GetModuleFromFullId (item.Id.ToString ());
						bundle = ResourceBundle.Create (this.ResourceManager, this.ResourceManager.ActivePrefix, moduleId, this.GetBundleName (), level, culture, 0);
						bundle.DefineType ("String");
						this.ResourceManager.SetBundle (bundle, ResourceSetMode.InMemory);
					}

					ResourceBundle.Field field = bundle[item.Id];

					if (field.IsEmpty)
					{
						field = bundle.CreateField (ResourceFieldType.Data);
						field.SetId (item.Id);
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

					if ((twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName) &&
						((!usePatchModule) || (item.Source != CultureMapSource.DynamicMerge)))
					{
						field.SetName (item.Name);
					}
					else
					{
						//	We don't want to name secondary resources, nor do we need
						//	to name patch resources which override an existing reference
						//	resource.

						field.SetName (null);
					}
					
					field.SetStringValue (text);
					field.SetAbout (about);

					StringResourceAccessor.SetModificationId (field, modId);
					
					nonEmptyFieldCount++;
				}
			}
		}

		/// <summary>
		/// Loads data from a resource bundle field.
		/// </summary>
		/// <param name="field">The resource bundle field.</param>
		/// <param name="module">The source module id.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <returns>
		/// The data which describes the specified resource.
		/// </returns>
		protected override Types.StructuredData LoadFromField(ResourceBundle.Field field, int module, string twoLetterISOLanguageName)
		{
			Druid id = new Druid (field.Id, module);
			bool insert;
			bool record;
			bool freezeName = false;
			bool original = false;

			CultureMap item = this.Collection[id];
			CultureMapSource fieldSource = this.GetCultureMapSource (field);
			StructuredData data = new StructuredData (Res.Types.ResourceString);

			StringResourceAccessor.FillDataFromField (field, data);

			if ((fieldSource == CultureMapSource.ReferenceModule) &&
				(this.ResourceManager.BasedOnPatchModule))
			{
				freezeName = true;
				original   = true;
			}

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

					if (!ResourceBundle.Field.IsNullString (field.AsString))
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
				freezeName  = true;
			}

			if ((!item.IsNameReadOnly) &&
				(twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName))
			{
				item.Name = field.Name ?? item.Name;
			}

			if (freezeName)
			{
				item.FreezeName ();
			}

			if (original)
			{
				data.PromoteToOriginal ();
			}

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

		/// <summary>
		/// Checks if the data stored in the field matches this accessor. This
		/// implementation always returns <c>true</c>.
		/// </summary>
		/// <param name="field">The field to check.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns>Always <c>true</c>.</returns>
		protected override bool FilterField(ResourceBundle.Field field, string fieldName)
		{
			return true;
		}

		/// <summary>
		/// Determines whether the specified data record is empty (only undefined
		/// values in the text, comment and modification id fields).
		/// </summary>
		/// <param name="data">The data record.</param>
		/// <returns>
		/// 	<c>true</c> if the specified data record is empty; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsEmpty(StructuredData data)
		{
			string text    = data.GetValue (Res.Fields.ResourceString.Text) as string;
			string comment = data.GetValue (Res.Fields.ResourceBase.Comment) as string;
			int    modifId = StringResourceAccessor.GetModificationId (data);

			return (ResourceBundle.Field.IsNullString (text))
				&& (ResourceBundle.Field.IsNullString (comment))
				&& (modifId < 1);
		}

		/// <summary>
		/// Computes the delta between the reference data and the current data
		/// provided in the data record.
		/// </summary>
		/// <param name="data">The current data record.</param>
		/// <param name="refBundle">The bundle from the reference module.</param>
		/// <param name="druid">The id for the item.</param>
		/// <returns>
		/// <c>true</c> if the current data is the same as the reference data;
		/// otherwise, <c>false</c>.
		/// </returns>
		private static bool ComputeDelta(ref StructuredData data, ResourceBundle refBundle, Druid druid)
		{
			if (refBundle == null)
			{
				return false;
			}

			ResourceBundle.Field refField = refBundle[druid];
			
			if (refField.IsEmpty)
			{
				return false;
			}

			bool replace = false;
			
			int    dataModifId = StringResourceAccessor.GetModificationId (StringResourceAccessor.GetDeltaValue (data, Res.Fields.ResourceBase.ModificationId, ref replace));
			string dataText    = StringResourceAccessor.GetDeltaValue (data, Res.Fields.ResourceString.Text, ref replace) as string;
			string dataComment = StringResourceAccessor.GetDeltaValue (data, Res.Fields.ResourceBase.Comment, ref replace) as string;

			if (replace)
			{
				data = new StructuredData (Res.Types.ResourceString);

				data.SetValue (Res.Fields.ResourceString.Text, dataText);
				data.SetValue (Res.Fields.ResourceBase.Comment, dataComment);
				data.SetValue (Res.Fields.ResourceBase.ModificationId, dataModifId);
			}

			return false;
		}

		/// <summary>
		/// Fills the values in the data record based on a resource bundle field.
		/// </summary>
		/// <param name="field">The resource bundle field.</param>
		/// <param name="data">The data record to fill.</param>
		private static void FillDataFromField(ResourceBundle.Field field, Types.StructuredData data)
		{
			data.SetValue (Res.Fields.ResourceString.Text, ResourceBundle.Field.IsNullString (field.AsString) ? null : field.AsString);
			data.SetValue (Res.Fields.ResourceBase.Comment, ResourceBundle.Field.IsNullString (field.About) ? null : field.About);
			data.SetValue (Res.Fields.ResourceBase.ModificationId, field.ModificationId);
		}

		/// <summary>
		/// Sets the modification id for the specified resource bundle field,
		/// based on an object representation.
		/// </summary>
		/// <param name="field">The resource bundle field.</param>
		/// <param name="modId">The modification id (which can be an <c>UndefinedValue.Value</c>).</param>
		internal static void SetModificationId(ResourceBundle.Field field, object modId)
		{
			if (UndefinedValue.IsUndefinedValue (modId))
			{
				field.SetModificationId (0);
			}
			else
			{
				field.SetModificationId ((int) modId);
			}
		}

		internal static int GetModificationId(StructuredData data)
		{
			if (data == null)
			{
				return 0;
			}
			else
			{
				return StringResourceAccessor.GetModificationId (data.GetValue (Res.Fields.ResourceBase.ModificationId));
			}
		}

		internal static object GetDeltaValue(StructuredData data, Druid id)
		{
			bool replace = false;
			return StringResourceAccessor.GetDeltaValue (data, id, ref replace);
		}

		/// <summary>
		/// Gets the value used when computing the delta for a given data field.
		/// </summary>
		/// <param name="data">The structured data record.</param>
		/// <param name="id">The field id.</param>
		/// <param name="replace">Set to <c>true</c> if there is a replacement for the original data.</param>
		/// <returns>The value or <c>UndefinedValue.Value</c>.</returns>
		internal static object GetDeltaValue(StructuredData data, Druid id, ref bool replace)
		{
			bool usesOriginalData;

			object value = data.GetValue (id, out usesOriginalData);

			if (usesOriginalData)
			{
				return UndefinedValue.Value;
			}
			else
			{
				replace = true;
				return value;
			}
		}

		internal static int GetModificationId(object modId)
		{
			if ((UndefinedValue.IsUndefinedValue (modId)) ||
				(modId == null))
			{
				return 0;
			}
			else
			{
				return (int) modId;
			}
		}
	}
}
