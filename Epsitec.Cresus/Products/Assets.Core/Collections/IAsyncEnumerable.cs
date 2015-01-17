//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Threading;
using System.Threading.Tasks;

namespace Epsitec.Cresus.Assets.Core.Collections
{
	public interface IAsyncEnumerable<T>
	{
		IAsyncEnumerator<T> GetAsyncEnumerator(int index, int count, CancellationToken token);
	}
}

