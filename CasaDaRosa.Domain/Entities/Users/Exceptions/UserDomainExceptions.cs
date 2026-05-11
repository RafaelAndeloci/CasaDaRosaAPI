using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.Entities.Users.Exceptions;

public sealed class UserPasswordRequiredException()
    : DomainValidationException("user.password.invalid", "User password hash is required.");

public sealed class UserAddressRequiredException()
    : DomainValidationException("user.address.required", "Address is required.");

public sealed class UserAddressLimitExceededException()
    : DomainValidationException("user.address.limit_exceeded", "A user cannot have more than 5 addresses.");

public sealed class UserAddressDuplicateException()
    : DomainValidationException("user.address.duplicate", "This address is already assigned to the user.");

public sealed class EmailRequiredException()
    : DomainValidationException("user.email.required", "Email is required.");

public sealed class InvalidEmailFormatException()
    : DomainValidationException("user.email.invalid_format", "Invalid email format.");

public sealed class InvalidPhoneNumberFormatException()
    : DomainValidationException("user.phone_number.invalid_format", "Invalid phone number format. Expected format: +CC (AC) VALUE (e.g +55 16 9123-1234)");

public sealed class UserNameRequiredException()
    : DomainValidationException("user.name.required", "User full name is required.");

public sealed class UserNameInvalidException()
    : DomainValidationException("user.name.invalid", "User full name must contain at least a first name and a surname.");
