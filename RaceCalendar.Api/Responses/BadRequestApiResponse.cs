using Microsoft.AspNetCore.Mvc.ModelBinding;
using RaceCalendar.Api.Responses.Base;

namespace RaceCalendar.Api.Responses;

public class BadRequestApiResponse : ApiResponse
{
    public IEnumerable<string> Errors { get; }

    public BadRequestApiResponse(ModelStateDictionary modelState)
        : base(400)
    {
        if (modelState.IsValid)
        {
            throw new ArgumentException("ModelState must be invalid", nameof(modelState));
        }

        Errors = modelState
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage).ToArray();
    }
}
