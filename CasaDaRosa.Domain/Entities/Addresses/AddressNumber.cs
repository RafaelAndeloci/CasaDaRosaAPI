using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasaDaRosa.Domain.Entities.Addresses;

public record AddressNumber
{
    public short Value { get; private set; }

    private AddressNumber() { }

    public static AddressNumber Create(short value)
    {
        if (value <= 0) throw new ArgumentException("Address number must be greater than zero.");
        if (value.ToString().Length > 9) throw new ArgumentException("Address number cannot exceed 9 digits.");
        return new AddressNumber() with { Value = value };
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
