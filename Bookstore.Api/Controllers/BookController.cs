using Asp.Versioning;
using Book.Application.Inputs;
using Book.Application.Services.Book;
using Bookstore.Api.Mappers.Book;
using Bookstore.Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateBookRequest request, CancellationToken cancellationToken)
    {
        var createBookInput = BookMappers.MapToInput(request);
        await _bookService.CreateAsync(createBookInput, cancellationToken);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> ListAsync([FromQuery] int limit, [FromQuery] int offset, CancellationToken cancellationToken)
    {
        var response = await _bookService.ListAsync(offset, limit, cancellationToken);
        return Ok(response);
    }
    [HttpGet("search")]
    public async Task<IActionResult> SearchAsync([FromQuery] SearchBooksRequest request, CancellationToken cancellationToken)
    {
        var input = new SearchBooksInput
        {
            Limit = request.Limit,
            Offset = request.Offset,
            Title = request.Title
        };
        var response = await _bookService.SearchAsync(input, cancellationToken);
        return Ok(response);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetailAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await _bookService.GetDetailAsync(id, cancellationToken);
        return Ok(response);
    }
    
}