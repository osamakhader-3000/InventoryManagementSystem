
namespace SecurityApplicationLogic
{
    internal class Exceptions
    {
        [Serializable]
        internal class UserNotFoundException : Exception
        {
            public UserNotFoundException()
            {
            }

            public UserNotFoundException(string? message) : base(message)
            {
            }

            public UserNotFoundException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }

        [Serializable]
        internal class InvalidPasswordException : Exception
        {
            public InvalidPasswordException()
            {
            }

            public InvalidPasswordException(string? message) : base(message)
            {
            }

            public InvalidPasswordException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }
    }
}