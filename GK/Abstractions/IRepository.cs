using GK.Domain.Models.Speaker;

namespace GK.Abstractions
{
    public interface IRepository
    {
        int SaveSpeaker(Speaker speaker);
    }
}
