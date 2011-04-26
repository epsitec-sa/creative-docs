//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Bricks
{
	/// <summary>
	/// The <c>AttributeValue</c> class is used to store attribute values
	/// associated to <see cref="BrickPropertyKey.Attribute"/> properties.
	/// </summary>
	public abstract class AttributeValue
	{
		/// <summary>
		/// Gets the value of the specified type, if possible.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <returns>The value if it can be retrieved; otherwise, <c>default(T)</c>.</returns>
		public T GetValue<T>()
		{
			object value = this.GetInternalValue ();
			if (value is T)
				return (T) value;
			else
				return default (T);
		}

		/// <summary>
		/// Determines whether this attribute contains a value of the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>
		///   <c>true</c> if this attribute contains a value of the specified type; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsValue<T>()
		{
			return this.ContainsInternalValue (typeof (T));
		}
		
		
		protected abstract object GetInternalValue();
		protected abstract bool ContainsInternalValue(System.Type type);
	}
}
