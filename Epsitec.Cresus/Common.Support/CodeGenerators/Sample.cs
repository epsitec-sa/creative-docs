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
	class Unit�
	{
	}
	class ArticleStock
	{
	}
	class Article : AbstractEntity
	{
		public string Num�ro
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

		public string D�signation
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

		public Unit� Unit�
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

		public int Quantit�EnStock
		{
			get
			{
				return 0;
			}
		}
	}
}
