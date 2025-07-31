using GK.Abstractions;
using GK.Domain.Actions.Speaker.Register;
using GK.Domain.Models.Speaker;
using System.Diagnostics.CodeAnalysis;

namespace GK.Domain.Validation
{
    public class SpeakerRegistrationValidator
    {
        private static readonly ISpecification<Speaker> _minimumSpeakerStandardsSpecification =
            new SpeakerMinimumStandardsSpecification();

        public bool Validate(
            Speaker speaker,
            [NotNullWhen(returnValue: false)] out RegisterError? validationError)
        {
            validationError = null;
            if (string.IsNullOrEmpty(speaker.FirstName))
            {
                validationError = RegisterError.FirstNameRequired;
                return false;
            }

            if (string.IsNullOrEmpty(speaker.LastName))
            {
                validationError = RegisterError.LastNameRequired;
                return false;
            }

            if (string.IsNullOrEmpty(speaker.Email))
            {
                validationError = RegisterError.EmailRequired;
                return false;
            }

            if (!_minimumSpeakerStandardsSpecification.IsSatisifiedBy(speaker))
            {
                validationError = RegisterError.SpeakerDoesNotMeetStandards;
                return false;
            }

            if (speaker.Sessions.Count() == 0)
            {
                validationError = RegisterError.NoSessionsProvided;
                return false;
            }

            return validationError == null;
        }
    }
}
