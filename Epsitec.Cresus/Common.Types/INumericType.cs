//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>INumericType</c> interface describes a numeric type which is
	/// constrained by a <see cref="T:DecimalRange"/>.
	/// </summary>
	public interface INumericType : INamedType
	{
		/// <summary>
		/// Gets the range of values accepted by this numeric type.
		/// </summary>
		/// <value>The range of values.</value>
		DecimalRange Range
		{
			get;
		}

		/// <summary>
		/// Gets the default increment step.
		/// </summary>
		/// <value>The default increment step.</value>
		decimal DefaultIncrementStep
		{
			get;
		}
	}
}
