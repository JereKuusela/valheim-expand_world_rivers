using System.Collections.Generic;
using HarmonyLib;
namespace ExpandWorldRivers;

[HarmonyPatch(typeof(WorldGenerator), nameof(WorldGenerator.FindLakes))]
public class FindLakes
{
  public static bool Prefix(WorldGenerator __instance)
  {
    __instance.m_lakes = [];
    return false;
  }
  static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
  {
    CodeMatcher matcher = new(instructions);
    matcher = Helper.Replace(matcher, 0.05f, Helper.AltitudeToBaseHeight(Configuration.LakeDepth));
    matcher = Helper.Replace(matcher, 128d, Configuration.LakeSearchInterval);
    matcher = Helper.Replace(matcher, 128d, Configuration.LakeSearchInterval);
    matcher = Helper.Replace(matcher, 800f, Configuration.LakeMergeRadius);
    return matcher.InstructionEnumeration();
  }
}
[HarmonyPatch(typeof(WorldGenerator), nameof(WorldGenerator.PlaceRivers))]
public class PlaceRivers
{
  public static bool Prefix(ref List<WorldGenerator.River> __result)
  {
    __result = new();
    return false;
  }

  static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
  {
    CodeMatcher matcher = new(instructions);
    if (Configuration.RiverSeed != null)
      matcher = Helper.ReplaceSeed(matcher, nameof(WorldGenerator.m_riverSeed), Configuration.RiverSeed.Value);
    matcher = Helper.Replace(matcher, 2000f, Configuration.LakeMaxDistance1);
    matcher = Helper.Replace(matcher, 0.4f, Helper.AltitudeToBaseHeight(Configuration.RiverMaxAltitude));
    matcher = Helper.Replace(matcher, 128f, Configuration.RiverCheckInterval);
    matcher = Helper.Replace(matcher, 5000f, Configuration.LakeMaxDistance2);
    matcher = Helper.Replace(matcher, 0.4f, Helper.AltitudeToBaseHeight(Configuration.RiverMaxAltitude));
    matcher = Helper.Replace(matcher, 128f, Configuration.RiverCheckInterval);
    matcher = Helper.Replace(matcher, 60f, Configuration.RiverMinWidth);
    matcher = Helper.Replace(matcher, 100f, Configuration.RiverMaxWidth);
    matcher = Helper.Replace(matcher, 60f, Configuration.RiverMinWidth);
    matcher = Helper.Replace(matcher, 15d, Configuration.RiverCurveWidth);
    matcher = Helper.Replace(matcher, 20d, Configuration.RiverCurveWaveLength);
    return matcher.InstructionEnumeration();
  }
}


[HarmonyPatch(typeof(WorldGenerator), nameof(WorldGenerator.IsRiverAllowed))]
public class IsRiverAllowed
{
  static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
  {
    CodeMatcher matcher = new(instructions);
    matcher = Helper.Replace(matcher, 0.05f, Helper.AltitudeToBaseHeight(Configuration.LakeDepth));
    return matcher.InstructionEnumeration();
  }
}

[HarmonyPatch(typeof(WorldGenerator), nameof(WorldGenerator.PlaceStreams))]
public class PlaceStreams
{
  public static bool Prefix(ref List<WorldGenerator.River> __result)
  {
    __result = new();
    return false;
  }
  static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
  {
    CodeMatcher matcher = new(instructions);
    if (Configuration.StreamSeed != null)
      matcher = Helper.ReplaceSeed(matcher, nameof(WorldGenerator.m_streamSeed), Configuration.StreamSeed.Value);
    matcher = Helper.Replace(matcher, (sbyte)100, Configuration.StreamSearchIterations);
    matcher = Helper.Replace(matcher, 26f, Helper.AltitudeToHeight(Configuration.StreamStartMinAltitude));
    matcher = Helper.Replace(matcher, 31f, Helper.AltitudeToHeight(Configuration.StreamStartMaxAltitude));
    matcher = Helper.Replace(matcher, (sbyte)100, Configuration.StreamSearchIterations);
    matcher = Helper.Replace(matcher, 36f, Helper.AltitudeToHeight(Configuration.StreamEndMinAltitude));
    matcher = Helper.Replace(matcher, 44f, Helper.AltitudeToHeight(Configuration.StreamEndMaxAltitude));
    matcher = Helper.Replace(matcher, 80f, Configuration.StreamMinLength);
    matcher = Helper.Replace(matcher, 200f, Configuration.StreamMaxLength);
    matcher = Helper.Replace(matcher, 26f, Helper.AltitudeToHeight(Configuration.StreamStartMinAltitude));
    matcher = Helper.Replace(matcher, 44f, Helper.AltitudeToHeight(Configuration.StreamEndMaxAltitude));
    matcher = Helper.Replace(matcher, 20f, Configuration.StreamMaxWidth);
    matcher = Helper.Replace(matcher, 15d, Configuration.StreamCurveWidth);
    matcher = Helper.Replace(matcher, 20d, Configuration.StreamCurveWaveLength);
    matcher = Helper.Replace(matcher, 3000, Configuration.StreamMaxAmount ?? 3000);
    return matcher.InstructionEnumeration();
  }
}
