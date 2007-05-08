//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	public class StringResourceAccessor : AbstractResourceAccessor
	{
		public StringResourceAccessor()
			: base (new ResourceBrokers.StringDataBroker ())
		{
		}

		public override void Load(ResourceManager manager)
		{
			this.Initialize (manager);

			ResourceBundle bundle = manager.GetBundle (Resources.StringsBundleName, ResourceLevel.Default);

			this.LoadFromBundle (bundle, "00");
		}

		private void LoadFromBundle(ResourceBundle bundle, string twoLetterISOLanguageName)
		{
			if (bundle != null)
			{
				using (this.SuspendNotifications ())
				{
					int module = bundle.Module.Id;

					foreach (ResourceBundle.Field field in bundle.Fields)
					{
						Druid id = new Druid (field.Id, module);

						CultureMap item = this.Collection[id] ?? new CultureMap (this, id);
						StructuredData data = new StructuredData (Res.Types.ResourceString);

						data.SetValue (Res.Fields.ResourceString.Text, field.AsString);
						data.SetValue (Res.Fields.ResourceString.About, field.About);

						item.Name = field.Name;
						item.RecordCultureData (twoLetterISOLanguageName, data);

						this.Collection.Add (item);
					}
				}
			}
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
