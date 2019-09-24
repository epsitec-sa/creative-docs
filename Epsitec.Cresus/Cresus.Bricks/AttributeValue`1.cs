//	Copyright © 2011-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Bricks
{
	public sealed class AttributeValue<T> : AttributeValue
	{
		public AttributeValue(T value)
		{
			this.value = value;
		}

        public AttributeValue(T value, string arg)
            : this (value)
        {
            this.arg = arg;
        }


        public override string                  Arg => this.arg;
		

		protected override object GetInternalValue()
		{
			return this.value;
		}
		
		protected override bool ContainsInternalValue(System.Type type)
		{
			return typeof (T) == type;
		}
		
		
		private readonly T						value;
        private readonly string                 arg;
	}
}
