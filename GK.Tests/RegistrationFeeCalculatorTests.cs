using GK.Talks;

namespace GK.Tests
{
    public class RegistrationFeeCalculatorTests
    {
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
        public void RegistrationFee(int yearsOfExperience, int expected)
        {
            var calculator = new RegistrationFeeCalculator();
            var actual = calculator.CalculateFee(yearsOfExperience);
            Assert.Equal(expected, actual);
        }
    }
}