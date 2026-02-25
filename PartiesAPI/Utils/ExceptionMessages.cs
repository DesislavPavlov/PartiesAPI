namespace PartiesAPI.Utils
{
    public static class ExceptionMessages
    {
        public const string EventNotFound = "Event with ID {0} not found!";
        public const string InvalidEventModel = "The provided model for an event is invalid!";
        public const string UserNotFound = "User with ID {0} not found!";
        public const string InvalidUserModel = "The provided model for a user is invalid.";
        public const string UserAlreadyParticipant = "User with ID {0} already participates in event with ID {1}!";
        public const string UserAlreadyOrganizer = "User with ID {0} already organizes event with ID {1}!";
        public const string InvalidEmailFormat = "The provided email does not correspond to the correct format: example@domain.com!";
        public const string EmailTaken = "The provided email already exists!";
        public const string CannotDeleteOrganizer = "User with ID {0} is an organizer of one or more event(s)! Please, remove all organization rights and try again!";
        public const string DatabaseError = "Something went wrong while trying to access the database, we will be fixing the problem ASAP! Please, try again later!";
    }
}
