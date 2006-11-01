//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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

		public override bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}

		/// <summary>
		/// Makes this collection read only. Any further modification is prohibited
		/// and will throw an exception.
		/// </summary>
		public void Lock()
		{
			this.isReadOnly = true;
		}

		internal void Unlock()
		{
			this.isReadOnly = false;
		}

		#region IReadOnlyLock Members

		void IReadOnlyLock.Lock()
		{
			this.Lock ();
		}

		void IReadOnlyLock.Unlock()
		{
			this.Unlock ();
		}

		#endregion

		#region IReadOnly Members

		bool IReadOnly.IsReadOnly
		{
			get
			{
				return this.IsReadOnly;
			}
		}

		#endregion
		
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
