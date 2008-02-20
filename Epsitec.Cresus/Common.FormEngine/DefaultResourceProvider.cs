//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// The <c>DefaultResourceProvider</c> class implements a simple version of
	/// the <see cref="IFormResourceProvider"/> interface, based on a real
	/// resource manager.
	/// </summary>
	public class DefaultResourceProvider : IFormResourceProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultResourceProvider"/> class.
		/// </summary>
		/// <param name="resourceManager">The resource manager.</param>
		public DefaultResourceProvider(ResourceManager resourceManager)
		{
			this.resourceManager = resourceManager;
		}

		#region IFormResourceProvider Members

		/// <summary>
		/// Clears the cached information.
		/// </summary>
		public void ClearCache()
		{
		}

		/// <summary>
		/// Gets the XML source for the specified form.
		/// </summary>
		/// <param name="formId">The form id.</param>
		/// <returns>The XML source or <c>null</c>.</returns>
		public string GetFormXmlSource(Druid formId)
		{
			ResourceBundle bundle = this.resourceManager.GetBundle (formId);

			if (bundle == null)
			{
				return null;
			}
			else
			{
				return bundle[Support.ResourceAccessors.FormResourceAccessor.Strings.XmlSource].AsString;
			}
		}

		#endregion

		#region IStructuredTypeResolver Members

		/// <summary>
		/// Gets the structured type for the specified id.
		/// </summary>
		/// <param name="id">The id for the structured type.</param>
		/// <returns>The structured type or <c>null</c>.</returns>
		public StructuredType GetStructuredType(Druid id)
		{
			return this.resourceManager.GetStructuredType (id);
		}

		#endregion

		#region ICaptionResolver Members

		/// <summary>
		/// Gets the caption for the specified id.
		/// </summary>
		/// <param name="captionId">The caption id.</param>
		/// <returns>The caption <c>null</c>.</returns>
		public Caption GetCaption(Druid captionId)
		{
			return CaptionCache.Instance.GetCaption (this.resourceManager, captionId);
		}

		#endregion

		private readonly ResourceManager resourceManager;
	}
}
