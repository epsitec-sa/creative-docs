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

		public IdentityCard(string userName, int developerId, byte[] rawImage)
		{
			this.UserName    = userName;
			this.DeveloperId = developerId;
			this.RawImage    = rawImage;
		}

		public int DeveloperId
		{
			get
			{
				return (int) this.GetValue (IdentityCard.DeveloperIdProperty);
			}
			set
			{
				if (value == -1)
				{
					this.ClearValue (IdentityCard.DeveloperIdProperty);
				}
				else
				{
					this.SetValue (IdentityCard.DeveloperIdProperty, value);
				}
			}
		}

		public string UserName
		{
			get
			{
				object value = this.GetValue (IdentityCard.UserNameProperty);

				if (UndefinedValue.IsUndefinedValue (value))
				{
					return null;
				}
				else
				{
					return ((string) value) ?? "";
				}
			}
			set
			{
				if (value == null)
				{
					this.ClearValue (IdentityCard.UserNameProperty);
				}
				else
				{
					this.SetValue (IdentityCard.UserNameProperty, value);
				}
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
				if (value == null)
				{
					this.ClearValue (IdentityCard.RawImageProperty);
				}
				else
				{
					this.SetValue (IdentityCard.RawImageProperty, value);
				}
			}
		}

		public Drawing.Image GetImage()
		{
			if (this.cachedImage == null)
			{
				byte[] data = this.RawImage;

				if ((data != null) &&
					(data.Length > 0))
				{
					this.cachedImage = Drawing.Bitmap.FromData (data);
				}
			}

			return this.cachedImage;
		}

		private static void NotifyRawImageChanged(DependencyObject obj, object oldValue, object newValue)
		{
			IdentityCard that = (IdentityCard) obj;
			that.cachedImage = null;
		}

		public static readonly DependencyProperty DeveloperIdProperty = DependencyProperty.Register ("DeveloperId", typeof (int), typeof (IdentityCard), new DependencyPropertyMetadata (-1));
		public static readonly DependencyProperty RawImageProperty = DependencyProperty.Register ("RawImage", typeof (byte[]), typeof (IdentityCard), new DependencyPropertyMetadata (IdentityCard.NotifyRawImageChanged));
		public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register ("UserName", typeof (string), typeof (IdentityCard), new DependencyPropertyMetadata ());

		private Drawing.Image cachedImage;
	}
}
