namespace Epsitec.Common.Types
{


	public abstract class Freezable : IReadOnly
	{


		public Freezable()
		{
			this.IsReadOnly = false;
		}


		#region IReadOnly Members


		public bool IsReadOnly
		{
			get;
			private set;
		}


		#endregion


		public void Freeze()
		{
			this.IsReadOnly = true;

			this.HandleFreeze ();
		}


		protected virtual void HandleFreeze()
		{
		}


	}


}
