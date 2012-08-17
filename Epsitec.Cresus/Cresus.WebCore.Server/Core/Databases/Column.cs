using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Expressions;

using System;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal sealed class Column
	{


		public Column(string title, string name, ColumnType type, bool sortable, SortOrder? sortOrder, LambdaExpression lambdaExpression)
		{
			this.title = title;
			this.name = name;
			this.type = type;
			this.sortable = sortable;
			this.sortOrder = sortOrder;
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


		public ColumnType Type
		{
			get
			{
				return this.type;
			}
		}


		public bool Sortable
		{
			get
			{
				return this.sortable;
			}
		}


		public SortOrder? SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}


		public LambdaExpression LambdaExpression
		{
			get
			{
				return this.lambdaExpression;
			}
		}


		public static Column Create<T1, T2>(string title, string name, ColumnType type, bool sortable, SortOrder? sortOrder, Expression<Func<T1, T2>> lambdaExpression)
		{
			return new Column(title, name, type, sortable, sortOrder, lambdaExpression);
		}


		private readonly string title;


		private readonly string name;


		private readonly ColumnType type;


		private readonly bool sortable;


		private readonly SortOrder? sortOrder;


		private readonly LambdaExpression lambdaExpression;


	}


}
