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

		public string FirstName { get; init; } = string.Empty;
		public string LastName { get; init; } = string.Empty;
		public string Email { get; init; } = string.Empty;
		public int YearsOfExperience { get; init; }
		public bool HasBlog { get; init; }
		public string BlogURL { get; init; } = string.Empty;
		public required WebBrowser Browser { get; init; }
		public string[] Certifications { get; init; } = Array.Empty<string>();
		public string Employer { get; init; } = string.Empty;
		public int RegistrationFee { get; private set; }
		public Session[] Sessions { get; init; } = Array.Empty<Session>();

        /// <summary>
        /// Determine which sessions are approved
        /// </summary>
        /// <remarks>
        /// Mutates the internal state of this instances <see cref="Sessions"/> elements
        /// </remarks>
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

		/// <summary>
		/// Register a speaker
		/// </summary>
        /// <param name="repository">The implementation of <paramref name="repository"/> for persisting the speaker</param>
		/// <returns>Returns the unique ID for the speaker as determined by <paramref name="repository"/>.</returns>
		public RegisterResponse Register(IRepository repository)
		{
            var validator = new SpeakerRegistrationValidator();
            if (!validator.Validate(this, out RegisterError? validationError))
            {
                return RegisterResponse.Failure(error: validationError.Value);
            }

            ApproveSessions();
            if (Sessions.All(s => !s.Approved))
            {
                return RegisterResponse.Failure(RegisterError.NoSessionsApproved);
            }

            //if we got this far, the speaker is approved
            //let's go ahead and register him/her now.
            RegistrationFee = new RegistrationFeeCalculator().CalculateFee(YearsOfExperience);

            //Now, save the speaker and sessions to the db.
            var speakerId = repository.SaveSpeaker(this);

			//if we got this far, the speaker is registered.
			return RegisterResponse.Success(speakerId);
		}
	}
}
