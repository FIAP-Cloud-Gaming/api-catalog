namespace Catalog.Domain.Exceptions;

public class InvalidGameTitleException(string? title) 
    : CatalogDomainException($"Game title '{title}' is invalid. Title cannot be empty and must be less than 200 characters.")
{ }
