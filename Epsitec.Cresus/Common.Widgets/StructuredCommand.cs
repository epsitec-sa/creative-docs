//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Widgets.Collections;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	public class StructuredCommand : Command, IStructuredTree
	{
		public StructuredCommand(string name) : base (name)
		{
			this.StateObjectType = Types.DependencyObjectType.FromSystemType (typeof (StructuredState));
		}

		#region IStructuredTree Members

		string[] IStructuredTree.GetFieldNames()
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		string[] IStructuredTree.GetFieldPaths(string path)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		#endregion

		private class StructuredState : CommandState
		{
			public StructuredState()
			{
			}
		}
	}
}
