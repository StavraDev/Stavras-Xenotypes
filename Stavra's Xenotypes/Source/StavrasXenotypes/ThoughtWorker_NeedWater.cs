using StavrasXenotypes;
using System;
using RimWorld;
using Verse;

public class ThoughtWorker_NeedWater:ThoughtWorker {

    private int dessicationProgress;
    private int stageToReturn;

    protected override ThoughtState CurrentStateInternal(pawn p) {
        if(p.needs.TryGetNeed<Need_Water>() == null) {
            return ThoughtState.Inactive;
        }
        switch(p.needs.TryGetNeed<Need_Water>().curCategory ?? 0) {
            case HydrationCategory.Hydrated:
                return ThoughtState.Inactive;
            case HydrationCategory.Dry:
                return ThoughtState.ActiveAtStage(0);
            case HydrationCategory.Parched:
                return ThoughtState.ActiveAtStage(1);
            case HydrationCategory.Dehydrated:
                return ThoughtState.ActiveAtStage(2);
            case HydrationCategory.Dessicated: {
                dessicationProgress = p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.StavrasXenotypes_Dessication)?.CurStageIndex ?? 0;
                stageToReturn = 3 + dessicationProgress;
                if(stageToReturn > 4) {
                    stageToReturn = 4;
                }
                return ActiveAtStage(stageToReturn);
            }
            default:
                throw new NotImplementedException();
        }
    }
}
