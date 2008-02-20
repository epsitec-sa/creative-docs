//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		/// Suggestion mode; the placeholder value defines the hint text in
		/// an active search field.
		/// </summary>
		DisplayActiveHint,

		/// <summary>
		/// Suggestion mode; the placeholder value defines the hint text in
		/// a passive search field.
		/// </summary>
		DisplayPassiveHint,

		/// <summary>
		/// Suggestion mode; the placeholder value defines the text; this is a
		/// special mode used only to reset what the user typed into the fields.
		/// </summary>
		DisplayHintResetText,
	}
}
     