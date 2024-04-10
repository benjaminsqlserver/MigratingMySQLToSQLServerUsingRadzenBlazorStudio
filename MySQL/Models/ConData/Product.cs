using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCatalogue.Models.ConData
{
    [Table("product")]
    public partial class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int product_id { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string product_name { get; set; }

        [Required]
        [ConcurrencyCheck]
        public decimal price { get; set; }

        [ConcurrencyCheck]
        public int? category_id { get; set; }

        public Productcategory Productcategory { get; set; }

    }
}