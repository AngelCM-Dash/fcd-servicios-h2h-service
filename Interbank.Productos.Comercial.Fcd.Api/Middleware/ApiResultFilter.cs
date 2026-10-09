using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Interbank.Productos.Comercial.Fcd.Api.Middleware
{
    public class ApiResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = new Dictionary<string, List<string>>();

                foreach (var keyModelStatePair in context.ModelState)
                {
                    var key = keyModelStatePair.Key;
                    var modelErrors = keyModelStatePair.Value.Errors;
                    if (modelErrors != null && modelErrors.Count > 0)
                    {
                        var errorMessages = modelErrors.Select(error => error.ErrorMessage).ToList();
                        errors.Add(key, errorMessages);
                    }
                }
                if (errors.Count > 0)
                {
                    throw new ValidationException(errors);
                }
            }
        }

    }
}
