using System.Text.Json.Serialization;

namespace GooglePlayApi.Models
{
    public class UserProfile
    {
        [JsonConstructor]
        public UserProfile() { }

        public UserProfile(Proto.UserProfile userProfile)
        {
            Name = userProfile.Name;
            Email = userProfile.ProfileDescription;
            Artwork = new Artwork(userProfile.Image[0]);
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public Artwork Artwork { get; set; }
    }
}