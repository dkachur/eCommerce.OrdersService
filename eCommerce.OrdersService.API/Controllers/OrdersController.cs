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
using eCommerce.OrdersService.Application.Queries.GetOrdersByUserId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.OrdersService.API.Controllers
{
    /// <summary>
    /// Provides API endpoints for managing customer orders.
    /// </summary>
    [ApiController]
    [Route("/api/orders")]
    public class OrdersController: ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance for dispatching requests.</param>
        /// <param name="mapper">The mapper instance for transforming objects between layers.</param>
        public OrdersController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        /// <returns>A list of all orders.</returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrders()
        {
            var result = await _mediator.Send(new GetOrdersQuery());
            return result.ToOkApiResult<List<OrderDto>, List<OrderResponse>>(this, _mapper);
        }

        /// <summary>
        /// Retrieves a specific order by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the order.</param>
        /// <returns>The requested order if found; otherwise, a 404 response.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(id));
            return result.ToOkApiResult<OrderDto, OrderResponse>(this, _mapper);
        }

        /// <summary>
        /// Retrieves all orders that contain a specific product.
        /// </summary>
        /// <param name="productId">The unique identifier of the product.</param>
        /// <returns>A list of orders that include the specified product; if not found, an empty list.</returns>
        [HttpGet("search/productid/{productId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrdersByProductId([FromRoute] Guid productId)
        {
            var result = await _mediator.Send(new GetOrdersByProductIdQuery(productId));
            return result.ToOkApiResult<List<OrderDto>, List<OrderResponse>>(this, _mapper);
        }

        /// <summary>
        /// Retrieves all orders that correspond to the specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A list of orders that correspond to the specific user; if not found, an empty list.</returns>
        [HttpGet("search/userid/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrdersByUserId([FromRoute] Guid userId)
        {
            var result = await _mediator.Send(new GetOrdersByUserIdQuery(userId));
            return result.ToOkApiResult<List<OrderDto>, List<OrderResponse>>(this, _mapper);
        }

        /// <summary>
        /// Retrieves all orders created on a specific date.
        /// </summary>
        /// <param name="orderDate">The date of the orders to search for.</param>
        /// <returns>A list of orders placed on the specified date; if not found, an empty list.</returns>
        [HttpGet("search/orderdate/{orderDate:datetime}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrdersByOrderDate([FromRoute] DateTime orderDate)
        {
            var result = await _mediator.Send(new GetOrdersByOrderDateQuery(orderDate));
            return result.ToOkApiResult<List<OrderDto>, List<OrderResponse>>(this, _mapper);
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="request">The order data to create.</param>
        /// <returns>
        /// The created order with a 201 response and a location header if successful;
        /// otherwise, an error response.
        /// </returns>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostOrder([FromBody] AddOrderRequest request)
        {
            var addOrderDto = _mapper.Map<AddOrderDto>(request);
            var result = await _mediator.Send(new AddOrderCommand(addOrderDto));
            return result.ToCreatedApiResult<OrderDto, OrderResponse>(
                this, _mapper, nameof(GetOrderById));
        }

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="id">The identifier of the order to update.</param>
        /// <param name="request">The updated order data.</param>
        /// <returns>The updated order if successful; otherwise, an error response.</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Deletes an order by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the order to delete.</param>
        /// <returns>
        /// A 204 No Content response if the deletion is successful; 
        /// otherwise, an error response.
        /// </returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteOrder([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteOrderCommand(id));
            return result.ToNoContentApiResult(this);
        }
    }
}
