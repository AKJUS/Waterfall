﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Waterfall
{
  /// <summary>
  ///   A controller that pulls from the current engine's thrust. Returns a fractional thrust value
  ///   normalized to [0, 1] where 1 corresponds to the max thrust possible under current conditions.
  /// </summary>
  [Serializable]
  [DisplayName("Thrust")]
  public class ThrustController : WaterfallController
  {
    public  string        engineID = String.Empty;
    public  float         currentThrustFraction;
    private ModuleEngines engineController;

    public ThrustController() { }

    public ThrustController(ConfigNode node)
    {
      node.TryGetValue(nameof(name),     ref name);
      node.TryGetValue(nameof(engineID), ref engineID);
    }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      engineController = host.GetComponents<ModuleEngines>().ToList().Find(x => x.engineID == engineID);
      if (engineController == null)
      {
        Utils.Log($"[ThrustController] Could not find engine ID {engineID}, using first module");
        engineController = host.GetComponent<ModuleEngines>();
      }

      if (engineController == null)
        Utils.LogError("[ThrustController] Could not find engine controller on Initialize");
    }

    public override ConfigNode Save()
    {
      var c = base.Save();

      c.AddValue(nameof(engineID), engineID);
      return c;
    }

    public override List<float> Get()
    {
      if (overridden)
        return new() { overrideValue };

      if (engineController == null)
      {
        Utils.LogWarning("[ThrustController] Engine controller not assigned");
        return new() { 0f };
      }

      if (!engineController.isOperational)
      {
        currentThrustFraction = 0f;
      }
      else
      {
        // Thanks to NathanKell for the formula.
        currentThrustFraction = engineController.fuelFlowGui
                              / engineController.maxFuelFlow
                              / (float)engineController.ratioSum
                              * engineController.mixtureDensity
                              * engineController.multIsp;
      }

      return new() { currentThrustFraction };
    }
  }
}