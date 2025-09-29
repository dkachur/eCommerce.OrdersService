using AutoMapper;
using eCommerce.OrdersService.API.DTOs;
using eCommerce.OrdersService.API.Extensions;
using eCommerce.OrdersService.Application.Commands.AddOrder;
using eCommerce.OrdersService.Application.Commands.DeleteOrder;
using eCommerce.OrdersService.Application.Commands.UpdateOrder;
using eCommerce.OrdersService.Application.DTOs;
using eCommerce.OrdersService.Application.Queries.GetOrderById;
using eCommerce.OrdersService.Application.Queries.GetOrders;
using eCommerce.OrdersService.Application.Queries.GetOrdersByOrderDate;
using eCommerce.OrdersService.Application.Queries.GetOrdersByProductId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.OrdersService.API.Controllers
{
    [ApiController]
    [Route("/api/orders")]
    public class OrdersController: ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public OrdersController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet()]
        public async Task<IActionResult> GetOrders()
        {
            var result = await _mediator.Send(new GetOrdersQuery());
            return result.ToOkApiResult<List<OrderDto>, List<OrderResponse>>(this, _mapper);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(id));
            return result.ToOkApiResult<OrderDto, OrderResponse>(this, _mapper);
        }

        [HttpGet("search/productid/{productId:guid}")]
        public async Task<IActionResult> GetOrdersByProductId([FromRoute] Guid productId)
        {
            var result = await _mediator.Send(new GetOrdersByProductIdQuery(productId));
            return result.ToOkApiResult<List<OrderDto>, List<OrderResponse>>(this, _mapper);
        }

        [HttpGet("search/orderdate/{orderDate:datetime}")]
        public async Task<IActionResult> GetOrdersByOrderDate([FromRoute] DateTime orderDate)
        {
            var result = await _mediator.Send(new GetOrdersByOrderDateQuery(orderDate));
            return result.ToOkApiResult<List<OrderDto>, List<OrderResponse>>(this, _mapper);
        }

        [HttpPost()]
        public async Task<IActionResult> PostOrder([FromBody] AddOrderRequest request)
        {
            var addOrderDto = _mapper.Map<AddOrderDto>(request);
            var result = await _mediator.Send(new AddOrderCommand(addOrderDto));
            return result.ToCreatedApiResult<OrderDto, OrderResponse>(
                this, _mapper, nameof(GetOrderById));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutOrder([FromRoute] Guid id, [FromBody] UpdateOrderRequest request)
        {
            if (id != request.OrderId)
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "ID does not match",
                    detail: "ID from body must match ID from route");

            var updateOrderDto = _mapper.Map<UpdateOrderDto>(request);
            var result = await _mediator.Send(new UpdateOrderCommand(updateOrderDto));
            return result.ToOkApiResult<OrderDto, OrderResponse>(this, _mapper);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteOrderCommand(id));
            return result.ToNoContentApiResult(this);
        }
    }
}
