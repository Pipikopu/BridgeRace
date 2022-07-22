using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constant
{
    public const string MASK_STEP = "Step";

    public const string ANIM_IS_RUN = "isRun";
    public const string ANIM_WIN = "Win";
    public const string ANIM_IS_FALL = "isFall";

    public const string NEW_STAGE_TAG = "NewStage";
    public const string FINISH_TAG = "Finish";
    public const string UNTAGGED_TAG = "Untagged";

    public enum BrickTags { BlueBrick, RedBrick, GreenBrick, YellowBrick };
    public enum StepTags { BlueStep, RedStep, GreenStep, YellowStep};

    public const string NORMAL_BRICK_TAG = "NormalBrick";
    public const string PLAYER_STRING = "Player";

    public const string BRIDGE_TAG = "Bridge";
    public const string GROUND_TAG = "Ground";

    public const string HORIZONTAL_AXIS = "Horizontal";
    public const string VERTICAL_AXIS = "Vertical";

    public const string CURRENT_LEVEL_STRING = "CurrentLevel";
}
