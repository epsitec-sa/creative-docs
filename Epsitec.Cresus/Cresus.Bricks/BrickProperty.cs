//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	public struct BrickProperty
	{
		public BrickProperty(BrickPropertyKey key)
			: this (key, "")
		{
		}

		public BrickProperty(BrickPropertyKey key, string value)
		{
			this.key = key;
			this.value = value;
		}

		public BrickProperty(BrickPropertyKey key, int value)
		{
			this.key = key;
			this.value = value;
		}

		public BrickProperty(BrickPropertyKey key, Brick value)
		{
			this.key = key;
			this.value = value;
		}

		public BrickPropertyKey					Key
		{
			get
			{
				return this.key;
			}
		}
		
		public Brick							Brick
		{
			get
			{
				return this.value as Brick;
			}
		}

		public override string ToString()
		{
			return string.Format ("{0} = {1}", this.key, this.value ?? "<null>");
		}

		private readonly BrickPropertyKey key;
		private readonly object value;
	}
}
