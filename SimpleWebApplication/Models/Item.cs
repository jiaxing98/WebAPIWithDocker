using SimpleWebApplication.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleWebApplication.Models
{
    public record Item
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public int Price { get; init; }
        public DateTimeOffset CreatedDate { get; init; }

        public ItemDto ToDto()
        {
            return new ItemDto
            {
                Id = this.Id,
                Name = this.Name,
                Price = this.Price,
                CreatedDate = this.CreatedDate
            };
        }
    }
}
