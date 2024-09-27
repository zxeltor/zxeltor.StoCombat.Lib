// Copyright (c) 2024, Todd Taylor (https://github.com/zxeltor)
// All rights reserved.
// 
// This source code is licensed under the Apache-2.0-style license found in the
// LICENSE file in the root directory of this source tree.

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace zxeltor.StoCombat.Lib.Parser;

public class RealtimeCombatLogParseSettings : INotifyPropertyChanged
{
    #region Private Fields

    private int _announcementPlaybackVolumePercentage = 50;
    private string? _combatLogPath;
    private string _combatLogPathFilePattern = "combatlog*.log";
    private int _howLongAfterNoCombatBeforeRemoveFromGridInSeconds;
    private int _howLongBeforeNewCombatInSeconds = 20;
    private int _howOftenParseLogsInSeconds = 3;
    private bool _isEnableInactiveTimeCalculations = true;
    private bool _isIncludeAssistedKillsInAchievements = true;
    private bool _isProcessKillingSpreeAnnouncements = true;
    private bool _isProcessKillingSpreeSplash = true;
    private bool _isProcessMiscAnnouncements = true;
    private bool _isProcessMiscSplash = true;
    private bool _isProcessMultiKillAnnouncements = true;
    private bool _isProcessMultiKillSplash = true;
    private bool _isUnrealAnnouncementsEnabled = true;
    private int _minInActiveInSeconds = 6;
    private int _multiKillWaitInSeconds = 4;
    private string? _myCharacter;
    private TimeSpan _timeSpanHowLongAfterNoCombatBeforeRemoveFromGrid = TimeSpan.Zero;
    private TimeSpan _timeSpanHowLongBeforeNewCombat = TimeSpan.FromSeconds(20);
    private TimeSpan _timeSpanHowOftenParseLogs = TimeSpan.FromSeconds(0);

    #endregion

    #region Constructors

    /// <summary>
    ///     Default constructor.
    /// </summary>
    public RealtimeCombatLogParseSettings()
    {
    }

    #endregion

    #region Public Properties

    public bool IsEnableInactiveTimeCalculations
    {
        get => this._isEnableInactiveTimeCalculations;
        set => this.SetField(ref this._isEnableInactiveTimeCalculations, value);
    }

    public bool IsIncludeAssistedKillsInAchievements
    {
        get => this._isIncludeAssistedKillsInAchievements;
        set => this.SetField(ref this._isIncludeAssistedKillsInAchievements, value);
    }

    public bool IsProcessKillingSpreeAnnouncements
    {
        get => this._isProcessKillingSpreeAnnouncements;
        set => this.SetField(ref this._isProcessKillingSpreeAnnouncements, value);
    }

    public bool IsProcessKillingSpreeSplash
    {
        get => this._isProcessKillingSpreeSplash;
        set => this.SetField(ref this._isProcessKillingSpreeSplash, value);
    }

    public bool IsProcessMiscAnnouncements
    {
        get => this._isProcessMiscAnnouncements;
        set => this.SetField(ref this._isProcessMiscAnnouncements, value);
    }

    public bool IsProcessMiscSplash
    {
        get => this._isProcessMiscSplash;
        set => this.SetField(ref this._isProcessMiscSplash, value);
    }

    public bool IsProcessMultiKillAnnouncements
    {
        get => this._isProcessMultiKillAnnouncements;
        set => this.SetField(ref this._isProcessMultiKillAnnouncements, value);
    }

    public bool IsProcessMultiKillSplash
    {
        get => this._isProcessMultiKillSplash;
        set => this.SetField(ref this._isProcessMultiKillSplash, value);
    }

    public bool IsUnrealAnnouncementsEnabled
    {
        get => this._isUnrealAnnouncementsEnabled;
        set => this.SetField(ref this._isUnrealAnnouncementsEnabled, value);
    }

    public int AnnouncementPlaybackVolumePercentage
    {
        get => this._announcementPlaybackVolumePercentage;
        set => this.SetField(ref this._announcementPlaybackVolumePercentage, value);
    }


    public int HowLongAfterNoCombatBeforeRemoveFromGridInSeconds
    {
        get => this._howLongAfterNoCombatBeforeRemoveFromGridInSeconds;
        set
        {
            this.SetField(ref this._howLongAfterNoCombatBeforeRemoveFromGridInSeconds, value);
            this.TimeSpanHowLongAfterNoCombatBeforeRemoveFromGrid =
                this.TimeSpanHowLongBeforeNewCombat + TimeSpan.FromSeconds(value);
        }
    }

    /// <summary>
    ///     A timespan in seconds defining a new combat instance boundary in time.
    /// </summary>
    public int HowLongBeforeNewCombatInSeconds
    {
        get => this._howLongBeforeNewCombatInSeconds;
        set
        {
            this.SetField(ref this._howLongBeforeNewCombatInSeconds, value);
            this.TimeSpanHowLongBeforeNewCombat = TimeSpan.FromSeconds(value);
        }
    }

    public int HowOftenParseLogsInSeconds
    {
        get => this._howOftenParseLogsInSeconds;
        set
        {
            this.SetField(ref this._howOftenParseLogsInSeconds, value);
            this.TimeSpanHowOftenParseLogs = TimeSpan.FromSeconds(value);
        }
    }

    public int MinInActiveInSeconds
    {
        get => this._minInActiveInSeconds;
        set => this.SetField(ref this._minInActiveInSeconds, value);
    }

    public int MultiKillWaitInSeconds
    {
        get => this._multiKillWaitInSeconds;
        set => this.SetField(ref this._multiKillWaitInSeconds, value);
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

    public TimeSpan TimeSpanHowLongAfterNoCombatBeforeRemoveFromGrid
    {
        get => this._timeSpanHowLongAfterNoCombatBeforeRemoveFromGrid;
        set => this.SetField(ref this._timeSpanHowLongAfterNoCombatBeforeRemoveFromGrid, value);
    }

    public TimeSpan TimeSpanHowLongBeforeNewCombat
    {
        get => this._timeSpanHowLongBeforeNewCombat;
        set => this.SetField(ref this._timeSpanHowLongBeforeNewCombat, value);
    }

    public TimeSpan TimeSpanHowOftenParseLogs
    {
        get => this._timeSpanHowOftenParseLogs;
        set => this.SetField(ref this._timeSpanHowOftenParseLogs, value);
    }

    #endregion

    #region Public Members

    public event PropertyChangedEventHandler? PropertyChanged;

    #region Overrides of Object

    /// <inheritdoc />
    public override string ToString()
    {
        return
            $"Path={this.CombatLogPath}, File={this.CombatLogPathFilePattern}, HowLong={this.HowLongBeforeNewCombatInSeconds}";
    }

    #endregion

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
}