using GK.Abstractions;
using GK.Talks;

namespace GK.Tests.TestDoubles
{
    /// <summary>
    /// <para>
    /// Simple in-memory test double for <see cref="IRepository"/>.
    /// </para>
    /// <para>
    /// Modelled as a dictionary where the key is the unique id (speaker id) and the value is the speaker instance.
    /// </para>
    /// </summary>
    internal class FakeRepository : Dictionary<int, Speaker>, IRepository
    {
        private int _id = 0;

        /// <summary>
        /// Persist <paramref name="speaker"/> for future retrieval.
        /// </summary>
        /// <param name="speaker">The speaker instance to persist.</param>
        /// <returns>Returns the unique ID for the persisted speaker.</returns>
        /// <remarks>
        /// ID generation is thread safe
        /// </remarks>
        public int SaveSpeaker(Speaker speaker)
        {
            var id = Interlocked.Increment(ref _id);
            Add(id, speaker);
            return id;
        }
    }
}