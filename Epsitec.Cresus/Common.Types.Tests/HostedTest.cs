using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[TestFixture] public class HostedTest
	{
		[Test] public void CheckHostedList()
		{
			
		}

		private class Host<T> : IListHost<T>
		{
			public Host()
			{
			}

			public void SetExpectedInsertions(IEnumerable<T> collection)
			{
			}
			
			#region IListHost<T> Members

			public void NotifyListInsertion(T item)
			{
				
			}

			public void NotifyListRemoval(T item)
			{
				
			}

			#endregion

			private List<T> expectedInsertions;
			private List<T> expectedRemovals;
		}
	}
}
