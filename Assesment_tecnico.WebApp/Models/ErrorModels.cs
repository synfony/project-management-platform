namespace Assesment_tecnico.WebApp.Models
{
    public class IdentityError
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
    }

    public class ValidationProblemDetails
    {
        public string? Title { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
