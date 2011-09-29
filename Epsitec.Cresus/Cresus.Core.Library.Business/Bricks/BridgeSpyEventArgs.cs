//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Bricks
{
	public class BridgeSpyEventArgs : EventArgs
	{
		public BridgeSpyEventArgs(AbstractEntity entity, LambdaExpression lambdaExpression, object oldValue, object newValue)
		{
			this.entity           = entity;
			this.lambdaExpression = lambdaExpression;
			this.oldValue         = oldValue;
			this.newValue         = newValue;
		}


		public AbstractEntity					Entity
		{
			get
			{
				return this.entity;
			}
		}

		public Caption							FieldCaption
		{
			get
			{
				return EntityInfo.GetFieldCaption (this.lambdaExpression);
			}
		}

		public INamedType						FieldType
		{
			get
			{
				return EntityInfo.GetFieldType (this.lambdaExpression);
			}
		}

		public object							OldValue
		{
			get
			{
				return this.oldValue;
			}
		}

		public object							NewValue
		{
			get
			{
				return this.newValue;
			}
		}

		public LambdaExpression					Lambda
		{
			get
			{
				return this.lambdaExpression;
			}
		}


		private readonly AbstractEntity			entity;
		private readonly LambdaExpression		lambdaExpression;
		private readonly object					oldValue;
		private readonly object					newValue;
	}
}
