//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>IDocumentInfo</c> interface gives access to document information
	/// (a descriptive text and a thumbnail image).
	/// </summary>
	public interface IDocumentInfo
	{
		/// <summary>
		/// Gets the description of the document as a formatted string.
		/// </summary>
		/// <returns>The description of the document.</returns>
		string GetDescription();

		/// <summary>
		/// Gets the thumbnail image of the document.
		/// </summary>
		/// <returns>The thumbnail image or <c>null</c>.</returns>
		Drawing.Image GetThumbnail();

		/// <summary>
		/// Asynchronously gets the image thumbnail of the document.
		/// </summary>
		/// <param name="callback">The callback which will be invoked with the
		/// thumbnail image, as soon as it will be available; this may happen
		/// synchronously when calling <c>GetAsyncThumbnail</c>, or later.</param>
		void GetAsyncThumbnail(SimpleCallback<Drawing.Image> callback);
	}
}
