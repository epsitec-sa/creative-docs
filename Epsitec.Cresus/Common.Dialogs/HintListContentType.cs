//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>HintListContentType</c> enumeration specifies what content type
	/// is displayed by the hint list.
	/// </summary>
	public enum HintListContentType
	{
		/// <summary>
		/// The hint list has no defined content type; no specific header will
		/// be displayed.
		/// </summary>
		Default,
		
		/// <summary>
		/// The hint list displays catalog data, i.e. it is used as a browser
		/// to access a record in a data base.
		/// </summary>
		Catalog,
		
		/// <summary>
		/// The hint list displays suggestions, i.e. it is used as a central
		/// auto complete system.
		/// </summary>
		Suggestions
	}
}
