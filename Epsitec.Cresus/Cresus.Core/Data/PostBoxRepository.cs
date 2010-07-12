using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class PostBoxRepository : Repository
	{


		public PostBoxRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<PostBoxEntity> GetPostBoxesByExample(PostBoxEntity example)
		{
			return this.GetEntitiesByExample<PostBoxEntity> (example);
		}


		public IEnumerable<PostBoxEntity> GetPostBoxesByRequest(Request request)
		{
			return this.GetEntitiesByRequest<PostBoxEntity> (request);
		}


		public IEnumerable<PostBoxEntity> GetPostBoxesByExample(PostBoxEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<PostBoxEntity> (example, index, count);
		}


		public IEnumerable<PostBoxEntity> GetPostBoxesByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<PostBoxEntity> (request, index, count);
		}


		public IEnumerable<PostBoxEntity> GetAllPostBoxes()
		{
			PostBoxEntity example = this.CreatePostBoxExample ();

			return this.GetPostBoxesByExample (example);
		}


		public IEnumerable<PostBoxEntity> GetAllPostBoxes(int index, int count)
		{
			PostBoxEntity example = this.CreatePostBoxExample ();

			return this.GetPostBoxesByExample (example, index, count);
		}


		public IEnumerable<PostBoxEntity> GetPostBoxesByNumber(string number)
		{
			PostBoxEntity example = this.CreatePostBoxExampleByNumber (number);

			return this.GetPostBoxesByExample (example);
		}


		public IEnumerable<PostBoxEntity> GetPostBoxesByNumber(string number, int index, int count)
		{
			PostBoxEntity example = this.CreatePostBoxExampleByNumber (number);

			return this.GetPostBoxesByExample (example, index, count);
		}


		public PostBoxEntity CreatePostBoxExample()
		{
			return this.CreateExample<PostBoxEntity> ();
		}


		private PostBoxEntity CreatePostBoxExampleByNumber(string number)
		{
			PostBoxEntity example = this.CreatePostBoxExample ();
			example.Number = number;

			return example;
		}

            
	}


}
