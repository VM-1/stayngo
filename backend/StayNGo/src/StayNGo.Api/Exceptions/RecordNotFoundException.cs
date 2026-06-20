namespace StayNGo.Api.Exceptions;

public class RecordNotFoundException(string entityName, object id)
    : Exception($"{entityName} with Id {id} was not found.");