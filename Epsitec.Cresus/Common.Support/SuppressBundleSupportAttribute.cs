//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using System;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe SuppressBundleSupportAttribute d�finit un attribut
	/// [SuppressBundleSupport] qui signale que la classe ne doit pas
	/// �tre consid�r�e comme valable du point de vue de son interface
	/// IBundleSupport. Cela n'a aucune incidence sur les classes
	/// d�riv�es.
	/// </summary>
	
	[AttributeUsage (AttributeTargets.Class)]
	
	public class SuppressBundleSupportAttribute : System.Attribute
	{
		public SuppressBundleSupportAttribute()
		{
		}
	}
}
