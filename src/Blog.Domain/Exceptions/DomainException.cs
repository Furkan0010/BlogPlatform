namespace Blog.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception inner) : base(message, inner) { }
}

public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, int id)
        : base($"{entityName} with id {id} was not found.") { }
}

public class DuplicateSlugException : DomainException
{
    public DuplicateSlugException(string slug)
        : base($"A post with slug '{slug}' already exists.") { }
}