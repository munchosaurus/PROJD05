using UnityEngine;

public static class Constants
{
    // Add new constants to use here.
    // Level settings
    public const float LEVEL_LOAD_INPUT_PAUSE_TIME = 1f;
    public const float LEVEL_DEFAULT_CAMERA_LEEWAY = 1f;
    public const float LEVEL_DEFAULT_CAMERA_MOVEMENT_SMOOTHTIME = 0.3f;
    public const float LEVEL_DEFAULT_CAMERA_MINIMUM_X = 9.5f;
    public const float LEVEL_DEFAULT_CAMERA_MAXIMUM_X = 9.5f;
    public const float LEVEL_DEFAULT_CAMERA_MINIMUM_Y = 9.5f;
    public const float LEVEL_DEFAULT_CAMERA_MAXIMUM_Y = 9.5f;
    public const float LEVEL_DEFAULT_CAMERA_MAXIMUM_DISTANCE = -40f;
    public const float LEVEL_DEFAULT_CAMERA_MINIMUM_DISTANCE = -38f;
    public const float LEVEL_DEFAULT_CAMERA_TARGET_X = 0; 
    public const float LEVEL_DEFAULT_CAMERA_TARGET_Y = 0; 
    public const float LEVEL_DEFAULT_CAMERA_TARGET_Z = -38f; 
    
    
    // Add finished player input settings here:
    public const float PLAYER_JUMP_FORCE = 10f;
    public const float PLAYER_JUMP_COOLDOWN = 1f;
    public const float PLAYER_MAX_WALKING_VELOCITY = 8f;
    public const float PLAYER_MOVEMENT_ACCELERATION = 40f;
    public const float PLAYER_MOVEMENT_DECELERATION = 15f;
    public const float PLAYER_JUMP_FORCE_MULTIPLIER = 100f;
    public const float PLAYER_AIR_MOVEMENT_MULTIPLIER = 0.1f;
    public const float PLAYER_AIMING_POINT_POSITIONING_MULTIPLIER = 1f;
    public const float PLAYER_Z_VALUE = 1f;
    public const float PLAYER_AIR_SPEED_DAMPER = 0.1f;
    public const float PLAYER_AIR_MOVEMENT_WINDOW = 1f;

    // Gravity
    public const float GRAVITY = 20f;
    
    // UI
    public const float GRAVITY_ARROW_ROTATION_SPEED = 5f;
    
    // Death timer
    public const float PLAYER_DEATH_TIME = 1f;

}