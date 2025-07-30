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

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public int? Exp { get; set; }
		public bool HasBlog { get; set; }
		public string BlogURL { get; set; }
		public WebBrowser Browser { get; set; }
		public List<string> Certifications { get; set; }
		public string Employer { get; set; }
		public int RegistrationFee { get; set; }
		public List<Session> Sessions { get; set; }

		/// <summary>
		/// Register a speaker
		/// </summary>
        /// <param name="repository">The implementation of <paramref name="repository"/> for persisting the speaker</param>
        /// <param name="Email">The speaker's email address</param>
		/// <returns>Returns the unique ID for the speaker as determined by <paramref name="repository"/>.</returns>
		public RegisterResponse Register(IRepository repository, string Email)
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

			bool good =
                Exp > 10 ||
                HasBlog ||
                Certifications.Count() > 3 ||
                AcceptedEmployers.Contains(Employer) ||
                (
                    !AcceptedEmailDomains.Contains(Email.Split('@').Last()) &&
                    !(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9)
                );

			if (!good)
			{
				return new RegisterResponse(RegisterError.SpeakerDoesNotMeetStandards);
			}

            if (Sessions.Count() == 0)
            {
                return new RegisterResponse(RegisterError.NoSessionsProvided);
            }

			bool appr = false;
            foreach (var session in Sessions)
            {
                foreach (var tech in OldTechnology)
                {
                    if (session.Title.Contains(tech) || session.Description.Contains(tech))
                    {
                        appr = false;
                        session.Approved = false;
                        break;
                    }
                    else
                    {
                        session.Approved = true;
                        appr = true;
                    }
                }
            }

            if (!appr)
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
