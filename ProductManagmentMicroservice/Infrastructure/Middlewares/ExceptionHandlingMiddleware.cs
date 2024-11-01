using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using ProductManagmentMicroservice.Infrastructure.Exceptions;


namespace ProductManagmentMicroservice.Infrastructure.Middlewares
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionHandlingMiddleware> _logger;

		public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (BadRequestException ex)
			{
				_logger.LogError(ex, ex.Message);
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				await context.Response.WriteAsync(ex.Message);
			}
			catch (ValidationException ex) 
			{
				_logger.LogError(ex, ex.Message);
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				await context.Response.WriteAsync(ex.Message);
			}
			catch (NotFoundException ex)
			{
				_logger.LogError(ex, ex.Message);
				context.Response.StatusCode = StatusCodes.Status404NotFound;
				await context.Response.WriteAsync(ex.Message);
			}
			catch (DatabaseException ex)
			{
				_logger.LogError(ex, ex.Message);
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				await context.Response.WriteAsync("An internal server error occurred while accessing the database.");
			}
			catch (ProductUnauthorizedAccessException ex)
			{
				_logger.LogError(ex, ex.Message);
				context.Response.StatusCode = StatusCodes.Status403Forbidden;
				await context.Response.WriteAsync(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				await context.Response.WriteAsync("An internal server error occurred.");
			}
		}

	}
}
