﻿/*
    ELS FiveM - A ELS implementation for FiveM
    Copyright (C) 2017  E.J. Bevenour

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using ELS.configuration;
using SharpConfig;

namespace ELS
{
    internal class SirenManager
    {
        private readonly List<Siren.Siren> _sirens;

        /// <summary>
        ///     Siren that LocalPlayer can manage
        /// </summary>
        private Siren.Siren _currentSiren;

        public SirenManager()
        {
            FileLoader.OnSettingsLoaded += FileLoader_OnSettingsLoaded;
            _sirens = new List<Siren.Siren>();
        }

        private void RunGC()
        {
            _sirens.RemoveAll(obj => !obj._vehicle.Exists());
        }

        internal void FullSync()
        {
            _currentSiren.FullSync();
            //_sirens.ForEach((siren) =>
            //{
            //    siren.FullSync();
            //});
        }

        internal void FullSync(string dataType, Dictionary<string, object> dataDic, int playerId)
        {
            RunGC();
            var randVehicle = new PlayerList()[playerId].Character.CurrentVehicle;
            Debug.WriteLine("FullSyncRecieved");
            _sirens.Find(siren => siren._vehicle.Handle == randVehicle.Handle).SetFullSync(dataType, dataDic);
        }

        private void FileLoader_OnSettingsLoaded(SettingsType.Type type, string data)
        {
            switch (type)
            {
                case SettingsType.Type.GLOBAL:
                    var u = Configuration.LoadFromString(data);
                    var t = u["GENERAL"]["MaxActiveVehs"].IntValue;
                    if (_sirens != null)
                        _sirens.Capacity = t;
                    break;
                case SettingsType.Type.LIGHTING:
                    break;
            }
        }

        internal void CleanUp()
        {
#if DEBUG
            Debug.WriteLine("Running CleanUp()");
#endif
            _sirens.ForEach(siren => siren.CleanUP());
        }

        internal void Runtick()
        {
            var localPlayer = Game.Player;
            if (localPlayer.Character.IsInVehicle()
                && localPlayer.Character.IsSittingInVehicle()
                && localPlayer.Character.CurrentVehicle.IsEls()
                && (
                    localPlayer.Character.CurrentVehicle.GetPedOnSeat(VehicleSeat.Driver) == localPlayer.Character
                    || localPlayer.Character.CurrentVehicle.GetPedOnSeat(VehicleSeat.Passenger) == localPlayer.Character
                    )
                )
            {
                if (_currentSiren == null || _currentSiren._vehicle != Game.Player.Character.CurrentVehicle)
                {
                    var vehicle = Game.Player.Character.CurrentVehicle;
                    AddVehicleIfNotRegistered(vehicle);
                    _currentSiren = _sirens.Find(siren => siren._vehicle.Handle == vehicle.Handle);
                }
                _currentSiren.ticker();
            }
        }

        internal void UpdateSirens(string command, int netId, bool state)
        {
            if (Game.Player.ServerId == netId) return;

#if DEBUG
            Debug.WriteLine($"netId:{netId} localId:{Game.Player.ServerId}");
#endif
            var vehicle = new PlayerList()[netId].Character.CurrentVehicle;
            if (!vehicle.Exists()) throw new Exception("Vehicle does not exist");
            AddVehicleIfNotRegistered(vehicle);
            _sirens.Find(siren => siren._vehicle.Handle == vehicle.Handle).updateLocalRemoteSiren(command, state);
        }

        private void AddVehicleIfNotRegistered(Vehicle vehicle)
        {
            if (!_sirens.Exists(siren => siren._vehicle.Handle == vehicle.Handle))
            {
                _sirens.Add(new Siren.Siren(vehicle));
#if DEBUG
                Debug.WriteLine("added new siren");
#endif
            }
#if DEBUG
            else
            {
                Debug.WriteLine("added existing siren");
            }
#endif
        }

        public void GetAllSirenData()
        {
            var localPLayer = Game.Player;
            var players = new PlayerList().Where((player, i) => player.Character.CurrentVehicle.IsEls() || player.Character.LastVehicle.IsEls());
            foreach (var player in players)
            {
                if (player.Character.CurrentVehicle.IsEls())
                {
                    AddVehicleIfNotRegistered(player.Character.CurrentVehicle);
                }

                if (player.Character.LastVehicle.IsEls())
                    AddVehicleIfNotRegistered(player.Character.LastVehicle);
            }
        }
    }
}