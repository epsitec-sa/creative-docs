//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Workflows
{
	/// <summary>
	/// The <c>WorkflowException</c> class is used to abort or cancel a
	/// running workflow. A reason can be provided, so that the user can
	/// be notified.
	/// </summary>
	[System.Serializable]

	public sealed class WorkflowException : System.Exception
	{
		public WorkflowException(WorkflowCancellation cancellation)
			: this (cancellation, FormattedText.Empty)
		{
		}

		public WorkflowException(WorkflowCancellation cancellation, FormattedText reason)
		{
			this.cancellation = cancellation;
			this.reason       = reason;
		}


		public WorkflowCancellation				Cancellation
		{
			get
			{
				return this.cancellation;
			}
		}

		public FormattedText					Reason
		{
			get
			{
				return this.reason;
			}
		}

		#region ISerializable Members

		private WorkflowException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base (info, context)
		{
			this.cancellation = (WorkflowCancellation) info.GetValue ("cancellation", typeof (WorkflowCancellation));
			this.reason       = (FormattedText) info.GetValue ("reason", typeof (FormattedText));
		}

		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue ("cancellation", this.cancellation);
			info.AddValue ("reason", this.reason);

			base.GetObjectData (info, context);
		}

		#endregion

		private readonly WorkflowCancellation	cancellation;
		private readonly FormattedText			reason;
	}
}