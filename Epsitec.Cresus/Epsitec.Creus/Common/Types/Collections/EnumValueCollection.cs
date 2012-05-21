//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
