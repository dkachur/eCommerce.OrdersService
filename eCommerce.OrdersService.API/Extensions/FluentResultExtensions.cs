using AutoMapper;
using eCommerce.OrdersService.API.Interfaces;
using eCommerce.OrdersService.Application.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.OrdersService.API.Extensions;

public static class FluentResultExtensions
{
    public static IActionResult ToOkApiResult<TSource, TDest>(
        this Result<TSource> result,
        ControllerBase controller,
        IMapper mapper)
    {
        return result.ToResult().ToApiResult(() =>
        {
            TDest mapped = mapper.Map<TDest>(result.Value);
            return controller.Ok(mapped);
        }, controller); 
    }

    public static IActionResult ToCreatedApiResult<TSource, TDest>(
        this Result<TSource> result, 
        ControllerBase controller,
        IMapper mapper,
        string actionName)
        where TDest : IResourceWithId
    {
        return result.ToResult().ToApiResult(() =>
        {
            TDest mapped = mapper.Map<TDest>(result.Value);
            var routeValues = new { id = mapped.ResourceId };
            return controller.CreatedAtAction(actionName, routeValues, mapped);
        }, controller);
    }

    public static IActionResult ToNoContentApiResult(
        this Result result,
        ControllerBase controller)
    {
        return result.ToApiResult(() => controller.NoContent(), controller);
    }

    public static IActionResult ToApiResult(this Result result, Func<IActionResult> successFactory, ControllerBase controller)
    {
        if (result.IsFailed)
            return result.ToErrorResult(controller);

        return successFactory();
    }

    private static IActionResult ToErrorResult(this Result result, ControllerBase controller)
    {
        var firstError = result.Errors.FirstOrDefault();

        if (firstError is null)
            return controller.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred.",
                detail: "Unknown error");

        return firstError switch
        {
            OrderNotFoundError => controller.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Order not found.",
                detail: firstError.Message),

            PersistenceError => controller.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred.",
                detail: firstError.Message),

            ValidationError => controller.ValidationProblem(
                result.Errors.ToModelStateDictionary()),

            _ => controller.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred.",
                detail: firstError.Message),
        };
    }

    private static IActionResult ToErrorResult<T>(this Result<T> result, ControllerBase controller) 
        => result.ToResult().ToErrorResult(controller);

    #region Helpers

    private static ValidationProblemDetails ToModelStateDictionary(this IReadOnlyList<IError> errors)
    {
        return new(errors.OfType<ValidationError>()
                         .GroupBy(e => e.PropertyName)
                         .ToDictionary(group => group.Key,
                                       group => group.Select(e => e.Message)
                                                     .ToArray()));
    }

    #endregion
}