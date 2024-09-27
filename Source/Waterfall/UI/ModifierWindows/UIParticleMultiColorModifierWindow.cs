﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall.UI
{
  public class UIParticleMultiColorModifierWindow : UIModifierWindow
  {

    private readonly Vector2 curveButtonDims = new(100f, 50f);

    private Texture2D gradientTexture1;
    private Texture2D gradientTexture2;

    private Gradient currentGradient1 = new();
    private float lower1 = 0f;
    private float upper1 = 1f;
    float gain2;

    public string paramName = "_TintColor";
    private int paramIndex;
    private string[] paramNames;
    public EffectParticleMultiColorModifier particleMod;

    protected readonly GradientUpdateFunction gradient1Function;
    public UIParticleMultiColorModifierWindow(EffectParticleMultiColorModifier mod, bool show) : base(mod, show)
    {
      particleMod = mod;
      paramName = particleMod.paramName;
      paramNames = ParticleUtils.FindValidParticlePropeties(particleMod.GetParticleSystem(), WaterfallParticlePropertyType.Color).ToArray();
      paramIndex = paramNames.ToList().FindIndex(x => x == particleMod.paramName);
      gradient1Function = UpdateGradient1;

      currentGradient1 = Utils.CreateGradientFromCurves(particleMod.c1RedCurve, particleMod.c1GreenCurve, particleMod.c1BlueCurve, particleMod.c1AlphaCurve, out lower1, out upper1);
      GenerateThumbs(mod);
    }

    public override void UpdateCurves(FloatCurve newCurve, string tag)
    {
      base.UpdateCurves(newCurve, tag);
    }

    /// <summary>
    ///   Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - Particle Color";
    }

    /// <summary>
    ///   Draws the modifier content
    /// </summary>
    protected override void DrawModifierPanel()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Particle Parameter Name", GUILayout.Width(hdrWidth));
      int selectedIndex = GUILayout.SelectionGrid(paramIndex, paramNames, 4);
      if (selectedIndex != paramIndex)
      {
        paramIndex = selectedIndex;
        paramName = paramNames[paramIndex];
        particleMod.ApplyParameterName(paramName);
      }
      GUILayout.EndHorizontal();

      GUILayout.Label($"<b>Selected Parameter Mode</b>: {particleMod.curveMode}");

      if (particleMod.curveMode == ParticleSystemGradientMode.Color)
      {

        GUILayout.BeginHorizontal();
        GUILayout.Label("Controller Value vs Color", GUILayout.Width(hdrWidth));
        GUILayout.FlexibleSpace();

        var buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y, GUILayout.Width(125));
        var imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
        if (GUI.Button(buttonRect, ""))
        {
          EditGradient(currentGradient1, gradient1Function, lower1, upper1);
        }
        GUI.DrawTexture(imageRect, gradientTexture1);
        GUILayout.EndHorizontal();

      }
      if (particleMod.curveMode == ParticleSystemGradientMode.TwoColors)
      {

      }
      if (particleMod.curveMode == ParticleSystemGradientMode.Gradient)
      {
        GUILayout.Label("Gradient mode parameters are not supported", GUILayout.Width(hdrWidth));
      }
      if (particleMod.curveMode == ParticleSystemGradientMode.TwoGradients)
      {
        GUILayout.Label("Two gradient mode parameters are not supported", GUILayout.Width(hdrWidth));
      }
    }
    protected void EditGradient(Gradient toEdit, GradientUpdateFunction updateFunction, float lower, float upper)
    {
      Utils.Log($"Started editing gradient {toEdit}", LogType.UI);
      WaterfallUI.Instance.OpenGradientEditor(toEdit, updateFunction, lower, upper);
    }
    protected override void UpdateModifierPanel()
    {
      GenerateThumbs(particleMod);
    }

    protected void UpdateGradient1(Gradient g, float lower, float upper)
    {
      currentGradient1 = g;
      lower1 = lower;
      upper1 = upper;
      Utils.CreateCurvesFromGradient(g, out particleMod.c1RedCurve, out particleMod.c1GreenCurve, out particleMod.c1BlueCurve, out particleMod.c1AlphaCurve, lower1, upper1);
      UpdateModifierPanel();
    }


    protected void GenerateThumbs(EffectParticleMultiColorModifier pMod)
    {

      gradientTexture1 = TextureUtils.GenerateGradientTexture(
        texWidth,
        texHeight,
        currentGradient1);

    }
  }
}