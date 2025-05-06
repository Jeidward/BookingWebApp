using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class ReviewDto
    {
        public int Id { get; }
        public AccountHolderDto Account { get; }
        public int Rating { get; }      //  1 to 5 stars
        public string Comment { get; }
        public int CleanlinessRating { get; }
        public int LocationRating { get; }
        public int ComfortRating { get; }
        public int ValueRating { get; }
        public DateTime? CreatedAt { get; }
    }
}
