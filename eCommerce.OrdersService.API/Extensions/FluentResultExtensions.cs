using AutoMapper;
using eCommerce.OrdersService.API.Interfaces;
using eCommerce.OrdersService.Application.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.OrdersService.API.Extensions;

/// <summary>
/// Provides extension methods for converting <see cref="Result"/> and <see cref="Result{T}"/> 
/// from FluentResults into standardized <see cref="IActionResult"/> responses.
/// </summary>
public static class FluentResultExtensions
{
    /// <summary>
    /// Converts a successful <see cref="Result{T}"/> into an <c>OK</c> response with mapped data.
    /// </summary>
    /// <typeparam name="TSource">The source type contained in the result.</typeparam>
    /// <typeparam name="TDest">The destination type to map to.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="controller">The controller instance.</param>
    /// <param name="mapper">The AutoMapper instance used for mapping.</param>
    /// <returns>An <see cref="OkObjectResult"/> if successful, or an error response otherwise.</returns>
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

    /// <summary>
    /// Converts a successful <see cref="Result{T}"/> into a <c>CreatedAtAction</c> response with mapped data.
    /// </summary>
    /// <typeparam name="TSource">The source type contained in the result.</typeparam>
    /// <typeparam name="TDest">The destination type to map to. Must implement <see cref="IResourceWithId"/>.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="controller">The controller instance.</param>
    /// <param name="mapper">The AutoMapper instance used for mapping.</param>
    /// <param name="actionName">The action name for the <c>CreatedAtAction</c> response.</param>
    /// <returns>A <see cref="CreatedAtActionResult"/> if successful, or an error response otherwise.</returns>
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

    /// <summary>
    /// Converts a successful <see cref="Result"/> into a <c>NoContent</c> response.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="controller">The controller instance.</param>
    /// <returns>A <see cref="NoContentResult"/> if successful, or an error response otherwise.</returns>
    public static IActionResult ToNoContentApiResult(
        this Result result,
        ControllerBase controller)
    {
        return result.ToApiResult(() => controller.NoContent(), controller);
    }

    /// <summary>
    /// Converts a <see cref="Result"/> into an <see cref="IActionResult"/> 
    /// by applying a provided factory for the success case, or generating an error response otherwise.
    /// </summary>
    /// <param name="result">The operation result to convert.</param>
    /// <param name="successFactory">
    /// A factory function that produces the <see cref="IActionResult"/> to return when the result is successful.
    /// </param>
    /// <param name="controller">The controller instance used to generate error responses.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> representing either the successful response produced by 
    /// <paramref name="successFactory"/> or a standardized error response.
    /// </returns>
    /// <remarks>
    /// This method serves as the common conversion point for <c>Ok</c>, <c>Created</c>, and <c>NoContent</c> 
    /// convenience methods, but can also be used directly for custom behaviors.
    /// </remarks>
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
            PersistenceError => controller.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred.",
                detail: firstError.Message),

            OrderNotFoundError => controller.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Order not found.",
                detail: firstError.Message),

            InvalidUserIdError => controller.ValidationProblem(
                result.Errors.OfType<InvalidUserIdError>()
                    .ToModelStateDictionary(e => $"UserId[{e.UserId}]")),

            ValidationError => controller.ValidationProblem(
                result.Errors.OfType<ValidationError>()
                    .ToModelStateDictionary(e => e.PropertyName)),

            _ => controller.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred.",
                detail: firstError.Message),
        };
    }

    private static IActionResult ToErrorResult<T>(this Result<T> result, ControllerBase controller) 
        => result.ToResult().ToErrorResult(controller);

    #region Helpers

    private static ValidationProblemDetails ToModelStateDictionary<TError>(
        this IEnumerable<TError> errors,
        Func<TError, string> keySelector)
        where TError : Error
    {
        return new(errors.GroupBy(keySelector)
                         .ToDictionary(group => group.Key,
                                       group => group.Select(e => e.Message)
                                                     .ToArray()));
    }

    #endregion
}