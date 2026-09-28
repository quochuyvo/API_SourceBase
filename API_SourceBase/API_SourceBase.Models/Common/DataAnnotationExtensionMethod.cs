using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API_SourceBase.Models.Common
{
    public static class DataAnnotationExtensionMethod
    {
        public static string GetErrorMessage(ModelStateDictionary modelState)
        {
            var errors = modelState.Values
                .SelectMany(v => v.Errors)
                .Select(b => b.ErrorMessage)
                .ToList();
            return string.Join("; ", errors);
        }
    }
}
