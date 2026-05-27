using Blog.Application.DTOs;
using Blog.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _service;

    public AuthorsController(IAuthorService service) => _service = service;

    /// <summary>Get all authors</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AuthorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAll(CancellationToken ct)
    {
        var authors = await _service.GetAllAsync(ct);
        return Ok(authors);
    }

    /// <summary>Get an author by id</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuthorDto>> GetById(int id, CancellationToken ct)
    {
        var author = await _service.GetByIdAsync(id, ct);
        return author == null ? NotFound() : Ok(author);
    }

    /// <summary>Create a new author</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthorDto>> Create([FromBody] CreateAuthorDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        if (!result.IsSuccess)
            return BadRequest(new { Errors = result.Errors });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>Update an existing author</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateAuthorDto dto,
        CancellationToken ct)
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

    /// <summary>Delete an author</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return result.IsSuccess ? NoContent() : NotFound(new { Errors = result.Errors });
    }
}