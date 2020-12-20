namespace FsMosquito
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.OpenApi.Models;
	using Swashbuckle.AspNetCore.SwaggerGen;
    using System.Linq;

    public class FileOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			var anyFileStreamResult = context.ApiDescription.SupportedResponseTypes
				.Any(x => x.Type == typeof(FileStreamResult));

			if (anyFileStreamResult)
			{
				//operation. = new[] { "application/octet-stream" };
				//operation.Responses["200"].Schema = new Schema { Type = "file" };
			}
		}
	}
}
