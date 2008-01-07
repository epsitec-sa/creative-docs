//	Copyright © 2005-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>EnumValueCollection</c> represents a collection of <see cref="EnumValue"/> items.
	/// </summary>
	public class EnumValueCollection : HostedList<EnumValue>, IReadOnlyLock
	{
		public EnumValueCollection()
			: base (null)
		{
		}
	}
}
