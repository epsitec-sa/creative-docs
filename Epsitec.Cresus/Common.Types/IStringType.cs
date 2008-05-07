//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IStringType</c> interface describes a text type.
	/// </summary>
	public interface IStringType : INamedType
	{
		/// <summary>
		/// Gets the minimum length for the text.
		/// </summary>
		/// <value>The minimum length.</value>
		int MinimumLength
		{
			get;
		}

		/// <summary>
		/// Gets the maximum length for the text.
		/// </summary>
		/// <value>The maximum length.</value>
		int MaximumLength
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the strings use fixed length storage
		/// (this information is required by the database engine, for instance).
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the strings use fixed length storage; otherwise, <c>false</c>.
		/// </value>
		bool UseFixedLengthStorage
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the strings use multilingual storage.
		/// storage.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the strings use multilingual storage; otherwise, <c>false</c>.
		/// </value>
		bool UseMultilingualStorage
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the strings store formatted text.
		/// </summary>
		/// <value><c>true</c> if the strings store formatted text; otherwise, <c>false</c>.</value>
		bool UseFormattedText
		{
			get;
		}


		/// <summary>
		/// Gets the default search behavior for this string type.
		/// </summary>
		/// <value>The default search behavior.</value>
		StringSearchBehavior DefaultSearchBehavior
		{
			get;
		}

		/// <summary>
		/// Gets the default comparison behavior for this string type.
		/// </summary>
		/// <value>The default comparison behavior.</value>
		StringComparisonBehavior DefaultComparisonBehavior
		{
			get;
		}
	}
}
