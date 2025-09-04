/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Katarina Kostkoa
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
namespace GhostlyLib.Elements
{
    public enum GameState
    {
        DeviceConnected,
        DeviceConnecting,
        DeviceCannotConnect,
        DeviceTrained,
        GameOver,
        LevelDone,
        Paused,
        Running,
        Starting
    }


    //public enum CharacterState
    //{
    //    Blocked,
    //    Drowning,
    //    Falling,
    //    Hit,
    //    Jumping,
    //    LongJumping,
    //    Sliding,
    //    StuckInSand,
    //    Swimming,
    //    Walking        
    //}

    public enum CharacterLiveState
    {
        Hit,
        Normal
    }

    public enum Instruction
    { 
        None,
        Contract,
        Hold,
        Release
    }

    public enum Direction
    {
        East,
        South,
        West,
        North
    }

    public enum TurningDirection
    {
        Left,
        Right,
        None
    }

    public enum RotationStatus
    {
        Completed,
        Interupted,
        Rotating,
        None
    }

    public enum RotationDirection
    {
        Left,
        Right,
        None
    }
    public enum AutomaticMovement
    {
        Blocked,
        MovingForward
    }

    public enum ActionMovement
    {
        Falling,
        Jumping,
        LongJumping,
        Standing,
        Swimming,
        Left,
        Right,
        None
    }

    public enum EnemyState
    {
        FullHealth,
        HalfHealth,
        LittleHealth
    }

    //public enum GameState
    //{
    //    GameOver,
    //    LevelDone,
    //    Paused
    //}

    public enum TileType
    {
        Crate,
        DeepLava,
        DeepWater,
        Dirt,
        Exclamation,
        Exit,
        ExitSign,
        Fence,
        Ground,
        HillLarge,
        HillSmall,
        Ice,
        InvisibleTile,
        Lava,
        //Sand,
        Water,
        Planet,
        Checkpoint,
        Star,
        Ufo,
        Debris,
        DebrisLong,
        SpaceSpiral,
        SpaceMist,
        FinishLine,
        LeftRotation,
        RightRotation
    }
}
