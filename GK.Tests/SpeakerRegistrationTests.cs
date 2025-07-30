using GK.Models;
using GK.Talks;
using GK.Tests.TestDoubles;

namespace GK.Tests
{
    public partial class SpeakerRegistrationTests
    {
        private readonly static RegisterError? Success = null;

        private readonly static Session ApprovableSession = new Session(
            title: "A talk about .NET",
            description: "Overview of the .NET runtime");

        private static readonly Speaker s_validSpeaker = new Speaker()
        {
            FirstName = "firstName",
            LastName = "lastName",
            Sessions = [ApprovableSession],
            Exp = 20,
            Browser = new WebBrowser(Name: WebBrowser.BrowserName.GoogleChrome, MajorVersion: 27)
        };

        [Fact]
        public void SuccessfulRegistration()
        {
            var repository = new FakeRepository();
            var response = s_validSpeaker.Register(
                repository: repository,
                email: "test@example.org");

            Assert.True(
                repository.ContainsKey(response.SpeakerId),
                userMessage: "The test double models the unique ID as the key");
        }


        [Theory]
        [InlineData(null,         null,         RegisterError.FirstNameRequired)]
        [InlineData("",           null,         RegisterError.FirstNameRequired)]
        [InlineData("firstName",  null,         RegisterError.LastNameRequired)]
        [InlineData("firstName",  "",           RegisterError.LastNameRequired)]
        public void NameValidation(string firstName, string lastName, RegisterError expected)
        {
            var repository = new FakeRepository();
            var s = new Speaker()
            {
                FirstName = firstName,
                LastName = lastName,
                Browser = s_validSpeaker.Browser
            };
            var response = s.Register(
                repository: repository,
                email: "test@example.org");

            Assert.Equal(expected, response.Error);
            Assert.Empty(repository);
        }

        [Theory]
        [InlineData(null, RegisterError.EmailRequired)]
        [InlineData("",   RegisterError.EmailRequired)]
        public void EmailValidation(string email, RegisterError expected)
        {
            var repository = new FakeRepository();
            var s = new Speaker()
            {
                FirstName = "firstName",
                LastName = "lastName",
                Email = email,
                Browser = s_validSpeaker.Browser
            };
            var response = s.Register(
                repository: new FakeRepository(),
                email: s.Email);

            Assert.Equal(expected, response.Error);
            Assert.Empty(repository);
        }

        [Theory]
        [InlineData(0,  500)]
        [InlineData(1,  500)]
        [InlineData(2,  250)]
        [InlineData(3,  250)]
        [InlineData(4,  100)]
        [InlineData(5,  100)]
        [InlineData(6,  50)]
        [InlineData(7,  50)]
        [InlineData(8,  50)]
        [InlineData(9,  50)]
        [InlineData(10, 0)]
        public void RegistrationFee(int experience, int expected)
        {
            var repository = new FakeRepository();
            var s = new Speaker()
            {
                FirstName = "firstName",
                LastName = "lastName",
                Email = "test@example.org",
                Sessions = [ApprovableSession],
                Certifications = [],
                Browser = new WebBrowser(Name: WebBrowser.BrowserName.GoogleChrome, MajorVersion: 27),
                Exp = experience
            };
            s.Register(
                repository: repository,
                email: s.Email);

            Assert.Equal(expected, s.RegistrationFee);
        }
    }
}