﻿using System;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Service;

namespace ExpandWorldRivers;
[BepInPlugin(GUID, NAME, VERSION)]
[BepInIncompatibility("expand_world")]
public class EWR : BaseUnityPlugin
{
  public const string GUID = "expand_world_rivers";
  public const string NAME = "Expand World Rivers";
  public const string VERSION = "1.10";
#nullable disable
  public static ManualLogSource Log;
  public static Harmony harmony;
#nullable enable
  public static ServerSync.ConfigSync ConfigSync = new(GUID)
  {
    DisplayName = NAME,
    CurrentVersion = VERSION,
    ModRequired = true,
    IsLocked = true
  };
  public static string ConfigName = $"{GUID}.cfg";
  public static bool NeedsMigration = File.Exists(Path.Combine(Paths.ConfigPath, "expand_world.cfg")) && !File.Exists(Path.Combine(Paths.ConfigPath, ConfigName));
  public void Awake()
  {
    Log = Logger;
    ConfigWrapper wrapper = new(Config, ConfigSync, InvokeRegenerate);
    Configuration.Init(wrapper);
    if (NeedsMigration)
      MigrateOldConfig();
    Patcher.Init(new(GUID));
    try
    {
      SetupWatcher();
    }
    catch (Exception e)
    {
      Log.LogError(e);
    }
  }
  private void MigrateOldConfig()
  {
    Log.LogWarning("Migrating old config file.");
    Config.Save();
    var from = File.ReadAllLines(Path.Combine(Paths.ConfigPath, "expand_world.cfg"));
    var to = File.ReadAllLines(Path.Combine(Paths.ConfigPath, ConfigName));
    foreach (var line in from)
    {
      var split = line.Split('=');
      if (split.Length != 2) continue;
      for (var i = 0; i < to.Length; i++)
      {
        if (to[i].StartsWith(split[0]))
          to[i] = line;
      }
    }
    File.WriteAllLines(Path.Combine(Paths.ConfigPath, ConfigName), to);
    Config.Reload();
  }
  public void InvokeRegenerate()
  {
    if (Patcher.IsMenu) return;
    // Debounced for smooth config editing.
    CancelInvoke("Regenerate");
    Invoke("Regenerate", 1.0f);
  }
  public void Regenerate() => Generate.World();
  public void Start()
  {
    EWS.Run();
    EWD.Run();
  }
#pragma warning disable IDE0051
  private void OnDestroy()
  {
    Config.Save();
  }
#pragma warning restore IDE0051

  private void SetupWatcher()
  {
    FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigName);
    watcher.Changed += ReadConfigValues;
    watcher.Created += ReadConfigValues;
    watcher.Renamed += ReadConfigValues;
    watcher.IncludeSubdirectories = true;
    watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
    watcher.EnableRaisingEvents = true;
  }
  private void ReadConfigValues(object sender, FileSystemEventArgs e)
  {
    if (!File.Exists(Config.ConfigFilePath)) return;
    try
    {
      Config.Reload();
    }
    catch
    {
      Log.LogError($"There was an issue loading your {Config.ConfigFilePath}");
      Log.LogError("Please check your config entries for spelling and format!");
    }
  }
}
