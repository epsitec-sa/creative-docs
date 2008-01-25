//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.FormEngine
{
	public class DefaultResourceProvider : IFormResourceProvider
	{
		public DefaultResourceProvider(ResourceManager resourceManager)
		{
			this.resourceManager = resourceManager;
		}

		#region IFormResourceProvider Members

		public string GetXmlSource(Druid id)
		{
			ResourceBundle bundle = this.resourceManager.GetBundle (id);

			if (bundle == null)
			{
				return null;
			}
			else
			{
				return bundle[Support.ResourceAccessors.FormResourceAccessor.Strings.XmlSource].AsString;
			}
		}

		public string GetCaptionDefaultLabel(Druid id)
		{
			Caption caption = this.resourceManager.GetCaption (id);

			if (caption == null)
			{
				return null;
			}
			else
			{
				return caption.DefaultLabel;
			}
		}

		public ResourceManager ResourceManager
		{
			get
			{
				return this.resourceManager;
			}
		}

		#endregion

		#region IStructuredTypeProviderId Members

		public StructuredType GetStructuredType(Druid id)
		{
			return this.resourceManager.GetStructuredType (id);
		}

		#endregion


		private readonly ResourceManager resourceManager;
	}
}
