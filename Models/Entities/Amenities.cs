using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Amenities
    {
        public int Id { get; }
        public string Name { get; }
        public string ImgIcon { get; }

        public Amenities(int id, string name, string imgIcon)
        {
            Id = id;
            Name = name;
            ImgIcon = imgIcon;
        }

    }
}
