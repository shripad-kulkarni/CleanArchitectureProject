namespace Project.Domain.Exceptions
{
    public sealed class UserNotFoundException : DomainException
    {
        public UserNotFoundException(int id)
            : base($"User with Id '{id}' was not found.")
        {
        }

        public UserNotFoundException(string identifier)
            : base($"User with identifier '{identifier}' was not found.")
        {
        }
    }
}
