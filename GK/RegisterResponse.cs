namespace GK.Talks
{
    public class RegisterResponse
    {
        public RegisterResponse(int speakerId)
        {
            SpeakerId = speakerId;
        }

        public RegisterResponse(RegisterError error)
        {
            Error = error;
        }

        public int SpeakerId { get; }
        public RegisterError? Error { get; }
    }
}
