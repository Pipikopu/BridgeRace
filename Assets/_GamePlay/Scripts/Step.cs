using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : GameUnit
{
    public Constant.StepTags stepTag;

    public bool compareStepTag(Constant.StepTags newStepTag)
    {
        if (stepTag == newStepTag)
        {
            return true;
        }
        else return false;
    }

    public void changeStepTag(Constant.StepTags newStepTag)
    {
        stepTag = newStepTag;
    }
}
