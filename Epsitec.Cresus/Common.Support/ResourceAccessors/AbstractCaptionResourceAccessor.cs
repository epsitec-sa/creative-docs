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
			: base (new ResourceBrokers.StringDataBroker ())
		{
		}

		public override void Load(ResourceManager manager)
		{
			this.Initialize (manager);

			ResourceBundle bundle = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default);

			this.LoadFromBundle (bundle, Resources.DefaultTwoLetterISOLanguageName);
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
				item.RecordCultureData (twoLetterISOLanguageName, data);
			}
			else
			{
				data = this.LoadFromField (field, bundle.Module.Id, twoLetterISOLanguageName);
			}

			return data;
		}

		protected abstract IStructuredType GetStructuredType();

		protected override Druid CreateId()
		{
			ResourceBundle bundle = this.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default);

			int devId   = 0;
			int localId = -1;

			foreach (ResourceBundle.Field field in bundle.Fields)
			{
				Druid id = field.Id;

				System.Diagnostics.Debug.Assert (id.IsValid);

				if (id.Developer == devId)
				{
					localId = System.Math.Max (localId, id.Local);
				}
			}

			return new Druid (bundle.Module.Id, devId, localId+1);
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

			Caption caption = this.GetCaptionFromData (data, item.Name);
			string  name    = this.GetFieldNameFromName (data, item.Name);
			string  about   = data.GetValue (Res.Fields.Resource.Comment) as string;
			object  modId   = data.GetValue (Res.Fields.Resource.ModificationId);
			
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
				caption = this.GetCaptionFromData (data, null);
				about   = data.GetValue (Res.Fields.Resource.Comment) as string;
				modId   = data.GetValue (Res.Fields.Resource.ModificationId);
				
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

		protected abstract string GetFieldNameFromName(Types.StructuredData data, string name);

		protected abstract Caption GetCaptionFromData(Types.StructuredData data, string name);

		protected abstract void FillDataFromCaption(CultureMap item, Types.StructuredData data, Caption caption);

		protected abstract string GetNameFromFieldName(string fieldName);

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


		private Types.StructuredData LoadFromField(ResourceBundle.Field field, int module, string twoLetterISOLanguageName)
		{
			Druid id = new Druid (field.Id, module);
			bool insert = false;

			CultureMap item = this.Collection[id];

			if (item == null)
			{
				item   = new CultureMap (this, id);
				insert = true;
			}

			Types.StructuredData data = new Types.StructuredData (this.GetStructuredType ());

			Caption caption = new Caption ();
			string  name    = string.IsNullOrEmpty (field.Name) ? null : this.GetNameFromFieldName (field.Name);
			
			caption.DeserializeFromString (field.AsString);

			this.FillDataFromCaption (item, data, caption);
			data.SetValue (Res.Fields.Resource.Comment, field.About);
			data.SetValue (Res.Fields.Resource.ModificationId, field.ModificationId);

			item.Name = name ?? item.Name;
			item.RecordCultureData (twoLetterISOLanguageName, data);

			if (insert)
			{
				this.Collection.Add (item);
			}

			return data;
		}
	}
}
