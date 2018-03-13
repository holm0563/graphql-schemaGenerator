namespace GraphQL.SchemaGenerator.Models
{
    public enum RequiredType
    {
        // Default required based on a nullable type
        Default = 0,

        // It is required based on attributes
        Required = 1,

        // It is not required based on attributes
        NotRequired
    }
}
