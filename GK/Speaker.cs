using GK.Abstractions;
using GK.Models;

namespace GK.Talks
{
	/// <summary>
	/// Represents a single speaker
	/// </summary>
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
		/// Prams 
		/// strFirstName speakers first name
		///	strLastName ^^^ last name
		/// Email the email
		/// blogs etc.....
		/// </summary>
		/// <returns>speakerID</returns>
		public RegisterResponse Register(IRepository repository, string strFirstName, String strLastName, string Email, int iExp, Boolean BHasBlog, string URL, string strBrowser, string csvCertifications, String s_Emp, int iFee, string csvSess)
		{
			// lets init some vars
			int? speakerId = null;
			//var nt = new List<string> {"Node.js", "Docker"};

			//DEFECT #5274 CL 12/10/2010
			//We weren't filtering out the prodigy domain so I added it.
			var domains = new List<string>() { "aol.com", "prodigy.com", "compuserve.com" };

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
                    !domains.Contains(Email.Split('@').Last()) &&
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
                //foreach (var tech in nt)
                //{
                //    if (session.Title.Contains(tech))
                //    {
                //        session.Approved = true;
                //        break;
                //    }
                //}

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

            if (appr)
            {
                //if we got this far, the speaker is approved
                //let's go ahead and register him/her now.
                //First, let's calculate the registration fee.
                //More experienced speakers pay a lower fee.
                if (Exp <= 1)
                {
                    RegistrationFee = 500;
                }
                else if (Exp >= 2 && Exp <= 3)
                {
                    RegistrationFee = 250;
                }
                else if (Exp >= 4 && Exp <= 5)
                {
                    RegistrationFee = 100;
                }
                else if (Exp >= 6 && Exp <= 9)
                {
                    RegistrationFee = 50;
                }
                else
                {
                    RegistrationFee = 0;
                }


                //Now, save the speaker and sessions to the db.
                speakerId = repository.SaveSpeaker(this);
            }
            else
            {
                return new RegisterResponse(RegisterError.NoSessionsApproved);
            }

			//if we got this far, the speaker is registered.
			return new RegisterResponse((int)speakerId);
		}
	}
}
