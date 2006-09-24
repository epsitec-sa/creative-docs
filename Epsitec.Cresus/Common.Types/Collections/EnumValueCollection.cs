//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>EnumValueCollection</c> represents a collection of <see cref="EnumValue"/> items.
	/// </summary>
	public class EnumValueCollection : HostedList<EnumValue>
	{
		public EnumValueCollection()
			: base (null)
		{
		}

		/// <summary>
		/// Makes this collection read only. Any further modification is prohibited
		/// and will throw an exception.
		/// </summary>
		public void Lock()
		{
			this.isReadOnly = true;
		}

		protected override void NotifyBeforeChange()
		{
			if (this.isReadOnly)
			{
				throw new System.InvalidOperationException ("The collection is read only");
			}

			base.NotifyBeforeChange ();
		}

		private bool isReadOnly;
	}
}
