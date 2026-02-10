namespace Catalog.Domain.Exceptions;

public class InvalidRatingException(int rating) 
    : CatalogDomainException($"Rating '{rating}' is invalid. Rating must be between 1 and 5.")
{ }
