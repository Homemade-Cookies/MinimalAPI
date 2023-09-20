namespace MinimalApi.Contracts;

public sealed record SessionContext
{
    public Guid SessionId { get; set; }
    public string UserName { get; set; }
}
