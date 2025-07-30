using GK.Models;
using GK.Talks;
using GK.Tests.TestDoubles;

namespace GK.Tests
{
    public partial class SpeakerRegistrationTests
    {
        [Theory]
        [MemberData(nameof(SessionInputData.Inputs), MemberType = typeof(SessionInputData))]
        public void SessionValidation(Session[] input, RegisterError? expected)
        {
            var repository = new FakeRepository();
            var s = new Speaker()
            {
                FirstName = "firstName",
                LastName = "lastName",
                Email = "test@example.org",
                Sessions = input,
                Certifications = [],
                Browser = new WebBrowser(Name: WebBrowser.BrowserName.GoogleChrome, MajorVersion: 27)
            };
            var response = s.Register(repository: new FakeRepository());

            Assert.Equal(expected, response.Error);
            Assert.Empty(repository);
        }

        public class SessionInputData
        {
            public static IEnumerable<object?[]> Inputs()
            {
                // No session submitted
                yield return [ Array.Empty<Session>(), RegisterError.NoSessionsProvided ];

                // Session title contains "old tech"
                yield return
                [
                    new []
                    {
                        new Session(title: "Cobol", description: "descriptive text")
                    },
                    RegisterError.NoSessionsApproved
                ];


                // Session description contains "old tech"
                yield return [
                    new []
                    {
                        // Currently this will fail because of a bug in the session approval loop - will fix in subsequent pass
                        new Session(title: "Permissable title", description: "A talk about Punch Cards")
                    },
                    RegisterError.NoSessionsApproved
                ];

                // 1 of 2 sessions are approved
                yield return [
                    new []
                    {
                        new Session(title: "Cobol", description: "descriptive text"),
                        new Session(title: "Permissable title", description: "Permissable description")
                    },
                    Success
                ];

                // All sessions are approved
                yield return [
                    new []
                    {
                        new Session(title: "Permissable title #1", description: "Permissable description #1"),
                        new Session(title: "Permissable title #2", description: "Permissable description #2")
                    },
                    Success
                ];
            }
        }
    }
}
