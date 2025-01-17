﻿// Decompiled with JetBrains decompiler
// Type: ScriptKidAntiCheat.Punishments.DoYouEvenAimBro
// Assembly: SteamWrapper, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b9d60819726d5649
// MVID: 52813C95-239A-43DA-B46E-DCB9B069035E
// Assembly location: C:\Users\Ni1kk\Desktop\New folder\bin\SteamWrapper.exe

using ScriptKidAntiCheat.Internal;
using ScriptKidAntiCheat.Utils;
using ScriptKidAntiCheat.Utils.Maths;
using SharpDX;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ScriptKidAntiCheat.Punishments
{
  internal class DoYouEvenAimBro : Punishment
  {
    public static bool logDelay;

    public override int ActivateOnRound { get; set; } = 6;

    public DoYouEvenAimBro()
      : base(0, false, 100)
    {
      this.GameData = Program.GameData;
      this.Player = Program.GameData.Player;
    }

    public override void Tick(object source, ElapsedEventArgs e)
    {
      try
      {
        if (!Program.GameProcess.IsValidAndActiveWindow || !this.Player.IsAlive() || (!this.CanActivate() || Program.GameData.MatchInfo.isFreezeTime))
          return;
        switch ((Weapons) this.Player.ActiveWeapon)
        {
          case Weapons.Flashbang:
            break;
          case Weapons.Grenade:
            break;
          case Weapons.Smoke:
            break;
          case Weapons.Molotov:
            break;
          case Weapons.Incendiary:
            break;
          case Weapons.Knife:
            break;
          default:
            Vector3 aimDirection = this.Player.AimDirection;
            Vector3 eyePosition = this.Player.EyePosition;
            if ((double) ((Vector3) ref aimDirection).Length() < 0.001)
              break;
            Line3D aimRayWorld = new Line3D(eyePosition, Vector3.op_Addition(eyePosition, Vector3.op_Multiply(aimDirection, 8192f)));
            foreach (Entity entity in this.GameData.Entities)
            {
              if (entity.IsAlive() && !(entity.AddressBase == this.Player.AddressBase) && (entity.Team != this.Player.Team && entity.Spotted))
              {
                if (Helper.IntersectsHitBox(aimRayWorld, entity, 1f) >= 0)
                  this.ActivatePunishment();
                Thread.Sleep(10);
              }
            }
            break;
        }
      }
      catch (Exception ex)
      {
      }
    }

    public void ActivatePunishment()
    {
      if (!this.CanActivate())
        return;
      Task.Run((Action) (() =>
      {
        for (int index = 0; index < 150; index += 25)
        {
          SendInput.MouseMove(-25, 0);
          Thread.Sleep(10);
        }
      }));
      if (DoYouEvenAimBro.logDelay)
        return;
      DoYouEvenAimBro.logDelay = true;
      this.AfterActivate(true, "");
      Task.Run((Action) (() =>
      {
        Thread.Sleep(12000);
        DoYouEvenAimBro.logDelay = false;
      }));
    }
  }
}
