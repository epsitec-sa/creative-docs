//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


	[TestClass]
	public sealed class UnitTestGetEntitiesByRequestAndLambda
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void RegularEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue == 42);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void RegularEqualInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => 42 == v.IntegerValue);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void RegularNotEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue != 42);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void RegularNotEqualInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => 42 != v.IntegerValue);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void RegularGreater()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue > 42);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
			}
		}


		[TestMethod]
		public void RegularGreaterInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => 42 > v.IntegerValue);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void RegularGreaterOrEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue >= 42);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
			}
		}


		[TestMethod]
		public void RegularGreaterOrEqualInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => 42 >= v.IntegerValue);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void RegularLower()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue < 42);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void RegularLowerInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => 42 < v.IntegerValue);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
			}
		}


		[TestMethod]
		public void RegularLowerOrEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue <= 42);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void RegularLowerOrEqualInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => 42 <= v.IntegerValue);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
			}
		}


		[TestMethod]
		public void StringEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => SqlMethods.CompareTo (v.StringValue, "blupi") == 0);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void StringEqualInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => 0 == SqlMethods.CompareTo ("blupi", v.StringValue));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void StringNotEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => SqlMethods.CompareTo (v.StringValue, "blupi") != 0);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void StringNotEqualInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => 0 != SqlMethods.CompareTo ("blupi", v.StringValue));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void StringGreater()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => SqlMethods.CompareTo (v.StringValue, "blupi") > 0);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
			}
		}


		[TestMethod]
		public void StringGreaterInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => 0 > SqlMethods.CompareTo ("blupi", v.StringValue));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
			}
		}


		[TestMethod]
		public void StringGreaterOrEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => SqlMethods.CompareTo (v.StringValue, "blupi") >= 0);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
			}
		}


		[TestMethod]
		public void StringGreaterOrEqualInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => 0 >= SqlMethods.CompareTo ("blupi", v.StringValue));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
			}
		}


		[TestMethod]
		public void StringLower()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => SqlMethods.CompareTo (v.StringValue, "blupi") < 0);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void StringLowerInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => 0 < SqlMethods.CompareTo ("blupi", v.StringValue));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void StringLowerOrEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => SqlMethods.CompareTo (v.StringValue, "blupi") <= 0);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void StringLowerOrEqualInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => 0 <= SqlMethods.CompareTo ("blupi", v.StringValue));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void Not()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => !(v.IntegerValue == 42));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void RegularIsNull()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue == null);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (0, result.Count);
			}
		}


		[TestMethod]
		public void RegularIsNullReversed()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => null == v.IntegerValue);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (0, result.Count);
			}
		}


		[TestMethod]
		public void RegularIsNotNull()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue != null);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (3, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void RegularIsNotNullReversed()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => null != v.IntegerValue);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (3, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void StringIsNull()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.StringValue == null);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (0, result.Count);
			}
		}


		[TestMethod]
		public void StringIsNullReversed()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => null == v.StringValue);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (0, result.Count);
			}
		}


		[TestMethod]
		public void StringIsNotNull()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.StringValue != null);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (3, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void StringIsNotNullReversed()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => null != v.StringValue);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (3, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void IsLike()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => SqlMethods.Like (v.StringValue, "bl_pi"));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void IsLikeEscape()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => SqlMethods.EscapedLike (v.StringValue, "%i%"));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
			}
		}


		[TestMethod]
		public void And()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue > 41 && v.IntegerValue < 43);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void Or()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue < 41 || v.IntegerValue > 43);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void MemberChain()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person = new NaturalPersonEntity ()
				{
					Gender = new PersonGenderEntity ()
				};

				var request = Request.Create (person);

				request.AddCondition (dataContext, person, p => p.Gender.Name == "Male");
				
				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void ConstantCapturedVariable()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				var constant = 42;

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue == constant);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void ConstantCapturedPropertyAccess()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				var constant = new DateTime (42);

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue == constant.Ticks);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void ConstantCapturedMethodCall()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				var constant = Enumerable.Range (0, 42).ToList ();

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue == constant.Count ());

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void ConstantCapturedMember()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue == this.member);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void ConstantMathOperation()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue == this.member + 6501);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
			}
		}


		[TestMethod]
		public void EvaluableOperationWithinNonEvaluableOperation()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				var pi = "pi";
				request.AddCondition (dataContext, valueDataEntity, v => SqlMethods.Like (v.StringValue, "blu" + pi.ToString ()));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void NullableMember()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var gender = new PersonGenderEntity ();
				var request = Request.Create (gender);

				request.AddCondition (dataContext, gender, g => g.Rank == 0);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (0, result.Count);
			}
		}


		[TestMethod]
		public void Conversion1()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue == null);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (0, result.Count);
			}
		}


		[TestMethod]
		public void Conversion2()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var valueDataEntity = new ValueDataEntity ();
				var request = Request.Create (valueDataEntity);

				request.AddCondition (dataContext, valueDataEntity, v => v.IntegerValue == 42m);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void EntityComparison1()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person1 = new NaturalPersonEntity ();
				var person2 = dataContext.GetByExample (new NaturalPersonEntity ()).First ();

				var request = Request.Create (person1);

				request.AddCondition (dataContext, person1, p => p == person2);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (p => p == person2));
			}
		}


		[TestMethod]
		public void EntityComparison2()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person = new NaturalPersonEntity ();
				var gender = dataContext.GetByExample (new PersonGenderEntity ()).First ();

				var request = Request.Create (person);

				request.AddCondition (dataContext, person, p => p.Gender == gender);

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void AnyCall1()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person = new NaturalPersonEntity ();
				var contact = new UriContactEntity ();
				person.Contacts.Add (contact);

				var request = Request.Create (person);

				request.AddCondition (dataContext, person, p => p.Contacts.Any (c => ((UriContactEntity) c).Uri == "alfred@coucou.com"));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void AnyCall2()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person = new NaturalPersonEntity ();
				var contact = new UriContactEntity ();
				person.Contacts.Add (contact);

				var request = Request.Create (person);

				request.AddCondition (dataContext, person, p => p.Firstname == "Alfred" && p.Contacts.Any (c => ((UriContactEntity) c).Uri == "alfred@coucou.com"));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void AnyCall3()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person = new NaturalPersonEntity ();
				var contact = new UriContactEntity ();
				var role = new ContactRoleEntity ();

				person.Contacts.Add (contact);
				contact.Roles.Add (role);

				var request = Request.Create (person);

				request.AddCondition (dataContext, person, p => p.Contacts.Any (c => c.Roles.Any (r => r.Name == "role")));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (0, result.Count);
			}
		}


		[TestMethod]
		public void IsInSetCall1()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person = new NaturalPersonEntity ();
				var firstnames = new List<string> () { "Alfred", "Gertrude" };
				
				var request = Request.Create (person);
				request.AddCondition (dataContext, person, p => SqlMethods.IsInSet (p.Firstname, firstnames));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (p => DatabaseCreator2.CheckAlfred (p)));
				Assert.IsTrue (result.Any (p => DatabaseCreator2.CheckGertrude (p)));
			}
		}


		[TestMethod]
		public void IsInSetCall2()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var nullFirstNamePerson = dataContext.CreateEntity<NaturalPersonEntity> ();

				nullFirstNamePerson.Firstname = null;
				nullFirstNamePerson.Lastname = "I have no first name";

				dataContext.SaveChanges();
				
				var person = new NaturalPersonEntity ();
				var firstnames = new List<string> () { "Alfred", null };

				var request = Request.Create (person);
				request.AddCondition (dataContext, person, p => SqlMethods.IsInSet (p.Firstname, firstnames));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (p => DatabaseCreator2.CheckAlfred (p)));
				Assert.IsTrue (result.Any (p => p == nullFirstNamePerson));
			}
		}


		[TestMethod]
		public void IsInSetCall3()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var contact = new AbstractContactEntity ();
				var subQuery = new Request ()
				{
					RootEntity = new NaturalPersonEntity ()
					{
						Firstname = "Alfred"
					}
				};

				var request = Request.Create (contact);
				request.AddCondition(dataContext, contact, c => SqlMethods.IsInSet (c.NaturalPerson, subQuery));

				var result = dataContext.GetByRequest<AbstractContactEntity> (request).ToArray ();
				var alfred = dataContext.GetByRequest<NaturalPersonEntity> (subQuery).Single ();

				Assert.IsTrue (result.Count () == 2);
				Assert.IsTrue (result.Any (c => c == alfred.Contacts[0]));
				Assert.IsTrue (result.Any (c => c == alfred.Contacts[1]));
			}
		}


		[TestMethod]
		public void IsNotInSetCall1()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person = new NaturalPersonEntity ();
				var firstnames = new List<string> () { "Alfred", "Gertrude" };

				var request = Request.Create (person);
				request.AddCondition (dataContext, person, p => SqlMethods.IsNotInSet (p.Firstname, firstnames));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (result.Any (p => DatabaseCreator2.CheckHans (p)));
			}
		}


		[TestMethod]
		public void IsNotInSetCall2()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var nullFirstNamePerson = dataContext.CreateEntity<NaturalPersonEntity> ();

				nullFirstNamePerson.Firstname = null;
				nullFirstNamePerson.Lastname = "I have no first name";

				dataContext.SaveChanges ();

				var person = new NaturalPersonEntity ();
				var firstnames = new List<string> () { "Alfred", null };

				var request = Request.Create (person);
				request.AddCondition (dataContext, person, p => SqlMethods.IsNotInSet (p.Firstname, firstnames));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (2, result.Count);
				Assert.IsTrue (result.Any (p => DatabaseCreator2.CheckGertrude (p)));
				Assert.IsTrue (result.Any (p => DatabaseCreator2.CheckHans (p)));

			}
		}


		[TestMethod]
		public void IsNotInSetCall3()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person = new NaturalPersonEntity ();
				var subRequest = new Request ()
				{
					RootEntity = new NaturalPersonEntity ()
					{
						Firstname = "Alfred",
					},
				};

				var request = Request.Create (person);
				request.AddCondition (dataContext, person, p => SqlMethods.IsNotInSet (p, subRequest));

				var result = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (result.Count () == 2);
				Assert.IsTrue (result.Any (p => DatabaseCreator2.CheckGertrude (p)));
				Assert.IsTrue (result.Any (p => DatabaseCreator2.CheckHans (p)));
			}
		}


		[TestMethod]
		public void Convert()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person = new NaturalPersonEntity ();

				var request = Request.Create (person);
				request.AddCondition (dataContext, person, p => p.Lastname == SqlMethods.Convert<int, string> (42));

				var result = dataContext.GetByRequest (request).ToList ();
				Assert.AreEqual (0, result.Count);
			}
		}


		private int member = 42;


	}


}
