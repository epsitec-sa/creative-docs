//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ICaptionResolver</c> interface is used to resolve an id into a
	/// <see cref="Caption"/> instance.
	/// </summary>
	public interface ICaptionResolver
	{
		/// <summary>
		/// Gets the caption for the specified id.
		/// </summary>
		/// <param name="id">The caption id.</param>
		/// <returns>The caption <c>null</c>.</returns>
		Caption GetCaption(Druid id);
	}
}
