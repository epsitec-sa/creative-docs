using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


	[TestClass]
	public sealed class UnitTestGetCount
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void GetCountByExample1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample1 ();

				long count = dataContext.GetCount (example);

				Assert.AreEqual (1, count);
			}
		}


		[TestMethod]
		public void GetCountByRequest1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample3 ();
				
				Request request = new Request ()
				{
					RootEntity = example,
				};

				long count = dataContext.GetCount (request);

				Assert.AreEqual (2, count);
			}
		}


		[TestMethod]
		public void GetCountByExample2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetIncorrectExample1 ();

				long count = dataContext.GetCount (example);

				Assert.AreEqual (0, count);
			}
		}


		[TestMethod]
		public void GetCountByRequest2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetIncorrectExample2 ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				long count = dataContext.GetCount (request);

				Assert.AreEqual (0, count);
			}
		}


	}


}
