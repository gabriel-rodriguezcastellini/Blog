using AutoMapper;
using Blog.API.Entities;

namespace Blog.API.Profiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            _ = CreateMap<Post, Model.Post>();
            _ = CreateMap<Model.PostForUpdate, Post>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.DateOfPublish, x => x.Ignore())
                .ForMember(x => x.AuthorId, x => x.Ignore())
                .ForMember(x => x.AuthorId, x => x.Ignore())
                .ForMember(x => x.Status, x => x.Ignore())
                .ForMember(x => x.Comments, x => x.Ignore());
            _ = CreateMap<Model.PostForCreation, Post>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.DateOfPublish, x => x.Ignore())
                .ForMember(x => x.AuthorId, x => x.Ignore())
                .ForMember(x => x.Author, x => x.Ignore())
                .ForMember(x => x.Status, x => x.Ignore())
                .ForMember(x => x.Comments, x => x.Ignore());
        }
    }
}
