using AutoMapper;
using Blog.Model;

namespace Blog.API.Profiles
{
	public class CommentProfile : Profile
	{
		public CommentProfile()
		{
			_ = CreateMap<CommentForCreation, Entities.Comment>()
				.ForMember(x => x.Id, x => x.Ignore())
				.ForMember(x => x.Post, x => x.Ignore())
				.ForMember(x => x.UserId, x => x.Ignore())
				.ForMember(x => x.User, x => x.Ignore())
				.ForMember(x => x.DateOfPublish, x => x.Ignore());
			_ = CreateMap<Entities.Comment, Comment>();
		}
	}
}
