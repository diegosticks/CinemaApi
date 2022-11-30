﻿using System.ComponentModel.DataAnnotations;

namespace CinemaApi.Models.Dto
{
    public class UserDto
    {
        [Required(ErrorMessage = "Email cannot be empty.")]       
        public string Email { get; set; }
        [Required(ErrorMessage = "Password cannot be empty.")]
        public string Password { get; set; }
    }
}
