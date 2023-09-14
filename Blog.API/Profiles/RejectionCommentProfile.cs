using AutoMapper;
using Blog.Model;

namespace Blog.API.Profiles
{
    public class RejectionCommentProfile : Profile
    {
        public RejectionCommentProfile()
        {
            _ = CreateMap<PostRejection, Entities.RejectionComment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Content, x => x.MapFrom(x => x.RejectionComment))
                .ForMember(x => x.Post, x => x.Ignore())
                .ForMember(x => x.PostId, x => x.Ignore())
                .ForMember(x => x.UserId, x => x.Ignore())
                .ForMember(x => x.User, x => x.Ignore())
                .ForMember(x => x.DateOfPublish, x => x.Ignore());
            _ = CreateMap<Entities.RejectionComment, RejectionComment>();
        }
    }
}
