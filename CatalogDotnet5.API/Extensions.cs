using CatalogDotnet5.API.Dtos;
using CatalogDotnet5.API.Entities;

namespace CatalogDotnet5.API
{
    public static class Extensions
    {
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto (item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }
    }
}