using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPro.Application.Features.Orders.Queries.GetAllItems
{
    internal class ________GetAllItemsDto
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public int InterestedCount { get; set; }

        public int GoingCount { get; set; }

        public required string Author { get; set; }

        public DateTime EventDate { get; set; }
    }
}
