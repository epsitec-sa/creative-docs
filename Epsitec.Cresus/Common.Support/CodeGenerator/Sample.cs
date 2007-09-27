namespace Epsitec.Demo5juin
{
	abstract class AbstractEntity
	{
		public object GetFieldValue(string fieldId)
		{
			return null;
		}

		public void SetFieldValue(string fieldId, object value)
		{
		}
	}

	class Prix
	{
	}
	class Unité
	{
	}
	class ArticleStock
	{
	}
	class Article : AbstractEntity
	{
		public string Numéro
		{
			get
			{
				return (string) this.GetFieldValue ("[1234]");
			}
			set
			{
				this.SetFieldValue ("[1234]", value);
			}
		}

		public string Désignation
		{
			get
			{
				return null;
			}
		}


		public Prix PrixVente
		{
			get
			{
				return null;
			}
		}

		public Unité Unité
		{
			get
			{
				return null;
			}
		}

		public System.Collections.Generic.IList<ArticleStock> ArticlesEnStock
		{
			get
			{
				return null;
			}
		}

		public int QuantitéEnStock
		{
			get
			{
				return 0;
			}
		}
	}
}
