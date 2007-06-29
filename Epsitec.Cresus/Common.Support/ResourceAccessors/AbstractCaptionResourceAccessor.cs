//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

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
		public AbstractCaptionResourceAccessor()
		{
		}

		public override void Load(ResourceManager manager)
		{
			this.Initialize (manager);

			ResourceBundle bundle = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default);

			this.LoadFromBundle (bundle, Resources.DefaultTwoLetterISOLanguageName);
		}

		public override CultureMap CreateItem()
		{
			return this.CreateItem (null, this.CreateId ());
		}

		public override Types.StructuredData LoadCultureData(CultureMap item, string twoLetterISOLanguageName)
		{
			CultureInfo          culture = Resources.FindCultureInfo (twoLetterISOLanguageName);
			ResourceBundle       bundle  = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Localized, culture);
			ResourceBundle.Field field   = bundle == null ? ResourceBundle.Field.Empty : bundle[item.Id];
			Types.StructuredData data    = null;

			if (field.IsEmpty)
			{
				data = new Types.StructuredData (this.GetStructuredType ());
				this.FillDataFromCaption (item, data, new Caption ());
				item.RecordCultureData (twoLetterISOLanguageName, data);
			}
			else
			{
				data = this.LoadFromField (field, bundle.Module.Id, twoLetterISOLanguageName);
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
			return AbstractResourceAccessor.CreateId (bundle);
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

			ResourceBundle bundle;
			CultureInfo culture;

			bundle = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default);

			ResourceBundle.Field field = bundle[item.Id];

			if (field.IsEmpty)
			{
				field = bundle.CreateField (ResourceFieldType.Data);
				field.SetDruid (item.Id);
				bundle.Add (field);
			}

			Types.StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);

			Caption caption = this.GetCaptionFromData (bundle, data, item.Name, Resources.DefaultTwoLetterISOLanguageName);
			string  name    = this.GetFieldNameFromName (item, data);
			string  about   = data.GetValue (Res.Fields.ResourceBase.Comment) as string;
			object  modId   = data.GetValue (Res.Fields.ResourceBase.ModificationId);

			field.SetName (name);
			field.SetStringValue (caption.SerializeToString ());
			field.SetAbout (about);
			StringResourceAccessor.SetModificationId (field, modId);

			this.ResourceManager.ClearCaptionCache (item.Id, ResourceLevel.Default, null);

			foreach (string twoLetterISOLanguageName in item.GetDefinedCultures ())
			{
				if (twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName)
				{
					continue;
				}

				culture = Resources.FindCultureInfo (twoLetterISOLanguageName);
				bundle  = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Localized, culture);

				if (bundle == null)
				{
					bundle = ResourceBundle.Create (this.ResourceManager, this.ResourceManager.ActivePrefix, this.ResourceManager.GetModuleFromFullId (item.Id.ToString ()), Resources.CaptionsBundleName, ResourceLevel.Localized, culture, 0);
					bundle.DefineType ("Caption");
					this.ResourceManager.SetBundle (bundle, ResourceSetMode.InMemory);
				}
				
				field = bundle[item.Id];

				if (field.IsEmpty)
				{
					field = bundle.CreateField (ResourceFieldType.Data);
					field.SetDruid (item.Id);
					bundle.Add (field);
				}

				data    = item.GetCultureData (twoLetterISOLanguageName);
				caption = this.GetCaptionFromData (bundle, data, null, twoLetterISOLanguageName);
				about   = data.GetValue (Res.Fields.ResourceBase.Comment) as string;
				modId   = data.GetValue (Res.Fields.ResourceBase.ModificationId);
				
				if (caption == null)
				{
					bundle.Remove (bundle.IndexOf (item.Id));
				}
				else
				{
					field.SetStringValue (caption.SerializeToString ());
					field.SetAbout (about);
					StringResourceAccessor.SetModificationId (field, modId);
				}

				this.ResourceManager.ClearCaptionCache (item.Id, ResourceLevel.Localized, culture);
				this.ResourceManager.ClearCaptionCache (item.Id, ResourceLevel.Merged, culture);
			}
		}

		protected abstract string GetFieldNameFromName(CultureMap item, Types.StructuredData data);

		protected abstract Caption GetCaptionFromData(ResourceBundle sourceBundle, Types.StructuredData data, string name, string twoLetterISOLanguageName);

		protected abstract void FillDataFromCaption(CultureMap item, Types.StructuredData data, Caption caption);

		protected abstract string GetNameFromFieldName(CultureMap item, string fieldName);

		protected abstract bool FilterField(ResourceBundle.Field field);
		
		private void LoadFromBundle(ResourceBundle bundle, string twoLetterISOLanguageName)
		{
			using (this.SuspendNotifications ())
			{
				int module = bundle.Module.Id;

				foreach (ResourceBundle.Field field in bundle.Fields)
				{
					if (this.FilterField (field))
					{
						this.LoadFromField (field, module, twoLetterISOLanguageName);
					}
				}
			}
		}


		/// <summary>
		/// Loads data from a resource bundle field.
		/// </summary>
		/// <param name="field">The resource bundle field.</param>
		/// <param name="module">The source module id.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <returns>The data which describes the specified resource.</returns>
		private Types.StructuredData LoadFromField(ResourceBundle.Field field, int module, string twoLetterISOLanguageName)
		{
			Druid id     = new Druid (field.Id, module);
			bool  insert = false;

			CultureMap item = this.Collection[id];

			if (item == null)
			{
				item   = this.CreateItem (field, id);
				insert = true;
			}

			Types.StructuredData data = new Types.StructuredData (this.GetStructuredType ());

			Caption caption = new Caption (id);
			string  name    = string.IsNullOrEmpty (field.Name) ? null : this.GetNameFromFieldName (item, field.Name);

			caption.DeserializeFromString (field.AsString, this.ResourceManager);

			this.FillDataFromCaption (item, data, caption);
			data.SetValue (Res.Fields.ResourceBase.Comment, field.About);
			data.SetValue (Res.Fields.ResourceBase.ModificationId, field.ModificationId);

			//	It is important to first associate the culture data, then defining
			//	the item name, since AnyTypeResourceAccessor listens for name changes
			//	in order to update enumeration value names.
			
			item.RecordCultureData (twoLetterISOLanguageName, data);
			item.Name = name ?? item.Name;

			if (insert)
			{
				this.Collection.Add (item);
			}

			return data;
		}

		protected virtual CultureMap CreateItem(ResourceBundle.Field field, Druid id)
		{
			return new CultureMap (this, id);
		}
	}
}
