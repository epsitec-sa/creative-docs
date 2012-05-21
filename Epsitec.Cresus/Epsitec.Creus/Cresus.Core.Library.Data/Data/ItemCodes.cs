//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>ItemCodes</c> class provides <see cref="ItemCode"/> singletons for
	/// all well-known items.
	/// </summary>
	public static class ItemCodes
	{
		/// <summary>
		/// The <c>ImageCategories</c> class provides <see cref="ItemCode"/> singletons
		/// for all well-known image categories (see <see cref="ImageCategoryEntity"/>).
		/// </summary>
		public static class ImageCategories
		{
			public static readonly ItemCode Person = ItemCode.Create ("Person");
		}

		/// <summary>
		/// The <c>ImageGroups</c> class provides <see cref="ItemCode"/> singletons
		/// for all well-known image groups (see <see cref="ImageGroupEntity"/>).
		/// </summary>
		public static class ImageGroups
		{
			public static readonly ItemCode ImageRefPhoto = ItemCode.Create ("RefPhoto");
		}
	}
}
