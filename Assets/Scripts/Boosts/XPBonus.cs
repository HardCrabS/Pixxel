using UnityEngine;

public class XPBonus : BoostBase
{
    [SerializeField] float procentForXP = 2;
    [SerializeField] float timeForBonusLast = 10f;

    bool needToResetXPprocent = false;
    float timer;
    GridA grid;

    void Update()
    {
        if (needToResetXPprocent)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                grid.SetXPpointsPerBoxByProcent(1 / procentForXP);
                needToResetXPprocent = false;
                finished = true;
            }
        }
    }

    public override void ExecuteBonus()
    {
        grid = GridA.Instance;
        grid.SetXPpointsPerBoxByProcent(procentForXP);
        needToResetXPprocent = true;
        timer = timeForBonusLast;
    }
}