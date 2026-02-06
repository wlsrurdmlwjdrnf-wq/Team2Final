using System;
using UnityEngine;

[System.Serializable]
public class BigNumber : IComparable<BigNumber>
{
    public double mantissa; // 항상 양수 (1 <= m < 10)
    public long exponent;
    public int sign; // 부호 (1, -1, 0)

    // 생성자들
    public BigNumber() : this(0.0) { }

    public BigNumber(double value)
    {
        Set(value);
    }

    public BigNumber(double m, long e, int s = 1)
    {
        mantissa = Math.Abs(m);
        exponent = e;
        sign = (m == 0) ? 0 : (m > 0 ? 1 : -1) * Math.Sign(s); // 부호 결정
        Normalize();
    }

    private void Set(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentException("Invalid value: NaN or Infinity");

        if (value == 0)
        {
            mantissa = 0;
            exponent = 0;
            sign = 0;
            return;
        }

        sign = Math.Sign(value);
        double absValue = Math.Abs(value);
        exponent = (long)Math.Floor(Math.Log10(absValue));
        mantissa = absValue / Math.Pow(10, exponent);
        Normalize();
    }

    private void Normalize()
    {
        if (mantissa == 0)
        {
            exponent = 0;
            sign = 0;
            return;
        }

        // mantissa를 1 <= m < 10으로 조정 (양수만)
        while (mantissa >= 10.0)
        {
            mantissa /= 10.0;
            exponent++;
        }
        while (mantissa < 1.0)
        {
            mantissa *= 10.0;
            exponent--;
        }
    }

    // 덧셈
    public static BigNumber operator +(BigNumber a, BigNumber b)
    {
        if (a.sign == 0) return b;
        if (b.sign == 0) return a;

        long expDiff = a.exponent - b.exponent;
        double adjustedA = a.mantissa * a.sign;
        double adjustedB = b.mantissa * b.sign;

        if (Math.Abs(expDiff) > 308) // double Pow overflow 방지
        {
            return Math.Abs(expDiff) > 15 ? (expDiff > 0 ? a : b) : null; // 차이 크면 큰 쪽 반환 (정밀도 한계)
        }

        if (a.exponent > b.exponent)
        {
            double diff = Math.Pow(10, expDiff);
            return new BigNumber(adjustedA + adjustedB / diff, a.exponent);
        }
        else if (b.exponent > a.exponent)
        {
            double diff = Math.Pow(10, -expDiff);
            return new BigNumber(adjustedB + adjustedA / diff, b.exponent);
        }
        else
        {
            return new BigNumber(adjustedA + adjustedB, a.exponent);
        }
    }

    // 뺄셈
    public static BigNumber operator -(BigNumber a, BigNumber b)
    {
        return a + Negate(b);
    }

    private static BigNumber Negate(BigNumber n)
    {
        if (n.sign == 0) return new BigNumber(0);
        return new BigNumber(n.mantissa, n.exponent, -n.sign);
    }

    // 곱셈
    public static BigNumber operator *(BigNumber a, BigNumber b)
    {
        if (a.sign == 0 || b.sign == 0) return new BigNumber(0);
        return new BigNumber(a.mantissa * b.mantissa, a.exponent + b.exponent, a.sign * b.sign);
    }

    // 비교
    public int CompareTo(BigNumber other)
    {
        if (other == null) return 1;
        if (sign != other.sign) return sign.CompareTo(other.sign); // 부호 먼저 (양 > 0 > 음)

        if (sign == 0) return 0; // 둘 다 0

        long expDiff = exponent - other.exponent;

        if (expDiff > 1) return sign; // this가 더 큼 (sign=1 or -1)
        if (expDiff < -1) return -sign; // other가 더 큼

        double thisAdj = mantissa;
        double otherAdj = other.mantissa;

        if (expDiff == 1) otherAdj *= 0.1; // other 보정
        else if (expDiff == -1) thisAdj *= 0.1; // this 보정

        int cmp = thisAdj.CompareTo(otherAdj);
        return sign * cmp; // 부호 고려 (음수면 반대)
    }

    public static bool operator >(BigNumber a, BigNumber b) => a.CompareTo(b) > 0;
    public static bool operator <(BigNumber a, BigNumber b) => a.CompareTo(b) < 0;
    public static bool operator >=(BigNumber a, BigNumber b) => a.CompareTo(b) >= 0;
    public static bool operator <=(BigNumber a, BigNumber b) => a.CompareTo(b) <= 0;

    public static bool operator ==(BigNumber a, BigNumber b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        if (a.sign != b.sign) return false;
        if (a.exponent != b.exponent) return false;
        return Math.Abs(a.mantissa - b.mantissa) < 1e-10;
    }

    public static bool operator !=(BigNumber a, BigNumber b) => !(a == b);

    public override bool Equals(object obj)
    {
        return obj is BigNumber other && this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(sign, mantissa, exponent);
    }
}