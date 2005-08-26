//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe SuppressBundleSupportAttribute définit un attribut
	/// [SuppressBundleSupport] qui signale que la classe ne doit pas
	/// être considérée comme valable du point de vue de son interface
	/// IBundleSupport. Cela n'a aucune incidence sur les classes
	/// dérivées.
	/// </summary>
	
	[System.Serializable]
	[System.AttributeUsage (System.AttributeTargets.Class)]
	
	public class SuppressBundleSupportAttribute : System.Attribute
	{
		public SuppressBundleSupportAttribute()
		{
		}
	}
}
