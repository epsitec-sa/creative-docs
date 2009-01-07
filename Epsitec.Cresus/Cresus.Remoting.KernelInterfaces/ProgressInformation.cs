//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>ProgressInformation</c> structure stores the progress information
	/// about an operation, including the operation ID of the running operation.
	/// </summary>
	[System.Serializable]
	public struct ProgressInformation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProgressInformation"/> structure.
		/// </summary>
		/// <param name="progressPercent">The progress percent.</param>
		/// <param name="progressState">The progress state.</param>
		/// <param name="currentStep">The current step.</param>
		/// <param name="expectedLastStep">The expected last step.</param>
		/// <param name="runningDuration">The running duration.</param>
		/// <param name="expectedDuration">The expected duration.</param>
		/// <param name="operationId">The operation id.</param>
		public ProgressInformation(int progressPercent, ProgressState progressState, int currentStep, int expectedLastStep, System.TimeSpan runningDuration, System.TimeSpan expectedDuration, long operationId)
		{
			this.progressPercent = progressPercent;
			this.progressState = progressState;
			this.currentStep = currentStep;
			this.expectedLastStep = expectedLastStep;
			this.runningDuration = runningDuration;
			this.expectedDuration = expectedDuration;
			this.operationId = operationId;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProgressInformation"/> structure.
		/// The instance is used only for the <c>Immediate</c> value.
		/// </summary>
		/// <param name="flag">Dummy.</param>
		private ProgressInformation(DummyImmediateResult flag)
		{
			this.progressPercent = 100;
			this.progressState = ProgressState.Succeeded;
			this.currentStep = 0;
			this.expectedLastStep = 0;
			this.runningDuration = System.TimeSpan.Zero;
			this.expectedDuration = System.TimeSpan.Zero;
			this.operationId = 0;
		}


		/// <summary>
		/// Gets the progress in percent (0...100; -1 means undefined).
		/// </summary>
		/// <value>The progress in percent.</value>
		public int								ProgressPercent
		{
			get
			{
				return this.progressPercent;
			}
		}

		/// <summary>
		/// Gets the progress state.
		/// </summary>
		/// <value>The progress state.</value>
		public ProgressState					ProgressState
		{
			get
			{
				return this.progressState;
			}
		}

		/// <summary>
		/// Gets the index of the current step (0...n).
		/// </summary>
		/// <value>The current step.</value>
		public int								CurrentStep
		{
			get
			{
				return this.currentStep;
			}
		}

		/// <summary>
		/// Gets the index of the expected last step (-1 means undefined).
		/// </summary>
		/// <value>The last step.</value>
		public int								ExpectedLastStep
		{
			get
			{
				return this.expectedLastStep;
			}
		}

		/// <summary>
		/// Gets the total running duration.
		/// </summary>
		/// <value>The total running duration.</value>
		public System.TimeSpan					RunningDuration
		{
			get
			{
				return this.runningDuration;
			}
		}

		/// <summary>
		/// Gets the expected duration (if negative, means undefined).
		/// </summary>
		/// <value>The expected duration.</value>
		public System.TimeSpan					ExpectedDuration
		{
			get
			{
				return this.expectedDuration;
			}
		}

		/// <summary>
		/// Gets the associated operation id.
		/// </summary>
		/// <value>The operation id.</value>
		public long								OperationId
		{
			get
			{
				return this.operationId;
			}
		}

		#region Dummy enum for specific Immediate constructor

		private enum DummyImmediateResult
		{
			Flag
		};

		#endregion

		public static readonly ProgressInformation Empty = new ProgressInformation ();
		public static readonly ProgressInformation Immediate = new ProgressInformation (DummyImmediateResult.Flag);

		readonly int							progressPercent;
		readonly ProgressState					progressState;
		readonly int							currentStep;
		readonly int							expectedLastStep;
		readonly System.TimeSpan				runningDuration;
		readonly System.TimeSpan				expectedDuration;
		readonly long							operationId;
	}
}
