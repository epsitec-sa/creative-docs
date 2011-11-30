//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>ActionAttribute</c> class defines a <c>[Action]</c> attribute,
	/// which is used by the <see cref="ActionDispatcher"/> to locate methods
	/// implementing publicly available actions.
	/// </summary>
	
	[System.Serializable]
	[System.AttributeUsage (System.AttributeTargets.Method, AllowMultiple = true)]
	
	public sealed class ActionAttribute : System.Attribute
	{
		public ActionAttribute(long captionId, double weight = 0.0)
			: this (ActionClasses.None, captionId, weight)
		{
		}

		public ActionAttribute(ActionClasses actionClass, long captionId, double weight = 0.0)
		{
			this.actionClass = actionClass;
			this.captionId   = captionId;
			this.weight      = weight;
		}

		
		public ActionClasses					ActionClass
		{
			get
			{
				return this.actionClass;
			}
		}

		public Druid							CaptionId
		{
			get
			{
				return this.captionId;
			}
		}

		public double							Weight
		{
			get
			{
				return this.weight;
			}
		}


		private readonly ActionClasses			actionClass;
		private readonly Druid					captionId;
		private readonly double					weight;
	}
}