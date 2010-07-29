using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestConstant
	{


		[TestMethod]
		public void ConstantConstructorTest1()
		{
			new Constant ((short) 0);
			new Constant ((int) 0);
			new Constant ((long) 0);
			new Constant ((decimal) 0);
			new Constant ((float) 0);
			new Constant ((double) 0);
			new Constant (true);
			new Constant ("test");
			new Constant (Date.Today);
			new Constant (Time.Now);
			new Constant (System.DateTime.Now);
		}


		[TestMethod]
		[ExpectedException (typeof(System.ArgumentNullException))]
		public void ConstantConstructorTest2()
		{
			new Constant ((string) null);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void TypeTest()
		{
			Assert.AreEqual (Type_Accessor.Int16, new Constant_Accessor ((short) 0).Type);
			Assert.AreEqual (Type_Accessor.Int32, new Constant_Accessor ((int) 0).Type);
			Assert.AreEqual (Type_Accessor.Int64, new Constant_Accessor ((long) 0).Type);
			Assert.AreEqual (Type_Accessor.Double, new Constant_Accessor ((decimal) 0).Type);
			Assert.AreEqual (Type_Accessor.Double, new Constant_Accessor ((float) 0).Type);
			Assert.AreEqual (Type_Accessor.Double, new Constant_Accessor ((double) 0).Type);
			Assert.AreEqual (Type_Accessor.Boolean, new Constant_Accessor (true).Type);
			Assert.AreEqual (Type_Accessor.String, new Constant_Accessor ("test").Type);
			Assert.AreEqual (Type_Accessor.Date, new Constant_Accessor (Date.Today).Type);
			Assert.AreEqual (Type_Accessor.Time, new Constant_Accessor (Time.Now).Type);
			Assert.AreEqual (Type_Accessor.DateTime, new Constant_Accessor (System.DateTime.Now).Type);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void ValueTest()
		{
			Assert.AreEqual (0, (short) new Constant ((short) 0).Value);
			Assert.AreEqual (0, (int) new Constant ((int) 0).Value);
			Assert.AreEqual (0, (long) new Constant ((long) 0).Value);
			Assert.AreEqual (0, (double) new Constant ((decimal) 0).Value);
			Assert.AreEqual (0, (float) new Constant ((float) 0).Value);
			Assert.AreEqual (0, (double) new Constant ((double) 0).Value);
			Assert.AreEqual (true, (bool) new Constant (true).Value);
			Assert.AreEqual ("test", (string) new Constant ("test").Value);
			Assert.AreEqual (Date.Today, (Date) new Constant (Date.Today).Value);
			Assert.AreEqual (new Time (12, 0, 0), (Time) new Constant (new Time (12, 0, 0)).Value);
			Assert.AreEqual (new System.DateTime (2010, 7, 22, 8, 50, 0), (System.DateTime) new Constant (new System.DateTime (2010, 7, 22, 8, 50, 0)).Value);
		}


	}


}
