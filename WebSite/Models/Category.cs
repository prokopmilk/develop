using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace WebSite.Models
{
    public class Category
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public int Ca_ID { get; set; }

        [Display(Name = "Название")]
        [Required(ErrorMessage = "Пожалуйста, введите название категории")]
        public string Category_Name{ get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Изображение категории")]
        [Column(TypeName = "image")]
        public byte[] Pict { get; set; }

    }
}