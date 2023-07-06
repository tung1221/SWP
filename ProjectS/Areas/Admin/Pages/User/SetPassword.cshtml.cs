﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project.Admin.User
{
	[Authorize(Roles = "Admin")]

	public class SetPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public SetPasswordModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

   
        [BindProperty]
        public InputModel Input { get; set; }

       
        [TempData]
        public string StatusMessage { get; set; }

  
        public class InputModel
        {
       
            [Required(ErrorMessage ="Phải nhập {0}")]
            [StringLength(100, ErrorMessage = " {0} phải dài từ  {2} đến {1} ký tự.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu mới")]
            public string NewPassword { get; set; }

        
            [DataType(DataType.Password)]
            [Display(Name = "Xác nhận mật khẩu")]
            [Compare("NewPassword", ErrorMessage = "Lặp lại mật khẩu không chính xác.")]
            public string ConfirmPassword { get; set; }
        }

        public IdentityUser user { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return NotFound($"Không có user");
            }
             user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không thấy user ID = '{id}'.");
            }

          
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
			if (string.IsNullOrEmpty(id))
			{
				return NotFound($"Không có user");
			}
			user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound($"Không thấy user ID = '{id}'.");
			}

			if (!ModelState.IsValid)
            {
                return Page();
            }
            await _userManager.RemovePasswordAsync(user);
           
            var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = $"Vừa cập hật mật khẩu cho user :{user.UserName}";

            return RedirectToPage("./Index");
        }
    }
}