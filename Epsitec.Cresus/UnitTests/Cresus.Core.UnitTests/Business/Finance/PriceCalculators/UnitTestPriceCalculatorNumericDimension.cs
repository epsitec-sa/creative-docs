using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	[TestClass]
	public sealed class UnitTestPriceCalculatorNumericDimension
	{


		[TestMethod]
		public void RoundingModeTest()
		{
			string name = "blabla";

			List<decimal> values = new List<decimal> ()
			{
				10,
				20,
				30,
			};

			List<RoundingMode> roundingModes = new List<RoundingMode> ()
			{
				RoundingMode.Down,
				RoundingMode.Nearest,
				RoundingMode.Up,
			};

			foreach (RoundingMode roundingMode in roundingModes)
			{
				PriceCalculatorNumericDimension pcnd = new PriceCalculatorNumericDimension (name, values, roundingMode);

				Assert.AreEqual (roundingMode, pcnd.RoundingMode);
			}
		}


		[TestMethod]
		public void GetValueArgumentCheck()
		{
			string name = "name";
			
			List<decimal> values = new List<decimal> ()
			{
				10,
				20,
			};

			RoundingMode roundingMode = RoundingMode.Down;

			PriceCalculatorNumericDimension pcnd = new PriceCalculatorNumericDimension (name, values, roundingMode);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => pcnd.GetValue (9.9999999m)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => pcnd.GetValue (20.000001m)
			);
		}


		[TestMethod]
		public void GetValueTest()
		{
			List<decimal> values = new List<decimal> ()
			{
				10,
				20,
			};

			Dictionary<decimal, decimal> testDataDown = new Dictionary<decimal, decimal> ()
			{
				{10, 10},
				{10.0000001m, 10},
				{15, 10},
				{19.9999999m, 10},
				{20, 20},
			};

			Dictionary<decimal, decimal> testDataNearest = new Dictionary<decimal, decimal> ()
			{
				{10, 10},
				{10.0000001m, 10},
				{15, 20},
				{19.9999999m, 20},
				{20, 20},
			};

			Dictionary<decimal, decimal> testDataUp = new Dictionary<decimal, decimal> ()
			{
				{10, 10},
				{10.0000001m, 20},
				{15, 20},
				{19.9999999m, 20},
				{20, 20},
			};

			this.GetValueTest (values, testDataDown, RoundingMode.Down);
			this.GetValueTest (values, testDataNearest, RoundingMode.Nearest);
			this.GetValueTest (values, testDataUp, RoundingMode.Up);
		}


		private void GetValueTest(List<decimal> values, Dictionary<decimal, decimal> testData, RoundingMode roundingMode)
		{
			PriceCalculatorNumericDimension pcnd = new PriceCalculatorNumericDimension ("name", values, roundingMode);

			foreach (var item in testData)
			{
				Assert.AreEqual (item.Value, pcnd.GetGenericValue (item.Key));
				Assert.AreEqual (item.Value, pcnd.GetValue (item.Key));
			}
		}




	}


}
