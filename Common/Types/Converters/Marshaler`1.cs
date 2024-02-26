//	Copyright © 2010-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types.Converters
{
    public abstract class Marshaler<T> : Marshaler
    {
        public System.Func<T> ValueGetter { get; internal set; }

        public System.Action<T> ValueSetter { get; internal set; }

        public T InitialValue { get; internal set; }

        public sealed override System.Type MarshaledType
        {
            get { return typeof(T); }
        }

        public abstract void SetValue(T value);

        public abstract T GetValue();

        protected sealed override object GetObjectValue()
        {
            return this.GetValue();
        }
    }
}
