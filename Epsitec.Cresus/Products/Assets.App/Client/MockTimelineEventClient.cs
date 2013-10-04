//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Assets.Core.Business;
using Epsitec.Cresus.Assets.Core.Collections;

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Epsitec.Cresus.Assets.App.Client
{
	public class MockTimelineEventClient : IAsyncEnumerable<TimelineEventCell>
	{
		public MockTimelineEventClient(Date date)
		{
			this.date = date;
			this.seed = 1;
		}


		public Date								Date
		{
			get
			{
				return this.date;
			}
		}

		
		public void ChangeRandomSeed()
		{
			this.seed++;
		}


		#region IAsyncEnumerable<TimelineEventCell> Members

		public IAsyncEnumerator<TimelineEventCell> GetAsyncEnumerator(int index, int count, CancellationToken token)
		{
			return new Enumerator (this, index, count, token);
		}

		#endregion

		#region Enumerator Class

		private class Enumerator : IAsyncEnumerator<TimelineEventCell>
		{
			public Enumerator(MockTimelineEventClient provider, int index, int count, CancellationToken token)
			{
				this.provider = provider;
				this.token = token;
				this.start = index;
				this.count = count;
				this.step  = count < 0 ? -1 : 1;
				this.Reset ();
			}


			#region IAsyncEnumerator<TimelineEventCell> Members

			public TimelineEventCell Current
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

				await Task.Delay (20, this.token);

				this.RandomlyGenerateNextDate ();

				this.value  = new TimelineEventCell (this.date);
				this.index += this.step;
				this.more  -= this.step;

				return true;
			}

			public void Reset()
			{
				this.random = new System.Random (this.step * this.provider.seed);
				this.date   = this.provider.date;
				this.index  = this.start;
				this.more   = this.count;
				this.value  = null;

				//	Depending on the starting point, move forward or backward in
				//	the virtual date collection, so that we fetch the next item at
				//	the correct position:
				
				int skip = this.start;

				if (skip > 0)
				{
					while (skip-- > 0)
					{
						this.RandomlyGenerateNextDate ();
					}
				}
				else if (skip < 0)
				{
					this.date = this.date.AddDays (-1);

					while (skip++ < 0)
					{
						this.RandomlyGenerateNextDate ();
					}
				}
			}

			#endregion


			private void RandomlyGenerateNextDate()
			{
				int days = this.random.Next (0, 4) * this.step;

				this.date = this.date.AddDays (days);
			}


			private readonly MockTimelineEventClient provider;
			private readonly CancellationToken	token;
			private readonly int				start;
			private readonly int				count;
			private readonly int				step;

			private System.Random				random;
			private int							index;
			private int							more;
			private TimelineEventCell			value;
			private Date						date;
		}

		#endregion


		private readonly Date					date;
		private int								seed;
	}
}
