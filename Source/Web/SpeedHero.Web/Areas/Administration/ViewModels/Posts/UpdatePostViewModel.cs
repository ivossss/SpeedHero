﻿namespace SpeedHero.Web.Areas.Administration.ViewModels.Posts
{
    using System.ComponentModel.DataAnnotations;

    public class UpdatePostViewModel
    {
        // Server side validation
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string CoverPhotoPath { get; set; }
    }
}