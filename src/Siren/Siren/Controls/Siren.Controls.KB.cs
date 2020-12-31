﻿using System;
using CitizenFX.Core;
using ELS.configuration;
using ELS.TrafficControl;

namespace ELS.Siren
{
    partial class Siren : IManagerEntry
    {
        void AirHornControlsKB()
        {
            if ((Game.IsControlJustPressed(0, ElsConfiguration.KBBindings.Sound_Ahorn) &&
                 Game.CurrentInputMode == InputMode.MouseAndKeyboard) ||
                (Game.IsControlJustPressed(2, ElsConfiguration.GPBindings.Sound_Ahorn) && Game.CurrentInputMode == InputMode.GamePad && Global.AllowController))
            {
#if !REMOTETEST
                AirHornLogic(true, true);
#endif
                RemoteEventManager.SendEvent(RemoteEventManager.Commands.AirHorn, _vehicle, true, Game.Player.ServerId);

            }
            if ((Game.IsControlJustReleased(0, ElsConfiguration.KBBindings.Sound_Ahorn) &&
                 Game.CurrentInputMode == InputMode.MouseAndKeyboard)
                || (Game.IsControlJustReleased(2, Control.SpecialAbility) && Game.CurrentInputMode == InputMode.GamePad && Global.AllowController))
            {
#if !REMOTETEST
                int[] patts = { _patternController.CurrentPrmPattern, _patternController.CurrentSecPattern, _patternController.CurrentWrnPattern };
                AirHornLogic(false, true);
#endif
                RemoteEventManager.SendEvent(RemoteEventManager.Commands.AirHorn, _vehicle, false, Game.Player.ServerId);
            }
        }
        void ManualTone1ControlsKB()
        {
            if ((Game.IsControlJustReleased(0, ElsConfiguration.KBBindings.Snd_SrnTon1) && Game.CurrentInputMode == InputMode.MouseAndKeyboard) || (Global.AllowController && Game.IsControlJustPressed(2, ElsConfiguration.GPBindings.Snd_SrnTon1) && Game.CurrentInputMode == InputMode.GamePad))
            {
#if !REMOTETEST
                SirenTone1Logic(true, true);
#endif
                RemoteEventManager.SendEvent(RemoteEventManager.Commands.ManualTone1, _vehicle, true, Game.Player.ServerId);
            }
        }
        void ManualTone2ControlsKB()
        {
            if ((Game.IsControlJustReleased(0, ElsConfiguration.KBBindings.Snd_SrnTon2) && Game.CurrentInputMode == InputMode.MouseAndKeyboard) || (Global.AllowController && Game.IsControlPressed(2, ElsConfiguration.GPBindings.Snd_SrnTon1) && Game.IsControlJustPressed(2, ElsConfiguration.GPBindings.Snd_SrnTon3) && Game.CurrentInputMode == InputMode.GamePad))
            {
#if !REMOTETEST
                SirenTone2Logic(true, true);
#endif
                RemoteEventManager.SendEvent(RemoteEventManager.Commands.ManualTone2, _vehicle, true, Game.Player.ServerId);
            }
        }
        void ManualTone3ControlsKB()
        {
            if ((Game.IsControlJustReleased(0, ElsConfiguration.KBBindings.Snd_SrnTon3) && Game.CurrentInputMode == InputMode.MouseAndKeyboard) || (Global.AllowController && Game.IsControlJustPressed(2, ElsConfiguration.GPBindings.Snd_SrnTon3) && Game.CurrentInputMode == InputMode.GamePad))
            {
#if !REMOTETEST
                SirenTone3Logic(true, true);
#endif
                RemoteEventManager.SendEvent(RemoteEventManager.Commands.ManualTone3, _vehicle, true, Game.Player.ServerId);
            }
        }
        void ManualTone4ControlsKB()
        {
            if (Game.IsControlJustReleased(0, ElsConfiguration.KBBindings.Snd_SrnTon4))
            {
#if !REMOTETEST
                SirenTone4Logic(true, true);
#endif
                RemoteEventManager.SendEvent(RemoteEventManager.Commands.ManualTone4, _vehicle, true, Game.Player.ServerId);
            }
        }

        void MainSirenToggleControlsKB()
        {
            if ((Game.IsControlJustReleased(0, ElsConfiguration.KBBindings.Toggle_SIRN) 
                && Game.CurrentInputMode == InputMode.MouseAndKeyboard) || (Global.AllowController 
                && Game.IsControlJustReleased(2, ElsConfiguration.GPBindings.Toggle_SIRN) 
                && Game.CurrentInputMode == InputMode.GamePad))
            {
#if !REMOTETEST
                MainSirenToggleLogic(true, true);
#endif
                RemoteEventManager.SendEvent(RemoteEventManager.Commands.MainSiren, _vehicle, true, Game.Player.ServerId);
            }
        }

        void ManualSoundControlsKB()
        {
            Game.DisableControlThisFrame(0, ElsConfiguration.KBBindings.Sound_Manul);
            if (Game.IsDisabledControlJustPressed(0, ElsConfiguration.KBBindings.Sound_Manul))
            {
#if !REMOTETEST
                ManualSoundLogic(true, true);
#endif
                RemoteEventManager.SendEvent(RemoteEventManager.Commands.ManualSound, _vehicle, true, Game.Player.ServerId);
            }
            if (Game.IsDisabledControlJustReleased(0, ElsConfiguration.KBBindings.Sound_Manul))
            {
#if !REMOTETEST
                ManualSoundLogic(false, true);
#endif
                RemoteEventManager.SendEvent(RemoteEventManager.Commands.ManualSound, _vehicle, false, Game.Player.ServerId);
            }
        }

        void DualSirenControlsKB()
        {
            if (Game.IsControlJustReleased(0, ElsConfiguration.KBBindings.Toggle_DSRN))
            {
#if !REMOTETEST
                DualSirenLogic(true, true);
#endif
                System.Collections.Generic.Dictionary<String, object> dic = new System.Collections.Generic.Dictionary<string, object>();
               // Manager.VehicleManager.SyncRequestReply(_vehicle.Plate());
                RemoteEventManager.SendEvent(RemoteEventManager.Commands.DualSiren, _vehicle, true, Game.Player.ServerId);
            }
        }
        int pressedTime;

        void PanicAlarmControlsKB()
        {
            var bonePos = _vehicle.Bones["door_dside_f"].Position;
            var pos = Game.Player.Character.GetPositionOffset(bonePos);
            if (pos.Length() > 1.5)
            {
                return;
            }
            if (Game.IsControlJustPressed(0, ElsConfiguration.KBBindings.Snd_SrnPnic))
            {
                pressedTime = Game.GameTime;
            }
            if (Game.IsControlPressed(0, ElsConfiguration.KBBindings.Snd_SrnPnic))
            {
                if (pressedTime != -1 && Game.GameTime - pressedTime >= 499)
                {
                    pressedTime = -1;
                    var state = !_tones.panicAlarm._state;
#if !REMOTETEST
                    PanicAlarmLogic(state);
#endif
                    RemoteEventManager.SendEvent(RemoteEventManager.Commands.PanicAlarm, _vehicle, state, Game.Player.ServerId);
                }
            }
            if (Game.IsControlJustReleased(0, ElsConfiguration.KBBindings.Snd_SrnPnic))
            {
                pressedTime = -1;
            }
        }
    }
}