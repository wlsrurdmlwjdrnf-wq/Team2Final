using System;
using System.Numerics;
public static class BigNumberFormatter
{
    // 약어 배열 천 단위로
    private static readonly string[] suffixes = new string[]
    {
        "", "K", "M", "B", "T",
        "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj",
        "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at",
        "au", "av", "aw", "ax", "ay", "az",
        "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj",
        "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt",
        "bu", "bv", "bw", "bx", "by", "bz"
    };

    // 1000이상 다 과학적 표기
    public static string ToString(BigNumber bn)
    {
        if (bn.mantissa == 0) return "0";

        if (bn.exponent >= 0 && bn.exponent <= 3)
        {
            double value = bn.mantissa * Math.Pow(10, bn.exponent);
            return value.ToString("N2");
        }

        return $"{bn.mantissa:F2}e{bn.exponent}";
    }

    // 천단위 약어 포맷팅
    public static string ToFormatted(BigNumber bn)
    {
        if (bn.mantissa == 0) return "0";

        bool isNegative = bn.sign < 0;
        double absMantissa = Math.Abs(bn.mantissa);

        // exponent 0~3 -> 일반 숫자
        if (bn.exponent >= 0 && bn.exponent <= 3)
        {
            double value = absMantissa * Math.Pow(10, bn.exponent);
            string formatted = value.ToString("N2");
            formatted = CleanDecimal(formatted);
            return isNegative ? "-" + formatted : formatted;
        }

        // 천단위 약어 적용
        long powerIndex = bn.exponent / 3;
        double displayMantissa = absMantissa * Math.Pow(10, bn.exponent % 3);

        string suffix = powerIndex >= 0 && powerIndex < suffixes.Length
            ? suffixes[powerIndex]
            : "";  // 부족하면 빈 문자열 (또는 과학적 표기 fallback)

        string numberPart = displayMantissa.ToString("0.##");
        numberPart = CleanDecimal(numberPart);

        string result = numberPart + suffix;
        return isNegative ? "-" + result : result;
    }

    // 오버로딩
    public static string ToFormatted(float value)
    {
        if (value == 0) return "0";

        bool isNegative = value < 0;
        double absValue = Math.Abs(value);

        // 1000 미만 -> 일반 숫자
        if (absValue < 1000)
        {
            string formatted = absValue.ToString("N2");
            formatted = CleanDecimal(formatted);
            return isNegative ? "-" + formatted : formatted;
        }

        // 천단위 약어 적용
        int powerIndex = (int)(Math.Floor(Math.Log10(absValue)) / 3);
        double displayValue = absValue / Math.Pow(1000, powerIndex);

        string suffix = powerIndex >= 0 && powerIndex < suffixes.Length
            ? suffixes[powerIndex]
            : "";

        string numberPart = displayValue.ToString("0.##");
        numberPart = CleanDecimal(numberPart);

        string result = numberPart + suffix;
        return isNegative ? "-" + result : result;
    }

    // 소수점 정리 헬퍼
    private static string CleanDecimal(string s)
    {
        if (s.EndsWith(".00")) return s[..^3];
        if (s.EndsWith(".0")) return s[..^2];
        return s;
    }
}
