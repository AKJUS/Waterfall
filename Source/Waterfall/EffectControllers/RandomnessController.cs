using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Waterfall
{
  /// <summary>
  ///   A controller that generates randomness
  /// </summary>
  [Serializable]
  [DisplayName("Randomness")]
  public class RandomnessController : WaterfallController
  {
    public delegate float NoiseFunction();

    public const string PerlinNoiseName = "perlin";
    public const string RandomNoiseName = "random";

    [Persistent] public Vector2 range;
    [Persistent] public string noiseType = RandomNoiseName;

    [Persistent] public bool randomSeed = true;
    [Persistent] public float seed;
    [Persistent] public float scale = 1f;
    [Persistent] public float minimum;
    [Persistent] public float speed = 1f;

    float speedX;
    float speedY;

    private NoiseFunction noiseFunc;

    public RandomnessController() : base() { }
    public RandomnessController(ConfigNode node) : base(node)
    {
      
    }


    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      values = new float[1];

      if (noiseType == PerlinNoiseName)
      {
        noiseFunc = PerlinNoise;
        if (randomSeed)
        { 
          // floats only have ~7 decimal digits of precision, so let's not waste too much on the seed
          seed = Random.Range(-1000f, 1000f);
        }

        // randomize the direction that we move through the noise texture
        float speedAngle = Random.Range(0f, 2f * Mathf.PI);
        speedX = Mathf.Cos(speedAngle) * speed;
        speedY = Mathf.Sin(speedAngle) * speed;
      }
      else if (noiseType == RandomNoiseName)
      {
        noiseFunc = RandomNoise;
      }
      else
        noiseFunc = RandomNoise;
    }

    public float RandomNoise() => Random.Range(range.x, range.y);

    public float PerlinNoise()
    {
      float time = Time.time;
      return Mathf.PerlinNoise(seed + Time.time * speedX, seed + Time.time * speedY) * (scale - minimum) + minimum;
    }

    protected override bool UpdateInternal()
    {
      values[0] = noiseFunc();
      return Settings.RandomControllersAwake;
    }
  }
}
