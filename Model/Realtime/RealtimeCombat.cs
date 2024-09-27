// Copyright (c) 2024, Todd Taylor (https://github.com/zxeltor)
// All rights reserved.
// 
// This source code is licensed under the Apache-2.0-style license found in the
// LICENSE file in the root directory of this source tree.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using zxeltor.StoCombat.Lib.Classes;
using zxeltor.StoCombat.Lib.Helpers;
using zxeltor.StoCombat.Lib.Model.CombatLog;
using zxeltor.StoCombat.Lib.Parser;
using zxeltor.Types.Lib.Collections;
using zxeltor.Types.Lib.Extensions;

namespace zxeltor.StoCombat.Lib.Model.Realtime;

/// <summary>
///     This class represents a collection of <see cref="CombatEntity" /> objects which fall within the time range of
///     <see cref="CombatStart" /> and <see cref="CombatEnd" />
/// </summary>
public class RealtimeCombat : INotifyPropertyChanged
{
    #region Private Fields

    [ResetFieldOnRefresh] private SyncNotifyCollection<CombatEvent>? _allCombatEvents;

    [ResetFieldOnRefresh] private TimeSpan? _combatDuration;

    [ResetFieldOnRefresh] private DateTime? _combatEnd;

    [ResetFieldOnRefresh] private DateTime? _combatStart;

    [ResetFieldOnRefresh] private int? _eventsCount;

    #endregion

    #region Constructors

    /// <summary>
    ///     Constructor need for JSON deserialization
    /// </summary>
    public RealtimeCombat()
    {
        this.PlayerEntities.CollectionChanged += this.PlayerEntitiesOnCollectionChanged;
    }

    /// <summary>
    ///     The main constructor
    /// </summary>
    /// <param name="combatEvent">
    ///     The initial combat event, which establishes our first <see cref="CombatEntity" /> for this
    ///     instance.
    /// </param>
    /// <param name="combatLogParseSettings">The parser settings</param>
    public RealtimeCombat(CombatEvent combatEvent, RealtimeCombatLogParseSettings combatLogParseSettings)
    {
        this.AddCombatEvent(combatEvent, combatLogParseSettings);
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///     Used to establish the end time for this combat instance.
    ///     <para>The last timestamp from our <see cref="CombatEntity" /> collections, based on ann ordered list.</para>
    /// </summary>
    [NotifyPropertyChangedOnRefresh]
    public DateTime? CombatEnd
    {
        get
        {
            if (this._combatEnd == null)
                this._combatEnd = this.PlayerEntities.Count > 0 && this.PlayerEntities.Count(entity => entity.EntityCombatEnd.HasValue) > 0
                    ? this.PlayerEntities.Where(entity => entity.EntityCombatEnd.HasValue).Max(entity => entity.EntityCombatEnd!.Value)
                    : null;

            return this._combatEnd;
        }
    }

    /// <summary>
    ///     Used to establish the start time for this combat instance.
    ///     <para>The first timestamp from our <see cref="CombatEntity" /> collections, based on an ordered list.</para>
    /// </summary>
    [NotifyPropertyChangedOnRefresh]
    public DateTime? CombatStart
    {
        get
        {
            if (this._combatStart == null)
                this._combatStart = this.PlayerEntities.Count > 0 && this.PlayerEntities.Count(entity => entity.EntityCombatStart.HasValue) > 0
                    ? this.PlayerEntities.Where(entity => entity.EntityCombatStart.HasValue).Min(entity => entity.EntityCombatStart!.Value)
                    : null;

            return this._combatStart;
        }
    }

    /// <summary>
    ///     A total number of events for Player and NonPlayer entities.
    /// </summary>
    [NotifyPropertyChangedOnRefresh]
    public int? EventsCount
    {
        get
        {
            if (this._eventsCount == null)
                this._eventsCount = this.PlayerEntities.Sum(ent => ent.CombatEventsList.Count);

            return this._eventsCount;
        }
    }

    //this.PlayerEntities.Sum(en => en.CombatEventsList.Count) +
    //             this.NonPlayerEntities.Sum(en => en.CombatEventsList.Count);
    /// <summary>
    ///     A list of all events for this combat instance.
    /// </summary>
    [JsonIgnore]
    [NotifyPropertyChangedOnRefresh]
    public SyncNotifyCollection<CombatEvent>? AllCombatEvents
    {
        get
        {
            if (this._allCombatEvents == null)
                this._allCombatEvents = this.PlayerEntities.Count > 0
                    ? this.PlayerEntities.SelectMany(ent => ent.CombatEventsList).ToSyncNotifyCollection()
                    : null;

            return this._allCombatEvents;
        }
    }

    /// <summary>
    ///     A list of player <see cref="RealtimeCombatEntity" /> objects.
    /// </summary>
    public SyncNotifyCollection<RealtimeCombatEntity> PlayerEntities { get; set; } = [];

    [JsonIgnore]
    [NotifyPropertyChangedOnRefresh]
    public SyncNotifyCollection<RealtimeCombatEntity> PlayerEntitiesOrderByName =>
        new(this.PlayerEntities.OrderBy(ent => ent.OwnerDisplay));

    /// <summary>
    ///     A humanized string based on combat duration. (<see cref="CombatEnd" /> - <see cref="CombatStart" />)
    /// </summary>
    [NotifyPropertyChangedOnRefresh]
    public TimeSpan? CombatDuration
    {
        get
        {
            if (this._combatDuration == null)
                if (this.CombatEnd.HasValue && this.CombatStart.HasValue)
                    return this._combatDuration = this.CombatEnd - this.CombatStart;

            return this._combatDuration;
        }
    }

    #endregion

    #region Public Members

    /// <summary>
    ///     A method used to inject new <see cref="CombatEvent" /> objects into our <see cref="CombatEntity" /> hierarchy.
    /// </summary>
    /// <param name="combatEvent">A new combat event to inject into our hierarchy.</param>
    /// <param name="combatLogParseSettings">The parser settings</param>
    public void AddCombatEvent(CombatEvent combatEvent, RealtimeCombatLogParseSettings combatLogParseSettings)
    {
        if (combatEvent.IsOwnerPlayer)
        {
            // Insert the incoming event under an existing player combat entity, or create a new one if we need too.
            var existingPlayer =
                this.PlayerEntities.FirstOrDefault(owner => owner.OwnerInternal.Equals(combatEvent.OwnerInternal));
            if (existingPlayer == null)
            {
                existingPlayer = new RealtimeCombatEntity(combatEvent, combatLogParseSettings);
                this.PlayerEntities.Add(existingPlayer);
                this.OnPropertyChanged(nameof(this.PlayerEntitiesOrderByName));
            }
            else
            {
                existingPlayer.AddCombatEvent(combatEvent);
            }
        }
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    public void Refresh()
    {
        this.PlayerEntities.ToList().ForEach(player =>
        {
            if (player.IsInCombat) player.Refresh();
        });

        CombatRefreshHelper.RefreshObject(this);
    }


    /// <inheritdoc />
    public override string ToString()
    {
        return $"EntityCombatDuration={this.CombatDuration}, Start={this.CombatStart}, End={this.CombatEnd}";
    }

    #endregion

    #region Other Members

    /// <summary>
    ///     A helper method created to support the <see cref="INotifyPropertyChanged" /> implementation of this class.
    /// </summary>
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void PlayerEntitiesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        this.OnPropertyChanged(nameof(this.PlayerEntitiesOrderByName));
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