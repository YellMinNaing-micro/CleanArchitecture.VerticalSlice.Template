using FluentValidation;
using CleanArch.WebApi.Helpers;
using CleanArch.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CleanArch.WebApi.Middleware;

public class ApiExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionHandlingMiddleware> _logger;

    public ApiExceptionHandlingMiddleware(RequestDelegate next, ILogger<ApiExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        MessageResponse message;
        IReadOnlyDictionary<string, string[]>? errors = null;

        if (exception is ValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            message = new MessageResponse
            {
                EN = "One or more validation errors occurred.",
                MM = "အချက်အလက်တစ်ခု သို့မဟုတ် တစ်ခုထက်ပို၍ မှားယွင်းနေပါသည်။"
            };

            errors = validationException.Errors
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

        }
        else if (exception is KeyNotFoundException keyNotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            message = new MessageResponse
            {
                EN = keyNotFoundException.Message,
                MM = "တောင်းဆိုထားသော အချက်အလက်ကို ရှာမတွေ့ပါ။"
            };
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            message = new MessageResponse
            {
                EN = "An unexpected error occurred.",
                MM = "မျှော်လင့်မထားသော ချို့ယွင်းချက်တစ်ခု ဖြစ်ပွားခဲ့ပါသည်။"
            };
        }

        var json = JsonSerializer.Serialize(
            ApiResponse.Error(message, errors),
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        await context.Response.WriteAsync(json);
    }
}
