//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Threading;
using System.Threading.Tasks;

namespace Epsitec.Cresus.Assets.Core.Collections
{
	public interface IAsyncValueProvider<T>
	{
		IAsyncEnumerator<T> GetValuesAsync(int index, int count, CancellationToken token);
	}
}

