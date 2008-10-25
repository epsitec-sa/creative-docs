//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe AbstractService sert de base à toutes les classes implémentant des
	/// services dans Crésus Réseau.
	/// </summary>
	public abstract class AbstractServiceEngine : System.MarshalByRefObject, System.IDisposable, IRemoteService
	{
		protected AbstractServiceEngine(Engine engine)
		{
			this.engine = engine;
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
			//	recyclé (sinon, après un temps défini par ILease, l'objet est retiré
			//	de la table des objets joignables par "remoting").
			
			return null;
		}

		#region IRemotingService Members

		public abstract System.Guid GetServiceId();

		public string GetServiceName()
		{
			return this.GetType ().FullName;
		}

		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
		}
		
		
		protected readonly Engine				engine;
	}
}
