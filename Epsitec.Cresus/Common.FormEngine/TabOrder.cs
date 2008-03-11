using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// Détermine l'ordre (pour TabIndex) d'un ensemble d'éléments décrits par leurs System.Guid.
	/// </summary>
	public sealed class TabOrder : System.IEquatable<TabOrder>
	{
		public TabOrder()
		{
			//	Constructeur.
			this.guids = new List<System.Guid>();
		}


		public void Clear()
		{
			this.guids.Clear();
		}

		public int Index(System.Guid guid)
		{
			//	Retourne l'index d'un élément, ou -1 s'il n'en a pas.
			return this.guids.IndexOf(guid);
		}

		public void Move(System.Guid guid, int direction)
		{
			//	Déplace un élément de n rangs vers l'avant (si direction est négatif) ou vers
			//	l'arrière (si direction est positif).
			int index = this.guids.IndexOf(guid);
			if (index != -1)
			{
				this.guids.RemoveAt(index);

				index += direction;
				index = System.Math.Max(index, 0);
				index = System.Math.Min(index, this.guids.Count);

				this.guids.Insert(index, guid);
			}
		}


		#region IEquatable<TabOrder> Members

		public bool Equals(TabOrder other)
		{
			return TabOrder.Equals(this, other);
		}

		#endregion

		public override bool Equals(object obj)
		{
			return (obj is TabOrder) && (TabOrder.Equals(this, (TabOrder) obj));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool Equals(TabOrder a, TabOrder b)
		{
			//	Retourne true si les deux objets sont égaux.
			if ((a == null) != (b == null))
			{
				return false;
			}

			if (a == null && b == null)
			{
				return true;
			}

			if (a.guids != null)
			{
				if (a.guids.Count != b.guids.Count)
				{
					return false;
				}

				for (int i=0; i<a.guids.Count; i++)
				{
					if (a.guids[i] != b.guids[i])
					{
						return false;
					}
				}
			}

			return true;
		}


		public override string ToString()
		{
			//	Retourne l'intance courante sous forme d'une string.
			//	Utilisé pour la sérialisation.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			for (int i=0; i<this.guids.Count; i++)
			{
				System.Guid guid = this.guids[i];
				builder.Append(guid.ToString());

				if (i < this.guids.Count-1)
				{
					builder.Append(";");
				}
			}

			return builder.ToString();
		}

		static public TabOrder Parse(string value)
		{
			//	Retourne une instance de TabOrder initialisée d'après une string.
			//	Utilisé pour la désérialisation.
			TabOrder tab = new TabOrder();

			string[] list = value.Split(';');
			foreach (string one in list)
			{
				tab.guids.Add(new System.Guid(one));
			}

			return tab;
		}


		private List<System.Guid>		guids;
	}
}
