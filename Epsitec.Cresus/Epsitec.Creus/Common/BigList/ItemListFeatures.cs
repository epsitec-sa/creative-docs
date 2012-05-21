//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	/// <summary>
	/// The <c>ItemListFeatures</c> define the behavior of an <see cref="ItemList"/>.
	/// </summary>
	public class ItemListFeatures
	{
		/// <summary>
		/// Gets or sets the selection mode (multiple, single, etc.)
		/// </summary>
		/// <value>
		/// The selection mode.
		/// </value>
		public ItemSelectionMode				SelectionMode
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether row margins are enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if row margins are enabled; otherwise, <c>false</c>.
		/// </value>
		public bool								EnableRowMargins
		{
			get;
			set;
		}
	}
}
