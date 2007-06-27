//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Identity;
using Epsitec.Common.Types;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (IdentityCard))]

namespace Epsitec.Common.Identity
{
	public class IdentityCard : DependencyObject
	{
		public IdentityCard()
		{
		}

		public int DeveloperId
		{
			get
			{
				return (int) this.GetValue (IdentityCard.DeveloperIdProperty);
			}
			set
			{
				this.SetValue (IdentityCard.DeveloperIdProperty, value);
			}
		}

		public byte[] RawImage
		{
			get
			{
				object value = this.GetValue (IdentityCard.RawImageProperty);

				if (UndefinedValue.IsUndefinedValue (value))
				{
					return null;
				}
				else
				{
					return (byte[]) value;
				}
			}
			set
			{
				this.SetValue (IdentityCard.RawImageProperty, value);
			}
		}


		public static readonly DependencyProperty DeveloperIdProperty = DependencyProperty.Register ("DeveloperId", typeof (int), typeof (IdentityCard), new DependencyPropertyMetadata (-1));
		public static readonly DependencyProperty RawImageProperty = DependencyProperty.Register ("RawImage", typeof (byte[]), typeof (IdentityCard), new DependencyPropertyMetadata ());
	}
}
