using System;
using UnityEngine;

[Serializable]
public class BigNumber : IComparable<BigNumber>
{
    public double mantissa; // 항상 양수 (1 <= m < 10)
    public long exponent;
    public int sign; // 부호 (1, -1, 0)

    private const double ComparisonEpsilon = 1e-9;

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

        // 극단적인 underflow 방지
        if (exponent < -400)
        {
            mantissa = 0;
            exponent = 0;
            sign = 0;
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
    
    // 나눗셈
    public static BigNumber operator /(BigNumber a, BigNumber b)
    {
        if (b.sign == 0)
            throw new DivideByZeroException("0으로 나눌 수 없습니다! (Divide by zero)");

        if (a.sign == 0)
            return new BigNumber(0);

        double newMantissa = a.mantissa / b.mantissa;
        long newExponent = a.exponent - b.exponent;
        int newSign = a.sign * b.sign;  // 나눗셈은 부호 반대

        // Normalize()가 생성자에서 자동 호출되므로 안전
        return new BigNumber(newMantissa, newExponent, newSign);
    }
    // 비교
    public int CompareTo(BigNumber other)
    {
        if (other == null) return 1;

        if (sign != other.sign)
            return sign.CompareTo(other.sign);

        if (sign == 0) return 0;

        if (exponent != other.exponent)
            return exponent.CompareTo(other.exponent) * sign;

        double diff = mantissa - other.mantissa;
        if (Math.Abs(diff) < ComparisonEpsilon)
            return 0;

        return diff > 0 ? sign : -sign;
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

        return Math.Abs(a.mantissa - b.mantissa) < ComparisonEpsilon;
    }

    public static bool operator !=(BigNumber a, BigNumber b) => !(a == b);

    public double ToDoubleSafe()
    {
        if (sign == 0) return 0.0;

        // double이 표현 가능한 범위 대략 체크
        if (exponent > 308)
        {
            return double.PositiveInfinity * sign;  // 또는 그냥 double.MaxValue * sign 써도 됨
        }
        if (exponent < -308)
        {
            return 0.0;  // 아주 작은 값은 0으로 취급 
        }

        // 정상 범위 -> 안전하게 계산
        double value = mantissa * Math.Pow(10.0, exponent);
        return sign * value;
    }
    public override bool Equals(object obj)
    {
        return obj is BigNumber other && this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(sign, mantissa, exponent);
    }
}