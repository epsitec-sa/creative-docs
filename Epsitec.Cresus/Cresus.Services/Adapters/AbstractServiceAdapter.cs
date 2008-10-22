namespace Epsitec.Cresus.Services.Adapters
{
	class AbstractServiceAdapter : System.MarshalByRefObject
	{
		public override object InitializeLifetimeService()
		{
			//	En retournant null ici, on garantit que le service ne sera jamais
			//	recyclé (sinon, après un temps défini par ILease, l'objet est retiré
			//	de la table des objets joignables par "remoting").

			return null;
		}
	}
}
