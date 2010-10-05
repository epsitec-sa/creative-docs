//	Copyright © 2004-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// La classe EmptyEnumerator implémente un énumérateur par défaut pour
	/// toutes les collections vides.
	/// </summary>
	/// <typeparam name="T">The type of the expected items.</typeparam>
	public sealed class EmptyEnumerator<T> : System.Collections.Generic.IEnumerator<T>
	{
		private EmptyEnumerator()
		{
		}

		#region IEnumerator Members

		public void Reset()
		{
		}

		public T Current
		{
			get
			{
				return default (T);
			}
		}

		public bool MoveNext()
		{
			return false;
		}
		
		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion

		#region IEnumerator Members

		object System.Collections.IEnumerator.Current
		{
			get
			{
				return null;
			}
		}

		#endregion

		public static readonly EmptyEnumerator<T> Instance = new EmptyEnumerator<T> ();
	}
}
