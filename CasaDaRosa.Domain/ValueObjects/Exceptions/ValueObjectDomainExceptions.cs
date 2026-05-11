using CasaDaRosa.Domain.Exceptions;

namespace CasaDaRosa.Domain.ValueObjects.Exceptions;

public sealed class CurrencyCodeInvalidException(string code)
    : DomainValidationException("currency.invalid_code", $"The currency code is invalid: {code}");

public sealed class CurrencyMismatchException()
    : DomainValidationException("money.currency_mismatch", "Currencies have to be equal");
