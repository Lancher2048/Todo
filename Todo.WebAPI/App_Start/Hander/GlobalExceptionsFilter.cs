using Microsoft.AspNetCore.Mvc.Filters;

namespace Todo.WebAPI.App_Start
{
    public class GlobalExceptionsFilter : IExceptionFilter
    {

        public void OnException(ExceptionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
