//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

		public IdentityCard(IdentityCard card)
		{
			this.MergeWithCard (card);
		}

		
		public int								DeveloperId
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

		public string							UserName
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

		public byte[]							RawImage
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

		public void MergeWithCard(IdentityCard card)
		{
			if (!string.IsNullOrEmpty (card.UserName))
			{
				this.UserName    = card.UserName;
			}

			if (card.DeveloperId != -1)
			{
				this.DeveloperId = card.DeveloperId;
			}

			if (card.RawImage != null)
			{
				this.RawImage = (byte[]) card.RawImage.Clone ();
			}
		}

		
		internal void Attach(IdentityRepository repository)
		{
			if (this.repository != repository)
			{
				System.Diagnostics.Debug.Assert ((this.repository == null) && (repository != null));
				this.repository = repository;
			}
		}

		internal void Detach(IdentityRepository repository)
		{
			System.Diagnostics.Debug.Assert (this.repository == repository);
			this.repository = null;
		}

		
		private static void NotifyRawImageChanged(DependencyObject obj, object oldValue, object newValue)
		{
			IdentityCard that = (IdentityCard) obj;
			that.cachedImage = null;
		}

		
		public static readonly DependencyProperty DeveloperIdProperty = DependencyProperty<IdentityCard>.Register (x => x.DeveloperId, new DependencyPropertyMetadata (-1));
		public static readonly DependencyProperty RawImageProperty    = DependencyProperty<IdentityCard>.Register (x => x.RawImage, new DependencyPropertyMetadata (IdentityCard.NotifyRawImageChanged));
		public static readonly DependencyProperty UserNameProperty    = DependencyProperty<IdentityCard>.Register (x => x.UserName, new DependencyPropertyMetadata ());

		private Drawing.Image cachedImage;
		private IdentityRepository repository;
	}
}
