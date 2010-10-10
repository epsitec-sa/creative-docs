//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner.Objects
{
	public enum RouteType
	{
		// Cas A:
		// o--------->
		//
		// Cas A personnalisé:
		//    x---x
		//    |   |
		// o--|   |-->
		//
		// 
		// Cas Bt:
		//       ^
		//       |
		// o-----|
		// 
		// Cas Bt personnalisé:
		//       ^
		//       |
		//    x--|
		//    |
		// o--|
		// 
		// Cas Bb:
		// o-----|
		//       |
		//       V
		// 
		// Cas Bb personnalisé:
		// o--|
		//    |
		//    x--|
		//       |
		//       V
		// 
		// Cas C:
		// o----|
		//      x
		//      |---->
		// 
		// Cas D:
		// o----|
		//      x
		//   <--|
		// 
		// Les cas A et B ont un routage automatique ou personnalisé.
		// 'x' = poignée pour personnaliser le routage.

		Close,		// connexion fermée
		Himself,	// connexion sur soi-même
		A,			// connexion de type A
		Bt,			// connexion de type B vers le haut
		Bb,			// connexion de type B vers le bas
		C,			// connexion de type C
		D,			// connexion de type D
	}
}
