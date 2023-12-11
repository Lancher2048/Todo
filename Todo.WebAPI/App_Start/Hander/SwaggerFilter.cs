using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Todo.WebAPI.App_Start
{
    public class SwaggerFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //operation.Parameters.Add(new OpenApiParameter
            //{
            //    Name = "token",
            //    Description = "Token",
            //    In = ParameterLocation.Header,
            //    Required = false
            //});
        }
    }
}
