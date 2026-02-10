namespace Catalog.Domain.Exceptions;

public class InvalidPriceException(decimal price) 
    : CatalogDomainException($"Price '{price}' is invalid. Price must be greater than or equal to zero.")
{ }
