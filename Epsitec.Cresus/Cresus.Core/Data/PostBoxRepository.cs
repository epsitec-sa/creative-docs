using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

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


		public PostBoxEntity GetPostBoxByExample(PostBoxEntity example)
		{
			return this.GetEntityByExample<PostBoxEntity> (example);
		}


		public IEnumerable<PostBoxEntity> GetAllPostBoxes()
		{
			PostBoxEntity example = this.CreatePostBoxExample ();

			return this.GetPostBoxesByExample (example);
		}


		public IEnumerable<PostBoxEntity> GetPostBoxesByNumber(string number)
		{
			PostBoxEntity example = this.CreatePostBoxExampleByNumber (number);

			return this.GetPostBoxesByExample (example);
		}


		public PostBoxEntity GetPostBoxByNumber(string number)
		{
			PostBoxEntity example = this.CreatePostBoxExampleByNumber (number);

			return this.GetPostBoxByExample (example);
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
