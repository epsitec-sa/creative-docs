//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe Copier permet de réaliser des copies de données simples.
	/// </summary>
	public static class Copier
	{
		public static object Copy(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is string)
			{
				return obj;
			}
			if (obj is System.ValueType)
			{
				return obj;
			}
			
			//	TODO: compléter...

			throw new System.NotSupportedException (string.Format ("Cannot copy type {0}, not supported", obj.GetType ().Name));
		}
		
		public static T[] CopyArray<T>(T[] array)
		{
			if (array == null)
			{
				return null;
			}
			else
			{
				T[] copy = new T[array.Length];
				array.CopyTo (copy, 0);
				return copy;
			}
		}
		
		public static T[] CopyArray<T>(System.Collections.Generic.ICollection<T> collection)
		{
			T[] copy = new T[collection.Count];
			collection.CopyTo (copy, 0);
			return copy;
		}
	}
}
