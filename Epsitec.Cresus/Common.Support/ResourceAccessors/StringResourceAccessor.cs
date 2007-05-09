//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	using CultureInfo=System.Globalization.CultureInfo;
	
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
			
			this.LoadFromBundle (bundle, "00");
		}

		public override Types.StructuredData LoadCultureData(CultureMap item, string twoLetterISOLanguageName)
		{
			CultureInfo          culture = Resources.FindCultureInfo (twoLetterISOLanguageName);
			ResourceBundle       bundle  = this.ResourceManager.GetBundle (Resources.StringsBundleName, ResourceLevel.Localized, culture);
			ResourceBundle.Field field   = bundle == null ? null : bundle[item.Id];
			Types.StructuredData data    = null;

			if (field != null)
			{
				data = this.LoadFromField (field, bundle.Module.Id, twoLetterISOLanguageName);
			}
			else
			{
				data = new StructuredData (Res.Types.ResourceString);
				item.RecordCultureData (twoLetterISOLanguageName, data);
			}

			return data;
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
			
			StructuredData data = new StructuredData (Res.Types.ResourceString);

			data.SetValue (Res.Fields.ResourceString.Text, field.AsString);
			data.SetValue (Res.Fields.ResourceString.About, field.About);

			item.Name = field.Name ?? item.Name;
			item.RecordCultureData (twoLetterISOLanguageName, data);

			if (insert)
			{
				this.Collection.Add (item);
			}

			return data;
		}

		protected override Druid CreateId()
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		protected override void PersistItem(CultureMap item)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}
	}
}
