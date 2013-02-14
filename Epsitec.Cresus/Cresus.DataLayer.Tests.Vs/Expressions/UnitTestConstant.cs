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


		// TODO Add tests for CreateSqlField(...)
		// TODO Add tests for CheckField(...)


		[TestMethod]
		public void ConstructorTest()
		{
			this.ConstructorTest ((short) 0);
			this.ConstructorTest ((int) 0);
			this.ConstructorTest ((long) 0);
			this.ConstructorTest ((decimal) 0);
			this.ConstructorTest (true);
			this.ConstructorTest ("test");
			this.ConstructorTest (new byte[] { 0x00 });
			this.ConstructorTest (Date.Today);
			this.ConstructorTest (Time.Now);
			this.ConstructorTest (System.DateTime.Now);
			this.ConstructorTest (SimpleEnum.Value1);
		}


		private void ConstructorTest<T>(T value)
		{
			var constant = new Constant (value);

			Assert.AreEqual (value, constant.Value);
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
