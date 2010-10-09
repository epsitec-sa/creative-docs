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

		Close,		// connection fermée
		Himself,	// connection sur soi-même
		A,			// connection de type A
		Bt,			// connection de type B vers le haut
		Bb,			// connection de type B vers le bas
		C,			// connection de type C
		D,			// connection de type D
	}
}
