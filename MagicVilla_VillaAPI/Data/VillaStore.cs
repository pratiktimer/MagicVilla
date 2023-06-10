using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Data
{
    public static class VillaStore
    {
       public static List<VillaDTO> villaList = new List<VillaDTO>()
        {
                new VillaDTO() { ID = 1, Name = "Pol View" },
                new VillaDTO() { ID = 2, Name = "Ocean Paradise" },
                new VillaDTO() { ID = 3, Name = "Sunset Retreat" },
                new VillaDTO() { ID = 4, Name = "Tropical Haven" },
                new VillaDTO() { ID = 5, Name = "Island Oasis" },
                new VillaDTO() { ID = 6, Name = "Seaside Serenity" },
                new VillaDTO() { ID = 7, Name = "Palm Villa" },
                new VillaDTO() { ID = 8, Name = "Vista Del Mar" },
                new VillaDTO() { ID = 9, Name = "Azure Dream" },
                new VillaDTO() { ID = 10, Name = "Harmony Hills" },
                // Add more villa objects with fake data as needed
            };
    }
}
