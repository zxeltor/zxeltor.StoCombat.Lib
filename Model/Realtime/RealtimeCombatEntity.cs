// Copyright (c) 2024, Todd Taylor (https://github.com/zxeltor)
// All rights reserved.
// 
// This source code is licensed under the Apache-2.0-style license found in the
// LICENSE file in the root directory of this source tree.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Humanizer;
using Newtonsoft.Json;
using zxeltor.StoCombat.Lib.Classes;
using zxeltor.StoCombat.Lib.Helpers;
using zxeltor.StoCombat.Lib.Model.CombatLog;
using zxeltor.StoCombat.Lib.Parser;
using zxeltor.Types.Lib.Collections;

namespace zxeltor.StoCombat.Lib.Model.Realtime;

/// <summary>
///     This object represents a Player or Non-Player entity from the STO combat logs.
/// </summary>
public class RealtimeCombatEntity : INotifyPropertyChanged
{
    #region Private Fields

    [ResetFieldOnRefresh] private SyncNotifyCollection<CombatEntityDeadZone>? _deadZones;

    [ResetFieldOnRefresh] private int? _entityCombatAttacks;

    [ResetFieldOnRefresh] private TimeSpan? _entityCombatDuration;

    [ResetFieldOnRefresh] private DateTime? _entityCombatEnd;

    [ResetFieldOnRefresh] private TimeSpan? _entityCombatInActive;

    [ResetFieldOnRefresh] private int? _entityCombatKills;

    [ResetFieldOnRefresh] private DateTime? _entityCombatStart;

    [ResetFieldOnRefresh] private double? _entityMagnitudePerSecond;

    [ResetFieldOnRefresh] private double? _entityMaxMagnitude;

    [ResetFieldOnRefresh] private double? _entityTotalMagnitude;

    private bool _isEnableInactiveTimeCalculations = true;

    private bool _isInCombat;

    private int? _minInActiveInSeconds;

    #endregion

    #region Constructors

    /// <summary>
    ///     Constructor needed for JSON deserialization
    /// </summary>
    public RealtimeCombatEntity(string ownerDisplay, string? ownerInternal = null)
    {
        this.OwnerDisplay = ownerDisplay;
        this.OwnerInternal = ownerInternal ?? ownerDisplay;
        this.IsInCombat = true;
    }

    /// <summary>
    ///     The main constructor
    /// </summary>
    public RealtimeCombatEntity(CombatEvent combatEvent, RealtimeCombatLogParseSettings combatLogParseSettings) : this(
        combatEvent.OwnerDisplay, combatEvent.OwnerInternal)
    {
        // Determine if this entity is a player or non-player.
        this.IsPlayer = !string.IsNullOrWhiteSpace(this.OwnerInternal) && this.OwnerInternal.StartsWith("P[");

        this.IsEnableInactiveTimeCalculations = combatLogParseSettings.IsEnableInactiveTimeCalculations;
        this.MinInActiveInSeconds = combatLogParseSettings.MinInActiveInSeconds;

        this.AddCombatEvent(combatEvent);
    }

    #endregion

    #region Public Properties

    public bool IsEnableInactiveTimeCalculations
    {
        get => this._isEnableInactiveTimeCalculations;
        set => this.SetField(ref this._isEnableInactiveTimeCalculations, value);
    }

    public bool IsInCombat
    {
        get => this._isInCombat;
        set => this.SetField(ref this._isInCombat, value);
    }

    /// <summary>
    ///     If true this entity is a Player. If false the entity is a Non-Player.
    /// </summary>
    public bool IsPlayer { get; set; }

    /// <summary>
    ///     Used to establish the end time for this combat entity.
    ///     <para>The last timestamp from our <see cref="CombatEvent" /> collections, based on ann ordered list.</para>
    /// </summary>
    [NotifyPropertyChangedOnRefresh]
    public DateTime? EntityCombatEnd
    {
        get
        {
            if (this._entityCombatEnd == null)
                this._entityCombatEnd =
                    this.CombatEventsList.Count == 0 ? null : this.CombatEventsList.Last().Timestamp;

            return this._entityCombatEnd;
        }
    }

    /// <summary>
    ///     Used to establish the start time for this combat entity.
    ///     <para>The first timestamp from our <see cref="CombatEvent" /> collections, based on an ordered list.</para>
    /// </summary>
    [NotifyPropertyChangedOnRefresh]
    public DateTime? EntityCombatStart
    {
        get
        {
            if (this._entityCombatStart == null)
                this._entityCombatStart = this.CombatEventsList.Count == 0
                    ? null // This should be possible, but checking for it anyway.
                    : this.CombatEventsList.First().Timestamp; //.Min(ev => ev.Timestamp);

            return this._entityCombatStart;
        }
    }

    /// <summary>
    ///     A rudimentary calculation for player events EntityMagnitudePerSecond, and probably incorrect.
    /// </summary>
    [NotifyPropertyChangedOnRefresh]
    public double? EntityMagnitudePerSecond
    {
        get
        {
            if (this._entityMagnitudePerSecond == null)
            {
                var entityEvents = this.CombatEventsList
                    .Where(ev => !ev.Type.Equals("HitPoints", StringComparison.CurrentCultureIgnoreCase)).ToList();

                if (entityEvents.Count == 0 || this.EntityCombatDuration == null)
                    return this._entityMagnitudePerSecond = 0;

                if (this.EntityCombatInActive != null &&
                    this.EntityCombatDuration.Value > this.EntityCombatInActive.Value)
                {
                    var duration = this.EntityCombatDuration.Value - this.EntityCombatInActive.Value;
                    return this._entityMagnitudePerSecond = this.EntityTotalMagnitude / duration.TotalSeconds;
                }

                return this._entityMagnitudePerSecond = 0;
            }

            return this._entityMagnitudePerSecond;
        }
    }

    /// <summary>
    ///     A rudimentary calculation for max damage for player events, and probably incorrect.
    /// </summary>
    [NotifyPropertyChangedOnRefresh]
    public double? EntityMaxMagnitude
    {
        get
        {
            if (this._entityMaxMagnitude == null)
            {
                var entityEvents = this.CombatEventsList
                    .Where(ev => !ev.Type.Equals("HitPoints", StringComparison.CurrentCultureIgnoreCase)).ToList();

                if (entityEvents.Count == 0) return this._entityMaxMagnitude = 0;

                return this._entityMaxMagnitude = entityEvents.Max(dam => Math.Abs(dam.Magnitude));
            }

            return this._entityMaxMagnitude;
        }
    }

    /// <summary>
    ///     A rudimentary calculation for total damage for player events, and probably incorrect.
    /// </summary>
    [NotifyPropertyChangedOnRefresh]
    public double? EntityTotalMagnitude
    {
        get
        {
            if (this._entityTotalMagnitude == null)
            {
                var entityEvents = this.CombatEventsList
                    .Where(ev => !ev.Type.Equals("HitPoints", StringComparison.CurrentCultureIgnoreCase)).ToList();

                if (entityEvents.Count == 0) return this._entityTotalMagnitude = 0;

                return this._entityTotalMagnitude = entityEvents.Sum(dam => Math.Abs(dam.Magnitude));
            }

            return this._entityTotalMagnitude;
        }
    }

    /// <summary>
    ///     Get a number of attacks for this entity
    /// </summary>
    [NotifyPropertyChangedOnRefresh]
    public int? EntityCombatAttacks
    {
        get
        {
            if (this._entityCombatAttacks == null)
            {
                this._entityCombatAttacks = 0;

                this._entityCombatAttacks = this.CombatEventsList.Count(ev =>
                    !ev.Type.Equals("Shield", StringComparison.CurrentCultureIgnoreCase)
                    && !ev.Type.Equals("HitPoints", StringComparison.CurrentCultureIgnoreCase));
            }

            return this._entityCombatAttacks;
        }
    }

    /// <summary>
    ///     Get a number of kills for this entity.
    /// </summary>
    [NotifyPropertyChangedOnRefresh]
    public int? EntityCombatKills
    {
        get
        {
            if (this._entityCombatKills == null)
                this._entityCombatKills = this.CombatEventsList.Count(ev =>
                    ev.Flags.Contains("kill", StringComparison.CurrentCultureIgnoreCase));
            return this._entityCombatKills;
        }
    }

    public int? MinInActiveInSeconds
    {
        get => this._minInActiveInSeconds;
        set => this.SetField(ref this._minInActiveInSeconds, value);
    }

    /// <summary>
    ///     A label for our entity
    /// </summary>
    public string OwnerDisplay { get; set; }

    /// <summary>
    ///     The ID for our entity
    /// </summary>
    public string OwnerInternal { get; set; }

    [JsonIgnore]
    public string ToCombatStats
    {
        get
        {
            var str = new StringBuilder($"{this.OwnerDisplay}: ");
            str.Append($"Attacks={this.EntityCombatAttacks ?? 0}, ");

            if (this.EntityTotalMagnitude == null)
                str.Append("Dam=0, ");
            else
                str.Append($"Dam={this.EntityTotalMagnitude.Value.ToMetric(decimals: 2)}, ");

            if (this.EntityMagnitudePerSecond == null)
                str.Append("DPS=0, ");
            else
                str.Append($"DPS={this.EntityMagnitudePerSecond.Value.ToMetric(decimals: 2)}, ");

            if (this.EntityCombatInActive == null)
                str.Append("InActive=0");
            else
                str.Append($"InActive={this.EntityCombatInActive.Value.ToString("g")}");

            return str.ToString();
        }
    }

    /// <summary>
    ///     A list of timespans where the Player is considered Inactive.
    /// </summary>
    public SyncNotifyCollection<CombatEntityDeadZone> DeadZones
    {
        get
        {
            if (this._deadZones == null)
            {
                if (!this.IsEnableInactiveTimeCalculations || this.CombatEventsList.Count == 0)
                    return new SyncNotifyCollection<CombatEntityDeadZone>(0);

                var deadZones = new SyncNotifyCollection<CombatEntityDeadZone>();

                var minNoActivity = this.MinInActiveInSeconds.HasValue &&
                                    TimeSpan.FromSeconds(this.MinInActiveInSeconds.Value) > TimeSpan.FromSeconds(1)
                    ? TimeSpan.FromSeconds(this.MinInActiveInSeconds.Value)
                    : TimeSpan.FromSeconds(1);

                var lastTimestamp = this.CombatEventsList.First().Timestamp; // .Min(evt => evt.Timestamp);

                foreach (var combatEvent in this.CombatEventsList)
                {
                    if (combatEvent.Timestamp == this.EntityCombatStart) continue;

                    if (combatEvent.Timestamp - lastTimestamp >= minNoActivity)
                        deadZones.Add(new CombatEntityDeadZone(lastTimestamp, combatEvent.Timestamp));

                    lastTimestamp = combatEvent.Timestamp;
                }

                this._deadZones = deadZones;
            }

            return this._deadZones;
        }
    }

    public SyncNotifyCollection<CombatEvent> CombatEventsList { get; } = [];

    /// <summary>
    ///     A humanized string base on combat duration. (<see cref="EntityCombatEnd" /> - <see cref="EntityCombatStart" />)
    /// </summary>
    [NotifyPropertyChangedOnRefresh]
    public TimeSpan? EntityCombatDuration
    {
        get
        {
            if (this._entityCombatDuration == null)
            {
                if (this.EntityCombatEnd.HasValue && this.EntityCombatStart.HasValue)
                    if (this.EntityCombatEnd.Value - this.EntityCombatStart.Value <= Constants.MINCOMBATDURATION)
                        return this._entityCombatDuration = Constants.MINCOMBATDURATION;
                    else
                        return this._entityCombatDuration = this.EntityCombatEnd.Value - this.EntityCombatStart.Value;

                return this._entityCombatDuration = TimeSpan.Zero;
            }

            return this._entityCombatDuration;
        }
    }

    /// <summary>
    ///     Calculate the total amount of Player inactive time.
    /// </summary>
    [NotifyPropertyChangedOnRefresh]
    public TimeSpan? EntityCombatInActive
    {
        get
        {
            if (this._entityCombatInActive == null)
            {
                if (this.DeadZones.Count > 0)
                    return this._entityCombatInActive =
                        TimeSpan.FromSeconds(this.DeadZones.Sum(dead => dead.Duration.TotalSeconds));

                return this._entityCombatInActive = TimeSpan.Zero;
            }

            return this._entityCombatInActive;
        }
    }

    #endregion

    #region Public Members

    public void AddCombatEvent(CombatEvent combatEvent)
    {
        this.IsInCombat = true;

        this.CombatEventsList.Add(combatEvent);
    }

    /// <summary>
    ///     A helper method created to support the <see cref="INotifyPropertyChanged" /> implementation of this class.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    public void Refresh()
    {
        CombatRefreshHelper.RefreshObject(this);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return
            $"Owner={this.OwnerDisplay}, Player={this.IsPlayer}, EntityCombatKills={this.EntityCombatKills}, EntityCombatDuration={this.EntityCombatDuration:G}, Start={this.EntityCombatStart}, End={this.EntityCombatEnd}";
    }

    #endregion

    #region Other Members

    // Create the OnPropertyChanged method to raise the event
    // The calling member's name will be used as the parameter.
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        if (name != null) this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    ///     A helper method created to support the <see cref="INotifyPropertyChanged" /> implementation of this class.
    /// </summary>
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        if (propertyName != null) this.OnPropertyChanged(propertyName);
        return true;
    }

    #endregion
}