﻿using System;

namespace homepageJUnpro
{
    public class Barang
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; } // Menyesuaikan dengan penamaan sebelumnya
        public string Category { get; set; }  // Menyesuaikan dengan penamaan sebelumnya
    }
}
