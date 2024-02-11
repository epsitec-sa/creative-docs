//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>BusinessRuleException</c> class is intended to be thrown from BusinessRules when some
	/// business condition is violated. The user interface is then supposed to catch such exceptions
	/// and display them to the user.
	/// </summary>
	public sealed class BusinessRuleException : System.Exception
	{
		public BusinessRuleException(string message)
			: this (null, message)
		{
		}

		public BusinessRuleException(AbstractEntity entity, string message)
			: base (message)
		{
			this.entity = entity;
			this.formattedMessage = FormattedText.FromSimpleText (message);
		}

		public BusinessRuleException(AbstractEntity entity, FormattedText formattedMessage)
			: base (formattedMessage.ToSimpleText ())
		{
			this.entity = entity;
			this.formattedMessage = formattedMessage;
		}

		
		public AbstractEntity					Entity
		{
			get
			{
				return this.entity;
			}
		}

		public FormattedText					FormattedMessage
		{
			get
			{
				return this.formattedMessage;
			}
		}
	
		
		private readonly AbstractEntity			entity;
		private readonly FormattedText			formattedMessage;
	}
}
