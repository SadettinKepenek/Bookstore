using Asp.Versioning;
using Bookstore.Api.Mappers.Order;
using Bookstore.Api.Models.Requests;
using Bookstore.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Enums;
using Order.Application.Services;
using Saga.Application;
using Saga.Application.Enums;

namespace Bookstore.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class OrderController : ControllerBase
{
    private readonly SagaOrchestrator _sagaOrchestrator;
    private readonly IOrderService _orderService;

    public OrderController(SagaOrchestrator sagaOrchestrator, IOrderService orderService)
    {
        _sagaOrchestrator = sagaOrchestrator;
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var context = new OrderSagaContext
        {
            BookId = request.BookId,
            Quantity = request.Quantity
        };

        var result = await _sagaOrchestrator.ExecuteAsync(context, cancellationToken);

        if (result.Success)
            return Created();
        
        return FailureOrderCreationResponse(result);
      
    }

   

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetailsAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var orderOutput = await _orderService.GetDetailsAsync(id, cancellationToken);
        var orderResponse = OrderMappers.MapToGetOrderDetailsResponse(orderOutput);

        return Ok(orderResponse);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStatusAsync([FromRoute] Guid id, [FromBody] UpdateOrderStatusRequest request,CancellationToken cancellationToken)
    {
        await _orderService.UpdateStatusAsync(id,request.Status, cancellationToken);
        return NoContent();
    }
    
    private IActionResult FailureOrderCreationResponse(SagaOrchestrationResult result)
    {
        return result.ErrorType switch
        {
            SagaErrorType.NotFound => NotFound(new OrderCreateFailResponse{Message = result.ErrorMessage}),
            SagaErrorType.BadRequest => BadRequest(new OrderCreateFailResponse{Message = result.ErrorMessage}),
            SagaErrorType.DomainError => BadRequest(new OrderCreateFailResponse{Message = result.ErrorMessage}),
            _ => StatusCode(500, new OrderCreateFailResponse{Message = "Bilinmeyen bir hata oluştu"})
        };
    }
}