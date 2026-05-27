namespace Blog.Application.DTOs;

public record AuthorDto(
    int Id,
    string Name,
    string Email,
    string Bio,
    DateTime CreatedAt);

public record CreateAuthorDto(
    string Name,
    string Email,
    string Bio);

public record UpdateAuthorDto(
    string Name,
    string Email,
    string Bio);