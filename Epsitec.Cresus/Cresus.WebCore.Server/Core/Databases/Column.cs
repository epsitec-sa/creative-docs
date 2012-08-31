using Epsitec.Cresus.DataLayer.Expressions;

using System;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal sealed class Column
	{


		public Column(string title, string name, bool hidden, bool sortable, bool filterable, LambdaExpression lambdaExpression)
		{
			this.title = title;
			this.name = name;
			this.hidden = hidden;
			this.sortable = sortable;
			this.filterable = filterable;
			this.lambdaExpression = lambdaExpression;
		}


		public string Title
		{
			get
			{
				return this.title;
			}
		}


		public string Name
		{
			get
			{
				return this.name;
			}
		}


		public bool Hidden
		{
			get
			{
				return this.hidden;
			}
		}


		public bool Sortable
		{
			get
			{
				return this.sortable;
			}
		}


		public bool Filterable
		{
			get
			{
				return this.filterable;
			}
		}


		public LambdaExpression LambdaExpression
		{
			get
			{
				return this.lambdaExpression;
			}
		}


		public static Column Create<T1, T2>(string title, string name, bool hidden, bool sortable, bool filterable, Expression<Func<T1, T2>> lambdaExpression)
		{
			return new Column(title, name, hidden, sortable, filterable, lambdaExpression);
		}


		private readonly string title;


		private readonly string name;


		private readonly bool hidden;


		private readonly bool sortable;


		private readonly bool filterable;


		private readonly LambdaExpression lambdaExpression;


	}


}
