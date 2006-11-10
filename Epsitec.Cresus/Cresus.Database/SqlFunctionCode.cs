//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlFunctionCode</c> defines all SQL functions.
	/// </summary>
	public enum SqlFunctionCode
	{
		Unknown,

		MathAdd,								//	a + b
		MathSubstract,							//	a - b
		MathMultiply,							//	a * b
		MathDivide,								//	a / b

		CompareEqual,							//	a = b
		CompareNotEqual,						//	a <> b
		CompareLessThan,						//	a < b
		CompareLessThanOrEqual,					//	a <= b
		CompareGreaterThan,						//	a > b
		CompareGreaterThanOrEqual,				//	a <= b
		CompareIsNull,							//	a IS NULL
		CompareIsNotNull,						//	a NOT IS NULL
		CompareLike,							//	a LIKE b
		CompareNotLike,							//	a NOT LIKE b
		CompareFalse,							//	0 = 1
		CompareTrue,							//	1 = 1

		SetIn,									//	a IN b
		SetNotIn,								//	a NOT IN b
		SetBetween,								//	a BETWEEN b AND c
		SetNotBetween,							//	a NOT BETWEEN b AND c
		SetExists,								//	a EXISTS
		SetNotExists,							//	a NOT EXISTS

		LogicNot,								//	NOT a
		LogicAnd,								//	a AND b
		LogicOr,								//	a OR b

		Substring,								//	SUBSTRING(a FROM b FOR c)
		Upper,									//	UPPER(a)
		Cast,									//	CAST(a AS b)

		//	Equivalents :

		CompareNotLessThan = CompareGreaterThanOrEqual,
		CompareNotGreaterThan = CompareLessThanOrEqual,
		CompareNotLessThanOrEqual = CompareGreaterThan,
		CompareNotGreaterThanOrEqual = CompareLessThan,
	}
}
