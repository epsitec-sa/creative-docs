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

		
		
		protected Engine						engine;
		private string							service_name;
	}
}
