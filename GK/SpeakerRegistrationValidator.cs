using GK.Models;
using System.Diagnostics.CodeAnalysis;

namespace GK.Talks
{
    public class SpeakerRegistrationValidator
    {
		private static readonly string[] AcceptedEmployers =
        [
			"Pluralsight", "Microsoft", "Google"
        ];

		private static readonly string[] AcceptedEmailDomains =
        [
			"aol.com", "prodigy.com", "compuserve.com"
        ];

        private bool SpeakerMeetsStandards(Speaker speaker)
        {
            var domain = speaker.Email.Split('@').Last();
            return
                speaker.Exp > 10 ||
                speaker.HasBlog ||
                speaker.Certifications.Count() > 3 ||
                AcceptedEmployers.Contains(speaker.Employer) ||
                (
                    !AcceptedEmailDomains.Contains(domain) &&
                    !(speaker.Browser.Name == WebBrowser.BrowserName.InternetExplorer && speaker.Browser.MajorVersion < 9)
                );
        }

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

            if (!SpeakerMeetsStandards(speaker))
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
