//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe EmptyEnumerator impl�mente un �num�rateur par d�faut pour
	/// toutes les collections vides.
	/// </summary>
	public sealed class EmptyEnumerator : System.Collections.IEnumerator
	{
		public EmptyEnumerator()
		{
		}
		
		
		#region IEnumerator Members
		public void Reset()
		{
		}
		
		public object Current
		{
			get
			{
				return null;
			}
		}
		
		public bool MoveNext()
		{
			return false;
		}
		#endregion
	}
}
