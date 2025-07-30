namespace GK.Domain.Actions.Speaker.Register
{
    public enum RegisterError
    {
        NoSessionsProvided,
        NoSessionsApproved,
        SpeakerDoesNotMeetStandards,
        EmailRequired,
        LastNameRequired,
        FirstNameRequired
    }
}
