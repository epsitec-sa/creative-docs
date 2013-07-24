namespace Epsitec.Cresus.WebCore.Server.Core
{


	/// <summary>
	/// This class is used to keep a bidirectional mapping between instances of a type and auto
	/// generated ids. For each instance of T that you give it, it maintains the two mappings:
	/// - t => id
	/// - id => t
	/// This allows references to some complex object to be given to the client as a small id that
	/// can then be resolved back to the same complex object, when the client gives bach the
	/// reference to the server.
	/// </summary>
	internal sealed class IdCache<T> : ItemCache<T, T, string, string, T>
	{


		public string GetId(T item)
		{
			return this.Get1 (item);
		}


		public T GetItem(string id)
		{
			return this.Get2 (id);
		}


		protected override T GetKey1(T itemIn1)
		{
			return itemIn1;
		}


		protected override string GetItemOut1(T itemIn1)
		{
			return this.GetCurrentId ();
		}


		protected override string GetItemIn2(T itemIn1, string itemOut1)
		{
			return itemOut1;
		}


		protected override T GetItemOut2(T itemIn1, string itemOut1, string itemIn2)
		{
			return itemIn1;
		}


	}


}
