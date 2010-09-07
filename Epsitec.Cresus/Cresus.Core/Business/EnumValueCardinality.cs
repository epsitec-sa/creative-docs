//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>EnumValueCardinality</c> defines how many values can be used for
	/// a given enumeration (see <see cref="EnumValueArticleParameterDefinitionEntity"/>).
	/// </summary>
	[DesignerVisible]
	public enum EnumValueCardinality
	{
		/// <summary>
		/// 0 or 1
		/// </summary>
		ZeroOrOne	= 0,
		
		/// <summary>
		/// 1
		/// </summary>
		ExactlyOne	= 1,

		/// <summary>
		/// 1...n
		/// </summary>
		AtLeastOne	= 2,

		/// <summary>
		/// 0...n
		/// </summary>
		Any			= 3,
	}
}
