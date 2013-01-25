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
	[System.Serializable]
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

		
		#region Support for ISerializable
		
		public BusinessRuleException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base (info, context)
		{
			this.formattedMessage = (FormattedText) info.GetValue ("formattedMessage", typeof (FormattedText));

			//	We cannot restore the entity, as this is usually used when crossing remoting
			//	boundaries (or app domains, for that matter) and it is impossible to serialize
			//	an entity.
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue ("formattedMessage", this.formattedMessage);
			
			base.GetObjectData (info, context);
		}
		
		#endregion
		
		
		private readonly AbstractEntity			entity;
		private readonly FormattedText			formattedMessage;
	}
}
