namespace ProductManagmentMicroservice.Infrastructure.Exceptions
{
	public class NotFoundException : Exception
	{
		public NotFoundException(string message) : base(message) { }
	}
	public class BadRequestException : Exception
	{
		public BadRequestException(string message) : base(message) { }
	}

	public class DatabaseException : Exception
	{
		public DatabaseException(string message, Exception innerException)
			: base(message, innerException) { }
	}
	public class ProductUnauthorizedAccessException : Exception
	{
		public ProductUnauthorizedAccessException(string message) : base(message) { }
	}
}
