using Epsitec.Common.Types;
using Epsitec.Common.Types.Exceptions;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Common.Tests.Vs.Types
{


	[TestClass]
	public sealed class UnitTestIReadOnly
	{


		[TestMethod]
		public void IsReadOnlyTest()
		{
			ExceptionAssert.Throw<ReadOnlyException>
			(
				() => new TestObject () { IsReadOnly = false }.ThrowIfReadWrite ()
			);

			new TestObject () { IsReadOnly = true }.ThrowIfReadWrite ();
		}


		[TestMethod]
		public void IsNotReadOnlyTest()
		{
			ExceptionAssert.Throw<ReadOnlyException>
			(
				() => new TestObject () { IsReadOnly = true }.ThrowIfReadOnly ()
			);
			
			new TestObject () { IsReadOnly = false }.ThrowIfReadOnly ();
		}


		private class TestObject : IReadOnly
		{

			
			#region IReadOnly Members


			public bool IsReadOnly
			{
				get;
				set;
			}


			#endregion
		
		
		}


	}


}
