//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>PlaceholderSuggestionMode</c> enumeration defines how the
	/// <see cref="Placeholder"/> class manages its text fields (e.g. if they
	/// should display hints while interactively searching for information).
	/// </summary>
	public enum PlaceholderSuggestionMode
	{
		/// <summary>
		/// No suggestion mode. This is the default behaviour when editing and
		/// inputting data.
		/// </summary>
		None,

		/// <summary>
		/// Suggestion mode; the placeholder value defines the hint text.
		/// </summary>
		DisplayActiveHint,

		DisplayPassiveHint,

		/// <summary>
		/// Suggestion mode; the placeholder value defines the text; this is a
		/// special mode used only to reset what the user typed into the fields.
		/// </summary>
		DisplayHintResetText,
	}
}
     