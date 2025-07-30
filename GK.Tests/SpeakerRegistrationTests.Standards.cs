using GK.Models;
using GK.Talks;
using GK.Tests.TestDoubles;
using System.Runtime.CompilerServices;

namespace GK.Tests
{
    public partial class SpeakerRegistrationTests
    {
        [Theory]
        [MemberData(nameof(StandardsInputData.Inputs), MemberType = typeof(StandardsInputData))]
        public void StandardsValidation(StandardsInputData.Input fixture, string email, WebBrowser browser, RegisterError? expected)
        {
            var s = new Speaker()
            {
                FirstName = "firstName",
                LastName = "lastName",
                Email = email,
                Browser = browser,
                Exp = fixture.Experience,
                HasBlog = fixture.HasBlog,
                Certifications = fixture.Certifications,
                Employer = fixture.Employer,
                Sessions = [ApprovableSession]
            };
            var response = s.Register(repository: new FakeRepository());

            Assert.Equal(expected, response.Error);
        }

        public class StandardsInputData
        {
            private const string StandardEmployer = "Microsoft";
            private const string NonStandardEmployer = "Lidl";

            private const string PermissableEmailDomain = "example.org";
            private const string NotPermissableEmailDomain = "aol.com";

            private readonly static WebBrowser GoogleChromeBrowserVersion27 = new WebBrowser(Name: "Google Chrome", MajorVersion: 27);
            private readonly static WebBrowser InternetExplorerPermissableVersion = new WebBrowser(
                Name: WebBrowser.BrowserName.InternetExplorer,
                MajorVersion: 10);

            private static readonly WebBrowser InternetExplorerUnpermissableVersion = InternetExplorerPermissableVersion with { MajorVersion = 6 };

            private static readonly object? SuccessErrorResponse = null;

            public record Input(int Experience, bool HasBlog, string[] Certifications, string Employer)
            {
                // All values meet the standards
                public static Input Sufficient = new Input(
                    Experience: 12,
                    HasBlog: true,
                    Certifications: ["Certification #1", "Certification #2", "Certification #3"],
                    Employer: StandardEmployer);

                // No values meet the standards
                public static Input Insufficient = new Input(
                    Experience: 4,
                    HasBlog: false,
                    Certifications: [],
                    Employer: NonStandardEmployer);
            }

            public static IEnumerable<object?[]> Inputs()
            {
                // Meets standards - doesn't meet secondard allowance but this is OK since primary standard checks are sufficient
                yield return [ Input.Sufficient, $"test@{NotPermissableEmailDomain}", GoogleChromeBrowserVersion27,         SuccessErrorResponse ];
                yield return [ Input.Sufficient, $"test@{NotPermissableEmailDomain}", InternetExplorerPermissableVersion,   SuccessErrorResponse ];
                yield return [ Input.Sufficient, $"test@{PermissableEmailDomain}",    InternetExplorerUnpermissableVersion, SuccessErrorResponse ];

                // Doesn't meet standards but meets the secondary allowance checks
                yield return [ Input.Sufficient with {  Experience = 4 },                 $"test@{PermissableEmailDomain}", InternetExplorerPermissableVersion, SuccessErrorResponse ];
                yield return [ Input.Sufficient with {  HasBlog  = false },               $"test@{PermissableEmailDomain}", InternetExplorerPermissableVersion, SuccessErrorResponse ];
                yield return [ Input.Sufficient with {  Certifications = [] },            $"test@{PermissableEmailDomain}", InternetExplorerPermissableVersion, SuccessErrorResponse ];
                yield return [ Input.Sufficient with {  Employer = NonStandardEmployer }, $"test@{PermissableEmailDomain}", InternetExplorerPermissableVersion, SuccessErrorResponse ];

                // Doesn't meet standards or the secondary allowance checks of email and browser
                yield return [ Input.Insufficient, $"test@{NotPermissableEmailDomain}", GoogleChromeBrowserVersion27,         RegisterError.SpeakerDoesNotMeetStandards ];
                yield return [ Input.Insufficient, $"test@{NotPermissableEmailDomain}", InternetExplorerPermissableVersion,   RegisterError.SpeakerDoesNotMeetStandards ];
                yield return [ Input.Insufficient, $"test@{PermissableEmailDomain}",    InternetExplorerUnpermissableVersion, RegisterError.SpeakerDoesNotMeetStandards ];
            }
        }
    }
}
