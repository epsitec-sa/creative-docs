using Epsitec.Common.Support.EntityEngine;

using System;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>BusinessRuleException</c> class is intended to be thrown from BusinessRules when some
	/// business condition is violated. The user interface is then supposed to catch such exceptions
	/// and display them to the user.
	/// </summary>
	public sealed class BusinessRuleException : Exception
	{
		public BusinessRuleException(AbstractEntity entity, string message)
			: base (message)
		{
			this.entity = entity;
		}

		public AbstractEntity Entity
		{
			get
			{
				return this.entity;
			}
		}

		private readonly AbstractEntity entity;
	}
}
