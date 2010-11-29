using Epsitec.Cresus.Database;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;
using Epsitec.Cresus.Core.Controllers.ArticleParameterControllers;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Context;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.Entities
{
	
	
	[TestClass]
	public class UnitTestPriceCalculatorEntity
	{


		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			using (DbInfrastructure dbInfrastructure = TestHelper.CreateAndConnectToDatabase ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
				{
					dataInfrastructure.OpenConnection ("id");

					dataInfrastructure.CreateSchema<ArticleDefinitionEntity> ();
					dataInfrastructure.CreateSchema<AbstractArticleParameterDefinitionEntity> ();
					dataInfrastructure.CreateSchema<NumericValueArticleParameterDefinitionEntity> ();
					dataInfrastructure.CreateSchema<EnumValueArticleParameterDefinitionEntity> ();
					dataInfrastructure.CreateSchema<ArticlePriceEntity> ();
					dataInfrastructure.CreateSchema<PriceCalculatorEntity> ();
					dataInfrastructure.CreateSchema<ArticleDocumentItemEntity> ();

					using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
					{
						UnitTestPriceCalculatorEntity.CreateDataForArticleNumericParameter (dataContext);
						UnitTestPriceCalculatorEntity.CreateDataForArticleEnumParameterZeroOrOne (dataContext);
						UnitTestPriceCalculatorEntity.CreateDataForArticleEnumParameterExactlyOne(dataContext);
						UnitTestPriceCalculatorEntity.CreateDataForArticleEnumParameterAtLeastOne(dataContext);
						UnitTestPriceCalculatorEntity.CreateDataForArticleEnumParameterAny(dataContext);
						UnitTestPriceCalculatorEntity.CreateDataForArticleMultipleParameters(dataContext);
					}
				}
			}
		}


		private static void CreateDataForArticleNumericParameter(DataContext dataContext)
		{
			var articleDefinition = dataContext.CreateEntity<ArticleDefinitionEntity> ();

			var articlePrice = dataContext.CreateEntity<ArticlePriceEntity> ();
			articlePrice.BeginDate = System.DateTime.MinValue;
			articlePrice.EndDate = System.DateTime.MaxValue;
			articleDefinition.ArticlePrices.Add (articlePrice);

			var articleParameter = dataContext.CreateEntity<NumericValueArticleParameterDefinitionEntity> ();
			articleParameter.Code = "p";
			articleParameter.MinValue = 1;
			articleParameter.DefaultValue = 1;
			articleParameter.MaxValue = 3;
			articleParameter.PreferredValues = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3");
			articleDefinition.ArticleParameterDefinitions.Add (articleParameter);

			IDictionary<decimal, decimal> parameterCodeToValues = new Dictionary<decimal, decimal> ()
			{
				{ 1, 1 },
				{ 2, 2 },
				{ 3, 3 },
			};

			var priceCalculator = dataContext.CreateEntity<PriceCalculatorEntity> ();

			DimensionTable priceTable = PriceCalculatorEntity.CreatePriceTable (articleParameter, parameterCodeToValues, RoundingMode.Up);

			priceCalculator.SetPriceTable (priceTable);
			articlePrice.PriceCalculators.Add (priceCalculator);

			var articleItem = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
			articleItem.ArticleDefinition = articleDefinition;

			var parameterValues = new string[] { "p", "1.5" };
			articleItem.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues);

			dataContext.SaveChanges ();
		}


		private static void CreateDataForArticleEnumParameterZeroOrOne(DataContext dataContext)
		{
			var articleDefinition = dataContext.CreateEntity<ArticleDefinitionEntity> ();

			var articlePrice = dataContext.CreateEntity<ArticlePriceEntity> ();
			articlePrice.BeginDate = System.DateTime.MinValue;
			articlePrice.EndDate = System.DateTime.MaxValue;
			articleDefinition.ArticlePrices.Add (articlePrice);

			var articleParameter = dataContext.CreateEntity<EnumValueArticleParameterDefinitionEntity> ();
			articleParameter.Code = "p";
			articleParameter.Cardinality = EnumValueCardinality.ZeroOrOne;
			articleParameter.Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3");
			articleParameter.DefaultValue = "1";
			articleDefinition.ArticleParameterDefinitions.Add (articleParameter);

			IDictionary<string, decimal> parameterCodeToValues = new Dictionary<string, decimal> ()
			{
				{ "1", 1 },
				{ "2", 2 },
				{ "3", 3 },
			};

			var priceCalculator = dataContext.CreateEntity<PriceCalculatorEntity> ();

			DimensionTable priceTable = PriceCalculatorEntity.CreatePriceTable (articleParameter, parameterCodeToValues);

			priceCalculator.SetPriceTable (priceTable);
			articlePrice.PriceCalculators.Add (priceCalculator);

			var articleItem1 = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
			articleItem1.ArticleDefinition = articleDefinition;

			var parameterValues1 = new string[] { "p", "2" };
			articleItem1.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues1);

			var articleItem2 = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
			articleItem2.ArticleDefinition = articleDefinition;

			var parameterValues2 = new string[] { "p", ""};
			articleItem2.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues2);

			dataContext.SaveChanges ();
		}


		private static void CreateDataForArticleEnumParameterExactlyOne(DataContext dataContext)
		{
			var articleDefinition = dataContext.CreateEntity<ArticleDefinitionEntity> ();

			var articlePrice = dataContext.CreateEntity<ArticlePriceEntity> ();
			articlePrice.BeginDate = System.DateTime.MinValue;
			articlePrice.EndDate = System.DateTime.MaxValue;
			articleDefinition.ArticlePrices.Add (articlePrice);

			var articleParameter = dataContext.CreateEntity<EnumValueArticleParameterDefinitionEntity> ();
			articleParameter.Code = "p";
			articleParameter.Cardinality = EnumValueCardinality.ExactlyOne;
			articleParameter.Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3");
			articleParameter.DefaultValue = "1";
			articleDefinition.ArticleParameterDefinitions.Add (articleParameter);

			IDictionary<string, decimal> parameterCodeToValues = new Dictionary<string, decimal> ()
			{
				{ "1", 1 },
				{ "2", 2 },
				{ "3", 3 },
			};

			var priceCalculator = dataContext.CreateEntity<PriceCalculatorEntity> ();

			DimensionTable priceTable = PriceCalculatorEntity.CreatePriceTable (articleParameter, parameterCodeToValues);

			priceCalculator.SetPriceTable (priceTable);
			articlePrice.PriceCalculators.Add (priceCalculator);

			var articleItem = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
			articleItem.ArticleDefinition = articleDefinition;

			var parameterValues = new string[] { "p", "2" };
			articleItem.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues);

			dataContext.SaveChanges ();
		}


		private static void CreateDataForArticleEnumParameterAtLeastOne(DataContext dataContext)
		{
			var articleDefinition = dataContext.CreateEntity<ArticleDefinitionEntity> ();

			var articlePrice = dataContext.CreateEntity<ArticlePriceEntity> ();
			articlePrice.BeginDate = System.DateTime.MinValue;
			articlePrice.EndDate = System.DateTime.MaxValue;
			articleDefinition.ArticlePrices.Add (articlePrice);

			var articleParameter = dataContext.CreateEntity<EnumValueArticleParameterDefinitionEntity> ();
			articleParameter.Code = "p";
			articleParameter.Cardinality = EnumValueCardinality.AtLeastOne;
			articleParameter.Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3");
			articleParameter.DefaultValue = "1";
			articleDefinition.ArticleParameterDefinitions.Add (articleParameter);

			IDictionary<string, decimal> parameterCodeToValues = new Dictionary<string, decimal> ()
			{
				{ "1", 1 },
				{ "2", 2 },
				{ "3", 3 },
			};

			var priceCalculator = dataContext.CreateEntity<PriceCalculatorEntity> ();

			DimensionTable priceTable = PriceCalculatorEntity.CreatePriceTable (articleParameter, parameterCodeToValues);

			priceCalculator.SetPriceTable (priceTable);
			articlePrice.PriceCalculators.Add (priceCalculator);

			var articleItem1 = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
			articleItem1.ArticleDefinition = articleDefinition;

			var parameterValues1 = new string[] { "p", "2" };
			articleItem1.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues1);

			var articleItem2 = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
			articleItem2.ArticleDefinition = articleDefinition;

			var parameterValues2 = new string[] { "p", AbstractArticleParameterDefinitionEntity.Join( "2", "3") };
			articleItem2.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues2);

			dataContext.SaveChanges ();
		}


		private static void CreateDataForArticleEnumParameterAny(DataContext dataContext)
		{
			var articleDefinition = dataContext.CreateEntity<ArticleDefinitionEntity> ();

			var articlePrice = dataContext.CreateEntity<ArticlePriceEntity> ();
			articlePrice.BeginDate = System.DateTime.MinValue;
			articlePrice.EndDate = System.DateTime.MaxValue;
			articleDefinition.ArticlePrices.Add (articlePrice);

			var articleParameter = dataContext.CreateEntity<EnumValueArticleParameterDefinitionEntity> ();
			articleParameter.Code = "p";
			articleParameter.Cardinality = EnumValueCardinality.Any;
			articleParameter.Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3");
			articleParameter.DefaultValue = "1";
			articleDefinition.ArticleParameterDefinitions.Add (articleParameter);

			IDictionary<string, decimal> parameterCodeToValues = new Dictionary<string, decimal> ()
			{
				{ "1", 1 },
				{ "2", 2 },
				{ "3", 3 },
			};

			var priceCalculator = dataContext.CreateEntity<PriceCalculatorEntity> ();

			DimensionTable priceTable = PriceCalculatorEntity.CreatePriceTable (articleParameter, parameterCodeToValues);

			priceCalculator.SetPriceTable (priceTable);
			articlePrice.PriceCalculators.Add (priceCalculator);

			var articleItem1 = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
			articleItem1.ArticleDefinition = articleDefinition;

			var parameterValues1 = new string[] { "p", "2" };
			articleItem1.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues1);

			var articleItem2 = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
			articleItem2.ArticleDefinition = articleDefinition;

			var parameterValues2 = new string[] { "p", AbstractArticleParameterDefinitionEntity.Join ("2", "3") };
			articleItem2.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues2);

			var articleItem3 = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
			articleItem3.ArticleDefinition = articleDefinition;

			var parameterValues3 = new string[] { "p", "" };
			articleItem3.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues3);

			dataContext.SaveChanges ();
		}


		private static void CreateDataForArticleMultipleParameters(DataContext dataContext)
		{
			var articleDefinition = dataContext.CreateEntity<ArticleDefinitionEntity> ();

			var articlePrice = dataContext.CreateEntity<ArticlePriceEntity> ();
			articlePrice.BeginDate = System.DateTime.MinValue;
			articlePrice.EndDate = System.DateTime.MaxValue;
			articleDefinition.ArticlePrices.Add (articlePrice);

			var articleParameter1 = dataContext.CreateEntity<NumericValueArticleParameterDefinitionEntity> ();
			articleParameter1.Code = "p1";
			articleParameter1.MinValue = 1;
			articleParameter1.DefaultValue = 1;
			articleParameter1.MaxValue = 3;
			articleParameter1.PreferredValues = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3");
			articleDefinition.ArticleParameterDefinitions.Add (articleParameter1);

			var articleParameter2 = dataContext.CreateEntity<EnumValueArticleParameterDefinitionEntity> ();
			articleParameter2.Code = "p2";
			articleParameter2.Cardinality = EnumValueCardinality.ExactlyOne;
			articleParameter2.Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3");
			articleParameter2.DefaultValue = "1";
			articleDefinition.ArticleParameterDefinitions.Add (articleParameter2);

			var priceCalculator = dataContext.CreateEntity<PriceCalculatorEntity> ();
			
			NumericDimension dimension1 = PriceCalculatorEntity.CreateDimension (articleParameter1, RoundingMode.Down);
			CodeDimension dimension2 = PriceCalculatorEntity.CreateDimension (articleParameter2);
			DimensionTable priceTable = new DimensionTable (dimension1, dimension2);

			int value = 1;

			foreach (object o1 in dimension1.Values)
			{
				foreach (object o2 in dimension2.Values)
				{
					priceTable[o1, o2] = value;
					value++;
				}
			}
			priceCalculator.SetPriceTable (priceTable);
			articlePrice.PriceCalculators.Add (priceCalculator);

			var articleItem = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
			articleItem.ArticleDefinition = articleDefinition;

			var parameterValues = new string[] { "p1", "1.5", "p2", "3" };
			articleItem.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues);

			dataContext.SaveChanges ();
		}


		[TestMethod]
		public void NumericParameterTest()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
				{
					dataInfrastructure.OpenConnection ("id");

					using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
					{
						var articleItem = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000001)));
						var priceCalculator = articleItem.ArticleDefinition.ArticlePrices.First ().PriceCalculators.First ();

						Assert.AreEqual (2m, priceCalculator.Compute (articleItem));
					}
				}
			}
		}


		[TestMethod]
		public void EnumZeroOrOneParameterTest()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
				{
					dataInfrastructure.OpenConnection ("id");

					using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
					{
						var articleItem1 = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000002)));
						var priceCalculator1 = articleItem1.ArticleDefinition.ArticlePrices.First ().PriceCalculators.First ();

						Assert.AreEqual (2m, priceCalculator1.Compute (articleItem1));

						var articleItem2 = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000003)));
						var priceCalculator2 = articleItem2.ArticleDefinition.ArticlePrices.First ().PriceCalculators.First ();

						Assert.AreEqual (0m, priceCalculator2.Compute (articleItem2));
					}
				}
			}
		}


		[TestMethod]
		public void EnumExactlyOneParameterTest()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
				{
					dataInfrastructure.OpenConnection ("id");

					using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
					{
						var articleItem = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000004)));
						var priceCalculator = articleItem.ArticleDefinition.ArticlePrices.First ().PriceCalculators.First ();

						Assert.AreEqual (2m, priceCalculator.Compute (articleItem));
					}
				}
			}
		}


		[TestMethod]
		public void EnumAtLeastOneParameterTest()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
				{
					dataInfrastructure.OpenConnection ("id");

					using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
					{
						var articleItem1 = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000005)));
						var priceCalculator1 = articleItem1.ArticleDefinition.ArticlePrices.First ().PriceCalculators.First ();

						Assert.AreEqual (2m, priceCalculator1.Compute (articleItem1));

						var articleItem2 = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000006)));
						var priceCalculator2 = articleItem2.ArticleDefinition.ArticlePrices.First ().PriceCalculators.First ();

						Assert.AreEqual (5m, priceCalculator2.Compute (articleItem2));
					}
				}
			}
		}


		[TestMethod]
		public void EnumAnyParameterTest()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
				{
					dataInfrastructure.OpenConnection ("id");

					using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
					{
						var articleItem1 = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000007)));
						var priceCalculator1 = articleItem1.ArticleDefinition.ArticlePrices.First ().PriceCalculators.First ();

						Assert.AreEqual (2m, priceCalculator1.Compute (articleItem1));

						var articleItem2 = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000008)));
						var priceCalculator2 = articleItem2.ArticleDefinition.ArticlePrices.First ().PriceCalculators.First ();

						Assert.AreEqual (5m, priceCalculator2.Compute (articleItem2));

						var articleItem3 = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000009)));
						var priceCalculator3 = articleItem3.ArticleDefinition.ArticlePrices.First ().PriceCalculators.First ();

						Assert.AreEqual (0m, priceCalculator3.Compute (articleItem3));
					}
				}
			}
		}


		[TestMethod]
		public void MultipleParametersTest()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
				{
					dataInfrastructure.OpenConnection ("id");

					using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
					{
						var articleItem = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000010)));
						var priceCalculator = articleItem.ArticleDefinition.ArticlePrices.First ().PriceCalculators.First ();

						Assert.AreEqual (3m, priceCalculator.Compute (articleItem));
					}
				}
			}
		}
		

	}


}
