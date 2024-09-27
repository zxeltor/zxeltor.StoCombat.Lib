// Copyright (c) 2024, Todd Taylor (https://github.com/zxeltor)
// All rights reserved.
// 
// This source code is licensed under the Apache-2.0-style license found in the
// LICENSE file in the root directory of this source tree.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using zxeltor.StoCombat.Lib.Model.CombatMap;

namespace zxeltor.StoCombat.Lib.Parser;

public class CombatLogParseSettings : INotifyPropertyChanged
{
    #region Private Fields

    private int _combatDurationPercentage = 1;
    private int _combatEventCountMinimum = 4;
    private string? _combatLogPath;
    private string _combatLogPathFilePattern = "combatlog*.log";
    private int _combineCombatMaxTimeSeconds = 180;
    private int _howFarBackForCombatInHours = 24;
    private int _howLongBeforeNewCombatInSeconds = 20;
    private bool _isCombinePets = true;
    private bool _isCombineSimilarCombatInstances = true;
    private bool _isDisplayRejectedParserItemsInUi;
    private bool _isEnableInactiveTimeCalculations = true;
    private bool _isEnforceCombatEventMinimum;
    private bool _isEnforceMapMaxPlayerCount = true;
    private bool _isEnforceMapMinPlayerCount = true;
    private bool _isRejectCombatIfUserPlayerNotIncluded = true;
    private bool _isRejectCombatWithNoPlayers = true;
    private bool _isRemoveEntityOutliers = true;
    private bool _isRemoveEntityOutliersNonPlayers;
    private bool _isRemoveEntityOutliersPlayers;
    private CombatMapDetectionSettings? _mapDetectionSettings;
    private int _minInActiveInSeconds = 4;
    private string? _myCharacter;
    private TimeSpan _timeSpanMaxBetweenCombatForCombine = TimeSpan.FromMinutes(3);

    #endregion

    #region Constructors

    /// <summary>
    ///     Default constructor.
    /// </summary>
    public CombatLogParseSettings()
    {
    }
    
    #endregion

    #region Public Properties

    public bool IsCombinePets
    {
        get => this._isCombinePets;
        set => this.SetField(ref this._isCombinePets, value);
    }

    public bool IsCombineSimilarCombatInstances
    {
        get => this._isCombineSimilarCombatInstances;
        set => this.SetField(ref this._isCombineSimilarCombatInstances, value);
    }

    public bool IsDisplayRejectedParserItemsInUi
    {
        get => this._isDisplayRejectedParserItemsInUi;
        set => this.SetField(ref this._isDisplayRejectedParserItemsInUi, value);
    }

    public bool IsEnableInactiveTimeCalculations
    {
        get => this._isEnableInactiveTimeCalculations;
        set => this.SetField(ref this._isEnableInactiveTimeCalculations, value);
    }

    public bool IsEnforceCombatEventMinimum
    {
        get => this._isEnforceCombatEventMinimum;
        set => this.SetField(ref this._isEnforceCombatEventMinimum, value);
    }

    public bool IsEnforceMapMaxPlayerCount
    {
        get => this._isEnforceMapMaxPlayerCount;
        set => this.SetField(ref this._isEnforceMapMaxPlayerCount, value);
    }

    public bool IsEnforceMapMinPlayerCount
    {
        get => this._isEnforceMapMinPlayerCount;
        set => this.SetField(ref this._isEnforceMapMinPlayerCount, value);
    }

    public bool IsRejectCombatIfUserPlayerNotIncluded
    {
        get => this._isRejectCombatIfUserPlayerNotIncluded;
        set => this.SetField(ref this._isRejectCombatIfUserPlayerNotIncluded, value);
    }

    public bool IsRejectCombatWithNoPlayers
    {
        get => this._isRejectCombatWithNoPlayers;
        set => this.SetField(ref this._isRejectCombatWithNoPlayers, value);
    }

    public bool IsRemoveEntityOutliers
    {
        get => this._isRemoveEntityOutliers;
        set => this.SetField(ref this._isRemoveEntityOutliers, value);
    }

    public bool IsRemoveEntityOutliersNonPlayers
    {
        get => this._isRemoveEntityOutliersNonPlayers;
        set => this.SetField(ref this._isRemoveEntityOutliersNonPlayers, value);
    }

    public bool IsRemoveEntityOutliersPlayers
    {
        get => this._isRemoveEntityOutliersPlayers;
        set => this.SetField(ref this._isRemoveEntityOutliersPlayers, value);
    }

    /// <summary>
    ///     Used in map/event detection when parsing the STO combat logs.
    /// </summary>
    [JsonIgnore]
    public CombatMapDetectionSettings? MapDetectionSettings
    {
        get => this._mapDetectionSettings;
        set => this.SetField(ref this._mapDetectionSettings, value);
    }

    public int CombatDurationPercentage
    {
        get => this._combatDurationPercentage;
        set => this.SetField(ref this._combatDurationPercentage, value);
    }

    public int CombatEventCountMinimum
    {
        get => this._combatEventCountMinimum;
        set => this.SetField(ref this._combatEventCountMinimum, value);
    }

    public int CombineCombatMaxTimeSeconds
    {
        get => this._combineCombatMaxTimeSeconds;
        set
        {
            this.SetField(ref this._combineCombatMaxTimeSeconds, value);
            this.TimeSpanMaxBetweenCombatForCombine = TimeSpan.FromSeconds(value);
        }
    }

    /// <summary>
    ///     A timespan in hours to retrieve combat log data.
    /// </summary>
    public int HowFarBackForCombatInHours
    {
        get => this._howFarBackForCombatInHours;
        set => this.SetField(ref this._howFarBackForCombatInHours, value);
    }

    /// <summary>
    ///     A timespan in seconds defining a new combat instance boundary in time.
    /// </summary>
    public int HowLongBeforeNewCombatInSeconds
    {
        get => this._howLongBeforeNewCombatInSeconds;
        set => this.SetField(ref this._howLongBeforeNewCombatInSeconds, value);
    }

    public int MinInActiveInSeconds
    {
        get => this._minInActiveInSeconds;
        set => this.SetField(ref this._minInActiveInSeconds, value);
    }

    /// <summary>
    ///     A file pattern used to search for combat log files.
    ///     <para>Wildcards can be used to return multiple files.</para>
    /// </summary>
    public string CombatLogPathFilePattern
    {
        get => this._combatLogPathFilePattern;
        set => this.SetField(ref this._combatLogPathFilePattern, value);
    }

    /// <summary>
    ///     The folder to the STO combat log files.
    /// </summary>
    public string? CombatLogPath
    {
        get => this._combatLogPath;
        set => this.SetField(ref this._combatLogPath, value);
    }
    
    public string? MyCharacter
    {
        get => this._myCharacter;
        set => this.SetField(ref this._myCharacter, value);
    }

    public TimeSpan TimeSpanMaxBetweenCombatForCombine
    {
        get => this._timeSpanMaxBetweenCombatForCombine;
        set => this.SetField(ref this._timeSpanMaxBetweenCombatForCombine, value);
    }

    #endregion

    #region Public Members

    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region Other Members

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        this.OnPropertyChanged(propertyName);
        return true;
    }

    #endregion

    #region Overrides of Object

    /// <inheritdoc />
    public override string ToString()
    {
        return
            $"Path={this.CombatLogPath}, File={this.CombatLogPathFilePattern}, HowFar={this.HowFarBackForCombatInHours}, HowLong={this.HowLongBeforeNewCombatInSeconds}";
    }

    #endregion
}