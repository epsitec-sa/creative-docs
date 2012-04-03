using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System;

using System.Collections.Generic;


namespace Epsitec.Aider.Data.Eerv
{
	
	
	internal sealed class EervHousehold : Freezable
	{


		public EervHousehold(string id, EervAddress address, EervCoordinates coordinates, string remarks)
		{
			this.Id = id;
			this.Address = address;
			this.Coordinates = coordinates;
			this.Remarks = remarks;

			this.children = new List<EervPerson> ();
		}


		public EervPerson Head1
		{
			get
			{
				return this.head1;
			}
			set
			{
				this.ThrowIfReadOnly ();

				this.head1 = value;
			}
		}


		public EervPerson Head2
		{
			get
			{
				return this.head2;
			}
			set
			{
				this.ThrowIfReadOnly ();

				this.head2 = value;
			}
		}


		public IEnumerable<EervPerson> Heads
		{
			get
			{
				if (this.head1 != null)
				{
					yield return this.head1;
				}

				if (this.head2 != null)
				{
					yield return this.head2;
				}
			}
		}


		public IList<EervPerson> Children
		{
			get
			{
				return this.children;
			}
		}


		public IEnumerable<EervPerson> Members
		{
			get
			{
				foreach (var head in this.Heads)
				{
					yield return head;
				}

				foreach (var child in this.Children)
				{
					yield return child;
				}
			}
		}


		protected override void HandleFreeze()
		{
			base.HandleFreeze ();

			this.children = this.children.AsReadOnlyCollection ();
		}


		public readonly string Id;
		public readonly EervAddress Address;
		public readonly EervCoordinates Coordinates;
		public readonly string Remarks;


		private EervPerson head1;
		private EervPerson head2;	
		private IList<EervPerson> children;
	
	
	}


}