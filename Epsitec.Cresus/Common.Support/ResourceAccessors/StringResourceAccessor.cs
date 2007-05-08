//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	class StringResourceAccessor : AbstractResourceAccessor
	{
		public StringResourceAccessor()
			: base (new ResourceBrokers.StringDataBroker ())
		{
		}

		public override void Load(ResourceManager manager)
		{
			this.Initialize (manager);

			ResourceBundle bundle = manager.GetBundle (Resources.StringsBundleName, ResourceLevel.Default);

			foreach (ResourceBundle.Field field in bundle.Fields)
			{
				CultureMap item = new CultureMap (this, field.Id);
				StructuredData data = new StructuredData ();

				item.Name = field.Name;
				item.RecordCultureData ("00", data);

				this.Collection.Add (item);
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
