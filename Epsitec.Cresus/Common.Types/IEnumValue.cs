//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IEnumValue</c> interface describes values defined by enumerations.
	/// </summary>
	public interface IEnumValue : ICaption, IName
	{
		/// <summary>
		/// Gets the <see cref="System.Enum"/> value of the enumeration value.
		/// </summary>
		/// <value>The enumeration value.</value>
		System.Enum Value
		{
			get;
		}

		/// <summary>
		/// Gets the rank of the enumeration value. See<see cref="T:RankAttribute"/>
		/// attribute.
		/// </summary>
		/// <value>The rank of the enumeration value.</value>
		int Rank
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether this enumeration value is hidden. See
		/// <see cref="T:HiddenAttribute"/> attribute.
		/// </summary>
		/// <value><c>true</c> if this enumeration value is hidden; otherwise,
		/// <c>false</c>.</value>
		bool IsHidden
		{
			get;
		}
	}
}
