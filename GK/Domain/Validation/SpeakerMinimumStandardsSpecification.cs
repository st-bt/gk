using GK.Abstractions;
using GK.Domain.Models;
using GK.Domain.Models.Speaker;

namespace GK.Domain.Validation
{
    public class SpeakerMinimumStandardsSpecification : ISpecification<Speaker>
    {
		private static readonly string[] DisallowedEmailDomains =
        [
			"aol.com", "prodigy.com", "compuserve.com"
        ];

        private static readonly PrimaryStandardsSpecification _primarySpecification = new PrimaryStandardsSpecification();
        private static readonly SecondaryStandardsSpecification _secondarySpecification = new SecondaryStandardsSpecification();

        public bool IsSatisifiedBy(Speaker speaker) =>
            _primarySpecification.IsSatisifiedBy(speaker) ||
            _secondarySpecification.IsSatisifiedBy(speaker);

        private class PrimaryStandardsSpecification : ISpecification<Speaker>
        {
            private const int s_minimumYearsOfExperience = 11;
            private const int s_minimumNumberOfCertifications = 4;

            private static readonly string[] AcceptedEmployers =
            [
                "Pluralsight", "Microsoft", "Google"
            ];

            public bool IsSatisifiedBy(Speaker speaker) =>
                speaker.YearsOfExperience >= s_minimumYearsOfExperience ||
                speaker.HasBlog ||
                speaker.Certifications.Count() >= s_minimumNumberOfCertifications ||
                AcceptedEmployers.Contains(speaker.Employer);
        }

        private class SecondaryStandardsSpecification : ISpecification<Speaker>
        {
            public bool IsSatisifiedBy(Speaker speaker)
            {
                var domain = GetDomainFromEmailAddress(speaker.Email);
                return
                    IsBrowserPermitted(speaker.Browser) &&
                    !DisallowedEmailDomains.Contains(GetDomainFromEmailAddress(speaker.Email));
            }

            private bool IsBrowserPermitted(WebBrowser browser) => browser is not
            {
               Name: WebBrowser.BrowserName.InternetExplorer,
               MajorVersion: < 9
            };

            private string GetDomainFromEmailAddress(string email) => email.Split('@').Last();
        }
    }
}
