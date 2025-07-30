using System.Diagnostics.CodeAnalysis;

namespace GK.Domain.Actions.Speaker.Register
{
    public record RegisterResponse
    {
        private RegisterResponse(int speakerId)
        {
            SpeakerId = speakerId;
            IsSuccessful = true;
        }

        private RegisterResponse(RegisterError error)
        {
            Error = error;
            IsSuccessful = false;
        }

        [MemberNotNullWhen(returnValue: true, members: nameof(SpeakerId))]
        [MemberNotNullWhen(returnValue: false, members: nameof(Error))]
        public bool IsSuccessful { get; }

        public int? SpeakerId { get; }

        public RegisterError? Error { get; }

        public static RegisterResponse Success(int speakerId) => new RegisterResponse(speakerId);

        public static RegisterResponse Failure(RegisterError error) => new RegisterResponse(error);
    }
}
