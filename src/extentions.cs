﻿using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

namespace ELS
{
    public static class Extentions
    {
        public static bool IsEls(this Vehicle vehicle)
        {
            return configuration.VCF.ELSVehicle.ContainsKey(vehicle.Model);
        }
        
        public static bool IsSittingInELSVehicle(this Ped ped)
        {
            return (ped.IsSittingInVehicle() && ped.CurrentVehicle.IsEls()) ? true : false ; 
        }

        public static string Plate(this Vehicle vehicle)
        {
            return API.GetVehicleNumberPlateText(vehicle.Handle);
        }

        async public static Task<bool> RequestCollision(this Vehicle vehicle)
        {

            CitizenFX.Core.Native.Function.Call(Hash.REQUEST_COLLISION_AT_COORD, vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z);
            while (Function.Call<bool>(Hash.HAS_COLLISION_LOADED_AROUND_ENTITY, vehicle))
            {
                await BaseScript.Delay(0);
            }
            Utils.DebugWriteLine("collision loaded");
            return true;
        }
        //public static void CleanUp(this PoolObject poolObject)
        //{
        //    if (!poolObject.Exists()) poolObject.CleanUp();
        //}

        public static int GetNetworkId(this Entity entity)
        {
            return Function.Call<int>(Hash.VEH_TO_NET, entity.Handle);
        }

        public static void RegisterAsNetworked(this Entity entity)
        {
            Function.Call((Hash)0x06FAACD625D80CAA, entity.Handle);
        }

        public static void SetExistOnAllMachines(this Entity entity, bool b)
        {
            Function.Call(Hash.SET_NETWORK_ID_EXISTS_ON_ALL_MACHINES, entity.GetNetworkId(), b);
        }
    }
}
