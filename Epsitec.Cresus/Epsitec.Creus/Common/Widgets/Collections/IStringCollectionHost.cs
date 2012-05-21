//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// The <c>IStringCollectionHost</c> interface must be implemented by users
	/// of the <see cref="StringCollection"/> class.
	/// </summary>
	public interface IStringCollectionHost
	{
		void NotifyStringCollectionChanged();

		StringCollection Items
		{
			get;
		}
	}
}
