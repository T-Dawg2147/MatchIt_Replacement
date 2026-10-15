namespace MatchIt.MatchReplacement.Core.Normalization;

public sealed class PhoneticEncoder
{
    private readonly TextNormalizer _textNormalizer;

    public PhoneticEncoder(TextNormalizer textNormalizer)
    {
        _textNormalizer = textNormalizer;
    }

    public string Encode(string? value)
    {
        var input = _textNormalizer.RemoveDiacritics(value);
        input = _textNormalizer.NormalizeForComparison(input).Replace(" ", string.Empty);

        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        // Simple Soundex-style implementation as a first pass.
        // Can be swapped later for Double Metaphone if needed.
        var firstLetter = input[0];

        var encodedDigits = new List<char>(4);
        var previousDigit = MapChar(firstLetter);

        for (var i = 1; i < input.Length; i++)
        {
            var digit = MapChar(input[i]);

            if (digit == '0')
            {
                previousDigit = digit;
                continue;
            }

            if (digit != previousDigit)
            {
                encodedDigits.Add(digit);
            }

            previousDigit = digit;

            if (encodedDigits.Count == 3)
            {
                break;
            }
        }

        while (encodedDigits.Count < 3)
        {
            encodedDigits.Add('0');
        }

        return $"{firstLetter}{new string(encodedDigits.ToArray())}";
    }

    private static char MapChar(char c)
    {
        return c switch
        {
            'B' or 'F' or 'P' or 'V' => '1',
            'C' or 'G' or 'J' or 'K' or 'Q' or 'S' or 'X' or 'Z' => '2',
            'D' or 'T' => '3',
            'L' => '4',
            'M' or 'N' => '5',
            'R' => '6',
            _ => '0'
        };
    }
}