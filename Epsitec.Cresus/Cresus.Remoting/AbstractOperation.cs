//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// Summary description for AbstractOperation.
	/// </summary>
	public abstract class AbstractOperation : AbstractProgress, IOperation
	{
		public AbstractOperation()
		{
		}
		
		
		#region IOperation Members
		public void CancelOperation()
		{
			IProgressInformation progress;
			this.CancelOperation (out progress);
		}
		
		public virtual void CancelOperation(out IProgressInformation progress_information)
		{
			progress_information = new ImmediateProgress ();
		}
		#endregion
	}
}
