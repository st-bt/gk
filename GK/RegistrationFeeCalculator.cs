namespace GK.Talks
{
    public class RegistrationFeeCalculator
    {
        /// <summary>
        /// <para>
        /// Determine a speaker's registration fee based on their years of experience.
        /// </para>
        /// <para>
        /// More experienced speakers pay a lower fee.
        /// </para>
        /// </summary>
        /// <param name="yearsOfExperience">The years of experience of the speaker</param>
        /// <returns>Returns the registration fee for the speaker</returns>
        public int CalculateFee(int yearsOfExperience) => yearsOfExperience switch
        {
            <= 1 => 500,
            <= 3 => 250,
            <= 5 => 100,
            <= 9 => 50,
            _ => 0
        };
    }
}
