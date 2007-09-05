//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Support.ResourceAccessors.AbstractCaptionResourceAccessor))]

namespace Epsitec.Common.Support.ResourceAccessors
{
	using CultureInfo=System.Globalization.CultureInfo;
	
	/// <summary>
	/// The <c>AbstractCaptionResourceAccessor</c> is used to access caption
	/// based resources stored in the <c>Captions</c> resource bundle. Use the
	/// specialized classes for the real access.
	/// </summary>
	public abstract class AbstractCaptionResourceAccessor : AbstractResourceAccessor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractCaptionResourceAccessor"/> class.
		/// </summary>
		public AbstractCaptionResourceAccessor()
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

			this.RegisterAccessorWithActiveBundle ();

			if (this.ResourceManager.BasedOnPatchModule)
			{
				ResourceManager patchModuleManager = this.ResourceManager;
				ResourceManager refModuleManager   = this.ResourceManager.GetManagerForReferenceModule ();

				System.Diagnostics.Debug.Assert (refModuleManager != null);
				System.Diagnostics.Debug.Assert (refModuleManager.BasedOnPatchModule == false);

				//	Load the data from the reference module first, then from the
				//	patch module; this will effectively merge both information :

				this.LoadFromBundle (refModuleManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default), Resources.DefaultTwoLetterISOLanguageName);
				this.LoadFromBundle (patchModuleManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default), Resources.DefaultTwoLetterISOLanguageName);
			}
			else
			{
				this.LoadFromBundle (this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default), Resources.DefaultTwoLetterISOLanguageName);
			}
		}

		/// <summary>
		/// Creates a new item which can then be added to the collection.
		/// </summary>
		/// <returns>A new <see cref="CultureMap"/> item.</returns>
		public override CultureMap CreateItem()
		{
			CultureMap item = this.CreateItem (null, this.CreateId ());
			item.IsNewItem = true;
			return item;
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
				refBundle   = this.ResourceManager.GetManagerForReferenceModule ().GetBundle (Resources.CaptionsBundleName, level, culture);
				patchBundle = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, level, culture);
			}
			else
			{
				refBundle   = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, level, culture);
				patchBundle = null;
			}

			ResourceBundle.Field refField   = refBundle   == null ? ResourceBundle.Field.Empty : refBundle[item.Id];
			ResourceBundle.Field patchField = patchBundle == null ? ResourceBundle.Field.Empty : patchBundle[item.Id];

			Types.StructuredData data  = null;

			if ((refField.IsEmpty || ResourceBundle.Field.IsNullString (refField.AsString)) &&
				(patchField.IsEmpty || ResourceBundle.Field.IsNullString (patchField.AsString)))
			{
				Caption caption = new Caption ();
				ResourceManager.SetSourceBundle (caption, refBundle);
				data = new Types.StructuredData (this.GetStructuredType ());
				this.FillDataFromCaption (item, data, caption);
				item.RecordCultureData (twoLetterISOLanguageName, data);
			}
			else
			{
				Types.StructuredData data1 = (refField.IsEmpty   || ResourceBundle.Field.IsNullString (refField.AsString))   ? null : this.LoadFromField (refField, refBundle.Module.Id, twoLetterISOLanguageName);
				Types.StructuredData data2 = (patchField.IsEmpty || ResourceBundle.Field.IsNullString (patchField.AsString)) ? null : this.LoadFromField (patchField, patchBundle.Module.Id, twoLetterISOLanguageName);

				data = data1 ?? data2;
			}

			return data;
		}

		public override IDataBroker GetDataBroker(StructuredData container, string fieldId)
		{
			return base.GetDataBroker (container, fieldId);
		}

		protected abstract IStructuredType GetStructuredType();

		protected override Druid CreateId()
		{
			ResourceBundle bundle = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default);
			AccessorsCollection accessors = AbstractCaptionResourceAccessor.GetAccessors (bundle);
			return AbstractResourceAccessor.CreateId (accessors.AllCollections, bundle);
		}

		protected override void DeleteItem(CultureMap item)
		{
			foreach (string twoLetterISOLanguageName in item.GetDefinedCultures ())
			{
				ResourceBundle bundle;
				CultureInfo culture;

				if (twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName)
				{
					bundle = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default);
					this.ResourceManager.ClearCaptionCache (item.Id, ResourceLevel.Default, null);
				}
				else
				{
					culture = Resources.FindCultureInfo (twoLetterISOLanguageName);
					bundle  = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Localized, culture);
					this.ResourceManager.ClearCaptionCache (item.Id, ResourceLevel.Localized, culture);
					this.ResourceManager.ClearCaptionCache (item.Id, ResourceLevel.Merged, culture);
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
				ResourceBundle bundle  = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, level, culture);

				bool deleteField = false;

				if (usePatchModule)
				{
					//	The resource should be stored as a delta (patch) relative
					//	to the reference resource. Compute what that is...
					
					if ((this.ComputeDelta (item, ref data, refModuleManager.GetBundle (Resources.CaptionsBundleName, level, culture), item.Id, bundle, twoLetterISOLanguageName)) ||
						(this.IsEmpty (data)))
					{
						//	The resource is empty... but we may not remove it if
						//	this is the primary patch resource and we found some
						//	non-empty secondary resources.

						if ((level == ResourceLevel.Default) &&
							(nonEmptyFieldCount > 0))
						{
							//	Don't delete the field for this resource...
						}
						else
						{
							deleteField = true;
						}
					}
				}
				else
				{
					//	If this an empty secondary resource, delete the corresponding
					//	field to avoid cluttering the resource bundle.

					if ((level == ResourceLevel.Localized) &&
						(this.IsEmpty (data)))
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
						bundle = ResourceBundle.Create (this.ResourceManager, this.ResourceManager.ActivePrefix, moduleId, Resources.CaptionsBundleName, level, culture, 0);
						bundle.DefineType ("Caption");
						this.ResourceManager.SetBundle (bundle, ResourceSetMode.InMemory);
					}

					ResourceBundle.Field field = bundle[item.Id];

					if (field.IsEmpty)
					{
						field = bundle.CreateField (ResourceFieldType.Data);
						field.SetDruid (item.Id);
						bundle.Add (field);
					}

					string  capName = level == ResourceLevel.Default ? item.Name : null;
					Caption caption = this.GetCaptionFromData (bundle, data, capName, twoLetterISOLanguageName);
					string  about   = data.GetValue (Res.Fields.ResourceBase.Comment) as string;
					object  modId   = data.GetValue (Res.Fields.ResourceBase.ModificationId);

#if false
					if (ResourceBundle.Field.IsNullString (text))
					{
						text = null;
					}
#endif
					if (ResourceBundle.Field.IsNullString (about))
					{
						about = null;
					}

					if ((level == ResourceLevel.Default) &&
						(item.Source != CultureMapSource.DynamicMerge))
					{
						field.SetName (this.GetFieldNameFromName (item, data));
					}
					else
					{
						//	We don't want to name secondary resources, nor do we need
						//	to name patch resources which override an existing reference
						//	resource.

						field.SetName (null);
					}

					field.SetStringValue (caption == null ? null : caption.SerializeToString ());
					field.SetAbout (about);

					StringResourceAccessor.SetModificationId (field, modId);

					nonEmptyFieldCount++;
				}

#if true
				if (level == ResourceLevel.Localized)
				{
					this.ResourceManager.ClearCaptionCache (item.Id, ResourceLevel.Localized, culture);
					this.ResourceManager.ClearCaptionCache (item.Id, ResourceLevel.Merged, culture);
				}
				else
				{
					this.ResourceManager.ClearCaptionCache (item.Id, ResourceLevel.Default, null);
				}
#endif
			}
		}

		protected virtual bool IsEmpty(StructuredData data)
		{
			IList<string> labels = data.GetValue (Res.Fields.ResourceCaption.Labels) as IList<string>;

			string description = data.GetValue (Res.Fields.ResourceCaption.Description) as string;
			string icon        = data.GetValue (Res.Fields.ResourceCaption.Icon) as string;
			string comment     = data.GetValue (Res.Fields.ResourceBase.Comment) as string;
			int    modifId     = StringResourceAccessor.GetModificationId (data);

			return ((labels == null) || (labels.Count == 0))
				&& (ResourceBundle.Field.IsNullString (description))
				&& (ResourceBundle.Field.IsNullString (icon))
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
		protected virtual bool ComputeDelta(CultureMap item, ref StructuredData data, ResourceBundle refBundle, Druid druid, ResourceBundle dataBundle, string twoLetterISOLanguageName)
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

			//	Get the reference data from the reference module resource :

			int     refModifId = refField.ModificationId;
			string  refComment = refField.About;
			Caption refCaption = new Caption ();

			ResourceManager.SetSourceBundle (refCaption, refBundle);
			refCaption.DeserializeFromString (refField.AsString, refBundle.ResourceManager);


			//	Get the resulting data the user would like to get when applying
			//	the patch to the reference data :

			object dataModifIdValue = data.GetValue (Res.Fields.ResourceBase.ModificationId);

			int     dataModifId = StringResourceAccessor.GetModificationId (data.GetValue (Res.Fields.ResourceBase.ModificationId));
			string  dataComment = data.GetValue (Res.Fields.ResourceBase.Comment) as string;
			Caption dataCaption = this.GetCaptionFromData (dataBundle, data, null, twoLetterISOLanguageName);

			//	If some of the resulting data are undefined, assume that this
			//	means that the user wants to get the reference data instead;
			//	so just merge the reference with the provided data :

			int    mergeModifId = dataModifId < 1                                 ? refModifId : dataModifId;
			string mergeComment = ResourceBundle.Field.IsNullString (dataComment) ? refComment : dataComment;

			//	TODO: mergeCaption = ...

			//	If the merged data is exactly the same as the reference data,
			//	then there is nothing left to patch; tell the caller that the
			//	patch resource can be safely discarded :

			if ((mergeModifId == refModifId) &&
				/*(mergeText    == refText) &&*/
				(mergeComment == refComment))
			{
				return true;
			}

			//	Wherever the patch data is the same as the reference data, use
			//	the "undefined" value instead :

			bool replace = false;

			if ((mergeModifId == refModifId) &&
				(dataModifId > 0))
			{
				dataModifId = 0;
				replace     = true;
			}

			/*
			if ((mergeText == refText) &&
				(dataText != null))
			{
				dataText = null;
				replace  = true;
			}
			 */

			if ((mergeComment == refComment) &&
				(dataComment != null))
			{
				dataComment = null;
				replace     = true;
			}

			if (replace)
			{
				data = new StructuredData (Res.Types.ResourceString);

				data.SetValue (Res.Fields.ResourceBase.Comment, dataComment);
				data.SetValue (Res.Fields.ResourceBase.ModificationId, dataModifId);

				this.FillDataFromCaption (item, data, dataCaption);
			}

			return false;
		}



		protected abstract string GetFieldNameFromName(CultureMap item, Types.StructuredData data);

		protected abstract Caption GetCaptionFromData(ResourceBundle sourceBundle, Types.StructuredData data, string name, string twoLetterISOLanguageName);

		protected abstract void FillDataFromCaption(CultureMap item, Types.StructuredData data, Caption caption);

		protected abstract string GetNameFromFieldName(CultureMap item, string fieldName);


		protected override Types.StructuredData LoadFromField(ResourceBundle.Field field, int module, string twoLetterISOLanguageName)
		{
			Druid id     = new Druid (field.Id, module);
			bool insert = false;
			bool record;
			bool freezeName = false;

			CultureMap item = this.Collection[id];
			CultureMapSource fieldSource = this.GetCultureMapSource (field);
			StructuredData data = new StructuredData (this.GetStructuredType ());

			//	Preliminary fill of the very basic data fields :
			
			data.SetValue (Res.Fields.ResourceBase.Comment, ResourceBundle.Field.IsNullString (field.About) ? null : field.About);
			data.SetValue (Res.Fields.ResourceBase.ModificationId, field.ModificationId);

			if ((fieldSource == CultureMapSource.ReferenceModule) &&
				(this.ResourceManager.BasedOnPatchModule))
			{
				freezeName = true;
			}

			if (item == null)
			{
				//	Fresh item, not yet known :

				item   = this.CreateItem (field, id, fieldSource);
				
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

			Caption caption = new Caption (id);
			string  name    = string.IsNullOrEmpty (field.Name) ? null : this.GetNameFromFieldName (item, field.Name);

			ResourceManager.SetSourceBundle (caption, field.ParentBundle);
			caption.DeserializeFromString (field.AsString, this.ResourceManager);

			this.FillDataFromCaption (item, data, caption);
			
			//	It is important to first associate the culture data, then defining
			//	the item name, since AnyTypeResourceAccessor listens for name changes
			//	in order to update enumeration value names.

			if (record)
			{
				item.RecordCultureData (twoLetterISOLanguageName, data);
			}
			
			if ((!item.IsNameReadOnly) &&
				(twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName))
			{
				item.Name = name ?? item.Name;
			}

			if (freezeName)
			{
				item.FreezeName ();
			}

			if (insert)
			{
				this.Collection.Add (item);
			}

			return data;
		}

		protected CultureMap CreateItem(ResourceBundle.Field field, Druid id)
		{
			return this.CreateItem (field, id, this.GetCultureMapSource (null));
		}

		protected virtual CultureMap CreateItem(ResourceBundle.Field field, Druid id, CultureMapSource source)
		{
			return new CultureMap (this, id, source);
		}

		#region AccessorsCollection Class

		/// <summary>
		/// The <c>AccessorsCollection</c> class maintains a collection of
		/// <see cref="AbstractCaptionResourceAccessor"/> instances. This is
		/// used to iterate over all caption items associated to the same
		/// resource manager.
		/// </summary>
		private class AccessorsCollection
		{
			public AccessorsCollection()
			{
				this.list = new List<AbstractCaptionResourceAccessor> ();
			}

			public void Add(AbstractCaptionResourceAccessor item)
			{
				this.list.Add (item);
			}

			public void Remove(AbstractCaptionResourceAccessor item)
			{
				this.list.Remove (item);
			}

			public IEnumerable<CultureMap> AllCollections
			{
				get
				{
					foreach (AbstractCaptionResourceAccessor accessor in this.list)
					{
						foreach (CultureMap item in accessor.Collection)
						{
							yield return item;
						}
					}
				}
			}

			List<AbstractCaptionResourceAccessor> list;
		}

		#endregion

		/// <summary>
		/// Registers the current accessor with the active bundle.
		/// </summary>
		private void RegisterAccessorWithActiveBundle()
		{
			ResourceBundle bundle = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default);
			AccessorsCollection accessors = AbstractCaptionResourceAccessor.GetAccessors (bundle);

			//	Associate a collection of caption accessors to the bundle, so we
			//	can later on iterate over all caption items, managed by different,
			//	separate accessors :

			if (accessors == null)
			{
				accessors = new AccessorsCollection ();
				AbstractCaptionResourceAccessor.SetAccessors (bundle, accessors);
			}
			else
			{
				accessors.Remove (this);
			}

			accessors.Add (this);
		}

		private static void SetAccessors(DependencyObject obj, AccessorsCollection collection)
		{
			if (collection == null)
			{
				obj.ClearValue (AbstractCaptionResourceAccessor.AccessorsProperty);
			}
			else
			{
				obj.SetValue (AbstractCaptionResourceAccessor.AccessorsProperty, collection);
			}
		}

		private static AccessorsCollection GetAccessors(DependencyObject obj)
		{
			return obj.GetValue (AbstractCaptionResourceAccessor.AccessorsProperty) as AccessorsCollection;
		}


		private static readonly DependencyProperty AccessorsProperty = DependencyProperty.RegisterAttached ("Accessors", typeof (AccessorsCollection), typeof (AbstractCaptionResourceAccessor), new DependencyPropertyMetadata ().MakeNotSerializable ());
	}
}
