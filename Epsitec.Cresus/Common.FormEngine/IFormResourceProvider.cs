//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// The <c>IFormResourceProvider</c> interface defines the methods needed
	/// by the form engine to access data stored in the resources.
	/// </summary>
	public interface IFormResourceProvider : IStructuredTypeResolver
	{
		/// <summary>
		/// Clears the cached information.
		/// </summary>
		void ClearCache();
		
		/// <summary>
		/// Gets the XML source for the specified form.
		/// </summary>
		/// <param name="formId">The form id.</param>
		/// <returns>The XML source or <c>null</c>.</returns>
		string GetFormXmlSource(Druid formId);

		/// <summary>
		/// Gets the default label for the specified caption.
		/// </summary>
		/// <param name="captionId">The caption id.</param>
		/// <returns>The default label or <c>null</c>.</returns>
		string GetCaptionDefaultLabel(Druid captionId);
		
		
		ResourceManager ResourceManager
		{
			get;
		}
	}
}
