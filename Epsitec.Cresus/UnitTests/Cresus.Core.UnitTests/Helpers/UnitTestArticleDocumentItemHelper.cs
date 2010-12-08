using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers.ArticleParameterControllers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.Core.UnitTests.Helpers
{


	[TestClass]
	public class UnitTestArticleDocumentItemHelper
	{

		
		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void GetParameterCodesToValuesArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ArticleDocumentItemHelper.GetParameterCodesToValues (null)
			);
		}


		[TestMethod]
		public void GetParameterCodesToValuesTest()
		{
			var articleDefinition = new ArticleDefinitionEntity ();

			var articleParameter1 = new NumericValueArticleParameterDefinitionEntity
			{
				Code = "p1",
				DefaultValue = 1,
				PreferredValues = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3")
			};

			var articleParameter2 = new EnumValueArticleParameterDefinitionEntity
			{
				Cardinality = EnumValueCardinality.Any,
				Code = "p2",
				DefaultValue = "1",
				Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3")
			};

			var articleParameter3 = new EnumValueArticleParameterDefinitionEntity
			{
				Cardinality = EnumValueCardinality.AtLeastOne,
				Code = "p3",
				DefaultValue = "1",
				Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3")
			};

			var articleParameter4 = new EnumValueArticleParameterDefinitionEntity
			{
				Cardinality = EnumValueCardinality.ExactlyOne,
				Code = "p4",
				DefaultValue = "1",
				Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3")
			};

			var articleParameter5 = new EnumValueArticleParameterDefinitionEntity
			{
				Cardinality = EnumValueCardinality.ZeroOrOne,
				Code = "p5",
				DefaultValue = "1",
				Values = AbstractArticleParameterDefinitionEntity.Join ("1", "2", "3")
			};

			var articleParameter6 = new FreeTextValueArticleParameterDefinitionEntity
			{
				Code = "p6",
				ShortText = "1",
			};

			articleDefinition.ArticleParameterDefinitions.Add (articleParameter1);
			articleDefinition.ArticleParameterDefinitions.Add (articleParameter2);
			articleDefinition.ArticleParameterDefinitions.Add (articleParameter3);
			articleDefinition.ArticleParameterDefinitions.Add (articleParameter4);
			articleDefinition.ArticleParameterDefinitions.Add (articleParameter5);
			articleDefinition.ArticleParameterDefinitions.Add (articleParameter6);

			var articleItem1 = new ArticleDocumentItemEntity
			{
				ArticleDefinition = articleDefinition,
				ArticleParameters = string.Join (AbstractArticleParameterController.Separator, new string[] { "p1", "2", "p2", "2", "p3", "2", "p4", "2", "p5", "2", "p6", "2" })
			};

			var articleItemValues1 = ArticleDocumentItemHelper.GetParameterCodesToValues (articleItem1);

			Assert.AreEqual (2m, articleItemValues1["p1"].Single ());
			Assert.AreEqual ("2", articleItemValues1["p2"].Single ());
			Assert.AreEqual ("2", articleItemValues1["p3"].Single ());
			Assert.AreEqual ("2", articleItemValues1["p4"].Single ());
			Assert.AreEqual ("2", articleItemValues1["p5"].Single ());
			Assert.AreEqual ("2", articleItemValues1["p6"].Single ());

			var articleItem2 = new ArticleDocumentItemEntity
			{
				ArticleDefinition = articleDefinition,
				ArticleParameters = string.Join (AbstractArticleParameterController.Separator, new string[] { "p1", "2", "p2", "", "p3", "2", "p4", "2", "p5", "2", "p6", "2" })
			};

			var articleItemValues2 = ArticleDocumentItemHelper.GetParameterCodesToValues (articleItem2);

			Assert.AreEqual (2m, articleItemValues2["p1"].Single ());
			Assert.IsFalse (articleItemValues2.ContainsKey("p2"));
			Assert.AreEqual ("2", articleItemValues2["p3"].Single ());
			Assert.AreEqual ("2", articleItemValues2["p4"].Single ());
			Assert.AreEqual ("2", articleItemValues2["p5"].Single ());
			Assert.AreEqual ("2", articleItemValues2["p6"].Single ());

			var articleItem3 = new ArticleDocumentItemEntity
			{
				ArticleDefinition = articleDefinition,
				ArticleParameters = string.Join (AbstractArticleParameterController.Separator, new string[] { "p1", "2", "p2", AbstractArticleParameterDefinitionEntity.Join ("2", "3"), "p3", "2", "p4", "2", "p5", "2", "p6", "2" })
			};

			var articleItemValues3 = ArticleDocumentItemHelper.GetParameterCodesToValues (articleItem3);

			Assert.AreEqual (2m, articleItemValues3["p1"].Single ());
			Assert.AreEqual (2, articleItemValues3["p2"].Count ());
			Assert.AreEqual ("2", articleItemValues3["p2"].ElementAt (0));
			Assert.AreEqual ("3", articleItemValues3["p2"].ElementAt (1));
			Assert.AreEqual ("2", articleItemValues3["p3"].Single ());
			Assert.AreEqual ("2", articleItemValues3["p4"].Single ());
			Assert.AreEqual ("2", articleItemValues3["p5"].Single ());
			Assert.AreEqual ("2", articleItemValues3["p6"].Single ());

			var articleItem4 = new ArticleDocumentItemEntity
			{
				ArticleDefinition = articleDefinition,
				ArticleParameters = string.Join (AbstractArticleParameterController.Separator, new string[] { "p1", "2", "p2", "2", "p3", AbstractArticleParameterDefinitionEntity.Join ("2", "3"), "p4", "2", "p5", "2", "p6", "2" })
			};

			var articleItemValues4 = ArticleDocumentItemHelper.GetParameterCodesToValues (articleItem4);

			Assert.AreEqual (2m, articleItemValues4["p1"].Single ());
			Assert.AreEqual ("2", articleItemValues4["p2"].Single ());
			Assert.AreEqual (2, articleItemValues4["p3"].Count ());
			Assert.AreEqual ("2", articleItemValues4["p3"].ElementAt (0));
			Assert.AreEqual ("3", articleItemValues4["p3"].ElementAt (1));
			Assert.AreEqual ("2", articleItemValues4["p4"].Single ());
			Assert.AreEqual ("2", articleItemValues4["p5"].Single ());
			Assert.AreEqual ("2", articleItemValues4["p6"].Single ());

			var articleItem5 = new ArticleDocumentItemEntity
			{
				ArticleDefinition = articleDefinition,
				ArticleParameters = string.Join (AbstractArticleParameterController.Separator, new string[] { "p1", "2", "p2", "2", "p3", "2", "p4", "2", "p5", "", "p6", "2" })
			};

			var articleItemValues5 = ArticleDocumentItemHelper.GetParameterCodesToValues (articleItem5);

			Assert.AreEqual (2m, articleItemValues5["p1"].Single ());
			Assert.AreEqual ("2", articleItemValues5["p2"].Single ());
			Assert.AreEqual ("2", articleItemValues5["p3"].Single ());
			Assert.AreEqual ("2", articleItemValues5["p4"].Single ());
			Assert.IsFalse (articleItemValues5.ContainsKey ("p5"));
			Assert.AreEqual ("2", articleItemValues5["p6"].Single ());

			var articleItem6 = new ArticleDocumentItemEntity
			{
				ArticleDefinition = articleDefinition,
				ArticleParameters = "",
			};

			var articleItemValues6 = ArticleDocumentItemHelper.GetParameterCodesToValues (articleItem6);

			Assert.AreEqual (1m, articleItemValues6["p1"].Single ());
			Assert.AreEqual ("1", articleItemValues6["p2"].Single ());
			Assert.AreEqual ("1", articleItemValues6["p3"].Single ());
			Assert.AreEqual ("1", articleItemValues6["p4"].Single ());
			Assert.AreEqual ("1", articleItemValues6["p5"].Single ());
			Assert.AreEqual ("1", articleItemValues6["p6"].Single ());
		}


	}


}
