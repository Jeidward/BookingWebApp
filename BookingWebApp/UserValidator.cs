using System;
using System.Collections.Generic;
using Models.Entities;

public static class UserValidator
{
    public static DomainValidationResult ValidUser(string email, string password, string name)
    {
        var validationResult = new DomainValidationResult();
        if (string.IsNullOrWhiteSpace(email))
        {
            validationResult.Errors.Add("Email is required.");
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            validationResult.Errors.Add("password cannot be empty");
        }
        else if (password.Length < 6)
        {
            validationResult.Errors.Add("Password must be at least 6 characters long.");
        }
        return validationResult;
    }
}
