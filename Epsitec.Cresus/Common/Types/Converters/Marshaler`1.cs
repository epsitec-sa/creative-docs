//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	public abstract class Marshaler<T> : Marshaler
	{
		public System.Func<T>					ValueGetter
		{
			get;
			internal set;
		}

		public System.Action<T>					ValueSetter
		{
			get;
			internal set;
		}

		public T								InitialValue
		{
			get;
			internal set;
		}

		public override sealed System.Type		MarshaledType
		{
			get
			{
				return typeof (T);
			}
		}

		
		public abstract void SetValue(T value);

		public abstract T GetValue();

		
		protected override sealed object GetObjectValue()
		{
			return this.GetValue ();
		}
	}
}