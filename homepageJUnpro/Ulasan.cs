using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homepageJUnpro
{
    class Ulasan
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public int Reviewer { get; set; }
        public string Description { get; set; }
        public int Score { get; set; }
        public string ImagePath { get; set; }
    }
}
