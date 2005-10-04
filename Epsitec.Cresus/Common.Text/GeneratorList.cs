//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe GeneratorList gère la liste des générateurs, accessibles par
	/// leur nom.
	/// </summary>
	public sealed class GeneratorList
	{
		public GeneratorList(Text.TextContext context)
		{
			this.context    = context;
			this.generators = new System.Collections.Hashtable ();
		}
		
		
		public Generator						this[string name]
		{
			get
			{
				return this.generators[name] as Generator;
			}
		}
		
		public Generator						this[Properties.GeneratorProperty property]
		{
			get
			{
				return property == null ? null : this[property.Generator];
			}
		}
		
		
		public Generator NewGenerator(string name)
		{
			System.Diagnostics.Debug.Assert (this.generators.Contains (name) == false);
			
			Generator generator = new Generator (name);
			
			this.generators[name] = generator;
			
			return generator;
		}
		
		public void DisposeGenerator(Generator generator)
		{
			System.Diagnostics.Debug.Assert (this.generators.Contains (generator.Name));
			
			this.generators.Remove (generator.Name);
		}
		
		
		
		private Text.TextContext					context;
		private System.Collections.Hashtable	generators;
	}
}
