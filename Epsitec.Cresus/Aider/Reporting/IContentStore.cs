//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Reporting
{
	/// <summary>
	/// The <c>IContentStore</c> interface provides access to blob data
	/// serialization.
	/// </summary>
	public interface IContentStore
	{
		/// <summary>
		/// Gets the associated serialization format.
		/// </summary>
		/// <value>The format.</value>
		string Format
		{
			get;
		}

		/// <summary>
		/// Gets the content data as a BLOB.
		/// </summary>
		/// <returns>The BLOB.</returns>
		byte[] GetContentBlob();

		/// <summary>
		/// Setups the content data based on the specified BLOB.
		/// </summary>
		/// <param name="blob">The BLOB.</param>
		/// <returns>Itself or an equivalent content.</returns>
		IContentStore Setup(byte[] blob);
	}
}

