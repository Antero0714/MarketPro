using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MarketPro.Application.DTOs.Product
{
    public class EditProductDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название товара обязательно")]
        [MaxLength(200, ErrorMessage = "Название не может быть длиннее 200 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Краткое описание обязательно")]
        [MaxLength(500, ErrorMessage = "Краткое описание не может быть длиннее 500 символов")]
        [Display(Name = "Краткое описание")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Подробное описание обязательно")]
        [Display(Name = "Подробное описание")]
        public string DetailedDescription { get; set; }

        [Required(ErrorMessage = "Характеристики обязательны")]
        [Display(Name = "Характеристики")]
        public string Specifications { get; set; }

        [Range(0, 5, ErrorMessage = "Рейтинг должен быть от 0 до 5")]
        [Display(Name = "Рейтинг")]
        public decimal Rating { get; set; }

        [Required(ErrorMessage = "Тип товара обязателен")]
        [Display(Name = "Тип товара")]
        public int ProductTypeId { get; set; }

        [Required(ErrorMessage = "Магазин обязателен")]
        [Display(Name = "Магазин")]
        public int StoreId { get; set; }

        [Display(Name = "Изображения")]
        public List<IFormFile> Images { get; set; }
    }
} 