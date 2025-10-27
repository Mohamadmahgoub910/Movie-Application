using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Core.Entities
{
    public class MovieActor
    {
        public int MovieId { get; set; }
        public int ActorId { get; set; }

        // Navigation Properties
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }

        [ForeignKey("ActorId")]
        public Actor Actor { get; set; }
    }
}