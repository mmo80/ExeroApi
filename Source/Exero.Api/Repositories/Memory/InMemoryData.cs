using System;
using System.Collections.Generic;
using Exero.Api.Models;

namespace Exero.Api.Repositories.Memory
{
    public static class InMemoryData
    {
        public static IList<User> Users => new List<User>
        {
            new User() { Id = Guid.Parse("f93ac602-e896-47ec-b5b5-d11702c033de"), Email = "mmo_80@yahoo.se" }
        };

        public static User FirstUser => Users[0];

        public static IList<Category> Categories => new List<Category>()
        {
            new Category() { Id = Guid.Parse("b6a521ef-a47b-4966-ae02-cb889e8f2cc3"), Name = "Strength"}, // , User = FirstUser
            new Category() { Id = Guid.Parse("d064b338-55e3-4d5d-a33a-10ae6711894a"), Name = "Conditioning"} // , User = FirstUser
        };

        //public static IList<Exercise> Exercices => new List<Exercise>()
        //{
        //    new Exercise() { Id = Guid.Parse("7160d2c3-734a-4f02-867e-fcc343f551c8"), Name = "Bench Press", Category = Categories[1]},
        //    new Exercise() { Id = Guid.Parse("5bedd1a0-d3c5-4feb-bc42-82f7271597b5"), Name = "Incline Bench Press" , Category = Categories[1]},
        //    new Exercise() { Id = Guid.Parse("977b7a0c-2a9c-4bd7-932f-63d2b6ee09ea"), Name = "Alternated Biceps Curl" , Category = Categories[0]}
        //};
    }
}
