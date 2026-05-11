using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CasaDaRosa.Domain.Entities.Addresses;

public record Street
{
    public string Value { get; private set; }
    private Street() { }

    public static Street Create(string value)
    {
        if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("Cannot create a street without a value.");
        return new Street() with { Value = value };
    }

    public override string ToString()
    {
        return Value;
    }
}
