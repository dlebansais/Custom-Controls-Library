namespace ExtendedTreeView.Demo;

using System;

/// <summary>
/// Represents a random number generator.
/// </summary>
public class RandomNumberGenerator
{
    /// <summary>
    /// Gets or sets the seed.
    /// </summary>
    public int Seed
    {
        get => SeedInternal;
        set
        {
            SeedInternal = value;
            N1 = 0;
            N2 = 0;
        }
    }

    /// <summary>
    /// Gets the next random number.
    /// </summary>
    /// <param name="max">The maximum value of the number range (from 0 to <paramref name="max"/>-1).</param>
    public int Next(int max)
    {
        uint n = GetUint();
        return (int)(n % (uint)max);
    }

    private uint GetUint()
    {
        N1 = (36969 * (N1 & 65535)) + (N1 >> 16);
        N2 = (18000 * (N2 & 65535)) + (N2 >> 16);
        return (N1 << 16) + N2;
    }

    private uint N1;
    private uint N2;
    private int SeedInternal;
}
