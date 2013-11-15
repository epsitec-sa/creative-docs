//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public static class EventExtensions
	{
		public delegate void EventHandler<T>         (object sender, T val1);
		public delegate void EventHandler<T1, T2>    (object sender, T1 val1, T2 val2);
		public delegate void EventHandler<T1, T2, T3>(object sender, T1 val1, T2 val2, T3 val3);

		public static void Raise<T>(this EventHandler<T> handler, object sender, T val1)
		{
			if (handler != null)
			{
				handler (sender, val1);
			}
		}

		public static void Raise<T1, T2>(this EventHandler<T1, T2> handler, object sender, T1 val1, T2 val2)
		{
			if (handler != null)
			{
				handler (sender, val1, val2);
			}
		}

		public static void Raise<T1, T2, T3>(this EventHandler<T1, T2, T3> handler, object sender, T1 val1, T2 val2, T3 val3)
		{
			if (handler != null)
			{
				handler (sender, val1, val2, val3);
			}
		}
	}
}
