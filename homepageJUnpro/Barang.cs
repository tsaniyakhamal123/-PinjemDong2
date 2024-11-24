public class Barang
{
    public int Id { get; set; } // barang_id
    public int OwnerId { get; set; } // owner_id
    public string Name { get; set; } // nama_barang
    public int Stock { get; set; } // stock
    public string Ulasan { get; set; } // ulasan
    public string Category { get; set; } // kategori
    public decimal Price { get; set; } // harga
    public string Description { get; set; } // deskripsi
    public string Address { get; set; } // alamat
    public string ImagePath { get; set; } // gambar (Base64 string)
}
