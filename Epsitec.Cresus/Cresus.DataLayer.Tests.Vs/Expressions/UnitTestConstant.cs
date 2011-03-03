using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Expressions
{


	[TestClass]
	public sealed class UnitTestConstant
	{


		[TestMethod]
		public void ConstantConstructorTest()
		{
			new Constant ((short) 0);
			new Constant ((int) 0);
			new Constant ((long) 0);
			new Constant ((decimal) 0);
			new Constant (true);
			new Constant ("test");
			new Constant (Date.Today);
			new Constant (Time.Now);
			new Constant (System.DateTime.Now);
			new Constant (SimpleEnum.Value1);
		}


		[TestMethod]
		public void ConstantConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new Constant ((string) null)
			);
		}


		[TestMethod]
		public void TypeTest()
		{
			Assert.AreEqual (Type.Int16, new Constant ((short) 0).Type);
			Assert.AreEqual (Type.Int32, new Constant ((int) 0).Type);
			Assert.AreEqual (Type.Int64, new Constant ((long) 0).Type);
			Assert.AreEqual (Type.Decimal, new Constant ((decimal) 0).Type);
			Assert.AreEqual (Type.Boolean, new Constant (true).Type);
			Assert.AreEqual (Type.String, new Constant ("test").Type);
			Assert.AreEqual (Type.Date, new Constant (Date.Today).Type);
			Assert.AreEqual (Type.Time, new Constant (Time.Now).Type);
			Assert.AreEqual (Type.DateTime, new Constant (System.DateTime.Now).Type);
			Assert.AreEqual (Type.ByteArray, new Constant (new byte[] { 0x00 }).Type);
			Assert.AreEqual (Type.Enum, new Constant (SimpleEnum.Value1).Type);
		}


		[TestMethod]
		public void ValueTest()
		{
			Assert.AreEqual (0, (short) new Constant ((short) 0).Value);
			Assert.AreEqual (0, (int) new Constant ((int) 0).Value);
			Assert.AreEqual (0, (long) new Constant ((long) 0).Value);
			Assert.AreEqual (0, (decimal) new Constant ((decimal) 0).Value);
			Assert.AreEqual (true, (bool) new Constant (true).Value);
			Assert.AreEqual ("test", (string) new Constant ("test").Value);
			Assert.AreEqual (Date.Today, (Date) new Constant (Date.Today).Value);
			Assert.AreEqual (new Time (12, 0, 0), (Time) new Constant (new Time (12, 0, 0)).Value);
			Assert.AreEqual (new System.DateTime (2010, 7, 22, 8, 50, 0), (System.DateTime) new Constant (new System.DateTime (2010, 7, 22, 8, 50, 0)).Value);
			Assert.AreEqual (SimpleEnum.Value1, new Constant (SimpleEnum.Value1).Value);
		}


		[TestMethod]
		public void EscapeTest()
		{
			Assert.AreEqual ("#%", Constant.Escape ("%"));
			Assert.AreEqual ("#_", Constant.Escape ("_"));
			Assert.AreEqual ("##", Constant.Escape ("#"));
			Assert.AreEqual ("#%#_##", Constant.Escape ("%_#"));
			Assert.AreEqual ("cou#_cou#_bla#_bla", Constant.Escape ("cou_cou_bla_bla"));
			Assert.AreEqual ("cou#%cou#_bla##bla", Constant.Escape ("cou%cou_bla#bla"));
			Assert.AreEqual ("#%#%#%", Constant.Escape ("%%%"));
			Assert.AreEqual ("#_#_#_", Constant.Escape ("___"));
			Assert.AreEqual ("######", Constant.Escape ("###"));
		}


	}


}
