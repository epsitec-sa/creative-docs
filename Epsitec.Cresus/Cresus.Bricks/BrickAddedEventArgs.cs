//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Cresus.Bricks
{
	public sealed class BrickAddedEventArgs : EventArgs
	{
		public BrickAddedEventArgs(System.Type fieldType, Brick brick)
		{
			this.fieldType = fieldType;
			this.brick = brick;
		}

		public System.Type FieldType
		{
			get
			{
				return this.fieldType;
			}
		}

		public Brick Brick
		{
			get
			{
				return this.brick;
			}
		}

		private readonly System.Type fieldType;
		private readonly Brick brick;
	}
}
