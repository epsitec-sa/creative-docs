//	Copyright © 2007-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>TypeCode</c> enumeration lists all well known types, for which
	/// there is a <see cref="AbstractType"/> derived class.
	/// </summary>
	[DesignerVisible]
	public enum TypeCode
	{
		Invalid=0,

		Boolean=10,
		Integer,
		LongInteger,
		Decimal,
		Double,

		DateTime=20,
		Date,
		Time,

		Binary=30,
		String,
		Enum,
		
		Structured,
		Dynamic,

		Collection=40,

		Other = 100,
	}
}
