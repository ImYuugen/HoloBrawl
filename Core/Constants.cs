using System;

namespace HoloBrawl.Core;

public static class Constants
{
    // The amount of frames that must pass before the player can act after dashing (2_000_000 ticks = 200ms = 12 frames at 60fps)
    public static TimeSpan DashLag { get; } = new(2_000_000);
}