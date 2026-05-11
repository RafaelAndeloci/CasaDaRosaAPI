using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.Entities.Addresses.Exceptions;

public sealed class StreetRequiredException()
    : DomainValidationException("address.street.required", "Street is required.");

public sealed class AddressNumberMustBeGreaterThanZeroException()
    : DomainValidationException("address.number.invalid", "Address number must be greater than zero.");

public sealed class AddressNumberTooLargeException()
    : DomainValidationException("address.number.invalid", "Address number cannot exceed 9 digits.");

public sealed class ZipCodeRequiredException()
    : DomainValidationException("address.zip_code.required", "Zip code is required.");

public sealed class ZipCodeInvalidFormatException()
    : DomainValidationException("address.zip_code.invalid_format", "Zip code must be in the format '00000-000'.");

public sealed class ZipCodeOnlyNumbersAndHyphenException()
    : DomainValidationException("address.zip_code.invalid_characters", "Zip code must contain only numbers and a hyphen.");

public sealed class ZipCodeEightDigitsRequiredException()
    : DomainValidationException("address.zip_code.invalid_length", "Zip code must have 8 numbers in order to format to '00000-000'.");

public sealed class AddressUfRequiredException()
    : DomainValidationException("address.uf.invalid", "State abbreviation is required.");

public sealed class AddressUfCodeInvalidException(string code)
    : DomainValidationException("address.uf.invalid", $"The UF code is invalid: {code}");
