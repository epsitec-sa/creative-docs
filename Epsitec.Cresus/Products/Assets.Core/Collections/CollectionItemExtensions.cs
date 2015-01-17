//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System;
using System.Threading;

namespace Epsitec.Cresus.Assets.Core.Collections
{
	public static class CollectionItemExtensions
	{
		public static void Subscribe<T>(this System.IObservable<T> observable, System.Func<T, bool> func, CancellationTokenSource cts)
		{
			observable.Subscribe (item =>
			{
				if (func (item) == false)
				{
					cts.Cancel ();
				}
			});
		}
	}
}
