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
			this.ConstructorTest ((short) 0, Type.Int16, v => new Constant (v));
			this.ConstructorTest ((int) 0, Type.Int32, v => new Constant (v));
			this.ConstructorTest ((long) 0, Type.Int64, v => new Constant (v));
			this.ConstructorTest ((decimal) 0, Type.Decimal, v => new Constant (v));
			this.ConstructorTest (true, Type.Boolean, v => new Constant (v));
			this.ConstructorTest ("test", Type.String, v => new Constant (v));
			this.ConstructorTest (new byte[] { 0x00 }, Type.ByteArray, v => new Constant (v));
			this.ConstructorTest (Date.Today, Type.Date, v => new Constant (v));
			this.ConstructorTest (Time.Now, Type.Time, v => new Constant (v));
			this.ConstructorTest (System.DateTime.Now, Type.DateTime, v => new Constant (v));
			this.ConstructorTest (SimpleEnum.Value1, Type.Enum, v => new Constant (v));
		}


		private void ConstructorTest<T>(T value, Type type, System.Func<T, Constant> constructor)
		{
			var constant = constructor (value);

			Assert.AreEqual (value, constant.Value);
			Assert.AreEqual (type, constant.Type);
		}


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new Constant ((string) null)
			);
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
