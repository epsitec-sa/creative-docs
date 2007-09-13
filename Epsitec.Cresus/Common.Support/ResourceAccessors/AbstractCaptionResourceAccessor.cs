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

			if ((refField.IsEmpty || ResourceBundle.Field.IsNullString (refField.AsString)) &&
				(patchField.IsEmpty || ResourceBundle.Field.IsNullString (patchField.AsString)))
			{
				//	There is absolutely no data to start working with for the
				//	current item. Create a data record from scratch :

				StructuredData data    = this.CreateStructuredData ();
				Caption        caption = this.CreateCaption (patchBundle ?? refBundle);
				
				this.FillDataFromCaption (item, data, caption, DataCreationMode.Public);
				item.RecordCultureData (twoLetterISOLanguageName, data);

				return data;
			}
			else
			{
				Types.StructuredData data1 = (refField.IsEmpty   || ResourceBundle.Field.IsNullString (refField.AsString))   ? null : this.LoadFromField (refField, refBundle.Module.Id, twoLetterISOLanguageName);
				Types.StructuredData data2 = (patchField.IsEmpty || ResourceBundle.Field.IsNullString (patchField.AsString)) ? null : this.LoadFromField (patchField, patchBundle.Module.Id, twoLetterISOLanguageName);

				return data1 ?? data2;
			}
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
			return base.GetDataBroker (container, fieldId);
		}

		/// <summary>
		/// Creates a new unique id.
		/// </summary>
		/// <returns>The new unique id.</returns>
		protected override Druid CreateId()
		{
			ResourceBundle      bundle    = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default);
			AccessorsCollection accessors = AbstractCaptionResourceAccessor.GetAccessors (bundle);

			//	Create a new unique id, making sure we don't have any collisions
			//	with ids defined in any other accessor attached to the same bundle
			//	as the current accessor :
			
			return AbstractResourceAccessor.CreateId (accessors.AllCollections, bundle);
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
					bundle = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default);
					this.ClearCaptionCache (item.Id, ResourceLevel.Default, null);
				}
				else
				{
					culture = Resources.FindCultureInfo (twoLetterISOLanguageName);
					bundle  = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Localized, culture);
					this.ClearCaptionCache (item.Id, ResourceLevel.Localized, culture);
					this.ClearCaptionCache (item.Id, ResourceLevel.Merged, culture);
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
		/// Clears the caption cache in the associated resource manager.
		/// </summary>
		/// <param name="id">The resource id.</param>
		/// <param name="level">The resource level.</param>
		/// <param name="culture">The resource culture.</param>
		protected void ClearCaptionCache(Druid id, ResourceLevel level, CultureInfo culture)
		{
//-			this.ResourceManager.ClearCaptionCache (id, level, culture);
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

					ResourceBundle refBundle = refModuleManager.GetBundle (Resources.CaptionsBundleName, level, culture);
					
					if ((this.ComputeDelta (item, ref data, refBundle, item.Id, bundle, twoLetterISOLanguageName)) ||
						((item.Source != CultureMapSource.PatchModule) && (this.IsEmpty (data))))
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

					if (item.Source == CultureMapSource.ReferenceModule)
					{
						item.Source = CultureMapSource.DynamicMerge;
					}
				}
				else
				{
					//	If this is an empty secondary resource, delete the corresponding
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

					string  capName = (level == ResourceLevel.Default) && (item.Source != CultureMapSource.DynamicMerge) ? item.Name : null;
					Caption caption = this.CreateCaptionFromData (bundle, data, capName, twoLetterISOLanguageName);
					string  about   = data.GetValue (Res.Fields.ResourceBase.Comment) as string;
					object  modId   = data.GetValue (Res.Fields.ResourceBase.ModificationId);

					if ((capName == null) &&
						(this.IsEmptyCaption (data)))
					{
						caption = null;
					}
					
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
					this.ClearCaptionCache (item.Id, ResourceLevel.Localized, culture);
					this.ClearCaptionCache (item.Id, ResourceLevel.Merged, culture);
				}
				else
				{
					this.ClearCaptionCache (item.Id, ResourceLevel.Default, null);
				}
#endif
			}
		}

		/// <summary>
		/// Creates an empty data record, attached to the proper type descriptor.
		/// </summary>
		/// <returns>A <see cref="StructuredData"/> instance.</returns>
		protected StructuredData CreateStructuredData()
		{
			return new StructuredData (this.GetStructuredType ());
		}

		/// <summary>
		/// Creates an empty caption.
		/// </summary>
		/// <param name="bundle">The resource bundle where the caption will be stored.</param>
		/// <returns>An empty <see cref="Caption"/> instance.</returns>
		protected Caption CreateCaption(ResourceBundle bundle)
		{
			return this.CreateCaption (bundle, Druid.Empty);
		}

		/// <summary>
		/// Creates an empty caption.
		/// </summary>
		/// <param name="bundle">The resource bundle where the caption will be stored.</param>
		/// <param name="id">The caption id.</param>
		/// <returns>An empty <see cref="Caption"/> instance.</returns>
		protected Caption CreateCaption(ResourceBundle bundle, Druid id)
		{
			Caption caption = new Caption (id);
			ResourceManager.SetSourceBundle (caption, bundle);
			return caption;
		}

		/// <summary>
		/// Gets the structured type which describes the caption data.
		/// </summary>
		/// <returns>The <see cref="StructuredType"/> instance.</returns>
		protected abstract IStructuredType GetStructuredType();

		/// <summary>
		/// Determines whether the specified data record describes an empty
		/// caption.
		/// </summary>
		/// <param name="data">The data record.</param>
		/// <returns>
		/// 	<c>true</c> if this is an empty caption; otherwise, <c>false</c>.
		/// </returns>
		protected abstract bool IsEmptyCaption(StructuredData data);
		
		/// <summary>
		/// Computes the difference between a raw data record and a reference
		/// data record and fills the patch data record with the resulting
		/// delta.
		/// </summary>
		/// <param name="rawData">The raw data record.</param>
		/// <param name="refData">The reference data record.</param>
		/// <param name="patchData">The patch data, which will be filled with the delta.</param>
		protected abstract void ComputeDataDelta(StructuredData rawData, StructuredData refData, StructuredData patchData);

		/// <summary>
		/// Creates a caption based on the definitions stored in a data record.
		/// </summary>
		/// <param name="sourceBundle">The source bundle.</param>
		/// <param name="data">The data record.</param>
		/// <param name="name">The name of the caption.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <returns>A <see cref="Caption"/> instance.</returns>
		protected abstract Caption CreateCaptionFromData(ResourceBundle sourceBundle, Types.StructuredData data, string name, string twoLetterISOLanguageName);

		/// <summary>
		/// Fills the data record from a given caption.
		/// </summary>
		/// <param name="item">The item associated with the data record.</param>
		/// <param name="data">The data record.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="mode">The creation mode for the data record.</param>
		protected abstract void FillDataFromCaption(CultureMap item, Types.StructuredData data, Caption caption, DataCreationMode mode);

		/// <summary>
		/// Gets the pure caption name from a field name. This simply strips of
		/// the field name prefix.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="fieldName">The field name.</param>
		/// <returns>The pure caption name.</returns>
		protected abstract string GetNameFromFieldName(CultureMap item, string fieldName);

		/// <summary>
		/// Gets the resource field name of the resource based on the caption
		/// name.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="data">The data record.</param>
		/// <returns>The resource field name.</returns>
		protected abstract string GetFieldNameFromName(CultureMap item, Types.StructuredData data);

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
			bool insert = false;
			bool record;
			bool freezeName = false;

			Druid id = new Druid (field.Id, module);
			CultureMap item = this.Collection[id];
			CultureMapSource fieldSource = this.GetCultureMapSource (field);
			StructuredData data = this.CreateStructuredData ();

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
					//	for the culture. Merge the basic resource data :

					data = item.GetCultureData (twoLetterISOLanguageName);

					if (!ResourceBundle.Field.IsNullString (field.About))
					{
						data.SetValue (Res.Fields.ResourceBase.Comment, field.About);
					}
					if (field.ModificationId > 0)
					{
						data.SetValue (Res.Fields.ResourceBase.ModificationId, field.ModificationId);
					}

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

			//	Restore the caption definition, then fill the data record while
			//	merging the fields which might need to be merged.
			
			Caption caption = this.CreateCaption (field.ParentBundle, id);
			string  name    = string.IsNullOrEmpty (field.Name) ? null : this.GetNameFromFieldName (item, field.Name);

			caption.DeserializeFromString (field.AsString, this.ResourceManager);

			this.FillDataFromCaption (item, data, caption, DataCreationMode.Public);
			
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

		/// <summary>
		/// Creates a new item which can then be added to the collection.
		/// </summary>
		/// <param name="field">The resource field.</param>
		/// <param name="id">The id for the new item.</param>
		/// <returns>A new <see cref="CultureMap"/> item.</returns>
		protected CultureMap CreateItem(ResourceBundle.Field field, Druid id)
		{
			return this.CreateItem (field, id, this.GetCultureMapSource (null));
		}

		/// <summary>
		/// Creates a new item which can then be added to the collection.
		/// </summary>
		/// <param name="field">The resource field.</param>
		/// <param name="id">The id for the new item.</param>
		/// <param name="source">The source for the data.</param>
		/// <returns>A new <see cref="CultureMap"/> item.</returns>
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

			public IList<AbstractCaptionResourceAccessor> List
			{
				get
				{
					return this.list;
				}
			}

			List<AbstractCaptionResourceAccessor> list;
		}

		#endregion

		/// <summary>
		/// Determines whether the specified data record is empty. This calls
		/// the <see cref="IsEmptyCaption"/> method.
		/// </summary>
		/// <param name="data">The data record.</param>
		/// <returns>
		/// 	<c>true</c> if the specified data record is empty; otherwise, <c>false</c>.
		/// </returns>
		private bool IsEmpty(StructuredData data)
		{
			string comment     = data.GetValue (Res.Fields.ResourceBase.Comment) as string;
			int    modifId     = StringResourceAccessor.GetModificationId (data);

			return (this.IsEmptyCaption (data))
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
		private bool ComputeDelta(CultureMap item, ref StructuredData rawData, ResourceBundle refBundle, Druid druid, ResourceBundle dataBundle, string twoLetterISOLanguageName)
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

			StructuredData refData   = this.CreateStructuredData (item, refBundle, refField, DataCreationMode.Temporary);
			StructuredData patchData = this.CreateStructuredData ();

			int    refModifId = StringResourceAccessor.GetModificationId (refData.GetValue (Res.Fields.ResourceBase.ModificationId));
			int    rawModifId = StringResourceAccessor.GetModificationId (rawData.GetValue (Res.Fields.ResourceBase.ModificationId));
			
			string refComment = refData.GetValue (Res.Fields.ResourceBase.Comment) as string;
			string rawComment = rawData.GetValue (Res.Fields.ResourceBase.Comment) as string;

			if ((rawModifId > 0) &&
				(rawModifId != refModifId))
			{
				patchData.SetValue (Res.Fields.ResourceBase.ModificationId, rawModifId);
			}
			
			if (!ResourceBundle.Field.IsNullString (rawComment) &&
				(rawComment != refComment))
			{
				patchData.SetValue (Res.Fields.ResourceBase.Comment, rawComment);
			}

			this.ComputeDataDelta (rawData, refData, patchData);

			if (patchData.IsEmpty)
			{
				return true;
			}
			else
			{
				rawData = patchData;
				return false;
			}
		}

		/// <summary>
		/// Creates a structured data record, properly initialized based on the
		/// <see cref="StructuredType"/> associated with the accessor's resource
		/// type. The comment and modification id are taken from the field.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="bundle">The resource bundle.</param>
		/// <param name="field">The resource field.</param>
		/// <param name="mode">The creation mode.</param>
		/// <returns>An initialized data record.</returns>
		private StructuredData CreateStructuredData(CultureMap item, ResourceBundle bundle, ResourceBundle.Field field, DataCreationMode mode)
		{
			StructuredData data    = this.CreateStructuredData ();
			Caption        caption = this.CreateCaption (bundle);

			caption.DeserializeFromString (field.AsString, bundle.ResourceManager);

			data.SetValue (Res.Fields.ResourceBase.Comment, field.About);
			data.SetValue (Res.Fields.ResourceBase.ModificationId, field.ModificationId);

			this.FillDataFromCaption (item, data, caption, mode);
			
			return data;
		}

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
