//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'interface IBundleSupport définit les méthodes nécessaires
	/// pour gérer l'initialisation d'objets basée sur des bundles.
	/// </summary>
	public interface IBundleSupport : System.IDisposable
	{
		string			PublicClassName		{ get; }
		string			BundleName			{ get; }
		
		void RestoreFromBundle(ObjectBundler bundler, ResourceBundle bundle);
		void SerializeToBundle(ObjectBundler bundler, ResourceBundle bundle);
	}
}
