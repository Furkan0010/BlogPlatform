using Blog.Application.DTOs;
using Blog.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class PostsController : ControllerBase
{
    private readonly IPostService _service;
    private readonly ICommentService _commentService;

    public PostsController(IPostService service, ICommentService commentService)
    {
    _service = service;
    _commentService = commentService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PostListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PostListDto>>> GetAll(CancellationToken ct)
    {
        var posts = await _service.GetAllAsync(ct);
        return Ok(posts);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PostDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostDetailDto>> GetById(int id, CancellationToken ct)
    {
        var post = await _service.GetByIdAsync(id, ct);
        return post == null ? NotFound() : Ok(post);
    }

    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(PostDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostDetailDto>> GetBySlug(string slug, CancellationToken ct)
    {
        var post = await _service.GetBySlugAsync(slug, ct);
        return post == null ? NotFound() : Ok(post);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PostDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PostDetailDto>> Create([FromBody] CreatePostDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        if (!result.IsSuccess)
            return BadRequest(new { Errors = result.Errors });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePostDto dto, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, dto, ct);
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("not found")))
                return NotFound(new { Errors = result.Errors });
            return BadRequest(new { Errors = result.Errors });
        }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return result.IsSuccess ? NoContent() : NotFound(new { Errors = result.Errors });
    }

    [HttpGet("test-error")]
    public IActionResult TestError()
    {
    throw new InvalidOperationException("Test exception!");
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<PostListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PostListDto>>> Search([FromQuery] SearchCriteria criteria, CancellationToken ct)
    {
    var result = await _service.SearchAsync(criteria, ct);
    return Ok(result);
    }

    [HttpPost("{postId:int}/comments")]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CommentDto>> AddComment(int postId, [FromBody] CreateCommentDto dto, CancellationToken ct)
    {
    var result = await _commentService.AddCommentAsync(postId, dto, ct);
    if (!result.IsSuccess)
    {
        if (result.Errors.Any(e => e.Contains("not found")))
            return NotFound(new { Errors = result.Errors });
        return BadRequest(new { Errors = result.Errors });
    }
    return CreatedAtAction(nameof(GetById), new { id = postId }, result.Value);
    }
}