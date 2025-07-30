using GK.Talks;

namespace GK.Tests
{
    public class RegisterResponseTests
    {
        [Fact]
        public void Success()
        {
            var result = RegisterResponse.Success(speakerId: 10);
            Assert.True(result.IsSuccessful);
            Assert.True(result.SpeakerId.HasValue);
            Assert.False(result.Error.HasValue);
            Assert.Equal(10, result.SpeakerId);
        }

        [Fact]
        public void Failure()
        {
            var result = RegisterResponse.Failure(error: RegisterError.EmailRequired);
            Assert.False(result.IsSuccessful);
            Assert.False(result.SpeakerId.HasValue);
            Assert.True(result.Error.HasValue);
            Assert.Equal(RegisterError.EmailRequired, result.Error);
        }
    }
}