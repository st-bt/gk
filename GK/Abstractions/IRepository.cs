using GK.Talks;

namespace GK.Abstractions
{
    public interface IRepository
    {
        int SaveSpeaker(Speaker speaker);
    }
}
