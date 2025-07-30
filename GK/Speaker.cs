using GK.Abstractions;
using GK.Models;

namespace GK.Talks
{
	/// <summary>
	/// Represents a single speaker
	/// </summary>
	public class Speaker
	{
		public static readonly string[] OldTechnology =
        [
            "Cobol", "Punch Cards", "Commodore", "VBScript"
        ];

		public static readonly string[] AcceptedEmployers =
        [
			"Pluralsight", "Microsoft", "Google"
        ];

		public static readonly string[] AcceptedEmailDomains =
        [
			"aol.com", "prodigy.com", "compuserve.com"
        ];

		public string FirstName { get; init; } = string.Empty;
		public string LastName { get; init; } = string.Empty;
		public string Email { get; init; } = string.Empty;
		public int Exp { get; init; }
		public bool HasBlog { get; init; }
		public string BlogURL { get; init; } = string.Empty;
		public required WebBrowser Browser { get; init; }
		public List<string> Certifications { get; init; } = new List<string>();
		public string Employer { get; init; } = string.Empty;
		public int RegistrationFee { get; private set; }
		public List<Session> Sessions { get; init; } = new List<Session>();

        private void ApproveSessions()
        {
            foreach (var session in Sessions)
            {
                foreach (var tech in OldTechnology)
                {
                    if (session.Title.Contains(tech) || session.Description.Contains(tech))
                    {
                        session.Approved = false;
                        break;
                    }
                    else
                    {
                        session.Approved = true;
                    }
                }
            }
        }

        private bool SpeakerMeetsStandards()
        {
            var domain = Email.Split('@').Last();
            return
                Exp > 10 ||
                HasBlog ||
                Certifications.Count() > 3 ||
                AcceptedEmployers.Contains(Employer) ||
                (
                    !AcceptedEmailDomains.Contains(domain) &&
                    !(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9)
                );
        }

		/// <summary>
		/// Register a speaker
		/// </summary>
        /// <param name="repository">The implementation of <paramref name="repository"/> for persisting the speaker</param>
		/// <returns>Returns the unique ID for the speaker as determined by <paramref name="repository"/>.</returns>
		public RegisterResponse Register(IRepository repository)
		{
			if (string.IsNullOrEmpty(FirstName))
			{
				return new RegisterResponse(RegisterError.FirstNameRequired);
			}

			if (string.IsNullOrEmpty(LastName))
			{
				return new RegisterResponse(RegisterError.LastNameRequired);
			}

			if (string.IsNullOrEmpty(Email))
			{
				return new RegisterResponse(RegisterError.EmailRequired);
			}

			if (!SpeakerMeetsStandards())
			{
				return new RegisterResponse(RegisterError.SpeakerDoesNotMeetStandards);
			}

            if (Sessions.Count() == 0)
            {
                return new RegisterResponse(RegisterError.NoSessionsProvided);
            }


            ApproveSessions();
            if (Sessions.All(s => !s.Approved))
            {
                return new RegisterResponse(RegisterError.NoSessionsApproved);
            }

            //if we got this far, the speaker is approved
            //let's go ahead and register him/her now.
            //First, let's calculate the registration fee.
            //More experienced speakers pay a lower fee.
            RegistrationFee = Exp switch
            {
                <= 1 => 500,
                <= 3 => 250,
                <= 5 => 100,
                <= 9 => 50,
                _ => 0
            };

            //Now, save the speaker and sessions to the db.
            var speakerId = repository.SaveSpeaker(this);

			//if we got this far, the speaker is registered.
			return new RegisterResponse(speakerId);
		}
	}
}
