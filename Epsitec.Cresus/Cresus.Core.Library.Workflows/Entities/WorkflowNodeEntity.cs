//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Workflows;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class WorkflowNodeEntity : IEntityInitializer
	{
		public void InitializeDefaultValues()
		{
			this.Code = (string) ItemCodeGenerator.NewCode ();
		}

		public bool IsAuto
		{
			get
			{
				return this.Attributes.HasFlag (WorkflowNodeAttributes.Auto);
			}
			set
			{
				if (value)
				{
					this.Attributes = this.Attributes.SetFlag (WorkflowNodeAttributes.Auto);
				}
				else
				{
					this.Attributes = this.Attributes.ClearFlag (WorkflowNodeAttributes.Auto);
				}
			}
		}

		public bool IsPublic
		{
			get
			{
				return this.Attributes.HasFlag (WorkflowNodeAttributes.Public);
			}
			set
			{
				if (value)
				{
					this.Attributes = this.Attributes.SetFlag (WorkflowNodeAttributes.Public);
				}
				else
				{
					this.Attributes = this.Attributes.ClearFlag (WorkflowNodeAttributes.Public);
				}
			}
		}

		public bool IsForeign
		{
			get
			{
				return this.Attributes.HasFlag (WorkflowNodeAttributes.Foreign);
			}
			set
			{
				if (value)
				{
					this.Attributes = this.Attributes.SetFlag (WorkflowNodeAttributes.Foreign);
				}
				else
				{
					this.Attributes = this.Attributes.ClearFlag (WorkflowNodeAttributes.Foreign);
				}
			}
		}
	}
}
