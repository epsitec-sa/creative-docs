using NUnit.Framework;

namespace Epsitec.Common.Support.Tests
{
	[TestFixture]
	public class DataBinderTest
	{
		[Test] public void CheckBinderFactory()
		{
			Epsitec.Cresus.UserInterface.IBinder binder = Epsitec.Cresus.UserInterface.BinderFactory.FindBinder ("Test");
			Assertion.AssertNull (binder);
		}
	}
}
