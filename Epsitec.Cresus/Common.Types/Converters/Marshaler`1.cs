﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Common.Types.Converters
{
	public abstract class Marshaler<T> : Marshaler
	{
		public System.Func<T> ValueGetter
		{
			get;
			internal set;
		}

		public System.Action<T> ValueSetter
		{
			get;
			internal set;
		}

		public T InitialValue
		{
			get;
			internal set;
		}

		public abstract void SetValue(T value);

		public abstract T GetValue();

		public override sealed System.Type MarshaledType
		{
			get
			{
				return typeof (T);
			}
		}

		protected override sealed object GetObjectValue()
		{
			return this.GetValue ();
		}
	}
}