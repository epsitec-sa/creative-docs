//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe AbstractService sert de base à toutes les classes implémentant des
	/// services dans Crésus Réseau.
	/// </summary>
	public abstract class AbstractServiceEngine : System.MarshalByRefObject
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
		
		
		public override object InitializeLifetimeService()
		{
			//	En retournant null ici, on garantit que le service ne sera jamais
			//	recyclé (sinon, après un temps défini par ILease, l'objet est retiré
			//	de la table des objets joignables par "remoting").
			
			return null;
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
