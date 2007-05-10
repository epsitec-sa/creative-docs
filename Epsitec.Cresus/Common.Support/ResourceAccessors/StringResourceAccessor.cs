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
			: base (new ResourceBrokers.StringDataBroker ())
		{
		}

		public override void Load(ResourceManager manager)
		{
			this.Initialize (manager);

			ResourceBundle bundle = this.ResourceManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Default);

			this.LoadFromBundle (bundle, Resources.DefaultTwoLetterISOLanguageName);
		}

		public override Types.StructuredData LoadCultureData(CultureMap item, string twoLetterISOLanguageName)
		{
			CultureInfo          culture = Resources.FindCultureInfo (twoLetterISOLanguageName);
			ResourceBundle       bundle  = this.ResourceManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Localized, culture);
			ResourceBundle.Field field   = bundle == null ? ResourceBundle.Field.Empty : bundle[item.Id];
			Types.StructuredData data    = null;

			if (field.IsEmpty)
			{
				data = new Types.StructuredData (Res.Types.ResourceString);
				item.RecordCultureData (twoLetterISOLanguageName, data);
			}
			else
			{
				data = this.LoadFromField (field, bundle.Module.Id, twoLetterISOLanguageName);
			}

			return data;
		}

		protected override Druid CreateId()
		{
			ResourceBundle bundle = this.ResourceManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Default);

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

			ResourceBundle bundle;
			CultureInfo culture;

			bundle = this.ResourceManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Default);

			ResourceBundle.Field field = bundle[item.Id];

			if (field.IsEmpty)
			{
				field = bundle.CreateField (ResourceFieldType.Data);
				field.SetDruid (item.Id);
				bundle.Add (field);
			}

			string text = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName).GetValue (Res.Fields.ResourceString.Text) as string;
			string about = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName).GetValue (Res.Fields.ResourceString.Comment) as string;

			field.SetName (item.Name);
			field.SetStringValue (text);
			field.SetAbout (about);

			foreach (string twoLetterISOLanguageName in item.GetDefinedCultures ())
			{
				if (twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName)
				{
					continue;
				}

				culture = Resources.FindCultureInfo (twoLetterISOLanguageName);
				bundle  = this.ResourceManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Localized, culture);

				if (bundle == null)
				{
					bundle = ResourceBundle.Create (this.ResourceManager, this.ResourceManager.ActivePrefix, this.ResourceManager.GetModuleFromFullId (item.Id.ToString ()), Resources.StringsBundleName, ResourceLevel.Localized, culture, 0);
					bundle.DefineType ("String");
					this.ResourceManager.SetBundle (bundle, ResourceSetMode.InMemory);
				}
				
				field = bundle[item.Id];

				if (field.IsEmpty)
				{
					field = bundle.CreateField (ResourceFieldType.Data);
					field.SetDruid (item.Id);
					bundle.Add (field);
				}

				Types.StructuredData data = item.GetCultureData (twoLetterISOLanguageName);
				
				if (Types.UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceString.Text)))
				{
					bundle.Remove (bundle.IndexOf (item.Id));
				}
				else
				{
					text  = data.GetValue (Res.Fields.ResourceString.Text) as string;
					about = data.GetValue (Res.Fields.ResourceString.Comment) as string;
					
					field.SetStringValue (text);
					field.SetAbout (about);
				}
			}
		}

		private void LoadFromBundle(ResourceBundle bundle, string twoLetterISOLanguageName)
		{
			using (this.SuspendNotifications ())
			{
				int module = bundle.Module.Id;

				foreach (ResourceBundle.Field field in bundle.Fields)
				{
					this.LoadFromField (field, module, twoLetterISOLanguageName);
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

			Types.StructuredData data = new Types.StructuredData (Res.Types.ResourceString);

			data.SetValue (Res.Fields.ResourceString.Text, field.AsString);
			data.SetValue (Res.Fields.ResourceString.Comment, field.About);

			item.Name = field.Name ?? item.Name;
			item.RecordCultureData (twoLetterISOLanguageName, data);

			if (insert)
			{
				this.Collection.Add (item);
			}

			return data;
		}
	}
}
