//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityExpressionEncoding</c> enumeration lists the encodings
	/// which can be used to represent an expression.
	/// </summary>
	public enum EntityExpressionEncoding
	{
		/// <summary>
		/// Invalid encoding.
		/// </summary>
		Invalid,

		/// <summary>
		/// C# lambda expression, in source code.
		/// </summary>
		LambdaCSharpSourceCode,
	}
}
