#if false
//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Data
{
	public class CoreDataBrowser<T>
		where T : AbstractEntity
	{
		public CoreDataBrowser()
		{
			this.entityId = EntityClassFactory.GetEntityId (typeof (T));
			this.fieldPaths = new List<EntityFieldPath> ();
		}

		public void AddColumn(System.Linq.Expressions.Expression<System.Func<T, string>> expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Lambda:
					this.fieldPaths.Add (this.GetEntityFieldPath (expression as LambdaExpression));
					break;

				default:
					throw new System.NotSupportedException ();
			}
		}

		public void AddColumn(System.Linq.Expressions.Expression<System.Func<T, FormattedText>> expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Lambda:
					this.fieldPaths.Add (this.GetEntityFieldPath (expression as LambdaExpression));
					break;

				default:
					throw new System.NotSupportedException ();
			}
		}

		private EntityFieldPath GetEntityFieldPath(LambdaExpression expression)
		{
			var body = expression.Body;

			if ((expression.ReturnType != typeof (string)) &&
				(expression.ReturnType != typeof (FormattedText)))
			{
				throw new System.InvalidOperationException ("Lambda does not return a supported type");
			}

			var members = new List<Node> ();
			var castTo  = Druid.Empty;

			while (body != null)
			{
				switch (body.NodeType)
				{
					case ExpressionType.MemberAccess:
						MemberExpression member = body as MemberExpression;
						body = member.Expression;
						if (member.Member is System.Reflection.PropertyInfo)
						{
							members.Add (new Node (member.Member as System.Reflection.PropertyInfo, castTo));
							castTo = Druid.Empty;
							continue;
						}
						break;

					case ExpressionType.Call:
						MethodCallExpression call = body as MethodCallExpression;
						if ((call.Method.MemberType == System.Reflection.MemberTypes.Method) &&
							(call.Method.DeclaringType == typeof (CastExtension)) &&
							(call.Method.Name == "CastTo"))
						{
							var returnType = call.Method.ReturnType;
							castTo = EntityClassFactory.GetEntityId (returnType);
							body = call.Arguments[0];
							continue;
						}
						break;

					case ExpressionType.Parameter:
						body = null;
						continue;
				}

				throw new System.InvalidOperationException ("Cannot map expression to a field path");
			}

			if (members.Count == 0)
			{
				throw new System.InvalidOperationException ("Cannot map flat expression to a field path");
			}
			
			members.Reverse ();
			return EntityFieldPath.CreateAbsolutePath (this.entityId, members.Select (node => node.FieldId));
		}

		class Node
		{
			public Node(System.Reflection.PropertyInfo propertyInfo, Druid castTo)
			{
				var attribute = propertyInfo == null ? null : propertyInfo.GetCustomAttributes (typeof (EntityFieldAttribute), false).FirstOrDefault () as EntityFieldAttribute;
				this.fieldId = attribute == null ? null : attribute.FieldId;
				this.castTo  = castTo;
			}

			public string FieldId
			{
				get
				{
					return this.fieldId;
				}
			}

			public Druid CastTo
			{
				get
				{
					return this.castTo;
				}
			}

			private readonly string fieldId;
			private readonly Druid  castTo;
		}

		private readonly Druid entityId;
		private readonly List<EntityFieldPath> fieldPaths;
	}

	public static class CastExtension
	{
		public static T CastTo<T>(this AbstractEntity value)
			where T : AbstractEntity
		{
			return (T) value;
		}
	}

	public static class Test
	{
		public static void Example1()
		{
			var browser = new CoreDataBrowser<RelationEntity> ();

			browser.AddColumn (customer => customer.IdA);
			browser.AddColumn (customer => customer.DefaultAddress.Location.PostalCode);
			browser.AddColumn (customer => customer.DefaultAddress.Location.Name);
			browser.AddColumn (customer => customer.Person.CastTo<LegalPersonEntity> ().Name);
			browser.AddColumn (customer => customer.Person.CastTo<NaturalPersonEntity> ().Firstname);
			browser.AddColumn (customer => customer.Person.CastTo<NaturalPersonEntity> ().Lastname);
		}
	}
}
#endif
