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

			ResourceBundle bundle = this.ResourceManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Default);

			this.LoadFromBundle (bundle, Resources.DefaultTwoLetterISOLanguageName);
		}

		public override Types.StructuredData LoadCultureData(CultureMap item, string twoLetterISOLanguageName)
		{
			ResourceBundle bundle;

			if (twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName)
			{
				bundle = this.ResourceManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Default);
			}
			else
			{
				CultureInfo culture = Resources.FindCultureInfo (twoLetterISOLanguageName);
				bundle = this.ResourceManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Localized, culture);
			}

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

		public override IDataBroker GetDataBroker(StructuredData container, string fieldId)
		{
			return base.GetDataBroker (container, fieldId);
		}

		protected override Druid CreateId()
		{
			ResourceBundle bundle = this.ResourceManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Default);
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

			StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);

			string text  = data.GetValue (Res.Fields.ResourceString.Text) as string;
			string about = data.GetValue (Res.Fields.ResourceBase.Comment) as string;
			object modId = data.GetValue (Res.Fields.ResourceBase.ModificationId);

			field.SetName (item.Name);
			field.SetStringValue (text);
			field.SetAbout (about);

			StringResourceAccessor.SetModificationId (field, modId);

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

				data = item.GetCultureData (twoLetterISOLanguageName);
				
				if (Types.UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceString.Text)))
				{
					bundle.Remove (bundle.IndexOf (item.Id));
				}
				else
				{
					text  = data.GetValue (Res.Fields.ResourceString.Text) as string;
					about = data.GetValue (Res.Fields.ResourceBase.Comment) as string;
					modId = data.GetValue (Res.Fields.ResourceBase.ModificationId);
					
					field.SetStringValue (text);
					field.SetAbout (about);

					StringResourceAccessor.SetModificationId (field, modId);
				}
			}
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
			data.SetValue (Res.Fields.ResourceBase.Comment, field.About);
			data.SetValue (Res.Fields.ResourceBase.ModificationId, field.ModificationId);

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
