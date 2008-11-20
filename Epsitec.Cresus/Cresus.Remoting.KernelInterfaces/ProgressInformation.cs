//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// 
	/// </summary>
	[System.Serializable]
	public struct ProgressInformation
	{
		public ProgressInformation(int progressPercent, ProgressStatus progressStatus, int currentStep, int lastStep, System.TimeSpan runningDuration, System.TimeSpan expectedDuration, long operationId)
		{
			this.progressPercent = progressPercent;
			this.progressStatus = progressStatus;
			this.currentStep = currentStep;
			this.lastStep = lastStep;
			this.runningDuration = runningDuration;
			this.expectedDuration = expectedDuration;
			this.operationId = operationId;
		}

		private ProgressInformation(ImmediateResult flag)
		{
			this.progressPercent = 100;
			this.progressStatus = ProgressStatus.Succeeded;
			this.currentStep = 0;
			this.lastStep = 0;
			this.runningDuration = System.TimeSpan.Zero;
			this.expectedDuration = System.TimeSpan.Zero;
			this.operationId = 0;
		}

		public int ProgressPercent
		{
			get
			{
				return this.progressPercent;
			}
		}	//	avancement 0..100, -1 = ind�termin�

		public ProgressStatus ProgressStatus
		{
			get
			{
				return this.progressStatus;
			}
		}	//	�tat (en cours d'ex�cution, termin�, annul�, �chou�)
		public int CurrentStep
		{
			get
			{
				return this.currentStep;
			}
		}	//	# de l'�tape en cours (0..n)
		public int LastStep
		{
			get
			{
				return this.lastStep;
			}
		}	//	# de la derni�re �tape, -1 = ind�termin�

		public System.TimeSpan RunningDuration
		{
			get
			{
				return this.runningDuration;
			}
		}	//	dur�e d'ex�cution actuelle

		public System.TimeSpan ExpectedDuration
		{
			get
			{
				return this.expectedDuration;
			}
		}	//	dur�e totale estim�e, ind�termin�e si < 0

		public long OperationId
		{
			get
			{
				return this.operationId;
			}
		}


		private enum ImmediateResult
		{
			Flag
		};

		public static readonly ProgressInformation Empty = new ProgressInformation ();
		public static readonly ProgressInformation Immediate = new ProgressInformation (ImmediateResult.Flag);

		readonly int progressPercent;
		readonly ProgressStatus progressStatus;
		readonly int currentStep;
		readonly int lastStep;
		readonly System.TimeSpan runningDuration;
		readonly System.TimeSpan expectedDuration;
		readonly long operationId;
	}
}
