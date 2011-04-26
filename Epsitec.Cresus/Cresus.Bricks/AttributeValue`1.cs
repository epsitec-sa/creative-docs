//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Bricks
{
	public sealed class AttributeValue<T> : AttributeValue
	{
		public AttributeValue(T value)
		{
			this.value = value;
		}
		
		public T								Value
		{
			get
			{
				return this.value;
			}
		}
		
		protected override object GetInternalValue()
		{
			return this.value;
		}
		
		protected override bool ContainsInternalValue(System.Type type)
		{
			return typeof (T) == type;
		}
		
		
		private readonly T						value;
	}
}
