//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Library.Settings;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Bricks
{
	public sealed class FieldInfo
	{
		public FieldInfo(Druid entityId, LambdaExpression lambda)
		{
			this.entityId = entityId;
			this.fieldId  = FieldInfo.GetTypeIdFromLambda (lambda);
			this.name     = lambda.ToString ();
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public Druid EntityId
		{
			get
			{
				return this.entityId;
			}
		}

		public Druid FieldId
		{
			get
			{
				return this.fieldId;
			}
		}

		public TileFieldEditionSettings Settings
		{
			get;
			set;
		}

		
		private static Druid GetTypeIdFromLambda(LambdaExpression lambda)
		{
			switch (lambda.Body.NodeType)
			{
				case ExpressionType.MemberAccess:
					return FieldInfo.GetTypeIdFromMemberAccess (lambda.Body as MemberExpression);

				case ExpressionType.Parameter:
					return Druid.Empty;

				default:
					return Druid.Empty;
			}
		}

		private static Druid GetTypeIdFromMemberAccess(MemberExpression lambdaMember)
		{
			if (lambdaMember == null)
			{
				return Druid.Empty;
			}

			var propertyInfo = lambdaMember.Member as System.Reflection.PropertyInfo;
			var caption      = EntityInfo.GetFieldCaption (propertyInfo);

			if (caption == null)
			{
				return Druid.Empty;
			}
			else
			{
				return caption.Id;
			}
		}
		

		private readonly string name;
		private readonly Druid entityId;
		private readonly Druid fieldId;
	}
}
