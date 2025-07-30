namespace GK.Domain.Models
{
    public class Session
    {
        public Session(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public string Title { get; }
        public string Description { get; }
        public bool Approved { get; set; }
    }
}
