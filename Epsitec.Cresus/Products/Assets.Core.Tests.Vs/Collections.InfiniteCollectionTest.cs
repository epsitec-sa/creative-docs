//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Assets.Core.Collections;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading;
using System.Threading.Tasks;

namespace Epsitec.Cresus.Assets.Core.Tests
{
	[TestClass]
	public class InfiniteCollectionTest
	{
		[TestMethod]
		public void CheckBasicBehavior()
		{
			var collection = new InfiniteCollection<string> (new AsyncValueProvider ());

			Assert.IsNull (collection[0]);
			Assert.IsNull (collection[1]);
			Assert.IsNull (collection[-2]);

			System.Threading.Thread.Sleep (2*100);

			Assert.AreEqual ("0",  collection[0]);
			Assert.AreEqual ("1",  collection[1]);
			Assert.AreEqual ("-2", collection[-2]);

			Assert.IsNull (collection[2]);

			System.Threading.Thread.Sleep (10);

			collection.Clear ();

			System.Threading.Thread.Sleep (100);
			Assert.IsNull (collection[2]);
		}


		private class AsyncValueProvider : IAsyncValueProvider<string>
		{
			#region IAsyncValueProvider<string> Members

			public IAsyncEnumerator<string> GetValuesAsync(int index, int count, CancellationToken token)
			{
				return new AsyncEnumerator (index, count, token);
			}

			#endregion
		}

		private class AsyncEnumerator : IAsyncEnumerator<string>
		{
			public AsyncEnumerator(int start, int count, CancellationToken token)
			{
				this.token = token;
				this.start = start;
				this.count = count;
				this.step  = count < 0 ? -1 : 1;
				this.Reset ();
			}
			#region IAsyncEnumerator<string> Members

			public string Current
			{
				get
				{
					return this.value;
				}
			}

			public async Task<bool> MoveNext()
			{
				if (this.more == 0)
				{
					return false;
				}

				this.token.ThrowIfCancellationRequested ();

				await TaskEx.Delay (100, this.token);

				this.value = this.index.ToString ();

				System.Diagnostics.Debug.WriteLine ("Produced " + this.value);
				
				this.index += this.step;
				this.more  -= this.step;

				return true;
			}

			public void Reset()
			{
				this.index = this.start;
				this.more  = this.count;
				this.value = null;
			}

			#endregion


			private readonly CancellationToken	token;
			private readonly int				start;
			private readonly int				count;
			private readonly int				step;
			
			private int							index;
			private int							more;

			private string value;
		}
	}
}
