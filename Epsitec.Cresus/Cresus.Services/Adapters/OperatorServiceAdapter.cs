//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.ServiceModel;

namespace Epsitec.Cresus.Services.Adapters
{
	[ServiceBehavior (InstanceContextMode=InstanceContextMode.Single, IncludeExceptionDetailInFaults=true)]
	class OperatorServiceAdapter : AbstractServiceAdapter, IOperatorService
	{
		public OperatorServiceAdapter(IOperatorService target)
		{
			this.target = target;
		}

		private readonly IOperatorService target;

		#region IOperatorService Members

		public void CreateRoamingClient(string client_name, out IOperation operation)
		{
			IOperation op;
			this.target.CreateRoamingClient (client_name, out op);
			operation = new OperationWrapper (op);
		}

		public void GetRoamingClientData(IOperation operation, out ClientIdentity client, out byte[] compressed_data)
		{
			OperationWrapper wr = (OperationWrapper) operation;
			this.target.GetRoamingClientData (wr.Target, out client, out compressed_data);
		}

		#endregion

		#region IRemotingService Members

		public System.Guid GetServiceId()
		{
			throw new System.NotImplementedException ();
		}

		public string GetServiceName()
		{
			throw new System.NotImplementedException ();
		}

		#endregion
	}

	class OperationWrapper : System.MarshalByRefObject, IOperation
	{
		public OperationWrapper(IOperation target)
		{
			this.target = target;
		}
		
		~OperationWrapper()
		{
			System.Diagnostics.Debug.WriteLine ("OperationWrapper destroyed");
		}

		public IOperation Target
		{
			get
			{
				return this.target;
			}
		}

		readonly IOperation target;

		#region IOperation Members

		public void CancelOperation()
		{
			this.target.CancelOperation ();
		}

		public void CancelOperation(out IProgressInformation progress_information)
		{
			this.target.CancelOperation (out progress_information);
		}

		#endregion

		#region IProgressInformation Members

		public int ProgressPercent
		{
			get
			{
				return this.target.ProgressPercent;
			}
		}

		public ProgressStatus ProgressStatus
		{
			get
			{
				return this.target.ProgressStatus;
			}
		}

		public int CurrentStep
		{
			get
			{
				return this.target.CurrentStep;
			}
		}

		public int LastStep
		{
			get
			{
				return this.target.LastStep;
			}
		}

		public System.TimeSpan RunningDuration
		{
			get
			{
				return this.target.RunningDuration;
			}
		}

		public System.TimeSpan ExpectedDuration
		{
			get
			{
				return this.target.ExpectedDuration;
			}
		}

		public bool WaitForProgress(int minimum_progress, System.TimeSpan timeout)
		{
			return this.target.WaitForProgress (minimum_progress, timeout);
		}

		#endregion
	}
}
