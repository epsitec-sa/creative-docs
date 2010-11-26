using Microsoft.VisualStudio.TestTools.UnitTesting;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Controllers.ArticleParameterControllers;


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

						int value = 0;

						var priceCalculator1 = dataContext.CreateEntity<PriceCalculatorEntity> ();
						NumericDimension dimension1 = PriceCalculatorEntity.CreateDimension (articleParameter1, RoundingMode.Up);
						DimensionTable priceTable1 = new DimensionTable (dimension1);
						foreach (decimal d in dimension1.Values.Cast<decimal> ())
						{
							priceTable1[d] = value;
							value++;
						}
						priceCalculator1.SetPriceTable (priceTable1);
						articlePrice.PriceCalculators.Add (priceCalculator1);

						var priceCalculator2 = dataContext.CreateEntity<PriceCalculatorEntity> ();
						CodeDimension dimension2 = PriceCalculatorEntity.CreateDimension (articleParameter2);
						DimensionTable priceTable2 = new DimensionTable (dimension2);
						foreach (string s in dimension2.Values.Cast<string> ())
						{
							priceTable2[s] = value;
							value++;
						}
						priceCalculator2.SetPriceTable (priceTable2);
						articlePrice.PriceCalculators.Add (priceCalculator2);

						var priceCalculator3 = dataContext.CreateEntity<PriceCalculatorEntity> ();
						NumericDimension dimension3a = PriceCalculatorEntity.CreateDimension (articleParameter1, RoundingMode.Down);
						CodeDimension dimension3b = PriceCalculatorEntity.CreateDimension (articleParameter2);
						DimensionTable priceTable3 = new DimensionTable (dimension3a, dimension3b);
						foreach (decimal d in dimension3a.Values.Cast<decimal> ())
						{
							foreach (string s in dimension3b.Values.Cast<string> ())
							{
								priceTable3[d, s] = value;
								value++;
							}
						}
						priceCalculator3.SetPriceTable (priceTable3);
						articlePrice.PriceCalculators.Add (priceCalculator3);

						var articleItem = dataContext.CreateEntity<ArticleDocumentItemEntity> ();

						var parameterValues = new string[] { "p1", "1.5", "p2", "2" };
						articleItem.ArticleDefinition = articleDefinition;

						// TODO Find a better was to obtain this string. This might mean that the
						// parameter join logic must be rewritten in a central location, because it
						// seems it isn't yet. Seems like a code smell to me.
						// Marc

						articleItem.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues);

						dataContext.SaveChanges ();
					}
				}
			}
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
						var priceCalculator = articleItem.ArticleDefinition.ArticlePrices[0].PriceCalculators[0];

						Assert.AreEqual (1m, priceCalculator.Compute (articleItem));
					}
				}
			}
		}


		[TestMethod]
		public void ZeroOrOneEnumParameterTest()
		{
			// TODO

			Assert.Inconclusive ();
		}


		[TestMethod]
		public void ExactlyOneEnumParameterTest()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
				{
					dataInfrastructure.OpenConnection ("id");

					using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
					{
						var articleItem = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000001)));
						var priceCalculator = articleItem.ArticleDefinition.ArticlePrices[0].PriceCalculators[1];

						Assert.AreEqual (4m, priceCalculator.Compute (articleItem));
					}
				}
			}
		}


		[TestMethod]
		public void OneOrMoreEnumParameterTest()
		{
			// TODO

			Assert.Inconclusive ();
		}


		[TestMethod]
		public void ZeroOrMoreEnumParameterTest()
		{
			// TODO

			Assert.Inconclusive ();
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
						var articleItem = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000001)));
						var priceCalculator = articleItem.ArticleDefinition.ArticlePrices[0].PriceCalculators[2];

						Assert.AreEqual (7m, priceCalculator.Compute (articleItem));
					}
				}
			}
		}
		

	}


}
