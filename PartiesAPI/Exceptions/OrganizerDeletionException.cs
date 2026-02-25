namespace PartiesAPI.Exceptions
{
    public class OrganizerDeletionException : Exception
    {
        public OrganizerDeletionException() { }

        public OrganizerDeletionException(string message) : base(message) { }

        public OrganizerDeletionException(string message, Exception inner) : base(message, inner) { }
    }
}
