//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IEnumType</c> interface describes an enumeration.
	/// </summary>
	public interface IEnumType : INamedType
	{
		/// <summary>
		/// Enumerates through the <see cref="IEnumValue"/>, sorted first by
		/// rank, then by name.
		/// </summary>
		/// <value>The sorted enumeration values.</value>
		IEnumerable<IEnumValue> Values
		{
			get;
		}

		/// <summary>
		/// Gets the first <see cref="IEnumValue"/> with the specified name.
		/// </summary>
		/// <value>The <see cref="IEnumValue"/> or <c>null</c> if no match could
		/// be found.</value>
		IEnumValue this[string name]
		{
			get;
		}

		/// <summary>
		/// Gets the first <see cref="IEnumValue"/> with the specified rank.
		/// </summary>
		/// <value>The <see cref="IEnumValue"/> or <c>null</c> if no match could
		/// be found.</value>
		IEnumValue this[int rank]
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether this enumeration is customizable.
		/// </summary>
		/// <value><c>true</c> if this instance is customizable; otherwise,
		/// <c>false</c>.</value>
		bool IsCustomizable
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether this enumeration represents a set
		/// of flags.
		/// </summary>
		/// <value><c>true</c> if this instance represents a set of flags;
		/// otherwise, <c>false</c>.</value>
		bool IsDefinedAsFlags
		{
			get;
		}
	}
}
