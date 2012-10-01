//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.Eerv
{
	public sealed class PathPrefixReplacement
	{
		public PathPrefixReplacement(string name, string template, string output = null, int level = 0)
		{
			this.name = name;
			this.template = template;
			this.output = output;
			this.level = level;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string							Template
		{
			get
			{
				return this.template;
			}
		}
		
		public string							Output
		{
			get
			{
				return this.output ?? this.template;
			}
		}

		public int Level
		{
			get
			{
				return this.level;
			}
		}
		
		public override string ToString()
		{
			return string.Format ("{0}: {1} > {2}", this.Level, this.Template, this.Output);
		}

		public string Map(Entities.AiderGroupDefEntity groupDef)
		{
			System.Diagnostics.Debug.Assert (groupDef.PathTemplate.StartsWith (this.Template));

			var path = this.Output + groupDef.PathTemplate.Substring (this.Template.Length);

			return path;
		}


		private readonly string					name;
		private readonly string					template;
		private readonly string					output;
		private readonly int					level;
	}
}
