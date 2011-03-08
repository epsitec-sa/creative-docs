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
				() => new TestObject () { IsReadOnly = false }.AssertIsReadOnly ()
			);

			new TestObject () { IsReadOnly = true }.AssertIsReadOnly ();
		}


		[TestMethod]
		public void IsNotReadOnlyTest()
		{
			ExceptionAssert.Throw<ReadOnlyException>
			(
				() => new TestObject () { IsReadOnly = true }.AssertIsNotReadOnly ()
			);
			
			new TestObject () { IsReadOnly = false }.AssertIsNotReadOnly ();
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
