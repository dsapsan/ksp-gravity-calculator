﻿using System.Collections.Generic;
using System.Linq;

public abstract class IteratorBase
{
    private const float mMaximumCostExcessMultiplayer = 1.25f;
    private const float mMaximumCostExcessAddition = 1000f;
    private const float mMinimumFirstStageDeltaV = 0.15f;
    private const int mMaximumAssembliesOnScreen = 30;

    protected const int mMaximumEnginesPerStage = 9;

    protected Decoupler mRadialDecoupler;
    protected Decoupler mStraightDecoupler;

    public float Payload { get; set; } = 1000f;
    public List<Technology> Technologies { get; set; } = new List<Technology>();
    public bool UseAllTechnologies { get; set; } = true;

    public List<IEngineAssembly> Assemblies { get; private set; } = new List<IEngineAssembly>();
    public float BestCost { get; set; }

    public void Calculate()
    {
        //TODO Расчёт двигателей для полезной нагрузки в космосе
        //TODO Фильтр по технологиям
        //TODO Сохранения фильтра в реестр

        Assemblies.Clear();
        BestCost = float.PositiveInfinity;

        if (UseAllTechnologies)
            Technologies = Technology.GetAll().ToList();

        mRadialDecoupler = Part.GetAll<Decoupler>().FirstOrDefault(p => p.Alias == "TT-70");
        mStraightDecoupler = Part.GetAll<Decoupler>().FirstOrDefault(p => p.Alias == "TD-12");

        FillAssembliesList();

        Assemblies = Assemblies.Where(IsCheep).OrderBy(set => set.Cost).Take(mMaximumAssembliesOnScreen).ToList();
    }

    protected abstract void FillAssembliesList();

    protected bool IsCheep(IEngineAssembly assembly)
    {
        return assembly.Cost < BestCost * mMaximumCostExcessMultiplayer + mMaximumCostExcessAddition;
    }

    protected bool IsDeltaVEnough(float firstStageDeltaV, float allStagesDeltaV)
    {
        return firstStageDeltaV > Constants.MinKerbinDeltaV * mMinimumFirstStageDeltaV && allStagesDeltaV > Constants.MinKerbinDeltaV;
    }
}