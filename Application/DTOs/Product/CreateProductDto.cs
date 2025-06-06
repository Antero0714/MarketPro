using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MarketPro.Application.DTOs.Product
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Название товара обязательно")]
        [MaxLength(200, ErrorMessage = "Название не может быть длиннее 200 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }

        [MaxLength(500, ErrorMessage = "Краткое описание не может быть длиннее 500 символов")]
        public string ShortDescription { get; set; }

        public string DetailedDescription { get; set; }

        public string Specifications { get; set; }

        [Range(0, 5, ErrorMessage = "Рейтинг должен быть от 0 до 5")]
        public decimal Rating { get; set; }

        [Required(ErrorMessage = "Тип товара обязателен")]
        public int ProductTypeId { get; set; }

        [Required(ErrorMessage = "Магазин обязателен")]
        public int StoreId { get; set; }

        // Для загрузки изображений
        public List<IFormFile> Images { get; set; }
    }
} 