//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 08/10/2003

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'interface IBundleSupport définit les méthodes nécessaires
	/// pour gérer l'initialisation d'objets basée sur des bundles.
	/// </summary>
	public interface IBundleSupport : System.IDisposable
	{
		string			PublicClassName		{ get; }
		
		void RestoreFromBundle(Epsitec.Common.Support.ObjectBundler bundler, Epsitec.Common.Support.ResourceBundle bundle);
	}
}
