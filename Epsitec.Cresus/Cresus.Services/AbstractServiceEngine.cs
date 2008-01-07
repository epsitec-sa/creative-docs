//	Copyright � 2004-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe AbstractService sert de base � toutes les classes impl�mentant des
	/// services dans Cr�sus R�seau.
	/// </summary>
	public abstract class AbstractServiceEngine : System.MarshalByRefObject, System.IDisposable
	{
		protected AbstractServiceEngine(Engine engine, string service_name)
		{
			this.engine = engine;
			this.service_name = service_name;
		}
		
		
		public string							ServiceName
		{
			get
			{
				return this.service_name;
			}
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		public override object InitializeLifetimeService()
		{
			//	En retournant null ici, on garantit que le service ne sera jamais
			//	recycl� (sinon, apr�s un temps d�fini par ILease, l'objet est retir�
			//	de la table des objets joignables par "remoting").
			
			return null;
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
		}
		
		protected void ThrowExceptionBasedOnStatus(Remoting.ProgressStatus status)
		{
			switch (status)
			{
				case Remoting.ProgressStatus.None:		throw new Remoting.Exceptions.InvalidOperationException ();
				case Remoting.ProgressStatus.Running:	throw new Remoting.Exceptions.PendingException ();
				case Remoting.ProgressStatus.Cancelled:	throw new Remoting.Exceptions.CancelledException ();
				case Remoting.ProgressStatus.Failed:	throw new Remoting.Exceptions.FailedException ();
				
				case Remoting.ProgressStatus.Succeeded:
					break;
				
				default:
					throw new System.ArgumentOutOfRangeException ("status", status, "Unsupported status value.");
			}
		}
		
		
		protected Engine						engine;
		private string							service_name;
	}
}
