// The Sōla need womter

using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using StavrasXenotypes;

public class Need_Water : Need {

    public const float BaseHumidityFallPerTick = 3E-06f;
    private const float BaseDehydrationSeverityPerInterval = 0.00225f;

    public bool Dessicated => CurCategory == HydrationCategory.Dessicated;

    public const float ThresholdDessicated = 0.01f;
    public const float ThresholdDehydrated = 0.10f;
    public const float ThresholdParched = 0.20f;
    public const float ThresholdDry = 0.30f;

    //Create dehydration hediff
    stavrasXenotypes_Dehydration = HediffMaker.MadeHediff()

    public HydrationCategory CurCategory {
        get {
            //Note to self: convert this to switch statement aaaa ;w;
            if(CurLevel < 0.01f) {
                return HydrationCategory.Dessicated;
            }

            if(CurLevel < 0.10f) {
                return HydrationCategory.Dehydrated;
            }

            if(CurLevel < 0.20f) {
                return HydrationCategory.Parched;
            }

            if(CurLevel < 0.30f) {
                return HydrationCategory.Dry;
            }
            return HydrationCategory.Hydrated;
        }
    }

    public float BiomeHumidityFallFactor => pawn.Map.Biome switch {
        //Vanilla
        AridShrubland => 1.2f,
        BorealForest => 0.975f,
        ColdBog => 0.69f, //ayyyyy
        Desert => 1.75f,
        ExtremeDesert => 2.0f,
        IceSheet => 1.05f,
        SeaIce => 1.25f, //In real life, Antarctica is a desert. Sea Ice would most likely be a very dry biome due to how cold it is.
        TemperateForest => 1.0f,
        TemperateSwamp => 0.7f,
        TropicalRainforest => 0.6f,
        TropicalSwamp => 0.5f,
        Tundra => 0.9125f,

        //Alpha Biomes
        AB_FeraliskInfestedJungle => 0.6f,
        AB_GallatrossGraveyard => 1.2f,
        AB_GelatinousSuperorganism => 0.95f,
        AB_IdyllicMeadows => 0.9f,
        AB_MechanoidIntrusion => 1.25f,
        AB_MiasmicMangrove => 0.49f,
        AB_MycoticJungle => 0.4f,
        AB_OcularForest => 0.9f,
        AB_PropaneLakes => 0.85f,
        AB_PyroclasticConflagration => 2.6f,
        AB_RockyCrags => 1.4f,
        AB_TarPits = 1.5f,
        //Default case; If a modded biome isn't supported yet, it will be treated as a Temperate Forest.
        _ => 1.0f,
    };

    public float TerrainHumidityRiseFactor => map.terrainGrid.TerrainAt() switch {
        WaterMovingChestDeep => 0.0015f,
        WaterShallow => 0.001f,
        WaterMovingShallow => 0.001f,
        WaterOceanShallow => 0.001f,
        Marsh => 0.00005f,
        _ => 0.0f,
    }

    //Whether pawn is standing in rain
    public bool IsInRain() {
        if (pawn.Position.GetRoof(pawn.Map) == null && building.Map.weatherManager.RainRate > 0.01f) {
            return true;
        }
        else {
            return false;
        }
    }

    //Whether need is increasing or decreasing
    public bool IsHydrating() {
        if(TerrainHumidityRiseFactor > 0 || IsInRain()) {
            return true;
        }
    }

    //Initial level range
    public override void SetInitialLevel() {
		CurLevel = Rand.Range(0.5f, 0.95f);
	}

	public override void NeedInterval() {
        if(!IsFrozen) {
            if(IsHydrating()) {
                CurLevel += (TerrainHumidityRiseFactor + (building.Map.weatherManager.RainRate * 1.5E-5f)) * 150.0f;
            }
            else {
                CurLevel -= BaseHumidityFallPerTick * 150.0f * BiomeHumidityFallFactor;
            }

            if (Dessicated) {
                HealthUtility.AdjustSeverty(pawn, HediffDefOf.StavrasXenotypes_Dehydration, BaseDehydrationSeverityPerInterval);
            }
            else {
                HealthUtility.AdjustSeverty(pawn, HediffDefOf.StavrasXenotypes_Dehydration, 0f - BaseDehydrationSeverityPerInterval);
            }
        }
    }
}
