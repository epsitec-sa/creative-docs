using Epsitec.Common.Support.EntityEngine;

using System;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal sealed class Column
	{


		public Column(string title, string name, ColumnType type, LambdaExpression lambdaExpression)
		{
			this.title = title;
			this.name = name;
			this.type = type;
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


		public LambdaExpression LambdaExpression
		{
			get
			{
				return this.lambdaExpression;
			}
		}


		public static Column Create<T1, T2>(string title, string name, ColumnType type, Expression<Func<T1, T2>> lambdaExpression)
		{
			return new Column(title, name, type, lambdaExpression);
		}


		private readonly string title;


		private readonly string name;


		private readonly ColumnType type;


		private readonly LambdaExpression lambdaExpression;


	}


}
