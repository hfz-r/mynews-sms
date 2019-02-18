﻿using FluentValidation;
using StockManagementSystem.Models.Install;
using StockManagementSystem.Web.Validators;

namespace StockManagementSystem.Validators.Install
{
    public class InstallValidator : BaseValidator<InstallModel>
    {
        public InstallValidator()
        {
            RuleFor(x => x.AdminEmail).NotEmpty().WithMessage("Enter admin email");
            RuleFor(x => x.AdminEmail).EmailAddress();

            RuleFor(x => x.AdminPassword).NotEmpty().WithMessage("Enter admin password");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Enter confirm password");
            RuleFor(x => x.AdminPassword).Equal(x => x.ConfirmPassword).WithMessage("The passwords do not match");

            RuleFor(x => x.DataProvider).NotEmpty().WithMessage("Select data provider");
        }
    }
}