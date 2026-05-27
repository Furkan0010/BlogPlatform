using AutoMapper;
using Blog.Application.DTOs;
using Blog.Domain.Entities;

namespace Blog.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Author
        CreateMap<Author, AuthorDto>();
        CreateMap<CreateAuthorDto, Author>();
        CreateMap<UpdateAuthorDto, Author>();

        // Post — list (minimal)
        CreateMap<Post, PostListDto>()
            .ForCtorParam(nameof(PostListDto.AuthorName),
                opt => opt.MapFrom(src => src.Author.Name));

        // Post — detail (rich)
        CreateMap<Post, PostDetailDto>()
            .ForCtorParam(nameof(PostDetailDto.Tags),
                opt => opt.MapFrom(src => src.PostTags.Select(pt => pt.Tag.Name)));

        // Comment
        CreateMap<Comment, CommentDto>();
        CreateMap<CreateCommentDto, Comment>();
    }
}