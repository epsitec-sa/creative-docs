//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.ECh
{
	public struct Change<T> : System.IEquatable<Change<T>>
			where T : class
	{
		public Change(T oldValue, T newValue)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		public T OldValue
		{
			get
			{
				return this.oldValue;
			}
		}

		public T NewValue
		{
			get
			{
				return this.newValue;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is Change<T>)
			{
				return this.Equals ((Change<T>) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return (this.oldValue.GetHashCode () * 11) ^ this.newValue.GetHashCode ();
		}

		#region IEquatable<Change<T>> Members

		public bool Equals(Change<T> other)
		{
			return (other.oldValue == this.oldValue)
					&& (other.newValue == this.newValue);
		}

		#endregion

		private readonly T oldValue;
		private readonly T newValue;
	}
}

