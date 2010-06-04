//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters.Marshalers
{
	public abstract class GenericMarshaler<T1, T2> : Marshaler
	{
		public System.Func<T1> ValueGetter
		{
			get;
			set;
		}

		public System.Action<T1> ValueSetter
		{
			get;
			set;
		}

		public T1 InitialValue
		{
			get;
			set;
		}

		public bool UsesNullableType
		{
			get
			{
				return GenericMarshaler<T1, T2>.usesNullableType;
			}
		}

		public GenericConverter<T2> Converter
		{
			get
			{
				return GenericMarshaler<T1, T2>.converter;
			}
		}


		public T1 GetValue()
		{
			if (this.ValueGetter == null)
			{
				return default (T1);
			}
			else
			{
				return this.ValueGetter ();
			}
		}

		public void SetValue(T1 value)
		{
			if (this.ValueSetter == null)
			{
				throw new System.InvalidOperationException ("Cannot set value without a setter");
			}
			else
			{
				this.ValueSetter (value);
			}
		}


		private static readonly bool usesNullableType = typeof (T1).IsGenericType && typeof (T1).FullName.StartsWith ("System.Nullable`1");
		private static readonly GenericConverter<T2> converter = GenericConverter.GetConverter<T2> ();
	}
}
