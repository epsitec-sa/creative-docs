//using Epsitec.Common.UnitTesting;

//using Epsitec.Cresus.Database;

//using Epsitec.Cresus.Core.Business;
//using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;
//using Epsitec.Cresus.Core.Controllers.ArticleParameterControllers;

//using Epsitec.Cresus.DataLayer.Infrastructure;
//using Epsitec.Cresus.DataLayer.Context;

//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using System.Collections.Generic;

//using System.Linq;
//using System.Xml.Linq;
//using System.Text;


//namespace Epsitec.Cresus.Core.UnitTests.Entities
//{
	
	
//    [TestClass]
//    public class UnitTestPriceCalculatorEntity
//    {


//        [ClassInitialize]
//        public static void Initialize(TestContext testContext)
//        {
//            TestHelper.Initialize ();

//            using (DbInfrastructure dbInfrastructure = TestHelper.CreateAndConnectToDatabase ())
//            {
//                using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
//                {
//                    dataInfrastructure.OpenConnection ("id");

//                    dataInfrastructure.CreateSchema<ArticleDefinitionEntity> ();
//                    dataInfrastructure.CreateSchema<AbstractArticleParameterDefinitionEntity> ();
//                    dataInfrastructure.CreateSchema<NumericValueArticleParameterDefinitionEntity> ();
//                    dataInfrastructure.CreateSchema<EnumValueArticleParameterDefinitionEntity> ();
//                    dataInfrastructure.CreateSchema<ArticlePriceEntity> ();
//                    dataInfrastructure.CreateSchema<PriceCalculatorEntity> ();
//                    dataInfrastructure.CreateSchema<ArticleDocumentItemEntity> ();

//                    using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
//                    {
//                        UnitTestPriceCalculatorEntity.CreateDataForArticleNumericParameter (dataContext);
//                        UnitTestPriceCalculatorEntity.CreateDataForArticleEnumParameterZeroOrOne (dataContext);
//                        UnitTestPriceCalculatorEntity.CreateDataForArticleEnumParameterExactlyOne(dataContext);
//                        UnitTestPriceCalculatorEntity.CreateDataForArticleEnumParameterAtLeastOne(dataContext);
//                        UnitTestPriceCalculatorEntity.CreateDataForArticleEnumParameterAny(dataContext);
//                        UnitTestPriceCalculatorEntity.CreateDataForArticleMultipleParameters(dataContext);
//                    }
//                }
//            }
//        }


//        private static void CreateDataForArticleNumericParameter(DataContext dataContext)
//        {
//            var articleDefinition = dataContext.CreateEntity<ArticleDefinitionEntity> ();

//            var articlePrice = dataContext.CreateEntity<ArticlePriceEntity> ();
//            articlePrice.BeginDate = System.DateTime.MinValue;
//            articlePrice.EndDate = System.DateTime.MaxValue;
//            articleDefinition.ArticlePrices.Add (articlePrice);

//            var articleParameter = dataContext.CreateEntity<NumericValueArticleParameterDefinitionEntity> ();
//            articleParameter.Code = "p";
//            articleParameter.MinValue = 1;
//            articleParameter.DefaultValue = 1;
//            articleParameter.MaxValue = 3;
//            articleParameter.PreferredValues = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3");
//            articleDefinition.ArticleParameterDefinitions.Add (articleParameter);

//            IDictionary<decimal, decimal> parameterCodeToValues = new Dictionary<decimal, decimal> ()
//            {
//                { 1, 1 },
//                { 2, 2 },
//                { 3, 3 },
//            };

//            var priceCalculator1 = dataContext.CreateEntity<PriceCalculatorEntity> ();
//            DimensionTable priceTable1 = PriceCalculatorEntity.CreatePriceTable (articleParameter, parameterCodeToValues, RoundingMode.Up);
//            priceCalculator1.SetPriceTable (articleDefinition, priceTable1);
//            articlePrice.PriceCalculators.Add (priceCalculator1);

//            var priceCalculator2 = dataContext.CreateEntity<PriceCalculatorEntity> ();
//            DimensionTable priceTable2 = PriceCalculatorEntity.CreatePriceTable (articleParameter, parameterCodeToValues, RoundingMode.Up);
//            priceTable2[2m] = null;
//            priceCalculator2.SetPriceTable (articleDefinition, priceTable2);
//            articlePrice.PriceCalculators.Add (priceCalculator2);

//            var articleItem = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
//            articleItem.ArticleDefinition = articleDefinition;

//            var parameterValues = new string[] { "p", "1.5" };
//            articleItem.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues);

//            dataContext.SaveChanges ();
//        }


//        private static void CreateDataForArticleEnumParameterZeroOrOne(DataContext dataContext)
//        {
//            var articleDefinition = dataContext.CreateEntity<ArticleDefinitionEntity> ();

//            var articlePrice = dataContext.CreateEntity<ArticlePriceEntity> ();
//            articlePrice.BeginDate = System.DateTime.MinValue;
//            articlePrice.EndDate = System.DateTime.MaxValue;
//            articleDefinition.ArticlePrices.Add (articlePrice);

//            var articleParameter = dataContext.CreateEntity<EnumValueArticleParameterDefinitionEntity> ();
//            articleParameter.Code = "p";
//            articleParameter.Cardinality = EnumValueCardinality.ZeroOrOne;
//            articleParameter.Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3");
//            articleParameter.DefaultValue = "1";
//            articleDefinition.ArticleParameterDefinitions.Add (articleParameter);

//            IDictionary<string, decimal> parameterCodeToValues = new Dictionary<string, decimal> ()
//            {
//                { "1", 1 },
//                { "2", 2 },
//                { "3", 3 },
//            };

//            var priceCalculator1 = dataContext.CreateEntity<PriceCalculatorEntity> ();
//            DimensionTable priceTable1 = PriceCalculatorEntity.CreatePriceTable (articleParameter, parameterCodeToValues);
//            priceCalculator1.SetPriceTable (articleDefinition, priceTable1);
//            articlePrice.PriceCalculators.Add (priceCalculator1);

//            var priceCalculator2 = dataContext.CreateEntity<PriceCalculatorEntity> ();
//            DimensionTable priceTable2 = PriceCalculatorEntity.CreatePriceTable (articleParameter, parameterCodeToValues);
//            priceTable2["2"] = null;
//            priceCalculator2.SetPriceTable (articleDefinition, priceTable2);
//            articlePrice.PriceCalculators.Add (priceCalculator2);

//            var articleItem1 = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
//            articleItem1.ArticleDefinition = articleDefinition;

//            var parameterValues1 = new string[] { "p", "2" };
//            articleItem1.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues1);

//            var articleItem2 = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
//            articleItem2.ArticleDefinition = articleDefinition;

//            var parameterValues2 = new string[] { "p", ""};
//            articleItem2.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues2);

//            dataContext.SaveChanges ();
//        }


//        private static void CreateDataForArticleEnumParameterExactlyOne(DataContext dataContext)
//        {
//            var articleDefinition = dataContext.CreateEntity<ArticleDefinitionEntity> ();

//            var articlePrice = dataContext.CreateEntity<ArticlePriceEntity> ();
//            articlePrice.BeginDate = System.DateTime.MinValue;
//            articlePrice.EndDate = System.DateTime.MaxValue;
//            articleDefinition.ArticlePrices.Add (articlePrice);

//            var articleParameter = dataContext.CreateEntity<EnumValueArticleParameterDefinitionEntity> ();
//            articleParameter.Code = "p";
//            articleParameter.Cardinality = EnumValueCardinality.ExactlyOne;
//            articleParameter.Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3");
//            articleParameter.DefaultValue = "1";
//            articleDefinition.ArticleParameterDefinitions.Add (articleParameter);

//            IDictionary<string, decimal> parameterCodeToValues = new Dictionary<string, decimal> ()
//            {
//                { "1", 1 },
//                { "2", 2 },
//                { "3", 3 },
//            };

//            var priceCalculator1 = dataContext.CreateEntity<PriceCalculatorEntity> ();
//            DimensionTable priceTable1 = PriceCalculatorEntity.CreatePriceTable (articleParameter, parameterCodeToValues);
//            priceCalculator1.SetPriceTable (articleDefinition, priceTable1);
//            articlePrice.PriceCalculators.Add (priceCalculator1);

//            var priceCalculator2 = dataContext.CreateEntity<PriceCalculatorEntity> ();
//            DimensionTable priceTable2 = PriceCalculatorEntity.CreatePriceTable (articleParameter, parameterCodeToValues);
//            priceTable2["2"] = null;
//            priceCalculator2.SetPriceTable (articleDefinition, priceTable2);
//            articlePrice.PriceCalculators.Add (priceCalculator2);

//            var articleItem = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
//            articleItem.ArticleDefinition = articleDefinition;

//            var parameterValues = new string[] { "p", "2" };
//            articleItem.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues);

//            dataContext.SaveChanges ();
//        }


//        private static void CreateDataForArticleEnumParameterAtLeastOne(DataContext dataContext)
//        {
//            var articleDefinition = dataContext.CreateEntity<ArticleDefinitionEntity> ();

//            var articlePrice = dataContext.CreateEntity<ArticlePriceEntity> ();
//            articlePrice.BeginDate = System.DateTime.MinValue;
//            articlePrice.EndDate = System.DateTime.MaxValue;
//            articleDefinition.ArticlePrices.Add (articlePrice);

//            var articleParameter = dataContext.CreateEntity<EnumValueArticleParameterDefinitionEntity> ();
//            articleParameter.Code = "p";
//            articleParameter.Cardinality = EnumValueCardinality.AtLeastOne;
//            articleParameter.Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3");
//            articleParameter.DefaultValue = "1";
//            articleDefinition.ArticleParameterDefinitions.Add (articleParameter);

//            IDictionary<string, decimal> parameterCodeToValues = new Dictionary<string, decimal> ()
//            {
//                { "1", 1 },
//                { "2", 2 },
//                { "3", 3 },
//            };

//            var priceCalculator1 = dataContext.CreateEntity<PriceCalculatorEntity> ();
//            DimensionTable priceTable1 = PriceCalculatorEntity.CreatePriceTable (articleParameter, parameterCodeToValues);
//            priceCalculator1.SetPriceTable (articleDefinition, priceTable1);
//            articlePrice.PriceCalculators.Add (priceCalculator1);

//            var priceCalculator2 = dataContext.CreateEntity<PriceCalculatorEntity> ();
//            DimensionTable priceTable2 = PriceCalculatorEntity.CreatePriceTable (articleParameter, parameterCodeToValues);
//            priceTable2["2"] = null;
//            priceCalculator2.SetPriceTable (articleDefinition, priceTable2);
//            articlePrice.PriceCalculators.Add (priceCalculator2);

//            var articleItem1 = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
//            articleItem1.ArticleDefinition = articleDefinition;

//            var parameterValues1 = new string[] { "p", "2" };
//            articleItem1.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues1);

//            var articleItem2 = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
//            articleItem2.ArticleDefinition = articleDefinition;

//            var parameterValues2 = new string[] { "p", AbstractArticleParameterDefinitionEntity.Join( "2", "3") };
//            articleItem2.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues2);

//            dataContext.SaveChanges ();
//        }


//        private static void CreateDataForArticleEnumParameterAny(DataContext dataContext)
//        {
//            var articleDefinition = dataContext.CreateEntity<ArticleDefinitionEntity> ();

//            var articlePrice = dataContext.CreateEntity<ArticlePriceEntity> ();
//            articlePrice.BeginDate = System.DateTime.MinValue;
//            articlePrice.EndDate = System.DateTime.MaxValue;
//            articleDefinition.ArticlePrices.Add (articlePrice);

//            var articleParameter = dataContext.CreateEntity<EnumValueArticleParameterDefinitionEntity> ();
//            articleParameter.Code = "p";
//            articleParameter.Cardinality = EnumValueCardinality.Any;
//            articleParameter.Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3");
//            articleParameter.DefaultValue = "1";
//            articleDefinition.ArticleParameterDefinitions.Add (articleParameter);

//            IDictionary<string, decimal> parameterCodeToValues = new Dictionary<string, decimal> ()
//            {
//                { "1", 1 },
//                { "2", 2 },
//                { "3", 3 },
//            };

//            var priceCalculator1 = dataContext.CreateEntity<PriceCalculatorEntity> ();
//            DimensionTable priceTable1 = PriceCalculatorEntity.CreatePriceTable (articleParameter, parameterCodeToValues);
//            priceCalculator1.SetPriceTable (articleDefinition, priceTable1);
//            articlePrice.PriceCalculators.Add (priceCalculator1);

//            var priceCalculator2 = dataContext.CreateEntity<PriceCalculatorEntity> ();
//            DimensionTable priceTable2 = PriceCalculatorEntity.CreatePriceTable (articleParameter, parameterCodeToValues);
//            priceTable2["2"] = null;
//            priceCalculator2.SetPriceTable (articleDefinition, priceTable2);
//            articlePrice.PriceCalculators.Add (priceCalculator2);

//            var articleItem1 = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
//            articleItem1.ArticleDefinition = articleDefinition;

//            var parameterValues1 = new string[] { "p", "2" };
//            articleItem1.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues1);

//            var articleItem2 = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
//            articleItem2.ArticleDefinition = articleDefinition;

//            var parameterValues2 = new string[] { "p", AbstractArticleParameterDefinitionEntity.Join ("2", "3") };
//            articleItem2.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues2);

//            var articleItem3 = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
//            articleItem3.ArticleDefinition = articleDefinition;

//            var parameterValues3 = new string[] { "p", "" };
//            articleItem3.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues3);

//            dataContext.SaveChanges ();
//        }


//        private static void CreateDataForArticleMultipleParameters(DataContext dataContext)
//        {
//            var articleDefinition = dataContext.CreateEntity<ArticleDefinitionEntity> ();

//            var articlePrice = dataContext.CreateEntity<ArticlePriceEntity> ();
//            articlePrice.BeginDate = System.DateTime.MinValue;
//            articlePrice.EndDate = System.DateTime.MaxValue;
//            articleDefinition.ArticlePrices.Add (articlePrice);

//            var articleParameter1 = dataContext.CreateEntity<NumericValueArticleParameterDefinitionEntity> ();
//            articleParameter1.Code = "p1";
//            articleParameter1.MinValue = 1;
//            articleParameter1.DefaultValue = 1;
//            articleParameter1.MaxValue = 3;
//            articleParameter1.PreferredValues = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3");
//            articleDefinition.ArticleParameterDefinitions.Add (articleParameter1);

//            var articleParameter2 = dataContext.CreateEntity<EnumValueArticleParameterDefinitionEntity> ();
//            articleParameter2.Code = "p2";
//            articleParameter2.Cardinality = EnumValueCardinality.ExactlyOne;
//            articleParameter2.Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3");
//            articleParameter2.DefaultValue = "1";
//            articleDefinition.ArticleParameterDefinitions.Add (articleParameter2);

//            NumericDimension dimension1 = PriceCalculatorEntity.CreateDimension (articleParameter1, RoundingMode.Down);
//            CodeDimension dimension2 = PriceCalculatorEntity.CreateDimension (articleParameter2);
//            DimensionTable priceTable = new DimensionTable (dimension2, dimension1);
//            int value = 1;
//            foreach (object o1 in dimension1.Values)
//            {
//                foreach (object o2 in dimension2.Values)
//                {
//                    priceTable[o1, o2] = value;
//                    value++;
//                }
//            }

//            var priceCalculator1 = dataContext.CreateEntity<PriceCalculatorEntity> ();
//            priceCalculator1.SetPriceTable (articleDefinition, priceTable);
//            articlePrice.PriceCalculators.Add (priceCalculator1);

//            priceTable[1m, "3"] = null;

//            var priceCalculator2 = dataContext.CreateEntity<PriceCalculatorEntity> ();
//            priceCalculator2.SetPriceTable (articleDefinition, priceTable);
//            articlePrice.PriceCalculators.Add (priceCalculator2);

//            var articleItem = dataContext.CreateEntity<ArticleDocumentItemEntity> ();
//            articleItem.ArticleDefinition = articleDefinition;

//            var parameterValues = new string[] { "p1", "1.5", "p2", "3" };
//            articleItem.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, parameterValues);

//            dataContext.SaveChanges ();
//        }


//        [TestMethod]
//        public void ComputeArgumentCheck()
//        {
//            ExceptionAssert.Throw<System.ArgumentNullException>
//            (
//                () => new PriceCalculatorEntity ().Compute (null)
//            );
//        }


//        [TestMethod]
//        public void ComputeWithDeserializationProblem()
//        {
//            var articleDefinition = new ArticleDefinitionEntity ();

//            var articlePrice = new ArticlePriceEntity
//            {
//                BeginDate = System.DateTime.MinValue,
//                EndDate = System.DateTime.MaxValue
//            };
//            articleDefinition.ArticlePrices.Add (articlePrice);

//            var articleParameter = new EnumValueArticleParameterDefinitionEntity
//            {
//                Code = "p",
//                DefaultValue = "1",
//                Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3")
//            };
//            articleDefinition.ArticleParameterDefinitions.Add (articleParameter);

//            var articleItem = new ArticleDocumentItemEntity
//            {
//                ArticleDefinition = articleDefinition,
//                ArticleParameters = string.Join (AbstractArticleParameterController.Separator, new string[] { "p", "2", }),
//            };

//            CodeDimension dimension1 = new CodeDimension ("p", new string[] { "1", "2", });
//            CodeDimension dimension2 = new CodeDimension ("p", new string[] { "1", "2", "4" });
//            CodeDimension dimension3 = new CodeDimension ("X", new string[] { "1", "2", "3" });

//            DimensionTable table1 = new DimensionTable (dimension1);
//            DimensionTable table2 = new DimensionTable (dimension2);
//            DimensionTable table3 = new DimensionTable (dimension3);

//            PriceCalculatorEntity pce = new PriceCalculatorEntity ();

//            pce.SerializedData = this.EmulateByteSerialization (table1);

//            ExceptionAssert.Throw
//            (
//                () => pce.Compute (articleItem)
//            );

//            pce.SerializedData = this.EmulateByteSerialization (table2);

//            ExceptionAssert.Throw
//            (
//                () => pce.Compute (articleItem)
//            );

//            pce.SerializedData = this.EmulateByteSerialization (table3);

//            ExceptionAssert.Throw
//            (
//                () => pce.Compute (articleItem)
//            );
//        }


//        private byte[] EmulateByteSerialization(DimensionTable table)
//        {
//            XElement xTable = table.XmlExport ();
//            string tableDataAsXmlString = xTable.ToString (SaveOptions.DisableFormatting);
//            byte[] tableDataAsByteArray = Encoding.UTF8.GetBytes (tableDataAsXmlString);

//            return tableDataAsByteArray;
//        }


//        [TestMethod]
//        public void ComputeForNumericParameterTest()
//        {
//            using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
//            {
//                using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
//                {
//                    dataInfrastructure.OpenConnection ("id");

//                    using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
//                    {
//                        var articleItem = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000001)));
//                        var priceCalculator1 = articleItem.ArticleDefinition.ArticlePrices.First ().PriceCalculators[0];
//                        var priceCalculator2 = articleItem.ArticleDefinition.ArticlePrices.First ().PriceCalculators[1];

//                        Assert.AreEqual (2m, priceCalculator1.Compute (articleItem));
//                        Assert.IsNull (priceCalculator2.Compute (articleItem));
//                    }
//                }
//            }
//        }


//        [TestMethod]
//        public void ComputeForEnumZeroOrOneParameterTest()
//        {
//            using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
//            {
//                using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
//                {
//                    dataInfrastructure.OpenConnection ("id");

//                    using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
//                    {
//                        var articleItem1 = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000002)));
//                        var articleItem2 = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000003)));

//                        var priceCalculator1 = articleItem1.ArticleDefinition.ArticlePrices.First ().PriceCalculators[0];
//                        var priceCalculator2 = articleItem2.ArticleDefinition.ArticlePrices.First ().PriceCalculators[1];

//                        Assert.AreEqual (2m, priceCalculator1.Compute (articleItem1));
//                        Assert.AreEqual (0m, priceCalculator1.Compute (articleItem2));
//                        Assert.IsNull (priceCalculator2.Compute (articleItem1));
//                        Assert.AreEqual (0m, priceCalculator2.Compute (articleItem2));
//                    }
//                }
//            }
//        }


//        [TestMethod]
//        public void ComputeForEnumExactlyOneParameterTest()
//        {
//            using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
//            {
//                using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
//                {
//                    dataInfrastructure.OpenConnection ("id");

//                    using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
//                    {
//                        var articleItem = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000004)));
//                        var priceCalculator1 = articleItem.ArticleDefinition.ArticlePrices.First ().PriceCalculators[0];
//                        var priceCalculator2 = articleItem.ArticleDefinition.ArticlePrices.First ().PriceCalculators[1];

//                        Assert.AreEqual (2m, priceCalculator1.Compute (articleItem));
//                        Assert.IsNull (priceCalculator2.Compute (articleItem));
//                    }
//                }
//            }
//        }


//        [TestMethod]
//        public void ComputeForEnumAtLeastOneParameterTest()
//        {
//            using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
//            {
//                using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
//                {
//                    dataInfrastructure.OpenConnection ("id");

//                    using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
//                    {
//                        var articleItem1 = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000005)));
//                        var articleItem2 = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000006)));

//                        var priceCalculator1 = articleItem1.ArticleDefinition.ArticlePrices.First ().PriceCalculators[0];
//                        var priceCalculator2 = articleItem2.ArticleDefinition.ArticlePrices.First ().PriceCalculators[1];
						
//                        Assert.AreEqual (2m, priceCalculator1.Compute (articleItem1));
//                        Assert.AreEqual (5m, priceCalculator1.Compute (articleItem2));
//                        Assert.IsNull (priceCalculator2.Compute (articleItem1));
//                        Assert.IsNull (priceCalculator2.Compute (articleItem2));
//                    }
//                }
//            }
//        }


//        [TestMethod]
//        public void ComputeForEnumAnyParameterTest()
//        {
//            using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
//            {
//                using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
//                {
//                    dataInfrastructure.OpenConnection ("id");

//                    using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
//                    {
//                        var articleItem1 = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000007)));
//                        var articleItem2 = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000008)));
//                        var articleItem3 = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000009)));
						
//                        var priceCalculator1 = articleItem1.ArticleDefinition.ArticlePrices.First ().PriceCalculators[0];
//                        var priceCalculator2 = articleItem1.ArticleDefinition.ArticlePrices.First ().PriceCalculators[1];

//                        Assert.AreEqual (2m, priceCalculator1.Compute (articleItem1));
//                        Assert.AreEqual (5m, priceCalculator1.Compute (articleItem2));
//                        Assert.AreEqual (0m, priceCalculator1.Compute (articleItem3));
//                        Assert.IsNull (priceCalculator2.Compute (articleItem1));
//                        Assert.IsNull (priceCalculator2.Compute (articleItem2));
//                        Assert.AreEqual (0m, priceCalculator2.Compute (articleItem3));
//                    }
//                }
//            }
//        }


//        [TestMethod]
//        public void ComputeForMultipleParametersTest()
//        {
//            using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
//            {
//                using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
//                {
//                    dataInfrastructure.OpenConnection ("id");

//                    using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
//                    {
//                        var articleItem = dataContext.ResolveEntity<ArticleDocumentItemEntity> (new DbKey (new DbId (1000000010)));
//                        var priceCalculator1 = articleItem.ArticleDefinition.ArticlePrices.First ().PriceCalculators[0];
//                        var priceCalculator2 = articleItem.ArticleDefinition.ArticlePrices.First ().PriceCalculators[1];

//                        Assert.AreEqual (3m, priceCalculator1.Compute (articleItem));
//                        Assert.IsNull (priceCalculator2.Compute (articleItem));
//                    }
//                }
//            }
//        }


//        [TestMethod]
//        public void SetPriceTableArgumentCheck()
//        {
//            var articleDefinition = new ArticleDefinitionEntity ();

//            var articlePrice = new ArticlePriceEntity
//            {
//                BeginDate = System.DateTime.MinValue,
//                EndDate = System.DateTime.MaxValue
//            };
//            articleDefinition.ArticlePrices.Add (articlePrice);

//            var articleParameter = new EnumValueArticleParameterDefinitionEntity
//            {
//                Code = "p",
//                DefaultValue = "1",
//                Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3")
//            };
//            articleDefinition.ArticleParameterDefinitions.Add (articleParameter);
					
//            CodeDimension dimension1 = new CodeDimension ("p", new string[] { "1", "2", });
//            CodeDimension dimension2 = new CodeDimension ("p", new string[] { "1", "2", "4" });
//            CodeDimension dimension3 = new CodeDimension ("X", new string[] { "1", "2", "3" });

//            DimensionTable table1 = new DimensionTable (dimension1);
//            DimensionTable table2 = new DimensionTable (dimension2);
//            DimensionTable table3 = new DimensionTable (dimension3);

//            ExceptionAssert.Throw<System.ArgumentNullException>
//            (
//                () => new PriceCalculatorEntity ().SetPriceTable (articleDefinition, null)
//            );

//            ExceptionAssert.Throw<System.ArgumentNullException>
//            (
//                () => new PriceCalculatorEntity ().SetPriceTable (null, new DimensionTable (new CodeDimension ("p", new string[] { "1", "2", "3" })))
//            );

//            ExceptionAssert.Throw<System.Exception>
//            (
//                () => new PriceCalculatorEntity ().SetPriceTable (articleDefinition, table1)
//            );

//            ExceptionAssert.Throw<System.Exception>
//            (
//                () => new PriceCalculatorEntity ().SetPriceTable (articleDefinition, table2)
//            );

//            ExceptionAssert.Throw<System.Exception>
//            (
//                () => new PriceCalculatorEntity ().SetPriceTable (articleDefinition, table3)
//            );
//        }


//        [TestMethod]
//        public void GetAndSetPriceTableTest()
//        {
//            var articleDefinition = new ArticleDefinitionEntity ();

//            var articlePrice = new ArticlePriceEntity ();
//            articlePrice.BeginDate = System.DateTime.MinValue;
//            articlePrice.EndDate = System.DateTime.MaxValue;
//            articleDefinition.ArticlePrices.Add (articlePrice);

//            var articleParameter1 = new EnumValueArticleParameterDefinitionEntity ();
//            articleParameter1.Code = "p1";
//            articleParameter1.DefaultValue = "1";
//            articleParameter1.Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3");
//            articleDefinition.ArticleParameterDefinitions.Add (articleParameter1);

//            var articleParameter2 = new EnumValueArticleParameterDefinitionEntity ();
//            articleParameter2.Code = "p2";
//            articleParameter2.DefaultValue = "4";
//            articleParameter2.Values = AbstractArticleParameterDefinitionEntity.Join ("4", "5", "6");
//            articleDefinition.ArticleParameterDefinitions.Add (articleParameter2);
			
//            CodeDimension dimension1 = new CodeDimension ("p1", new string[] { "1", "2", "3"});
//            CodeDimension dimension2 = new CodeDimension ("p2", new string[] { "4", "5", "6"});

//            DimensionTable table1 = new DimensionTable (dimension1);
//            DimensionTable table2 = new DimensionTable (dimension2);

//            PriceCalculatorEntity pce = new PriceCalculatorEntity ();

//            Assert.IsNull (pce.GetPriceTable ());

//            pce.SetPriceTable (articleDefinition, table1);

//            Assert.AreEqual (1, pce.GetPriceTable ().Dimensions.Count ());
//            Assert.AreEqual (dimension1.Name, pce.GetPriceTable ().Dimensions.First ().Name);
//            CollectionAssert.AreEqual (dimension1.Values.ToList (), pce.GetPriceTable ().Dimensions.First ().Values.ToList ());

//            pce.SetPriceTable (articleDefinition, table2);

//            Assert.AreEqual (1, pce.GetPriceTable ().Dimensions.Count ());
//            Assert.AreEqual (dimension2.Name, pce.GetPriceTable ().Dimensions.First ().Name);
//            CollectionAssert.AreEqual (dimension2.Values.ToList (), pce.GetPriceTable ().Dimensions.First ().Values.ToList ());
//        }


//        [TestMethod]
//        public void CreateNumericDimensionArgumentCheck()
//        {
//            ExceptionAssert.Throw<System.ArgumentNullException>
//            (
//                () => PriceCalculatorEntity.CreateDimension ((NumericValueArticleParameterDefinitionEntity) null, RoundingMode.None)
//            );
//        }


//        [TestMethod]
//        public void CreateNumericDimensionTest()
//        {
//            NumericValueArticleParameterDefinitionEntity parameter = new NumericValueArticleParameterDefinitionEntity ()
//            {
//                Code = "code",
//                PreferredValues = AbstractArticleParameterDefinitionEntity.Join("1", "2", "3"),
//            };

//            NumericDimension dimension = PriceCalculatorEntity.CreateDimension (parameter, RoundingMode.Nearest);

//            Assert.AreEqual (parameter.Code, dimension.Name);
//            CollectionAssert.AreEqual (new List<decimal> () { 1, 2, 3 }, dimension.Values.ToList ());
//            Assert.AreEqual (RoundingMode.Nearest, dimension.RoundingMode);
//        }


//        [TestMethod]
//        public void CreateCodeDimensionArgumentCheck()
//        {
//            ExceptionAssert.Throw<System.ArgumentNullException>
//            (
//                () => PriceCalculatorEntity.CreateDimension ((EnumValueArticleParameterDefinitionEntity) null)
//            );

//            ExceptionAssert.Throw<System.ArgumentException>
//            (
//                () => PriceCalculatorEntity.CreateDimension (new EnumValueArticleParameterDefinitionEntity ()
//                {
//                    Cardinality = EnumValueCardinality.Any,
//                })
//            );

//            ExceptionAssert.Throw<System.ArgumentException>
//            (
//                () => PriceCalculatorEntity.CreateDimension (new EnumValueArticleParameterDefinitionEntity ()
//                {
//                    Cardinality = EnumValueCardinality.ZeroOrOne,
//                })
//            );

//            ExceptionAssert.Throw<System.ArgumentException>
//            (
//                () => PriceCalculatorEntity.CreateDimension (new EnumValueArticleParameterDefinitionEntity ()
//                {
//                    Cardinality = EnumValueCardinality.AtLeastOne,
//                })
//            );
//        }


//        [TestMethod]
//        public void CreateCodeDimensionTest()
//        {
//            EnumValueArticleParameterDefinitionEntity parameter = new EnumValueArticleParameterDefinitionEntity ()
//            {
//                Code = "code",
//                Cardinality = EnumValueCardinality.ExactlyOne,
//                Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3"),
//            };

//            CodeDimension dimension = PriceCalculatorEntity.CreateDimension (parameter);

//            Assert.AreEqual (parameter.Code, dimension.Name);
//            CollectionAssert.AreEqual (new List<string> () { "1", "2", "3" }, dimension.Values.ToList ());
//        }


//        [TestMethod]
//        public void CreateNumericPriceTableArgumentCheck()
//        {
//            NumericValueArticleParameterDefinitionEntity parameter = new NumericValueArticleParameterDefinitionEntity ()
//            {
//                Code = "Name",
//                PreferredValues = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3"),
//            };

//            Dictionary<decimal, decimal> codeToValues = new Dictionary<decimal, decimal> ()
//            {
//                {1, 1},
//                {2, 2},
//                {3, 3},
//            };

//            ExceptionAssert.Throw<System.ArgumentNullException>
//            (
//                () => PriceCalculatorEntity.CreatePriceTable ((NumericValueArticleParameterDefinitionEntity) null, codeToValues, RoundingMode.Nearest)
//            );

//            ExceptionAssert.Throw<System.ArgumentNullException>
//            (
//                () => PriceCalculatorEntity.CreatePriceTable (parameter, null, RoundingMode.Nearest)
//            );

//            ExceptionAssert.Throw<System.ArgumentException>
//            (
//                () => PriceCalculatorEntity.CreatePriceTable (parameter, new Dictionary<decimal, decimal> (), RoundingMode.Nearest)
//            );
//        }


//        [TestMethod]
//        public void CreateNumericPriceTableTest()
//        {
//            NumericValueArticleParameterDefinitionEntity parameter = new NumericValueArticleParameterDefinitionEntity ()
//            {
//                Code = "Name",
//                PreferredValues = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3"),
//            };

//            Dictionary<decimal, decimal> codeToValues = new Dictionary<decimal, decimal> ()
//            {
//                {1, 1},
//                {2, 2},
//                {3, 3},
//            };

//            DimensionTable table = PriceCalculatorEntity.CreatePriceTable (parameter, codeToValues, RoundingMode.Down);

//            Assert.AreEqual (1, table.Dimensions.Count ());
//            Assert.AreEqual (parameter.Code, table.Dimensions.First ().Name);

//            foreach (var item in codeToValues)
//            {
//                Assert.AreEqual (item.Value, table[item.Key]);
//            }
//        }


//        [TestMethod]
//        public void CreateCodePriceTableArgumentCheck()
//        {
//            EnumValueArticleParameterDefinitionEntity parameter = new EnumValueArticleParameterDefinitionEntity ()
//            {
//                Code = "Name",
//                Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3"),
//            };

//            Dictionary<string, decimal> codeToValues = new Dictionary<string, decimal> ()
//            {
//                {"1", 1},
//                {"2", 2},
//                {"3", 3},
//            };

//            ExceptionAssert.Throw<System.ArgumentNullException>
//            (
//                () => PriceCalculatorEntity.CreatePriceTable ((EnumValueArticleParameterDefinitionEntity) null, codeToValues)
//            );

//            ExceptionAssert.Throw<System.ArgumentNullException>
//            (
//                () => PriceCalculatorEntity.CreatePriceTable (parameter, null)
//            );

//            ExceptionAssert.Throw<System.ArgumentException>
//            (
//                () => PriceCalculatorEntity.CreatePriceTable (parameter, new Dictionary<string, decimal> ())
//            );
//        }


//        [TestMethod]
//        public void CreateCodePriceTableTest()
//        {
//            EnumValueArticleParameterDefinitionEntity parameter = new EnumValueArticleParameterDefinitionEntity ()
//            {
//                Code = "Name",
//                Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3"),
//            };

//            Dictionary<string, decimal> codeToValues = new Dictionary<string, decimal> ()
//            {
//                {"1", 1},
//                {"2", 2},
//                {"3", 3},
//            };

//            DimensionTable table = PriceCalculatorEntity.CreatePriceTable (parameter, codeToValues);

//            Assert.AreEqual (1, table.Dimensions.Count ());
//            Assert.AreEqual (parameter.Code, table.Dimensions.First ().Name);

//            foreach (var item in codeToValues)
//            {
//                Assert.AreEqual (item.Value, table[item.Key]);
//            }
//        }


//        [TestMethod]
//        public void CreateKeyArgumentCheck()
//        {
//            ExceptionAssert.Throw<System.ArgumentNullException>
//            (
//                () => PriceCalculatorEntity.CreateKey (null)
//            );
//        }


//        [TestMethod]
//        public void CreateKeyTest()
//        {
//            int nbParameters = 10;
			
//            var parametersToValues = new Dictionary<AbstractArticleParameterDefinitionEntity, object> ();
//            object[] expected = new object[nbParameters];
			
//            for (int i = 0; i < nbParameters; i++)
//            {
//                var articleParameter = new EnumValueArticleParameterDefinitionEntity ();
//                articleParameter.Code = "1";
//                articleParameter.Cardinality = EnumValueCardinality.ZeroOrOne;
//                articleParameter.Values = AbstractArticleParameterDefinitionEntity.Join ("0", "1", "2", "3", "4", "5", "6", "7", "8", "9");
//                articleParameter.DefaultValue = "0";

//                parametersToValues[articleParameter] =  i;
//                expected[i] = i;
//            }

//            CollectionAssert.AreEqual (expected, PriceCalculatorEntity.CreateKey (parametersToValues));
//        }
		

//    }


//}
