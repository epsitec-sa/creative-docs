using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervGroupDefinition : Freezable
	{


		public EervGroupDefinition(string id, string name)
		{
			this.Id = id;
			this.Name = name;

			this.children = new List<EervGroupDefinition> ();
		}

		public EervGroupDefinition Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.ThrowIfReadOnly ();
				
				this.parent = value;
			}
		}


		public IList<EervGroupDefinition> Children
		{
			get
			{
				return this.children;
			}
		}


		protected override void HandleFreeze()
		{
			base.HandleFreeze ();

			this.children = this.children.AsReadOnlyCollection ();
		}


		public readonly string Id;
		public readonly string Name;


		private EervGroupDefinition parent;
		private IList<EervGroupDefinition> children;


	}


}
